//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    8ce65bc3-a290-482c-bc4c-f56e0f728344
//        CLR Version:              4.0.30319.42000
//        Name:                     UCPathLister
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS2106
//        File Name:                UCPathLister
//
//        Created by Charley at 2016/10/24 16:05:26
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS2106.Models;
using UMPS2106.Wcf21061;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common21061;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS2106
{
    /// <summary>
    /// UCPathLister.xaml 的交互逻辑
    /// </summary>
    public partial class UCPathLister
    {

        #region Members

        public PackageRecoverMainView PageParent;
        public ResourceObjectItem ServerItem;

        private bool mIsInited;
        private ObjectItem mRootItem;

        #endregion


        public UCPathLister()
        {
            InitializeComponent();

            mRootItem = new ObjectItem();

            Loaded += UCPathLister_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCPathLister_Loaded(object sender, RoutedEventArgs e)
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
                TreeViewPathList.ItemsSource = mRootItem.Children;

                if (PageParent != null)
                {
                    PageParent.SetBusy(true, string.Empty);
                }
                mRootItem.Children.Clear();
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    LoadDriverList();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false, string.Empty);
                    }

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadDriverList()
        {
            try
            {
                if (ServerItem == null) { return; }
                var serverInfo = ServerItem.Info;
                if (serverInfo == null) { return; }
                string strAddress = serverInfo.FullName;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S2106Codes.GetDiskDriverList;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(strAddress);
                Service21061Client client = new Service21061Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null."));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                    if (arrInfo.Length < 2) { continue; }
                    DirInfo dirInfo = new DirInfo();
                    dirInfo.Name = arrInfo[0];
                    dirInfo.Path = arrInfo[0] + "\\";
                    ObjectItem item = new ObjectItem();
                    item.Data = dirInfo;
                    item.Name = dirInfo.Name;
                    item.Icon = "/UMPS2106;component/Themes/Default/UMPS2106/Images/00006.png";
                    Dispatcher.Invoke(new Action(() => mRootItem.AddChild(item)));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadChildDirList(ObjectItem parentItem)
        {
            try
            {
                if (parentItem == null) { return; }
                parentItem.Children.Clear();
                if (PageParent != null)
                {
                    PageParent.SetBusy(true, string.Empty);
                }
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    DoLoadChildDirList(parentItem);
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    if (PageParent != null)
                    {
                        PageParent.SetBusy(false, string.Empty);
                    }

                    parentItem.IsExpanded = true;
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DoLoadChildDirList(ObjectItem parentItem)
        {
            try
            {
                if (parentItem == null) { return; }
                var parentDir = parentItem.Data as DirInfo;
                if (parentDir == null) { return; }
                string strDir = parentDir.Path;
                if (ServerItem == null) { return; }
                var serverInfo = ServerItem.Info;
                if (serverInfo == null) { return; }
                string strAddress = serverInfo.FullName;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S2106Codes.GetChildDirList;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(strAddress);
                webRequest.ListData.Add(strDir);
                Service21061Client client = new Service21061Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service21061"));
                WebReturn webReturn = client.DoOperation(webRequest);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null."));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                    if (arrInfo.Length < 2) { continue; }
                    DirInfo dirInfo = new DirInfo();
                    dirInfo.Name = arrInfo[0];
                    dirInfo.Path = arrInfo[1];
                    ObjectItem item = new ObjectItem();
                    item.Data = dirInfo;
                    item.Name = dirInfo.Name;
                    item.Icon = "/UMPS2106;component/Themes/Default/UMPS2106/Images/00007.png";
                    Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

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
            try
            {
                var item = TreeViewPathList.SelectedItem as ObjectItem;
                if (item == null) { return; }
                var dirInfo = item.Data as DirInfo;
                if (dirInfo == null) { return; }
                string strPath = dirInfo.Path;
                if (PageParent != null)
                {
                    PageParent.DoSelectPath(strPath);
                }
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

        private void DirItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                var panel = sender as Border;
                if (panel == null) { return; }
                var item = panel.Tag as ObjectItem;
                if (item == null) { return; }
                LoadChildDirList(item);
            }
        }

        #endregion


        #region ChangeLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            BtnConfirm.Content = CurrentApp.GetLanguageInfo("COM001", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("COM002", "Close");

            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.Title = CurrentApp.GetLanguageInfo("2106015", string.Format("Select package file path"));
            }
        }

        #endregion

    }
}
