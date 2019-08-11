using CommandLine;

namespace Sculptor.Core.Domain.Create
{
    [Verb("create", HelpText = "Create a new Sculptor project")]
    public sealed class CreateCommand : ICommand
    {
        [Option('n', "name", HelpText = "The name of your project", Required = true)]
        public string ProjectName { get; set; }

        [Option('o', "output", HelpText = "The output directory for your static website content", Default = ReservedDirectories.OutputDirectoryName)]
        public string OutputDirectoryName { get; set; }
    }
}