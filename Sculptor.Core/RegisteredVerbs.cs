using Sculptor.Core.Commands;
using System;
using System.Linq;
using System.Reflection;

namespace Sculptor.Core
{
    public sealed class RegisteredVerbs : IRegisteredVerbs
    {
        public RegisteredVerbs()
        {
            var commandType = typeof(ICommand);

            string restrictedNamespace
                = $"{nameof(Sculptor)}.{nameof(Core)}.{nameof(Commands)}";

            var types = from type in Assembly.GetExecutingAssembly().GetTypes()
                        where type.IsClass
                          && type.Namespace == restrictedNamespace
                          && typeof(ICommand).IsAssignableFrom(type)
                        select type;

            KnownVerbs = types.ToArray();
        }

        public Type[] KnownVerbs { get; }
    }
}