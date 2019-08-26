﻿using System;
using System.IO.Abstractions;
using FluentValidation;
using Sculptor.Core;
using Sculptor.Core.Domain;
using Sculptor.Core.Domain.Create;
using Sculptor.Infrastructure;
using Sculptor.Infrastructure.Configuration;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Infrastructure.Exceptions;
using Sculptor.Infrastructure.Logging;
using Sculptor.Infrastructure.OutputFormatters;
using Sculptor.Parsing;
using SimpleInjector;

namespace Sculptor
{
    internal static class Bootstrapper
    {
        private static readonly Container _container;

        static Bootstrapper()
        {
            _container = new Container();
        }

        public static void InitialiseApplication(CommandScope commandScope)
        {
            var fileSystem = new FileSystem();
            BuildInfrastructure(fileSystem, commandScope);

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

            // Note: that the order of registration is significant for decorators as each
            // successive rule for a given type is wrapped by the previous.
            _container.RegisterDecorator(typeof(ICommandHandler<>),
                typeof(ValidationCommandHandlerDecorator<>));

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
        private static void BuildInfrastructure(FileSystem fileSystem, CommandScope commandType)
        {
            var configurationResolver = new ConfigurationResolver(fileSystem);

            var globalConfig = configurationResolver
                .BuildConfiguration<GlobalConfiguration>();

            _container.RegisterSingleton<IGlobalConfiguration>(
                () => new GlobalConfiguration(globalConfig));

            _container.RegisterSingleton<IGlobalLogger>(
               () => new GlobalLogger(globalConfig.AddLogging()));

            if (commandType == CommandScope.Local)
            {
                var localConfig = configurationResolver
                .BuildConfiguration<LocalConfiguration>();

                _container.RegisterSingleton<ILocalConfiguration>(
                    () => new LocalConfiguration(localConfig));

                _container.RegisterSingleton<ILocalLogger>(
                () => new LocalLogger(localConfig.AddLogging()));
            }
        }

        #endregion Helpers
    }
}