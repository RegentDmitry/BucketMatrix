using System;
using System.Collections.Generic;
using System.Xml.Serialization;

public class XmlSerializableDictionary<TKey, TValue>
    : Dictionary<TKey, TValue>, IXmlSerializable, ICloneable
{
    public virtual object Clone()
    {
        var clone = new XmlSerializableDictionary<TKey, TValue>();
        foreach (var pair in this)
            clone.Add(pair.Key,pair.Value);
        return clone;
    }
    public System.Xml.Schema.XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(System.Xml.XmlReader reader)
    {
        var keySerializer = new XmlSerializer(typeof(TKey));
        var valueSerializer = new XmlSerializer(typeof(TValue));
        var wasEmpty = reader.IsEmptyElement;
        reader.Read();
        if (wasEmpty)
            return;
        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
        {
            reader.ReadStartElement("item");
            reader.ReadStartElement("key");
            var key = (TKey)keySerializer.Deserialize(reader);
            reader.ReadEndElement();
            reader.ReadStartElement("value");
            var value = (TValue)valueSerializer.Deserialize(reader);
            reader.ReadEndElement();
            Add(key, value);
            reader.ReadEndElement();
            reader.MoveToContent();
        }
        reader.ReadEndElement();
    }

    public void WriteXml(System.Xml.XmlWriter writer)
    {
        var keySerializer = new XmlSerializer(typeof(TKey));
        var valueSerializer = new XmlSerializer(typeof(TValue));
        foreach (var key in Keys)
        {
            writer.WriteStartElement("item");
            writer.WriteStartElement("key");
            keySerializer.Serialize(writer, key);
            writer.WriteEndElement();
            writer.WriteStartElement("value");
            var value = this[key];
            valueSerializer.Serialize(writer, value);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}
