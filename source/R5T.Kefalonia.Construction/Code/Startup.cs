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
using R5T.D0010;
using R5T.D0010.Default;
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
            var isConstruction = true; // Perhaps get from service or configuration.

            // -1
            (
            IServiceAction<IDirectoryNameOperator> _,
            IServiceAction<IDirectorySeparatorOperator> _,
            IServiceAction<IFileExtensionOperator> _,
            IServiceAction<IFileNameOperator> fileNameOperatorAction,
            IServiceAction<IStringlyTypedPathOperator> stringlyTypedPathOperatorAction
            ) = services.AddStringlyTypedPathRelatedOperatorsAction();

            // 0
            IServiceAction<DeepEqualsXElementEqualityComparer> deepEqualsXElementEqualityComparerAction = ServiceAction<DeepEqualsXElementEqualityComparer>.New(() => services.AddSingleton<DeepEqualsXElementEqualityComparer>());
            IServiceAction<FunctionalityDirectoryNameProvider> functionalityDirectoryNameProviderAction = ServiceAction<FunctionalityDirectoryNameProvider>.New(() => services.AddSingleton<FunctionalityDirectoryNameProvider>());
            IServiceAction<IGuidProvider> guidProviderAction = services.AddGuidProviderAction();
            IServiceAction<IMessageFormatter> messageFormatterAction = ServiceAction<IMessageFormatter>.New(serviceCollection =>
            {
                serviceCollection
                    .AddSingleton<IMessageFormatter, MessageFormatter>()
                    ;
            });
            //IServiceAction<IMessageRepository> messageRepositoryAction = ServiceAction<IMessageRepository>.New(serviceCollection => serviceCollection.AddSingleton<IMessageRepository, InMemoryMessageRepository>()); // Adds the default in-memory message repository.
            //IServiceAction<IMessageSink> messageSinkAction = ServiceAction<IMessageSink>.New(
            //    serviceCollection => serviceCollection.AddSingleton<IMessageSink>(
            //        serviceProvider =>
            //        {
            //            var programStartTimeSpecificMessagesOutputDirectoryPathProvider = serviceProvider.GetRequiredService<IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider>();
            //            var programStartTimeSpecificMessagesOutputDirectoryPath = programStartTimeSpecificMessagesOutputDirectoryPathProvider.GetProgramStartTimeSpecificMessagesOutputDirectoryPathAsync().Result;

            //            var stringlyTypedPathOperator = serviceProvider
            //        })); // No async Add() methods allowed!
            IServiceAction<IMessageSinkProvider> messageSinkProviderAction = ServiceAction<IMessageSinkProvider>.New(() => services.AddSingleton<IMessageSinkProvider, DefaultMessageSinkProvider>()); // One message sink provider for the whole application.
            IServiceAction<INowUtcProvider> nowUtcProviderAction = services.AddNowUtcProviderAction();
            IServiceAction<OrderIndependentXElementEqualityComparer> orderIndependentXElementEqualityComparerAction = ServiceAction<OrderIndependentXElementEqualityComparer>.New(() => services.AddSingleton<OrderIndependentXElementEqualityComparer>());
            IServiceAction<IProcessStartTimeUtcProvider> processStartTimeUtcProviderAction = services.AddProcessStartTimeUtcProviderAction();
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
                        settings.ThrowIfAnyErrorAtEnd = true;
                        settings.ThrowIfInvalidProjectFile = false;
                    });
            });

            // 1
            IServiceAction<IProcessStartTimeUtcDirectoryNameProvider> processStartTimeUtcDirectoryNameProviderAction = services.AddProcessStartTimeUtcDirectoryNameProviderAction(
                processStartTimeUtcProviderAction,
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
            IServiceAction<IProjectFileDeserializationMessagesOutputFileNameProvider> projectFileDeserializationMessagesOutputFileNameProviderAction = 
                isConstruction
                ? ServiceAction<IProjectFileDeserializationMessagesOutputFileNameProvider>.New((serviceCollection) =>
                {
                    serviceCollection
                        .AddSingleton<IProjectFileDeserializationMessagesOutputFileNameProvider, ConstructionTimeProjectFileDeserializationMessagesOutputFileNameProvider>()
                        .Run(fileNameOperatorAction)
                        .Run(stringlyTypedPathOperatorAction)
                        ;
                })
                : ServiceAction<IProjectFileDeserializationMessagesOutputFileNameProvider>.New((serviceCollection) =>
                {
                    serviceCollection
                        .AddSingleton<IProjectFileDeserializationMessagesOutputFileNameProvider, ProjectFileDeserializationMessagesOutputFileNameProvider>()
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

            // 2
            IServiceAction<IProgramSpecificMessagesOutputDirectoryPathProvider> programSpecificMessagesOutputDirectoryPathProviderAction = ServiceAction<IProgramSpecificMessagesOutputDirectoryPathProvider>.New(serviceCollection =>
            {
                serviceCollection
                    .AddSingleton<IProgramSpecificMessagesOutputDirectoryPathProvider, ProgramSpecificMessagesOutputDirectoryPathProvider>()
                    .Run(messagesOutputBaseDirectoryPathProviderAction)
                    .Run(programNameDirectoryNameProviderAction)
                    .Run(stringlyTypedPathOperatorAction)
                    ;
            });
            IServiceAction<IRelativeFilePathsVisualStudioProjectFileStreamSerializer> relativeFilePathsVisualStudioProjectFileSerializerAction = ServiceAction<IRelativeFilePathsVisualStudioProjectFileStreamSerializer>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IRelativeFilePathsVisualStudioProjectFileStreamSerializer, RelativeFilePathsVisualStudioProjectFileStreamSerializer>()
                    .Run(stringlyTypedPathOperatorAction)
                    .Run(visualStudioProjectFileToXElementConverter)
                    ;
            });

            // 3
            IServiceAction<IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider> programNameStartTimeMessagesOutputDirectoryPathProviderAction =
                isConstruction
                ? ServiceAction<IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider>.New(serviceCollection =>
                {
                    serviceCollection
                        .AddSingleton<IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider, ConstructionTimeMessagesOutputDirectoryPathProvider>()
                        .Run(programSpecificMessagesOutputDirectoryPathProviderAction)
                        .Run(stringlyTypedPathOperatorAction)
                        ;
                })
                : ServiceAction<IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider>.New(serviceCollection =>
                {
                    serviceCollection
                        .AddSingleton<IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider, ProgramNameStartTimeMessagesOutputDirectoryPathProvider>()
                        .Run(programSpecificMessagesOutputDirectoryPathProviderAction)
                        .Run(processStartTimeUtcDirectoryNameProviderAction)
                        .Run(stringlyTypedPathOperatorAction)
                        ;
                })
                ;

            // 4
            IServiceAction<IFunctionalitySpecificMessagesOutputDirectoryPathProvider> programNameStartTimeFunctionalityMessagesOutputDirectoryPathProviderAction = ServiceAction<IFunctionalitySpecificMessagesOutputDirectoryPathProvider>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IFunctionalitySpecificMessagesOutputDirectoryPathProvider, ProgramNameStartTimeFunctionalityMessagesOutputDirectoryPathProvider>()
                    .Run(functionalityDirectoryNameProviderAction)
                    .Run(programNameStartTimeMessagesOutputDirectoryPathProviderAction)
                    .Run(stringlyTypedPathOperatorAction)
                    ;
            });

            // 5
            IServiceAction<IVisualStudioProjectFileSerializerMessagesOutputFilePathProvider> visualStudioProjectFileSerializerMessagesOutputFilePathProviderAction = ServiceAction<IVisualStudioProjectFileSerializerMessagesOutputFilePathProvider>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IVisualStudioProjectFileSerializerMessagesOutputFilePathProvider, VisualStudioProjectFileSerializerMessagesOutputFilePathProvider>()
                    .Run(projectFileDeserializationMessagesOutputFileNameProviderAction)
                    .Run(programNameStartTimeFunctionalityMessagesOutputDirectoryPathProviderAction)
                    .Run(stringlyTypedPathOperatorAction)
                    ;
            });

            // 6
            IServiceAction<IFunctionalVisualStudioProjectFileStreamSerializer> functionalVisualStudioProjectFileSerializerAction = ServiceAction<IFunctionalVisualStudioProjectFileStreamSerializer>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IFunctionalVisualStudioProjectFileStreamSerializer, FunctionalVisualStudioProjectFileStreamSerializer>()
                    .Run(messageFormatterAction)
                    .Run(nowUtcProviderAction)
                    .Run(relativeFilePathsVisualStudioProjectFileSerializerAction)
                    .Run(stringlyTypedPathOperatorAction)
                    .Run(visualStudioProjectFileDeserializationSettingsAction)
                    .Run(visualStudioProjectFileSerializerMessagesOutputFilePathProviderAction)
                    .Run(visualStudioProjectFileValidatorAction)
                    ;
            });

            // 7
            IServiceAction<IVisualStudioProjectFileStreamSerializer> visualStudioProjectFileStreamSerializerAction = ServiceAction<IVisualStudioProjectFileStreamSerializer>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IVisualStudioProjectFileStreamSerializer, VisualStudioProjectFileStreamSerializer>()
                    .Run(functionalVisualStudioProjectFileSerializerAction)
                    .Run(messageSinkProviderAction)
                    ;
            });

            // 8
            IServiceAction<IVisualStudioProjectFileSerializer> visualStudioProjectFileSerializerAction = ServiceAction<IVisualStudioProjectFileSerializer>.New((serviceCollection) =>
            {
                serviceCollection
                    .AddSingleton<IVisualStudioProjectFileSerializer, VisualStudioProjectFileSerializer>()
                    .Run(visualStudioProjectFileStreamSerializerAction)
                    ;
            });

            services
                .Run(deepEqualsXElementEqualityComparerAction)
                .Run(fileNameOperatorAction)
                .Run(functionalityDirectoryNameProviderAction)
                .Run(functionalVisualStudioProjectFileSerializerAction)
                .Run(guidProviderAction)
                .Run(messagesOutputBaseDirectoryPathProviderAction)
                .Run(messageFormatterAction)
                .Run(messageSinkProviderAction)
                .Run(nowUtcProviderAction)
                .Run(orderIndependentXElementEqualityComparerAction)
                .Run(processStartTimeUtcDirectoryNameProviderAction)
                .Run(processStartTimeUtcProviderAction)
                .Run(programNameDirectoryNameProviderAction)
                .Run(programNameProviderAction)
                .Run(programNameStartTimeMessagesOutputDirectoryPathProviderAction)
                .Run(programNameStartTimeFunctionalityMessagesOutputDirectoryPathProviderAction)
                .Run(projectFileDeserializationMessagesOutputFileNameProviderAction)
                .Run(relativeFilePathsVisualStudioProjectFileSerializerAction)
                .Run(stringlyTypedPathOperatorAction)
                .Run(temporaryDirectoryFilePathProviderAction)
                .Run(testingDataDirectoryContentPathsProviderAction)
                .Run(timestampUtcDirectoryNameProviderAction)
                .Run(visualStudioProjectFileSerializerAction)
                .Run(visualStudioProjectFileStreamSerializerAction)
                .Run(visualStudioProjectFileSerializerMessagesOutputFilePathProviderAction)
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
