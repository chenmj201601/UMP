//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    73445655-d1d4-4581-afb3-9498f7c85156
//        CLR Version:              4.0.30319.18063
//        Name:                     EncryptionModeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPEncryptionDemo
//        File Name:                EncryptionModeItem
//
//        created by Charley at 2015/9/11 17:57:19
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common;

namespace UMPEncryptionDemo
{
    public class EncryptionModeItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public int Value { get; set; }
        public EncryptionMode Mode { get; set; }
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
