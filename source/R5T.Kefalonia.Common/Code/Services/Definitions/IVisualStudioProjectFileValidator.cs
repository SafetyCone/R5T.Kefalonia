using System;

using R5T.Gloucester.Types;


namespace R5T.Kefalonia.Common
{
    public interface IVisualStudioProjectFileValidator
    {
        ValidationResult Validate(ProjectFile projectFile);
    }
}
