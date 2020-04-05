using System;
using System.Xml.Serialization;

namespace PlanetbaseFramework
{
    [Serializable]
    [XmlRoot(ElementName = "strings")]
    public class StringFile
    {
        [XmlElement(ElementName = "string")]
        public XmlString[] Strings { get; set; }

        [Serializable]
        public class XmlString
        {
            [XmlText]
            public string Value { get; set; }

            [XmlAttribute(AttributeName = "name")]
            public string Key { get; set; }
        }
    }
}