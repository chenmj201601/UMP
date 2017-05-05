//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fd30815b-f08f-4061-b736-01259d639c84
//        CLR Version:              4.0.30319.18444
//        Name:                     EnumItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Models
//        File Name:                EnumItem
//
//        created by Charley at 2014/10/14 17:29:10
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.ComponentModel;

namespace UMPS3101.Models
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
