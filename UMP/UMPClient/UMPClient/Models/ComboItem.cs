//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    d5af767e-67f3-4b88-92c2-829c7c7379b9
//        CLR Version:              4.0.30319.42000
//        Name:                     ComboItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPClient.Models
//        File Name:                ComboItem
//
//        Created by Charley at 2016/8/23 14:21:40
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;


namespace UMPClient.Models
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

        private string mStrValue;

        public string StrValue
        {
            get { return mStrValue; }
            set { mStrValue = value; OnPropertyChanged("StrValue"); }
        }

        private int mIntValue;

        public int IntValue
        {
            get { return mIntValue; }
            set { mIntValue = value; OnPropertyChanged("IntValue"); }
        }

        public object Data { get; set; }

        public override string ToString()
        {
            return Display;
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
