using System;
using System.Threading.Tasks;

using R5T.D0006;
using R5T.Lombardy;
using R5T.Thessaloniki;


namespace R5T.Kefalonia.Construction
{
    class MessagesOutputBaseDirectoryPathProvider : IMessagesOutputBaseDirectoryPathProvider
    {
        private ITemporaryDirectoryPathProvider TemporaryDirectoryPathProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public MessagesOutputBaseDirectoryPathProvider(
            ITemporaryDirectoryPathProvider temporaryDirectoryPathProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.TemporaryDirectoryPathProvider = temporaryDirectoryPathProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public Task<string> GetMessagesOutputBaseDirectoryPathAsync()
        {
            var temporaryDirectoryPath = this.TemporaryDirectoryPathProvider.GetTemporaryDirectoryPath();

            var messagesOutputDirectoryPath = this.StringlyTypedPathOperator.GetDirectoryPath(temporaryDirectoryPath, Constants.MessagesOutputDirectoryName);
            return Task.FromResult(messagesOutputDirectoryPath);
        }
    }
}
