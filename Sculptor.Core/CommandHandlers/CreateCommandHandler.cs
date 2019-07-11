using Sculptor.Core.Commands;
using Sculptor.Core.ConsoleAbstractions;

namespace Sculptor.Core.CommandHandlers
{
    public sealed class CreateCommandHandler : ICommandHandler<CreateCommand>
    {
        private readonly ITerminal _console;

        public CreateCommandHandler(ITerminal console)
        {
            _console = console;
        }

        public void Handle(CreateCommand command)
        {
            _console.RenderText($"Succesfully called the {nameof(Handle)} method of {nameof(CreateCommandHandler)}");
        }
    }
}