using Sculptor.Core.Commands;

namespace Sculptor.Core.CommandHandlers
{
    public interface ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        void Handle(TCommand command);
    }
}