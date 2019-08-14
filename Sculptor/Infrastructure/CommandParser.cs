using CommandLine;
using Sculptor.Core;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Serilog;
using System.Linq;

namespace Sculptor.Infrastructure
{
    public sealed class CommandParser : ICommandParser
    {
        private readonly ICommandProcessor _commandProcessor;
        private readonly IRegisteredVerbs _registeredVerbs;
        private readonly ILogger _logger;

        public CommandParser(
            ICommandProcessor commandProcessor,
            IRegisteredVerbs registeredVerbs,
            ILogger logger)
        {
            _commandProcessor = commandProcessor;
            _registeredVerbs = registeredVerbs;
            _logger = logger;
        }

        public void Parse(IUserInput userInput)
        {
            _logger.Information($"[{nameof(CommandParser)}.{nameof(CommandParser.Parse)}] called with the following parameter: {{@UserInput}}", userInput);

            var result = Parser.Default.ParseArguments(
                userInput.Arguments,
                _registeredVerbs.KnownVerbs.ToArray());

            if (result.Tag == ParserResultType.Parsed)
            {
                _logger.Information($"[{nameof(CommandParser)}.{nameof(CommandParser.Parse)}] succesfully parsed: {{@UserInput}}", userInput);

                dynamic parsedCommand = (result as Parsed<object>)?.Value;

                if (parsedCommand is null)
                {
                    _logger.Fatal($"[{nameof(CommandParser)}.{nameof(CommandParser.Parse)}] failed to parse: {{@UserInput}} to a non-null result", userInput);
                    return;
                }

                // Make the raw parse result available to any part of the program
                // that takes a dependency on the Singleton `IUserInput`.
                userInput.ParsedCommand = result;

                _commandProcessor.Process(parsedCommand);
                return;
            }

            _logger.Fatal($"[{nameof(CommandParser)}.{nameof(CommandParser.Parse)}] failed trying to parse: {{@UserInput}}", userInput);
        }
    }
}