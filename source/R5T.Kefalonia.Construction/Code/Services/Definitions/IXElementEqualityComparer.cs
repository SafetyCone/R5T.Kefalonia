using System;
using System.Threading.Tasks;
using System.Xml.Linq;using R5T.T0064;


namespace R5T.Kefalonia.Construction
{[ServiceDefinitionMarker]
    public interface IXElementEqualityComparer:IServiceDefinition
    {
        Task<bool> AreEqual(XElement xElement1, XElement xElement2);
    }
}
