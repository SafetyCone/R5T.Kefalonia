using System;
using System.Threading.Tasks;

using R5T.Lombardy;using R5T.T0064;


namespace R5T.Kefalonia.Construction
{[ServiceImplementationMarker]
    class ConstructionTimeProjectFileDeserializationMessagesOutputFileNameProvider : IProjectFileDeserializationMessagesOutputFileNameProvider,IServiceImplementation
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
