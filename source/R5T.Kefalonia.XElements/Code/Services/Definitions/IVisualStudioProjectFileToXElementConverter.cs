using System;

using R5T.Gloucester.Types;
using R5T.Magyar;


namespace R5T.Kefalonia.XElements
{
    public interface IVisualStudioProjectFileToXElementConverter
    {
        ProjectXElement ToProjectXElement(ProjectFile projectFile);
        Result<ProjectFile> ToProjectFile(ProjectXElement projectXElement);
    }
}
