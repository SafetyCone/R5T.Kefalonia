using System;
using System.Threading.Tasks;

using R5T.D0010;
using R5T.Gloucester.Types;


namespace R5T.Kefalonia.Common
{
    /// <summary>
    /// Performs all the functional aspects of Visual Studio project file serialization while leaving the handling of result messages to a more-presentation layer appropriate service.
    /// 1) Handles adjustment of project-reference paths from relative to absolution.
    /// 2) Validates the project file.
    /// 3) Performs post-deserialization behavior (including throwing if any errors are present and the deserialization settings say to throw if any errors are present).
    /// </summary>
    public interface IFunctionalVisualStudioProjectFileSerializer
    {
        Task<ProjectFile> DeserializeAsync(string projectFilePath, IMessageRepository messageRepository);
        Task SerializeAsync(string filePath, ProjectFile value, IMessageRepository messageRepository, bool overwrite = true);
    }
}
