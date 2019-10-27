namespace Sculptor.Infrastructure.Markers
{
    /// <summary>
    /// Marker interface to indicate what concrete implementors should not have the
    /// validation decorators applied to them.
    /// </summary>
    public interface IExcludeFromValidation
    {
    }
}