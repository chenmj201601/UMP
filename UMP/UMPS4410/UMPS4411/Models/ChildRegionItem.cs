//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0de897ea-a073-4736-80b6-d2ed4097e1fe
//        CLR Version:              4.0.30319.18408
//        Name:                     ChildRegionItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                ChildRegionItem
//
//        created by Charley at 2016/7/17 13:20:32
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Controls;


namespace UMPS4411.Models
{
    public class ChildRegionItem : INotifyPropertyChanged
    {
        public long RegionID { get; set; }
        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private ObservableCollection<RegionStateItem> mlistRegionStateItems =
            new ObservableCollection<RegionStateItem>();

        public ObservableCollection<RegionStateItem> ListRegionStateItems
        {
            get { return mlistRegionStateItems; }
        }

        private List<RegionExtensionItem> mListRegionExtItems = new List<RegionExtensionItem>();

        public List<RegionExtensionItem> ListRegionExtItems
        {
            get { return mListRegionExtItems; }
        } 

        public RegionInfo RegionInfo;
        public ObjItem RegionItem;

        public UCRegionItemViewer Viewer;

        public UMPApp CurrentApp;


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", RegionID, Name);
        }
    }
}
