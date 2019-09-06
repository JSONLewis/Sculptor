using System.Collections.Generic;

namespace Sculptor.Infrastructure.ConsoleAbstractions
{
    public interface IUserInput
    {
        string Raw { get; }

        ICollection<string> Arguments { get; set; }
    }
}