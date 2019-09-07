using Microsoft.Extensions.Configuration;

namespace Sculptor.Infrastructure.Configuration
{
    public class LocalConfiguration : ILocalConfiguration
    {
        private readonly IConfiguration _configuration;

        public LocalConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string OutputPath
        {
            get
            {
                return _configuration["Sculptor:Configuration:OutputPath"];
            }
        }

        public string ProjectName
        {
            get
            {
                return _configuration["Sculptor:Configuration:ProjectName"];
            }
        }
    }
}