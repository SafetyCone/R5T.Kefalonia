using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

using R5T.D0001;
using R5T.T0002;
using R5T.T0006;
using R5T.D0010;

using R5T.Angleterria;
using R5T.Ostersund.Extensions;
using R5T.Gloucester.Types;

using R5T.Kefalonia.Common;using R5T.T0064;


namespace R5T.Kefalonia.XElements
{[ServiceImplementationMarker]
    public class VisualStudioProjectFileToXElementConverter : IVisualStudioProjectFileToXElementConverter,IServiceImplementation
    {
        #region Static

        // Cannot be unified with attributes since XAttribute and XElement only share an ancestor at XObject, which lacks a Name property.
        private static async Task PerformLookupElements(IEnumerable<XElement> elementSource, Dictionary<string, Func<XElement, ProjectFile, IMessageSink, Task>> handlers, Func<XElement, ProjectFile, IMessageSink, Task> unknownHandler, ProjectFile projectFile, IMessageSink messageSink)
        {
            // Sequential task operations.
            foreach (var element in elementSource)
            {
                var elementName = element.Name.LocalName;
                if (handlers.ContainsKey(elementName))
                {
                    await handlers[elementName](element, projectFile, messageSink);
                }
                else
                {
                    await unknownHandler(element, projectFile, messageSink);
                }
            }
        }

        // Cannot be unified with elements since XAttribute and XElement only share an ancestor at XObject, which lacks a Name property.
        private static async Task PerformLookupAttributes(IEnumerable<XAttribute> attributeSource, Dictionary<string, Func<XAttribute, ProjectFile, IMessageSink, Task>> handlers, Func<XAttribute, ProjectFile, IMessageSink, Task> unknownHandler, ProjectFile projectFile, IMessageSink messageSink)
        {
            // Sequential task operations.
            foreach (var attribute in attributeSource)
            {
                var elementName = attribute.Name.LocalName;
                if (handlers.ContainsKey(elementName))
                {
                    await handlers[elementName](attribute, projectFile, messageSink);
                }
                else
                {
                    await unknownHandler(attribute, projectFile, messageSink);
                }
            }
        }

        #endregion


        private INowUtcProvider NowUtcProvider { get; }
        private IVisualStudioProjectFileDeserializationSettings VisualStudioProjectFileDeserializationSettings { get; }

        private Dictionary<string, Func<XAttribute, ProjectFile, IMessageSink, Task>> ProjectAttributeHandlers { get; set; }
        private Dictionary<string, Func<XElement, ProjectFile, IMessageSink, Task>> ProjectChildElementHandlers { get; set; }
        private Dictionary<string, Func<XElement, ProjectFile, IMessageSink, Task>> PropertyGroupChildElementHandlers { get; set; }
        private Dictionary<string, Func<XElement, ProjectFile, IMessageSink, Task>> ItemGroupChildElementHandlers { get; set; }


        public VisualStudioProjectFileToXElementConverter(
            INowUtcProvider nowUtcProvider,
            IVisualStudioProjectFileDeserializationSettings visualStudioProjectFileDeserializationSettings)
        {
            this.NowUtcProvider = nowUtcProvider;
            this.VisualStudioProjectFileDeserializationSettings = visualStudioProjectFileDeserializationSettings;

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

        private async Task ReportError(string message, IMessageSink messageSink)
        {
            var nowUtc = await this.NowUtcProvider.GetNowUtcAsync();

            await messageSink.AddErrorMessageAsync(nowUtc, message);

            if (this.VisualStudioProjectFileDeserializationSettings.ThrowAtErrorOccurrence)
            {
                throw new Exception(message);
            }
        }

        private async Task ReportErrors(IEnumerable<string> messages, IMessageSink messageSink)
        {
            var nowUtc = await this.NowUtcProvider.GetNowUtcAsync();

            foreach (var message in messages)
            {
                await messageSink.AddErrorMessageAsync(nowUtc, message);
            }

            if (this.VisualStudioProjectFileDeserializationSettings.ThrowAtErrorOccurrence)
            {
                throw new Exception("There were errors.");
            }
        }

        private async Task ProjectElementHandler(XElement xElement, ProjectFile projectFile, IMessageSink messageSink)
        {
            await VisualStudioProjectFileToXElementConverter.PerformLookupAttributes(xElement.Attributes(), this.ProjectAttributeHandlers, this.UnknownProjectAttributeHandler, projectFile, messageSink);
            await VisualStudioProjectFileToXElementConverter.PerformLookupElements(xElement.Elements(), this.ProjectChildElementHandlers, this.UnknownProjectChildElementHandler, projectFile, messageSink);
        }

        private Dictionary<string, Func<XAttribute, ProjectFile, IMessageSink, Task>> GetProjectAttributeHandlers()
        {
            var projectAttributeHandlers = new Dictionary<string, Func<XAttribute, ProjectFile, IMessageSink, Task>>
            {
                {  ProjectFileXmlElementName.Sdk, this.HandleSdkProjectAttribute },
            };

            return projectAttributeHandlers;
        }

        private Task UnknownProjectAttributeHandler(XAttribute xAttribute, ProjectFile projectFile, IMessageSink messageSink)
        {
            var message = $"Unnown project attribute: {xAttribute.Name}";

            return this.ReportError(message, messageSink);
        }

        private Task HandleSdkProjectAttribute(XAttribute sdkAttribute, ProjectFile projectFile, IMessageSink messageSink)
        {
            projectFile.SDK = sdkAttribute.Value;

            return Task.CompletedTask;
        }

        private Dictionary<string, Func<XElement, ProjectFile, IMessageSink, Task>> GetProjectChildElementHandlers()
        {
            var projectChildElementHandlers = new Dictionary<string, Func<XElement, ProjectFile, IMessageSink, Task>>()
            {
                { ProjectFileXmlElementName.PropertyGroup, this.PropertyGroupElementHandler },
                { ProjectFileXmlElementName.ItemGroup, this.ItemGroupElementHandler },
            };

            return projectChildElementHandlers;
        }

        private Task UnknownProjectChildElementHandler(XElement unknownChild, ProjectFile projectFile, IMessageSink messageSink)
        {
            var message = $"Unnown project child element: {unknownChild.Name}";

            return this.ReportError(message, messageSink);
        }

        public Task PropertyGroupElementHandler(XElement xPropertyGroup, ProjectFile projectFile, IMessageSink messageSink)
        {
            return VisualStudioProjectFileToXElementConverter.PerformLookupElements(xPropertyGroup.Elements(), this.PropertyGroupChildElementHandlers, this.UnknownPropertyGroupChildElementHandler, projectFile, messageSink);
        }

        public Task ItemGroupElementHandler(XElement xItemGroup, ProjectFile projectFile, IMessageSink messageSink)
        {
            return VisualStudioProjectFileToXElementConverter.PerformLookupElements(xItemGroup.Elements(), this.ItemGroupChildElementHandlers, this.UnknownItemGroupChildElementHandler, projectFile, messageSink);
        }

        private Dictionary<string, Func<XElement, ProjectFile, IMessageSink, Task>> GetPropertyGroupChildElementHandlers()
        {
            var propertyGroupChildElementHandlers = new Dictionary<string, Func<XElement, ProjectFile, IMessageSink, Task>>()
            {
                { ProjectFileXmlElementName.GenerateDocumentationFile, this.GenerateDocumentationFileElementHandler },
                { ProjectFileXmlElementName.IsPackable, this.IsPackableElementHandler },
                { ProjectFileXmlElementName.LanguageVersion, this.LanguageVersionElementHandler },
                { ProjectFileXmlElementName.OutputType, this.OutputTypeElementHandler },
                { ProjectFileXmlElementName.TargetFramework, this.TargetFrameworkElementHandler },
            };

            return propertyGroupChildElementHandlers;
        }

        private Task UnknownPropertyGroupChildElementHandler(XElement unknownChild, ProjectFile projectFile, IMessageSink messageSink)
        {
            var message = $"Unnown property group child element: {unknownChild.Name}";

            return this.ReportError(message, messageSink);
        }

        private Task GenerateDocumentationFileElementHandler(XElement xGenerateDocumentationFile, ProjectFile projectFile, IMessageSink messageSink)
        {
            projectFile.GenerateDocumentationFile = ProjectFileValues.ParseBoolean(xGenerateDocumentationFile.Value);

            return Task.CompletedTask;
        }

        private Task IsPackableElementHandler(XElement xIsPackable, ProjectFile projectFile, IMessageSink messageSink)
        {
            projectFile.IsPackable = ProjectFileValues.ParseBoolean(xIsPackable.Value);

            return Task.CompletedTask;
        }

        private Task LanguageVersionElementHandler(XElement xLanguageVersion, ProjectFile projectFile, IMessageSink messageSink)
        {
            projectFile.LanguageVersion = ProjectFileValues.ParseVersion(xLanguageVersion.Value);

            return Task.CompletedTask;
        }

        private Task NoWarnElementHandler(XElement xNoWarn, ProjectFile projectFile, IMessageSink messageSink)
        {
            projectFile.NoWarn = ProjectFileValues.ParseNoWarn(xNoWarn.Value);

            return Task.CompletedTask;
        }

        private Task OutputTypeElementHandler(XElement xOutputType, ProjectFile projectFile, IMessageSink messageSink)
        {
            projectFile.OutputType = ProjectFileValues.ParseOutputType(xOutputType.Value);

            return Task.CompletedTask;
        }

        private Task TargetFrameworkElementHandler(XElement xTargetFramework, ProjectFile projectFile, IMessageSink messageSink)
        {
            projectFile.TargetFramework = ProjectFileValues.ParseTargetFramework(xTargetFramework.Value);

            return Task.CompletedTask;
        }

        private Dictionary<string, Func<XElement, ProjectFile, IMessageSink, Task>> GetItemGroupChildElementHandlers()
        {
            var itemGroupChildElementHandlers = new Dictionary<string, Func<XElement, ProjectFile, IMessageSink, Task>>()
            {
                { ProjectFileXmlElementName.PackageReference, this.PackageReferenceElementHandler },
                { ProjectFileXmlElementName.ProjectReference, this.ProjectReferenceElementHandler },
            };

            return itemGroupChildElementHandlers;
        }

        private Task UnknownItemGroupChildElementHandler(XElement unknownChild, ProjectFile projectFile, IMessageSink messageSink)
        {
            var message = $"Unnown item group child element: {unknownChild.Name}";

            return this.ReportError(message, messageSink);
        }

        private Task PackageReferenceElementHandler(XElement xPackageReference, ProjectFile projectFile, IMessageSink messageSink)
        {
            var packageName = xPackageReference.Attribute(ProjectFileXmlElementName.Include).Value;
            var packageVersionString = xPackageReference.Attribute(ProjectFileXmlElementName.Version).Value;

            var packageReference = new PackageReference(packageName, packageVersionString);

            projectFile.PackageReferences.Add(packageReference);

            return Task.CompletedTask;
        }

        private Task ProjectReferenceElementHandler(XElement xProjectReference, ProjectFile projectFile, IMessageSink messageSink)
        {
            var projectReferenceRelativePath = xProjectReference.Attribute(ProjectFileXmlElementName.Include).Value;

            var projectReference = new ProjectReference(projectReferenceRelativePath);

            projectFile.ProjectReferences.Add(projectReference);

            return Task.CompletedTask;
        }

        public async Task<ProjectFile> ToProjectFile(ProjectXElement projectXElement, IMessageSink messageSink)
        {
            var projectFile = new ProjectFile();

            await this.ProjectElementHandler(projectXElement.Value, projectFile, messageSink);

            return projectFile;
        }

        public Task<ProjectXElement> ToProjectXElement(ProjectFile projectFile, IMessageSink messageSink)
        {
            var projectXElement = new XElement(ProjectFileXmlElementName.Project);

            this.WriteProjectXElement(projectFile, projectXElement);

            // Finish.
            var typedProjectXElement = new ProjectXElement(projectXElement);
            return Task.FromResult(typedProjectXElement);
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
