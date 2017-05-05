//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2b730bd7-7584-464a-8e47-6c3d3f2b608f
//        CLR Version:              4.0.30319.18063
//        Name:                     UserMonitorObjectConfig
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102.Models
//        File Name:                UserMonitorObjectConfig
//
//        created by Charley at 2015/6/30 18:50:57
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Xml.Serialization;

namespace UMPS2102.Models
{
    [XmlRoot]
    public class UserMonitorObjectConfig
    {
        [XmlAttribute]
        public bool IsRemember { get; set; }
        [XmlAttribute]
        public long UserID { get; set; }
        [XmlAttribute]
        public int ViewType { get; set; }

        private List<MonitorData> mListStateMonObjList;
        [XmlArray]
        public List<MonitorData> ListStateMonObjList
        {
            get { return mListStateMonObjList; }
        }

        private List<MonitorData> mListNetMonObjList;
        [XmlArray]
        public List<MonitorData> ListNetMonObjList
        {
            get { return mListNetMonObjList; }
        }

        public UserMonitorObjectConfig()
        {
            mListStateMonObjList = new List<MonitorData>();
            mListNetMonObjList = new List<MonitorData>();
        }
    }
}
