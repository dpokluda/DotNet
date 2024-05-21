using System.Collections.Concurrent;
using System.Threading.Channels;
using BenchmarkDotNet.Attributes;

namespace ParallelProcessing;

[MemoryDiagnoser]
public class ParallelProcessors
{
    private const int Size = 200;
    private static IEnumerable<MyData> Data = Enumerable.Range(0, Size).Select(i => new MyData()
    {
        Name = $"name: {i}"
    });
    
    [Benchmark]
    public void Option1_ConcurrentQueue()
    {
        var output = new List<MyData>(Size);
        foreach (var o in Option1_ConcurrentQueue_Translate(Data))
        {
            output.Add(o);
        }
    }
    
    private IEnumerable<MyData> Option1_ConcurrentQueue_Translate(IEnumerable<MyData> rows)
    {
        var taskQueue = new ConcurrentQueue<Task<MyData>>();
        Parallel.ForEach(rows, row =>
        {
            Task<MyData> processingTask = ProcessAsync(row);
            taskQueue.Enqueue(processingTask);
        });

        while (!taskQueue.IsEmpty)
        {
            Task<MyData> task;
            while (taskQueue.TryDequeue(out task))
            {
                if (task.IsCompleted)
                {
                    MyData result = task.Result; // Synchronously get the result
                    if (result != null)
                    {
                        yield return result;
                    }
                }
                else
                {
                    // If the task is not completed, re-enqueue it for later processing
                    taskQueue.Enqueue(task);
                    Task.Delay(10).Wait(); // Introduce a small delay to reduce tight looping
                }
            }
        }
    }
    
    [Benchmark]
    public void Option2_ConcurrentQueue()
    {
        var output = new List<MyData>(Size);
        foreach (var o in Option2_ConcurrentQueue_Translate(Data))
        {
            output.Add(o);
        }
    }
    
    private IEnumerable<MyData> Option2_ConcurrentQueue_Translate(IEnumerable<MyData> rows)
    {
        var taskQueue = new ConcurrentQueue<Task<MyData>>();
        Parallel.ForEach(rows, row =>
        {
            Task<MyData> processingTask = ProcessAsync(row);
            taskQueue.Enqueue(processingTask);
        });

        while (!taskQueue.IsEmpty)
        {
            Task<MyData> task;
            while (taskQueue.TryDequeue(out task))
            {
                if (task.IsCompleted)
                {
                    MyData result = task.Result; // Synchronously get the result
                    if (result != null)
                    {
                        yield return result;
                    }
                }
                else
                {
                    // If the task is not completed, re-enqueue it for later processing
                    taskQueue.Enqueue(task);
                    Task.Delay(10).Wait(); // Introduce a small delay to reduce tight looping
                }
            }
        }
    }
    
    [Benchmark]
    public void Option3_BlockingCollectionContinueWith()
    {
        var output = new List<MyData>(Size);
        foreach (var o in Option3_BlockingCollectionContinueWith_Translate(Data))
        {
            output.Add(o);
        }
    }
    
    private IEnumerable<MyData> Option3_BlockingCollectionContinueWith_Translate(IEnumerable<MyData> rows)
    {
        var blockingCollection = new BlockingCollection<MyData>();
        var tasks = new List<Task>();

        foreach (var row in rows)
        {
            var task = Task.Run(() =>
            {
                var result = Process(row);
                if (result is not null)
                {
                    blockingCollection.Add(result);
                }
            });
            tasks.Add(task);
        }

        var completionTask = Task.WhenAll(tasks).ContinueWith(_ => blockingCollection.CompleteAdding());

        foreach (var item in blockingCollection.GetConsumingEnumerable())
        {
            yield return item;
        }

        completionTask.Wait();
    }
    
    [Benchmark]
    public void Option4_BlockingCollectionAwait()
    {
        var output = new List<MyData>(Size);
        foreach (var o in Option4_BlockingCollectionAwait_Translate(Data))
        {
            output.Add(o);
        }
    }
    
    private IEnumerable<MyData> Option4_BlockingCollectionAwait_Translate(IEnumerable<MyData> rows)
    {
        var blockingCollection = new BlockingCollection<MyData>();
        var tasks = new List<Task>();

        Task enqueueTask = Task.Run(async () =>
        {
            foreach (var row in rows)
            {
                var addToCollectionTask = Task.Run(async () =>
                {
                    var result = await ProcessAsync(row);
                    if (result != null)
                    {
                        blockingCollection.Add(result);
                    }
                });
                tasks.Add(addToCollectionTask);
            }
        
            await Task.WhenAll(tasks);
            blockingCollection.CompleteAdding();
        });

        foreach (var item in blockingCollection.GetConsumingEnumerable())
        {
            yield return item;
        }
        
        enqueueTask.Wait();
    }
    

    private MyData Process(MyData data)
    {
        Task.Delay(Size).GetAwaiter().GetResult();
        return new MyData
        {
            Name = data.Name
        };
    }    
    
    private async Task<MyData> ProcessAsync(MyData data)
    {
        await Task.Delay(Size);
        return new MyData
        {
            Name = data.Name
        };
    }

    public class MyData
    {
        public string Name { get; set; }
    }
}