using Microsoft.Extensions.Configuration;
using System;
using System.IO.Abstractions;
using System.Text;

// TODO: wrap this in another layer that verifies the configuration we get back is valid.
namespace Sculptor.Infrastructure.Configuration
{
    public sealed class ConfigurationResolver : IConfigurationResolver
    {
        private readonly IFileSystem _fileSystem;

        public ConfigurationResolver(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IConfiguration BuildConfiguration<TConfig>()
        {
            var builder = new ConfigurationBuilder();

            string typeName = typeof(TConfig).Name;

            switch (typeName)
            {
                case nameof(GlobalConfiguration):
                    BuildGlobalConfig(builder);
                    break;

                case nameof(LocalConfiguration):
                    BuildLocalConfig(builder);
                    break;

                default:
                    throw new NotSupportedException($"[{nameof(ConfigurationResolver)}.{nameof(ConfigurationResolver.BuildConfiguration)}] encountered an unknown configuration type.");
            }

            return builder.Build();
        }

        private void BuildGlobalConfig(ConfigurationBuilder configurationBuilder)
        {
            // TODO: handle this directory already existing - but from another tool or
            // component etc. Offer the user the ability to set their own value for this?
            const string configDirectoryName = "sculptor-cli";

            string rootConfigPath = _fileSystem.Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData), configDirectoryName);

            string configFilePath = _fileSystem.Path.Combine(
                rootConfigPath,
                FilePathHelper.GlobalConfigFileName);

            _fileSystem.Directory.CreateDirectory(rootConfigPath);

            configurationBuilder
                .SetBasePath(rootConfigPath)
                .AddJsonFile(
                    FilePathHelper.GlobalConfigFileName,
                    optional: false,
                    reloadOnChange: false);

            if (_fileSystem.File.Exists(configFilePath))
                return;

            using (var file = _fileSystem.File.Create(configFilePath))
            {
                string globalTemplatePath = FilePathHelper
                    .GetTemplatePath(FilePathHelper.GlobalConfigTemplateFileName);

                string fileText = _fileSystem.File.ReadAllText(
                    globalTemplatePath,
                    Encoding.UTF8);

                string defaultLogPath = _fileSystem.Path.Combine(rootConfigPath, "log");

                _fileSystem.Directory.CreateDirectory(defaultLogPath);

                string processedText = fileText.Replace(
                    "<<PATH_FORMAT>>",
                    FilePathHelper.BuildPlatformIndependentPath(
                        _fileSystem.Path.Combine(
                            defaultLogPath,
                            "{Date}-sculptor-cli.log")));

                byte[] configFileBytes = Encoding.UTF8.GetBytes(processedText);
                file.Write(configFileBytes);
            }
        }

        private void BuildLocalConfig(ConfigurationBuilder configurationBuilder)
        {
            string projectRootPath = Environment.CurrentDirectory;

            configurationBuilder
                .SetBasePath(projectRootPath)
                .AddJsonFile(
                    FilePathHelper.LocalConfigFileName,
                    optional: false,
                    reloadOnChange: false);

            string localConfigFilePath = _fileSystem.Path.Combine(
                Environment.CurrentDirectory,
                FilePathHelper.LocalConfigFileName);

            if (_fileSystem.File.Exists(localConfigFilePath))
                return;

            // No existing configuration. Create the necessary default directory.
            _fileSystem.Directory.CreateDirectory(
                _fileSystem.Path.Combine(projectRootPath, "log"));
        }
    }
}