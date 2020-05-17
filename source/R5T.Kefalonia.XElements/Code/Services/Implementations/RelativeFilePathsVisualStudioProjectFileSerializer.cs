using System;
using System.Threading.Tasks;
using System.Xml.Linq;

using R5T.D0010;
using R5T.Gloucester.Types;
using R5T.Lombardy;
using R5T.Magyar.IO;

using R5T.Kefalonia.Common;


namespace R5T.Kefalonia.XElements
{
    public class RelativeFilePathsVisualStudioProjectFileSerializer : IRelativeFilePathsVisualStudioProjectFileSerializer
    {
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }
        private IVisualStudioProjectFileToXElementConverter VisualStudioProjectFileToXElementConverter { get; }


        public RelativeFilePathsVisualStudioProjectFileSerializer(
            IStringlyTypedPathOperator stringlyTypedPathOperator,
            IVisualStudioProjectFileToXElementConverter visualStudioProjectFileToXElementConverter)
        {
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
            this.VisualStudioProjectFileToXElementConverter = visualStudioProjectFileToXElementConverter;
        }

        public Task<ProjectFile> Deserialize(string projectFilePath, IMessageSink messageSink)
        {
            var xElement = XElement.Load(projectFilePath); // No async version.

            var projectXElement = new ProjectXElement(xElement);

            var gettingProjectFile = this.VisualStudioProjectFileToXElementConverter.ToProjectFile(projectXElement, messageSink);
            return gettingProjectFile;
        }

        public async Task Serialize(string filePath, ProjectFile projectFile, IMessageSink messageSink, bool overwrite = true)
        {
            var projectXElement = await this.VisualStudioProjectFileToXElementConverter.ToProjectXElement(projectFile, messageSink);

            using (var fileStream = FileStreamHelper.NewWrite(filePath, overwrite))
            using (var xmlWriter = XmlWriterHelper.New(fileStream))
            {
                projectXElement.Value.Save(xmlWriter); // No async version.
            }
        }
    }
}
