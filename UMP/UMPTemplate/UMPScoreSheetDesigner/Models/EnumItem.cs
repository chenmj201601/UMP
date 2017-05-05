//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8df0ec3d-490c-4b08-8a7f-66c1da7caaee
//        CLR Version:              4.0.30319.18444
//        Name:                     EnumItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPScoreSheetDesigner.Models
//        File Name:                EnumItem
//
//        created by Charley at 2014/7/29 11:47:47
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.ComponentModel;

namespace UMPScoreSheetDesigner.Models
{
    public class EnumItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Display { get; set; }
        public Type Type { get; set; }

        private bool mIsSelected;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; SubPropertyChanged("IsSelected"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SubPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
