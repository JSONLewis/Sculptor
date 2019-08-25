using System;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Infrastructure.OutputFormatters;
using Serilog;

namespace Sculptor.Infrastructure.Exceptions
{
    public class ExceptionHandler : IExceptionHandler
    {
        private readonly IOutputFormatter _outputFormatter;
        private readonly ITerminal _terminal;
        private readonly ILogger _logger;

        public ExceptionHandler(
            IOutputFormatter outputFormatter,
            ITerminal terminal,
            ILogger logger)
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
            _logger.Error(exception, errorMessage);
        }
    }
}