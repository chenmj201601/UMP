//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4d01316d-27e7-49e4-8c02-cd9244dbec3e
//        CLR Version:              4.0.30319.18408
//        Name:                     AlarmSettingMainView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4415
//        File Name:                AlarmSettingMainView
//
//        created by Charley at 2016/7/11 10:15:15
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using UMPS4415.Models;
using UMPS4415.Wcf11012;
using UMPS4415.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;

namespace UMPS4415
{
    /// <summary>
    /// AlarmSettingMainView.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmSettingMainView
    {

        #region Members

        private List<OperationInfo> mListUserOperations;
        private List<ViewColumnInfo> mListViewColumns;
        private List<AlarmMessageInfo> mListAlarmMessageInfos;
        private List<AgentStateInfo> mListAgentStateInfos;
        private ObservableCollection<AlarmMessageItem> mListAlarmMessageItems;

        #endregion


        public AlarmSettingMainView()
        {
            InitializeComponent();

            mListUserOperations = new List<OperationInfo>();
            mListViewColumns = new List<ViewColumnInfo>();
            mListAlarmMessageInfos = new List<AlarmMessageInfo>();
            mListAgentStateInfos = new List<AgentStateInfo>();
            mListAlarmMessageItems = new ObservableCollection<AlarmMessageItem>();
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "AlarmSettingMainView";
                StylePath = "UMPS4415/AlarmSettingMainView.xaml";

                ListViewAlarmList.ItemsSource = mListAlarmMessageItems;

                base.Init();

                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }
                if (CurrentApp != null)
                {
                    SetBusy(true, CurrentApp.GetLanguageInfo("COMN005", string.Format("Loading basic data...")));
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadUserOperations();
                    LoadViewColumns();
                    LoadAgentStateInfos();
                    LoadAlarmMessageInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    SetBusy(false, string.Empty);

                    CreateBasicOperations();
                    CreateAlarmColumns();
                    CreateAlarmMessageItems();

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
                webRequest.ListData.Add("4415");
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

        private void LoadViewColumns()
        {
            try
            {
                mListViewColumns.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("4415001");
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

        private void LoadAlarmMessageInfos()
        {
            try
            {
                mListAlarmMessageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetAlarmMessageList;
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

                    optReturn = XMLHelper.DeserializeObject<AlarmMessageInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    AlarmMessageInfo info = optReturn.Data as AlarmMessageInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tAlarmMessage is null"));
                        return;
                    }
                    mListAlarmMessageInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadAlarmMessage", string.Format("Load end.\t{0}", mListAlarmMessageInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAgentStateInfos()
        {
            try
            {
                mListAgentStateInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetAgentStateList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("1");
                Service44101Client client = new Service44101Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
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
                        ShowException(string.Format("AgentStateInfo is null"));
                        return;
                    }
                    mListAgentStateInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadAgentStateInfos", string.Format("End.\t{0}", mListAgentStateInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create

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

        private void CreateAlarmColumns()
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
                    gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL4415001{0}", columnInfo.ColumnName),
                        columnInfo.ColumnName);
                    gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL4415001{0}", columnInfo.ColumnName),
                        columnInfo.ColumnName);
                    gvc.Header = gvch;
                    gvc.Width = columnInfo.Width;
                    DataTemplate dt = null;
                    string strName = columnInfo.ColumnName;
                    if (strName == "Color")
                    {
                        dt = Resources["CellColorTemplate"] as DataTemplate;
                    }
                    if (strName == "State")
                    {
                        strName = "StrState";
                    }
                    if (dt != null)
                    {
                        gvc.CellTemplate = dt;
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(strName);
                    }

                    gv.Columns.Add(gvc);
                }

                ListViewAlarmList.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateAlarmMessageItems()
        {
            try
            {
                mListAlarmMessageItems.Clear();
                for (int i = 0; i < mListAlarmMessageInfos.Count; i++)
                {
                    var info = mListAlarmMessageInfos[i];

                    AlarmMessageItem item = new AlarmMessageItem();
                    item.Info = info;
                    item.CurrentApp = CurrentApp;
                    item.SerialID = info.SerialID;
                    item.Name = info.Name;
                    item.Type = info.Type.ToString();
                    item.Rank = info.Rank.ToString();
                    item.Value = info.Value;
                    item.Color = info.Color;
                    item.Icon = info.Icon;
                    item.HoldTime = info.HoldTime.ToString();

                    item.Type = CurrentApp.GetLanguageInfo(string.Format("4415015{0}", info.Type.ToString("000")), info.Type.ToString());
                    item.StrState = info.State == 0
                        ? CurrentApp.GetLanguageInfo("4415021000", "Enable")
                        : CurrentApp.GetLanguageInfo("4415021002", "Disable");
                    item.BrushColor = GetBrushFromColorString(info.Color);

                    string strState = string.Empty;
                    if (info.StateID > 0)
                    {
                        var agentState = mListAgentStateInfos.FirstOrDefault(s => s.ObjID == info.StateID);
                        if (agentState != null)
                        {
                            strState = agentState.Name;
                        }
                    }
                    item.RelativeState = strState;

                    mListAlarmMessageItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void AddAlarmMessage()
        {
            try
            {
                UCAlarmInfoModify uc = new UCAlarmInfoModify();
                uc.IsModify = false;
                uc.CurrentApp = CurrentApp;
                uc.PageParent = this;
                uc.ListAgentStateInfos = mListAgentStateInfos;
                PopupPanel.Title = string.Format("Add Alarm Message");
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyAlarmMessage()
        {
            try
            {
                var item = ListViewAlarmList.SelectedItem as AlarmMessageItem;
                if (item == null) { return; }
                UCAlarmInfoModify uc = new UCAlarmInfoModify();
                uc.IsModify = true;
                uc.CurrentApp = CurrentApp;
                uc.AlarmItem = item;
                uc.PageParent = this;
                uc.ListAgentStateInfos = mListAgentStateInfos;
                PopupPanel.Title = string.Format("Modify Alarm Message");
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DeleteAlarmMessage()
        {
            try
            {
                var item = ListViewAlarmList.SelectedItem as AlarmMessageItem;
                if (item == null) { return; }
                var info = item.Info;
                if (info == null) { return; }
                var result = MessageBox.Show(string.Format("{0}\r\n{1}", CurrentApp.GetLanguageInfo("4415N001", "Confirm delete?"), info.Name), CurrentApp.AppTitle,
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }

                bool isFail = true;
                string strMsg = string.Empty;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SaveAlarmMessage;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("2");
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("1");
                OperationReturn optReturn = XMLHelper.SeriallizeObject(info);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        Service44101Client client =
                            new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        if (!webReturn.Result)
                        {
                            strMsg = string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                            return;
                        }
                        if (webReturn.ListData == null)
                        {
                            strMsg = string.Format("Fail.ListData is null");
                            return;
                        }
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            string str = webReturn.ListData[i];

                            CurrentApp.WriteLog("DeleteAlarmMessage", string.Format("{0}", str));
                        }
                        isFail = false;
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    if (!isFail)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("4415N002",string.Format("Delete successful")));

                        ReloadData();
                    }
                    else
                    {
                        ShowException(strMsg);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetAlarmUser()
        {
            try
            {
                var item = ListViewAlarmList.SelectedItem as AlarmMessageItem;
                if (item == null) { return; }
                UCAlarmUserSetting uc = new UCAlarmUserSetting();
                uc.CurrentApp = CurrentApp;
                uc.AlarmItem = item;
                uc.PageParent = this;
                PopupPanel.Title = string.Format("Set User");
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void ReloadData()
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadAlarmMessageInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateAlarmMessageItems();
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
                        case S4410Consts.OPT_ADDALARM:
                            AddAlarmMessage();
                            break;
                        case S4410Consts.OPT_MODIFYALARM:
                            ModifyAlarmMessage();
                            break;
                        case S4410Consts.OPT_DELETEALARM:
                            DeleteAlarmMessage();
                            break;
                        case S4410Consts.OPT_SETUSER:
                            SetAlarmUser();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private Brush GetBrushFromColorString(string strColor)
        {
            Brush brush = Brushes.Transparent;
            try
            {
                brush = new SolidColorBrush(Utils.GetColorFromRgbString(strColor));
            }
            catch { }
            return brush;
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
                    string uri = string.Format("/UMPS4415;component/Themes/{0}/{1}",
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

            //加载静态资源
            try
            {
                string uri = string.Format("/UMPS4415;component/Themes/{0}/{1}",
                    "Default"
                    , "MainViewStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("2" + ex.Message);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("FO4415", "Alarm Management");

                ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("4415001", "Basic Operations");
                ExpOtherPos.Header = CurrentApp.GetLanguageInfo("4415002", "Others Positions");

                for (int i = 0; i < mListUserOperations.Count; i++)
                {
                    var optInfo = mListUserOperations[i];
                    optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                    optInfo.Description = optInfo.Display;
                }
                CreateBasicOperations();
                CreateAlarmColumns();

                for (int i = 0; i < mListAlarmMessageItems.Count; i++)
                {
                    var item = mListAlarmMessageItems[i];
                    var info = item.Info;
                    if (info == null) { continue; }

                    item.Type = CurrentApp.GetLanguageInfo(string.Format("4415015{0}", info.Type.ToString("000")), info.Type.ToString());
                    item.StrState = info.State == 0
                        ? CurrentApp.GetLanguageInfo("4415021000", "Enable")
                        : CurrentApp.GetLanguageInfo("4415021002", "Disable");
                }

                PopupPanel.ChangeLanguage();
            }
            catch { }
        }

        #endregion

    }
}
