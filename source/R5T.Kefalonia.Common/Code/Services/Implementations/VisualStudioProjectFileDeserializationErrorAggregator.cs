using System;
using System.Collections.Generic;


namespace R5T.Kefalonia.Common
{
    public class VisualStudioProjectFileDeserializationErrorAggregator : IVisualStudioProjectFileDeserializationErrorAggregator
    {
        private List<string> Errors { get; } = new List<string>();


        public void AddError(string message)
        {
            this.Errors.Add(message);
        }

        public IEnumerable<string> GetErrors()
        {
            return this.Errors;
        }
    }
}
