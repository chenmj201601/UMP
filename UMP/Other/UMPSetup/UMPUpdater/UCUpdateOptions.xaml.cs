//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6aa362f3-53af-49a2-8677-2b3db51b3490
//        CLR Version:              4.0.30319.18408
//        Name:                     UCUpdateOptions
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UCUpdateOptions
//
//        created by Charley at 2016/8/8 16:05:14
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using VoiceCyber.UMP.Updates;
using MessageBox = System.Windows.MessageBox;

namespace UMPUpdater
{
    /// <summary>
    /// UCUpdateOptions.xaml 的交互逻辑
    /// </summary>
    public partial class UCUpdateOptions : ILeftView
    {

        #region Members

        public UpdateWindow PageParent { get; set; }
        public UpdateInfo UpdateInfo { get; set; }
        public InstallState InstallState { get; set; }

        public List<InstallProduct> ListProducts;

        private bool mIsInited;

        #endregion


        public UCUpdateOptions()
        {
            InitializeComponent();

            Loaded += UCUpdateOptions_Loaded;

            BtnPrevious.Click += BtnPrevious_Click;
            BtnNext.Click += BtnNext_Click;
            BtnClose.Click += BtnClose_Click;

            CbBackupUMP.Click += (s, e) =>
            {
                PanelBackupUMP.IsEnabled = CbBackupUMP.IsChecked == true;
                RadioLangBackup.IsEnabled = CbBackupUMP.IsChecked == true;
            };
            CbUpdateLang.Click += (s, e) => PanelUpdateLang.IsEnabled = CbUpdateLang.IsChecked == true;
            BtnBackupPath.Click += BtnBackupPath_Click;
        }

        void UCUpdateOptions_Loaded(object sender, RoutedEventArgs e)
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
                if (InstallState == null)
                {
                    CbBackupUMP.IsChecked = false;
                    CbUpdateLang.IsChecked = true;
                    PanelBackupUMP.IsEnabled = false;
                    PanelUpdateLang.IsEnabled = true;
                    RadioLangBackup.IsEnabled = false;
                    TxtBackupPath.Text = string.Empty;
                    CbCompressBackup.IsChecked = true;
                    RadioLangAppend.IsChecked = true;
                    RadioLangModify.IsChecked = false;
                    RadioLangReset.IsChecked = false;
                    RadioLangBackup.IsChecked = false;
                    CbSaveUpdateData.IsChecked = false;
                }
                else
                {
                    bool isBackupUMP = InstallState.IsBackupUMP;
                    bool isUpdateLang = InstallState.IsUpdateLang;
                    string strBackupPath = InstallState.UMPBackupPath;
                    int intLangMode = InstallState.LangUpdateMode;
                    CbBackupUMP.IsChecked = isBackupUMP;
                    CbUpdateLang.IsChecked = isUpdateLang;
                    PanelBackupUMP.IsEnabled = isBackupUMP;
                    PanelUpdateLang.IsEnabled = isUpdateLang;
                    RadioLangBackup.IsEnabled = isBackupUMP;
                    TxtBackupPath.Text = strBackupPath;
                    CbCompressBackup.IsChecked = InstallState.IsCompressBackup;
                    RadioLangAppend.IsChecked = intLangMode == 1;
                    RadioLangModify.IsChecked = intLangMode == 2;
                    RadioLangReset.IsChecked = intLangMode == 3;
                    RadioLangBackup.IsChecked = intLangMode == 4;
                    CbSaveUpdateData.IsChecked = InstallState.IsSaveUpdateData;
                }

                CheckHybridRecording();

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CheckHybridRecording()
        {
            try
            {
                BorderHybridRecording.Visibility = Visibility.Collapsed;
                BorderIxPatch.Visibility = Visibility.Collapsed;
                BorderIPPatch.Visibility = Visibility.Collapsed;

                if (ListProducts == null) { return; }
                var hybridRecording =
                    ListProducts.FirstOrDefault(
                        p =>
                            p.ProductGuid == UpdateConsts.PACKAGE_GUID_IXPATCH ||
                            p.ProductGuid == UpdateConsts.PACKAGE_GUID_SOFTRECORD);
                BorderHybridRecording.Visibility = hybridRecording == null ? Visibility.Collapsed : Visibility.Visible;
                var ixPatch = ListProducts.FirstOrDefault(p => p.ProductGuid == UpdateConsts.PACKAGE_GUID_IXPATCH);
                BorderIxPatch.Visibility = ixPatch == null ? Visibility.Collapsed : Visibility.Visible;
                var ipPatch = ListProducts.FirstOrDefault(p => p.ProductGuid == UpdateConsts.PACKAGE_GUID_SOFTRECORD);
                BorderIPPatch.Visibility = ipPatch == null ? Visibility.Collapsed : Visibility.Visible;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnBackupPath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = App.GetLanguageInfo("N004", string.Format("Please select backup path"));
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string str = dialog.SelectedPath;
                    if (!Directory.Exists(str))
                    {
                        ShowException(string.Format("Directory not exist."));
                        return;
                    }
                    TxtBackupPath.Text = str;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Button Event Handler

        void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (PageParent != null)
            {
                PageParent.ToPrevious();
            }
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            if (PageParent != null)
            {
                PageParent.ToClose();
            }
        }

        void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (InstallState == null)
                {
                    ShowException(string.Format("InstallState is null"));
                    return;
                }
                InstallState.IsBackupUMP = CbBackupUMP.IsChecked == true;
                string str = TxtBackupPath.Text;
                if (string.IsNullOrEmpty(str)
                    && InstallState.IsBackupUMP)
                {
                    ShowException(App.GetLanguageInfo("N004", string.Format("Backup path is empty")));
                    return;
                }
                InstallState.UMPBackupPath = str;
                InstallState.IsCompressBackup = CbCompressBackup.IsChecked == true;
                InstallState.IsUpdateLang = CbUpdateLang.IsChecked == true;
                int intMode = RadioLangAppend.IsChecked == true
                    ? 1
                    : RadioLangModify.IsChecked == true
                        ? 2
                        : RadioLangReset.IsChecked == true ? 3 : RadioLangBackup.IsChecked == true ? 4 : 0;
                InstallState.LangUpdateMode = intMode;
                InstallState.IsSaveUpdateData = CbSaveUpdateData.IsChecked == true;

                if (ListProducts != null)
                {
                    string strIxName = TxtIxPatchRename.Text;
                    string strIPName = TxtIPPatchRename.Text;
                    if (string.IsNullOrEmpty(strIxName))
                    {
                        strIxName = "NtiDrv.dll";
                    }
                    if (string.IsNullOrEmpty(strIPName))
                    {
                        strIPName = "NtiDrv.dll";
                    }

                    var hybridRecording =
                        ListProducts.Count(
                            p =>
                                p.ProductGuid == UpdateConsts.PACKAGE_GUID_IXPATCH ||
                                p.ProductGuid == UpdateConsts.PACKAGE_GUID_SOFTRECORD);
                    if (hybridRecording >= 2
                        && strIxName.Equals(strIPName))
                    {
                        //同时安装了IP 和 三汇 录音时，重命名不能同名
                        ShowException(App.GetLanguageInfo("N011", string.Format("Ntidrv.dll can not be equal name!")));
                        return;
                    }
                    InstallState.IxPatchName = strIxName;
                    InstallState.IPPatchName = strIPName;
                }

                if (PageParent != null)
                {
                    PageParent.ToNext();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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


        #region ChangeLanguage

        public void ChangeLanguage()
        {
            try
            {
                CbBackupUMP.Content = App.GetLanguageInfo("T010", "Backup the UMP system");
                LbBackupPath.Text = App.GetLanguageInfo("T011", "UMP backup path");
                CbCompressBackup.Content = App.GetLanguageInfo("T012", "Compress backup file");
                CbUpdateLang.Content = App.GetLanguageInfo("T013", "Update UMP languages");
                LbLangUpdateMode.Text = App.GetLanguageInfo("T014", "UMP language update mode");
                RadioLangAppend.Content = App.GetLanguageInfo("LM001", "Append");
                RadioLangModify.Content = App.GetLanguageInfo("LM002", "Modify");
                RadioLangReset.Content = App.GetLanguageInfo("LM003", "Reset");
                RadioLangBackup.Content = App.GetLanguageInfo("LM004", "Backup");
                LbOtherOptions.Text = App.GetLanguageInfo("T015", "Other options");
                CbSaveUpdateData.Content = App.GetLanguageInfo("T016", "Save update file");
                LbHybridRecording.Text = App.GetLanguageInfo("T023", "Hybrid Recording");
                LbHybridDesc.Text = App.GetLanguageInfo("T024", "Hybrid recording");
                LbHybridIPPatch.Text = App.GetLanguageInfo("T025", "IP");
                LbHybridIxPatch.Text = App.GetLanguageInfo("T026", "Sanway");
                LbIPPatchRename.Text = App.GetLanguageInfo("T027", "Rename to");
                LbIxPatchRename.Text = App.GetLanguageInfo("T027", "Rename to");

                BtnPrevious.Content = App.GetLanguageInfo("B001", "Previous");
                BtnNext.Content = App.GetLanguageInfo("B002", "Next");
                BtnClose.Content = App.GetLanguageInfo("B003", "Close");
            }
            catch { }
        }

        #endregion

    }
}
