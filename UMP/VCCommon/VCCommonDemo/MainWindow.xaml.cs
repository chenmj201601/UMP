using System;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Windows;
using VoiceCyber.Common;

namespace VCCommonDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private LogOperator mLogOperator;
        private BackgroundWorker mWorker;

        public MainWindow()
        {
            InitializeComponent();

            mLogOperator = new LogOperator();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (mLogOperator != null) mLogOperator.Start();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
            }
        }

        private void BtnTest_OnClick(object sender, RoutedEventArgs e)
        {
            //WriteLog(string.Format("Begin\t{0}", Thread.CurrentThread.ManagedThreadId));
            //var result = System.Threading.Tasks.Parallel.For(0, 102400, i => WriteLog(string.Format("Log test.\t{0}\t{1}", i, Guid.NewGuid())));
            //SubDebug(string.Format("{0}", result));

            //OperationReturn optReturn = RegistryHelper.WriteItemData(RegistryType.CurrentUser, @"Software\Intel\test\test", "TEST", 123456);
            ////OperationReturn optReturn = RegistryHelper.GetItemData(RegistryType.CurrentUser, @"Software\Intel\test", "TEST");
            //SubDebug(string.Format("{0}-{1}", optReturn.Code, optReturn.Message));

            //string strConfigFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.xml");
            //if (!System.IO.File.Exists(strConfigFile))
            //{
            //    SubDebug(string.Format("ConfigFile not exist.\t{0}", strConfigFile));
            //    return;
            //}
            //try
            //{
            //    OperationReturn optReturn = XMLHelper.GetElementValue(strConfigFile, "configurations/LogMode");
            //    if (!optReturn.Result)
            //    {
            //        SubDebug(string.Format("Fail.\t{0}", optReturn));
            //        return;
            //    }
            //    SubDebug(string.Format("{0}", optReturn.Data));

            //    //OperationReturn optReturn = XMLHelper.GetChildElements(strConfigFile, "configurations");
            //    //if (!optReturn.Result)
            //    //{
            //    //    SubDebug(string.Format("Fail.\t{0}",optReturn));
            //    //    return;
            //    //}
            //    //List<XmlElement> listConfig = optReturn.Data as List<XmlElement>;
            //    //if (listConfig == null)
            //    //{
            //    //    SubDebug(string.Format("ListConfig is null"));
            //    //    return;
            //    //}
            //    //for (int i = 0; i < listConfig.Count; i++)
            //    //{
            //    //    XmlElement element = listConfig[i];
            //    //    string configName = element.GetAttribute("key");
            //    //    string configValue = element.GetAttribute("value");
            //    //    switch (configName)
            //    //    {
            //    //        case "LogMode":
            //    //            SubDebug(string.Format("{0}\t{1}", configName, configValue));
            //    //            break;
            //    //        case "LogSaveDays":
            //    //            SubDebug(string.Format("{0}\t{1}", configName, configValue));
            //    //            break;
            //    //    }
            //    //}
            //    //SubDebug(string.Format("End"));
            //}
            //catch (Exception ex)
            //{
            //    SubDebug(string.Format("Fail.\t{0}", ex.Message));
            //}

            //MyClass person = new MyClass();
            //person.Name = "Test";
            //person.Display = "Display";
            //person.Age = 20;
            //person.Birthday = DateTime.Now;
            //OperationReturn optReturn = XMLHelper.SeriallizeObject(person);
            //if (!optReturn.Result)
            //{
            //    SubDebug(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
            //    return;
            //}
            //string strContent = optReturn.Data.ToString();
            //SubDebug(string.Format("Success.\t{0}", strContent));
            //string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "jjj.xml");
            //optReturn = XMLHelper.SerializeFile(person, filePath);
            //if (!optReturn.Result)
            //{
            //    SubDebug(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
            //    return;
            //}
            //MyClass result=new MyClass();
            //optReturn = XMLHelper.DeserializeObject<MyClass>(strContent);
            //if (!optReturn.Result)
            //{
            //    SubDebug(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
            //    return;
            //}
            //result = optReturn.Data as MyClass;
            //if (result != null)
            //{
            //    SubDebug(string.Format("Name:{0}", result.Birthday));
            //}
            //optReturn = XMLHelper.DeserializeFile<MyClass>(filePath);
            //if (!optReturn.Result)
            //{
            //    SubDebug(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
            //    return;
            //}
            //result = optReturn.Data as MyClass;
            //if (result != null)
            //{
            //    SubDebug(string.Format("Name:{0}", result.Age));
            //}

            try
            {
                string str = "{\"Extension\": [ \"80   02\" ,   \"80 1 0 3\"]}";
                JsonObject json = new JsonObject(str);
                string str2 = json.ToString();
                SubDebug(str2);
            }
            catch (Exception ex)
            {
                SubDebug(ex.Message);
            }
        }

        private void BtnWriteLog_OnClick(object sender, RoutedEventArgs e)
        {
            SubDebug(string.Format("WriteLog starting..."));
            WriteLog(string.Format("WriteLog starting..."));
            DateTime dtBegin = DateTime.Now;
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                int i = 0;
                while (i < 102400)
                {
                    WriteLog(string.Format("LogTest\t{0}", i));
                    i++;
                }
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                mWorker = null;
                DateTime dtEnd = DateTime.Now;
                double seconds = (dtEnd - dtBegin).TotalSeconds;
                WriteLog(string.Format("All log writted.\t{0}s", seconds.ToString("0.000")));
                SubDebug(string.Format("All logWriteted.\t{0}s", seconds.ToString("0.000")));
            };
            mWorker.RunWorkerAsync();
        }

        private void BtnDownloadFTP_OnClick(object sender, RoutedEventArgs e)
        {
            string[] parameters = new string[7];
            parameters[0] = "192.168.4.28";
            parameters[1] = "21";
            parameters[2] = "administrator";
            parameters[3] = "voicecyber";
            parameters[4] = @"vox\20140320\00\000\C0000020140320151633.wav";
            parameters[5] = @"D:\temp\test.wav";
            parameters[6] = "1";
            SubDebug(string.Format("Start Downloading..."));
            DateTime dtBegin = DateTime.Now;
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                OperationReturn optReturn = DownloadHelper.DownloadFileFTP(parameters);
                if (!optReturn.Result)
                {
                    SubDebug(string.Format("Download fail.\t{0}", optReturn));
                }
                else
                {
                    DateTime dtEnd = DateTime.Now;
                    double seconds = (dtEnd - dtBegin).TotalSeconds;
                    SubDebug(string.Format("Download end.\t{0}\t{1}s", optReturn.StringValue, seconds.ToString("0.000")));
                }
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                mWorker = null;
            };
            mWorker.RunWorkerAsync();
        }

        private void BtnDownloadHTTP_OnClick(object sender, RoutedEventArgs e)
        {
            string[] parameters = new string[7];
            parameters[0] = "192.168.4.28";
            parameters[1] = "8001";
            parameters[2] = string.Empty;
            parameters[3] = string.Empty;
            parameters[4] = @"IMPWaveUtil\vox\C0000020140320151633-20140320160412.wav";
            parameters[5] = @"D:\temp\test.wav";
            parameters[6] = "1";
            SubDebug(string.Format("Start Downloading..."));
            DateTime dtBegin = DateTime.Now;
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                OperationReturn optReturn = DownloadHelper.DownloadFileHTTP(parameters);
                if (!optReturn.Result)
                {
                    SubDebug(string.Format("Download fail.\t{0}", optReturn));
                }
                else
                {
                    DateTime dtEnd = DateTime.Now;
                    double seconds = (dtEnd - dtBegin).TotalSeconds;
                    SubDebug(string.Format("Download end.\t{0}\t{1}s", optReturn.StringValue, seconds.ToString("0.000")));
                }
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                mWorker = null;
            };
            mWorker.RunWorkerAsync();
        }

        private void BtnGetDataSet_OnClick(object sender, RoutedEventArgs e)
        {
            string strConn = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}"
                , "192.168.4.181", 1433, "UMPDataDB0319", "VCLogIMP", "imp_123");
            string strSql = string.Format("select top 200 * from T_21_000");
            SubDebug(string.Format("Getting data..."));
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                OperationReturn optReturn = DBAccessHelper.GetDataSet(strConn, strSql);
                if (!optReturn.Result)
                {
                    SubDebug(string.Format("Fail.\t{0}", optReturn));
                }
                else
                {
                    DataSet objDS = optReturn.Data as DataSet;
                    if (objDS == null)
                    {
                        SubDebug(string.Format("DataSet is null"));
                    }
                    else
                    {
                        SubDebug(string.Format("End.\t{0}", objDS.Tables[0].Rows.Count));
                    }
                }
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                mWorker = null;
            };
            mWorker.RunWorkerAsync();
        }

        private void BtnExecuteCommand_OnClick(object sender, RoutedEventArgs e)
        {
            string strConn = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}"
               , "192.168.4.181", 1433, "UMPDataDB0319", "VCLogIMP", "imp_123");
            string strSql = string.Format("delete from T_11_701_OL");
            SubDebug(string.Format("Executing command..."));
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                OperationReturn optReturn = DBAccessHelper.ExecuteCommand(strConn, strSql);
                if (!optReturn.Result)
                {
                    SubDebug(string.Format("Fail.\t{0}", optReturn));
                }
                else
                {
                    SubDebug(string.Format("End\t{0}", optReturn.IntValue));
                }
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {
                mWorker.Dispose();
                mWorker = null;
            };
            mWorker.RunWorkerAsync();
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WriteLog(string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.LogInfo("LogOperatorDemo", msg);
            }
        }

        private void SubDebug(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss.fff"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }
    }

    public class MyClass
    {
        public string Name { get; set; }
        public string Display { get; set; }
        public int Age { get; set; }
        public DateTime Birthday { get; set; }
    }
}
