//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    36f59924-a030-456a-9012-48d224d97839
//        CLR Version:              4.0.30319.18408
//        Name:                     UCUpdateProgress
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                UCUpdateProgress
//
//        created by Charley at 2016/8/8 16:05:42
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Xml;
using UMPUpdater.Models;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.SharpZips.Zip;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Updates;

namespace UMPUpdater
{
    /// <summary>
    /// UCUpdateProgress.xaml 的交互逻辑
    /// </summary>
    public partial class UCUpdateProgress : ILeftView
    {

        #region Members

        public UpdateWindow PageParent { get; set; }
        public UpdateInfo UpdateInfo { get; set; }
        public InstallState InstallState { get; set; }

        public List<InstallProduct> ListProducts;
        public List<ServiceInfo> ListAllServices;
        public List<ServiceInfo> ListInstalledServices;
        public DatabaseInfo DatabaseInfo;

        private bool mIsInited;
        private bool mIsShowDetail;
        private bool mIsOptFail;
        private string mErrMsg;
        private InstallInfo mInstallInfo;

        private ObservableCollection<UpdateStateItem> mListUpdateStateItems;
        private List<LoggingServerInfo> mListLoggingServerInfos;
        private AppServerInfo mAppServerInfo;

        #endregion


        public UCUpdateProgress()
        {
            InitializeComponent();

            mListUpdateStateItems = new ObservableCollection<UpdateStateItem>();
            mListLoggingServerInfos = new List<LoggingServerInfo>();

            Loaded += UCUpdateProgress_Loaded;

            BtnPrevious.Click += BtnPrevious_Click;
            BtnUpdate.Click += BtnUpdate_Click;
            BtnClose.Click += BtnClose_Click;

            BtnDetail.Click += (s, e) =>
            {
                mIsShowDetail = !mIsShowDetail;
                TxtDetail.Visibility = mIsShowDetail ? Visibility.Visible : Visibility.Collapsed;
            };
        }

        void UCUpdateProgress_Loaded(object sender, RoutedEventArgs e)
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
                LvUpdateStateItems.ItemsSource = mListUpdateStateItems;

                mIsShowDetail = false;
                InitUpdateStateItems();
                CreateUpdateItemColumns();

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitUpdateStateItems()
        {
            try
            {
                mListUpdateStateItems.Clear();
                UpdateStateItem item = new UpdateStateItem();
                item.State = ConstDefines.STA_BACKUPUMP;
                item.Name = ConstDefines.STA_NAME_BACKUPUMP;
                item.Display = App.GetLanguageInfo(string.Format("U{0}", item.State.ToString("000")), item.Name);
                item.Progress = 0;
                item.Result = 0;
                SetStateItemDisplay(item);
                mListUpdateStateItems.Add(item);
                item = new UpdateStateItem();
                item.State = ConstDefines.STA_STOPSERVICE;
                item.Name = ConstDefines.STA_NAME_STOPSERVICE;
                item.Display = App.GetLanguageInfo(string.Format("U{0}", item.State.ToString("000")), item.Name);
                item.Progress = 0;
                item.Result = 0;
                SetStateItemDisplay(item);
                mListUpdateStateItems.Add(item);
                item = new UpdateStateItem();
                item.State = ConstDefines.STA_UPDATEFILE;
                item.Name = ConstDefines.STA_NAME_UPDATEFILE;
                item.Display = App.GetLanguageInfo(string.Format("U{0}", item.State.ToString("000")), item.Name);
                item.Progress = 0;
                item.Result = 0;
                SetStateItemDisplay(item);
                mListUpdateStateItems.Add(item);
                item = new UpdateStateItem();
                item.State = ConstDefines.STA_UPDATEDATABASE;
                item.Name = ConstDefines.STA_NAME_UPDATEDATABASE;
                item.Display = App.GetLanguageInfo(string.Format("U{0}", item.State.ToString("000")), item.Name);
                item.Progress = 0;
                item.Result = 0;
                SetStateItemDisplay(item);
                mListUpdateStateItems.Add(item);
                item = new UpdateStateItem();
                item.State = ConstDefines.STA_UPDATELANG;
                item.Name = ConstDefines.STA_NAME_UPDATELANG;
                item.Display = App.GetLanguageInfo(string.Format("U{0}", item.State.ToString("000")), item.Name);
                item.Progress = 0;
                item.Result = 0;
                SetStateItemDisplay(item);
                mListUpdateStateItems.Add(item);
                item = new UpdateStateItem();
                item.State = ConstDefines.STA_UPDATESERVICE;
                item.Name = ConstDefines.STA_NAME_UPDATESERVICE;
                item.Display = App.GetLanguageInfo(string.Format("U{0}", item.State.ToString("000")), item.Name);
                item.Progress = 0;
                item.Result = 0;
                SetStateItemDisplay(item);
                mListUpdateStateItems.Add(item);
                item = new UpdateStateItem();
                item.State = ConstDefines.STA_STARTSERVICE;
                item.Name = ConstDefines.STA_NAME_STARTSERVICE;
                item.Display = App.GetLanguageInfo(string.Format("U{0}", item.State.ToString("000")), item.Name);
                item.Progress = 0;
                item.Result = 0;
                SetStateItemDisplay(item);
                mListUpdateStateItems.Add(item);
                item = new UpdateStateItem();
                item.State = ConstDefines.STA_UPDATESERVER;
                item.Name = ConstDefines.STA_NAME_UPDATESERVER;
                item.Display = App.GetLanguageInfo(string.Format("U{0}", item.State.ToString("000")), item.Name);
                item.Progress = 0;
                item.Result = 0;
                SetStateItemDisplay(item);
                mListUpdateStateItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void UpdateUMP()
        {
            try
            {
                if (InstallState == null) { return; }
                if (UpdateInfo == null) { return; }

                var a = CompareVersion(UpdateInfo.Version, InstallState.CurrentVersion);
                if (a > 0)
                {
                    ShowException(App.GetLanguageInfo("N010", string.Format("Newer product has installed on this machine, can not update!")));
                    return;
                }

                if (InstallState.IsUMPInstalled)
                {
                    if (!InstallState.IsLogined)
                    {
                        ShowException(App.GetLanguageInfo("N005", string.Format("Please login before Update!")));
                        return;
                    }
                }

                var result = MessageBox.Show(App.GetLanguageInfo("N006", string.Format("Confirm update UMP?")), App.AppName, MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }

                BtnPrevious.IsEnabled = false;
                BtnUpdate.IsEnabled = false;
                BtnClose.IsEnabled = false;

                ResetStateItem();
                PanelUpdateResult.Visibility = Visibility.Collapsed;
                if (InstallState == null) { return; }
                if (UpdateInfo == null) { return; }

                #region Init InstallInfo

                DateTime now = DateTime.Now;
                InstallInfo installInfo = new InstallInfo();
                installInfo.SessionID = Guid.NewGuid().ToString();
                installInfo.InstallTime = now.ToString("yyyy-MM-dd HH:mm:ss");
                installInfo.BeginTime = now.ToString("yyyy-MM-dd HH:mm:ss");
                installInfo.MachineName = Environment.MachineName;
                installInfo.OSAccount = Environment.UserName;
                installInfo.OSVersion = Environment.OSVersion.ToString();
                installInfo.UpdateInfo = UpdateInfo;
                installInfo.Type = UpdateInfo.Type;
                installInfo.Version = UpdateInfo.Version;
                mInstallInfo = installInfo;
                App.WriteLog("Update", string.Format("InstallInfo inited.\t{0}", installInfo.SessionID));

                #endregion

                mIsOptFail = false;
                mErrMsg = string.Empty;
                AppendMessage(string.Format("Begin update UMP..."));
                App.WriteLog("Update", string.Format("Begin update UMP..."));
                SetTotalProgress(0);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    BackupUMP();
                    StopServices();
                    UpdateFiles();
                    UpdateDatabase();
                    UpdateLanguages();
                    UpdateServices();
                    StartServices();
                    UpdateServers();
                    UpdateFollows();
                    DeleteTempFiles();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    now = DateTime.Now;
                    mInstallInfo.EndTime = now.ToString("yyyy-MM-dd HH:mm:ss");

                    SetCurrentOperation(string.Empty);
                    BtnPrevious.IsEnabled = true;
                    BtnUpdate.IsEnabled = true;
                    BtnClose.IsEnabled = true;

                    InstallState.IsOptFail = mIsOptFail;
                    InstallState.ErrorMessage = mErrMsg;
                    if (mIsOptFail)
                    {
                        AppendMessage(string.Format("Update fail.\t{0}", mErrMsg));
                        App.WriteLog("Update", string.Format("Update fail.\t{0}", mErrMsg));
                        TxtUpdateResult.Text = App.GetLanguageInfo("T018", string.Format("UMP update fail!"));
                        ShowException(string.Format("Update fail.\t{0}", mErrMsg));
                    }
                    else
                    {
                        AppendMessage(string.Format("Update successful.\t{0}", mInstallInfo.SessionID));
                        App.WriteLog("Update", string.Format("Update successful.\t{0}", mInstallInfo.SessionID));
                        SaveInstallInfo();
                        SaveInstallInfoDB();
                        SaveUpdateModuleDB();
                        SaveUpdateDetailDB();
                        if (PageParent != null)
                        {
                            PageParent.RefreshRightView();
                        }
                        TxtUpdateResult.Text = App.GetLanguageInfo("T017", string.Format("UMP update successful!"));
                    }
                    PanelUpdateResult.Visibility = Visibility.Visible;
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Operations

        private void BackupUMP()
        {
            if (mIsOptFail) { return; }

            var stateItem = mListUpdateStateItems.FirstOrDefault(s => s.State == ConstDefines.STA_BACKUPUMP);
            try
            {
                AppendMessage(string.Format("Begin backup UMP..."));
                App.WriteLog("BackupUMP", string.Format("Begin backup UMP..."));
                if (stateItem != null)
                {
                    stateItem.Result = 1;
                    stateItem.Progress = 0;
                    SetStateItemDisplay(stateItem);
                }

                if (InstallState.IsBackupUMP)
                {
                    //备份程序文件分为两步，第一步（占比 30% ）将文件Copy到备份目录；第二步（占比 70% ）压缩备份的文件

                    if (ListProducts == null) { return; }
                    if (!Directory.Exists(InstallState.UMPBackupPath))
                    {
                        Directory.CreateDirectory(InstallState.UMPBackupPath);
                    }
                    string strInstallTime = DateTime.Parse(mInstallInfo.InstallTime).ToString("yyyyMMddHHmmss");
                    string strBackupPath = Path.Combine(InstallState.UMPBackupPath,
                        string.Format("UMPData_{0}", strInstallTime));
                    if (!Directory.Exists(strBackupPath))
                    {
                        Directory.CreateDirectory(strBackupPath);
                    }

                    #region Step 1  此步占比 30 %

                    int count = ListProducts.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var product = ListProducts[i];
                        string strProductName = product.ProductName;
                        LogInfo(string.Format("Backup product {0}...", strProductName));
                        App.WriteLog("BackupUMP", string.Format("Backup product {0}...", strProductName));
                        string strSource = product.InstallPath;
                        if (!Directory.Exists(strSource))
                        {
                            App.WriteLog("BackupUMP", string.Format("Source directory not exist.\t{0}", strSource));
                            continue;
                        }
                        string strTarget = Path.Combine(strBackupPath, strProductName);
                        if (!Directory.Exists(strTarget))
                        {
                            Directory.CreateDirectory(strTarget);
                        }

                        DirectoryInfo SourceDir = new DirectoryInfo(strSource);
                        FileInfo[] subFiles = SourceDir.GetFiles();
                        DirectoryInfo[] subDirs = SourceDir.GetDirectories();
                        int fileCount = subFiles.Length;
                        int dirCount = subDirs.Length;
                        int num = fileCount + dirCount;
                        for (int j = 0; j < fileCount; j++)
                        {
                            FileInfo subInfo = subFiles[j];
                            string source = Path.Combine(strSource, subInfo.Name);
                            string target = Path.Combine(strTarget, subInfo.Name);
                            BackupSingFile(source, target);

                            #region 更新进度

                            var pp = (((j + 1) / (num * 1.0)) / count + (i * 1.0) / count) * 0.3;
                            if (stateItem != null)
                            {
                                stateItem.Progress = pp * 100;
                                SetStateItemDisplay(stateItem);
                            }
                            SetTotalProgress(pp * ConstDefines.PRO_BACKUPUMP + ConstDefines.PRO_BASE_BACKUPUMP);

                            #endregion

                        }
                        for (int j = 0; j < dirCount; j++)
                        {
                            DirectoryInfo subInfo = subDirs[j];
                            string source = Path.Combine(strSource, subInfo.Name);
                            string target = Path.Combine(strTarget, subInfo.Name);
                            BackupDirectory(source, target);

                            #region 更新进度

                            var pp = (((j + 1 + fileCount) / (num * 1.0)) / count + (i * 1.0) / count) * 0.3;
                            if (stateItem != null)
                            {
                                stateItem.Progress = pp * 100;
                                SetStateItemDisplay(stateItem);
                            }
                            SetTotalProgress(pp * ConstDefines.PRO_BACKUPUMP + ConstDefines.PRO_BASE_BACKUPUMP);

                            #endregion

                        }

                        AppendMessage(string.Format("Backup product {0} end.", strProductName));


                        #region 更新进度

                        var ppp = (i + 1) / (count * 1.0) * 0.3;
                        if (stateItem != null)
                        {
                            stateItem.Progress = ppp * 100;
                            SetStateItemDisplay(stateItem);
                        }
                        SetTotalProgress(ppp * ConstDefines.PRO_BACKUPUMP + ConstDefines.PRO_BASE_BACKUPUMP);

                        #endregion

                    }

                    #region 更新进度

                    var p = 0.3;
                    if (stateItem != null)
                    {
                        stateItem.Progress = p * 100;
                        SetStateItemDisplay(stateItem);
                    }
                    SetTotalProgress(p * ConstDefines.PRO_BACKUPUMP + ConstDefines.PRO_BASE_BACKUPUMP);

                    #endregion

                    #endregion


                    #region Step 2 此步占比 70%

                    if (InstallState.IsCompressBackup)
                    {
                        string compressFile = Path.Combine(InstallState.UMPBackupPath,
                      string.Format("UMPData_{0}.zip", strInstallTime));
                        ZipOutputStream stream = new ZipOutputStream(File.Create(compressFile));
                        stream.SetLevel(9);
                        count = ListProducts.Count;
                        for (int i = 0; i < count; i++)
                        {
                            var product = ListProducts[i];
                            string strProductName = product.ProductName;
                            LogInfo(string.Format("Compress product {0}...", strProductName));
                            App.WriteLog("BackupUMP", string.Format("Compress product {0}...", strProductName));
                            string strPath = Path.Combine(strBackupPath, strProductName);
                            if (!Directory.Exists(strPath)) { continue; }
                            DirectoryInfo dirInfo = new DirectoryInfo(strPath);
                            long totalSize = GetDirSize(dirInfo);
                            App.WriteLog("BackupUMP", string.Format("Compress product {0}... Path {1} Total size {2}", strProductName, strPath, totalSize));

                            BackupState backupState = new BackupState();
                            backupState.Dir = strPath;
                            backupState.Name = strProductName;
                            backupState.ProductName = strProductName;
                            backupState.Stream = stream;
                            backupState.TotalSize = totalSize;
                            backupState.CurrentSize = 0;
                            backupState.ProductCount = count;
                            backupState.ProductIndex = i;
                            backupState.StateItem = stateItem;

                            BackupUMP(backupState);

                            #region 更新进度

                            p = ((i + 1) / (count * 1.0)) * 0.7 + 0.3;
                            if (stateItem != null)
                            {
                                stateItem.Progress = p * 100;
                                SetStateItemDisplay(stateItem);
                            }
                            SetTotalProgress(p * ConstDefines.PRO_BACKUPUMP + ConstDefines.PRO_BASE_BACKUPUMP);

                            #endregion

                            AppendMessage(string.Format("Compress product {0} end.", strProductName));
                            App.WriteLog("BackupUMP", string.Format("Compress product {0} end.", strProductName));
                        }
                        stream.Flush();
                        stream.Close();
                        App.WriteLog("BackupUMP", string.Format("Compress UMP end.\t{0}", compressFile));
                    }

                    #endregion


                    #region 压缩完成之后把Copy的临时文件删掉

                    try
                    {
                        Directory.Delete(strBackupPath, true);
                    }
                    catch (Exception ex)
                    {
                        App.WriteLog("BackupUMP", string.Format("Delete temp directory fail.\t{0}", ex.Message));
                    }

                    #endregion

                }

                SetTotalProgress(ConstDefines.PRO_BASE_BACKUPUMP + ConstDefines.PRO_BACKUPUMP);
                if (stateItem != null)
                {
                    stateItem.Result = 2;
                    stateItem.Progress = 100;
                    SetStateItemDisplay(stateItem);
                }
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrMsg = ex.Message;
                if (stateItem != null)
                {
                    stateItem.Result = 3;
                    SetStateItemDisplay(stateItem);
                }
            }
        }

        private void BackupUMP(BackupState backupState)
        {
            string strDir = backupState.Dir;
            string strName = backupState.Name;
            string strProductName = backupState.ProductName;
            ZipOutputStream stream = backupState.Stream;
            long totalSize = backupState.TotalSize;
            long currentSize = backupState.CurrentSize;
            int productCount = backupState.ProductCount;
            int productIndex = backupState.ProductIndex;
            UpdateStateItem stateItem = backupState.StateItem;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(strDir);
                FileInfo[] files = dirInfo.GetFiles();
                for (int i = 0; i < files.Length; i++)
                {
                    FileInfo file = files[i];
                    long size = file.Length;

                    //App.WriteLog("BackupUMP", string.Format("Backup product {0}... file {1}", strProductName, file.Name));

                    currentSize += size;
                    backupState.CurrentSize = currentSize;
                    FileStream fs = File.OpenRead(file.FullName);
                    byte[] buffer = new byte[fs.Length];
                    fs.Read(buffer, 0, buffer.Length);
                    string name = string.Format("{0}{1}", string.IsNullOrEmpty(strName) ? string.Empty : strName + "\\",
                        file.Name);
                    ZipEntry entry = new ZipEntry(name);
                    entry.DateTime = file.LastWriteTime;
                    stream.PutNextEntry(entry);
                    stream.Write(buffer, 0, buffer.Length);

                    #region 更新进度

                    double p = ((currentSize / (totalSize * 1.0)) / productCount + (productIndex * 1.0) / productCount) * 0.7 + 0.3;  //计算进度，这是个百分比值
                    if (stateItem != null)
                    {
                        stateItem.Progress = p * 100;
                        SetStateItemDisplay(stateItem);
                    }
                    SetTotalProgress(p * ConstDefines.PRO_BACKUPUMP + ConstDefines.PRO_BASE_BACKUPUMP);

                    #endregion

                    //App.WriteLog("BackupUMP", string.Format("Backup product {0} file {1} end.", strProductName, file.Name));
                }
                DirectoryInfo[] dirs = dirInfo.GetDirectories();
                for (int i = 0; i < dirs.Length; i++)
                {
                    //递归方法，备份下级子目录
                    backupState.Dir = dirs[i].FullName;
                    string name = string.Format("{0}{1}", string.IsNullOrEmpty(strName) ? string.Empty : strName + "\\",
                         dirs[i].Name);
                    backupState.Name = name;
                    BackupUMP(backupState);
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("BackupUMP", string.Format("Fail.\t{0}\t{1}", strProductName, ex.Message));
            }
        }

        private void BackupDirectory(string strSource, string strTarget)
        {
            if (!Directory.Exists(strTarget))
            {
                Directory.CreateDirectory(strTarget);
            }
            DirectoryInfo dirInfo = new DirectoryInfo(strSource);
            FileInfo[] files = dirInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo fileInfo = files[i];
                string source = Path.Combine(strSource, fileInfo.Name);
                string target = Path.Combine(strTarget, fileInfo.Name);
                BackupSingFile(source, target);
            }
            DirectoryInfo[] subDirs = dirInfo.GetDirectories();
            for (int i = 0; i < subDirs.Length; i++)
            {
                DirectoryInfo subInfo = subDirs[i];
                string source = Path.Combine(strSource, subInfo.Name);
                string target = Path.Combine(strTarget, subInfo.Name);
                BackupDirectory(source, target);
            }
        }

        private void BackupSingFile(string strSource, string strTarget)
        {
            try
            {
                File.Copy(strSource, strTarget, true);
            }
            catch (Exception ex)
            {
                App.WriteLog("BackupSingFile", string.Format("Backup file fail.\t{0}", ex.Message));
            }
        }

        private void StopServices()
        {
            if (mIsOptFail) { return; }

            var stateItem = mListUpdateStateItems.FirstOrDefault(s => s.State == ConstDefines.STA_STOPSERVICE);
            try
            {
                AppendMessage(string.Format("Begin stop services..."));
                App.WriteLog("StopServices", string.Format("Begin stop services..."));
                if (stateItem != null)
                {
                    stateItem.Result = 1;
                    stateItem.Progress = 0;
                    SetStateItemDisplay(stateItem);
                }
                if (ListAllServices == null) { return; }
                if (ListInstalledServices == null) { return; }
                ServiceController[] controllers = ServiceController.GetServices();
                var updateServices = UpdateInfo.ListServices;
                int count = updateServices.Count;
                for (int i = 0; i < count; i++)
                {
                    var updateService = updateServices[i];
                    var service = ListInstalledServices.FirstOrDefault(s => s.ServiceName == updateService.ServiceName);
                    if (service != null)
                    {
                        string strServiceName = service.ServiceName;
                        LogInfo(string.Format("Stop service {0}...", strServiceName));
                        App.WriteLog("StopServices", string.Format("Stop service {0}...", strServiceName));
                        ServiceController controller = controllers.FirstOrDefault(s => s.ServiceName == strServiceName);
                        if (controller == null)
                        {
                            App.WriteLog("StopServices", string.Format("Service not exist.\t{0}", strServiceName));
                        }
                        else
                        {
                            if (controller.Status == ServiceControllerStatus.Stopped)
                            {
                                App.WriteLog("StopServices",
                                    string.Format("Service {0} already stopped.", strServiceName));
                            }
                            else
                            {
                                try
                                {
                                    controller.Stop();
                                    controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 60));
                                }
                                catch (Exception ex)
                                {
                                    App.WriteLog("StopServices", string.Format("Stop service fail.\t{0}", ex.Message));
                                }
                                controller.Close();
                                controller.Dispose();
                                Thread.Sleep(3 * 1000); //等待3s，检查进程是否存在，如果存在，强制杀死进程
                                KillServiceProcess(strServiceName);
                                AppendMessage(string.Format("Stop service {0} end.", strServiceName));
                                App.WriteLog("StopServices", string.Format("Service {0} stopped.", strServiceName));
                            }
                        }
                    }

                    #region 更新进度

                    double p = (i + 1) / (count * 1.0);
                    if (stateItem != null)
                    {
                        stateItem.Progress = p * 100;
                        SetStateItemDisplay(stateItem);
                    }
                    SetTotalProgress(p * ConstDefines.PRO_STOPSERVICE + ConstDefines.PRO_BASE_STOPSERVICE);

                    #endregion

                }
                App.WriteLog("StopServices", string.Format("Stop service end."));
                SetTotalProgress(ConstDefines.PRO_BASE_STOPSERVICE + ConstDefines.PRO_STOPSERVICE);
                if (stateItem != null)
                {
                    stateItem.Result = 2;
                    stateItem.Progress = 100;
                    SetStateItemDisplay(stateItem);
                }
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrMsg = ex.Message;
                if (stateItem != null)
                {
                    stateItem.Result = 3;
                    SetStateItemDisplay(stateItem);
                }
            }
        }

        private void KillServiceProcess(string serviceName)
        {
            try
            {
                var serviceInfo = ListInstalledServices.FirstOrDefault(s => s.ServiceName == serviceName);
                if (serviceInfo == null)
                {
                    App.WriteLog("KillProcess", string.Format("ServiceInfo not exist.\t{0}", serviceName));
                    return;
                }
                string processName = serviceInfo.ProcessName;
                Process[] processes = Process.GetProcesses();
                for (int i = 0; i < processes.Length; i++)
                {
                    Process process = processes[i];
                    if (!process.ProcessName.Equals(processName)) { continue; }
                    process.Kill();
                    process.Dispose();
                    App.WriteLog("KillProcess", string.Format("Kill process end.\t{0}", processName));
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("KillProcess", string.Format("KillServiceProcess fail.\t{0}", ex.Message));
            }
        }

        private void UpdateFiles()
        {
            if (mIsOptFail) { return; }

            var stateItem = mListUpdateStateItems.FirstOrDefault(s => s.State == ConstDefines.STA_UPDATEFILE);
            try
            {
                AppendMessage(string.Format("Begin update files..."));
                App.WriteLog("UpdateFiles", string.Format("Begin update files..."));
                if (stateItem != null)
                {
                    stateItem.Result = 1;
                    stateItem.Progress = 0;
                    SetStateItemDisplay(stateItem);
                }
                //更新文件总共分为三步，1、将UMPData.zip拷贝到临时目录；2、解压文件；3、复制文件
                double per1 = 0.2;
                double per2 = 0.6;
                double per3 = 0.2;
                string strTempPath =
                           Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                               "UMP\\UMPUpdater\\Temp");
                if (!Directory.Exists(strTempPath))
                {
                    Directory.CreateDirectory(strTempPath);
                }
                string strInstallTime = DateTime.Parse(mInstallInfo.InstallTime).ToString("yyyyMMddHHmmss");
                string strUMPDataFile = Path.Combine(strTempPath, string.Format("UMPData_{0}.zip", strInstallTime));
                string strSourceDir = Path.Combine(strTempPath, string.Format("UMPData_{0}", strInstallTime));

                #region 将UMPData保存到临时文件夹中   此步骤占比 20%

                var resource = App.GetResourceStream(new Uri("Resources/UMPData.zip", UriKind.Relative));
                if (resource != null)
                {
                    var stream = resource.Stream as UnmanagedMemoryStream;
                    if (stream != null)
                    {
                        LogInfo(string.Format("Copy UMPData.zip..."));
                        App.WriteLog("UpdateFiles", string.Format("Copy UMPData.zip..."));
                        long totalLength = stream.Length;
                        long position = 0;
                        var fs = File.Create(strUMPDataFile);
                        byte[] buffer = new byte[102400];
                        int num = stream.Read(buffer, 0, 102400);
                        while (num > 0)
                        {
                            fs.Write(buffer, 0, num);
                            position += num;

                            #region 更新进度

                            double p = (position * 1.0 / totalLength) * per1;
                            if (stateItem != null)
                            {
                                stateItem.Progress = p * 100;
                                SetStateItemDisplay(stateItem);
                            }
                            SetTotalProgress(p * ConstDefines.PRO_UPDATEFILE + ConstDefines.PRO_BASE_UPDATEFILE);

                            #endregion

                            num = stream.Read(buffer, 0, 102400);
                        }
                        fs.Flush();
                        fs.Close();
                        stream.Close();
                        AppendMessage(string.Format("Copy UMPData.zip end."));
                        App.WriteLog("UpdateFiles", string.Format("Copy UMPData.zip end.\t{0}", strUMPDataFile));
                    }
                }

                #endregion

                #region 解压UMPData    此步骤占比 60%

                if (File.Exists(strUMPDataFile))
                {
                    LogInfo(string.Format("Extract UMPData..."));
                    App.WriteLog("UpdateFiles", string.Format("Extract UMPData..."));
                    string strDir = strSourceDir;
                    if (!Directory.Exists(strDir))
                    {
                        Directory.CreateDirectory(strDir);
                    }
                    var stream = new FileStream(strUMPDataFile, FileMode.Open, FileAccess.Read);
                    using (var zipStream = new ZipInputStream(stream))
                    {
                        long totalSize = stream.Length;
                        long zipSize;
                        ZipEntry theEntry;
                        while ((theEntry = zipStream.GetNextEntry()) != null)
                        {
                            string dirName = strDir;
                            string pathToZip = theEntry.Name;
                            if (!string.IsNullOrEmpty(pathToZip))
                            {
                                dirName = Path.GetDirectoryName(Path.Combine(dirName, pathToZip));
                            }
                            DateTime datetime = theEntry.DateTime;
                            if (string.IsNullOrEmpty(dirName))
                            {
                                continue;
                            }
                            string fileName = Path.GetFileName(pathToZip);
                            Directory.CreateDirectory(dirName);
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                string filePath = Path.Combine(dirName, fileName);
                                using (FileStream streamWriter = File.Create(filePath))
                                {
                                    int size;
                                    byte[] data = new byte[102400];
                                    while (true)
                                    {
                                        size = zipStream.Read(data, 0, data.Length);
                                        zipSize = zipStream.Position;
                                        if (size > 0)
                                        {
                                            streamWriter.Write(data, 0, size);

                                            #region 更新进度

                                            double p = (zipSize * 1.0 / totalSize) * per2 + per1;
                                            if (stateItem != null)
                                            {
                                                stateItem.Progress = p * 100;
                                                SetStateItemDisplay(stateItem);
                                            }
                                            SetTotalProgress(p * ConstDefines.PRO_UPDATEFILE + ConstDefines.PRO_BASE_UPDATEFILE);

                                            #endregion

                                        }
                                        else
                                            break;
                                    }
                                    streamWriter.Close();

                                    //还原文件的修改时间
                                    FileInfo fileInfo = new FileInfo(filePath);
                                    fileInfo.LastWriteTime = datetime;
                                }
                            }
                        }
                    }
                    AppendMessage(string.Format("Extract UMPData end."));
                    App.WriteLog("UpdateFiles", string.Format("Extract UMPData end"));
                }

                #endregion

                #region 复制文件   此步骤占比 20%

                if (Directory.Exists(strSourceDir))
                {
                    LogInfo(string.Format("Copy files..."));
                    App.WriteLog("UpdateFiles", string.Format("Copy files..."));
                    var updateFiles = UpdateInfo.ListFiles;
                    int count = updateFiles.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var updateFile = updateFiles[i];
                        updateFile.Result = 0;
                        updateFile.ErrCode = 0;
                        updateFile.ErrMsg = string.Empty;
                        string strName = updateFile.Name;
                        string strFileName = updateFile.FileName;
                        string strPackage = updateFile.Package;
                        string strSourcePath = updateFile.SourcePath;
                        string strTargetPath = updateFile.TargetPath;
                        var product = ListProducts.FirstOrDefault(p => p.Package == strPackage);
                        if (product != null)
                        {
                            LogInfo(string.Format("Copy files... {0}", strName));
                            App.WriteLog("UpdateFiles", string.Format("Copy files... {0}", strName));
                            string strSource = Path.Combine(strSourceDir, strSourcePath, strFileName);
                            if (!File.Exists(strSource)
                                && !Directory.Exists(strSource))
                            {
                                App.WriteLog("UpdateFiles",
                                    string.Format("Source directory or file not exist.\t{0}", strSource));
                            }
                            else
                            {
                                string strTarget = product.InstallPath;
                                if (!string.IsNullOrEmpty(strTargetPath))
                                {
                                    strTarget = Path.Combine(strTarget, strTargetPath);
                                }
                                if (!Directory.Exists(strTarget))
                                {
                                    Directory.CreateDirectory(strTarget);
                                }
                                strTarget = Path.Combine(strTarget, strFileName);

                                App.WriteLog("UpdateFiles", string.Format("Source:{0};Target:{1}", strSource, strTarget));

                                int fileType = updateFile.Type;
                                if (fileType == (int)UpdateFileType.Directory)
                                {
                                    CopyDirectory(strSource, strTarget, updateFile);
                                }
                                if (fileType == (int)UpdateFileType.File)
                                {
                                    CopySingFile(strSource, strTarget, updateFile);
                                }

                                AppendMessage(string.Format("Copy files {0} end.", strName));
                                App.WriteLog("UpdateFiles", string.Format("Copy file {0} end", strName));

                                #region 更新进度

                                double p = (i + 1) * 1.0 / count * per3 + per1 + per2;
                                if (stateItem != null)
                                {
                                    stateItem.Progress = p * 100;
                                    SetStateItemDisplay(stateItem);
                                }
                                SetTotalProgress(p * ConstDefines.PRO_UPDATEFILE + ConstDefines.PRO_BASE_UPDATEFILE);

                                #endregion

                            }
                        }
                    }
                }

                #endregion

                App.WriteLog("UpdateFiles", string.Format("Update files end."));
                SetTotalProgress(ConstDefines.PRO_BASE_UPDATEFILE + ConstDefines.PRO_UPDATEFILE);
                if (stateItem != null)
                {
                    stateItem.Result = 2;
                    stateItem.Progress = 100;
                    SetStateItemDisplay(stateItem);
                }
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrMsg = ex.Message;
                if (stateItem != null)
                {
                    stateItem.Result = 3;
                    SetStateItemDisplay(stateItem);
                }
            }
        }

        private void CopyDirectory(string strSource, string strTarget, UpdateFile updateFile)
        {
            if (!Directory.Exists(strTarget))
            {
                Directory.CreateDirectory(strTarget);
            }
            DirectoryInfo dirInfo = new DirectoryInfo(strSource);
            FileInfo[] files = dirInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo fileInfo = files[i];
                string source = Path.Combine(strSource, fileInfo.Name);
                string target = Path.Combine(strTarget, fileInfo.Name);
                CopySingFile(source, target, updateFile);
            }
            DirectoryInfo[] subDirs = dirInfo.GetDirectories();
            for (int i = 0; i < subDirs.Length; i++)
            {
                DirectoryInfo subInfo = subDirs[i];
                string source = Path.Combine(strSource, subInfo.Name);
                string target = Path.Combine(strTarget, subInfo.Name);
                CopyDirectory(source, target, updateFile);
            }
        }

        private void CopySingFile(string strSource, string strTarget, UpdateFile updateFile)
        {
            try
            {
                File.Copy(strSource, strTarget, true);      //如果文件已经存在，覆盖，暂不考虑其他情况
                //App.WriteLog("CopySingFile", string.Format("Copy file {0} end.", strTarget));
            }
            catch (Exception ex)
            {
                App.WriteLog("CopySingFile", string.Format("Copy file fail.\t{0}", ex.Message));
                if (updateFile != null)
                {
                    updateFile.Result = 1;
                    updateFile.ErrCode = Defines.RET_FAIL;
                    if (updateFile.ErrMsg.Length < 1000)
                    {
                        updateFile.ErrMsg += string.Format("{0};", ex.Message);
                    }
                }
            }
        }

        private void UpdateDatabase()
        {
            if (mIsOptFail) { return; }

            var stateItem = mListUpdateStateItems.FirstOrDefault(s => s.State == ConstDefines.STA_UPDATEDATABASE);
            try
            {
                AppendMessage(string.Format("Begin update Database..."));
                App.WriteLog("UpdateDatabase", string.Format("Begin update Database..."));
                if (stateItem != null)
                {
                    stateItem.Result = 1;
                    stateItem.Progress = 0;
                    SetStateItemDisplay(stateItem);
                }
                if (ListProducts == null) { return; }
                var product = ListProducts.FirstOrDefault(p => p.Package == UpdateConsts.PACKAGE_NAME_UMP);
                if (product != null)
                {
                    //只有UMP服务器才需要更新数据库
                    if (!InstallState.IsDatabaseCreated)
                    {
                        mIsOptFail = true;
                        mErrMsg = string.Format("Database not created!");
                        if (stateItem != null)
                        {
                            stateItem.Result = 3;
                            SetStateItemDisplay(stateItem);
                        }
                        return;
                    }
                    if (DatabaseInfo == null) { return; }
                    var updateSqlScripts = UpdateInfo.ListSqlScripts;
                    int count = updateSqlScripts.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var updateSqlScript = updateSqlScripts[i];
                        updateSqlScript.Result = 0;
                        updateSqlScript.ErrCode = 0;
                        updateSqlScript.ErrMsg = string.Empty;
                        string strName = updateSqlScript.Name;
                        LogInfo(string.Format("UpdateDatabase ... {0}", strName));
                        App.WriteLog("UpdateDatabase", string.Format("UpdateDatabase ... {0}", strName));
                        int sqlType = updateSqlScript.SqlType;
                        if (sqlType == (int)UpdateSqlType.File)
                        {
                            int dbType = InstallState.DBType;
                            string strConnString = InstallState.DBConnectionString;
                            var updateScript = updateSqlScript.ListScripts.FirstOrDefault(s => s.DBType == dbType);
                            if (updateScript == null)
                            {
                                App.WriteLog("UpdateDatabase", string.Format("UpdateScript not exist.\t{0}", dbType));
                                updateSqlScript.ErrCode = Defines.RET_NOT_EXIST;
                                updateSqlScript.ErrMsg = string.Format("UpdateScript not exist.\t{0}", dbType);
                            }
                            else
                            {
                                string strInstallTime = DateTime.Parse(mInstallInfo.InstallTime).ToString("yyyyMMddHHmmss");
                                string strFile = updateScript.Path;
                                strFile =
                                    Path.Combine(
                                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                        string.Format("UMP\\UMPUpdater\\Temp\\UMPData_{0}\\SqlScripts", strInstallTime), dbType == 3 ? "ORCL" : "MSSQL",
                                        strFile);
                                if (!File.Exists(strFile))
                                {
                                    App.WriteLog("UpdateDatabase", string.Format("Script file not exist.\t{0}", strFile));
                                    updateSqlScript.ErrCode = Defines.RET_FILE_NOT_EXIST;
                                    updateSqlScript.ErrMsg = string.Format("Script file not exist.\t{0}", strFile);
                                }
                                else
                                {
                                    int scriptType = updateSqlScript.ScriptType;
                                    List<string> listSqls = new List<string>();
                                    if (scriptType == (int)UpdateScriptType.Create
                                        || scriptType == (int)UpdateScriptType.Procedure)
                                    {
                                        //此两种脚本，需要整个文件一次执行
                                        string strContent = File.ReadAllText(strFile, Encoding.UTF8);
                                        listSqls.Add(strContent);
                                    }
                                    else
                                    {
                                        //以分号拆分出各个sql语句，然后逐个执行
                                        ParseSqlScriptFromFile(strFile, listSqls);
                                    }
                                    if (dbType == 2)
                                    {
                                        IDbConnection sqlConn = MssqlOperation.GetConnection(strConnString);
                                        sqlConn.Open();
                                        IDbCommand sqlCommand = MssqlOperation.GetCommand();
                                        sqlCommand.Connection = sqlConn;
                                        for (int j = 0; j < listSqls.Count; j++)
                                        {
                                            string strSql = listSqls[j];
                                            sqlCommand.CommandText = strSql;
                                            try
                                            {
                                                sqlCommand.ExecuteNonQuery();

                                                App.WriteLog("UpdateDatabase",
                                                    string.Format("Execute sql end.\t{0}\t{1}", strName, j));
                                            }
                                            catch (Exception ex)
                                            {
                                                App.WriteLog("UpdateDatabase",
                                                    string.Format("Execute sql  fail.\t{0}\t{1}\t{2}", strName, j,
                                                        ex.Message));
                                                updateSqlScript.Result = 1;
                                                updateSqlScript.ErrCode = Defines.RET_FAIL;
                                                updateSqlScript.ErrMsg += string.Format("{0};", ex.Message);
                                            }
                                        }
                                        if (sqlConn.State != ConnectionState.Closed)
                                        {
                                            sqlConn.Close();
                                        }
                                        sqlConn.Dispose();
                                    }
                                    if (dbType == 3)
                                    {
                                        IDbConnection orclConn = MssqlOperation.GetConnection(strConnString);
                                        orclConn.Open();
                                        IDbCommand orclCmd = MssqlOperation.GetCommand();
                                        orclCmd.Connection = orclConn;
                                        for (int j = 0; j < listSqls.Count; j++)
                                        {
                                            string strSql = listSqls[j];
                                            orclCmd.CommandText = strSql;
                                            try
                                            {
                                                orclCmd.ExecuteNonQuery();

                                                App.WriteLog("UpdateDatabase", string.Format("Execute sql end.\t{0}\t{1}", strName, j));
                                            }
                                            catch (Exception ex)
                                            {
                                                App.WriteLog("UpdateDatabase", string.Format("Execute sql  fail.\t{0}\t{1}\t{2}", strName, j, ex.Message));
                                                updateSqlScript.Result = 1;
                                                updateSqlScript.ErrCode = Defines.RET_FAIL;
                                                updateSqlScript.ErrMsg += string.Format("{0};", ex.Message);
                                            }
                                        }
                                        if (orclConn.State != ConnectionState.Closed)
                                        {
                                            orclConn.Close();
                                        }
                                        orclConn.Dispose();
                                    }

                                    AppendMessage(string.Format("UpdateDatabase {0} end.", strName));

                                    #region 更新进度

                                    double p = (i + 1) / (count * 1.0);
                                    if (stateItem != null)
                                    {
                                        stateItem.Progress = p * 100;
                                        SetStateItemDisplay(stateItem);
                                    }
                                    SetTotalProgress(p * ConstDefines.PRO_UPDATEDATABASE +
                                                     ConstDefines.PRO_BASE_UPDATEDATABASE);

                                    #endregion

                                }
                            }
                        }
                    }
                }
                App.WriteLog("UpdateDatabase", string.Format("Update Database end."));
                SetTotalProgress(ConstDefines.PRO_BASE_UPDATEDATABASE + ConstDefines.PRO_UPDATEDATABASE);
                if (stateItem != null)
                {
                    stateItem.Result = 2;
                    stateItem.Progress = 100;
                    SetStateItemDisplay(stateItem);
                }
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrMsg = ex.Message;
                if (stateItem != null)
                {
                    stateItem.Result = 3;
                    SetStateItemDisplay(stateItem);
                }
            }
        }

        private void ParseSqlScriptFromFile(string strFile, List<string> listSqls)
        {
            IEnumerable<string> listLines = File.ReadLines(strFile, Encoding.UTF8);
            string strTemp = string.Empty;
            foreach (var strContent in listLines)
            {
                if (string.IsNullOrEmpty(strContent)) { continue; }
                string strSql = strContent.Trim(' ');
                if (strSql.StartsWith("--")) { continue; }
                if (strSql.StartsWith("GO")) { continue; }
                if (!string.IsNullOrEmpty(strTemp))
                {
                    strSql = strTemp + " " + strSql;
                }
                if (strSql.EndsWith(";"))
                {
                    listSqls.Add(strSql);
                    strTemp = string.Empty;
                }
                else
                {
                    strTemp = strSql;
                }
            }
        }

        private void UpdateLanguages()
        {
            if (mIsOptFail) { return; }

            var stateItem = mListUpdateStateItems.FirstOrDefault(s => s.State == ConstDefines.STA_UPDATELANG);
            try
            {
                OperationReturn optReturn;
                int count;
                AppendMessage(string.Format("Begin update Languages..."));
                App.WriteLog("UpdateLangs", string.Format("Begin update Languages..."));
                if (stateItem != null)
                {
                    stateItem.Result = 1;
                    stateItem.Progress = 0;
                    SetStateItemDisplay(stateItem);
                }
                if (ListProducts == null) { return; }
                var product = ListProducts.FirstOrDefault(p => p.Package == UpdateConsts.PACKAGE_NAME_UMP);
                if (product != null)
                {
                    //只有UMP服务器才需要更新语言包
                    if (!InstallState.IsDatabaseCreated)
                    {
                        mIsOptFail = true;
                        mErrMsg = string.Format("Database not created!");
                        if (stateItem != null)
                        {
                            stateItem.Result = 3;
                            SetStateItemDisplay(stateItem);
                        }
                        return;
                    }
                    if (DatabaseInfo == null) { return; }
                    string strConn = InstallState.DBConnectionString;
                    int dbType = InstallState.DBType;


                    #region 如果选择了备份语言包（Mode 为 4），先备份一下语言包

                    if (InstallState.LangUpdateMode == 4)
                    {
                        if (!InstallState.IsBackupUMP
                            || string.IsNullOrEmpty(InstallState.UMPBackupPath))
                        {
                            App.WriteLog("UpdateLangs", string.Format("Fail.Backup not configed."));
                        }
                        else
                        {
                            string strBackupPath = Path.Combine(InstallState.UMPBackupPath, "Langs");
                            if (!Directory.Exists(strBackupPath))
                            {
                                Directory.CreateDirectory(strBackupPath);
                            }
                            List<int> listLangIDs = new List<int>();
                            listLangIDs.Add(1033);
                            listLangIDs.Add(1028);
                            listLangIDs.Add(1041);
                            listLangIDs.Add(2052);
                            count = listLangIDs.Count;
                            for (int i = 0; i < count; i++)
                            {
                                int langID = listLangIDs[i];
                                LogInfo(string.Format("Backup Language {0}...", langID));
                                App.WriteLog("UpdateLangs", string.Format("Backup language {0}...", langID));
                                string strSql;
                                optReturn = null;
                                if (dbType == 2)
                                {
                                    strSql = string.Format("SELECT * FROM T_00_005 WHERE C001 = {0} ORDER BY C001,C002",
                                        langID);
                                    optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                                }
                                if (dbType == 3)
                                {
                                    strSql = string.Format("SELECT * FROM T_00_005 WHERE C001 = {0} ORDER BY C001,C002",
                                        langID);
                                    optReturn = OracleOperation.GetDataSet(strConn, strSql);
                                }
                                if (optReturn == null)
                                {
                                    App.WriteLog("UpdateLangs", string.Format("Fail.\tDatabase type invalid."));
                                    continue;
                                }
                                if (!optReturn.Result)
                                {
                                    App.WriteLog("UpdateLangs",
                                        string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                    continue;
                                }
                                DataSet objDataSet = optReturn.Data as DataSet;
                                if (objDataSet == null
                                    || objDataSet.Tables.Count <= 0)
                                {
                                    App.WriteLog("UpdateLangs", string.Format("Fail.\tDataTable not exist."));
                                    continue;
                                }
                                LangLister lister = new LangLister();
                                lister.LangID = langID;
                                lister.LangName = langID.ToString();
                                for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                                {
                                    DataRow dr = objDataSet.Tables[0].Rows[j];
                                    LanguageInfo langInfo = new LanguageInfo();
                                    langInfo.LangID = Convert.ToInt32(dr["C001"]);
                                    langInfo.Name = dr["C002"].ToString();
                                    langInfo.Module = Convert.ToInt32(dr["C009"]);
                                    langInfo.SubModule = Convert.ToInt32(dr["C010"]);
                                    string str = dr["C005"].ToString() + dr["C006"] + dr["C007"] + dr["C008"];
                                    langInfo.Display = str;
                                    langInfo.Page = dr["C011"].ToString();
                                    langInfo.ObjName = dr["C012"].ToString();
                                    lister.ListLangInfos.Add(langInfo);
                                }
                                string strFile = Path.Combine(strBackupPath, string.Format("{0}.XML", langID));
                                optReturn = XMLHelper.SerializeFile(lister, strFile);
                                if (!optReturn.Result)
                                {
                                    App.WriteLog("UpdateLangs", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                    continue;
                                }
                                AppendMessage(string.Format("Backup language {0} end.", langID));
                                App.WriteLog("UpdateLangs", string.Format("Backup Language end.\t{0}\t{1}", langID, strFile));
                            }
                        }
                    }

                    #endregion


                    #region 如果选择了重置语言包（Mode 为 3 或 4），先清空语言包表

                    if (InstallState.LangUpdateMode == 4
                        || InstallState.LangUpdateMode == 5)
                    {
                        string strSql;
                        optReturn = null;
                        LogInfo(string.Format("Reset Language..."));
                        App.WriteLog("UpdateLangs", string.Format("Reset language..."));
                        if (dbType == 2)
                        {
                            strSql = string.Format("DELETE FROM T_00_005");
                            optReturn = MssqlOperation.ExecuteSql(strConn, strSql);
                        }
                        if (dbType == 3)
                        {
                            strSql = string.Format("DELETE FROM T_00_005");
                            optReturn = OracleOperation.ExecuteSql(strConn, strSql);
                        }
                        if (optReturn == null
                            || !optReturn.Result)
                        {
                            if (optReturn != null)
                            {
                                App.WriteLog("UpdateLangs", string.Format("Reset language fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            }
                            else
                            {
                                App.WriteLog("UpdateLangs", string.Format("Reset language fail.\tDatabase invalid"));
                            }
                        }
                        else
                        {
                            AppendMessage(string.Format("Reset language end."));
                            App.WriteLog("UpdateLangs", string.Format("Reset language end."));
                        }
                    }

                    #endregion


                    #region 更新语言包

                    IDbConnection objConn = null;
                    IDbDataAdapter objAdapter = null;
                    DbCommandBuilder objCmdBuilder = null;
                    var updateLangs = UpdateInfo.ListLangs;
                    count = updateLangs.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var updateLang = updateLangs[i];
                        updateLang.Result = 0;
                        updateLang.ErrCode = 0;
                        updateLang.ErrMsg = string.Empty;
                        string strName = updateLang.Name;
                        LogInfo(string.Format("Update language... {0}", strName));
                        App.WriteLog("UpdateLangs", string.Format("Update language... {0}", strName));
                        string strInstallTime = DateTime.Parse(mInstallInfo.InstallTime).ToString("yyyyMMddHHmmss");
                        string strPath = updateLang.Path;
                        string strFile =
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                                string.Format("UMP\\UMPUpdater\\Temp\\UMPData_{0}\\Langs", strInstallTime), strPath);
                        if (!File.Exists(strFile))
                        {
                            App.WriteLog("UpdateLangs", string.Format("Language file not exist.\t{0}", strFile));
                        }
                        else
                        {
                            optReturn = XMLHelper.DeserializeFile<LangLister>(strFile);
                            if (!optReturn.Result)
                            {
                                App.WriteLog("UpdateLangs",
                                    string.Format("Load language file fail.\t{0}\t{1}", optReturn.Code,
                                        optReturn.Message));
                            }
                            else
                            {
                                LangLister lister = optReturn.Data as LangLister;
                                if (lister == null)
                                {
                                    App.WriteLog("UpdateLangs", string.Format("Fail.\tLangLister is null."));
                                }
                                else
                                {
                                    int langID = updateLang.LangID;

                                    #region 获取数据集

                                    string strSql;
                                    if (dbType == 2)
                                    {
                                        strSql = string.Format("SELECT * FROM T_00_005 WHERE C001 = {0}", langID);
                                        objConn = MssqlOperation.GetConnection(strConn);
                                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                                    }
                                    if (dbType == 3)
                                    {
                                        strSql = string.Format("SELECT * FROM T_00_005 WHERE C001 = {0}", langID);
                                        objConn = OracleOperation.GetConnection(strConn);
                                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                                    }

                                    #endregion

                                    if (objConn == null || objAdapter == null || objCmdBuilder == null)
                                    {
                                        App.WriteLog("UpdateLangs", string.Format("Fail.\tDbObject is null."));
                                    }
                                    else
                                    {
                                        objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                                        objCmdBuilder.SetAllValues = false;
                                        try
                                        {
                                            DataSet objDataSet = new DataSet();
                                            objAdapter.Fill(objDataSet);

                                            #region 更新

                                            int append = 0;
                                            int modify = 0;
                                            int block = 0;
                                            int total = lister.ListLangInfos.Count;
                                            for (int j = 0; j < lister.ListLangInfos.Count; j++)
                                            {
                                                LanguageInfo langInfo = lister.ListLangInfos[j];
                                                string langName = langInfo.Name;

                                                DataRow dr =
                                                    objDataSet.Tables[0].Select(string.Format("C002 = '{0}'", langName)).FirstOrDefault();
                                                if (dr == null)
                                                {
                                                    dr = objDataSet.Tables[0].NewRow();
                                                    dr["C001"] = langID;
                                                    dr["C002"] = langName;
                                                    dr["C003"] = 0;
                                                    dr["C004"] = 0;
                                                    dr["C005"] = langInfo.Display;
                                                    dr["C009"] = langInfo.Module;
                                                    dr["C010"] = langInfo.SubModule;
                                                    dr["C011"] = langInfo.Page;
                                                    dr["C012"] = langInfo.ObjName;

                                                    objDataSet.Tables[0].Rows.Add(dr);
                                                    append++;
                                                }
                                                else
                                                {
                                                    if (InstallState.LangUpdateMode == 2)
                                                    {
                                                        string str = dr["C005"].ToString();
                                                        if (!str.Equals(langInfo.Display))
                                                        {
                                                            //如果选择了修改语言包，则修改已经存在的语言包
                                                            dr["C003"] = 0;
                                                            dr["C004"] = 0;
                                                            dr["C005"] = langInfo.Display;
                                                            dr["C009"] = langInfo.Module;
                                                            dr["C010"] = langInfo.SubModule;
                                                            dr["C011"] = langInfo.Page;
                                                            dr["C012"] = langInfo.ObjName;
                                                            modify++;
                                                        }
                                                    }
                                                }
                                                block++;
                                                if (block >= 500)
                                                {
                                                    //每500条记录提交一次
                                                    block = 0;
                                                    objAdapter.Update(objDataSet);
                                                    objDataSet.AcceptChanges();

                                                    #region 更新进度

                                                    var pp = ((j + 1) / (total * 1.0)) / count + (i * 1.0) / count;
                                                    if (stateItem != null)
                                                    {
                                                        stateItem.Progress = pp * 100;
                                                        SetStateItemDisplay(stateItem);
                                                    }
                                                    SetTotalProgress(pp * ConstDefines.PRO_UPDATELANG +
                                                                     ConstDefines.PRO_BASE_UPDATELANG);

                                                    #endregion

                                                }
                                            }

                                            objAdapter.Update(objDataSet);
                                            objDataSet.AcceptChanges();

                                            #endregion

                                            AppendMessage(string.Format("Update Lang {0} end.", langID));
                                            App.WriteLog("UpdateLangs", string.Format("End.\t{0}\tAppend:{1};Modify:{2}", langID, append, modify));

                                            #region 更新进度

                                            double p = (i + 1) / (count * 1.0);
                                            if (stateItem != null)
                                            {
                                                stateItem.Progress = p * 100;
                                                SetStateItemDisplay(stateItem);
                                            }
                                            SetTotalProgress(p * ConstDefines.PRO_UPDATELANG +
                                                             ConstDefines.PRO_BASE_UPDATELANG);

                                            #endregion

                                        }
                                        catch (Exception ex)
                                        {
                                            App.WriteLog("UpdateLangs", string.Format("Fail.\t{0}", ex.Message));

                                            updateLang.Result = 1;
                                            updateLang.ErrCode = Defines.RET_FAIL;
                                            updateLang.ErrMsg += string.Format("{0};", ex.Message);
                                        }
                                        finally
                                        {
                                            if (objConn.State == ConnectionState.Open)
                                            {
                                                objConn.Close();
                                            }
                                            objConn.Dispose();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                }
                App.WriteLog("UpdateLangs", string.Format("Update languages end."));
                SetTotalProgress(ConstDefines.PRO_BASE_UPDATELANG + ConstDefines.PRO_UPDATELANG);
                if (stateItem != null)
                {
                    stateItem.Result = 2;
                    stateItem.Progress = 100;
                    SetStateItemDisplay(stateItem);
                }
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrMsg = ex.Message;
                if (stateItem != null)
                {
                    stateItem.Result = 3;
                    SetStateItemDisplay(stateItem);
                }
            }
        }

        private void UpdateServices()
        {
            if (mIsOptFail) { return; }

            var stateItem = mListUpdateStateItems.FirstOrDefault(s => s.State == ConstDefines.STA_UPDATESERVICE);
            try
            {
                AppendMessage(string.Format("Begin update services..."));
                App.WriteLog("UpdateServices", string.Format("Begin update services..."));
                if (stateItem != null)
                {
                    stateItem.Result = 1;
                    stateItem.Progress = 0;
                    SetStateItemDisplay(stateItem);
                }
                if (ListProducts == null) { return; }
                ServiceController[] controllers = ServiceController.GetServices();
                var updateServices = UpdateInfo.ListServices;
                int count = updateServices.Count;
                for (int i = 0; i < count; i++)
                {
                    var updateService = updateServices[i];
                    updateService.Result = 0;
                    updateService.ErrCode = 0;
                    updateService.ErrMsg = string.Empty;
                    string strPackage = updateService.Package;
                    var product = ListProducts.FirstOrDefault(p => p.Package == strPackage);
                    if (product != null)
                    {
                        string strName = updateService.Name;
                        LogInfo(string.Format("Update service {0}...", strName));
                        App.WriteLog("UpdateServices", string.Format("Update service {0}...", strName));
                        string strServiceName = updateService.ServiceName;
                        string strFilePath = updateService.TargetPath;
                        strFilePath = Path.Combine(product.InstallPath, strFilePath);
                        string strFileDir = strFilePath.Substring(0, strFilePath.LastIndexOf("\\"));
                        string strFileName = strFilePath.Substring(strFilePath.LastIndexOf("\\") + 1);
                        App.WriteLog("UpdateServices",
                            string.Format("Service Info ServiceName:{0};FileName:{1};FilePath:{2}", strServiceName,
                                strFileName, strFileDir));
                        string log = Path.Combine(strFileDir, string.Format("{0}.InstallLog", strFileName));
                        int installMode = updateService.InstallMode;
                        var controller =
                            controllers.FirstOrDefault(s => s.ServiceName == strServiceName);
                        if (installMode == (int)ServiceInstallMode.Install)
                        {
                            //如果服务已经存在，先卸载，再安装
                            if (controller != null)
                            {

                                #region 停止服务并卸载

                                if (controller.Status == ServiceControllerStatus.Running)
                                {
                                    //如果正在运行，先停止
                                    try
                                    {
                                        controller.Stop();
                                        controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 60));
                                    }
                                    catch (Exception ex)
                                    {
                                        App.WriteLog("UpdateServices", string.Format("Stop service fail.\t{0}\t{1}", strName, ex.Message));
                                    }
                                    controller.Close();
                                    controller.Dispose();
                                    Thread.Sleep(3 * 1000); //等待3s，检查进程是否存在，如果存在，强制杀死进程
                                    KillServiceProcess(strServiceName);
                                }
                                try
                                {
                                    string[] options = { string.Format("/LogFile={0}", log) };
                                    TransactedInstaller transacted = new TransactedInstaller();
                                    AssemblyInstaller installer = new AssemblyInstaller(strFilePath, options);
                                    transacted.Installers.Add(installer);
                                    transacted.Uninstall(null);
                                    transacted.Dispose();
                                }
                                catch (Exception ex)
                                {
                                    App.WriteLog("UpdateServices", string.Format("Uninstall service fail.\t{0}\t{1}", strName, ex.Message));
                                }

                                #endregion

                            }

                            #region 安装服务

                            try
                            {
                                string[] options = { };
                                TransactedInstaller transacted = new TransactedInstaller();
                                AssemblyInstaller installer = new AssemblyInstaller(strFilePath, options);
                                transacted.Installers.Add(installer);
                                transacted.Install(new Hashtable());
                                transacted.Dispose();

                                AppendMessage(string.Format("Install service {0} end.", strName));
                                App.WriteLog("UpdateServices", string.Format("Install service end.\t{0}", strName));
                            }
                            catch (Exception ex)
                            {
                                App.WriteLog("UpdateServices", string.Format("Install service fail.\t{0}\t{1}", strName, ex.Message));
                                updateService.Result = 1;
                                updateService.ErrCode = Defines.RET_FAIL;
                                updateService.ErrMsg = string.Format("{0};", ex.Message);
                            }

                            #endregion

                        }
                        if (installMode == (int)ServiceInstallMode.InstallSkip)
                        {
                            //如果服务已经存在，跳过
                            if (controller == null)
                            {

                                #region 安装服务

                                try
                                {
                                    string[] options = { };
                                    TransactedInstaller transacted = new TransactedInstaller();
                                    AssemblyInstaller installer = new AssemblyInstaller(strFilePath, options);
                                    transacted.Installers.Add(installer);
                                    transacted.Install(new Hashtable());
                                    transacted.Dispose();

                                    AppendMessage(string.Format("Install service {0} end.", strName));
                                    App.WriteLog("UpdateServices", string.Format("Install service end.\t{0}", strName));
                                }
                                catch (Exception ex)
                                {
                                    App.WriteLog("UpdateServices", string.Format("Install service fail.\t{0}\t{1}", strName, ex.Message));
                                    updateService.Result = 1;
                                    updateService.ErrCode = Defines.RET_FAIL;
                                    updateService.ErrMsg = string.Format("{0};", ex.Message);
                                }

                                #endregion

                            }
                        }

                        #region 更新进度

                        double p = (i + 1) / (count * 1.0);
                        if (stateItem != null)
                        {
                            stateItem.Progress = p * 100;
                            SetStateItemDisplay(stateItem);
                        }
                        SetTotalProgress(p * ConstDefines.PRO_UPDATESERVICE +
                                         ConstDefines.PRO_BASE_UPDATESERVICE);

                        #endregion

                    }
                }

                App.WriteLog("UpdateServices", string.Format("Update services end."));
                SetTotalProgress(ConstDefines.PRO_BASE_UPDATESERVICE + ConstDefines.PRO_UPDATESERVICE);
                if (stateItem != null)
                {
                    stateItem.Result = 2;
                    stateItem.Progress = 100;
                    SetStateItemDisplay(stateItem);
                }
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrMsg = ex.Message;
                if (stateItem != null)
                {
                    stateItem.Result = 3;
                    SetStateItemDisplay(stateItem);
                }
            }
        }

        private void StartServices()
        {
            if (mIsOptFail) { return; }

            var stateItem = mListUpdateStateItems.FirstOrDefault(s => s.State == ConstDefines.STA_STARTSERVICE);
            try
            {
                AppendMessage(string.Format("Begin start services..."));
                App.WriteLog("StartServices", string.Format("Begin start services..."));
                if (stateItem != null)
                {
                    stateItem.Result = 1;
                    stateItem.Progress = 0;
                    SetStateItemDisplay(stateItem);
                }
                if (ListProducts == null) { return; }
                ServiceController[] controllers = ServiceController.GetServices();
                var updateServices = UpdateInfo.ListServices;
                int count = updateServices.Count;
                for (int i = count - 1; i >= 0; i--)
                {
                    var updateService = updateServices[i];
                    string strName = updateService.Name;
                    LogInfo(string.Format("Start service {0}...", strName));
                    App.WriteLog("StartServices", string.Format("Start service {0}...", strName));
                    string strServiceName = updateService.ServiceName;
                    var controller =
                        controllers.FirstOrDefault(s => s.ServiceName == strServiceName);
                    if (controller != null)
                    {
                        try
                        {
                            controller.Start();
                            controller.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 60));

                            AppendMessage(string.Format("Start service {0} end.", strName));
                            App.WriteLog("StartServices", string.Format("Start service {0} end.", strName));
                        }
                        catch (Exception ex)
                        {
                            App.WriteLog("StartServices", string.Format("Start service fail.\t{0}\t{1}", strName, ex.Message));
                        }
                        controller.Close();
                        controller.Dispose();

                        #region 更新进度

                        var j = count - i;
                        double p = j / (count * 1.0);
                        if (stateItem != null)
                        {
                            stateItem.Progress = p * 100;
                            SetStateItemDisplay(stateItem);
                        }
                        SetTotalProgress(p * ConstDefines.PRO_UPDATESERVICE +
                                         ConstDefines.PRO_BASE_UPDATESERVICE);

                        #endregion
                    }
                }

                App.WriteLog("StartServices", string.Format("Start services end."));
                SetTotalProgress(ConstDefines.PRO_BASE_STARTSERVICE + ConstDefines.PRO_STARTSERVICE);
                if (stateItem != null)
                {
                    stateItem.Result = 2;
                    stateItem.Progress = 100;
                    SetStateItemDisplay(stateItem);
                }
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrMsg = ex.Message;
                if (stateItem != null)
                {
                    stateItem.Result = 3;
                    SetStateItemDisplay(stateItem);
                }
            }
        }

        private void UpdateServers()
        {
            if (mIsOptFail) { return; }

            var stateItem = mListUpdateStateItems.FirstOrDefault(s => s.State == ConstDefines.STA_UPDATESERVER);
            try
            {
                AppendMessage(string.Format("Begin update servers..."));
                App.WriteLog("UpdateServers", string.Format("Begin update servers..."));
                if (stateItem != null)
                {
                    stateItem.Result = 1;
                    stateItem.Progress = 0;
                    SetStateItemDisplay(stateItem);
                }
                if (InstallState.IsUMPInstalled
                    && InstallState.IsDatabaseCreated)
                {
                    //只有UMP服务器升级才会执行以下操作
                    InstallProduct umpProduct =
                        ListProducts.FirstOrDefault(p => p.Package == UpdateConsts.PACKAGE_NAME_UMP);
                    mAppServerInfo = null;
                    LoadAppServerInfo();
                    if (mAppServerInfo != null)
                    {
                        LoadLoggingServers();
                        int count = mListLoggingServerInfos.Count;
                        if (count > 0)
                        {
                            //存在其他LoggingServer，需要对其他LoggingServer进行联动升级
                            LoggingUpdateHelper helper = new LoggingUpdateHelper();
                            helper.Debug += App.WriteLog;
                            helper.ProgressEvent += LoggingUpdateHelper_Progress;
                            helper.AppendMessageEvent += LoggingUpdateHelper_AppendMessage;
                            helper.ListLoggingServers = mListLoggingServerInfos;
                            helper.InstallInfo = mInstallInfo;
                            helper.AppServerInfo = mAppServerInfo;
                            helper.UMPProductInfo = umpProduct;
                            helper.DoUpdate();
                        }
                    }
                }

                App.WriteLog("UpdateServers", string.Format("Update servers end."));
                SetTotalProgress(ConstDefines.PRO_BASE_UPDATESERVER + ConstDefines.PRO_UPDATESERVER);
                if (stateItem != null)
                {
                    stateItem.Result = 2;
                    stateItem.Progress = 100;
                    SetStateItemDisplay(stateItem);
                }
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrMsg = ex.Message;
                if (stateItem != null)
                {
                    stateItem.Result = 3;
                    SetStateItemDisplay(stateItem);
                }
            }
        }

        private void LoadAppServerInfo()
        {
            try
            {
                if (ListProducts == null) { return; }
                var product = ListProducts.FirstOrDefault(p => p.Package == UpdateConsts.PACKAGE_NAME_UMP);
                if (product == null) { return; }
                string strPath = product.InstallPath;
                strPath = Path.Combine(strPath, "GlobalSettings");
                string strFile = Path.Combine(strPath, "UMP.Server.01.XML");
                if (!File.Exists(strFile))
                {
                    App.WriteLog("LoadAppServerInfo", string.Format("UMP.Server.01.XML not exist.\t{0}", strFile));
                    return;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(strFile);
                var iisNode = doc.SelectSingleNode("UMPSetted/IISBindingProtocol");
                if (iisNode == null)
                {
                    App.WriteLog("LoadAppServerInfo", string.Format("IISBindingProtocol node not exist."));
                    return;
                }
                int intValue;
                AppServerInfo appServerInfo = null;
                for (int i = 0; i < iisNode.ChildNodes.Count; i++)
                {
                    var ele = iisNode.ChildNodes[i] as XmlElement;
                    if (ele == null) { continue; }
                    var protocol = ele.GetAttribute("Protocol");
                    if (!protocol.ToLower().Equals("http")) { continue; }
                    var strAddress = ele.GetAttribute("IPAddress");
                    var strPort = ele.GetAttribute("BindInfo");
                    if (string.IsNullOrEmpty(strAddress)) { continue; }
                    if (!int.TryParse(strPort, out intValue)) { continue; }
                    appServerInfo = new AppServerInfo();
                    appServerInfo.Protocol = "http";
                    appServerInfo.Address = strAddress;
                    appServerInfo.Port = intValue;
                    appServerInfo.SupportHttps = false;
                    break;
                }
                if (appServerInfo != null)
                {
                    mAppServerInfo = appServerInfo;
                    App.WriteLog("LoadAppServerInfo", string.Format("End.\t{0}://{1}:{2}", mAppServerInfo.Protocol, mAppServerInfo.Address, mAppServerInfo.Port));
                }
            }
            catch (Exception ex)
            {
                App.WriteLog("LoadAppServerInfo", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void LoadLoggingServers()
        {
            try
            {
                if (InstallState == null) { return; }
                if (!InstallState.IsUMPInstalled) { return; }
                if (!InstallState.IsDatabaseCreated) { return; }
                if (DatabaseInfo == null) { return; }

                mListLoggingServerInfos.Clear();
                DatabaseInfo dbInfo = DatabaseInfo;
                int dbType = dbInfo.TypeID;
                string strConn = dbInfo.GetConnectionString();
                OperationReturn optReturn = null;
                string strSql;
                if (dbType == 2)
                {
                    strSql =
                        string.Format(
                            "SELECT * FROM T_11_101_00000 WHERE C001> 2100000000000000000 AND C001 < 2110000000000000000 AND C002 = 1");
                    optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                }
                if (dbType == 3)
                {
                    strSql =
                       string.Format(
                           "SELECT * FROM T_11_101_00000 WHERE C001> 2100000000000000000 AND C001 < 2110000000000000000 AND C002 = 1");
                    optReturn = OracleOperation.GetDataSet(strConn, strSql);
                }
                if (optReturn == null)
                {
                    App.WriteLog("LoadLoggingServers", string.Format("Database type invalid."));
                    return;
                }
                if (!optReturn.Result)
                {
                    App.WriteLog("LoadLoggingServers",
                        string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                DataSet objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null || objDataSet.Tables.Count <= 0)
                {
                    App.WriteLog("LoadLoggingServers", string.Format("DataSet is null table not exist."));
                    return;
                }
                string strValue;
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    LoggingServerInfo info = new LoggingServerInfo();
                    info.ObjID = Convert.ToInt64(dr["C001"]);
                    strValue = dr["C017"].ToString();
                    info.HostAddress = DecodeEncryptValue(strValue);
                    //UMP 服务器排除掉
                    if (mAppServerInfo != null)
                    {
                        if (mAppServerInfo.Address.Equals(info.HostAddress)) { continue; }
                    }
                    mListLoggingServerInfos.Add(info);
                }

                App.WriteLog("LoadLoggingServers", string.Format("End.\t{0}", mListLoggingServerInfos.Count));
            }
            catch (Exception ex)
            {
                App.WriteLog("LoadLoggingServers", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void UpdateFollows()
        {
            if (mIsOptFail) { return; }

            try
            {
                AppendMessage(string.Format("Begin update follows..."));
                App.WriteLog("UpdateFollows", string.Format("Begin update follows..."));
                for (int i = 0; i < UpdateInfo.ListFollows.Count; i++)
                {
                    var updateFollow = UpdateInfo.ListFollows[i];
                    string strName = updateFollow.Name;
                    LogInfo(string.Format("Update follow {0}...", strName));
                    App.WriteLog("UpdateFollows", string.Format("Update follow {0}...", strName));
                    int key = updateFollow.Key;
                    switch (key)
                    {
                        case UpdateConsts.FOLLOW_KEY_BINDSITE:
                            LanchMAMT();
                            break;
                    }
                    AppendMessage(string.Format("Update follow {0} end.", strName));
                }

                App.WriteLog("UpdateFollows", string.Format("Update follows end."));
            }
            catch (Exception ex)
            {
                mIsOptFail = true;
                mErrMsg = ex.Message;
            }
        }

        private void DeleteTempFiles()
        {
            try
            {
                string strTempPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "UMP\\UMPUpdater\\Temp");
                string strInstallTime = DateTime.Parse(mInstallInfo.InstallTime).ToString("yyyyMMddHHmmss");
                string strUMPDataFile = Path.Combine(strTempPath, string.Format("UMPData_{0}.zip", strInstallTime));
                string strSourceDir = Path.Combine(strTempPath, string.Format("UMPData_{0}", strInstallTime));
                if (Directory.Exists(strSourceDir))
                {
                    Directory.Delete(strSourceDir, true);
                }
                if (!InstallState.IsSaveUpdateData)
                {
                    if (File.Exists(strUMPDataFile))
                    {
                        File.Delete(strUMPDataFile);
                    }
                }

                App.WriteLog("DeleteTempFiles", string.Format("Delete temp directory and file end."));
            }
            catch (Exception ex)
            {
                App.WriteLog("DeleteTempFiles", string.Format("Delete temp directory and file fail.\t{0}", ex.Message));
            }
        }

        private void LanchMAMT()
        {
            try
            {
                if (!InstallState.IsUMPInstalled) { return; }
                string strPath = Path.Combine(InstallState.UMPInstallPath, "ManagementMaintenance");
                if (!Directory.Exists(strPath))
                {
                    return;
                }
                string strExe = Path.Combine(strPath, "UMP.MAMT.exe");
                if (!File.Exists(strExe))
                {
                    App.WriteLog("LanchMAMT", string.Format("File not exist.\t{0}", strExe));
                    return;
                }
                Process process = new Process();
                process.StartInfo.FileName = strExe;
                process.StartInfo.WorkingDirectory = strPath;
                process.Start();
                process.WaitForExit();
                process.Dispose();
                App.WriteLog("LanchMAMT", string.Format("MAMT launch end."));
            }
            catch (Exception ex)
            {
                App.WriteLog("LanchMAMT", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void SaveInstallInfo()
        {
            try
            {
                if (mInstallInfo == null) { return; }
                mInstallInfo.ListProducts.Clear();
                var products = ListProducts;
                if (products != null)
                {
                    for (int i = 0; i < products.Count; i++)
                    {
                        var product = products[i];
                        product.Version = mInstallInfo.Version;
                    }
                    for (int i = 0; i < products.Count; i++)
                    {
                        mInstallInfo.ListProducts.Add(products[i]);
                    }
                }
                string strPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "VoiceCyber\\UMP");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                string strFile = Path.Combine(strPath, InstallInfo.FILE_NAME);
                OperationReturn optReturn = XMLHelper.SerializeFile(mInstallInfo, strFile);
                if (!optReturn.Result)
                {
                    App.WriteLog("SaveInstallInfo", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                App.WriteLog("SaveInstallInfo", string.Format("End.\t{0}", strFile));
            }
            catch (Exception ex)
            {
                App.WriteLog("SaveInstallInfo", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void SaveInstallInfoDB()
        {
            try
            {
                if (mInstallInfo == null) { return; }
                if (!InstallState.IsDatabaseCreated) { return; }
                if (!InstallState.IsUMPInstalled) { return; }
                if (DatabaseInfo == null) { return; }

                string strSessionID = mInstallInfo.SessionID;
                int dbType = DatabaseInfo.TypeID;
                string strConn = DatabaseInfo.GetConnectionString();
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DbCommandBuilder objCmdBuilder = null;
                string strSql;
                if (dbType == 2)
                {
                    strSql = string.Format("SELECT * FROM T_00_301 WHERE C001 = '{0}' ORDER BY C002", strSessionID);
                    objConn = MssqlOperation.GetConnection(strConn);
                    objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                    objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                }
                if (dbType == 3)
                {
                    strSql = string.Format("SELECT * FROM T_00_301 WHERE C001 = '{0}' ORDER BY C002", strSessionID);
                    objConn = OracleOperation.GetConnection(strConn);
                    objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                    objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    App.WriteLog("SaveInstallInfoDB", string.Format("Fail.\tDbObject is null."));
                    return;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    bool isAdd = false;
                    for (int i = 0; i < mInstallInfo.ListProducts.Count; i++)
                    {
                        var product = mInstallInfo.ListProducts[i];
                        string strProductName = product.ProductName;
                        string strInstallPath = product.InstallPath;
                        string strVersion = FormatVersion(product.Version);
                        DataRow dr =
                            objDataSet.Tables[0].Select(string.Format("C002 = '{0}'", strProductName)).FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strSessionID;
                            dr["C002"] = strProductName;
                            isAdd = true;
                        }
                        dr["C003"] = DateTime.Parse(mInstallInfo.BeginTime).ToString("yyyyMMddHHmmss");
                        dr["C004"] = DateTime.Parse(mInstallInfo.EndTime).ToString("yyyyMMddHHmmss");
                        dr["C005"] = mInstallInfo.Type;
                        dr["C006"] = strVersion;
                        dr["C009"] = strInstallPath;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    App.WriteLog("SaveInstallInfoDB", string.Format("Fail.\t{0}", ex.Message));
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

                App.WriteLog("SaveInstallInfoDB", string.Format("End."));
            }
            catch (Exception ex)
            {
                App.WriteLog("SaveInstallInfoDB", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void SaveUpdateModuleDB()
        {
            try
            {
                if (mInstallInfo == null) { return; }
                if (!InstallState.IsDatabaseCreated) { return; }
                if (!InstallState.IsUMPInstalled) { return; }
                if (DatabaseInfo == null) { return; }
                UpdateInfo updateInfo = mInstallInfo.UpdateInfo;
                if (updateInfo == null) { return; }

                string strVersion = updateInfo.Version;
                string strFomatVersion = FormatVersion(strVersion);
                int dbType = DatabaseInfo.TypeID;
                string strConn = DatabaseInfo.GetConnectionString();
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DbCommandBuilder objCmdBuilder = null;
                string strSql;
                if (dbType == 2)
                {
                    strSql = string.Format("SELECT * FROM T_00_302 WHERE C001 LIKE '{0}%' ORDER BY C001", strFomatVersion);
                    objConn = MssqlOperation.GetConnection(strConn);
                    objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                    objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                }
                if (dbType == 3)
                {
                    strSql = string.Format("SELECT * FROM T_00_302 WHERE C001 LIKE '{0}%' ORDER BY C001", strFomatVersion);
                    objConn = OracleOperation.GetConnection(strConn);
                    objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                    objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    App.WriteLog("SaveUpdateModuleDB", string.Format("Fail.\tDbObject is null."));
                    return;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    bool isAdd;
                    for (int i = 0; i < updateInfo.ListModules.Count; i++)
                    {
                        var updateModule = updateInfo.ListModules[i];
                        string strSerialNo = updateModule.SerialNo;

                        isAdd = false;
                        DataRow dr =
                            objDataSet.Tables[0].Select(string.Format("C001 = '{0}'", strSerialNo)).FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strSerialNo;
                            isAdd = true;
                        }
                        dr["C002"] = updateModule.Type;
                        dr["C003"] = updateModule.ModuleID;
                        dr["C004"] = updateModule.ModuleName;
                        dr["C006"] = updateModule.Level;
                        dr["C007"] = updateModule.Content;
                        dr["C008"] = updateModule.LangID;
                        dr["C009"] = updateModule.ModuleLangID;
                        dr["C010"] = strVersion;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    App.WriteLog("SaveUpdateModuleDB", string.Format("Fail.\t{0}", ex.Message));
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

                App.WriteLog("SaveUpdateModuleDB", string.Format("End."));
            }
            catch (Exception ex)
            {
                App.WriteLog("SaveUpdateModuleDB", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void SaveUpdateDetailDB()
        {
            try
            {
                if (mInstallInfo == null) { return; }
                if (!InstallState.IsDatabaseCreated) { return; }
                if (!InstallState.IsUMPInstalled) { return; }
                if (DatabaseInfo == null) { return; }
                UpdateInfo updateInfo = mInstallInfo.UpdateInfo;
                if (updateInfo == null) { return; }

                string strVersion = updateInfo.Version;
                string strFomatVersion = FormatVersion(strVersion);
                int dbType = DatabaseInfo.TypeID;
                string strConn = DatabaseInfo.GetConnectionString();
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DbCommandBuilder objCmdBuilder = null;
                string strSql;
                if (dbType == 2)
                {
                    strSql = string.Format("SELECT * FROM T_00_303 WHERE C001 = '{0}' ORDER BY C001", strFomatVersion);
                    objConn = MssqlOperation.GetConnection(strConn);
                    objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                    objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                }
                if (dbType == 3)
                {
                    strSql = string.Format("SELECT * FROM T_00_303 WHERE C001 = '{0}' ORDER BY C001", strFomatVersion);
                    objConn = OracleOperation.GetConnection(strConn);
                    objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                    objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    App.WriteLog("SaveUpdateFileDB", string.Format("Fail.\tDbObject is null."));
                    return;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    bool isAdd;

                    #region UpdateFile

                    int type = 1;
                    for (int i = 0; i < updateInfo.ListFiles.Count; i++)
                    {
                        var updateFile = updateInfo.ListFiles[i];
                        string strName = updateFile.Name;

                        isAdd = false;
                        DataRow dr =
                            objDataSet.Tables[0].Select(string.Format("C002 = {0} AND C003 = '{1}'", type, strName)).FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strFomatVersion;
                            dr["C002"] = type;
                            dr["C003"] = strName;
                            isAdd = true;
                        }
                        dr["C004"] = updateFile.Description;
                        dr["C005"] = updateFile.ErrorReply;
                        dr["C006"] = updateFile.Result;
                        dr["C007"] = updateFile.ErrCode;
                        string strMsg = updateFile.ErrMsg;
                        if (strMsg.Length > 1000)
                        {
                            dr["C008"] = strMsg.Substring(0, 1000) + "...";
                        }
                        else
                        {
                            dr["C008"] = strMsg;
                        }
                        dr["C011"] = updateFile.Package;
                        dr["C012"] = updateFile.InstallMode;
                        dr["C013"] = updateFile.DependFile;
                        dr["C014"] = updateFile.Type;
                        dr["C015"] = updateFile.FileName;
                        dr["C016"] = updateFile.SourcePath;
                        dr["C017"] = updateFile.TargetPath;
                        dr["C018"] = updateFile.TargetPathType;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();

                    App.WriteLog("SaveUpdateDetailDB", string.Format("Save update file end."));

                    #endregion


                    #region UpdateSqlScript

                    type = 2;
                    for (int i = 0; i < updateInfo.ListSqlScripts.Count; i++)
                    {
                        var updateSqlScript = updateInfo.ListSqlScripts[i];
                        string strName = string.Format("{0}_{1}", dbType == 2 ? "MSSQL" : "ORCL", updateSqlScript.Name);

                        isAdd = false;
                        DataRow dr =
                            objDataSet.Tables[0].Select(string.Format("C002 = {0} AND C003 = '{1}'", type, strName)).FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strFomatVersion;
                            dr["C002"] = type;
                            dr["C003"] = strName;
                            isAdd = true;
                        }
                        dr["C004"] = updateSqlScript.Description;
                        dr["C005"] = updateSqlScript.ErrorReply;
                        dr["C006"] = updateSqlScript.Result;
                        dr["C007"] = updateSqlScript.ErrCode;
                        string strMsg = updateSqlScript.ErrMsg;
                        if (strMsg.Length > 1000)
                        {
                            dr["C008"] = strMsg.Substring(0, 1000) + "...";
                        }
                        else
                        {
                            dr["C008"] = strMsg;
                        }
                        dr["C011"] = updateSqlScript.SqlType;
                        dr["C012"] = updateSqlScript.ScriptType;

                        var script = updateSqlScript.ListScripts.FirstOrDefault(s => s.DBType == dbType);
                        if (script != null)
                        {
                            dr["C013"] = script.DBType;
                            dr["C014"] = script.Text;
                            dr["C015"] = script.Path;
                        }

                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();

                    App.WriteLog("SaveUpdateDetailDB", string.Format("Save update sqlscript end."));

                    #endregion


                    #region UpdateService

                    type = 3;
                    for (int i = 0; i < updateInfo.ListServices.Count; i++)
                    {
                        var updateService = updateInfo.ListServices[i];
                        string strName = updateService.Name;

                        isAdd = false;
                        DataRow dr =
                            objDataSet.Tables[0].Select(string.Format("C002 = {0} AND C003 = '{1}'", type, strName)).FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strFomatVersion;
                            dr["C002"] = type;
                            dr["C003"] = strName;
                            isAdd = true;
                        }
                        dr["C004"] = updateService.Description;
                        dr["C005"] = updateService.ErrorReply;
                        dr["C006"] = updateService.Result;
                        dr["C007"] = updateService.ErrCode;
                        string strMsg = updateService.ErrMsg;
                        if (strMsg.Length > 1000)
                        {
                            dr["C008"] = strMsg.Substring(0, 1000) + "...";
                        }
                        else
                        {
                            dr["C008"] = strMsg;
                        }
                        dr["C011"] = updateService.Package;
                        dr["C012"] = updateService.InstallMode;
                        dr["C013"] = updateService.ServiceName;
                        dr["C014"] = updateService.StartMode;
                        dr["C015"] = updateService.DelayTime;
                        dr["C016"] = updateService.TargetPath;
                        dr["C017"] = updateService.TargetPathType;
                        dr["C018"] = updateService.InstallCommand;
                        dr["C019"] = updateService.UnInstallCommand;
                        dr["C020"] = updateService.ServiceType;

                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();

                    App.WriteLog("SaveUpdateDetailDB", string.Format("Save update service end."));

                    #endregion


                    #region UpdateLang

                    type = 4;
                    for (int i = 0; i < updateInfo.ListLangs.Count; i++)
                    {
                        var updateLang = updateInfo.ListLangs[i];
                        string strName = updateLang.Name;

                        isAdd = false;
                        DataRow dr =
                            objDataSet.Tables[0].Select(string.Format("C002 = {0} AND C003 = '{1}'", type, strName)).FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strFomatVersion;
                            dr["C002"] = type;
                            dr["C003"] = strName;
                            isAdd = true;
                        }
                        dr["C004"] = updateLang.Description;
                        dr["C005"] = updateLang.ErrorReply;
                        dr["C006"] = updateLang.Result;
                        dr["C007"] = updateLang.ErrCode;
                        string strMsg = updateLang.ErrMsg;
                        if (strMsg.Length > 1000)
                        {
                            dr["C008"] = strMsg.Substring(0, 1000) + "...";
                        }
                        else
                        {
                            dr["C008"] = strMsg;
                        }
                        dr["C011"] = updateLang.LangID;
                        dr["C012"] = updateLang.LangName;
                        dr["C013"] = updateLang.InstallMode;
                        dr["C014"] = updateLang.Path;

                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();

                    App.WriteLog("SaveUpdateDetailDB", string.Format("Save update lang end."));

                    #endregion

                }
                catch (Exception ex)
                {
                    App.WriteLog("SaveUpdateDetailDB", string.Format("Fail.\t{0}", ex.Message));
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

                App.WriteLog("SaveUpdateDetailDB", string.Format("End."));
            }
            catch (Exception ex)
            {
                App.WriteLog("SaveUpdateDetailDB", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Others

        private void CreateUpdateItemColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc = new GridViewColumn();
                GridViewColumnHeader gvch = new GridViewColumnHeader();
                gvch.Content = App.GetLanguageInfo("COL001", "Item");
                gvc.Header = gvch;
                gvc.Width = 280;
                gvc.DisplayMemberBinding = new Binding("Display");
                gv.Columns.Add(gvc);
                gvc = new GridViewColumn();
                gvch = new GridViewColumnHeader();
                gvch.Content = App.GetLanguageInfo("COL002", "Progress");
                gvc.Header = gvch;
                gvc.Width = 100;
                gvc.DisplayMemberBinding = new Binding("StrProgress");
                gv.Columns.Add(gvc);
                gvc = new GridViewColumn();
                gvch = new GridViewColumnHeader();
                gvch.Content = App.GetLanguageInfo("COL003", "Status");
                gvc.Header = gvch;
                gvc.Width = 100;
                var dt = Resources["CellResultTemplate"] as DataTemplate;
                if (dt != null)
                {
                    gvc.CellTemplate = dt;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("StrResult");
                }
                gv.Columns.Add(gvc);
                LvUpdateStateItems.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LogInfo(string msg)
        {
            SetCurrentOperation(msg);
            AppendMessage(msg);
        }

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtDetail.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtDetail.ScrollToEnd();
            }));
        }

        private void SetCurrentOperation(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtCurrentOperation.Text = msg;
                TxtCurrentOperation.ToolTip = msg;
            }));
        }

        private void SetTotalProgress(double progress)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                ProgressUpdate.Value = progress;
                TxtUpdateProgress.Text = string.Format(" {0} %", progress.ToString("0.00"));
            }));
        }

        private void SetStateItemDisplay(UpdateStateItem item)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                item.StrProgress = string.Format(" {0} %", item.Progress.ToString("0.0"));
                item.StrResult = string.Format("Result:{0}", item.Result);
                Brush brush = Brushes.Black;
                if (item.Result == 0)
                {
                    brush = Brushes.Black;
                }
                if (item.Result == 1)
                {
                    brush = Brushes.Green;
                }
                if (item.Result == 2)
                {
                    brush = Brushes.DarkGreen;
                }
                if (item.Result == 3)
                {
                    brush = Brushes.Red;
                }
                item.Foreground = brush;
            }));
        }

        private void ResetStateItem()
        {
            for (int i = 0; i < mListUpdateStateItems.Count; i++)
            {
                var item = mListUpdateStateItems[i];
                item.Progress = 0;
                item.Result = 0;
                item.StrProgress = string.Format(" {0} %", item.Progress.ToString("0.0"));
                item.StrResult = string.Format("Result:{0}", item.Result);
                Brush brush = Brushes.Black;
                if (item.Result == 0)
                {
                    brush = Brushes.Black;
                }
                if (item.Result == 1)
                {
                    brush = Brushes.Green;
                }
                if (item.Result == 2)
                {
                    brush = Brushes.DarkGreen;
                }
                if (item.Result == 3)
                {
                    brush = Brushes.Red;
                }
                item.Foreground = brush;
            }
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

        private string FormatVersion(string strVersion)
        {
            //格式化版本，将点分形式的版本（如：8.03.001 P02.000）转换成标准版本信息（803001P02000）
            string strReturn = strVersion;
            try
            {
                strVersion = strVersion.Replace('P', '.');
                strVersion = strVersion.Replace('p', '.');
                string[] strs = strVersion.Split(new[] { '.' });
                int a = 0;
                int b = 0;
                int c = 0;
                int d = 0;
                int e = 0;
                bool hasPack = false;
                if (strs.Length > 0)
                {
                    a = int.Parse(strs[0].Trim());
                }
                if (strs.Length > 1)
                {
                    b = int.Parse(strs[1].Trim());
                }
                if (strs.Length > 2)
                {
                    c = int.Parse(strs[2].Trim());
                }
                if (strs.Length > 3)
                {
                    d = int.Parse(strs[3].Trim());
                    hasPack = true;
                }
                if (strs.Length > 4)
                {
                    e = int.Parse(strs[4].Trim());
                }
                strReturn = string.Format("{0}{1}{2}",
                    a.ToString("0"),
                    b.ToString("00"),
                    c.ToString("000"));
                if (hasPack)
                {
                    strReturn += string.Format("P{0}{1}", d.ToString("00"), e.ToString("000"));
                }
            }
            catch { }
            return strReturn;
        }

        private double CompareVersion(string ver1, string ver2)
        {
            //比较两个版本号的大小，返回正表示ver2比ver1新版本
            try
            {
                string str;
                if (ver1.Contains('.'))
                {
                    str = ver1.Replace('P', '.');
                    str = str.Replace('p', '.');
                    string[] list = str.Split(new[] { '.' });
                    int a = 0;
                    int b = 0;
                    int c = 0;
                    int d = 0;
                    int e = 0;
                    bool hasPack = false;
                    if (list.Length > 0)
                    {
                        a = int.Parse(list[0].Trim());
                    }
                    if (list.Length > 1)
                    {
                        b = int.Parse(list[1].Trim());
                    }
                    if (list.Length > 2)
                    {
                        c = int.Parse(list[2].Trim());
                    }
                    if (list.Length > 3)
                    {
                        d = int.Parse(list[3].Trim());
                        hasPack = true;
                    }
                    if (list.Length > 4)
                    {
                        e = int.Parse(list[4].Trim());
                    }
                    if (!hasPack)
                    {
                        str = string.Format("{0}{1}{2}", a.ToString("0"), b.ToString("00"), c.ToString("000"));
                    }
                    else
                    {
                        str = string.Format("{0}{1}{2}.{3}{4}", a.ToString("0"), b.ToString("00"), c.ToString("000"),
                            d.ToString("00"), e.ToString("000"));
                    }
                }
                else
                {
                    str = ver1.Replace('P', '.');
                    str = str.Replace('p', '.');
                }
                double doubleVer1 = double.Parse(str);

                if (ver2.Contains('.'))
                {
                    str = ver2.Replace('P', '.');
                    str = str.Replace('p', '.');
                    string[] list = str.Split(new[] { '.' });
                    int a = 0;
                    int b = 0;
                    int c = 0;
                    int d = 0;
                    int e = 0;
                    bool hasPack = false;
                    if (list.Length > 0)
                    {
                        a = int.Parse(list[0].Trim());
                    }
                    if (list.Length > 1)
                    {
                        b = int.Parse(list[1].Trim());
                    }
                    if (list.Length > 2)
                    {
                        c = int.Parse(list[2].Trim());
                    }
                    if (list.Length > 3)
                    {
                        d = int.Parse(list[3].Trim());
                        hasPack = true;
                    }
                    if (list.Length > 4)
                    {
                        e = int.Parse(list[4].Trim());
                    }
                    if (!hasPack)
                    {
                        str = string.Format("{0}{1}{2}", a.ToString("0"), b.ToString("00"), c.ToString("000"));
                    }
                    else
                    {
                        str = string.Format("{0}{1}{2}.{3}{4}", a.ToString("0"), b.ToString("00"), c.ToString("000"),
                            d.ToString("00"), e.ToString("000"));
                    }
                }
                else
                {
                    str = ver2.Replace('P', '.');
                    str = str.Replace('p', '.');
                }
                double doubleVer2 = double.Parse(str);
                return doubleVer2 - doubleVer1;
            }
            catch
            {
                return -1;
            }
        }

        private string DecodeEncryptValue(string strValue)
        {
            string strReturn = strValue;
            try
            {
                if (strValue.StartsWith(string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR)))
                {
                    strValue = strValue.Substring(3);
                    string[] arrContent = strValue.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                    string strVersion = string.Empty, strMode = string.Empty, strContent = strValue;
                    if (arrContent.Length > 0)
                    {
                        strVersion = arrContent[0];
                    }
                    if (arrContent.Length > 1)
                    {
                        strMode = arrContent[1];
                    }
                    if (arrContent.Length > 2)
                    {
                        strContent = arrContent[2];
                    }
                    if (strVersion == "2" && strMode == "hex")
                    {
                        strValue = App.DecryptStringM002(strContent);
                    }
                    strReturn = strValue;
                }
            }
            catch { }
            return strReturn;
        }

        #endregion


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

        void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdateUMP();
        }

        void LoggingUpdateHelper_Progress(double progress)
        {
            try
            {
                var p = progress / 100.0;
                var stateItem = mListUpdateStateItems.FirstOrDefault(s => s.State == ConstDefines.STA_UPDATESERVER);
                if (stateItem != null)
                {
                    stateItem.Progress = p * 100;
                    SetStateItemDisplay(stateItem);
                }
                SetTotalProgress(p * ConstDefines.PRO_UPDATESERVER + ConstDefines.PRO_BASE_UPDATESERVER);
            }
            catch (Exception ex)
            {
                App.WriteLog("LoggingHelper", string.Format("Progress fail.\t{0}", ex.Message));
            }
        }

        void LoggingUpdateHelper_AppendMessage(bool isCurrentOpt, string msg)
        {
            AppendMessage(msg);
            if (isCurrentOpt)
            {
                SetCurrentOperation(msg);
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
                for (int i = 0; i < mListUpdateStateItems.Count; i++)
                {
                    var item = mListUpdateStateItems[i];
                    item.Display = App.GetLanguageInfo(string.Format("U{0}", item.State.ToString("000")), item.Name);
                }

                CreateUpdateItemColumns();

                BtnDetail.Content = App.GetLanguageInfo("B006", "Detail informations");

                TxtUpdateResult.Text = mIsOptFail ? App.GetLanguageInfo("T018", "Update fail!") : App.GetLanguageInfo("T017", "Update successful!");

                BtnPrevious.Content = App.GetLanguageInfo("B001", "Previous");
                BtnUpdate.Content = App.GetLanguageInfo("B004", "Next");
                BtnClose.Content = App.GetLanguageInfo("B003", "Close");
            }
            catch { }
        }

        #endregion

    }
}
