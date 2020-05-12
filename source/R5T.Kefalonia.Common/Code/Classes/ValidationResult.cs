using System;
using System.Collections.Generic;


namespace R5T.Kefalonia.Common
{
    public class ValidationResult
    {
        public bool IsValid { get; }
        public IEnumerable<string> ErrorMessages { get; }


        public ValidationResult(bool isValid, IEnumerable<string> errorMessages)
        {
            this.IsValid = IsValid;
            this.ErrorMessages = errorMessages;
        }
    }
}
