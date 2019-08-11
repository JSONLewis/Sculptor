using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using Sculptor.Core.Commands;
using Sculptor.Core.ConsoleAbstractions;

namespace Sculptor.Core.CommandHandlers
{
    public sealed class CreateCommandHandler : ICommandHandler<CreateCommand>
    {
        private readonly ITerminal _console;
        private readonly IFileSystem _fileSystem;
        private readonly IReservedDirectories _reservedDirectories;

        public CreateCommandHandler(
            ITerminal console,
            IFileSystem fileSystem,
            IReservedDirectories reservedDirectories)
        {
            _console = console;
            _fileSystem = fileSystem;
            _reservedDirectories = reservedDirectories;
        }

        public void Handle(CreateCommand command)
        {
            _console.RenderText($"Succesfully called the {nameof(Handle)} method of {nameof(CreateCommandHandler)}");

            // TODO: look into abstracting validation out of the handler.
            if (command.OutputDirectoryName.IndexOfAny(_fileSystem.Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException($"Invalid path character(s) provided for: {nameof(command.OutputDirectoryName)}");

            if (command.ProjectName.IndexOfAny(_fileSystem.Path.GetInvalidPathChars()) != -1)
                throw new ArgumentException($"Invalid path character(s) provided for: {nameof(command.ProjectName)}");

            string projectRootPath = _fileSystem.Path.Combine(Environment.CurrentDirectory, command.ProjectName);

            if (_fileSystem.Directory.Exists(projectRootPath))
                throw new ArgumentException($"A Sculptor project already exists in the provided directory: `{nameof(command.ProjectName)}`");

            if (command.OutputDirectoryName != ReservedDirectories.OutputDirectoryName
                && _reservedDirectories.IsDirectoryNameReserved(command.OutputDirectoryName))
            {
                throw new ArgumentException($"The value provided for: {nameof(command.OutputDirectoryName)} is reserved for internal use by Sculptor");
            }

            CreateProjectStructureOnDisk(projectRootPath, command);
        }

        #region Helpers

        private void CreateProjectStructureOnDisk(string projectRootPath, CreateCommand command)
        {
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

            // The file will not already exist so there's no content to append to.
            const bool appendToFile = false;
            using (var writer = new StreamWriter(configFilePath, appendToFile, Encoding.UTF8))
            {
                writer.WriteLine("{}");
            }

            _console.RenderText("Successfully created a Sculptor project on disk.");
            _console.RenderText($"The project root can be found at: `{projectRootPath}`");
        }

        #endregion

    }
}