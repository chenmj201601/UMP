//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    67328af7-d2fa-4e71-a317-13a6940f41eb
//        CLR Version:              4.0.30319.42000
//        Name:                     ThemeInfoItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1203
//        File Name:                ThemeInfoItem
//
//        created by Charley at 2016/2/17 17:44:00
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace UMPS1203
{
    public class ThemeInfoItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private string mDisplay;
        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
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
