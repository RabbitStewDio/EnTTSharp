using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Serilog;

namespace EnTTSharp.Test
{
    [SetUpFixture]
    public class SetupLoggingFixture
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            var configuration = new ConfigurationBuilder()
                                .AddJsonFile("appsettings.json")
                                .Build();

            var logger = new LoggerConfiguration()
                         .ReadFrom.Configuration(configuration)
                         .CreateLogger();
            Log.Logger = logger;
        }

        [OneTimeTearDown]
        public void RunAfterAnyTests()
        {
            // ...
        }
    }
}