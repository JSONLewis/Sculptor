using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Sculptor.Infrastructure.Configuration
{
    public class GlobalConfiguration : IGlobalConfiguration
    {
        private readonly IConfiguration _configuration;

        public GlobalConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string LogDirectoryPath
        {
            get
            {
                return _configuration
                    .GetSection("Serilog")
                    .GetSection("WriteTo")
                    .GetChildren()
                    .FirstOrDefault(x => x["Name"] == "RollingFile")
                    .GetSection("Args")
                    .GetValue<string>("pathFormat");
            }
        }
    }
}