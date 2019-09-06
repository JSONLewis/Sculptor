using System.Collections.Generic;

namespace Sculptor.Infrastructure.ConsoleAbstractions
{
    public sealed class UserInput : IUserInput
    {
        public string Raw { get { return string.Join(' ', Arguments); } }

        public ICollection<string> Arguments { get; set; }
    }
}