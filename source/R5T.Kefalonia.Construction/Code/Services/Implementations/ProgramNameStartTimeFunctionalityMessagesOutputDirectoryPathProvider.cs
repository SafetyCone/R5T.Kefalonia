using System;
using System.Threading.Tasks;

using R5T.D0006;
using R5T.D0007;
using R5T.D0012;
using R5T.Lombardy;


namespace R5T.Kefalonia.Construction
{
    /// <summary>
    /// Async, stringly-typed paths.
    /// </summary>
    public class ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider : IFunctionalitySpecificMessagesOutputDirectorypathProvider
    {
        private FunctionalityDirectoryNameProvider FunctionalityDirectoryNameProvider { get; }
        private IMessagesOutputBaseDirectoryPathProvider MessagesOutputBaseDirectoryPathProvider { get; }
        private IProcessStartTimeUtcDirectoryNameProvider ProcessStartTimeUtcDirectoryNameProvider { get; }
        private IProgramNameDirectoryNameProvider ProgramNameDirectoryNameProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider(
            FunctionalityDirectoryNameProvider functionalityDirectoryNameProvider,
            IMessagesOutputBaseDirectoryPathProvider messagesOutputBaseDirectoryPathProvider,
            IProcessStartTimeUtcDirectoryNameProvider processStartTimeUtcDirectoryNameProvider,
            IProgramNameDirectoryNameProvider programNameDirectoryNameProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.FunctionalityDirectoryNameProvider = functionalityDirectoryNameProvider;
            this.MessagesOutputBaseDirectoryPathProvider = messagesOutputBaseDirectoryPathProvider;
            this.ProcessStartTimeUtcDirectoryNameProvider = processStartTimeUtcDirectoryNameProvider;
            this.ProgramNameDirectoryNameProvider = programNameDirectoryNameProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<string> GetFunctionalitySpecificMessagesOutputDirectoryPath(string functionalityName)
        {
            var gettingMessagesOutputBaseDirectoryPath = this.MessagesOutputBaseDirectoryPathProvider.GetMessagesOutputBaseDirectoryPathAsync();
            var gettingProgramNameDirectoryName = this.ProgramNameDirectoryNameProvider.GetProgramNameDirectoryNameAsync();
            var gettingProcessStartTimeUtcDirectoryName = this.ProcessStartTimeUtcDirectoryNameProvider.GetProcessStartTimeUtcDirectoryNameAsync();
            var gettingFunctionalityDirectoryName = this.FunctionalityDirectoryNameProvider.GetFunctionalityDirectoryNameAsync(functionalityName);

            await Task.WhenAll(
                gettingMessagesOutputBaseDirectoryPath,
                gettingProgramNameDirectoryName,
                gettingProcessStartTimeUtcDirectoryName,
                gettingFunctionalityDirectoryName);

            var messagesOutputDirectoryPath = this.StringlyTypedPathOperator.Combine(
                gettingMessagesOutputBaseDirectoryPath.Result,
                gettingProgramNameDirectoryName.Result,
                gettingProcessStartTimeUtcDirectoryName.Result,
                gettingFunctionalityDirectoryName.Result);
            return messagesOutputDirectoryPath;
        }
    }
}
