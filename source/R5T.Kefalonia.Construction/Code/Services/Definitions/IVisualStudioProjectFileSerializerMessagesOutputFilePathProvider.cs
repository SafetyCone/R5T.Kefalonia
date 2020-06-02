using System;
using System.Threading.Tasks;


namespace R5T.Kefalonia.Construction
{
    public interface IVisualStudioProjectFileSerializerMessagesOutputFilePathProvider
    {
        Task<string> GetVisualStudioProjectFileSerializerMessagesOutputFilePathAsync(string functionalityName, string projectFilePath);
    }
}
