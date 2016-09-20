//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    18e10f28-4dc9-4c6b-8abc-349fc66bd8ad
//        CLR Version:              4.0.30319.42000
//        Name:                     ModuleGroupItem
//        Computer:                 DESKTOP-AH05P0E
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.Models
//        File Name:                ModuleGroupItem
//
//        created by Charley at 2016/3/10 14:57:37
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;
using VoiceCyber.UMP.Controls;


namespace UMPS1206.Models
{
    public class ModuleGroupItem : INotifyPropertyChanged
    {
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

        private ObservableCollection<BasicModuleItem> mListApps;

        public ObservableCollection<BasicModuleItem> ListApps
        {
            get { return mListApps; }
        }

        public UMPApp CurrentApp;

        public ModuleGroupItem()
        {
            mListApps = new ObservableCollection<BasicModuleItem>();
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
