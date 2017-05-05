//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0e1e5119-2ad0-4223-8b49-ae683c6d8981
//        CLR Version:              4.0.30319.18063
//        Name:                     UCSvnUpdate
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder
//        File Name:                UCSvnUpdate
//
//        created by Charley at 2015/12/23 10:01:14
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using UMPBuilder.Models;

namespace UMPBuilder
{
    /// <summary>
    /// UCSvnUpdate.xaml 的交互逻辑
    /// </summary>
    public partial class UCSvnUpdate :IOptObjectLister
    {

        public List<OptObjectItem> ListOptObjects;
        public MainWindow PageParent;

        private ObservableCollection<OptObjectItem> mListOptObjItems;

        public UCSvnUpdate()
        {
            InitializeComponent();

            mListOptObjItems = new ObservableCollection<OptObjectItem>();

            Loaded += UCSvnUpdate_Loaded;
        }

        void UCSvnUpdate_Loaded(object sender, RoutedEventArgs e)
        {
            ListViewSvnUpdate.ItemsSource = mListOptObjItems;
            InitOptObjItems();
            Init();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewSvnUpdate.ItemsSource);
            if (view != null)
            {
                if (view.GroupDescriptions != null)
                {
                    view.GroupDescriptions.Clear();
                    view.GroupDescriptions.Add(new PropertyGroupDescription("StrCategory"));
                }
            }
        }

        private void Init()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void InitOptObjItems()
        {
            try
            {
                mListOptObjItems.Clear();
                if (ListOptObjects == null) { return; }
                var items = ListOptObjects.Where(o => o.Operation == UMPBuilderConsts.OPTOBJ_SVNUPDATE).ToList();
                for (int i = 0; i < items.Count; i++)
                {
                    mListOptObjItems.Add(items[i]);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ShowErrorMessage(string msg)
        {
            if (PageParent != null)
            {
                PageParent.ShowErrorMessage(msg);
            }
        }

        public OptObjectItem GetSelectedItem()
        {
            var item = ListViewSvnUpdate.SelectedItem as OptObjectItem;
            if (item == null)
            {
                item = mListOptObjItems.FirstOrDefault();
            }
            return item;
        }

        public void SetSelectedItem(OptObjectItem item)
        {
            try
            {
                var index = mListOptObjItems.IndexOf(item);
                if (index >= 0)
                {
                    ListViewSvnUpdate.SelectedIndex = index;
                    ListViewSvnUpdate.ScrollIntoView(item);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }
    }
}
