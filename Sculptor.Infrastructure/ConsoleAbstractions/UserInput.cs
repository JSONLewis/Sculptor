using System;
using System.Collections.Generic;
using CommandLine;

namespace Sculptor.Infrastructure.ConsoleAbstractions
{
    public class UserInput : IUserInput
    {
        public UserInput()
        {
            SubmittedOn = DateTime.UtcNow;
        }

        public string Raw { get { return string.Join(' ', Arguments); } }

        public ICollection<string> Arguments { get; set; }

        public DateTime SubmittedOn { get; }

        public ParserResult<object> ParsedCommand { get; set; }
    }
}