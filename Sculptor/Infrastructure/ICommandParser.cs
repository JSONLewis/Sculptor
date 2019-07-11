using Sculptor.Core.ConsoleAbstractions;

namespace Sculptor.Infrastructure
{
    public interface ICommandParser
    {
        void Parse(IUserInput userInput);
    }
}