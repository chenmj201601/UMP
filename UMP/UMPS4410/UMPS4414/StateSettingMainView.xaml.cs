//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a6ecaf53-0125-45a3-91de-6d467074ff86
//        CLR Version:              4.0.30319.18408
//        Name:                     StateSettingMainView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4414
//        File Name:                StateSettingMainView
//
//        created by Charley at 2016/6/21 16:59:17
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
using UMPS4414.Models;
using UMPS4414.Wcf11012;
using UMPS4414.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;

namespace UMPS4414
{
    /// <summary>
    /// StateSettingMainView.xaml 的交互逻辑
    /// </summary>
    public partial class StateSettingMainView
    {

        #region Members

        private List<OperationInfo> mListUserOperations;
        private List<ViewColumnInfo> mListViewColumns;
        private List<AgentStateInfo> mListAgentStateInfos;
        private ObservableCollection<ObjItem> mListAgentStateItems;

        #endregion


        public StateSettingMainView()
        {
            InitializeComponent();

            mListUserOperations = new List<OperationInfo>();
            mListViewColumns = new List<ViewColumnInfo>();
            mListAgentStateInfos = new List<AgentStateInfo>();
            mListAgentStateItems = new ObservableCollection<ObjItem>();
        }


        #region Init and Load

        protected override void Init()
        {
            PageName = "StateSettingMainView";
            StylePath = "UMPS4414/StateSettingMainView.xaml";

            ListViewStateList.ItemsSource = mListAgentStateItems;

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
            };
            worker.RunWorkerCompleted += (s, re) =>
            {
                worker.Dispose();
                SetBusy(false, string.Empty);

                CreateBasicOperations();
                CreateStateColumns();
                CreateAgentStateItems();

                ChangeLanguage();
            };
            worker.RunWorkerAsync();
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
                webRequest.ListData.Add("4414");
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
                webRequest.ListData.Add("4414001");
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

        private void LoadAgentStateInfos()
        {
            try
            {
                mListAgentStateInfos.Clear();
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
                    mListAgentStateInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadAgentStateInfos", string.Format("Load end.\t{0}", mListAgentStateInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Create

        private void CreateStateColumns()
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
                    gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL4414001{0}", columnInfo.ColumnName),
                        columnInfo.ColumnName);
                    gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL4414001{0}", columnInfo.ColumnName),
                        columnInfo.ColumnName);
                    gvc.Header = gvch;
                    gvc.Width = columnInfo.Width;
                    DataTemplate dt = null;
                    string strName = columnInfo.ColumnName;
                    if (strName == "State")
                    {
                        strName = "StrState";
                    }
                    if (strName == "Type")
                    {
                        strName = "StrStateType";
                    }
                    if (strName == "IsWorkTime")
                    {
                        strName = "StrIsWorkTime";
                    }
                    if (strName == "Color")
                    {
                        strName = "StrColor";
                        dt = Resources["CellColorTemplate"] as DataTemplate;
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

                ListViewStateList.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateAgentStateItems()
        {
            try
            {
                mListAgentStateItems.Clear();
                for (int i = 0; i < mListAgentStateInfos.Count; i++)
                {
                    var info = mListAgentStateInfos[i];

                    ObjItem item = new ObjItem();
                    item.Data = info;
                    item.ObjType = S4410Consts.RESOURCE_AGENTSTATE;
                    item.ObjID = info.ObjID;
                    item.Number = info.Number;
                    item.Name = info.Name;
                    item.State = info.State;
                    item.StrState = item.State.ToString();
                    item.StrColor = info.Color;
                    item.IsWorkTime = info.IsWorkTime;
                    item.StrIsWorkTime = item.IsWorkTime.ToString();
                    item.StateType = info.Type;
                    item.StrStateType = item.StateType.ToString();
                    item.Description = info.Description;

                    item.StrState = CurrentApp.GetLanguageInfo(string.Format("4414014{0}", info.State.ToString("000")),
                        info.Type.ToString());
                    item.StrStateType = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", info.Type.ToString("000")),
                        info.Type.ToString());
                    item.StrIsWorkTime =
                        CurrentApp.GetLanguageInfo(string.Format("4414015{0}", info.IsWorkTime ? "001" : "000"),
                            info.IsWorkTime ? "Yes" : "No");

                    mListAgentStateItems.Add(item);
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

        public void ReloadData()
        {
            try
            {
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadAgentStateInfos();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    CreateAgentStateItems();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddAgentState()
        {
            try
            {
                UCAgentStateModify uc = new UCAgentStateModify();
                uc.PageParent = this;
                uc.CurrentApp = CurrentApp;
                uc.IsModify = false;
                uc.ListAllStateInfos = mListAgentStateInfos;
                PopupPanel.Title = "Add AgentState";
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyAgentState()
        {
            try
            {
                var item = ListViewStateList.SelectedItem as ObjItem;
                if (item == null) { return; }
                UCAgentStateModify uc = new UCAgentStateModify();
                uc.PageParent = this;
                uc.CurrentApp = CurrentApp;
                uc.IsModify = true;
                uc.StateItem = item;
                uc.ListAllStateInfos = mListAgentStateInfos;
                PopupPanel.Title = "Modify AgentState";
                PopupPanel.Content = uc;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DeleteAgentState()
        {
            try
            {
                var items = ListViewStateList.SelectedItems;
                List<AgentStateInfo> listStateInfos = new List<AgentStateInfo>();
                string strNames = string.Empty;
                bool bOver = false;
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i] as ObjItem;
                    if (item == null) { continue; }
                    var info = item.Data as AgentStateInfo;
                    if (info == null) { continue; }
                    if (i >= 5)
                    {
                        bOver = true;
                    }
                    else
                    {
                        strNames += string.Format("{0}\r\n", info.Name);
                    }
                    listStateInfos.Add(info);
                }
                if (bOver)
                {
                    strNames += string.Format("...");
                }
                int count = listStateInfos.Count;
                if (count <= 0) { return; }
                var result = MessageBox.Show(string.Format("{0}\r\n{1}", CurrentApp.GetLanguageInfo("4414N001", "Confirm delete AgentState?"), strNames),
                    CurrentApp.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                OperationReturn optReturn;
                bool isSuccess = false;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S4410Codes.SaveAgentStateInfo;
                        webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                        webRequest.ListData.Add("2");
                        webRequest.ListData.Add("0");
                        webRequest.ListData.Add(count.ToString());
                        for (int i = 0; i < count; i++)
                        {
                            optReturn = XMLHelper.SeriallizeObject(listStateInfos[i]);
                            if (!optReturn.Result)
                            {
                                ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            webRequest.ListData.Add(optReturn.Data.ToString());
                        }
                        Service44101Client client =
                            new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                            return;
                        }
                        if (webReturn.ListData != null)
                        {
                            for (int i = 0; i < webReturn.ListData.Count; i++)
                            {
                                CurrentApp.WriteLog("DeleteAgentState", string.Format("{0}", webReturn.ListData[i]));
                            }
                        }
                        ShowInformation(CurrentApp.GetLanguageInfo("4414N002",string.Format("Delete AgentState end")));
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
                            ReloadData();
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

        #endregion


        #region EventHandlers

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
                        case S4410Consts.OPT_ADDSTATE:
                            AddAgentState();
                            break;
                        case S4410Consts.OPT_MODIFYSTATE:
                            ModifyAgentState();
                            break;
                        case S4410Consts.OPT_DELETESTATE:
                            DeleteAgentState();
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
                    string uri = string.Format("/UMPS4414;component/Themes/{0}/{1}",
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
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("FO4414", "AgentState Management");

                ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("4414001", "Basic Operations");
                ExpOtherPos.Header = CurrentApp.GetLanguageInfo("4414002", "Others Positions");

                for (int i = 0; i < mListUserOperations.Count; i++)
                {
                    var optInfo = mListUserOperations[i];
                    optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                    optInfo.Description = optInfo.Display;
                }
                CreateBasicOperations();
                CreateStateColumns();

                for (int i = 0; i < mListAgentStateItems.Count; i++)
                {
                    var item = mListAgentStateItems[i];
                    var info = item.Data as AgentStateInfo;
                    if (info == null) { continue; }

                    item.StrState = CurrentApp.GetLanguageInfo(string.Format("4414014{0}", info.State.ToString("000")),
                      info.Type.ToString());
                    item.StrStateType = CurrentApp.GetLanguageInfo(string.Format("4414013{0}", info.Type.ToString("000")),
                        info.Type.ToString());
                    item.StrIsWorkTime =
                        CurrentApp.GetLanguageInfo(string.Format("4414015{0}", info.IsWorkTime ? "001" : "000"),
                            info.IsWorkTime ? "Yes" : "No");
                }

                PopupPanel.ChangeLanguage();
            }
            catch { }
        }

        #endregion

    }
}
