using System;
using System.Collections.Generic;

using R5T.Gloucester.Types;
using R5T.Ostersund;


namespace R5T.Kefalonia.Common
{
    public class VisualStudioProjectFileValidator : IVisualStudioProjectFileValidator
    {
        // Instance-level to allow actions to bind to instance service instances.
        private IEnumerable<Func<ProjectFile, IMessageAggregator, bool>> ValidationActions { get; set; }


        public VisualStudioProjectFileValidator()
        {
            this.ConstructorEndSetup();
        }

        private void ConstructorEndSetup()
        {
            // Add at end to allow binding to instance service instances.
            this.ValidationActions = this.GetValidationActions();
        }

        private IEnumerable<Func<ProjectFile, IMessageAggregator, bool>> GetValidationActions()
        {
            var validationActions = new List<Func<ProjectFile, IMessageAggregator, bool>>()
            {
                this.SdkIsSet,
                this.TargetFrameworkIsSet,
            };

            return validationActions;
        }

        public ValidationResult Validate(ProjectFile projectFile)
        {
            var isValid = true;

            var messageAggregator = new MessageAggregator();
            foreach (var validationAction in this.ValidationActions)
            {
                var actionResult = validationAction(projectFile, messageAggregator);

                isValid = isValid && actionResult;
            }

            var validationResult = new ValidationResult(isValid, messageAggregator.GetMessages());
            return validationResult;
        }

        private bool SdkIsSet(ProjectFile projectFile, IMessageAggregator messageAggregator)
        {
            var result = projectFile.SDK.IsSet;
            if(!result)
            {
                var message = "SDK was not set.";
                messageAggregator.AddMessage(message);
            }

            return result;
        }

        private bool TargetFrameworkIsSet(ProjectFile projectFile, IMessageAggregator messageAggregator)
        {
            var result = projectFile.TargetFramework.IsSet;
            if (!result)
            {
                var message = "TargetFramework was not set.";
                messageAggregator.AddMessage(message);
            }

            return result;
        }
    }
}
