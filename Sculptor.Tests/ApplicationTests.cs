using Moq;
using NUnit.Framework;
using Sculptor.Infrastructure;
using Sculptor.Infrastructure.Configuration;
using Sculptor.Infrastructure.ConsoleAbstractions;
using Sculptor.Infrastructure.Logging;
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

            var mockGlobalLogger = new Mock<IGlobalLogger>();
            var mockGlobalConfiguration = new Mock<IGlobalConfiguration>();
            var mockTerminal = new Mock<ITerminal>();

            var application = new Application(
                mockCommandParser.Object,
                mockCommandProcessor.Object,
                mockGlobalLogger.Object,
                mockGlobalConfiguration.Object,
                mockTerminal.Object);

            application.Run(userInput);

            mockCommandParser.VerifyAll();
        }

        // TODO: verify that an unknown / badly formatted command is treated as an error.
    }
}