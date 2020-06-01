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
    public class ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider : IFunctionalitySpecificMessagesOutputDirectoryPathProvider
    {
        private FunctionalityDirectoryNameProvider FunctionalityDirectoryNameProvider { get; }
        private ProgramNameStartTimeMessagesOutputDirectoryPathProvider ProgramNameStartTimeMessagesOutputDirectoryPathProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider(
            FunctionalityDirectoryNameProvider functionalityDirectoryNameProvider,
            ProgramNameStartTimeMessagesOutputDirectoryPathProvider programNameStartTimeMessagesOutputDirectoryPathProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.FunctionalityDirectoryNameProvider = functionalityDirectoryNameProvider;
            this.ProgramNameStartTimeMessagesOutputDirectoryPathProvider = programNameStartTimeMessagesOutputDirectoryPathProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<string> GetFunctionalitySpecificMessagesOutputDirectoryPath(string functionalityName)
        {
            var gettingProgramNameStartTimeMessagesOutputDirectoryPath = this.ProgramNameStartTimeMessagesOutputDirectoryPathProvider.GetProgramNameStartTimeMessagesOutputDirectoryPath();
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
