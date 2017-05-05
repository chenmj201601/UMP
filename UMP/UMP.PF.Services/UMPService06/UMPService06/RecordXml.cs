using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace UMPService06
{


    public class Root
    {
        List<Record> recList = new List<Record>();

        [XmlElement(ElementName = "Record")]
        public List<Record> RecList
        {
            get { return recList; }
            set { recList = value; }
        }
    }


    public class Record
    {
        [XmlAttribute]
        public string RecordReference { get; set; }

        [XmlIgnore]
        public string RecordReference1 { get; set; }

        [XmlAttribute]
        public string PolicyID { get; set; }

        [XmlAttribute]
        public string KeyID { get; set; }


        [XmlAttribute]
        public string Key1d { get; set; }
        [XmlAttribute]
        public string Key1b { get; set; }

        [XmlIgnore]
        public DateTime StartRecordTime { get; set; }

        [XmlAttribute]
        public string Path { get; set; }

        [XmlAttribute]
        public string Path1 { get; set; }


        [XmlIgnore]
        public string VoiceIP { get; set; }

        [XmlIgnore]
        public string TrueVoiceIP { get; set; }
    }
}
