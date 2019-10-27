using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Sculptor.Infrastructure.Logging;

namespace Sculptor.Server
{
    public class Startup
    {
        private readonly ILocalLogger localLogger;

        public Startup(ILocalLogger localLogger)
        {
            this.localLogger = localLogger;
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment hostingEnvironment)
        {
            if (hostingEnvironment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            localLogger.Instance.Information("Configured webserver");
        }
    }
}