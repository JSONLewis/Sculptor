﻿using Sculptor.Core.Domain;
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

            KnownVerbs = (from type in Assembly.GetExecutingAssembly().GetTypes()
                          where type.IsClass
                            && !type.IsAbstract
                            && typeof(ICommand).IsAssignableFrom(type)
                          select type).ToArray();
        }

        public Type[] KnownVerbs { get; }
    }
}