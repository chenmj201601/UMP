using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common11021;
using UMPS1102.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using UMPS1102.Wcf11021;

namespace UMPS1102
{
    /// <summary>
    /// OperationRole.xaml 的交互逻辑
    /// </summary>
    public partial class OperationRole 
    {
        public OperationRole()
        {
            InitializeComponent();
            Loaded+=RoleInfo_Loaded;
        }

        #region Members
        public RoleModel ObjRoleItem;
        public RoleManage PageParent;
        public RoleModel RoleInfo;
        public bool IsAddOrModify;
        public ObservableCollection<RoleModel> LstRoleModel;
        private BackgroundWorker mBackgroundWorker;
        private bool mAsyncResult;

        #endregion
        //public S1102App CurrentApp;
        void RoleInfo_Loaded(object sender, RoutedEventArgs e) 
        {
            BtnConfirm.Click+=BtnConfirm_Click;
            BtnClose.Click+=BtnClose_Click;

            InitCtrol();
            ChangeLanguage();
        }

        void InitCtrol()
        {
            if (IsAddOrModify) 
            {
                TxtRoleName.Text = "";
                DateStart.Value = DateTime.Now;
                DateEnd.Value = DateTime.Parse(S1102Consts.Default_StrEndTime);
                ChkStartRole.IsChecked = true;
               
            }
            else 
            {
                RoleModel oldrolemodel = this.ObjRoleItem as RoleModel;
                if(oldrolemodel !=null)
                {
                    TxtRoleName.Text = oldrolemodel.RoleName;
                    DateStart.Value = DateTime.Parse(oldrolemodel.StrEnableTime).ToLocalTime();
                    DateEnd.Value = DateTime.Parse(oldrolemodel.StrEndTime);
                    ChkStartRole.IsChecked = oldrolemodel.IsActive.Equals("1");
                }
            }

        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
            {
                return;
            }
            if (this.IsAddOrModify)
            {
                AddRoleInfo();
            }
            else
            {
                ModifyRoleInfo();
            }
        }

        private void ModifyRoleInfo() 
        {
            try
            {
                RoleModel RoleInfo = this.ObjRoleItem as RoleModel;
                string msg = string.Format("{0}{1}", Utils.FormatOptLogString("FO1102002"), TxtRoleName.Text);
                //px屏蔽
                //RoleInfo.RoleName = S1102App.EncryptString(TxtRoleName.Text);
                //RoleInfo.IsActive = ChkStartRole.IsChecked == true ? "1" : "0";

                //RoleInfo.EnableTime = DateTime.Parse(DateStart.Value.ToString());
                //RoleInfo.EndTime = DateTime.Parse(DateEnd.Value.ToString());
                //RoleInfo.StrEnableTime = S1102App.EncryptString(RoleInfo.EnableTime.ToString("yyyy/MM/dd HH:mm:ss"));
                //RoleInfo.StrEndTime = S1102App.EncryptString(RoleInfo.EndTime.ToString("yyyy/MM/dd HH:mm:ss"));
                //px-end
                List<string> RoleList = new List<string>();
                RoleList.Add(RoleInfo.RoleID.ToString());
                RoleList.Add(S1102App.EncryptString(TxtRoleName.Text.Trim()));
                RoleList.Add(RoleInfo.ParentRoleID.ToString());
                RoleList.Add(RoleInfo.ModeID.ToString());
                RoleList.Add(RoleInfo.CreatorID.ToString());
                RoleList.Add(RoleInfo.CreatTime.ToString());
                RoleList.Add(ChkStartRole.IsChecked == true ? "1" : "0");
                RoleList.Add(RoleInfo.IsDelete.ToString());
                RoleList.Add(S1102App.EncryptString(DateTime.Parse(DateStart.Text).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss")));
                RoleList.Add(S1102App.EncryptString(DateTime.Parse(DateEnd.Text).ToString("yyyy/MM/dd HH:mm:ss")));
                RoleList.Add(RoleInfo.OtherStatus.ToString());

                mAsyncResult = false;
                if(PageParent != null) 
                {
                    PageParent.SetBusy(true,string.Empty);
                }

                mBackgroundWorker = new BackgroundWorker();
                mBackgroundWorker.DoWork += (s, de) =>
                {
                    ShowStausMessage(CurrentApp.GetLanguageInfo("1102T00037", "Modifing  The Role Information")+"...");
                    ModifyRoleInfo(RoleList);
                    #region 写操作日志
                    CurrentApp.WriteOperationLog(S1102Consts.OPT_ModifyRole.ToString(), ConstValue.OPT_RESULT_SUCCESS, msg);
                    #endregion
                };
                mBackgroundWorker.RunWorkerCompleted += (s, re) =>
                {
                    mBackgroundWorker.Dispose();

                    if (PageParent != null) 
                    {
                        PageParent.SetBusy(false,string.Empty);
                    }
                    ShowStausMessage(CurrentApp.GetLanguageInfo("1102T00038", "Modify The Role Information End"));
                    if (mAsyncResult)
                    {       
                        if (PageParent != null)
                        {
                            PageParent.ReloadData();
                            PageParent.ShowObjectDetail();
                        }
                      
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    }
                };
                mBackgroundWorker.RunWorkerAsync();
            }    
            catch (Exception)
            {
                throw;
            }
        }

        private void ModifyRoleInfo(List<string> RoleList)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1102Codes.ModifyRole;
                //px屏蔽
                //OperationReturn optReturn = XMLHelper.SeriallizeObject(RoleInfo);
                //if (!optReturn.Result)
                //{
                //    S1102App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                //    return;
                //}
                //webRequest.Data = optReturn.Data.ToString();
                //px-end
                
                //px+
                webRequest.ListData = RoleList;
                //end

                //Service11021Client client = new Service11021Client();
                Wcf11021.Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.URPOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code == Defines.RET_DBACCESS_EXIST)
                    {
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00033", "Role Name Already Exists"));
                        //引用对象的加密数据还原
                        //RoleInfo.RoleName = S1102App.DecryptString(RoleInfo.RoleName);
                        //RoleInfo.StrEnableTime = S1102App.DecryptString(RoleInfo.StrEnableTime);
                        //RoleInfo.StrEndTime = S1102App.DecryptString(RoleInfo.StrEndTime);
                        return;
                    }
                    else
                    {
                        CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                }
                RoleModel RoleInfo = this.ObjRoleItem as RoleModel;
                RoleInfo.RoleName = S1102App.DecryptString(RoleList[1]);
                RoleInfo.StrEnableTime = Convert.ToDateTime(S1102App.DecryptString(RoleList[8])).ToLocalTime().ToString();
                RoleInfo.StrEndTime = S1102App.DecryptString(RoleList[9]);
                mAsyncResult = true;
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void AddRoleInfo() 
        {
            try
            {
                RoleInfo = new RoleModel();

                RoleInfo.ParentRoleID = 0;
                RoleInfo.ModeID = 0;
                string msg = string.Format("{0}{1}", Utils.FormatOptLogString("FO1102001"), TxtRoleName.Text);
                if(string.IsNullOrWhiteSpace(TxtRoleName.Text))
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00034", "Role Information Is Null"));
                    return;
                }
                RoleInfo.RoleName = S1102App.EncryptString(TxtRoleName.Text.Trim());
                RoleInfo.IsActive = ChkStartRole.IsChecked == true ? "1" : "0";
                RoleInfo.IsDelete = "0";
                RoleInfo.OtherStatus = "11111111111111111111111111111111";
               
                RoleInfo.EnableTime = DateTime.Parse(DateStart.Value.ToString());
                RoleInfo.EndTime = DateTime.Parse(DateEnd.Value.ToString());

                RoleInfo.StrEnableTime = S1102App.EncryptString(RoleInfo.EnableTime.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss"));
                RoleInfo.StrEndTime = S1102App.EncryptString(RoleInfo.EndTime.ToString("yyyy/MM/dd HH:mm:ss"));
                RoleInfo.CreatorID = CurrentApp.Session.UserInfo.UserID;
                RoleInfo.CreatTime = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")).ToUniversalTime();
                mAsyncResult = false;
                if (PageParent != null)
                {
                    PageParent.SetBusy(true,string.Empty);
                }
                mBackgroundWorker=new BackgroundWorker();
                mBackgroundWorker.DoWork += (s, de) =>
                {
                    ShowStausMessage(CurrentApp.GetLanguageInfo("1102T00035", "Adding Role Information") + "....");
                    AddNewRoleInfo(RoleInfo);
                #region 写操作日志
                  
                CurrentApp.WriteOperationLog(S1102Consts.OPT_AddRole.ToString(),ConstValue.OPT_RESULT_SUCCESS,msg);
                #endregion
                };
                mBackgroundWorker.RunWorkerCompleted += (s, re) =>
                {
                    mBackgroundWorker.Dispose();
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false,string.Empty);
                    }

                    ShowStausMessage(CurrentApp.GetLanguageInfo("1102T00036", "Add Role Information End") + ".");
                    if (mAsyncResult)
                    {
                        if (PageParent != null)
                        {
                            PageParent.ReloadData();
                            PageParent.ShowObjectDetail();
                        }
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    }
                };
                mBackgroundWorker.RunWorkerAsync();
            }
            catch (Exception)
            {                
                throw;
            }
        }

        void AddNewRoleInfo(RoleModel newRoleInfo) 
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1102Codes.AddNewRole;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(newRoleInfo);
                if (!optReturn.Result)
                {
                    #region
                    string msg = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO1102001"), RoleInfo.RoleName);
                    CurrentApp.WriteOperationLog(S1102Consts.OPT_AddRole.ToString(), ConstValue.OPT_RESULT_FAIL, msg);
                    #endregion
                    CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.Data = optReturn.Data.ToString();

                //Service11021Client client = new Service11021Client();
                Wcf11021.Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.URPOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code == Defines.RET_DBACCESS_EXIST)
                    {
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00033", "Role Name Already Exists"));
                        return;
                    }
                    else
                    {
                        CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                }
                else 
                {
                    long RoleID = Convert.ToInt64(webReturn.Data);
                    AddRoleUser(RoleID);
                }
                mAsyncResult = true;
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        protected string XmlRoleUser(long RoleID,long UserID) 
        {
            RoleUsersInfo newroleuser = new RoleUsersInfo();
            newroleuser.ParentID = 0;
            newroleuser.RoleID = RoleID;
            newroleuser.UserID = UserID;
            newroleuser.StrStartTime = DateTime.Parse(S1102Consts.Default_StrStartTime).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
            newroleuser.StrEndTime = DateTime.Parse(S1102Consts.Default_StrEndTime).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
            newroleuser.IsDelete = false;
            OperationReturn optReturn = XMLHelper.SeriallizeObject(newroleuser);
            return optReturn.Data.ToString();
        }

        //添加角色时同时将人一起写入
        void AddRoleUser(long RoleID) 
        {
            List<string> lstRoleUsers = new List<string>();
            lstRoleUsers.Add(XmlRoleUser(RoleID, S1102App.LongParse(string.Format(S1102Consts.USER_ADMIN, CurrentApp.Session.RentInfo.Token), 0)));
            if (CurrentApp.Session.UserInfo.UserID != S1102App.LongParse(string.Format(S1102Consts.USER_ADMIN, CurrentApp.Session.RentInfo.Token), 0))
            {
                lstRoleUsers.Add(XmlRoleUser(RoleID, CurrentApp.Session.UserInfo.UserID));
            }
  

            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1102Codes.SubmitRoleUser;
                webRequest.ListData = lstRoleUsers;
                webRequest.Data = RoleID.ToString();
                Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11021"));
                //Service11021Client client  = new Service11021Client();
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.URPOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        void BtnClose_Click(object sender, RoutedEventArgs e) 
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }


        #region Basic

        private bool CheckInput()
        {
            string tempName = TxtRoleName.Text.Replace(" ", "");
            if (tempName == "")
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00034", "Role Information Is Not Complete"));
                return false;
            }
            if (tempName.Count() > 128)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00041", "Role Name Is too long"));
                return false;
            }
            if (string.IsNullOrEmpty(TxtRoleName.Text) || string.IsNullOrEmpty(DateStart.Value.ToString())
                || string.IsNullOrEmpty(DateEnd.Value.ToString()))
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00034", "Role Information Is Not Complete"));
                return false;
            }

            DateTime starttiime = DateTime.Parse(DateStart.Value.ToString());
            DateTime stoptime = DateTime.Parse(DateEnd.Value.ToString());
            if (starttiime >= stoptime)
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1102T00039", "Start Time Must Smaller Than The Valid Time"));
                return false;
            }

            return true;
        }

        private void ShowStausMessage(string msg)
        {
            if (PageParent != null)
            {
                PageParent.SetBusy(false, msg);
            }
        }

        #endregion


        #region Language

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            labRoleName.Content = CurrentApp.GetLanguageInfo("1102T00001", "Role Name");
            labEnableTime.Content = CurrentApp.GetLanguageInfo("1102T00002", "StartTime");
            labExpireTime.Content = CurrentApp.GetLanguageInfo("1102T00003", "EndTime");
            labEnable.Content = CurrentApp.GetLanguageInfo("1102T00021", "Active");

            BtnConfirm.Content = CurrentApp.GetLanguageInfo("1102T00026", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("1102T00027", "Cancle");

            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                if (IsAddOrModify)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("1102T00023", "Add New Role");
                }
                else
                {
                    parent.Title = CurrentApp.GetLanguageInfo("FO1102002", "Modify Role");
                }
            }
        }

        #endregion

    }
}
