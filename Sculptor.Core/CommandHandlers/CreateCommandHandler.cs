using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using Sculptor.Core.Commands;
using Sculptor.Infrastructure.ConsoleAbstractions;

namespace Sculptor.Core.CommandHandlers
{
    public sealed class CreateCommandHandler : ICommandHandler<CreateCommand>
    {
        private readonly ITerminal _console;
        private readonly IFileSystem _fileSystem;

        public CreateCommandHandler(
            ITerminal console,
            IFileSystem fileSystem)
        {
            _console = console;
            _fileSystem = fileSystem;
        }

        public void Handle(CreateCommand command)
        {
            // TODO: add messages like this to a log instead of Stdout. (Maybe we would
            // want to do something like this in a 'verbose' mode though).
            _console.RenderText($"Succesfully called the {nameof(Handle)} method of {nameof(CreateCommandHandler)}");

            string projectRootPath = _fileSystem.Path.Combine(Environment.CurrentDirectory, command.ProjectName);

            string outputPath = _fileSystem.Path.Combine(
                projectRootPath,
                command.OutputDirectoryName);

            _fileSystem.Directory.CreateDirectory(outputPath);

            string contentPath = _fileSystem.Path.Combine(
                projectRootPath,
                ReservedDirectories.ContentDirectoryName);

            _fileSystem.Directory.CreateDirectory(contentPath);

            const string configFileName = "appsettings.json";
            string configFilePath = _fileSystem.Path.Combine(projectRootPath, configFileName);

            var fileContent = Encoding.UTF8.GetBytes("{}");

            using (Stream stream = _fileSystem.FileStream.Create(configFilePath, FileMode.Create))
            using (var memoryStream = new MemoryStream(fileContent))
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