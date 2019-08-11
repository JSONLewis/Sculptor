namespace Sculptor.Core.Domain
{
    public interface ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        void Handle(TCommand command);
    }
}