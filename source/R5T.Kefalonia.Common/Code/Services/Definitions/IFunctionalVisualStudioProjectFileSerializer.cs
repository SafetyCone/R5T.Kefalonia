using System;

using R5T.Gloucester.Types;
using R5T.Magyar;


namespace R5T.Kefalonia.Common
{
    /// <summary>
    /// Performs all the functional aspects of Visual Studio project file serialization while leaving the handling of result messages to a more-presentation layer appropriate service.
    /// Handles adjustment of project-reference paths from relative to absolution.
    /// </summary>
    public interface IFunctionalVisualStudioProjectFileSerializer
    {
        Result<ProjectFile> Deserialize(string projectFilePath);
        void Serialize(string filePath, ProjectFile value, bool overwrite = true);
    }
}
