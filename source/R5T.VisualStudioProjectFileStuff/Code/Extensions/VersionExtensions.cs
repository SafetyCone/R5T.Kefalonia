using System;


namespace R5T.VisualStudioProjectFileStuff
{
    public static class VersionExtensions
    {
        public static string ToStringProjectFileStandard(this Version version)
        {
            var representation = $"{version.Major}.{version.Minor}";
            return representation;
        }
    }
}
