//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    123ab82f-e3ef-410e-a08a-d7fcb65cf1f4
//        CLR Version:              4.0.30319.18063
//        Name:                     ComponentItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPSetup.Models
//        File Name:                ComponentItem
//
//        created by Charley at 2015/12/28 15:59:17
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Updates;

namespace UMPSetup.Models
{
    public class ComponentItem : INotifyPropertyChanged
    {
        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private bool mIsChecked;

        public bool IsChecked
        {
            get { return mIsChecked; }
            set { mIsChecked = value; OnPropertyChanged("IsChecked"); }
        }

        private bool mIsEnabled;

        public bool IsEnabled
        {
            get { return mIsEnabled; }
            set { mIsEnabled = value; OnPropertyChanged("IsEnabled"); }
        }

        public InstallComponent Info { get; set; }

        public ComponentItem()
        {
            mIsChecked = true;
            mIsEnabled = true;
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
