using Sculptor.Core.Domain;

namespace Sculptor.Infrastructure
{
    public interface ICommandProcessor
    {
        void Process(ICommand command);
    }
}