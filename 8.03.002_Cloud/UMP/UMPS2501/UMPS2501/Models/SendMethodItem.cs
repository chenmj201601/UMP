//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4d441677-042c-45f8-9cda-40f48dad1748
//        CLR Version:              4.0.30319.18063
//        Name:                     SendMethodItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501.Models
//        File Name:                SendMethodItem
//
//        created by Charley at 2015/5/28 16:16:42
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common25011;

namespace UMPS2501.Models
{
    public class SendMethodItem:INotifyPropertyChanged
    {
        public AlarmReceiverInfo Info { get; set; }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        private string mIcon;

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
        }

        #region PropertyChanged Event

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
