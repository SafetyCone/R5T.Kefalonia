using System;
using System.IO;
using System.Threading.Tasks;

using R5T.D0010;
using R5T.Gloucester.Types;using R5T.T0064;


namespace R5T.Kefalonia.Common
{
    /// <summary>
    /// Performs all the functional aspects of Visual Studio project file serialization while leaving the handling of result messages to a more-presentation layer appropriate service.
    /// 1) Handles adjustment of project-reference paths from relative to absolution.
    /// 2) Validates the project file.
    /// 3) Performs post-deserialization behavior (including throwing if any errors are present and the deserialization settings say to throw if any errors are present).
    /// </summary>
    [ServiceDefinitionMarker]
    public interface IFunctionalVisualStudioProjectFileStreamSerializer:IServiceDefinition
    {
        /// <summary>
        /// Includes both <paramref name="stream"/> and <paramref name="projectFilePath"/> since project reference relative file paths will need to be made absolute.
        /// </summary>
        Task<ProjectFile> DeserializeAsync(Stream stream, string projectFilePath, IMessageSink messageSink);

        /// <summary>
        /// Includes both <paramref name="stream"/> and <paramref name="projectFilePath"/> since project reference absolute file paths will need to be made relative.
        /// </summary>
        Task SerializeAsync(Stream stream, string projectFilePath, ProjectFile value, IMessageSink messageSink);
    }
}
