using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows;
using VoiceCyber.SharpZips.Zip;

namespace UMPClientPackageSetup
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public const string SERVICE_NAME = "UMP Client Package";
        public const string PROCESS_NAME = "UMPClientPackage";

        private bool mIsInited;
        private bool mIsSetuped;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                mIsInited = true;
                mIsSetuped = false;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) => DoSetup();
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    if (mIsSetuped)
                    {
                        Close();
                    }
                };
                worker.RunWorkerAsync();
            }
        }

        private void DoSetup()
        {
            try
            {
                //停止服务
                ServiceController sc;
                bool isInstalled = false;
                try
                {
                    ServiceController[] services = ServiceController.GetServices();
                    sc = services.FirstOrDefault(s => s.ServiceName == SERVICE_NAME);
                    if (sc != null)
                    {
                        //如果服务已经安装，则停止服务
                        isInstalled = true;
                        if (sc.Status != ServiceControllerStatus.Stopped)
                        {
                            AppendMessage(string.Format("Stopping service..."));
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, new TimeSpan(0, 0, 60));
                        }
                        KillProcess(PROCESS_NAME);
                    }
                }
                catch (Exception ex)
                {
                    AppendMessage(string.Format("Stop service failed.\t{0}", ex.Message));
                }

                //判断数据文件是否存在
                var rsUMPData = App.GetResourceStream(new Uri("/UMPClientPackageSetup;component/Resources/UMPClientData.zip", UriKind.RelativeOrAbsolute));
                if (rsUMPData == null)
                {
                    AppendMessage(string.Format("UMPClientData not exist."));
                    return;
                }
                var stream = rsUMPData.Stream;
                if (stream == null)
                {
                    AppendMessage(string.Format("UMPClientData not exist."));
                    return;
                }

                //拷贝证书及设置文件到UMPClient目录
                string strUserClientDir =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UMP.Client");
                if (!Directory.Exists(strUserClientDir))
                {
                    AppendMessage(string.Format("UserClientDir not exist.\t{0}", strUserClientDir));
                    return;
                }
                string strClientDir =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\UMPClient");
                if (!Directory.Exists(strClientDir))
                {
                    Directory.CreateDirectory(strClientDir);
                }
                try
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(strUserClientDir);
                    FileInfo[] files = dirInfo.GetFiles();
                    for (int i = 0; i < files.Length; i++)
                    {
                        FileInfo file = files[i];
                        if (file.Name.ToLower() == "ump.setted.xml"
                            || file.Extension.ToLower() == ".pfx")
                        {
                            string strTarget = Path.Combine(strClientDir, file.Name);
                            try
                            {
                                File.Copy(file.FullName, strTarget, true);
                                AppendMessage(string.Format("Copy file end.\t{0}\t{1}", file.FullName, strTarget));
                            }
                            catch (Exception ex)
                            {
                                AppendMessage(string.Format("Copy file fail.\t{0}", ex.Message));
                            }
                        }
                    }
                    AppendMessage(string.Format("Copy file end"));
                }
                catch (Exception ex)
                {
                    AppendMessage(string.Format("Copy files fail.\t{0}", ex.Message));
                    return;
                }

                string strClientPackage = Path.Combine(strClientDir, "UMPClientPackage");
                if (!Directory.Exists(strClientPackage))
                {
                    Directory.CreateDirectory(strClientPackage);
                }
                //解压文件
                AppendMessage(string.Format("Extracting files..."));
                using (var zipStream = new ZipInputStream(stream))
                {
                    ZipEntry theEntry;
                    while ((theEntry = zipStream.GetNextEntry()) != null)
                    {
                        string dirName = strClientPackage;
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
                    }
                }
                AppendMessage(string.Format("Extract files end."));

                //如果服务未安装，先安装服务
                if (!isInstalled)
                {
                    string strServiceFile = Path.Combine(strClientPackage, "UMPClientPackage.exe");
                    if (!File.Exists(strServiceFile))
                    {
                        AppendMessage(string.Format("UMPClientPackage.exe not exist.\t{0}", strServiceFile));
                        return;
                    }
                    string[] options = { };
                    try
                    {
                        AppendMessage(string.Format("Installing service..."));
                        TransactedInstaller transacted = new TransactedInstaller();
                        AssemblyInstaller installer = new AssemblyInstaller(strServiceFile, options);
                        transacted.Installers.Add(installer);
                        transacted.Install(new Hashtable());

                        isInstalled = true;
                        AppendMessage(string.Format("Service installed"));
                    }
                    catch (Exception ex)
                    {
                        AppendMessage(string.Format("InstallService fail.\t{0}", ex.Message));
                    }
                }

                //启动服务
                if (!isInstalled)
                {
                    AppendMessage(string.Format("Service not installed."));
                    return;
                }
                try
                {
                    AppendMessage(string.Format("Starting service..."));
                    sc = new ServiceController(SERVICE_NAME);
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, new TimeSpan(0, 0, 60));
                    AppendMessage(string.Format("Service started."));
                }
                catch (Exception ex)
                {
                    AppendMessage(string.Format("StartService fail.\t{0}", ex.Message));
                    return;
                }
                AppendMessage(string.Format("Setup end"));
                Thread.Sleep(1000);

                mIsSetuped = true;
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        private void KillProcess(string processName)
        {
            try
            {
                Process[] processes = Process.GetProcesses();
                var process = processes.FirstOrDefault(p => p.ProcessName == processName);
                if (process != null)
                {
                    process.Kill();
                    AppendMessage(string.Format("Kill process {0} end.", processName));
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("KillProcess fail.\t{0}", ex.Message));
            }
        }

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\r\n", msg));
                TxtMsg.ScrollToEnd();
            }));
        }
    }
}
