using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using R5T.D0010;


namespace R5T.Kefalonia.Common
{
    public class ConsoleFormattedMessageSink : IFormattedMessageSink
    {
        public Task AddAsync(string formattedMessage)
        {
            Console.WriteLine(formattedMessage);

            return Task.CompletedTask;
        }
    }
}
