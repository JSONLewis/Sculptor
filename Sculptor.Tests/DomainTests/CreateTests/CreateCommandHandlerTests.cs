using System;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using NUnit.Framework;
using Sculptor.Core;
using Sculptor.Core.Domain.Create;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Serilog;

namespace Sculptor.Tests.DomainTests.CreateTests
{
    public class CreateCommandHandlerTests
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
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(Environment.CurrentDirectory);

            var mockLogger = new Mock<ILogger>();

            var handler = new CreateCommandHandler(
                _mockTerminal.Object,
                mockFileSystem,
                mockLogger.Object);

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
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(Environment.CurrentDirectory);

            var mockLogger = new Mock<ILogger>();

            var handler = new CreateCommandHandler(
                _mockTerminal.Object,
                mockFileSystem,
                mockLogger.Object);

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
        public void CreateCommandHandlerOnSuccessInformsUserOfActionsPerformed()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(Environment.CurrentDirectory);

            var mockLogger = new Mock<ILogger>();

            var handler = new CreateCommandHandler(
                _mockTerminal.Object,
                mockFileSystem,
                mockLogger.Object);

            handler.Handle(new CreateCommand
            {
                OutputDirectoryName = "public",
                ProjectName = "MyUnitTestProject"
            });

            _mockTerminal.Verify(mock => mock.RenderText(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}
