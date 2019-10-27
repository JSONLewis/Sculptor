using System.IO;
using System.Reflection;

namespace Sculptor.Infrastructure
{
    public static class FilePathHelper
    {
        public const string GlobalConfigFileName = "global-config.json";
        public const string GlobalConfigTemplateFileName = "global-config-template.json";

        public const string LocalConfigTemplateFileName = "local-config-template.json";
        public const string LocalConfigFileName = "local-config.json";

        public static string ExecutingDirectory = Path
            .GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string CurrentDirectory = Directory.GetCurrentDirectory();

        /// <summary>
        /// Any path written to a config file needs to be written in a way that works on
        /// Windows, Linux, and OSX. To ensure that we're consistent and don't have to
        /// worry about escaping the directory separator this method enforces "/" as the
        /// separator on all platforms.
        /// </summary>
        /// <param name="path">The path to be made platform independent.</param>
        public static string BuildPlatformIndependentPath(string path)
        {
            return path.Replace(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
        }

        /// <summary>
        /// Formats the provided file name as a path pointing to the template directory.
        /// </summary>
        /// <param name="fileName"></param>
        public static string GetTemplatePath(string fileName)
        {
            return Path.Combine(ExecutingDirectory, "Configuration", "Templates", fileName);
        }
    }
}