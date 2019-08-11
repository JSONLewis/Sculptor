using Sculptor.Core.Domain;
using SimpleInjector;
using System.Diagnostics;

namespace Sculptor.Infrastructure
{
    public sealed class CommandProcessor : ICommandProcessor
    {
        private readonly Container _container;

        public CommandProcessor(Container container)
        {
            _container = container;
        }

        [DebuggerStepThrough]
        public void Process(ICommand command)
        {
            var handlerType = typeof(ICommandHandler<>)
                .MakeGenericType(command.GetType());

            dynamic handler = _container.GetInstance(handlerType);

            handler.Handle((dynamic)command);
        }
    }
}