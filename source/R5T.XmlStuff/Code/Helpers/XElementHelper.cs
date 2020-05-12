using System;
using System.Xml.Linq;


namespace R5T.XmlStuff
{
    public static class XElementHelper
    {
        public const XElement NotFound = default(XElement); // null


        public static bool WasFound(XElement xElement)
        {
            var wasFound = xElement != XElementHelper.NotFound;
            return wasFound;
        }
    }
}
