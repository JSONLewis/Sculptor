using System;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Serilog;

namespace Sculptor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                Bootstrapper.InitialiseApplication();

                var userInput = Bootstrapper.GetInstance<IUserInput>();
                userInput.Arguments = args;

                var app = Bootstrapper.GetInstance<IApplication>();
                app.Run(userInput);
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