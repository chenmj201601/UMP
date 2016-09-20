using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UMPS3108.Models
{
    public class CombinedParamTabItem : INotifyPropertyChanged
    {
        /// <summary>
        /// 这个ID对应的是可组合的参数大项的ID
        /// </summary>
        public long ID { get; set; }

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

        private bool mIsEnable;

        public bool IsEnable
        {
            get { return mIsEnable; }
            set { mIsEnable = value; OnPropertyChanged("IsEnable"); }
        }

        private ObservableCollection<CombinedParamItemModel> mListItems;

        public ObservableCollection<CombinedParamItemModel> Items
        {
            get { return mListItems; }
        }

        public CombinedParamTabItem()
        {
            mListItems = new ObservableCollection<CombinedParamItemModel>();
        }

        public void SetSortID()
        {
            for (int i = 0; i < mListItems.Count; i++)
            {
                mListItems[i].SortID = i;
                mListItems[i].ParamItem.SortID = i;
                mListItems[i].ID = ID;
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
