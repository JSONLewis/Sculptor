using Sculptor.Core.ConsoleAbstractions;
using Sculptor.Infrastructure;

namespace Sculptor
{
    public sealed class Application : IApplication
    {
        private readonly ICommandParser _commandParser;

        public Application(ICommandParser commandParser)
        {
            _commandParser = commandParser;
        }

        public void Run(IUserInput userInput)
        {
            _commandParser.Parse(userInput);
        }
    }
}