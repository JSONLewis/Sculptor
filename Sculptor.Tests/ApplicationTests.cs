using Moq;
using NUnit.Framework;
using Sculptor.Infrastructure;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Infrastructure.Exceptions;
using Sculptor.Parsing;

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

            var mockCommandParser = new Mock<ICommandParser>();
            mockCommandParser.Setup(x => x.Parse(userInput)).Verifiable();

            var mockExceptionHandler = new Mock<IExceptionHandler>();

            var application = new Application(
                mockCommandParser.Object,
                mockCommandProcessor.Object,
                mockExceptionHandler.Object);

            application.Run(userInput);

            mockCommandParser.VerifyAll();
        }

        // TODO: verify that an unknown / badly formatted command is treated as an error.
    }
}