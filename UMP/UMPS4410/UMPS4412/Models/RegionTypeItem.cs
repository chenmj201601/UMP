//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    80d3e237-8480-412a-9171-84a16a78a1c0
//        CLR Version:              4.0.30319.18408
//        Name:                     RegionTypeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412.Models
//        File Name:                RegionTypeItem
//
//        created by Charley at 2016/5/11 11:38:51
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;


namespace UMPS4412.Models
{
    public class RegionTypeItem:INotifyPropertyChanged
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
