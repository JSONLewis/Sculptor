using System.Diagnostics;
using CommandLine;
using Sculptor.Core;
using Sculptor.Infrastructure.ConsoleAbstractions;

namespace Sculptor.Infrastructure
{
    internal sealed class CommandParser : ICommandParser
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly IRegisteredVerbs _registeredVerbs;

        public CommandParser(
            ICommandProcessor commandProcessor,
            IRegisteredVerbs registeredVerbs)
        {
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
                    // Make the raw parse result available to any part of the program
                    // that takes a dependency on the Singleton `IUserInput`.
                    userInput.ParsedCommand = result;

                    _commandProcessor.Process(parsedCommand);
                    return;
                }
            }

            HandleFailedParsing(userInput);
        }

        private void HandleFailedParsing(IUserInput userInput)
        {
            // TODO: replace this with a call to a logger so that we record this
            // information on disk.
            Trace.WriteLine($"Failure processing `{userInput.Raw}`. Now invoking {nameof(HandleFailedParsing)}");
        }
    }
}