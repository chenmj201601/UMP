using System.Runtime.Serialization;
using VoiceCyber.UMP.Common;

namespace Common3604
{
    [DataContract]
    public class UpRequest
    {
        public UpRequest() { }

        [DataMember]
        public string SvPath { get; set; }
        [DataMember]
        public byte[] ListByte { get; set; }
        [DataMember]
        public SessionInfo Session { get; set; }

        //public string LogInfo() { }
        //public override string ToString()
        //{

        //}
    }
}
