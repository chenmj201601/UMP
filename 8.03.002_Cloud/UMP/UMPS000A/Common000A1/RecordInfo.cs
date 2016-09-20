//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f59a1cd8-0b7b-4e3d-8864-3f4ee2362333
//        CLR Version:              4.0.30319.18063
//        Name:                     RecordInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common000A1
//        File Name:                RecordInfo
//
//        created by Charley at 2015/9/2 11:56:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Common000A1
{
    public class RecordInfo
    {
        public long RowID { get; set; }
        public long SerialID { get; set; }
        public string RecordReference { get; set; }
        public DateTime StartRecordTime { get; set; }
        public DateTime StopRecordTime { get; set; }
        public string Extension { get; set; }
        public string Agent { get; set; }
        public int VoiceID { get; set; }
        public string VoiceIP { get; set; }
        public int ChannelID { get; set; }
        public int MediaType { get; set; }
        public string EncryptFlag { get; set; }
    }
}
