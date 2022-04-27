using System;using R5T.T0064;


namespace R5T.Kefalonia.Common
{[ServiceImplementationMarker]
    public class VisualStudioProjectFileDeserializationSettings : IVisualStudioProjectFileDeserializationSettings,IServiceImplementation
    {
        public bool ThrowAtErrorOccurrence { get; set; }
        public bool ThrowIfAnyErrorAtEnd { get; set; }
        public bool ThrowIfInvalidProjectFile { get; set; }
    }
}
