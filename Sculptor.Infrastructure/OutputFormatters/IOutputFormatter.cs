using CommandLine.Text;

namespace Sculptor.Infrastructure.OutputFormatters
{
    /// <summary>
    /// Provides a way for styling all terminal output in the same format as used by the
    /// <see cref="CommandLine"/> library.
    /// </summary>
    public interface IOutputFormatter
    {
        /// <summary>
        /// Cached instance of the <see cref="HelpText"/> value returned when parsing the
        /// current user input.
        /// </summary>
        HelpText HelpText { set; }

        /// <summary>
        /// Creates a string consistent with the output from the <see cref="CommandLine"/>
        /// library with the value provided in <paramref name="message"/>.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        string FormatMessageForOutput(string message);
    }
}