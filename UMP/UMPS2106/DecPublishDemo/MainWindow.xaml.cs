using System;
using System.ComponentModel;
using System.Windows;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.SDKs.DEC;

namespace DecPublishDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        private DecHelper mDecHelper;

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
            if (mDecHelper != null)
            {
                mDecHelper.Close();
                mDecHelper = null;
            }
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XmlDocument doc;
                DateTime dt;
                string strMessageID;
                XmlElement nodeMessage;
                XmlElement nodeFileParam;
                XmlElement nodeChannels;
                XmlElement nodeChannel;
                string strMessage;

                if (mDecHelper != null)
                {
                    mDecHelper.Close();
                    mDecHelper = null;
                }
                mDecHelper = new DecHelper();
                mDecHelper.Debug += (mode, cat, msg) => AppendMessage(string.Format("{0}\t{1}", cat, msg));
                mDecHelper.HostAddress = "192.168.6.74";
                mDecHelper.HostPort = 3072;

                MessageString message = new MessageString();
                message.SourceModule = 0x0000;
                message.SourceNumber = 0x0000;
                message.TargetModule = 0x152B;
                message.TargetNumber = 0x0000;
                message.Number = 0x0003;            //开始恢复录音
                message.SmallType = 0x0001;
                message.MiddleType = 0x0001;
                message.LargeType = 0x0009;

                doc = new XmlDocument();
                doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");

                dt = DateTime.Now;
                dt = dt.AddYears(-1600);
                strMessageID = string.Format("0x{0:X4}{1:X4}{2:X4}{3:X4}",
                    message.LargeType,
                    message.MiddleType,
                    message.SmallType,
                    message.Number);
                nodeMessage = doc.CreateElement(DecHelper.NODE_MESSAGE);
                nodeMessage.SetAttribute(DecHelper.ATTR_MESSAGEID, strMessageID);
                nodeMessage.SetAttribute(DecHelper.ATTR_CURRENTTIME, dt.Ticks.ToString());
                nodeFileParam = doc.CreateElement(DecHelper.NODE_FILEPARAM);
                nodeFileParam.SetAttribute(DecHelper.ATTR_FILEPATH, @"F:\voipdata\sip");
                nodeFileParam.SetAttribute(DecHelper.ATTR_TIMEFROM, "2016-10-22 00:00:00");
                nodeFileParam.SetAttribute(DecHelper.ATTR_TIMETO, "2016-10-24 00:00:00");
                nodeMessage.AppendChild(nodeFileParam);
                nodeChannels = doc.CreateElement(DecHelper.NODE_RECOVERCHANNELS);
                nodeChannel = doc.CreateElement(DecHelper.NODE_RECOVERCHANNEL);
                nodeChannel.SetAttribute(DecHelper.ATTR_CHANNELID, "0");
                nodeChannel.SetAttribute(DecHelper.ATTR_ORIGINALVOICENUMBER, "0");
                nodeChannel.SetAttribute(DecHelper.ATTR_ORIGINALCHANNELID, "0");
                nodeChannels.AppendChild(nodeChannel);
                nodeMessage.AppendChild(nodeChannels);
                doc.AppendChild(nodeMessage);
                strMessage = doc.OuterXml;

                AppendMessage(string.Format("{0}", strMessage));

                OperationReturn optReturn = null;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (s, de) =>
                {
                    optReturn = mDecHelper.PublishMessage(strMessage, message);
                };
                worker.RunWorkerCompleted += (s, re) =>
                {
                    worker.Dispose();

                    if (optReturn == null)
                    {
                        AppendMessage(string.Format("OptReturn is null."));
                        return;
                    }
                    if (!optReturn.Result)
                    {
                        AppendMessage(string.Format("Publish fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }

                    AppendMessage(string.Format("Publish end."));
                };
                worker.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }

        private void ShowException(string msg)
        {
            MessageBox.Show(msg, "DecPublishDemo", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
