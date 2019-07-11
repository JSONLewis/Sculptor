using CommandLine;
using Sculptor.Core;
using Sculptor.Core.ConsoleAbstractions;

namespace Sculptor.Infrastructure
{
    public sealed class CommandParser : ICommandParser
    {
        private readonly ITerminal _console;
        private readonly ICommandProcessor _commandProcessor;
        private readonly IRegisteredVerbs _registeredVerbs;

        public CommandParser(
            ITerminal console,
            ICommandProcessor commandProcessor,
            IRegisteredVerbs registeredVerbs)
        {
            _console = console;
            _commandProcessor = commandProcessor;
            _registeredVerbs = registeredVerbs;
        }

        public void Parse(IUserInput userInput)
        {
            var result = Parser.Default.ParseArguments(
                userInput.Arguments,
                _registeredVerbs.KnownVerbs);

            if (result.Tag == ParserResultType.Parsed)
            {
                dynamic parsedCommand = (result as Parsed<object>)?.Value;

                if (parsedCommand != null)
                {
                    _commandProcessor.Process(parsedCommand);
                    return;
                }
            }

            HandleFailedParsing(userInput);
        }

        private void HandleFailedParsing(IUserInput userInput)
        {
            _console.RenderText($"Failure processing `{userInput.Raw}`. Now invoking {nameof(HandleFailedParsing)}");
        }
    }
}