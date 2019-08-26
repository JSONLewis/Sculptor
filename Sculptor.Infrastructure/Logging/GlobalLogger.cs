using Serilog;

namespace Sculptor.Infrastructure.Logging
{
    public class GlobalLogger : IGlobalLogger
    {
        public GlobalLogger(ILogger logger)
        {
            Instance = logger;
        }

        public ILogger Instance { get; }
    }
}