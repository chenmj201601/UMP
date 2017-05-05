//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    12e6755d-ef23-43ea-9732-6e26d1501d74
//        CLR Version:              4.0.30319.42000
//        Name:                     FavoriteModuleItem
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.Models
//        File Name:                FavoriteModuleItem
//
//        created by Charley at 2016/3/9 16:00:15
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Controls;


namespace UMPS1206.Models
{
    public class FavoriteModuleItem:INotifyPropertyChanged
    {
        public int ModuleID { get; set; }
        public int AppID { get; set; }

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

        private string mIcon;

        public string Icon
        {
            get { return mIcon; }
            set { mIcon = value; OnPropertyChanged("Icon"); }
        }

        private int mUseCount;

        public int UseCount
        {
            get { return mUseCount; }
            set { mUseCount = value; OnPropertyChanged("UseCount"); }
        }

        private Brush mBackground;

        public Brush Background
        {
            get { return mBackground; }
            set { mBackground = value; OnPropertyChanged("Background"); }
        }

        public BasicAppInfo ModuleInfo { get; set; }
        public ModuleUsageInfo UsageInfo { get; set; }

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
