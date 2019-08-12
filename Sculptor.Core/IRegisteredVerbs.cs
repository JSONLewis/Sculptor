using System;
using System.Collections.Generic;

namespace Sculptor.Core
{
    public interface IRegisteredVerbs
    {
        IEnumerable<Type> KnownVerbs { get; }
    }
}