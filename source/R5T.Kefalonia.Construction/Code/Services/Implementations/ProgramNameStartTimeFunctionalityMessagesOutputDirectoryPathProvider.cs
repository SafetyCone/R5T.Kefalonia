using System;
using System.Threading.Tasks;

using R5T.Lombardy;

using R5T.D0006;
using R5T.D0007;
using R5T.D0012;
using R5T.T0064;


namespace R5T.Kefalonia.Construction
{
    /// <summary>
    /// Async, stringly-typed paths.
    /// </summary>
    [ServiceImplementationMarker]
    public class ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider : IFunctionalitySpecificMessagesOutputDirectoryPathProvider, IServiceImplementation
    {
        private FunctionalityDirectoryNameProvider FunctionalityDirectoryNameProvider { get; }
        private IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider ProgramStartTimeSpecificMessagesOutputDirectoryPathProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider(
            // TODO, remove NotServiceComponent attribute when we actually test for service components.
            [NotServiceComponent] FunctionalityDirectoryNameProvider functionalityDirectoryNameProvider,
            IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider programStartTimeSpecificMessagesOutputDirectoryPathProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.FunctionalityDirectoryNameProvider = functionalityDirectoryNameProvider;
            this.ProgramStartTimeSpecificMessagesOutputDirectoryPathProvider = programStartTimeSpecificMessagesOutputDirectoryPathProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<string> GetFunctionalitySpecificMessagesOutputDirectoryPath(string functionalityName)
        {
            var gettingProgramNameStartTimeMessagesOutputDirectoryPath = this.ProgramStartTimeSpecificMessagesOutputDirectoryPathProvider.GetProgramStartTimeSpecificMessagesOutputDirectoryPathAsync();
            var gettingFunctionalityDirectoryName = this.FunctionalityDirectoryNameProvider.GetFunctionalityDirectoryNameAsync(functionalityName);

            await Task.WhenAll(
                gettingProgramNameStartTimeMessagesOutputDirectoryPath,
                gettingFunctionalityDirectoryName);

            var messagesOutputDirectoryPath = this.StringlyTypedPathOperator.Combine(
                gettingProgramNameStartTimeMessagesOutputDirectoryPath.Result,
                gettingFunctionalityDirectoryName.Result);
            return messagesOutputDirectoryPath;
        }
    }
}
