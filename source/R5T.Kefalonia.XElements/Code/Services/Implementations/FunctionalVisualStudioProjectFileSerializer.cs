using System;
using System.Linq;
using System.Xml.Linq;

using R5T.Gloucester.Types;
using R5T.Lombardy;
using R5T.Magyar;
using R5T.Magyar.IO;

using R5T.Kefalonia.Common;


namespace R5T.Kefalonia.XElements
{
    public class FunctionalVisualStudioProjectFileSerializer : IFunctionalVisualStudioProjectFileSerializer
    {
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }
        private IVisualStudioProjectFileToXElementConverter VisualStudioProjectFileToXElementConverter { get; }
        private IVisualStudioProjectFileDeserializationSettings VisualStudioProjectFileDeserializationSettings { get; }


        public FunctionalVisualStudioProjectFileSerializer(
            IStringlyTypedPathOperator stringlyTypedPathOperator,
            IVisualStudioProjectFileToXElementConverter visualStudioProjectFileToXElementConverter,
            IVisualStudioProjectFileDeserializationSettings visualStudioProjectFileDeserializationSettings)
        {
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
            this.VisualStudioProjectFileToXElementConverter = visualStudioProjectFileToXElementConverter;
            this.VisualStudioProjectFileDeserializationSettings = visualStudioProjectFileDeserializationSettings;
        }

        public Result<ProjectFile> Deserialize(string projectFilePath)
        {
            var xElement = XElement.Load(projectFilePath);

            var projectXElement = new ProjectXElement(xElement);

            var projectFileResult = this.VisualStudioProjectFileToXElementConverter.ToProjectFile(projectXElement);

            foreach (var projectReference in projectFileResult.Value.ProjectReferences)
            {
                var projectReferenceAbsolutePath = this.StringlyTypedPathOperator.Combine(projectFilePath, projectReference.ProjectFilePath);

                projectReference.ProjectFilePath = projectReferenceAbsolutePath;
            }

            if(projectFileResult.Messages.Any())
            {
                if(this.VisualStudioProjectFileDeserializationSettings.ThrowIfAnyErrorAtEnd)
                {
                    throw new Exception("There were deserialization errors.\nSee result messages.");
                }
            }

            return projectFileResult;
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
