using Serilog;

namespace Sculptor.Infrastructure.Logging
{
    public class LocalLogger : ILocalLogger
    {
        public LocalLogger(ILogger logger)
        {
            Instance = logger;
        }

        public ILogger Instance { get; }
    }
}