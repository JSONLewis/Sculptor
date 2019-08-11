using Sculptor.Core.Domain;

namespace Sculptor.Infrastructure
{
    internal interface ICommandProcessor
    {
        void Process(ICommand command);
    }
}