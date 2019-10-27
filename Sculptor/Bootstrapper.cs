using FluentValidation;
using Sculptor.Core;
using Sculptor.Core.CrossCuttingConcerns;
using Sculptor.Core.Domain;
using Sculptor.Core.Domain.Create;
using Sculptor.Infrastructure;
using Sculptor.Infrastructure.Configuration;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Infrastructure.Exceptions;
using Sculptor.Infrastructure.Logging;
using Sculptor.Infrastructure.Markers;
using Sculptor.Infrastructure.OutputFormatters;
using Sculptor.Parsing;
using Sculptor.Server;
using SimpleInjector;
using System;
using System.IO.Abstractions;
using System.Linq;

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
            var fileSystem = new FileSystem();
            BuildInfrastructure(fileSystem);

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            _container.Register(typeof(IValidator<>), assemblies);

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
            _container.RegisterSingleton<IFileSystem>(() => fileSystem);

            _container.Register<IExceptionHandler, ExceptionHandler>(Lifestyle.Singleton);
            _container.Register<IOutputFormatter, OutputFormatter>(Lifestyle.Singleton);

            _container.Register<IConfigComposer, ConfigComposer>(Lifestyle.Singleton);
            _container.Register<IContentComposer, ContentComposer>(Lifestyle.Singleton);

            _container.Register(typeof(ICommandHandler<>), assemblies);

            _container.Register<IWebServer, KestrelWebServer>();

            // Note: that the order of registration is significant for decorators as each
            // successive rule wraps the one before it.
            _container.RegisterDecorator(typeof(ICommandHandler<>),
                typeof(ValidationCommandHandlerDecorator<>),
                context => ShouldApplyValidationDecorator(context.ServiceType));

            _container.RegisterDecorator(typeof(ICommandHandler<>),
                typeof(AuditCommandHandlerDecorator<>));

            _container.RegisterDecorator(typeof(ICommandHandler<>),
                typeof(ExceptionCommandHandlerDecorator<>));

            _container.Verify();
        }

        /// <summary>
        /// Acts as a Service Locator for use by the entry point (Program.cs) as this
        /// cannot have its dependencies injected.
        /// </summary>
        /// <typeparam name="TInstance"></typeparam>
        /// <returns></returns>
        public static TInstance GetInstance<TInstance>() where TInstance : class
        {
            return _container.GetInstance<TInstance>();
        }

        #region Helpers

        /// <summary>
        /// Registers the co-dependent configuration and logging dependencies.
        /// </summary>
        private static void BuildInfrastructure(FileSystem fileSystem)
        {
            var configurationResolver = new ConfigurationResolver(fileSystem);

            var globalConfig = configurationResolver
                .BuildConfiguration<GlobalConfiguration>();

            _container.RegisterSingleton<IGlobalConfiguration>(
                () => new GlobalConfiguration(globalConfig));

            _container.RegisterSingleton<IGlobalLogger>(
               () => new GlobalLogger(globalConfig.AddLogging()));

            var localConfig = configurationResolver
            .BuildConfiguration<LocalConfiguration>();

            _container.RegisterSingleton<ILocalConfiguration>(
                () => new LocalConfiguration(localConfig));

            _container.RegisterSingleton<ILocalLogger>(
            () => new LocalLogger(localConfig.AddLogging()));
        }

        /// <summary>
        /// For a given type that implements <see cref="ICommandHandler{TCommand}"/>
        /// check to see if its TCommand has a marker interface of
        /// <see cref="IExcludeFromValidation"/>.
        /// </summary>
        /// <param name="serviceType">The concrete command handler to verify.</param>
        /// <returns>
        /// Boolean: where true signifies that this service type will be decorated by
        /// <see cref="ValidationCommandHandlerDecorator{TCommand}"/>.
        /// </returns>
        private static bool ShouldApplyValidationDecorator(Type serviceType)
        {
            bool preventDecoration = serviceType
                .GenericTypeArguments[0]
                .GetInterfaces()
                .Any(type => type == typeof(IExcludeFromValidation));

            return !preventDecoration;
        }

        #endregion Helpers
    }
}