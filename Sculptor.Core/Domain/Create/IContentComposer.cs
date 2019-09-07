namespace Sculptor.Core.Domain.Create
{
    /// <summary>
    /// Responsible for setting up the Content directory and the initial markdown content
    /// within for a given new project.
    /// </summary>
    public interface IContentComposer
    {
        void Compose(in string projectRootPath);
    }
}