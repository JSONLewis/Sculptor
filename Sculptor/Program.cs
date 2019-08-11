using FluentValidation;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.ValidationFormatters;
using System;

namespace Sculptor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            // Valid Command using default name.
            args = SplitArguments("create -n \"MyFirstProject\"");

            // Valid Command using non-default name.
            //args = SplitArguments("create -n \"MyFirstProject\" -o \"site\"");

            // Invalid Command with no value provided for the name.
            //args = SplitArguments("create -n");

            // Invalid Command Argument.
            //args = SplitArguments("create -n \"MyFirstProject\" -o \"public\" --invalidOption");

            // Invalid Command Verb.
            //args = SplitArguments("invalidverb -t");
#endif         

            try
            {
                Bootstrapper.InitialiseApplication();
                var app = Bootstrapper.GetInstance<IApplication>();

                var command = Bootstrapper.GetInstance<IUserInput>();
                command.Arguments = args;

                app.Run(command);
            }
            catch (ValidationException e)
            {
                var validationFormatter = Bootstrapper
                    .GetInstance<IFluentValidationFormatter>();

                // TODO: serialise this object and log the full thing to file.
                var groupedErrors = validationFormatter.GroupExceptionsByName(e.Errors);

                // TODO: look into a way of better highlighting errors in a cross platform
                // compliant way. Should they be in a different colour etc?
                var terminal = Bootstrapper.GetInstance<ITerminal>();
                var templateBuilder = Bootstrapper.GetInstance<ITemplateBuilder>();

                string message = groupedErrors.FormatPrimaryErrorMessages();
                string template = templateBuilder.BuildErrorTemplate(message);

                // TODO: look into a verbose flag for controlling how much information
                // should be shown.
                terminal.RenderText(template);
            }
            catch (Exception e)
            {
                // TODO: try to log as much information as possible before letting this
                // bubble up. For now pretend we're doing something extra.
                throw e;
            }
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