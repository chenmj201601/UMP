//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7250b1ee-f42d-42cd-86fe-287cd9834cfb
//        CLR Version:              4.0.30319.18408
//        Name:                     StateAlarmInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                StateAlarmInfo
//
//        created by Charley at 2016/7/16 17:10:56
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using VoiceCyber.UMP.Common44101;


namespace UMPS4411.Models
{
    public class StateAlarmInfo
    {
        public long AlarmID { get; set; }
        public DateTime AlarmTime { get; set; }

        public AlarmMessageInfo AlarmMessage;
    }
}
