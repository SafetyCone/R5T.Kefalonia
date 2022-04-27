using System;
using System.Threading.Tasks;

using R5T.D0005;
using R5T.D0006;
using R5T.D0010;
using R5T.D0010.Default;

using R5T.Lombardy;

using R5T.Kefalonia.Common;using R5T.T0064;


namespace R5T.Kefalonia.Construction
{[ServiceImplementationMarker]
    /// <summary>
    /// Manages a singleton <see cref="IMessageSink"/> instance for the application.
    /// * DI recommended.
    /// * Singleton recommended.
    /// </summary>
    public class DefaultMessageSinkProvider : IMessageSinkProvider,IServiceImplementation
    {
        private const IMessageSink NullMessageSink = null;


        private IFileNameOperator FileNameOperator { get; }
        private IMessageFormatter MessageFormatter { get; }
        private IProgramNameProvider ProgramNameProvider { get; }
        private IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider ProgramStartTimeSpecificMessagesOutputDirectoryPathProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }

        private object SynchronizationRoot { get; } = new object();
        private IMessageSink MessageSink { get; set; } = DefaultMessageSinkProvider.NullMessageSink;


        public DefaultMessageSinkProvider(
            IFileNameOperator fileNameOperator,
            IMessageFormatter messageFormatter,
            IProgramNameProvider programNameProvider,
            IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider programStartTimeSpecificMessagesOutputDirectoryPathProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.FileNameOperator = fileNameOperator;
            this.MessageFormatter = messageFormatter;
            this.ProgramNameProvider = programNameProvider;
            this.ProgramStartTimeSpecificMessagesOutputDirectoryPathProvider = programStartTimeSpecificMessagesOutputDirectoryPathProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<IMessageSink> GetMessageSinkAsync()
        {
            if(this.MessageSink == DefaultMessageSinkProvider.NullMessageSink)
            {
                var gettingProgramName = this.ProgramNameProvider.GetProgramNameAsync();
                var gettingMessagesOutputDirectoryPath = this.ProgramStartTimeSpecificMessagesOutputDirectoryPathProvider.GetProgramStartTimeSpecificMessagesOutputDirectoryPathAsync();

                await Task.WhenAll(gettingProgramName, gettingMessagesOutputDirectoryPath);

                var fileName = this.FileNameOperator.GetFileName(gettingProgramName.Result, Constants.OutputFileExtension);

                var messagesOutputFilePath = this.StringlyTypedPathOperator.Combine(gettingMessagesOutputDirectoryPath.Result, fileName);

                this.MessageSink = CompositeMessageSink.NewDefault(this.MessageFormatter, this.StringlyTypedPathOperator, messagesOutputFilePath);
            }
            
            return this.MessageSink;
        }
    }
}
