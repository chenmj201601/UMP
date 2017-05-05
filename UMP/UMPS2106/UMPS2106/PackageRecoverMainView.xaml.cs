//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    81fecaa3-989f-4124-982d-dc6617be444c
//        CLR Version:              4.0.30319.42000
//        Name:                     PackageRecoverMainView
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS2106
//        File Name:                PackageRecoverMainView
//
//        Created by Charley at 2016/10/19 14:05:08
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using UMPS2106.Models;
using UMPS2106.Wcf11012;
using UMPS2106.Wcf21061;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common21061;
using VoiceCyber.UMP.Communications;
using Binding = System.Windows.Data.Binding;

namespace UMPS2106
{
    /// <summary>
    /// PackageRecoverMainView.xaml 的交互逻辑
    /// </summary>
    public partial class PackageRecoverMainView
    {

        #region Members

        private List<OperationInfo> mListOperations;
        private List<ViewColumnInfo> mListViewColumns;
        private List<ResourceObject> mListDecObjects;
        private List<ResourceObject> mListRecoverObjects;
        private List<RecoverStrategyInfo> mListStrategyInfos;
        private List<RecoverChannelInfo> mListChannelInfos;
        private ObservableCollection<StrategyItem> mListStrategyItems;
        private ObservableCollection<ChannelItem> mListChannelItems;
        private ObservableCollection<ResourceObjectItem> mListRecoverServers;
        private StrategyItem mCurrentItem;
        private Timer mTimerRefreshFlag;

        #endregion


        public PackageRecoverMainView()
        {
            InitializeComponent();

            mListOperations = new List<OperationInfo>();
            mListViewColumns = new List<ViewColumnInfo>();
            mListDecObjects = new List<ResourceObject>();
            mListRecoverObjects = new List<ResourceObject>();
            mListStrategyInfos = new List<RecoverStrategyInfo>();
            mListChannelInfos = new List<RecoverChannelInfo>();
            mListStrategyItems = new ObservableCollection<StrategyItem>();
            mListChannelItems = new ObservableCollection<ChannelItem>();
            mListRecoverServers = new ObservableCollection<ResourceObjectItem>();
            mTimerRefreshFlag = new Timer(2 * 1000);

            ListBoxStrategyList.SelectionChanged += ListBoxStrategyList_SelectionChanged;
            BtnSelectPath.Click += BtnSelectPath_Click;
            mTimerRefreshFlag.Elapsed += TimerRefreshFlag_Elapsed;
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "PackageRecoverMainPage";
                StylePath = "UMPS2106/PackageRecoverMainPage.xaml";

                base.Init();

                ListBoxStrategyList.ItemsSource = mListStrategyItems;
                ListViewChannels.ItemsSource = mListChannelItems;
                ComboRecoverServers.ItemsSource = mListRecoverServers;

                CurrentApp.SendLoadedMessage();

                mListStrategyItems.Clear();
                SetBusy(true, CurrentApp.GetLanguageInfo("COMN001", string.Format("Loading basic data...")));
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadUserOperations();
                    LoadViewColumns();
                    LoadStrategyInfos();
                    LoadChannelInfos();
                    LoadDECObjects();
                    LoadRecoverObjects();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    CreateOptButtons();
                    CreateChannelColumns();
                    CreateStrategyItems();
                    CreateRecoverServerItems();

                    ChangeLanguage();

                    var item = mListStrategyItems.FirstOrDefault();
                    ListBoxStrategyList.SelectedItem = item;

                    mTimerRefreshFlag.Start();
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
                mListOperations.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("21");
                webRequest.ListData.Add("2106");
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
                        mListOperations.Add(optInfo);
                    }
                }

                CurrentApp.WriteLog("LoadOperations", string.Format("Load end.\t{0}", mListOperations.Count));
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
                webRequest.ListData.Add("2106001");
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

        private void LoadDECObjects()
        {
            try
            {
                mListDecObjects.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S2106Codes.GetDECList;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                Service21061Client client = new Service21061Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null."));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        ShowException(string.Format("ResourceObject is null."));
                        return;
                    }
                    mListDecObjects.Add(obj);
                }

                CurrentApp.WriteLog("LoadDECObjects", string.Format("End.\t{0}", mListDecObjects.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadRecoverObjects()
        {
            try
            {
                mListRecoverObjects.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S2106Codes.GetRecoverServerList;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                Service21061Client client = new Service21061Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null."));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        ShowException(string.Format("RecoverServer is null."));
                        return;
                    }
                    mListRecoverObjects.Add(obj);
                }

                CurrentApp.WriteLog("LoadRecoverObjects", string.Format("End.\t{0}", mListRecoverObjects.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadStrategyInfos()
        {
            try
            {
                mListStrategyInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S2106Codes.GetRecoverStrategyList;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                Service21061Client client = new Service21061Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null."));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<RecoverStrategyInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RecoverStrategyInfo info = optReturn.Data as RecoverStrategyInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("RecoverStrategyInfo is null."));
                        return;
                    }
                    mListStrategyInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadStrategyInfos", string.Format("End.\t{0}", mListStrategyInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadChannelInfos()
        {
            try
            {
                mListChannelInfos.Clear();
                OperationReturn optReturn;
                for (int i = 0; i < mListStrategyInfos.Count; i++)
                {
                    var strategy = mListStrategyInfos[i];
                    long serialNo = strategy.SerialNo;
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S2106Codes.GetRecoverChannelList;
                    webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                    webRequest.ListData.Add(serialNo.ToString());
                    Service21061Client client = new Service21061Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    if (webReturn.ListData == null)
                    {
                        ShowException(string.Format("ListData is null."));
                        return;
                    }
                    int count = 0;
                    for (int j = 0; j < webReturn.ListData.Count; j++)
                    {
                        string strInfo = webReturn.ListData[j];
                        optReturn = XMLHelper.DeserializeObject<RecoverChannelInfo>(strInfo);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        RecoverChannelInfo info = optReturn.Data as RecoverChannelInfo;
                        if (info == null)
                        {
                            ShowException(string.Format("RecoverStrategyInfo is null."));
                            return;
                        }
                        mListChannelInfos.Add(info);
                        count++;
                    }

                    CurrentApp.WriteLog("LoadChannelInfos", string.Format("End.\t{0}\t{1}", serialNo, count));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadStrategyFlag()
        {
            try
            {
                List<long> listStrategyIDs = new List<long>();
                for (int i = 0; i < mListStrategyItems.Count; i++)
                {
                    var item = mListStrategyItems[i];
                    var info = item.Info;
                    if (info == null) { continue; }
                    long id = info.SerialNo;
                    if (id <= 0) { continue; }
                    listStrategyIDs.Add(id);
                }
                int count = listStrategyIDs.Count;
                if (count <= 0) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S2106Codes.GetStrategyFlagList;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    webRequest.ListData.Add(listStrategyIDs[i].ToString());
                }
                Service21061Client client = new Service21061Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null."));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    //CurrentApp.WriteLog("LoadStrategyFlag", string.Format("Flag:{0}", strInfo));
                    Dispatcher.Invoke(new Action(() => RefreshStrategyFlag(strInfo)));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create

        private void CreateOptButtons()
        {
            try
            {
                OperationInfo optInfo;
                Button btn;

                //StrategyList
                PanelStrategyListOpts.Children.Clear();
                optInfo = mListOperations.FirstOrDefault(o => o.ID == S2106Consts.OPT_ADD_STRATEGY);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += OptButton_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelStrategyListOpts.Children.Add(btn);
                }
                optInfo = mListOperations.FirstOrDefault(o => o.ID == S2106Consts.OPT_DELETE_STRATEGY);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += OptButton_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelStrategyListOpts.Children.Add(btn);
                }
                optInfo = mListOperations.FirstOrDefault(o => o.ID == S2106Consts.OPT_REFRESH_STRATEGY);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += OptButton_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelStrategyListOpts.Children.Add(btn);
                }

                //StrategyDetail
                PanelStrategyDetailOpts.Children.Clear();
                optInfo = mListOperations.FirstOrDefault(o => o.ID == S2106Consts.OPT_SAVE_STRATEGY);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += OptButton_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelStrategyDetailOpts.Children.Add(btn);
                }
                optInfo = mListOperations.FirstOrDefault(o => o.ID == S2106Consts.OPT_EXECUTE_STRATEGY);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += OptButton_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelStrategyDetailOpts.Children.Add(btn);
                }

                //ChannelList
                PanelChannelOpts.Children.Clear();
                optInfo = mListOperations.FirstOrDefault(o => o.ID == S2106Consts.OPT_ADD_CHANNEL);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += OptButton_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelChannelOpts.Children.Add(btn);
                }
                optInfo = mListOperations.FirstOrDefault(o => o.ID == S2106Consts.OPT_DELETE_CHANNEL);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += OptButton_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelChannelOpts.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateStrategyItems()
        {
            try
            {
                mListStrategyItems.Clear();
                for (int i = 0; i < mListStrategyInfos.Count; i++)
                {
                    var strategy = mListStrategyInfos[i];
                    long strategyID = strategy.SerialNo;

                    StrategyItem strategyItem = new StrategyItem();
                    strategyItem.Info = strategy;
                    strategyItem.SerialNo = strategy.SerialNo;
                    strategyItem.Name = strategy.Name;
                    strategyItem.Description = strategy.Description;

                    var channels = mListChannelInfos.Where(c => c.StrategyID == strategyID).ToList();
                    for (int j = 0; j < channels.Count; j++)
                    {
                        var channel = channels[j];
                        strategyItem.ListChannels.Add(channel);
                    }
                    SetStrategyFlag(strategyItem);

                    mListStrategyItems.Add(strategyItem);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateChannelItems()
        {
            try
            {
                mListChannelItems.Clear();
                if (mCurrentItem == null) { return; }
                for (int i = 0; i < mCurrentItem.ListChannels.Count; i++)
                {
                    var info = mCurrentItem.ListChannels[i];
                    info.Number = i;

                    ChannelItem item = new ChannelItem();
                    item.Info = info;
                    item.Number = info.Number;
                    item.Extension = info.Extension;
                    item.Channel = info.ChannelID;
                    item.Voice = string.Format("[{0}]{1}", info.VoiceID, info.VoiceIP);
                    mListChannelItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateRecoverServerItems()
        {
            try
            {
                mListRecoverServers.Clear();
                for (int i = 0; i < mListRecoverObjects.Count; i++)
                {
                    var info = mListRecoverObjects[i];
                    ResourceObjectItem item = new ResourceObjectItem();
                    item.Info = info;
                    item.ObjID = info.ObjID;
                    item.ObjType = info.ObjType;
                    item.Display = string.Format("[{0}]{1}", info.Name, info.FullName);
                    mListRecoverServers.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateChannelColumns()
        {
            try
            {
                GridView gv = new GridView();

                var listColumns = mListViewColumns.OrderBy(c => c.SortID).ToList();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = listColumns[i];

                    GridViewColumn gvc = new GridViewColumn();
                    GridViewColumnHeader gvch = new GridViewColumnHeader();
                    gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL2106001{0}", columnInfo.ColumnName),
                        columnInfo.ColumnName);
                    gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL2106001{0}", columnInfo.ColumnName),
                        columnInfo.ColumnName);
                    gvc.Header = gvch;
                    gvc.Width = columnInfo.Width;
                    string strName = columnInfo.ColumnName;
                    gvc.DisplayMemberBinding = new Binding(strName);

                    gv.Columns.Add(gvc);
                }

                ListViewChannels.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void ShowStrategyDetail()
        {
            try
            {
                if (mCurrentItem == null) { return; }
                var info = mCurrentItem.Info;
                if (info == null) { return; }
                TxtStrategyDetailTitle.Text = info.Name;

                TxtStrategyName.Text = info.Name;
                RadioStrategyEnable.IsChecked = info.State == 0;
                RadioStrategyDisable.IsChecked = info.State == 1;
                TxtBeginTime.Value = info.BeginTime.ToLocalTime();
                TxtEndTime.Value = info.EndTime.ToLocalTime();
                TxtPackagePath.Text = info.PackagePath;
                TxtStrategyDesc.Text = info.Description;

                var server = ComboRecoverServers.SelectedItem as ResourceObjectItem;
                if (server == null)
                {
                    ComboRecoverServers.SelectedItem = mListRecoverServers.FirstOrDefault();
                }

                CreateChannelItems();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ResetStrategyDetail()
        {
            try
            {
                TxtStrategyDetailTitle.Text = string.Empty;
                TxtStrategyName.Text = string.Empty;
                RadioStrategyEnable.IsChecked = true;
                RadioStrategyDisable.IsChecked = false;
                TxtBeginTime.Value = DateTime.Now.AddDays(-1);
                TxtEndTime.Value = DateTime.Now;
                TxtPackagePath.Text = string.Empty;
                TxtStrategyDesc.Text = string.Empty;

                ComboRecoverServers.SelectedItem = null;

                mListChannelItems.Clear();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddNewStrategyItem()
        {
            try
            {
                DateTime now = DateTime.Now.ToUniversalTime();

                RecoverStrategyInfo info = new RecoverStrategyInfo();
                info.SerialNo = 0;
                info.Name = CurrentApp.GetLanguageInfo("2106017", "New recover strategy");
                info.Description = info.Name;
                info.State = 0;
                info.BeginTime = now.AddDays(-1);
                info.EndTime = now;
                info.Flag = 0;
                info.Times = 0;
                info.LastOptTime = DateTime.Parse("2014/1/1");
                StrategyItem item = new StrategyItem();
                item.Info = info;
                item.SerialNo = info.SerialNo;
                item.Name = info.Name;
                item.Description = info.Description;
                mListStrategyItems.Add(item);
                ListBoxStrategyList.SelectedItem = item;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DeleteStrategyItem()
        {
            try
            {
                var item = ListBoxStrategyList.SelectedItem as StrategyItem;
                if (item == null) { return; }
                long serialNo = item.SerialNo;
                string strName = item.Name;
                var result =
                    MessageBox.Show(
                        string.Format("{0}\r\n\r\n{1}", CurrentApp.GetLanguageInfo("2106N004", "Confirm delete?"),
                            strName),
                        CurrentApp.AppTitle,
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }

                mListStrategyItems.Remove(item);

                item = mListStrategyItems.FirstOrDefault();
                ListBoxStrategyList.SelectedItem = item;
                mCurrentItem = null;
                ResetStrategyDetail();

                var optReturn = DeleteStrategyDB(serialNo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("{0}\r\n\r\n{1}\t{2}",
                        CurrentApp.GetLanguageInfo("2106N002", "Save fail!"), optReturn.Code, optReturn.Message));
                    return;
                }

                ShowInformation(CurrentApp.GetLanguageInfo("2106N005", string.Format("Delete successful.")));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void RefreshStrategyItem()
        {
            try
            {
                mCurrentItem = null;
                ResetStrategyDetail();
                mListStrategyItems.Clear();
                SetBusy(true, string.Empty);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadStrategyInfos();
                    LoadChannelInfos();
                    LoadDECObjects();
                    LoadRecoverObjects();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    CreateStrategyItems();
                    CreateRecoverServerItems();

                    var item = mListStrategyItems.FirstOrDefault();
                    ListBoxStrategyList.SelectedItem = item;
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private OperationReturn DeleteStrategyDB(long serialNo)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S2106Codes.SaveRecoverStrategy;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("2");
                webRequest.ListData.Add(serialNo.ToString());
                Service21061Client client = new Service21061Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                if (webReturn.ListData != null)
                {
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        CurrentApp.WriteLog("DeleteStrategyDB", string.Format("End.{0}", webReturn.ListData[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private void SaveStrategyInfo()
        {
            try
            {
                if (mCurrentItem == null) { return; }
                var info = mCurrentItem.Info;
                if (info == null) { return; }
                if (string.IsNullOrEmpty(TxtStrategyName.Text))
                {
                    ShowException(CurrentApp.GetLanguageInfo("2106N006", string.Format("Strategy name is empty!")));
                    return;
                }
                info.Name = TxtStrategyName.Text;
                info.State = RadioStrategyEnable.IsChecked == true ? 0 : 1;
                if (TxtBeginTime.Value == null
                    || TxtEndTime.Value == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("2106N007", string.Format("Datetime invalid!")));
                    return;
                }
                DateTime dtBegin = Convert.ToDateTime(TxtBeginTime.Value);
                DateTime dtEnd = Convert.ToDateTime(TxtEndTime.Value);
                DateTime now = DateTime.Now;
                if (dtEnd < dtBegin
                    || dtEnd > now)
                {
                    ShowException(CurrentApp.GetLanguageInfo("2106N007", string.Format("Datetime invalid!")));
                    return;
                }
                info.BeginTime = dtBegin.ToUniversalTime();
                info.EndTime = dtEnd.ToUniversalTime();
                info.PackagePath = TxtPackagePath.Text;
                info.Description = TxtStrategyDesc.Text;

                mCurrentItem.Name = info.Name;
                mCurrentItem.Description = info.Description;

                TxtStrategyDetailTitle.Text = mCurrentItem.Name;

                var optReturn = SaveStrategyDB();
                if (!optReturn.Result)
                {
                    ShowException(string.Format("{0}\r\n\r\n{1}\t{2}",
                        CurrentApp.GetLanguageInfo("2106N002", "Save fail!"), optReturn.Code, optReturn.Message));
                    return;
                }

                ShowInformation(CurrentApp.GetLanguageInfo("2106N001", string.Format("Save end.")));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private OperationReturn SaveStrategyDB()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                for (int i = 0; i < mListStrategyItems.Count; i++)
                {
                    var strategyItem = mListStrategyItems[i];
                    var strategy = strategyItem.Info;
                    if (strategy == null) { continue; }


                    #region StrategyInfo

                    long serialNo = strategy.SerialNo;
                    optReturn = XMLHelper.SeriallizeObject(strategy);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    string strInfo = optReturn.Data.ToString();
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S2106Codes.SaveRecoverStrategy;
                    webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                    webRequest.ListData.Add(serialNo > 0 ? "1" : "0");
                    webRequest.ListData.Add(strInfo);
                    Service21061Client client = new Service21061Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    if (!webReturn.Result)
                    {
                        optReturn.Result = false;
                        optReturn.Code = webReturn.Code;
                        optReturn.Message = webReturn.Message;
                        return optReturn;
                    }
                    if (webReturn.ListData == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("ListData is null.");
                        return optReturn;
                    }
                    string strSerialNo = webReturn.ListData[0];
                    CurrentApp.WriteLog("SaveStrategyDB", string.Format("End.{0}", strSerialNo));
                    if (long.TryParse(strSerialNo, out serialNo))
                    {
                        strategy.SerialNo = serialNo;
                        strategyItem.SerialNo = serialNo;
                    }

                    #endregion


                    #region ChannelInfo

                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S2106Codes.SaveRecoverChannels;
                    webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                    webRequest.ListData.Add(strSerialNo);
                    int count = strategyItem.ListChannels.Count;
                    webRequest.ListData.Add(count.ToString());
                    for (int j = 0; j < strategyItem.ListChannels.Count; j++)
                    {
                        var channel = strategyItem.ListChannels[j];

                        optReturn = XMLHelper.SeriallizeObject(channel);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }
                    webReturn = client.DoOperation(webRequest);
                    if (!webReturn.Result)
                    {
                        optReturn.Result = false;
                        optReturn.Code = webReturn.Code;
                        optReturn.Message = webReturn.Message;
                        return optReturn;
                    }
                    if (webReturn.ListData != null)
                    {
                        for (int j = 0; j < webReturn.ListData.Count; j++)
                        {
                            CurrentApp.WriteLog("SaveStrategyDB", string.Format("End.{0}", webReturn.ListData[j]));
                        }
                    }

                    #endregion

                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private void ExecuteStrategy()
        {
            try
            {

                #region SetInfo

                if (mCurrentItem == null) { return; }
                var info = mCurrentItem.Info;
                if (info == null) { return; }
                if (string.IsNullOrEmpty(TxtStrategyName.Text))
                {
                    ShowException(string.Format("Strategy name is empty!"));
                    return;
                }
                info.Name = TxtStrategyName.Text;
                info.State = RadioStrategyEnable.IsChecked == true ? 0 : 1;
                if (TxtBeginTime.Value == null
                    || TxtEndTime.Value == null)
                {
                    ShowException(string.Format("Datetime invalid!"));
                    return;
                }
                DateTime dtBegin = Convert.ToDateTime(TxtBeginTime.Value);
                DateTime dtEnd = Convert.ToDateTime(TxtEndTime.Value);
                DateTime now = DateTime.Now;
                if (dtEnd < dtBegin
                    || dtEnd > now)
                {
                    ShowException(string.Format("Datetime invalid!"));
                    return;
                }
                info.BeginTime = dtBegin.ToUniversalTime();
                info.EndTime = dtEnd.ToUniversalTime();
                info.PackagePath = TxtPackagePath.Text;
                info.Description = TxtStrategyDesc.Text;

                mCurrentItem.Name = info.Name;
                mCurrentItem.Description = info.Description;

                TxtStrategyDetailTitle.Text = mCurrentItem.Name;

                #endregion


                //执行任务之前先把策略置为初始未执行状态
                if (mCurrentItem == null) { return; }
                var strategy = mCurrentItem.Info;
                if (strategy == null) { return; }
                strategy.Flag = 0;
                strategy.Progress = 0;

                //先保存到数据库
                OperationReturn optReturn = SaveStrategyDB();
                if (!optReturn.Result)
                {
                    ShowException(string.Format("{0}\r\n\r\n{1}\t{2}",
                        CurrentApp.GetLanguageInfo("2106N002", "Save fail!"), optReturn.Code, optReturn.Message));
                    return;
                }

                optReturn = XMLHelper.SeriallizeObject(strategy);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                string strStrategy = optReturn.Data.ToString();
                var dec = mListDecObjects.FirstOrDefault();
                if (dec == null)
                {
                    ShowException(CurrentApp.GetLanguageInfo("2106N003", string.Format("DEC Server not exist.")));
                    return;
                }
                string strAddress = dec.FullName;
                string strPort = dec.Other01;
                int intPort;
                if (!int.TryParse(strPort, out intPort))
                {
                    ShowException(string.Format("DEC Port invalid."));
                    return;
                }
                int count = mCurrentItem.ListChannels.Count;
                if (count <= 0) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S2106Codes.ExecuteStrategy;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(strAddress);
                webRequest.ListData.Add(intPort.ToString());
                webRequest.ListData.Add(strStrategy);
                webRequest.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    var channel = mCurrentItem.ListChannels[i];
                    optReturn = XMLHelper.SeriallizeObject(channel);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }

                bool isFail = false;
                string strMsg = string.Empty;
                SetBusy(true, string.Empty);

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        Service21061Client client =
                            new Service21061Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            isFail = true;
                            strMsg = string.Format("{0}\t{1}", webReturn.Code, webReturn.Message);
                            return;
                        }
                        if (webReturn.ListData != null)
                        {
                            for (int i = 0; i < webReturn.ListData.Count; i++)
                            {
                                string str = webReturn.ListData[i];

                                CurrentApp.WriteLog("ExecuteStrategy", string.Format("ExecuteReturn:{0}", str));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        isFail = true;
                        strMsg = ex.Message;
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    if (isFail)
                    {
                        ShowException(strMsg);
                        return;
                    }

                    ShowInformation(string.Format("End."));
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddStrategyChannel()
        {
            try
            {
                if (mCurrentItem == null) { return; }
                UCAddChannel uc = new UCAddChannel();
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                uc.StrategyItem = mCurrentItem;
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void AddedStrategyChannel()
        {
            try
            {
                CreateChannelItems();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DeleteStrategyChannel()
        {
            try
            {
                if (mCurrentItem == null) { return; }
                var objs = ListViewChannels.SelectedItems;
                for (int i = objs.Count - 1; i >= 0; i--)
                {
                    var item = objs[i] as ChannelItem;
                    if (item == null) { continue; }
                    var channel = item.Info;
                    if (channel == null) { continue; }
                    mCurrentItem.ListChannels.Remove(channel);
                    mListChannelItems.Remove(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SelectPath()
        {
            try
            {
                var server = ComboRecoverServers.SelectedItem as ResourceObjectItem;
                if (server == null) { return; }
                UCPathLister uc = new UCPathLister();
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                uc.ServerItem = server;
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void DoSelectPath(string path)
        {
            try
            {
                TxtPackagePath.Text = path;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetStrategyFlag(StrategyItem item)
        {
            try
            {
                item.StrFlag = string.Empty;
                var info = item.Info;
                if (info == null) { return; }
                item.StrFlag = CurrentApp.GetLanguageInfo(string.Format("2106016{0}", info.Flag.ToString("000")),
                    info.Flag.ToString());
            }
            catch { }
        }

        private void RefreshStrategyFlag(string strInfo)
        {
            try
            {
                string[] arrInfos = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                if (arrInfos.Length > 2)
                {
                    string strID = arrInfos[0];
                    string strFlag = arrInfos[1];
                    long id;
                    int flag;
                    if (long.TryParse(strID, out id)
                        && int.TryParse(strFlag, out flag))
                    {
                        var temp = mListStrategyItems.FirstOrDefault(s => s.SerialNo == id);
                        if (temp != null)
                        {
                            var info = temp.Info;
                            if (info != null)
                            {
                                info.Flag = flag;
                                SetStrategyFlag(temp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Event Handlers

        private void OptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn == null) { return; }
                var optInfo = btn.DataContext as OperationInfo;
                if (optInfo == null) { return; }
                long optID = optInfo.ID;
                switch (optID)
                {
                    case S2106Consts.OPT_ADD_STRATEGY:
                        AddNewStrategyItem();
                        break;
                    case S2106Consts.OPT_DELETE_STRATEGY:
                        DeleteStrategyItem();
                        break;
                    case S2106Consts.OPT_REFRESH_STRATEGY:
                        RefreshStrategyItem();
                        break;
                    case S2106Consts.OPT_SAVE_STRATEGY:
                        SaveStrategyInfo();
                        break;
                    case S2106Consts.OPT_EXECUTE_STRATEGY:
                        ExecuteStrategy();
                        break;
                    case S2106Consts.OPT_ADD_CHANNEL:
                        AddStrategyChannel();
                        break;
                    case S2106Consts.OPT_DELETE_CHANNEL:
                        DeleteStrategyChannel();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ListBoxStrategyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = ListBoxStrategyList.SelectedItem as StrategyItem;
                if (item == null) { return; }
                mCurrentItem = item;
                ShowStrategyDetail();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnSelectPath_Click(object sender, RoutedEventArgs e)
        {
            SelectPath();
        }

        void TimerRefreshFlag_Elapsed(object sender, ElapsedEventArgs e)
        {
            LoadStrategyFlag();
        }

        #endregion


        #region ChanhgeTheme

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
                    string uri = string.Format("/Themes/{0}/{1}",
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
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID), "Package Recover Center");

                TxtStrategyListTitle.Text = CurrentApp.GetLanguageInfo("2106001", "Recover Strategy List");
                GroupBasicInfo.Header = CurrentApp.GetLanguageInfo("2106002", "Basic Information");
                GroupRecoverChannel.Header = CurrentApp.GetLanguageInfo("2106003", "Recover Channel List");
                LbStrategyName.Text = CurrentApp.GetLanguageInfo("2106004", "Name");
                LbStrategyState.Text = CurrentApp.GetLanguageInfo("2106005", "State");
                LbStrategyDatetime.Text = CurrentApp.GetLanguageInfo("2106006", "Time");
                LbRecoverServer.Text = CurrentApp.GetLanguageInfo("2106007", "Recover Server");
                LbPackagePath.Text = CurrentApp.GetLanguageInfo("2106008", "Package Path");
                LbStrategyDesc.Text = CurrentApp.GetLanguageInfo("2106009", "Description");

                RadioStrategyEnable.Content = CurrentApp.GetLanguageInfo("2106010", "Enable");
                RadioStrategyDisable.Content = CurrentApp.GetLanguageInfo("2106011", "Disable");

                CreateOptButtons();
                CreateChannelColumns();

                PopupPanel.ChangeLanguage();
            }
            catch { }
        }

        #endregion

    }
}
