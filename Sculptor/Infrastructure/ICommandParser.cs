using Sculptor.Infrastructure.ConsoleAbstractions;

namespace Sculptor.Infrastructure
{
    public interface ICommandParser
    {
        void Parse(IUserInput userInput);
    }
}