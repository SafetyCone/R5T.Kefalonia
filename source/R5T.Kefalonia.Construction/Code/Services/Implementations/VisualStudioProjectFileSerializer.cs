﻿using System;
using System.IO;
using System.Threading.Tasks;

using R5T.D0001;
using R5T.D0010;
using R5T.Gloucester.Types;
using R5T.Lombardy;

using R5T.Kefalonia.Common;


namespace R5T.Kefalonia.Construction
{
    class VisualStudioProjectFileSerializer : IVisualStudioProjectFileSerializer
    {
        private INowUtcProvider NowUtcProvider { get; }
        private IFunctionalVisualStudioProjectFileSerializer FunctionalVisualStudioProjectFileSerializer { get; }
        private MessagesOutputFilePathProvider MessagesOutputFilePathProvider { get; }
        private IStringlyTypedPathOperator StringlyTypedPathOperator { get; }


        public VisualStudioProjectFileSerializer(
            INowUtcProvider nowUtcProvider,
            IFunctionalVisualStudioProjectFileSerializer functionalVisualStudioProjectFileSerializer,
            MessagesOutputFilePathProvider messagesOutputFilePathProvider,
            IStringlyTypedPathOperator stringlyTypedPathOperator)
        {
            this.NowUtcProvider = nowUtcProvider;
            this.FunctionalVisualStudioProjectFileSerializer = functionalVisualStudioProjectFileSerializer;
            this.MessagesOutputFilePathProvider = messagesOutputFilePathProvider;
            this.StringlyTypedPathOperator = stringlyTypedPathOperator;
        }

        public async Task<ProjectFile> DeserializeAsync(string projectFilePath)
        {
            var messagesOutputFilePath = await this.MessagesOutputFilePathProvider.GetMessagesOutputFilePathAsync(
                Constants.ProjectFileDeserializationFunctionalityName, projectFilePath);

            var messageRepository = new MessageRepository(this.StringlyTypedPathOperator, messagesOutputFilePath);

            await messageRepository.AddOutputMessageAsync(this.NowUtcProvider, $"Deserialization of:\n{projectFilePath}");

            var projectFile = await this.FunctionalVisualStudioProjectFileSerializer.DeserializeAsync(projectFilePath, messageRepository);
            return projectFile;
        }

        public async Task SerializeAsync(string projectFilePath, ProjectFile value, bool overwrite = true)
        {
            var messagesOutputFilePath = await this.MessagesOutputFilePathProvider.GetMessagesOutputFilePathAsync(
                Constants.ProjectFileSerializationFunctionalityName, projectFilePath);

            var messageRepository = new MessageRepository(this.StringlyTypedPathOperator, messagesOutputFilePath);

            await messageRepository.AddOutputMessageAsync(this.NowUtcProvider, $"Serialization of:\n{projectFilePath}");

            await this.FunctionalVisualStudioProjectFileSerializer.SerializeAsync(projectFilePath, value, messageRepository, overwrite);
        }
    }
}
