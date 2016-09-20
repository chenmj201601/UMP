//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6035c4b4-8c41-4e67-83a2-e2bf55b5e621
//        CLR Version:              4.0.30319.18063
//        Name:                     AlarmMessageMainPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501
//        File Name:                AlarmMessageMainPage
//
//        created by Charley at 2015/5/20 11:05:31
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UMPS2501.Commands;
using UMPS2501.Models;
using UMPS2501.Wcf11012;
using UMPS2501.Wcf25011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common25011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;
/*======================================================================
 * 告警信息管理主页面
 * 
 * 名词:
 * 1、AlarmMessage(告警消息):表示告警信息的消息属性,包括类型,所在模块,消息ID(序号),状态ID(序号)等
 * 2、AlarmInfomation(告警信息):表示单个告警信息,包括可供用户配置的名称,等级等属性
 * 3、AlarmReceiver(告警接收人):表示一条告警信息的单个接收人信息,包括接收人ID,发送方式及回复方式等属性
 * 
 * 逻辑关系:
 * 1、AlarmMessage通常是系统初始化后就确定的,用户不可随意增加删除AlarmMessage
 * 2、每个AlarmMessage都拥有一个唯一的流水号(类型编码202),此流水号也是系统初始化就确定的,并且语言包标识就是根据这个流水号标识的(2501MSG+SerialID)
 * 3、AlarmInfomation依赖于AlarmMessage，使用MessageID属性关联到对应的AlarmMessage
 * 4、逻辑上多个AlarmInfomation可以指定同一个AlarmMessage，但是系统目前限制同一AlarmMessage不能重复添加到AlarmInfomation中（暂时的限制）
 * 5、每个AlarmInfomation可以指定多个AlarmReceiver，通过AlarmInfoID关联
 * 6、AlarmReceiver中可以设定发送方式和回复方式（这两个属性都是按位组合的）
 * 
 ======================================================================*/
using VoiceCyber.Wpf.CustomControls.ListItem.Implementation;

namespace UMPS2501
{
    /// <summary>
    /// AlarmMessageMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class AlarmMessageMainPage
    {

        #region Members
        /// <summary>
        /// 当前用户操作权限
        /// </summary>
        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();

        private BackgroundWorker mWorker;
        private List<BasicDataInfo> mListBasicDataInfos;        //基础信息列表
        private ObservableCollection<ViewColumnInfo> mListColumnItems;      //告警信息列表列
        private ObservableCollection<ViewColumnInfo> mListReceiverColumnItems;      //告警接收人列表列
        private List<AlarmMessageInfo> mListAlarmMessageInfos;      //告警消息信息列表
        private List<AlarmReceiverInfo> mListAlarmReceiverInfos;        //告警接收人信息列表
        private ObservableCollection<AlarmInfomationItem> mListAlarmInfomationItems;        //告警信息列表的绑定源
        private List<ObjectItem> mListAlarmMessageItems;        //告警消息项列表,告警消息树的二维列表
        private List<ObjectItem> mListAlarmReceiverItems;       //告警接收人项列表,告警接收人树的二维列表
        private List<long> mListRemoveAlarmInfoIDs;         //待删除的告警信息的ID列表
        private ObjectItem mAlarmMessageRoot;           //告警消息树的根节点
        private ObjectItem mAlarmReceiverRoot;      //告警接收人的根节点
        private AlarmInfomationItem mCurrentAlarmInfoItem;      //当前选中的告警信息节点
        private bool mIsOptSuccess; //操作标识

        #endregion


        public AlarmMessageMainPage()
        {
            InitializeComponent();

            mListBasicDataInfos = new List<BasicDataInfo>();
            mListColumnItems = new ObservableCollection<ViewColumnInfo>();
            mListReceiverColumnItems = new ObservableCollection<ViewColumnInfo>();
            mListAlarmMessageInfos = new List<AlarmMessageInfo>();
            mListAlarmReceiverInfos = new List<AlarmReceiverInfo>();
            mListAlarmInfomationItems = new ObservableCollection<AlarmInfomationItem>();
            mListAlarmMessageItems = new List<ObjectItem>();
            mListAlarmReceiverItems = new List<ObjectItem>();
            mListRemoveAlarmInfoIDs = new List<long>();
            mAlarmMessageRoot = new ObjectItem();
            mAlarmReceiverRoot = new ObjectItem();

            TvAlarmMessageList.SelectedItemChanged += TvAlarmMessageList_SelectedItemChanged;
            LvAlarmInfoList.SelectionChanged += LvAlarmInfoList_SelectionChanged;
            LvAlarmInfoList.Drop += LvAlarmInfoList_Drop;
            TvAlarmMessageList.AddHandler(ListItemPanel.ItemMouseDoubleClickEvent,
                new RoutedPropertyChangedEventHandler<ListItemEventArgs>(TreeItemAlarmMessage_MouseDoubleClick));

        }


        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "AlarmMessageMainPage";
                StylePath = "UMPS2501/AlarmMessageMainPage.xaml";
                //PageHeadType = PageHeadType.Middle;

                TvAlarmMessageList.ItemsSource = mAlarmMessageRoot.Children;
                GridTreeReceiverList.ItemsSource = mAlarmReceiverRoot.Children;
                LvAlarmInfoList.ItemsSource = mListAlarmInfomationItems;

                base.Init();

                BindCommands();
              
                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("001", "Loading basic information..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    //触发Loaded消息
                    SendLoadedMessage();

                    InitOperations();
                    LoadBasicDataInfos();
                    LoadViewColumnItems();
                    LoadReceiverColumnItems();
                    LoadAlarmMessageInfos();
                    LoadAlarmInfomationItems();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);

                    CreateAlarmInfoListButtons();
                    CreateAlarmMessageButtons();
                    CreateAlarmReceiverButtons();
                    InitViewColumnItems();
                    InitReceiverColumnItems();
                    InitAlarmMessageItems();
                    InitAlarmReceiverItems();
                    ChangeLanguage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void BindCommands()
        {
            CommandBindings.Add(new CommandBinding(MainPageCommands.EnableCommand, EnableCommand_Executed,
                (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(MainPageCommands.DisableCommand, DisableCommand_Executed,
               (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(MainPageCommands.TreeItemCheckCommand, TreeItemCheckCommand_Executed,
                (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(MainPageCommands.TreeItemDoubleClickCommand, TreeItemDoubleClickCommand_Exeuted,
               (s, e) => e.CanExecute = true));
        }

        private void InitOperations()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("25");
                webRequest.ListData.Add("2501");
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
                ListOperations.Clear();
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
                    }
                }

                App.WriteLog("PageLoad", string.Format("Init Operations"));
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
                webRequest.ListData.Add("250100000");
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
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadViewColumnItems()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("2501001");
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
                mListColumnItems.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListColumnItems.Add(listColumns[i]);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadReceiverColumnItems()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("2501002");
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
                mListReceiverColumnItems.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListReceiverColumnItems.Add(listColumns[i]);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadAlarmMessageInfos()
        {
            try
            {
                //获取告警消息信息
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S2501Codes.GetAlarmMessageList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service25011Client client = new Service25011Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service25011"));
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
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                OperationReturn optReturn;
                List<AlarmMessageInfo> listInfos = new List<AlarmMessageInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<AlarmMessageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    AlarmMessageInfo info = optReturn.Data as AlarmMessageInfo;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("AlarmMessageInfo is null"));
                        return;
                    }
                    listInfos.Add(info);
                }
                listInfos = listInfos.OrderBy(info => info.SerialID).ToList();
                mListAlarmMessageInfos.Clear();
                foreach (var typeParam in listInfos)
                {
                    mListAlarmMessageInfos.Add(typeParam);
                }

                App.WriteLog("PageLoad", string.Format("Init AlarmMessageInfos"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadAlarmReceiverInfos()
        {
            try
            {
                //获取指定告警信息的接收人的信息
                if (mCurrentAlarmInfoItem == null) { return; }
                long alarmInfoID = mCurrentAlarmInfoItem.SerialID;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S2501Codes.GetAlarmReceiverList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add(alarmInfoID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service25011Client client = new Service25011Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service25011"));
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
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                OperationReturn optReturn;
                List<AlarmReceiverInfo> listInfos = new List<AlarmReceiverInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<AlarmReceiverInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    AlarmReceiverInfo info = optReturn.Data as AlarmReceiverInfo;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("AlarmReceiverInfo is null"));
                        return;
                    }
                    listInfos.Add(info);
                }
                listInfos = listInfos.OrderBy(info => info.UserID).ThenBy(info => info.AlarmInfoID).ToList();
                for (int i = 0; i < listInfos.Count; i++)
                {
                    mListAlarmReceiverInfos.Add(listInfos[i]);
                }

                App.WriteLog("LoadData", string.Format("Load AlarmReceiverInfo end"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadAlarmInfomationItems()
        {
            try
            {
                //获取所有告警信息
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S2501Codes.GetAlarmInfoList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                App.MonitorHelper.AddWebRequest(webRequest);
                Service25011Client client = new Service25011Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service25011"));
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
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                OperationReturn optReturn;
                List<AlarmInfomationInfo> listInfos = new List<AlarmInfomationInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<AlarmInfomationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    AlarmInfomationInfo info = optReturn.Data as AlarmInfomationInfo;
                    if (info == null)
                    {
                        App.ShowExceptionMessage(string.Format("AlarmInfomationInfo is null"));
                        return;
                    }
                    listInfos.Add(info);
                }
                listInfos = listInfos.OrderBy(info => info.SerialID).ToList();
                Dispatcher.Invoke(new Action(() => mListAlarmInfomationItems.Clear()));
                for (int i = 0; i < listInfos.Count; i++)
                {
                    var info = listInfos[i];
                    var item = AlarmInfomationItem.CreateItem(info);
                    item.RowNumber = i + 1;
                    item.ListBasicDataInfos = mListBasicDataInfos;
                    item.ListAlarmMessageInfos = mListAlarmMessageInfos;
                    item.InitItem();
                    Dispatcher.Invoke(new Action(() => mListAlarmInfomationItems.Add(item)));
                }

                App.WriteLog("PageLoad", string.Format("Init AlarmInfomationInfos"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitAlarmMessageItems()
        {
            try
            {
                //构造告警消息树
                mListAlarmMessageItems.Clear();
                mAlarmMessageRoot.Children.Clear();
                InitAlarmModuleItems(mAlarmMessageRoot);
                //展开第一节点
                mAlarmMessageRoot.IsExpanded = true;
                if (mAlarmMessageRoot.Children.Count > 0)
                {
                    var child = mAlarmMessageRoot.Children[0] as ObjectItem;
                    if (child != null)
                    {
                        child.IsSelected = true;
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitAlarmModuleItems(ObjectItem parentItem)
        {
            try
            {
                var infos = mListAlarmMessageInfos.Where(i => i.ModuleID != 0 && i.MessageID == 0 && i.StatusID == 0).ToList();
                for (int i = 0; i < infos.Count; i++)
                {
                    var info = infos[i];
                    ObjectItem item = new ObjectItem();
                    item.StartDragged += ObjectItem_StartDragged;
                    item.DragOver += ObjectItem_DragOver;
                    item.Dropped += ObjectItem_Dropped;
                    item.Type = S2501Consts.NODE_MODULE;
                    item.Name = App.GetLanguageInfo(string.Format("2501MSG{0}", info.SerialID), info.Description);
                    item.Description = info.Description;
                    item.Data = info;
                    item.Icon = string.Format("Images/00003.png");
                    InitAlarmMessageItems(item);
                    parentItem.AddChild(item);
                    mListAlarmMessageItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitAlarmMessageItems(ObjectItem parentItem)
        {
            try
            {
                var parentInfo = parentItem.Data as AlarmMessageInfo;
                if (parentInfo == null) { return; }
                int moduleID = parentInfo.ModuleID;
                if (moduleID <= 0) { return; }
                var infos = mListAlarmMessageInfos.Where(i => i.ModuleID == moduleID && i.MessageID != 0 && i.StatusID == 0).ToList();
                for (int i = 0; i < infos.Count; i++)
                {
                    var info = infos[i];
                    ObjectItem item = new ObjectItem();
                    item.StartDragged += ObjectItem_StartDragged;
                    item.DragOver += ObjectItem_DragOver;
                    item.Dropped += ObjectItem_Dropped;
                    item.Type = S2501Consts.NODE_MESSAGE;
                    item.Name = App.GetLanguageInfo(string.Format("2501MSG{0}", info.SerialID), info.Description);
                    item.Description = info.Description;
                    item.Data = info;
                    item.Icon = string.Format("Images/00001.png");
                    InitAlarmStatusItems(item);
                    parentItem.AddChild(item);
                    mListAlarmMessageItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitAlarmStatusItems(ObjectItem parentItem)
        {
            try
            {
                var parentInfo = parentItem.Data as AlarmMessageInfo;
                if (parentInfo == null) { return; }
                int moduleID = parentInfo.ModuleID;
                int messageID = parentInfo.MessageID;
                if (moduleID <= 0 || messageID <= 0) { return; }
                var infos = mListAlarmMessageInfos.Where(i => i.ModuleID == moduleID && i.MessageID == messageID && i.StatusID != 0).ToList();
                for (int i = 0; i < infos.Count; i++)
                {
                    var info = infos[i];
                    ObjectItem item = new ObjectItem();
                    item.StartDragged += ObjectItem_StartDragged;
                    item.DragOver += ObjectItem_DragOver;
                    item.Dropped += ObjectItem_Dropped;
                    item.Type = S2501Consts.NODE_STATUS;
                    item.Name = App.GetLanguageInfo(string.Format("2501MSG{0}", info.SerialID), info.Description);
                    item.Description = info.Description;
                    item.Data = info;
                    item.Icon = string.Format("Images/00002.png");
                    parentItem.AddChild(item);
                    mListAlarmMessageItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitViewColumnItems()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListColumnItems.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListColumnItems[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = App.GetLanguageInfo(string.Format("COL2501001{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = App.GetLanguageInfo(string.Format("COL2501001{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;

                        DataTemplate dt = null;
                        if (columnInfo.ColumnName == "IsEnabled")
                        {
                            dt = Resources["StateCellTemplate"] as DataTemplate;
                        }
                        if (dt == null)
                        {
                            var columName = columnInfo.ColumnName;
                            if (columName == "Type")
                            {
                                columName = "StrType";
                            }
                            if (columName == "Level")
                            {
                                columName = "StrLevel";
                            }
                            if (columName == "Message")
                            {
                                columName = "StrMessage";
                            }
                            if (columName == "CreateTime")
                            {
                                columName = "StrCreateTime";
                            }
                            Binding binding = new Binding(columName);
                            if (columName == "CreateTime")
                            {
                                binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                            }
                            gvc.DisplayMemberBinding = binding;
                        }
                        else
                        {
                            gvc.CellTemplate = dt;
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                LvAlarmInfoList.View = gv;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitReceiverColumnItems()
        {
            try
            {
                ViewColumnInfo column;
                int nameColumnWidth;
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                List<GridViewColumn> listColumns = new List<GridViewColumn>();
                column = mListReceiverColumnItems.FirstOrDefault(c => c.ColumnName == "Name");
                gvch = new GridViewColumnHeader();
                gvch.Content = string.Empty;
                if (column != null)
                {
                    gvch.Content = App.GetLanguageInfo(string.Format("COL2501002{0}", column.ColumnName),
                        column.ColumnName);
                    nameColumnWidth = column.Width;
                }
                else
                {
                    nameColumnWidth = 180;
                }

                column = mListReceiverColumnItems.FirstOrDefault(c => c.ColumnName == "SendMethod");
                if (column != null && column.Visibility == "1")
                {
                    gvc = new GridViewColumn();
                    gvc.Header = App.GetLanguageInfo(string.Format("COL2501002{0}", column.ColumnName),
                        column.ColumnName);
                    gvc.Width = column.Width;
                    DataTemplate sendMethodTemplate = (DataTemplate)Resources["SendMethodTemplate"];
                    if (sendMethodTemplate != null)
                    {
                        gvc.CellTemplate = sendMethodTemplate;
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding();
                    }
                    listColumns.Add(gvc);
                }

                DataTemplate nameColumnTemplate = (DataTemplate)Resources["NameColumnTemplate"];
                if (nameColumnTemplate != null)
                {
                    GridTreeReceiverList.SetColumns(nameColumnTemplate, gvch, nameColumnWidth, listColumns);
                }
                else
                {
                    GridTreeReceiverList.SetColumns(gvch, nameColumnWidth, listColumns);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitAlarmReceiverItems()
        {
            try
            {
                //构造接收人树
                mListAlarmReceiverItems.Clear();
                mAlarmReceiverRoot.Children.Clear();
                InitAlarmReceiverOrg(mAlarmReceiverRoot, "-1");
                mAlarmReceiverRoot.IsExpanded = true;
                if (mAlarmReceiverRoot.Children.Count > 0)
                {
                    var first = mAlarmReceiverRoot.Children[0];
                    first.IsExpanded = true;
                }
                mAlarmReceiverRoot.IsChecked = false;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitAlarmReceiverOrg(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)RequestCode.WSGetUserObjList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_ORG.ToString());
                webRequest.ListData.Add(parentID);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
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
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    ObjectItem item = new ObjectItem();
                    item.Type = S2501Consts.NODE_ORG;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Icon = "Images/00015.ico";
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    InitAlarmReceiverOrg(item, strID);
                    InitAlarmReceiverUser(item, strID);
                    mListAlarmReceiverItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitAlarmReceiverUser(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)RequestCode.WSGetUserObjList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_USER.ToString());
                webRequest.ListData.Add(parentID);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
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
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    ObjectItem item = new ObjectItem();
                    item.Type = S2501Consts.NODE_USER;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Icon = "Images/00016.ico";
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    InitAlarmReceiverOrg(item, strID);
                    mListAlarmReceiverItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void TvAlarmMessageList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = e.NewValue as ObjectItem;
            if (item == null) { return; }
            try
            {
                item.IsExpanded = true;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void LvAlarmInfoList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var items = e.AddedItems;
            if (items == null || items.Count <= 0) { return; }
            var item = items[0] as AlarmInfomationItem;
            if (item == null) { return; }
            mCurrentAlarmInfoItem = item;
            try
            {
                CreateAlarmInfoDetailViewer();
                InitAlarmInfoReceivers();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void LvAlarmInfoList_Drop(object sender, DragEventArgs e)
        {
            try
            {
                //获取拖入的AlarmMessageInfo
                var data = e.Data;
                if (data == null) { return; }
                var obj = data.GetData(typeof(ObjectItem)) as ObjectItem;
                if (obj == null) { return; }
                var message = obj.Data as AlarmMessageInfo;
                if (message == null) { return; }
                AddNewAlarmInfo(message);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

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
                            PopupPanel.ChangeLanguage();
                            SendLanguageChangeMessage();
                        };
                        mWorker.RunWorkerAsync();
                    }
                    break;
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
                case S2501Consts.OPT_RELOAD:
                    ReloadAlarmInfo();
                    break;
                case S2501Consts.OPT_ADDALARMINFO:
                    AddNewAlarmInfo();
                    break;
                case S2501Consts.OPT_REMOVEALARMINFO:
                    RemoveAlarmInfo();
                    break;
                case S2501Consts.OPT_SAVEALARMINFO:
                    SaveAlarmInfo();
                    break;
                case S2501Consts.OPT_ALARMMESSAGERELOAD:
                    ReloadAlarmMessage();
                    break;
                case S2501Consts.OPT_SAVEALARMRECEIVER:
                    SaveAlarmReceiverInfo();
                    break;
                case S2501Consts.OPT_ALARMRECEIVERRELOAD:
                    ReloadAlarmReceiver();
                    break;
            }
        }

        void TreeItemAlarmMessage_MouseDoubleClick(object sender, RoutedPropertyChangedEventArgs<ListItemEventArgs> e)
        {
            var item = TvAlarmMessageList.SelectedItem as ObjectItem;
            if (item == null) { return; }
            var message = item.Data as AlarmMessageInfo;
            if (message == null) { return; }
            //只能使用消息和状态类别下的告警消息
            if (message.MessageID == 0 && message.StatusID == 0) { return; }

            ////2015/09/22注释，应广大人民要求，双击只添加告警信息，不做修改
            //var infoItem = LvAlarmInfoList.SelectedItem as AlarmInfomationItem;
            //if (infoItem == null)
            //{
            //    AddNewAlarmInfo(message);
            //}
            //else
            //{
            //    var temp = mListAlarmInfomationItems.FirstOrDefault(i => i.MessageID == message.SerialID);
            //    if (temp != null)
            //    {
            //        App.ShowExceptionMessage(App.GetMessageLanguageInfo("002", "Alarm message already exist."));
            //        return;
            //    }
            //    if (mCurrentAlarmInfoItem != null)
            //    {
            //        mCurrentAlarmInfoItem.MessageID = message.SerialID;
            //        mCurrentAlarmInfoItem.InitItem();
            //        var detail = BorderAlarmInfoDetail.Child as UCAlarmInfoDetail;
            //        if (detail != null)
            //        {
            //            detail.Reload();
            //        }
            //    }
            //}
            AddNewAlarmInfo(message);
        }

        #endregion


        #region CommandHandlers

        private void EnableCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as AlarmInfomationItem;
            if (item != null)
            {
                item.IsEnabled = true;
                item.SetPropertyDisplay();
            }
        }

        private void DisableCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var item = e.Parameter as AlarmInfomationItem;
            if (item != null)
            {
                item.IsEnabled = false;
                item.SetPropertyDisplay();
            }
        }

        private void TreeItemCheckCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var obj = e.Parameter as ObjectItem;
            if (obj == null) { return; }
            if (obj.Type == S2501Consts.NODE_ORG
                || obj.Type == S2501Consts.NODE_USER)
            {
                //本节点及下级节点都要重新加载一下
                var viewer = obj.SendMothodViewer;
                if (viewer != null) { viewer.Reload(); }
                for (int i = 0; i < obj.Children.Count; i++)
                {
                    var child = obj.Children[i] as ObjectItem;
                    if (child != null)
                    {
                        viewer = child.SendMothodViewer;
                        if (viewer != null)
                        {
                            viewer.Reload();
                        }
                    }
                }
            }
        }

        private void TreeItemDoubleClickCommand_Exeuted(object sender, ExecutedRoutedEventArgs e)
        {
            var param = e.Parameter as ObjectItem;
            if (param != null)
            {
                App.ShowExceptionMessage(param.ToString());
            }
        }

        #endregion


        #region Others

        private void InitAlarmInfoReceivers()
        {
            try
            {
                mListAlarmReceiverInfos.Clear();
                //重置
                for (int i = 0; i < mListAlarmReceiverItems.Count; i++)
                {
                    var item = mListAlarmReceiverItems[i];
                    item.IsChecked = false;
                    item.Data = null;
                    item.OtherData01 = mCurrentAlarmInfoItem;
                    if (item.SendMothodViewer != null)
                    {
                        item.SendMothodViewer.Reload();
                    }
                }
                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("006", "Loading alarm infomation receiver users..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) => LoadAlarmReceiverInfos();
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);

                    try
                    {
                        for (int i = 0; i < mListAlarmReceiverInfos.Count; i++)
                        {
                            var info = mListAlarmReceiverInfos[i];

                            var item = mListAlarmReceiverItems.FirstOrDefault(o => o.ObjID == info.UserID);
                            if (item != null)
                            {
                                item.Data = info;
                                item.IsChecked = true;
                                if (item.SendMothodViewer != null)
                                {
                                    item.SendMothodViewer.Reload();
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        App.ShowExceptionMessage(ex.Message);
                    }
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateAlarmInfoListButtons()
        {
            try
            {
                PanelAlarmInfoButtons.Children.Clear();
                OperationInfo optInfo;
                Button btn;
                //刷新告警信息
                optInfo = new OperationInfo();
                optInfo.ID = S2501Consts.OPT_RELOAD;
                optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), "Reload");
                optInfo.Icon = "Images/00010.png";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = optInfo;
                btn.SetResourceReference(StyleProperty, "HeadOptButtonStyle");
                PanelAlarmInfoButtons.Children.Add(btn);
                //增加告警信息
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S2501Consts.OPT_ADDALARMINFO);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "HeadOptButtonStyle");
                    PanelAlarmInfoButtons.Children.Add(btn);
                }
                //删除告警信息
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S2501Consts.OPT_REMOVEALARMINFO);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "HeadOptButtonStyle");
                    PanelAlarmInfoButtons.Children.Add(btn);
                }
                //保存告警信息
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S2501Consts.OPT_SAVEALARMINFO);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "HeadOptButtonStyle");
                    PanelAlarmInfoButtons.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateAlarmMessageButtons()
        {
            try
            {
                PanelMessageListButtons.Children.Clear();
                OperationInfo optInfo;
                Button btn;
                //刷新告警Message
                optInfo = new OperationInfo();
                optInfo.ID = S2501Consts.OPT_ALARMMESSAGERELOAD;
                optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), "Reload");
                optInfo.Icon = "Images/00010.png";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = optInfo;
                btn.SetResourceReference(StyleProperty, "HeadOptButtonStyle");
                PanelMessageListButtons.Children.Add(btn);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateAlarmReceiverButtons()
        {
            try
            {
                PanelAlarmReceiverButtons.Children.Clear();
                OperationInfo optInfo;
                Button btn;
                //刷新告警接收人
                optInfo = new OperationInfo();
                optInfo.ID = S2501Consts.OPT_ALARMRECEIVERRELOAD;
                optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), "Reload");
                optInfo.Icon = "Images/00010.png";
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = optInfo;
                btn.SetResourceReference(StyleProperty, "HeadOptButtonStyle");
                PanelAlarmReceiverButtons.Children.Add(btn);
                //保存告警接收人
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S2501Consts.OPT_SAVEALARMRECEIVER);
                if (optInfo != null)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = optInfo;
                    btn.SetResourceReference(StyleProperty, "HeadOptButtonStyle");
                    PanelAlarmReceiverButtons.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateAlarmInfoDetailViewer()
        {
            UCAlarmInfoDetail uc = new UCAlarmInfoDetail();
            uc.PageParent = this;
            uc.AlarmInfoItem = mCurrentAlarmInfoItem;
            uc.ListBasicDataInfos = mListBasicDataInfos;
            uc.ListAlarmMessageInfos = mListAlarmMessageInfos;
            uc.ListAllAlarmInfoItems = mListAlarmInfomationItems;
            BorderAlarmInfoDetail.Child = uc;
        }

        private long GetNewAlarmInfoSerialID()
        {
            try
            {
                //获取新的告警信息的流水号
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.Session = App.Session;
                webRequest.ListData.Add("25");
                webRequest.ListData.Add("203");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return -1;
                }
                long id = Convert.ToInt64(webReturn.Data);
                return id;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                return -1;
            }
        }

        private int GetRowNumber()
        {
            int row = 0;
            for (int i = 0; i < mListAlarmInfomationItems.Count; i++)
            {
                row = Math.Max(row, mListAlarmInfomationItems[i].RowNumber);
            }
            row++;
            return row;
        }

        #endregion


        #region ObjectItem DragDrop

        private void ObjectItem_StartDragged(object sender, DragDropEventArgs e)
        {
            var dragSource = e.DragSource;
            //将当前选中的节点作为拖放数据
            var dragData = TvAlarmMessageList.SelectedItem as ObjectItem;
            if (dragSource != null && dragData != null)
            {
                DragDrop.DoDragDrop(dragSource, dragData, DragDropEffects.Copy);
            }
        }

        private void ObjectItem_DragOver(object sender, DragDropEventArgs e)
        {

        }

        private void ObjectItem_Dropped(object sender, DragDropEventArgs e)
        {

        }

        #endregion


        #region Operations

        private void ReloadAlarmInfo()
        {
            try
            {
                //相关对象或列表重置
                mListAlarmInfomationItems.Clear();
                mListRemoveAlarmInfoIDs.Clear();
                mCurrentAlarmInfoItem = null;
                BorderAlarmInfoDetail.Child = null;
                InitAlarmInfoReceivers();
                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("001", "Loading basic information..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) => LoadAlarmInfomationItems();
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void ReloadAlarmMessage()
        {
            try
            {
                //相关对象或列表重置
                mListAlarmMessageInfos.Clear();
                mListAlarmMessageItems.Clear();
                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("001", "Loading basic information..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) => LoadAlarmMessageInfos();
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);

                    InitAlarmMessageItems();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void ReloadAlarmReceiver()
        {
            try
            {
                //相关对象或列表重置
                mListAlarmReceiverInfos.Clear();
                mAlarmReceiverRoot.IsChecked = false;
                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("006", "Loading alarm receiver information..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) => LoadAlarmReceiverInfos();
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);

                    InitAlarmInfoReceivers();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void AddNewAlarmInfo()
        {
            try
            {
                var objItem = TvAlarmMessageList.SelectedItem as ObjectItem;
                if (objItem == null) { return; }
                //只有消息或状态才能作为告警信息的Message
                if (objItem.Type != S2501Consts.NODE_MESSAGE
                    && objItem.Type != S2501Consts.NODE_STATUS) { return; }
                var message = objItem.Data as AlarmMessageInfo;
                AddNewAlarmInfo(message);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void AddNewAlarmInfo(AlarmMessageInfo message)
        {
            try
            {
                if (message == null) { return; }
                //只能添加消息和状态类别下的告警消息
                if (message.MessageID == 0 && message.StatusID == 0) { return; }
                //判断是否有增加告警权限
                var optInfo = ListOperations.FirstOrDefault(o => o.ID == S2501Consts.OPT_ADDALARMINFO);
                if (optInfo == null) { return; }
                //判断是否已经存在
                var temp = mListAlarmInfomationItems.FirstOrDefault(a => a.MessageID == message.SerialID);
                if (temp != null)
                {
                    App.ShowExceptionMessage(App.GetMessageLanguageInfo("002", "Alarm infomation already exist"));
                    return;
                }
                //获取一个新的流水号
                long serialID = GetNewAlarmInfoSerialID();
                if (serialID <= 0) { return; }
                //创建一个新的告警信息
                AlarmInfomationInfo info = new AlarmInfomationInfo();
                info.SerialID = serialID;
                info.Name = message.Description;
                info.MessageID = message.SerialID;
                info.Level = S2501Consts.ALARM_LEVEL_SOURCE_LEVEL;
                info.IsEnabled = true;
                info.Description = info.Name;
                info.CreateTime = DateTime.Now.ToUniversalTime();
                info.Creator = App.Session.UserID;
                info.LastModifyTime = DateTime.Now.ToUniversalTime();
                info.LastModifyUser = App.Session.UserID;
                AlarmInfomationItem item = AlarmInfomationItem.CreateItem(info);
                item.RowNumber = GetRowNumber();
                item.ListAlarmMessageInfos = mListAlarmMessageInfos;
                item.ListBasicDataInfos = mListBasicDataInfos;
                //初始化
                item.InitItem();
                mListAlarmInfomationItems.Add(item);
                LvAlarmInfoList.SelectedItem = item;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void RemoveAlarmInfo()
        {
            try
            {
                var items = LvAlarmInfoList.SelectedItems;
                if (items == null) { return; }
                List<AlarmInfomationItem> listItems = new List<AlarmInfomationItem>();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < items.Count; i++)
                {
                    var item = items[i] as AlarmInfomationItem;
                    if (item == null) { continue; }
                    if (i < 5)
                    {
                        sb.Append(string.Format("{0}({1})\r\n", item.Name, item.SerialID));
                    }
                    if (i == 5)
                    {
                        sb.Append("...");
                    }
                    listItems.Add(item);
                }
                if (listItems.Count <= 0) { return; }
                var result =
                    MessageBox.Show(
                        string.Format("{0}\r\n\r\n{1}",
                            App.GetMessageLanguageInfo("007", "Confirm delete alarm infomation?"), sb), App.AppName,
                        MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }
                for (int i = listItems.Count - 1; i >= 0; i--)
                {
                    var item = listItems[i];
                    mListAlarmInfomationItems.Remove(item);
                    //加入待删除列表
                    if (!mListRemoveAlarmInfoIDs.Contains(item.SerialID))
                    {
                        mListRemoveAlarmInfoIDs.Add(item.SerialID);
                    }
                }
                mCurrentAlarmInfoItem = null;
                BorderAlarmInfoDetail.Child = null;
                InitAlarmInfoReceivers();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void SaveAlarmInfo()
        {
            try
            {
                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("005", "Saving alarm informations..."));
                mIsOptSuccess = true;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    DoSaveAlarmInfo();
                    DoRemoveAlarmInfo();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);
                    if (!mIsOptSuccess)
                    {
                        App.ShowExceptionMessage(App.GetMessageLanguageInfo("004", "Save alarm infomation fail."));
                    }
                    else
                    {
                        App.ShowInfoMessage(App.GetMessageLanguageInfo("003", "Save alarm infomation end."));
                    }
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void SaveAlarmReceiverInfo()
        {
            try
            {
                if (mCurrentAlarmInfoItem == null) { return; }
                long alarmInfoID = mCurrentAlarmInfoItem.SerialID;
                List<AlarmReceiverInfo> listInfos = new List<AlarmReceiverInfo>();
                for (int i = 0; i < mListAlarmReceiverItems.Count; i++)
                {
                    var item = mListAlarmReceiverItems[i];
                    if (item.IsChecked != true) { continue; }
                    if (item.Type != S2501Consts.NODE_USER) { continue; }
                    var info = item.Data as AlarmReceiverInfo;
                    if (info == null)
                    {
                        info = new AlarmReceiverInfo();
                        info.UserID = item.ObjID;
                        info.AlarmInfoID = alarmInfoID;
                        info.TenantID = 0;
                        info.TenantToken = "00000";
                        info.Method = 0;
                        info.ReplyMode = 0;
                    }
                    item.Data = info;
                    listInfos.Add(info);
                }
                mListAlarmReceiverInfos.Clear();
                for (int i = 0; i < listInfos.Count; i++)
                {
                    mListAlarmReceiverInfos.Add(listInfos[i]);
                }
                MyWaiter.Visibility = Visibility.Visible;
                SetStatuMessage(App.GetMessageLanguageInfo("005", "Saving alarm infomation..."));
                mIsOptSuccess = true;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    DoSaveAlarmInfo();
                    DoSaveAlarmReceiverInfo();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Hidden;
                    SetStatuMessage(string.Empty);

                    if (!mIsOptSuccess)
                    {
                        App.ShowExceptionMessage(App.GetMessageLanguageInfo("004", "Save alarm info fail."));
                    }
                    else
                    {
                        App.ShowInfoMessage(App.GetMessageLanguageInfo("003", "Save alarm info end."));
                    }
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void DoSaveAlarmInfo()
        {
            try
            {
                if (!mIsOptSuccess) { return; }
                int count = mListAlarmInfomationItems.Count;
                if (count <= 0) { return; }
                OperationReturn optReturn;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S2501Codes.SaveAlarmInfoList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    var item = mListAlarmInfomationItems[i];
                    item.UpdateInfo();
                    var info = item.Info;
                    if (info == null) { return; }
                    optReturn = XMLHelper.SeriallizeObject(info);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        mIsOptSuccess = false;
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                App.MonitorHelper.AddWebRequest(webRequest);
                Service25011Client client = new Service25011Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service25011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    mIsOptSuccess = false;
                    return;
                }

                App.WriteLog("SaveAlarm", "Do Save alarm information end");
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                mIsOptSuccess = false;
            }
        }

        private void DoSaveAlarmReceiverInfo()
        {
            try
            {
                if (!mIsOptSuccess) { return; }
                var alarmInfoItem = mCurrentAlarmInfoItem;
                if (alarmInfoItem == null) { return; }
                int count = mListAlarmReceiverInfos.Count;
                if (count <= 0) { return; }
                OperationReturn optReturn;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S2501Codes.SaveAlarmReceiverList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add(alarmInfoItem.SerialID.ToString());
                webRequest.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    var info = mListAlarmReceiverInfos[i];
                    optReturn = XMLHelper.SeriallizeObject(info);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        mIsOptSuccess = false;
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                App.MonitorHelper.AddWebRequest(webRequest);
                Service25011Client client = new Service25011Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service25011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    mIsOptSuccess = false;
                    return;
                }

                App.WriteLog("SaveReceiver", "Do Save alarm receiver end");
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                mIsOptSuccess = false;
            }
        }

        private void DoRemoveAlarmInfo()
        {
            try
            {
                if (!mIsOptSuccess) { return; }
                List<long> listIDs = mListRemoveAlarmInfoIDs;
                for (int i = 0; i < mListAlarmInfomationItems.Count; i++)
                {
                    var id = mListAlarmInfomationItems[i].SerialID;
                    if (listIDs.Contains(id))
                    {
                        listIDs.Remove(id);
                    }
                }
                int count = listIDs.Count;
                if (count <= 0) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S2501Codes.RemoveAlarmInfoList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    webRequest.ListData.Add(listIDs[i].ToString());
                }
                App.MonitorHelper.AddWebRequest(webRequest);
                Service25011Client client = new Service25011Client(
                    WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(
                        App.Session.AppServerInfo,
                        "Service25011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    mIsOptSuccess = false;
                    return;
                }
                mListRemoveAlarmInfoIDs.Clear();

                App.WriteLog("SaveAlarm", "Do remove alarm information end");
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                mIsOptSuccess = false;
            }
        }

        #endregion


        #region ChangedLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                if (PageHead != null)
                {
                    PageHead.AppName = App.GetLanguageInfo("2501003", "UMP Alarm Information Management");
                }

                LbAlarmMessageList.Text = App.GetLanguageInfo("2501101", "Alarm Message List");
                LbAlarmInfoList.Text = App.GetLanguageInfo("2501102", "Alarm Infomation List");
                LbAlarmInfoDetail.Text = App.GetLanguageInfo("2501103", "Alarm Infomation Detail");
                LbReceiverList.Text = App.GetLanguageInfo("2501104", "Alarm Receiver List");

                //列标题
                InitViewColumnItems();
                InitReceiverColumnItems();

                //操作
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    var opt = ListOperations[i];
                    opt.Display = App.GetLanguageInfo(string.Format("FO{0}", opt.ID), opt.ID.ToString());
                }
                CreateAlarmInfoListButtons();

                //告警消息
                for (int i = 0; i < mListAlarmMessageItems.Count; i++)
                {
                    var item = mListAlarmMessageItems[i];
                    var info = item.Data as AlarmMessageInfo;
                    if (info != null)
                    {
                        item.Name = App.GetLanguageInfo(string.Format("2501MSG{0}", info.SerialID), info.Description);
                    }
                }

                //告警信息
                for (int i = 0; i < mListAlarmInfomationItems.Count; i++)
                {
                    mListAlarmInfomationItems[i].SetPropertyDisplay();
                }

                var detail = BorderAlarmInfoDetail.Child as ILanguagePage;
                if (detail != null)
                {
                    detail.ChangeLanguage();
                }

                //告警接收人
                for (int i = 0; i < mListAlarmReceiverItems.Count; i++)
                {
                    var item = mListAlarmReceiverItems[i];
                    if (item.SendMothodViewer != null)
                    {
                        item.SendMothodViewer.ChangeLanguage();
                    }
                }

                var popup = PopupPanel.Content as ILanguagePage;
                if (popup != null)
                {
                    popup.ChangeLanguage();
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("Lang", ex.Message);
            }
        }

        #endregion


        #region ChangTheme

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

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/Themes/Default/UMPS2501/MainPageStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage("3" + ex.Message);
            }
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
