using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using PFShareClassesC;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Common;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11021;
using UMPS1102.Models;
using UMPS1102.Commands;
using UMPS1102.Converters;
using UMPS1102.Wcf11021;
using System.Timers;
using UMPS1102.Wcf11012;




namespace UMPS1102
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class RoleManage
    {

        #region Members

        //机构下用户
        private ObjectItem mUserORGRoot;

        //机构下座席
        private ObjectItem mAgentORGRoot;

        //座席专用的权限树
        private ObjectItem mAgentPermissionRoot;

        //创建权限树 
        private ObjectItem mPermissionRoot;

        //创建按钮
        private ObservableCollection<OperationInfo> mListBasicOperations;

        //当前人管理的机构
        private List<OrganizationInfo> lstCtrolOrgInfos;
        //当前人管理的用户
        private List<BasicUserInfo> lstCtrolUserInfos;
        //当前人管理的全部座席
        private List<UCtrolAgent> lstCtrolAgentInfos;

        //存放被隐藏的权限
        private List<ROperationInfo> lstHidePermission;
        private string StrHidePremission;

        private BackgroundWorker mWorker;
        #endregion
        //private S1102App CurrentApp;

        #region StaticMembers
        //所有权限
        public static ObservableCollection<ROperationInfo> ListOperations = new ObservableCollection<ROperationInfo>();
        //所有角色
        public static ObservableCollection<RoleModel> ListRoleModels = new ObservableCollection<RoleModel>();
        //当前角色对应的用户
        public static ObservableCollection<RoleUsersInfo> ListRoleUsers = new ObservableCollection<RoleUsersInfo>();
        #endregion


        public RoleManage()
        {
            InitializeComponent();

            mUserORGRoot = new ObjectItem();
            mAgentORGRoot = new ObjectItem();

            mPermissionRoot = new ObjectItem();
            mAgentPermissionRoot = new ObjectItem();
            mListBasicOperations = new ObservableCollection<OperationInfo>();

            lstCtrolOrgInfos = new List<OrganizationInfo>();
            lstCtrolUserInfos = new List<BasicUserInfo>();
            lstCtrolAgentInfos = new List<UCtrolAgent>();
            lstHidePermission = new List<ROperationInfo>();
        }

        /// <summary>
        /// 初始化用户信息以及样式
        /// </summary>
        protected override void Init()
        {
            try
            {
                PageName = "RoleManage";
                StylePath = "UMPS1102/RoleManageStyle.xaml";

                LVRoleList.ItemsSource = ListRoleModels;
                LVRoleList.SelectionChanged += LVRoleList_SelectedItemChanged;
                TCPermissionAndUser.SelectionChanged += TCPermissionAndUser_Selected;
                GTUsersList.ItemsSource = mUserORGRoot.Children;
                GTPermissionsList.ItemsSource = mPermissionRoot.Children;

                base.Init();

                BindCommands();
                ListRoleModels.Clear();
                SetBusy(true, string.Empty);
                InitRoles();
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    //InitRoles();

                    InitOperations();

                    InitControledOrgsAndUsers();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);
                    InitRoleColumns();
                    InitUsersColumns();
                    InitPermissionColumns();
                    InitBasicOperations();
                    ChangListRoleModelsData();
                    ChangeLanguage();
                    //触发Loaded消息
                    CurrentApp.SendLoadedMessage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region CommandInit
        private void BindCommands()
        {
            CommandBindings.Add(
                new CommandBinding(URMainPageCommands.AddRoleCommand,
                    AddRoleCommand_Executed,
                    (s, e) => e.CanExecute = true));
            CommandBindings.Add(
               new CommandBinding(URMainPageCommands.DeleteRoleCommand,
                   DeleteRoleCommand_Executed,
                   (s, e) => e.CanExecute = true));
            CommandBindings.Add(
               new CommandBinding(URMainPageCommands.ModifyRoleCommand,
                   ModifyRoleCommand_Executed,
                   (s, e) => e.CanExecute = true));
            CommandBindings.Add(
             new CommandBinding(URMainPageCommands.SubmitRolePermissionsCommand,
                 SubmitRolePermissionsCommand_Executed,
                 (s, e) => e.CanExecute = true));
            CommandBindings.Add(
               new CommandBinding(URMainPageCommands.SubmitRoleUsersCommand,
                   SubmitRoleUsersCommand_Executed,
                   (s, e) => e.CanExecute = true));
        }


        /// <summary>
        /// 列表的按钮触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddRoleCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as RoleModel;
            if (objItem != null)
            {
                AddRole();
            }
        }
        private void DeleteRoleCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as RoleModel;
            if (objItem != null)
            {
                bool iswork = false;
                for (int i = 0; i < S1102App.ListOperationInfos.Count; i++)
                {
                    OperationInfo item = S1102App.ListOperationInfos[i];
                    if (item.ID == S1102Consts.OPT_DeleteRole)
                    { iswork = true; break; }
                }
                if (iswork)
                { DeleteRole(objItem); }
                else { CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102D0001", "您没有此权限！")); }
            }
        }
        private void ModifyRoleCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var objItem = e.Parameter as RoleModel;
            if (objItem != null)
            {
                bool iswork = false;
                for (int i = 0; i < S1102App.ListOperationInfos.Count; i++)
                {
                    OperationInfo item = S1102App.ListOperationInfos[i];
                    if (item.ID == S1102Consts.OPT_ModifyRole)
                    { iswork = true; break; }
                }
                if (iswork)
                { ModifyRole(objItem); }
                else { CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102D0001", "您没有此权限！")); }
               
            }
        }
        private void SubmitRolePermissionsCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }
        private void SubmitRoleUsersCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }

        #endregion

        #region Tab
        private void TCPermissionAndUser_Selected(object sender, RoutedEventArgs e)
        {
            InitBasicOperations();
        }
        #endregion

        #region OperationButton And PermissionTree

        private void InitOperations()
        {
            ListOperations.Clear();
            ClearChildren(mPermissionRoot);
            ClearChildren(mAgentPermissionRoot);
            InitControledOperations();
            if (ListOperations.Count() > 0)
            {

                InitmPermissionRoot(mPermissionRoot, 0);
                InitmAgentPermissionRoot();

            }
        }

        private void InitmAgentPermissionRoot()
        {
            //if (ListOperations.Where(p => p.ID == 3104 && p.ParentID == 31).Count() > 0)
            ////if (ListOperations.Where(p => p.ID == 31 && p.ParentID == 0).Count() > 0)
            //{
            //    ObjectItem item = new ObjectItem();
            //    //ROperationInfo rOperationTemp = ListOperations.Where(p => p.ID == 31 && p.ParentID == 0).First();
            //    ROperationInfo rOperationTemp = ListOperations.Where(p => p.ID == 3104 && p.ParentID == 31).First();
            //    item.ObjType = S1102Consts.OBJTYPE_PERMISSIONS;
            //    item.ObjID = rOperationTemp.ID;
            //    item.Name = App.GetLanguageInfo(string.Format("FO{0}", rOperationTemp.ID), rOperationTemp.Display);
            //    item.FullName = rOperationTemp.Description;
            //    item.Data = rOperationTemp;
            //    item.IsCanUse = "0";
            //    item.IsCanDownAssign = "0";
            //    item.IsCanCascadeRecycle = "0";
            //    //item.IsExpanded = true;
            //    item.IsExpanded = false;
            //    item.IsChecked = false;
            //    AddChildObjectItem(mAgentPermissionRoot, item);
            if (ListOperations.Where(p => p.ID == 3104 && p.ParentID == 31).Count() > 0)
            {
                ObjectItem item1 = new ObjectItem();
                ROperationInfo rOperationTemp = ListOperations.Where(p => p.ID == 3104 && p.ParentID == 31).First();
                item1.ObjType = S1102Consts.OBJTYPE_PERMISSIONS;
                item1.ObjID = rOperationTemp.ID;
                item1.Name = CurrentApp.GetLanguageInfo(string.Format("FO{0}", rOperationTemp.ID), rOperationTemp.Display);
                item1.FullName = rOperationTemp.Description;
                item1.Data = rOperationTemp;
                item1.IsCanUse = "0";
                item1.IsCanDownAssign = "0";
                item1.IsCanCascadeRecycle = "0";
                //item1.IsExpanded = true;
                item1.IsExpanded = false;
                item1.IsChecked = false;
                item1.IsHidden = !(rOperationTemp.IsHide == "H");
                //AddChildObjectItem(item, item1);
                AddChildObjectItem(mAgentPermissionRoot, item1);
                if (ListOperations.Where(p => p.ParentID == rOperationTemp.ID).Count() > 0)
                {
                    InitmPermissionRoot(item1, rOperationTemp.ID);
                }
            }
            //}
        }

        //构建权限树
        private void InitmPermissionRoot(ObjectItem objectParent, long parantId)
        {
            List<ROperationInfo> lstRoleOperationTemp = ListOperations.Where(p => p.ParentID == parantId && p.IsCanDownAssign == "1").ToList();
            foreach (ROperationInfo ROperation in lstRoleOperationTemp)
            {

                ObjectItem item = new ObjectItem();
                item.ObjType = S1102Consts.OBJTYPE_PERMISSIONS;
                item.ObjID = ROperation.ID;
                //item.Name = ROperation.Display;
                item.Name = CurrentApp.GetLanguageInfo(string.Format("FO{0}", ROperation.ID), ROperation.Display);
                item.FullName = ROperation.Description;
                item.Data = ROperation;
                item.IsCanUse = "0";
                item.IsCanDownAssign = "0";
                item.IsCanCascadeRecycle = "0";
                //item.IsExpanded = true;
                item.IsExpanded = false;
                item.IsChecked = false;
                item.IsHidden = !(ROperation.IsHide == "H");

                if (ROperation.ID.ToString().Count() <= 2)
                {
                    InitmPermissionRoot(objectParent, ROperation.ID);
                    lstHidePermission.Add(ROperation);
                }
                else
                {
                    AddChildObjectItem(objectParent, item);
                    if (ListOperations.Where(p => p.ParentID == ROperation.ID).Count() > 0)
                    {
                        InitmPermissionRoot(item, ROperation.ID);
                    }
                }

            }
        }

        //得到当前用户所有角色的权限并集
        private void InitControledOperations()
        {
            try
            {
                #region 获取权限（改了）
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1102Codes.GetOperationList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                //Service11021Client client =new Service11021Client();
                Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.URPOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                //string msgID = "";
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ROperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ROperationInfo optInfo = optReturn.Data as ROperationInfo;

                    if (optInfo.ID.ToString() == "1111" && CurrentApp.Session.RentInfo.Token != "00000")
                    {
                        continue;
                    }

                    if (optInfo != null)
                    {
                        optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        //msgID += optInfo.Display + "\n";
                        ListOperations.Add(optInfo);
                    }
                    //App.WriteLog(msgID);
                }
                #endregion

                //权限合并
                List<ROperationInfo> lstROperationTemp = new List<ROperationInfo>();
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    ROperationInfo ROperationInfo = ListOperations[i];
                    if (lstROperationTemp.Where(p => p.ID == ROperationInfo.ID).Count() > 0)
                    {
                        ROperationInfo ROperationOld = lstROperationTemp.Where(p => p.ID == ROperationInfo.ID).First();
                        if (ROperationInfo.IsCanUse.Equals("1") || ROperationOld.IsCanUse.Equals("1"))
                        {
                            ROperationOld.IsCanUse = "1";
                        }
                        if (ROperationInfo.IsCanDownAssign.Equals("1") || ROperationOld.IsCanDownAssign.Equals("1"))
                        {
                            ROperationOld.IsCanDownAssign = "1";
                        }
                    }
                    else
                    {
                        lstROperationTemp.Add(ListOperations[i]);
                    }
                }

                ListOperations = new ObservableCollection<ROperationInfo>(lstROperationTemp.OrderBy(p => p.ID).ToList());
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitBasicOperations()
        {

            for (int i = 0; i < S1102App.ListOperationInfos.Count; i++)
            {
                S1102App.ListOperationInfos[i].Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S1102App.ListOperationInfos[i].ID),
                    S1102App.ListOperationInfos[i].ID.ToString());
            }
            mListBasicOperations.Clear();
            var objItem = LVRoleList.SelectedItem as RoleModel;
            if (objItem != null)
            {
                for (int i = 0; i < S1102App.ListOperationInfos.Count; i++)
                {
                    OperationInfo item = S1102App.ListOperationInfos[i];
                    if (item.ID == S1102Consts.OPT_AddRole)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1102Consts.OPT_ModifyRole)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1102Consts.OPT_DeleteRole)
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1102Consts.OPT_SubmitRolePermissions && (TCPermissionAndUser.SelectedIndex == 0))
                    {
                        mListBasicOperations.Add(item);
                    }
                    if (item.ID == S1102Consts.OPT_SubmitRoleUsers && (TCPermissionAndUser.SelectedIndex == 1))
                    {
                        mListBasicOperations.Add(item);
                    }
                }
            }
            else
            {
                for (int i = 0; i < S1102App.ListOperationInfos.Count; i++)
                {
                    OperationInfo item = S1102App.ListOperationInfos[i];
                    if (item.ID == S1102Consts.OPT_AddRole)
                    {
                        mListBasicOperations.Add(item);
                    }
                }
            }
            CreateOptButtons();
        }

        private void CreateOptButtons()
        {
            PanelBasicOpts.Children.Clear();
            OperationInfo item;
            Button btn;
            for (int i = 0; i < mListBasicOperations.Count; i++)
            {
                item = mListBasicOperations[i];
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }
        }

        //提交角色对应权限
        private void SubmitRolePermissions(RoleModel RoleInfo)
        {
            SetBusy(true,string.Empty);
            if (RoleInfo != null)
            {
                long roleid = RoleInfo.RoleID;

                if (RoleInfo.RoleID < S1102Consts.RoleID_Limit)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00029", "Syste Role Permission Can''t  Modify"));
                    return;
                }
                List<string> lstRolePermissions = new List<string>();
                if (roleid == S1102App.LongParse(string.Format(S1102Consts.ROLE_SYSTEMAGENT, S1102Consts.RENT_DEFAULT_TOKEN), 0))
                {
                    StrHidePremission = string.Empty;
                    CommpleteRolePermission(roleid, ref lstRolePermissions, mAgentPermissionRoot);
                }
                else
                {
                    StrHidePremission = string.Empty;
                    CommpleteRolePermission(roleid, ref lstRolePermissions, mPermissionRoot);
                }
                GetHidePermission(roleid, ref lstRolePermissions);
                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S1102Codes.SubmitRolePermission;
                    webRequest.ListData = lstRolePermissions;
                    webRequest.Data = roleid.ToString();
                    Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                    //Service11021Client client  = new Service11021Client();
                    WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.URPOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    }
                    else
                    {
                        #region 记录日志
                        string strAdded = string.Empty;
                        string strRemoved = string.Empty;
                        List<string> listLogParams = new List<string>();
                        if (webReturn.ListData != null && webReturn.ListData.Count > 0)
                        {
                            for (int i = 0; i < webReturn.ListData.Count; i++)
                            {
                                if (i > 10)
                                { break; }
                                string strInfo = webReturn.ListData[i];
                                string[] arrInfos = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                                    StringSplitOptions.RemoveEmptyEntries);
                                if (arrInfos.Length >= 2)
                                {
                                    if (arrInfos[0] == "A")
                                    {
                                        //strAdded += "FO" + arrInfos[1] + ",";

                                        strAdded += Utils.FormatOptLogString("FO" + arrInfos[1]) + ",";
                                    }
                                    if (arrInfos[0] == "D")
                                    {
                                        strRemoved += Utils.FormatOptLogString("FO" + arrInfos[1]) + ",";
                                        //strRemoved += "FO" + arrInfos[1] + ",";
                                    }
                                }
                            }
                            //strAdded = strAdded.TrimEnd(new[] { ',' });
                            strRemoved = strRemoved.TrimEnd(new[] { ',' });
                        }
                        listLogParams.Add(RoleInfo.RoleName);
                        listLogParams.Add(strAdded);
                        listLogParams.Add(strRemoved);
                        CurrentApp.WriteOperationLog(S1102Consts.OPT_SubmitRolePermissions.ToString(), ConstValue.OPT_RESULT_SUCCESS, "1102Log0001", listLogParams);
                        #endregion

                        //要更新该权限所有用户的权限
                        List<string> lstRoleUsers = new List<string>();
                        foreach (RoleUsersInfo userinfo in ListRoleUsers)
                        {
                            if (userinfo.UserID != S1102App.LongParse(string.Format(S1102Consts.USER_ADMIN, CurrentApp.Session.RentInfo.Token), 0))
                            {
                                OperationReturn optReturn = XMLHelper.SeriallizeObject(userinfo);
                                lstRoleUsers.Add(optReturn.Data.ToString());
                            }
                        }
                        if (lstRoleUsers.Count > 0)
                        {
                            webReturn = new WebReturn();
                            webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)S1102Codes.UpdateUserPerimission;
                            webRequest.ListData = lstRoleUsers;
                            client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                            //client = new Service11021Client();
                            WebHelper.SetServiceClient(client);
                            webReturn = client.URPOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                                ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                            }
                            else
                            {
                                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00030", "Submit Success"));
                            }
                        }
                        else
                        {
                            CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00030", "Submit Success"));
                        }

                    }
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }

            }
            SetBusy(false,string.Empty);
        }

        private void CommpleteRolePermission(long roleid, ref List<string> lstRolePermissions, ObjectItem parent)
        {
            foreach (ObjectItem o in parent.Children)
            {
                if (o.IsChecked == null || o.IsChecked == true)
                {
                    RolePermissionInfo newrolepermission = new RolePermissionInfo();
                    newrolepermission.RoleID = roleid;
                    newrolepermission.PermissionID = o.ObjID;
                    newrolepermission.IsCanUse = o.IsCanUse.Equals("1") ? "1" : "0";
                    newrolepermission.IsCanDownAssign = o.IsCanDownAssign.Equals("1") ? "1" : "0";
                    newrolepermission.IsCanCascadeRecycle = o.IsCanCascadeRecycle.Equals("1") ? "1" : "0";
                    newrolepermission.ModifyID = CurrentApp.Session.UserInfo.UserID;
                    newrolepermission.StrModifyTime = DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    newrolepermission.StrEnableTime = DateTime.Parse(S1102Consts.Default_StrStartTime).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    newrolepermission.StrEndTime = DateTime.Parse(S1102Consts.Default_StrEndTime).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    newrolepermission.IsDelete = false;
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(newrolepermission);
                    lstRolePermissions.Add(optReturn.Data.ToString());
                    //GetHidePermission(roleid, o.ObjID, ref lstRolePermissions, false);
                    string Codeid = o.ObjID.ToString().Substring(0, 2);
                    StrHidePremission += Codeid + ",";
                }
                else
                {
                    RolePermissionInfo newrolepermission = new RolePermissionInfo();
                    newrolepermission.RoleID = roleid;
                    newrolepermission.PermissionID = o.ObjID;

                    newrolepermission.IsCanUse = o.IsCanUse.Equals("1") ? "1" : "0";
                    newrolepermission.IsCanDownAssign = o.IsCanDownAssign.Equals("1") ? "1" : "0";
                    newrolepermission.IsCanCascadeRecycle = o.IsCanCascadeRecycle.Equals("1") ? "1" : "0";
                    newrolepermission.ModifyID = CurrentApp.Session.UserInfo.UserID;
                    newrolepermission.StrModifyTime = DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    newrolepermission.StrEnableTime = DateTime.Parse(S1102Consts.Default_StrStartTime).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    newrolepermission.StrEndTime = DateTime.Parse(S1102Consts.Default_StrEndTime).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");

                    ////隐藏的操作默认就有权限，不能删除掉           by charley at 2017/2/4
                    
                    //newrolepermission.IsDelete = true;
                    if (!o.IsHidden)
                    {
                        newrolepermission.IsDelete = false;
                    }
                    else
                    {
                        newrolepermission.IsDelete = true;
                    }

                    OperationReturn optReturn = XMLHelper.SeriallizeObject(newrolepermission);
                    lstRolePermissions.Add(optReturn.Data.ToString());
                    //GetHidePermission(roleid, o.ObjID, ref lstRolePermissions, true);
                }

                if (o.Children.Count > 0)
                {
                    CommpleteRolePermission(roleid, ref lstRolePermissions, o);
                }
            }
        }

        private void GetHidePermission(long roleid, ref List<string> lstRolePermissions)
        {
            for (int i = 0; i < lstHidePermission.Count; i++)
            {
                ROperationInfo LHidePermission = lstHidePermission[i];
                RolePermissionInfo newrolepermission = new RolePermissionInfo();
                newrolepermission.RoleID = roleid;
                newrolepermission.PermissionID = LHidePermission.ID;
                newrolepermission.IsCanUse = LHidePermission.IsCanUse.Equals("1") ? "1" : "0";
                newrolepermission.IsCanDownAssign = LHidePermission.IsCanDownAssign.Equals("1") ? "1" : "0";
                newrolepermission.IsCanCascadeRecycle = LHidePermission.IsCanCascadeRecycle.Equals("1") ? "1" : "0";
                newrolepermission.ModifyID = CurrentApp.Session.UserInfo.UserID;
                newrolepermission.StrModifyTime = DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                newrolepermission.StrEnableTime = DateTime.Parse(S1102Consts.Default_StrStartTime).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                newrolepermission.StrEndTime = DateTime.Parse(S1102Consts.Default_StrEndTime).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");

                string HideCode = LHidePermission.ID.ToString();
                if (StrHidePremission.Contains(HideCode))
                {
                    newrolepermission.IsDelete = false;
                }
                else
                {
                    newrolepermission.IsDelete = true;
                }
                OperationReturn optReturn = XMLHelper.SeriallizeObject(newrolepermission);

                lstRolePermissions.Add(optReturn.Data.ToString());
            }
        }

        private void CommpleteRoleUsers(long roleid, ref List<string> lstRoleUsers, ObjectItem parent)
        {
            foreach (ObjectItem o in parent.Children)
            {
                if (o.IsChecked == true && o.ObjType != S1102Consts.OBJTYPE_ORG)
                {
                    RoleUsersInfo newroleuser = new RoleUsersInfo();
                    newroleuser.ParentID = 0;
                    newroleuser.RoleID = roleid;
                    newroleuser.UserID = o.ObjID;
                    //此片两个时间暂时预留没用
                    newroleuser.StrStartTime = DateTime.Parse(S1102Consts.Default_StrStartTime).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    newroleuser.StrEndTime = DateTime.Parse(S1102Consts.Default_StrEndTime).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    newroleuser.IsDelete = false;
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(newroleuser);
                    lstRoleUsers.Add(optReturn.Data.ToString());
                }
                else if (o.ObjType != S1102Consts.OBJTYPE_ORG)
                {
                    //如果静态list不存在，则不用去更新
                    if (ListRoleUsers.Where(p => p.UserID == o.ObjID).Count() > 0)
                    {
                        RoleUsersInfo newroleuser = new RoleUsersInfo();
                        newroleuser.ParentID = 0;
                        newroleuser.RoleID = roleid;
                        newroleuser.UserID = o.ObjID;
                        newroleuser.IsDelete = true;
                        OperationReturn optReturn = XMLHelper.SeriallizeObject(newroleuser);
                        lstRoleUsers.Add(optReturn.Data.ToString());
                    }
                }
                if (o.Children.Count > 0 && o.ObjType == S1102Consts.OBJTYPE_ORG)
                {
                    CommpleteRoleUsers(roleid, ref lstRoleUsers, o);
                }
            }
        }

        //提交角色对应用户
        private void SubmitRoleUsers(RoleModel RoleInfo)
        {
            SetBusy(true,string.Empty);
            if (RoleInfo != null)
            {
                long roleid = RoleInfo.RoleID;
                List<string> lstRoleUsers = new List<string>();
                if (RoleInfo.RoleID == S1102App.LongParse(string.Format(S1102Consts.ROLE_SYSTEMAGENT, S1102Consts.RENT_DEFAULT_TOKEN), 0))
                {
                    CommpleteRoleUsers(roleid, ref lstRoleUsers, mAgentORGRoot);
                }
                else
                {
                    CommpleteRoleUsers(roleid, ref lstRoleUsers, mUserORGRoot);
                }

                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S1102Codes.SubmitRoleUser;
                    webRequest.ListData = lstRoleUsers;
                    webRequest.Data = roleid.ToString();
                    Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                    //Service11021Client client =  new Service11021Client();
                    WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.URPOperation(webRequest);
                    client.Close();

                    #region 记录日志

                    string strAdded = string.Empty;
                    string strRemoved = string.Empty;
                    List<string> listLogParams = new List<string>();
                    if (webReturn.ListData != null && webReturn.ListData.Count > 0)
                    {
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            string strInfo = webReturn.ListData[i];
                            string[] arrInfos = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.RemoveEmptyEntries);
                            if (arrInfos.Length >= 2)
                            {
                                if (arrInfos[0] == "A")
                                {
                                    var objItem =
                                        lstCtrolUserInfos.FirstOrDefault(o => o.UserID.ToString() == arrInfos[1]);
                                    if (objItem != null)
                                    {
                                        strAdded += objItem.Account + ",";
                                    }
                                    else
                                    {
                                        strAdded += arrInfos[1] + ",";
                                    }
                                }
                                if (arrInfos[0] == "D")
                                {
                                    var objItem =
                                        lstCtrolUserInfos.FirstOrDefault(o => o.UserID.ToString() == arrInfos[1]);
                                    if (objItem != null)
                                    {
                                        strRemoved += objItem.Account + ",";
                                    }
                                    else
                                    {
                                        strRemoved += arrInfos[1] + ",";
                                    }
                                }
                            }
                        }
                        strAdded = strAdded.TrimEnd(new[] { ',' });
                        strRemoved = strRemoved.TrimEnd(new[] { ',' });
                    }
                    listLogParams.Add(RoleInfo.RoleName);
                    listLogParams.Add(strAdded);
                    listLogParams.Add(strRemoved);
                    CurrentApp.WriteOperationLog(S1102Consts.OPT_SubmitRoleUsers.ToString(), ConstValue.OPT_RESULT_SUCCESS, "1102Log0002", listLogParams);
                    #endregion

                    ListRoleUsers.Clear();

                    IsCheckUser();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    }
                    else
                    {
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00030", "Submit Success"));
                    }
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
            }
            SetBusy(false,string.Empty);
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                RoleModel objItem;
                if (optItem != null)
                {
                    switch (optItem.ID)
                    {
                        case S1102Consts.OPT_AddRole:
                            objItem = LVRoleList.SelectedItem as RoleModel;
                            AddRole();
                            break;
                        case S1102Consts.OPT_DeleteRole:
                            objItem = LVRoleList.SelectedItem as RoleModel;
                            DeleteRole(objItem);
                            break;
                        case S1102Consts.OPT_ModifyRole:
                            objItem = LVRoleList.SelectedItem as RoleModel;
                            ModifyRole(objItem);
                            break;
                        case S1102Consts.OPT_SubmitRolePermissions:
                            {
                                SetBusy(true, string.Empty);
                                objItem = LVRoleList.SelectedItem as RoleModel;
                                SubmitRolePermissions(objItem);
                                SetBusy(false, string.Empty);
                            }
                            break;
                        case S1102Consts.OPT_SubmitRoleUsers:
                            {
                                SetBusy(true, string.Empty);
                                objItem = LVRoleList.SelectedItem as RoleModel;
                                SubmitRoleUsers(objItem);
                                SetBusy(false, string.Empty);
                            }
                            break;
                    }
                }
            }
        }

        //查询所有权限在资源表的信息，确定是否打勾
        void IsCheckPermission()
        {
            var objItem = LVRoleList.SelectedItem as RoleModel;
            if (objItem != null)
            {
                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S1102Codes.GetRolePermission;
                    webRequest.Session = CurrentApp.Session;
                    webRequest.ListData.Add(objItem.RoleID.ToString());
                    //Service11021Client client = new Service11021Client();
                    Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                    WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.URPOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }

                    List<RolePermissionInfo> lstRolePermissionTemp = new List<RolePermissionInfo>();
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<RolePermissionInfo>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        RolePermissionInfo roleInfo = optReturn.Data as RolePermissionInfo;
                        if (roleInfo != null)
                        {
                            lstRolePermissionTemp.Add(roleInfo);
                        }
                    }
                    if (objItem.RoleID == S1102App.LongParse(string.Format(S1102Consts.ROLE_SYSTEMAGENT, S1102Consts.RENT_DEFAULT_TOKEN), 0))
                    {
                        ClearmPermissionRootCheck(mAgentPermissionRoot);
                        if (lstRolePermissionTemp.Count > 0)
                        {
                            CheckmpermissionRoot(mAgentPermissionRoot, lstRolePermissionTemp);
                        }
                    }
                    else
                    {
                        ClearmPermissionRootCheck(mPermissionRoot);
                        if (lstRolePermissionTemp.Count > 0)
                        {
                            CheckmpermissionRoot(mPermissionRoot, lstRolePermissionTemp);
                        }
                    }

                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
            }
        }

        //先全清
        void ClearmPermissionRootCheck(ObjectItem objectParent)
        {
            foreach (ObjectItem item in objectParent.Children)
            {
                item.IsCanUse = "1";
                item.IsCanDownAssign = "1";
                item.IsCanCascadeRecycle = "0";
                //item.IsExpanded = true;
                item.IsExpanded = false;
                item.IsChecked = false;
                if (objectParent.Children.Count > 0)
                {
                    ClearmPermissionRootCheck(item);
                }
            }
        }

        //选中
        void CheckmpermissionRoot(ObjectItem objectParent, List<RolePermissionInfo> lstRolePermissionTemp)
        {
            foreach (ObjectItem item in objectParent.Children)
            {
                foreach (RolePermissionInfo roleInfo in lstRolePermissionTemp)
                {
                    if (item.ObjID == roleInfo.PermissionID)
                    {
                        item.IsCanUse = string.IsNullOrEmpty(roleInfo.IsCanUse) ? "0" : roleInfo.IsCanUse;
                        item.IsCanDownAssign = string.IsNullOrEmpty(roleInfo.IsCanDownAssign) ? "0" : roleInfo.IsCanDownAssign;
                        item.IsCanCascadeRecycle = string.IsNullOrEmpty(roleInfo.IsCanCascadeRecycle) ? "0" : roleInfo.IsCanCascadeRecycle;
                        //item.IsExpanded = true;
                        item.IsExpanded = false;
                        if (item.Children.Count > 0)
                        {
                            item.IsChecked = null;
                            CheckmpermissionRoot(item, lstRolePermissionTemp);
                        }
                        else
                        {
                            item.IsChecked = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 初始化权限列
        /// </summary>
        private void InitPermissionColumns()
        {
            GridViewColumn gvc;
            GridViewColumnHeader gvch;
            List<GridViewColumn> listColumns = new List<GridViewColumn>();
            gvch = new GridViewColumnHeader();
            gvch.Content = string.Empty;
            DataTemplate nameColumnTemplate;


            gvc = new GridViewColumn();
            gvc.Header = CurrentApp.GetLanguageInfo("1102T00006", "Is Down Assign");
            gvc.Width = 120;
            nameColumnTemplate = (DataTemplate)Resources["PermisionIsCanDownAssign"];
            if (nameColumnTemplate != null)
            {
                gvc.CellTemplate = nameColumnTemplate;
            }
            else
            {
                gvc.DisplayMemberBinding = new Binding("IsCanDownAssign");
            }
            listColumns.Add(gvc);


            gvc = new GridViewColumn();
            gvc.Header = CurrentApp.GetLanguageInfo("1102T00007", "Is Can Use");
            gvc.Width = 120;
            nameColumnTemplate = (DataTemplate)Resources["PermissionIsCanUse"];
            if (nameColumnTemplate != null)
            {
                gvc.CellTemplate = nameColumnTemplate;
            }
            else
            {
                gvc.DisplayMemberBinding = new Binding("IsCanUse");
            }
            listColumns.Add(gvc);


            //gvc = new GridViewColumn();
            //gvc.Header = "Is Can CascadeRecycle";
            //gvc.Width = 100;
            //nameColumnTemplate = (DataTemplate)Resources["PermissionIsCanCascadeRecycle"];
            //if (nameColumnTemplate != null)
            //{
            //    gvc.CellTemplate = nameColumnTemplate;
            //}
            //else
            //{
            //    gvc.DisplayMemberBinding = new Binding("IsCanCascadeRecycle");
            //}
            //listColumns.Add(gvc);


            nameColumnTemplate = (DataTemplate)Resources["NameColumnTemplate"];
            if (nameColumnTemplate != null)
            {
                GTPermissionsList.SetColumns(nameColumnTemplate, gvch, 280, listColumns);
            }
            else
            {
                GTPermissionsList.SetColumns(gvch, 280, listColumns);
            }
        }
        #endregion

        #region RoleInit

        //角色选择时更新操作按钮
        public void LVRoleList_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            var CurrentFocusRecord = new RoleModel();
            CurrentFocusRecord = (RoleModel)LVRoleList.SelectedItem;
            if (CurrentFocusRecord != null)
            {
                //更新详情信息
                ShowObjectDetail();
                //更新按钮
                InitBasicOperations();
                //更新选择权限
                IsCheckPermission();

                //更新选择用户
                //ListRoleUsers.Clear();
                if (CurrentFocusRecord.RoleID == S1102App.LongParse(string.Format(S1102Consts.ROLE_SYSTEMAGENT, S1102Consts.RENT_DEFAULT_TOKEN), 0))
                {
                    GTPermissionsList.ItemsSource = mAgentPermissionRoot.Children;
                    GTUsersList.ItemsSource = mAgentORGRoot.Children;
                    ExpandedFirstLeve(mAgentORGRoot);
                }
                else
                {
                    //ReloadData();
                    GTPermissionsList.ItemsSource = mPermissionRoot.Children;
                    GTUsersList.ItemsSource = mUserORGRoot.Children;
                    ExpandedFirstLeve(mUserORGRoot);
                }
                IsCheckUser();
            }
            //else
            //{
            //    //string str = "jdifjid";
            //    //MessageBox.Show(str);
            //    return;
            //}
        }
        //public void Changechild(ObjectItem ObjItem)
        //{
        //    foreach (ObjectItem Obj in ObjItem.Children)
        //    {
        //        mUserORGRoot.AddChild(Obj);
        //        Changechild(Obj);
        //    }
        //}

        //初始化角色列
        private void InitRoleColumns()
        {
            GridView ColumnGridView = new GridView();
            GridViewColumn gvc;

            gvc = new GridViewColumn();
            gvc.Header = CurrentApp.GetLanguageInfo("1102T00001", "Role Name");
            gvc.Width = 250;
            DataTemplate fullNameTemplate = (DataTemplate)Resources["RoleNameCellTemplate"];
            if (fullNameTemplate != null)
            {
                gvc.CellTemplate = fullNameTemplate;
            }
            else
            {
                gvc.DisplayMemberBinding = new Binding("RoleName");
            }
            ColumnGridView.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvc.Header = CurrentApp.GetLanguageInfo("1102T00004", "Is Active");
            gvc.Width = 100;
            DataTemplate EnableCellTemplate = (DataTemplate)Resources["IsActiveCellTemplate"];
            if (EnableCellTemplate != null)
            {
                gvc.CellTemplate = EnableCellTemplate;
            }
            else
            {
                gvc.DisplayMemberBinding = new Binding("StrIsActive");
            }
            ColumnGridView.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvc.Header = CurrentApp.GetLanguageInfo("1102T00018", "Operate");
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
            ColumnGridView.Columns.Add(gvc);

            LVRoleList.View = ColumnGridView;
        }


        private void InitRoles()
        {
            //Dispatcher.Invoke(new Action(() => ListRoleModels.Clear()));

            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1102Codes.GetRoleList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());

                //Service11021Client client  = new Service11021Client();
                Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.URPOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                ListRoleModels.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<RoleModel>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RoleModel roleInfo = optReturn.Data as RoleModel;

                    if (roleInfo != null)
                    {
                        roleInfo.RoleName = S1102App.DecryptString(roleInfo.RoleName);
                        roleInfo.StrEnableTime = S1102App.DecryptString(roleInfo.StrEnableTime);
                        roleInfo.StrEndTime = S1102App.DecryptString(roleInfo.StrEndTime);
                        roleInfo.CreatorName = S1102App.DecryptString(roleInfo.CreatorName);
                        if (roleInfo.RoleID.ToString() == string.Format(S1102Consts.ROLE_SYSTEMADMIN, CurrentApp.Session.RentInfo.Token))
                        {
                            roleInfo.RoleName = CurrentApp.GetLanguageInfo(string.Format("COMR{0}", roleInfo.RoleID), "Administrator");
                        }
                        if (roleInfo.RoleID.ToString() == string.Format(S1102Consts.ROLE_SYSTEMAGENT, CurrentApp.Session.RentInfo.Token))
                        {
                            roleInfo.RoleName = CurrentApp.GetLanguageInfo(string.Format("COMR{0}", roleInfo.RoleID), "Agent");
                        }
                        Dispatcher.Invoke(new Action(() =>
                        {
                            var temp = roleInfo;
                            ListRoleModels.Add(temp);
                        }));
                    }
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ChangListRoleModelsData()
        {
            if (ListRoleModels != null)
            {
                for (int i = 0; i < ListRoleModels.Count; i++)
                {
                    if (ListRoleModels[i].IsActive.Equals("1"))
                    {
                        ListRoleModels[i].StrIsActive = CurrentApp.GetLanguageInfo("1102T00021", "Active");
                    }
                    else
                    {
                        ListRoleModels[i].StrIsActive = CurrentApp.GetLanguageInfo("1102T00022", "Disable");
                    }
                    ListRoleModels[i].TipModifyRole = CurrentApp.GetLanguageInfo("FO1102002", "Modify Role");
                    ListRoleModels[i].TipRemoveRole = CurrentApp.GetLanguageInfo("FO1102003", "Delete Role");
                    if (ListRoleModels[i].RoleID.ToString() == string.Format(S1102Consts.ROLE_SYSTEMADMIN, CurrentApp.Session.RentInfo.Token))
                    {
                        ListRoleModels[i].RoleName = CurrentApp.GetLanguageInfo(string.Format("COMR{0}", ListRoleModels[i].RoleID), "Administrator");
                    }
                    if (ListRoleModels[i].RoleID.ToString() == string.Format(S1102Consts.ROLE_SYSTEMAGENT, CurrentApp.Session.RentInfo.Token))
                    {
                        ListRoleModels[i].RoleName = CurrentApp.GetLanguageInfo(string.Format("COMR{0}", ListRoleModels[i].RoleID), "Agent");
                    }
                    if (ListRoleModels[i].RoleID < S1102Consts.RoleID_Limit)
                    {
                        ListRoleModels[i].RoleName = CurrentApp.GetLanguageInfo("COM" + ListRoleModels[i].RoleID, ListRoleModels[i].RoleName);
                    }
                }
            }
        }

        public void ReloadData()
        {
            ListRoleModels.Clear();
            InitRoles();
            ClearmPermissionRootCheck(mPermissionRoot);
            ClearChildren(mUserORGRoot);
            InitOrgsTree(mUserORGRoot, 1, -1);
            ChangListRoleModelsData();
        }

        private void AddRole()
        {
            PopupPanel.Title = CurrentApp.GetLanguageInfo("1102T00023", "Add New Role");
            OperationRole operationRoleAdd = new OperationRole();
            operationRoleAdd.IsAddOrModify = true;
            operationRoleAdd.PageParent = this;
            operationRoleAdd.CurrentApp = CurrentApp;
            PopupPanel.Content = operationRoleAdd;
            PopupPanel.IsOpen = true;
        }

        private void ModifyRole(RoleModel roleItem)
        {
            if (roleItem == null) { SetBusy(false,CurrentApp.GetLanguageInfo("1102T00028", "please select a Role")); return; }
            if (roleItem.RoleID < S1102Consts.RoleID_Limit)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00029", "Syste Role Can''t  Modify"));
                return;
            }
            PopupPanel.Title = CurrentApp.GetLanguageInfo("FO1102002", "Modify Role");
            OperationRole operationRoleModify = new OperationRole();
            operationRoleModify.PageParent = this;
            operationRoleModify.IsAddOrModify = false;
            operationRoleModify.CurrentApp = CurrentApp;
            operationRoleModify.ObjRoleItem = roleItem;
            PopupPanel.Content = operationRoleModify;
            PopupPanel.IsOpen = true;
        }

        private void DeleteRole(RoleModel roleItem)
        {
            if (roleItem == null)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00028", "Please Select A Role"));
                return;
            }
            if (roleItem.RoleID < S1102Consts.RoleID_Limit)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00031", "Syste Role Can't  Delete"));
                return;
            }

            if (roleItem != null)
            {
                var result = MessageBox.Show(string.Format(CurrentApp.GetLanguageInfo("1102T00032", "Confirm Delete Role") + " \r\n\r\n{0}", roleItem.RoleName), "UMP",
                MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {

                        roleItem.IsDelete = "1";
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S1102Codes.ModifyRole;
                        OperationReturn optReturn = XMLHelper.SeriallizeObject(roleItem);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        webRequest.Data = optReturn.Data.ToString();

                        //px+
                        List<string> RoleList = new List<string>();
                        RoleList.Add(roleItem.RoleID.ToString());
                        RoleList.Add(S1102App.EncryptString(roleItem.RoleName));
                        RoleList.Add(roleItem.ParentRoleID.ToString());
                        RoleList.Add(roleItem.ModeID.ToString());
                        RoleList.Add(roleItem.CreatorID.ToString());
                        RoleList.Add(roleItem.CreatTime.ToString());
                        RoleList.Add(roleItem.IsActive);
                        RoleList.Add(roleItem.IsDelete.ToString());
                        RoleList.Add(S1102App.EncryptString(roleItem.StrEnableTime.ToString()));
                        RoleList.Add(S1102App.EncryptString(roleItem.StrEndTime.ToString()));
                        RoleList.Add(roleItem.OtherStatus.ToString());
                        webRequest.ListData = RoleList;
                        //end
                        //Service11021Client client = new Service11021Client();
                        Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                        WebHelper.SetServiceClient(client);
                        WebReturn webReturn = client.URPOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        }

                        string msg = string.Format("{0}{1}", Utils.FormatOptLogString("FO1102003"), roleItem.RoleName);
                        #region 写操作日志
                        CurrentApp.WriteOperationLog(S1102Consts.OPT_DeleteRole.ToString(), ConstValue.OPT_RESULT_SUCCESS, msg);
                        #endregion
                        //删除角色对应权限
                        webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S1102Codes.DeleteRolePermission;
                        webRequest.Data = roleItem.RoleID.ToString();
                        //client = new Service11021Client();
                        client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                        WebHelper.SetServiceClient(client);
                        webReturn = client.URPOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        }

                        //得到角色下用户
                        webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S1102Codes.GetRoleUsers;
                        webRequest.ListData.Add(roleItem.RoleID.ToString());
                        //client = new Service11021Client();
                        client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                        WebHelper.SetServiceClient(client);
                        webReturn = client.URPOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        }
                        List<RoleUsersInfo> lstRoleUserInfoTemp = new List<RoleUsersInfo>();
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            optReturn = XMLHelper.DeserializeObject<RoleUsersInfo>(webReturn.ListData[i]);
                            if (!optReturn.Result)
                            {
                                ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            RoleUsersInfo roleUsersInfo = optReturn.Data as RoleUsersInfo;
                            if (roleUsersInfo != null)
                            {
                                lstRoleUserInfoTemp.Add(roleUsersInfo);
                            }
                        }

                        List<string> lstStrRoleUser = new List<string>();
                        foreach (RoleUsersInfo userinfo in lstRoleUserInfoTemp)
                        {
                            optReturn = XMLHelper.SeriallizeObject(userinfo);
                            lstStrRoleUser.Add(optReturn.Data.ToString());
                        }

                        //更新这些用户的权限
                        if (lstStrRoleUser.Count() > 0)
                        {
                            webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)S1102Codes.UpdateUserPerimission;
                            webRequest.ListData = lstStrRoleUser;
                            //client = new Service11021Client();
                            client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                            WebHelper.SetServiceClient(client);
                            webReturn = client.URPOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                                ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                            }
                        }



                        //删除角色对应用户
                        webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S1102Codes.DeleteRoleUser;
                        webRequest.Data = roleItem.RoleID.ToString();
                        //client = new Service11021Client();
                        client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                        WebHelper.SetServiceClient(client);
                        webReturn = client.URPOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        }
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00030", "Submit Success"));
                        ReloadData();
                        ShowObjectDetail();

                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                }
            }
        }
        #endregion

        #region User Agent Init

        //选择角色时，角色对应用户是否选择
        private void IsCheckUser()
        {
            var objItem = LVRoleList.SelectedItem as RoleModel;
            if (objItem != null)
            {
                try
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S1102Codes.GetRoleUsers;
                    webRequest.Session = CurrentApp.Session;
                    webRequest.ListData.Add(objItem.RoleID.ToString());
                    //Service11021Client client = new Service11021Client();
                    Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                    WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.URPOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }

                    List<RoleUsersInfo> lstRoleUserInfoTemp = new List<RoleUsersInfo>();
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<RoleUsersInfo>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        RoleUsersInfo roleUsersInfo = optReturn.Data as RoleUsersInfo;
                        if (roleUsersInfo != null)
                        {
                            lstRoleUserInfoTemp.Add(roleUsersInfo);
                            ListRoleUsers.Add(roleUsersInfo);
                        }
                    }
                    if (objItem.RoleID == S1102App.LongParse(string.Format(S1102Consts.ROLE_SYSTEMAGENT, S1102Consts.RENT_DEFAULT_TOKEN), 0))
                    {
                        ClearmUserORGRootCheck(mAgentORGRoot);
                        if (lstRoleUserInfoTemp.Count > 0)
                        {
                            foreach (RoleUsersInfo roleinfo in lstRoleUserInfoTemp)
                            {
                                IsCheckmUserORGRoot(mAgentORGRoot, roleinfo.UserID);
                            }
                        }
                    }
                    else
                    {
                        ClearmUserORGRootCheck(mUserORGRoot);
                        if (lstRoleUserInfoTemp.Count > 0)
                        {
                            foreach (RoleUsersInfo roleinfo in lstRoleUserInfoTemp)
                            {
                                IsCheckmUserORGRoot(mUserORGRoot, roleinfo.UserID);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
            }
        }

        //changelanguage
        void ChangeLanguageTree(ObjectItem objectParent)
        {
            foreach (ObjectItem item in objectParent.Children)
            {
                item.Name = CurrentApp.GetLanguageInfo(string.Format("FO{0}", item.ObjID), item.Name) ;
                if (objectParent.Children.Count > 0)
                {
                    ChangeLanguageTree(item);
                }
            }
        }

        //先全清钩钩
        void ClearmUserORGRootCheck(ObjectItem objectParent)
        {
            foreach (ObjectItem item in objectParent.Children)
            {
                item.IsChecked = false;
                if (objectParent.Children.Count > 0)
                {
                    ClearmUserORGRootCheck(item);
                }
            }
        }

        private void IsCheckmUserORGRoot(ObjectItem objectParent, long UserID)
        {
            foreach (ObjectItem item in objectParent.Children)
            {
                if (item.ObjID == UserID && item.ObjType != S1102Consts.OBJTYPE_ORG)
                {
                    item.IsChecked = true;
                    return;
                }

                if (item.Children.Count > 0)
                {
                    IsCheckmUserORGRoot(item, UserID);
                }
            }
        }

        private void InitControledOrgsAndUsers()
        {
            InitControledOrgs("-1");
            ClearChildren(mUserORGRoot);
            ClearChildren(mAgentORGRoot);
            InitOrgsTree(mUserORGRoot, 1, -1);
            InitOrgsTree(mAgentORGRoot, 2, -1);
            //展开到下一级
            ExpandedFirstLeve(mUserORGRoot);
            ExpandedFirstLeve(mAgentORGRoot);
        }

        //展开第一层
        private void ExpandedFirstLeve(ObjectItem objectParent)
        {
            objectParent.IsExpanded = true;
            //objectParent.IsExpanded = false;
            if (objectParent.Children.Count > 0)
            {
                for (int i = 0; i < objectParent.Children.Count; i++)
                {
                    //objectParent.Children[i].IsExpanded = true;
                    objectParent.Children[i].IsExpanded = false;
                }

                var currentItem = objectParent.Children[0] as ObjectItem;
                if (currentItem != null)
                {
                    currentItem.IsSingleSelected = true;
                }
            }
        }

        //初始化机构及用户
        private void InitControledOrgs(string parentId)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1102Codes.GetOrganizationList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(parentId);
                //Service11021Client client =  new Service11021Client();
                Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.URPOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }

                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OrganizationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OrganizationInfo orgInfo = optReturn.Data as OrganizationInfo;
                    if (orgInfo != null)
                    {
                        if (parentId.Equals("-1"))
                        {
                            S1102App.CurrentOrg = orgInfo.OrgID;
                        }
                        if (lstCtrolOrgInfos.Where(p => p.OrgID == orgInfo.OrgID).Count() == 0)
                        {
                            lstCtrolOrgInfos.Add(orgInfo);

                            InitControledAgents(orgInfo.OrgID);
                            InitControledUsers(orgInfo.OrgID);
                        }

                        InitControledOrgs(orgInfo.OrgID.ToString());

                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //初始化管理的座席
        private void InitControledAgents(long orgID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1102Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(orgID.ToString());
                //Service11021Client client = new Service11021Client();
                Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.URPOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }

                    UCtrolAgent ctrolAgent = new UCtrolAgent();
                    ctrolAgent.AgentID = arrInfo[0];
                    ctrolAgent.AgentName = arrInfo[1];
                    ctrolAgent.AgentFullName = arrInfo[2];
                    ctrolAgent.AgentOrgID = orgID.ToString();
                    if (lstCtrolAgentInfos.Where(p => p.AgentID == ctrolAgent.AgentID).Count() == 0)
                    {
                        lstCtrolAgentInfos.Add(ctrolAgent);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //初始化管理的用户
        private void InitControledUsers(long orgID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1102Codes.GetUserList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(orgID.ToString());
                //Service11021Client client =  new Service11021Client();
                Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.URPOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ObjectItem> listChild = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicUserInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicUserInfo userInfo = optReturn.Data as BasicUserInfo;
                    if (userInfo != null)
                    {
                        //如果包含超级管理员不加入
                        if (userInfo.UserID != S1102App.LongParse(string.Format(S1102Consts.USER_ADMIN, CurrentApp.Session.RentInfo.Token), 0))
                        {
                            if (lstCtrolUserInfos.Where(p => p.UserID == userInfo.UserID).Count() == 0)
                            {
                                lstCtrolUserInfos.Add(userInfo);
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


        /// <summary>
        /// 初始化树
        /// </summary>
        /// <param name="parentObj">树名</param>
        /// <param name="type">1为初始化用户树，2初始化座席树</param>
        /// <param name="orgID">机构名</param>
        private void InitOrgsTree(ObjectItem parentObj, int type, long orgID)
        {
            List<OrganizationInfo> lstCtrolOrgTemp = new List<OrganizationInfo>();
            if (orgID == -1)
            {
                lstCtrolOrgTemp = lstCtrolOrgInfos.Where(p => p.OrgID == S1102App.CurrentOrg).ToList();
            }
            else
            {
                lstCtrolOrgTemp = lstCtrolOrgInfos.Where(p => p.ParentID == orgID).ToList();
            }

            List<ObjectItem> listChild = new List<ObjectItem>();
            foreach (OrganizationInfo orgInfo in lstCtrolOrgTemp)
            {
                ObjectItem item = new ObjectItem();
                item.ObjType = S1102Consts.OBJTYPE_ORG;
                item.ObjID = orgInfo.OrgID;
                item.Name = orgInfo.OrgName;
                item.FullName = item.Name;
                item.Description = orgInfo.Description;
                item.State = Convert.ToInt32(orgInfo.IsActived);
                item.IsHidden = true;
                switch (orgInfo.OrgType)
                {
                    case 901:
                        item.Icon = "/UMPS1102;component/Themes/Default/UMPS1102/Images/root.ico";
                        break;
                    case 902:
                        item.Icon = "/UMPS1102;component/Themes/Default/UMPS1102/Images/company.ico";
                        break;
                    case 903:
                        item.Icon = "/UMPS1102;component/Themes/Default/UMPS1102/Images/group.ico";
                        break;
                }
                item.Data = orgInfo;
                InitOrgsTree(item, type, orgInfo.OrgID);
                InitOrgsUserOrAgentTree(item, type, orgInfo.OrgID);
                listChild.Add(item);
            }
            listChild = listChild.OrderBy(o => o.Name).ToList();
            for (int i = 0; i < listChild.Count; i++)
            {
                AddChildObjectItem(parentObj, listChild[i]);
            }
        }

        //初始化机构座席树
        private void InitOrgsUserOrAgentTree(ObjectItem parentObj, int type, long parentID)
        {
            try
            {
                List<ObjectItem> listChild = new List<ObjectItem>();
                if (type == 1)
                {
                    List<BasicUserInfo> lstCtrolUserTemp = new List<BasicUserInfo>();
                    lstCtrolUserTemp = lstCtrolUserInfos.Where(p => p.OrgID == parentID).ToList();
                    foreach (BasicUserInfo userInfo in lstCtrolUserTemp)
                    {
                        ObjectItem item = new ObjectItem();
                        item.ObjType = S1102Consts.OBJTYPE_USER;
                        item.ObjID = userInfo.UserID;
                        item.Name = userInfo.Account;
                        item.FullName = userInfo.FullName;
                        item.State = Convert.ToInt32(userInfo.IsActived);
                        item.IsHidden = true;
                        item.Icon = "/UMPS1102;component/Themes/Default/UMPS1102/Images/user.png";
                        item.Data = userInfo;
                        listChild.Add(item);
                    }
                }
                else
                {
                    List<UCtrolAgent> lstCtrolAgentTemp = new List<UCtrolAgent>();
                    lstCtrolAgentTemp = lstCtrolAgentInfos.Where(p => p.AgentOrgID == parentID.ToString()).ToList();
                    foreach (UCtrolAgent agent in lstCtrolAgentTemp)
                    {
                        ObjectItem item = new ObjectItem();
                        item.ObjType = ConstValue.RESOURCE_AGENT;
                        item.ObjID = Convert.ToInt64(agent.AgentID);
                        item.Name = agent.AgentName;
                        item.FullName = agent.AgentFullName;
                        item.Description = agent.AgentFullName;
                        item.Data = agent;
                        item.IsHidden = true;
                        item.Icon = "/UMPS1102;component/Themes/Default/UMPS1102/Images/user_suit.png";
                        listChild.Add(item);
                    }
                }

                listChild = listChild.OrderBy(o => o.Name).ToList();
                for (int i = 0; i < listChild.Count; i++)
                {
                    AddChildObjectItem(parentObj, listChild[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        /// <summary>
        /// 初始化用户列
        /// </summary>
        private void InitUsersColumns()
        {
            GridViewColumn gvc;
            GridViewColumnHeader gvch;
            List<GridViewColumn> listColumns = new List<GridViewColumn>();
            gvch = new GridViewColumnHeader();
            gvch.Content = string.Empty;

            gvc = new GridViewColumn();
            //gvc.Header = "Full Name";
            gvc.Header = CurrentApp.GetLanguageInfo("1102T00040", "Full Name");
            gvc.Width = 150;
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

            DataTemplate nameColumnTemplate = (DataTemplate)Resources["NameColumnTemplate"];
            if (nameColumnTemplate != null)
            {
                GTUsersList.SetColumns(nameColumnTemplate, gvch, 320, listColumns);
            }
            else
            {
                GTUsersList.SetColumns(gvch, 320, listColumns);
            }
        }

        #endregion

        #region OtherMethod
        private void ClearChildren(ObjectItem item)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (item != null)
                {
                    item.Children.Clear();
                }
            }));
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

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("FO1102", "UMP Role Management");
                //Operation
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    ListOperations[i].Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", ListOperations[i].ID),
                        ListOperations[i].ID.ToString());
                }
                ChangeLanguageTree(mPermissionRoot);
                ChangeLanguageTree(mAgentORGRoot);
                //ClearChildren(mPermissionRoot);
                ClearChildren(mAgentPermissionRoot);
                //ClearChildren(mUserORGRoot);
                if (ListOperations.Count() > 0)
                {
                    //InitmPermissionRoot(mPermissionRoot, 0);
                    InitmAgentPermissionRoot();
                }
                InitBasicOperations();
                //popup
                if (this.PopupPanel != null)
                {
                    PopupPanel.ChangeLanguage();
                }

                //给换语言包
                TabItemRoles.Header = CurrentApp.GetLanguageInfo("1102T00010", "Role");
                TabItemPermissions.Header = CurrentApp.GetLanguageInfo("1102T00012", "Permission");
                TabItemUsers.Header = CurrentApp.GetLanguageInfo("1102T00009", "User");
                ExpanderBasic.Header = CurrentApp.GetLanguageInfo("1102T00024", "Basic Operations");
                ExpanderOther.Header = CurrentApp.GetLanguageInfo("1102T00025", "Other Position");
                //列名
                InitRoleColumns();
                ChangListRoleModelsData();
                InitUsersColumns();
                InitPermissionColumns();

                //详细信息
                ShowObjectDetail();
            }
            catch (Exception ex)
            {

            }
        }

        public void ShowObjectDetail()
        {

            try
            {
                var RoleInfo = new RoleModel();
                RoleInfo = (RoleModel)LVRoleList.SelectedItem;
                if (RoleInfo != null)
                {
                    ObjectDetail.Visibility = Visibility.Visible;
                    ObjectDetail.Title = RoleInfo.RoleName;
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.UriSource = new Uri(string.Format("/UMPS1102;component/Themes/Default/UMPS1102/{0}", "Images/user.png"), UriKind.Relative);
                    image.EndInit();
                    ObjectDetail.Icon = image;
                    List<PropertyItem> listProperties = new List<PropertyItem>();


                    PropertyItem property = new PropertyItem();
                    property.Name = CurrentApp.GetLanguageInfo("1102T00010", "Role");
                    property.ToolTip = property.Name;
                    property.Value = RoleInfo.RoleName;
                    listProperties.Add(property);

                    property = new PropertyItem();
                    property.Name = CurrentApp.GetLanguageInfo("1102T00004", "Is Active");
                    if (RoleInfo.IsActive.Equals("1"))
                    {
                        property.Value = CurrentApp.GetLanguageInfo("1102T00019", "Yes");
                    }
                    else
                    {
                        property.Value = CurrentApp.GetLanguageInfo("1102T00020", "No");
                    }
                    property.ToolTip = property.Name;
                    listProperties.Add(property);

                    property = new PropertyItem();
                    property.Name = CurrentApp.GetLanguageInfo("1102T00002", "StartTime");
                    property.ToolTip = property.Name;
                    property.Value = Convert.ToDateTime(RoleInfo.StrEnableTime).ToLocalTime().ToString();
                    listProperties.Add(property);

                    property = new PropertyItem();
                    property.Name = CurrentApp.GetLanguageInfo("1102T00003", "EndTime");
                    property.ToolTip = property.Name;
                    property.Value = RoleInfo.StrEndTime;
                    listProperties.Add(property);

                    property = new PropertyItem();
                    property.Name = CurrentApp.GetLanguageInfo("1102T00016", "Creator");
                    property.ToolTip = property.Name;
                    property.Value = RoleInfo.CreatorName;
                    listProperties.Add(property);

                    property = new PropertyItem();
                    property.Name = CurrentApp.GetLanguageInfo("1102T00017", "Create Time");
                    property.ToolTip = property.Name;
                    property.Value = RoleInfo.CreatTime.ToString();
                    listProperties.Add(property);
                    ObjectDetail.ItemsSource = listProperties;
                }
                else
                {
                    ObjectDetail.Visibility = Visibility.Hidden;
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

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
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS1102;component/Themes/{0}/{1}",
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
                string uri = string.Format("/UMPS1102;component/Themes/Default/UMPS1102/MainPageStatic.xaml");
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
    }
}
