//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6aacab3d-a62e-42f5-99b5-27d24c28f425
//        CLR Version:              4.0.30319.18408
//        Name:                     ObjItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4413.Models
//        File Name:                ObjItem
//
//        created by Charley at 2016/6/7 16:18:44
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;


namespace UMPS4413.Models
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

        private int mLevel;

        public int Level
        {
            get { return mLevel; }
            set { mLevel = value; OnPropertyChanged("Level"); }
        }

        private int mState;

        public int State
        {
            get { return mState; }
            set { mState = value; OnPropertyChanged("State"); }
        }

        private string mStrState;

        public string StrState
        {
            get { return mStrState; }
            set { mStrState = value; OnPropertyChanged("StrState"); }
        }

        private string mStrLevel;

        public string StrLevel
        {
            get { return mStrLevel; }
            set { mStrLevel = value; OnPropertyChanged("StrLevel"); }
        }

        private string mExtension;

        public string Extension
        {
            get { return mExtension; }
            set { mExtension = value; OnPropertyChanged("Extension"); }
        }

        public object Data { get; set; }

        public UMPApp CurrentApp;
    }
}
