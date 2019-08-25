using System;
using Sculptor.Infrastructure;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Infrastructure.Exceptions;
using Sculptor.Infrastructure.Exceptions.ParserExceptions;
using Sculptor.Infrastructure.Exceptions.ValidationExceptions;
using Sculptor.Parsing;

namespace Sculptor
{
    public sealed class Application : IApplication
    {
        private readonly ICommandParser _commandParser;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IExceptionHandler _exceptionHandler;

        public Application(
            ICommandParser commandParser,
            ICommandProcessor commandProcessor,
            IExceptionHandler exceptionHandler)
        {
            _commandParser = commandParser;
            _commandProcessor = commandProcessor;
            _exceptionHandler = exceptionHandler;
        }

        public void Run(IUserInput userInput)
        {
            try
            {
                var parserResult = _commandParser.Parse(userInput);

                if (parserResult is null == false)
                {
                    _commandProcessor.Process(parserResult);
                }
            }
            catch (Exception e) when (e is IFormattableException)
            {
                // NOTE: it would be far better not to have to switch on the type of
                // exception, but the compiler is not smart enough (even when we check
                // explicitly) to not require some form of casting.
                switch (e)
                {
                    case SculptorParserException ex:
                        _exceptionHandler.Handle(ex);
                        break;
                    case SculptorValidationException ex:
                        _exceptionHandler.Handle(ex);
                        break;
                    default:
                        throw new InvalidOperationException($"[{nameof(Application)}.{nameof(Application.Run)}] encountered a custom {nameof(IFormattableException)} but it was not handled", e);
                }
            }
        }
    }
}