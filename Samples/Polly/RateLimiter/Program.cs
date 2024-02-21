using System.Threading.RateLimiting;
using Polly;
using Polly.RateLimiting;

namespace RateLimiter;

class Program
{
    /// <summary>
    /// For details see https://devblogs.microsoft.com/dotnet/announcing-rate-limiting-for-dotnet/
    /// </summary>
    static async Task Main(string[] args)
    {
        Console.WriteLine("Concurrency Limiter:");
        await RunAsync(CreateConcurrencyLimiter());

        Console.WriteLine("\nSliding Window Rate Limiter:");
        await RunAsync(CreateSlidingWindowRateLimiter());
    }

    private static async Task RunAsync(ResiliencePipeline rateLimiter)
    {
        var results = new List<Task>(10);
        for (var i = 1; i <= 10; i++)
        {
            results.Add(StartParallelTask(i, rateLimiter));
        }
        await Task.WhenAll(results);
    }

    private static ResiliencePipeline CreateConcurrencyLimiter()
    {
        var options = new ConcurrencyLimiterOptions()
        {
            PermitLimit = 3,
            QueueLimit = 0,
        };
        
        return new ResiliencePipelineBuilder()
            .AddRateLimiter(new ConcurrencyLimiter(options))
            .Build();
    }

    private static ResiliencePipeline CreateSlidingWindowRateLimiter()
    {
        var options = new SlidingWindowRateLimiterOptions()
        {
            PermitLimit = 3,
            Window = TimeSpan.FromSeconds(1),
            SegmentsPerWindow = 1,
        };
        
        return new ResiliencePipelineBuilder()
            .AddRateLimiter(new SlidingWindowRateLimiter(options))
            .Build();
    }

    private static async Task StartParallelTask(int i, ResiliencePipeline rateLimiter)
    {
        await Task.Delay((i-1) * 250);
        try
        {
            await rateLimiter.ExecuteAsync(async (ct) => await RunRateLimitedOperation(i));
        }
        catch (RateLimiterRejectedException e)
        {
            Console.WriteLine($"Operation {i} was rejected by rate limiter");
        }
    } 
    
    private static async Task RunRateLimitedOperation(int i)
    {
        Console.WriteLine($"Running operation {i}...");
        await Task.Delay(1000);
    }
}