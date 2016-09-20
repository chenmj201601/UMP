//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e95c0735-10be-4a11-b14a-40d7ced3a1a2
//        CLR Version:              4.0.30319.42000
//        Name:                     AppGroupItem
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS1205.Models
//        File Name:                AppGroupItem
//
//        created by Charley at 2016/3/1 10:59:45
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;


namespace UMPS1205.Models
{
    public class AppGroupItem:INotifyPropertyChanged
    {
        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mTitle;

        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; OnPropertyChanged("Title"); }
        }

        private ObservableCollection<AppInfoItem> mListApps;

        public ObservableCollection<AppInfoItem> ListApps
        {
            get { return mListApps; }
        }

        public AppGroupItem()
        {
            mListApps = new ObservableCollection<AppInfoItem>();
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
