namespace Sculptor.Core.Domain.Create
{
    /// <summary>
    /// Responsible for writing the necessary config files to disk for a given new
    /// project.
    /// </summary>
    public interface IConfigComposer
    {
        void Compose(
            in string projectName,
            in string projectRootPath,
            in string outputPath);
    }
}