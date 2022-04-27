using System;using R5T.T0064;


namespace R5T.Kefalonia.Common
{[ServiceDefinitionMarker]
    public interface IVisualStudioProjectFileDeserializationSettings:IServiceDefinition
    {
        bool ThrowAtErrorOccurrence { get; }
        bool ThrowIfInvalidProjectFile { get; }
        bool ThrowIfAnyErrorAtEnd { get; }
    }
}
