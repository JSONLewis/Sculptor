using System;
using Sculptor.Infrastructure.ConsoleAbstractions;

namespace Sculptor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
#if DEBUG
            // For the sake of debugging commands that will be invoked in project
            // directories (such as launching the web server) set the value here to
            // match the folder created under the bin folder as appropriate.
            // Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "MyFirstProject");
#endif
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

            app.Run(userInput);
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