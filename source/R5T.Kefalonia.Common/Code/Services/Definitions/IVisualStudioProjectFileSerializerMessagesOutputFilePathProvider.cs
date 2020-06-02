using System;
using System.Threading.Tasks;


namespace R5T.Kefalonia.Common
{
    public interface IVisualStudioProjectFileSerializerMessagesOutputFilePathProvider
    {
        Task<string> GetVisualStudioProjectFileSerializerMessagesOutputFilePathAsync(string functionalityName, string projectFilePath);
    }
}
