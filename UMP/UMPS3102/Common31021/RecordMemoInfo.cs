//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9e9d530e-3484-4404-9d8a-4667b3fad4b3
//        CLR Version:              4.0.30319.18444
//        Name:                     RecordMemoInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                RecordMemoInfo
//
//        created by Charley at 2014/11/14 15:50:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 记录备注信息
    /// </summary>
    public class RecordMemoInfo
    {
        public long ID { get; set; }
        public long RecordSerialID { get; set; }
        public long UserID { get; set; }
        public DateTime MemoTime { get; set; }
        public string Content { get; set; }
        public string State { get; set; }
        public long LastModifyUserID { get; set; }
        public DateTime LastModifyTime { get; set; }
        public long DeleteUserID { get; set; }
        public DateTime DeleteTime { get; set; }
        public string Source { get; set; }
    }
}
