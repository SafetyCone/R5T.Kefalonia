using System;
using System.Threading.Tasks;

using R5T.Gloucester.Types;
using R5T.Magyar.IO;using R5T.T0064;


namespace R5T.Kefalonia.Construction
{[ServiceImplementationMarker]
    class VisualStudioProjectFileSerializer : IVisualStudioProjectFileSerializer,IServiceImplementation
    {
        private IVisualStudioProjectFileStreamSerializer VisualStudioProjectFileStreamSerializer { get; }


        public VisualStudioProjectFileSerializer(
            IVisualStudioProjectFileStreamSerializer visualStudioProjectFileStreamSerializer)
        {
            this.VisualStudioProjectFileStreamSerializer = visualStudioProjectFileStreamSerializer;
        }

        public async Task<ProjectFile> DeserializeAsync(string projectFilePath)
        {
            using (var fileStream = FileStreamHelper.NewRead(projectFilePath))
            {
                var projectFile = await this.VisualStudioProjectFileStreamSerializer.DeserializeAsync(fileStream, projectFilePath);
                return projectFile;
            }
        }

        public async Task SerializeAsync(string projectFilePath, ProjectFile value, bool overwrite = true)
        {
            using (var fileStream = FileStreamHelper.NewWrite(projectFilePath, overwrite))
            {
                await this.VisualStudioProjectFileStreamSerializer.SerializeAsync(fileStream, projectFilePath, value);
            }
        }
    }
}
