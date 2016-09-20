//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    05a0c9bf-88c4-41fd-8f68-299867c87057
//        CLR Version:              4.0.30319.42000
//        Name:                     UCGroupItem
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS1205
//        File Name:                UCGroupItem
//
//        created by Charley at 2016/3/1 13:33:38
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS1205.Models;

namespace UMPS1205
{
    /// <summary>
    /// UCGroupItem.xaml 的交互逻辑
    /// </summary>
    public partial class UCGroupItem
    {

        #region Members

        private bool mIsInited;
        private ObservableCollection<AppInfoItem> mListAppItems; 

        #endregion


        public UCGroupItem()
        {
            InitializeComponent();

            mListAppItems = new ObservableCollection<AppInfoItem>();

            Loaded += UCGroupItem_Loaded;
        }

        void UCGroupItem_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        private void Init()
        {
            try
            {
                if (AppGroupItem == null) { return;}
                DataContext = AppGroupItem;
                //TxtTitle.Text = AppGroupItem.Title;
                mListAppItems = AppGroupItem.ListApps;
                ListBoxAppList.ItemsSource = mListAppItems;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public static readonly DependencyProperty AppGroupItemProperty =
            DependencyProperty.Register("AppGroupItem", typeof (AppGroupItem), typeof (UCGroupItem), new PropertyMetadata(default(AppGroupItem)));

        public AppGroupItem AppGroupItem
        {
            get { return (AppGroupItem) GetValue(AppGroupItemProperty); }
            set { SetValue(AppGroupItemProperty, value); }
        }
    }
}
