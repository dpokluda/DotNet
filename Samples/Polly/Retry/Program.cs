using Polly;
using SimpleConsole.Exceptions;

namespace SimpleConsole
{
    internal class Program
    {
        private static int Counter = 0;

        static void Main(string[] args)
        {
            // Define the retry policy: retry 3 times with a 2-second delay between retries
            var retryPolicy = Policy
                .Handle<MyException1>()
                .Or<MyException2>()
                .WaitAndRetry(3, retryAttempt => TimeSpan.FromSeconds(2),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine($"Attempt {retryCount}: Retrying in {timeSpan.Seconds} seconds due to {exception.Message}");
                    });

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