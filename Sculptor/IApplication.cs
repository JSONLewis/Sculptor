using Sculptor.Infrastructure.ConsoleAbstractions;

namespace Sculptor
{
    public interface IApplication
    {
        void Run(IUserInput userInput);
    }
}