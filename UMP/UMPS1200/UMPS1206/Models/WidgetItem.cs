//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1bea99ab-dcbe-4a9d-9091-335eaa8c3574
//        CLR Version:              4.0.30319.42000
//        Name:                     WidgetItem
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.Models
//        File Name:                WidgetItem
//
//        created by Charley at 2016/3/2 17:03:10
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.ComponentModel;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Controls;


namespace UMPS1206.Models
{
    public class WidgetItem : INotifyPropertyChanged
    {
        public long WidgetID { get; set; }

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

        private bool mIsFull;

        public bool IsFull
        {
            get { return mIsFull; }
            set { mIsFull = value; OnPropertyChanged("IsFull"); }
        }

        private bool mIsCenter;

        public bool IsCenter
        {
            get { return mIsCenter; }
            set { mIsCenter = value; OnPropertyChanged("IsCenter"); }
        }

        public WidgetInfo Info { get; set; }

        public UMPApp CurrentApp;

        public IList<BasicDataInfo> ListBasicDataInfos;

        public UMPUserControl View { get; set; }

        public UCWidgetView WidgetView { get; set; }

        public UMPMainView MainView { get; set; }

        public static WidgetItem CreateItem(WidgetInfo info)
        {
            WidgetItem item = new WidgetItem();
            item.WidgetID = info.WidgetID;
            item.Name = info.Name;
            item.Title = info.Title;
            item.Info = info;
            return item;
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
