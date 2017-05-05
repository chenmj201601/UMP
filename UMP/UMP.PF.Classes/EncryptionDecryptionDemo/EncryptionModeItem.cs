//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8810f92c-8cb2-4f97-8ed5-1a7c39b5f240
//        CLR Version:              4.0.30319.18444
//        Name:                     KeyIVTypeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                EncryptionDecryptionDemo
//        File Name:                KeyIVTypeItem
//
//        created by Charley at 2015/2/27 13:05:11
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common;

namespace EncryptionDecryptionDemo
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
