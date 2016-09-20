//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9db8a2ab-275d-4052-8a1a-cfb5b5779476
//        CLR Version:              4.0.30319.18408
//        Name:                     InstalledItems
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                LoggingUpdater
//        File Name:                InstalledItems
//
//        created by Charley at 2016/6/13 18:13:39
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Updates;


namespace LoggingUpdater
{
    public class InstalledItems : INotifyPropertyChanged
    {
        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mVersion;

        public string Version
        {
            get { return mVersion; }
            set { mVersion = value; OnPropertyChanged("Version"); }
        }

        public InstallProduct Info { get; set; }


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
