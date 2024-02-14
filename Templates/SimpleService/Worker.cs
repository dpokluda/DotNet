using Microsoft.Extensions.Options;
using SimpleService.Configuration;

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
            string logMessage = "Background worker is running";
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                logMessage += " in debug mode";
            }
            _logger.LogInformation(logMessage);
            
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
        private const string SimpleName = "Default";
        private readonly MyConfig _myConfig;
        private readonly ILogger<MyTest> _logger;

        public MyTest(IOptions<MyConfig> myConfig, ILogger<MyTest> logger)
        {
            _myConfig = myConfig.Value;
            _logger = logger;
        }

        public string Name
        {
            get
            {
                var name = _myConfig.Name ?? SimpleName;
                _logger.LogDebug($"MyTest.Name: {name}");
                return name;
            }
        }
    }
}