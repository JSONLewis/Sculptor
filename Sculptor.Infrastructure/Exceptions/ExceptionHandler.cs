using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Infrastructure.Logging;
using Sculptor.Infrastructure.OutputFormatters;
using System;

namespace Sculptor.Infrastructure.Exceptions
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly IOutputFormatter _outputFormatter;
        private readonly ITerminal _terminal;
        private readonly IGlobalLogger _logger;

        public ExceptionHandler(
            IOutputFormatter outputFormatter,
            ITerminal terminal,
            IGlobalLogger logger)
        {
            _outputFormatter = outputFormatter;
            _terminal = terminal;
            _logger = logger;
        }

        public void Handle<TException>(TException exception)
            where TException : Exception, IFormattableException
        {
            string errorMessage = exception.FormatExceptionToString();
            string template = _outputFormatter.FormatMessageForOutput(errorMessage);

            _terminal.RenderText(template);
            _logger.Instance.Fatal(exception, errorMessage);
        }
    }
}