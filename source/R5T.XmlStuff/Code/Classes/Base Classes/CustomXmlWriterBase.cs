using System;
using System.Xml;


namespace R5T.XmlStuff
{
    /// <summary>
    /// Note: class is almost useless because XmlWriter is almost useless!
    /// The problem is that there is no expansion point for after the closing '>' (alligator) of an element (opening element) is written. Thus there is no way to customize line spacing.
    /// This class is preserved since work was required to create it.
    /// </summary>
    public class CustomXmlWriterBase : XmlWriter
    {
        protected XmlWriter XmlWriter { get; }


        public CustomXmlWriterBase(XmlWriter xmlWriter)
        {
            this.XmlWriter = xmlWriter;
        }

        public override WriteState WriteState => this.XmlWriter.WriteState;

        public override void Flush()
        {
            this.XmlWriter.Flush();
        }

        public override string LookupPrefix(string ns)
        {
            return this.XmlWriter.LookupPrefix(ns);
        }

        public override void WriteBase64(byte[] buffer, int index, int count)
        {
            this.XmlWriter.WriteBase64(buffer, index, count);
        }

        public override void WriteCData(string text)
        {
            this.XmlWriter.WriteCData(text);
        }

        public override void WriteCharEntity(char ch)
        {
            this.XmlWriter.WriteCharEntity(ch);
        }

        public override void WriteChars(char[] buffer, int index, int count)
        {
            this.XmlWriter.WriteChars(buffer, index, count);
        }

        public override void WriteComment(string text)
        {
            this.XmlWriter.WriteComment(text);
        }

        public override void WriteDocType(string name, string pubid, string sysid, string subset)
        {
            this.XmlWriter.WriteDocType(name, pubid, sysid, subset);
        }

        public override void WriteEndAttribute()
        {
            this.XmlWriter.WriteEndAttribute();
        }

        public override void WriteEndDocument()
        {
            this.XmlWriter.WriteEndDocument();
        }

        public override void WriteEndElement()
        {
            this.XmlWriter.WriteEndElement();
        }

        public override void WriteEntityRef(string name)
        {
            this.XmlWriter.WriteEntityRef(name);
        }

        public override void WriteFullEndElement()
        {
            this.XmlWriter.WriteFullEndElement();
        }

        public override void WriteProcessingInstruction(string name, string text)
        {
            this.XmlWriter.WriteProcessingInstruction(name, text);
        }

        public override void WriteRaw(char[] buffer, int index, int count)
        {
            this.XmlWriter.WriteRaw(buffer, index, count);
        }

        public override void WriteRaw(string data)
        {
            this.XmlWriter.WriteRaw(data);
        }

        public override void WriteStartAttribute(string prefix, string localName, string ns)
        {
            this.XmlWriter.WriteStartAttribute(prefix, localName, ns);
        }

        public override void WriteStartDocument()
        {
            this.XmlWriter.WriteStartDocument();
        }

        public override void WriteStartDocument(bool standalone)
        {
            this.XmlWriter.WriteStartDocument(standalone);
        }

        public override void WriteStartElement(string prefix, string localName, string ns)
        {
            this.XmlWriter.WriteStartElement(prefix, localName, ns);
        }

        public override void WriteString(string text)
        {
            this.XmlWriter.WriteString(text);
        }

        public override void WriteSurrogateCharEntity(char lowChar, char highChar)
        {
            this.XmlWriter.WriteSurrogateCharEntity(lowChar, highChar);
        }

        public override void WriteWhitespace(string ws)
        {
            this.XmlWriter.WriteWhitespace(ws);
        }
    }
}
