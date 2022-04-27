using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0010;
using R5T.T0001;

using R5T.Lombardy;
using R5T.Magyar.Extensions;
using R5T.Magyar.IO;

using R5T.T0064;

using R5T.Kefalonia.Common;


namespace R5T.Kefalonia.Construction
{
    [ServiceImplementationMarker]
    public class MessageRepository : IMessageRepository, IServiceImplementation
    {
        private IMessageFormatter MessageFormatter { get; }

        private string MessagesOutputFilePath { get; }

        private List<Message> InMemorySink { get; } = new List<Message>();


        public MessageRepository(
            IMessageFormatter messageFormatter,
            IStringlyTypedPathOperator stringlyTypedPathOperator,
            [NotServiceComponent] string messagesOutputFilePath)
        {
            this.MessageFormatter = messageFormatter;

            // Create the directory if it does not exist.
            var messagesOutputDirectoryPath = stringlyTypedPathOperator.GetDirectoryPathForFilePath(messagesOutputFilePath);

            DirectoryHelper.CreateDirectoryOkIfExists(messagesOutputDirectoryPath);

            // Delete the output file path, if it exists.
            FileHelper.DeleteOnlyIfExists(messagesOutputFilePath);

            this.MessagesOutputFilePath = messagesOutputFilePath;
        }

        public async Task AddAsync(Message message)
        {
            var formattedMessage = await this.MessageFormatter.FormatAsync(message);

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

        public Task<IEnumerable<Message>> GetAsync(Func<Message, bool> predicate)
        {
            var messages = this.InMemorySink
                .Where(predicate);

            return Task.FromResult(messages);
        }
    }
}
