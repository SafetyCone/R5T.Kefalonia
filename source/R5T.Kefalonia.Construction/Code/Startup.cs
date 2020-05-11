using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using R5T.Chalandri.DropboxRivetTestingData;
using R5T.Evosmos.CDriveTemp;
using R5T.Richmond;

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
                .AddSingleton<IVisualStudioProjectFileSerializer, VisualStudioProjectFileSerializer>()
                .AddSingleton<IVisualStudioProjectFileToXElementConverter, VisualStudioProjectFileToXElementConverter>()
                ;
        }
    }
}
