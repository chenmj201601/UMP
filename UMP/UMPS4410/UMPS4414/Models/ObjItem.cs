//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b126dbe3-096b-466f-b057-c71c780c3a70
//        CLR Version:              4.0.30319.18408
//        Name:                     ObjItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4414.Models
//        File Name:                ObjItem
//
//        created by Charley at 2016/6/22 11:00:10
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;


namespace UMPS4414.Models
{
    public class ObjItem : CheckableItemBase
    {
        public int ObjType { get; set; }
        public long ObjID { get; set; }

        private int mNumber;

        public int Number
        {
            get { return mNumber; }
            set { mNumber = value; OnPropertyChanged("Number"); }
        }
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

        private int mStateType;

        public int StateType
        {
            get { return mStateType; }
            set { mStateType = value; OnPropertyChanged("StateType"); }
        }

        private string mStrStateType;

        public string StrStateType
        {
            get { return mStrStateType; }
            set { mStrStateType = value; OnPropertyChanged("StrStateType"); }
        }

        private bool mIsWorkTime;

        public bool IsWorkTime
        {
            get { return mIsWorkTime; }
            set { mIsWorkTime = value; OnPropertyChanged("IsWorkTime"); }
        }

        private string mStrIsWorkTime;

        public string StrIsWorkTime
        {
            get { return mStrIsWorkTime; }
            set { mStrIsWorkTime = value; OnPropertyChanged("StrIsWorkTime"); }
        }

        private string mStrParent;

        public string StrParent
        {
            get { return mStrParent; }
            set { mStrParent = value; OnPropertyChanged("StrParent"); }
        }

        private string mStrColor;

        public string StrColor
        {
            get { return mStrColor; }
            set { mStrColor = value; OnPropertyChanged("StrColor"); }
        }

        public object Data { get; set; }

        public UMPApp CurrentApp;
    }
}
