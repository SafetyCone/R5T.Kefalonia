using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using R5T.Angleterria;
using R5T.Ostersund.Extensions;
using R5T.Gloucester.Types;
using R5T.Magyar;
using R5T.VisualStudioProjectFileStuff;

using R5T.Kefalonia.Common;


namespace R5T.Kefalonia.XElements
{
    public class VisualStudioProjectFileToXElementConverter : IVisualStudioProjectFileToXElementConverter
    {
        #region Static

        // Cannot be unified with attributes since XAttribute and XElement only share an ancestor at XObject, which lacks a Name property.
        private static void PerformLookupElements(IEnumerable<XElement> elementSource, Dictionary<string, Action<XElement, ProjectFile>> handlers, Action<XElement, ProjectFile> unknownHandler, ProjectFile projectFile)
        {
            foreach (var element in elementSource)
            {
                var elementName = element.Name.LocalName;
                if (handlers.ContainsKey(elementName))
                {
                    handlers[elementName](element, projectFile);
                }
                else
                {
                    unknownHandler(element, projectFile);
                }
            }
        }

        // Cannot be unified with elements since XAttribute and XElement only share an ancestor at XObject, which lacks a Name property.
        private static void PerformLookupAttributes(IEnumerable<XAttribute> attributeSource, Dictionary<string, Action<XAttribute, ProjectFile>> handlers, Action<XAttribute, ProjectFile> unknownHandler, ProjectFile projectFile)
        {
            foreach (var attribute in attributeSource)
            {
                var elementName = attribute.Name.LocalName;
                if (handlers.ContainsKey(elementName))
                {
                    handlers[elementName](attribute, projectFile);
                }
                else
                {
                    unknownHandler(attribute, projectFile);
                }
            }
        }

        #endregion


        private IVisualStudioProjectFileDeserializationErrorAggregator VisualStudioProjectFileDeserializationErrorAggregator { get; }
        private IVisualStudioProjectFileDeserializationSettings VisualStudioProjectFileDeserializationSettings { get; }
        private IVisualStudioProjectFileValidator VisualStudioProjectFileValidator { get; }

        private Dictionary<string, Action<XAttribute, ProjectFile>> ProjectAttributeHandlers { get; set; }
        private Dictionary<string, Action<XElement, ProjectFile>> ProjectChildElementHandlers { get; set; }
        private Dictionary<string, Action<XElement, ProjectFile>> PropertyGroupChildElementHandlers { get; set; }
        private Dictionary<string, Action<XElement, ProjectFile>> ItemGroupChildElementHandlers { get; set; }


        public VisualStudioProjectFileToXElementConverter(
            IVisualStudioProjectFileDeserializationErrorAggregator visualStudioProjectFileDeserializationErrorAggregator,
            IVisualStudioProjectFileDeserializationSettings visualStudioProjectFileDeserializationSettings,
            IVisualStudioProjectFileValidator visualStudioProjectFileValidator)
        {
            this.VisualStudioProjectFileDeserializationErrorAggregator = visualStudioProjectFileDeserializationErrorAggregator;
            this.VisualStudioProjectFileDeserializationSettings = visualStudioProjectFileDeserializationSettings;
            this.VisualStudioProjectFileValidator = visualStudioProjectFileValidator;

            this.ConstrutorEndSetup();
        }

        private void ConstrutorEndSetup()
        {
            // Perform these steps at the end of constructor at the instance-level to allow binding to instance's service instances.
            this.ProjectAttributeHandlers = this.GetProjectAttributeHandlers();
            this.ProjectChildElementHandlers = this.GetProjectChildElementHandlers();
            this.PropertyGroupChildElementHandlers = this.GetPropertyGroupChildElementHandlers();
            this.ItemGroupChildElementHandlers = this.GetItemGroupChildElementHandlers();
        }

        private void ReportError(string message)
        {
            if (this.VisualStudioProjectFileDeserializationSettings.ThrowAtErrorOccurrence)
            {
                throw new Exception(message);
            }
            else
            {
                this.VisualStudioProjectFileDeserializationErrorAggregator.AddError(message);
            }
        }

        private void ReportErrors(IEnumerable<string> messages)
        {
            foreach (var message in messages)
            {
                this.ReportError(message);
            }
        }

        private void ProjectElementHandler(XElement xElement, ProjectFile projectFile)
        {
            VisualStudioProjectFileToXElementConverter.PerformLookupAttributes(xElement.Attributes(), this.ProjectAttributeHandlers, this.UnknownProjectAttributeHandler, projectFile);
            VisualStudioProjectFileToXElementConverter.PerformLookupElements(xElement.Elements(), this.ProjectChildElementHandlers, this.UnknownProjectChildElementHandler, projectFile);
        }

        private Dictionary<string, Action<XAttribute, ProjectFile>> GetProjectAttributeHandlers()
        {
            var projectAttributeHandlers = new Dictionary<string, Action<XAttribute, ProjectFile>>
            {
                {  ProjectFileXmlElementName.Sdk, this.HandleSdkProjectAttribute },
            };

            return projectAttributeHandlers;
        }

        private void UnknownProjectAttributeHandler(XAttribute xAttribute, ProjectFile projectFile)
        {
            var message = $"Unnown project attribute: {xAttribute.Name}";

            this.ReportError(message);
        }

        private void HandleSdkProjectAttribute(XAttribute sdkAttribute, ProjectFile projectFile)
        {
            projectFile.SDK = sdkAttribute.Value;
        }

        private Dictionary<string, Action<XElement, ProjectFile>> GetProjectChildElementHandlers()
        {
            var projectChildElementHandlers = new Dictionary<string, Action<XElement, ProjectFile>>()
            {
                { ProjectFileXmlElementName.PropertyGroup, this.PropertyGroupElementHandler },
                { ProjectFileXmlElementName.ItemGroup, this.ItemGroupElementHandler },
            };

            return projectChildElementHandlers;
        }

        private void UnknownProjectChildElementHandler(XElement unknownChild, ProjectFile projectFile)
        {
            var message = $"Unnown project child element: {unknownChild.Name}";

            this.ReportError(message);
        }

        public void PropertyGroupElementHandler(XElement xPropertyGroup, ProjectFile projectFile)
        {
            VisualStudioProjectFileToXElementConverter.PerformLookupElements(xPropertyGroup.Elements(), this.PropertyGroupChildElementHandlers, this.UnknownPropertyGroupChildElementHandler, projectFile);
        }

        public void ItemGroupElementHandler(XElement xItemGroup, ProjectFile projectFile)
        {
            VisualStudioProjectFileToXElementConverter.PerformLookupElements(xItemGroup.Elements(), this.ItemGroupChildElementHandlers, this.UnknownItemGroupChildElementHandler, projectFile);
        }

        private Dictionary<string, Action<XElement, ProjectFile>> GetPropertyGroupChildElementHandlers()
        {
            var propertyGroupChildElementHandlers = new Dictionary<string, Action<XElement, ProjectFile>>()
            {
                { ProjectFileXmlElementName.IsPackable, this.IsPackableElementHandler },
                { ProjectFileXmlElementName.LanguageVersion, this.LanguageVersionElementHandler },
                { ProjectFileXmlElementName.OutputType, this.OutputTypeElementHandler },
                { ProjectFileXmlElementName.TargetFramework, this.TargetFrameworkElementHandler },
            };

            return propertyGroupChildElementHandlers;
        }

        private void UnknownPropertyGroupChildElementHandler(XElement unknownChild, ProjectFile projectFile)
        {
            var message = $"Unnown property group child element: {unknownChild.Name}";

            this.ReportError(message);
        }

        private void GenerateDocumentationFileElementHandler(XElement xGenerateDocumentationFile, ProjectFile projectFile)
        {
            projectFile.GenerateDocumentationFile = ProjectFileValues.ParseBoolean(xGenerateDocumentationFile.Value);
        }

        private void IsPackableElementHandler(XElement xIsPackable, ProjectFile projectFile)
        {
            projectFile.IsPackable = ProjectFileValues.ParseBoolean(xIsPackable.Value);
        }

        private void LanguageVersionElementHandler(XElement xLanguageVersion, ProjectFile projectFile)
        {
            projectFile.LanguageVersion = ProjectFileValues.ParseVersion(xLanguageVersion.Value);
        }

        private void NoWarnElementHandler(XElement xNoWarn, ProjectFile projectFile)
        {
            projectFile.NoWarn = ProjectFileValues.ParseNoWarn(xNoWarn.Value);
        }

        private void OutputTypeElementHandler(XElement xOutputType, ProjectFile projectFile)
        {
            projectFile.OutputType = ProjectFileValues.ParseOutputType(xOutputType.Value);
        }

        private void TargetFrameworkElementHandler(XElement xTargetFramework, ProjectFile projectFile)
        {
            projectFile.TargetFramework = ProjectFileValues.ParseTargetFramework(xTargetFramework.Value);
        }

        private Dictionary<string, Action<XElement, ProjectFile>> GetItemGroupChildElementHandlers()
        {
            var itemGroupChildElementHandlers = new Dictionary<string, Action<XElement, ProjectFile>>()
            {
                { ProjectFileXmlElementName.PackageReference, this.PackageReferenceElementHandler },
                { ProjectFileXmlElementName.ProjectReference, this.ProjectReferenceElementHandler },
            };

            return itemGroupChildElementHandlers;
        }

        private void UnknownItemGroupChildElementHandler(XElement unknownChild, ProjectFile projectFile)
        {
            var message = $"Unnown item group child element: {unknownChild.Name}";

            this.ReportError(message);
        }

        private void PackageReferenceElementHandler(XElement xPackageReference, ProjectFile projectFile)
        {
            var packageName = xPackageReference.Attribute(ProjectFileXmlElementName.Include).Value;
            var packageVersionString = xPackageReference.Attribute(ProjectFileXmlElementName.Version).Value;

            var packageReference = new PackageReference(packageName, packageVersionString);

            projectFile.PackageReferences.Add(packageReference);
        }

        private void ProjectReferenceElementHandler(XElement xProjectReference, ProjectFile projectFile)
        {
            var projectReferenceRelativePath = xProjectReference.Attribute(ProjectFileXmlElementName.Include).Value;

            var projectReference = new ProjectReference(projectReferenceRelativePath);

            projectFile.ProjectReferences.Add(projectReference);
        }

        public Result<ProjectFile> ToProjectFile(ProjectXElement projectXElement)
        {
            var projectFile = new ProjectFile();

            this.ProjectElementHandler(projectXElement.Value, projectFile);

            var validationResult = this.VisualStudioProjectFileValidator.Validate(projectFile);
            if(!validationResult.IsValid)
            {
                this.ReportErrors(validationResult.ErrorMessages);
            }

            this.ReportError("There was an error!");

            var deserializationErrors = this.VisualStudioProjectFileDeserializationErrorAggregator.GetErrors();

            var result = new Result<ProjectFile>(projectFile, deserializationErrors);
            return result;
        }

        public ProjectXElement ToProjectXElement(ProjectFile projectFile)
        {
            var projectXElement = new XElement(ProjectFileXmlElementName.Project);

            this.WriteProjectXElement(projectFile, projectXElement);

            // Finish.
            var typedProjectXElement = new ProjectXElement(projectXElement);
            return typedProjectXElement;
        }

        private void WriteProjectXElement(ProjectFile projectFile, XElement projectXElement)
        {
            if(projectFile.SDK.IsSet)
            {
                // Should always be set for a valid project, but don't assume.
                var sdkXAttribute = new XAttribute(ProjectFileXmlElementName.Sdk, Sdk.MicrosoftNetSdk);
                projectXElement.Add(sdkXAttribute);
            }

            // No need to check, since there will always be a property group XElement.
            var propertyGroupXElement = new XElement(ProjectFileXmlElementName.PropertyGroup);
            projectXElement.Add(propertyGroupXElement);

            this.WritePropertyGroupXElement(projectFile, propertyGroupXElement);

            if(projectFile.PackageReferences.Any())
            {
                var packageReferencesItemGroupXElement = new XElement(ProjectFileXmlElementName.ItemGroup);
                projectXElement.Add(packageReferencesItemGroupXElement);

                this.WritePackageReferencesItemGroupXElement(projectFile, packageReferencesItemGroupXElement);
            }

            if(projectFile.ProjectReferences.Any())
            {
                var projectReferencesItemGroupXElement = new XElement(ProjectFileXmlElementName.ItemGroup);
                projectXElement.Add(projectReferencesItemGroupXElement);

                this.WriteProjectReferencesItemGroupXElement(projectFile, projectReferencesItemGroupXElement);
            }
        }

        private void WritePropertyGroupXElement(ProjectFile projectFile, XElement propertyGroupXElement)
        {
            if(projectFile.TargetFramework.IsSet)
            {
                // Should always be set for a valid project, but don't assume.
                var targetFrameworkXElement = new XElement(ProjectFileXmlElementName.TargetFramework, projectFile.TargetFramework.Value.ToStringStandard());
                propertyGroupXElement.Add(targetFrameworkXElement);
            }

            if (projectFile.OutputType.IsSet)
            {
                var outputTypeXElement = new XElement(ProjectFileXmlElementName.OutputType, projectFile.OutputType.Value.ToStringStandard());
                propertyGroupXElement.Add(outputTypeXElement);
            }

            if (projectFile.LanguageVersion.IsSet)
            {
                var languageVersionXElement = new XElement(ProjectFileXmlElementName.LanguageVersion, projectFile.LanguageVersion.Value.ToStringProjectFileStandard());
                propertyGroupXElement.Add(languageVersionXElement);
            }

            if (projectFile.IsPackable.IsSet)
            {
                var isPackableXElement = new XElement(ProjectFileXmlElementName.IsPackable, projectFile.IsPackable.Value);
                propertyGroupXElement.Add(isPackableXElement);
            }
        }

        private void WritePackageReferencesItemGroupXElement(ProjectFile projectFile, XElement packageReferencesItemGroupXElement)
        {
            foreach (var packageReference in projectFile.PackageReferences)
            {
                var includeXAttribute = new XAttribute(ProjectFileXmlElementName.Include, packageReference.Name);
                var versionXAttribute = new XAttribute(ProjectFileXmlElementName.Version, packageReference.VersionString);

                var packageReferenceXElement = new XElement(ProjectFileXmlElementName.PackageReference, includeXAttribute, versionXAttribute);
                packageReferencesItemGroupXElement.Add(packageReferenceXElement);
            }
        }

        private void WriteProjectReferencesItemGroupXElement(ProjectFile projectFile, XElement projectReferencesItemGroupXElement)
        {
            foreach (var projectReference in projectFile.ProjectReferences)
            {
                var includeXAttribute = new XAttribute(ProjectFileXmlElementName.Include, projectReference.ProjectFilePath);

                var projectReferenceXElement = new XElement(ProjectFileXmlElementName.ProjectReference, includeXAttribute);
                projectReferencesItemGroupXElement.Add(projectReferenceXElement);
            }
        }
    }
}
