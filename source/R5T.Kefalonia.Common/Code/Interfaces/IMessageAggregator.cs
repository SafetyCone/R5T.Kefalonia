using System;
using System.Collections.Generic;


namespace R5T.Kefalonia.Common
{
    public interface IMessageAggregator
    {
        void AddMessage(string message);
        IEnumerable<string> GetMessages();
    }
}
