//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    add723d4-3dff-4c80-b284-0dd8e19f01fc
//        CLR Version:              4.0.30319.18408
//        Name:                     UCRegionItemViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411
//        File Name:                UCRegionItemViewer
//
//        created by Charley at 2016/7/17 18:21:21
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Windows;
using UMPS4411.Models;

namespace UMPS4411
{
    /// <summary>
    /// UCRegionItemViewer.xaml 的交互逻辑
    /// </summary>
    public partial class UCRegionItemViewer
    {

        private bool mIsInited;

        public UCRegionItemViewer()
        {
            InitializeComponent();

            Loaded += UCRegionItemViewer_Loaded;
        }

        void UCRegionItemViewer_Loaded(object sender, RoutedEventArgs e)
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
                if (ChildRegionItem == null) { return; }
                CurrentApp = ChildRegionItem.CurrentApp;
                ChildRegionItem.Viewer = this;
                TxtRegionName.Text = ChildRegionItem.Name;
                TxtWait.Text = CurrentApp.GetLanguageInfo("4411018", "Collecting data..., please wait.");
                TxtNoData.Text = CurrentApp.GetLanguageInfo("4411019", "There is no data to display.");
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void Refresh()
        {
            try
            {
                if (ChildRegionItem == null) { return; }
                List<RegionStateItem> listItems = new List<RegionStateItem>();
                for (int i = 0; i < ChildRegionItem.ListRegionStateItems.Count; i++)
                {
                    var regionStateItem = ChildRegionItem.ListRegionStateItems[i];
                    if (regionStateItem.SeatNum <= 0) { continue; }
                    listItems.Add(regionStateItem);
                }
                ChartRegionStates.ItemsSource = listItems;
                ListBoxRegionState.ItemsSource = listItems;

                GridWait.Visibility = Visibility.Collapsed;
                GridNoData.Visibility = listItems.Count <= 0 ? Visibility.Visible : Visibility.Collapsed;
                GridChart.Visibility = listItems.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region ChildRegionItem

        public static readonly DependencyProperty ChildRegionItemProperty =
            DependencyProperty.Register("ChildRegionItem", typeof(ChildRegionItem), typeof(UCRegionItemViewer), new PropertyMetadata(default(ChildRegionItem)));

        public ChildRegionItem ChildRegionItem
        {
            get { return (ChildRegionItem)GetValue(ChildRegionItemProperty); }
            set { SetValue(ChildRegionItemProperty, value); }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                TxtWait.Text = CurrentApp.GetLanguageInfo("4411018", "Collecting data..., please wait.");
                TxtNoData.Text = CurrentApp.GetLanguageInfo("4411019", "There is no data to display.");
            }
            catch { }
        }

        #endregion
    }
}
