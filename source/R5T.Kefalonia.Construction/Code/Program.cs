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

        protected override Task SubMainAsync()
        {
            var exampleVisualStudioProjectFilePath01 = this.TestingDataDirectoryContentPathsProvider.GetExampleVisualStudioProjectFilePath01();

            var projectFile = this.VisualStudioProjectFileSerializer.Deserialize(exampleVisualStudioProjectFilePath01);

            var outputFilePath01 = this.TemporaryDirectoryFilePathProvider.GetTemporaryDirectoryFilePath("ProjectFile01.csproj");

            this.VisualStudioProjectFileSerializer.Serialize(outputFilePath01, projectFile);

            return Task.CompletedTask;
        }
    }
}
