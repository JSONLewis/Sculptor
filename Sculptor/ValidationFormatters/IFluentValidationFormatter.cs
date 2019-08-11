using System.Collections.Generic;
using FluentValidation.Results;

namespace Sculptor.ValidationFormatters
{
    internal interface IFluentValidationFormatter
    {
        IEnumerable<ValidationFailureFormat> GroupExceptionsByName(
            IEnumerable<ValidationFailure> validationFailures);
    }
}