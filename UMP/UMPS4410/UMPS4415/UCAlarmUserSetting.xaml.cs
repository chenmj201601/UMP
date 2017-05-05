//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    27b0bd95-7607-4255-af31-c9ffbc1d8250
//        CLR Version:              4.0.30319.18408
//        Name:                     UCAlarmUserSetting
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4415
//        File Name:                UCAlarmUserSetting
//
//        created by Charley at 2016/7/13 15:00:32
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
using UMPS4415.Models;
using UMPS4415.Wcf11012;
using UMPS4415.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4415
{
    /// <summary>
    /// UCAlarmUserSetting.xaml 的交互逻辑
    /// </summary>
    public partial class UCAlarmUserSetting
    {

        #region Members

        public AlarmSettingMainView PageParent;
        public AlarmMessageItem AlarmItem;

        private ObservableCollection<ComboItem> mListUserTypeItems;
        private List<AlarmUserInfo> mListAlarmUserInfos;
        private List<ObjItem> mListUserItems;
        private List<long> mListUserIDs;
        private ObjItem mRootUserItem;

        private bool mIsInited;
        private int mUserType;

        #endregion


        public UCAlarmUserSetting()
        {
            InitializeComponent();

            mListUserTypeItems = new ObservableCollection<ComboItem>();
            mListAlarmUserInfos = new List<AlarmUserInfo>();
            mListUserItems = new List<ObjItem>();
            mListUserIDs = new List<long>();
            mRootUserItem = new ObjItem();

            Loaded += UCAlarmUserSetting_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            ComboUserType.SelectionChanged += ComboUserType_SelectionChanged;
        }

        void UCAlarmUserSetting_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                ComboUserType.ItemsSource = mListUserTypeItems;
                TreeUserList.ItemsSource = mRootUserItem.Children;

                InitUserTypeItems();
                mUserType = ConstValue.RESOURCE_AGENT;
                var item = mListUserTypeItems.FirstOrDefault(u => u.IntValue == mUserType);
                ComboUserType.SelectedItem = item;
                mRootUserItem.Children.Clear();
                mListUserItems.Clear();
                var worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadUserIDs();
                    LoadAlarmUserInfos();
                    LoadOrgItems(mRootUserItem, 0);
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    mRootUserItem.IsChecked = false;
                    if (mRootUserItem.Children.Count > 0)
                    {
                        mRootUserItem.Children[0].IsExpanded = true;
                    }

                    InitCheckState();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitUserTypeItems()
        {
            try
            {
                mListUserTypeItems.Clear();
                ComboItem item = new ComboItem();
                item.Name = "Agent";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4415019{0}", ConstValue.RESOURCE_AGENT.ToString("000")), "Agent");
                item.IntValue = ConstValue.RESOURCE_AGENT;
                mListUserTypeItems.Add(item);
                //item = new ComboItem();
                //item.Name = "Group";
                //item.Display = CurrentApp.GetLanguageInfo(string.Format("4415019{0}", ConstValue.RESOURCE_TECHGROUP.ToString("000")), "Agent");
                //item.IntValue = ConstValue.RESOURCE_TECHGROUP;
                //mListUserTypeItems.Add(item);
                item = new ComboItem();
                item.Name = "Department";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("4415019{0}", ConstValue.RESOURCE_ORG.ToString("000")), "Agent");
                item.IntValue = ConstValue.RESOURCE_ORG;
                mListUserTypeItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserIDs()
        {
            try
            {
                mListUserIDs.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("1");
                //webRequest.ListData.Add(string.Format("{0}{1}{2}{1}{3}", ConstValue.RESOURCE_ORG,
                //    ConstValue.SPLITER_CHAR, ConstValue.RESOURCE_AGENT, ConstValue.RESOURCE_TECHGROUP));
                webRequest.ListData.Add(string.Format("{0}{1}{2}", ConstValue.RESOURCE_ORG,
                    ConstValue.SPLITER_CHAR, ConstValue.RESOURCE_AGENT));
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("0");
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("ResourceObject is null"));
                        return;
                    }
                    long objID = info.ObjID;
                    mListUserIDs.Add(objID);
                }

                CurrentApp.WriteLog("LoadUserIDs", string.Format("End.\t{0}", mListUserIDs.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadOrgItems(ObjItem parent, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_ORG.ToString());
                webRequest.ListData.Add(parentID.ToString());
                webRequest.ListData.Add("0");
                Service11012Client client = new Service11012Client(
                   WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                int count = 0;
                int valid = 0;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    count++;
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("ResourceObject is null"));
                        return;
                    }
                    if (mListUserIDs.Contains(info.ObjID))
                    {
                        ObjItem item = new ObjItem();
                        item.Data = info;
                        item.ObjID = info.ObjID;
                        item.ObjType = info.ObjType;
                        item.Name = info.Name;
                        item.Icon = "Images/00001.png";
                        item.Tip = info.FullName;
                        AddChild(parent, item);
                        mListUserItems.Add(item);
                        LoadOrgItems(item, info.ObjID);
                        if (mUserType == ConstValue.RESOURCE_AGENT)
                        {
                            LoadAgentItems(item, info.ObjID);
                        }
                        valid++;
                    }
                    else
                    {
                        LoadOrgItems(parent, info.ObjID);
                        if (mUserType == ConstValue.RESOURCE_AGENT)
                        {
                            LoadAgentItems(parent, info.ObjID);
                        }
                    }
                }

                CurrentApp.WriteLog("LoadOrgItems", string.Format("End.\tCount:{0};Valid:{1}", count, valid));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAgentItems(ObjItem parent, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_AGENT.ToString());
                webRequest.ListData.Add(parentID.ToString());
                webRequest.ListData.Add("0");
                Service11012Client client = new Service11012Client(
                   WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                int count = 0;
                int valid = 0;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    count++;
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("ResourceObject is null"));
                        return;
                    }
                    if (mListUserIDs.Contains(info.ObjID))
                    {
                        ObjItem item = new ObjItem();
                        item.Data = info;
                        item.ObjID = info.ObjID;
                        item.ObjType = info.ObjType;
                        item.Name = info.Name;
                        item.Icon = "Images/00002.png";
                        item.Tip = info.FullName;
                        AddChild(parent, item);
                        mListUserItems.Add(item);
                        valid++;
                    }
                }

                CurrentApp.WriteLog("LoadAgentItems", string.Format("End.\tCount:{0};Valid:{1}", count, valid));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadAlarmUserInfos()
        {
            try
            {
                mListAlarmUserInfos.Clear();
                if (AlarmItem == null) { return; }
                long alarmID = AlarmItem.SerialID;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetAlarmUserList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(alarmID.ToString());
                Service44101Client client = new Service44101Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<AlarmUserInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    AlarmUserInfo info = optReturn.Data as AlarmUserInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("AlarmUserInfo is null"));
                        return;
                    }
                    mListAlarmUserInfos.Add(info);
                }

                CurrentApp.WriteLog("LoadAlarmUserInfos", string.Format("End.\t{0}", mListAlarmUserInfos.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitCheckState()
        {
            try
            {
                for (int i = 0; i < mListUserItems.Count; i++)
                {
                    var item = mListUserItems[i];
                    var temp = mListAlarmUserInfos.FirstOrDefault(a => a.UserID == item.ObjID);
                    if (temp == null)
                    {
                        item.IsChecked = false;
                    }
                    else
                    {
                        item.IsChecked = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void AddChild(ObjItem parent, ObjItem child)
        {
            Dispatcher.Invoke(new Action(() => parent.AddChild(child)));
        }

        #endregion


        #region Event Handlers

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SaveAlarmUserInfos();
        }

        void ComboUserType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count <= 0) { return; }
            var item = ComboUserType.SelectedItem as ComboItem;
            if (item == null) { return; }
            mUserType = item.IntValue;
            try
            {
                mRootUserItem.Children.Clear();
                mListUserItems.Clear();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadOrgItems(mRootUserItem, 0);
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    mRootUserItem.IsChecked = null;
                    mRootUserItem.IsChecked = false;
                    if (mRootUserItem.Children.Count > 0)
                    {
                        mRootUserItem.Children[0].IsExpanded = true;
                    }

                    InitCheckState();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Operations

        private void SaveAlarmUserInfos()
        {
            try
            {
                if (AlarmItem == null) { return; }
                long alarmID = AlarmItem.SerialID;
                List<ObjItem> listItems = new List<ObjItem>();
                GetCheckItems(mRootUserItem, ref listItems);
                List<AlarmUserInfo> listUserInfos = new List<AlarmUserInfo>();
                for (int i = 0; i < listItems.Count; i++)
                {
                    var item = listItems[i];
                    var objID = item.ObjID;
                    var objType = item.ObjType;
                    var temp = mListAlarmUserInfos.FirstOrDefault(a => a.UserID == objID);
                    if (temp == null)
                    {
                        temp = new AlarmUserInfo();
                        temp.AlarmID = alarmID;
                        temp.UserID = objID;
                        temp.UserType = objType;
                        temp.Creator = CurrentApp.Session.UserID;
                        temp.CreateTime = DateTime.Now.ToUniversalTime();
                    }
                    temp.Modifier = CurrentApp.Session.UserID;
                    temp.ModifyTime = DateTime.Now.ToUniversalTime();
                    listUserInfos.Add(temp);
                }

                int count = listUserInfos.Count;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SaveAlarmUser;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(alarmID.ToString());
                webRequest.ListData.Add(count.ToString());
                OperationReturn optReturn;
                for (int i = 0; i < count; i++)
                {
                    var userInfo = listUserInfos[i];
                    optReturn = XMLHelper.SeriallizeObject(userInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }

                bool isFail = true;
                string strMsg = string.Empty;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    try
                    {
                        Service44101Client client =
                            new Service44101Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            strMsg = string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                            return;
                        }
                        if (webReturn.ListData == null)
                        {
                            strMsg = string.Format("Fail.\tListData is null");
                            return;
                        }
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            CurrentApp.WriteLog("SaveAlarmUserInfos", string.Format("{0}", webReturn.ListData[i]));
                        }
                        isFail = false;
                    }
                    catch (Exception ex)
                    {
                        strMsg = ex.Message;
                    }
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    if (isFail)
                    {
                        ShowException(strMsg);
                    }
                    else
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("4415N010",string.Format("Save Alarm User end.")));

                        var parent = Parent as PopupPanel;
                        if (parent != null)
                        {
                            parent.IsOpen = false;
                        }
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetCheckItems(ObjItem item, ref List<ObjItem> listItems)
        {
            try
            {
                if (item.IsChecked == false) { return; }
                if (item.IsChecked == true)
                {
                    listItems.Add(item);
                }
                for (int i = 0; i < item.Children.Count; i++)
                {
                    var child = item.Children[i] as ObjItem;
                    if (child == null) { continue; }
                    GetCheckItems(child, ref listItems);
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
                    parent.Title = CurrentApp.GetLanguageInfo("4415017", "Add Alarm");
                }

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");

                LbUserType.Text = CurrentApp.GetLanguageInfo("4415018", "User Type");

                for (int i = 0; i < mListUserTypeItems.Count; i++)
                {
                    var item = mListUserTypeItems[i];
                    int intValue = item.IntValue;

                    item.Display = CurrentApp.GetLanguageInfo(string.Format("4415019{0}", intValue.ToString("000")), intValue.ToString());
                }
            }
            catch { }
        }

        #endregion

    }
}
