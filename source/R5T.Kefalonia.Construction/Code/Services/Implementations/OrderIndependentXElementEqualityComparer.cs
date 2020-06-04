using System;
using System.Threading.Tasks;
using System.Xml.Linq;

using R5T.XmlStuff;


namespace R5T.Kefalonia.Construction
{
    public class OrderIndependentXElementEqualityComparer : IXElementEqualityComparer
    {
        public Task<bool> AreEqual(XElement xElement1, XElement xElement2)
        {
            var normalizedXElement1 = xElement1.Normalize();
            var normalizedXElement2 = xElement2.Normalize();

            var result = XElement.DeepEquals(normalizedXElement1, normalizedXElement2);

            return Task.FromResult(result);
        }
    }
}
