//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4052b201-0352-4a8e-8177-ea691b8bc160
//        CLR Version:              4.0.30319.18408
//        Name:                     UCRegionManagement
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412
//        File Name:                UCRegionManagement
//
//        created by Charley at 2016/5/12 9:52:58
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using UMPS4412.Models;
using UMPS4412.Wcf11012;
using UMPS4412.Wcf44101;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4412
{
    /// <summary>
    /// UCRegionManagement.xaml 的交互逻辑
    /// </summary>
    public partial class UCRegionManagement
    {

        #region Members

        public ObjItem RegionItem;
        public RegionManageMainView PageParent;

        private bool mIsInited;
        private OrgUserItem mRootItem;
        private List<OrgUserItem> mListOrgUserItems;
        private List<long> mListRegionUserIDs;

        #endregion


        public UCRegionManagement()
        {
            InitializeComponent();

            mRootItem = new OrgUserItem();
            mListOrgUserItems = new List<OrgUserItem>();
            mListRegionUserIDs = new List<long>();

            Loaded += UCRegionManagement_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCRegionManagement_Loaded(object sender, RoutedEventArgs e)
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
                TreeViewOrgUsers.ItemsSource = mRootItem.Children;

                mListOrgUserItems.Clear();
                mRootItem.Children.Clear();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadUserOrgInfos(mRootItem, 0);
                    LoadRegionUserIDs();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    mRootItem.IsChecked = false;
                    if (mRootItem.Children.Count > 0)
                    {
                        mRootItem.Children[0].IsExpanded = true;
                    }
                    InitUserCheckState();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserOrgInfos(OrgUserItem parentItem, long parentID)
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
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                int count = webReturn.ListData.Count;
                for (int i = 0; i < count; i++)
                {
                    string strInfo = webReturn.ListData[i];

                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tResourceObject is null"));
                        return;
                    }
                    OrgUserItem item = new OrgUserItem();
                    item.ObjType = info.ObjType;
                    item.ObjID = info.ObjID;
                    item.Name = info.Name;
                    item.Description = info.FullName;
                    item.Icon = "Images/00001.png";
                    item.Data = info;
                    mListOrgUserItems.Add(item);
                    AddChild(parentItem, item);

                    LoadUserOrgInfos(item, info.ObjID);
                    LoadUserUserInfos(item, info.ObjID);
                }

                CurrentApp.WriteLog("LoadUserOrgInfos", string.Format("Load end.\t{0}", count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserUserInfos(OrgUserItem parentItem, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_USER.ToString());
                webRequest.ListData.Add(parentID.ToString());
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                int count = webReturn.ListData.Count;
                for (int i = 0; i < count; i++)
                {
                    string strInfo = webReturn.ListData[i];

                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tResourceObject is null"));
                        return;
                    }
                    OrgUserItem item = new OrgUserItem();
                    item.ObjType = info.ObjType;
                    item.ObjID = info.ObjID;
                    item.Name = info.Name;
                    item.Description = info.FullName;
                    item.Icon = "Images/00002.png";
                    item.Data = info;
                    mListOrgUserItems.Add(item);
                    AddChild(parentItem, item);
                }

                CurrentApp.WriteLog("LoadUserOrgInfos", string.Format("Load end.\t{0}", count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadRegionUserIDs()
        {
            try
            {
                mListRegionUserIDs.Clear();
                if (RegionItem == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.GetRegionUserList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(RegionItem.ObjID.ToString());
                Service44101Client client = new Service44101Client(
                   WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strID = webReturn.ListData[i];
                    long id;
                    if (long.TryParse(strID, out id))
                    {
                        mListRegionUserIDs.Add(id);
                    }
                }

                CurrentApp.WriteLog("LoadRegionUserIDs", string.Format("Load end.\t{0}", mListRegionUserIDs.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitUserCheckState()
        {
            try
            {
                for (int i = 0; i < mListOrgUserItems.Count; i++)
                {
                    var item = mListOrgUserItems[i];
                    if (item.ObjType != ConstValue.RESOURCE_USER) { continue; }
                    long id = item.ObjID;
                    if (mListRegionUserIDs.Contains(id))
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


        #region Operations

        private void SetRegionMmt()
        {
            try
            {
                List<string> listUserIDStates = new List<string>();
                for (int i = 0; i < mListOrgUserItems.Count; i++)
                {
                    var item = mListOrgUserItems[i];
                    if (item.ObjType != ConstValue.RESOURCE_USER) { continue; }
                    listUserIDStates.Add(string.Format("{0};{1}", item.ObjID, item.IsChecked == true ? 1 : 0));
                }
                SetRegionMmt(RegionItem, listUserIDStates);

                ShowInformation(string.Format("Save end"));

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetRegionMmt(ObjItem regionItem, List<string> listUserIDStates)
        {
            try
            {
                int count = listUserIDStates.Count;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4410Codes.SetRegionMmt;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(regionItem.ObjID.ToString());
                webRequest.ListData.Add(count.ToString());
                for (int i = 0; i < count; i++)
                {
                    webRequest.ListData.Add(listUserIDStates[i]);
                }
                Service44101Client client = new Service44101Client(
                   WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service44101"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }

                CurrentApp.WriteLog("SetRegionMmt", string.Format("End.\t{0}", regionItem.ObjID));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Others

        private void AddChild(OrgUserItem parent, OrgUserItem child)
        {
            Dispatcher.Invoke(new Action(() => parent.AddChild(child)));
        }

        #endregion


        #region Event Handlers

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SetRegionMmt();
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
                    parent.Title = CurrentApp.GetLanguageInfo("4412016", "Region Management");
                }

                BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
                BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");

            }
            catch { }
        }

        #endregion

    }
}
