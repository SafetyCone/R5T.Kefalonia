using System;
using System.Threading.Tasks;
using System.Xml.Linq;using R5T.T0064;


namespace R5T.Kefalonia.Construction
{[ServiceImplementationMarker]
    public class DeepEqualsXElementEqualityComparer : IXElementEqualityComparer,IServiceImplementation
    {
        public Task<bool> AreEqual(XElement xElement1, XElement xElement2)
        {
            var result = XNode.DeepEquals(xElement1, xElement2);

            return Task.FromResult(result);
        }
    }
}
