using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MirthDotNet.Model
{
    [Serializable]
    public class DataTypeSerializationProperties
    {
        [XmlAttribute("class")]
        public string Class { get;set; }

        [XmlElement("handleRepetitions")]
        public bool HandleRepetitions { get; set; }
        [XmlElement("handleSubcomponents")]
        public bool HandleSubcomponents { get; set; }
        [XmlElement("useStrictParser")]
        public bool UseStrictParser { get; set; }
        [XmlElement("useStrictValidation")]
        public bool UseStrictValidation { get; set; }
        [XmlElement("stripNamespaces")]
        public bool StripNamespaces { get; set; }
        [XmlElement("segmentDelimiter")]
        public string SegmentDelimiter { get; set; }
        [XmlElement("convertLineBreaks")]
        public bool ConvertLineBreaks { get; set; }
    }
}
