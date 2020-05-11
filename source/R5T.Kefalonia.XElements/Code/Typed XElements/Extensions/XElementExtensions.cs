using System;
using System.Xml.Linq;


namespace R5T.Kefalonia.XElements
{
    public static class XElementExtensions
    {
        public static ProjectXElement AsProject(this XElement xElement)
        {
            var projectXElement = new ProjectXElement(xElement);
            return projectXElement;
        }

        public static PropertyGroupXElement AsPropertyGroup(this XElement xElement)
        {
            var propertyGroupXElement = new PropertyGroupXElement(xElement);
            return propertyGroupXElement;
        }
    }
}
