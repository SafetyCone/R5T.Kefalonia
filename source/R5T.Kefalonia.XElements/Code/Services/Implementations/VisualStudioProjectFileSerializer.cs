using System;
using System.Xml.Linq;

using R5T.Gloucester.Types;
using R5T.Magyar.IO;


namespace R5T.Kefalonia.XElements
{
    public class VisualStudioProjectFileSerializer : IVisualStudioProjectFileSerializer
    {
        private IVisualStudioProjectFileToXElementConverter VisualStudioProjectFileToXElementConverter { get; }


        public VisualStudioProjectFileSerializer(IVisualStudioProjectFileToXElementConverter visualStudioProjectFileToXElementConverter)
        {
            this.VisualStudioProjectFileToXElementConverter = visualStudioProjectFileToXElementConverter;
        }

        public ProjectFile Deserialize(string projectFilePath)
        {
            var xElement = XElement.Load(projectFilePath);

            var projectXElement = new ProjectXElement(xElement);

            var projectFileResult = this.VisualStudioProjectFileToXElementConverter.ToProjectFile(projectXElement);

            foreach (var message in projectFileResult.Messages)
            {
                Console.WriteLine(message);
            }

            return projectFileResult.Value;
        }

        public void Serialize(string filePath, ProjectFile projectFile, bool overwrite = true)
        {
            var projectXElement = this.VisualStudioProjectFileToXElementConverter.ToProjectXElement(projectFile);

            using (var fileStream = FileStreamHelper.NewWrite(filePath, overwrite))
            using (var xmlWriter = XmlWriterHelper.New(fileStream))
            {
                projectXElement.Value.Save(xmlWriter);
            }
        }
    }
}
