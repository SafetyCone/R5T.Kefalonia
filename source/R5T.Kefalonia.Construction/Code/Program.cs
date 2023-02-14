using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using R5T.Chalandri;
using R5T.Evosmos;
using R5T.Glenrothes;
using R5T.Liverpool;
using R5T.Lombardy;
using R5T.Magyar.IO;
using R5T.Magyar.Xml;


namespace R5T.Kefalonia.Construction
{
    class Program : AsyncHostedServiceProgramBase
    {
        static async Task Main(string[] args)
        {
            await HostedServiceProgram.RunAsync<Program, Startup>();
        }


        private IServiceProvider ServiceProvider { get; }
        private ITemporaryDirectoryFilePathProvider TemporaryDirectoryFilePathProvider { get; }
        private ITestingDataDirectoryContentPathsProvider TestingDataDirectoryContentPathsProvider { get; }
        private IVisualStudioProjectFileSerializer VisualStudioProjectFileSerializer { get; }


        public Program(IApplicationLifetime applicationLifetime,
            IServiceProvider serviceProvider,
            ITemporaryDirectoryFilePathProvider temporaryDirectoryFilePathProvider,
            ITestingDataDirectoryContentPathsProvider testingDataDirectoryContentPathsProvider,
            IVisualStudioProjectFileSerializer visualStudioProjectFileSerializer)
            : base(applicationLifetime)
        {
            this.ServiceProvider = serviceProvider;
            this.TemporaryDirectoryFilePathProvider = temporaryDirectoryFilePathProvider;
            this.TestingDataDirectoryContentPathsProvider = testingDataDirectoryContentPathsProvider;
            this.VisualStudioProjectFileSerializer = visualStudioProjectFileSerializer;
        }

        protected override async Task SubMainAsync()
        {
            await this.DeserializeExampleProjectFile();
            //await this.TestXmlWriter();
            //await this.CompareXElementsUsingDeepEquals();
            //await this.CompareXElementsUsingOrderIndependentEquals();
        }

        private async Task CompareXElementsUsingOrderIndependentEquals()
        {
            var expectedFilePath = this.TestingDataDirectoryContentPathsProvider.GetExampleVisualStudioProjectFilePath01();
            var actualFilePath = this.TemporaryDirectoryFilePathProvider.GetTemporaryDirectoryFilePath("ProjectFile01.csproj");

            var expectedXElement = XElement.Load(expectedFilePath);
            var actualXElement = XElement.Load(actualFilePath);

            var orderIndependentXElementEqualityComparer = this.ServiceProvider.GetRequiredService<OrderIndependentXElementEqualityComparer>();

            var shouldBeTrue = await orderIndependentXElementEqualityComparer.AreEqual(expectedXElement, expectedXElement);
            var shouldBeTrue2 = await orderIndependentXElementEqualityComparer.AreEqual(actualXElement, actualXElement);

            var shouldBeTrue3 = await orderIndependentXElementEqualityComparer.AreEqual(expectedXElement, actualXElement);
        }

        private async Task CompareXElementsUsingDeepEquals()
        {
            var expectedFilePath = this.TestingDataDirectoryContentPathsProvider.GetExampleVisualStudioProjectFilePath01();
            var actualFilePath = this.TemporaryDirectoryFilePathProvider.GetTemporaryDirectoryFilePath("ProjectFile01.csproj");

            var expectedXElement = XElement.Load(expectedFilePath);
            var actualXElement = XElement.Load(actualFilePath);

            var deepEqualsXElementEqualityComparer = this.ServiceProvider.GetRequiredService<DeepEqualsXElementEqualityComparer>();

            var shouldBeTrue = await deepEqualsXElementEqualityComparer.AreEqual(expectedXElement, expectedXElement);
            var shouldBeTrue2 = await deepEqualsXElementEqualityComparer.AreEqual(actualXElement, actualXElement);

            var shouldBeFalse = await deepEqualsXElementEqualityComparer.AreEqual(expectedXElement, actualXElement);
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

            var projectFile = await this.VisualStudioProjectFileSerializer.Deserialize(exampleVisualStudioProjectFilePath01);

            var testingDataDirectoryPathProvider = this.ServiceProvider.GetRequiredService<ITestingDataDirectoryPathProvider>();

            var testingDataDirectoryPath = testingDataDirectoryPathProvider.GetTestingDataDirectoryPath();

            var stringlyTypedPathOperator = this.ServiceProvider.GetRequiredService<IStringlyTypedPathOperator>();

            // Pretend to serialize to a file in the testing data directory to get the project reference relative file paths to be correct.
            var tempOutputFilePath01 = stringlyTypedPathOperator.GetFilePath(testingDataDirectoryPath, "ProjectFile01.csproj");

            var visualStudioProjectFileStreamSerializer = this.ServiceProvider.GetRequiredService<IVisualStudioProjectFileStreamSerializer>();

            using (var memoryStream = new MemoryStream())
            {
                await visualStudioProjectFileStreamSerializer.SerializeAsync(memoryStream, tempOutputFilePath01, projectFile);

                // Now actually serialize to the output file.
                var outputFilePath01 = this.TemporaryDirectoryFilePathProvider.GetTemporaryDirectoryFilePath("ProjectFile01.csproj");
                using (var fileStream = FileStreamHelper.NewWrite(outputFilePath01))
                {
                    fileStream.Write(memoryStream.ToArray());
                }
            }
        }
    }
}
