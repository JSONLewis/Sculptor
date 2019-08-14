using Moq;
using NUnit.Framework;
using Sculptor.Core;
using Sculptor.Infrastructure;
using Sculptor.Infrastructure.ConsoleAbstractions;

namespace Sculptor.Tests
{
    public class ApplicationTests
    {
        [Test]
        public void ApplicationCallsCommandParserOnRun()
        {
            var userInput = new UserInput
            {
                Arguments = ArgumentHelper.SplitArguments("create -n \"MyFirstProject\"")
            };

            var mockCommandProcessor = new Mock<ICommandProcessor>();
            var mockRegisteredVerbs = new Mock<IRegisteredVerbs>();

            var mockCommandParser = new Mock<ICommandParser>();
            mockCommandParser.Setup(x => x.Parse(userInput)).Verifiable();

            var application = new Application(mockCommandParser.Object);
            application.Run(userInput);

            mockCommandParser.VerifyAll();
        }

        // TODO: verify that an unknown / badly formatted command is treated as an error.
    }
}