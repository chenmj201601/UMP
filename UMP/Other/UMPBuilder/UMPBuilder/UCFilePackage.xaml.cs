//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    75c45075-ef1c-4510-ab58-2f74e56efc92
//        CLR Version:              4.0.30319.18063
//        Name:                     UCFilePackage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder
//        File Name:                UCFilePackage
//
//        created by Charley at 2015/12/24 14:44:40
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.IO;
using System.Linq;
using System.Windows;
using UMPBuilder.Models;

namespace UMPBuilder
{
    /// <summary>
    /// UCFilePackage.xaml 的交互逻辑
    /// </summary>
    public partial class UCFilePackage
    {
        public MainWindow PageParent;
        public SystemConfig SystemConfig;

        private long mTotalSize;

        public UCFilePackage()
        {
            InitializeComponent();

            Loaded += UCFilePackage_Loaded;
        }

        void UCFilePackage_Loaded(object sender, RoutedEventArgs e)
        {
            Init();
        }

        private void Init()
        {
            try
            {
                if (SystemConfig != null)
                {
                    var setting =
                        SystemConfig.ListSettings.FirstOrDefault(s => s.Key == UMPBuilderConsts.GS_BUILDUPDATER);
                    string dir;
                    if (setting != null && setting.Value == "1")
                    {
                        dir = SystemConfig.UpdateDir;
                    }
                    else
                    {
                        dir = SystemConfig.CopyDir;
                    }
                    if (Directory.Exists(dir))
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(dir);
                        long size = GetDirSize(dirInfo);
                        mTotalSize = size;
                        ProgressPackage.Maximum = mTotalSize;
                        TxtProgress.Text = string.Format("0 %");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        public void SetProgress(long size, string msg)
        {
            try
            {
                ProgressPackage.Value = size;
                double percentage = 0.0;
                if (mTotalSize > 0)
                {
                    percentage = size * 1.0 / mTotalSize;
                }
                percentage = percentage * 100;
                TxtProgress.Text = string.Format("{0} %", percentage.ToString("0.00"));
                TxtMsg.AppendText(string.Format("{0}\r\n", msg));
                TxtMsg.ScrollToEnd();
            }
            catch { }
        }

        private long GetDirSize(DirectoryInfo dirInfo)
        {
            long size = 0;
            DirectoryInfo[] dirs = dirInfo.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                var dir = dirs[i];
                size += GetDirSize(dir);
            }
            FileInfo[] files = dirInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                var file = files[i];
                size += file.Length;
            }
            return size;
        }

        private void ShowErrorMessage(string msg)
        {
            if (PageParent != null)
            {
                PageParent.ShowErrorMessage(msg);
            }
        }
    }
}
