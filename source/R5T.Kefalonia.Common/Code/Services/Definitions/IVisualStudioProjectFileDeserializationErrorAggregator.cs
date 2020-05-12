using System;
using System.Collections.Generic;


namespace R5T.Kefalonia.Common
{
    public interface IVisualStudioProjectFileDeserializationErrorAggregator
    {
        void AddError(string message);
        IEnumerable<string> GetErrors();
    }
}
