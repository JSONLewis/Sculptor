using Serilog;

namespace Sculptor.Infrastructure.Logging
{
    public interface IGlobalLogger
    {
        ILogger Instance { get; }
    }
}