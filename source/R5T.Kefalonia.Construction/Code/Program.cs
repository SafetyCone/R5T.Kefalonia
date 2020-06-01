using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using R5T.Chalandri;
using R5T.Evosmos;
using R5T.Liverpool;


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
        }

        //private async Task DeserializeAllProjectFilesIn

        private async Task DeserializeExampleProjectFile()
        {
            var exampleVisualStudioProjectFilePath01 = this.TestingDataDirectoryContentPathsProvider.GetExampleVisualStudioProjectFilePath01();

            var projectFile = await this.VisualStudioProjectFileSerializer.DeserializeAsync(exampleVisualStudioProjectFilePath01);

            var outputFilePath01 = this.TemporaryDirectoryFilePathProvider.GetTemporaryDirectoryFilePath("ProjectFile01.csproj");

            await this.VisualStudioProjectFileSerializer.SerializeAsync(outputFilePath01, projectFile);
        }
    }
}
