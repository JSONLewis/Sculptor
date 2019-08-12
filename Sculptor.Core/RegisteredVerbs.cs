using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sculptor.Core.Domain;

namespace Sculptor.Core
{
    public sealed class RegisteredVerbs : IRegisteredVerbs
    {
        public RegisteredVerbs()
        {
            var commandType = typeof(ICommand);

            KnownVerbs = from type in Assembly.GetExecutingAssembly().GetTypes()
                         where type.IsClass
                           && !type.IsAbstract
                           && typeof(ICommand).IsAssignableFrom(type)
                         select type;
        }

        public IEnumerable<Type> KnownVerbs { get; }
    }
}