//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    69fbccff-45f9-4029-a903-a1ce980ac7f3
//        CLR Version:              4.0.30319.18063
//        Name:                     UCFileCopy
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder
//        File Name:                UCFileCopy
//
//        created by Charley at 2015/12/23 10:58:31
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
    /// UCFileCopy.xaml 的交互逻辑
    /// </summary>
    public partial class UCFileCopy:IOptObjectLister
    {

        public List<OptObjectItem> ListOptObjects;
        public MainWindow PageParent;

        private ObservableCollection<OptObjectItem> mListOptObjItems;


        public UCFileCopy()
        {
            InitializeComponent();

            mListOptObjItems = new ObservableCollection<OptObjectItem>();

            Loaded += UCFileCopy_Loaded;
        }

        void UCFileCopy_Loaded(object sender, RoutedEventArgs e)
        {
            ListViewFileCopy.ItemsSource = mListOptObjItems;
            InitOptObjItems();
            Init();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewFileCopy.ItemsSource);
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
                var items = ListOptObjects.Where(o => o.Operation == UMPBuilderConsts.OPTOBJ_COPYFILE).ToList();
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
            var item = ListViewFileCopy.SelectedItem as OptObjectItem;
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
                    ListViewFileCopy.SelectedIndex = index;
                    ListViewFileCopy.ScrollIntoView(item);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }
    }
}
