using System;
using System.Linq;
using System.Threading.Tasks;

using R5T.D0001;
using R5T.D0010;
using R5T.Gloucester.Types;
using R5T.Lombardy;


namespace R5T.Kefalonia.Common
{
    public class FunctionalVisualStudioProjectFileSerializer : IFunctionalVisualStudioProjectFileSerializer
    {
        private IMessageFormatter MessageFormatter { get; }
        private INowUtcProvider NowUtcProvider { get; }
        private IRelativeFilePathsVisualStudioProjectFileSerializer RelativeFilePathsVisualStudioProjectFileSerializer { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }
        private IVisualStudioProjectFileDeserializationSettings VisualStudioProjectFileDeserializationSettings { get; }
        private IVisualStudioProjectFileValidator VisualStudioProjectFileValidator { get; }


        public FunctionalVisualStudioProjectFileSerializer(
            IMessageFormatter messageFormatter,
            INowUtcProvider nowUtcProvider,
            IRelativeFilePathsVisualStudioProjectFileSerializer relativeFilePathsVisualStudioProjectFileSerializer,
            IStringlyTypedPathOperator stringlyTypedPathOperator,
            IVisualStudioProjectFileDeserializationSettings visualStudioProjectFileDeserializationSettings,
            IVisualStudioProjectFileValidator visualStudioProjectFileValidator)
        {
            this.MessageFormatter = messageFormatter;
            this.NowUtcProvider = nowUtcProvider;
            this.RelativeFilePathsVisualStudioProjectFileSerializer = relativeFilePathsVisualStudioProjectFileSerializer;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
            this.VisualStudioProjectFileDeserializationSettings = visualStudioProjectFileDeserializationSettings;
            this.VisualStudioProjectFileValidator = visualStudioProjectFileValidator;
        }

        public async Task<ProjectFile> DeserializeAsync(string projectFilePath, IMessageSink messageSink)
        {
            var messageRepository = new InMemoryMessageRepository();

            var compositeMessageSink = new CompositeMessageSink(this.MessageFormatter, new[] { messageSink, messageRepository });

            var projectFile = await this.RelativeFilePathsVisualStudioProjectFileSerializer.Deserialize(projectFilePath, compositeMessageSink);

            // Change all project reference paths to be absolute, not relative, using the input project file path.
            foreach (var projectReference in projectFile.ProjectReferences)
            {
                var projectReferenceAbsolutePath = this.StringlyTypedPathOperator.Combine(projectFilePath, projectReference.ProjectFilePath);

                projectReference.ProjectFilePath = projectReferenceAbsolutePath;
            }

            // Validates the project file.
            var isValidProjectFile = await this.VisualStudioProjectFileValidator.Validate(projectFile, compositeMessageSink);
            if(!isValidProjectFile)
            {
                var timestampUtc = await this.NowUtcProvider.GetNowUtcAsync();
                await messageRepository.AddErrorMessageAsync(timestampUtc, "Project file invalid.");

                if(this.VisualStudioProjectFileDeserializationSettings.ThrowIfInvalidProjectFile)
                {
                    throw new Exception("Invalid project file.");
                }
            }

            // Output any result output messages or error messages.

            // If there are any error messages, and the deserializations settings say we should throw if there are any error messages, throw an exception.
            var errorMessages = await messageRepository.GetErrorsAsync();
            if (errorMessages.Any())
            {
                if (this.VisualStudioProjectFileDeserializationSettings.ThrowIfAnyErrorAtEnd)
                {
                    throw new Exception("There were deserialization errors.\nSee result messages.");
                }
            }

            return projectFile;
        }

        public Task SerializeAsync(string projectFilePath, ProjectFile projectFile, IMessageSink messageSink, bool overwrite = true)
        {
            return this.RelativeFilePathsVisualStudioProjectFileSerializer.Serialize(projectFilePath, projectFile, messageSink, overwrite);
        }
    }
}
