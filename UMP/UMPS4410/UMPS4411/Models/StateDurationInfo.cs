//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bd2a5f0b-1cbd-4b84-934c-bb5870119c3c
//        CLR Version:              4.0.30319.18408
//        Name:                     StateDurationInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                StateDurationInfo
//
//        created by Charley at 2016/7/16 15:37:03
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;


namespace UMPS4411.Models
{
    public class StateDurationInfo
    {
        public int StateType { get; set; }
        public int StateValue { get; set; }
        public DateTime BeginTime { get; set; }
        public double Duration { get; set; }
        public string Extension { get; set; }

        public RegionSeatItem SeatItem;
        public UCAlarmPanel AlarmPanel;
        public UCAlarmIconPanel AlarmIconPanel;

        private List<StateAlarmInfo> mListStateAlarms = new List<StateAlarmInfo>();

        public List<StateAlarmInfo> ListStateAlarms
        {
            get { return mListStateAlarms; }
        } 
    }
}
