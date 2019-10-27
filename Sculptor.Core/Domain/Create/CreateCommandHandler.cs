using Sculptor.Infrastructure;
using Sculptor.Infrastructure.ConsoleAbstractions;
using System.IO.Abstractions;

namespace Sculptor.Core.Domain.Create
{
    public sealed class CreateCommandHandler : ICommandHandler<CreateCommand>
    {
        private readonly IConfigComposer _configComposer;
        private readonly IContentComposer _contentComposer;
        private readonly IFileSystem _fileSystem;
        private readonly ITerminal _terminal;

        public CreateCommandHandler(
            IConfigComposer configComposer,
            IContentComposer contentComposer,
            IFileSystem fileSystem,
            ITerminal terminal)
        {
            _configComposer = configComposer;
            _contentComposer = contentComposer;
            _fileSystem = fileSystem;
            _terminal = terminal;
        }

        public void Handle(CreateCommand command)
        {
            string projectRootPath = _fileSystem.Path.Combine(
                FilePathHelper.ExecutingDirectory,
                command.ProjectName);

            string outputPath = _fileSystem.Path.Combine(
                projectRootPath,
                command.OutputDirectoryName);

            _fileSystem.Directory.CreateDirectory(projectRootPath);
            _fileSystem.Directory.CreateDirectory(outputPath);

            _configComposer.Compose(command.ProjectName, projectRootPath, outputPath);
            _contentComposer.Compose(projectRootPath);

            _terminal.RenderText("Successfully created a Sculptor project on disk.");
            _terminal.RenderText($"The project root can be found at: `{projectRootPath}`");
        }
    }
}