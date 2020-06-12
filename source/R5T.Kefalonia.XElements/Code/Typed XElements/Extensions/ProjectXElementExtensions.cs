using System;
using System.Linq;
using System.Xml.Linq;

using R5T.T0006;


namespace R5T.Kefalonia.XElements
{
    public static class ProjectXElementExtensions
    {
        public static bool IsPropertyGroup(XElement xElement)
        {
            var output = xElement.Name == ProjectFileXmlElementName.PropertyGroup;
            return output;
        }

        public static PropertyGroupXElement GetPropertyGroup(this ProjectXElement projectXElement)
        {
            var output = projectXElement.Value.Elements()
                .Where(x => ProjectXElementExtensions.IsPropertyGroup(x))
                .Single()
                .AsPropertyGroup();

            return output;
        }
    }
}
