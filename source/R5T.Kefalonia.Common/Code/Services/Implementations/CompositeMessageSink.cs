using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0010;
using R5T.T0001;

using R5T.Lombardy;


namespace R5T.Kefalonia.Common
{
    /// <summary>
    /// * Manual construction recommended.
    /// </summary>
    public class CompositeMessageSink : IMessageSink
    {
        #region Static

        public static CompositeMessageSink NewDefault(IMessageFormatter messageFormatter, IStringlyTypedPathOperator stringlyTypedPathOperator, string messagesOutputFilePath)
        {
            var compositeMessageRepository = new CompositeMessageSink(
                messageFormatter,
                Enumerable.Empty<IMessageSink>(),
                new IFormattedMessageSink[]
                {
                    new ConsoleFormattedMessageSink(),
                    new FileFormattedMessageSink(stringlyTypedPathOperator, messagesOutputFilePath)
                });

            return compositeMessageRepository;
        }

        public static CompositeMessageSink NewStandard(IMessageFormatter messageFormatter, InMemoryMessageRepository inMemoryMessageRepository, IStringlyTypedPathOperator stringlyTypedPathOperator, string messagesOutputFilePath)
        {
            var compositeMessageRepository = new CompositeMessageSink(
                messageFormatter,
                new[]
                {
                    inMemoryMessageRepository,
                },
                new IFormattedMessageSink[]
                {
                    new ConsoleFormattedMessageSink(),
                    new FileFormattedMessageSink(stringlyTypedPathOperator, messagesOutputFilePath)
                });

            return compositeMessageRepository;
        }


        #endregion


        private IMessageFormatter MessageFormatter { get; }

        private List<IMessageSink> ComponentMessageSinks { get; } = new List<IMessageSink>();
        private List<IFormattedMessageSink> ComponentFormattedMessageSinks { get; } = new List<IFormattedMessageSink>();


        public CompositeMessageSink(IMessageFormatter messageFormatter, IEnumerable<IMessageSink> componentMessageSinks, IEnumerable<IFormattedMessageSink> componentFormattedMessageSinks)
        {
            this.MessageFormatter = messageFormatter;
            this.ComponentMessageSinks.AddRange(componentMessageSinks);
            this.ComponentFormattedMessageSinks.AddRange(componentFormattedMessageSinks);
        }

        public CompositeMessageSink(IMessageFormatter messageFormatter, IEnumerable<IMessageSink> componentMessageSinks)
            : this(messageFormatter, componentMessageSinks, Enumerable.Empty<IFormattedMessageSink>())
        {

        }

        public async Task AddAsync(Message message)
        {
            // Sequential (non-parallel).
            foreach (var messageSink in this.ComponentMessageSinks)
            {
                await messageSink.AddAsync(message);
            }

            var formattedMessage = await this.MessageFormatter.FormatAsync(message);
            foreach (var formattedMessageSink in this.ComponentFormattedMessageSinks)
            {
                await formattedMessageSink.AddAsync(formattedMessage);
            }
        }
    }
}
