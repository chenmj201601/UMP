//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1cc2b86b-5556-4b30-af10-5f152daaa6e0
//        CLR Version:              4.0.30319.18444
//        Name:                     UserResourceManagement
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101
//        File Name:                UserResourceManagement
//
//        created by Charley at 2014/9/28 17:48:02
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using UMPS1101.Models;
using UMPS1101.Wcf11011;
using UMPS1101.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1101
{
    /// <summary>
    /// UserResourceManagement.xaml 的交互逻辑
    /// </summary>
    public partial class UserResourceManagement
    {
        #region Memembers

        public OUMMainView PageParent;
        public ObjectItem UserItem;

        private ObjectItem mRootItem;
        private List<string> mListUserObjects;
        private List<ObjectItem> mListObjectItems;
        private BackgroundWorker mWorder;
        private bool IsCheckSelfPrimission;
        //public S1101App CurrentApp;

        #endregion

        public UserResourceManagement()
        {
            InitializeComponent();

            mRootItem = new ObjectItem();
            mListUserObjects = new List<string>();
            mListObjectItems = new List<ObjectItem>();
            Loaded += UserResourceManagement_Loaded;
        }

        void UserResourceManagement_Loaded(object sender, RoutedEventArgs e)
        {
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            TvObjects.ItemsSource = mRootItem.Children;

            Init();
            ChangeLanguage();
        }


        #region Init and Load

        private void Init()
        {
            mRootItem.Children.Clear();
            mListObjectItems.Clear();
            if (PageParent != null)
            {
                PageParent.SetBusy(true, string.Empty);
            }
            mWorder = new BackgroundWorker();
            mWorder.DoWork += (s, de) =>
            {
                LoadAvaliableObject();
                LoadControledObject();
            };
            mWorder.RunWorkerCompleted += (s, re) =>
            {
                mWorder.Dispose();
                if (PageParent != null)
                {
                    PageParent.SetBusy(false, string.Empty);
                }
                mRootItem.IsExpanded = true;
                if (mRootItem.Children.Count > 0)
                {
                    mRootItem.Children[0].IsExpanded = true;
                }
                mRootItem.IsChecked = false;
                SetObjectCheckState();
            };
            mWorder.RunWorkerAsync();
        }

        private void LoadAvaliableObject()
        {
            if (UserItem == null) { return; }
            var parentItem = UserItem.Parent as ObjectItem;
            if (parentItem != null)
            {
                LoadUserOrgs(mRootItem);
            }
        }

        private void LoadAvaliableOrgs(ObjectItem parentItem, string parentID)
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
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];

                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Data = strInfo;
                    if (strID == ConstValue.ORG_ROOT.ToString())
                    {
                        item.Icon = "Images/root.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }

                    LoadAvaliableOrgs(item, strID);
                    LoadAvaliableUsers(item, strID);
                    if (PageParent.InitParameter.Contains("A"))
                        LoadAvaliableAgents(item, strID);
                    if (PageParent.InitParameter.Contains("E"))
                        LoadAvaliableExts(item, strID);
                    if (PageParent.InitParameter.Contains("R"))
                        LoadAvaliableRealExts(item, strID);

                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListObjectItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserOrgs(ObjectItem parentItem)
        {
            string OrgID = string.Empty;

            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserObjList;
                webRequest.ListData.Add(UserItem.ObjID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_ORG.ToString());
                webRequest.ListData.Add("-1");
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));

                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));

                }
                if (webReturn.ListData.Count > 0)
                //for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[0];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { return; }
                    OrgID = arrInfo[0];
                    string strName = arrInfo[1];

                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(OrgID);
                    item.Name = strName;
                    item.Data = strInfo;
                    if (OrgID == ConstValue.ORG_ROOT.ToString())
                    {
                        item.Icon = "Images/root.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }

                    LoadAvaliableOrgs(item, OrgID);
                    LoadAvaliableUsers(item, OrgID);
                    if (PageParent.InitParameter.Contains("A"))
                        LoadAvaliableAgents(item, OrgID);
                    if (PageParent.InitParameter.Contains("E"))
                        LoadAvaliableExts(item, OrgID);
                    if (PageParent.InitParameter.Contains("R"))
                        LoadAvaliableRealExts(item, OrgID);
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListObjectItems.Add(item);
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);

            }
        }

        private void LoadAvaliableUsers(ObjectItem parentItem, string parentID)
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
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName.Replace("@", @"\");
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/user.ico";
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListObjectItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAvaliableAgents(ObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_AGENT.ToString());
                webRequest.ListData.Add(parentID);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
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
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName.Replace("@", @"\");
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/agent.ico";
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListObjectItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAvaliableExts(ObjectItem parentItem, string parentID)
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
                //List<ObjectItem> listChild = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicUserInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitControlExtension Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicUserInfo ExtensionInfo = optReturn.Data as BasicUserInfo;
                    if (ExtensionInfo.IsDeleted == "0")
                    {
                        ObjectItem item = new ObjectItem();

                        item.ObjType = S1101Consts.OBJTYPE_EXTENSION;
                        item.ObjID = ExtensionInfo.UserID;
                        item.Icon = "Images/extension.png";
                        item.Name = ExtensionInfo.Account;
                        item.FullName = ExtensionInfo.FullName;
                        item.Description = ExtensionInfo.FullName;
                        item.Data = ExtensionInfo;
                        //listChild.Add(item);
                        Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                        mListObjectItems.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAvaliableRealExts(ObjectItem parentItem, string parentID)
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
                    ShowException(string.Format("InitControlExtension 1 Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                //List<ObjectItem> listChild = new List<ObjectItem>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicUserInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitControlExtension Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicUserInfo ExtensionInfo = optReturn.Data as BasicUserInfo;
                    if (ExtensionInfo.IsDeleted == "0")
                    {
                        ObjectItem item = new ObjectItem();

                        item.ObjType = S1101Consts.OBJTYPE_REALEXTENSION;
                        item.ObjID = ExtensionInfo.UserID;
                        item.Icon = "Images/RealExtension.ico";
                        item.Name = ExtensionInfo.Account;
                        item.FullName = ExtensionInfo.FullName;
                        item.Description = ExtensionInfo.FullName;
                        item.Data = ExtensionInfo;
                        //listChild.Add(item);
                        Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                        mListObjectItems.Add(item);
                    }
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadControledObject()
        {
            Dispatcher.Invoke(new Action(() => mListUserObjects.Clear()));
            try
            {
                if (UserItem == null) { return; }
                BasicUserInfo userInfo = UserItem.Data as BasicUserInfo;
                if (userInfo == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserObjList;
                webRequest.ListData.Add(userInfo.UserID.ToString());
                webRequest.ListData.Add("1");
                string strType = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}", ConstValue.RESOURCE_ORG, ConstValue.SPLITER_CHAR,
                    ConstValue.RESOURCE_USER, ConstValue.RESOURCE_AGENT, ConstValue.RESOURCE_EXTENSION, ConstValue.RESOURCE_REALEXT);
                webRequest.ListData.Add(strType);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                   WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo,
                       "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string objID = webReturn.ListData[i];
                    mListUserObjects.Add(objID);
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

        private void SetObjectCheckState(ObjectItem parentItem)
        {
            if (parentItem == null) { return; }
            try
            {
                if (parentItem.Children.Count > 0)
                {
                    for (int i = 0; i < parentItem.Children.Count; i++)
                    {
                        SetObjectCheckState(parentItem.Children[i] as ObjectItem);
                    }
                }
                else
                {
                    if (mListUserObjects.Contains(parentItem.ObjID.ToString()))
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

        private void AddChildItem(ObjectItem parent, ObjectItem item)
        {
            Dispatcher.Invoke(new Action(() => parent.AddChild(item)));
        }

        #endregion


        #region EventHandlers

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            IsCheckSelfPrimission = true;
            SetUserControlObject();
            if (IsCheckSelfPrimission == false)
                return;
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


        #region Operations

        private void SetUserControlObject()
        {
            if (UserItem == null) { return; }
            BasicUserInfo userInfo = UserItem.Data as BasicUserInfo;
            if (userInfo == null) { return; }
            List<string> listObjectState = new List<string>();
            SetUserControlObject(mRootItem, ref listObjectState);

            if (IsCheckSelfPrimission)
            {
                if (listObjectState.Count > 0)
                {
                    int count = listObjectState.Count;
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(true, string.Empty);
                    }
                    mWorder = new BackgroundWorker();
                    mWorder.DoWork += (s, de) =>
                    {
                        try
                        {
                            WebRequest webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)S1101Codes.SetUserControlObject;
                            webRequest.ListData.Add(userInfo.UserID.ToString());
                            webRequest.ListData.Add(count.ToString());
                            for (int i = 0; i < count; i++)
                            {
                                webRequest.ListData.Add(listObjectState[i]);
                            }
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
                                ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                            }

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
                                                mListObjectItems.FirstOrDefault(o => o.ObjID.ToString() == arrInfos[1]);
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
                                                mListObjectItems.FirstOrDefault(o => o.ObjID.ToString() == arrInfos[1]);
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
                            listLogParams.Add(userInfo.FullName);
                            listLogParams.Add(strAdded);
                            listLogParams.Add(strRemoved);
                            CurrentApp.WriteOperationLog(S1101Consts.OPT_SETUSERMANAGEMENT.ToString(), ConstValue.OPT_RESULT_SUCCESS, "LOG1101001", listLogParams);

                            #endregion
                        }
                        catch (Exception ex)
                        {
                            ShowException(ex.Message);
                        }
                    };
                    mWorder.RunWorkerCompleted += (s, re) =>
                    {
                        mWorder.Dispose();
                        if (PageParent != null)
                        {
                            PageParent.SetBusy(false, string.Empty);
                        }
                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    };
                    mWorder.RunWorkerAsync();
                }
            }
        }

        private void SetUserControlObject(ObjectItem objItem, ref List<string> listObjectState)
        {
            for (int i = 0; i < objItem.Children.Count; i++)
            {
                ObjectItem child = objItem.Children[i] as ObjectItem;
                if (child == null) { continue; }
                if (child.ObjID == UserItem.ObjID)
                {
                    if (child.IsChecked == false)
                    {
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1101N025", "You cannot cancel the management authority."));
                        IsCheckSelfPrimission = false;
                    }
                }
                if (child.IsChecked == true || child.IsChecked == null)
                {
                    listObjectState.Add(string.Format("{0}{1}{2}", child.ObjID, ConstValue.SPLITER_CHAR, "1"));
                }
                else if (child.IsChecked == false)
                {
                    listObjectState.Add(string.Format("{0}{1}{2}", child.ObjID, ConstValue.SPLITER_CHAR, "0"));
                }
                SetUserControlObject(child, ref listObjectState);
            }
        }

        #endregion


        #region Languages

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("110110", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("110111", "Close");

                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.Title = CurrentApp.GetLanguageInfo("11011400", "Set User Role");
                }
            }
            catch (Exception ex)
            { }
        }

        #endregion
    }
}
