using System;
using System.Threading.Tasks;

using R5T.Lombardy;


namespace R5T.Kefalonia.Construction
{
    class ProjectFileDeserializationMessagesOutputFilePathProvider
    {
        private ProjectFileDeserializationMessagesOutputFileNameProvider ProjectFileDeserializationMessagesOutputFileNameProvider { get; }
        private ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public ProjectFileDeserializationMessagesOutputFilePathProvider(
            ProjectFileDeserializationMessagesOutputFileNameProvider projectFileDeserializationMessagesOutputFileNameProvider,
            ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider programNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.ProjectFileDeserializationMessagesOutputFileNameProvider = projectFileDeserializationMessagesOutputFileNameProvider;
            this.ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider = programNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<string> GetProjectFileDeserializationMessagesOutputFilePath(string projectFilePath)
        {
            var gettingMessagesOutputDirectoryPath = this.ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider.GetProgramNameStartTimeFunctionalityMessagesOutputDirectoryPath(
                Constants.ProjectFileDeserializationFunctionalityName);
            var gettingMessagesOutputFileName = this.ProjectFileDeserializationMessagesOutputFileNameProvider.GetProjectFileDeserializationMessagesOutputFileNameAsync(projectFilePath);

            await Task.WhenAll(
                gettingMessagesOutputDirectoryPath,
                gettingMessagesOutputFileName);

            var messagesOutputFilePath = this.StringlyTypedPathOperator.GetFilePath(gettingMessagesOutputDirectoryPath.Result, gettingMessagesOutputFileName.Result);
            return messagesOutputFilePath;
        }
    }
}
