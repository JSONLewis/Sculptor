using FluentValidation;
using Sculptor.Infrastructure;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.ValidationFormatters;
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
                // TODO:
                // Use this to store configuration information about the Sculptor tool
                // itself. This should be verified on startup. Offer a rollback to
                // default if it fails (and a dedicated command) and allow users to make
                // changes for various internal settings.
                // string path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                // TODO:
                // Once we have configuration files being read in properly we can then
                // integrate a logging framework that uses this file to determine where to
                // persist logs. (By default create a "log" folder in the same location).

                // TODO:
                // When we have a functioning logger using Serilog we can then replace the
                // any method calling "Trace.WriteLine" with our injected logger and
                // update the tests accordingly for verifying that failures are recorded.

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
    }
}