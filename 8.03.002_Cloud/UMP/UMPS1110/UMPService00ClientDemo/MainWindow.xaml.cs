using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using PFShareClassesC;
using VoiceCyber.Common;

namespace UMPService00ClientDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private ObservableCollection<OperationItem> mListOperations;
        private Service00Helper mService00Helper;

        public MainWindow()
        {
            InitializeComponent();

            mListOperations = new ObservableCollection<OperationItem>();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnTest.Click += BtnTest_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ComboOperations.ItemsSource = mListOperations;
            InitOperationItems();

            WindowState = WindowState.Maximized;
            TxtHostAddress.Text = "192.168.6.104";
            TxtHostPort.Text = "8009";
            if (ComboOperations.Items.Count > 0)
            {
                ComboOperations.SelectedIndex = 0;
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mService00Helper != null)
            {
                mService00Helper.Stop();
            }
        }

        private void InitOperationItems()
        {
            mListOperations.Clear();
            OperationItem item = new OperationItem();
            item.Name = string.Format("GetServerName");
            item.Value = "G001";
            mListOperations.Add(item);
            item = new OperationItem();
            item.Name = string.Format("GetDiskInformation");
            item.Value = "G002";
            mListOperations.Add(item);
            item = new OperationItem();
            item.Name = string.Format("GetNetworkCards");
            item.Value = "G003";
            mListOperations.Add(item);
            item = new OperationItem();
            item.Name = string.Format("GetSubDirectory");
            item.Value = "G004";
            mListOperations.Add(item);
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opt = ComboOperations.SelectedItem as OperationItem;
                if(opt==null){return;}
                string command = opt.Value;
                string args = TxtParams.Text;
                string[] arrArgs = args.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                List<string> listArgs = new List<string>();
                switch (command)
                {
                    case RequestCommand.GET_HOST_NAME:

                        break;
                    case RequestCommand.GET_DISK_INFO:
                        listArgs.Add("1");
                        break;
                    case RequestCommand.GET_SUBDIRECTORY:
                        if (arrArgs.Length < 1)
                        {
                            AppendMessage(string.Format("Param is null"));
                            return;
                        }
                        listArgs.Add(arrArgs[0]);
                        break;
                    case RequestCommand.GET_NETWORK_CARD:

                        break;
                    default:
                        return;
                }
                if (mService00Helper == null)
                {
                    mService00Helper = new Service00Helper();
                    mService00Helper.Debug += mService00Helper_Debug;
                    mService00Helper.Start();
                }
                mService00Helper.HostAddress = TxtHostAddress.Text;
                mService00Helper.HostPort = int.Parse(TxtHostPort.Text);
                ThreadPool.QueueUserWorkItem(s =>
                {
                    OperationReturn optReturn = mService00Helper.DoOperation(command, listArgs);
                    if (!optReturn.Result)
                    {
                        AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    string strMessage = optReturn.Data.ToString();
                    AppendMessage(string.Format("{0}", strMessage));
                });

                //OperationReturn optReturn = mService00Helper.DoOperation(RequestCommand.GET_NETWORK_CARD, null);
                //if (!optReturn.Result)
                //{
                //    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                //    return;
                //}
                //string strMessage = optReturn.Data.ToString();
                //AppendMessage(string.Format("{0}", strMessage));


                //TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork);
                //tcpClient.Connect("192.168.6.104", 8009);
                //SslStream sslStream = new SslStream(tcpClient.GetStream(), false, (s, cert, chain, errs) => true, null);
                //sslStream.AuthenticateAsClient("192.168.6.104");
                //string strSendMessage = string.Format("{0}\r\n", EncryptString("G003"));
                //sslStream.Write(Encoding.UTF8.GetBytes(strSendMessage));
                //sslStream.Flush();
                //int intReaded = 0;
                //byte[] buffer = new byte[1024];
                //string strReturn = string.Empty;
                //int intCount = 0;
                //do
                //{
                //    intReaded = sslStream.Read(buffer, 0, 1024);
                //    if (intReaded > 0)
                //    {
                //        string strMessage = Encoding.UTF8.GetString(buffer, 0, intReaded);
                //        strReturn += strMessage;
                //        if (strReturn.EndsWith(string.Format("{0}End{0}\r\n", ConstValue.SPLITER_CHAR)))
                //        {
                //            strReturn = strReturn.Substring(0, strReturn.Length - 7);
                //            if (strReturn.EndsWith("\r\n"))
                //            {
                //                strReturn = strReturn.Substring(0, strReturn.Length - 2);
                //            }
                //            break;
                //        }
                //    }
                //    intCount++;
                //    //超时
                //    if (intCount > 100)
                //    {
                //        AppendMessage(string.Format("Timeout"));
                //    }
                //    Thread.Sleep(100);
                //} while (intReaded > 0);
                //AppendMessage(string.Format("{0}", strReturn));
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 128)
                {
                    strTemp = strSource.Substring(0, 128);
                    strSource = strSource.Substring(128, strSource.Length - 128);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string DecryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 512)
                {
                    strTemp = strSource.Substring(0, 512);
                    strSource = strSource.Substring(512, strSource.Length - 512);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
                    EncryptionAndDecryption.UMPKeyAndIVType.M104);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
        {
            string lStrReturn;
            int LIntRand;
            Random lRandom = new Random();
            string LStrTemp;

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = lRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "VCT");
                LIntRand = lRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "UMP");
                LIntRand = lRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, ((int)aKeyIVID).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }

        #endregion

        void mService00Helper_Debug(string msg)
        {
            AppendMessage(string.Format("Service00Helper\t{0}", msg));
        }

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }
    }
}
