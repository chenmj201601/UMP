//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c281e051-76f0-4c52-aff7-104f77408758
//        CLR Version:              4.0.30319.18408
//        Name:                     ObjItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4415.Models
//        File Name:                ObjItem
//
//        created by Charley at 2016/7/13 15:35:04
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;


namespace UMPS4415.Models
{
    public class ObjItem : CheckableItemBase
    {
        private long mObjID;

        public long ObjID
        {
            get { return mObjID; }
            set { mObjID = value; OnPropertyChanged("ObjID"); }
        }

        private int mObjType;

        public int ObjType
        {
            get { return mObjType; }
            set { mObjType = value; OnPropertyChanged("ObjType"); }
        }

        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mTip;

        public string Tip
        {
            get { return mTip; }
            set { mTip = value; OnPropertyChanged("Tip"); }
        }

        public object Data { get; set; }

        public UMPApp CurrentApp;
    }
}
