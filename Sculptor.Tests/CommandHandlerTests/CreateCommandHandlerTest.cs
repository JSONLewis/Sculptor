using System;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using NUnit.Framework;
using Sculptor.Core;
using Sculptor.Core.CommandHandlers;
using Sculptor.Core.Commands;
using Sculptor.Core.ConsoleAbstractions;

namespace Sculptor.Tests.CommandHandlerTests
{
    public class CreateCommandHandlerTest
    {
        private Mock<ITerminal> _mockTerminal;

        #region Setup

        [SetUp]
        public void Setup()
        {
            _mockTerminal = new Mock<ITerminal>();
            _mockTerminal.Setup(x => x.RenderText(It.IsAny<string>()));
        }

        #endregion

        [Test]
        public void CreateCommandHandlerOnSuccessHasValidDirectoryStructure()
        {
            var mockReservedDirectories = new Mock<IReservedDirectories>();

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(Environment.CurrentDirectory);

            var handler = new CreateCommandHandler(
                _mockTerminal.Object,
                mockFileSystem,
                mockReservedDirectories.Object);

            const string projectName = "MyUnitTestProject";
            const string outputDirectoryName = "public";

            handler.Handle(new CreateCommand
            {
                OutputDirectoryName = outputDirectoryName,
                ProjectName = projectName
            });

            string projectRootPath = mockFileSystem.Path.Combine(
                Environment.CurrentDirectory,
                projectName);

            string projectOutputPath = mockFileSystem.Path.Combine(
                projectRootPath,
                outputDirectoryName);

            string projectContentPath = mockFileSystem.Path.Combine(
                projectRootPath,
                ReservedDirectories.ContentDirectoryName);

            Assert.Multiple(() =>
            {
                Assert.True(mockFileSystem.Directory.Exists(projectRootPath));
                Assert.True(mockFileSystem.Directory.Exists(projectOutputPath));
                Assert.True(mockFileSystem.Directory.Exists(projectContentPath));
            });
        }

        [Test]
        public void CreateCommandHandlerOnSucessHasValidSettingsFile()
        {
            var mockReservedDirectories = new Mock<IReservedDirectories>();

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(Environment.CurrentDirectory);

            var handler = new CreateCommandHandler(
                _mockTerminal.Object,
                mockFileSystem,
                mockReservedDirectories.Object);

            const string projectName = "MyUnitTestProject";

            handler.Handle(new CreateCommand
            {
                OutputDirectoryName = "public",
                ProjectName = projectName
            });

            string projectRootPath = mockFileSystem.Path.Combine(
                Environment.CurrentDirectory,
                projectName);

            Assert.True(mockFileSystem.File.Exists(mockFileSystem.Path.Combine(
                projectRootPath,
                "appsettings.json")));
        }

        [Test]
        public void CreateCommandHandlerFailsOnNullCommand()
        {
            var mockReservedDirectories = new Mock<IReservedDirectories>();
            var mockFileSystem = new MockFileSystem();

            var handler = new CreateCommandHandler(
                _mockTerminal.Object,
                mockFileSystem,
                mockReservedDirectories.Object);

            Assert.Throws<ArgumentException>(() => handler.Handle(null));
        }

        [TestCase(null, null)]
        [TestCase(null, "public")]
        [TestCase("\0", "public")]
        [TestCase("MyUnitTestProject", "\0")]
        [TestCase("\0", "\0")]
        public void CreateCommandHandlerFailsOnInvalidCommand(
            string projectName,
            string outputDirectoryName)
        {
            var mockReservedDirectories = new Mock<IReservedDirectories>();
            var mockFileSystem = new MockFileSystem();

            var handler = new CreateCommandHandler(
                _mockTerminal.Object,
                mockFileSystem,
                mockReservedDirectories.Object);

            var command = new CreateCommand
            {
                ProjectName = projectName,
                OutputDirectoryName = outputDirectoryName
            };

            var exception = Assert.Throws<ArgumentException>(() => handler.Handle(command));
            StringAssert.StartsWith("Invalid path character(s)", exception.Message);
        }

        [Test]
        public void CreateCommandHandlerFailsIfProjectAlreadyExists()
        {
            var mockReservedDirectories = new Mock<IReservedDirectories>();

            var mockFileSystem = new MockFileSystem();

            const string projectName = "MyUnitTestProject";
            const string outputDirectoryName = "public";

            string projectRootPath = mockFileSystem.Path.Combine(
                Environment.CurrentDirectory,
                projectName);

            string projectOutputPath = mockFileSystem.Path.Combine(
                projectRootPath,
                outputDirectoryName);

            string projectContentPath = mockFileSystem.Path.Combine(
                projectRootPath,
                ReservedDirectories.ContentDirectoryName);

            // Simulate the `Create` command already having successfully run by adding
            // the generated folders to the file system mock.
            mockFileSystem.AddDirectory(projectOutputPath);
            mockFileSystem.AddDirectory(projectContentPath);

            var handler = new CreateCommandHandler(
                _mockTerminal.Object,
                mockFileSystem,
                mockReservedDirectories.Object);

            var exception = Assert.Throws<ArgumentException>(
                () => handler.Handle(new CreateCommand
                {
                    OutputDirectoryName = outputDirectoryName,
                    ProjectName = projectName
                }));

            StringAssert.StartsWith(
                "A Sculptor project already exists in the provided directory",
                exception.Message);
        }

        [Test]
        public void CreateCommandHandlerFailsIfOutputDirectoryMatchesAnyReservedName()
        {
            var mockReservedDirectories = new Mock<IReservedDirectories>();
            mockReservedDirectories
                .Setup(x => x.IsDirectoryNameReserved(It.IsAny<string>()))
                .Returns(true);

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(Environment.CurrentDirectory);

            var handler = new CreateCommandHandler(
                _mockTerminal.Object,
                mockFileSystem,
                mockReservedDirectories.Object);

            var exception = Assert.Throws<ArgumentException>(
                () => handler.Handle(new CreateCommand
                {
                    // It doesn't matter what this is set to as we have stubbed the
                    // `IsDirectoryNameReserved` method above to always return true.
                    OutputDirectoryName = "reserved-for-internal-use",
                    ProjectName = "MyUnitTestProject"
                }));

            StringAssert.EndsWith(
                "is reserved for internal use by Sculptor",
                exception.Message);
        }

        [Test]
        public void CreateCommandHandlerOnSuccessInformsUserOfActionsPerformed()
        {
            var mockReservedDirectories = new Mock<IReservedDirectories>();

            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(Environment.CurrentDirectory);

            var handler = new CreateCommandHandler(
                _mockTerminal.Object,
                mockFileSystem,
                mockReservedDirectories.Object);

            handler.Handle(new CreateCommand
            {
                OutputDirectoryName = "public",
                ProjectName = "MyUnitTestProject"
            });

            _mockTerminal.Verify(mock => mock.RenderText(It.IsAny<string>()), Times.Exactly(3));
        }
    }
}
