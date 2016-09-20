using System;
using System.ComponentModel;
using System.Windows;
using VoiceCyber.Common;
using VoiceCyber.SDKs.DEC;

namespace DECDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private LogOperator mLogOperator;
        private DecConnector mDecConnector;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnTest.Click += BtnTest_Click;
            BtnConnect.Click += BtnConnect_Click;
            BtnAddSubscribe.Click += BtnAddSubscribe_Click;
            BtnClearSubscribe.Click += BtnClearSubscribe_Click;
            BtnPublishMessage.Click += BtnPublishMessage_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            mLogOperator = new LogOperator();
            mLogOperator.Start();
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (mDecConnector != null)
            {
                mDecConnector.Close();
            }
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
                mLogOperator = null;
            }
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mDecConnector != null)
                {
                    mDecConnector.Close();
                }
                mDecConnector = new DecConnector();
                mDecConnector.Debug += msg => AppendMessage(string.Format("Connector\t{0}", msg));
                mDecConnector.MessageReceivedEvent += DealMessageReceicedEvent;
                mDecConnector.Host = "192.168.6.27";
                mDecConnector.Port = 3072;
                mDecConnector.ModuleType = 1101;
                mDecConnector.ModuleNumber = 0;
                mDecConnector.Connect();
                if (!mDecConnector.IsConnected)
                {
                    AppendMessage(string.Format("Connected fail."));
                }
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mDecConnector != null)
                {
                    mDecConnector.Close();
                }
                mDecConnector = new DecConnector();
                mDecConnector.Debug += msg => AppendMessage(string.Format("Connector\t{0}", msg));
                mDecConnector.MessageReceivedEvent += DealMessageReceicedEvent;
                mDecConnector.Host = "192.168.6.27";
                mDecConnector.Port = 3072;
                mDecConnector.ModuleType = 0x1101;
                mDecConnector.ModuleNumber = 0;

                //订阅由VoiceServer相关的所有消息
                MessageString mask = new MessageString();
                mask.SourceModule = 0xffff;      //模块掩码
                mask.SourceNumber = 0x0000;
                mask.TargetModule = 0x0000;
                mask.TargetNumber = 0x0000;
                mask.LargeType = 0x0000;
                mask.MiddleType = 0x0000;
                mask.SmallType = 0x0000;
                mask.Number = 0x0000;
                MessageString message = new MessageString();
                message.SourceModule = 0x1514;      //由VoiceServer发布的所有消息
                message.SourceNumber = 0x0000;     
                message.TargetModule = 0x0000;
                message.TargetNumber = 0x0000;
                message.LargeType = 0x0000;
                message.MiddleType = 0x0000;
                message.SmallType = 0x0000;
                message.Number = 0x0000;
                mDecConnector.MaskMsg = mask;
                mDecConnector.MessageMsg = message;

                mDecConnector.Connect();
                if (!mDecConnector.IsConnected)
                {
                    AppendMessage(string.Format("Connected fail."));
                }
                //mDecConnector.BeginConnect();   //启用了断线重连
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            //ulong end = 385125809280516471;
            //string str = "sgwgw";
            //byte[] arr = Helpers.EncryptString(2, end, str);

            Close();
        }

        void BtnAddSubscribe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //订阅由VoiceServer相关的所有消息
                MessageString mask = new MessageString();
                mask.SourceModule = 0x0000;      //模块掩码
                mask.SourceNumber = 0x0000;
                mask.TargetModule = 0xffff;
                mask.TargetNumber = 0x0000;
                mask.LargeType = 0x0000;
                mask.MiddleType = 0x0000;
                mask.SmallType = 0x0000;
                mask.Number = 0x0000;
                MessageString message = new MessageString();
                message.SourceModule = 0x0000;      //由VoiceServer发布的所有消息
                message.SourceNumber = 0x0000;      //发给VoiceServer的消息
                message.TargetModule = 0x1514;
                message.TargetNumber = 0x0000;
                message.LargeType = 0x0000;
                message.MiddleType = 0x0000;
                message.SmallType = 0x0000;
                message.Number = 0x0000;
                if (mDecConnector != null)
                {
                    mDecConnector.AddSubscribe(mask, message);
                }
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnClearSubscribe_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (mDecConnector != null)
                {
                    mDecConnector.ClearSubscribe();
                }
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnPublishMessage_Click(object sender, RoutedEventArgs e)
        {
            string str = string.Format("Hello! my name is charley");
            if (mDecConnector != null)
            {
                MessageString message = new MessageString();
                message.SourceModule = 0x0000;
                message.SourceNumber = 0x0000;
                message.TargetModule = 0x1514;      //发送给VoiceServer
                message.TargetNumber = 0x0000;
                message.LargeType = 0x0001;
                message.MiddleType = 0x0002;
                message.SmallType = 0x0003;
                message.Number = 0x0004;
                mDecConnector.DataFormat = DecDefines.NETPACK_BASEHEAD_VER1_FORMAT_TEXT_UTF8;
                mDecConnector.PublishMessage(str, message);
            }
        }

        private void AppendMessage(string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, "Demo", msg);
            }
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }

        private void DealMessageReceicedEvent(object sender, MessageReceivedEventArgs args)
        {
            try
            {
                var distHead = args.DistHead;
                if (distHead.BaseType == DecDefines.NETPACK_BASETYPE_APPLICATION_VER1)
                {
                    AppendMessage(string.Format("Time:{0}\tSerial:0x{1:X16}\tModuleID:{2:X4}",
                        distHead.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                        distHead.SourceID,
                        distHead.ModuleID));
                    var message = distHead.Message;
                    AppendMessage(
                        string.Format("Message:{0}", message));
                    var appHead = args.AppHead;
                    if (appHead != null)
                    {
                        AppendMessage(
                            string.Format(
                                "Channel:{0}\tEncrypt:{1}\tCompress:{2}\tFormat:{3}\tDataSize:{4}\tValidSize:{5}",
                                appHead.Channel,
                                appHead.Encrypt,
                                appHead.Compress,
                                appHead.Format,
                                appHead.DataSize,
                                appHead.ValidSize));

                        string strData = Helpers.DecryptString(appHead.Encrypt, distHead.SourceID, args.Data);
                        AppendMessage(string.Format("AppMessage:{0}", strData));
                    }
                }
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }
    }
}
