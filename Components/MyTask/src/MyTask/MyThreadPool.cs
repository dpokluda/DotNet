using System.Collections.Concurrent;

namespace MyTask;

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

    public static void QueueUserWorkItem(Action action)
    {
        s_workItems.Add((action, ExecutionContext.Capture()));
    }
}