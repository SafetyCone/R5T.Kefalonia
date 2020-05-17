using System;
using System.Threading.Tasks;

using R5T.D0010;
using R5T.Gloucester.Types;


namespace R5T.Kefalonia.XElements
{
    public interface IVisualStudioProjectFileToXElementConverter
    {
        Task<ProjectXElement> ToProjectXElement(ProjectFile projectFile, IMessageSink messageSink);
        Task<ProjectFile> ToProjectFile(ProjectXElement projectXElement, IMessageSink messageSink);
    }
}
