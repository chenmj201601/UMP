//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    19a0ad6b-1c16-45f6-aebe-31052f8e0bdf
//        CLR Version:              4.0.30319.18408
//        Name:                     UCBugInfoLister
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UCBugInfoLister
//
//        created by Charley at 2016/8/4 10:27:10
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using UMPUpdater.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.Updates;

namespace UMPUpdater
{
    /// <summary>
    /// UCBugInfoLister.xaml 的交互逻辑
    /// </summary>
    public partial class UCBugInfoLister : ILeftView
    {

        #region Members

        public UpdateWindow PageParent { get; set; }
        public UpdateInfo UpdateInfo { get; set; }
        public InstallState InstallState { get; set; }

        private bool mIsInited;

        private ObservableCollection<BugInfoItem> mListAllBugInfoItems;
        private ObservableCollection<BugVersionItem> mListBugVersionItems;

        #endregion


        public UCBugInfoLister()
        {
            InitializeComponent();

            mListAllBugInfoItems = new ObservableCollection<BugInfoItem>();
            mListBugVersionItems = new ObservableCollection<BugVersionItem>();

            Loaded += UCBugInfoLister_Loaded;
            BtnNext.Click += BtnNext_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCBugInfoLister_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        private void Init()
        {
            try
            {
                TabControlBugVersions.ItemsSource = mListBugVersionItems;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    //
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    mListAllBugInfoItems.Clear();
                    mListBugVersionItems.Clear();
                    InitAllBugInfoItems();
                    InitBugVersionItems();

                    TabControlBugVersions.SelectedItem = mListBugVersionItems.FirstOrDefault();

                    ChangeLanguage();
                };
                worker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitAllBugInfoItems()
        {
            try
            {
                if (UpdateInfo == null) { return; }
                var infos = UpdateInfo.ListModules.OrderByDescending(m => m.SerialNo).ToList();
                for (int i = 0; i < infos.Count; i++)
                {
                    var info = infos[i];
                    BugInfoItem item = new BugInfoItem();
                    item.Info = info;
                    item.Type = 0;
                    string strSerialNo = info.SerialNo;
                    item.SerialNo = strSerialNo;
                    item.ModuleID = info.ModuleID;
                    item.Version = GetBugInfoVersion(strSerialNo);
                    //item.Module = GetModuleName(info.ModuleID, info.ModuleName);
                    //item.Content = GetBugContent(strSerialNo, info.Content);
                    mListAllBugInfoItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitBugVersionItems()
        {
            try
            {
                var groups = mListAllBugInfoItems.GroupBy(b => b.Version);
                foreach (var group in groups)
                {
                    BugVersionItem item = new BugVersionItem();
                    item.Version = group.Key;
                    foreach (var bug in group)
                    {
                        item.ListBugInfoItems.Add(bug);
                    }
                    mListBugVersionItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Button Event Handler

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (PageParent != null)
            {
                PageParent.ToClose();
            }
        }

        void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            if (PageParent != null)
            {
                PageParent.ToNext();
            }
        }

        #endregion


        #region Others

        private string GetBugInfoVersion(string strSerialNo)
        {
            try
            {
                string strVersion = string.Empty;
                if (strSerialNo.Length >= 12)
                {
                    int ver1 = int.Parse(strSerialNo.Substring(0, 1));
                    int ver2 = int.Parse(strSerialNo.Substring(1, 2));
                    int ver3 = int.Parse(strSerialNo.Substring(3, 3));
                    int ver4 = int.Parse(strSerialNo.Substring(7, 2));
                    int ver5 = int.Parse(strSerialNo.Substring(9, 3));
                    strVersion = string.Format("{0}.{1}.{2}P{3}.{4}", ver1.ToString("0"), ver2.ToString("00"),
                        ver3.ToString("000"), ver4.ToString("00"), ver5.ToString("000"));
                }
                return strVersion;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetModuleName(int moduleID, string moduleName)
        {
            try
            {
                return App.GetLanguageInfo(string.Format("M{0}", moduleID.ToString("0000")), moduleName);
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetBugContent(string serialNo, string content)
        {
            try
            {
                return App.GetLanguageInfo(string.Format("MC{0}", serialNo), content);
            }
            catch
            {
                return string.Empty;
            }
        }

        #endregion


        #region Basics

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowInformation(string msg)
        {
            MessageBox.Show(msg, App.AppName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion


        #region CheckConfig

        public OperationReturn CheckConfig()
        {
            OperationReturn optReturn=new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        #endregion


        #region ChangeLanguage

        public void ChangeLanguage()
        {
            try
            {
                TxtBugInfoTitle.Text = App.GetLanguageInfo("T001", string.Format("Content of this update package"));

                BtnNext.Content = App.GetLanguageInfo("B002", "Next");
                BtnClose.Content = App.GetLanguageInfo("B003", "Close");

                for (int i = 0; i < mListBugVersionItems.Count; i++)
                {
                    var item = mListBugVersionItems[i];
                    var viewer = item.Viewer;
                    if (viewer != null)
                    {
                        viewer.ChangeLanguage();
                    }
                }
            }
            catch { }
        }

        #endregion

    }
}
