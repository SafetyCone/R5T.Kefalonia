using System;

using R5T.Gloucester.Types;

using R5T.Kefalonia.Common;


namespace R5T.Kefalonia
{
    public class VisualStudioProjectFileSerializer : IVisualStudioProjectFileSerializer
    {
        private IFunctionalVisualStudioProjectFileSerializer FunctionalVisualStudioProjectFileSerializer { get; }


        public VisualStudioProjectFileSerializer(IFunctionalVisualStudioProjectFileSerializer functionalVisualStudioProjectFileSerializer)
        {
            this.FunctionalVisualStudioProjectFileSerializer = functionalVisualStudioProjectFileSerializer;
        }

        public ProjectFile Deserialize(string projectFilePath)
        {
            var result = this.FunctionalVisualStudioProjectFileSerializer.Deserialize(projectFilePath);

            foreach (var message in result.Messages)
            {
                Console.WriteLine(message);
            }

            return result.Value;
        }

        public void Serialize(string filePath, ProjectFile value, bool overwrite = true)
        {
            this.FunctionalVisualStudioProjectFileSerializer.Serialize(filePath, value, overwrite);
        }
    }
}
