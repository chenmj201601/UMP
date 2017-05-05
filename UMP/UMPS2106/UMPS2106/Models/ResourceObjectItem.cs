//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    e3c0b143-ca31-4553-a668-9c03b170ab54
//        CLR Version:              4.0.30319.42000
//        Name:                     ResourceObjectItem
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS2106.Models
//        File Name:                ResourceObjectItem
//
//        Created by Charley at 2016/10/20 14:04:00
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common;


namespace UMPS2106.Models
{
    public class ResourceObjectItem : INotifyPropertyChanged
    {
        public long ObjID { get; set; }
        public int ObjType { get; set; }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        private bool? mIsChecked;

        public bool? IsChecked
        {
            get { return mIsChecked; }
            set { mIsChecked = value; OnPropertyChanged("IsChecked"); }
        }

        public ResourceObject Info { get; set; }

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
