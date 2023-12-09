using Polly;
using SimpleConsole.Exception;

namespace SimpleConsole
{
    internal class Program
    {
        private static int Counter = 0;

        static void Main(string[] args)
        {
            // Define the CircuitBreaker policy
            var circuitBreakerPolicy = Policy
                .Handle<System.Exception>()
                .CircuitBreaker(
                    exceptionsAllowedBeforeBreaking: 2,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (ex, breakDelay) =>
                    {
                        Console.WriteLine($"Circuit broken! Exception: {ex.Message}. Breaking for {breakDelay.TotalSeconds} seconds.");
                    },
                    onReset: () => Console.WriteLine("Circuit reset."),
                    onHalfOpen: () => Console.WriteLine("Circuit half-open, next call is a trial.")
                );

            // Simulate calls with the CircuitBreaker policy
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