using System;
using System.IO;
using System.Threading.Tasks;

using R5T.D0001;
using R5T.D0010;
using R5T.Gloucester.Types;
using R5T.Lombardy;

using R5T.Kefalonia.Common;


namespace R5T.Kefalonia.Construction
{
    class VisualStudioProjectFileSerializer : IVisualStudioProjectFileSerializer
    {
        private IFunctionalVisualStudioProjectFileSerializer FunctionalVisualStudioProjectFileSerializer { get; }
        private IMessageFormatter MessageFormatter { get; }
        private IMessageSinkProvider MessageSinkProvider { get; }
        private INowUtcProvider NowUtcProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }
        private IVisualStudioProjectFileSerializerMessagesOutputFilePathProvider VisualStudioProjectFileSerializerMessagesOutputFilePathProvider { get; }


        public VisualStudioProjectFileSerializer(
            IFunctionalVisualStudioProjectFileSerializer functionalVisualStudioProjectFileSerializer,
            IMessageFormatter messageFormatter,
            IMessageSinkProvider messageSinkProvider,
            INowUtcProvider nowUtcProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator,
            IVisualStudioProjectFileSerializerMessagesOutputFilePathProvider messagesOutputFilePathProvider)
        {
            this.FunctionalVisualStudioProjectFileSerializer = functionalVisualStudioProjectFileSerializer;
            this.MessageFormatter = messageFormatter;
            this.MessageSinkProvider = messageSinkProvider;
            this.NowUtcProvider = nowUtcProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
            this.VisualStudioProjectFileSerializerMessagesOutputFilePathProvider = messagesOutputFilePathProvider;
        }

        public async Task<ProjectFile> DeserializeAsync(string projectFilePath)
        {
            var messageSink = await this.MessageSinkProvider.GetMessageSinkAsync();

            var messagesOutputFilePath = await this.VisualStudioProjectFileSerializerMessagesOutputFilePathProvider.GetVisualStudioProjectFileSerializerMessagesOutputFilePathAsync(
                Constants.ProjectFileDeserializationFunctionalityName,
                projectFilePath);

            var fileFormattedMessageSink = new FileFormattedMessageSink(this.StringlyTypedPathOperator, messagesOutputFilePath);

            var messageRepository = new CompositeMessageSink(this.MessageFormatter, new[] { messageSink }, new[] { fileFormattedMessageSink });

            await messageRepository.AddOutputMessageAsync(this.NowUtcProvider, $"Deserialization of:\n{projectFilePath}");

            var projectFile = await this.FunctionalVisualStudioProjectFileSerializer.DeserializeAsync(projectFilePath, messageRepository);
            return projectFile;
        }

        public async Task SerializeAsync(string projectFilePath, ProjectFile value, bool overwrite = true)
        {
            var messageSink = await this.MessageSinkProvider.GetMessageSinkAsync();

            var messagesOutputFilePath = await this.VisualStudioProjectFileSerializerMessagesOutputFilePathProvider.GetVisualStudioProjectFileSerializerMessagesOutputFilePathAsync(
                Constants.ProjectFileSerializationFunctionalityName,
                projectFilePath);

            var fileFormattedMessageSink = new FileFormattedMessageSink(this.StringlyTypedPathOperator, messagesOutputFilePath);

            var messageRepository = new CompositeMessageSink(this.MessageFormatter, new[] { messageSink }, new[] { fileFormattedMessageSink });

            await messageRepository.AddOutputMessageAsync(this.NowUtcProvider, $"Serialization of:\n{projectFilePath}");

            await this.FunctionalVisualStudioProjectFileSerializer.SerializeAsync(projectFilePath, value, messageRepository, overwrite);
        }
    }
}
