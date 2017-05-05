//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    97684299-231c-49ac-a579-bbc34531de21
//        CLR Version:              4.0.30319.18408
//        Name:                     BugInfoItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater.Models
//        File Name:                BugInfoItem
//
//        created by Charley at 2016/8/4 10:14:51
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;


namespace UMPUpdater.Models
{
    public class BugInfoItem : INotifyPropertyChanged
    {
        private string mSerialNo;

        public string SerialNo
        {
            get { return mSerialNo; }
            set { mSerialNo = value; OnPropertyChanged("SerialNo"); }
        }

        private string mContent;

        public string Content
        {
            get { return mContent; }
            set { mContent = value; OnPropertyChanged("Content"); }
        }

        private string mVersion;

        public string Version
        {
            get { return mVersion; }
            set { mVersion = value; OnPropertyChanged("Version"); }
        }

        private int mModuleID;

        public int ModuleID
        {
            get { return mModuleID; }
            set { mModuleID = value; OnPropertyChanged("ModuleID"); }
        }

        private string mModule;

        public string Module
        {
            get { return mModule; }
            set { mModule = value; OnPropertyChanged("Module"); }
        }

        /// <summary>
        /// 0       UpdateModule
        /// </summary>
        public int Type { get; set; }

        public object Info { get; set; }


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
