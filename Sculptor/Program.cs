using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
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
            }
            catch (Exception ex)
            {
                // If the container fails just inform the user and exit.
                Console.WriteLine($"[{nameof(Program)}.{nameof(Program.Main)}] could not initialise a valid container. The following error was returned: {ex.Message}");
                return;
            }

            var userInput = Bootstrapper.GetInstance<IUserInput>();
            userInput.Arguments = args;

            var app = Bootstrapper.GetInstance<IApplication>();

            try
            {
                app.Run(userInput);
            }
            catch (Exception e)
            {
                var logger = Bootstrapper.GetInstance<ILogger>();
                logger.Fatal($"[{nameof(Program)}.{nameof(Program.Main)}] encountered an unexpected, unrecoverable exception and had to terminate. The following exception was captured: {{@Exception}}", e);

                var terminal = Bootstrapper.GetInstance<ITerminal>();
                var configuration = Bootstrapper.GetInstance<IConfiguration>();

                // TODO: abstract away items in the config we expect to always exist -
                // maybe verify on startup and throw if not as well?
                var rollingFileConfig = configuration
                    .GetSection("Serilog")
                    .GetSection("WriteTo")
                    .GetChildren()
                    .FirstOrDefault(x => x["Name"] == "RollingFile");

                var globalLogPath = rollingFileConfig["Args:pathFormat"];

                terminal.RenderText($"[{nameof(Program)}.{nameof(Program.Main)}] encountered an unrecoverable error. Check the latest log at `{globalLogPath}` for full details");

                // Now crash out as we have no idea how to recover from this exception.
                throw;
            }
        }
    }
}