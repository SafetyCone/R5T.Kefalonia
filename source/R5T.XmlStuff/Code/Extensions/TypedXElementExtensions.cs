using System;
using System.Xml.Linq;


namespace R5T.XmlStuff
{
    public static class TypedXElementExtensions
    {
        public static string GetChildValue(this TypedXElement typedXElement, string childName)
        {
            var output = typedXElement.Value.GetChildValue(childName);
            return output;
        }

        public static bool HasChild(this TypedXElement typedXElement, string childName, out XElement child)
        {
            var hasChild = typedXElement.Value.HasChild(childName, out child);
            return hasChild;
        }
    }
}
