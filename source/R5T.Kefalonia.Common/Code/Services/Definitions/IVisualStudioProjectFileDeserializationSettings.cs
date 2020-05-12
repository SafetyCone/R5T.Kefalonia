using System;


namespace R5T.Kefalonia.Common
{
    public interface IVisualStudioProjectFileDeserializationSettings
    {
        bool ThrowAtErrorOccurrence { get; }
        bool ThrowIfAnyErrorAtEnd { get; }
    }
}
