using Sculptor.Core.ConsoleAbstractions;

namespace Sculptor
{
    public interface IApplication
    {
        void Run(IUserInput userInput);
    }
}