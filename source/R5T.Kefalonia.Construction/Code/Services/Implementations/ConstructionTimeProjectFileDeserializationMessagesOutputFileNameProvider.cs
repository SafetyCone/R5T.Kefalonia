using System;
using System.Threading.Tasks;

using R5T.Lombardy;


namespace R5T.Kefalonia.Construction
{
    class ConstructionTimeProjectFileDeserializationMessagesOutputFileNameProvider : IProjectFileDeserializationMessagesOutputFileNameProvider
    {
        private IFileNameOperator FileNameOperator { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public ConstructionTimeProjectFileDeserializationMessagesOutputFileNameProvider(
            IFileNameOperator fileNameOperator,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.FileNameOperator = fileNameOperator;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public Task<string> GetProjectFileDeserializationMessagesOutputFileNameAsync(string projectFilePath)
        {
            var fileNameWithoutExtension = this.StringlyTypedPathOperator.GetFileNameWithoutExtension(projectFilePath);
            var outputFileExtension = Constants.OutputFileExtension;

            var projectFileDeserializationMessagesOutputFileName = this.FileNameOperator.GetFileName(fileNameWithoutExtension, outputFileExtension);

            return Task.FromResult(projectFileDeserializationMessagesOutputFileName);
        }
    }
}
