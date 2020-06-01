using System;
using System.Threading.Tasks;

using R5T.D0006;
using R5T.D0007;

using R5T.Lombardy;


namespace R5T.Kefalonia.Construction
{
    public class ProgramSpecificMessagesOutputDirectoryPathProvider : IProgramSpecificMessagesOutputDirectoryPathProvider
    {
        private IMessagesOutputBaseDirectoryPathProvider MessagesOutputBaseDirectoryPathProvider { get; }
        private IProgramNameDirectoryNameProvider ProgramNameDirectoryNameProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public ProgramSpecificMessagesOutputDirectoryPathProvider(
            IMessagesOutputBaseDirectoryPathProvider messagesOutputBaseDirectoryPathProvider,
            IProgramNameDirectoryNameProvider programNameDirectoryNameProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.MessagesOutputBaseDirectoryPathProvider = messagesOutputBaseDirectoryPathProvider;
            this.ProgramNameDirectoryNameProvider = programNameDirectoryNameProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<string> GetProgramSpecificMessagesOutputDirectoryPath()
        {
            var gettingMessagesOutputBaseDirectoryPath = this.MessagesOutputBaseDirectoryPathProvider.GetMessagesOutputBaseDirectoryPathAsync();
            var gettingProgramNameDirectoryName = this.ProgramNameDirectoryNameProvider.GetProgramNameDirectoryNameAsync();

            await Task.WhenAll(
                gettingMessagesOutputBaseDirectoryPath,
                gettingProgramNameDirectoryName);

            var messagesOutputDirectoryPath = this.StringlyTypedPathOperator.Combine(
                gettingMessagesOutputBaseDirectoryPath.Result,
                gettingProgramNameDirectoryName.Result);
            return messagesOutputDirectoryPath;
        }
    }
}
