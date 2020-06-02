using System;
using System.Threading.Tasks;

using R5T.T0001;


namespace R5T.Kefalonia.Common
{
    public interface IMessageFormatter
    {
        Task<string> FormatAsync(Message message);
    }
}
