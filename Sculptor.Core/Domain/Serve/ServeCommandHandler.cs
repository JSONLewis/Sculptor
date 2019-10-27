using Sculptor.Infrastructure.Configuration;
using Sculptor.Infrastructure.Logging;
using Sculptor.Server;

namespace Sculptor.Core.Domain.Serve
{
    public class ServeCommandHandler : ICommandHandler<ServeCommand>
    {
        private readonly IGlobalLogger globalLogger;
        private readonly ILocalConfiguration localConfiguration;
        private readonly IWebServer webServer;

        public ServeCommandHandler(
            IGlobalLogger globalLogger,
            ILocalConfiguration localConfiguration,
            IWebServer webServer)
        {
            this.globalLogger = globalLogger;
            this.localConfiguration = localConfiguration;
            this.webServer = webServer;
        }

        public void Handle(ServeCommand command)
        {
            globalLogger.Instance.Information($"[{nameof(ServeCommandHandler)}.{nameof(ServeCommandHandler.Handle)}] succesfully called with the parameter: {{@Command}}", command);

            webServer.Run(localConfiguration.OutputPath, command.Port);
        }
    }
}