using System;
using System.Threading.Tasks;

using R5T.Gloucester.Types;
using R5T.D0010;using R5T.T0064;


namespace R5T.Kefalonia.Common
{[ServiceDefinitionMarker]
    public interface IVisualStudioProjectFileValidator:IServiceDefinition
    {
        Task<bool> Validate(ProjectFile projectFile, IMessageSink messageSink);
    }
}
