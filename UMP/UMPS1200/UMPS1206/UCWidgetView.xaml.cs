//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5c00ba6f-6ebe-4ef5-816b-9fc9f41ad7b1
//        CLR Version:              4.0.30319.42000
//        Name:                     UCWidgetView
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206
//        File Name:                UCWidgetView
//
//        created by Charley at 2016/3/2 16:38:24
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS1206.Models;
using UMPS1206.WidgetViews;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12002;
using VoiceCyber.UMP.Controls;

namespace UMPS1206
{
    /// <summary>
    /// UCWidgetView.xaml 的交互逻辑
    /// </summary>
    public partial class UCWidgetView
    {
        public IList<BasicDataInfo> ListBasicDataInfos;

        private bool mIsInited;
        private List<ToolButtonItem> mListToolBarBtns;
        private object mView;
        private bool mIsCollasped;

        public UCWidgetView()
        {
            InitializeComponent();

            mListToolBarBtns = new List<ToolButtonItem>();

            Loaded += UCWidgetView_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnCancel.Click += BtnCancel_Click;
        }

        void UCWidgetView_Loaded(object sender, RoutedEventArgs e)
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
                if (WidgetItem == null) { return; }
                WidgetItem.WidgetView = this;
                DataContext = WidgetItem;
                CurrentApp = WidgetItem.CurrentApp;
                ListBasicDataInfos = WidgetItem.ListBasicDataInfos;
                InitToolBarBtns();
                UMPUserControl view = null;
                switch (WidgetItem.WidgetID)
                {
                    case S1206Consts.WIDGET_ID_FAVORITEMODULE:
                        view = new UCFavoriteModuleView();
                        break;
                    case S1206Consts.WIDGET_ID_OPTHISTORY:
                        view = new UCOptLogView();
                        break;
                    case S1206Consts.WIDGET_ID_FASTQUERY:
                        view = new UCFastQueryView();
                        break;
                }
                if (view != null)
                {
                    mView = view;
                    view.CurrentApp = CurrentApp;
                    var widgetView = view as IWidgetView;
                    widgetView.WidgetItem = WidgetItem;
                    widgetView.ListBasicDataInfos = ListBasicDataInfos;
                    widgetView.IsCenter = WidgetItem.IsCenter;
                    widgetView.IsFull = WidgetItem.IsFull;
                    BorderContent.Child = view;
                }
                CreateToolBarBtns();

                InitButtonLang();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitToolBarBtns()
        {
            try
            {
                mListToolBarBtns.Clear();

                ToolButtonItem item = new ToolButtonItem();
                item.Name = S1206Consts.TOOLBTN_REFRESH;
                item.Display = CurrentApp.GetLanguageInfo("1206TB001", "Refresh");
                item.ToolTip = item.Display;
                item.Icon = "Images/00003.png";
                item.Data = WidgetItem;
                mListToolBarBtns.Add(item);

                item = new ToolButtonItem();
                item.Name = S1206Consts.TOOLBTN_EXPAND;
                if (mIsCollasped)
                {
                    item.Display = CurrentApp.GetLanguageInfo("1206TB002", "Expand/Collaspe");
                    item.ToolTip = item.Display;
                    item.Icon = "Images/00001.png";
                    item.Data = WidgetItem;
                    mListToolBarBtns.Add(item);
                }
                else
                {
                    item.Display = CurrentApp.GetLanguageInfo("1206TB002", "Expand/Collaspe");
                    item.ToolTip = item.Display;
                    item.Icon = "Images/00002.png";
                    item.Data = WidgetItem;
                    mListToolBarBtns.Add(item);
                }

                item = new ToolButtonItem();
                item.Name = S1206Consts.TOOLBTN_FULL;
                item.Display = CurrentApp.GetLanguageInfo("1206TB003", "Full Screen");
                item.ToolTip = item.Display;
                item.Icon = "Images/00004.png";
                item.Data = WidgetItem;
                mListToolBarBtns.Add(item);

                item = new ToolButtonItem();
                item.Name = S1206Consts.TOOLBTN_CONFIG;
                item.Display = CurrentApp.GetLanguageInfo("1206TB004", "Config");
                item.ToolTip = item.Display;
                item.Icon = "Images/00009.png";
                item.Data = WidgetItem;
                mListToolBarBtns.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateToolBarBtns()
        {
            try
            {
                PanelToolBar.Children.Clear();
                for (int i = 0; i < mListToolBarBtns.Count; i++)
                {
                    var item = mListToolBarBtns[i];
                    string strCode = item.Name;
                    Button btn = new Button();
                    btn.DataContext = item;
                    switch (strCode)
                    {
                        case S1206Consts.TOOLBTN_FULL:
                            btn.Command = ToolBtnCommand;
                            btn.CommandParameter = item;
                            break;
                        case S1206Consts.TOOLBTN_EXPAND:
                        case S1206Consts.TOOLBTN_REFRESH:
                        case S1206Consts.TOOLBTN_CONFIG:
                            btn.Click += ToolBtn_Click;
                            break;
                    }
                    btn.SetResourceReference(StyleProperty, "BtnWidgetHeadToolBarStyle");
                    PanelToolBar.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Event Handlers

        private void ToolBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn == null) { return; }
                var toolBtn = btn.DataContext as ToolButtonItem;
                if (toolBtn == null) { return; }
                string strCode = toolBtn.Name;
                switch (strCode)
                {
                    case S1206Consts.TOOLBTN_EXPAND:
                        mIsCollasped = !mIsCollasped;
                        BorderContent.Visibility = mIsCollasped ? Visibility.Collapsed : Visibility.Visible;
                        InitToolBarBtns();
                        CreateToolBarBtns();
                        break;
                    case S1206Consts.TOOLBTN_REFRESH:
                        if (mView != null)
                        {
                            var widgetView = mView as IWidgetView;
                            if (widgetView != null)
                            {
                                widgetView.Refresh();
                            }
                        }
                        break;
                    case S1206Consts.TOOLBTN_CONFIG:
                        OpenConfigForm();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BorderConfig.Visibility = Visibility.Collapsed;
                BorderContent.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var lister = BorderPropertyList.Child as UCWidgetPropertyLister;
                if (lister == null) { return; }
                OperationReturn optReturn = lister.SaveWidgetPropertyValues();
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Save Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                if (mView != null)
                {
                    var widgetView = mView as IWidgetView;
                    if (widgetView != null)
                    {
                        widgetView.Refresh();
                    }
                }
                BorderConfig.Visibility = Visibility.Collapsed;
                BorderContent.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void OpenConfigForm()
        {
            try
            {
                UCWidgetPropertyLister lister = new UCWidgetPropertyLister();
                lister.CurrentApp = CurrentApp;
                lister.WidgetItem = WidgetItem;
                BorderPropertyList.Child = lister;
                BorderContent.Visibility = Visibility.Collapsed;
                BorderConfig.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void InitButtonLang()
        {
            try
            {
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnCancel.Content = CurrentApp.GetLanguageInfo("COM002", "Cancel");
            }
            catch { }
        }

        #endregion


        #region WidgetItem

        public static readonly DependencyProperty WidgetItemProperty =
            DependencyProperty.Register("WidgetItem", typeof(WidgetItem), typeof(UCWidgetView), new PropertyMetadata(default(WidgetItem)));

        public WidgetItem WidgetItem
        {
            get { return (WidgetItem)GetValue(WidgetItemProperty); }
            set { SetValue(WidgetItemProperty, value); }
        }

        #endregion


        #region FullScreenCommand

        private static RoutedUICommand mFullScreenCommand = new RoutedUICommand();

        public static RoutedUICommand FullScreenCommand
        {
            get { return mFullScreenCommand; }
        }

        #endregion


        #region Tool Button Command

        private static RoutedUICommand mToolBtnCommand = new RoutedUICommand();

        public static RoutedUICommand ToolBtnCommand
        {
            get { return mToolBtnCommand; }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                InitToolBarBtns();
                CreateToolBarBtns();
                InitButtonLang();
                var view = mView as UMPUserControl;
                if (view != null)
                {
                    view.ChangeLanguage();
                }
                var lister = BorderPropertyList.Child as UMPUserControl;
                if (lister != null)
                {
                    lister.ChangeLanguage();
                }
            }
            catch { }
        }

        #endregion

    }
}
