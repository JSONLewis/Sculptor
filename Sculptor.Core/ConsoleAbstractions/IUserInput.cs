﻿using System;
using System.Collections.Generic;

namespace Sculptor.Core.ConsoleAbstractions
{
    public interface IUserInput
    {
        string Raw { get; }

        ICollection<string> Arguments { get; set; }

        DateTime SubmittedOn { get; }
    }
}