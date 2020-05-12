using System;
using System.Collections.Generic;


namespace R5T.Kefalonia.Common
{
    public class MessageAggregator : IMessageAggregator
    {
        private List<string> Messages { get; } = new List<string>();


        public void AddMessage(string message)
        {
            this.Messages.Add(message);
        }

        public IEnumerable<string> GetMessages()
        {
            return this.Messages;
        }
    }
}
