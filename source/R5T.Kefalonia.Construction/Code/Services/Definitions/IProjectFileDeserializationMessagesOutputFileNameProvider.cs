using System;
using System.Threading.Tasks;using R5T.T0064;


namespace R5T.Kefalonia.Construction
{[ServiceDefinitionMarker]
    public interface IProjectFileDeserializationMessagesOutputFileNameProvider:IServiceDefinition
    {
        Task<string> GetProjectFileDeserializationMessagesOutputFileNameAsync(string projectFilePath);
    }
}
