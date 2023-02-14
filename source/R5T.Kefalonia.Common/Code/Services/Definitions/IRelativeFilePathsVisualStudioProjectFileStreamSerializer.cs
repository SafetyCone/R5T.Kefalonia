using System;
using System.IO;
using System.Threading.Tasks;

using R5T.D0010;
using R5T.Gloucester.Types;using R5T.T0064;


namespace R5T.Kefalonia.Common
{
    /// <summary>
    /// Produces a Visual Studio project file result, where the project file result still has relative project file paths.
    /// </summary>
    [ServiceDefinitionMarker]
    public interface IRelativeFilePathsVisualStudioProjectFileStreamSerializer:IServiceDefinition
    {
        Task<ProjectFile> Deserialize(Stream stream, IMessageSink messageSink);
        Task Serialize(Stream stream, ProjectFile value, IMessageSink messageSink);
    }
}
