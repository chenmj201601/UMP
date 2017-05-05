//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    aa8e532c-f2e7-4534-b6f0-e1cf239f2a4e
//        CLR Version:              4.0.30319.18408
//        Name:                     StateTypeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4414.Models
//        File Name:                StateTypeItem
//
//        created by Charley at 2016/6/24 16:16:07
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;


namespace UMPS4414.Models
{
    public class StateTypeItem : INotifyPropertyChanged
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

        private int mValue;

        public int Value
        {
            get { return mValue; }
            set { mValue = value; OnPropertyChanged("Value"); }
        }


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
