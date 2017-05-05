//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1688ae32-1e32-44d9-af5b-1032fc6fcf52
//        CLR Version:              4.0.30319.42000
//        Name:                     ChanMonitorMainView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102
//        File Name:                ChanMonitorMainView
//
//        created by Charley at 2016/1/29 16:21:43
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using UMPS2102.Models;
using UMPS2102.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common21021;
using VoiceCyber.UMP.CommonService04;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;
using Timer = System.Timers.Timer;

namespace UMPS2102
{
    /// <summary>
    /// ChanMonitorMainView.xaml 的交互逻辑
    /// </summary>
    public partial class ChanMonitorMainView
    {

        #region Members

        private const int MAX_MONOBJ_SIZE = 20;

        /// <summary>
        /// 当前用户操作权限
        /// </summary>
        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();

        private BackgroundWorker mWorker;
        private List<ObjectItem> mListControlObjects;
        private ObjectItem mRootObject;
        private NetClient mMonitorClient;

        private List<GlobalParamInfo> mListGlobalParamInfos;
        private List<BasicDataInfo> mListBasicDataInfos;        //基础信息列表
        private ObservableCollection<MonitorItem> mListMonitorItems;  //监视对象列表
        private List<MonitorData> mListMonitorDatas;                 //监视数据列表，主要用于保存监视对象信息
        private List<MonitorObject> mListMonitorObjects;         //一个临时存放监视对象的地方，方便与服务器的交互
        private MonitorObjectConfig mMonObjConfig;
        private ObservableCollection<ViewColumnInfo> mListMonitorListColumns;
        private ObservableCollection<ViewTypeItem> mListViewTypeItems;
        private List<UserParamInfo> mListUserParams;

        private int mViewType;
        private Timer mRecordLengthTimer;
        private double mTimeDeviation;  //与Service04服务器的时间偏差

        #endregion


        public ChanMonitorMainView()
        {
            InitializeComponent();

            mListGlobalParamInfos = new List<GlobalParamInfo>();
            mListBasicDataInfos = new List<BasicDataInfo>();
            mListControlObjects = new List<ObjectItem>();
            mRootObject = new ObjectItem();
            mListMonitorItems = new ObservableCollection<MonitorItem>();
            mListMonitorDatas = new List<MonitorData>();
            mListMonitorObjects = new List<MonitorObject>();
            mListMonitorListColumns = new ObservableCollection<ViewColumnInfo>();
            mListViewTypeItems = new ObservableCollection<ViewTypeItem>();
            mListUserParams = new List<UserParamInfo>();

            Unloaded += ChanMonitorMainView_Unloaded;
            ComboViewTypes.SelectionChanged += ComboViewTypes_SelectionChanged;

            mViewType = 1;
            mRecordLengthTimer = new Timer(1000);
            mTimeDeviation = 0;
        }

        void ChanMonitorMainView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (mMonitorClient != null)
            {
                mMonitorClient.Stop();
                mMonitorClient = null;
            }
        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "ChanMonitorMainPage";
                StylePath = "UMPS2102/ChanMonitorMainPage.xaml";

                TreeObjects.ItemsSource = mRootObject.Children;
                LvMonitorList.ItemsSource = mListMonitorItems;
                ComboViewTypes.ItemsSource = mListViewTypeItems;
                mRecordLengthTimer.Elapsed += mRecordLengthTimer_Elapsed;
                LvMonitorList.SelectionChanged += LvMonitorList_SelectionChanged;

                base.Init();

                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", "Loading basic information..."));

                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    //触发Loaded消息
                    if (CurrentApp != null)
                    {
                        CurrentApp.SendLoadedMessage();
                    }

                    LoadGlobalParamInfos();
                    LoadOperations();
                    LoadBasicDataInfos();
                    LoadUserParams();
                    LoadMonitorListColumns();
                    LoadControlObjects();
                    LoadMonitorConfig();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);

                    CreateOperationButtons();
                    CreateViewTypeItems();
                    SetStateMonitorListView();

                    mRootObject.IsChecked = false;
                    if (mRootObject.Children.Count > 0)
                    {
                        var root = mRootObject.Children[0] as ObjectItem;
                        if (root != null)
                        {
                            root.IsExpanded = true;
                        }
                    }

                    mRecordLengthTimer.Start();
                    InitMonitorItems();
                    InitMonitorClient();
                    InitNetMonitor();

                    ChangeTheme();
                    ChangeLanguage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadGlobalParamInfos()
        {
            try
            {
                mListGlobalParamInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList2;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(string.Format("{0}", ConstValue.GP_GROUP_OBJ_CONTROL));
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("WSFail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    GlobalParamInfo info = optReturn.Data as GlobalParamInfo;
                    if (info != null)
                    {
                        mListGlobalParamInfos.Add(info);
                    }
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Init GlobalParamInfo end"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadOperations()
        {
            try
            {
                ListOperations.Clear();
                LoadOperations(CurrentApp.ModuleID);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadOperations(long parentOptID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("21");
                webRequest.ListData.Add(parentOptID.ToString());
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
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
                        ListOperations.Add(optInfo);
                        //加载下级操作
                        LoadOperations(optInfo.ID);
                    }
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Init Operations"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserParams()
        {
            try
            {
                mListUserParams.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserParamList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(string.Format("{0}{1}{2}", 210201, ConstValue.SPLITER_CHAR, 210202));
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("WSFail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<UserParamInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    UserParamInfo info = optReturn.Data as UserParamInfo;
                    if (info != null)
                    {
                        mListUserParams.Add(info);
                    }
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Init UserParamList end"));
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
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetBasicDataInfoList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add("3");       //获取指定InfoID范围的所有BasicDataInfo
                webRequest.ListData.Add("210200000");
                webRequest.ListData.Add("0");
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<BasicDataInfo> listinfos = new List<BasicDataInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicDataInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicDataInfo info = optReturn.Data as BasicDataInfo;
                    if (info != null)
                    {
                        listinfos.Add(info);
                    }
                }
                listinfos = listinfos.OrderBy(c => c.SortID).ToList();
                mListBasicDataInfos.Clear();
                for (int i = 0; i < listinfos.Count; i++)
                {
                    mListBasicDataInfos.Add(listinfos[i]);
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Init BasicDataInfos"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadMonitorListColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("2102001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListMonitorListColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListMonitorListColumns.Add(listColumns[i]);
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Init MonitorListColumns"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadMonitorConfig()
        {
            try
            {
                string path = CurrentApp.TempPath;
                path = Path.Combine(path, MonitorObjectConfig.FILE_NAME);
                if (!File.Exists(path))
                {
                    CurrentApp.WriteLog("LoadMonitorConfig", string.Format("Fail.\tConfig file not exist.\t{0}", path));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<MonitorObjectConfig>(path);
                if (!optReturn.Result)
                {
                    CurrentApp.WriteLog("LoadMonitorConfig", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                var monObjConfig = optReturn.Data as MonitorObjectConfig;
                if (monObjConfig == null)
                {
                    CurrentApp.WriteLog("LoadMonitorConfig", string.Format("Fail.\tMonitorObjectConfig is null."));
                    return;
                }
                mMonObjConfig = monObjConfig;
                var userConfig = mMonObjConfig.ListUserMonObjConfig.FirstOrDefault(c => c.UserID == CurrentApp.Session.UserID);
                if (userConfig != null
                    && userConfig.IsRemember)
                {
                    mViewType = userConfig.ViewType;
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Init MonitorConfig"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadControlObjects()
        {
            try
            {
                LoadControlOrgs(mRootObject, -1);

                CurrentApp.WriteLog("PageLoad", string.Format("Init ControlObjects"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadControlOrgs(ObjectItem parentItem, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_ORG.ToString());
                webRequest.ListData.Add(parentID.ToString());
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("WSFail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        ShowException(string.Format("Fail.\tResourceObject is null"));
                        return;
                    }

                    long orgID = obj.ObjID;
                    string strName = obj.Name;
                    string strDesc = obj.FullName;
                    if (string.IsNullOrEmpty(strDesc))
                    {
                        strDesc = strName;
                    }
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = orgID;
                    item.Name = strName;
                    item.Description = strDesc;
                    item.Icon = string.Format("Images/{0}.png", "00001");
                    item.Data = obj;

                    AddChild(parentItem, item);
                    mListControlObjects.Add(item);

                    LoadControlOrgs(item, orgID);
                    LoadControlExtension(item, orgID);
                    LoadControlRealExt(item, orgID);
                    LoadControlAgents(item, orgID);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadControlAgents(ObjectItem parentItem, long parentID)
        {
            try
            {
                //判断是否坐席管理模式
                var gpInfo = mListGlobalParamInfos.FirstOrDefault(p => p.ParamID == ConstValue.GP_OBJ_CONTROLMODE);
                if (gpInfo == null) { return; }
                string strValue = gpInfo.ParamValue;
                if (strValue.Length < ConstValue.GP_OBJ_CONTROLMODE.ToString().Length) { return; }
                strValue = strValue.Substring(ConstValue.GP_OBJ_CONTROLMODE.ToString().Length);
                string[] listValues = strValue.Split(new[] { ConstValue.SPLITER_CHAR_3 },
                    StringSplitOptions.RemoveEmptyEntries);
                if (!listValues.Contains(ConstValue.GS_OBJ_CONTROLMODE_AGT)) { return; }

                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_AGENT.ToString());
                webRequest.ListData.Add(parentID.ToString());
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("WSFail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        ShowException(string.Format("Fail.\tResourceObj is null"));
                        return;
                    }
                    long objID = obj.ObjID;
                    string strName = obj.Name;
                    string strDesc = obj.FullName;
                    if (string.IsNullOrEmpty(strDesc))
                    {
                        strDesc = strName;
                    }
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = objID;
                    item.Name = strName;
                    item.Description = strDesc;
                    item.Icon = string.Format("Images/{0}.png", "00002");
                    item.Data = obj;
                    AddChild(parentItem, item);
                    mListControlObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadControlExtension(ObjectItem parentItem, long parentID)
        {
            try
            {
                //判断是否分机管理模式
                var gpInfo = mListGlobalParamInfos.FirstOrDefault(p => p.ParamID == ConstValue.GP_OBJ_CONTROLMODE);
                if (gpInfo == null) { return; }
                string strValue = gpInfo.ParamValue;
                if (strValue.Length < ConstValue.GP_OBJ_CONTROLMODE.ToString().Length) { return; }
                strValue = strValue.Substring(ConstValue.GP_OBJ_CONTROLMODE.ToString().Length);
                string[] listValues = strValue.Split(new[] { ConstValue.SPLITER_CHAR_3 },
                    StringSplitOptions.RemoveEmptyEntries);
                if (!listValues.Contains(ConstValue.GS_OBJ_CONTROLMODE_EXT)) { return; }

                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_EXTENSION.ToString());
                webRequest.ListData.Add(parentID.ToString());
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("WSFail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        ShowException(string.Format("Fail.\tResourceObject is null"));
                        return;
                    }
                    long objID = obj.ObjID;
                    string strExt = obj.Name;
                    string strServerIP = obj.Other04;
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_EXTENSION;
                    item.ObjID = objID;
                    item.Name = strExt;
                    item.Description = string.Format("{0}[{1}]", strExt, strServerIP);
                    item.Icon = string.Format("Images/{0}.png", "00003");
                    item.Data = obj;

                    AddChild(parentItem, item);
                    mListControlObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadControlRealExt(ObjectItem parentItem, long parentID)
        {
            try
            {
                //判断是否真实分机管理模式
                var gpInfo = mListGlobalParamInfos.FirstOrDefault(p => p.ParamID == ConstValue.GP_OBJ_CONTROLMODE);
                if (gpInfo == null) { return; }
                string strValue = gpInfo.ParamValue;
                if (strValue.Length < ConstValue.GP_OBJ_CONTROLMODE.ToString().Length) { return; }
                strValue = strValue.Substring(ConstValue.GP_OBJ_CONTROLMODE.ToString().Length);
                string[] listValues = strValue.Split(new[] { ConstValue.SPLITER_CHAR_3 },
                    StringSplitOptions.RemoveEmptyEntries);
                if (!listValues.Contains(ConstValue.GS_OBJ_CONTROLMODE_REALEXT)) { return; }

                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_REALEXT.ToString());
                webRequest.ListData.Add(parentID.ToString());
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("WSFail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        ShowException(string.Format("Fail.\tResourceObject is null"));
                        return;
                    }
                    long objID = obj.ObjID;
                    string strName = obj.Name;
                    string strDesc = obj.FullName;
                    if (string.IsNullOrEmpty(strDesc))
                    {
                        strDesc = strName;
                    }
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_REALEXT;
                    item.ObjID = objID;
                    item.Name = strName;
                    item.Description = strDesc;
                    item.Icon = string.Format("Images/{0}.png", "00015");
                    item.Data = obj;
                    AddChild(parentItem, item);
                    mListControlObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitMonitorItems()
        {
            try
            {
                mListMonitorObjects.Clear();
                mListMonitorItems.Clear();
                mListMonitorDatas.Clear();
                if (mMonObjConfig == null) { return; }
                var userConfig = mMonObjConfig.ListUserMonObjConfig.FirstOrDefault(c => c.UserID == CurrentApp.Session.UserID);
                if (userConfig != null
                    && userConfig.IsRemember)
                {
                    for (int i = 0; i < userConfig.ListStateMonObjList.Count; i++)
                    {
                        var data = userConfig.ListStateMonObjList[i];
                        //筛选可管理的资源
                        var temp = mListControlObjects.FirstOrDefault(o => o.ObjID == data.ObjID);
                        if (temp != null)
                        {
                            mListMonitorDatas.Add(data);
                        }
                    }
                }
                for (int i = 0; i < mListMonitorDatas.Count; i++)
                {
                    var data = mListMonitorDatas[i];
                    MonitorItem item = MonitorItem.CreateItem(data);
                    item.CurrentApp = CurrentApp;
                    item.ListUserParams = mListUserParams;
                    MonitorObject monObj = new MonitorObject();
                    monObj.MonType = MonitorType.State;
                    monObj.ObjID = data.ObjID;
                    monObj.ObjType = data.ObjType;
                    monObj.ObjValue = data.Name;
                    monObj.Role = 1;
                    string strOther03 = data.Other03;
                    if (!string.IsNullOrEmpty(strOther03))
                    {
                        string[] arrOther03 = strOther03.Split(new[] { ';' }, StringSplitOptions.None);
                        if (arrOther03.Length > 0)
                        {
                            monObj.Other03 = arrOther03[0];
                        }
                    }
                    item.VoiceChanMonObject = monObj;
                    mListMonitorObjects.Add(monObj);
                    monObj = new MonitorObject();
                    monObj.MonType = MonitorType.State;
                    monObj.ObjID = data.ObjID;
                    monObj.ObjType = data.ObjType;
                    monObj.ObjValue = data.Name;
                    monObj.Role = 2;
                    if (!string.IsNullOrEmpty(strOther03))
                    {
                        string[] arrOther03 = strOther03.Split(new[] { ';' }, StringSplitOptions.None);
                        if (arrOther03.Length > 1)
                        {
                            monObj.Other03 = arrOther03[1];
                        }
                        else if (arrOther03.Length > 0)
                        {
                            monObj.Other03 = arrOther03[0];
                        }
                    }
                    item.ScreenChanMonObject = monObj;
                    mListMonitorObjects.Add(monObj);
                    item.UpdateState();
                    mListMonitorItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitMonitorClient()
        {
            try
            {
                if (mMonitorClient != null)
                {
                    mMonitorClient.Stop();
                    mMonitorClient = null;
                }

                string strAddress = CurrentApp.Session.AppServerInfo.Address;
                int intPort = CurrentApp.Session.AppServerInfo.SupportHttps
                    ? CurrentApp.Session.AppServerInfo.Port - 5
                    : CurrentApp.Session.AppServerInfo.Port - 4;

                //string strAddress = "192.168.5.31";
                //int intPort = 8081 - 4;

                mMonitorClient = new NetClient();
                mMonitorClient.Debug += (mode, cat, msg) => CurrentApp.WriteLog("MonitorClient", string.Format("{0}\t{1}", cat, msg));
                mMonitorClient.ReturnMessageReceived += StateMonitorClient_ReturnMessageReceived;
                mMonitorClient.NotifyMessageReceived += StateMonitorClient_NotifyMessageReceived;
                mMonitorClient.IsSSL = true;
                mMonitorClient.Connect(strAddress, intPort);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitNetMonitor()
        {
            try
            {
                if (mMonObjConfig == null) { return; }
                var userConfig = mMonObjConfig.ListUserMonObjConfig.FirstOrDefault(u => u.UserID == CurrentApp.Session.UserID);
                if (userConfig == null) { return; }
                var monData = userConfig.ListNetMonObjList.FirstOrDefault();
                if (monData == null) { return; }
                var temp = mListControlObjects.FirstOrDefault(o => o.ObjID == monData.ObjID);
                if (temp == null) { return; }
                NMonMonitorObject(monData);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations


        #region 状态监视消息处理

        private void SetMonType()
        {
            try
            {
                if (mMonitorClient == null) { return; }
                OperationReturn optReturn;
                RequestMessage request = new RequestMessage();
                request.Command = (int)Service04Command.ReqSetMonType;
                request.SessionID = mMonitorClient.SessionID;
                request.ListData.Add(((int)MonitorType.State).ToString());
                lock (this)
                {
                    optReturn = mMonitorClient.SendMessage(request);
                }
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                CurrentApp.WriteLog("MonitorClient", string.Format("Send SetMonType message end"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddQueryChanMonObject()
        {
            try
            {
                if (mMonitorClient == null) { return; }
                OperationReturn optReturn;
                int total = mListMonitorObjects.Count;
                if (total > 0)
                {
                    //如果对象较多，分批次发送请求
                    int blockIndex = 0;
                    int blockSize = MAX_MONOBJ_SIZE;
                    int count;
                    do
                    {
                        count = Math.Min(total, blockSize);
                        total = total - count;
                        RequestMessage request = new RequestMessage();
                        request.Command = (int)Service04Command.ReqAddQueryChan;
                        request.ListData.Add(count.ToString());
                        for (int i = 0; i < count; i++)
                        {
                            var obj = mListMonitorObjects[i + (blockIndex * blockSize)];
                            optReturn = XMLHelper.SeriallizeObject(obj);
                            if (!optReturn.Result)
                            {
                                ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            request.ListData.Add(optReturn.Data.ToString());
                        }
                        //添加
                        CurrentApp.WriteLog("AddQueryChan", string.Format("Count:{0}", count));
                        lock (this)
                        {
                            optReturn = mMonitorClient.SendMessage(request);
                        }
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        //等待2s
                        Thread.Sleep(2000);
                        blockIndex++;
                    } while (total > 0);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void QueryChanInfo(MonitorObject obj)
        {
            try
            {
                OperationReturn optReturn;
                if (obj != null)
                {
                    string strMonID = obj.MonID;
                    RequestMessage request = new RequestMessage();
                    request.Command = (int)Service04Command.ReqQueryChan;
                    request.ListData.Add("1");
                    request.ListData.Add(strMonID);
                    CurrentApp.WriteLog("QueryChan", string.Format("MonObj:{0}", obj.ObjValue));
                    lock (this)
                    {
                        optReturn = mMonitorClient.SendMessage(request);
                    }
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("QueryChan",
                           string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void RemoveMonObject()
        {
            try
            {
                if (mMonitorClient == null) { return; }
                OperationReturn optReturn;
                int count = mListMonitorObjects.Count;
                if (count > 0)
                {
                    RequestMessage request = new RequestMessage();
                    request.Command = (int)Service04Command.ReqRemoveMonObj;
                    request.ListData.Add(count.ToString());
                    CurrentApp.WriteLog("RemoveMonObj", string.Format("Count:{0}", count));
                    for (int i = 0; i < count; i++)
                    {
                        request.ListData.Add(mListMonitorObjects[i].MonID);
                    }
                    lock (this)
                    {
                        optReturn = mMonitorClient.SendMessage(request);
                    }
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void QueryChanState(MonitorObject obj)
        {
            try
            {
                OperationReturn optReturn;
                if (obj != null)
                {
                    string strMonID = obj.MonID;
                    RequestMessage request = new RequestMessage();
                    request.Command = (int)Service04Command.ReqQueryState;
                    request.ListData.Add(strMonID);
                    CurrentApp.WriteLog("QueryState", string.Format("MonObj:{0}", obj.ObjValue));
                    lock (this)
                    {
                        optReturn = mMonitorClient.SendMessage(request);
                    }
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("QueryChanState",
                           string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("QueryChanState", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealWelcomeMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData == null || retMessage.ListData.Count < 4)
                {
                    CurrentApp.WriteLog("DealQueryChanResponse", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strServerTime = retMessage.ListData[3];
                DateTime dtServerTime;
                if (DateTime.TryParse(strServerTime, out dtServerTime))
                {
                    DateTime now = DateTime.Now.ToUniversalTime();
                    double timeDeviation = (now - dtServerTime).TotalSeconds;
                    mTimeDeviation = timeDeviation;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DealAddQueryChanResponse(ReturnMessage retMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (retMessage.ListData == null || retMessage.ListData.Count < 1)
                {
                    CurrentApp.WriteLog("DealAddQueryChan", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strCount = retMessage.ListData[0];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    CurrentApp.WriteLog("DealAddQueryChan", string.Format("ListData count param invalid"));
                    return;
                }
                if (retMessage.ListData.Count < intCount + 1)
                {
                    CurrentApp.WriteLog("DealAddQueryChan", string.Format("ListData count invalid"));
                    return;
                }
                for (int i = 0; i < intCount; i++)
                {
                    var strInfo = retMessage.ListData[i + 1];
                    optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("DealAddQueryChan",
                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    MonitorObject obj = optReturn.Data as MonitorObject;
                    if (obj == null)
                    {
                        CurrentApp.WriteLog("DealAddQueryChan", string.Format("MonitorObject is null"));
                        return;
                    }
                    var temp = mListMonitorObjects.FirstOrDefault(m => m.ObjID == obj.ObjID && m.Role == obj.Role);
                    if (temp != null)
                    {
                        temp.MonID = obj.MonID;
                        temp.UpdateInfo(obj);

                        var temp2 = mListMonitorItems.FirstOrDefault(m => m.ObjID == temp.ObjID);
                        if (temp2 != null)
                        {
                            Dispatcher.Invoke(new Action(temp2.UpdateState));
                        }

                        //查询通道状态
                        QueryChanState(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DealQueryChanResponse(ReturnMessage retMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (retMessage.ListData == null || retMessage.ListData.Count < 1)
                {
                    CurrentApp.WriteLog("DealQueryChan", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strCount = retMessage.ListData[0];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    CurrentApp.WriteLog("DealQueryChan", string.Format("ListData count param invalid"));
                    return;
                }
                if (retMessage.ListData.Count < intCount + 1)
                {
                    CurrentApp.WriteLog("DealQueryChan", string.Format("ListData count invalid"));
                    return;
                }
                for (int i = 0; i < intCount; i++)
                {
                    var strInfo = retMessage.ListData[i + 1];
                    optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("DealQueryChan",
                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    MonitorObject obj = optReturn.Data as MonitorObject;
                    if (obj == null)
                    {
                        CurrentApp.WriteLog("DealQueryChan", string.Format("MonitorObject is null"));
                        return;
                    }
                    var temp = mListMonitorObjects.FirstOrDefault(m => m.MonID == obj.MonID);
                    if (temp != null)
                    {
                        temp.UpdateInfo(obj);

                        var temp2 = mListMonitorItems.FirstOrDefault(m => m.ObjID == temp.ObjID);
                        if (temp2 != null)
                        {
                            Dispatcher.Invoke(new Action(temp2.UpdateState));
                        }

                        //查询通道状态
                        QueryChanState(temp);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DealQueryStateResponse(ReturnMessage retMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (retMessage.ListData == null || retMessage.ListData.Count < 2)
                {
                    CurrentApp.WriteLog("DealQueryState", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = retMessage.ListData[0];
                string strState = retMessage.ListData[1];
                MonitorObject obj = mListMonitorObjects.FirstOrDefault(o => o.MonID == strMonID);
                if (obj == null)
                {
                    CurrentApp.WriteLog("DealQueryState", string.Format("Monitor object not exist"));
                    return;
                }
                optReturn = XMLHelper.DeserializeObject<ChanState>(strState);
                if (!optReturn.Result)
                {
                    CurrentApp.WriteLog("DealQueryState", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ChanState chanState = optReturn.Data as ChanState;
                if (chanState == null)
                {
                    CurrentApp.WriteLog("DealQueryState", string.Format("ChannelState object is null"));
                    return;
                }
                var temp = mListMonitorItems.FirstOrDefault(m => m.ObjID == obj.ObjID);
                if (temp != null)
                {
                    if (obj.Role == 1)
                    {
                        temp.VoiceChanState = chanState;
                    }
                    if (obj.Role == 2)
                    {
                        temp.ScreenChanState = chanState;
                    }
                    temp.TimeDeviation = mTimeDeviation;
                    Dispatcher.Invoke(new Action(temp.UpdateState));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DealChanStateChanged(NotifyMessage notMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (notMessage.ListData == null || notMessage.ListData.Count < 3)
                {
                    CurrentApp.WriteLog("DealChanStateChanged", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = notMessage.ListData[0];
                string strState = notMessage.ListData[1];
                string strNewChanObjID = notMessage.ListData[2];
                long newChanObjID;
                MonitorObject obj = null;
                MonitorItem item = null;
                for (int i = 0; i < mListMonitorItems.Count; i++)
                {
                    item = mListMonitorItems[i];
                    var temp = item.VoiceChanMonObject;
                    if (temp != null && temp.MonID == strMonID)
                    {
                        obj = temp;
                        break;
                    }
                    temp = item.ScreenChanMonObject;
                    if (temp != null && temp.MonID == strMonID)
                    {
                        obj = temp;
                        break;
                    }
                }
                if (obj == null
                    || item == null)
                {
                    CurrentApp.WriteLog("DealChanStateChanged", string.Format("MonitorObject not exist."));
                    return;
                }
                if (!string.IsNullOrEmpty(strNewChanObjID)
                    && long.TryParse(strNewChanObjID, out newChanObjID)
                    && newChanObjID > 0)
                {
                    //关联的通道变了，需要重新查询通道信息
                    QueryChanInfo(obj);
                    return;
                }
                optReturn = XMLHelper.DeserializeObject<ChanState>(strState);
                if (!optReturn.Result)
                {
                    CurrentApp.WriteLog("DealChanStateChanged", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ChanState state = optReturn.Data as ChanState;
                if (state == null)
                {
                    CurrentApp.WriteLog("DealChanStateChanged", string.Format("ChanState is null"));
                    return;
                }
                if (obj.Role == 1)
                {
                    item.VoiceChanMonObject = obj;
                    item.VoiceChanState = state;
                }
                if (obj.Role == 2)
                {
                    item.ScreenChanMonObject = obj;
                    item.ScreenChanState = state;
                }
                item.TimeDeviation = mTimeDeviation;
                Dispatcher.Invoke(new Action(item.UpdateState));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DealServerErrorMessage(ReturnMessage retMessage)
        {
            try
            {
                string strMsg = retMessage.Message;
                if (retMessage.ListData != null)
                {
                    if (retMessage.ListData.Count > 0)
                    {
                        strMsg = string.Format("{0};\t[{1}]", strMsg, retMessage.ListData[0]);
                    }
                    if (retMessage.ListData.Count > 1)
                    {
                        strMsg = string.Format("{0};\t[{1}]", strMsg, retMessage.ListData[1]);
                    }
                }
                CurrentApp.WriteLog("DealServerError", strMsg);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        private void AddMonObjToMonList()
        {
            try
            {
                List<ObjectItem> listObjs = new List<ObjectItem>();

                for (int i = 0; i < mListControlObjects.Count; i++)
                {
                    var item = mListControlObjects[i];
                    if (item.IsChecked == true)
                    {
                        if (item.ObjType == ConstValue.RESOURCE_REALEXT
                            || item.ObjType == ConstValue.RESOURCE_EXTENSION
                            || item.ObjType == ConstValue.RESOURCE_AGENT)
                        {
                            listObjs.Add(item);
                        }
                    }
                }
                if (listObjs.Count <= 0) { return; }
                mListMonitorObjects.Clear();
                string strLog = string.Empty;
                for (int i = 0; i < listObjs.Count; i++)
                {
                    var item = listObjs[i];
                    var temp = mListMonitorItems.FirstOrDefault(m => m.ObjID == item.ObjID);
                    //如果已经存在，跳过
                    if (temp != null) { continue; }
                    MonitorItem monItem;
                    MonitorObject monObj;
                    MonitorData monData;

                    monData = new MonitorData();
                    monData.ObjID = item.ObjID;
                    monData.ObjType = item.ObjType;
                    monData.Name = item.Name;
                    var obj = item.Data as ResourceObject;
                    if (obj != null)
                    {
                        monData.Other03 = obj.Other04;      //所在服务器地址
                    }
                    monData.Role = 3;
                    var temp2 = mListMonitorDatas.FirstOrDefault(d => d.ObjID == monData.ObjID);
                    if (temp2 != null)
                    {
                        mListMonitorDatas.Remove(temp2);
                    }
                    mListMonitorDatas.Add(monData);

                    monItem = MonitorItem.CreateItem(monData);
                    monItem.CurrentApp = CurrentApp;
                    monItem.ListUserParams = mListUserParams;
                    monObj = new MonitorObject();
                    monObj.MonType = MonitorType.State;
                    monObj.ObjID = monData.ObjID;
                    monObj.ObjType = monData.ObjType;
                    monObj.ObjValue = monData.Name;
                    monObj.Role = 1;
                    if (obj != null)
                    {
                        string strRole = obj.Other05;
                        if (!string.IsNullOrEmpty(obj.Other04))
                        {
                            string[] arrIPs = obj.Other04.Split(new[] { ';' }, StringSplitOptions.None);
                            if (strRole == "1"
                                || strRole == "3")
                            {
                                if (arrIPs.Length > 0)
                                {
                                    monObj.Other03 = arrIPs[0];     //所在录音服务器的IP
                                }
                            }
                        }
                    }
                    monItem.VoiceChanMonObject = monObj;
                    mListMonitorObjects.Add(monObj);
                    monObj = new MonitorObject();
                    monObj.MonType = MonitorType.State;
                    monObj.ObjID = monData.ObjID;
                    monObj.ObjType = monData.ObjType;
                    monObj.ObjValue = monData.Name;
                    monObj.Role = 2;
                    if (obj != null)
                    {
                        string strRole = obj.Other05;
                        if (!string.IsNullOrEmpty(obj.Other04))
                        {
                            string[] arrIPs = obj.Other04.Split(new[] { ';' }, StringSplitOptions.None);
                            if (strRole == "2")
                            {
                                if (arrIPs.Length > 0)
                                {
                                    monObj.Other03 = arrIPs[0];     //所在录屏服务器的IP
                                }
                            }
                            if (strRole == "3")
                            {
                                if (arrIPs.Length > 1)
                                {
                                    monObj.Other03 = arrIPs[1];      //所在录屏服务器的IP
                                }
                            }
                        }
                    }
                    monItem.ScreenChanMonObject = monObj;
                    mListMonitorObjects.Add(monObj);
                    mListMonitorItems.Add(monItem);

                    strLog += string.Format("{0}[{1}];", monItem.Name,
                        Utils.FormatOptLogString(string.Format("OBJ{0}", monObj.ObjType)));
                }

                #region 写操作日志

                CurrentApp.WriteOperationLog(S2102Consts.OPT_ADDTOMONLIST.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                if (mListMonitorObjects.Count > 0)
                {
                    AddQueryChanMonObject();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void RefreshStateMonList()
        {
            try
            {
                mListMonitorObjects.Clear();
                for (int i = 0; i < mListMonitorItems.Count; i++)
                {
                    var item = mListMonitorItems[i];
                    item.VoiceChanState = null;
                    item.ScreenChanState = null;
                    var obj = item.VoiceChanMonObject;
                    if (obj != null)
                    {
                        obj.ChanObjID = 0;
                        mListMonitorObjects.Add(obj);
                    }
                    obj = item.ScreenChanMonObject;
                    if (obj != null)
                    {
                        obj.ChanObjID = 0;
                        mListMonitorObjects.Add(obj);
                    }
                    item.TimeDeviation = mTimeDeviation;
                    Dispatcher.Invoke(new Action(item.UpdateState));
                }
                if (mMonitorClient != null)
                {
                    mMonitorClient.Stop();
                    mMonitorClient = null;
                }
                InitMonitorClient();

                #region 写操作日志

                CurrentApp.WriteOperationLog(S2102Consts.OPT_REFRESHMONLIST.ToString(), ConstValue.OPT_RESULT_SUCCESS, string.Empty);

                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveStateMonitorList()
        {
            try
            {
                if (mMonObjConfig == null)
                {
                    mMonObjConfig = new MonitorObjectConfig();
                }
                var userConfig = mMonObjConfig.ListUserMonObjConfig.FirstOrDefault(c => c.UserID == CurrentApp.Session.UserID);
                if (userConfig == null)
                {
                    userConfig = new UserMonitorObjectConfig();
                    userConfig.UserID = CurrentApp.Session.UserID;
                    mMonObjConfig.ListUserMonObjConfig.Add(userConfig);
                }
                userConfig.IsRemember = true;
                userConfig.ViewType = mViewType;
                userConfig.ListStateMonObjList.Clear();
                for (int i = 0; i < mListMonitorDatas.Count; i++)
                {
                    var data = mListMonitorDatas[i];
                    userConfig.ListStateMonObjList.Add(data);
                }
                userConfig.ListNetMonObjList.Clear();
                var uc = BorderNetMonitor.Child as UCNMonPanel;
                if (uc != null)
                {
                    if (uc.MonitorData != null)
                    {
                        userConfig.ListNetMonObjList.Add(uc.MonitorData);
                    }
                }
                string path = CurrentApp.TempPath;
                path = Path.Combine(path, MonitorObjectConfig.FILE_NAME);
                OperationReturn optReturn = XMLHelper.SerializeFile(mMonObjConfig, path);
                if (!optReturn.Result)
                {
                    #region 写操作日志

                    CurrentApp.WriteOperationLog(S2102Consts.OPT_SAVEMONLIST.ToString(), ConstValue.OPT_RESULT_EXCEPTION,
                        string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                    #endregion

                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }

                #region 写操作日志

                CurrentApp.WriteOperationLog(S2102Consts.OPT_SAVEMONLIST.ToString(), ConstValue.OPT_RESULT_SUCCESS, string.Empty);

                #endregion

                ShowInformation(CurrentApp.GetMessageLanguageInfo("003", "Save monitor list end"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void RemoveMonObjFromMonList()
        {
            try
            {
                List<MonitorItem> listItems = new List<MonitorItem>();
                string strName = string.Empty;
                string strLog = string.Empty;
                var items = LvMonitorList.SelectedItems;
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i] as MonitorItem;
                    if (item != null)
                    {
                        if (i < 5)
                        {
                            strName += string.Format("{0}[{1}]\r\n", item.Name,
                                CurrentApp.GetLanguageInfo(string.Format("OBJ{0}", item.ObjType.ToString("000")),
                                    item.ObjType.ToString()));
                        }
                        if (i == 5)
                        {
                            strName += "...";
                        }
                        strLog += string.Format("{0}[{1}];", item.Name, Utils.FormatOptLogString(string.Format("OBJ{0}", item.ObjType)));
                        listItems.Add(item);
                    }
                }
                if (listItems.Count <= 0) { return; }
                var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}", CurrentApp.GetMessageLanguageInfo("002", "Confirm remove this object?"), strName),
                    CurrentApp.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    #region 写操作日志

                    CurrentApp.WriteOperationLog(S2102Consts.OPT_REMOVEMONOBJ.ToString(), ConstValue.OPT_RESULT_CANCEL, strLog);

                    #endregion

                    return;
                }
                //服务器上移除对象
                mListMonitorObjects.Clear();
                for (int i = 0; i < listItems.Count; i++)
                {
                    var item = listItems[i];
                    var obj = item.VoiceChanMonObject;
                    if (obj != null)
                    {
                        mListMonitorObjects.Add(obj);
                    }
                    obj = item.ScreenChanMonObject;
                    if (obj != null)
                    {
                        mListMonitorObjects.Add(obj);
                    }
                }
                RemoveMonObject();
                //列表中移除对象
                for (int i = 0; i < listItems.Count; i++)
                {
                    var item = listItems[i];
                    var data = item.Data;
                    if (data != null)
                    {
                        mListMonitorDatas.Remove(data);
                    }
                    mListMonitorItems.Remove(item);
                }

                #region 写操作日志

                CurrentApp.WriteOperationLog(S2102Consts.OPT_REMOVEMONOBJ.ToString(), ConstValue.OPT_RESULT_SUCCESS, strName);

                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void NMonMonitorObject()
        {
            try
            {
                var item = LvMonitorList.SelectedItem as MonitorItem;
                if (item == null) { return; }
                var monData = mListMonitorDatas.FirstOrDefault(d => d.ObjID == item.ObjID);
                if (monData == null) { return; }
                var netMonData = new MonitorData();
                netMonData.ObjID = monData.ObjID;
                netMonData.ObjType = monData.ObjType;
                netMonData.Name = monData.Name;
                netMonData.Role = 1;
                netMonData.Other03 = monData.Other03;
                NMonMonitorObject(netMonData);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void NMonMonitorObject(MonitorData monData)
        {
            try
            {
                UCNMonPanel panelNMon = new UCNMonPanel();
                panelNMon.CurrentApp = CurrentApp;
                panelNMon.PageParent = this;
                panelNMon.ListOperations = ListOperations;
                panelNMon.MonitorData = monData;
                panelNMon.ListUserParams = mListUserParams;
                BorderNetMonitor.Child = panelNMon;
                var height = GridNMon.ActualHeight;
                if (height <= 0)
                {
                    GridNMon.Height = new GridLength(120);
                }

                ////2015-10-08 注释，不在此处写操作日志，移至消息处理过程中
                //#region 写操作日志

                //CurrentApp.WriteOperationLog(S2102Consts.OPT_NETMON.ToString(), ConstValue.OPT_RESULT_SUCCESS,
                //    string.Format("{0}[{1}]", monData.Name, monData.ObjType));

                //#endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SMonMonitorObject()
        {
            try
            {
                var item = LvMonitorList.SelectedItem as MonitorItem;
                if (item == null) { return; }
                var monData = mListMonitorDatas.FirstOrDefault(d => d.ObjID == item.ObjID);
                if (monData == null) { return; }
                var scrMonData = new MonitorData();
                scrMonData.ObjID = monData.ObjID;
                scrMonData.ObjType = monData.ObjType;
                scrMonData.Name = monData.Name;
                scrMonData.Role = 2;
                scrMonData.Other03 = monData.Other03;
                SMonMonitorObject(scrMonData);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SMonMonitorObject(MonitorData monData)
        {
            try
            {
                UCSMonPanel panelSMon = new UCSMonPanel();
                panelSMon.CurrentApp = CurrentApp;
                panelSMon.PageParent = this;
                panelSMon.CurrentApp = CurrentApp;
                panelSMon.ListOperations = ListOperations;
                panelSMon.MonitorData = monData;
                panelSMon.ListUserParams = mListUserParams;
                BorderScrMonitor.Child = panelSMon;
                var height = GridSMon.ActualHeight;
                if (height <= 0)
                {
                    GridSMon.Height = new GridLength(120);
                }

                ////2015-10-08 注释，不在此处写操作日志，移至消息处理过程中
                //#region 写操作日志

                //CurrentApp.WriteOperationLog(S2102Consts.OPT_SCRMON.ToString(), ConstValue.OPT_RESULT_SUCCESS,
                //    string.Format("{0}[{1}]", monData.Name, monData.ObjType));

                //#endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void MonitorOption()
        {
            try
            {
                UCMonitorOption ucOption = new UCMonitorOption();
                ucOption.CurrentApp = CurrentApp;
                ucOption.ListUserParams = mListUserParams;
                ucOption.PageParent = this;
                ucOption.CurrentApp = CurrentApp;
                PopupPanel.Title = string.Format("Monitor Option");
                PopupPanel.Content = ucOption;
                PopupPanel.IsOpen = true;

                #region 写操作日志

                CurrentApp.WriteOperationLog(S2102Consts.OPT_OPTION.ToString(), ConstValue.OPT_RESULT_SUCCESS, string.Empty);

                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void AddChild(ObjectItem parentItem, ObjectItem child)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(child)));
        }

        private void CreateOperationButtons()
        {
            try
            {
                OperationInfo optInfo;
                Button btn;

                PanelObjListOpts.Children.Clear();
                //添加到监视列表
                optInfo = new OperationInfo();
                optInfo.ID = S2102Consts.OPT_ADDTOMONLIST;
                optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), "AddToMonList");
                optInfo.Description = optInfo.Display;
                optInfo.Icon = "Images/00005.png";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = optInfo;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelObjListOpts.Children.Add(btn);

                PanelMonListOpts.Children.Clear();
                //刷新监视列表
                optInfo = new OperationInfo();
                optInfo.ID = S2102Consts.OPT_REFRESHMONLIST;
                optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), "RefreshMonList");
                optInfo.Description = optInfo.Display;
                optInfo.Icon = "Images/00004.png";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = optInfo;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelMonListOpts.Children.Add(btn);

                //监听
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S2102Consts.OPT_NETMON
                    || o.ID == S2102Consts.OPT_EXTNETMON
                    || o.ID == S2102Consts.OPT_AGTNETMON
                    || o.ID == S2102Consts.OPT_CHANNETMON);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelMonListOpts.Children.Add(btn);
                }
                //监屏
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S2102Consts.OPT_SCRMON);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelMonListOpts.Children.Add(btn);
                }

                //保存监视列表
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S2102Consts.OPT_SAVEMONLIST);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelMonListOpts.Children.Add(btn);
                }
                //删除监视对象
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S2102Consts.OPT_REMOVEMONOBJ);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelMonListOpts.Children.Add(btn);
                }

                //选项（设置）
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S2102Consts.OPT_OPTION);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelMonListOpts.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateViewTypeItems()
        {
            try
            {
                mListViewTypeItems.Clear();
                var viewTypes = mListBasicDataInfos.Where(b => b.InfoID == S2102Consts.BID_VIEWTYPE).ToList();
                for (int i = 0; i < viewTypes.Count; i++)
                {
                    var viewType = viewTypes[i];
                    if (!viewType.IsEnable) { continue; }
                    ViewTypeItem item = new ViewTypeItem();
                    item.Type = Convert.ToInt32(viewType.Value);
                    item.Name =
                        CurrentApp.GetLanguageInfo(
                            string.Format("BID{0}{1}", viewType.InfoID, viewType.SortID.ToString("000")), viewType.Icon);
                    item.Display = item.Name;
                    mListViewTypeItems.Add(item);
                }

                var temp = mListViewTypeItems.FirstOrDefault(v => v.Type == mViewType);
                if (temp != null)
                {
                    ComboViewTypes.SelectedItem = temp;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetStateMonitorListView()
        {
            try
            {
                switch (mViewType)
                {
                    case 0:

                        #region 多列视图

                        Style gridItemStyle = (Style)Resources["ListViewItemMonitorListStyle"];
                        if (gridItemStyle != null)
                        {
                            LvMonitorList.ItemContainerStyle = gridItemStyle;
                        }

                        GridView gv = new GridView();
                        GridViewColumn gvc;
                        GridViewColumnHeader gvch;
                        long viewID = 2102001;
                        for (int i = 0; i < mListMonitorListColumns.Count; i++)
                        {
                            ViewColumnInfo columnInfo = mListMonitorListColumns[i];
                            if (columnInfo.Visibility == "1")
                            {
                                gvc = new GridViewColumn();
                                gvch = new GridViewColumnHeader();
                                gvch.Content = columnInfo.Display;
                                gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL{0}{1}", viewID, columnInfo.ColumnName), columnInfo.Display);
                                gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL{0}{1}", viewID, columnInfo.ColumnName), columnInfo.Display);
                                gvc.Header = gvch;
                                gvc.Width = columnInfo.Width;

                                DataTemplate dt = null;
                                if (columnInfo.ColumnName == "Icon")
                                {
                                    dt = Resources["MonitorListIconCellTemplate"] as DataTemplate;
                                }
                                if (dt == null)
                                {
                                    Binding binding = new Binding();
                                    string strColName = columnInfo.ColumnName;
                                    if (strColName == "LoginState")
                                    {
                                        strColName = "StrLoginState";
                                    }
                                    if (strColName == "CallState")
                                    {
                                        strColName = "StrCallState";
                                    }
                                    if (strColName == "RecordState")
                                    {
                                        strColName = "StrRecordState";
                                    }
                                    if (strColName == "StartRecordTime")
                                    {
                                        //strColName = "StrStartRecordTime";
                                        binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                                    }
                                    if (strColName == "StopRecordTime")
                                    {
                                        //strColName = "StrStopRecordTime";
                                        binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                                    }
                                    if (strColName == "DirectionFlag")
                                    {
                                        strColName = "StrDirection";
                                    }
                                    if (strColName == "RecordLength")
                                    {
                                        //strColName = "StrRecordLength";
                                    }
                                    binding.Path = new PropertyPath(strColName);
                                    gvc.DisplayMemberBinding = binding;
                                }
                                else
                                {
                                    gvc.CellTemplate = dt;
                                }
                                gv.Columns.Add(gvc);
                            }
                        }
                        LvMonitorList.View = gv;

                        #endregion

                        break;
                    case 1:

                        #region 大图标视图

                        Style largeItemStyle = (Style)Resources["LargeMonitorListItemStyle"];
                        if (largeItemStyle != null)
                        {
                            LvMonitorList.ItemContainerStyle = largeItemStyle;
                        }
                        TileView largeView = new TileView();
                        DataTemplate largeTemplate = (DataTemplate)Resources["LargeMonitorListItemTemplate"];
                        if (largeTemplate != null)
                        {
                            largeView.ItemTemplate = largeTemplate;
                        }
                        LvMonitorList.View = largeView;

                        #endregion

                        break;
                    case 2:

                        #region 大图标视图

                        Style smallItemStyle = (Style)Resources["SmallMonitorListItemStyle"];
                        if (smallItemStyle != null)
                        {
                            LvMonitorList.ItemContainerStyle = smallItemStyle;
                        }

                        TileView smallView = new TileView();
                        DataTemplate smallTemplate = (DataTemplate)Resources["MiddleMonitorListItemTemplate"];
                        if (smallTemplate != null)
                        {
                            smallView.ItemTemplate = smallTemplate;
                        }
                        LvMonitorList.View = smallView;

                        #endregion

                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void ClosePanel(int panel)
        {
            //Panel:
            //1:        监听面板
            //2:        监屏面板
            switch (panel)
            {
                case 1:
                    BorderNetMonitor.Child = null;
                    GridNMon.Height = new GridLength(0);
                    break;
                case 2:
                    BorderScrMonitor.Child = null;
                    GridSMon.Height = new GridLength(0);
                    break;
            }
        }

        private void CreateDetailView()
        {
            try
            {
                BorderMonitorDetail.Child = null;

                var item = LvMonitorList.SelectedItem as MonitorItem;
                if (item == null) { return; }
                BorderMonitorDetail.DataContext = item;

                StackPanel panelPanel = new StackPanel();
                panelPanel.Orientation = Orientation.Vertical;
                long viewID = 2102001;

                Style itemStyle = Resources["PanelDetailItemStyle"] as Style;
                Style itemTitleStyle = Resources["TxtDetailItemTitleStyle"] as Style;
                Style itemValueStyle = Resources["TxtDetailItemValueStyle"] as Style;

                for (int i = 0; i < mListMonitorListColumns.Count; i++)
                {
                    var column = mListMonitorListColumns[i];
                    if (column.Visibility == "1")
                    {
                        Binding binding = new Binding();
                        string strColName = column.ColumnName;
                        if (strColName == "Icon") { continue; }
                        if (strColName == "LoginState")
                        {
                            strColName = "StrLoginState";
                        }
                        if (strColName == "CallState")
                        {
                            strColName = "StrCallState";
                        }
                        if (strColName == "RecordState")
                        {
                            strColName = "StrRecordState";
                        }
                        if (strColName == "StartRecordTime")
                        {
                            //strColName = "StrStartRecordTime";
                            binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                        }
                        if (strColName == "StopRecordTime")
                        {
                            //strColName = "StrStopRecordTime";
                            binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                        }
                        if (strColName == "DirectionFlag")
                        {
                            strColName = "StrDirection";
                        }
                        if (strColName == "RecordLength")
                        {
                            //strColName = "StrRecordLength";
                        }
                        binding.Path = new PropertyPath(strColName);

                        StackPanel itemPanel = new StackPanel();
                        itemPanel.Orientation = Orientation.Horizontal;
                        if (itemStyle != null)
                        {
                            itemPanel.Style = itemStyle;
                        }

                        TextBlock txtItemTitle = new TextBlock();
                        txtItemTitle.Text = CurrentApp.GetLanguageInfo(string.Format("COL{0}{1}", viewID, column.ColumnName), column.Display);
                        if (itemTitleStyle != null)
                        {
                            txtItemTitle.Style = itemTitleStyle;
                        }
                        itemPanel.Children.Add(txtItemTitle);

                        //Label txtSep = new Label();
                        //txtSep.Content = " : ";
                        //itemPanel.Children.Add(txtSep);

                        TextBlock txtItemValue = new TextBlock();
                        txtItemValue.SetBinding(TextBlock.TextProperty, binding);
                        if (itemValueStyle != null)
                        {
                            txtItemValue.Style = itemValueStyle;
                        }

                        itemPanel.Children.Add(txtItemValue);
                        panelPanel.Children.Add(itemPanel);
                    }
                }
                BorderMonitorDetail.Child = panelPanel;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void SetMyWaiterVisibility(bool isShow)
        {
            //MyWaiter.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion


        #region EventHandlers

        protected void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        {
            //base.PageHead_PageHeadEvent(sender, e);

            //switch (e.Code)
            //{
            //    //切换主题
            //    case 100:
            //        ThemeInfo themeInfo = e.Data as ThemeInfo;
            //        if (themeInfo != null)
            //        {
            //            ThemeInfo = themeInfo;
            //            CurrentApp.Session.ThemeInfo = themeInfo;
            //            CurrentApp.Session.ThemeName = themeInfo.Name;
            //            ChangeTheme();
            //            SendThemeChangeMessage();
            //        }
            //        break;
            //    //切换语言
            //    case 110:
            //        LangTypeInfo langType = e.Data as LangTypeInfo;
            //        if (langType != null)
            //        {
            //            LangTypeInfo = langType;
            //            CurrentApp.Session.LangTypeInfo = langType;
            //            CurrentApp.Session.LangTypeID = langType.LangID;
            //            MyWaiter.Visibility = Visibility.Visible;
            //            mWorker = new BackgroundWorker();
            //            mWorker.DoWork += (s, de) => CurrentApp.InitAllLanguageInfos();
            //            mWorker.RunWorkerCompleted += (s, re) =>
            //            {
            //                mWorker.Dispose();
            //                MyWaiter.Visibility = Visibility.Hidden;
            //                ChangeLanguage();
            //                SendLanguageChangeMessage();
            //            };
            //            mWorker.RunWorkerAsync();
            //        }
            //        break;
            //    //导航到Home
            //    case 202:
            //        //如果当前正在监听，则关闭监听
            //        var panel = BorderNetMonitor.Child as UCNMonPanel;
            //        if (panel != null)
            //        {
            //            panel.CloseNMonPanel();
            //        }
            //        break;
            //}
        }

        protected override void OnAppEvent(WebRequest webRequest)
        {
            base.OnAppEvent(webRequest);

            try
            {
                int code = webRequest.Code;
                string strData = webRequest.Data;
                if (CurrentApp == null) { return; }
                var session = CurrentApp.Session;
                if (session == null) { return; }
                switch (code)
                {
                    case (int)RequestCode.CSLanguageChange:
                        var langTypeInfo = session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
                        if (langTypeInfo == null) { return; }
                        LangTypeInfo = langTypeInfo;
                        session.LangTypeInfo = langTypeInfo;
                        session.LangTypeID = langTypeInfo.LangID;
                        SetBusy(true, string.Empty);
                        BackgroundWorker worker = new BackgroundWorker();
                        worker.DoWork += (s, de) => CurrentApp.InitAllLanguageInfos();
                        worker.RunWorkerCompleted += (s, re) =>
                        {
                            worker.Dispose();
                            SetBusy(false, string.Empty);

                            ChangeLanguage();
                        };
                        worker.RunWorkerAsync();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void mRecordLengthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                for (int i = 0; i < mListMonitorItems.Count; i++)
                {
                    var item = mListMonitorItems[i];
                    item.TimeDeviation = mTimeDeviation;
                    Dispatcher.Invoke(new Action(item.UpdateState));
                }
            }
            catch { }
        }

        void ComboViewTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var item = ComboViewTypes.SelectedItem as ViewTypeItem;
                if (item != null)
                {
                    mViewType = item.Type;
                }
                SetStateMonitorListView();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void StateMonitorClient_NotifyMessageReceived(object sender, NotifyMessageReceivedEventArgs e)
        {
            try
            {
                var notMessage = e.NotifyMessage;
                if (notMessage == null) { return; }
                switch (notMessage.Command)
                {
                    case (int)Service04Command.NotStateChanged:
                        ThreadPool.QueueUserWorkItem(a => DealChanStateChanged(notMessage));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void StateMonitorClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            try
            {
                var retMessage = e.ReturnMessage;
                if (retMessage == null) { return; }
                if (!retMessage.Result)
                {
                    CurrentApp.WriteLog("DealMessage", string.Format("Fail.\t{0}\t{1}", retMessage.Code, retMessage.Message));
                    return;
                }
                switch (retMessage.Command)
                {
                    case (int)RequestCode.NCWelcome:
                        CurrentApp.WriteLog("DealMessage", string.Format("Welcome message"));
                        ThreadPool.QueueUserWorkItem(a => DealWelcomeMessage(retMessage));
                        //连接成功，设定监视方式
                        ThreadPool.QueueUserWorkItem(a => SetMonType());
                        break;
                    case (int)Service04Command.ResSetMonType:
                        CurrentApp.WriteLog("DealMessage", string.Format("SetMonType response"));
                        if (retMessage.ListData != null && retMessage.ListData.Count > 1)
                        {
                            string newType = retMessage.ListData[0];
                            string oldType = retMessage.ListData[1];
                            CurrentApp.WriteLog("DealMessage", string.Format("MonType\tNew:{0}\tOld:{1}", newType, oldType));
                        }
                        //设定监视方式后，添加监视对象，并查询通道信息
                        ThreadPool.QueueUserWorkItem(a => AddQueryChanMonObject());
                        break;
                    case (int)Service04Command.ResAddQueryChan:
                        CurrentApp.WriteLog("DealMessage", string.Format("AddQueryChan response"));
                        ThreadPool.QueueUserWorkItem(a => DealAddQueryChanResponse(retMessage));
                        break;
                    case (int)Service04Command.ResQueryChan:
                        CurrentApp.WriteLog("DealMessage", string.Format("QueryChan response"));
                        ThreadPool.QueueUserWorkItem(a => DealQueryChanResponse(retMessage));
                        break;
                    case (int)Service04Command.ResRemoveMonObj:
                        CurrentApp.WriteLog("DealMessage", string.Format("RemoveMonObj response"));
                        break;
                    case (int)Service04Command.ResQueryState:
                        CurrentApp.WriteLog("DealMessage", string.Format("QueryState response"));
                        ThreadPool.QueueUserWorkItem(a => DealQueryStateResponse(retMessage));
                        break;
                    case (int)RequestCode.NCError:
                        CurrentApp.WriteLog("DealMessage", string.Format("ServerError message"));
                        ThreadPool.QueueUserWorkItem(a => DealServerErrorMessage(retMessage));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn == null) { return; }
            var optItem = btn.DataContext as OperationInfo;
            if (optItem == null) { return; }
            switch (optItem.ID)
            {
                case S2102Consts.OPT_ADDTOMONLIST:
                    AddMonObjToMonList();
                    break;
                case S2102Consts.OPT_REFRESHMONLIST:
                    RefreshStateMonList();
                    break;
                case S2102Consts.OPT_SAVEMONLIST:
                    SaveStateMonitorList();
                    break;
                case S2102Consts.OPT_REMOVEMONOBJ:
                    RemoveMonObjFromMonList();
                    break;
                case S2102Consts.OPT_NETMON:
                case S2102Consts.OPT_EXTNETMON:
                case S2102Consts.OPT_CHANNETMON:
                case S2102Consts.OPT_AGTNETMON:
                    NMonMonitorObject();
                    break;
                case S2102Consts.OPT_SCRMON:
                    SMonMonitorObject();
                    break;
                case S2102Consts.OPT_OPTION:
                    MonitorOption();
                    break;
            }
        }

        void LvMonitorList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CreateDetailView();
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
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID), "Channel Monitor");

                TxtObjListTitle.Text = CurrentApp.GetLanguageInfo("2102010", "Object List");
                TxtMonListTitle.Text = CurrentApp.GetLanguageInfo("2102011", "Monitor List");
                TxtMonDetailTitle.Text = CurrentApp.GetLanguageInfo("2102012", "Detail Information");

                for (int i = 0; i < ListOperations.Count; i++)
                {
                    var opt = ListOperations[i];
                    opt.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", opt.ID), opt.ID.ToString());
                    opt.Description = opt.Display;
                }

                CreateViewTypeItems();
                CreateOperationButtons();
                SetStateMonitorListView();
                CreateDetailView();

                var subPanel = BorderNetMonitor.Child as ILanguagePage;
                if (subPanel != null)
                {
                    subPanel.ChangeLanguage();
                }

                PopupPanel.ChangeLanguage();
            }
            catch { }
        }

        #endregion

    }
}
