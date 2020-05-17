using System;


namespace R5T.Kefalonia.Common
{
    public class VisualStudioProjectFileDeserializationSettings : IVisualStudioProjectFileDeserializationSettings
    {
        public bool ThrowAtErrorOccurrence { get; set; }
        public bool ThrowIfAnyErrorAtEnd { get; set; }
        public bool ThrowIfInvalidProjectFile { get; set; }
    }
}
