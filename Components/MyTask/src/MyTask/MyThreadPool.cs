using System.Collections.Concurrent;

namespace MyTask;

/// <summary>
/// Custom implementation of ThreadPool class (based on https://www.youtube.com/watch?v=R-z2Hv-7nxk).
/// </summary>
public static class MyThreadPool
{
    // ExecutionContent is pretty much just a dictionary of key value pairs stored in a thread local storage with some APIs to manipulate it.
    private static readonly BlockingCollection<(Action, ExecutionContext?)> s_workItems = new ();

    static MyThreadPool()
    {
        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            new Thread(() =>
            {
                while (true)
                {
                    (Action workItem, ExecutionContext? context) = s_workItems.Take();
                    if (context is null)
                    {
                        workItem();
                    }
                    else
                    {
                        // ExecutionContext.Run(context, _ => workItem(), null);
                        ExecutionContext.Run(context, (object? state) => ((Action)state!)(), workItem);
                    }
                }
            })
            {
                IsBackground = true
            }.Start();
        }
    }

    /// <summary>
    /// Queues a method for execution. The method executes when a thread pool thread becomes available.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public static void QueueUserWorkItem(Action action)
    {
        s_workItems.Add((action, ExecutionContext.Capture()));
    }
}