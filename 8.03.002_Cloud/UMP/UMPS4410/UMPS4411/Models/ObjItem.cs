//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e3324911-8f26-405c-b3d0-4e755c5b7418
//        CLR Version:              4.0.30319.18408
//        Name:                     ObjItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                ObjItem
//
//        created by Charley at 2016/6/17 10:06:11
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;


namespace UMPS4411.Models
{
    public class ObjItem : CheckableItemBase
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

        public int Type { get; set; }

        public object Data { get; set; }

        public UMPApp CurrentApp;

        public override string ToString()
        {
            return string.Format("[{0}][{1}", ObjID, Name);
        }
    }
}
