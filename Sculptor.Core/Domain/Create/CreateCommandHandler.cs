using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Serilog;

namespace Sculptor.Core.Domain.Create
{
    public sealed class CreateCommandHandler : ICommandHandler<CreateCommand>
    {
        private readonly ITerminal _console;
        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;

        public CreateCommandHandler(
            ITerminal console,
            IFileSystem fileSystem,
            ILogger logger)
        {
            _console = console;
            _fileSystem = fileSystem;
            _logger = logger;
        }

        public void Handle(CreateCommand command)
        {
            _logger.Information($"[{nameof(CreateCommandHandler)}.{nameof(CreateCommandHandler.Handle)}] successfully called with the parameter: {{@Command}}", command);

            string projectRootPath = _fileSystem.Path.Combine(
                Environment.CurrentDirectory,
                command.ProjectName);

            string outputPath = _fileSystem.Path.Combine(
                projectRootPath,
                command.OutputDirectoryName);

            _fileSystem.Directory.CreateDirectory(outputPath);

            string contentPath = _fileSystem.Path.Combine(
                projectRootPath,
                ReservedDirectories.ContentDirectoryName);

            _fileSystem.Directory.CreateDirectory(contentPath);

            const string configFileName = "appsettings.json";
            string configFilePath = _fileSystem.Path.Combine(
                projectRootPath,
                configFileName);

            var emptyFileContent = Encoding.UTF8.GetBytes("{}");

            using (var stream = _fileSystem.FileStream.Create(configFilePath, FileMode.Create))
            using (var memoryStream = new MemoryStream(emptyFileContent))
            {
                memoryStream.CopyTo(stream);
                stream.Flush();
                stream.Close();
            }

            _console.RenderText("Successfully created a Sculptor project on disk.");
            _console.RenderText($"The project root can be found at: `{projectRootPath}`");
        }
    }
}