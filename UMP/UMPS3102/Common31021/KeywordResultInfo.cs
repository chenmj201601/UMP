//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    285d70f3-b45b-4789-86b6-9e504903ada4
//        CLR Version:              4.0.30319.42000
//        Name:                     KeywordResultInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                KeywordResultInfo
//
//        Created by Charley at 2016/11/8 10:13:19
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
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
