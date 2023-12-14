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
            // Define the CircuitBreaker policy
            var circuitBreakerPolicy = Policy
                .Handle<System.Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(10),
                    onBreak: (ex, breakDelay) =>
                    {
                        Console.WriteLine($"Circuit broken! Exception: {ex.Message}. Breaking for {breakDelay.TotalSeconds} seconds.");
                    },
                    onReset: () => Console.WriteLine("Circuit reset."),
                    onHalfOpen: () => Console.WriteLine("Circuit half-open, next call is a trial.")
                );

            // Simulate calls with the CircuitBreaker policy
            while (true)
            {
                for (int i = 0; i < 5; i++)
                {
                    try
                    {
                        circuitBreakerPolicy.Execute(() =>
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