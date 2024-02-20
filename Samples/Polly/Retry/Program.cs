using Polly;
using Polly.Retry;
using SimpleConsole.Exceptions;

namespace SimpleConsole
{
    internal class Program
    {
        private static int Counter = 0;

        static void Main(string[] args)
        {
            // Define the retry strategy: retry 3 times with a 2-second delay between retries
            var retryOptions = new RetryStrategyOptions()
            {
                ShouldHandle = new PredicateBuilder()
                    .Handle<MyException1>()
                    .Handle<MyException2>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(2),
                OnRetry = args =>
                {
                    Console.WriteLine($"Attempt {args.AttemptNumber}: Retrying in {args.RetryDelay.TotalSeconds}  seconds due to {args.Outcome.Exception.Message}");
                    return ValueTask.CompletedTask;
                }
            };
            var retryPolicy = new ResiliencePipelineBuilder()
                .AddRetry(retryOptions)
                .Build();
            
            try
            {
                // Execute the operation within the retry policy
                retryPolicy.Execute(() =>
                {
                    // Operation that may fail, e.g., calling an external service
                    PerformOperation();
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Operation failed after retries. Exception: {ex.Message}");
            }
        }

        static void PerformOperation()
        {
            Counter++;
            // Example operation that might fail
            Console.WriteLine($"Performing operation with counter of {Counter}...");
            if (Counter % 2 == 1)
            {
                throw new MyException1("MyException1");
            }
            else
            {
                throw new MyException2("MyException2");
            }
        }
    }
}