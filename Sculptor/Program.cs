using FluentValidation;
using Sculptor.Infrastructure;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.ValidationFormatters;
using Serilog;
using System;

namespace Sculptor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            // TODO:
            // Look into how we can perform acceptance / regression testing by running a
            // full instance of the application with a known set of commands.
            // Maybe comparing the output screenshots? Or piping the response from
            // STDOUT / STDERR etc.
#if DEBUG
            // Valid Command using default name.
            args = ArgumentHelper.SplitArguments("create -n \"MyFirstProject\"");

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
                var logger = Bootstrapper.GetInstance<ILogger>();
                logger.Error($"[{nameof(Program)}.{nameof(Program.Main)}] encountered a Validation Exception due to invalid user input. The following exception was captured: {{@Exception}}", e);

                var validationFormatter = Bootstrapper
                    .GetInstance<IFluentValidationFormatter>();

                // TODO: look into a way of better highlighting errors in a cross platform
                // compliant way. Should they be in a different colour etc or just let
                // the user's own preferences decide the formatting of the output?
                var terminal = Bootstrapper.GetInstance<ITerminal>();
                var templateBuilder = Bootstrapper.GetInstance<ITemplateBuilder>();

                var groupedErrors = validationFormatter.GroupExceptionsByName(e.Errors);
                string message = groupedErrors.FormatPrimaryErrorMessages();
                string template = templateBuilder.BuildErrorTemplate(message);

                // TODO: look into a verbose flag for controlling how much information
                // should be shown.
                terminal.RenderText(template);
            }
            catch (Exception e)
            {
                // TODO: verify that it's not an issue with the Container that's causing
                // us to crash and that we're still able to log. We don't want to muddy
                // the water by causing another exception to be thrown and masking the
                // actual issue.

                // Log as much information as we can (this will probably need to be more
                // than just the raw stack trace later on, but this is fine for now).
                var logger = Bootstrapper.GetInstance<ILogger>();
                logger.Fatal($"[{nameof(Program)}.{nameof(Program.Main)}] encountered an unexpected, unrecoverable exception and had to terminate. The following exception was captured: {{@Exception}}", e);

                // Now let the program crash as we have no idea how to recover from this.
                throw e;
            }
        }
    }
}