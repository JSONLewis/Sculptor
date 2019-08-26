using System;
using Sculptor.Infrastructure.Configuration;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Infrastructure.Logging;

namespace Sculptor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var commandScope = GetCommandScope(args);

            try
            {
                Bootstrapper.InitialiseApplication(commandScope);
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
#if DEBUG
                // For the sake of debugging commands that will be invoked in project
                // directories (such as launching the web server) set the value here to
                // match the folder created under the bin folder as appropriate.
                // Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "MyFirstProject");
#endif
                app.Run(userInput);
            }
            catch (Exception e)
            {
                var logger = Bootstrapper.GetInstance<IGlobalLogger>();
                logger.Instance.Fatal($"[{nameof(Program)}.{nameof(Program.Main)}] encountered an unexpected, unrecoverable exception and had to terminate. The following exception was captured: {{@Exception}}", e);

                var terminal = Bootstrapper.GetInstance<ITerminal>();
                var configuration = Bootstrapper.GetInstance<IGlobalConfiguration>();

                terminal.RenderText($"[{nameof(Program)}.{nameof(Program.Main)}] encountered an unrecoverable error. Check the latest log at `{configuration.LogDirectoryPath}` for full details");
            }
        }

        /// <summary>
        /// Works out if the arguments provided represent a change to an existing project
        /// (local scope) or one that can be invoked anywhere (global).
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static CommandScope GetCommandScope(string[] args)
        {
            if (args.Length <= 0)
                throw new NotSupportedException("Sculptor must be invoked with at least one verb");

            return args[0].Equals("create", StringComparison.OrdinalIgnoreCase)
                ? CommandScope.Global
                : CommandScope.Local;
        }
    }

    enum CommandScope
    {
        Global,
        Local
    }
}