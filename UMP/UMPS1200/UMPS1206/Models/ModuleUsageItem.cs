//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d09278a5-f1b7-4fda-9ba5-a1e172947df7
//        CLR Version:              4.0.30319.42000
//        Name:                     ModuleUsageItem
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.Models
//        File Name:                ModuleUsageItem
//
//        created by Charley at 2016/3/9 16:16:33
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.ComponentModel;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Controls;


namespace UMPS1206.Models
{
    public class ModuleUsageItem:INotifyPropertyChanged
    {
        private string mUser;

        public string User
        {
            get { return mUser; }
            set { mUser = value; OnPropertyChanged("User"); }
        }

        public DateTime BeginTime { get; set; }

        private string mUseTime;

        public string UseTime
        {
            get { return mUseTime; }
            set { mUseTime = value; OnPropertyChanged("UseTime"); }
        }

        private string mHostInfo;

        public string HostInfo
        {
            get { return mHostInfo; }
            set { mHostInfo = value; OnPropertyChanged("HostInfo"); }
        }

        public FavoriteModuleItem ModuleItem { get; set; }
        public ModuleUsageInfo UsageInfo { get; set; }

        public UMPApp CurrentApp;


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
