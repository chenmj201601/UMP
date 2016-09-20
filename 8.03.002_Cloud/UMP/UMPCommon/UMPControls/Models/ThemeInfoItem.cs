//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c1d17e9a-564b-40b2-b7b2-df4a98ba48a2
//        CLR Version:              4.0.30319.18444
//        Name:                     ThemeInfoItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls.Models
//        File Name:                ThemeInfoItem
//
//        created by Charley at 2014/8/24 21:26:44
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace VoiceCyber.UMP.Controls.Models
{
    public class ThemeInfoItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private string mDisplay;
        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value;OnPropertyChanged("Display"); }
        }
        public string Description { get; set; }
        public string ThumbImage { get; set; }

        private bool mIsSelected;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set
            {
                mIsSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
