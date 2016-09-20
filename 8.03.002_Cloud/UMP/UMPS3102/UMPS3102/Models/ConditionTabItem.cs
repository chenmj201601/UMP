//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fd88562e-0fdf-4d45-932d-fb910b86258c
//        CLR Version:              4.0.30319.18444
//        Name:                     ConditionTabItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                ConditionTabItem
//
//        created by Charley at 2014/11/7 17:25:45
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace UMPS3102.Models
{
    public class ConditionTabItem : INotifyPropertyChanged
    {
        public int TabIndex { get; set; }
        private string mTabName;

        public string TabName
        {
            get { return mTabName; }
            set
            {
                mTabName = value;
                OnPropertyChanged("TabName");
            }
        }

        private ObservableCollection<ConditionItemItem> mListItems;

        public ObservableCollection<ConditionItemItem> Items
        {
            get { return mListItems; }
        }

        public ConditionTabItem()
        {
            mListItems = new ObservableCollection<ConditionItemItem>();
        }

        public void SetSortID()
        {
            for (int i = 0; i < mListItems.Count; i++)
            {
                mListItems[i].SortID = i;
                mListItems[i].ConditionItem.SortID = i;
            }
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
