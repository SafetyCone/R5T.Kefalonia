using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;


namespace R5T.XmlStuff
{
    public static class XElementExtensions
    {
        public static string GetChildValueSingle(this XElement xElement, string childName)
        {
            var output = xElement.GetChildren(childName)
                .Single()
                .Value;

            return output;
        }

        public static string GetChildValueFirst(this XElement xElement, string childName)
        {
            var output = xElement.GetChildren(childName)
                .First()
                .Value;

            return output;
        }

        /// <summary>
        /// Uses <see cref="XElementExtensions.GetChildValueSingle(XElement, string)"/> as the default.
        /// </summary>
        public static string GetChildValue(this XElement xElement, string childName)
        {
            var output = xElement.GetChildValueSingle(childName);
            return output;
        }

        public static IEnumerable<XElement> GetChildren(this XElement xElement, string childName)
        {
            var output = xElement.Elements()
                .Where(x => x.Name == childName);

            return output;
        }

        public static bool HasChildSingle(this XElement xElement, string childName, out XElement child)
        {
            child = xElement.GetChildren(childName)
                .SingleOrDefault();

            var hasChild = XElementHelper.WasFound(child);
            return hasChild;
        }

        /// <summary>
        /// Uses <see cref="XElementExtensions.HasChildSingle(XElement, string, out XElement)"/> as the default.
        /// </summary>
        public static bool HasChild(this XElement xElement, string childName, out XElement child)
        {
            var hasChild = xElement.HasChildSingle(childName, out child);
            return hasChild;
        }

        public static bool HasChild(this XElement xElement, string childName)
        {
            var hasChild = xElement.HasChild(childName, out var _);
            return hasChild;
        }

        public static XElement Normalize(this XElement xElement)
        {
            if (xElement.HasElements)
            {
                return new XElement(
                    xElement.Name,
                    xElement.Attributes()
                        .Where(a => a.Name.Namespace == XNamespace.Xmlns)
                        .OrderBy(a => a.Name.ToString()),
                    xElement.Elements()
                        .OrderBy(a => a.Name.ToString())
                        .Select(e => e.Normalize()));
            }

            if (xElement.IsEmpty)
            {
                return new XElement(
                    xElement.Name,
                    xElement.Attributes()
                        .OrderBy(a => a.Name.ToString()));
            }

            return new XElement(
                xElement.Name,
                xElement.Attributes()
                    .OrderBy(a => a.Name.ToString()),
                xElement.Value);
        }
    }
}
