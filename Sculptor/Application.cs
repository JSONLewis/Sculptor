using System;
using Sculptor.Infrastructure;
using Sculptor.Infrastructure.Configuration;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Infrastructure.Logging;
using Sculptor.Parsing;

namespace Sculptor
{
    public sealed class Application : IApplication
    {
        private readonly ICommandParser _commandParser;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IGlobalLogger _globalLogger;
        private readonly IGlobalConfiguration _globalConfiguration;
        private readonly ITerminal _terminal;

        public Application(
            ICommandParser commandParser,
            ICommandProcessor commandProcessor,
            IGlobalLogger globalLogger,
            IGlobalConfiguration globalConfiguration,
            ITerminal terminal)
        {
            _commandParser = commandParser;
            _commandProcessor = commandProcessor;
            _globalLogger = globalLogger;
            _globalConfiguration = globalConfiguration;
            _terminal = terminal;
        }

        public void Run(IUserInput userInput)
        {
            var parserResult = _commandParser.Parse(userInput);

            // A null or otherwise failed parse attempt internally handles informing the
            // user of what went wrong, so we can just exit.
            if (parserResult is null)
                return;

            try
            {
                _commandProcessor.Process(parserResult);
            }
            catch (Exception ex)
            {
                _globalLogger.Instance.Fatal($"[{nameof(Application)}.{nameof(Application.Run)}] encountered an unrecoverable error and had to terminate. The following exception was captured: {{@Exception}}", ex);

                _terminal.RenderText($"[{nameof(Application)}.{nameof(Application.Run)}] encountered an unrecoverable error. Check the latest log at `{_globalConfiguration.LogDirectoryPath}` for full details");
            }
        }
    }
}