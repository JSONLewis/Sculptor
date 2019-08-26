using System;
using System.IO.Abstractions;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Infrastructure.Logging;

namespace Sculptor.Core.Domain.Create
{
    public sealed class CreateCommandHandler : ICommandHandler<CreateCommand>
    {
        private readonly IConfigComposer _configComposer;
        private readonly IContentComposer _contentComposer;
        private readonly IFileSystem _fileSystem;
        private readonly ITerminal _terminal;
        private readonly IGlobalLogger _logger;

        public CreateCommandHandler(
            IConfigComposer configComposer,
            IContentComposer contentComposer,
            IFileSystem fileSystem,
            ITerminal terminal,
            IGlobalLogger logger)
        {
            _configComposer = configComposer;
            _contentComposer = contentComposer;
            _fileSystem = fileSystem;
            _terminal = terminal;
            _logger = logger;
        }

        public void Handle(CreateCommand command)
        {
            _logger.Instance.Information($"[{nameof(CreateCommandHandler)}.{nameof(CreateCommandHandler.Handle)}] successfully called with the parameter: {{@Command}}", command);

            string projectRootPath = _fileSystem.Path.Combine(
                Environment.CurrentDirectory,
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

            _logger.Instance.Information($"[{nameof(CreateCommandHandler)}.{nameof(CreateCommandHandler.Handle)}] successfully created a Sculptor project on disk from the parameter: {{@Command}}", command);
        }
    }
}