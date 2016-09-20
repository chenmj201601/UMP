//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    58e23f4c-77f8-4502-8d40-1fdc9ad21cfe
//        CLR Version:              4.0.30319.18063
//        Name:                     UCProjectCompile
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder
//        File Name:                UCProjectCompile
//
//        created by Charley at 2015/12/22 16:33:45
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
    /// UCProjectCompile.xaml 的交互逻辑
    /// </summary>
    public partial class UCProjectCompile : IOptObjectLister
    {
        public List<OptObjectItem> ListOptObjects;
        public MainWindow PageParent;

        private ObservableCollection<OptObjectItem> mListOptObjItems;

        public UCProjectCompile()
        {
            InitializeComponent();

            mListOptObjItems = new ObservableCollection<OptObjectItem>();

            Loaded += UCProjectCompile_Loaded;
        }

        void UCProjectCompile_Loaded(object sender, RoutedEventArgs e)
        {
            ListViewCompile.ItemsSource = mListOptObjItems;
            InitOptObjItems();
            Init();

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(ListViewCompile.ItemsSource);
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
                var items = ListOptObjects.Where(o => o.Operation == UMPBuilderConsts.OPTOBJ_COMPILE).ToList();
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
            var item = ListViewCompile.SelectedItem as OptObjectItem;
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
                    ListViewCompile.SelectedIndex = index;
                    ListViewCompile.ScrollIntoView(item);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }
    }
}
