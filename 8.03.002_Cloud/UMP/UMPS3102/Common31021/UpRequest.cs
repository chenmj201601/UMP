using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common;
using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 上传请求
    /// </summary>
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

    }
}
