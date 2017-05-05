//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    25b9c500-fb7e-469b-8558-76c42682e5ec
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreUserManagement
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101
//        File Name:                ScoreUserManagement
//
//        created by Charley at 2014/10/24 15:50:35
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using UMPS3101.Models;
using UMPS3101.Wcf11012;
using UMPS3101.Wcf31011;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3101
{
    /// <summary>
    /// ScoreUserManagement.xaml 的交互逻辑
    /// </summary>
    public partial class ScoreUserManagement
    {
        #region members

        public SSMMainView PageParent;
        public ScoreSheetItem ScoreSheetItem;

        public OrgUserItem mRootItem;
        private List<string> mListScoreSheetUsers;
        private List<OrgUserItem> mListOrgUserItems; 
        private BackgroundWorker mWorker;

        #endregion


        public ScoreUserManagement()
        {
            InitializeComponent();

            mRootItem = new OrgUserItem();
            mListOrgUserItems = new List<OrgUserItem>();
            mListScoreSheetUsers = new List<string>();
            Loaded += ScoreUserManagement_Loaded;
        }

        void ScoreUserManagement_Loaded(object sender, RoutedEventArgs e)
        {
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            TvObjects.ItemsSource = mRootItem.Children;

            Init();
            ChangeLanguage();
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SetScoreSheetUser();
        }


        #region Init andLoad

        private void Init()
        {
            mRootItem.Children.Clear();
            mListOrgUserItems.Clear();
            if (PageParent != null)
            {
                PageParent.SetBusy(true, string.Empty);
            }
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                LoadAvaliableObject();
                LoadScoreSheetUser();
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                if (PageParent != null)
                {
                    PageParent.SetBusy(false, string.Empty);
                }
                SetObjectCheckState();
            };
            mWorker.RunWorkerAsync();
        }

        private void LoadAvaliableObject()
        {
            try
            {
                LoadAvaliableOrgs(mRootItem, "-1");
                Dispatcher.Invoke(new Action(() =>
                {
                    mRootItem.IsChecked = false;
                    if (mRootItem.Children.Count > 0)
                    {
                        mRootItem.Children[0].IsExpanded = true;
                    }
                }));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        private void LoadAvaliableOrgs(OrgUserItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_ORG.ToString());
                webRequest.ListData.Add(parentID);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
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
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    OrgUserItem item = new OrgUserItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Data = strInfo;
                    if (strID == ConstValue.ORG_ROOT.ToString())
                    {
                        item.Icon = "Images/rootorg.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }
                    LoadAvaliableOrgs(item, strID);
                    //LoadAvaliableUsers(item, strID);
                    if (S3101App.GroupingWay.Contains("A"))
                    {
                        LoadAvaliableAgent(item, strID);
                    }
                    else if (S3101App.GroupingWay.Contains("R"))
                    {
                        LoadAvaliableRealityExtension(item, strID);
                    }
                    else if (S3101App.GroupingWay.Contains("E"))
                    {
                        LoadAvaliableExtension(item, strID);
                    }
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListOrgUserItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAvaliableAgent(OrgUserItem parentItem, string parentID) 
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3101Codes.GetCtrolAgent;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31011Client client = new Service31011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31011"));
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
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    OrgUserItem item = new OrgUserItem();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/user_suit.png";
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListOrgUserItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAvaliableRealityExtension(OrgUserItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3101Codes.GetCtrolReExtension;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("R");
                Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,"Service31011"));
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
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    OrgUserItem item = new OrgUserItem();
                    item.ObjType = ConstValue.RESOURCE_REALEXT;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/RealExtension.ico";
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListOrgUserItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAvaliableExtension(OrgUserItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3101Codes.GetCtrolReExtension;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("E");
                Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,"Service31011"));
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
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    OrgUserItem item = new OrgUserItem();
                    item.ObjType = ConstValue.RESOURCE_EXTENSION;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/extension.ico";
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListOrgUserItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAvaliableUsers(OrgUserItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_USER.ToString());
                webRequest.ListData.Add(parentID);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
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
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    OrgUserItem item = new OrgUserItem();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/user.ico";
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListOrgUserItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadScoreSheetUser()
        {
            try
            {
                if (ScoreSheetItem == null) { return; }
                string scoreSheetID = ScoreSheetItem.ID.ToString();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3101Codes.GetScoreSheetUserList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(scoreSheetID);
                Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("WebReturn ListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    mListScoreSheetUsers.Add(webReturn.ListData[i]);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void SetObjectCheckState()
        {
            SetObjectCheckState(mRootItem);
        }

        private void SetObjectCheckState(OrgUserItem parentItem)
        {
            if (parentItem == null) { return; }
            try
            {
                if (parentItem.Children.Count > 0)
                {
                    for (int i = 0; i < parentItem.Children.Count; i++)
                    {
                        SetObjectCheckState(parentItem.Children[i] as OrgUserItem);
                    }
                }
                else
                {
                    if (mListScoreSheetUsers.Contains(parentItem.ObjID.ToString()))
                    {
                        parentItem.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void SetScoreSheetUser()
        {
            if (ScoreSheetItem == null) { return; }
            List<string> listObjectState = new List<string>();
            SetScoreSheetUser(mRootItem, ref listObjectState);

            if (listObjectState.Count > 0)
            {
                if (PageParent != null)
                {
                    PageParent.SetBusy(true,string.Empty);
                }
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
                        int count = listObjectState.Count;
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = (int)S3101Codes.SetScoreSheetUser;
                        webRequest.ListData.Add(ScoreSheetItem.ID.ToString());
                        webRequest.ListData.Add(count.ToString());
                        for (int i = 0; i < count; i++)
                        {
                            webRequest.ListData.Add(listObjectState[i]);
                        }
                        Service31011Client client = new Service31011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                         WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31011"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                            return;
                        }
                        CurrentApp.WriteLog("SetScoreSheetUser", webReturn.Data);

                        #region 写操作日志

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
                                            mListOrgUserItems.FirstOrDefault(o => o.ObjID.ToString() == arrInfos[1]);
                                        if (objItem != null)
                                        {
                                            strAdded += objItem.Name + ",";
                                        }
                                        else
                                        {
                                            strAdded += arrInfos[1] + ",";
                                        }
                                    }
                                    if (arrInfos[0] == "D")
                                    {
                                        var objItem =
                                            mListOrgUserItems.FirstOrDefault(o => o.ObjID.ToString() == arrInfos[1]);
                                        if (objItem != null)
                                        {
                                            strRemoved += objItem.Name + ",";
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
                        listLogParams.Add(strAdded);
                        listLogParams.Add(strRemoved);
                        CurrentApp.WriteOperationLog(S3101Consts.OPT_SETMANAGEUSER.ToString(), ConstValue.OPT_RESULT_SUCCESS, "3101Log0001", listLogParams);

                        #endregion

                        ShowInformation(CurrentApp.GetMessageLanguageInfo("004", "Set ScoreSheet user end"));
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false,string.Empty);
                    }
                    var parent = Parent as PopupPanel;
                    if (parent != null)
                    {
                        parent.IsOpen = false;
                    }
                };
                mWorker.RunWorkerAsync();
            }
        }

        private void SetScoreSheetUser(OrgUserItem objItem, ref List<string> listObjectState)
        {
            try
            {
                for (int i = 0; i < objItem.Children.Count; i++)
                {
                    OrgUserItem child = objItem.Children[i] as OrgUserItem;
                    if (child == null) { continue; }
                    if (child.ObjType == ConstValue.RESOURCE_AGENT || child.ObjType == ConstValue.RESOURCE_REALEXT || child.ObjType == ConstValue.RESOURCE_EXTENSION)
                    {
                        if (child.IsChecked == true)
                        {
                            listObjectState.Add(string.Format("{0}{1}{2}", child.ObjID, ConstValue.SPLITER_CHAR, "1"));
                        }
                        else if (child.IsChecked == false)
                        {
                            listObjectState.Add(string.Format("{0}{1}{2}", child.ObjID, ConstValue.SPLITER_CHAR, "0"));
                        }
                    }
                    SetScoreSheetUser(child, ref listObjectState);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("31011100", "Set ScoreSheet Permissions");
                }
                BtnConfirm.Content = CurrentApp.GetLanguageInfo("31010", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("31012", "Close");
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion
    }
}
