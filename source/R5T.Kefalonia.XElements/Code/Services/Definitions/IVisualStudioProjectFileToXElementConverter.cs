using System;
using System.Threading.Tasks;

using R5T.D0010;
using R5T.Gloucester.Types;using R5T.T0064;


namespace R5T.Kefalonia.XElements
{[ServiceDefinitionMarker]
    public interface IVisualStudioProjectFileToXElementConverter:IServiceDefinition
    {
        Task<ProjectXElement> ToProjectXElement(ProjectFile projectFile, IMessageSink messageSink);
        Task<ProjectFile> ToProjectFile(ProjectXElement projectXElement, IMessageSink messageSink);
    }
}
