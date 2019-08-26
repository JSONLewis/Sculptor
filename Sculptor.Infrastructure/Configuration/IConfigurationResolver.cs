using Microsoft.Extensions.Configuration;

namespace Sculptor.Infrastructure.Configuration
{
    public interface IConfigurationResolver
    {
        IConfiguration BuildConfiguration<TConfig>();
    }
}