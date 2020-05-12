using System;
using System.Collections.Generic;
using System.Linq;

using R5T.Ostersund;
using R5T.Ostersund.Extensions;


namespace R5T.VisualStudioProjectFileStuff
{
    public static class ProjectFileValues
    {
        public static bool ParseBoolean(string booleanString)
        {
            var isPackable = bool.Parse(booleanString);
            return isPackable;
        }

        public static Version ParseVersion(string languageVersionString)
        {
            var languageVersion = Version.Parse(languageVersionString);
            return languageVersion;
        }

        public static List<int> ParseNoWarn(string noWarnString)
        {
            var tokens = noWarnString.Split(';');

            var warningNumbers = tokens.Select(x => Int32.Parse(x)).ToList();
            return warningNumbers;
        }

        public static OutputType ParseOutputType(string outputTypeString)
        {
            var outputType = outputTypeString.ToOutputType();
            return outputType;
        }

        public static TargetFramework ParseTargetFramework(string targetFrameworkString)
        {
            var targetFramework = targetFrameworkString.ToTargetFramework();
            return targetFramework;
        }
    }
}
