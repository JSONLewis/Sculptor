using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Results;

namespace Sculptor.Infrastructure.Exceptions.ValidationExceptions
{
    /// <summary>
    /// Thrown whenever a <see cref="ValidationException"/> is raised. We do this in order
    /// to wrap the internal <see cref="FluentValidation"/> failures with the current
    /// context and the necessary formatting to expose only the necessary messages to the
    /// user's terminal.
    /// </summary>
    public sealed class SculptorValidationException : ValidationException, IFormattableException
    {
        public SculptorValidationException(string message, IEnumerable<ValidationFailure> errors) : base(message, errors)
        {
        }

        public SculptorValidationException(string message, IEnumerable<ValidationFailure> errors, object context) : base(message, errors)
        {
            Context = context;
        }

        public object Context { get; }

        public string FormatExceptionToString()
        {
            var failures = from x in Errors
                           group x by x.PropertyName into g
                           select new ValidationFailureFormat
                           {
                               PropertyName = g.Key,
                               AttemptedValue = g.FirstOrDefault()?.AttemptedValue,
                               PrimaryErrorMessage =
                                    g.FirstOrDefault()?.ErrorMessage ?? string.Empty,
                               ErrorMessages = from errors in g
                                               select errors.ErrorMessage
                           };

            return failures.FormatPrimaryErrorMessages();
        }
    }
}