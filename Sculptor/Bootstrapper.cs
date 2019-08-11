using FluentValidation;
using Sculptor.Core;
using Sculptor.Core.Domain;
using Sculptor.Infrastructure;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.ValidationFormatters;
using SimpleInjector;
using System;
using System.IO.Abstractions;

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
            // Acts as a service locator for use in the entry point (Program.cs) which
            // cannot make use of the container for injecting its dependencies.
            return _container.GetInstance<TInstance>();
        }
    }
}