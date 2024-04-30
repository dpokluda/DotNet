using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace MyTask;

/// <summary>
/// Custom implementation of Task class (based on https://www.youtube.com/watch?v=R-z2Hv-7nxk).
/// </summary>
public class MyTask
{
    private readonly object _syncRoot = new object();
    private bool _completed;
    private Exception? _exception;
    private Action? _continuation;
    private ExecutionContext? _context;

    /// <summary>
    /// A simplified custom awaiter implementation for MyTask.
    /// </summary>
    /// <param name="t">A MyTask to process.</param>
    public struct Awaiter(MyTask t) : INotifyCompletion
    {
        /// <summary>
        /// Gets the awaiter.
        /// </summary>
        /// <returns>
        /// The awaiter.
        /// </returns>
        public Awaiter GetAwaiter() => this;

        /// <summary>
        /// Gets a value indicating whether this MyTask is completed.
        /// </summary>
        /// <value>
        /// True if this MyTask is completed, false if not.
        /// </value>
        public bool IsCompleted => t.IsCompleted;

        /// <summary>
        /// Schedules the continuation action that's invoked when the instance completes.
        /// </summary>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="continuation" /> argument is null (Nothing in Visual Basic).</exception>
        /// <param name="continuation">The action to invoke when the operation completes.</param>
        /// <seealso cref="INotifyCompletion.OnCompleted(Action)"/>
        public void OnCompleted(Action continuation) => t.ContinueWith(continuation);

        /// <summary>
        /// Gets the result.
        /// </summary>
        public void GetResult() => t.Wait();
    }

    /// <summary>
    /// Gets the awaiter.
    /// </summary>
    /// <returns>
    /// The awaiter.
    /// </returns>
    public Awaiter GetAwaiter() => new(this);

    /// <summary>
    /// Gets a value indicating whether this MyTask is completed.
    /// </summary>
    /// <value>
    /// True if this MyTask is completed, false if not.
    /// </value>
    public bool IsCompleted
    {
        get
        {
            lock (_syncRoot)
            {
                return _completed;
            }
        }
    }

    /// <summary>
    /// Sets the result.
    /// </summary>
    public void SetResult()
    {
        Complete(null);
    }

    /// <summary>
    /// Sets an exception.
    /// </summary>
    /// <param name="exception">The exception.</param>
    public void SetException(Exception exception)
    {
        Complete(exception);
    }
    
    private void Complete(Exception? exception)
    {
        lock (_syncRoot)
        {
            if (_completed)
            {
                throw new InvalidOperationException("Task already completed");
            }
            
            _completed = true;
            _exception = exception;
            
            if (_continuation is not null)
            {
                MyThreadPool.QueueUserWorkItem(() =>
                {
                    if (_context is null)
                    {
                        _continuation();
                    }
                    else
                    {
                        ExecutionContext.Run(_context, (object? state) => ((Action)state!).Invoke(), _continuation);
                    }
                });
            }
        }
    }

    /// <summary>
    /// Waits this MyTask.
    /// </summary>
    public void Wait()
    {
        ManualResetEventSlim? mres = null;
        
        lock(_syncRoot)
        {
            if (!_completed)
            {
                mres = new ManualResetEventSlim();
                ContinueWith(mres.Set);
            }
        }
        
        mres?.Wait();
        
        if (_exception is not null)
        {
            ExceptionDispatchInfo.Throw(_exception);
        }
    }

    /// <summary>
    /// Continue with.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>
    /// A MyTask.
    /// </returns>
    public MyTask ContinueWith(Action action)
    {
        MyTask task = new MyTask();
        
        Action callback = () =>
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                task.SetException(e);
               return;
            }
            
            task.SetResult();
        };
        
        lock (_syncRoot)
        {
            if (_completed)
            {
                MyThreadPool.QueueUserWorkItem(callback);
            }
            else
            {
                _continuation = callback;
                _context = ExecutionContext.Capture();
            }
        }

        return task;
    }

    /// <summary>
    /// Continue with.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>
    /// A MyTask.
    /// </returns>
    public MyTask ContinueWith(Func<MyTask> action)
    {
        MyTask t = new();

        Action callback = () =>
        {
            try
            {
                MyTask next = action();
                next.ContinueWith(delegate
                {
                    if (next._exception is not null)
                    {
                        t.SetException(next._exception);
                    }
                    else
                    {
                        t.SetResult();
                    }
                });
            }
            catch (Exception e)
            {
                t.SetException(e);
                return;
            }
        };

        lock (this)
        {
            if (_completed)
            {
                MyThreadPool.QueueUserWorkItem(callback);
            }
            else
            {
                _continuation = callback;
                _context = ExecutionContext.Capture();
            }
        }

        return t;
    }

    /// <summary>
    /// Runs the given action.
    /// </summary>
    /// <param name="action">The action.</param>
    /// <returns>
    /// A MyTask.
    /// </returns>
    public static MyTask Run(Action action)
    {
        MyTask task = new MyTask();
        MyThreadPool.QueueUserWorkItem(() =>
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                task.SetException(e);
                return;
            }

            task.SetResult();
        });
        
        return task;
    }

    /// <summary>
    /// When all.
    /// </summary>
    /// <param name="tasks">The tasks.</param>
    /// <returns>
    /// A MyTask.
    /// </returns>
    public static MyTask WhenAll(List<MyTask> tasks)
    {
        MyTask task = new MyTask();
        if (tasks.Count == 0)
        {
            task.SetResult();
        }
        else
        {
            int remainingTasks = tasks.Count;
        
            Action continuation = () =>
            {
                if (Interlocked.Decrement(ref remainingTasks) == 0)
                {
                    // TODO: exceptions
                    task.SetResult();
                }
            };
            
            foreach (var t in tasks)
            {
                t.ContinueWith(continuation);
            }

        }
        
        return task;
    }

    /// <summary>
    /// When any.
    /// </summary>
    /// <param name="tasks">The tasks.</param>
    /// <returns>
    /// A MyTask.
    /// </returns>
    public static MyTask WhenAny(List<MyTask> tasks)
    {
        MyTask task = new MyTask();
        foreach (var t in tasks)
        {
            t.ContinueWith(() =>
            {
                if (t._exception is not null)
                {
                    task.SetException(t._exception);
                }
                else
                {
                    task.SetResult();
                }
            });
        }
        
        return task;
    }

    /// <summary>
    /// Delays.
    /// </summary>
    /// <param name="milliseconds">The milliseconds.</param>
    /// <returns>
    /// A MyTask.
    /// </returns>
    public static MyTask Delay(int milliseconds)
    {
        MyTask task = new MyTask();
        Timer timer = new Timer(_ =>
        {
            task.SetResult();
        }, null, milliseconds, Timeout.Infinite);
        
        return task;
    }

    /// <summary>
    /// Iterates the given tasks.
    /// </summary>
    /// <param name="tasks">The tasks.</param>
    /// <returns>
    /// A MyTask.
    /// </returns>
    public static MyTask Iterate(IEnumerable<MyTask> tasks)
    {
        MyTask task = new MyTask();

        IEnumerator<MyTask> enumerator = tasks.GetEnumerator(); // TODO: dispose

        void MoveNext()
        {
            try
            {
                while (enumerator.MoveNext())
                {
                    MyTask next = enumerator.Current;
                    if (next.IsCompleted)
                    {
                        next.Wait();
                        continue;
                    }

                    next.ContinueWith(MoveNext);
                    return;
                }
            }
            catch (Exception ex)
            {
                task.SetException(ex);
                return;
            }

            task.SetResult();
        }

        MoveNext();

        return task;
    }
}