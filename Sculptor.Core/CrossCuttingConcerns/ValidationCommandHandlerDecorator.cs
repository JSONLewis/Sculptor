using FluentValidation;
using Sculptor.Core.Domain;
using Sculptor.Infrastructure.Exceptions.ValidationExceptions;

namespace Sculptor.Core
{
    /// <summary>
    /// This decorator wraps every command handler so that we can consistently run the
    /// associated validation rules for each implementation of <see cref="ICommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">
    /// The implementation of <see cref="ICommand"/> we want to validate.
    /// </typeparam>
    public sealed class ValidationCommandHandlerDecorator<TCommand>
        : ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly IValidator<TCommand> _validator;

        public ValidationCommandHandlerDecorator(
            ICommandHandler<TCommand> decorated,
            IValidator<TCommand> validator)
        {
            _decorated = decorated;
            _validator = validator;
        }

        public void Handle(TCommand command)
        {
            var context = new ValidationContext(command);
            var validationResult = _validator.Validate(context);

            // Exit out of the decorator calls and let the caller handle informing
            // the user of the found errors.
            if (validationResult.Errors.Count > 0)
                throw new SculptorValidationException("Validation failure(s) encountered", validationResult.Errors, command);

            // No errors reported so fall into the next inner command.
            _decorated.Handle(command);
        }
    }
}