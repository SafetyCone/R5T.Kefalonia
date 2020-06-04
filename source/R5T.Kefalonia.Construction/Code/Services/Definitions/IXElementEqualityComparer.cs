using System;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace R5T.Kefalonia.Construction
{
    public interface IXElementEqualityComparer
    {
        Task<bool> AreEqual(XElement xElement1, XElement xElement2);
    }
}
