using System;
using System.Threading.Tasks;

using R5T.Gloucester.Types;
using R5T.D0010;


namespace R5T.Kefalonia.Common
{
    public interface IVisualStudioProjectFileValidator
    {
        Task<bool> Validate(ProjectFile projectFile, IMessageSink messageSink);
    }
}
