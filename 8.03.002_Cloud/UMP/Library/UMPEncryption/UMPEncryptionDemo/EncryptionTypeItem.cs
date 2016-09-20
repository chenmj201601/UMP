//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4ccd6504-0ab9-423a-a7c8-c84f05858c10
//        CLR Version:              4.0.30319.18063
//        Name:                     EncryptionTypeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPEncryptionDemo
//        File Name:                EncryptionTypeItem
//
//        created by Charley at 2015/9/11 17:57:38
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace UMPEncryptionDemo
{
    public class EncryptionTypeItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Value { get; set; }

        private bool mIsSelected;

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
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
