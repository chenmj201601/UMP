//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    35510caa-f025-4124-a9bf-0fc6c5ea67a7
//        CLR Version:              4.0.30319.18063
//        Name:                     ObjectItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102.Models
//        File Name:                ObjectItem
//
//        created by Charley at 2015/6/22 11:42:55
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.Wpf.CustomControls;

namespace UMPS2102.Models
{
    public class ObjectItem : CheckableItemBase
    {
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

        public int ObjType { get; set; }
        public long ObjID { get; set; }
        public object Data { get; set; }

        private bool mIsSelected;

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        private bool mIsMultiSelected;

        public bool IsMultiSelected
        {
            get { return mIsMultiSelected; }
            set { mIsMultiSelected = value; OnPropertyChanged("IsMultiSelected"); }
        }

        public override string ToString()
        {
            return string.Format("[{0}]{1}({2})", ObjType, Name, ObjID);
        }
    }
}
