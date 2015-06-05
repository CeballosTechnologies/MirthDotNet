using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MirthDotNet.Model
{
    [Serializable]
    public class DataTypeResponseGenerationProperties
    {
        [XmlElement("segmentDelimiter")]
        public string SegmentDelimiter { get; set; }

        [XmlElement("successfulACKCode")]
        public string SuccessfulACKCode { get; set; }
        [XmlElement("successfulACKMessage")]
        public string SuccessfulACKMessage { get; set; }
        [XmlElement("errorACKCode")]
        public string ErrorACKCode { get; set; }
        [XmlElement("errorACKMessage")]
        public string ErrorACKMessage { get; set; }
        [XmlElement("rejectedACKCode")]
        public string RejectedACKCode { get; set; }
        [XmlElement("rejectedACKMessage")]
        public string RejectedACKMessage { get; set; }
        [XmlElement("msh15ACKAccept")]
        public bool MSH15ACKAccept { get; set; }
        [XmlElement("dateFormat")]
        public string DateFormat { get; set; }
    }
}
