//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    699c0aca-c4f1-4106-8378-230fecaa3b13
//        CLR Version:              4.0.30319.18444
//        Name:                     OUMMainPage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101
//        File Name:                OUMMainPage
//
//        created by Charley at 2014/8/25 16:40:59
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
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UMPS1101.Commands;
using UMPS1101.Models;
using UMPS1101.Wcf11011;
using UMPS1101.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;
using System.Reflection;
using System.Diagnostics;
using System.Data.OleDb;
using System.Data;
using System.IO;
using UMPS1101.WCFService00000;

namespace UMPS1101
{
    /// <summary>
    /// OUMMainPage.xaml 的交互逻辑
    /// </summary>
    public partial class OUMMainView
    {

        #region Members

        private ObjectItem mRoot;
        private List<ObjectItem> mListObjectItems;
        private ObservableCollection<OperationInfo> mListBasicOperations;
        private ObservableCollection<ViewColumnInfo> mListGridTreeColumns;
        private ObservableCollection<OrgTypeInfo> mListOrgTypeInfos;
        private BackgroundWorker mWorker;
        private SelectedInfo mSelectInfo;
        private double mViewerScale;
        private ObjectItem mCurrentObjectItem;
        NPOIHelper NPOIhelper;
        bool Is2007 = false;
        private S1101App S1101App;
        //px
        public string InitParameter = "A";
        DataSet Dataset;
        private string ImportWhat = string.Empty;
        int ExcelType = 2007;

        #endregion


        #region StaticMembers

        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();

        #endregion


        public OUMMainView()
        {
            InitializeComponent();

            mViewerScale = 1;
            mRoot = new ObjectItem();
            mListObjectItems = new List<ObjectItem>();
            mListBasicOperations = new ObservableCollection<OperationInfo>();
            mListGridTreeColumns = new ObservableCollection<ViewColumnInfo>();
            mListOrgTypeInfos = new ObservableCollection<OrgTypeInfo>();
            //Loaded += OUMMainPage_Loaded;
            //App.NetPipeEvent += App_NetPipeEvent;
            //PageHead.PageHeadEvent += PageHead_PageHeadEvent;
            TvObjects.ItemsSource = mRoot.Children;
            TvObjects.SelectedItemChanged += TreeView_SelectedItemChanged;
            TvSample.ItemsSource = mRoot.Children;
            TvSample.SelectedItemChanged += TreeView_SelectedItemChanged;
            TvDiagram.ItemsSource = mRoot.Children;
            TvDiagram.SelectedItemChanged += TreeView_SelectedItemChanged;
            TabControlView.SelectionChanged += TabControlView_SelectionChanged;
            SliderScale.ValueChanged += SliderScale_ValueChanged;
            //App.NetPipeEvent += App_NetPipeEvent;
            TvSample.MouseRightButtonDown += TvSample_MouseRightButtonDown;
        }

        void TvSample_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (mSelectInfo == null)
            { return; }
            else
            {
                PopupPanel.Title = CurrentApp.GetLanguageInfo("1101T10221", "Batch Move");
                UCBatchMove BatchMove = new UCBatchMove();
                BatchMove.mSelectInfo = mSelectInfo;
                BatchMove.ParentPage = this;
                BatchMove.CurrentApp = CurrentApp;
                //orgInfoModify.ObjParent = parent;
                PopupPanel.Content = BatchMove;
                PopupPanel.IsOpen = true;

            }
        }

        protected override void Init()
        {
            try
            {
                PageName = "OUMMainView";
                StylePath = "UMPS1101/OUMMainPage.xaml";
                base.Init();
                if (CurrentApp != null)
                {
                    S1101App = CurrentApp as S1101App;
                }
                else
                {
                    S1101App = new S1101App(false);
                }
                ChangeTheme();
                ChangeLanguage();
                BindCommands();
                CreateOptButtons();
                Initparameter();

                //触发Loaded消息
                CurrentApp.SendLoadedMessage();

                SetBusy(true, string.Empty);
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    InitOperations();
                    InitOrgType();
                    InitColumnData();
                    InitControledOrgs();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);

                    InitColumns();
                    if (mRoot != null)
                    {
                        mRoot.IsChecked = false;
                    }
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region Inits

        private void BindCommands()
        {
            CommandBindings.Add(
                new CommandBinding(OUMMainPageCommands.AddOrgCommand,
                    AddOrgCommand_Executed,
                    (s, e) => e.CanExecute = true));
            CommandBindings.Add(
               new CommandBinding(OUMMainPageCommands.DeleteOrgCommand,
                   DeleteOrgCommand_Executed,
                   (s, e) => e.CanExecute = true));
            CommandBindings.Add(
               new CommandBinding(OUMMainPageCommands.ModifyOrgCommand,
                   ModifyOrgCommand_Executed,
                   (s, e) => e.CanExecute = true));
            CommandBindings.Add(
             new CommandBinding(OUMMainPageCommands.AddUserCommand,
                 AddUserCommand_Executed,
                 (s, e) => e.CanExecute = true));
            CommandBindings.Add(
               new CommandBinding(OUMMainPageCommands.DeleteUserCommand,
                   DeleteUserCommand_Executed,
                   (s, e) => e.CanExecute = true));
            CommandBindings.Add(
               new CommandBinding(OUMMainPageCommands.ModifyUserCommand,
                   ModifyUserCommand_Executed,
                   (s, e) => e.CanExecute = true));
            CommandBindings.Add(
             new CommandBinding(OUMMainPageCommands.SetUserRoleCommand,
                 SetUserRoleCommand_Executed,
                 (s, e) => e.CanExecute = true));
            CommandBindings.Add(
             new CommandBinding(OUMMainPageCommands.SetUserManagementCommand,
                 SetUserManagementCommand_Executed,
                 (s, e) => e.CanExecute = true));
            CommandBindings.Add(
             new CommandBinding(OUMMainPageCommands.SetUserResourceManagementCommand,
                 SetUserResourceManagementCommand_Executed,
                 (s, e) => e.CanExecute = true));
            CommandBindings.Add(
             new CommandBinding(OUMMainPageCommands.ImportUserDataCommand,
                 ImportUserDataCommand_Executed,
                 (s, e) => e.CanExecute = true));
            CommandBindings.Add(
             new CommandBinding(OUMMainPageCommands.ImportAgentDataCommand,
                 ImportAgentDataCommand_Executed,
                 (s, e) => e.CanExecute = true));
            CommandBindings.Add(
            new CommandBinding(OUMMainPageCommands.LDAPCommand,
                LDAPCommand_Executed,
                (s, e) => e.CanExecute = true));
        }

        private void InitControledOrgs()
        {
            ClearChildren(mRoot);
            mListObjectItems.Clear();
            InitControledOrgs(mRoot, "-1");
            //展开到下一级
            mRoot.IsExpanded = true;
            if (mRoot.Children.Count > 0)
            {
                for (int i = 0; i < mRoot.Children.Count; i++)
                {
                    mRoot.Children[i].IsExpanded = true;
                }
                var currentItem = mRoot.Children[0] as ObjectItem;
                if (currentItem != null)
                {
                    currentItem.IsSingleSelected = true;
                    mCurrentObjectItem = currentItem;
                }
            }

            CurrentApp.WriteLog("PageInit", string.Format("Init OrgAndUser"));
        }

        private void InitControledOrgs(ObjectItem parentObj, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1101Codes.GetOrganizationList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitControledOrgs Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ObjectItem> listChild = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicOrgInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitControledOrgs Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicOrgInfo orgInfo = optReturn.Data as BasicOrgInfo;
                    if (orgInfo != null)
                    {
                        ObjectItem item = new ObjectItem();
                        item.StartDragged += ObjectItem_StartDragged;
                        item.DragOver += ObjectItem_DragOver;
                        item.Dropped += ObjectItem_Dropped;
                        item.ObjType = S1101Consts.OBJTYPE_ORG;
                        item.ObjID = orgInfo.OrgID;
                        orgInfo.OrgName = orgInfo.OrgName;
                        orgInfo.StrStartTime = DateTime.Parse(orgInfo.StrStartTime).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
                        orgInfo.StrEndTime = orgInfo.StrEndTime;
                        if (!orgInfo.StrEndTime.ToUpper().Equals("UNLIMITED"))
                        {
                            orgInfo.EndTime = DateTime.Parse(orgInfo.StrEndTime).ToLocalTime();
                            orgInfo.StrEndTime = DateTime.Parse(orgInfo.StrEndTime).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
                        }
                        orgInfo.CreateTime = orgInfo.CreateTime.ToLocalTime();
                        item.Name = orgInfo.OrgName;
                        item.FullName = item.Name;
                        item.Description = orgInfo.Description;
                        item.State = Convert.ToInt32(orgInfo.IsActived);
                        item.ObjParentID = orgInfo.ParentID;
                        if (item.State == 1)
                        {
                            item.TipState = CurrentApp.GetLanguageInfo("1101T10109", "IsActived");
                        }
                        else
                        {
                            item.TipState = CurrentApp.GetLanguageInfo("1101T10110", "Disable");
                        }
                        if (item.ObjID == ConstValue.ORG_ROOT)
                        {
                            item.Icon = "Images/root.ico";
                        }
                        else
                        {
                            item.Icon = "Images/org.ico";
                        }
                        item.TipAddOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDORG), "AddOrg");
                        item.TipRemoveOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEORG),
                            "RemoveOrg");
                        item.TipModifyOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYORG),
                            "ModifyOrg");
                        item.TipAddUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDUSER), "AddUser");
                        item.TipRemoveUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEUSER),
                            "RemoveUser");
                        item.TipModifyUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYUSER),
                            "ModifyUser");
                        item.TipSetUserRole = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERROLE),
                            "SetUserRole");
                        item.TipSetUserManagement =
                            CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERMANAGEMENT),
                                "SetUserManagement");
                        item.Data = orgInfo;
                        InitControledOrgs(item, orgInfo.OrgID.ToString());
                        InitControledUsers(item, orgInfo.OrgID.ToString());

                        //px
                        if (InitParameter.Contains("A"))
                            InitControlAgents(item, orgInfo.OrgID.ToString());
                        if (InitParameter.Contains("E"))
                            InitControlExtension(item, orgInfo.OrgID.ToString());
                        if (InitParameter.Contains("R"))
                            InitControlRealExtension(item, orgInfo.OrgID.ToString());

                        //px-end

                        listChild.Add(item);
                    }
                }
                listChild = listChild.OrderBy(o => o.Name).ToList();
                for (int i = 0; i < listChild.Count; i++)
                {
                    mListObjectItems.Add(listChild[i]);
                    AddChildObjectItem(parentObj, listChild[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControledUsers(ObjectItem parentObj, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1101Codes.GetUserList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitControledUsers Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ObjectItem> listChild = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicUserInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitControledUsers Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicUserInfo userInfo = optReturn.Data as BasicUserInfo;
                    if (userInfo != null)
                    {
                        ObjectItem item = new ObjectItem();

                        item.StartDragged += ObjectItem_StartDragged;
                        item.DragOver += ObjectItem_DragOver;
                        item.Dropped += ObjectItem_Dropped;
                        item.ObjType = S1101Consts.OBJTYPE_USER;
                        item.ObjID = userInfo.UserID;
                        if (userInfo.SourceFlag == "L")
                            userInfo.Account = userInfo.Account.Replace("@", @"\");
                        else
                            userInfo.Account = userInfo.Account;
                        userInfo.FullName = userInfo.FullName;
                        if (userInfo.IsOrgManagement!=null)
                        userInfo.IsOrgManagement = userInfo.IsOrgManagement;
                        userInfo.StrStartTime = userInfo.StrStartTime;
                        userInfo.StartTime = DateTime.Parse(userInfo.StrStartTime).ToLocalTime();
                        userInfo.StrEndTime = userInfo.StrEndTime;
                        if (!userInfo.StrEndTime.ToUpper().Equals("UNLIMITED"))
                        {
                            userInfo.EndTime = DateTime.Parse(userInfo.StrEndTime).ToLocalTime();
                        }
                        userInfo.StrCreateTime = userInfo.StrCreateTime;
                        userInfo.CreateTime = Convert.ToDateTime(userInfo.StrCreateTime).ToLocalTime();
                        item.Name = userInfo.Account;
                        item.FullName = userInfo.FullName;
                        item.LockMethod = userInfo.LockMethod;
                        item.State = Convert.ToInt32(userInfo.IsActived);
                        item.ObjParentID = userInfo.OrgID;
                        if (userInfo.IsLocked != "0")
                        {
                            if (userInfo.LockMethod == "U")
                            {
                                item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10115", "Interface Lock");
                            }
                            else if (userInfo.LockMethod == "L")
                            {
                                item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10116", "Safe Lock");
                            }
                            else if (userInfo.LockMethod == "D")
                            {
                                item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10117", "Time Lock");
                            }
                            else
                            {
                                item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10111", "Other");
                            }
                        }
                        if (item.State == 1)
                        {
                            item.TipState = CurrentApp.GetLanguageInfo("1101T10109", "IsActived");
                        }
                        else
                        {
                            item.TipState = CurrentApp.GetLanguageInfo("1101T10110", "Disable");
                        }
                        item.Icon = "Images/user.ico";
                        item.TipAddOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDORG), "AddOrg");
                        item.TipRemoveOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEORG),
                            "RemoveOrg");
                        item.TipModifyOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYORG),
                            "ModifyOrg");
                        item.TipAddUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDUSER), "AddUser");
                        item.TipRemoveUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEUSER),
                            "RemoveUser");
                        item.TipModifyUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYUSER),
                            "ModifyUser");
                        item.TipSetUserRole = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERROLE),
                            "SetUserRole");
                        item.TipSetUserManagement =
                            CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERMANAGEMENT),
                                "SetUserManagement");
                        item.Data = userInfo;
                        listChild.Add(item);
                    }
                }
                listChild = listChild.OrderBy(o => o.Name).ToList();
                for (int i = 0; i < listChild.Count; i++)
                {
                    mListObjectItems.Add(listChild[i]);
                    AddChildObjectItem(parentObj, listChild[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //px
        private void InitControlAgents(ObjectItem parentObj, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                //Service11011Client client = new Service11011Client();
                Service11011Client client = new Service11011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitControlAgents Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                List<ObjectItem> listChild = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicUserInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitControlAgents Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicUserInfo AgentInfo = optReturn.Data as BasicUserInfo;

                    ObjectItem item = new ObjectItem();

                    item.StartDragged += ObjectItem_StartDragged;
                    item.DragOver += ObjectItem_DragOver;
                    item.Dropped += ObjectItem_Dropped;
                    item.ObjType = S1101Consts.OBJTYPE_AGENT;
                    item.ObjID = AgentInfo.UserID;
                    if (!AgentInfo.StrStartTime.Equals(""))
                    {
                        AgentInfo.StartTime = DateTime.Parse(AgentInfo.StrEndTime).ToLocalTime();
                    }
                    if (!AgentInfo.StrEndTime.Equals(""))
                    {
                        AgentInfo.EndTime = DateTime.Parse(AgentInfo.StrEndTime).ToLocalTime();
                    }
                    //userInfo.StrCreateTime = userInfo.StrCreateTime;
                    //userInfo.CreateTime = Convert.ToDateTime(userInfo.StrCreateTime).ToLocalTime();
                    item.Name = AgentInfo.Account;
                    item.FullName = AgentInfo.FullName;
                    item.LockMethod = AgentInfo.LockMethod;
                    //item.State = Convert.ToInt32(AgentInfo.IsActived);
                    int IsAct = 1;
                    item.State = int.TryParse(AgentInfo.IsActived, out IsAct) ? IsAct : 1;
                    item.ObjParentID = AgentInfo.OrgID;
                    if (AgentInfo.IsLocked != "0")
                    {
                        //分类锁定方式
                        //if (AgentInfo.LockMethod == "U")
                        //{
                        //    item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10115", "Interface Lock");
                        //}
                        //else if (AgentInfo.LockMethod == "L")
                        //{
                        //    item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10116", "Safe Lock");
                        //}
                        //else
                        //{
                        //    item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10111", "Other");
                        //}
                    }
                    if (item.State == 1)
                    {
                        item.TipState = CurrentApp.GetLanguageInfo("1101T10109", "IsActived");
                    }
                    else
                    {
                        item.TipState = CurrentApp.GetLanguageInfo("1101T10110", "Disable");
                    }
                    item.Icon = "Images/agent.ico";
                    //item.TipAddOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDORG), "AddOrg");
                    //item.TipRemoveOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEORG),
                    //    "RemoveOrg");
                    //item.TipModifyOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYORG),
                    //    "ModifyOrg");
                    //item.TipAddUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDUSER), "AddUser");
                    //item.TipRemoveUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEUSER),
                    //    "RemoveUser");
                    //item.TipModifyUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYUSER),
                    //    "ModifyUser");
                    //item.TipSetUserRole = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERROLE),
                    //    "SetUserRole");
                    //item.TipSetUserManagement =
                    //    CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERMANAGEMENT),
                    //        "SetUserManagement");
                    item.Data = AgentInfo;
                    listChild.Add(item);
                }
                listChild = listChild.OrderBy(o => o.Name).ToList();
                for (int i = 0; i < listChild.Count; i++)
                {
                    mListObjectItems.Add(listChild[i]);
                    AddChildObjectItem(parentObj, listChild[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlExtension(ObjectItem parentObj, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.GetControlExtensionInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                //Service11011Client client = new Service11011Client();
                Service11011Client client = new Service11011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitControlExtension 1 Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                List<ObjectItem> listChild = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicUserInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitControlExtension Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicUserInfo ExtensionInfo = optReturn.Data as BasicUserInfo;
                    if (ExtensionInfo.IsDeleted == "1") { continue; }
                    ObjectItem item = new ObjectItem();
                    item.StartDragged += ObjectItem_StartDragged;
                    item.DragOver += ObjectItem_DragOver;
                    item.Dropped += ObjectItem_Dropped;
                    item.ObjType = S1101Consts.OBJTYPE_EXTENSION;
                    item.ObjID = ExtensionInfo.UserID;
                    if (ExtensionInfo.StrStartTime != "" && ExtensionInfo.StrStartTime != null)
                    {
                        ExtensionInfo.StartTime = DateTime.Parse(ExtensionInfo.StrEndTime).ToLocalTime();
                    }
                    if (ExtensionInfo.StrEndTime != "" && ExtensionInfo.StrEndTime != null)
                    {
                        ExtensionInfo.EndTime = DateTime.Parse(ExtensionInfo.StrEndTime).ToLocalTime();
                    }
                    //userInfo.StrCreateTime = userInfo.StrCreateTime;
                    //userInfo.CreateTime = Convert.ToDateTime(userInfo.StrCreateTime).ToLocalTime();
                    item.Name = ExtensionInfo.Account;
                    item.FullName = ExtensionInfo.FullName;
                    item.LockMethod = ExtensionInfo.LockMethod;
                    if (ExtensionInfo.IsActived != "")
                        item.State = Convert.ToInt32(ExtensionInfo.IsActived);
                    item.ObjParentID = ExtensionInfo.OrgID;
                    if (ExtensionInfo.IsLocked != "0")
                    {
                        //分类锁定方式

                        //if (AgentInfo.LockMethod == "U")
                        //{
                        //    item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10115", "Interface Lock");
                        //}
                        //else if (AgentInfo.LockMethod == "L")
                        //{
                        //    item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10116", "Safe Lock");
                        //}
                        //else
                        //{
                        //    item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10111", "Other");
                        //}
                    }
                    if (item.State == 1)
                    {
                        item.TipState = CurrentApp.GetLanguageInfo("1101T10109", "IsActived");
                    }
                    else
                    {
                        item.TipState = CurrentApp.GetLanguageInfo("1101T10110", "Disable");
                    }
                    item.Icon = "Images/extension.png";
                    //item.TipAddOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDORG), "AddOrg");
                    //item.TipRemoveOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEORG),
                    //    "RemoveOrg");
                    //item.TipModifyOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYORG),
                    //    "ModifyOrg");
                    //item.TipAddUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDUSER), "AddUser");
                    //item.TipRemoveUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEUSER),
                    //    "RemoveUser");
                    //item.TipModifyUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYUSER),
                    //    "ModifyUser");
                    //item.TipSetUserRole = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERROLE),
                    //    "SetUserRole");
                    //item.TipSetUserManagement =
                    //    CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERMANAGEMENT),
                    //        "SetUserManagement");
                    item.Data = ExtensionInfo;
                    listChild.Add(item);
                }
                listChild = listChild.OrderBy(o => o.Name).ToList();
                for (int i = 0; i < listChild.Count; i++)
                {
                    mListObjectItems.Add(listChild[i]);
                    AddChildObjectItem(parentObj, listChild[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlRealExtension(ObjectItem parentObj, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.GetControlRealExtensionInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                //Service11011Client client = new Service11011Client();
                Service11011Client client = new Service11011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitControlRealExtension 1 Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                List<ObjectItem> listChild = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicUserInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitControlExtension Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicUserInfo ExtensionInfo = optReturn.Data as BasicUserInfo;
                    if (ExtensionInfo.IsDeleted == "1") { continue; }
                    ObjectItem item = new ObjectItem();
                    item.StartDragged += ObjectItem_StartDragged;
                    item.DragOver += ObjectItem_DragOver;
                    item.Dropped += ObjectItem_Dropped;
                    item.ObjType = S1101Consts.OBJTYPE_REALEXTENSION;
                    item.ObjID = ExtensionInfo.UserID;
                    if (ExtensionInfo.StrStartTime != "" && ExtensionInfo.StrStartTime != null)
                    {
                        ExtensionInfo.StartTime = DateTime.Parse(ExtensionInfo.StrEndTime).ToLocalTime();
                    }
                    if (ExtensionInfo.StrEndTime != "" && ExtensionInfo.StrEndTime != null)
                    {
                        ExtensionInfo.EndTime = DateTime.Parse(ExtensionInfo.StrEndTime).ToLocalTime();
                    }
                    //userInfo.StrCreateTime = userInfo.StrCreateTime;
                    //userInfo.CreateTime = Convert.ToDateTime(userInfo.StrCreateTime).ToLocalTime();
                    item.Name = ExtensionInfo.Account;
                    item.FullName = ExtensionInfo.FullName;
                    item.LockMethod = ExtensionInfo.LockMethod;
                    if (ExtensionInfo.IsActived != "")
                        item.State = Convert.ToInt32(ExtensionInfo.IsActived);
                    item.ObjParentID = ExtensionInfo.OrgID;
                    if (ExtensionInfo.IsLocked != "0")
                    {
                        //分类锁定方式

                        //if (AgentInfo.LockMethod == "U")
                        //{
                        //    item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10115", "Interface Lock");
                        //}
                        //else if (AgentInfo.LockMethod == "L")
                        //{
                        //    item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10116", "Safe Lock");
                        //}
                        //else
                        //{
                        //    item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10111", "Other");
                        //}
                    }
                    if (item.State == 1)
                    {
                        item.TipState = CurrentApp.GetLanguageInfo("1101T10109", "IsActived");
                    }
                    else
                    {
                        item.TipState = CurrentApp.GetLanguageInfo("1101T10110", "Disable");
                    }
                    item.Icon = "Images/RealExtension.ico";

                    //item.TipAddOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDORG), "AddOrg");
                    //item.TipRemoveOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEORG),
                    //    "RemoveOrg");
                    //item.TipModifyOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYORG),
                    //    "ModifyOrg");
                    //item.TipAddUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDUSER), "AddUser");
                    //item.TipRemoveUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEUSER),
                    //    "RemoveUser");
                    //item.TipModifyUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYUSER),
                    //    "ModifyUser");
                    //item.TipSetUserRole = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERROLE),
                    //    "SetUserRole");
                    //item.TipSetUserManagement =
                    //    CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERMANAGEMENT),
                    //        "SetUserManagement");
                    item.Data = ExtensionInfo;
                    listChild.Add(item);
                }
                listChild = listChild.OrderBy(o => o.Name).ToList();
                for (int i = 0; i < listChild.Count; i++)
                {
                    mListObjectItems.Add(listChild[i]);
                    AddChildObjectItem(parentObj, listChild[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        //px-end

        private void InitColumnData()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1101001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitColumnData Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("No columns"));
                    return;
                }
                mListGridTreeColumns.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitColumnData Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo column = optReturn.Data as ViewColumnInfo;
                    if (column != null) { mListGridTreeColumns.Add(column); }
                }

                CurrentApp.WriteLog("PageInit", string.Format("Init ViewColumn"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitColumns()
        {
            try
            {
                ViewColumnInfo column;
                int nameColumnWidth;
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                List<GridViewColumn> listColumns = new List<GridViewColumn>();
                column = mListGridTreeColumns.FirstOrDefault(c => c.ColumnName == "Name");
                gvch = new GridViewColumnHeader();
                gvch.Content = string.Empty;
                if (column != null)
                {
                    nameColumnWidth = column.Width;
                }
                else
                {
                    nameColumnWidth = 250;
                }

                gvc = new GridViewColumn();
                gvc.Header = string.Empty;
                gvc.Width = 25;
                DataTemplate checkTemplate = (DataTemplate)Resources["CheckCellTemplate"];
                if (checkTemplate != null)
                {
                    gvc.CellTemplate = checkTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("IsChecked");
                }
                listColumns.Add(gvc);

                //column = mListGridTreeColumns.FirstOrDefault(c => c.ColumnName == "FullName");
                //if (column != null && column.Visibility == "1")
                //{
                //    gvc = new GridViewColumn();
                //    gvc.Header = CurrentApp.GetLanguageInfo("COL110100102", "Full Name");
                //    gvc.Width = column.Width;
                //    DataTemplate fullNameTemplate = (DataTemplate)Resources["FullNameCellTemplate"];
                //    if (fullNameTemplate != null)
                //    {
                //        gvc.CellTemplate = fullNameTemplate;
                //    }
                //    else
                //    {
                //        gvc.DisplayMemberBinding = new Binding("FullName");
                //    }
                //    listColumns.Add(gvc);
                //}
                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL110100104", "State");
                gvc.Width = 80;
                DataTemplate objectStateTemplate = (DataTemplate)Resources["ObjectStateCellTemplate"];
                if (objectStateTemplate != null)
                {
                    gvc.CellTemplate = objectStateTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("State");
                }
                listColumns.Add(gvc);


                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL110100102", "Full Name");
                gvc.Width = 250;
                DataTemplate fullNameTemplate = (DataTemplate)Resources["FullNameCellTemplate"];
                if (fullNameTemplate != null)
                {
                    gvc.CellTemplate = fullNameTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("FullName");
                }
                listColumns.Add(gvc);


                //column = mListGridTreeColumns.FirstOrDefault(c => c.ColumnName == "Lock");
                //if (column != null && column.Visibility == "1")
                //{
                //    gvc = new GridViewColumn();
                //    gvc.Header = CurrentApp.GetLanguageInfo("COL110100103", "Lock");
                //    gvc.Width = column.Width;
                //    DataTemplate lockMethodTemplate = (DataTemplate)Resources["LockMethodCellTemplate"];
                //    if (lockMethodTemplate != null)
                //    {
                //        gvc.CellTemplate = lockMethodTemplate;
                //    }
                //    else
                //    {
                //        gvc.DisplayMemberBinding = new Binding("LockMethod");
                //    }
                //    listColumns.Add(gvc);
                //}

                //kong

                //column = mListGridTreeColumns.FirstOrDefault(c => c.ColumnName == "Description");
                //if (column != null && column.Visibility == "1")
                //{
                //    gvc = new GridViewColumn();
                //    gvc.Header = CurrentApp.GetLanguageInfo("COL110100105", "Description");
                //    gvc.Width = column.Width;
                //    DataTemplate descriptionTemplate = (DataTemplate)Resources["DescriptionCellTemplate"];
                //    if (descriptionTemplate != null)
                //    {
                //        gvc.CellTemplate = descriptionTemplate;
                //    }
                //    else
                //    {
                //        gvc.DisplayMemberBinding = new Binding("Description");
                //    }
                //    listColumns.Add(gvc);
                //}
                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL110100105", "Description");
                gvc.Width = 300;
                DataTemplate descriptionTemplate = (DataTemplate)Resources["DescriptionCellTemplate"];
                if (descriptionTemplate != null)
                {
                    gvc.CellTemplate = descriptionTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("Description");
                }
                listColumns.Add(gvc);


                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL110100103", "Lock");
                gvc.Width = 80;
                DataTemplate lockMethodTemplate = (DataTemplate)Resources["LockMethodCellTemplate"];
                if (lockMethodTemplate != null)
                {
                    gvc.CellTemplate = lockMethodTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("LockMethod");
                }
                listColumns.Add(gvc);

                //column = mListGridTreeColumns.FirstOrDefault(c => c.ColumnName == "State");
                //if (column != null && column.Visibility == "1")
                //{
                //    gvc = new GridViewColumn();
                //    gvc.Header = CurrentApp.GetLanguageInfo("COL110100104", "State");
                //    gvc.Width = column.Width;
                //    DataTemplate objectStateTemplate = (DataTemplate)Resources["ObjectStateCellTemplate"];
                //    if (objectStateTemplate != null)
                //    {
                //        gvc.CellTemplate = objectStateTemplate;
                //    }
                //    else
                //    {
                //        gvc.DisplayMemberBinding = new Binding("State");
                //    }
                //    listColumns.Add(gvc);
                //}

                gvc = new GridViewColumn();
                gvc.Header = string.Empty;
                gvc.Width = 150;
                DataTemplate operationTemplate = (DataTemplate)Resources["OperationCellTemplate"];
                if (operationTemplate != null)
                {
                    gvc.CellTemplate = operationTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding();
                }
                listColumns.Add(gvc);

                DataTemplate nameColumnTemplate = (DataTemplate)Resources["NameColumnTemplate"];
                if (nameColumnTemplate != null)
                {
                    TvObjects.SetColumns(nameColumnTemplate, gvch, nameColumnWidth, listColumns);
                }
                else
                {
                    TvObjects.SetColumns(gvch, nameColumnWidth, listColumns);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitOperations()
        {
            ListOperations.Clear();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("11");
                string parentID = "1101";
                webRequest.ListData.Add(parentID);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitOperations Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitOperations Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        ListOperations.Add(optInfo);
                    }
                }

                CurrentApp.WriteLog("PageInit", string.Format("Init UserOperation"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitBasicOperations(ObjectItem objItem)
        {
            mListBasicOperations.Clear();
            if (objItem != null)
            {
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    OperationInfo item = ListOperations[i];
                    if (item.ID == S1101Consts.OPT_ADDORG && objItem.ObjType == S1101Consts.OBJTYPE_ORG)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1101Consts.OPT_DELETEORG && objItem.ObjType == S1101Consts.OBJTYPE_ORG)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1101Consts.OPT_MODIFYORG && objItem.ObjType == S1101Consts.OBJTYPE_ORG)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1101Consts.OPT_ADDUSER && objItem.ObjType == S1101Consts.OBJTYPE_ORG)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1101Consts.OPT_DELETEUSER && objItem.ObjType == S1101Consts.OBJTYPE_USER)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1101Consts.OPT_MODIFYUSER && objItem.ObjType == S1101Consts.OBJTYPE_USER)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1101Consts.OPT_SETUSERROLE && objItem.ObjType == S1101Consts.OBJTYPE_USER)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1101Consts.OPT_SETUSERMANAGEMENT && objItem.ObjType == S1101Consts.OBJTYPE_USER)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1101Consts.OPT_SETUSERRESOURCEMANAGEMENT && objItem.ObjType == S1101Consts.OBJTYPE_USER)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1101Consts.OPT_IMPORTUSERDATA)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1101Consts.OPT_IMPORTAGENTDATA)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1101Consts.OPT_LDAP && objItem.ObjType == S1101Consts.OBJTYPE_ORG)
                    {
                        mListBasicOperations.Add(item);
                    }
                    //if (item.ID == S1101Consts.OPT_ABCD && objItem.ObjType == S1101Consts.OBJTYPE_ORG)
                    //{
                    //    mListBasicOperations.Add(item);
                    //}
                }
            }
            CreateOptButtons();
        }

        private void InitOrgType()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.GetOrgTypeList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitOrgType Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                mListOrgTypeInfos.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OrgTypeInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitOrgType Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OrgTypeInfo item = optReturn.Data as OrgTypeInfo;
                    if (item == null)
                    {
                        ShowException(string.Format("Fail.\tOrgTypeInfo is null"));
                        return;
                    }
                    mListOrgTypeInfos.Add(item);
                }

                CurrentApp.WriteLog("PageInit", string.Format("Init OrgType"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private string LoadObjectProperty(string objType, string objID, string propertyName)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetResourceProperty;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(objType);
                webRequest.ListData.Add(propertyName);
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(objID);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteLog("LoadObjProperty", string.Format("LoadObjectProperty Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return string.Empty;
                }
                if (webReturn.ListData != null && webReturn.ListData.Count > 0)
                {
                    string strInfo = webReturn.ListData[0];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length > 1)
                    {
                        return arrInfo[1];
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("LoadObjProperty", string.Format("Fail.\t{0}", ex.Message));
                return string.Empty;
            }
        }

        #endregion

        #region EventHandlers

        void TabControlView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabControlView.SelectedIndex == 1)
            {
                BorderToolBar.Visibility = Visibility.Collapsed;
            }
            else
            {
                BorderToolBar.Visibility = Visibility.Visible;
            }
        }

        void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var tree = sender as TreeView;
            if (tree != null)
            {
                var item = tree.SelectedItem as ObjectItem;
                mCurrentObjectItem = item;
                if (item != null)
                {
                    InitBasicOperations(item);
                    //准备拖放的数据
                    PrepareDragDropData(item);
                    //更新详细信息窗口
                    ShowObjectDetail();
                }
            }
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            TreeView treeView = null;
            switch (TabControlView.SelectedIndex)
            {
                case 0:
                    treeView = TvSample;
                    break;
                case 1:
                    treeView = TvObjects;
                    break;
                case 2:
                    treeView = TvDiagram;
                    break;
            }
            if (treeView == null) { return; }
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                ObjectItem objItem;
                if (optItem != null)
                {
                    switch (optItem.ID)
                    {
                        case -1:
                            ClearChildren(mRoot);
                            InitControledOrgs(mRoot, "-1");
                            //展开到下一级
                            mRoot.IsExpanded = true;
                            if (mRoot.Children.Count > 0)
                            {
                                for (int i = 0; i < mRoot.Children.Count; i++)
                                {
                                    mRoot.Children[i].IsExpanded = true;
                                }
                                var currentItem = mRoot.Children[0] as ObjectItem;
                                if (currentItem != null)
                                {
                                    currentItem.IsSingleSelected = true;
                                    mCurrentObjectItem = currentItem;
                                }
                            }
                            break;
                        case S1101Consts.OPT_ADDORG:
                            objItem = treeView.SelectedItem as ObjectItem;
                            AddOrg(objItem);
                            break;
                        case S1101Consts.OPT_DELETEORG:
                            objItem = treeView.SelectedItem as ObjectItem;
                            DeleteOrg(objItem);
                            break;
                        case S1101Consts.OPT_MODIFYORG:
                            objItem = treeView.SelectedItem as ObjectItem;
                            ModifyOrg(objItem);
                            break;
                        case S1101Consts.OPT_ADDUSER:
                            objItem = treeView.SelectedItem as ObjectItem;
                            AddUser(objItem);
                            break;
                        case S1101Consts.OPT_MODIFYUSER:
                            objItem = treeView.SelectedItem as ObjectItem;
                            ModifyUser(objItem);
                            break;
                        case S1101Consts.OPT_DELETEUSER:
                            objItem = treeView.SelectedItem as ObjectItem;
                            DeleteUser(objItem);
                            break;
                        case S1101Consts.OPT_SETUSERROLE:
                            objItem = treeView.SelectedItem as ObjectItem;
                            SetUserRole(objItem);
                            break;
                        case S1101Consts.OPT_SETUSERMANAGEMENT:
                            objItem = treeView.SelectedItem as ObjectItem;
                            SetUserManagement(objItem);
                            break;
                        case S1101Consts.OPT_SETUSERRESOURCEMANAGEMENT:
                            objItem = treeView.SelectedItem as ObjectItem;
                            SetUserResourceManagement(objItem);
                            break;
                        case S1101Consts.OPT_IMPORTUSERDATA:
                            objItem = treeView.SelectedItem as ObjectItem;
                            ImportWhat = "USER";
                            ImportData(objItem);
                            break;
                        case S1101Consts.OPT_IMPORTAGENTDATA:
                            objItem = treeView.SelectedItem as ObjectItem;
                            ImportWhat = "AGENT";
                            ImportData(objItem);
                            break;
                        case S1101Consts.OPT_LDAP:
                            objItem = treeView.SelectedItem as ObjectItem;
                            GetLDAPUsers();
                            break;
                    }
                }
            }
        }

        void SliderScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                int value;
                if (int.TryParse(SliderScale.Value.ToString(), out value))
                {
                    switch (value)
                    {
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
                    }
                }
                ScaleTransform tran = new ScaleTransform();
                tran.ScaleX = mViewerScale;
                tran.ScaleY = mViewerScale;
                BorderDiagramView.LayoutTransform = tran;
                SliderScale.Tag = mViewerScale;
                BindingExpression be = SliderScale.GetBindingExpression(ToolTipProperty);
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

        #endregion

        #region Others

        private void ClearChildren(ObjectItem item)
        {
            if (item == null) { return; }
            for (int i = 0; i < item.Children.Count; i++)
            {
                var child = item.Children[i] as ObjectItem;
                if (child != null)
                {
                    var temp = mListObjectItems.FirstOrDefault(j => j.ObjID == child.ObjID);
                    if (temp != null) { mListObjectItems.Remove(temp); }
                    ClearChildren(child);
                }
            }
            Dispatcher.Invoke(new Action(() => item.Children.Clear()));
        }

        private void AddChildObjectItem(ObjectItem parent, ObjectItem child)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (parent != null)
                {
                    parent.AddChild(child);
                }
            }));
        }

        private void CreateOptButtons()
        {
            try
            {
                PanelBasicOpts.Children.Clear();
                PanelToolButton.Children.Clear();
                OperationInfo item;
                Button btn;
                //刷新按钮
                item = new OperationInfo();
                item.ID = -1;
                item.Icon = "Images/refresh.png";
                item.Display = CurrentApp.GetLanguageInfo("110112", "Refresh");
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "ToolButtonStyle");
                PanelToolButton.Children.Add(btn);
                for (int i = 0; i < mListBasicOperations.Count; i++)
                {
                    item = mListBasicOperations[i];
                    //基本操作按钮
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelBasicOpts.Children.Add(btn);
                    //工具栏按钮
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = item;
                    btn.SetResourceReference(StyleProperty, "ToolButtonStyle");
                    PanelToolButton.Children.Add(btn);
                }
            }
            catch (Exception ex)
            { }
        }

        public void ReloadData()
        {
            ReloadData(mRoot);
        }

        public void ReloadData(ObjectItem parent)
        {
            if (parent == mRoot)
            {
                InitControledOrgs();
                return;
            }
            BasicOrgInfo orgInfo = parent.Data as BasicOrgInfo;
            if (orgInfo != null)
            {
                ClearChildren(parent);
                InitControledOrgs(parent, orgInfo.OrgID.ToString());
                InitControledUsers(parent, orgInfo.OrgID.ToString());
                if (InitParameter.Contains("A"))
                    InitControlAgents(parent, orgInfo.OrgID.ToString());
                if (InitParameter.Contains("E"))
                    InitControlExtension(parent, orgInfo.OrgID.ToString());
                if (InitParameter.Contains("R"))
                    InitControlRealExtension(parent, orgInfo.OrgID.ToString());
                //if (parent == mRoot || parent == null)
                if ((parent.ObjParentID == 0 && parent.ObjType == 101) || (parent.ObjParentID == 0 && parent.ObjType == 0))
                {
                    parent.IsExpanded = true;
                }
                else
                {
                    parent.IsExpanded = false;
                }
            }
        }

        private void ShowObjectDetail()
        {
            try
            {
                ObjectItem item = mCurrentObjectItem;
                if (item == null) { return; }
                ObjectDetail.Title = item.Name;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(string.Format("/UMPS1101;component/Themes/Default/UMPS1101/{0}", item.Icon), UriKind.Relative);
                image.EndInit();
                ObjectDetail.Icon = image;
                List<PropertyItem> listProperties = new List<PropertyItem>();
                switch (item.ObjType)
                {
                    case S1101Consts.OBJTYPE_ORG:
                        ObjectDetail.Description = item.Description;
                        BasicOrgInfo orgInfo = item.Data as BasicOrgInfo;
                        if (orgInfo != null)
                        {
                            PropertyItem property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10102", "Orgnization Type");
                            property.ToolTip = property.Name;
                            OrgTypeInfo orgType =
                                mListOrgTypeInfos.FirstOrDefault(
                                    ot =>
                                        ot.ID.ToString().Substring(11) == orgInfo.OrgType.ToString("00000000"));
                            if (orgType != null)
                            {
                                if (orgType.Name.Equals("InitData"))
                                {
                                    //特别翻译InitData的机构类型
                                    property.Value = CurrentApp.GetLanguageInfo("1101T10112", "System Init Data");
                                }
                                else
                                {
                                    property.Value = orgType.Name;
                                }
                            }
                            else
                            {
                                property.Value = orgInfo.OrgType.ToString();
                            }
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10105", "Valid Time");
                            property.ToolTip = property.Name;
                            property.Value = orgInfo.StrStartTime;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10106", "Invalid Time");
                            property.ToolTip = property.Name;
                            if (orgInfo.StrEndTime.ToUpper().Equals("UNLIMITED"))
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10215", "UNLIMITED");
                            }
                            else
                            {
                                property.Value = orgInfo.EndTime.ToString("yyyy/MM/dd HH:mm:ss");
                            }
                            //property.Value = orgInfo.StrEndTime;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10107", "Creator");
                            property.ToolTip = property.Name;
                            property.Value = GetUserAccountName(orgInfo.Creator);
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10108", "Create Time");
                            property.ToolTip = property.Name;
                            property.Value = orgInfo.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
                            listProperties.Add(property);
                        }
                        break;
                    case S1101Consts.OBJTYPE_USER:
                        ObjectDetail.Description = item.FullName;
                        BasicUserInfo basicUserInfo = item.Data as BasicUserInfo;
                        if (basicUserInfo != null)
                        {
                            PropertyItem property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10210", "Source Flag");
                            property.ToolTip = property.Name;
                            if (basicUserInfo.SourceFlag.Equals("S"))
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10112", "System Init Data");
                            }
                            else if (basicUserInfo.SourceFlag.Equals("L"))
                            {
                                property.Value = "LADP";
                            }
                            else if (basicUserInfo.SourceFlag.Equals("U"))
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10113", "Manual Add");
                            }
                            else
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10111", "Other");
                            }

                            //property.Value = basicUserInfo.SourceFlag;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10211", "Is Locked");
                            property.ToolTip = property.Name;
                            property.Value = basicUserInfo.IsLocked;
                            if (basicUserInfo.IsLocked != "0")
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10114", "Yes");
                                listProperties.Add(property);
                            }
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10212", "Lock Method");
                            property.ToolTip = property.Name;
                            property.Value = basicUserInfo.LockMethod;
                            if (basicUserInfo.IsLocked != "0")
                            {
                                if (basicUserInfo.LockMethod == "U")
                                {
                                    property.Value = CurrentApp.GetLanguageInfo("1101T10115", "Interface Lock");
                                }
                                else if (basicUserInfo.LockMethod == "L")
                                {
                                    property.Value = CurrentApp.GetLanguageInfo("1101T10116", "Safe Lock");
                                }
                                else
                                {
                                    property.Value = CurrentApp.GetLanguageInfo("1101T10111", "Other");
                                }

                                listProperties.Add(property);
                            }
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10206", "Valid Time");
                            property.ToolTip = property.Name;
                            property.Value = basicUserInfo.StartTime.ToString("yyyy/MM/dd HH:mm:ss");
                            //basicUserInfo.StrStartTime;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10207", "Invalid Time");
                            property.ToolTip = property.Name;
                            if (basicUserInfo.StrEndTime.ToUpper().Equals("UNLIMITED"))
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10215", "UNLIMITED");
                            }
                            else
                            {
                                property.Value = basicUserInfo.EndTime.ToString("yyyy/MM/dd HH:mm:ss");
                            }
                            //basicUserInfo.StrEndTime;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10208", "Creator");
                            property.ToolTip = property.Name;
                            property.Value = GetUserAccountName(basicUserInfo.Creator);
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10209", "Create Time");
                            property.ToolTip = property.Name;
                            property.Value = basicUserInfo.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
                            listProperties.Add(property);
                        }
                        break;
                    case S1101Consts.OBJTYPE_AGENT:
                        ObjectDetail.Description = item.FullName;
                        BasicUserInfo basicAgentInfo = item.Data as BasicUserInfo;
                        if (basicAgentInfo != null)
                        {
                            PropertyItem property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10210", "Source Flag");
                            property.ToolTip = property.Name;
                            if (basicAgentInfo.SourceFlag.Equals("S"))
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10112", "System Init Data");
                            }
                            else if (basicAgentInfo.SourceFlag.Equals("L"))
                            {
                                property.Value = "LADP";
                            }
                            else if (basicAgentInfo.SourceFlag.Equals("U"))
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10113", "Manual Add");
                            }
                            else
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10111", "Other");
                            }

                            //property.Value = basicUserInfo.SourceFlag;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10211", "Is Locked");
                            property.ToolTip = property.Name;
                            property.Value = basicAgentInfo.IsLocked;
                            if (basicAgentInfo.IsLocked != "0")
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10114", "Yes");
                                listProperties.Add(property);
                            }
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10212", "Lock Method");
                            property.ToolTip = property.Name;
                            property.Value = basicAgentInfo.LockMethod;
                            if (basicAgentInfo.IsLocked != "0")
                            {
                                if (basicAgentInfo.LockMethod == "U")
                                {
                                    property.Value = CurrentApp.GetLanguageInfo("1101T10115", "Interface Lock");
                                }
                                else if (basicAgentInfo.LockMethod == "L")
                                {
                                    property.Value = CurrentApp.GetLanguageInfo("1101T10116", "Safe Lock");
                                }
                                else
                                {
                                    property.Value = CurrentApp.GetLanguageInfo("1101T10111", "Other");
                                }

                                listProperties.Add(property);
                            }
                            //property = new PropertyItem();
                            //property.Name = CurrentApp.GetLanguageInfo("1101T10206", "Valid Time");
                            //property.ToolTip = property.Name;
                            //property.Value = basicAgentInfo.StartTime.ToString("yyyy/MM/dd HH:mm:ss");
                            ////basicUserInfo.StrStartTime;
                            //listProperties.Add(property);
                            //property = new PropertyItem();
                            //property.Name = CurrentApp.GetLanguageInfo("1101T10207", "Invalid Time");
                            //property.ToolTip = property.Name;
                            //if (basicAgentInfo.StrEndTime.ToUpper().Equals("UNLIMITED"))
                            //{
                            //    property.Value = CurrentApp.GetLanguageInfo("1101T10215", "UNLIMITED");
                            //}
                            //else
                            //{
                            //    property.Value = basicAgentInfo.EndTime.ToString("yyyy/MM/dd HH:mm:ss");
                            //}
                            ////basicUserInfo.StrEndTime;
                            //listProperties.Add(property);
                            //property = new PropertyItem();
                            //property.Name = CurrentApp.GetLanguageInfo("1101T10208", "Creator");
                            //property.ToolTip = property.Name;
                            //property.Value = GetUserAccountName(basicAgentInfo.Creator);
                            //listProperties.Add(property);
                            //property = new PropertyItem();
                            //property.Name = CurrentApp.GetLanguageInfo("1101T10209", "Create Time");
                            //property.ToolTip = property.Name;
                            //property.Value = basicAgentInfo.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
                            //listProperties.Add(property);
                        }
                        break;
                    case S1101Consts.OBJTYPE_EXTENSION:
                        ObjectDetail.Description = item.FullName;
                        BasicUserInfo basicExtInfo = item.Data as BasicUserInfo;
                        if (basicExtInfo != null)
                        {
                            PropertyItem property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10210", "Source Flag");
                            property.ToolTip = property.Name;
                            if (basicExtInfo.SourceFlag.Equals("S"))
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10112", "System Init Data");
                            }
                            else if (basicExtInfo.SourceFlag.Equals("L"))
                            {
                                property.Value = "LADP";
                            }
                            else if (basicExtInfo.SourceFlag.Equals("U"))
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10113", "Manual Add");
                            }
                            else
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10111", "Other");
                            }

                            //property.Value = basicUserInfo.SourceFlag;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10211", "Is Locked");
                            property.ToolTip = property.Name;
                            property.Value = basicExtInfo.IsLocked;
                            if (basicExtInfo.IsLocked != "0")
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1101T10114", "Yes");
                                listProperties.Add(property);
                            }
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1101T10212", "Lock Method");
                            property.ToolTip = property.Name;
                            property.Value = basicExtInfo.LockMethod;
                            if (basicExtInfo.IsLocked != "0")
                            {
                                if (basicExtInfo.LockMethod == "U")
                                {
                                    property.Value = CurrentApp.GetLanguageInfo("1101T10115", "Interface Lock");
                                }
                                else if (basicExtInfo.LockMethod == "L")
                                {
                                    property.Value = CurrentApp.GetLanguageInfo("1101T10116", "Safe Lock");
                                }
                                else
                                {
                                    property.Value = CurrentApp.GetLanguageInfo("1101T10111", "Other");
                                }

                                listProperties.Add(property);
                            }
                            //property = new PropertyItem();
                            //property.Name = CurrentApp.GetLanguageInfo("1101T10206", "Valid Time");
                            //property.ToolTip = property.Name;
                            //property.Value = basicAgentInfo.StartTime.ToString("yyyy/MM/dd HH:mm:ss");
                            ////basicUserInfo.StrStartTime;
                            //listProperties.Add(property);
                            //property = new PropertyItem();
                            //property.Name = CurrentApp.GetLanguageInfo("1101T10207", "Invalid Time");
                            //property.ToolTip = property.Name;
                            //if (basicAgentInfo.StrEndTime.ToUpper().Equals("UNLIMITED"))
                            //{
                            //    property.Value = CurrentApp.GetLanguageInfo("1101T10215", "UNLIMITED");
                            //}
                            //else
                            //{
                            //    property.Value = basicAgentInfo.EndTime.ToString("yyyy/MM/dd HH:mm:ss");
                            //}
                            ////basicUserInfo.StrEndTime;
                            //listProperties.Add(property);
                            //property = new PropertyItem();
                            //property.Name = CurrentApp.GetLanguageInfo("1101T10208", "Creator");
                            //property.ToolTip = property.Name;
                            //property.Value = GetUserAccountName(basicAgentInfo.Creator);
                            //listProperties.Add(property);
                            //property = new PropertyItem();
                            //property.Name = CurrentApp.GetLanguageInfo("1101T10209", "Create Time");
                            //property.ToolTip = property.Name;
                            //property.Value = basicAgentInfo.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
                            //listProperties.Add(property);
                        }
                        break;
                }
                ObjectDetail.ItemsSource = listProperties;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private string GetUserAccountName(long userID)
        {
            var temp =
                S1101App.ListCacheInfos.FirstOrDefault(
                    c => c.Name == string.Format("ID2N{0}", userID));
            if (temp == null)
            {
                temp = new CacheInfo();
                temp.Name = string.Format("ID2N{0}", userID);
                temp.Value = LoadObjectProperty("102", userID.ToString(), "Account");

            }
            if (string.IsNullOrEmpty(temp.Value))
            {
                return userID.ToString();
            }
            return temp.Value;
        }

        private void OpenCloseLeftPanel()
        {
            if (GridLeft.Width.Value > 0)
            {
                GridLeft.Width = new GridLength(0);
            }
            else
            {
                GridLeft.Width = new GridLength(200);
            }
        }

        //px+
        //public void RefreshHeadIcon()
        //{
        //    Dispatcher.Invoke(new Action(() => { this.PageHead.InitInfo(); }));
        //}
        //end

        #endregion

        #region DragDrop

        private void PrepareDragDropData(ObjectItem item)
        {
            if (item != null)
            {
                #region 如果没有按下Shift和Ctl，直接重置SelectInfo，然后添加当前项

                if (!Keyboard.IsKeyDown(Key.LeftShift)
                    && !Keyboard.IsKeyDown(Key.RightShift)
                    && !Keyboard.IsKeyDown(Key.LeftCtrl)
                    && !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    //重置SelectInfo,将SelectInfo里的对象的IsSelected属性都设为false
                    //然后清除所有项目
                    if (mSelectInfo != null)
                    {
                        for (int i = 0; i < mSelectInfo.ListItems.Count; i++)
                        {
                            mSelectInfo.ListItems[i].IsSelected = false;
                        }
                        mSelectInfo.ListItems.Clear();
                        mSelectInfo.Parent = null;
                    }
                    //插入当前ObjectItem，将IsSelected设为True
                    //暂时只能是用户对象可以拖放
                    //if (item.ObjType == ConstValues.OBJTYPE_USER)
                    //{
                    //    item.IsSelected = true;
                    //    mSelectInfo = new SelectedInfo();
                    //    mSelectInfo.ObjType = item.ObjType;
                    //    mSelectInfo.Parent = item.Parent;
                    //    mSelectInfo.ListItems.Add(item);
                    //}
                    item.IsSelected = true;
                    mSelectInfo = new SelectedInfo();
                    mSelectInfo.ObjType = item.ObjType;
                    mSelectInfo.Parent = item.Parent;
                    mSelectInfo.ListItems.Add(item);
                }

                #endregion

                #region 如果按下了Shift或Ctl键，向SelectInfo里追加

                else
                {
                    //如果SelectInfo为空，创建并初始化
                    if (mSelectInfo == null)
                    {
                        mSelectInfo = new SelectedInfo();
                        mSelectInfo.ObjType = item.ObjType;
                        mSelectInfo.Parent = item.Parent;
                    }
                    //如果类型不同，重置SelectInfo
                    if (mSelectInfo.ObjType != item.ObjType)
                    {
                        for (int i = 0; i < mSelectInfo.ListItems.Count; i++)
                        {
                            mSelectInfo.ListItems[i].IsSelected = false;
                        }
                        mSelectInfo.ListItems.Clear();
                        mSelectInfo.ObjType = item.ObjType;
                        mSelectInfo.Parent = item.Parent;
                    }
                    var parent = mSelectInfo.Parent;
                    if (parent != null)
                    {
                        //如果当前父级与SelectInfo的父级不同，重置SelectInfo
                        if (parent != item.Parent)
                        {
                            for (int i = 0; i < mSelectInfo.ListItems.Count; i++)
                            {
                                mSelectInfo.ListItems[i].IsSelected = false;
                            }
                            mSelectInfo.ListItems.Clear();
                            mSelectInfo.Parent = item.Parent;
                        }
                        //如果按下Ctl项，追加当前项，否则追加一系列连续的项（按下了Shift键）
                        if (Keyboard.IsKeyDown(Key.LeftCtrl)
                            || Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            item.IsSelected = true;
                            if (!mSelectInfo.ListItems.Contains(item))
                            {
                                mSelectInfo.ListItems.Add(item);
                            }
                        }
                        else
                        {
                            var objParent = parent as ObjectItem;
                            if (objParent != null)
                            {
                                //取得最小和最大索引
                                int index, minIndex = int.MaxValue, maxIndex = int.MinValue, currentIndex;
                                for (int i = 0; i < mSelectInfo.ListItems.Count; i++)
                                {
                                    index = objParent.Children.IndexOf(mSelectInfo.ListItems[i]);
                                    if (index >= 0)
                                    {
                                        minIndex = Math.Min(index, minIndex);
                                        maxIndex = Math.Max(index, maxIndex);
                                    }
                                }
                                currentIndex = objParent.Children.IndexOf(item);
                                if (currentIndex >= 0)
                                {
                                    minIndex = Math.Min(minIndex, currentIndex);
                                    maxIndex = Math.Max(maxIndex, currentIndex);

                                    //追加需要追加的项
                                    List<ObjectItem> tempList = new List<ObjectItem>();
                                    ObjectItem tempItem;
                                    for (int i = minIndex; i <= maxIndex; i++)
                                    {
                                        tempItem = objParent.Children[i] as ObjectItem;
                                        if (tempItem != null && tempItem.ObjType == mSelectInfo.ObjType)
                                        {
                                            tempList.Add(tempItem);
                                            if (!mSelectInfo.ListItems.Contains(tempItem))
                                            {
                                                tempItem.IsSelected = true;
                                                mSelectInfo.ListItems.Add(tempItem);
                                            }
                                        }
                                    }
                                    //重置需要重置的项
                                    for (int i = 0; i < mSelectInfo.ListItems.Count; i++)
                                    {
                                        tempItem = mSelectInfo.ListItems[i];
                                        if (!tempList.Contains(tempItem))
                                        {
                                            tempItem.IsSelected = false;
                                            mSelectInfo.ListItems.Remove(tempItem);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
        }

        private void ObjectItem_StartDragged(object sender, DragDropEventArgs e)
        {
            var dragSource = e.DragSource;
            var dragData = mSelectInfo;
            if (dragSource != null && dragData != null)
            {
                DragDrop.DoDragDrop(dragSource, dragData, DragDropEffects.Move);
            }
        }

        private void ObjectItem_DragOver(object sender, DragDropEventArgs e)
        {

        }

        private void ObjectItem_Dropped(object sender, DragDropEventArgs e)
        {
            var currentObjItem = sender as ObjectItem;
            if (currentObjItem != null)
            {
                var dragData = e.DragData as IDataObject;
                if (dragData != null)
                {
                    var selectInfo = dragData.GetData(typeof(SelectedInfo)) as SelectedInfo;
                    if (selectInfo != null)
                    {
                        MoveObject(selectInfo, currentObjItem);
                    }
                }
            }
        }

        #endregion


        #region Operations

        private void AddOrg(ObjectItem parent)
        {
            if (parent != null)
            {
                PopupPanel.Title = "Add New Organizaiton";
                OrgInfoModify orgInfoModify = new OrgInfoModify();
                orgInfoModify.PageParent = this;
                orgInfoModify.IsAddOrg = true;
                orgInfoModify.ObjParent = parent;
                orgInfoModify.CurrentApp = CurrentApp;
                PopupPanel.Content = orgInfoModify;
                PopupPanel.IsOpen = true;
            }
        }

        private void ModifyOrg(ObjectItem orgItem)
        {
            if (orgItem == null) { return; }
            if (orgItem.ObjID == ConstValue.ORG_ROOT) { return; }
            PopupPanel.Title = "Modify Organization";
            OrgInfoModify orgInfoModify = new OrgInfoModify();
            orgInfoModify.PageParent = this;
            orgInfoModify.IsAddOrg = false;
            orgInfoModify.OrgItem = orgItem;
            orgInfoModify.CurrentApp = CurrentApp;
            //List<ObjectItem> lstobjectitemp = new List<ObjectItem>();
            //lstobjectitemp = mListObjectItems.Where(p => p.ObjParentID == orgItem.ObjParentID && p.ObjType == orgItem.ObjType).ToList();
            //orgInfoModify.mListObjectItem = lstobjectitemp;
            PopupPanel.Content = orgInfoModify;

            PopupPanel.IsOpen = true;
        }

        private void DeleteOrg(ObjectItem orgItem)
        {
            if (orgItem == null) { return; }
            BasicOrgInfo orgInfo = orgItem.Data as BasicOrgInfo;
            if (orgInfo != null)
            {
                //根机构不能删除
                if (orgInfo.OrgID == S1101Consts.ORG_ROOT)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N011", "Root Orgnization Can Not Be Delete"));
                    return;
                }
                //机构下有下级机构或用户及其他对象时不能删除
                if (orgItem.Children.Count > 0)
                {
                    ShowException(CurrentApp.GetMessageLanguageInfo("001", "This Orgnization has child object, can not be deleted!"));
                    return;
                }
                var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}",
                    CurrentApp.GetMessageLanguageInfo("002", "Confirm delete organization ?"),
                    orgItem.Name), "UMP",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SetBusy(true, string.Empty);
                    mWorker = new BackgroundWorker();
                    mWorker.DoWork += (s, de) =>
                    {
                        try
                        {
                            WebRequest webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)S1101Codes.DeleteOrgInfo;
                            webRequest.Data = orgInfo.OrgID.ToString();
                            Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                            WebHelper.SetServiceClient(client);
                            WebReturn webReturn = client.DoOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                                ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                                return;
                            }
                            ObjectItem parent = orgItem.Parent as ObjectItem;
                            if (parent != null)
                            {
                                ReloadData(parent);
                            }

                            #region 写操作日志

                            string msg = string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10101"), orgInfo.OrgName);
                            CurrentApp.WriteOperationLog(S1101Consts.OPT_DELETEORG.ToString(), ConstValue.OPT_RESULT_SUCCESS, msg);

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            ShowException(ex.Message);
                        }
                    };
                    mWorker.RunWorkerCompleted += (s, re) =>
                    {
                        mWorker.Dispose();
                        SetBusy(false, string.Empty);
                    };
                    mWorker.RunWorkerAsync();
                }
            }
        }

        private void AddUser(ObjectItem parent)
        {
            if (parent != null)
            {
                PopupPanel.Title = "Add New User";
                UserInfoModify userInfoModify = new UserInfoModify();
                userInfoModify.PageParent = this;
                userInfoModify.IsAddUser = true;
                userInfoModify.ObjParent = parent;
                //userInfoModify.ListObjItems = mListObjectItems;
                userInfoModify.CurrentApp = CurrentApp;
                PopupPanel.Content = userInfoModify;
                PopupPanel.IsOpen = true;
            }
        }

        private void ModifyUser(ObjectItem userItem)
        {
            if (userItem != null)
            {
                PopupPanel.Title = "Modify User Information";
                UserInfoModify userInfoModify = new UserInfoModify();
                userInfoModify.PageParent = this;
                userInfoModify.IsAddUser = false;
                userInfoModify.UserItem = userItem;
                userInfoModify.CurrentApp = CurrentApp;
                //if (userItem.ObjID == S1101Consts.USER_ADMIN)
                //{
                //    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N015", "Admin Account Can Not Be Modify"));
                //    return;
                //}

                PopupPanel.Content = userInfoModify;
                PopupPanel.IsOpen = true;
            }
        }

        private void DeleteUser(ObjectItem userItem)
        {
            if (userItem == null)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N014", "Please Select A User"));
                return;
            }
            BasicUserInfo userInfo = userItem.Data as BasicUserInfo;
            if (userInfo != null)
            {
                string strUserID = string.Empty;
                string strInfoMsg = string.Empty;
                if (mSelectInfo == null) { return; }
                for (int i = 0; i < mSelectInfo.ListItems.Count; i++)
                {
                    var tempItem = mSelectInfo.ListItems[i].Data as BasicUserInfo;
                    if (tempItem != null)
                    {
                        //内置管理员帐号及本登录帐号不能删除
                        if (tempItem.UserID == S1101Consts.USER_ADMIN)
                        {
                            CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N012", "Admin Account Can Not Be Delete"));
                            return;
                        }
                        if (tempItem.UserID == CurrentApp.Session.UserID)
                        {
                            CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N016", "Current Account Can Not Be Delete"));
                            return;
                        }
                        strUserID += string.Format("{0}{1}", tempItem.UserID, ConstValue.SPLITER_CHAR);
                        strInfoMsg += tempItem.Account + " \t";
                    }
                }
                if (string.IsNullOrEmpty(strUserID)) { return; }
                var result = MessageBox.Show(string.Format("{0}\r\n\r\n{1}",
                    CurrentApp.GetMessageLanguageInfo("003", "Confirm delete user ?"),
                    strInfoMsg), "UMP",
                   MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    SetBusy(true, string.Empty);
                    mWorker = new BackgroundWorker();
                    mWorker.DoWork += (s, de) =>
                    {
                        try
                        {
                            WebRequest webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)S1101Codes.DeleteUserInfo;
                            webRequest.Data = strUserID;
                            Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                            WebHelper.SetServiceClient(client);
                            WebReturn webReturn = client.DoOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                                ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                                return;
                            }
                            ObjectItem parent = userItem.Parent as ObjectItem;
                            if (parent != null)
                            {
                                ReloadData(parent);
                            }

                            #region 写操作日志

                            string msg = string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10201"), strInfoMsg);
                            CurrentApp.WriteOperationLog(S1101Consts.OPT_DELETEUSER.ToString(), ConstValue.OPT_RESULT_SUCCESS, msg);

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            ShowException(ex.Message);
                        }
                    };
                    mWorker.RunWorkerCompleted += (s, re) =>
                    {
                        mWorker.Dispose();
                        SetBusy(false, string.Empty);
                    };
                    mWorker.RunWorkerAsync();
                }
            }
        }

        private void SetUserRole(ObjectItem userItem)
        {
            if (userItem != null)
            {
                BasicUserInfo userInfo = userItem.Data as BasicUserInfo;
                //不能给当前用户自身分配角色

                if (userInfo.UserID == CurrentApp.Session.UserInfo.UserID)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N017", "Can Not Set Self 's Role"));
                    return;
                }
                if (userInfo.UserID == S1101Consts.USER_ADMIN)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N018", "Admin Account Can Not Set Role"));
                    return;
                }
                PopupPanel.Title = "Set User Role";
                UserRoleModify userRoleModify = new UserRoleModify();
                userRoleModify.PageParent = this;
                userRoleModify.UserItem = userItem;
                userRoleModify.CurrentApp = CurrentApp;
                PopupPanel.Content = userRoleModify;
                PopupPanel.IsOpen = true;
            }
        }

        private void SetUserManagement(ObjectItem userItem)
        {
            if (userItem != null)
            {
                BasicUserInfo userInfo = userItem.Data as BasicUserInfo;
                //不能给当前用户自身分配管理权限

                if (userInfo.UserID == CurrentApp.Session.UserInfo.UserID)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N019", "Can Not Set Self  Role's User "));
                    return;
                }
                if (userInfo.UserID == S1101Consts.USER_ADMIN)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N020", "Admin Account Can Not Set Role's User"));
                    return;
                }

                PopupPanel.Title = "Set User Management";
                UserResourceManagement userResourceMmt = new UserResourceManagement();
                userResourceMmt.PageParent = this;
                userResourceMmt.UserItem = userItem;
                userResourceMmt.CurrentApp = CurrentApp;
                PopupPanel.Content = userResourceMmt;
                PopupPanel.IsOpen = true;
            }
        }

        private void SetUserResourceManagement(ObjectItem userItem)
        {
            if (userItem != null)
            {
                BasicUserInfo userInfo = userItem.Data as BasicUserInfo;
                //不能给当前用户自身分配管理权限

                if (userInfo.UserID == CurrentApp.Session.UserInfo.UserID)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N031", "Can Not Set Self  Role's User "));
                    return;
                }
                //if (userInfo.UserID == S1101Consts.USER_ADMIN)
                //{
                //    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N032", "Admin Account Can Not Set Role's User"));
                //    return;
                //}

                PopupPanel.Title = "Set User Resource Management";
                UserResourcesPage userResourceMmt = new UserResourcesPage();
                userResourceMmt.PageParent = this;
                userResourceMmt.UserItem = userItem;
                userResourceMmt.CurrentApp = CurrentApp;
                PopupPanel.Content = userResourceMmt;
                PopupPanel.IsOpen = true;
            }
        }

        private void ImportData(ObjectItem objItem)
        {
            //提示是否下载模板
            System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show(CurrentApp.GetLanguageInfo("1101T10218", "是否已经下载模板"), CurrentApp.AppName, System.Windows.Forms.MessageBoxButtons.YesNoCancel, System.Windows.Forms.MessageBoxIcon.Question);
            if (dr == System.Windows.Forms.DialogResult.No)
            {
                //没有下载模板，则下载
                System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
                dlg.Title = CurrentApp.GetLanguageInfo("1101T10216", "导出模板");
                dlg.FileName = CurrentApp.GetLanguageInfo("1101T10219", "模板名称.xls");
                dlg.Filter = "xlsx files (*.xlsx)|*.xlsx";
                if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //检查路径正确性        
                    if (dlg.FileName.Trim().Length <= 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N034", "请选择路径。");
                        return;
                    }
                    //检查文件夹是否存在        
                    int n = dlg.FileName.LastIndexOf(@"\") + 1;
                    if (n < 0)
                    {
                        CurrentApp.GetLanguageInfo("1101N035", "路径错误。");
                        return;
                    }
                    string PathStr = dlg.FileName.Substring(0, n);
                    if (!Directory.Exists(PathStr))
                    {
                        CurrentApp.GetLanguageInfo("1101N036", "路径不存在，请检查。");
                        return;
                    }
                    //导出  我的Excel资源：UserAgentTemplate.xls  
                    byte[] template = Properties.Resources.UserAgentTemplate;//Excel资源去掉后缀名   
                    File.WriteAllBytes(dlg.FileName, template);
                    //以下两种方法择一使用       
                    //方法一：      
                    //FileStream stream = new FileStream(dlg.FileName, FileMode.Create);
                    //stream.Write(template, 0, template.Length);
                    //stream.Close();
                    //stream.Dispose();
                    //方法二：      
                    //File.WriteAllBytes(dlg.FileName, template);
                }
            }
            else if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                LoadingIn();
            }
        }

        private void MoveObject(SelectedInfo selectInfo, ObjectItem targetItem)
        {
            if (targetItem == null || selectInfo == null || selectInfo.Parent == null) { return; }

            //如果按下了方向键，同一父亲下调换位置
            if (Keyboard.IsKeyDown(Key.U))
            {
                //目标项与源项目父亲要相同
                if (targetItem.Parent != selectInfo.Parent) { return; }
                //源项目不能包含有目标项
                if (selectInfo.ListItems.Contains(targetItem)) { return; }
                ChangeObjectIndex(selectInfo, targetItem.Parent as ObjectItem, targetItem, false);
            }
            else if (Keyboard.IsKeyDown(Key.D))
            {
                //目标项与源项目父亲要相同
                if (targetItem.Parent != selectInfo.Parent) { return; }
                //源项目不能包含有目标项
                if (selectInfo.ListItems.Contains(targetItem)) { return; }
                ChangeObjectIndex(selectInfo, targetItem.Parent as ObjectItem, targetItem, true);
            }
            else
            {
                //目标项不能是源项目的父亲
                if (targetItem == selectInfo.Parent) { return; }
                //目标项只能是机构类型
                var targetOrg = targetItem.Data as BasicOrgInfo;
                if (targetOrg == null) { return; }
                if (selectInfo.ObjType == S1101Consts.OBJTYPE_USER)
                {
                    MoveUser(selectInfo, targetItem);
                    ReloadData(selectInfo.Parent as ObjectItem);
                    ReloadData(targetItem);
                }
                else if (selectInfo.ObjType == S1101Consts.OBJTYPE_AGENT || selectInfo.ObjType == S1101Consts.OBJTYPE_EXTENSION || selectInfo.ObjType == S1101Consts.OBJTYPE_REALEXTENSION)
                {
                    MoveAgent(selectInfo, targetItem);
                    ReloadData(selectInfo.Parent as ObjectItem);
                    ReloadData(targetItem);
                }
            }
        }

        private void MoveUser(SelectedInfo selectInfo, ObjectItem orgItem)
        {
            try
            {
                var orgInfo = orgItem.Data as BasicOrgInfo;
                if (orgInfo == null) { return; }
                string strUserID = string.Empty;
                string strInfoMsg = string.Empty;
                for (int i = 0; i < selectInfo.ListItems.Count; i++)
                {
                    var userItem = selectInfo.ListItems[i].Data as BasicUserInfo;
                    if (userItem != null)
                    {
                        //管理员不能移动
                        if (userItem.UserID == S1101Consts.USER_ADMIN || userItem.UserID == CurrentApp.Session.UserID) { continue; }
                        strUserID += string.Format("{0}{1}", userItem.UserID, ConstValue.SPLITER_CHAR);
                        //strInfoMsg += string.Format("{0} \t", userItem.Account);
                        if (i < 10)
                        {
                            strInfoMsg += string.Format("{0} \t", userItem.Account);
                        }
                        else if (i == 10)
                        {
                            strInfoMsg += string.Format("......", selectInfo.ListItems.Count);
                        }
                    }
                }
                strUserID = strUserID.TrimEnd(new[] { ConstValue.SPLITER_CHAR });
                if (string.IsNullOrEmpty(strUserID)) { return; }

                strInfoMsg = string.Format("{0}\r\n\r\n{1} -> {2}",
                    CurrentApp.GetMessageLanguageInfo("004", "Confirm Move User ?"),
                    strInfoMsg,
                    orgInfo.OrgName);
                var result = MessageBox.Show(strInfoMsg, CurrentApp.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }
                SetBusy(true, string.Empty);
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S1101Codes.MoveUser;
                        webRequest.ListData.Add(orgInfo.OrgID.ToString());
                        webRequest.ListData.Add(strUserID);
                        Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                            WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                        WebHelper.SetServiceClient(client);
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            ShowException(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void MoveAgent(SelectedInfo selectInfo, ObjectItem orgItem)
        {
            try
            {
                var orgInfo = orgItem.Data as BasicOrgInfo;
                if (orgInfo == null) { return; }
                string strUserID = string.Empty;
                string strInfoMsg = string.Empty;
                for (int i = 0; i < selectInfo.ListItems.Count; i++)
                {
                    var userItem = selectInfo.ListItems[i].Data as BasicUserInfo;
                    if (userItem != null)
                    {
                        //管理员不能移动
                        if (userItem.UserID == S1101Consts.USER_ADMIN || userItem.UserID == CurrentApp.Session.UserID) { continue; }
                        strUserID += string.Format("{0}{1}", userItem.UserID, ConstValue.SPLITER_CHAR);
                        if (i < 10)
                        {
                            strInfoMsg += string.Format("{0} \t", userItem.Account);
                        }
                        else if (i == 10)
                        {
                            strInfoMsg += string.Format("......", selectInfo.ListItems.Count);
                        }
                    }
                }
                strUserID = strUserID.TrimEnd(new[] { ConstValue.SPLITER_CHAR });
                if (string.IsNullOrEmpty(strUserID)) { return; }

                strInfoMsg = string.Format("{0}\r\n\r\n{1} -> {2}",
                    CurrentApp.GetMessageLanguageInfo("004", "Confirm Move User ?"),
                    strInfoMsg,
                    orgInfo.OrgName);
                var result = MessageBox.Show(strInfoMsg, CurrentApp.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }
                SetBusy(true, string.Empty);
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S1101Codes.MoveAgent;
                        webRequest.ListData.Add(orgInfo.OrgID.ToString());
                        webRequest.ListData.Add(strUserID);
                        Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                            WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                        WebHelper.SetServiceClient(client);
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            ShowException(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetLDAPUsers()
        {
            //打开窗口
            PopupPanel.Title = CurrentApp.GetLanguageInfo("11011600", "域服务器登录");
            UCLDAPLinkPage LdapPage = new UCLDAPLinkPage();
            LdapPage.ParenntPage = this;
            LdapPage.CurrentApp = CurrentApp;
            LdapPage.OrgObjItem = mCurrentObjectItem;
            PopupPanel.Content = LdapPage;
            PopupPanel.IsOpen = true;
        }

        private void ChangeObjectIndex(SelectedInfo selectInfo, ObjectItem parentItem, ObjectItem targetItem, bool isDown)
        {
            try
            {
                if (selectInfo == null || parentItem == null || targetItem == null) { return; }

                //用一个集合暂存待移动的项
                List<ObjectItem> listTemp = new List<ObjectItem>();
                for (int i = 0; i < selectInfo.ListItems.Count; i++)
                {
                    listTemp.Add(selectInfo.ListItems[i]);
                }
                for (int i = 0; i < listTemp.Count; i++)
                {
                    var tempItem = listTemp[i];
                    if (parentItem.Children.Contains(tempItem))
                    {
                        parentItem.Children.Remove(tempItem);
                    }
                }
                var index = parentItem.Children.IndexOf(targetItem);
                if (index >= 0)
                {
                    if (isDown)
                    {
                        for (int i = index + 1; i <= listTemp.Count + index; i++)
                        {
                            var tempItem = listTemp[i - index - 1];
                            if (i > parentItem.Children.Count) { parentItem.Children.Add(tempItem); }
                            else
                            {
                                parentItem.Children.Insert(i, tempItem);
                            }
                        }
                    }
                    else
                    {
                        for (int i = index; i < listTemp.Count + index; i++)
                        {
                            var tempItem = listTemp[i - index];
                            if (i > parentItem.Children.Count) { parentItem.Children.Add(tempItem); }
                            else
                            {
                                parentItem.Children.Insert(i, tempItem);
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

        public void BatchMove(SelectedInfo selectInfo, ObjectItem orgItem)
        {
            MoveObject(selectInfo, orgItem);
            ReloadData();
        }
        #endregion


        #region CommandHandlers

        private void AddOrgCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                AddOrg(objItem);
            }
        }

        private void DeleteOrgCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                DeleteOrg(objItem);
            }
        }

        private void ModifyOrgCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                ModifyOrg(objItem);
            }
        }

        private void AddUserCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                AddUser(objItem);
            }
        }

        private void DeleteUserCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                if (mSelectInfo == null)
                {
                    mSelectInfo = new SelectedInfo();
                }
                mSelectInfo.ObjType = S1101Consts.OBJTYPE_USER;
                for (int i = mSelectInfo.ListItems.Count - 1; i >= 0; i--)
                {
                    mSelectInfo.ListItems[i].IsSelected = false;
                    mSelectInfo.ListItems.Remove(mSelectInfo.ListItems[i]);
                }
                objItem.IsSelected = true;
                mSelectInfo.ListItems.Add(objItem);
                DeleteUser(objItem);
            }
        }

        private void ModifyUserCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                ModifyUser(objItem);
            }
        }

        private void SetUserRoleCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                SetUserRole(objItem);
            }
        }

        private void SetUserManagementCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                SetUserManagement(objItem);
            }
        }

        private void SetUserResourceManagementCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                SetUserResourceManagement(objItem);
            }
        }

        private void ImportUserDataCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                ImportWhat = "USER";
                ImportData(objItem);
            }
        }

        private void ImportAgentDataCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                ImportWhat = "AGENT";
                ImportData(objItem);
            }
        }

        private void LDAPCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as ObjectItem;
            if (objItem != null)
            {
                GetLDAPUsers();
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
                    //ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS1101;component/Themes/{0}/{1}",
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

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/UMPS1101;component/Themes/Default/UMPS1101/MainPageStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
        }

        #endregion


        #region ChangLanguage

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("FO1101", "UMP User and Organization Management");
                //Operation
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    ListOperations[i].Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", ListOperations[i].ID),
                        ListOperations[i].ID.ToString());
                }
                CreateOptButtons();

                //Other
                ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("11011000", "Basic Operations");
                ExpOtherPos.Header = CurrentApp.GetLanguageInfo("11011001", "Other Position");
                TabSampleView.Header = CurrentApp.GetLanguageInfo("11011010", "Sample View");
                TabGridView.Header = CurrentApp.GetLanguageInfo("11011011", "Grid View");
                TabDiagramView.Header = CurrentApp.GetLanguageInfo("11011012", "Diagram View");

                //列名 
                InitColumns();

                //详细信息
                ShowObjectDetail();

                //ListObjectItem
                for (int i = 0; i < mListObjectItems.Count; i++)
                {
                    ObjectItem item = mListObjectItems[i];
                    if (item.State == 1)
                    {
                        item.TipState = CurrentApp.GetLanguageInfo("1101T10109", "IsActived");
                    }
                    else
                    {
                        item.TipState = CurrentApp.GetLanguageInfo("1101T10110", "Disable");
                    }
                    if (item.ObjType == S1101Consts.OBJTYPE_USER)
                    {
                        BasicUserInfo userInfo = (BasicUserInfo)item.Data;
                        if (userInfo.IsLocked != "0")
                        {
                            if (item.LockMethod == "U")
                            {
                                item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10115", "Interface Lock");
                            }
                            else if (item.LockMethod == "L")
                            {
                                item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10116", "Safe Lock");
                            }
                            else
                            {
                                item.TipLockMethod = CurrentApp.GetLanguageInfo("1101T10111", "Other");
                            }
                        }
                    }
                    item.TipAddOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDORG), "AddOrg");
                    item.TipRemoveOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEORG),
                        "RemoveOrg");
                    item.TipModifyOrg = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYORG),
                        "ModifyOrg");
                    item.TipAddUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_ADDUSER), "AddUser");
                    item.TipRemoveUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_DELETEUSER),
                        "RemoveUser");
                    item.TipModifyUser = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_MODIFYUSER),
                        "ModifyUser");
                    item.TipSetUserRole = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERROLE),
                        "SetUserRole");
                    item.TipSetUserManagement =
                        CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1101Consts.OPT_SETUSERMANAGEMENT),
                            "SetUserManagement");
                    if (PopupPanel.IsOpen == true)
                    {
                        PopupPanel.ChangeLanguage();
                    }

                }
            }
            catch (Exception ex)
            { }
        }

        #endregion


        #region NetPipeMessage

        //protected override void App_NetPipeEvent(WebRequest webRequest)
        //{
        //    Dispatcher.Invoke(new Action(() =>
        //    {
        //        try
        //        {
        //            var code = webRequest.Code;
        //            var session = webRequest.Session;
        //            var strData = webRequest.Data;
        //            switch (code)
        //            {
        //                case (int)RequestCode.SCLanguageChange:
        //                    LangTypeInfo langTypeInfo =
        //                        CurrentApp.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
        //                    if (langTypeInfo != null)
        //                    {
        //                        LangTypeInfo = langTypeInfo;
        //                        CurrentApp.Session.LangTypeInfo = langTypeInfo;
        //                        CurrentApp.Session.LangTypeID = langTypeInfo.LangID;
        //                        SetBusy(true, string.Empty);
        //                        mWorker = new BackgroundWorker();
        //                        mWorker.DoWork += (s, de) => CurrentApp.InitAllLanguageInfos();
        //                        mWorker.RunWorkerCompleted += (s, re) =>
        //                        {
        //                            mWorker.Dispose();
        //                            SetBusy(false, string.Empty);
        //                            ChangeLanguage();
        //                            PopupPanel.ChangeLanguage();
        //                        };
        //                        mWorker.RunWorkerAsync();
        //                        //App.InitLanguageInfos();
        //                        //ChangeLanguage();
        //                        //PopupPanel.ChangeLanguage();
        //                        //PageHead.SessionInfo = CurrentApp.Session;
        //                        //PageHead.InitInfo();
        //                        //MyWaiter.Visibility = Visibility.Collapsed;
        //                    }
        //                    break;
        //                case (int)RequestCode.SCThemeChange:
        //                    ThemeInfo themeInfo = CurrentApp.Session.SupportThemes.FirstOrDefault(t => t.Name == strData);
        //                    if (themeInfo != null)
        //                    {
        //                        ThemeInfo = themeInfo;
        //                        CurrentApp.Session.ThemeInfo = themeInfo;
        //                        CurrentApp.Session.ThemeName = themeInfo.Name;
        //                        ChangeTheme();
        //                    }
        //                    break;
        //                //case (int)RequestCode.SCIdleCheckStop:
        //                //    StartStopIdleTimer(false);
        //                //    break;
        //                //case (int)RequestCode.SCIdleCheckStart:
        //                //    StartStopIdleTimer(true);
        //                //    break;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            //ShowException(ex.Message);
        //        }
        //    }));
        //}

        #endregion

        #region px
        private void Initparameter()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1101Codes.GetParameter;
                webRequest.Session = CurrentApp.Session;
                Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                WebHelper.SetServiceClient(client);
                //Service11011Client client = new Service11011Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Initparameter Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string paramenter = webReturn.Data;
                paramenter = paramenter.Substring(8, paramenter.Length - 8);
                paramenter = paramenter.Replace(ConstValue.SPLITER_CHAR.ToString(), "");
                InitParameter = paramenter;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadingIn()
        {
            try
            {
                System.Windows.Forms.OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
                fileDialog.InitialDirectory = "C:\\";
                fileDialog.Title = CurrentApp.GetLanguageInfo("1101T10220", "加载");
                //fileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx";
                fileDialog.Filter = "xlsx files (*.xlsx)|*.xlsx|xls files(*.xls)|*.xls";
                //fileDialog.FilterIndex = 0;
                fileDialog.RestoreDirectory = true;
                string namePath;
                if (fileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    namePath = fileDialog.FileName;
                    string nameexp = fileDialog.SafeFileName;
                    string[] expname = nameexp.Split('.');
                    if (expname[1].Length == 4)
                    {
                        Is2007 = true;
                    }
                    else
                    {
                        Is2007 = false;
                    }
                }
                else
                {
                    //CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N027", "没有选择任何文件"));
                    return;
                }
                Dataset = new DataSet();
                //=================================================================================================
                NPOIhelper = new NPOIHelper();
                List<string> ExcelName = NPOIhelper.GetSheetName(namePath, Is2007);
                //=================================================================================================
                PopupPanel.Title = CurrentApp.GetLanguageInfo("1101T10217", "Excel Sheet Name Selection");
                ExcelNamePage ExcNamePage = new ExcelNamePage();
                ExcNamePage.pageParent = this;
                ExcNamePage.ExcelName = ExcelName;
                ExcNamePage.ExcelPath = namePath;
                PopupPanel.Content = ExcNamePage;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                //CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N033", "无法读取Excel数据，请先下载安装Microsoft Office 2007"));
                ShowException(ex.Message);
            }

        }

        public void LoadData(string ExcelName, string ExcelPath)
        {
            switch (ImportWhat)
            {
                case "USER":
                    LoadUserData(ExcelName, ExcelPath);
                    #region 日志
                    string msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO1101010")));
                    CurrentApp.WriteOperationLog("1101010", ConstValue.OPT_RESULT_SUCCESS, msg);
                    #endregion
                    break;
                case "AGENT":
                    LoadAgentData(ExcelName, ExcelPath);
                    #region 日志
                    msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO1101011")));
                    CurrentApp.WriteOperationLog("1101011", ConstValue.OPT_RESULT_SUCCESS, msg);
                    #endregion
                    break;
                default:
                    break;
            }

            ReloadData(mRoot);
        }

        private void LoadUserData(string ExcelName, string ExcelPath)
        {
            //OleDbDataAdapter myCom = new OleDbDataAdapter();
            try
            {
                //myCom = new OleDbDataAdapter(String.Format(@"SELECT * FROM [{0}$]", ExcelName), ocl);
                //myCom.Fill(Dataset);

                Dataset = NPOIhelper.ExcelToDataSet(ExcelPath, ExcelName, Is2007);
                if (Dataset.Tables == null)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N028", "没有检测到数据"));
                    return;
                }
                if (Dataset.Tables.Count == 0)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N028", "没有检测到数据"));
                    return;
                }
                foreach (DataRow dr in Dataset.Tables[0].Rows)
                {
                    if (dr[0].ToString() == string.Empty || dr[0].ToString().Contains("@") || dr[0].ToString().Contains(@"\")) { continue; }
                    if (dr[1].ToString().Contains("@") || dr[1].ToString().Contains(@"\")) { continue; }
                    BasicUserInfo basicUserInfo = new BasicUserInfo();
                    basicUserInfo.SourceFlag = "E";

                    basicUserInfo.CreateTime = DateTime.Now;
                    basicUserInfo.StartTime = DateTime.Parse(dr[3].ToString());
                    basicUserInfo.EndTime = DateTime.Parse(S1101Consts.Default_StrEndTime.ToString());
                    basicUserInfo.StrCreateTime = basicUserInfo.CreateTime.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    basicUserInfo.StrStartTime = basicUserInfo.StartTime.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    basicUserInfo.StrEndTime = basicUserInfo.EndTime.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    basicUserInfo.Account = dr[0].ToString();
                    basicUserInfo.Creator = CurrentApp.Session.UserID;
                    basicUserInfo.FullName = dr[1].ToString();
                    basicUserInfo.OrgID = S1101Consts.ORG_ROOT;
                    basicUserInfo.Password = dr[2].ToString();
                    basicUserInfo.IsActived = "1";
                    basicUserInfo.IsOrgManagement = "0";

                    OperationReturn optReturn = XMLHelper.SeriallizeObject(basicUserInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S1101Codes.LoadNewUser;
                    webRequest.Data = optReturn.Data.ToString();
                    Service11011Client client = new Service11011Client(
                        WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(
                            CurrentApp.Session.AppServerInfo,
                            "Service11011"));
                    WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        if (webReturn.Code == Defines.RET_DBACCESS_EXIST)
                        {
                            ShowException(CurrentApp.GetMessageLanguageInfo("007", "User account already exist"));
                            continue;
                        }
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    basicUserInfo.UserID = Convert.ToInt64(webReturn.Data);
                }
                ReloadData(mRoot);
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N029", "Import User Data Successful"));

                string msg = string.Format("{0} {1} ", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO1101010"));
                CurrentApp.WriteOperationLog(S1101Consts.OPT_IMPORTUSERDATA.ToString(), ConstValue.OPT_RESULT_SUCCESS, msg);
            }
            catch (Exception ex)
            {
                ShowException(CurrentApp.GetLanguageInfo("1101N030", "excel表格式有误:") + ex.Message);

                #region 日志
                string msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO1101010"));
                CurrentApp.WriteOperationLog("1101010", ConstValue.OPT_RESULT_FAIL, msg);
                #endregion
            }
        }

        private void LoadAgentData(string ExcelName, string ExcelPath)
        {
            //OleDbDataAdapter myCom = new OleDbDataAdapter();
            try
            {
                //myCom = new OleDbDataAdapter(String.Format(@"SELECT * FROM [{0}$]", ExcelName), ocl);
                //myCom.Fill(Dataset);
                Dataset = NPOIhelper.ExcelToDataSet(ExcelPath, ExcelName, Is2007);
                if (Dataset.Tables == null)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N028", "没有检测到数据"));
                    return;
                }
                if (Dataset.Tables.Count == 0)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N028", "没有检测到数据"));
                    return;
                }
                List<string> ListAddFail = new List<string>();
                foreach (DataRow dr in Dataset.Tables[0].Rows)
                {
                    if (dr[0].ToString() == string.Empty) { continue; }
                    string agentID = dr[0].ToString();
                    string agentName = dr[1].ToString();
                    if (agentID != null && agentName != null)
                    {
                        string org = S1101Consts.ORG_ROOT.ToString();
                        string active = dr[2].ToString();
                        string AdminUser = "102" + CurrentApp.Session.RentInfo.Token + "00000000001";

                        WebRequest webRequest = new WebRequest();
                        List<string> LListStrWcfArgs = new List<string>();
                        OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                        LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());
                        LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());
                        LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token.ToString());
                        LListStrWcfArgs.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                        LListStrWcfArgs.Add("A");
                        LListStrWcfArgs.Add("0");
                        LListStrWcfArgs.Add(agentID);
                        LListStrWcfArgs.Add(string.Format("B01{0}{1}", ConstValue.SPLITER_CHAR, org));
                        LListStrWcfArgs.Add(string.Format("B07{0}{1}", ConstValue.SPLITER_CHAR, agentID));
                        LListStrWcfArgs.Add(string.Format("B08{0}{1}", ConstValue.SPLITER_CHAR, agentName));
                        LListStrWcfArgs.Add(string.Format("B02{0}{1}", ConstValue.SPLITER_CHAR, active));
                        LListStrWcfArgs.Add(string.Format("U00{0}{1}", ConstValue.SPLITER_CHAR, AdminUser));
                        if (CurrentApp.Session.UserInfo.UserID.ToString() != AdminUser)
                        {
                            LListStrWcfArgs.Add("U00" + ConstValue.SPLITER_CHAR + CurrentApp.Session.UserInfo.UserID.ToString());
                        }

                        Service00000Client client = new Service00000Client(
                            WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                            WebHelper.CreateEndpointAddress(
                                CurrentApp.Session.AppServerInfo,
                                "Service00000"));
                        WebHelper.SetServiceClient(client);
                        LWCFOperationReturn = client.OperationMethodA(27, LListStrWcfArgs);
                        string IStrCallReturn = LWCFOperationReturn.StringReturn;
                        bool IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                        client.Close();
                        if (!IBoolCallReturn)
                        {
                            //重复的要去更新
                            string AgentID = LWCFOperationReturn.ListStringReturn[0];
                            string AgentName = agentName;
                            try
                            {
                                WebRequest webRequestM = new WebRequest();
                                webRequestM.Code = (int)S1101Codes.MotifyAgentNameByAccount;
                                webRequestM.Session = CurrentApp.Session;
                                webRequestM.ListData.Add(AgentID);
                                webRequestM.ListData.Add(AgentName);
                                Service11011Client clientM = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                                WebHelper.SetServiceClient(clientM);
                                //Service11011Client client = new Service11011Client();
                                WebReturn webReturn = clientM.DoOperation(webRequestM);
                                clientM.Close();
                                if (!webReturn.Result)
                                {
                                    ShowException(string.Format("MotifyAgentNameByAccount Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                ShowException(ex.Message);
                            }
                        }
                    }
                }
                ReloadData(mRoot);
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N029", "Import Agent Data Successful"));

                string msg = string.Format("{0} {1} ", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO1101011"));
                CurrentApp.WriteOperationLog(S1101Consts.OPT_IMPORTAGENTDATA.ToString(), ConstValue.OPT_RESULT_SUCCESS, msg);
            }
            catch (Exception ex)
            {
                ShowException(CurrentApp.GetLanguageInfo("1101N030", "excel表格式有误:") + ex.Message);
                #region 日志
                string msg = string.Format("{0}{1}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO1101011")));
                CurrentApp.WriteOperationLog("1101011", ConstValue.OPT_RESULT_FAIL, msg);
                #endregion
            }
            //finally
            //{
            //    ocl.Dispose();
            //    myCom.Dispose();
            //}
        }

        private void ReadUserDataFromDB()//从数据库获取用户信息，传给DataSet
        { }

        private void ReadAgentDataFromDB()//从数据库获取坐席信息，传给DataSet
        { }

        //释放可能还没释放的进程
        private bool KillAllExcel(Microsoft.Office.Interop.Excel.Application excelApp)
        {
            try
            {
                if (excelApp != null)
                {
                    excelApp.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    //释放COM组件，其实就是将其引用计数减1      
                    //System.Diagnostics.Process theProc;      
                    foreach (System.Diagnostics.Process theProc in System.Diagnostics.Process.GetProcessesByName("EXCEL"))
                    {
                        //先关闭图形窗口。如果关闭失败.有的时候在状态里看不到图形窗口的excel了，      
                        //但是在进程里仍然有EXCEL.EXE的进程存在，那么就需要释放它      
                        if (theProc.CloseMainWindow() == false)
                        {
                            theProc.Kill();
                        }
                    }
                    excelApp = null;
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void LoadExcel()
        {
            #region 创建excel

            Microsoft.Win32.SaveFileDialog objSFD = new Microsoft.Win32.SaveFileDialog() { DefaultExt = "xlsx", Filter = "Excel Files (*.xlsx)|*.xlsx|All files (*.*)|*.*", FilterIndex = 1 };
            objSFD.ShowDialog();

            string strFormat = objSFD.FileName;
            //创建Excel
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook excelWB = excelApp.Workbooks.Add(System.Type.Missing);    //创建工作簿（WorkBook：即Excel文件主体本身）   
            Microsoft.Office.Interop.Excel.Worksheet excelWS = (Microsoft.Office.Interop.Excel.Worksheet)excelWB.Worksheets[1];   //创建工作表（即Excel里的子表sheet） 1表示在子表sheet1里进行数据导出   
            excelWS.Cells.NumberFormat = "@";     //  如果数据中存在数字类型 可以让它变文本格式显示   
            //将数据导入到工作表的单元格  


            #endregion

            int j = 0;//第几列
            foreach (DataGridColumn column in Dataset.Tables[0].Rows)
            {
                j++;
                excelWS.Cells[1, j] = column.Header.ToString();//===========给excel插入表头内容
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            int i = 2;//行  
            int asd = 0;
            foreach (DataRow row in Dataset.Tables[0].Rows)
            {
                int d = 1;
                foreach (DataGridColumn column in Dataset.Tables[0].Rows)
                {
                    FrameworkElement element = column.GetCellContent(row);
                    if (element is TextBlock)
                    {
                        string str = (element as TextBlock).Text;
                        excelWS.Cells[i, d] = str;//================给excel插入内容=============
                        asd++;
                    }
                    d++;
                }
                //this.Dispatcher.Invoke((Action)(() =>
                //{
                //    this.Title = i.ToString() + "行" + "插入了" + asd + "列。这里有" + dgv.Items.Count + "行数据";
                //}));
                i++;
            }

            watch.Stop();
            System.Windows.MessageBox.Show(watch.Elapsed.Seconds.ToString());


            #region  保存时候
            //将其进行保存到指定的路径（先导出2007版本，当电脑没装2007版本时导出报错，直接进catch导出2003版本） 
            try
            {
                //导出2007版本
                excelWB.SaveAs(strFormat, 56, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            }
            catch (Exception)
            {
                //  导出2003版本，在2007版本下面打开有提示后缀名问题
                excelWB.SaveAs(strFormat, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
            }

            excelWB.Close();
            excelApp.Quit();
            KillAllExcel(excelApp); //释放可能还没释放的进程   
            #endregion

        }
        #endregion
    }
}
