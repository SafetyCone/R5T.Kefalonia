using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using R5T.D0001;
using R5T.D0010;
using R5T.Gloucester.Types;
using R5T.Ostersund;


namespace R5T.Kefalonia.Common
{
    public class VisualStudioProjectFileValidator : IVisualStudioProjectFileValidator
    {
        private INowUtcProvider NowUtcProvider { get; }

        // Instance-level to allow actions to bind to instance service instances.
        private IEnumerable<Func<ProjectFile, IMessageSink, Task<bool>>> ValidationActions { get; set; }


        public VisualStudioProjectFileValidator(
            INowUtcProvider nowUtcProvider)
        {
            this.NowUtcProvider = nowUtcProvider;

            this.ConstructorEndSetup();
        }

        private void ConstructorEndSetup()
        {
            // Add at end to allow binding to instance service instances.
            this.ValidationActions = this.GetValidationActions();
        }

        private IEnumerable<Func<ProjectFile, IMessageSink, Task<bool>>> GetValidationActions()
        {
            var validationActions = new List<Func<ProjectFile, IMessageSink, Task<bool>>>()
            {
                this.SdkIsSet,
                this.TargetFrameworkIsSet,
            };

            return validationActions;
        }

        public async Task<bool> Validate(ProjectFile projectFile, IMessageSink messageSink)
        {
            // Sequential task evaluation.
            var isValid = true;
            foreach (var validationAction in this.ValidationActions)
            {
                var actionResult = await validationAction(projectFile, messageSink);

                isValid = isValid && actionResult;
            }

            return isValid;
        }

        private async Task<bool> SdkIsSet(ProjectFile projectFile, IMessageSink messageSink)
        {
            var result = projectFile.SDK.IsSet;
            if(!result)
            {
                var message = "SDK was not set.";
                await messageSink.AddErrorMessageAsync(this.NowUtcProvider, message);
            }

            return result;
        }

        private async Task<bool> TargetFrameworkIsSet(ProjectFile projectFile, IMessageSink messageSink)
        {
            var result = projectFile.TargetFramework.IsSet;
            if (!result)
            {
                var message = "TargetFramework was not set.";
                await messageSink.AddErrorMessageAsync(this.NowUtcProvider, message);
            }

            return result;
        }
    }
}
