using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml;
using Microsoft.Win32;
using UMPUpdater.Models;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.SharpZips.Zip;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;
using VoiceCyber.UMP.Updates;

namespace UMPUpdater
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        #region Members

        private ObservableCollection<UpdateModuleItem> mListModuleItems;
        private List<ServiceInfo> mListServiceInfos;
        private bool mIsInited;
        private UpdateInfo mUpdateInfo;
        private InstallState mInstallState;
        private DatabaseInfo mDataBaseInfo;
        private InstallInfo mInstallInfo;
        private string mCurrentVersion;
        private string mUMPInstallPath;
        private bool mIsContinue;
        private bool mIsUMPInstalled;

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            mListModuleItems = new ObservableCollection<UpdateModuleItem>();
            mListServiceInfos = new List<ServiceInfo>();

            Loaded += MainWindow_Loaded;
            BtnTest.Click += BtnTest_Click;
            BtnStart.Click += BtnStart_Click;
            BtnStop.Click += BtnStop_Click;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
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
                LvModuleList.ItemsSource = mListModuleItems;
                InitServiceInfos();
                ReadUpdateInfo();
                InitDatabaseInfo();
                ReadInstallInfo();
                InitEnvironmentInformation();
                ShowInfomations();
                CreateUpdateModuleItems();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitEnvironmentInformation()
        {
            try
            {
                mIsUMPInstalled = false;
                mUMPInstallPath = string.Empty;
                RegistryKey key = Registry.LocalMachine;
                string strSubKey = string.Empty;
                //如果是64位系统
                if (Environment.Is64BitOperatingSystem)
                {
                    strSubKey = string.Format("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{0}", UpdateConsts.PACKAGE_GUID_UMP);
                }
                else
                {
                    strSubKey = string.Format("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\{0}", UpdateConsts.PACKAGE_GUID_UMP);
                }
                RegistryKey keyAppInfo = key.OpenSubKey(strSubKey);
                if (keyAppInfo == null)
                {
                    return;
                }
                mUMPInstallPath = keyAppInfo.GetValue("InstallLocation").ToString();
                if (mInstallInfo != null)
                {
                    mCurrentVersion = mInstallInfo.Version;
                }
                else
                {
                    mCurrentVersion = "8.03.001P00.000";
                }
                mIsUMPInstalled = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitDatabaseInfo()
        {
            try
            {
                mDataBaseInfo = null;
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP.Server\\Args01.UMP.xml");
                if (!File.Exists(path))
                {
                    AppendMessage(string.Format("Args01.UMP.xml not exist.\t{0}", path));
                    return;
                }
                DatabaseInfo dbInfo = new DatabaseInfo();
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode node = doc.SelectSingleNode("DatabaseParameters");
                if (node == null)
                {
                    AppendMessage(string.Format("DatabaseParameters node not exist"));
                    return;
                }
                string strValue;
                int intValue;
                bool isSetted = false;
                XmlNodeList listNodes = node.ChildNodes;
                for (int i = 0; i < listNodes.Count; i++)
                {
                    XmlNode temp = listNodes[i];
                    if (temp.Attributes != null)
                    {
                        var isEnableAttr = temp.Attributes["P03"];
                        if (isEnableAttr != null)
                        {
                            strValue = isEnableAttr.Value;
                            strValue = DecryptStringM004(strValue);
                            if (strValue != "1") { continue; }
                        }
                        var attr = temp.Attributes["P02"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptStringM004(strValue);
                            if (int.TryParse(strValue, out intValue))
                            {
                                dbInfo.TypeID = intValue;
                            }
                        }
                        attr = temp.Attributes["P04"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptStringM004(strValue);
                            dbInfo.Host = strValue;
                        }
                        attr = temp.Attributes["P05"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptStringM004(strValue);
                            if (int.TryParse(strValue, out intValue))
                            {
                                dbInfo.Port = intValue;
                            }
                        }
                        attr = temp.Attributes["P06"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptStringM004(strValue);
                            dbInfo.DBName = strValue;
                        }
                        attr = temp.Attributes["P07"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptStringM004(strValue);
                            dbInfo.LoginName = strValue;
                        }
                        attr = temp.Attributes["P08"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptStringM004(strValue);
                            dbInfo.Password = strValue;
                        }
                        isSetted = true;
                        break;
                    }
                }
                if (!isSetted)
                {
                    AppendMessage(string.Format("Database not setted."));
                    return;
                }
                mDataBaseInfo = dbInfo;

                AppendMessage(string.Format("Read database info end.\t{0}", mDataBaseInfo));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ShowInfomations()
        {
            try
            {
                if (!mIsUMPInstalled)
                {
                    TxtCurrentVersion.Text = "Not installed";
                    TxtUMPInstallPath.Text = string.Empty;
                    TxtDBInfo.Text = "Not created";
                }
                else
                {
                    TxtCurrentVersion.Text = mCurrentVersion;
                    TxtUMPInstallPath.Text = mUMPInstallPath;
                    if (mDataBaseInfo == null)
                    {
                        TxtDBInfo.Text = "Not exist";
                    }
                    else
                    {
                        TxtDBInfo.Text = mDataBaseInfo.ToString();
                    }
                }
                if (mUpdateInfo != null)
                {
                    TxtUpdateVersion.Text = mUpdateInfo.Version;
                    TxtPublishDate.Text = mUpdateInfo.PublishDate;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitServiceInfos()
        {
            try
            {
                mListServiceInfos.Clear();
                ServiceInfo serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService00";
                serviceInfo.ServiceName = "UMP Service 00";
                serviceInfo.DisplayName = "UMP Service 00";
                serviceInfo.FileName = "UMPService00.exe";
                serviceInfo.ProcessName = "UMPService00";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService01";
                serviceInfo.ServiceName = "UMP Service 01";
                serviceInfo.DisplayName = "UMP Service 01";
                serviceInfo.FileName = "UMPService01.exe";
                serviceInfo.ProcessName = "UMPService01";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService02";
                serviceInfo.ServiceName = "UMP Service 02";
                serviceInfo.DisplayName = "UMP Service 02";
                serviceInfo.FileName = "UMPService02.exe";
                serviceInfo.ProcessName = "UMPService02";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService03";
                serviceInfo.ServiceName = "UMP Service 03";
                serviceInfo.DisplayName = "UMP Service 03";
                serviceInfo.FileName = "UMPService03.exe";
                serviceInfo.ProcessName = "UMPService03";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService04";
                serviceInfo.ServiceName = "UMP Service 04";
                serviceInfo.DisplayName = "UMP Service 04";
                serviceInfo.FileName = "UMPService04.exe";
                serviceInfo.ProcessName = "UMPService04";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService05";
                serviceInfo.ServiceName = "UMP Service 05";
                serviceInfo.DisplayName = "UMP Service 05";
                serviceInfo.FileName = "UMPService05.exe";
                serviceInfo.ProcessName = "UMPService05";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService06";
                serviceInfo.ServiceName = "UMP Service 06";
                serviceInfo.DisplayName = "UMP Service 06";
                serviceInfo.FileName = "UMPService06.exe";
                serviceInfo.ProcessName = "UMPService06";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService07";
                serviceInfo.ServiceName = "UMP Service 07";
                serviceInfo.DisplayName = "UMP Service 07";
                serviceInfo.FileName = "UMPService07.exe";
                serviceInfo.ProcessName = "UMPService07";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService08";
                serviceInfo.ServiceName = "UMP Service 08";
                serviceInfo.DisplayName = "UMP Service 08";
                serviceInfo.FileName = "UMPService08.exe";
                serviceInfo.ProcessName = "UMPService08";
                mListServiceInfos.Add(serviceInfo);
                serviceInfo = new ServiceInfo();
                serviceInfo.Name = "UMPService09";
                serviceInfo.ServiceName = "UMP Service 09";
                serviceInfo.DisplayName = "UMP Service 09";
                serviceInfo.FileName = "UMPService09.exe";
                serviceInfo.ProcessName = "UMPService09";
                mListServiceInfos.Add(serviceInfo);
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("InitServiceInfos fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Operations

        private void ReadUpdateInfo()
        {
            try
            {
                var rsUpdateInfo =
                    App.GetResourceStream(new Uri("/UMPUpdater;component/UpdateInfo.xml", UriKind.RelativeOrAbsolute));
                if (rsUpdateInfo == null)
                {
                    ShowException(string.Format("UpdateInfo.xml not exist."));
                    return;
                }
                var stream = rsUpdateInfo.Stream;
                if (stream == null)
                {
                    ShowException(string.Format("UpdateInfo.xml not exist."));
                    return;
                }
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string strContent = reader.ReadToEnd();
                OperationReturn optReturn = XMLHelper.DeserializeObject<UpdateInfo>(strContent);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                UpdateInfo updateInfo = optReturn.Data as UpdateInfo;
                if (updateInfo == null)
                {
                    ShowException(string.Format("Fail.\t UpdateInfo is null"));
                    return;
                }
                mUpdateInfo = updateInfo;

                AppendMessage(string.Format("Read UpdateInfo end.\t{0}", mUpdateInfo.Name));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ReadInstallInfo()
        {
            try
            {
                string strPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "VoiceCyber\\UMP",
                    InstallInfo.FILE_NAME);
                if (!File.Exists(strPath))
                {
                    AppendMessage(string.Format("Read InstallInfo fail.\tFile not exist.\t{0}", strPath));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<InstallInfo>(strPath);
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Read InstallInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                InstallInfo installInfo = optReturn.Data as InstallInfo;
                if (installInfo == null)
                {
                    AppendMessage(string.Format("Read InstallInfo fail.\tInstallInfo is null"));
                    return;
                }
                mInstallInfo = installInfo;
                AppendMessage(string.Format("Read InstallInfo end.\t{0}", mInstallInfo.Version));
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
                if (!mIsUMPInstalled)
                {
                    ShowException(string.Format("UMP not installed."));
                    return;
                }
                if (mUpdateInfo == null)
                {
                    return;
                }
                if (mInstallState == null)
                {
                    mInstallState = new InstallState();
                }
                mInstallState.CurrentVersion = mCurrentVersion;
                mInstallState.UpdateVersion = mUpdateInfo.Version;
                mInstallState.UMPInstallPath = mUMPInstallPath;
                string tempPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "UMP\\UMPUpdater\\Temp");
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }
                mInstallState.TempPath = tempPath;
                mInstallState.IsOptFail = false;

                if (mInstallInfo == null)
                {
                    mInstallInfo = new InstallInfo();
                }
                mInstallInfo.SessionID = Guid.NewGuid().ToString();
                mInstallInfo.InstallTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                mInstallInfo.BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                mInstallInfo.Version = mUpdateInfo.Version;
                mInstallInfo.Type = mUpdateInfo.Type;
                mInstallInfo.MachineName = Environment.MachineName;
                mInstallInfo.OSVersion = Environment.OSVersion.ToString();
                mInstallInfo.OSAccount = Environment.UserName;
                var product = mInstallInfo.ListProducts.FirstOrDefault(p => p.Package == UpdateConsts.PACKAGE_NAME_UMP);
                if (product == null)
                {
                    product = new InstallProduct();
                    product.Package = UpdateConsts.PACKAGE_NAME_UMP;
                    mInstallInfo.ListProducts.Add(product);
                }
                product.Version = mInstallState.UpdateVersion;
                product.InstallPath = mInstallState.UMPInstallPath;
                var component =
                    product.ListComponents.FirstOrDefault(c => c.ModuleName == UpdateConsts.COMPONENT_NAME_UMP);
                if (component == null)
                {
                    component = new InstallComponent();
                    component.ModuleID = 0;
                    component.ModuleName = UpdateConsts.COMPONENT_NAME_UMP;
                    product.ListComponents.Add(component);
                }

                //DatabaseInfo dbInfo = new DatabaseInfo();
                //dbInfo.TypeID = 2;
                //dbInfo.Host = "192.168.4.182";
                //dbInfo.Port = 1433;
                //dbInfo.DBName = "UMPDataDB0516";
                //dbInfo.LoginName = "PFDEV";
                //dbInfo.Password = "PF,123";
                //mDataBaseInfo = dbInfo;

                if (mDataBaseInfo != null)
                {
                    mInstallState.DBType = mDataBaseInfo.TypeID;
                    mInstallState.DBConnectionString = mDataBaseInfo.GetConnectionString();
                }

                mIsContinue = true;
                SetProgress(true, 0);
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    StopServices();
                    ExtractUMPData();
                    CopyFiles();
                    UpdateDatabase();
                    UpdateLanguages();
                    StartServices();
                    UpdateOthers();
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();
                    //SetProgress(false, 0);

                    try
                    {
                        mInstallInfo.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        mInstallInfo.UpdateInfo = mUpdateInfo;
                        SaveInstallInfo();
                        if (mInstallState.IsOptFail)
                        {
                            AppendMessage(string.Format("Update fail.\t{0}", mInstallState.ErrorMessage));
                        }
                        else
                        {
                            AppendMessage(string.Format("Update UMP end."));
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void StopServices()
        {
            try
            {
                if (mInstallState.IsOptFail) { return; }
                int count = mUpdateInfo.ListServices.Count;
                AppendMessage(string.Format("Stoping services..."));
                ServiceController[] services = ServiceController.GetServices();
                for (int i = 0; i < count; i++)
                {
                    if (!mIsContinue) { return; }
                    UpdateService updateService = mUpdateInfo.ListServices[i];
                    if (updateService.Package != UpdateConsts.PACKAGE_NAME_UMP) { continue; }
                    string strServiceName = updateService.ServiceName;
                    AppendMessage(string.Format("Stoping service {0} ...", strServiceName));
                    ServiceController controller = services.FirstOrDefault(s => s.ServiceName == strServiceName);
                    if (controller == null)
                    {
                        AppendMessage(string.Format("Service not exist.\t{0}", strServiceName));
                        continue;
                    }
                    try
                    {
                        controller.Stop();
                        controller.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 60));
                        AppendMessage(string.Format("Service stopped.\t{0}", strServiceName));
                    }
                    catch (Exception ex)
                    {
                        AppendMessage(string.Format("Stop service fail.\t{0}", ex.Message));
                    }
                    //等待5s，检查进程是否仍在运行，如果任在运行，杀死进程
                    Thread.Sleep(5 * 1000);
                    KillServiceProcess(strServiceName);

                    double percentage = i / (count * 1.0);
                    percentage = percentage * PROGRESS_STOPSERVICE;
                    percentage = percentage + PROGRESS_BASE_STOPSERVICE;
                    SetProgress(percentage);
                }

                SetProgress(PROGRESS_BASE_STOPSERVICE + PROGRESS_STOPSERVICE);
                AppendMessage(string.Format("Stop services end."));
            }
            catch (Exception ex)
            {
                mInstallState.IsOptFail = true;
                mInstallState.ErrorMessage = ex.Message;
            }
        }

        private void KillServiceProcess(string serviceName)
        {
            try
            {
                var serviceInfo = mListServiceInfos.FirstOrDefault(s => s.ServiceName == serviceName);
                if (serviceInfo == null)
                {
                    AppendMessage(string.Format("ServiceInfo not exist.\t{0}", serviceName));
                    return;
                }
                string processName = serviceInfo.ProcessName;
                Process[] processes = Process.GetProcesses();
                for (int i = 0; i < processes.Length; i++)
                {
                    Process process = processes[i];
                    if (!process.ProcessName.Equals(processName)) { continue; }
                    process.Kill();
                    AppendMessage(string.Format("Kill process end.\t{0}", processName));
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("KillServiceProcess fail.\t{0}", ex.Message));
            }
        }

        private void ExtractUMPData()
        {
            try
            {
                if (mInstallState.IsOptFail) { return; }
                var rsUMPData = App.GetResourceStream(new Uri("/UMPUpdater;component/Resources/UMPData.zip", UriKind.RelativeOrAbsolute));
                if (rsUMPData == null)
                {
                    mInstallState.IsOptFail = true;
                    mInstallState.ErrorMessage = string.Format("UMPData.zip not exist.");
                    return;
                }
                var stream = rsUMPData.Stream;
                if (stream == null)
                {
                    mInstallState.IsOptFail = true;
                    mInstallState.ErrorMessage = string.Format("UMPData.zip not exist.");
                    return;
                }
                string strExtractPath = Path.Combine(mInstallState.TempPath, "UMPData");
                if (!Directory.Exists(strExtractPath))
                {
                    Directory.CreateDirectory(strExtractPath);
                }
                AppendMessage(string.Format("Extracting files..."));
                using (var zipStream = new ZipInputStream(stream))
                {
                    long totalSize = stream.Length;
                    long zipSize = 0;
                    ZipEntry theEntry;
                    while ((theEntry = zipStream.GetNextEntry()) != null)
                    {
                        if (!mIsContinue) { return; }
                        string dirName = strExtractPath;
                        string pathToZip = theEntry.Name;
                        if (!string.IsNullOrEmpty(pathToZip))
                        {
                            dirName = Path.GetDirectoryName(Path.Combine(dirName, pathToZip));
                        }
                        DateTime datetime = theEntry.DateTime;
                        if (string.IsNullOrEmpty(dirName)) { continue; }
                        string fileName = Path.GetFileName(pathToZip);
                        Directory.CreateDirectory(dirName);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            string filePath = Path.Combine(dirName, fileName);
                            AppendMessage(string.Format("Extracting file {0}", fileName));
                            using (FileStream streamWriter = File.Create(filePath))
                            {
                                int size;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = zipStream.Read(data, 0, data.Length);
                                    zipSize = zipStream.Position;
                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();

                                //还原文件的修改时间
                                FileInfo fileInfo = new FileInfo(filePath);
                                fileInfo.LastWriteTime = datetime;
                            }
                        }

                        double percentage = zipSize / (totalSize * 1.0);
                        percentage = percentage * PROGRESS_EXTRACTFILE;
                        percentage = percentage + PROGRESS_BASE_EXTRACTFILE;
                        SetProgress(percentage);

                    }
                }

                SetProgress(PROGRESS_BASE_EXTRACTFILE + PROGRESS_EXTRACTFILE);
                AppendMessage(string.Format("Extract UMPData end."));
            }
            catch (Exception ex)
            {
                mInstallState.IsOptFail = true;
                mInstallState.ErrorMessage = ex.Message;
            }
        }

        private void CopyFiles()
        {
            try
            {
                if (mInstallState.IsOptFail) { return; }
                int count = mUpdateInfo.ListFiles.Count;
                AppendMessage(string.Format("Copying files..."));
                for (int i = 0; i < count; i++)
                {
                    if (!mIsContinue) { return; }
                    UpdateFile updateFile = mUpdateInfo.ListFiles[i];
                    updateFile.ErrCode = 0;
                    updateFile.ErrMsg = string.Empty;
                    if (updateFile.Package != UpdateConsts.PACKAGE_NAME_UMP) { continue; }
                    string strTargetPath = updateFile.TargetPath;
                    string strInstallPath = mInstallState.UMPInstallPath;
                    string strSourcePath = Path.Combine(mInstallState.TempPath, "UMPData", updateFile.SourcePath, updateFile.FileName);
                    strTargetPath = Path.Combine(strInstallPath, strTargetPath, updateFile.FileName);
                    try
                    {
                        if (updateFile.Type == (int)UpdateFileType.Directory)
                        {
                            CopyDirectory(updateFile, strSourcePath, strTargetPath);
                        }
                        else
                        {
                            CopySingleFile(updateFile, strSourcePath, strTargetPath);
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendMessage(string.Format("Copy file fail.\t{0}\t{1}", strSourcePath, ex.Message));
                        updateFile.Result = 1;
                        updateFile.ErrCode = Defines.RET_FAIL;
                        updateFile.ErrMsg += string.Format("{0}", ex.Message);
                    }

                    double percentage = i / (count * 1.0);
                    percentage = percentage * PROGRESS_COPYFILE;
                    percentage = percentage + PROGRESS_BASE_COPYFILE;
                    SetProgress(percentage);
                }

                SetProgress(PROGRESS_BASE_COPYFILE + PROGRESS_COPYFILE);
                AppendMessage(string.Format("Copy files end."));
            }
            catch (Exception ex)
            {
                mInstallState.IsOptFail = true;
                mInstallState.ErrorMessage = ex.Message;
            }
        }

        private void CopyDirectory(UpdateFile updateFile, string strSourcePath, string strTargetPath)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(strSourcePath);
            if (!Directory.Exists(strTargetPath))
            {
                Directory.CreateDirectory(strTargetPath);
            }
            var dirs = dirInfo.GetDirectories();
            for (int i = 0; i < dirs.Length; i++)
            {
                if (!mIsContinue) { return; }
                string source = Path.Combine(strSourcePath, dirs[i].Name);
                string target = Path.Combine(strTargetPath, dirs[i].Name);
                CopyDirectory(updateFile, source, target);
            }
            var files = dirInfo.GetFiles();
            for (int i = 0; i < files.Length; i++)
            {
                if (!mIsContinue) { return; }
                string source = Path.Combine(strSourcePath, files[i].Name);
                string target = Path.Combine(strTargetPath, files[i].Name);
                CopySingleFile(updateFile, source, target);
            }
        }

        private void CopySingleFile(UpdateFile updateFile, string strSourcePath, string strTargetPath)
        {
            if (!File.Exists(strSourcePath))
            {
                AppendMessage(string.Format("Source file not exist.\t{0}", strSourcePath));
                updateFile.Result = 1;
                updateFile.ErrCode = Defines.RET_FILE_NOT_EXIST;
                updateFile.ErrMsg += string.Format("Source file not exist.\t{0}", strSourcePath);
            }
            else
            {
                AppendMessage(string.Format("Copying file \t{0}", strSourcePath));
                File.Copy(strSourcePath, strTargetPath, true);
                AppendMessage(string.Format("Copy file end.\t{0}", strTargetPath));
            }
        }

        private void UpdateDatabase()
        {
            try
            {
                if (mInstallState.IsOptFail) { return; }
                if (mDataBaseInfo == null)
                {
                    AppendMessage(string.Format("Database not setted. can not update database"));
                    return;
                }
                int count = mUpdateInfo.ListSqlScripts.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!mIsContinue) { return; }
                    var sqlScript = mUpdateInfo.ListSqlScripts[i];
                    sqlScript.ErrCode = 0;
                    sqlScript.ErrMsg = string.Empty;
                    if (sqlScript.SqlType == (int)UpdateSqlType.File)
                    {
                        UpdateSqlScriptFile(sqlScript);
                    }

                    double percentage = i / (count * 1.0);
                    percentage = percentage * PROGRESS_UPDATEDATABASE;
                    percentage = percentage + PROGRESS_BASE_UPDATEDATABASE;
                    SetProgress(percentage);
                }

                SetProgress(PROGRESS_UPDATEDATABASE + PROGRESS_BASE_UPDATEDATABASE);
                AppendMessage(string.Format("Update database end."));
            }
            catch (Exception ex)
            {
                mInstallState.IsOptFail = true;
                mInstallState.ErrorMessage = ex.Message;
            }
        }

        private void UpdateSqlScriptFile(UpdateSqlScript sqlScript)
        {
            int dbType = mInstallState.DBType;
            var script = sqlScript.ListScripts.FirstOrDefault(s => s.DBType == dbType);
            if (script == null)
            {
                AppendMessage(string.Format("Script not exist.\t{0}", dbType));
                sqlScript.Result = 1;
                sqlScript.ErrCode = Defines.RET_NOT_EXIST;
                sqlScript.ErrMsg += string.Format("Script not exist.\t{0}", dbType);
                return;
            }
            string strFile = script.Path;
            strFile = Path.Combine(mInstallState.TempPath, "UMPData", "SqlScripts", dbType == 2 ? "MSSQL" : "ORCL",
                strFile);
            if (!File.Exists(strFile))
            {
                AppendMessage(string.Format("ScriptFile not exist.\t{0}", strFile));
                sqlScript.Result = 1;
                sqlScript.ErrCode = Defines.RET_FILE_NOT_EXIST;
                sqlScript.ErrMsg += string.Format("ScriptFile not exist.\t{0}", strFile);
                return;
            }
            List<string> listSqls = new List<string>();
            if (sqlScript.ScriptType == (int)UpdateScriptType.Procedure)
            {
                string strContent = File.ReadAllText(strFile, Encoding.UTF8);
                listSqls.Add(strContent);
            }
            else
            {
                ParseSqlScriptFromFile(strFile, listSqls);
            }
            switch (mInstallState.DBType)
            {
                case 2:
                    IDbConnection sqlConn = MssqlOperation.GetConnection(mInstallState.DBConnectionString);
                    sqlConn.Open();
                    IDbCommand sqlCommand = MssqlOperation.GetCommand();
                    sqlCommand.Connection = sqlConn;
                    for (int i = 0; i < listSqls.Count; i++)
                    {
                        string strSql = listSqls[i];
                        sqlCommand.CommandText = strSql;
                        try
                        {
                            sqlCommand.ExecuteNonQuery();

                            AppendMessage(string.Format("Execute end.\t{0}\t{1}", sqlScript.Name, i));
                        }
                        catch (Exception ex)
                        {
                            AppendMessage(string.Format("Fail.\t{0}\t{1}\t{2}", sqlScript.Name, i, ex.Message));
                            sqlScript.Result = 1;
                            sqlScript.ErrCode = Defines.RET_FAIL;
                            sqlScript.ErrMsg += string.Format("Fail.\t{0}\t{1}\t{2}", sqlScript.Name, i, ex.Message);
                        }
                    }
                    if (sqlConn.State != ConnectionState.Closed)
                    {
                        sqlConn.Close();
                    }
                    sqlConn.Dispose();
                    break;
                case 3:
                    IDbConnection orclConn = MssqlOperation.GetConnection(mInstallState.DBConnectionString);
                    orclConn.Open();
                    IDbCommand orclCommand = MssqlOperation.GetCommand();
                    orclCommand.Connection = orclConn;
                    for (int i = 0; i < listSqls.Count; i++)
                    {
                        string strSql = listSqls[i];
                        orclCommand.CommandText = strSql;
                        try
                        {
                            orclCommand.ExecuteNonQuery();

                            AppendMessage(string.Format("Execute end.\t{0}\t{1}", sqlScript.Name, i));
                        }
                        catch (Exception ex)
                        {
                            AppendMessage(string.Format("Fail.\t{0}\t{1}\t{2}", sqlScript.Name, i, ex.Message));
                            sqlScript.Result = 1;
                            sqlScript.ErrCode = Defines.RET_FAIL;
                            sqlScript.ErrMsg += string.Format("Fail.\t{0}\t{1}\t{2}", sqlScript.Name, i, ex.Message);
                        }
                    }
                    if (orclConn.State != ConnectionState.Closed)
                    {
                        orclConn.Close();
                    }
                    orclConn.Dispose();
                    break;
                default:
                    AppendMessage(string.Format("DatabaseType invalid.\t{0}", mInstallState.DBType));
                    sqlScript.Result = 1;
                    sqlScript.ErrCode = Defines.RET_FAIL;
                    sqlScript.ErrMsg += string.Format("Fail.\t{0}\tDatabaseType invalid. ", sqlScript.Name);
                    return;
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
            try
            {
                if (mInstallState.IsOptFail) { return; }
                if (mDataBaseInfo == null)
                {
                    AppendMessage(string.Format("Database not setted. can not update languages"));
                    return;
                }
                OperationReturn optReturn;
                int count = mUpdateInfo.ListLangs.Count;
                AppendMessage(string.Format("Updating langugae..."));
                string strConn = mInstallState.DBConnectionString;
                int dbType = mInstallState.DBType;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                for (int i = 0; i < count; i++)
                {
                    if (!mIsContinue) { return; }
                    UpdateLang updateLang = mUpdateInfo.ListLangs[i];
                    updateLang.ErrCode = 0;
                    updateLang.ErrMsg = string.Empty;
                    string strPath = Path.Combine(mInstallState.TempPath, "UMPData\\Langs", updateLang.Path);
                    if (!File.Exists(strPath))
                    {
                        AppendMessage(string.Format("Fail.\t Language file not exist.\t{0}", strPath));
                        updateLang.Result = 1;
                        updateLang.ErrCode = Defines.RET_FILE_NOT_EXIST;
                        updateLang.ErrMsg += string.Format("Fail.\t Language file not exist.\t{0}", strPath);
                        continue;
                    }
                    optReturn = XMLHelper.DeserializeFile<LangLister>(strPath);
                    if (!optReturn.Result)
                    {
                        AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        updateLang.Result = 1;
                        updateLang.ErrCode = optReturn.Code;
                        updateLang.ErrMsg += string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message);
                        continue;
                    }
                    LangLister lister = optReturn.Data as LangLister;
                    if (lister == null)
                    {
                        AppendMessage(string.Format("Fail.\t LangLister is null"));
                        updateLang.Result = 1;
                        updateLang.ErrCode = Defines.RET_OBJECT_NULL;
                        updateLang.ErrMsg += string.Format("Fail.\t LangLister is null");
                        continue;
                    }
                    int langTypeID = updateLang.LangID;
                    AppendMessage(string.Format("Updating Language {0} ...", langTypeID));
                    string strSql;
                    switch (dbType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_00_005 WHERE C001 = {0}", langTypeID);
                            objConn = MssqlOperation.GetConnection(strConn);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_00_005 WHERE C001 = {0}", langTypeID);
                            objConn = OracleOperation.GetConnection(strConn);
                            objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                            break;
                        default:
                            AppendMessage(string.Format("Fail.\t DBType invalid"));
                            return;
                    }
                    if (objConn == null || objAdapter == null || objCmdBuilder == null)
                    {
                        AppendMessage(string.Format("Fail.\t DB object is null"));
                        updateLang.Result = 1;
                        updateLang.ErrCode = Defines.RET_OBJECT_NULL;
                        updateLang.ErrMsg += string.Format("Fail.\t DB object is null");
                        continue;
                    }
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    try
                    {
                        DataSet objDataSet = new DataSet();
                        objAdapter.Fill(objDataSet);

                        int num = 0;
                        for (int j = 0; j < lister.ListLangInfos.Count; j++)
                        {
                            LanguageInfo langInfo = lister.ListLangInfos[j];
                            string langName = langInfo.Name;

                            DataRow dr =
                                objDataSet.Tables[0].Select(string.Format("C002 = '{0}'", langName)).FirstOrDefault();
                            if (dr == null)
                            {
                                dr = objDataSet.Tables[0].NewRow();
                                dr["C001"] = langTypeID;
                                dr["C002"] = langName;
                                dr["C003"] = 0;
                                dr["C004"] = 0;
                                dr["C005"] = langInfo.Display;
                                dr["C009"] = langInfo.Module;
                                dr["C010"] = langInfo.SubModule;
                                dr["C011"] = langInfo.Page;

                                objDataSet.Tables[0].Rows.Add(dr);
                                num++;
                            }
                        }

                        objAdapter.Update(objDataSet);
                        objDataSet.AcceptChanges();

                        AppendMessage(string.Format("Insert langugage.\tLangID:{0};Num:{1}", langTypeID, num));
                    }
                    catch (Exception ex)
                    {
                        AppendMessage(string.Format("Fail.\t{0}", ex.Message));
                        updateLang.Result = 1;
                        updateLang.ErrCode = Defines.RET_FAIL;
                        updateLang.ErrMsg += string.Format("Fail.\t{0}", ex.Message);
                        continue;
                    }
                    finally
                    {
                        if (objConn.State == ConnectionState.Open)
                        {
                            objConn.Close();
                        }
                        objConn.Dispose();
                    }

                    AppendMessage(string.Format("Update language end.\t{0}", langTypeID));

                    double percentage = i / (count * 1.0);
                    percentage = percentage * PROGRESS_UPDATELANGUGAGE;
                    percentage = percentage + PROGRESS_BASE_UPDATELANGUAGE;
                    SetProgress(percentage);
                }

                SetProgress(PROGRESS_BASE_UPDATELANGUAGE + PROGRESS_UPDATELANGUGAGE);

                AppendMessage(string.Format("Update language end."));
            }
            catch (Exception ex)
            {
                mInstallState.IsOptFail = true;
                mInstallState.ErrorMessage = ex.Message;
            }
        }

        private void StartServices()
        {
            try
            {
                if (mInstallState.IsOptFail) { return; }
                int count = mUpdateInfo.ListServices.Count;
                AppendMessage(string.Format("Starting services..."));
                ServiceController[] services = ServiceController.GetServices();
                for (int i = 0; i < count; i++)
                {
                    if (!mIsContinue) { return; }
                    UpdateService updateService = mUpdateInfo.ListServices[i];
                    if (updateService.Package != UpdateConsts.PACKAGE_NAME_UMP) { continue; }
                    string strServiceName = updateService.ServiceName;
                    AppendMessage(string.Format("Starting service {0} ...", strServiceName));
                    ServiceController controller = services.FirstOrDefault(s => s.ServiceName == strServiceName);
                    if (controller == null)
                    {
                        AppendMessage(string.Format("Service not exist.\t{0}", strServiceName));
                        continue;
                    }
                    try
                    {
                        controller.Start();
                        controller.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 60));
                        AppendMessage(string.Format("Service started.\t{0}", strServiceName));
                    }
                    catch (Exception ex)
                    {
                        AppendMessage(string.Format("start service fail.\t{0}", ex.Message));
                    }

                    double percentage = i / (count * 1.0);
                    percentage = percentage * PROGRESS_STARTSERVICE;
                    percentage = percentage + PROGRESS_BASE_STARTSERVICE;
                    SetProgress(percentage);
                }

                SetProgress(PROGRESS_BASE_STARTSERVICE + PROGRESS_STARTSERVICE);
                AppendMessage(string.Format("start services end."));
            }
            catch (Exception ex)
            {
                mInstallState.IsOptFail = true;
                mInstallState.ErrorMessage = ex.Message;
            }
        }

        private void UpdateOthers()
        {
            try
            {
                if (mInstallState.IsOptFail) { return; }
                int count = mUpdateInfo.ListFollows.Count;
                AppendMessage(string.Format("Updating Others..."));
                for (int i = 0; i < count; i++)
                {
                    if (!mIsContinue) { return; }
                    UpdateFollow updateFollow = mUpdateInfo.ListFollows[i];
                    updateFollow.ErrCode = 0;
                    updateFollow.ErrMsg = string.Empty;
                    string strName = updateFollow.Name;
                    int intKey = updateFollow.Key;
                    AppendMessage(string.Format("Updating {0}...", strName));
                    switch (intKey)
                    {
                        case UpdateConsts.FOLLOW_KEY_BINDSITE:
                            if (updateFollow.Value == "1")
                            {
                                LaunchMAMT();
                            }
                            break;
                    }
                }

                SetProgress(PROGRESS_BASE_OTHERS + PROGRESS_OTHERS);
                AppendMessage(string.Format("Update others end."));
            }
            catch (Exception ex)
            {
                mInstallState.IsOptFail = true;
                mInstallState.ErrorMessage = ex.Message;
            }
        }

        private void LaunchMAMT()
        {
            try
            {
                AppendMessage(string.Format("Launching MAMT..."));
                string strPath = Path.Combine(mInstallState.UMPInstallPath, "ManagementMaintenance");
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                string strExe = Path.Combine(strPath, "UMP.MAMT.exe");
                if (!File.Exists(strExe))
                {
                    AppendMessage(string.Format("File not exist.\t{0}", strExe));
                    return;
                }
                Process process = new Process();
                process.StartInfo.FileName = strExe;
                process.StartInfo.WorkingDirectory = strPath;
                process.Start();
                process.WaitForExit();
                process.Dispose();
                AppendMessage(string.Format("MAMT launch end."));
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("LaunchMAMT fail.\t{0}", ex.Message));
            }
        }

        private void SaveInstallInfo()
        {
            try
            {
                if (mInstallInfo == null) { return; }
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "VoiceCyber\\UMP");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, InstallInfo.FILE_NAME);
                OperationReturn optReturn = XMLHelper.SerializeFile(mInstallInfo, path);
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Save InstallInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                AppendMessage(string.Format("Save InstallInfo end.\t{0}", path));
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Save InstallInfo fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Create

        private void CreateUpdateModuleItems()
        {
            try
            {
                mListModuleItems.Clear();
                if (mUpdateInfo == null) { return; }
                for (int i = 0; i < mUpdateInfo.ListModules.Count; i++)
                {
                    var info = mUpdateInfo.ListModules[i];
                    UpdateModuleItem item = new UpdateModuleItem();
                    item.Name = info.Name;
                    item.Title = info.Content;
                    item.StrType = info.Type.ToString();
                    item.StrLevel = info.Level.ToString();
                    item.Info = info;
                    mListModuleItems.Add(item);
                }

                AppendMessage(string.Format("Create module items end.\t{0}", mListModuleItems.Count));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region EventHandlers

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                #region 生成示例更新信息文件

                //UpdateInfo testInfo = new UpdateInfo();
                //testInfo.Name = "UMP8.03.001P02.000";
                //testInfo.Version = "8.03.001P02.000";
                //testInfo.BaseVersion = "8.03.001P00.000";
                //testInfo.Type = (int)PackType.Increment;
                //testInfo.PublishDate = "2016-5-24";
                //testInfo.Description = "";

                //UpdateModule testModule = new UpdateModule();
                //testModule.SerialNo = "803001P020000001";
                //testModule.Name = "UMPMain001";
                //testModule.Type = (int)UpdateModuleType.Bug;
                //testModule.Level = 3;
                //testModule.ModuleID = 0;
                //testModule.ModuleName = "UMPMain";
                //testModule.Content = "修改Bug";
                //testModule.LangID = "MC00001";
                //testModule.ModuleLangID = "M001";
                //testModule.Description = "";
                //testInfo.ListModules.Add(testModule);

                //UpdateFile testFile = new UpdateFile();
                //testFile.Name = "UMPMain";
                //testFile.Type = (int)UpdateFileType.Directory;
                //testFile.FileName = "UMPMain_8_03_001_50";
                //testFile.SourcePath = "UMP/Application Files";
                //testFile.TargetPath = "Application Files";
                //testFile.InstallMode = (int)FileInstallMode.CopyReplace;
                //testFile.ErrorReply = (int)ErrorMode.None;
                //testFile.Description = "";
                //testInfo.ListFiles.Add(testFile);

                //UpdateSqlScript testSqlScript = new UpdateSqlScript();
                //testSqlScript.Name = "I_T_00_005";
                //testSqlScript.SqlType = (int)UpdateSqlType.DML;
                //testSqlScript.ScriptType = (int)UpdateScriptType.Insert;
                //testSqlScript.ErrorReply = (int)ErrorMode.LogFile;
                //testSqlScript.Description = string.Empty;

                //UpdateScript testScript = new UpdateScript();
                //testScript.DBType = 2;
                //testScript.Text = "AEAEAEEAEFEFEFE";
                //testSqlScript.ListScripts.Add(testScript);
                //testInfo.ListSqlScripts.Add(testSqlScript);

                //UpdateLang testLang = new UpdateLang();
                //testLang.Name = "2052";
                //testLang.LangID = 2052;
                //testLang.LangName = "简体中文";
                //testLang.InstallMode = (int)LangInstallMode.Additional;
                //testLang.Path = "2052.lang";
                //testInfo.ListLangs.Add(testLang);

                //UpdateService testService = new UpdateService();
                //testService.Name = "UMPService03";
                //testService.ServiceName = "UMP Service 03";
                //testService.StartMode = 0;
                //testService.InstallMode = 0;
                //testService.Description = "";
                //testInfo.ListServices.Add(testService);

                //UpdateFollow testFollow = new UpdateFollow();
                //testFollow.Name = "RebindSite";
                //testFollow.Key = UpdateConsts.FOLLOW_KEY_BINDSITE;
                //testFollow.DataType = (int)FollowDataType.YesNo;
                //testFollow.Value = "1";
                //testInfo.ListFollows.Add(testFollow);

                //string strPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, UpdateInfo.FILE_NAME);
                //OperationReturn optReturn = XMLHelper.SerializeFile(testInfo, strPath);
                //if (!optReturn.Result)
                //{
                //    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                //    return;
                //}

                //AppendMessage(string.Format("End.\t{0}", strPath));

                #endregion


                #region 加载更新信息文件

                //var updateInfoStream =
                //  App.GetResourceStream(new Uri("/UMPUpdater;component/UpdateInfo.xml", UriKind.RelativeOrAbsolute));
                //if (updateInfoStream == null)
                //{
                //    ShowException(string.Format("UpdateInfo.xml not exist"));
                //    return;
                //}
                //var stream = updateInfoStream.Stream;
                //if (stream == null) { return; }

                //StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                //string strContent = reader.ReadToEnd();
                //reader.Close();
                //OperationReturn optReturn = XMLHelper.DeserializeObject<UpdateInfo>(strContent);
                //if (!optReturn.Result)
                //{
                //    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                //    return;
                //}
                //UpdateInfo updateInfo = optReturn.Data as UpdateInfo;
                //if (updateInfo == null)
                //{
                //    ShowException(string.Format("Fail.\tUpdateInfo is null"));
                //    return;
                //}
                //ShowException(string.Format("{0}", updateInfo.Version));

                #endregion


                #region 生成语言包文件

                //LangLister lister=new LangLister();
                //lister.LangID = 2052;
                //lister.LangName = "简体中文";
                //lister.Path = "2052.XML";
                //LanguageInfo langInfo=new LanguageInfo();
                //langInfo.LangID = lister.LangID;
                //langInfo.Module = 0;
                //langInfo.SubModule = 0;
                //langInfo.Name = "BtnConfirm";
                //langInfo.Display = "确定";
                //lister.ListLangInfos.Add(langInfo);

                //string strPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, lister.Path);
                //OperationReturn optReturn = XMLHelper.SerializeFile(lister, strPath);
                //if (!optReturn.Result)
                //{
                //    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                //    return;
                //}
                //AppendMessage(string.Format("End.\t{0}", strPath));

                #endregion


                #region 导出语言包

                //DatabaseInfo dbInfo = new DatabaseInfo();
                //dbInfo.TypeID = 3;
                //dbInfo.Host = "192.168.4.182";
                //dbInfo.Port = 1521;
                //dbInfo.DBName = "PFOrcl";
                //dbInfo.LoginName = "PFDEV831";
                //dbInfo.Password = "pfdev831";
                //int langTypeID = 2052;
                //string strConn = dbInfo.GetConnectionString();
                //string strSql;
                //OperationReturn optReturn;
                //switch (dbInfo.TypeID)
                //{
                //    case 2:
                //        strSql = string.Format("SELECT * FROM T_00_005 WHERE C001 = {0} ORDER BY C001,C002", langTypeID);
                //        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                //        break;
                //    case 3:
                //        strSql = string.Format("SELECT * FROM T_00_005 WHERE C001 = {0} ORDER BY C001,C002", langTypeID);
                //        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                //        break;
                //    default:
                //        AppendMessage(string.Format("Fail.\t DBType invalid"));
                //        return;
                //}
                //if (!optReturn.Result)
                //{
                //    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                //    return;
                //}
                //DataSet objDataSet = optReturn.Data as DataSet;
                //if (objDataSet == null)
                //{
                //    AppendMessage(string.Format("DataSet is null"));
                //    return;
                //}
                //LangLister lister = new LangLister();
                //lister.LangID = langTypeID;
                //lister.LangName = "简体中文";
                //lister.Path = string.Format("{0}.XML", langTypeID);
                //int count = objDataSet.Tables[0].Rows.Count;
                //for (int i = 0; i < count; i++)
                //{
                //    DataRow dr = objDataSet.Tables[0].Rows[i];

                //    LanguageInfo langInfo = new LanguageInfo();
                //    langInfo.LangID = langTypeID;
                //    langInfo.Name = dr["C002"].ToString();
                //    langInfo.Module = Convert.ToInt32(dr["C009"]);
                //    langInfo.SubModule = Convert.ToInt32(dr["C010"]);
                //    langInfo.Page = dr["C011"].ToString();
                //    string display = string.Empty;
                //    display += dr["C005"].ToString();
                //    display += dr["C006"].ToString();
                //    display += dr["C007"].ToString();
                //    display += dr["C008"].ToString();
                //    langInfo.Display = display;

                //    lister.ListLangInfos.Add(langInfo);
                //}

                //string strPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, lister.Path);
                //optReturn = XMLHelper.SerializeFile(lister, strPath);
                //if (!optReturn.Result)
                //{
                //    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                //    return;
                //}
                //AppendMessage(string.Format("End.\t{0}\t{1}", strPath, count));

                #endregion


                #region 生成InstallInfo文件

                InstallInfo installInfo = new InstallInfo();
                installInfo.SessionID = Guid.NewGuid().ToString();
                installInfo.InstallTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                installInfo.BeginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                installInfo.EndTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                installInfo.Version = "8.03.001P02.000";
                installInfo.Type = (int)PackType.Accumulate;
                installInfo.MachineName = Environment.MachineName;
                installInfo.OSVersion = Environment.OSVersion.ToString();
                installInfo.OSAccount = Environment.UserName;

                InstallProduct installProduct = new InstallProduct();
                installProduct.Package = UpdateConsts.PACKAGE_NAME_UMP;
                installProduct.InstallPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "VoiceCyber\\UMP");

                InstallComponent installComponent = new InstallComponent();
                installComponent.ModuleID = 0;
                installComponent.ModuleName = UpdateConsts.COMPONENT_NAME_UMP;

                installProduct.ListComponents.Add(installComponent);
                installInfo.ListProducts.Add(installProduct);

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, InstallInfo.FILE_NAME);
                OperationReturn optReturn = XMLHelper.SerializeFile(installInfo, path);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }

                AppendMessage(string.Format("End"));

                #endregion

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            mIsContinue = false;
        }

        void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            UpdateUMP();
        }

        #endregion


        #region Others

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();

                var app = App.Current as App;
                if (app != null)
                {
                    //app.WriteLog(msg);
                }
            }));
        }

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, App.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SetProgress(bool isShow, double current)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (isShow)
                {
                    PanelProgress.Visibility = Visibility.Visible;
                }
                else
                {
                    PanelProgress.Visibility = Visibility.Collapsed;
                }
                MyProgress.Value = current;
                TxtProgress.Text = string.Format("{0}%", current.ToString("0.00"));
            }));
        }

        private void SetProgress(double current)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                MyProgress.Value = current;
                TxtProgress.Text = string.Format("{0}%", current.ToString("0.00"));
            }));
        }

        private string DecryptStringM004(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        #endregion


        #region 进度

        public const int PROGRESS_BASE_STOPSERVICE = 0;
        public const int PROGRESS_BASE_EXTRACTFILE = 10;
        public const int PROGRESS_BASE_COPYFILE = 40;
        public const int PROGRESS_BASE_UPDATEDATABASE = 60;
        public const int PROGRESS_BASE_UPDATELANGUAGE = 70;
        public const int PROGRESS_BASE_STARTSERVICE = 80;
        public const int PROGRESS_BASE_OTHERS = 90;

        public const int PROGRESS_STOPSERVICE = 10;
        public const int PROGRESS_EXTRACTFILE = 30;
        public const int PROGRESS_COPYFILE = 20;
        public const int PROGRESS_UPDATEDATABASE = 10;
        public const int PROGRESS_UPDATELANGUGAGE = 10;
        public const int PROGRESS_STARTSERVICE = 10;
        public const int PROGRESS_OTHERS = 10;

        #endregion

    }
}
