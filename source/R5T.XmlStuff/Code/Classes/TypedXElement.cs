using System;
using System.Xml.Linq;

using R5T.Amphipolis;


namespace R5T.XmlStuff
{
    public abstract class TypedXElement : Typed<XElement>
    {
        public TypedXElement(XElement value)
            : base(value)
        {
        }
    }
}
