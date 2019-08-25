using CommandLine.Text;

namespace Sculptor.Infrastructure.OutputFormatters
{
    public sealed class OutputFormatter : IOutputFormatter
    {
        public string FormatMessageForOutput(string message)
        {
            if (message is null)
                return string.Empty;

            return HelpText.AddPreOptionsLine(message).ToString();
        }

        public HelpText HelpText { private get; set; }
    }
}