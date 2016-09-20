//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b7e5740b-b1ab-4726-b377-811bf6d171d5
//        CLR Version:              4.0.30319.18444
//        Name:                     ImageButtonItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Controls
//        File Name:                ImageButtonItem
//
//        created by Charley at 2014/12/8 17:32:56
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace VoiceCyber.NAudio.Controls
{
    public class ImageButtonItem:INotifyPropertyChanged
    {
        public string Name { get; set; }
        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        private string mToolTip;

        public string ToolTip
        {
            get { return mToolTip; }
            set { mToolTip = value; OnPropertyChanged("ToolTip"); }
        }

        private string mIcon;

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
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
