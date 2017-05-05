//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2e64919f-0158-4342-8a28-d6da3a4f628c
//        CLR Version:              4.0.30319.18408
//        Name:                     BugVersionItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater.Models
//        File Name:                BugVersionItem
//
//        created by Charley at 2016/8/7 16:12:29
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;


namespace UMPUpdater.Models
{
    public class BugVersionItem : INotifyPropertyChanged
    {
        private string mVersion;

        public string Version
        {
            get { return mVersion; }
            set { mVersion = value; OnPropertyChanged("Version"); }
        }

        private ObservableCollection<BugInfoItem> mListBugInfoItems = new ObservableCollection<BugInfoItem>();

        public ObservableCollection<BugInfoItem> ListBugInfoItems
        {
            get { return mListBugInfoItems; }
        }

        public UCBugVersionItem Viewer { get; set; }


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
