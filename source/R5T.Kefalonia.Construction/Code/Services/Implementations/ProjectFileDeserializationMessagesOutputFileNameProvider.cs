using System;
using System.Threading.Tasks;

using R5T.D0004;
using R5T.Lombardy;
using R5T.Magyar.Extensions;


namespace R5T.Kefalonia.Construction
{
    /// <summary>
    /// Async, stringly-typed paths.
    /// </summary>
    class ProjectFileDeserializationMessagesOutputFileNameProvider
    {
        private IFileNameOperator FileNameOperator { get; }
        private IGuidProvider GuidProvider { get; }


        public ProjectFileDeserializationMessagesOutputFileNameProvider(
            IFileNameOperator fileNameOperator,
            IGuidProvider guidProvider)
        {
            this.FileNameOperator = fileNameOperator;
            this.GuidProvider = guidProvider;
        }

        public async Task<string> GetProjectFileDeserializationMessagesOutputFileNameAsync(string projectFilePath)
        {
            var fileNameWithoutExtension = this.FileNameOperator.GetFileNameWithoutExtension(projectFilePath);

            var guid = await this.GuidProvider.GetGuidAsync();

            var projectFileDeserializationMessagesOutputFileNameWithoutExtension = $"{fileNameWithoutExtension}_{guid.ToStringStandard()}";

            var outputFileExtension = "output";

            var projectFileDeserializationMessagesOutputFileName = this.FileNameOperator.GetFileName(projectFileDeserializationMessagesOutputFileNameWithoutExtension, outputFileExtension);
            return projectFileDeserializationMessagesOutputFileName;
        }
    }
}
