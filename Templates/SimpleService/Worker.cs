namespace SimpleService
{
    public class Worker : BackgroundService
    {
        private readonly IProgramArguments _programArguments;
        private readonly ITest _test;
        private readonly ILogger<Worker> _logger;

        public Worker(IProgramArguments programArguments, ITest test, ILogger<Worker> logger)
        {
            _programArguments = programArguments;
            _test = test;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                // Console output will not be visible when running in docker
                Console.WriteLine($"Name: {_test.Name}");
                _logger.LogInformation("Name: {name} (running at: {time})", _test.Name, DateTimeOffset.Now);

                await Task.Delay(TimeSpan.FromSeconds(_programArguments.IntervalInSeconds), stoppingToken);
            }
        }
    }

    public interface ITest
    {
        string Name { get; }
    }

    public class MyTest : ITest
    {
        private const string SimpleName = "David Pokluda";
        private readonly ILogger<MyTest> _logger;

        public MyTest(ILogger<MyTest> logger)
        {
            _logger = logger;
        }

        public string Name
        {
            get
            {
                _logger.LogDebug($"MyTest.Name: {SimpleName}");
                return SimpleName;
            }
        }
    }
}