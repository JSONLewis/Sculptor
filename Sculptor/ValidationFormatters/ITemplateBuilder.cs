namespace Sculptor.ValidationFormatters
{
    internal interface ITemplateBuilder
    {
        /// <summary>
        /// Creates a string consistent with the output from the <see cref="CommandLine"/>
        /// library with the value provided in <paramref name="errorMessage"/>.
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        string BuildErrorTemplate(string errorMessage);
    }
}