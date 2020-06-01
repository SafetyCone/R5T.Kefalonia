using System;
using System.Threading.Tasks;

using R5T.D0006;
using R5T.D0007;
using R5T.D0012;

using R5T.Lombardy;


namespace R5T.Kefalonia.Construction
{
    public class ProgramNameStartTimeMessagesOutputDirectoryPathProvider : IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider
    {
        private IMessagesOutputBaseDirectoryPathProvider MessagesOutputBaseDirectoryPathProvider { get; }
        private IProcessStartTimeUtcDirectoryNameProvider ProcessStartTimeUtcDirectoryNameProvider { get; }
        private IProgramNameDirectoryNameProvider ProgramNameDirectoryNameProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public ProgramNameStartTimeMessagesOutputDirectoryPathProvider(
            IMessagesOutputBaseDirectoryPathProvider messagesOutputBaseDirectoryPathProvider,
            IProcessStartTimeUtcDirectoryNameProvider processStartTimeUtcDirectoryNameProvider,
            IProgramNameDirectoryNameProvider programNameDirectoryNameProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.MessagesOutputBaseDirectoryPathProvider = messagesOutputBaseDirectoryPathProvider;
            this.ProcessStartTimeUtcDirectoryNameProvider = processStartTimeUtcDirectoryNameProvider;
            this.ProgramNameDirectoryNameProvider = programNameDirectoryNameProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<string> GetProgramStartTimeSpecificMessagesOutputDirectoryPath()
        {
            var gettingMessagesOutputBaseDirectoryPath = this.MessagesOutputBaseDirectoryPathProvider.GetMessagesOutputBaseDirectoryPathAsync();
            var gettingProgramNameDirectoryName = this.ProgramNameDirectoryNameProvider.GetProgramNameDirectoryNameAsync();
            var gettingProcessStartTimeUtcDirectoryName = this.ProcessStartTimeUtcDirectoryNameProvider.GetProcessStartTimeUtcDirectoryNameAsync();

            await Task.WhenAll(
                gettingMessagesOutputBaseDirectoryPath,
                gettingProgramNameDirectoryName,
                gettingProcessStartTimeUtcDirectoryName);

            var messagesOutputDirectoryPath = this.StringlyTypedPathOperator.Combine(
                gettingMessagesOutputBaseDirectoryPath.Result,
                gettingProgramNameDirectoryName.Result,
                gettingProcessStartTimeUtcDirectoryName.Result);
            return messagesOutputDirectoryPath;
        }
    }
}
