using System;
using System.Threading.Tasks;


namespace R5T.Kefalonia.Construction
{
    public interface IProjectFileDeserializationMessagesOutputFileNameProvider
    {
        Task<string> GetProjectFileDeserializationMessagesOutputFileNameAsync(string projectFilePath);
    }
}
