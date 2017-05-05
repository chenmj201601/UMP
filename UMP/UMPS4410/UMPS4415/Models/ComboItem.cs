//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5482176d-a237-497d-82bf-42a3ca9a56ec
//        CLR Version:              4.0.30319.18408
//        Name:                     ComboItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4415.Models
//        File Name:                ComboItem
//
//        created by Charley at 2016/7/13 09:31:49
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;


namespace UMPS4415.Models
{
    public class ComboItem : INotifyPropertyChanged
    {
        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        private int mIntValue;

        public int IntValue
        {
            get { return mIntValue; }
            set { mIntValue = value; OnPropertyChanged("IntValue"); }
        }

        private long mLongValue;

        public long LongValue
        {
            get { return mLongValue; }
            set { mLongValue = value; OnPropertyChanged("LongValue"); }
        }

        private string mStrValue;

        public string StrValue
        {
            get { return mStrValue; }
            set { mStrValue = value; OnPropertyChanged("StrValue"); }
        }

        public object Data { get; set; }

        public override string ToString()
        {
            //return string.Format("[{0}][{1}][{2}][{3}]", Name, Display, IntValue, StrValue);
            return string.Format("{0}", Display);
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
