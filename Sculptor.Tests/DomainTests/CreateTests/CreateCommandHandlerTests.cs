using System;
using System.IO.Abstractions.TestingHelpers;
using Moq;
using NUnit.Framework;
using Sculptor.Core;
using Sculptor.Core.Domain.Create;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Tests.Helpers;

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

        #endregion Setup

        [Test]
        public void CreateCommandHandlerOnSuccessHasValidDirectoryStructure()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(Environment.CurrentDirectory);

            var globalTuple = MockFileSystemHelper.GetGlobalTemplateMock();
            mockFileSystem.AddFile(globalTuple.Item1, globalTuple.Item2);

            var localTuple = MockFileSystemHelper.GetLocalTemplateMock();
            mockFileSystem.AddFile(localTuple.Item1, localTuple.Item2);

            var configComposer = new ConfigComposer(mockFileSystem);
            var contentComposer = new ContentComposer(mockFileSystem);

            var handler = new CreateCommandHandler(
                configComposer,
                contentComposer,
                mockFileSystem,
                _mockTerminal.Object);

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
            // TODO: validate actual JSON content - not just that the file exists.
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(Environment.CurrentDirectory);

            var globalTuple = MockFileSystemHelper.GetGlobalTemplateMock();
            mockFileSystem.AddFile(globalTuple.Item1, globalTuple.Item2);

            var localTuple = MockFileSystemHelper.GetLocalTemplateMock();
            mockFileSystem.AddFile(localTuple.Item1, localTuple.Item2);

            var configComposer = new ConfigComposer(mockFileSystem);
            var contentComposer = new ContentComposer(mockFileSystem);

            var handler = new CreateCommandHandler(
                configComposer,
                contentComposer,
                mockFileSystem,
                _mockTerminal.Object);

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
                "local-config.json")));
        }

        [Test]
        public void CreateCommandHandlerOnSuccessInformsUserOfActionsPerformed()
        {
            var mockFileSystem = new MockFileSystem();
            mockFileSystem.AddDirectory(Environment.CurrentDirectory);

            var mockConfigComposer = new Mock<IConfigComposer>();
            var mockContentComposer = new Mock<IContentComposer>();

            var handler = new CreateCommandHandler(
                mockConfigComposer.Object,
                mockContentComposer.Object,
                mockFileSystem,
                _mockTerminal.Object);

            handler.Handle(new CreateCommand
            {
                OutputDirectoryName = "public",
                ProjectName = "MyUnitTestProject"
            });

            _mockTerminal.Verify(mock => mock.RenderText(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}