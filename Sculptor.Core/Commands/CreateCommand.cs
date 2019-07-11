using CommandLine;

namespace Sculptor.Core.Commands
{
    [Verb("create", HelpText = "Create a new Sculptor project")]
    public sealed class CreateCommand : ICommand
    {
        [Option('n', "name", HelpText = "The name of your project", Required = true)]
        public string ProjectName { get; set; }

        [Option('o', "output", HelpText = "The output directory for your static website content", Default = "site")]
        public string OutputDirectoryName { get; set; }
    }
}