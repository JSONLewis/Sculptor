using System;
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

            string restrictedNamespace
                = $"{nameof(Sculptor)}.{nameof(Core)}.{nameof(Domain)}";

            var types = from type in Assembly.GetExecutingAssembly().GetTypes()
                        where type.IsClass
                          && type.Namespace.StartsWith(
                              restrictedNamespace,
                              StringComparison.Ordinal)
                          && typeof(ICommand).IsAssignableFrom(type)
                        select type;

            KnownVerbs = types.ToArray();
        }

        public Type[] KnownVerbs { get; }
    }
}