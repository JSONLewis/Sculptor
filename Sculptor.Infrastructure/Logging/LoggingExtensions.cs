using Microsoft.Extensions.Configuration;
using Serilog;

namespace Sculptor.Infrastructure.Logging
{
    public static class LoggingExtensions
    {
        public static ILogger AddLogging(this IConfiguration configuration)
        {
            return new LoggerConfiguration()
                .ReadFrom
                .Configuration(configuration)
                .CreateLogger();
        }
    }
}