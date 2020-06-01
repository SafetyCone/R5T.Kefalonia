using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using R5T.Chalandri;
using R5T.Chalandri.DropboxRivetTestingData;
using R5T.D0001;
using R5T.D0001.Standard;
using R5T.D0004;
using R5T.D0004.Standard;
using R5T.D0005;
using R5T.D0005.Standard;
using R5T.D0006;
using R5T.D0007;
using R5T.D0007.Standard;
using R5T.D0008;
using R5T.D0008.Standard;
using R5T.D0011;
using R5T.D0011.Standard;
using R5T.D0012;
using R5T.D0012.Standard;
using R5T.Dacia;
using R5T.Evosmos;
using R5T.Evosmos.CDriveTemp;
using R5T.Richmond;
using R5T.Lombardy;
using R5T.Lombardy.Standard;

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
            (
            IServiceAction<IDirectoryNameOperator> _,
            IServiceAction<IDirectorySeparatorOperator> _,
            IServiceAction<IFileExtensionOperator> _,
            IServiceAction<IFileNameOperator> fileNameOperatorAction,
            IServiceAction<IStringlyTypedPathOperator> stringlyTypedPathOperatorAction
            ) = services.AddStringlyTypedPathRelatedOperatorsAction();

            IServiceAction<FunctionalityDirectoryNameProvider> functionalityDirectoryNameProviderAction = ServiceAction<FunctionalityDirectoryNameProvider>.New(() => services.AddSingleton<FunctionalityDirectoryNameProvider>());
            IServiceAction<IGuidProvider> guidProviderAction = services.AddGuidProviderAction();
            IServiceAction<INowUtcProvider> nowUtcProviderAction = services.AddNowUtcProviderAction();
            IServiceAction<IProcessStartTimeUtcProvider> processStartTimeProviderAction = services.AddProcessStartTimeUtcProviderAction();
            IServiceAction<IProgramNameProvider> programNameProviderAction = services.AddProgramNameProviderAction();
            IServiceAction<ITemporaryDirectoryFilePathProvider> temporaryDirectoryFilePathProviderAction = services.AddTemporaryDirectoryFilePathProviderAction();
            IServiceAction<ITestingDataDirectoryContentPathsProvider> testingDataDirectoryContentPathsProviderAction = services.AddTestingDataDirectoryContentPathsProviderAction();
            IServiceAction<ITimestampUtcDirectoryNameProvider> timestampUtcDirectoryNameProviderAction = services.AddTimestampUtcDirectoryNameProviderAction();
            IServiceAction<IVisualStudioProjectFileDeserializationSettings> visualStudioProjectFileDeserializationSettingsAction = ServiceAction<IVisualStudioProjectFileDeserializationSettings>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddVisualStudioProjectFileDeserializationSettings(settings =>
                    {
                        settings.ThrowAtErrorOccurrence = false;
                        settings.ThrowIfAnyErrorAtEnd = false;
                        settings.ThrowIfInvalidProjectFile = false;
                    });
            });

            IServiceAction<IProcessStartTimeUtcDirectoryNameProvider> processStartTimeUtcDirectoryNameProviderAction = services.AddProcessStartTimeUtcDirectoryNameProviderAction(
                processStartTimeProviderAction,
                timestampUtcDirectoryNameProviderAction);
            IServiceAction<IProgramNameDirectoryNameProvider> programNameDirectoryNameProviderAction = services.AddProgramNameDirectoryNameProviderAction(
                programNameProviderAction);
            IServiceAction<IMessagesOutputBaseDirectoryPathProvider> messagesOutputBaseDirectoryPathProviderAction = ServiceAction<IMessagesOutputBaseDirectoryPathProvider>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IMessagesOutputBaseDirectoryPathProvider, MessagesOutputBaseDirectoryPathProvider>()
                    .Run(temporaryDirectoryFilePathProviderAction)
                    .Run(stringlyTypedPathOperatorAction)
                    ;
            });
            IServiceAction<ProjectFileDeserializationMessagesOutputFileNameProvider> projectFileDeserializationMessagesOutputFileNameProviderAction = ServiceAction<ProjectFileDeserializationMessagesOutputFileNameProvider>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<ProjectFileDeserializationMessagesOutputFileNameProvider>()
                    .Run(fileNameOperatorAction)
                    .Run(guidProviderAction)
                    .Run(stringlyTypedPathOperatorAction)
                    ;
            });
            IServiceAction<IVisualStudioProjectFileToXElementConverter> visualStudioProjectFileToXElementConverter = ServiceAction<IVisualStudioProjectFileToXElementConverter>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IVisualStudioProjectFileToXElementConverter, VisualStudioProjectFileToXElementConverter>()
                    .Run(nowUtcProviderAction)
                    .Run(visualStudioProjectFileDeserializationSettingsAction)
                    ;
            });
            IServiceAction<IVisualStudioProjectFileValidator> visualStudioProjectFileValidatorAction = ServiceAction<IVisualStudioProjectFileValidator>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IVisualStudioProjectFileValidator, VisualStudioProjectFileValidator>()
                    .Run(nowUtcProviderAction)
                    ;
            });

            IServiceAction<ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider> programNameStartTimeFunctionalityMessagesOutputDirectoryPathProviderAction = ServiceAction<ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider>()
                    .Run(functionalityDirectoryNameProviderAction)
                    .Run(messagesOutputBaseDirectoryPathProviderAction)
                    .Run(processStartTimeUtcDirectoryNameProviderAction)
                    .Run(programNameDirectoryNameProviderAction)
                    .Run(stringlyTypedPathOperatorAction)
                    ;
            });
            IServiceAction<IRelativeFilePathsVisualStudioProjectFileSerializer> relativeFilePathsVisualStudioProjectFileSerializerAction = ServiceAction<IRelativeFilePathsVisualStudioProjectFileSerializer>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IRelativeFilePathsVisualStudioProjectFileSerializer, RelativeFilePathsVisualStudioProjectFileSerializer>()
                    .Run(stringlyTypedPathOperatorAction)
                    .Run(visualStudioProjectFileToXElementConverter)
                    ;
            });


            IServiceAction<IFunctionalVisualStudioProjectFileSerializer> functionalVisualStudioProjectFileSerializerAction = ServiceAction<IFunctionalVisualStudioProjectFileSerializer>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IFunctionalVisualStudioProjectFileSerializer, FunctionalVisualStudioProjectFileSerializer>()
                    .Run(nowUtcProviderAction)
                    .Run(relativeFilePathsVisualStudioProjectFileSerializerAction)
                    .Run(stringlyTypedPathOperatorAction)
                    .Run(visualStudioProjectFileDeserializationSettingsAction)
                    .Run(visualStudioProjectFileValidatorAction)
                    ;
            });
            IServiceAction<MessagesOutputFilePathProvider> messagesOutputFilePathProviderAction = ServiceAction<MessagesOutputFilePathProvider>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<MessagesOutputFilePathProvider>()
                    .Run(projectFileDeserializationMessagesOutputFileNameProviderAction)
                    .Run(programNameStartTimeFunctionalityMessagesOutputDirectoryPathProviderAction)
                    .Run(stringlyTypedPathOperatorAction)
                    ;
            });

            IServiceAction<IVisualStudioProjectFileSerializer> visualStudioProjectFileSerializerAction = ServiceAction<IVisualStudioProjectFileSerializer>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IVisualStudioProjectFileSerializer, VisualStudioProjectFileSerializer>()
                    .Run(functionalVisualStudioProjectFileSerializerAction)
                    .Run(messagesOutputFilePathProviderAction)
                    .Run(stringlyTypedPathOperatorAction)
                    ;
            });


            services
                .Run(fileNameOperatorAction)
                .Run(functionalityDirectoryNameProviderAction)
                .Run(functionalVisualStudioProjectFileSerializerAction)
                .Run(guidProviderAction)
                .Run(messagesOutputBaseDirectoryPathProviderAction)
                .Run(messagesOutputFilePathProviderAction)
                .Run(nowUtcProviderAction)
                .Run(processStartTimeUtcDirectoryNameProviderAction)
                .Run(processStartTimeProviderAction)
                .Run(programNameDirectoryNameProviderAction)
                .Run(programNameProviderAction)
                .Run(programNameStartTimeFunctionalityMessagesOutputDirectoryPathProviderAction)
                .Run(projectFileDeserializationMessagesOutputFileNameProviderAction)
                .Run(relativeFilePathsVisualStudioProjectFileSerializerAction)
                .Run(stringlyTypedPathOperatorAction)
                .Run(temporaryDirectoryFilePathProviderAction)
                .Run(testingDataDirectoryContentPathsProviderAction)
                .Run(timestampUtcDirectoryNameProviderAction)
                .Run(visualStudioProjectFileSerializerAction)
                .Run(visualStudioProjectFileToXElementConverter)
                .Run(visualStudioProjectFileDeserializationSettingsAction)
                .Run(visualStudioProjectFileValidatorAction)
                ;
        }

        private static (int a, (int ba, string bb), string c) Test1()
        {
            return (a: 1, Startup.Test2(), c: "c");
        }

        private static (int ba, string bb) Test2()
        {
            return (ba: 3, bb: "bb");
        }
    }
}
