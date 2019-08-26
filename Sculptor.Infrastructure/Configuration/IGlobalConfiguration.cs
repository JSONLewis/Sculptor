namespace Sculptor.Infrastructure.Configuration
{
    /// <summary>
    /// Provides strongly typed access to the expected global settings that apply to every
    /// Sculptor project instance a user creates.
    /// </summary>
    public interface IGlobalConfiguration
    {
        string LogDirectoryPath { get; }
    }
}