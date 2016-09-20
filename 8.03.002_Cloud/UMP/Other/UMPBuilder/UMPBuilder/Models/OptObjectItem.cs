//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8b72e792-6df0-486b-b6bb-4fe504baccdd
//        CLR Version:              4.0.30319.18063
//        Name:                     OptObjectItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder.Models
//        File Name:                OptObjectItem
//
//        created by Charley at 2015/12/22 15:57:01
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace UMPBuilder.Models
{
    public class OptObjectItem : INotifyPropertyChanged
    {

        private bool mIsChecked;

        public bool IsChecked
        {
            get { return mIsChecked; }
            set { mIsChecked = value; OnPropertyChanged("IsChecked"); }
        }

        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mDescription;

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        private int mCategory;

        public int Category
        {
            get { return mCategory; }
            set { mCategory = value; OnPropertyChanged("Category"); }
        }

        private string mStrCategory;

        public string StrCategory
        {
            get { return mStrCategory; }
            set { mStrCategory = value; OnPropertyChanged("StrCategory"); }
        }

        private int mStatus;

        public int Status
        {
            get { return mStatus; }
            set { mStatus = value; OnPropertyChanged("Status"); }
        }

        private string mStrStatus;

        public string StrStatus
        {
            get { return mStrStatus; }
            set { mStrStatus = value; OnPropertyChanged("StrStatus"); }
        }

        private string mStrMessage;

        public string StrMessage
        {
            get { return mStrMessage; }
            set { mStrMessage = value; OnPropertyChanged("StrMessage"); }
        }

        public object Info { get; set; }

        public int Operation { get; set; }

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
