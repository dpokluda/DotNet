

using System.Threading.RateLimiting;
using Polly;
using Polly.RateLimiting;

namespace RateLimiter;

class Program
{
    static async Task Main(string[] args)
    {
        // Define the RateLimiter strategy
        var rateLimiterOptions = new SlidingWindowRateLimiterOptions()
        {
            PermitLimit = 5,
            Window = TimeSpan.FromSeconds(2),
            SegmentsPerWindow = 1,
        };
        var rateLimiter = new ResiliencePipelineBuilder()
            .AddRateLimiter(new SlidingWindowRateLimiter(rateLimiterOptions))
            .Build();
        
        // Simulate calls with the RateLimiter
        var results = new List<ValueTask<int>>(10);
        for (int i = 1; i <= 10; i++)
        {
            await Task.Delay(250);
            results.Add(rateLimiter.ExecuteAsync(async (ct) =>
            { 
                return await PerformOperation(i);
            }));
        }
        
        await Task.Delay(1000);
        int counter = 0;
        foreach (ValueTask<int> valueTask in results)
        {
            counter++;
            if (valueTask.IsFaulted)
            {
                Console.WriteLine($"Operation {counter} failed to execute");
            }
        }
    }

    private static async Task<int> PerformOperation(int i)
    {
        Console.WriteLine($"Performing operation {i}...");
        await Task.Delay(1000);
        return i;
    }
}