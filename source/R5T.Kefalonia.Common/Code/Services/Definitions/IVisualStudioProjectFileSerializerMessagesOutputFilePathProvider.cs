using System;
using System.Threading.Tasks;using R5T.T0064;


namespace R5T.Kefalonia.Common
{[ServiceDefinitionMarker]
    public interface IVisualStudioProjectFileSerializerMessagesOutputFilePathProvider:IServiceDefinition
    {
        Task<string> GetVisualStudioProjectFileSerializerMessagesOutputFilePathAsync(string functionalityName, string projectFilePath);
    }
}
