using System;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace R5T.Kefalonia.Construction
{
    public class DeepEqualsXElementEqualityComparer : IXElementEqualityComparer
    {
        public Task<bool> AreEqual(XElement xElement1, XElement xElement2)
        {
            var result = XNode.DeepEquals(xElement1, xElement2);

            return Task.FromResult(result);
        }
    }
}
