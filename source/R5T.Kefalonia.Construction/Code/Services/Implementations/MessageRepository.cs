using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0010;
using R5T.Magyar.Extensions;
using R5T.T0001;


namespace R5T.Kefalonia.Construction
{
    public class MessageRepository : IMessageRepository
    {
        #region Static

        public static string FormatMessage(Message message)
        {
            var formattedMessage = $"{message.TimestampUtc.ToYYYYMMDD_HHMMSS_FFF()} - {message.MessageType}:\n{message.Value}";
            return formattedMessage;
        }

        #endregion


        private string MessagesOutputFilePath { get; }

        private List<Message> InMemorySink { get; } = new List<Message>();


        public MessageRepository(string messagesOutputFilePath)
        {
            this.MessagesOutputFilePath = messagesOutputFilePath;
        }

        public async Task AddAsync(Message message)
        {
            var formattedMessage = MessageRepository.FormatMessage(message);

            // Add to console sink.
            Console.WriteLine(formattedMessage);

            // Add to messages output file path sink.
            using (var file = File.AppendText(this.MessagesOutputFilePath))
            {
                await file.WriteLineAsync(formattedMessage);
            }

            // Add to in-memory sink.
            this.InMemorySink.Add(message);
        }

        public Task ClearAsync(Func<Message, bool> predicate)
        {
            var messagesToClear = this.InMemorySink
                .Where(predicate);

            this.InMemorySink.RemoveAll(messagesToClear);

            return Task.CompletedTask;
        }

        public Task<IEnumerable<Message>> GetAllAsync(Func<Message, bool> predicate)
        {
            var messages = this.InMemorySink
                .Where(predicate);

            return Task.FromResult(messages);
        }
    }
}
