//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    29a72249-a973-4d17-ad6a-9f6bfbea955a
//        CLR Version:              4.0.30319.18444
//        Name:                     Models
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VCCustomControlsDemo
//        File Name:                Models
//
//        created by Charley at 2014/7/18 15:55:16
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.Wpf.CustomControls;

namespace VCCustomControlsDemo
{
    public class PersonItem : INotifyPropertyChanged
    {
        public int ID
        {
            get { return mID; }
            set { mID = value; SubPropertyChanged("ID"); }

        }

        public string FirstName
        {
            get { return mFirstName; }
            set { mFirstName = value; SubPropertyChanged("FirstName"); }
        }

        public string LastName
        {
            get { return mLastName; }
            set { mLastName = value; SubPropertyChanged("LastName"); }
        }

        public int Age
        {
            get { return mAge; }
            set { mAge = value; SubPropertyChanged("Age"); }
        }

        public bool IsChecked
        {
            get { return mIsChecked; }
            set { mIsChecked = value; SubPropertyChanged("IsChecked"); }
        }

        public string Info
        {
            get
            {
                return string.Format("ID={0};FirstName={1};LastName={2};IsChecked={3}", mID, mFirstName, mLastName, mIsChecked);
            }
        }

        private int mID;
        private string mFirstName;
        private string mLastName;
        private int mAge;
        private string mInfo;
        private bool mIsChecked;

        private void SubPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
                PropertyChanged(this, new PropertyChangedEventArgs("Info"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class Employee : CheckableItemBase
    {
        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mJob;

        public string Job
        {
            get { return mJob; }
            set { mJob = value; OnPropertyChanged("Job"); }
        }

        private int mAge;

        public int Age
        {
            get { return mAge; }
            set { mAge = value; OnPropertyChanged("Age"); }
        }

     
    }
}
