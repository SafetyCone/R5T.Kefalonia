using System;
using System.Threading.Tasks;

using R5T.D0006;
using R5T.D0012;

using R5T.Lombardy;


namespace R5T.Kefalonia.Construction
{
    public class ProgramNameStartTimeMessagesOutputDirectoryPathProvider : IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider
    {
        private IProcessStartTimeUtcDirectoryNameProvider ProcessStartTimeUtcDirectoryNameProvider { get; }
        private IProgramSpecificMessagesOutputDirectoryPathProvider ProgramSpecificMessagesOutputDirectoryPathProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public ProgramNameStartTimeMessagesOutputDirectoryPathProvider(
            IProcessStartTimeUtcDirectoryNameProvider processStartTimeUtcDirectoryNameProvider,
            IProgramSpecificMessagesOutputDirectoryPathProvider programSpecificMessagesOutputDirectoryPathProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.ProcessStartTimeUtcDirectoryNameProvider = processStartTimeUtcDirectoryNameProvider;
            this.ProgramSpecificMessagesOutputDirectoryPathProvider = programSpecificMessagesOutputDirectoryPathProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<string> GetProgramStartTimeSpecificMessagesOutputDirectoryPath()
        {
            var gettingProcessStartTimeUtcDirectoryName = this.ProcessStartTimeUtcDirectoryNameProvider.GetProcessStartTimeUtcDirectoryNameAsync();
            var gettingProgramSpecificMessagesOutputDirectoryPath = this.ProgramSpecificMessagesOutputDirectoryPathProvider.GetProgramSpecificMessagesOutputDirectoryPath();

            await Task.WhenAll(
                gettingProcessStartTimeUtcDirectoryName,
                gettingProgramSpecificMessagesOutputDirectoryPath);

            var programStartTimeSpecificMessagesOutputDirectoryPath = this.StringlyTypedPathOperator.Combine(
                gettingProgramSpecificMessagesOutputDirectoryPath.Result,
                gettingProcessStartTimeUtcDirectoryName.Result);
            return programStartTimeSpecificMessagesOutputDirectoryPath;
        }
    }
}
