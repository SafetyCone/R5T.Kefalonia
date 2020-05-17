using System;
using System.Threading.Tasks;


namespace R5T.Kefalonia.Construction
{
    /// <summary>
    /// Async, stringly-typed paths.
    /// </summary>
    public class FunctionalityDirectoryNameProvider
    {
        public Task<string> GetFunctionalityDirectoryNameAsync(string functionalityName)
        {
            return Task.FromResult(functionalityName); // Assume that the functionality name can be a directory name.
        }
    }
}
