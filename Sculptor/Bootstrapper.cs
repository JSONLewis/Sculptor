using FluentValidation;
using Microsoft.Extensions.Configuration;
using Sculptor.Core;
using Sculptor.Core.Domain;
using Sculptor.Infrastructure;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.ValidationFormatters;
using Serilog;
using SimpleInjector;
using System;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using System.Text;

namespace Sculptor
{
    internal static class Bootstrapper
    {
        private static readonly Container _container;

        static Bootstrapper()
        {
            _container = new Container();
        }

        public static void InitialiseApplication()
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            _container.Register(typeof(IValidator<>), assemblies);

            _container.RegisterSingleton(BuildConfiguration);
            _container.RegisterSingleton(InitialiseLogger);

            _container.Register(
                typeof(ICommandProcessor),
                typeof(CommandProcessor),
                Lifestyle.Singleton);

            _container.Register<IRegisteredVerbs, RegisteredVerbs>(Lifestyle.Singleton);

            _container.Register<IReservedDirectories, ReservedDirectories>(
                Lifestyle.Singleton);

            _container.Register<ITerminal, Terminal>(Lifestyle.Singleton);
            _container.Register<ICommandParser, CommandParser>(Lifestyle.Singleton);
            _container.Register<IUserInput, UserInput>(Lifestyle.Singleton);

            _container.Register<IApplication, Application>(Lifestyle.Singleton);
            _container.Register<IFileSystem, FileSystem>(Lifestyle.Singleton);

            _container.Register<ITemplateBuilder, TemplateBuilder>(Lifestyle.Singleton);
            _container.Register<IFluentValidationFormatter, FluentValidationFormatter>(
                Lifestyle.Singleton);

            _container.Register(typeof(ICommandHandler<>), assemblies);

            // Note: that the order of registration is significant for decorators as each
            // successive rule for a given type is wrapped by the previous.
            _container.RegisterDecorator(typeof(ICommandHandler<>),
                typeof(ValidationCommandHandlerDecorator<>));

            _container.Verify();
        }

        /// <summary>
        /// Acts as a Service Locator for use by the entry point (Program.cs) as this
        /// cannot make use of the container for injecting its dependencies.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <returns></returns>
        public static TInstance GetInstance<TInstance>() where TInstance : class
        {
            return _container.GetInstance<TInstance>();
        }

        #region Helpers

        /// <summary>
        /// Creates and verifies that we have access to a configuration instance
        /// implemented by <see cref="IConfiguration"/>. This abstracts from the JSON
        /// settings file we store on disk in
        /// <see cref="Environment.SpecialFolder.LocalApplicationData"/>.
        /// </summary>
        /// <returns></returns>
        private static IConfiguration BuildConfiguration()
        {
            // TODO: handle this directory already existing - but from another tool or
            // component etc. Offer the user the ability to set their own value for this?
            const string configDirectoryName = "sculptor-cli";

            string rootConfigPath = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData), configDirectoryName);

            const string configFileName = "config.json";
            string configFilePath = Path.Combine(rootConfigPath, configFileName);

            // TODO: refactor this entire block into something a little nicer.
            if (!File.Exists(configFilePath))
            {
                Directory.CreateDirectory(rootConfigPath);

                using (var file = File.Create(configFilePath))
                {
                    string defaultConfigFilePath = Path.Combine(
                        Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                        "appsettings.json");

                    string fileText = File.ReadAllText(
                        defaultConfigFilePath,
                        Encoding.UTF8);

                    string defaultLogPath = Path.Combine(rootConfigPath, "log");

                    Directory.CreateDirectory(defaultLogPath);

                    string processedText = fileText.Replace(
                        "<<PATH_FORMAT>>",
                        BuildPlatformIndependentPath(
                            Path.Combine(defaultLogPath, "{Date}-sculptor-cli.log")));

                    byte[] configFileBytes = Encoding.UTF8.GetBytes(processedText);
                    file.Write(configFileBytes);
                }
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(rootConfigPath)
                .AddJsonFile(configFileName, optional: false, reloadOnChange: false);

            return builder.Build();
        }

        /// <summary>
        /// Using the <see cref="IConfiguration"/> instance provided by the previous call
        /// to <see cref="BuildConfiguration"/> we can now instantiate the logger using
        /// the settings stored in the JSON file. These settings can be edited by the end
        /// user so long as they conform to the <see cref="Serilog"/> docs.
        /// </summary>
        /// <returns></returns>
        private static ILogger InitialiseLogger()
        {
            return Log.Logger = new LoggerConfiguration()
                .ReadFrom
                .Configuration(GetInstance<IConfiguration>())
                //#if DEBUG
                // TODO: code as shown below doens't seem to be working in the way I
                // expect. Something is being done wrong and needs investigation.
                //.MinimumLevel.Debug()
                //.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Verbose)
                //.MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Verbose)
                //#endif
                .CreateLogger();
        }

        /// <summary>
        /// Any path written to a config file needs to be written in a way that works on
        /// Windows, Linux, and OSX. So that we're consistent and don't have to worry
        /// about escaping the directory separator this method enforces "/" as the
        /// separator on all platforms.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string BuildPlatformIndependentPath(string path)
        {
            return path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        #endregion Helpers
    }
}