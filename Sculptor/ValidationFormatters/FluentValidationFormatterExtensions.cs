using System;
using System.Collections.Generic;
using System.Text;

namespace Sculptor.ValidationFormatters
{
    internal static class FluentValidationFormatterExtensions
    {
        /// <summary>
        /// Processes only the error marked as `Primary` for each item in
        /// <paramref name="validationFailureFormats"/> so that we have the most
        /// meaningful, terse response to the user.
        /// </summary>
        /// <param name="validationFailureFormats"></param>
        /// <returns></returns>
        internal static string FormatPrimaryErrorMessages(
            this IEnumerable<ValidationFailureFormat> validationFailureFormats)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(Environment.NewLine);
            stringBuilder.Append("ERROR(S):");

            foreach (var failureFormat in validationFailureFormats)
            {
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("  Provided value: `");
                stringBuilder.Append(failureFormat.AttemptedValue);
                stringBuilder.Append("` was invalid for this option.");
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("  ");
                stringBuilder.Append(failureFormat.PrimaryErrorMessage);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Processes all errors contained in <paramref name="validationFailureFormats"/>
        /// so that we can have a single string value representing the complete reason
        /// for failure.
        /// </summary>
        /// <param name="validationFailureFormats"></param>
        /// <returns></returns>
        internal static string FormatAllErrorMessages(
            this IEnumerable<ValidationFailureFormat> validationFailureFormats)
        {
            var stringBuilder = new StringBuilder();

            foreach (var failureFormat in validationFailureFormats)
            {
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append("  Provided value: `");
                stringBuilder.Append(failureFormat.AttemptedValue);
                stringBuilder.Append("` was invalid for this option.");
                stringBuilder.Append(Environment.NewLine);

                foreach (string errorMessage in failureFormat.ErrorMessages)
                {
                    stringBuilder.Append(Environment.NewLine);
                    stringBuilder.Append("  * ");
                    stringBuilder.Append(errorMessage);
                }
            }

            return stringBuilder.ToString();
        }
    }
}