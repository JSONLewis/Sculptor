using Sculptor.Core.ConsoleAbstractions;
using System;

namespace Sculptor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            // Valid Command using default name.
            //args = SplitArguments("create -n \"MyFirstProject\" -o \"public\"");

            // Valid Command using non-default name.
            //args = SplitArguments("create -n \"MyFirstProject\" -o \"site\"");

            // Valid Command using the default value for the name.
            args = SplitArguments("create -n \"MyFirstProject\"");

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

            // TODO: add in an exception handler so that OSs like OSX do not show a
            // failure pop up etc when an intended exception is thrown.
            // These should return a failure exit code and write to STDERR but non zero.
            // This way the user still gets informed of what went wrong - but we don't
            // treat this as severly as a genuine unexpected failure.
            // To this end all exceptions thrown intentionally should be a ValidationException
            // anything else should fall back to the OS's specific way of handling crashes.
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