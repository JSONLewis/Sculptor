using FluentValidation.Results;
using System.Collections.Generic;

namespace Sculptor.ValidationFormatters
{
    internal interface IFluentValidationFormatter
    {
        IEnumerable<ValidationFailureFormat> GroupExceptionsByName(
            IEnumerable<ValidationFailure> validationFailures);
    }
}