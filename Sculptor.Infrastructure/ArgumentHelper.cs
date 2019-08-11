using System;
namespace Sculptor.Infrastructure
{
    public static class ArgumentHelper
    {
        /// <summary>
        /// For the purposes of tests and debugging this provides an easy way to convert
        /// the string representation of commands to an array. This allows us to simulate
        /// entering the "same" commands as the end user when running the application.
        /// </summary>
        /// <param name="rawInput"></param>
        /// <returns></returns>
        public static string[] SplitArguments(string rawInput)
        {
            var parmChars = rawInput.ToCharArray();
            var inSingleQuote = false;
            var inDoubleQuote = false;

            for (var index = 0; index < parmChars.Length; index++)
            {
                if (parmChars[index] == '"' && !inSingleQuote)
                {
                    inDoubleQuote = !inDoubleQuote;
                    parmChars[index] = '\n';
                }
                if (parmChars[index] == '\'' && !inDoubleQuote)
                {
                    inSingleQuote = !inSingleQuote;
                    parmChars[index] = '\n';
                }
                if (!inSingleQuote && !inDoubleQuote && parmChars[index] == ' ')
                    parmChars[index] = '\n';
            }

            return new string(parmChars)
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
