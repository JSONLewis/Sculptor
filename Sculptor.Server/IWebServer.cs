namespace Sculptor.Server
{
    public interface IWebServer
    {
        void Run(string websiteRootPath, int port);
    }
}