//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6339fc93-7a28-42a2-8edb-e5d276168f9e
//        CLR Version:              4.0.30319.18063
//        Name:                     ChanMonitorMainPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102
//        File Name:                ChanMonitorMainPage
//
//        created by Charley at 2015/6/19 16:22:50
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
    /// ChanMonitorMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChanMonitorMainPage
    {

        #region Members

        private const int MAX_MONOBJ_SIZE = 50;     //每次请求的监视对象的最大个数

        /// <summary>
        /// 当前用户操作权限
        /// </summary>
        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();

        private BackgroundWorker mWorker;
        private List<ObjectItem> mListControlObjects;
        private ObjectItem mRootObject;
        private MonitorClient mMonitorClient;

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


        public ChanMonitorMainPage()
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

            Unloaded += ChanMonitorMainPage_Unloaded;
            ComboViewTypes.SelectionChanged += ComboViewTypes_SelectionChanged;

            mViewType = 1;
            mRecordLengthTimer = new Timer(1000);
            mTimeDeviation = 0;
        }

        void ChanMonitorMainPage_Unloaded(object sender, RoutedEventArgs e)
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
                //PageHeadType = PageHeadType.Min;

                base.Init();

                TreeObjects.ItemsSource = mRootObject.Children;
                LvMonitorList.ItemsSource = mListMonitorItems;
                ComboViewTypes.ItemsSource = mListViewTypeItems;
                mRecordLengthTimer.Elapsed += mRecordLengthTimer_Elapsed;
                LvMonitorList.SelectionChanged += LvMonitorList_SelectionChanged;

                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("001", "Loading basic information..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    //触发Loaded消息
                    SendLoadedMessage();

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
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);

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

                    ChangeLanguage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadGlobalParamInfos()
        {
            try
            {
                mListGlobalParamInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList2;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(string.Format("{0}", ConstValue.GP_GROUP_OBJ_CONTROL));
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    GlobalParamInfo info = optReturn.Data as GlobalParamInfo;
                    if (info != null)
                    {
                        mListGlobalParamInfos.Add(info);
                    }
                }

                App.WriteLog("PageLoad", string.Format("Init GlobalParamInfo end"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadOperations()
        {
            try
            {
                ListOperations.Clear();
                LoadOperations(App.ModuleID);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadOperations(long parentOptID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("21");
                webRequest.ListData.Add(parentOptID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        ListOperations.Add(optInfo);
                        //加载下级操作
                        LoadOperations(optInfo.ID);
                    }
                }

                App.WriteLog("PageLoad", string.Format("Init Operations"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadUserParams()
        {
            try
            {
                mListUserParams.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserParamList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(string.Format("{0}{1}{2}", 210201, ConstValue.SPLITER_CHAR, 310204));
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<UserParamInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    UserParamInfo info = optReturn.Data as UserParamInfo;
                    if (info != null)
                    {
                        mListUserParams.Add(info);
                    }
                }

                App.WriteLog("PageLoad", string.Format("Init UserParamList end"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadBasicDataInfos()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetBasicDataInfoList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add("3");       //获取指定InfoID范围的所有BasicDataInfo
                webRequest.ListData.Add("210200000");
                webRequest.ListData.Add("0");
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                App.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<BasicDataInfo> listinfos = new List<BasicDataInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicDataInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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

                App.WriteLog("PageLoad", string.Format("Init BasicDataInfos"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadMonitorListColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("2102001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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

                App.WriteLog("PageLoad", string.Format("Init MonitorListColumns"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadMonitorConfig()
        {
            try
            {
                string path = App.TempPath;
                path = Path.Combine(path, MonitorObjectConfig.FILE_NAME);
                if (!File.Exists(path))
                {
                    App.WriteLog("LoadMonitorConfig", string.Format("Fail.\tConfig file not exist.\t{0}", path));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<MonitorObjectConfig>(path);
                if (!optReturn.Result)
                {
                    App.WriteLog("LoadMonitorConfig", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                var monObjConfig = optReturn.Data as MonitorObjectConfig;
                if (monObjConfig == null)
                {
                    App.WriteLog("LoadMonitorConfig", string.Format("Fail.\tMonitorObjectConfig is null."));
                    return;
                }
                mMonObjConfig = monObjConfig;
                var userConfig = mMonObjConfig.ListUserMonObjConfig.FirstOrDefault(c => c.UserID == App.Session.UserID);
                if (userConfig != null
                    && userConfig.IsRemember)
                {
                    mViewType = userConfig.ViewType;
                }

                App.WriteLog("PageLoad", string.Format("Init MonitorConfig"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadControlObjects()
        {
            try
            {
                LoadControlOrgs(mRootObject, -1);

                App.WriteLog("PageLoad", string.Format("Init ControlObjects"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadControlOrgs(ObjectItem parentItem, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserObjList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_ORG.ToString());
                webRequest.ListData.Add(parentID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfos = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    BasicOrgInfo orgInfo = new BasicOrgInfo();
                    long orgID;
                    if (arrInfos.Length > 0)
                    {
                        if (!long.TryParse(arrInfos[0], out orgID))
                        {
                            App.ShowExceptionMessage(string.Format("OrgID invalid.\t{0}", arrInfos[0]));
                            return;
                        }
                        orgInfo.OrgID = orgID;
                    }
                    if (arrInfos.Length > 1)
                    {
                        orgInfo.OrgName = arrInfos[1];
                    }
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = orgInfo.OrgID;
                    item.Name = orgInfo.OrgName;
                    item.Description = orgInfo.OrgName;
                    item.Icon = string.Format("Images/{0}.png", "00001");
                    item.Data = orgInfo;
                    AddChild(parentItem, item);
                    mListControlObjects.Add(item);

                    LoadControlOrgs(item, orgInfo.OrgID);
                    LoadControlExtension(item, orgInfo.OrgID);
                    LoadControlRealExt(item, orgInfo.OrgID);
                    LoadControlAgents(item, orgInfo.OrgID);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_AGENT.ToString());
                webRequest.ListData.Add(parentID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\tResourceObj is null"));
                        return;
                    }
                    BasicAgentInfo agentInfo = new BasicAgentInfo();
                    agentInfo.ID = obj.ObjID;
                    agentInfo.AgentID = obj.Name;
                    agentInfo.AgentName = obj.FullName;
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = agentInfo.ID;
                    item.Name = agentInfo.AgentID;
                    item.Description = string.Format("{0}[{1}]", agentInfo.AgentName, agentInfo.AgentID);
                    item.Icon = string.Format("Images/{0}.png", "00002");
                    item.Data = agentInfo;
                    AddChild(parentItem, item);
                    mListControlObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_EXTENSION.ToString());
                webRequest.ListData.Add(parentID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\tResourceObject is null"));
                        return;
                    }
                    BasicExtensionInfo extInfo = new BasicExtensionInfo();
                    extInfo.ObjID = obj.ObjID;
                    extInfo.Extension = obj.Name;
                    extInfo.ChanName = obj.FullName;
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_EXTENSION;
                    item.ObjID = extInfo.ObjID;
                    item.Name = extInfo.Extension;
                    item.Description = string.Format("{0}[{1}]", extInfo.ChanName, extInfo.Extension);
                    item.Icon = string.Format("Images/{0}.png", "00003");
                    item.Data = extInfo;
                    AddChild(parentItem, item);
                    mListControlObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_REALEXT.ToString());
                webRequest.ListData.Add(parentID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject obj = optReturn.Data as ResourceObject;
                    if (obj == null)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\tResourceObject is null"));
                        return;
                    }
                    BasicExtensionInfo extInfo = new BasicExtensionInfo();
                    extInfo.ObjID = obj.ObjID;
                    extInfo.Extension = obj.Name;
                    extInfo.ChanName = obj.FullName;
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_REALEXT;
                    item.ObjID = extInfo.ObjID;
                    item.Name = extInfo.Extension;
                    item.Description = string.Format("{0}[{1}]", extInfo.ChanName, extInfo.Extension);
                    item.Icon = string.Format("Images/{0}.png", "00015");
                    item.Data = extInfo;
                    AddChild(parentItem, item);
                    mListControlObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                var userConfig = mMonObjConfig.ListUserMonObjConfig.FirstOrDefault(c => c.UserID == App.Session.UserID);
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
                    item.ListUserParams = mListUserParams;
                    MonitorObject monObj = new MonitorObject();
                    monObj.MonType = MonitorType.State;
                    monObj.ObjID = data.ObjID;
                    monObj.ObjType = data.ObjType;
                    monObj.ObjValue = data.Name;
                    monObj.Role = 1;
                    item.VoiceChanMonObject = monObj;
                    mListMonitorObjects.Add(monObj);
                    monObj = new MonitorObject();
                    monObj.MonType = MonitorType.State;
                    monObj.ObjID = data.ObjID;
                    monObj.ObjType = data.ObjType;
                    monObj.ObjValue = data.Name;
                    monObj.Role = 2;
                    item.ScreenChanMonObject = monObj;
                    mListMonitorObjects.Add(monObj);
                    item.UpdateState();
                    mListMonitorItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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

                string strAddress = App.Session.AppServerInfo.Address;
                int intPort = App.Session.AppServerInfo.SupportHttps
                    ? App.Session.AppServerInfo.Port - 5
                    : App.Session.AppServerInfo.Port - 4;

                //string strAddress = "192.168.5.31";
                //int intPort = 8081 - 4;

                mMonitorClient = new MonitorClient();
                mMonitorClient.Debug += (mode, msg) => App.WriteLog("MonitorClient", msg);
                mMonitorClient.ReturnMessageReceived += StateMonitorClient_ReturnMessageReceived;
                mMonitorClient.NotifyMessageReceived += StateMonitorClient_NotifyMessageReceived;
                mMonitorClient.Connect(strAddress, intPort);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitNetMonitor()
        {
            try
            {
                if (mMonObjConfig == null) { return; }
                var userConfig = mMonObjConfig.ListUserMonObjConfig.FirstOrDefault(u => u.UserID == App.Session.UserID);
                if (userConfig == null) { return; }
                var monData = userConfig.ListNetMonObjList.FirstOrDefault();
                if (monData == null) { return; }
                var temp = mListControlObjects.FirstOrDefault(o => o.ObjID == monData.ObjID);
                if (temp == null) { return; }
                NMonMonitorObject(monData);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                App.WriteLog("MonitorClient", string.Format("Send SetMonType message end"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                                App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            request.ListData.Add(optReturn.Data.ToString());
                        }
                        //添加
                        App.WriteLog("AddQueryChan", string.Format("Count:{0}", count));
                        lock (this)
                        {
                            optReturn = mMonitorClient.SendMessage(request);
                        }
                        if (!optReturn.Result)
                        {
                            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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
                App.ShowExceptionMessage(ex.Message);
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
                    App.WriteLog("QueryChan", string.Format("MonObj:{0}", obj.ObjValue));
                    lock (this)
                    {
                        optReturn = mMonitorClient.SendMessage(request);
                    }
                    if (!optReturn.Result)
                    {
                        App.WriteLog("QueryChan",
                           string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                    App.WriteLog("RemoveMonObj", string.Format("Count:{0}", count));
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
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                    App.WriteLog("QueryState", string.Format("MonObj:{0}", obj.ObjValue));
                    lock (this)
                    {
                        optReturn = mMonitorClient.SendMessage(request);
                    }
                    if (!optReturn.Result)
                    {
                        App.WriteLog("QueryChanState",
                           string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("QueryChanState", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void DealWelcomeMessage(ReturnMessage retMessage)
        {
            try
            {
                if (retMessage.ListData == null || retMessage.ListData.Count < 4)
                {
                    App.WriteLog("DealQueryChanResponse", string.Format("ListData is null or count invalid"));
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
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void DealAddQueryChanResponse(ReturnMessage retMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (retMessage.ListData == null || retMessage.ListData.Count < 1)
                {
                    App.WriteLog("DealAddQueryChan", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strCount = retMessage.ListData[0];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    App.WriteLog("DealAddQueryChan", string.Format("ListData count param invalid"));
                    return;
                }
                if (retMessage.ListData.Count < intCount + 1)
                {
                    App.WriteLog("DealAddQueryChan", string.Format("ListData count invalid"));
                    return;
                }
                for (int i = 0; i < intCount; i++)
                {
                    var strInfo = retMessage.ListData[i + 1];
                    optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("DealAddQueryChan",
                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    MonitorObject obj = optReturn.Data as MonitorObject;
                    if (obj == null)
                    {
                        App.WriteLog("DealAddQueryChan", string.Format("MonitorObject is null"));
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
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void DealQueryChanResponse(ReturnMessage retMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (retMessage.ListData == null || retMessage.ListData.Count < 1)
                {
                    App.WriteLog("DealQueryChan", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strCount = retMessage.ListData[0];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    App.WriteLog("DealQueryChan", string.Format("ListData count param invalid"));
                    return;
                }
                if (retMessage.ListData.Count < intCount + 1)
                {
                    App.WriteLog("DealQueryChan", string.Format("ListData count invalid"));
                    return;
                }
                for (int i = 0; i < intCount; i++)
                {
                    var strInfo = retMessage.ListData[i + 1];
                    optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("DealQueryChan",
                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    MonitorObject obj = optReturn.Data as MonitorObject;
                    if (obj == null)
                    {
                        App.WriteLog("DealQueryChan", string.Format("MonitorObject is null"));
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
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void DealQueryStateResponse(ReturnMessage retMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (retMessage.ListData == null || retMessage.ListData.Count < 2)
                {
                    App.WriteLog("DealQueryState", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = retMessage.ListData[0];
                string strState = retMessage.ListData[1];
                MonitorObject obj = mListMonitorObjects.FirstOrDefault(o => o.MonID == strMonID);
                if (obj == null)
                {
                    App.WriteLog("DealQueryState", string.Format("Monitor object not exist"));
                    return;
                }
                optReturn = XMLHelper.DeserializeObject<ChanState>(strState);
                if (!optReturn.Result)
                {
                    App.WriteLog("DealQueryState", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ChanState chanState = optReturn.Data as ChanState;
                if (chanState == null)
                {
                    App.WriteLog("DealQueryState", string.Format("ChannelState object is null"));
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
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void DealChanStateChanged(NotifyMessage notMessage)
        {
            try
            {
                OperationReturn optReturn;
                if (notMessage.ListData == null || notMessage.ListData.Count < 3)
                {
                    App.WriteLog("DealChanStateChanged", string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = notMessage.ListData[0];
                string strState = notMessage.ListData[1];
                string strNewChanObjID = notMessage.ListData[2];
                long newChanObjID;
                MonitorObject obj = mListMonitorObjects.FirstOrDefault(o => o.MonID == strMonID);
                if (obj == null)
                {
                    App.WriteLog("DealChanStateChanged", string.Format("MonitorObject not exist."));
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
                var temp = mListMonitorItems.FirstOrDefault(m => m.ObjID == obj.ObjID);
                if (temp != null)
                {
                    optReturn = XMLHelper.DeserializeObject<ChanState>(strState);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("DealChanStateChanged", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ChanState state = optReturn.Data as ChanState;
                    if (state == null)
                    {
                        App.WriteLog("DealChanStateChanged", string.Format("ChanState is null"));
                        return;
                    }
                    if (obj.Role == 1)
                    {
                        temp.VoiceChanMonObject = obj;
                        temp.VoiceChanState = state;
                    }
                    if (obj.Role == 2)
                    {
                        temp.ScreenChanMonObject = obj;
                        temp.ScreenChanState = state;
                    }
                    temp.TimeDeviation = mTimeDeviation;
                    Dispatcher.Invoke(new Action(temp.UpdateState));
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                App.WriteLog("DealServerError", strMsg);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                    switch (item.ObjType)
                    {
                        case ConstValue.RESOURCE_AGENT:
                            BasicAgentInfo agentInfo = item.Data as BasicAgentInfo;
                            if (agentInfo != null)
                            {
                                monData.Name = agentInfo.AgentID;
                            }
                            break;
                        case ConstValue.RESOURCE_EXTENSION:
                        case ConstValue.RESOURCE_REALEXT:
                            BasicExtensionInfo extInfo = item.Data as BasicExtensionInfo;
                            if (extInfo != null)
                            {
                                monData.Name = extInfo.Extension;
                            }
                            break;
                    }
                    monData.Role = 3;
                    var temp2 = mListMonitorDatas.FirstOrDefault(d => d.ObjID == monData.ObjID);
                    if (temp2 != null)
                    {
                        mListMonitorDatas.Remove(temp2);
                    }
                    mListMonitorDatas.Add(monData);

                    monItem = MonitorItem.CreateItem(monData);
                    monItem.ListUserParams = mListUserParams;
                    monObj = new MonitorObject();
                    monObj.MonType = MonitorType.State;
                    monObj.ObjID = monData.ObjID;
                    monObj.ObjType = monData.ObjType;
                    monObj.ObjValue = monData.Name;
                    monObj.Role = 1;
                    monItem.VoiceChanMonObject = monObj;
                    mListMonitorObjects.Add(monObj);
                    monObj = new MonitorObject();
                    monObj.MonType = MonitorType.State;
                    monObj.ObjID = monData.ObjID;
                    monObj.ObjType = monData.ObjType;
                    monObj.ObjValue = monData.Name;
                    monObj.Role = 2;
                    monItem.ScreenChanMonObject = monObj;
                    mListMonitorObjects.Add(monObj);
                    mListMonitorItems.Add(monItem);
                    strLog += string.Format("{0}[{1}];", monItem.Name,
                        Utils.FormatOptLogString(string.Format("OBJ{0}", monObj.ObjType)));
                }

                #region 写操作日志

                App.WriteOperationLog(S2102Consts.OPT_ADDTOMONLIST.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                if (mListMonitorObjects.Count > 0)
                {
                    AddQueryChanMonObject();
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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

                App.WriteOperationLog(S2102Consts.OPT_REFRESHMONLIST.ToString(), ConstValue.OPT_RESULT_SUCCESS, string.Empty);

                #endregion
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                var userConfig = mMonObjConfig.ListUserMonObjConfig.FirstOrDefault(c => c.UserID == App.Session.UserID);
                if (userConfig == null)
                {
                    userConfig = new UserMonitorObjectConfig();
                    userConfig.UserID = App.Session.UserID;
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
                string path = App.TempPath;
                path = Path.Combine(path, MonitorObjectConfig.FILE_NAME);
                OperationReturn optReturn = XMLHelper.SerializeFile(mMonObjConfig, path);
                if (!optReturn.Result)
                {
                    #region 写操作日志

                    App.WriteOperationLog(S2102Consts.OPT_SAVEMONLIST.ToString(), ConstValue.OPT_RESULT_EXCEPTION,
                        string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));

                    #endregion

                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }

                #region 写操作日志

                App.WriteOperationLog(S2102Consts.OPT_SAVEMONLIST.ToString(), ConstValue.OPT_RESULT_SUCCESS, string.Empty);

                #endregion

                App.ShowInfoMessage(App.GetMessageLanguageInfo("003", "Save monitor list end"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void RemoveMonObjFromMonList()
        {
            try
            {
                List<MonitorItem> listItems = new List<MonitorItem>();
                string strName = string.Empty;
                var items = LvMonitorList.SelectedItems;
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i] as MonitorItem;
                    if (item != null)
                    {
                        if (i < 5)
                        {
                            strName += string.Format("{0}[{1}]\r\n", item.Name, Utils.FormatOptLogString(string.Format("OBJ{0}", item.ObjType)));
                        }
                        if (i == 5)
                        {
                            strName += "...";
                        }
                        listItems.Add(item);
                    }
                }
                if (listItems.Count <= 0) { return; }
                var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}", App.GetMessageLanguageInfo("002", "Confirm remove this object?"), strName),
                    App.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes)
                {
                    #region 写操作日志

                    App.WriteOperationLog(S2102Consts.OPT_REMOVEMONOBJ.ToString(), ConstValue.OPT_RESULT_CANCEL, strName);

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

                App.WriteOperationLog(S2102Consts.OPT_REMOVEMONOBJ.ToString(), ConstValue.OPT_RESULT_SUCCESS, strName);

                #endregion
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                NMonMonitorObject(netMonData);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void NMonMonitorObject(MonitorData monData)
        {
            try
            {
                UCNMonPanel panelNMon = new UCNMonPanel();
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

                //App.WriteOperationLog(S2102Consts.OPT_NETMON.ToString(), ConstValue.OPT_RESULT_SUCCESS,
                //    string.Format("{0}[{1}]", monData.Name, monData.ObjType));

                //#endregion
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                SMonMonitorObject(scrMonData);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void SMonMonitorObject(MonitorData monData)
        {
            try
            {
                UCSMonPanel panelSMon = new UCSMonPanel();
                panelSMon.PageParent = this;
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

                //App.WriteOperationLog(S2102Consts.OPT_SCRMON.ToString(), ConstValue.OPT_RESULT_SUCCESS,
                //    string.Format("{0}[{1}]", monData.Name, monData.ObjType));

                //#endregion
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void MonitorOption()
        {
            try
            {
                UCMonitorOption ucOption = new UCMonitorOption();
                ucOption.ListUserParams = mListUserParams;
                ucOption.PageParent = this;
                PopupPanel.Title = string.Format("Monitor Option");
                PopupPanel.Content = ucOption;
                PopupPanel.IsOpen = true;

                #region 写操作日志

                App.WriteOperationLog(S2102Consts.OPT_OPTION.ToString(), ConstValue.OPT_RESULT_SUCCESS, string.Empty);

                #endregion
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), "AddToMonList");
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
                optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), "RefreshMonList");
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
                App.ShowExceptionMessage(ex.Message);
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
                        App.GetLanguageInfo(
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
                App.ShowExceptionMessage(ex.Message);
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
                                gvch.Content = App.GetLanguageInfo(string.Format("COL{0}{1}", viewID, columnInfo.ColumnName), columnInfo.Display);
                                gvch.ToolTip = App.GetLanguageInfo(string.Format("COL{0}{1}", viewID, columnInfo.ColumnName), columnInfo.Display);
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
                App.ShowExceptionMessage(ex.Message);
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
                        txtItemTitle.Text = App.GetLanguageInfo(string.Format("COL{0}{1}", viewID, column.ColumnName), column.Display);
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
                App.ShowExceptionMessage(ex.Message);
            }
        }

        public void SetMyWaiterVisibility(bool isShow)
        {
            MyWaiter.Visibility = isShow ? Visibility.Visible : Visibility.Hidden;
        }

        #endregion


        #region EventHandlers

        protected override void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        {
            base.PageHead_PageHeadEvent(sender, e);

            switch (e.Code)
            {
                //切换主题
                case 100:
                    ThemeInfo themeInfo = e.Data as ThemeInfo;
                    if (themeInfo != null)
                    {
                        ThemeInfo = themeInfo;
                        App.Session.ThemeInfo = themeInfo;
                        App.Session.ThemeName = themeInfo.Name;
                        ChangeTheme();
                        SendThemeChangeMessage();
                    }
                    break;
                //切换语言
                case 110:
                    LangTypeInfo langType = e.Data as LangTypeInfo;
                    if (langType != null)
                    {
                        LangTypeInfo = langType;
                        App.Session.LangTypeInfo = langType;
                        App.Session.LangTypeID = langType.LangID;
                        MyWaiter.Visibility = Visibility.Visible;
                        mWorker = new BackgroundWorker();
                        mWorker.DoWork += (s, de) => App.InitAllLanguageInfos();
                        mWorker.RunWorkerCompleted += (s, re) =>
                        {
                            mWorker.Dispose();
                            MyWaiter.Visibility = Visibility.Hidden;
                            ChangeLanguage();
                            SendLanguageChangeMessage();
                        };
                        mWorker.RunWorkerAsync();
                    }
                    break;
                //导航到Home
                case 202:
                    //如果当前正在监听，则关闭监听
                    var panel = BorderNetMonitor.Child as UCNMonPanel;
                    if (panel != null)
                    {
                        panel.CloseNMonPanel();
                    }
                    break;
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
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void StateMonitorClient_NotifyMessageReceived(NotifyMessage notMessage)
        {
            try
            {
                switch (notMessage.Command)
                {
                    case (int)Service04Command.NotStateChanged:
                        ThreadPool.QueueUserWorkItem(a => DealChanStateChanged(notMessage));
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void StateMonitorClient_ReturnMessageReceived(ReturnMessage retMessage)
        {
            try
            {
                if (!retMessage.Result)
                {
                    App.WriteLog("DealMessage", string.Format("Fail.\t{0}\t{1}", retMessage.Code, retMessage.Message));
                    return;
                }
                switch (retMessage.Command)
                {
                    case (int)RequestCode.NCWelcome:
                        App.WriteLog("DealMessage", string.Format("Welcome message"));
                        ThreadPool.QueueUserWorkItem(a => DealWelcomeMessage(retMessage));
                        //连接成功，设定监视方式
                        ThreadPool.QueueUserWorkItem(a => SetMonType());
                        break;
                    case (int)Service04Command.ResSetMonType:
                        App.WriteLog("DealMessage", string.Format("SetMonType response"));
                        if (retMessage.ListData != null && retMessage.ListData.Count > 1)
                        {
                            string newType = retMessage.ListData[0];
                            string oldType = retMessage.ListData[1];
                            App.WriteLog("DealMessage", string.Format("MonType\tNew:{0}\tOld:{1}", newType, oldType));
                        }
                        //设定监视方式后，添加监视对象，并查询通道信息
                        ThreadPool.QueueUserWorkItem(a => AddQueryChanMonObject());
                        break;
                    case (int)Service04Command.ResAddQueryChan:
                        App.WriteLog("DealMessage", string.Format("AddQueryChan response"));
                        ThreadPool.QueueUserWorkItem(a => DealAddQueryChanResponse(retMessage));
                        break;
                    case (int)Service04Command.ResQueryChan:
                        App.WriteLog("DealMessage", string.Format("QueryChan response"));
                        ThreadPool.QueueUserWorkItem(a => DealQueryChanResponse(retMessage));
                        break;
                    case (int)Service04Command.ResRemoveMonObj:
                        App.WriteLog("DealMessage", string.Format("RemoveMonObj response"));
                        break;
                    case (int)Service04Command.ResQueryState:
                        App.WriteLog("DealMessage", string.Format("QueryState response"));
                        ThreadPool.QueueUserWorkItem(a => DealQueryStateResponse(retMessage));
                        break;
                    case (int)RequestCode.NCError:
                        App.WriteLog("DealMessage", string.Format("ServerError message"));
                        ThreadPool.QueueUserWorkItem(a => DealServerErrorMessage(retMessage));
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
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
                    //App.ShowExceptionMessage("1" + ex.Message);
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
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }

            //var subPanel = BorderNetMonitor.Child as IThemePage;
            //if (subPanel != null)
            //{
            //    subPanel.ChangeTheme();
            //}
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                if (PageHead != null)
                {
                    PageHead.AppName = App.GetLanguageInfo("2102001", "UMP Net Monitor Center");
                }

                TxtObjListTitle.Text = App.GetLanguageInfo("2102010", "Object List");
                TxtMonListTitle.Text = App.GetLanguageInfo("2102011", "Monitor List");
                TxtMonDetailTitle.Text = App.GetLanguageInfo("2102012", "Detail Information");

                for (int i = 0; i < ListOperations.Count; i++)
                {
                    var opt = ListOperations[i];
                    opt.Display = App.GetLanguageInfo(string.Format("FO{0}", opt.ID), opt.ID.ToString());
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


        #region NetPipeMessage

        protected override void App_NetPipeEvent(WebRequest webRequest)
        {
            base.App_NetPipeEvent(webRequest);

            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    var code = webRequest.Code;
                    var session = webRequest.Session;
                    var strData = webRequest.Data;
                    switch (code)
                    {
                        case (int)RequestCode.SCLanguageChange:
                            LangTypeInfo langTypeInfo =
                               App.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
                            if (langTypeInfo != null)
                            {
                                LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeInfo = langTypeInfo;
                                App.Session.LangTypeID = langTypeInfo.LangID;
                                if (MyWaiter != null)
                                {
                                    MyWaiter.Visibility = Visibility.Visible;
                                }
                                mWorker = new BackgroundWorker();
                                mWorker.DoWork += (s, de) => App.InitAllLanguageInfos();
                                mWorker.RunWorkerCompleted += (s, re) =>
                                {
                                    mWorker.Dispose();
                                    if (MyWaiter != null)
                                    {
                                        MyWaiter.Visibility = Visibility.Hidden;
                                    }
                                    ChangeLanguage();
                                    if (PopupPanel != null)
                                    {
                                        PopupPanel.ChangeLanguage();
                                    }
                                };
                                mWorker.RunWorkerAsync();
                            }
                            break;
                        case (int)RequestCode.SCThemeChange:
                            ThemeInfo themeInfo = App.Session.SupportThemes.FirstOrDefault(t => t.Name == strData);
                            if (themeInfo != null)
                            {
                                ThemeInfo = themeInfo;
                                App.Session.ThemeInfo = themeInfo;
                                App.Session.ThemeName = themeInfo.Name;
                                ChangeTheme();
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    App.ShowExceptionMessage(ex.Message);
                }
            }));
        }

        #endregion

    }
}
