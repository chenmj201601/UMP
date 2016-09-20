//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2dd76b39-a5f0-4991-adf2-4946508a17aa
//        CLR Version:              4.0.30319.18444
//        Name:                     Models
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPThemesDemo
//        File Name:                Models
//
//        created by Charley at 2014/8/14 15:22:55
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.ObjectModel;

namespace UMPThemesDemo
{
    public class PersonInfo
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
        public string Job { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", FullName, Name);
        }
    }

    public class DirInfo
    {
        public string Name { get; set; }
        public string FullPath { get; set; }

        private ObservableCollection<DirInfo> mChildren;

        public ObservableCollection<DirInfo> Children
        {
            get { return mChildren; }
        }

        public DirInfo()
        {
            mChildren = new ObservableCollection<DirInfo>();
        }
    }

    public class ColorInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
