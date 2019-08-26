namespace Sculptor.Infrastructure.Configuration
{
    /// <summary>
    /// Provides strongly typed access to the expected configuration values available for
    /// a given Sculptor project instance.
    /// </summary>
    public interface ILocalConfiguration
    {
        string ProjectName { get; }

        string OutputPath { get; }
    }
}