using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0001;
using R5T.D0010;
using R5T.D0010.Default;

using R5T.Gloucester.Types;
using R5T.Lombardy;using R5T.T0064;


namespace R5T.Kefalonia.Common
{[ServiceImplementationMarker]
    public class FunctionalVisualStudioProjectFileStreamSerializer : IFunctionalVisualStudioProjectFileStreamSerializer,IServiceImplementation
    {
        private IMessageFormatter MessageFormatter { get; }
        private INowUtcProvider NowUtcProvider { get; }
        private IRelativeFilePathsVisualStudioProjectFileStreamSerializer RelativeFilePathsVisualStudioProjectFileSerializer { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }
        private IVisualStudioProjectFileDeserializationSettings VisualStudioProjectFileDeserializationSettings { get; }
        private IVisualStudioProjectFileSerializerMessagesOutputFilePathProvider VisualStudioProjectFileSerializerMessagesOutputFilePathProvider { get; }
        private IVisualStudioProjectFileValidator VisualStudioProjectFileValidator { get; }


        public FunctionalVisualStudioProjectFileStreamSerializer(
            IMessageFormatter messageFormatter,
            INowUtcProvider nowUtcProvider,
            IRelativeFilePathsVisualStudioProjectFileStreamSerializer relativeFilePathsVisualStudioProjectFileSerializer,
            IStringlyTypedPathOperator stringlyTypedPathOperator,
            IVisualStudioProjectFileDeserializationSettings visualStudioProjectFileDeserializationSettings,
            IVisualStudioProjectFileSerializerMessagesOutputFilePathProvider visualStudioProjectFileSerializerMessagesOutputFilePathProvider,
            IVisualStudioProjectFileValidator visualStudioProjectFileValidator)
        {
            this.MessageFormatter = messageFormatter;
            this.NowUtcProvider = nowUtcProvider;
            this.RelativeFilePathsVisualStudioProjectFileSerializer = relativeFilePathsVisualStudioProjectFileSerializer;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
            this.VisualStudioProjectFileDeserializationSettings = visualStudioProjectFileDeserializationSettings;
            this.VisualStudioProjectFileSerializerMessagesOutputFilePathProvider = visualStudioProjectFileSerializerMessagesOutputFilePathProvider;
            this.VisualStudioProjectFileValidator = visualStudioProjectFileValidator;
        }

        public async Task<ProjectFile> DeserializeAsync(Stream stream, string projectFilePath, IMessageSink messageSink)
        {
            // Create a message repository that can be used to test if there were any errors.
            var messageRepository = new InMemoryMessageRepository();

            var compositeMessageSink = await this.CreateCompositeMessageSinkAsync(projectFilePath, messageSink, messageRepository, Constants.ProjectFileDeserializationFunctionalityName);

            // Test message output.
            await compositeMessageSink.AddOutputMessageAsync(this.NowUtcProvider, $"Deserialization of:\n{projectFilePath}");

            // Now deserialize.
            var projectFile = await this.RelativeFilePathsVisualStudioProjectFileSerializer.Deserialize(stream, compositeMessageSink);

            // Change all project reference paths to be absolute, not relative, using the input project file path.
            foreach (var projectReference in projectFile.ProjectReferences)
            {
                var projectReferenceAbsolutePath = this.StringlyTypedPathOperator.Combine(projectFilePath, projectReference.ProjectFilePath);

                projectReference.ProjectFilePath = projectReferenceAbsolutePath;
            }

            // Validate the project file.
            await this.ValidateProjectFileAsync(projectFile, compositeMessageSink);

            // If there are any error messages, and the deserializations settings say we should throw if there are any error messages, throw an exception.
            var errorMessages = await messageRepository.GetErrorsAsync();
            if (errorMessages.Any())
            {
                if (this.VisualStudioProjectFileDeserializationSettings.ThrowIfAnyErrorAtEnd)
                {
                    var messagesOutputFilePath = await this.GetMessagesOutputFilePathAsync(Constants.ProjectFileDeserializationFunctionalityName, projectFilePath);

                    throw new Exception($"There were deserialization errors. See:\n{messagesOutputFilePath}");
                }
            }

            return projectFile;
        }

        private async Task<string> GetMessagesOutputFilePathAsync(string functionalityName, string projectFilePath)
        {
            var messagesOutputFilePath = await this.VisualStudioProjectFileSerializerMessagesOutputFilePathProvider.GetVisualStudioProjectFileSerializerMessagesOutputFilePathAsync(
                functionalityName,
                projectFilePath);

            return messagesOutputFilePath;
        }

        private async Task<IMessageSink> CreateCompositeMessageSinkAsync(string projectFilePath, IMessageSink parentMessageSink, InMemoryMessageRepository inMemoryMessageRepository, string functionalityName)
        {
            // Now create the messages output file sink for this functionality.
            var messagesOutputFilePath = await this.GetMessagesOutputFilePathAsync(functionalityName, projectFilePath);

            var fileFormattedMessageSink = new FileFormattedMessageSink(messagesOutputFilePath, this.StringlyTypedPathOperator);

            // Create composite message sink for all sub-functionality to use, including 1) the parent functionality's message sink, the message repository for use in determining if there were any errors, and the file message sink for persistence of messages from this functionality.
            var compositeMessageSink = new CompositeMessageSink(this.MessageFormatter, new[] { parentMessageSink, inMemoryMessageRepository }, new[] { fileFormattedMessageSink });
            return compositeMessageSink;
        }

        private async Task ValidateProjectFileAsync(ProjectFile projectFile, IMessageSink messageSink)
        {
            var isValidProjectFile = await this.VisualStudioProjectFileValidator.Validate(projectFile, messageSink);
            if (!isValidProjectFile)
            {
                var timestampUtc = await this.NowUtcProvider.GetNowUtc();
                await messageSink.AddErrorMessageAsync(timestampUtc, "Project file invalid.");

                if (this.VisualStudioProjectFileDeserializationSettings.ThrowIfInvalidProjectFile)
                {
                    throw new Exception("Invalid project file.");
                }
            }
        }

        public async Task SerializeAsync(Stream stream, string projectFilePath, ProjectFile projectFile, IMessageSink messageSink)
        {
            // Create a message repository that can be used to test if there were any errors.
            var messageRepository = new InMemoryMessageRepository();

            var compositeMessageSink = await this.CreateCompositeMessageSinkAsync(projectFilePath, messageSink, messageRepository, Constants.ProjectFileSerializationFunctionalityName);

            // Test message output.
            await compositeMessageSink.AddOutputMessageAsync(this.NowUtcProvider, $"Serialization of:\n{projectFilePath}");

            // Change all project reference paths to be relative, not absolute, using the input project file path.
            foreach (var projectReference in projectFile.ProjectReferences)
            {
                var projectReferenceRelativePath = this.StringlyTypedPathOperator.GetRelativePathFileToFile(projectFilePath, projectReference.ProjectFilePath);

                projectReference.ProjectFilePath = projectReferenceRelativePath;
            }

            // Validate project file.
            await this.ValidateProjectFileAsync(projectFile, messageSink);
            
            await this.RelativeFilePathsVisualStudioProjectFileSerializer.Serialize(stream, projectFile, compositeMessageSink);

            // Any errors?
        }
    }
}
