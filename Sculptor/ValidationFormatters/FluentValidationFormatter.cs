using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace Sculptor.ValidationFormatters
{
    /// <summary>
    /// Provides a standardised way to inform the user of their invalid input and the
    /// error messages related to it.
    /// </summary>
    internal sealed class FluentValidationFormatter : IFluentValidationFormatter
    {
        /// <summary>
        /// Format all the errors into <see cref="ValidationFailureFormat"/> so that we
        /// return a consistent format of only the data the client needs.
        /// </summary>
        /// <param name="validationFailures"></param>
        /// <returns></returns>
        public IEnumerable<ValidationFailureFormat> GroupExceptionsByName(
            IEnumerable<ValidationFailure> validationFailures)
        {
            return from x in validationFailures
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
        }
    }

    /// <summary>
    /// Provides the specific format we want the client to depend on when iterating
    /// through each validation failure.
    /// </summary>
    internal sealed class ValidationFailureFormat
    {
        public string PropertyName { get; set; }

        public object AttemptedValue { get; set; }

        /// <summary>
        /// The main (usually first) error returned for a failed command.
        /// </summary>
        public string PrimaryErrorMessage { get; set; }

        /// <summary>
        /// There may be more than one reason for failure. Record all returned errors
        /// in case we need to process them further.
        /// </summary>
        public IEnumerable<string> ErrorMessages { get; set; }
    }
}