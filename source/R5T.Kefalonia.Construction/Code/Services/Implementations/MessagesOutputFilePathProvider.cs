using System;
using System.Threading.Tasks;

using R5T.D0006;

using R5T.Lombardy;


namespace R5T.Kefalonia.Construction
{
    class MessagesOutputFilePathProvider
    {
        private ProjectFileDeserializationMessagesOutputFileNameProvider ProjectFileDeserializationMessagesOutputFileNameProvider { get; }
        private IFunctionalitySpecificMessagesOutputDirectoryPathProvider FunctionalitySpecificMessagesOutputDirectoryPathProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public MessagesOutputFilePathProvider(
            ProjectFileDeserializationMessagesOutputFileNameProvider projectFileDeserializationMessagesOutputFileNameProvider,
            IFunctionalitySpecificMessagesOutputDirectoryPathProvider functionalitySpecificMessagesOutputDirectoryPathProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.ProjectFileDeserializationMessagesOutputFileNameProvider = projectFileDeserializationMessagesOutputFileNameProvider;
            this.FunctionalitySpecificMessagesOutputDirectoryPathProvider = functionalitySpecificMessagesOutputDirectoryPathProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<string> GetMessagesOutputFilePathAsync(string functionalityName, string projectFilePath)
        {
            var gettingMessagesOutputDirectoryPath = this.FunctionalitySpecificMessagesOutputDirectoryPathProvider.GetFunctionalitySpecificMessagesOutputDirectoryPath(
                functionalityName);
            var gettingMessagesOutputFileName = this.ProjectFileDeserializationMessagesOutputFileNameProvider.GetProjectFileDeserializationMessagesOutputFileNameAsync(projectFilePath);

            await Task.WhenAll(
                gettingMessagesOutputDirectoryPath,
                gettingMessagesOutputFileName);

            var messagesOutputFilePath = this.StringlyTypedPathOperator.GetFilePath(gettingMessagesOutputDirectoryPath.Result, gettingMessagesOutputFileName.Result);
            return messagesOutputFilePath;
        }
    }
}
