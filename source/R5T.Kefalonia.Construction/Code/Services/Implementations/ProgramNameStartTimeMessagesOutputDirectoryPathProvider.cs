using System;
using System.Threading.Tasks;

using R5T.D0006;
using R5T.D0012;

using R5T.Lombardy;using R5T.T0064;


namespace R5T.Kefalonia.Construction
{[ServiceImplementationMarker]
    public class ProgramNameStartTimeMessagesOutputDirectoryPathProvider : IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider,IServiceImplementation
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

        public async Task<string> GetProgramStartTimeSpecificMessagesOutputDirectoryPathAsync()
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
