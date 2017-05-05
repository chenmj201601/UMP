//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2a367f65-04bf-4650-9dc8-d8bb0f85bb38
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyValueEnumItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                PropertyValueEnumItem
//
//        created by Charley at 2015/1/20 11:43:44
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.ComponentModel;

namespace UMPS1110.Models
{
    public class PropertyValueEnumItem : INotifyPropertyChanged
    {
        private string mValue;
        public string Value
        {
            get { return mValue; }
            set { mValue = value; OnPropertyChanged("Value"); }
        }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        private string mDescription;

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        private bool mIsChecked;

        public bool IsChecked
        {
            get { return mIsChecked; }
            set { mIsChecked = value; OnPropertyChanged("IsChecked"); OnIsCheckedChanged(); }
        }

        private bool mIsSelected;

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        private int mSortID;

        public int SortID
        {
            get { return mSortID; }
            set { mSortID = value; OnPropertyChanged("SortID"); }
        }

        private string mIcon { get; set; }

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
        }
        public object Info { get; set; }


        #region PropertyChangedEvent

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion


        #region IsCheckedChangedEvent

        public event Action IsCheckedChanged;

        private void OnIsCheckedChanged()
        {
            if (IsCheckedChanged != null)
            {
                IsCheckedChanged();
            }
        }

        #endregion


        public override string ToString()
        {
            string str;
            if (!string.IsNullOrEmpty(Display))
            {
                str = Display;
            }
            else
            {
                str = string.Format("[{0}]", Value);
            }
            return str;
        }
    }
}
