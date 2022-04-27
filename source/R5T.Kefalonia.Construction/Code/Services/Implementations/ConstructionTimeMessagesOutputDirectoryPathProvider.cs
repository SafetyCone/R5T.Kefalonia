using System;
using System.Threading.Tasks;

using R5T.D0006;
using R5T.Lombardy;using R5T.T0064;


namespace R5T.Kefalonia.Construction
{[ServiceImplementationMarker]
    /// <summary>
    /// A dummy <see cref="IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider"/> implementation that always returns a single directory, so that during construction you don't have to chase after the output files.
    /// </summary>
    public class ConstructionTimeMessagesOutputDirectoryPathProvider : IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider,IServiceImplementation
    {
        public const string ConstructionDirectoryName = "Construction";


        private IProgramSpecificMessagesOutputDirectoryPathProvider ProgramSpecificMessagesOutputDirectoryPathProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public ConstructionTimeMessagesOutputDirectoryPathProvider(
            IProgramSpecificMessagesOutputDirectoryPathProvider programSpecificMessagesOutputDirectoryPathProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.ProgramSpecificMessagesOutputDirectoryPathProvider = programSpecificMessagesOutputDirectoryPathProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<string> GetProgramStartTimeSpecificMessagesOutputDirectoryPathAsync()
        {
            var programSpecificMessagesOutputDirectoryPath = await this.ProgramSpecificMessagesOutputDirectoryPathProvider.GetProgramSpecificMessagesOutputDirectoryPath();

            var programStartTimeSpecificMessagesOutputDirectoryPath = this.StringlyTypedPathOperator.Combine(
                programSpecificMessagesOutputDirectoryPath,
                ConstructionTimeMessagesOutputDirectoryPathProvider.ConstructionDirectoryName);
            return programStartTimeSpecificMessagesOutputDirectoryPath;
        }
    }
}
