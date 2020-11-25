using Serilog;
using Xunit.Abstractions;

namespace RfmOta.UnitTests
{
    public abstract class BaseTests
    {
        protected ILogger Logger;

        public BaseTests(ITestOutputHelper output)
        {
            Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.TestOutput(output, Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger();
        }
    }
}
