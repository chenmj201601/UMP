//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    255484c6-0bc6-4061-bfa6-a4d422305303
//        CLR Version:              4.0.30319.18408
//        Name:                     RegionManageMainView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412
//        File Name:                RegionManageMainView
//
//        created by Charley at 2016/5/10 11:16:23
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UMPS4412.Models;
using UMPS4412.Wcf11012;
using UMPS4412.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4412
{
    /// <summary>
    /// RegionManageMainView.xaml 的交互逻辑
    /// </summary>
    public partial class RegionManageMainView
    {

        #region Members

        private List<OperationInfo> mListUserOperations;
        private List<ViewColumnInfo> mListViewColumns;
        private List<long> mListUserRegionIDs;
        private List<ObjItem> mListRegionItems;
        private ObjItem mRootRegionItem;

        #endregion


        public RegionManageMainView()
        {
            InitializeComponent();

            mListUserOperations = new List<OperationInfo>();
            mListViewColumns = new List<ViewColumnInfo>();
            mListUserRegionIDs = new List<long>();
            mListRegionItems = new List<ObjItem>();
            mRootRegionItem = new ObjItem();

            GridTreeRegions.SelectedItemChanged += GridTreeRegions_SelectedItemChanged;
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "RegionManageMainView";
                StylePath = "UMPS4412/RegionManageMainView.xaml";

                GridTreeRegions.ItemsSource = mRootRegionItem.Children;

                base.Init();

                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }
                mListRegionItems.Clear();
                mRootRegionItem.Children.Clear();
                if (CurrentApp != null)
                {
                    SetBusy(true, CurrentApp.GetLanguageInfo("COMN005", string.Format("Loading basic data...")));
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadUserOperations();
                    LoadViewColumns();
                    LoadUserRegionIDs();
                    LoadRegionInfos(mRootRegionItem, 0);
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    CreateBasicOperations();
                    CreateRegionTreeColumns();

                    if (mRootRegionItem.Children.Count > 0)
                    {
                        mRootRegionItem.Children[0].IsExpanded = true;
                    }

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserOperations()
        {
            try
            {
                mListUserOperations.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("44");
                webRequest.ListData.Add("4412");
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        mListUserOperations.Add(optInfo);
                    }
                }

                CurrentApp.WriteLog("LoadOperations", string.Format("Load end.\t{0}", mListUserOperations.Count));
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
                    item.Description = info.Description;
                    item.Type = info.Type;
                    item.State = info.State;
                    item.IsDefault = info.IsDefault;
                    item.Icon = GetItemIcon(item);

                    item.StrType = CurrentApp.GetLanguageInfo(string.Format("4412013{0}", info.Type.ToString("000")),
                        info.Type.ToString());
                    item.StrState = CurrentApp.GetLanguageInfo(string.Format("4412014{0}", info.State.ToString("000")),
                        info.State.ToString());
                    item.StrIsDefault =
                        CurrentApp.GetLanguageInfo(
                            string.Format("4412015{0}", info.IsDefault ? 1.ToString("000") : 0.ToString("000")),
                            info.IsDefault ? "Yes" : "No");

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

        private void LoadViewColumns()
        {
            try
            {
                mListViewColumns.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("4412001");
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
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

                    optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo info = optReturn.Data as ViewColumnInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tViewColumnInfo is null"));
                        return;
                    }
                    mListViewColumns.Add(info);
                }

                CurrentApp.WriteLog("LoadViewColumns", string.Format("Load end.\t{0}", mListViewColumns.Count));
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

        #endregion


        #region Create

        private void CreateRegionTreeColumns()
        {
            try
            {
                var listColumns = mListViewColumns.OrderBy(c => c.SortID).ToList();
                DataTemplate nameColumnTemplate = Resources["NameColumnTemplate"] as DataTemplate;
                GridViewColumnHeader nameHeader = new GridViewColumnHeader();
                int nameWidth = 0;
                List<GridViewColumn> listGridViewColumns = new List<GridViewColumn>();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    var column = listColumns[i];

                    if (column.ColumnName == "Name")
                    {
                        nameWidth = column.Width;
                        nameHeader.Content = CurrentApp.GetLanguageInfo(string.Format("COL4412001{0}", column.ColumnName),
                            column.ColumnName);
                        nameHeader.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL4412001{0}", column.ColumnName),
                            column.ColumnName);
                    }
                    else
                    {
                        GridViewColumn gvc = new GridViewColumn();
                        GridViewColumnHeader gvch = new GridViewColumnHeader();
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL4412001{0}", column.ColumnName),
                            column.ColumnName);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL4412001{0}", column.ColumnName),
                            column.ColumnName);
                        gvc.Header = gvch;
                        gvc.Width = column.Width;
                        string strName = column.ColumnName;
                        if (strName == "Type")
                        {
                            strName = "StrType";
                        }
                        if (strName == "State")
                        {
                            strName = "StrState";
                        }
                        if (strName == "IsDefault")
                        {
                            strName = "StrIsDefault";
                        }
                        gvc.DisplayMemberBinding = new Binding(strName);
                        listGridViewColumns.Add(gvc);
                    }
                }
                if (nameColumnTemplate != null)
                {
                    GridTreeRegions.SetColumns(nameColumnTemplate, nameHeader, nameWidth, listGridViewColumns);
                }
                else
                {
                    GridTreeRegions.SetColumns(nameHeader, nameWidth, listGridViewColumns);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateBasicOperations()
        {
            try
            {
                PanelBasicOpts.Children.Clear();
                OperationInfo item;
                Button btn;

                for (int i = 0; i < mListUserOperations.Count; i++)
                {
                    item = mListUserOperations[i];
                    //基本操作按钮
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelBasicOpts.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void AddRegion()
        {
            try
            {
                var item = GridTreeRegions.SelectedItem as ObjItem;
                if (item == null) { return; }
                if (item.Type == 0) { return; }
                UCRegionModify uc = new UCRegionModify();
                uc.IsModify = false;
                uc.RegionItem = item;
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                PopupPanel.Title = string.Format("Add Region");
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyRegion()
        {
            try
            {
                var item = GridTreeRegions.SelectedItem as ObjItem;
                if (item == null) { return; }
                UCRegionModify uc = new UCRegionModify();
                uc.IsModify = true;
                uc.RegionItem = item;
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                PopupPanel.Title = string.Format("Modify Region");
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DeleteRegion()
        {
            try
            {
                var item = GridTreeRegions.SelectedItem as ObjItem;
                if (item == null) { return; }
                var info = item.Data as RegionInfo;
                if (info == null) { return; }

                var result = MessageBox.Show(string.Format("Confirm delete region \t{0} ?", info.Name),
                    CurrentApp.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }

                bool isSuccess = false;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S4410Codes.DeleteRegionInfo;
                        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                        webRequest.ListData.Add(info.ObjID.ToString());
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
                        ShowInformation(string.Format("Delete region end"));
                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    try
                    {
                        if (isSuccess)
                        {
                            var parentItem = item.Parent as ObjItem;
                            if (parentItem != null)
                            {
                                parentItem.IsExpanded = true;
                                ReloadRegionInfo(parentItem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void RegionMmtSetting()
        {
            try
            {
                var item = GridTreeRegions.SelectedItem as ObjItem;
                if (item == null) { return; }
                UCRegionManagement uc = new UCRegionManagement();
                uc.RegionItem = item;
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                PopupPanel.Title = string.Format("Region Management");
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void RegionSeatSetting()
        {
            try
            {
                var item = GridTreeRegions.SelectedItem as ObjItem;
                if (item == null) { return; }
                if (item.Type != 0) { return; } //只有工作区域才有座位排列图
                UCRegionSeatSetting uc = new UCRegionSeatSetting();
                uc.PageParent = this;
                uc.RegionItem = item;
                uc.CurrentApp = CurrentApp;
                BorderSeatSetting.Child = uc;
                if (GridRight.Width.Value <= 20)
                {
                    GridRight.Width = new GridLength(800);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void CloseSeatSetting()
        {
            try
            {
                GridRight.Width = new GridLength(0);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void SelectSeat(UMPUserControl uc, string title)
        {
            try
            {
                PopupPanel.Content = uc;
                PopupPanel.Title = title;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn != null)
                {
                    var optItem = btn.DataContext as OperationInfo;
                    if (optItem == null) { return; }
                    switch (optItem.ID)
                    {
                        case S4410Consts.OPT_ADDREGION:
                            AddRegion();
                            break;
                        case S4410Consts.OPT_MODIFYREGION:
                            ModifyRegion();
                            break;
                        case S4410Consts.OPT_DELETEREGION:
                            DeleteRegion();
                            break;
                        case S4410Consts.OPT_REGIONMANAGESETTING:
                            RegionMmtSetting();
                            break;
                        case S4410Consts.OPT_REGIONSEATSETTING:
                            RegionSeatSetting();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void GridTreeRegions_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var objItem = e.NewValue as ObjItem;
            if (objItem == null) { return; }
        }

        #endregion


        #region Others

        private void AddChild(ObjItem parent, ObjItem child)
        {
            Dispatcher.Invoke(new Action(() => parent.AddChild(child)));
        }

        public void ReloadRegionInfo(ObjItem parent)
        {
            try
            {
                LoadUserRegionIDs();
                parent.Children.Clear();
                LoadRegionInfos(parent, parent.ObjID);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
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


        #region ChangeLanguage



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
                    string uri = string.Format("/UMPS4412;component/Themes/{0}/{1}",
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
                ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("4412001", "Basic Operations");
                ExpOtherPos.Header = CurrentApp.GetLanguageInfo("4412002", "Other Position");

                for (int i = 0; i < mListUserOperations.Count; i++)
                {
                    var optInfo = mListUserOperations[i];
                    optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                    optInfo.Description = optInfo.Display;
                }
                CreateBasicOperations();
                CreateRegionTreeColumns();

                for (int i = 0; i < mListRegionItems.Count; i++)
                {
                    var item = mListRegionItems[i];
                    var info = item.Data as RegionInfo;
                    if (info == null) { continue;}

                    item.StrType = CurrentApp.GetLanguageInfo(string.Format("4412013{0}", info.Type.ToString("000")),
                      info.Type.ToString());
                    item.StrState = CurrentApp.GetLanguageInfo(string.Format("4412014{0}", info.State.ToString("000")),
                        info.State.ToString());
                    item.StrIsDefault =
                        CurrentApp.GetLanguageInfo(
                            string.Format("4412015{0}", info.IsDefault ? 1.ToString("000") : 0.ToString("000")),
                            info.IsDefault ? "Yes" : "No");
                }

                var child = BorderSeatSetting.Child as UMPUserControl;
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
