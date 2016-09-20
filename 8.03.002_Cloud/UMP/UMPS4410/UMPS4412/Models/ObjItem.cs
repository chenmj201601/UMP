//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b1ccbe7a-08a4-4624-bb98-430e8aade9f4
//        CLR Version:              4.0.30319.18408
//        Name:                     ObjItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412.Models
//        File Name:                ObjItem
//
//        created by Charley at 2016/5/10 14:29:29
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;


namespace UMPS4412.Models
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

        private int mType;

        public int Type
        {
            get { return mType; }
            set { mType = value; OnPropertyChanged("Type"); }
        }

        private int mState;

        public int State
        {
            get { return mState; }
            set { mState = value; OnPropertyChanged("State"); }
        }

        private bool mIsDefault;

        public bool IsDefault
        {
            get { return mIsDefault; }
            set { mIsDefault = value; OnPropertyChanged("IsDefault"); }
        }

        private string mStrType;

        public string StrType
        {
            get { return mStrType; }
            set { mStrType = value; OnPropertyChanged("StrType"); }
        }

        private string mStrState;

        public string StrState
        {
            get { return mStrState; }
            set { mStrState = value; OnPropertyChanged("StrState"); }
        }

        private string mStrIsDefault;

        public string StrIsDefault
        {
            get { return mStrIsDefault; }
            set { mStrIsDefault = value; OnPropertyChanged("StrIsDefault"); }
        }

        public object Data { get; set; }

        public UMPApp CurrentApp;
    }
}
