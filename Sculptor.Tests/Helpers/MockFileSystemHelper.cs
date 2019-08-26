using System.IO.Abstractions.TestingHelpers;
using Sculptor.Infrastructure;

namespace Sculptor.Tests.Helpers
{
    /// <summary>
    /// Provides named access to specific versions of test data that can be consumed by
    /// any unit test.
    /// </summary>
    internal static class MockFileSystemHelper
    {
        internal static (string, MockFileData) GetGlobalTemplateMock()
        {
            return (
                FilePathHelper.GetTemplatePath(FilePathHelper.GlobalConfigTemplateFileName),
                new MockFileData("{'Serilog':{'MinimumLevel':{'Default':'Information','Override':{'Microsoft':'Warning','System':'Warning'}},'WriteTo':[{'Name':'RollingFile','Args':{'pathFormat':'<<PATH_FORMAT>>','outputTemplate':'[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}','fileSizeLimitBytes':2147483648,'retainedFileCountLimit':5}}]}}"));
        }

        internal static (string, MockFileData) GetLocalTemplateMock()
        {
            return (
                FilePathHelper.GetTemplatePath(FilePathHelper.LocalConfigTemplateFileName),
                new MockFileData("{'Sculptor':{'Configuration':{'OutputPath':'<<OUTPUT_PATH>>','ProjectName':'<<PROJECT_NAME>>'}},'Serilog':{'MinimumLevel':'Verbose','WriteTo':[{'Name':'RollingFile','Args':{'pathFormat':'<<LOCAL_PATH_FORMAT>>','outputTemplate':'[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}','fileSizeLimitBytes':2147483648,'retainedFileCountLimit':5}}]}}"));
        }
    }
}