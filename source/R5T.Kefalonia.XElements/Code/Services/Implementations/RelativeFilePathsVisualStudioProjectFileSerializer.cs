using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

using R5T.D0010;
using R5T.Gloucester.Types;
using R5T.Lombardy;
using R5T.Magyar.IO;

using R5T.Kefalonia.Common;
using R5T.VisualStudioProjectFileStuff;


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

            // Save to a StringWriter, then adjust the string to have the desired extra line breaks before serialization.
            using (var fileStream = FileStreamHelper.NewWrite(filePath, overwrite)) // I want to use this overwrite logic.
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriterHelper.New(stringWriter))
                {
                    projectXElement.Value.WriteTo(xmlWriter); // No async version.
                }

                var text = stringWriter.ToString();

                var prefixNewLineMatchPatterns = new[]
                {
                    $@"^\s*<{ProjectFileXmlElementName.ItemGroup}>",
                    $@"^\s*<{ProjectFileXmlElementName.PropertyGroup}>",
                    $@"^\s*</{ProjectFileXmlElementName.Project}>",
                };

                var modifiedText = text;
                foreach(var prefixNewLineMatchPattern in prefixNewLineMatchPatterns)
                {
                    var matches = Regex.Matches(modifiedText, prefixNewLineMatchPattern, RegexOptions.Multiline); // Include beginning-of-line whitespace.
                    var numberOfMatches = matches.Count;

                    var substrings = new string[numberOfMatches]; // Not +1 since we will deal with the last sub-string separately.

                    var startIndex = 0;
                    for (int iMatch = 0; iMatch < numberOfMatches; iMatch++)
                    {
                        var match = matches[iMatch];

                        var substring = modifiedText.Substring(startIndex, match.Index);
                        substrings[iMatch] = substring;
                    }

                    var lastMatch = matches[numberOfMatches - 1];
                    var lastSubstring = lastMatch.Index + lastMatch.Length == modifiedText.Length + 1 ? String.Empty : modifiedText.Substring(lastMatch.Index + lastMatch.Length);

                    var stringBuilder = new StringBuilder();

                    for (int iMatch = 0; iMatch < numberOfMatches; iMatch++)
                    {
                        var substring = substrings[iMatch];
                        var match = matches[iMatch];

                        var replacement = "\r\n" + match.Value;

                        stringBuilder.Append(substring);
                        stringBuilder.Append(replacement);
                    }

                    stringBuilder.Append(lastSubstring);

                    modifiedText = stringBuilder.ToString();
                }

                using (var textWriter = new StreamWriter(fileStream))
                {
                    await textWriter.WriteAsync(modifiedText);
                }
            }
        }
    }
}
