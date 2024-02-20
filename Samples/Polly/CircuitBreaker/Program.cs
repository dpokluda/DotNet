using Polly;
using Polly.CircuitBreaker;
using SimpleConsole.Exception;

namespace SimpleConsole
{
    internal class Program
    {
        private static int Counter = 0;

        static async Task Main(string[] args)
        {
            // Define the CircuitBreaker strategy
            var circuitBreakerOptions = new CircuitBreakerStrategyOptions()
            {
                FailureRatio = 0.5,
                MinimumThroughput = 4,
                BreakDuration = TimeSpan.FromSeconds(10),
                ShouldHandle = new PredicateBuilder().Handle<System.Exception>(),
                OnOpened = args =>
                {
                    Console.WriteLine($"Circuit opened! Exception: {args.Outcome.Exception.Message}. Breaking for {args.BreakDuration.TotalSeconds} seconds.");
                    return ValueTask.CompletedTask;
                },
                OnClosed = args =>
                {
                    Console.WriteLine("Circuit closed.");
                    return ValueTask.CompletedTask;
                },
                OnHalfOpened = args =>
                {
                    Console.WriteLine("Circuit half-open.");
                    return ValueTask.CompletedTask;
                },
            };
            var circuitBreaker = new ResiliencePipelineBuilder()
                .AddCircuitBreaker(circuitBreakerOptions)
                .Build();
            
            // Simulate calls with the CircuitBreaker
            while (true)
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        circuitBreaker.Execute(() =>
                        {
                            Console.WriteLine("Attempting action...");
                            PerformOperation(i);
                        });
                    }
                    catch (MyException ex)
                    {
                        Console.WriteLine($"Action failed with exception: {ex.Message}");
                    }
                    catch (BrokenCircuitException ex)
                    {
                        Console.WriteLine($"Circuit breaker exception: {ex.Message}");
                    }
                }
                
                Console.Write("Waiting...");
                await Task.Delay(5000);
                Console.WriteLine();
            }
        }

        static void PerformOperation(int iteration)
        {
            // Simulate success or failure
            if (iteration % 2 == 0)
                throw new MyException("Simulated failure");
            else
                Console.WriteLine("Action succeeded.");
        }
    }
}