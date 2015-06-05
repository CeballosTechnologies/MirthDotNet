using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MirthDotNet.Model
{
    [Serializable]
    public class DataTypeProperties
    {
        [XmlElement("serializationProperties")]
        public DataTypeSerializationProperties SerializationProperties { get; set; }

        [XmlElement("deserializationProperties")]
        public DataTypeDeserializationProperties DeserializationProperties { get; set; }

        [XmlElement("responseGenerationProperties")]
        public DataTypeResponseGenerationProperties ResponseGenerationProperties { get; set; }

        [XmlElement("responseValidationProperties")]
        public DataTypeResponseValidationProperties responseValidationProperties { get; set; }
    }
}
