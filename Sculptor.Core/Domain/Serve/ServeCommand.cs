using CommandLine;
using Sculptor.Infrastructure.Markers;

namespace Sculptor.Core.Domain.Serve
{
    [Verb("serve", HelpText = "Host a Sculptor project using the embedded Kestrel server")]
    public class ServeCommand : ICommand, IExcludeFromValidation
    {
        [Option('p', "port", HelpText = "The port number the server should listen on for HTTP requests", Default = 5000)]
        public int Port { get; set; }
    }
}