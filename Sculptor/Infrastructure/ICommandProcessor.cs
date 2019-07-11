using Sculptor.Core.Commands;

namespace Sculptor.Infrastructure
{
    public interface ICommandProcessor
    {
        void Process(ICommand command);
    }
}