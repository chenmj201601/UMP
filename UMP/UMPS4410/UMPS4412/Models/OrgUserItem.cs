//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6bc85785-8e3b-49e7-8e46-9d9fff27456b
//        CLR Version:              4.0.30319.18408
//        Name:                     OrgUserItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412.Models
//        File Name:                OrgUserItem
//
//        created by Charley at 2016/5/12 10:05:51
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;


namespace UMPS4412.Models
{
    public class OrgUserItem : CheckableItemBase
    {
        public int ObjType { get; set; }

        public long ObjID { get; set; }

        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mDescription;

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        public object Data { get; set; }

        public UMPApp CurrentApp;
    }
}
