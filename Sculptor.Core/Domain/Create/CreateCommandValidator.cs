using FluentValidation;
using System;
using System.IO.Abstractions;

namespace Sculptor.Core.Domain.Create
{
    public class CreateCommandValidator : AbstractValidator<CreateCommand>
    {
        private readonly IFileSystem _fileSystem;
        private readonly IReservedDirectories _reservedDirectories;

        public CreateCommandValidator(
            IFileSystem fileSystem,
            IReservedDirectories reservedDirectories)
        {
            _fileSystem = fileSystem;
            _reservedDirectories = reservedDirectories;

            RuleFor(c => c.ProjectName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(IsValidPath)
                .WithMessage("Invalid path character(s) provided for: `name`")
                .Must(IsValidProjectLocation)
                .WithMessage(c => $"A Sculptor project with the name `{c.ProjectName}` already exists in the provided directory");

            RuleFor(c => c.OutputDirectoryName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .Must(IsValidPath)
                .WithMessage("Invalid path character(s) provided for: `output`")
                .Must(IsValidDirectoryName)
                .WithMessage("The value provided for: `output` is reserved for internal use by Sculptor");
        }

        #region Helpers

        private bool IsValidPath(string path)
        {
            return path.IndexOfAny(_fileSystem.Path.GetInvalidPathChars()) == -1;
        }

        private bool IsValidProjectLocation(string projectName)
        {
            string projectRootPath = _fileSystem.Path.Combine(Environment.CurrentDirectory, projectName);
            return !_fileSystem.Directory.Exists(projectRootPath);
        }

        private bool IsValidDirectoryName(string directoryName)
        {
            return directoryName == ReservedDirectories.OutputDirectoryName
                || !_reservedDirectories.IsDirectoryNameReserved(directoryName);
        }

        #endregion Helpers
    }
}