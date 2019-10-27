using Sculptor.Infrastructure;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Text;

namespace Sculptor.Core.Domain.Create
{
    public class ConfigComposer : IConfigComposer
    {
        private readonly IFileSystem _fileSystem;

        public ConfigComposer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public void Compose(
            in string projectName,
            in string projectRootPath,
            in string outputPath)
        {
            string templateFilePath = FilePathHelper.GetTemplatePath(
                FilePathHelper.LocalConfigTemplateFileName);

            // TODO: abstract file search and updates into a single routine
            // (and make it more efficient).
            string templateFileContent = _fileSystem.File.ReadAllText(templateFilePath);

            templateFileContent = templateFileContent.Replace(
                "<<OUTPUT_PATH>>",
                FilePathHelper.BuildPlatformIndependentPath(outputPath),
                StringComparison.Ordinal);

            templateFileContent = templateFileContent.Replace(
                "<<PROJECT_NAME>>",
                projectName,
                StringComparison.Ordinal);

            string localLogDirectory = _fileSystem.Path.Combine(
                    FilePathHelper.ExecutingDirectory,
                    projectName,
                    "log");

            _fileSystem.Directory.CreateDirectory(localLogDirectory);

            string localLogFile = _fileSystem.Path.Combine(
                    localLogDirectory,
                    "web-{Date}.log");

            templateFileContent = templateFileContent.Replace(
                "<<LOCAL_PATH_FORMAT>>",
                FilePathHelper.BuildPlatformIndependentPath(localLogFile),
                StringComparison.Ordinal);

            string configFilePath = _fileSystem.Path.Combine(
                projectRootPath,
                FilePathHelper.LocalConfigFileName);

            byte[] configFileBytes = Encoding.UTF8.GetBytes(templateFileContent);

            using (var stream = _fileSystem.FileStream.Create(configFilePath, FileMode.Create))
            using (var memoryStream = new MemoryStream(configFileBytes))
            {
                memoryStream.CopyTo(stream);
                stream.Flush();
                stream.Close();
            }
        }
    }
}