using System.Collections.Concurrent;
using System.Threading.Channels;
using BenchmarkDotNet.Attributes;

namespace ParallelProcessing;

[MemoryDiagnoser]
public class ParallelProcessors
{
    private static IEnumerable<MyData> Data = Enumerable.Range(0, 250).Select(i => new MyData()
    {
        Name = $"name: {i}"
    });
    
    [Benchmark]
    public void Option1_BlockingCollection()
    {
        var output = new List<MyData>(100);
        foreach (var o in Option1_BlockingCollection_Translate(Data))
        {
            output.Add(o);
        }
    }
    
    private IEnumerable<MyData> Option1_BlockingCollection_Translate(IEnumerable<MyData> rows)
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
    public void Option2_ParallelForBlockingCollection()
    {
        var output = new List<MyData>(100);
        foreach (var o in Option2_ParallelForBlockingCollection_Translate(Data))
        {
            output.Add(o);
        }
    }
    
    private IEnumerable<MyData> Option2_ParallelForBlockingCollection_Translate(IEnumerable<MyData> rows)
    {
        var blockingCollection = new BlockingCollection<MyData>();

        Parallel.ForEach(rows, row =>
        {
            var result = Process(row);
            if (result is not null)
            {
                blockingCollection.Add(result);
            }
        });

        // Mark the BlockingCollection as complete for adding.
        blockingCollection.CompleteAdding();

        foreach (var item in blockingCollection.GetConsumingEnumerable())
        {
            yield return item;
        }
    }

    [Benchmark]
    public void Option3_Channel()
    {
        var output = new List<MyData>(100);
        foreach (var o in Option3_Channel_Translate(Data))
        {
            output.Add(o);
        }
    }    
    private IEnumerable<MyData> Option3_Channel_Translate(IEnumerable<MyData> rows)
    {
        var channel = Channel.CreateUnbounded<MyData>();
        var completionTask = Option3_Channel_ProcessRowsAsync(rows, channel);

        foreach (var item in Option3_Channel_ReadAll(channel.Reader, completionTask))
        {
            yield return item;
        }

        // Ensure all rows are processed before completing the method.
        completionTask.Wait();
    }

    private async Task Option3_Channel_ProcessRowsAsync(IEnumerable<MyData> rows, Channel<MyData> channel)
    {
        var tasks = new List<Task>();

        foreach (var row in rows)
        {
            tasks.Add(Task.Run(async () =>
            {
                var result = Process(row);
                if (result is not null)
                {
                    await channel.Writer.WriteAsync(result);
                }
            }));
        }

        await Task.WhenAll(tasks);
        channel.Writer.Complete();
    }

    private IEnumerable<MyData> Option3_Channel_ReadAll(ChannelReader<MyData> reader, Task completionTask)
    {
        while (true)
        {
            MyData item;
            while (reader.TryRead(out item))
            {
                yield return item;
            }

            if (completionTask.IsCompleted && reader.Count == 0)
            {
                yield break;
            }

            // Introduce a small delay to avoid busy-waiting
            Task.Delay(10).Wait();
        }
    }

    [Benchmark]
    public void Option4_WhenAny()
    {
        var output = new List<MyData>(100);
        foreach (var o in Option4_WhenAny_Translate(Data))
        {
            output.Add(o);
        }
    }    
    
    private IEnumerable<MyData> Option4_WhenAny_Translate(IEnumerable<MyData> rows)
    {
        var tasks = rows.Select(row => Task.Run(() => Process(row))).ToList();
        var completedTasks = new HashSet<Task<MyData>>();

        while (tasks.Count > 0)
        {
            var completedTask = Task.WhenAny(tasks).Result;
            tasks.Remove(completedTask);
            completedTasks.Add(completedTask);

            if (completedTask.Result != null)
            {
                yield return completedTask.Result;
            }
        }
    }
    
    [Benchmark]
    public void Option5_WhenAnyHashset()
    {
        var output = new List<MyData>(100);
        foreach (var o in Option5_WhenAnyHashset_Translate(Data))
        {
            output.Add(o);
        }
    }    
    
    private IEnumerable<MyData> Option5_WhenAnyHashset_Translate(IEnumerable<MyData> rows)
    {
        var tasks = rows.Select(row => Task.Run(() =>
        {
            return Process(row);
        })).ToHashSet();

        while (tasks.Count > 0)
        {
            var completedTask = Task.WhenAny(tasks).Result;
            tasks.Remove(completedTask);

            if (completedTask.Result != null)
            {
                yield return completedTask.Result;
            }
        }
    }
 
    private MyData Process(MyData data)
    {
        Task.Delay(100).GetAwaiter().GetResult();
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