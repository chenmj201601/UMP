//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e97a44ca-dab7-450a-b58a-24b4257f39c2
//        CLR Version:              4.0.30319.42000
//        Name:                     DashboardMainView
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS1206
//        File Name:                DashboardMainView
//
//        created by Charley at 2016/3/2 15:42:12
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS1206.Models;
using UMPS1206.Wcf12001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Common12002;
using VoiceCyber.UMP.Communications;

namespace UMPS1206
{
    /// <summary>
    /// DashboardMainView.xaml 的交互逻辑
    /// </summary>
    public partial class DashboardMainView
    {

        #region Members

        private List<ToolButtonItem> mListToolButtonItems;
        private List<WidgetInfo> mListWidgetInfos;
        private List<BasicDataInfo> mListBasicDataInfos;
        private ObservableCollection<WidgetItem> mListLeftWidgetItems;
        private ObservableCollection<WidgetItem> mListCenterWidgetItems;
        private WidgetItem mFullWidgetItem;

        #endregion


        public DashboardMainView()
        {
            InitializeComponent();

            mListToolButtonItems = new List<ToolButtonItem>();
            mListWidgetInfos = new List<WidgetInfo>();
            mListBasicDataInfos = new List<BasicDataInfo>();
            mListLeftWidgetItems = new ObservableCollection<WidgetItem>();
            mListCenterWidgetItems = new ObservableCollection<WidgetItem>();
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "DashboardMainView";
                StylePath = "UMPS1206/DashboardMainView.xaml";

                ListBoxLeftWidgets.ItemsSource = mListLeftWidgetItems;
                ListBoxCenterWidgets.ItemsSource = mListCenterWidgetItems;

                CommandBindings.Add(new CommandBinding(UCWidgetView.ToolBtnCommand, ToolBtnCommand_Executed,
                    (s, ce) => ce.CanExecute = true));

                base.Init();

                SetBusy(true, string.Format("Loading basic informations..."));
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    InitToolButtonItems();
                    LoadBasicDataInfos();
                    LoadWidgetInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    CurrentApp.SendLoadedMessage();

                    CreateToolBtnItems();
                    CreateWidgetItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitToolButtonItems()
        {
            try
            {
                mListToolButtonItems.Clear();
                ToolButtonItem item = new ToolButtonItem();
                item.Name = S1206Consts.TOOLBTN_WIDGETCONFIG;
                item.Display = CurrentApp.GetLanguageInfo("1206TB010", "Config");
                item.ToolTip = item.Display;
                item.Icon = "Images/00008.png";
                mListToolButtonItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadWidgetInfos()
        {
            try
            {
                mListWidgetInfos.Clear();

                //WidgetInfo info = new WidgetInfo();
                //info.WidgetID = 0;
                //info.Name = S1200Consts.WIDGET_NAME_FAVORITEMODULE;
                //info.Title = info.Name;
                //info.IsCenter = false;
                //info.SortID = 0;
                //mListWidgetInfos.Add(info);

                ////info = new WidgetInfo();
                ////info.WidgetID = 1;
                ////info.Name = S1200Consts.WIDGET_NAME_ALLMODULE;
                ////info.IsCenter = true;
                ////info.Title = info.Name;
                ////info.SortID = 1;
                ////mListWidgetInfos.Add(info);

                //info = new WidgetInfo();
                //info.WidgetID = 2;
                //info.Name = S1200Consts.WIDGET_NAME_OPTHISTORY;
                //info.IsCenter = true;
                //info.Title = info.Name;
                //info.SortID = 2;
                //mListWidgetInfos.Add(info);

                //info = new WidgetInfo();
                //info.WidgetID = 3;
                //info.Name = S1200Consts.WIDGET_NAME_FASTQUERY;
                //info.IsCenter = false;
                //info.Title = info.Name;
                //info.SortID = 3;
                //mListWidgetInfos.Add(info);

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetUserWidgetList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<WidgetInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    WidgetInfo info = optReturn.Data as WidgetInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("WidgetInfo is null"));
                        return;
                    }
                    mListWidgetInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadWidgetInfos", string.Format("Load end.\t{0}", mListWidgetInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadBasicDataInfos()
        {
            try
            {
                mListBasicDataInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1200Codes.GetBasicInfoList;
                webRequest.ListData.Add("1");
                webRequest.ListData.Add("1206");
                Service12001Client client = new Service12001Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<BasicDataInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicDataInfo info = optReturn.Data as BasicDataInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("BasicDataInfo is null"));
                        return;
                    }
                    mListBasicDataInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadBasicDataInfos", string.Format("Load end.\t{0}", mListBasicDataInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create Operation

        private void CreateWidgetItems()
        {
            try
            {
                mListLeftWidgetItems.Clear();
                mListCenterWidgetItems.Clear();
                for (int i = 0; i < mListWidgetInfos.Count; i++)
                {
                    var info = mListWidgetInfos[i];

                    var item = WidgetItem.CreateItem(info);
                    item.CurrentApp = CurrentApp;
                    item.ListBasicDataInfos = mListBasicDataInfos;
                    item.MainView = this;
                    item.IsFull = false;
                    item.IsCenter = info.IsCenter;
                    item.Title = CurrentApp.GetLanguageInfo(string.Format("1206W{0}", info.WidgetID), info.Title);
                    if (info.IsCenter)
                    {
                        mListCenterWidgetItems.Add(item);
                    }
                    else
                    {
                        mListLeftWidgetItems.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateFullScreenWidget()
        {
            try
            {
                if (mFullWidgetItem == null) { return; }
                if (mFullWidgetItem.IsCenter)
                {
                    mListCenterWidgetItems.Remove(mFullWidgetItem);
                }
                else
                {
                    mListLeftWidgetItems.Remove(mFullWidgetItem);
                }
                UCWidgetView view = new UCWidgetView();
                view.CurrentApp = CurrentApp;
                view.WidgetItem = mFullWidgetItem;
                BorderFull.Child = view;
                GridFull.Visibility = Visibility.Visible;
                GridList.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateToolBtnItems()
        {
            try
            {
                PanelToolBarBtn.Children.Clear();
                for (int i = 0; i < mListToolButtonItems.Count; i++)
                {
                    var item = mListToolButtonItems[i];
                    Button btn = new Button();
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "BtnToolBarButtonStyle");
                    btn.Click += ToolBarButton_Click;
                    PanelToolBarBtn.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #endregion


        #region Others

        private void CloseFullScreenWidget()
        {
            try
            {
                if (mFullWidgetItem == null) { return; }
                if (mFullWidgetItem.IsCenter)
                {
                    mListCenterWidgetItems.Insert(0, mFullWidgetItem);
                }
                else
                {
                    mListLeftWidgetItems.Insert(0, mFullWidgetItem);
                }
                GridFull.Visibility = Visibility.Collapsed;
                GridList.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void FullScreenWidget(WidgetItem widgetItem)
        {
            try
            {
                if (widgetItem == null) { return; }
                mFullWidgetItem = widgetItem;
                bool isFull = widgetItem.IsFull;
                widgetItem.IsFull = !isFull;
                if (isFull)
                {
                    CloseFullScreenWidget();
                }
                else
                {
                    CreateFullScreenWidget();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void RefreshWidget(WidgetItem widgetItem)
        {
            try
            {
                var view = widgetItem.View as IWidgetView;
                if (view != null)
                {
                    view.Refresh();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenWidgetSetting()
        {
            try
            {
                UCWidgetSetting uc = new UCWidgetSetting();
                uc.PageParent = this;
                uc.CurrentApp = CurrentApp;
                PopupPanel.Content = uc;
                PopupPanel.Title = "Widget Setting";
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void Reload()
        {
            try
            {
                SetBusy(true, string.Format("Loading basic informations..."));
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    InitToolButtonItems();
                    LoadBasicDataInfos();
                    LoadWidgetInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    CreateToolBtnItems();
                    CreateWidgetItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void ToolBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn == null) { return; }
                var item = btn.DataContext as ToolButtonItem;
                if (item == null) { return; }
                string strName = item.Name;
                switch (strName)
                {
                    case S1206Consts.TOOLBTN_WIDGETCONFIG:
                        OpenWidgetSetting();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ToolBtnCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var btnItem = e.Parameter as ToolButtonItem;
                if (btnItem == null) { return; }
                var widgetItem = btnItem.Data as WidgetItem;
                string strName = btnItem.Name;
                switch (strName)
                {
                    case S1206Consts.TOOLBTN_FULL:
                        if (widgetItem == null) { return; }
                        FullScreenWidget(widgetItem);
                        break;
                    case S1206Consts.TOOLBTN_REFRESH:
                        if (widgetItem == null) { return; }
                        RefreshWidget(widgetItem);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region ChangeTheme

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS1206;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //ShowException("2" + ex.Message);
                }
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                CurrentApp.AppTitle = "UMP";

                InitToolButtonItems();
                CreateToolBtnItems();

                for (int i = 0; i < mListLeftWidgetItems.Count; i++)
                {
                    var item = mListLeftWidgetItems[i];
                    var info = item.Info;
                    if (info != null)
                    {
                        item.Title = CurrentApp.GetLanguageInfo(string.Format("1206W{0}", info.WidgetID), info.Title);
                    }
                    var view = item.View;
                    if (view != null)
                    {
                        view.ChangeLanguage();
                    }
                    var widgetView = item.WidgetView;
                    if (widgetView != null)
                    {
                        widgetView.ChangeLanguage();
                    }
                }
                for (int i = 0; i < mListCenterWidgetItems.Count; i++)
                {
                    var item = mListCenterWidgetItems[i];
                    var info = item.Info;
                    if (info != null)
                    {
                        item.Title = CurrentApp.GetLanguageInfo(string.Format("1206W{0}", info.WidgetID), info.Title);
                    }
                    var view = item.View;
                    if (view != null)
                    {
                        view.ChangeLanguage();
                    }
                    var widgetView = item.WidgetView;
                    if (widgetView != null)
                    {
                        widgetView.ChangeLanguage();
                    }
                }

                if (mFullWidgetItem != null)
                {
                    var item = mFullWidgetItem;
                    var info = item.Info;
                    if (info != null)
                    {
                        item.Title = CurrentApp.GetLanguageInfo(string.Format("1206W{0}", info.WidgetID), info.Title);
                    }
                    var view = item.View;
                    if (view != null)
                    {
                        view.ChangeLanguage();
                    }
                    var widgetView = item.WidgetView;
                    if (widgetView != null)
                    {
                        widgetView.ChangeLanguage();
                    }
                }

                var popup = PopupPanel;
                if (popup != null)
                {
                    popup.ChangeLanguage();
                }
            }
            catch { }
        }

        #endregion

    }
}
