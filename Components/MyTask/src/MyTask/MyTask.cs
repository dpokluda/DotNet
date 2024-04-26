using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace MyTask;

public class MyTask
{
    private object _syncRoot = new object();
    private bool _completed;
    private Exception? _exception;
    private Action? _continuation;
    private ExecutionContext? _context;

    public struct Awaiter(MyTask t) : INotifyCompletion
    {
        public Awaiter GetAwaiter() => this;
        public bool IsCompleted => t.IsCompleted;
        public void OnCompleted(Action continuation) => t.ContinueWith(continuation);
        public void GetResult() => t.Wait();
    }

    public Awaiter GetAwaiter() => new(this);

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

    public void SetResult()
    {
        Complete(null);
    }
    
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
    
    public static MyTask Delay(int milliseconds)
    {
        MyTask task = new MyTask();
        Timer timer = new Timer(_ =>
        {
            task.SetResult();
        }, null, milliseconds, Timeout.Infinite);
        
        return task;
    }


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