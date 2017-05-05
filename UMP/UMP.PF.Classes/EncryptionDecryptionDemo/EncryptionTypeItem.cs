//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cb917ddd-1ba4-4f48-a30c-79dd134c46bf
//        CLR Version:              4.0.30319.18444
//        Name:                     EncryptionTypeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                EncryptionDecryptionDemo
//        File Name:                EncryptionTypeItem
//
//        created by Charley at 2015/2/27 13:03:16
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;

namespace EncryptionDecryptionDemo
{
    public class EncryptionTypeItem:INotifyPropertyChanged
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
