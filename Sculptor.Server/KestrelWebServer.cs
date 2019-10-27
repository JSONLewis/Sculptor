using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Sculptor.Infrastructure.Logging;
using Serilog;

namespace Sculptor.Server
{
    public class KestrelWebServer : IWebServer
    {
        private readonly ILocalLogger localLogger;

        public KestrelWebServer(ILocalLogger localLogger)
        {
            this.localLogger = localLogger;
        }

        public void Run(string webRoot, int port)
        {
            localLogger.Instance.Information($"{nameof(KestrelWebServer)} is now listening for changes in {webRoot} on port: {port}");

            WebHost.CreateDefaultBuilder()
                .UseKestrel()
                .UseWebRoot(webRoot)
                .ConfigureServices(services =>
                {
                    services.AddSingleton(localLogger);
                })
            .UseStartup<Startup>()
            .UseSerilog()
            .Build()
            .Run();
        }
    }
}