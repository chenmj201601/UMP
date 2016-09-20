//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    523b32fb-5744-4b68-8017-01c2d329d635
//        CLR Version:              4.0.30319.18408
//        Name:                     WidgetPropertyItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206.Models
//        File Name:                WidgetPropertyItem
//
//        created by Charley at 2016/5/3 17:41:08
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.ComponentModel;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Common12002;
using VoiceCyber.UMP.Controls;


namespace UMPS1206.Models
{
    public class WidgetPropertyItem : INotifyPropertyChanged
    {
        public long WidgetID { get; set; }
        public int PropertyID { get; set; }

        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }

        private string mDescription;

        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }

        private bool mShowDescription;

        public bool ShowDescription
        {
            get { return mShowDescription; }
            set { mShowDescription = value; OnPropertyChanged("ShowDescription"); }
        }

        public WidgetPropertyInfo PropertyInfo { get; set; }
        public UserWidgetPropertyValue PropertyValue { get; set; }

        public UMPUserControl Editor;

        public UMPApp CurrentApp;
        public WidgetItem WidgetItem;
        public IList<WidgetPropertyItem> ListAllPropertyItems;
        public IList<UserWidgetPropertyValue> ListAllPropertyValues; 

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", WidgetID, PropertyID, Name);
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
