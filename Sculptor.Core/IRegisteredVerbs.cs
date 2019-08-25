using System;

namespace Sculptor.Core
{
    public interface IRegisteredVerbs
    {
        Type[] KnownVerbs { get; }
    }
}