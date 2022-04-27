using System;
using System.Threading.Tasks;

using R5T.T0064;


namespace R5T.Kefalonia.Construction
{
    /// <summary>
    /// Async, stringly-typed paths.
    /// </summary>
    [ServiceImplementationMarker]
    public class FunctionalityDirectoryNameProvider : INoServiceDefinition, IServiceImplementation
    {
        public Task<string> GetFunctionalityDirectoryNameAsync(
            [NotServiceComponent] string functionalityName)
        {
            return Task.FromResult(functionalityName); // Assume that the functionality name can be a directory name.
        }
    }
}
