//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    96550c0c-347d-4696-a43f-f949dfdecc1b
//        CLR Version:              4.0.30319.18444
//        Name:                     OrgInfoModify
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101
//        File Name:                OrgInfoModify
//
//        created by Charley at 2014/8/27 17:47:37
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using UMPS1101.Models;
using UMPS1101.Wcf11011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1101
{
    /// <summary>
    /// OrgInfoModify.xaml 的交互逻辑
    /// </summary>
    public partial class OrgInfoModify
    {
        #region Members

        public ObjectItem ObjParent;
        public ObjectItem OrgItem;
        public BasicOrgInfo OrgParent;
        public BasicOrgInfo OrgInfo;
        public bool IsAddOrg;
        public OUMMainView PageParent;

        ////用于保存同级别机构，
        //public List<ObjectItem> mListObjectItem;


        private ObservableCollection<OrgTypeItem> mListOrgTypeInfos;
        private BackgroundWorker mBackgroundWorker;
        private bool mAsyncResult;

        #endregion
        //public S1101App CurrentApp;

        public OrgInfoModify()
        {
            InitializeComponent();
            //mListObjectItem = new List<ObjectItem>();
            mListOrgTypeInfos = new ObservableCollection<OrgTypeItem>();
            Loaded += OrgInfoModify_Loaded;
        }

        void OrgInfoModify_Loaded(object sender, RoutedEventArgs e)
        {
            BtnClose.Click += BtnClose_Click;
            BtnConfirm.Click += BtnConfirm_Click;
            ComboOrgType.ItemsSource = mListOrgTypeInfos;
            InitOrgType();
            Init();
            ChangeLanguage();
        }


        #region Load and Init

        private void Init()
        {
            if (IsAddOrg)
            {
                TxtOrgName.Text = string.Empty;

                if (ObjParent != null)
                {
                    BasicOrgInfo orgParent = ObjParent.Data as BasicOrgInfo;
                    if (orgParent != null)
                    {
                        TxtParentOrg.Text = orgParent.OrgName;
                        OrgTypeItem orgType =
                                 mListOrgTypeInfos.FirstOrDefault(
                                     ot =>
                                         ot.ID == orgParent.OrgType);
                        ComboOrgType.SelectedItem = orgType;
                    }
                }
                TxtDescription.Text = string.Empty;
                chkActive.IsChecked = true;
            }
            else
            {
                if (OrgItem != null)
                {
                    ObjParent = OrgItem.Parent as ObjectItem;
                    OrgInfo = OrgItem.Data as BasicOrgInfo;
                    if (ObjParent != null)
                    {
                        OrgParent = ObjParent.Data as BasicOrgInfo;
                    }
                }
                if (OrgInfo != null)
                {
                    TxtOrgName.Text = OrgInfo.OrgName;
                    OrgTypeItem orgType =
                                 mListOrgTypeInfos.FirstOrDefault(
                                     ot =>
                                         ot.ObjID.ToString().Substring(11) == OrgInfo.OrgType.ToString("00000000"));
                    ComboOrgType.SelectedItem = orgType;
                    if (OrgParent != null)
                    {
                        TxtParentOrg.Text = OrgParent.OrgName;
                    }
                    TxtDescription.Text = OrgInfo.Description;
                    chkActive.IsChecked = OrgInfo.IsActived.Equals("1");
                }
            }
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
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                mListOrgTypeInfos.Clear();
                List<OrgTypeItem> TempOrgType = new List<OrgTypeItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OrgTypeInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OrgTypeInfo info = optReturn.Data as OrgTypeInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tOrgTypeInfo is null"));
                        return;
                    }
                    OrgTypeItem item = new OrgTypeItem();
                    item.Info = info;
                    item.ObjID = info.ID;
                    int id = 0;
                    string str = info.ID.ToString();
                    if (str.Length > 11)
                    {
                        id = int.Parse(str.Substring(11));
                    }
                    item.ID = id;
                    item.Name = info.Name;
                    item.Description = info.Description;
                    item.SortID = info.SortID;
                    if (info.Name.Equals("InitData"))
                    {
                        item.Name = CurrentApp.GetLanguageInfo("S1100018", "System Init Data");
                    }
                    TempOrgType.Add(item);
                }
                TempOrgType = TempOrgType.OrderBy(p => p.SortID).ToList();
                foreach (OrgTypeItem Org in TempOrgType)
                {
                    mListOrgTypeInfos.Add(Org);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
            {
                return;
            }
            if (IsAddOrg)
            {
                AddOrgInfo();
            }
            else
            {
                ModifyOrgInfo();
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

        #endregion


        #region Basic

        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(TxtOrgName.Text))
            {
                ShowException(CurrentApp.GetMessageLanguageInfo("005", "OrgName is empty!"));
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


        #region Operations

        private void AddOrgInfo()
        {
            try
            {
                string strLog = string.Empty;
                if (ObjParent == null) { return; }
                BasicOrgInfo orgParent = ObjParent.Data as BasicOrgInfo;
                if (orgParent == null) { return; }
                string orgName = TxtOrgName.Text;
                if (orgName.Length > 64)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N024", "Organizition Name is error."));
                    return;
                }
                if (orgName.Replace(" ", "") == string.Empty)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N026", "Organizition Name is error."));
                    return;
                }
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10101"), TxtOrgName.Text);
                string orgDesc = TxtDescription.Text;
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10104"), TxtDescription.Text);
                int orgType = 0;
                var orgTypeInfo = ComboOrgType.SelectedItem as OrgTypeInfo;
                if (orgTypeInfo != null)
                {
                    orgType = Convert.ToInt32(orgTypeInfo.ID.ToString().Substring(11));
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10102"), orgTypeInfo.Name);
                }
                BasicOrgInfo newOrgInfo = new BasicOrgInfo();
                newOrgInfo.OrgName = orgName;
                newOrgInfo.OrgType = orgType;
                newOrgInfo.Description = orgDesc;
                newOrgInfo.ParentID = orgParent.OrgID;
                newOrgInfo.IsActived = chkActive.IsChecked == true ? "1" : "0";
                //"1";
                newOrgInfo.IsDeleted = "0";
                newOrgInfo.State = "11111111111111111111111111111111";
                newOrgInfo.StrStartTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                newOrgInfo.StrEndTime = "UNLIMITED";
                newOrgInfo.Creator = CurrentApp.Session.UserInfo.UserID;
                newOrgInfo.CreateTime = DateTime.Now;

                mAsyncResult = false;
                if (PageParent != null)
                {
                    PageParent.SetBusy(true, string.Empty);
                }
                mBackgroundWorker = new BackgroundWorker();
                mBackgroundWorker.DoWork += (s, de) => AddNewOrgInfo(newOrgInfo);
                mBackgroundWorker.RunWorkerCompleted += (s, re) =>
                {
                    mBackgroundWorker.Dispose();
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false, string.Empty);
                    }
                    if (mAsyncResult)
                    {
                        #region 写操作日志

                        CurrentApp.WriteOperationLog(S1101Consts.OPT_ADDORG.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                        #endregion

                        if (PageParent != null)
                        {
                            PageParent.ReloadData(ObjParent);
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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void AddNewOrgInfo(BasicOrgInfo newOrgInfo)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.AddNewOrgInfo;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(newOrgInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.Data = optReturn.Data.ToString();
                Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code == Defines.RET_DBACCESS_EXIST)
                    {
                        ShowException(CurrentApp.GetMessageLanguageInfo("006", "Org already exist"));
                        return;
                    }
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                mAsyncResult = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ModifyOrgInfo()
        {
            try
            {
                string strLog = string.Empty;
                string orgName = TxtOrgName.Text;
                if (TxtOrgName.Text.Length > 64)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N024", "Organazition Name is invalid."));
                    return;
                }
                if (orgName.Replace(" ", "") == string.Empty)
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N026", "Organizition Name is error."));
                    return;
                }
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10101"), orgName);
                string orgDesc = TxtDescription.Text;
                strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10104"), orgDesc);
                int intOrgType = 0;
                var orgType = ComboOrgType.SelectedItem as OrgTypeInfo;
                if (orgType != null)
                {
                    intOrgType = Convert.ToInt32(orgType.ID.ToString().Substring(11));
                    strLog += string.Format("{0} {1} ", Utils.FormatOptLogString("1101T10102"), orgType.Name);
                }

                //0     OrgID
                //1     OrgName
                //2     OrgType
                //3     OrgDesc
                //4     IsActive
                //5    ObjParentID
                List<string> listParams = new List<string>();
                listParams.Add(OrgInfo.OrgID.ToString());
                listParams.Add(orgName);
                listParams.Add(intOrgType.ToString());
                listParams.Add(orgDesc);
                if (chkActive.IsChecked == true)
                {
                    listParams.Add("1");
                }
                else
                {
                    listParams.Add("0");
                }
                listParams.Add(OrgItem.ObjParentID.ToString());

                mAsyncResult = false;
                if (PageParent != null)
                {
                    PageParent.SetBusy(true, string.Empty);
                }
                mBackgroundWorker = new BackgroundWorker();
                mBackgroundWorker.DoWork += (s, de) => ModifyOrgInfo(listParams);
                mBackgroundWorker.RunWorkerCompleted += (s, re) =>
                {
                    mBackgroundWorker.Dispose();
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false, string.Empty);
                    }
                    if (mAsyncResult)
                    {
                        if (PageParent != null)
                        {
                            PageParent.ReloadData(ObjParent);
                        }
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }

                        #region 写操作日志

                        CurrentApp.WriteOperationLog(S1101Consts.OPT_MODIFYORG.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                        #endregion
                    }
                };
                mBackgroundWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ModifyOrgInfo(List<string> listParams)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S1101Codes.ModifyOrgInfo;
                webRequest.ListData = listParams;

                Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code == Defines.RET_DBACCESS_EXIST)
                    {
                        ShowException(CurrentApp.GetMessageLanguageInfo("006", "Org already exist"));
                        return;
                    }
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                mAsyncResult = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Language

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                LabelOrgName.Content = CurrentApp.GetLanguageInfo("1101T10101", "Orgnization Nanme");
                LabelOrgType.Content = CurrentApp.GetLanguageInfo("1101T10102", "Orgnization Type");
                LabelParentOrg.Content = CurrentApp.GetLanguageInfo("1101T10103", "Parent Orgnization");
                LabelDesc.Content = CurrentApp.GetLanguageInfo("1101T10104", "Description");
                LabelActive.Content = CurrentApp.GetLanguageInfo("1101T10109", "Is Active");

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("110110", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("110111", "Close");

                if (mListOrgTypeInfos != null && mListOrgTypeInfos.Count != 0)
                {
                    BasicOrgInfo orgParent = ObjParent.Data as BasicOrgInfo;
                    if (orgParent != null)
                    {
                        OrgTypeItem orgType = mListOrgTypeInfos.FirstOrDefault(
                          ot => ot.ObjID == 9050000000000000000);
                        if (orgType != null)
                        {
                            orgType.Name = CurrentApp.GetLanguageInfo("11011202", orgType.Name); //改显示名字,还有加修改语言时的修改
                        }
                    }
                }

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    if (IsAddOrg)
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("11011200", "Add Orgnization");
                    }
                    else
                    {
                        parent.Title = CurrentApp.GetLanguageInfo("11011201", "Modify Orgnization");
                    }
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion

    }
}
