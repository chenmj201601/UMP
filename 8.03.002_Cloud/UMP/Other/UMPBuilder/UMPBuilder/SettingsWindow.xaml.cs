//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b7f78daa-b307-40b9-b377-b7a160756b0a
//        CLR Version:              4.0.30319.18063
//        Name:                     SettingsWindow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder
//        File Name:                SettingsWindow
//
//        created by Charley at 2015/12/23 16:10:48
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using UMPBuilder.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace UMPBuilder
{
    /// <summary>
    /// SettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingsWindow
    {
        public SystemConfig SystemConfig;
        public MainWindow PageParent;

        public SettingsWindow()
        {
            InitializeComponent();

            Loaded += SettingsWindow_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            BtnRootDir.Click += BtnRootDir_Click;
            BtnCopyDir.Click += BtnCopyDir_Click;
            BtnUpdateDir.Click += BtnUpdateDir_Click;
            BtnPackageDir.Click += BtnPackageDir_Click;
            BtnCompilerPath.Click += BtnCompilerPath_Click;
            BtnSvnProcPath.Click += BtnSvnProcPath_Click;
        }

        void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }


        #region Init and Load

        private void Init()
        {
            try
            {
                if (SystemConfig == null) { return; }
                TxtRootDir.Text = SystemConfig.RootDir;
                TxtCopyDir.Text = SystemConfig.CopyDir;
                TxtUpdateDir.Text = SystemConfig.UpdateDir;
                TxtPackageDir.Text = SystemConfig.PackageDir;
                TxtCompilerPath.Text = SystemConfig.CompilerPath;
                TxtSvnProcPath.Text = SystemConfig.SvnProcPath;

                var settings = SystemConfig.ListSettings;
                if (settings == null) { return; }
                var setting = settings.FirstOrDefault(s => s.Key == UMPBuilderConsts.GS_SVNUPDATEALL);
                if (setting != null)
                {
                    CbSvnUpdateAll.IsChecked = setting.Value == "1";
                }
                setting = settings.FirstOrDefault(s => s.Key == UMPBuilderConsts.GS_BUILDUPDATER);
                if (setting != null)
                {
                    CbBuildUpdater.IsChecked = setting.Value == "1";
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region Operation

        private bool CheckInput()
        {
            if (string.IsNullOrEmpty(TxtRootDir.Text)
                || string.IsNullOrEmpty(TxtCopyDir.Text)
                || string.IsNullOrEmpty(TxtUpdateDir.Text)
                || string.IsNullOrEmpty(TxtPackageDir.Text)
                || string.IsNullOrEmpty(TxtCompilerPath.Text)
                || string.IsNullOrEmpty(TxtSvnProcPath.Text))
            {
                ShowErrorMessage(string.Format("Some setting invalid!"));
                return false;
            }
            return true;
        }

        private void SaveSettings()
        {
            try
            {
                if (!CheckInput()) { return; }
                if (SystemConfig == null) { return; }
                SystemConfig.RootDir = TxtRootDir.Text;
                SystemConfig.CopyDir = TxtCopyDir.Text;
                SystemConfig.PackageDir = TxtPackageDir.Text;
                SystemConfig.CompilerPath = TxtCompilerPath.Text;
                SystemConfig.SvnProcPath = TxtSvnProcPath.Text;
                SystemConfig.UpdateDir = TxtUpdateDir.Text;

                var setting = SystemConfig.ListSettings.FirstOrDefault(s => s.Key == UMPBuilderConsts.GS_SVNUPDATEALL);
                if (setting == null)
                {
                    setting = new GlobalSetting();
                    setting.Key = UMPBuilderConsts.GS_SVNUPDATEALL;
                    SystemConfig.ListSettings.Add(setting);
                }
                setting.Value = CbSvnUpdateAll.IsChecked == true ? "1" : "0";
                setting = SystemConfig.ListSettings.FirstOrDefault(s => s.Key == UMPBuilderConsts.GS_BUILDUPDATER);
                if (setting == null)
                {
                    setting = new GlobalSetting();
                    setting.Key = UMPBuilderConsts.GS_BUILDUPDATER;
                    SystemConfig.ListSettings.Add(setting);
                }
                setting.Value = CbBuildUpdater.IsChecked == true ? "1" : "0";
                SaveSettingsToConfig();
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void SaveSettingsToConfig()
        {
            try
            {
                if (SystemConfig == null) { return; }
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, SystemConfig.FILE_NAME);
                OperationReturn optReturn = XMLHelper.SerializeFile(SystemConfig, path);
                if (!optReturn.Result)
                {
                    ShowErrorMessage(string.Format("Save settings fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void BtnSvnProcPath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".exe";
                dialog.Filter = "(*.exe)|*.exe";
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    TxtSvnProcPath.Text = dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnCompilerPath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.DefaultExt = ".exe";
                dialog.Filter = "(*.exe)|*.exe";
                var result = dialog.ShowDialog();
                if (result == true)
                {
                    TxtCompilerPath.Text = dialog.FileName;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnPackageDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    TxtPackageDir.Text = dialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnCopyDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    TxtCopyDir.Text = dialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnRootDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    TxtRootDir.Text = dialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            SaveSettings();
        }

        void BtnUpdateDir_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    TxtUpdateDir.Text = dialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        #endregion


        #region Others

        private void ShowErrorMessage(string msg)
        {
            if (PageParent != null)
            {
                PageParent.ShowErrorMessage(msg);
            }
        }

        #endregion

    }
}
