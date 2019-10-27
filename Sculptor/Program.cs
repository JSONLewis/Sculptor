using Sculptor.Infrastructure.ConsoleAbstractions;
using System;

namespace Sculptor
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            SetCommandScope(args);

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

            app.Run(userInput);
        }

        /// <summary>
        /// Works out if the arguments provided represent a change to an existing project
        /// (local scope) or one that can be invoked anywhere (global).
        /// </summary>
        /// <param name="args"></param>
        private static void SetCommandScope(string[] args)
        {
#if DEBUG
            if (args.Length == 0)
                throw new NotSupportedException("Sculptor must be invoked with at least one verb");

            bool isLocalScope = !args[0].Equals("create", StringComparison.OrdinalIgnoreCase);

            if (isLocalScope)
            {
                // For the sake of debugging commands that will be invoked in project
                // directories (such as launching the web server) set the value here to
                // match the folder created under the bin folder as appropriate.
                Infrastructure.FilePathHelper.CurrentDirectory = System.IO.Path.Combine(Environment.CurrentDirectory, "MyFirstProject");
            }
#endif
        }
    }
}