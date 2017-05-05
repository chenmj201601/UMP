//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    847c311d-9c61-4c06-b326-8b59030d74eb
//        CLR Version:              4.0.30319.18444
//        Name:                     LangInfoItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls.Models
//        File Name:                LangInfoItem
//
//        created by Charley at 2014/8/24 22:10:49
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace VoiceCyber.UMP.Controls.Models
{
    public class LangInfoItem : INotifyPropertyChanged
    {
        public int Code { get; set; }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

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
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
