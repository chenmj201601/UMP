//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f41c4c00-af40-495a-98b3-8a254edb6b53
//        CLR Version:              4.0.30319.18408
//        Name:                     OnsiteMonitorMainView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411
//        File Name:                OnsiteMonitorMainView
//
//        created by Charley at 2016/6/17 09:30:08
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UMPS4411.Models;
using UMPS4411.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4411
{
    /// <summary>
    /// OnsiteMonitorMainView.xaml 的交互逻辑
    /// </summary>
    public partial class OnsiteMonitorMainView
    {

        #region Members

        private List<ObjItem> mListRegionItems;
        private List<SeatInfo> mListAllSeatInfos;
        private List<AgentStateInfo> mListAllStateInfos;
        private List<long> mListUserRegionIDs;
        private ObjItem mRegionRoot;
        private ObjItem mCurrentRegionItem;
        private List<ImageButtonItem> mListToolBtnItems;
        private double mViewerScale;
        private int mViewMode;          //1：座位视图；2：状态视图；3：统计视图

        #endregion


        public OnsiteMonitorMainView()
        {
            InitializeComponent();

            mListRegionItems = new List<ObjItem>();
            mListUserRegionIDs = new List<long>();
            mRegionRoot = new ObjItem();
            mListAllSeatInfos = new List<SeatInfo>();
            mListToolBtnItems = new List<ImageButtonItem>();
            mListAllStateInfos = new List<AgentStateInfo>();

            TreeRegionList.SelectedItemChanged += TreeRegionList_SelectedItemChanged;
            SliderMapScale.ValueChanged += SliderMapScale_ValueChanged;
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "OnsiteMonitorMainView";
                StylePath = "UMPS4411/OnsiteMonitorMainView.xaml";

                TreeRegionList.ItemsSource = mRegionRoot.Children;

                base.Init();

                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }
                mViewerScale = 1.0;
                mViewMode = 1;
                if (CurrentApp != null)
                {
                    SetBusy(true, CurrentApp.GetLanguageInfo("COMN005", string.Format("Loading basic data...")));
                }
                mListRegionItems.Clear();
                mRegionRoot.Children.Clear();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadAllStateInfos();
                    LoadAllSeatInfos();
                    LoadUserRegionIDs();
                    LoadRegionInfos(mRegionRoot, 0);
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    if (mRegionRoot.Children.Count > 0)
                    {
                        mRegionRoot.Children[0].IsExpanded = true;
                    }
                    InitToolBtnItems();
                    CreateToolBtnItems();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadRegionInfos(ObjItem parent, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetRegionInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID.ToString());
                Service44101Client client = new Service44101Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
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
                int count = webReturn.ListData.Count;
                int valid = 0;
                OperationReturn optReturn;
                for (int i = 0; i < count; i++)
                {
                    string strInfo = webReturn.ListData[i];

                    optReturn = XMLHelper.DeserializeObject<RegionInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RegionInfo info = optReturn.Data as RegionInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tRegionInfo is null"));
                        return;
                    }
                    ObjItem item = new ObjItem();
                    item.Data = info;
                    item.ObjType = S4410Consts.RESOURCE_REGION;
                    item.ObjID = info.ObjID;
                    item.Name = info.Name;
                    string strDescription = info.Description;
                    if (string.IsNullOrEmpty(strDescription))
                    {
                        strDescription = item.Name;
                    }
                    item.Description = strDescription;
                    item.Type = info.Type;
                    item.Icon = GetItemIcon(item);
                    item.CurrentApp = CurrentApp;

                    if (mListUserRegionIDs.Contains(info.ObjID))
                    {
                        mListRegionItems.Add(item);
                        AddChild(parent, item);
                        valid++;

                        LoadRegionInfos(item, info.ObjID);
                    }
                    else
                    {
                        LoadRegionInfos(parent, info.ObjID);
                    }
                }

                CurrentApp.WriteLog("LoadRegionInfos", string.Format("Load end.\t{0};Valid:{1}", count, valid));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserRegionIDs()
        {
            try
            {
                mListUserRegionIDs.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetUserRegionList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                Service44101Client client = new Service44101Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
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
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strID = webReturn.ListData[i];
                    long id;
                    if (long.TryParse(strID, out id))
                    {
                        mListUserRegionIDs.Add(id);
                    }
                }

                CurrentApp.WriteLog("LoadUserRegionIDs", string.Format("Load end.\t{0}", mListUserRegionIDs.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAllStateInfos()
        {
            try
            {
                mListAllStateInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetAgentStateList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                Service44101Client client = new Service44101Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
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
                int count = webReturn.ListData.Count;
                OperationReturn optReturn;
                for (int i = 0; i < count; i++)
                {
                    string strInfo = webReturn.ListData[i];

                    optReturn = XMLHelper.DeserializeObject<AgentStateInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    AgentStateInfo info = optReturn.Data as AgentStateInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tAgentStateInfo is null"));
                        return;
                    }
                    mListAllStateInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadAllStateInfos", string.Format("Load end.\t{0}", mListAllStateInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAllSeatInfos()
        {
            try
            {
                mListAllSeatInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetSeatInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                Service44101Client client = new Service44101Client(
                  WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                  WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
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
                    optReturn = XMLHelper.DeserializeObject<SeatInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    SeatInfo info = optReturn.Data as SeatInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tSeatInfo is null"));
                        return;
                    }
                    mListAllSeatInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadAllSeatInfos", string.Format("Load end.\t{0}", mListAllSeatInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitToolBtnItems()
        {
            try
            {
                mListToolBtnItems.Clear();
                ImageButtonItem item;
                if (mViewMode == 2)
                {
                    item = new ImageButtonItem();
                    item.Name = "BtnSeatList";
                    item.Display = CurrentApp.GetLanguageInfo("4411013", "Seat List");
                    item.ToolTip = item.Display;
                    item.Icon = "Images/00009.png";
                    mListToolBtnItems.Add(item);
                }
                if (mViewMode == 1)
                {
                    item = new ImageButtonItem();
                    item.Name = "BtnStateList";
                    item.Display = CurrentApp.GetLanguageInfo("4411014", "State List");
                    item.ToolTip = item.Display;
                    item.Icon = "Images/00010.png";
                    mListToolBtnItems.Add(item);
                }
                item = new ImageButtonItem();
                item.Name = "BtnRefresh";
                item.Display = CurrentApp.GetLanguageInfo("4411016", "Refresh");
                item.ToolTip = item.Display;
                item.Icon = "Images/00011.png";
                mListToolBtnItems.Add(item);
                item = new ImageButtonItem();
                item.Name = "BtnSetting";
                item.Display = CurrentApp.GetLanguageInfo("4411017", "Setting");
                item.ToolTip = item.Display;
                item.Icon = "Images/00001.png";
                mListToolBtnItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create

        private void CreateToolBtnItems()
        {
            try
            {
                PanelToolButtons.Children.Clear();
                for (int i = 0; i < mListToolBtnItems.Count; i++)
                {
                    var item = mListToolBtnItems[i];
                    Button btn = new Button();
                    btn.Click += ToolBtn_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "ToolBtnStyle");
                    PanelToolButtons.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void OpenRegionMap()
        {
            try
            {
                mViewMode = 1;
                LbRegionName.Text = string.Empty;
                BorderContent.Child = null;
                SliderMapScale.Visibility = Visibility.Collapsed;

                InitToolBtnItems();
                CreateToolBtnItems();

                if (mCurrentRegionItem == null) { return; }
                var regionInfo = mCurrentRegionItem.Data as RegionInfo;
                if (regionInfo == null) { return; }
                if (regionInfo.Type != 0) { return; }
                LbRegionName.Text = regionInfo.Name;

                UCWorkRegionMap uc = new UCWorkRegionMap();
                uc.PageParent = this;
                uc.RegionItem = mCurrentRegionItem;
                uc.CurrentApp = CurrentApp;
                uc.ListAllSeatInfos = mListAllSeatInfos;
                uc.ListAllStateInfos = mListAllStateInfos;
                BorderContent.Child = uc;

                SliderMapScale.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenStateList()
        {
            try
            {
                mViewMode = 2;
                LbRegionName.Text = string.Empty;
                BorderContent.Child = null;
                SliderMapScale.Visibility = Visibility.Collapsed;

                InitToolBtnItems();
                CreateToolBtnItems();

                if (mCurrentRegionItem == null) { return; }
                var regionInfo = mCurrentRegionItem.Data as RegionInfo;
                if (regionInfo == null) { return; }
                if (regionInfo.Type != 0) { return; }
                LbRegionName.Text = regionInfo.Name;

                UCAgentStateLister uc = new UCAgentStateLister();
                uc.PageParent = this;
                uc.RegionItem = mCurrentRegionItem;
                uc.CurrentApp = CurrentApp;
                uc.ListAllSeatInfos = mListAllSeatInfos;
                uc.ListAllStateInfos = mListAllStateInfos;
                BorderContent.Child = uc;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenRegionStateView()
        {
            try
            {
                mViewMode = 3;
                LbRegionName.Text = string.Empty;
                BorderContent.Child = null;
                SliderMapScale.Visibility = Visibility.Collapsed;

                InitToolBtnItems();
                CreateToolBtnItems();

                if (mCurrentRegionItem == null) { return; }
                var regionInfo = mCurrentRegionItem.Data as RegionInfo;
                if (regionInfo == null) { return; }
                if (regionInfo.Type == 0) { return; }
                LbRegionName.Text = regionInfo.Name;

                UCRegionStateViewer uc = new UCRegionStateViewer();
                uc.PageParent = this;
                uc.RegionItem = mCurrentRegionItem;
                uc.CurrentApp = CurrentApp;
                uc.ListAllStateInfos = mListAllStateInfos;
                uc.ListAllSeatInfos = mListAllSeatInfos;
                BorderContent.Child = uc;

                SliderMapScale.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void RefreshState()
        {
            try
            {
                var view = BorderContent.Child as IOnsiteView;
                if (view != null)
                {
                    view.RefreshState();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenSettingPanel()
        {
            try
            {

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public override void Close()
        {
            var map = BorderContent.Child as UCWorkRegionMap;
            if (map != null)
            {
                map.Close();
            }
            base.Close();
        }

        #endregion


        #region Others

        private void AddChild(ObjItem parent, ObjItem child)
        {
            Dispatcher.Invoke(new Action(() => parent.AddChild(child)));
        }

        private string GetItemIcon(ObjItem item)
        {
            string strReturn = string.Empty;
            try
            {
                var regionInfo = item.Data as RegionInfo;
                if (regionInfo != null)
                {
                    int type = regionInfo.Type;
                    switch (type)
                    {
                        case 1:
                            strReturn = string.Format("Images/00004.png");
                            break;
                        case 2:
                            strReturn = string.Format("Images/00003.png");
                            break;
                        default:
                            strReturn = string.Format("Images/00005.png");
                            break;
                    }
                }

            }
            catch { }
            return strReturn;
        }

        #endregion


        #region EventHandlers

        void TreeRegionList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var item = e.NewValue as ObjItem;
                if (item == null) { return; }
                mCurrentRegionItem = item;
                if (mCurrentRegionItem.Type == 0)
                {
                    OpenRegionMap();
                }
                else
                {
                    OpenRegionStateView();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void SliderMapScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int intValue;
                if (int.TryParse(SliderMapScale.Value.ToString(), out intValue))
                {
                    switch (intValue)
                    {
                        case 5:
                            mViewerScale = 0.1;
                            break;
                        case 10:
                            mViewerScale = 0.2;
                            break;
                        case 15:
                            mViewerScale = 0.3;
                            break;
                        case 20:
                            mViewerScale = 0.4;
                            break;
                        case 25:
                            mViewerScale = 0.5;
                            break;
                        case 30:
                            mViewerScale = 0.6;
                            break;
                        case 35:
                            mViewerScale = 0.7;
                            break;
                        case 40:
                            mViewerScale = 0.8;
                            break;
                        case 45:
                            mViewerScale = 0.9;
                            break;
                        case 50:
                            mViewerScale = 1.0;
                            break;
                        case 55:
                            mViewerScale = 1.5;
                            break;
                        case 60:
                            mViewerScale = 2.0;
                            break;
                        case 65:
                            mViewerScale = 2.5;
                            break;
                        case 70:
                            mViewerScale = 3.0;
                            break;
                        case 75:
                            mViewerScale = 3.5;
                            break;
                        case 80:
                            mViewerScale = 4.0;
                            break;
                        case 85:
                            mViewerScale = 4.5;
                            break;
                        case 90:
                            mViewerScale = 5.0;
                            break;
                        case 95:
                            mViewerScale = 5.5;
                            break;
                    }
                }
                var map = BorderContent.Child as UCWorkRegionMap;
                if (map != null)
                {
                    map.SetMapScale(mViewerScale);
                }
                SliderMapScale.Tag = mViewerScale;
                BindingExpression be = SliderMapScale.GetBindingExpression(ToolTipProperty);
                if (be != null)
                {
                    be.UpdateTarget();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ToolBtn_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn == null) { return; }
            var imageBtn = btn.DataContext as ImageButtonItem;
            if (imageBtn == null) { return; }
            string strName = imageBtn.Name;
            switch (strName)
            {
                case "BtnSeatList":
                    OpenRegionMap();
                    break;
                case "BtnStateList":
                    OpenStateList();
                    break;
                case "BtnRefresh":
                    RefreshState();
                    break;
                case "BtnSetting":
                    OpenSettingPanel();
                    break;
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
                    string uri = string.Format("/UMPS4411;component/Themes/{0}/{1}",
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
                LbRegionList.Text = CurrentApp.GetLanguageInfo("4411001", "Region List");

                InitToolBtnItems();
                CreateToolBtnItems();

                var child = BorderContent.Child as UMPUserControl;
                if (child != null)
                {
                    child.ChangeLanguage();
                }

                PopupPanel.ChangeLanguage();
            }
            catch { }
        }

        #endregion

    }
}
