using Sculptor.Infrastructure.ConsoleAbstractions;

namespace Sculptor.Infrastructure
{
    internal interface ICommandParser
    {
        void Parse(IUserInput userInput);
    }
}