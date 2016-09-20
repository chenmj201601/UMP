//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b23664db-f6b4-4e7e-96da-3369322a76bc
//        CLR Version:              4.0.30319.18408
//        Name:                     AlarmMessageItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4415.Models
//        File Name:                AlarmMessageItem
//
//        created by Charley at 2016/7/12 17:05:56
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Controls;


namespace UMPS4415.Models
{
    public class AlarmMessageItem : INotifyPropertyChanged
    {
        public long SerialID { get; set; }
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

        private string mIcon;

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
        }

        private string mColor;

        public string Color
        {
            get { return mColor; }
            set { mColor = value; OnPropertyChanged("Color"); }
        }

        private Brush mBrushColor;

        public Brush BrushColor
        {
            get { return mBrushColor; }
            set { mBrushColor = value; OnPropertyChanged("BrushColor"); }
        }

        private string mType;

        public string Type
        {
            get { return mType; }
            set { mType = value; OnPropertyChanged("Type"); }
        }

        private string mAlarmType;

        public string AlarmType
        {
            get { return mAlarmType; }
            set { mAlarmType = value; OnPropertyChanged("AlarmType"); }
        }

        private string mRank;

        public string Rank
        {
            get { return mRank; }
            set { mRank = value; OnPropertyChanged("Rank"); }
        }

        private string mValue;

        public string Value
        {
            get { return mValue; }
            set { mValue = value; OnPropertyChanged("Value"); }
        }

        private string mHoldTime;

        public string HoldTime
        {
            get { return mHoldTime; }
            set { mHoldTime = value; OnPropertyChanged("HoldTime"); }
        }

        private string mRelativeState;

        public string RelativeState
        {
            get { return mRelativeState; }
            set { mRelativeState = value; OnPropertyChanged("RelativeState"); }
        }

        public AlarmMessageInfo Info { get; set; }


        public UMPApp CurrentApp;

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", Name, SerialID, Value);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
