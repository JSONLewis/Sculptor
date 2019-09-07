using CommandLine;
using CommandLine.Text;
using Sculptor.Core;
using Sculptor.Core.Domain;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Infrastructure.Exceptions.ParserExceptions;
using Sculptor.Infrastructure.Logging;
using Sculptor.Infrastructure.OutputFormatters;

namespace Sculptor.Parsing
{
    public sealed class CommandParser : ICommandParser
    {
        private readonly IOutputFormatter _outputFormatter;
        private readonly IRegisteredVerbs _registeredVerbs;
        private readonly IGlobalLogger _logger;

        public CommandParser(
            IOutputFormatter templateBuilder,
            IRegisteredVerbs registeredVerbs,
            IGlobalLogger logger)
        {
            _outputFormatter = templateBuilder;
            _registeredVerbs = registeredVerbs;
            _logger = logger;
        }

        public ICommand Parse(IUserInput userInput)
        {
            _logger.Instance.Information($"[{nameof(CommandParser)}.{nameof(CommandParser.Parse)}] called with the following parameter: {{@UserInput}}", userInput);

            var parserResult = Parser.Default.ParseArguments(
                userInput.Arguments,
                _registeredVerbs.KnownVerbs);

            // Cache the HelpText on this parser result for future formatting.
            _outputFormatter.HelpText = HelpText.AutoBuild(parserResult, null, null);

            if (parserResult.Tag == ParserResultType.NotParsed)
            {
                _logger.Instance.Error($"[{nameof(CommandParser)}.{nameof(CommandParser.Parse)}] was unable to parse provided command: {{@UserInput}}", userInput);
                return null;
            }

            if (!((parserResult as Parsed<object>)?.Value is ICommand command))
                throw new SculptorParserException($"[{nameof(CommandParser)}.{nameof(CommandParser.Parse)}] could not find a valid implementation of {nameof(ICommand)} matching {{@UserInput}}", userInput);

            _logger.Instance.Information($"[{nameof(CommandParser)}.{nameof(CommandParser.Parse)}] succesfully parsed: {{@UserInput}}", userInput);

            return command;
        }
    }
}