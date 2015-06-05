using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MirthDotNet.Model
{
    [Serializable]
    public class DataTypeDeserializationProperties
    {
        [XmlAttribute("class")]
        public string Class { get; set; }

        [XmlElement("useStrictParser")]
        public bool UseStrictParser { get; set; }
        [XmlElement("useStrictValidation")]
        public bool UseStrictValidation { get; set; }
        [XmlElement("segmentDelimiter")]
        public string SegmentDelimiter { get; set; }
    }
}
