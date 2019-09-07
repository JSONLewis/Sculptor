using Moq;
using NUnit.Framework;
using Sculptor.Core;
using Sculptor.Core.Domain.Create;
using Sculptor.Infrastructure.ConsoleAbstractions;
using System;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;

namespace Sculptor.Tests.DomainTests.CreateTests
{
    public class CreateCommandValidatorTests
    {
        private Mock<ITerminal> _mockTerminal;

        #region Setup

        [SetUp]
        public void Setup()
        {
            _mockTerminal = new Mock<ITerminal>();
            _mockTerminal.Setup(x => x.RenderText(It.IsAny<string>()));
        }

        #endregion Setup

        [Test]
        public void CreateCommandValidatorFailsIfOutputDirectoryMatchesReservedName()
        {
            var mockReservedDirectories = new Mock<IReservedDirectories>();
            mockReservedDirectories
                .Setup(x => x.IsDirectoryNameReserved(It.IsAny<string>()))
                .Returns(true);

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(Environment.CurrentDirectory);

            var command = new CreateCommand
            {
                ProjectName = "MyUnitTestProject",
                // It doesn't matter what this is set to as we have stubbed the
                // `IsDirectoryNameReserved` method above to always return true.
                OutputDirectoryName = "reserved-for-internal-use"
            };

            var validator = new CreateCommandValidator(
                mockFileSystem,
                mockReservedDirectories.Object);

            var validationResult = validator.Validate(command);

            const string expectedErrorMessage = "The value provided for: `output` is reserved for internal use by Sculptor";

            bool hasExpectedNumberOfErrors = validationResult.Errors.Count() == 1;
            bool hasExpectedErrorMessage = validationResult
                .Errors
                .Any(x => x.ErrorMessage == expectedErrorMessage);

            Assert.Multiple(() =>
            {
                Assert.True(hasExpectedNumberOfErrors);
                Assert.True(hasExpectedErrorMessage);
            });
        }

        [TestCase("\0", "public", 1)]
        [TestCase("MyUnitTestProject", "\0", 1)]
        [TestCase("\0", "\0", 2)]
        public void CreateCommandValidatorFailsForInvalidCommand(
            string projectName,
            string outputDirectoryName,
            int expectedNumberOfErrors)
        {
            var mockReservedDirectories = new Mock<IReservedDirectories>();
            mockReservedDirectories
                .Setup(x => x.IsDirectoryNameReserved(It.IsAny<string>()))
                .Returns(true);

            var mockFileSystem = new MockFileSystem();

            var command = new CreateCommand
            {
                ProjectName = projectName,
                OutputDirectoryName = outputDirectoryName
            };

            var validator = new CreateCommandValidator(
                mockFileSystem,
                mockReservedDirectories.Object);

            var validationResult = validator.Validate(command);

            const string expectedErrorMessage = "Invalid path character(s) provided for:";
            bool hasExpectedNumberOfErrors = validationResult.Errors.Count() == expectedNumberOfErrors;
            bool hasExpectedErrorMessage = validationResult
                .Errors
                .Any(x => x.ErrorMessage.StartsWith(
                    expectedErrorMessage,
                    StringComparison.Ordinal));

            Assert.Multiple(() =>
            {
                Assert.True(hasExpectedNumberOfErrors);
                Assert.True(hasExpectedErrorMessage);
            });
        }

        [Test]
        public void CreateCommandValidatorFailsIfProjectAlreadyExists()
        {
            var mockReservedDirectories = new Mock<IReservedDirectories>();

            var mockFileSystem = new MockFileSystem();

            const string projectName = "MyUnitTestProject";
            const string outputDirectoryName = "public";

            string projectRootPath = mockFileSystem.Path.Combine(
                Environment.CurrentDirectory,
                projectName);

            // Simulate the `Create` command already having been successfully run by
            // adding a directory with the same project name.
            mockFileSystem.AddDirectory(projectRootPath);

            var command = new CreateCommand
            {
                ProjectName = projectName,
                OutputDirectoryName = outputDirectoryName
            };

            var validator = new CreateCommandValidator(
                mockFileSystem,
                mockReservedDirectories.Object);

            var validationResult = validator.Validate(command);

            string expectedErrorMessage = $"A Sculptor project with the name `{projectName}` already exists in the provided directory";
            bool hasExpectedNumberOfErrors = validationResult.Errors.Count() == 1;
            bool hasExpectedErrorMessage = validationResult
                .Errors
                .Any(x => x.ErrorMessage == expectedErrorMessage);

            Assert.Multiple(() =>
            {
                Assert.True(hasExpectedNumberOfErrors);
                Assert.True(hasExpectedErrorMessage);
            });
        }
    }
}