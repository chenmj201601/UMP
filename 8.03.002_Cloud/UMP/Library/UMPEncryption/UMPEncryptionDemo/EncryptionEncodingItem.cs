//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    80d1fe89-aeda-4f09-a9bd-0653e0756129
//        CLR Version:              4.0.30319.18063
//        Name:                     EncryptionEncodingItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPEncryptionDemo
//        File Name:                EncryptionEncodingItem
//
//        created by Charley at 2015/11/6 11:51:09
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Text;

namespace UMPEncryptionDemo
{
    public class EncryptionEncodingItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public Encoding Encoding { get; set; }

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
