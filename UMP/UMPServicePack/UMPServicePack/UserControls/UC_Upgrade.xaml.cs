using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using UMPServicePack.PublicClasses;
using UMPServicePackCommon;
using VoiceCyber.Common;
using System.IO;
using VoiceCyber.UMP.Common;
using System.Data;
using System.Threading;

namespace UMPServicePack.UserControls
{
    /// <summary>
    /// UC_Upgrade.xaml 的交互逻辑
    /// </summary>
    public partial class UC_Upgrade : UserControl
    {
        public MainWindow main = null;
        ObservableCollection<UpdateProgram> lstAllPrograms = new ObservableCollection<UpdateProgram>();
        bool bIsLoaded = false;
        BackgroundWorker mBackgroundWorker = null;
        public string BackupPath = string.Empty;
        public bool IsBackupUMP = true;
        //已经检查过的 且存在的依赖文件（避免多次查找文件）
        List<string> lstDependFiles = new List<string>();
        //更新文件解压的临时目录
        string strUpdateFileTempPath = string.Empty;
        /// <summary>
        /// 文件处理失败时 手动处理选择的结果 0：退出 1：重试
        /// </summary>
        int iChoose = -1;
        //正在处理的文件所属目录 用来在出错时打开目录
        string strDealingDir = string.Empty;

        //ManualResetEvent用来控制mBackgroundWorker的暂停和恢复
        private ManualResetEvent manualReset = new ManualResetEvent(true);

        //保存
        public string Ntidrv_IP_NewName = string.Empty;
        public string Ntidrv_Snyway_NewName = string.Empty;

        public UC_Upgrade()
        {
            InitializeComponent();
            Loaded += UC_Upgrade_Loaded;
        }

        void UC_Upgrade_Loaded(object sender, RoutedEventArgs e)
        {
            App.WriteLog("UC_Upgrade_Loaded");
            #region 关联事件
            btnShowDetail.Click += btnShowDetail_Click;
            btnExit.Click += btnExit_Click;
            btnRetry.Click += btnRetry_Click;
            btnOpenDirectory.Click += btnOpenDirectory_Click;
            #endregion

            main.SetBusy(true);

            lvUpdate.ItemsSource = lstAllPrograms;
            InitListViewHeader();
            InitListViewItem();
            bIsLoaded = true;

            UpdateStart();
        }

        #region 事件
        void btnShowDetail_Click(object sender, RoutedEventArgs e)
        {
            if (txtDetail.Visibility == System.Windows.Visibility.Visible)
            {
                txtDetail.Visibility = System.Windows.Visibility.Hidden;
                btnShowDetail.Content = App.GetLanguage("string37", "string37");
            }
            else
            {
                txtDetail.Visibility = System.Windows.Visibility.Visible;
                btnShowDetail.Content = App.GetLanguage("string85", "string85");
            }
        }

        void btnOpenDirectory_Click(object sender, RoutedEventArgs e)
        {
            OperationReturn optReturn = CommonFuncs.CmdOperator("explorer.exe " + strDealingDir);
            if (!optReturn.Result)
            {
                App.WriteLog("Open directory " + strDealingDir + " failed. " + optReturn.Message);
            }
        }

        void btnRetry_Click(object sender, RoutedEventArgs e)
        {
            popMsgBox.IsOpen = false;
            iChoose = 1;
            manualReset.Set();
        }

        void btnExit_Click(object sender, RoutedEventArgs e)
        {
            popMsgBox.IsOpen = false;
            iChoose = 0;
            manualReset.Set();
        }

        #endregion

        #region Init
        private void InitListViewHeader()
        {
            List<GridColumnClass> lstColumns = new List<GridColumnClass>();
            GridColumnClass coluClass = new GridColumnClass();
            coluClass.ColName = "ProgramName";
            coluClass.ColDisPlay = "string38";
            lstColumns.Add(coluClass);

            coluClass = new GridColumnClass();
            coluClass.ColName = "Descript";
            coluClass.ColDisPlay = "string39";
            coluClass.ColWidth = 400;
            lstColumns.Add(coluClass);

            coluClass = new GridColumnClass();
            coluClass.ColName = "Status";
            coluClass.ColDisPlay = "string40";
            lstColumns.Add(coluClass);

            GridView gv = new GridView();
            GridViewColumn gvc = null;
            GridViewColumnHeader gvch;
            for (int i = 0; i < lstColumns.Count; i++)
            {
                gvc = new GridViewColumn();
                gvch = new GridViewColumnHeader();
                coluClass = lstColumns[i];
                gvch.Content = App.GetLanguage(coluClass.ColDisPlay, coluClass.ColDisPlay);
                gvc.Header = gvch;
                gvc.Width = coluClass.ColWidth;

                DataTemplate dt = null;
                if (coluClass.ColName == "Status")
                {
                    dt = this.FindResource("StatusIconCellTemplate") as DataTemplate;
                }
                if (dt == null)
                {
                    string strColName = coluClass.ColName;
                    gvc.DisplayMemberBinding = new Binding(strColName);
                }
                else
                {
                    gvc.CellTemplate = dt;
                }
                gv.Columns.Add(gvc);
            }
            lvUpdate.View = gv;
        }

        private void InitListViewItem()
        {
            lstAllPrograms.Clear();
            UpdateProgram program = new UpdateProgram();
            program.ProgramName = App.GetLanguage("string41", "string41");
            program.Descript = App.GetLanguage("string42", "string42");
            program.Background = Brushes.LightGray;
            lstAllPrograms.Add(program);


            program = new UpdateProgram();
            program.ProgramName = App.GetLanguage("string43", "string43");
            program.Descript = App.GetLanguage("string44", "string44");
            program.Background = Brushes.Transparent;
            lstAllPrograms.Add(program);

            program = new UpdateProgram();
            program.ProgramName = App.GetLanguage("string45", "string45");
            program.Descript = App.GetLanguage("string46", "string46");
            program.Background = Brushes.LightGray;
            lstAllPrograms.Add(program);

            foreach (UpdateProgram item in lstAllPrograms)
            {
                item.IsWait = true;
                item.IsDone = false;
                item.IsDoing = false;
            }

        }

        /// <summary>
        /// 开始升级
        /// </summary>
        private void UpdateStart()
        {
            mBackgroundWorker = new BackgroundWorker();
            mBackgroundWorker.DoWork += mBackgroundWorker_DoWork;
            mBackgroundWorker.RunWorkerCompleted += mBackgroundWorker_RunWorkerCompleted;
            mBackgroundWorker.RunWorkerAsync();
            App.WriteLog("mBackgroundWorker start");
        }

        /// <summary>
        /// 升级完成的事后处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            main.SetBusy(false);
        }

        /// <summary>
        /// 开始升级
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

            OperationReturn optReturn = null;
            while (true)
            {
                if (!bIsLoaded)
                {
                    continue;
                }
                App.WriteLog("mBackgroundWorker begin, bIsLoaded=true");
                Dispatcher.Invoke(new Action(() =>
                {
                    string strDetail = DateTime.Now.ToString() + " : ";
                    strDetail += App.GetLanguage("string75", "string75");
                    txtDetail.Text += strDetail + "\r\n";
                    SetGridViewItemImage(0, false, true, false);
                }));
                //解压更新文件
                optReturn = UnZip();
                if (!optReturn.Result)
                {
                    //如果出错，直接跳出循环 进入完成界面
                    Dispatcher.Invoke(new Action(() =>
                        {
                            GoToDone(false);
                        }));
                    App.WriteLog("Error code :" + optReturn.Code + ". " + optReturn.Message);
                    break;
                }
                optReturn = RenameNtidrv();
                if (!optReturn.Result)
                {
                    //如果出错，直接跳出循环 进入完成界面
                    Dispatcher.Invoke(new Action(() =>
                    {
                        GoToDone(false);
                    }));
                    App.WriteLog("Error code :" + optReturn.Code + ". " + optReturn.Message);
                    break;
                }
                //备份每个安装包的文件
                if (IsBackupUMP)
                {
                    optReturn = CompressAllFiles();
                    if (!optReturn.Result)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            GoToDone(false);
                        }));
                        App.WriteLog("Error code : " + optReturn.Code + ". " + optReturn.Message);
                        break;
                    }
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    string strDetail = DateTime.Now.ToString() + " : ";
                    txtDetail.Text += strDetail + App.GetLanguage("string63", "string63") + "\r\n";
                }));
                //更新界面图标状态
                Dispatcher.Invoke(new Action(() =>
                {
                    SetGridViewItemImage(0, false, false, true);
                    SetGridViewItemImage(1, false, true, false);
                }));

                ////停止服务
                if (App.bIsUMPServer)
                {
                    // 如果是UMP服务器 停掉IIS服务
                    optReturn = CommonFuncs.CmdOperator("iisreset");
                    if (!optReturn.Result)
                    {
                        App.WriteLog("Restart IIS failed. " + optReturn.Message);
                    }
                }
                optReturn = StopService();
                if (!optReturn.Result)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        GoToDone(false);
                    }));
                }

                //服务停止完 开始更新文件
                optReturn = UpdateFiles();
                if (!optReturn.Result)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        GoToDone(false);
                    }));
                    break;
                }

                //文件更新完成 执行JavaScript
                Dispatcher.Invoke(new Action(() =>
                {
                    SetGridViewItemImage(1, false, false, true);
                    SetGridViewItemImage(2, false, true, false);
                }));
                if (App.bIsUMPServer)
                {
                    //如果是UMP服务器 需要更新数据库
                    optReturn = ExcuteSql();
                    if (!optReturn.Result)
                    {
                        Dispatcher.Invoke(new Action(() =>
                        {
                            GoToDone(false);
                        }));
                        break;
                    }
                }
                //启动服务
                optReturn = UpdateServices();
                if (!optReturn.Result)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        GoToDone(false);
                    }));
                    break;
                }
                else
                {
                    string strDetail = DateTime.Now.ToString() + " : ";
                    strDetail += App.GetLanguage("string84", "string84");
                    Dispatcher.Invoke(new Action(() =>
                    {
                        txtDetail.Text += strDetail + "\r\n";
                    }));
                }
                //删除解压出来的文件
                //try
                //{
                //    Directory.Delete(App.gStrUpdateFolderTempPath, true);
                //}
                //catch (Exception ex)
                //{
                //    App.WriteLog("Delete " + App.gStrUpdateFolderTempPath + " failed. " + ex.Message);
                //}

                Dispatcher.Invoke(new Action(() =>
                {
                    GoToDone(true);
                }));
                //不可删 不然会死循环
                break;
            }
        }
        #endregion

        #region 被调用的函数
        /// <summary>
        /// 设置listview“状态”列显示的图片
        /// </summary>
        /// <param name="index">listviewItemIndex</param>
        /// <param name="bIsWait"></param>
        /// <param name="bIsDoing"></param>
        /// <param name="bIsDone"></param>
        private void SetGridViewItemImage(int index, bool bIsWait, bool bIsDoing, bool bIsDone)
        {
            lstAllPrograms[index].IsWait = bIsWait;
            lstAllPrograms[index].IsDoing = bIsDoing;
            lstAllPrograms[index].IsDone = bIsDone;
        }

        /// <summary>
        /// 解压更新文件到 ProgramData/UMP/ServicePack/版本
        /// </summary>
        /// <returns></returns>
        private OperationReturn UnZip()
        {
            OperationReturn optReturn = new OperationReturn();
            Uri uri = new Uri(@"pack://application:,,,/UpdateFiles/Data.zip", UriKind.Absolute);
            var zipFile = App.GetResourceStream(uri);
            if (zipFile != null)
            {
                string strUpgradeFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData)
                    , @"VoiceCyber/UMP/ServicePack/{0}");
                strUpgradeFolder = string.Format(strUpgradeFolder, App.updateInfo.Version);
                strUpdateFileTempPath = strUpgradeFolder;
                App.gStrUpdateFolderTempPath = strUpgradeFolder;
                optReturn = ZipHelper.UnZip(zipFile.Stream, strUpgradeFolder);
            }
            else
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Compress_Src_Not_Found;
                App.WriteLog("Error code : " + ConstDefines.Upgrade_File_Not_Found);
            }
            return optReturn;
        }

        /// <summary>
        /// 重命名ntidrv.dll
        /// </summary>
        /// <returns></returns>
        private OperationReturn RenameNtidrv()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            try
            {
                List<UpdateFile> lstFiles = App.updateInfo.ListFiles.Where(p => p.FileName.ToLower() == "ntidrv.dll" && p.Package == ConstDefines.IxPatch).ToList();
                if (lstFiles.Count > 0 && !string.IsNullOrEmpty(Ntidrv_Snyway_NewName))
                {
                    optReturn = RenameFile(lstFiles[0], Ntidrv_Snyway_NewName);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("Rename " + lstFiles[0].Name + " to " + Ntidrv_Snyway_NewName + " failed, " + optReturn.Message);
                        return optReturn;
                    }
                    UpdateFile file = lstFiles[0];
                    file.FileName = Ntidrv_Snyway_NewName;
                    App.updateInfo.ListFiles.Remove(file);
                    App.updateInfo.ListFiles.Add(file);
                }
                lstFiles = App.updateInfo.ListFiles.Where(p => p.FileName.ToLower() == "ntidrv.dll" && p.Package == ConstDefines.UMPSoftRecord).ToList();
                if (lstFiles.Count > 0 && !string.IsNullOrEmpty(Ntidrv_IP_NewName))
                {
                    optReturn = RenameFile(lstFiles[0], Ntidrv_IP_NewName);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("Rename " + lstFiles[0].Name + " to " + Ntidrv_IP_NewName + " failed, " + optReturn.Message);
                        return optReturn;
                    }
                    UpdateFile file = lstFiles[0];
                    file.FileName = Ntidrv_IP_NewName;
                    App.updateInfo.ListFiles.Remove(file);
                    App.updateInfo.ListFiles.Add(file);
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Message = ex.Message;
                optReturn.Code = ConstDefines.Rename_File_Exception;
            }
            return optReturn;
        }

        /// <summary>
        /// 进入完成界面
        /// </summary>
        private void GoToDone(bool bIsSuccess)
        {
            UC_Done uc_Done = new UC_Done();
            uc_Done.bIsSuccess = bIsSuccess;
            uc_Done.main = main;
            main.borderUpdater.Child = uc_Done;
        }

        /// <summary>
        /// 备份并压缩所有安装包的安装文件
        /// </summary>
        /// <returns></returns>
        private OperationReturn CompressAllFiles()
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                //由于可能多个安装包按照在一个路径下 所以这个list记录已经备份的路径
                List<string> lstBackuped = new List<string>();
                string strDetail = string.Empty;
                foreach (KeyValuePair<string, UMPAppInfo> item in App.dicAppInstalled)
                {
                    Dispatcher.Invoke(new Action(() =>
                        {
                            strDetail = DateTime.Now.ToString() + " : ";
                            strDetail += App.GetLanguage("string56", "string56");
                            strDetail = string.Format(strDetail, item.Key.ToString());
                            txtDetail.Text += strDetail + "\r\n";
                        }));
                    App.WriteLog(item.Key.ToString() + " - " + item.Value.AppInstallPath);
                    if (lstBackuped.Contains(item.Value.AppInstallPath))
                    {
                        strDetail = DateTime.Now.ToString() + " : ";
                        strDetail += App.GetLanguage("string67", "string67");
                        strDetail = string.Format(strDetail, item.Key.ToString());
                        Dispatcher.Invoke(new Action(() =>
                        {
                            txtDetail.Text += strDetail + "\r\n";
                        }));
                        continue;
                    }
                    string strBackUpPath = BackupPath;
                    if (string.IsNullOrEmpty(strBackUpPath))
                    {
                        strBackUpPath = item.Value.AppInstallPath + "Backup";
                    }
                    string strDirName = item.Value.AppInstallPath.Substring(item.Value.AppInstallPath.LastIndexOf(@"\") + 1);
                    string strZipName = strDirName + ".zip";
                    optReturn = CommonFuncs.CopyFolder(item.Value.AppInstallPath, strBackUpPath);

                    App.WriteLog(item.Value.AppInstallPath + " copy to " + strBackUpPath);
                    if (!optReturn.Result)
                    {
                        strDetail = DateTime.Now.ToString() + " : ";
                        strDetail += App.GetLanguage("string58", "string58");
                        Dispatcher.Invoke(new Action(() =>
                       {
                           txtDetail.Text += strDetail + "\r\n";
                       }));
                        App.WriteLog("Error code : " + optReturn.Code + ". " + optReturn.Message);
                        return optReturn;
                    }
                    else
                    {
                        strDetail = DateTime.Now.ToString() + " : ";
                        strDetail += App.GetLanguage("string59", "string59");
                        Dispatcher.Invoke(new Action(() =>
                        {
                            txtDetail.Text += strDetail + "\r\n";
                        }));
                        strDetail = strDetail = DateTime.Now.ToString() + " : ";
                        strDetail += App.GetLanguage("string62", "string62");
                        strDetail = string.Format(strDetail, item.Value.AppInstallPath, strBackUpPath);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            txtDetail.Text += strDetail + "\r\n";
                        }));
                        App.WriteLog(item.Value.AppInstallPath + " copy  success");
                    }
                    strDetail = DateTime.Now.ToString() + " : ";
                    strDetail += App.GetLanguage("string57", "string57");
                    strDetail = string.Format(strDetail, item.Key.ToString());
                    Dispatcher.Invoke(new Action(() =>
                    {
                        txtDetail.Text += strDetail + "\r\n";
                    }));
                    string strTarget = strBackUpPath + "\\" + strZipName;
                    optReturn = ZipHelper.ZipFileDirectory(strBackUpPath + "\\" + strDirName, strTarget, string.Empty);
                    if (!optReturn.Result)
                    {
                        strDetail = DateTime.Now.ToString() + " : ";
                        strDetail += App.GetLanguage("string60", "string60");
                        Dispatcher.Invoke(new Action(() =>
                        {
                            txtDetail.Text += strDetail + "\r\n";
                        }));
                        App.WriteLog(optReturn.Message);
                        break;
                    }
                    else
                    {
                        try
                        {
                            Directory.Delete(strBackUpPath + "\\" + strDirName, true);
                        }
                        catch (Exception ex)
                        {
                            App.WriteLog("Delete directory failed . Dir = " + strBackUpPath + "\\" + strDirName + " . error:" + ex.Message);
                        }
                        strDetail = DateTime.Now.ToString() + " : ";
                        strDetail += App.GetLanguage("string61", "string61");
                        Dispatcher.Invoke(new Action(() =>
                        {
                            txtDetail.Text += strDetail + "\r\n";
                        }));
                        if (!lstBackuped.Contains(item.Value.AppInstallPath))
                        {
                            lstBackuped.Add(item.Value.AppInstallPath);
                        }
                        App.WriteLog("Backup directory" + item.Value.AppInstallPath + " to " + strTarget + " success");
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                App.WriteLog("compress file error : " + ex.Message);
                return optReturn;
            }
            App.WriteLog("CompressAllFiles done");
            return optReturn;
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <returns></returns>
        private OperationReturn StopService()
        {
            string strServiceName = string.Empty;
            OperationReturn optReturn = new OperationReturn();
            List<UpdateService> lstServices = App.updateInfo.ListServices;
            for (int i = 0; i < lstServices.Count; i++)
            {
                UpdateService serviceInfo = lstServices[i];
                if (serviceInfo.InstallMode == (int)ServiceInstallMode.Install || serviceInfo.InstallMode == (int)ServiceInstallMode.None
                    || serviceInfo.InstallMode == (int)ServiceInstallMode.Uninstall || serviceInfo.InstallMode == (int)ServiceInstallMode.UninstallError)
                {
                    App.WriteLog("Stop service " + serviceInfo.ServiceName);
                    string strDetail = DateTime.Now.ToString() + " : ";
                    strDetail += App.GetLanguage("string64", "string64");
                    strDetail += " " + serviceInfo.Name;
                    Dispatcher.Invoke(new Action(() =>
                    {
                        txtDetail.Text += strDetail + "\r\n";
                    }));
                    optReturn = CommonFuncs.StopServiceByName(serviceInfo.ServiceName);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("Stop service " + serviceInfo.ServiceName + " failed. " + optReturn.Message);
                        strDetail = DateTime.Now.ToString() + " : ";
                        strDetail += App.GetLanguage("string65", "string65");
                        strDetail = string.Format(strDetail, serviceInfo.Name);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            txtDetail.Text += strDetail + "\r\n";
                        }));
                        return optReturn;
                    }
                    else
                    {
                        App.WriteLog("Stop service " + serviceInfo.ServiceName + " success");
                        strDetail = DateTime.Now.ToString() + " : ";
                        strDetail += App.GetLanguage("string66", "string66");
                        Dispatcher.Invoke(new Action(() =>
                        {
                            txtDetail.Text += strDetail + "\r\n";
                        }));
                    }
                }
            }

            //optReturn = CommonFuncs.StopServiceByName(strServiceName);
            return optReturn;
        }

        /// <summary>
        /// 更新文件
        /// </summary>
        /// <returns></returns>
        private OperationReturn UpdateFiles()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                List<UpdateFile> lstFiles = App.updateInfo.ListFiles;
                UpdateFile file = null;
                string strDependFilePath = string.Empty;
                string strDetail = string.Empty;
                string strPackageService00 = "UMPService";
                for (int i = 0; i < lstFiles.Count; i++)
                {
                    file = lstFiles[i];
                    App.WriteLog("start update " + file.Name);
                    //判断安装包是否安装
                    if (!App.dicAppInstalled.Keys.Contains(file.Package))
                    {
                        //UMPService 例外，需特殊处理
                        if (!file.Package.Equals(strPackageService00))
                        {
                            continue;
                        }
                    }
                    strDetail = DateTime.Now.ToString() + " : ";
                    strDetail += App.GetLanguage("string89", "string89");
                    strDetail = string.Format(strDetail, file.Name);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        txtDetail.Text += strDetail + "\r\n";
                    }));
                    string strInstallPath;
                    if (file.Package.Equals(strPackageService00))
                    {
                        //录音服务器上的UMPService00的安装目录是固定的
                        strInstallPath =
                            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
                                "VoiceCyber\\UMPService");
                    }
                    else
                    {
                        strInstallPath = App.dicAppInstalled[file.Package].AppInstallPath;
                    }
                    //判断DependFile是否存在
                    optReturn = GetTargetPath(file.DependFile, file.TargetPathType, strInstallPath);
                    if (!optReturn.Result)
                    {
                        strDetail = DateTime.Now.ToString() + " : ";
                        strDetail += App.GetLanguage("string68", "string68");
                        strDetail = string.Format(strDetail, file.Name, file.DependFile);
                        Dispatcher.Invoke(new Action(() =>
                            {
                                txtDetail.Text += strDetail + "\r\n";
                            }));
                        App.WriteLog("Error code : " + optReturn.Code + " . " + strDetail + ". " + optReturn.Message);
                        return optReturn;
                    }
                    strDependFilePath = optReturn.Data as string;
                    if (!File.Exists(strDependFilePath))
                    {
                        //依赖文件不存在 不需要更新
                        App.WriteLog("Depend file " + strDependFilePath + " not exists , No need to update the file " + file.Name);
                        continue;
                    }
                    //需要更新 判断是更新文件还是文件夹
                    string strSourcePath = strUpdateFileTempPath + "\\" + file.SourcePath + "\\" + file.FileName;
                    optReturn = GetTargetPath(file.TargetPath, file.TargetPathType, strInstallPath);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("Error code : " + optReturn.Code + ". Get file {0} target path failed. " + optReturn.Message);
                        strDetail = DateTime.Now.ToString() + " : ";
                        strDetail += App.GetLanguage("string69", "string69");
                        strDetail = string.Format(strDetail, file.Name);
                        Dispatcher.Invoke(new Action(() =>
                        {
                            txtDetail.Text += strDetail + "\r\n";
                        }));
                        return optReturn;
                    }
                    string strTargetPath = optReturn.Data as string;
                    if (file.Type == (int)UpdateFileType.Directory)
                    {
                        //如果是文件夹
                        string strDirTargetPath = strTargetPath + "\\" + file.FileName;
                        DirectoryInfo dirInfo = new DirectoryInfo(strDirTargetPath);
                        //根据installMode来判断如果更新
                        switch (file.InstallMode)
                        {
                            case (int)FileInstallMode.CopyError:
                                if (dirInfo.Exists)
                                {
                                    //存在 在界面上报告错误，但升级仍继续
                                    strDetail = DateTime.Now.ToString() + " : ";
                                    strDetail += App.GetLanguage("string70", "string70");
                                    strDetail = string.Format(strDetail, file.Name, strTargetPath);
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        txtDetail.Text += strDetail + "\r\n";
                                    }));
                                }
                                else
                                {
                                    optReturn = CopyFolderInUpgrade(strSourcePath, strTargetPath);
                                    if (!optReturn.Result)
                                    {
                                        App.WriteLog("Error code : " + optReturn.Code + ". Update directory " + file.Name + " failed. " + optReturn.Message);
                                        return optReturn;
                                    }
                                }
                                break;
                            case (int)FileInstallMode.CopyIgnore:
                                if (dirInfo.Exists)
                                {
                                    //存在 在界面上提示已跳过
                                    strDetail = DateTime.Now.ToString() + " : ";
                                    strDetail += App.GetLanguage("string72", "string72");
                                    strDetail = string.Format(strDetail, file.Name, strTargetPath);
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        txtDetail.Text += strDetail + "\r\n";
                                    }));
                                }
                                else
                                {
                                    optReturn = CopyFolderInUpgrade(strSourcePath, strTargetPath);
                                    if (!optReturn.Result)
                                    {
                                        App.WriteLog("Error code : " + optReturn.Code + ". Update directory " + file.Name + " failed. " + optReturn.Message);
                                        return optReturn;
                                    }
                                }
                                break;
                            case (int)FileInstallMode.CopyReplace:
                                optReturn = CopyFolderInUpgrade(strSourcePath, strTargetPath);
                                if (!optReturn.Result)
                                {
                                    App.WriteLog("Error code : " + optReturn.Code + ". Update directory " + file.Name + " failed. " + optReturn.Message);
                                    return optReturn;
                                }
                                break;
                            case (int)FileInstallMode.CopyDelete:
                                if (dirInfo.Exists)
                                {
                                    try
                                    {
                                        Directory.Delete(strDirTargetPath, true);
                                        optReturn = CopyFolderInUpgrade(strSourcePath, strTargetPath);
                                        if (!optReturn.Result)
                                        {
                                            App.WriteLog("Error code : " + optReturn.Code + ". Update directory " + file.Name + " failed. " + optReturn.Message);
                                            return optReturn;
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        optReturn.Result = false;
                                        optReturn.Code = ConstDefines.Delete_Directory_Exception;
                                        optReturn.Message = ex.Message;
                                        App.WriteLog("Error code :" + ConstDefines.Delete_Directory_Exception + ". " + file.FileName + ". " + ex.Message);
                                        return optReturn;
                                    }
                                }
                                break;
                            case (int)FileInstallMode.RemoveError:
                                if (dirInfo.Exists)
                                {
                                    try
                                    {
                                        Directory.Delete(strDirTargetPath, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        optReturn.Result = false;
                                        optReturn.Code = ConstDefines.Delete_Directory_Exception;
                                        optReturn.Message = ex.Message;
                                        App.WriteLog("Error code :" + ConstDefines.Delete_Directory_Exception + ". " + file.FileName + ". " + ex.Message);
                                        return optReturn;
                                    }
                                }
                                else
                                {
                                    strDetail = DateTime.Now.ToString() + " : ";
                                    strDetail += App.GetLanguage("string73", "string73");
                                    strDetail = string.Format(strDetail, file.Name, strTargetPath);
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        txtDetail.Text += strDetail + "\r\n";
                                    }));
                                }
                                break;
                            case (int)FileInstallMode.RemoveIgnore:
                                if (dirInfo.Exists)
                                {
                                    try
                                    {
                                        Directory.Delete(strDirTargetPath, true);
                                    }
                                    catch (Exception ex)
                                    {
                                        optReturn.Result = false;
                                        optReturn.Code = ConstDefines.Delete_Directory_Exception;
                                        optReturn.Message = ex.Message;
                                        App.WriteLog("Error code :" + ConstDefines.Delete_Directory_Exception + ". " + file.FileName + ". " + ex.Message);
                                        return optReturn;
                                    }
                                }
                                else
                                {
                                    strDetail = DateTime.Now.ToString() + " : ";
                                    strDetail += App.GetLanguage("string74", "string74");
                                    strDetail = string.Format(strDetail, file.Name, strTargetPath);
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        txtDetail.Text += strDetail + "\r\n";
                                    }));
                                }
                                break;
                        }
                    }
                    else if (file.Type == (int)UpdateFileType.File)
                    {
                        //如果是文件
                        string strTargetFilePath = strTargetPath + "\\" + file.FileName;
                        FileInfo fileInfo = new FileInfo(strTargetFilePath);
                        switch (file.InstallMode)
                        {
                            case (int)FileInstallMode.CopyDelete:
                                File.Delete(strTargetFilePath);
                                File.Copy(strSourcePath, strTargetFilePath);
                                break;
                            case (int)FileInstallMode.CopyError:
                                if (fileInfo.Exists)
                                {
                                    strDetail = DateTime.Now.ToString() + " : ";
                                    strDetail += App.GetLanguage("string70", "string70");
                                    strDetail = string.Format(strDetail, file.Name, strTargetPath);
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        txtDetail.Text += strDetail + "\r\n";
                                    }));
                                }
                                break;
                            case (int)FileInstallMode.CopyIgnore:
                                if (!fileInfo.Exists)
                                {
                                    File.Copy(strSourcePath, strTargetFilePath);
                                }
                                else
                                {
                                    strDetail = DateTime.Now.ToString() + " : ";
                                    strDetail += App.GetLanguage("string72", "string72");
                                    strDetail = string.Format(strDetail, file.Name, strTargetPath);
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        txtDetail.Text += strDetail + "\r\n";
                                    }));
                                }
                                break;
                            case (int)FileInstallMode.CopyReplace:
                                if (!Directory.Exists(strTargetPath))
                                {
                                    Directory.CreateDirectory(strTargetPath);
                                }
                                File.Copy(strSourcePath, strTargetFilePath, true);
                                break;
                            case (int)FileInstallMode.RemoveError:
                                if (fileInfo.Exists)
                                {
                                    try
                                    {
                                        File.Delete(strTargetFilePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        optReturn.Result = false;
                                        optReturn.Code = ConstDefines.Delete_File_Exception;
                                        optReturn.Message = ex.Message;
                                        App.WriteLog("Error code :" + ConstDefines.Delete_File_Exception + ". " + file.FileName + ". " + ex.Message);
                                        return optReturn;
                                    }
                                }
                                else
                                {
                                    strDetail = DateTime.Now.ToString() + " : ";
                                    strDetail += App.GetLanguage("string73", "string73");
                                    strDetail = string.Format(strDetail, file.Name, strTargetPath);
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        txtDetail.Text += strDetail + "\r\n";
                                    }));
                                }
                                break;
                            case (int)FileInstallMode.RemoveIgnore:
                                if (fileInfo.Exists)
                                {
                                    try
                                    {
                                        File.Delete(strTargetFilePath);
                                    }
                                    catch (Exception ex)
                                    {
                                        optReturn.Result = false;
                                        optReturn.Code = ConstDefines.Delete_File_Exception;
                                        optReturn.Message = ex.Message;
                                        App.WriteLog("Error code :" + ConstDefines.Delete_File_Exception + ". " + file.FileName + ". " + ex.Message);
                                        return optReturn;
                                    }
                                }
                                else
                                {
                                    strDetail = DateTime.Now.ToString() + " : ";
                                    strDetail += App.GetLanguage("string74", "string74");
                                    strDetail = string.Format(strDetail, file.Name, strTargetPath);
                                    Dispatcher.Invoke(new Action(() =>
                                    {
                                        txtDetail.Text += strDetail + "\r\n";
                                    }));
                                }
                                break;
                        }
                    }
                    strDetail = DateTime.Now.ToString() + " : ";
                    strDetail += App.GetLanguage("string90", "string90");
                    strDetail = string.Format(strDetail, file.Name);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        txtDetail.Text += strDetail + "\r\n";
                    }));
                }
                strDetail = DateTime.Now.ToString() + " : ";
                strDetail += App.GetLanguage("string91", "string91");
                Dispatcher.Invoke(new Action(() =>
                {
                    txtDetail.Text += strDetail + "\r\n";
                }));
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Update_Files_Exception;
                optReturn.Message = ex.Message;
                App.WriteLog("Error code : " + optReturn.Code + ". " + ex.Message);
            }
            return optReturn;
        }

        /// <summary>
        /// 获得文件或文件夹的目标路径
        /// </summary>
        /// <param name="strFileName">文件名/文件夹名(相对于iTargetPathType的路径)</param>
        /// <param name="iTargetPathType">目标目录类型</param>
        /// <param name="strInstallPath">安装包的安装路径</param>
        /// <returns></returns>
        private OperationReturn GetTargetPath(string strFileName, int iTargetPathType, string strInstallPath)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            string strTargetPath = string.Empty;
            try
            {
                switch (iTargetPathType)
                {
                    case (int)TargetPathType.InstallPath:
                        strTargetPath = strInstallPath + "\\" + strFileName;
                        break;
                    case (int)TargetPathType.ProgramData:
                        strTargetPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\" + strFileName;
                        break;
                    case (int)TargetPathType.WinSysDir:
                        strTargetPath = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\" + strFileName;
                        break;
                }
                optReturn.Data = strTargetPath.TrimEnd('\\');
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Get_File_Path_Exception;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <returns></returns>
        private OperationReturn ExcuteSql()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                string strSqlDir = strUpdateFileTempPath + "\\SqlScripts";
                if (App.currDBInfo.TypeID == 2)
                {
                    strSqlDir += "\\MSSQL";
                }
                else if (App.currDBInfo.TypeID == 3)
                {
                    strSqlDir += "\\ORCL";
                }
                DirectoryInfo dirSql = new DirectoryInfo(strSqlDir);
                List<DirectoryInfo> lstChildDirs = dirSql.GetDirectories().ToList();
                DirectoryInfo item = null;
                for (int i = 0; i < lstChildDirs.Count; i++)
                {
                    item = lstChildDirs[i];
                    List<FileInfo> lstFiles = item.GetFiles().ToList();
                    for (int j = 0; j < lstFiles.Count; j++)
                    {
                        optReturn = CommonFuncs.GetScriptFile(lstFiles[j].FullName);
                        if (!optReturn.Result)
                        {
                            App.WriteLog("Get sql file content failed. file = " + lstFiles[j].FullName + ". Message : " + optReturn.Message);
                            return optReturn;
                        }
                        string strSql = optReturn.Data as string;
                        switch (App.currDBInfo.TypeID)
                        {
                            case 2:
                                optReturn = MssqlOperation.ExecuteSql(App.currDBInfo.GetConnectionString(), strSql);
                                break;
                            case 3:
                                optReturn = OracleOperation.ExecuteSql(App.currDBInfo.GetConnectionString(), strSql);
                                break;
                        }
                        if (!optReturn.Result)
                        {
                            App.WriteLog("Excute sql failed. Sql file : " + lstFiles[j].FullName + ". Message : " + optReturn.Message);
                            //return optReturn;
                        }
                        else
                        {
                            App.WriteLog("Excute sql success; Sql file : " + lstFiles[j].FullName);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.ExcuteSql_Exception;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 更新服务
        /// </summary>
        /// <returns></returns>
        private OperationReturn UpdateServices()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                List<UpdateService> lstServices = App.updateInfo.ListServices;
                foreach (UpdateService serviceItem in lstServices)
                {
                    if (!App.dicAppInstalled.Keys.Contains(serviceItem.Package))
                    {
                        continue;
                    }
                    switch (serviceItem.InstallMode)
                    {
                        case (int)ServiceInstallMode.None:
                            optReturn = StartService(serviceItem);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            break;
                        case (int)ServiceInstallMode.Install:
                            if (App.dicAllServiceStatus.Keys.Contains(serviceItem.ServiceName))
                            {
                                if (App.dicAllServiceStatus[serviceItem.ServiceName].ServiceStatus == (int)ServiceStatusType.Not_Exit)
                                {
                                    //如果不存在 就安装
                                    optReturn = ServiceOperator(serviceItem, 0);
                                    if (!optReturn.Result)
                                    {
                                        return optReturn;
                                    }
                                }
                                else
                                {
                                    //如果存在 先卸载 再安装
                                    optReturn = ServiceOperator(serviceItem, 1);
                                    if (!optReturn.Result)
                                    {
                                        return optReturn;
                                    }
                                    optReturn = ServiceOperator(serviceItem, 0);
                                    if (!optReturn.Result)
                                    {
                                        return optReturn;
                                    }
                                    optReturn = StartService(serviceItem);
                                    if (!optReturn.Result)
                                    {
                                        return optReturn;
                                    }
                                }
                            }
                            else
                            {
                                //如果不存在 表示是新增服务直接安装
                                optReturn = ServiceOperator(serviceItem, 0);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                //然后判断是否需要启动
                                optReturn = StartService(serviceItem);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                            }
                            break;
                        case (int)ServiceInstallMode.InstallSkip:

                            break;
                        case (int)ServiceInstallMode.InstallError:

                            break;
                        case (int)ServiceInstallMode.Uninstall:

                            break;
                        case (int)ServiceInstallMode.UninstallError:

                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Update_Service_Exception;
                optReturn.Message = ex.Message;
                App.WriteLog("Error code : " + optReturn.Code + ". " + optReturn.Message);
            }
            return optReturn;
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="strServiceName"></param>
        /// <returns></returns>
        private OperationReturn StartService(UpdateService serviceItem)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                string strDetail = string.Empty;
                switch (serviceItem.StartMode)
                {
                    case (int)ServiceStartMode.None:
                        if (App.dicAllServiceStatus.Keys.Contains(serviceItem.ServiceName))
                        {
                            if (App.dicAllServiceStatus[serviceItem.ServiceName].ServiceStatus == (int)ServiceStatusType.Started)
                            {
                                optReturn = StartServiceInShow(serviceItem);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                            }
                            else
                            {
                                App.WriteLog("No need to start service " + serviceItem.ServiceName);
                            }
                        }
                        else
                        {
                            App.WriteLog("No need to start service " + serviceItem.ServiceName);
                        }
                        break;
                    case (int)ServiceStartMode.Default:
                        App.lstServicesToStart.Add(serviceItem.ServiceName);
                        App.WriteLog(serviceItem.ServiceName + " service has been added to the list to start");
                        break;
                    case (int)ServiceStartMode.Immediately:
                        optReturn = StartServiceInShow(serviceItem);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case (int)ServiceStartMode.DelayTime:
                        optReturn = StartServiceInShow(serviceItem);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (serviceItem.DelayTime != 0)
                        {
                            Thread.Sleep(serviceItem.DelayTime * 1000);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return optReturn;
        }

        /// <summary>
        /// 在界面上显示启动服务的实时状况 
        /// </summary>
        /// <param name="serviceItem"></param>
        /// <returns></returns>
        private OperationReturn StartServiceInShow(UpdateService serviceItem)
        {
            OperationReturn optReturn = new OperationReturn();
            string strDetail = string.Empty;
            //启动
            strDetail = DateTime.Now.ToString() + " : ";
            strDetail += App.GetLanguage("string77", "string77");
            strDetail = string.Format(strDetail, serviceItem.ServiceName);
            Dispatcher.Invoke(new Action(() =>
            {
                txtDetail.Text += strDetail + "\r\n";
            }));

            optReturn = CommonFuncs.StartServiceByNmae(serviceItem.ServiceName);
            if (!optReturn.Result)
            {
                App.WriteLog("Start service " + serviceItem.ServiceName + " failed . " + optReturn.Message);
                return optReturn;
            }
            else
            {
                App.WriteLog("Start service " + serviceItem.ServiceName + " success . ");
                strDetail = DateTime.Now.ToString() + " : ";
                strDetail += App.GetLanguage("string76", "string76");
                strDetail = string.Format(strDetail, serviceItem.ServiceName);
                Dispatcher.Invoke(new Action(() =>
                {
                    txtDetail.Text += strDetail + "\r\n";
                }));
                optReturn.Result = true;
                return optReturn;
            }
        }

        /// <summary>
        /// 安装或卸载服务
        /// </summary>
        /// <param name="serItem"></param>
        /// <param name="OperType">操作类型 0：安装 1：卸载</param>
        /// <returns></returns>
        private OperationReturn ServiceOperator(UpdateService serItem, int OperType)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                string strDetail = DateTime.Now.ToString() + " : ";
                string strOperType = string.Empty;
                if (OperType == 0)
                {
                    strOperType = App.GetLanguage("string80", "string80");
                }
                else
                {
                    strOperType = App.GetLanguage("string80", "string80");
                }
                strDetail += App.GetLanguage("string78", "string78");
                strDetail = string.Format(strDetail, strOperType, serItem.ServiceName);
                Dispatcher.Invoke(new Action(() =>
                    {
                        txtDetail.Text += strDetail + "\r\n";
                    }));

                App.WriteLog("Start to install service " + serItem.ServiceName);
                string strCommand = string.Empty;
                //获得服务文件路径
                optReturn = GetTargetPath(serItem.TargetPath, serItem.TargetPathType, App.dicAppInstalled[serItem.Package].AppInstallPath);
                if (!optReturn.Result)
                {
                    App.WriteLog("Error code : " + optReturn.Code + ". Get service file path failed. " + optReturn.Message);
                    return optReturn;
                }
                string strFilePath = optReturn.Data as string;
                //判断是C#服务 还是C++服务
                if (serItem.ServiceType == (int)ServiceType.CSharp)
                {
                    optReturn = RegistryOperator.GetInstallUtilInstallPath();
                    if (!optReturn.Result)
                    {
                        App.WriteLog("Error code : " + optReturn.Code + ". Get installutil path failed. " + optReturn.Message);
                        return optReturn;
                    }
                    string strInstallUtilPath = optReturn.Data as string;
                    strCommand = strInstallUtilPath + " \"" + strFilePath + "\"";
                    if (OperType == 0)
                    {
                        strCommand += " -i";
                    }
                    else
                    {
                        strCommand += " -u";
                    }
                }
                else if (serItem.ServiceType == (int)ServiceType.CPP)
                {
                    strCommand = strFilePath + " ";
                    if (OperType == 0)
                    {
                        strCommand += serItem.InstallCommand;
                    }
                    else
                    {
                        strCommand += serItem.UnInstallCommand;
                    }
                }
                optReturn = CommonFuncs.CmdOperator(strCommand);
                if (!optReturn.Result)
                {
                    string strLog = string.Empty;
                    if (OperType == 0)
                    {
                        strLog = "Install ";
                    }
                    else if (OperType == 1)
                    {
                        strLog = "Uninstall ";
                    }
                    App.WriteLog("Error code : " + optReturn.Code + " . " + strLog + "service " + serItem.ServiceName + " failed. " + optReturn.Message);
                    return optReturn;
                }
                else
                {
                    if (OperType == 0)
                    {
                        strOperType = App.GetLanguage("string82", "string82");
                    }
                    else
                    {
                        strOperType = App.GetLanguage("string83", "string83");
                    }
                    strDetail = DateTime.Now.ToString() + " : ";
                    strDetail += App.GetLanguage("string79", "string79");
                    strDetail = string.Format(strDetail, serItem.ServiceName, strOperType);
                    Dispatcher.Invoke(new Action(() =>
                    {
                        txtDetail.Text += strDetail + "\r\n";
                    }));
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Service_Operator_Exception;
                optReturn.Message = ex.Message;
                App.WriteLog("Error code : " + optReturn.Code + ". " + optReturn.Message);
                return optReturn;
            }
            return optReturn;
        }

        /// <summary>
        /// 拷贝文件夹（用于升级时）
        /// </summary>
        /// 拷贝文件夹时，如果文件夹存在 就保留原文件结构 增加或修改文件夹中的文件
        /// <param name="strFromPath"></param>
        /// <param name="strToPath"></param>
        /// <returns></returns>
        OperationReturn CopyFolderInUpgrade(string strFromPath, string strToPath)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                //如果源文件夹不存在，则返回错误
                if (!Directory.Exists(strFromPath))
                {
                    optReturn.Result = false;
                    optReturn.Code = ConstDefines.Copy_Res_Not_Found;
                    return optReturn;
                }
                //取得要拷贝的文件夹名
                string strFolderName = strFromPath.Substring(strFromPath.LastIndexOf("\\") +
                1, strFromPath.Length - strFromPath.LastIndexOf("\\") - 1);
                if (!Directory.Exists(strToPath + "\\" + strFolderName))
                {
                    //如果目标文件夹不存在 就创建
                    Directory.CreateDirectory(strToPath + "\\" + strFolderName);
                }
                //如果文件夹存在 更新其中的文件和子文件夹
                //创建数组保存源文件夹下的文件名
                string[] strFiles = Directory.GetFiles(strFromPath);
                //循环拷贝文件
                for (int i = 0; i < strFiles.Length; i++)
                {
                    //FileIsInUse(strFiles[i]);
                    //取得拷贝的文件名，只取文件名，地址截掉。
                    string strFileName = strFiles[i].Substring(strFiles[i].LastIndexOf("\\") + 1, strFiles[i].Length - strFiles[i].LastIndexOf("\\") - 1);
                    //FileIsInUse(strToPath + "\\" + strFolderName + "\\" + strFileName);
                    //开始拷贝文件,true表示覆盖同名文件
                    string strFilePath = strToPath + "\\" + strFolderName + "\\" + strFileName;
                    CommonFuncs.FileIsInUse(strFiles[i]);
                    if (File.Exists(strFilePath))
                    {
                        CommonFuncs.FileIsInUse(strFilePath);
                        if ((File.GetAttributes(strFilePath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                        {
                            // 如果是将文件的属性设置为Normal 
                            File.SetAttributes(strFilePath, FileAttributes.Normal);
                        }
                    }
                    optReturn = CopyFile(strFiles[i], strToPath + "\\" + strFolderName + "\\" + strFileName);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                }
                //创建DirectoryInfo实例
                DirectoryInfo dirInfo = new DirectoryInfo(strFromPath);
                //取得源文件夹下的所有子文件夹名称
                DirectoryInfo[] ZiPath = dirInfo.GetDirectories();
                for (int j = 0; j < ZiPath.Length; j++)
                {
                    //获取所有子文件夹名
                    string strZiPath = strFromPath + "\\" + ZiPath[j].ToString();
                    //把得到的子文件夹当成新的源文件夹，从头开始新一轮的拷贝
                    optReturn = CopyFolderInUpgrade(strZiPath, strToPath.TrimEnd('\\') + "\\" + strFolderName);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Copy_File_Exception;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 拷贝文件
        /// </summary>
        /// <param name="strSrcPath"></param>
        /// <param name="strTargetPath"></param>
        /// <returns></returns>
        private OperationReturn CopyFile(string strSrcPath, string strTargetPath)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Code = Defines.RET_SUCCESS;
            optReturn.Result = true;

            try
            {
                for (int iCopyCount = 0; iCopyCount < 3; iCopyCount++)
                {
                    try
                    {
                        File.Copy(strSrcPath, strTargetPath, true);
                        break;
                    }
                    catch (Exception ex)
                    {
                        System.Threading.Thread.Sleep(5000);
                        if (iCopyCount == 2)    //第三次失败
                        {
                            string strOldName = strTargetPath;
                            App.WriteLog("Copy file " + strOldName + " failed, try rename. " + ex.Message);
                            //尝试重命名
                            FileInfo info = new FileInfo(strOldName);
                            string strLastModifyTime = info.LastWriteTime.Year.ToString() + info.LastWriteTime.Month.ToString()
                                 + info.LastWriteTime.Day.ToString() + info.LastWriteTime.Hour.ToString()
                                  + info.LastWriteTime.Minute.ToString() + info.LastWriteTime.Second.ToString();
                            string strFileName = strTargetPath.Substring(strTargetPath.LastIndexOf('\\') + 1);
                            string strFilePath = strTargetPath.Substring(0, strTargetPath.LastIndexOf('\\'));
                            string strName = strFileName.Substring(0, strFileName.LastIndexOf("."));
                            string strFileType = strFileName.Substring(strFileName.LastIndexOf("."));
                            string strNewName = strFilePath + "\\" + strName + " " + strLastModifyTime + strFileType;

                            for (int iRenameCount = 0; iRenameCount < 3; iRenameCount++)
                            {
                                try
                                {
                                    File.Move(strOldName, strNewName);
                                    App.WriteLog("strOldName rename to " + strNewName);
                                    File.Copy(strSrcPath, strTargetPath, true);
                                    break;
                                }
                                catch (Exception exc)
                                {
                                    System.Threading.Thread.Sleep(5000);
                                    if (iRenameCount == 2)
                                    {
                                        App.WriteLog("Rename file " + strOldName + " failed. " + exc.Message);
                                        string strMsg = App.GetLanguage("string86", "string86");
                                        strMsg = string.Format(strMsg, strOldName);
                                        strDealingDir = strFilePath;
                                        App.WriteLog("strDealingDir = " + strDealingDir);
                                        Dispatcher.Invoke(new Action(() =>
                                        {
                                            txtMsg.Text = strMsg;
                                            popMsgBox.IsOpen = true;
                                        }));
                                        App.WriteLog(",Thread pause");
                                        strDealingDir = string.Empty;
                                        //如果还删不掉 弹出messagebox后就阻塞，等着返回结果
                                        manualReset.Reset();
                                        manualReset.WaitOne();
                                        App.WriteLog("Thread continue");
                                        if (iChoose == 0)
                                        {
                                            optReturn.Result = false;
                                            optReturn.Message = ex.Message;
                                            return optReturn;
                                        }
                                        else if (iChoose == 1)
                                        {
                                            //重试时 继续copy一次 再出错 就不管啦
                                            File.Copy(strSrcPath, strTargetPath, true);
                                        }
                                        iChoose = -1;
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Message = ex.Message;
                optReturn.Code = ConstDefines.Copy_File_Exception;
            }

            return optReturn;
        }

        /// <summary>
        /// 重命名文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="strNewName"></param>
        /// <returns></returns>
        OperationReturn RenameFile(UpdateFile file, string strNewName)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            string strOldPath = App.gStrUpdateFolderTempPath.Trim('\\') + "\\" + file.SourcePath + "\\" + file.FileName;
            string strNewPath = App.gStrUpdateFolderTempPath.Trim('\\') + "\\" + file.SourcePath + "\\" + strNewName;
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    File.Move(strOldPath, strNewPath);
                    break;
                }
                catch (Exception ex)
                {
                    System.Threading.Thread.Sleep(3000);
                    if (i == 2)
                    {
                        App.WriteLog("Rname " + strOldPath + " to " + strNewName + " failed, " + ex.Message);
                        for (int j = 0; j < 3; j++)
                        {
                            try
                            {
                                //如果直接改名字出错 就复制一个 再改名字
                                File.Copy(strOldPath, strNewPath);
                                break;
                            }
                            catch (Exception exc)
                            {
                                if (j == 2)
                                {
                                    optReturn.Result = false;
                                    optReturn.Code = ConstDefines.Rename_File_Exception;
                                    optReturn.Message = exc.Message;
                                }
                            }
                        }
                    }
                }
            }
            return optReturn;
        }
        #endregion
    }
}
