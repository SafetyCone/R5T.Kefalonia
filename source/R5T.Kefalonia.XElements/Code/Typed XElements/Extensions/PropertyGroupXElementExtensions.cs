using System;

using R5T.T0006;
using R5T.T0005;

using R5T.Magyar;
using R5T.Ostersund;


namespace R5T.Kefalonia.XElements
{
    public static class PropertyGroupXElementExtensions
    {
        public static ProjectXElement GetProjectXElement(this PropertyGroupXElement propertyGroupXElement)
        {
            var projectXElement = propertyGroupXElement.Value.Parent.AsProject();
            return projectXElement;
        }

        public static string GetTargetFrameworkString(this PropertyGroupXElement propertyGroupXElement)
        {
            var targetFrameworkString = propertyGroupXElement.GetChildValue(ProjectFileXmlElementName.TargetFramework);
            return targetFrameworkString;
        }

        public static TargetFramework GetTargetFramework(this PropertyGroupXElement propertyGroupXElement)
        {
            var targetFrameworkString = propertyGroupXElement.GetTargetFrameworkString();

            // No need to check if the project HAS a TargetFramework, it must!
            var targetFramework = ProjectFileValues.ParseTargetFramework(targetFrameworkString);
            return targetFramework;
        }

        public static bool HasOutputType(this PropertyGroupXElement propertyGroupXElement, out OutputType outputType)
        {
            var hasOutput = propertyGroupXElement.HasChild(ProjectFileXmlElementName.OutputType, out var childXElement);
            if(hasOutput)
            {
                var outputTypeString = childXElement.Value;

                outputType = ProjectFileValues.ParseOutputType(outputTypeString);
            }
            else
            {
                outputType = OutputType.Unknown;
            }

            return hasOutput;
        }

        public static bool HasLanguageVersion(this PropertyGroupXElement propertyGroupXElement, out Version languageVersion)
        {
            var hasLanguageVersion = propertyGroupXElement.HasChild(ProjectFileXmlElementName.LanguageVersion, out var childXElement);
            if(hasLanguageVersion)
            {
                var languageVersionString = childXElement.Value;

                languageVersion = ProjectFileValues.ParseVersion(languageVersionString);
            }
            else
            {
                languageVersion = VersionHelper.None;
            }

            return hasLanguageVersion;
        }

        public static bool HasIsPackable(this PropertyGroupXElement propertyGroupXElement, out bool isPackable)
        {
            var hasIsPackable = propertyGroupXElement.HasChild(ProjectFileXmlElementName.IsPackable, out var childXElement);
            if(hasIsPackable)
            {
                var isPackableString = childXElement.Value;

                isPackable = ProjectFileValues.ParseBoolean(isPackableString);   
            }
            else
            {
                isPackable = false; // Dummy value.
            }

            return hasIsPackable;
        }
    }
}
