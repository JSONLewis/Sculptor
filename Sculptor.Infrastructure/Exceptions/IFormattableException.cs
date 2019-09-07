namespace Sculptor.Infrastructure.Exceptions
{
    /// <summary>
    /// Marker interface used for identifying which custom exceptions can be handled by
    /// <see cref="IExceptionHandler"/>.
    /// </summary>
    public interface IFormattableException
    {
        object Context { get; }

        string FormatExceptionToString();
    }
}