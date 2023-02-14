using System;
using System.Threading.Tasks;

using R5T.D0004;
using R5T.Lombardy;
using R5T.Magyar.Extensions;using R5T.T0064;


namespace R5T.Kefalonia.Construction
{
    /// <summary>
    /// Async, stringly-typed paths.
    /// </summary>
    [ServiceImplementationMarker]
    class ProjectFileDeserializationMessagesOutputFileNameProvider : IProjectFileDeserializationMessagesOutputFileNameProvider,IServiceImplementation
    {
        private IFileNameOperator FileNameOperator { get; }
        private IGuidProvider GuidProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public ProjectFileDeserializationMessagesOutputFileNameProvider(
            IFileNameOperator fileNameOperator,
            IGuidProvider guidProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.FileNameOperator = fileNameOperator;
            this.GuidProvider = guidProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<string> GetProjectFileDeserializationMessagesOutputFileNameAsync(string projectFilePath)
        {
            var fileNameWithoutExtension = this.StringlyTypedPathOperator.GetFileNameWithoutExtension(projectFilePath);

            var guid = await this.GuidProvider.GetGuidAsync();

            var projectFileDeserializationMessagesOutputFileNameWithoutExtension = $"{fileNameWithoutExtension}_{guid.ToStringStandard()}";

            var outputFileExtension = Constants.OutputFileExtension;

            var projectFileDeserializationMessagesOutputFileName = this.FileNameOperator.GetFileName(projectFileDeserializationMessagesOutputFileNameWithoutExtension, outputFileExtension);
            return projectFileDeserializationMessagesOutputFileName;
        }
    }
}
