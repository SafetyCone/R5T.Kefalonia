using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using R5T.D0006;
using R5T.D0010;
using R5T.T0001;


namespace R5T.Kefalonia.Construction
{
    public class DefaultProgramMessageRepository : IMessageRepository
    {
        private IProgramStartTimeSpecificMessagesOutputDirectoryPathProvider ProgramStartTimeSpecificMessagesOutputDirectoryPathProvider { get; }


        public Task AddAsync(Message message)
        {
            throw new NotImplementedException();
        }

        public Task ClearAsync(Func<Message, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Message>> GetAsync(Func<Message, bool> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
