using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{
    public class KeywordResultInfo
    {
        public long RecordNumber { get; set; }
        public long RecordSerialID { get; set; }
        public string RecordReference { get; set; }
        public int Offset { get; set; }
        public string KeywordName { get; set; }
        public long KeywordNo { get; set; }
        public string KeywordContent { get; set; }
        public long ContentNo { get; set; }
        public string Agent { get; set; }
    }
}
