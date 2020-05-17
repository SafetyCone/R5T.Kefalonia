using System;
using System.Threading.Tasks;

using R5T.D0010;
using R5T.Gloucester.Types;


namespace R5T.Kefalonia.Common
{
    /// <summary>
    /// Produces a Visual Studio project file result, where the project file result still has relative project file paths.
    /// </summary>
    public interface IRelativeFilePathsVisualStudioProjectFileSerializer
    {
        Task<ProjectFile> Deserialize(string projectFilePath, IMessageSink messageSink);
        Task Serialize(string filePath, ProjectFile value, IMessageSink messageSink, bool overwrite = true);
    }
}
