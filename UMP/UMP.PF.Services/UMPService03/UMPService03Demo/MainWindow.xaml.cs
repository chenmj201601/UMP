using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;

namespace UMPService03Demo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private Service03Helper mService03Helper;
        private NetClient mMediaClient;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnTest.Click += BtnTest_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mService03Helper != null)
            {
                mService03Helper.Close();
                mService03Helper = null;
            }
            if (mMediaClient != null)
            {
                mMediaClient.Stop();
                mMediaClient = null;
            }
        }

        private void downloadsftp()
        {
            string strAddress = "192.168.8.96";
            //string strAddress = "192.168.6.75";
            int port = 8081 - 3;

            if (mService03Helper != null)
            {
                mService03Helper.Close();
            }
            mService03Helper = new Service03Helper();
            mService03Helper.HostAddress = strAddress;
            mService03Helper.HostPort = port;
            RequestMessage request = new RequestMessage();
            request.Command = (int)Service03Command.DownloadRecordFile;
            request.ListData.Add("192.168.8.149");
            request.ListData.Add("3022");
            request.ListData.Add("1020000000000000001|00000");
            request.ListData.Add("asd.123");
            request.ListData.Add("166");
            request.ListData.Add("1505070200000000166");
            request.ListData.Add("");
            OperationReturn optReturn = mService03Helper.DoRequest(request);
            if (!optReturn.Result)
            {
                AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                return;
            }
            ReturnMessage ret = optReturn.Data as ReturnMessage;
            if (ret == null)
            {
                AppendMessage(string.Format("Fail.\tReturnMessage is null"));
                return;
            }
            AppendMessage(string.Format("End.\t{0}", @"http://192.168.8.100:8081/MediaData/" + ret.Data));
        }

        private void downftp()
        {
            string strAddress = "192.168.6.27";
            //string strAddress = "192.168.6.75";
            int port = 8081 - 3;
            if (mService03Helper != null)
            {
                mService03Helper.Close();
            }
            mService03Helper = new Service03Helper();
            mService03Helper.HostAddress = strAddress;
            mService03Helper.HostPort = port;
            RequestMessage request = new RequestMessage();
            request.Command = (int)Service03Command.DownloadRecordFileNas;
            request.ListData.Add("192.168.6.27");
            request.ListData.Add("21");
            request.ListData.Add("ftpuser");
            request.ListData.Add("123123");
            request.ListData.Add("vox/111.wav");
            request.ListData.Add("1505070200000000166");
            OperationReturn optReturn = mService03Helper.DoRequest(request);
            if (!optReturn.Result)
            {
                AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                return;
            }
            ReturnMessage ret = optReturn.Data as ReturnMessage;
            if (ret == null)
            {
                AppendMessage(string.Format("Fail.\tReturnMessage is null"));
                return;
            }
            AppendMessage(string.Format("End.\t{0}", @"http://192.168.6.27:8081/MediaData/" + ret.Data));
        }

        private void StartIsa()
        {
            try
            {
                if (mMediaClient == null) { return; }
                string strAddress = "192.168.6.112";
                RequestMessage request = new RequestMessage();
                request.SessionID = mMediaClient.SessionID;
                request.Command = (int)Service03Command.IsaStart;
                request.ListData.Add(strAddress);
                mMediaClient.SendMessage(request.Command, request);

                AppendMessage("End");
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        private void IsaPlay()
        {
            try
            {
                if (mMediaClient == null) { return; }
                string strAction = "play";
                string strRefID = "71015add-8dd7-478a-b4ca-bfd660bf020e";
                string strPosition = "0";
                string strSpeed = "0";
                RequestMessage request = new RequestMessage();
                request.SessionID = mMediaClient.SessionID;
                request.Command = (int)Service03Command.IsaBehavior;
                request.ListData.Add(strAction);
                request.ListData.Add(strRefID);
                request.ListData.Add(strPosition);
                request.ListData.Add(strSpeed);
                mMediaClient.SendMessage(request.Command, request);

                AppendMessage("End");
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        private void UploadFile()
        {
            try
            {
                string strAddress = "192.168.6.27";
                //string strAddress = "192.168.5.31";
                int port = 8081 - 3;
                if (mService03Helper != null)
                {
                    mService03Helper.Close();
                }
                mService03Helper = new Service03Helper();
                mService03Helper.HostAddress = strAddress;
                mService03Helper.HostPort = port;
                RequestMessage request = new RequestMessage();
                request.Command = (int)Service03Command.UploadFile;
                request.ListData.Add("123.txt");
                request.ListData.Add("1");
                request.ListData.Add("test.txt");
                OperationReturn optReturn = mService03Helper.DoRequest(request);
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ReturnMessage ret = optReturn.Data as ReturnMessage;
                if (ret == null)
                {
                    AppendMessage(string.Format("Fail.\tReturnMessage is null"));
                    return;
                }
                AppendMessage(string.Format("End."));
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            //downftp();
            //downloadsftp();  
            UploadFile();

            //try
            //{
            //    if (mMediaClient != null)
            //    {
            //        mMediaClient.Stop();
            //        mMediaClient = null;
            //    }
            //    mMediaClient = new NetClient();
            //    mMediaClient.Debug += (mode, cat, msg) => AppendMessage(string.Format("{0}\t{1}", cat, msg));
            //    mMediaClient.ConnectionEvent += MediaClient_ConnectionEvent;
            //    mMediaClient.ReturnMessageReceived += MediaClient_ReturnMessageReceived;
            //    mMediaClient.NotifyMessageReceived += MediaClient_NotifyMessageReceived;
            //    mMediaClient.IsSSL = true;
            //    mMediaClient.Host = "192.168.6.15";
            //    //mMediaClient.Host = "192.168.5.31";
            //    mMediaClient.Port = 8081 - 3;
            //    var optReturn = mMediaClient.Connect();
            //    if (!optReturn.Result)
            //    {
            //        AppendMessage(string.Format("Connect fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
            //        return;
            //    }
            //    AppendMessage("End");
            //}
            //catch (Exception ex)
            //{
            //    AppendMessage(ex.Message);
            //}
        }

        void MediaClient_ConnectionEvent(object sender, ConnectionEventArgs e)
        {
            try
            {
                if (e.Code == Defines.EVT_NET_CONNECTED)
                {
                    StartIsa();
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        void MediaClient_NotifyMessageReceived(object sender, NotifyMessageReceivedEventArgs e)
        {
            try
            {
                var notify = e.NotifyMessage;
                string str = string.Empty;
                if (notify.ListData != null)
                {
                    if (notify.ListData.Count >= 2)
                    {
                        string strPos = notify.ListData[0];
                        string strFile = notify.ListData[1];
                        str += string.Format("{0}\t{1};", strPos, strFile);

                        if (!string.IsNullOrEmpty(strFile))
                        {
                            //string path = string.Format("http://192.168.6.27:8081/MediaData/{0}", notify.ListData[1]);
                            //Dispatcher.Invoke(new Action(() =>
                            //{
                            //    BitmapImage bitmap = new BitmapImage();
                            //    bitmap.BeginInit();
                            //    bitmap.UriSource = new Uri(path, UriKind.RelativeOrAbsolute);
                            //    bitmap.EndInit();
                            //    ImgVedio.Source = bitmap;
                            //}));
                            OperationReturn optReturn = DownloadImage(strFile);
                            if (!optReturn.Result)
                            {
                                AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            string path = optReturn.Data.ToString();
                            Dispatcher.Invoke(new Action(() =>
                            {
                                string strTemp = path;
                                BitmapImage bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.UriSource = new Uri(strTemp, UriKind.RelativeOrAbsolute);
                                bitmap.EndInit();
                                ImgVedio.Source = bitmap;
                            }));
                        }
                    }
                }
                AppendMessage(str);
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        void MediaClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            try
            {
                var retMessage = e.ReturnMessage;
                if (retMessage.Command == (int)Service03Command.IsaStart)
                {
                    AppendMessage(string.Format("Replay:{0}", (Service03Command)retMessage.Command));
                    IsaPlay();
                }
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private OperationReturn DownloadImage(string file)
        {
            OperationReturn optReturn=new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string requestPath = Path.Combine(ConstValue.TEMP_DIR_MEDIADATA, file);
                string savePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConstValue.TEMP_DIR_MEDIADATA,
                    file);
                DownloadConfig config=new DownloadConfig();
                config.Method = 1;
                config.Host = "192.168.6.15";
                config.Port = 8081;
                config.IsAnonymous = true;
                config.RequestPath = requestPath;
                config.SavePath = savePath;
                config.IsReplace = true;
                optReturn = DownloadHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn.Data = savePath;
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
