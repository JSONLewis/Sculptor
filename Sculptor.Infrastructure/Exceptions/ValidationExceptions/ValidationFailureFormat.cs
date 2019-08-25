using System.Collections.Generic;

namespace Sculptor.Infrastructure.Exceptions.ValidationExceptions
{
    /// <summary>
    /// Provides the specific format we want the consumer to depend on when iterating
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
        /// As there may be more than one reason for a command failing we record all
        /// errors returned for further processing. Currently we only display the
        /// <see cref="PrimaryErrorMessage"/>.
        /// </summary>
        public IEnumerable<string> ErrorMessages { get; set; }
    }
}