using System;

namespace Sculptor.Core.ConsoleAbstractions
{
    public sealed class Terminal : ITerminal
    {
        public void RenderText(string text)
        {
            Console.WriteLine(text);
        }
    }
}