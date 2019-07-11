using Sculptor.Core.ConsoleAbstractions;
using System;

namespace Sculptor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            // Valid Command.
            args = SplitArguments("create -n \"MyFirstProject\" -o \"public\"");

            // Invalid Command Argument.
            //args = SplitArguments("create -n \"MyFirstProject\" -o \"public\" --invalidOption");

            // Invalid Command Verb.
            //args = SplitArguments("invalidverb -t");
#endif

            var app = Bootstrapper.InitialiseApplication();

            var command = new UserInput
            {
                Arguments = args
            };

            app.Run(command);
        }

        private static string[] SplitArguments(string rawInput)
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