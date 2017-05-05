//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e12da979-ba9e-49a5-854c-943c2d6daf6f
//        CLR Version:              4.0.30319.42000
//        Name:                     BasicModuleItem
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.Models
//        File Name:                BasicModuleItem
//
//        created by Charley at 2016/3/10 14:54:49
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Controls;


namespace UMPS1206.Models
{
    public class BasicModuleItem:INotifyPropertyChanged
    {
        public int ModuleID { get; set; }

        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mTitle;

        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; OnPropertyChanged("Title"); }
        }

        private string mTip;

        public string Tip
        {
            get { return mTip; }
            set { mTip = value; OnPropertyChanged("Tip"); }
        }

        private string mCategory;

        public string Category
        {
            get { return mCategory; }
            set { mCategory = value; OnPropertyChanged("Category"); }
        }

        private string mIcon;

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
        }

        public BasicAppInfo Info { get; set; }

        public UMPApp CurrentApp;


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
