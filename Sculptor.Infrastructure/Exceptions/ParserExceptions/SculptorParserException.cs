using System;
namespace Sculptor.Infrastructure.Exceptions.ParserExceptions
{
    /// <summary>
    /// Thrown when the <see cref="CommandLine"/> library is able to parse user input, but
    /// does so with a result that we cannot process further.
    /// </summary>
    public class SculptorParserException : Exception, IFormattableException
    {
        public SculptorParserException(string message, object context) : base(message)
        {
            Context = context;
        }

        public SculptorParserException(
            string message,
            object context,
            Exception innerException) : base(message, innerException)
        {
            Context = context;
        }

        public object Context { get; set; }

        public string FormatExceptionToString()
        {
            return Message;
        }
    }
}