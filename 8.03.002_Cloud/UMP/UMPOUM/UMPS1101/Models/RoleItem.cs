//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8ad7ee09-36e9-44b4-968a-a0a2fcd7a556
//        CLR Version:              4.0.30319.18444
//        Name:                     RoleItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Models
//        File Name:                RoleItem
//
//        created by Charley at 2014/9/22 12:10:20
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS1101.Models
{
    public class RoleItem : CheckableItemBase
    {
        
        public long RoleId { get; set; }
        private string mName ;
        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }
        public string Description { get; set; }
      
        private bool mIsSelected;

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }
    }
}
