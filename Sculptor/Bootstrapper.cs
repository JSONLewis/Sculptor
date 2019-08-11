using Sculptor.Core;
using Sculptor.Core.CommandHandlers;
using Sculptor.Core.ConsoleAbstractions;
using Sculptor.Infrastructure;
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

        public static IApplication InitialiseApplication()
        {
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

            _container.Register(typeof(ICommandHandler<>), AppDomain.CurrentDomain.GetAssemblies());

            _container.Verify();

            return _container.GetInstance<IApplication>();
        }
    }
}