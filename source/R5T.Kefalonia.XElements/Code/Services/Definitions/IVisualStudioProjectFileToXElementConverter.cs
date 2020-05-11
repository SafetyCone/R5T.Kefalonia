using System;
using System.Xml.Linq;

using R5T.Gloucester.Types;


namespace R5T.Kefalonia.XElements
{
    public interface IVisualStudioProjectFileToXElementConverter
    {
        ProjectXElement ToProjectXElement(ProjectFile projectFile);
        ProjectFile ToProjectFile(ProjectXElement projectXElement);
    }
}
