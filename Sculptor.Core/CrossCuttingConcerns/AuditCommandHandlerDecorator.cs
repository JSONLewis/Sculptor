using System;
using System.Diagnostics;
using Sculptor.Core.Domain;
using Sculptor.Infrastructure.Logging;

namespace Sculptor.Core.CrossCuttingConcerns
{
    public sealed class AuditCommandHandlerDecorator<TCommand>
        : ICommandHandler<TCommand> where TCommand : class, ICommand
    {
        private readonly ICommandHandler<TCommand> _decorated;
        private readonly IGlobalLogger _globalLogger;

        public AuditCommandHandlerDecorator(
            ICommandHandler<TCommand> decorated,
            IGlobalLogger globalLogger)
        {
            _decorated = decorated;
            _globalLogger = globalLogger;
        }

        public void Handle(TCommand command)
        {
            var timer = new Stopwatch();
            var state = new CurrentState(Guid.NewGuid(), typeof(TCommand).Name, timer);

            timer.Start();

            _globalLogger.Instance.Information($"[{nameof(AuditCommandHandlerDecorator<TCommand>)}.{nameof(AuditCommandHandlerDecorator<TCommand>.Handle)}] invoked with the following state: {{@State}}", state);

            try
            {
                _decorated.Handle(command);
            }
            finally
            {
                timer.Stop();

                _globalLogger.Instance.Information($"[{nameof(AuditCommandHandlerDecorator<TCommand>)}.{nameof(AuditCommandHandlerDecorator<TCommand>.Handle)}] completed with the following state: {{@State}}", state);
            }
        }
    }

    internal struct CurrentState
    {
        public CurrentState(Guid correlationId, string commandName, Stopwatch timer)
        {
            CommandName = commandName;
            CorrelationId = correlationId;
            Timer = timer;
        }

        public Guid CorrelationId { get; }

        public string CommandName { get; }

        public long ExecutionTimeMilliseconds
        {
            get { return Timer.ElapsedMilliseconds; }
        }

        private Stopwatch Timer { get; set; }
    }
}