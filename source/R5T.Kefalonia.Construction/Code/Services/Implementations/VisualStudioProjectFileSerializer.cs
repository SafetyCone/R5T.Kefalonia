using System;
using System.IO;
using System.Threading.Tasks;

using R5T.D0001;
using R5T.D0010;
using R5T.Gloucester.Types;
using R5T.Lombardy;

using R5T.Kefalonia.Common;


namespace R5T.Kefalonia.Construction
{
    class VisualStudioProjectFileSerializer : IVisualStudioProjectFileSerializer
    {
        private IFunctionalVisualStudioProjectFileSerializer FunctionalVisualStudioProjectFileSerializer { get; }
        private IMessageFormatter MessageFormatter { get; }
        private IMessageSinkProvider MessageSinkProvider { get; }
        private INowUtcProvider NowUtcProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public VisualStudioProjectFileSerializer(
            IFunctionalVisualStudioProjectFileSerializer functionalVisualStudioProjectFileSerializer,
            IMessageFormatter messageFormatter,
            IMessageSinkProvider messageSinkProvider,
            INowUtcProvider nowUtcProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.FunctionalVisualStudioProjectFileSerializer = functionalVisualStudioProjectFileSerializer;
            this.MessageFormatter = messageFormatter;
            this.MessageSinkProvider = messageSinkProvider;
            this.NowUtcProvider = nowUtcProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<ProjectFile> DeserializeAsync(string projectFilePath)
        {
            // Get the program message sink (root functionality message sink).
            var messageSink = await this.MessageSinkProvider.GetMessageSinkAsync();

            var projectFile = await this.FunctionalVisualStudioProjectFileSerializer.DeserializeAsync(projectFilePath, messageSink);
            return projectFile;
        }

        public async Task SerializeAsync(string projectFilePath, ProjectFile value, bool overwrite = true)
        {
            // Get the program message sink (root functionality message sink).
            var messageSink = await this.MessageSinkProvider.GetMessageSinkAsync();

            await this.FunctionalVisualStudioProjectFileSerializer.SerializeAsync(projectFilePath, value, messageSink, overwrite);
        }
    }
}
