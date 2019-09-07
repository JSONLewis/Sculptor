using Sculptor.Core.Domain;
using Sculptor.Infrastructure.Exceptions;
using Sculptor.Infrastructure.Exceptions.ParserExceptions;
using Sculptor.Infrastructure.Exceptions.ValidationExceptions;
using System;

namespace Sculptor.Core.CrossCuttingConcerns
{
    public class ExceptionCommandHandlerDecorator<TCommand>
        : ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly IExceptionHandler _exceptionHandler;

        public ExceptionCommandHandlerDecorator(
            ICommandHandler<TCommand> decorated,
            IExceptionHandler exceptionHandler)
        {
            _decorated = decorated;
            _exceptionHandler = exceptionHandler;
        }

        public void Handle(TCommand command)
        {
            try
            {
                _decorated.Handle(command);
            }
            catch (Exception ex) when (ex is IFormattableException)
            {
                switch (ex)
                {
                    case SculptorParserException exp:
                        _exceptionHandler.Handle(exp);
                        break;

                    case SculptorValidationException exv:
                        _exceptionHandler.Handle(exv);
                        break;

                    default:
                        throw new InvalidOperationException($"[{nameof(ExceptionCommandHandlerDecorator<TCommand>)}.{nameof(ExceptionCommandHandlerDecorator<TCommand>.Handle)}] encountered a custom {nameof(IFormattableException)} but it was not handled", ex);
                }
            }
        }
    }
}