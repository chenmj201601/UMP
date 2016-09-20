//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cd630dd8-203d-4a76-8226-e3fd433cdea6
//        CLR Version:              4.0.30319.18444
//        Name:                     TreeObjectViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                TreeObjectViewer
//
//        created by Charley at 2014/11/27 13:16:07
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls.Wcf11012;
using VoiceCyber.Wpf.CustomControls;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 一个树形列表控件，用来显示用户可管理的部门，用户，坐席等对象，
    /// 可以勾选若干对象，然后调用GetCheckedObject方法，获取勾选的对象
    /// </summary>
    public class TreeObjectViewer : UMPUserControl
    {

        #region Constructor

        static TreeObjectViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeObjectViewer),
                new FrameworkPropertyMetadata(typeof(TreeObjectViewer)));
        }

        public TreeObjectViewer()
        {
            mRootItem = new TreeObjectItem();
            mListObjects = new List<TreeObjectItem>();
            Loaded += TreeObjectViewer_Loaded;
        }

        #endregion


        #region SessionInfo

        public SessionInfo SessionInfo { get; set; }

        #endregion


        #region Memebers

        private const string PART_PopupSetting = "PART_TreeView";

        private CheckableTree mTreeView;
        private TreeObjectItem mRootItem;
        private List<TreeObjectItem> mListObjects;
        private BackgroundWorker mWorder;

        #endregion


        void TreeObjectViewer_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }


        #region Template

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mTreeView = GetTemplateChild(PART_PopupSetting) as CheckableTree;
            if (mTreeView != null)
            {
                mTreeView.ItemsSource = mRootItem.Children;
            }
        }

        #endregion


        #region Init and Load

        public void Init()
        {
            mRootItem.Children.Clear();
            if (SessionInfo == null)
            {
                return;
            }
            if (SessionInfo.UserInfo == null)
            {
                return;
            }
            mWorder = new BackgroundWorker();
            mWorder.DoWork += (s, de) => InitControlOrgs(mRootItem, "-1");
            mWorder.RunWorkerCompleted += (s, re) =>
            {
                mRootItem.IsChecked = false;
                if (mRootItem.Children.Count > 0)
                {
                    mRootItem.Children[0].IsExpanded = true;
                }
                TreeObjectEventArgs args = new TreeObjectEventArgs();
                args.Code = 100;
                args.Data = mListObjects;
                OnTreeObjectEvent(args);
            };
            mWorder.RunWorkerAsync();
        }

        private void InitControlOrgs(TreeObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = SessionInfo;
                webRequest.Code = (int)RequestCode.WSGetUserObjList;
                webRequest.ListData.Add(SessionInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_ORG.ToString());
                webRequest.ListData.Add(parentID);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(SessionInfo),
                    WebHelper.CreateEndpointAddress(
                        SessionInfo.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    return;
                }
                if (webReturn.ListData == null)
                {
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
                    TreeObjectItem item = new TreeObjectItem();
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
                    InitControlOrgs(item, strID);
                    InitControlUsers(item, strID);
                    InitControlAgents(item, strID);
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                TreeObjectEventArgs args = new TreeObjectEventArgs();
                args.Code = 999;
                args.Message = ex.Message;
                OnTreeObjectEvent(args);
            }
        }

        private void InitControlUsers(TreeObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = SessionInfo;
                webRequest.Code = (int)RequestCode.WSGetUserObjList;
                webRequest.ListData.Add(SessionInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_USER.ToString());
                webRequest.ListData.Add(parentID);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(SessionInfo),
                    WebHelper.CreateEndpointAddress(
                        SessionInfo.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    return;
                }
                if (webReturn.ListData == null)
                {
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
                    TreeObjectItem item = new TreeObjectItem();
                    item.ObjType = ConstValue.RESOURCE_USER;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/user.ico";
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                TreeObjectEventArgs args = new TreeObjectEventArgs();
                args.Code = 999;
                args.Message = ex.Message;
                OnTreeObjectEvent(args);
            }
        }

        private void InitControlAgents(TreeObjectItem parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = SessionInfo;
                webRequest.Code = (int)RequestCode.WSGetUserObjList;
                webRequest.ListData.Add(SessionInfo.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_AGENT.ToString());
                webRequest.ListData.Add(parentID);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(SessionInfo),
                    WebHelper.CreateEndpointAddress(
                        SessionInfo.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    return;
                }
                if (webReturn.ListData == null)
                {
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
                    TreeObjectItem item = new TreeObjectItem();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/agent.ico";
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                    mListObjects.Add(item);
                }
            }
            catch (Exception ex)
            {
                TreeObjectEventArgs args = new TreeObjectEventArgs();
                args.Code = 999;
                args.Message = ex.Message;
                OnTreeObjectEvent(args);
            }
        }

        #endregion


        #region Public Functions

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="listObjects"></param>
        public void GetObjects(ref List<TreeObjectItem> listObjects)
        {
            GetObjects(mRootItem, ref listObjects);
        }

        private void GetObjects(TreeObjectItem parentItem, ref List<TreeObjectItem> listObjects)
        {
            for (int i = 0; i < parentItem.Children.Count; i++)
            {
                TreeObjectItem item = parentItem.Children[i] as TreeObjectItem;
                if (item != null)
                {
                    listObjects.Add(item);
                    GetObjects(item, ref listObjects);
                }
            }
        }
        /// <summary>
        /// 获取选中的对象
        /// </summary>
        /// <param name="listObjects"></param>
        public void GetCheckedObjects(ref List<TreeObjectItem> listObjects)
        {
            GetCheckedObjects(mRootItem, ref listObjects);
        }

        private void GetCheckedObjects(TreeObjectItem parentItem, ref List<TreeObjectItem> listObjects)
        {
            for (int i = 0; i < parentItem.Children.Count; i++)
            {
                TreeObjectItem item = parentItem.Children[i] as TreeObjectItem;
                if (item != null)
                {
                    if (item.IsChecked == true)
                    {
                        listObjects.Add(item);
                    }
                    if (item.IsChecked == false)
                    {
                        continue;
                    }
                    GetCheckedObjects(item, ref listObjects);
                }
            }
        }

        #endregion


        #region TreeObjectEvent

        public event EventHandler<TreeObjectEventArgs> TreeObjectEvent;

        protected virtual void OnTreeObjectEvent(TreeObjectEventArgs e)
        {
            if (TreeObjectEvent != null)
            {
                TreeObjectEvent(this, e);
            }
        }

        #endregion

    }
}
