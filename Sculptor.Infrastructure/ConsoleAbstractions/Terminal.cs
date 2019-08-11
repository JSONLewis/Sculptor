using System;

namespace Sculptor.Infrastructure.ConsoleAbstractions
{
    public sealed class Terminal : ITerminal
    {
        public void RenderText(string text)
        {
            Console.WriteLine(text);
        }
    }
}