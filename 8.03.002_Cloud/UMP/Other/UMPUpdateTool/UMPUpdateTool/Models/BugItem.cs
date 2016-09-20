//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cd4d1c2e-2924-4209-805f-aa2c965e6bf3
//        CLR Version:              4.0.30319.18408
//        Name:                     BugItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdateTool.Models
//        File Name:                BugItem
//
//        created by Charley at 2016/5/31 16:08:24
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.ComponentModel;
using VoiceCyber.UMP.Updates;


namespace UMPUpdateTool.Models
{
    public class BugItem : INotifyPropertyChanged
    {
        private string mSerialNo;

        public string SerialNo
        {
            get { return mSerialNo; }
            set { mSerialNo = value; OnPropertyChanged("SerialNo"); }
        }

        private int mType;

        public int Type
        {
            get { return mType; }
            set { mType = value; OnPropertyChanged("Type"); }
        }

        private string mStrType;

        public string StrType
        {
            get { return mStrType; }
            set { mStrType = value; OnPropertyChanged("StrType"); }
        }

        private int mModuleID;

        public int ModuleID
        {
            get { return mModuleID; }
            set { mModuleID = value; OnPropertyChanged("ModuleID"); }
        }

        private string mModuleName;

        public string ModuleName
        {
            get { return mModuleName; }
            set { mModuleName = value; OnPropertyChanged("ModuleName"); }
        }

        private DateTime mUpdateDate;

        public DateTime UpdateDate
        {
            get { return mUpdateDate; }
            set { mUpdateDate = value; OnPropertyChanged("UpdateDate"); }
        }

        private string mStrUpdateDate;

        public string StrUpdateDate
        {
            get { return mStrUpdateDate; }
            set { mStrUpdateDate = value; OnPropertyChanged("StrUpdateDate"); }
        }

        private int mLevel;

        public int Level
        {
            get { return mLevel; }
            set { mLevel = value; OnPropertyChanged("Level"); }
        }

        private string mContent;

        public string Content
        {
            get { return mContent; }
            set { mContent = value; OnPropertyChanged("Content"); }
        }

        private string mContentLangID;

        public string ContentLangID
        {
            get { return mContentLangID; }
            set { mContentLangID = value; OnPropertyChanged("ContentLangID"); }
        }

        private string mModuleLangID;

        public string ModuleLangID
        {
            get { return mModuleLangID; }
            set { mModuleLangID = value; OnPropertyChanged("ModuleLangID"); }
        }

        public UpdateModule Info { get; set; }


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
