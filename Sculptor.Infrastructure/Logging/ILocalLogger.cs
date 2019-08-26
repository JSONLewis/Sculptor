using Serilog;

namespace Sculptor.Infrastructure.Logging
{
    public interface ILocalLogger
    {
        ILogger Instance { get; }
    }
}