//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fe4c52e7-d204-4bae-b0b1-8d2ee14703bc
//        CLR Version:              4.0.30319.18444
//        Name:                     RecordMemoItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                RecordMemoItem
//
//        created by Charley at 2014/11/14 16:15:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using VoiceCyber.UMP.Common31021;

namespace UMPS3102.Models
{
    public class RecordMemoItem
    {
        public long ID { get; set; }
        public long RecordSerialID { get; set; }
        public long UserID { get; set; }
        public DateTime MemoTime { get; set; }
        public string Content { get; set; }
        public string State { get; set; }
        public string Source { get; set; }

        public RecordMemoInfo RecordMemoInfo { get; set; }

        public RecordMemoItem(RecordMemoInfo recordMemoInfo)
        {
            ID = recordMemoInfo.ID;
            RecordSerialID = recordMemoInfo.RecordSerialID;
            UserID = recordMemoInfo.UserID;
            MemoTime = recordMemoInfo.MemoTime;
            Content = recordMemoInfo.Content;
            State = recordMemoInfo.State;
            Source = recordMemoInfo.Source;

            RecordMemoInfo = recordMemoInfo;
        }
    }
}
