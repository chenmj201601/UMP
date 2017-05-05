//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    53f85e11-dd2a-4d85-8da9-1b466ae82493
//        CLR Version:              4.0.30319.42000
//        Name:                     AppInfoItem
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS1205.Models
//        File Name:                AppInfoItem
//
//        created by Charley at 2016/3/1 10:58:17
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common12001;


namespace UMPS1205.Models
{
    public class AppInfoItem : INotifyPropertyChanged
    {
        public int ModuleID { get; set; }
        private string mTitle;

        public string Title
        {
            get { return mTitle; }
            set { mTitle = value; OnPropertyChanged("Title"); }
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
