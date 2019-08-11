using CommandLine.Text;
using Sculptor.Infrastructure.ConsoleAbstractions;

namespace Sculptor.ValidationFormatters
{
    internal sealed class TemplateBuilder : ITemplateBuilder
    {
        private readonly IUserInput _userInput;

        public TemplateBuilder(IUserInput userInput)
        {
            _userInput = userInput;
        }

        public string BuildErrorTemplate(string errorMessage)
        {
            if (errorMessage is null)
                return string.Empty;

            return HelpText.AutoBuild(_userInput.ParsedCommand, null, null)
                .AddPreOptionsLine(errorMessage)
                .ToString();
        }
    }
}