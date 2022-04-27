using System;
using System.IO;
using System.Threading.Tasks;

using R5T.D0010;
using R5T.Gloucester.Types;

using R5T.Kefalonia.Common;using R5T.T0064;


namespace R5T.Kefalonia.Construction
{[ServiceImplementationMarker]
    class VisualStudioProjectFileStreamSerializer : IVisualStudioProjectFileStreamSerializer,IServiceImplementation
    {
        private IFunctionalVisualStudioProjectFileStreamSerializer FunctionalVisualStudioProjectFileSerializer { get; }
        private IMessageSinkProvider MessageSinkProvider { get; }


        public VisualStudioProjectFileStreamSerializer(
            IFunctionalVisualStudioProjectFileStreamSerializer functionalVisualStudioProjectFileSerializer,
            IMessageSinkProvider messageSinkProvider)
        {
            this.FunctionalVisualStudioProjectFileSerializer = functionalVisualStudioProjectFileSerializer;
            this.MessageSinkProvider = messageSinkProvider;
        }

        public async Task<ProjectFile> DeserializeAsync(Stream stream, string projectFilePath)
        {
            // Get the program message sink (root functionality message sink).
            var messageSink = await this.MessageSinkProvider.GetMessageSinkAsync();

            var projectFile = await this.FunctionalVisualStudioProjectFileSerializer.DeserializeAsync(stream, projectFilePath, messageSink);
            return projectFile;
        }

        public async Task SerializeAsync(Stream stream, string projectFilePath, ProjectFile value)
        {
            // Get the program message sink (root functionality message sink).
            var messageSink = await this.MessageSinkProvider.GetMessageSinkAsync();

            await this.FunctionalVisualStudioProjectFileSerializer.SerializeAsync(stream, projectFilePath, value, messageSink);
        }
    }
}
