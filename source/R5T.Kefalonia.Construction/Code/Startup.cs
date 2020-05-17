using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using R5T.Chalandri.DropboxRivetTestingData;
using R5T.D0001.Standard;
using R5T.D0004.Standard;
using R5T.D0005.Standard;
using R5T.D0006;
using R5T.D0007.Standard;
using R5T.D0008.Standard;
using R5T.D0011.Standard;
using R5T.D0012.Standard;
using R5T.Evosmos.CDriveTemp;
using R5T.Richmond;
using R5T.Lombardy;

using R5T.Kefalonia.Common;
using R5T.Kefalonia.XElements;


namespace R5T.Kefalonia.Construction
{
    public class Startup : StartupBase
    {
        public Startup(ILogger<Startup> logger)
            : base(logger)
        {
        }

        protected override void ConfigureServicesBody(IServiceCollection services)
        {
            services
                .AddTemporaryDirectoryFilePathProvider()
                .AddTestingDataDirectoryContentPathsProvider()
                .AddSingleton<IFileNameOperator, FileNameOperator>()
                .AddSingleton<FunctionalityDirectoryNameProvider>()
                .AddSingleton<IFunctionalVisualStudioProjectFileSerializer, FunctionalVisualStudioProjectFileSerializer>()
                .AddGuidProvider()
                .AddSingleton<IMessagesOutputBaseDirectoryPathProvider, MessagesOutputBaseDirectoryPathProvider>()
                .AddSingleton<MessagesOutputFilePathProvider>()
                .AddNowUtcProvider()
                .AddProcessStartTimeUtcDirectoryNameProvider()
                .AddProcessStartTimeUtcProvider()
                .AddProgramNameDirectoryNameProvider()
                .AddProgramNameProvider()
                .AddSingleton<ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider>()
                .AddSingleton<ProjectFileDeserializationMessagesOutputFileNameProvider>()
                .AddSingleton<IRelativeFilePathsVisualStudioProjectFileSerializer, RelativeFilePathsVisualStudioProjectFileSerializer>()
                .AddDefaultStringlyTypedPathOperator<IStringlyTypedPathOperator>()
                .AddTimestampUtcDirectoryNameProvider()
                .AddSingleton<IVisualStudioProjectFileSerializer, VisualStudioProjectFileSerializer>()
                .AddSingleton<IVisualStudioProjectFileToXElementConverter, VisualStudioProjectFileToXElementConverter>()
                .AddVisualStudioProjectFileDeserializationSettings(settings =>
                {
                    settings.ThrowAtErrorOccurrence = false;
                    settings.ThrowIfAnyErrorAtEnd = false;
                    settings.ThrowIfInvalidProjectFile = false;
                })
                .AddSingleton<IVisualStudioProjectFileValidator, VisualStudioProjectFileValidator>()
                ;
        }
    }
}
