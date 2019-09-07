using System;

namespace Sculptor.Infrastructure.Exceptions
{
    /// <summary>
    /// Provides a consistent way of tersely informing a user what went wrong, while
    /// logging the full detail out according to the <see cref="Serilog"/> config.
    /// </summary>
    public interface IExceptionHandler
    {
        void Handle<TException>(TException exception)
            where TException : Exception, IFormattableException;
    }
}