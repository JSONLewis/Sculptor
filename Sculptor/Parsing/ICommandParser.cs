using Sculptor.Core.Domain;
using Sculptor.Infrastructure.ConsoleAbstractions;

namespace Sculptor.Parsing
{
    public interface ICommandParser
    {
        /// <summary>
        /// Takes the values provided in <see cref="IUserInput.Arguments"/> and uses them
        /// to construct the matching implementation of <see cref="ICommand"/>.
        /// </summary>
        /// <param name="userInput"></param>
        /// <returns></returns>
        ICommand Parse(IUserInput userInput);
    }
}