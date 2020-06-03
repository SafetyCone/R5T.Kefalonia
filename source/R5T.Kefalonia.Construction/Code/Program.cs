using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.Extensions.Hosting;

using R5T.Chalandri;
using R5T.Evosmos;
using R5T.Liverpool;
using R5T.Magyar.Extensions;
using R5T.Magyar.IO;


namespace R5T.Kefalonia.Construction
{
    class Program : AsyncHostedServiceProgramBase
    {
        static async Task Main(string[] args)
        {
            await HostedServiceProgram.RunAsync<Program, Startup>();
        }


        private ITemporaryDirectoryFilePathProvider TemporaryDirectoryFilePathProvider { get; }
        private ITestingDataDirectoryContentPathsProvider TestingDataDirectoryContentPathsProvider { get; }
        private IVisualStudioProjectFileSerializer VisualStudioProjectFileSerializer { get; }


        public Program(IApplicationLifetime applicationLifetime,
            ITemporaryDirectoryFilePathProvider temporaryDirectoryFilePathProvider,
            ITestingDataDirectoryContentPathsProvider testingDataDirectoryContentPathsProvider,
            IVisualStudioProjectFileSerializer visualStudioProjectFileSerializer)
            : base(applicationLifetime)
        {
            this.TemporaryDirectoryFilePathProvider = temporaryDirectoryFilePathProvider;
            this.TestingDataDirectoryContentPathsProvider = testingDataDirectoryContentPathsProvider;
            this.VisualStudioProjectFileSerializer = visualStudioProjectFileSerializer;
        }

        protected override async Task SubMainAsync()
        {
            await this.DeserializeExampleProjectFile();
            //await this.TestXmlWriter();
        }

        private Task TestXmlWriter()
        {
            var projectFilePath = this.TestingDataDirectoryContentPathsProvider.GetExampleVisualStudioProjectFilePath01();
            var outputProjectFilePath = this.TemporaryDirectoryFilePathProvider.GetTemporaryDirectoryFilePath("ProjectFile01.csproj");

            var xElement = XElement.Load(projectFilePath); // No async version.

            //using (var writer = new StreamWriter(outputProjectFilePath))
            //using (var customXmlWriter = new CustomXmlWriter(writer))
            //using (var xmlWriter = XmlWriterHelper.New(outputProjectFilePath))
            //using (var customXmlWriter = new CustomXmlWriter(xmlWriter))
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriterHelper.New(stringWriter))
                {
                    //writer.AutoFlush = true;

                    xElement.Save(xmlWriter);
                    //xElement.Save(customXmlWriter);
                }

                var text = stringWriter.ToString();

                var matches = Regex.Matches(text, @"^\s*<ItemGroup>", RegexOptions.Multiline);
                foreach (Match match in matches)
                {
                    var prefix = text.Substring(0, match.Index);
                    var replacement = "\n" + match.Value;
                    var suffix = text.Substring(match.Index + match.Length);

                    var newText = prefix + replacement + suffix;
                    text = newText;
                }

                File.WriteAllText(outputProjectFilePath, text);
            }

            return Task.CompletedTask;
        }

        private async Task DeserializeExampleProjectFile()
        {
            var exampleVisualStudioProjectFilePath01 = this.TestingDataDirectoryContentPathsProvider.GetExampleVisualStudioProjectFilePath01();

            var projectFile = await this.VisualStudioProjectFileSerializer.DeserializeAsync(exampleVisualStudioProjectFilePath01);

            var outputFilePath01 = this.TemporaryDirectoryFilePathProvider.GetTemporaryDirectoryFilePath("ProjectFile01.csproj");

            await this.VisualStudioProjectFileSerializer.SerializeAsync(outputFilePath01, projectFile);
        }
    }
}
