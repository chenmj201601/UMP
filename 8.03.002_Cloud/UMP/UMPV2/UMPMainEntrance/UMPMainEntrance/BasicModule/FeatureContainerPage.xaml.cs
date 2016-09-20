using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YoungClassesLibrary;
using YoungControlLibrary;

namespace UMPMainEntrance.BasicModule
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public partial class FeatureContainerPage : Page, IClient2ServerMessage, INotifyPropertyChanged
    {
        #region 属性定义

        /// <summary>
        /// 功能的图标
        /// </summary>
        private string _StrFeatureImageSource;
        public string StrFeatureImageSource
        {
            get { return _StrFeatureImageSource; }
            set
            {
                _StrFeatureImageSource = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrFeatureImageSource");
                }
            }
        }

        /// <summary>
        /// 功能编号
        /// </summary>
        private string _StrFeatureID = string.Empty;
        public string StrFeatureID
        {
            get { return _StrFeatureID; }
            set { _StrFeatureID = value; }
        }
        #endregion

        BackgroundWorker IBackgroundWorkerInitServerHost = null;

        public FeatureContainerPage()
        {
            InitializeComponent();
            this.Loaded += FeatureContainerPage_Loaded;
            this.Unloaded += FeatureContainerPage_Unloaded;
        }
        
        private void Page_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }

        private void FeatureContainerPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (App.GServiceHost != null)
            {
                App.GServiceHost.Close();
                App.GListStrClient.Clear();
            }
            if (IBackgroundWorkerInitServerHost != null)
            {
                if (IBackgroundWorkerInitServerHost != null)
                {
                    IBackgroundWorkerInitServerHost.Dispose(); IBackgroundWorkerInitServerHost = null;
                }
            }
        }

        #region 初始化ServiceHost，用来与各模块通讯
        private void FeatureContainerPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IBackgroundWorkerInitServerHost == null)
                {
                    IBackgroundWorkerInitServerHost = new BackgroundWorker();
                }
                IBackgroundWorkerInitServerHost.RunWorkerCompleted += IBackgroundWorkerInitServerHost_RunWorkerCompleted;
                IBackgroundWorkerInitServerHost.DoWork += IBackgroundWorkerInitServerHost_DoWork;
                IBackgroundWorkerInitServerHost.RunWorkerAsync();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        
        private void IBackgroundWorkerInitServerHost_DoWork(object sender, DoWorkEventArgs e)
        {
            App.GListStrClient.Clear();
            App.GServiceHost = new ServiceHost(this);
            App.GServiceHost.AddServiceEndpoint(typeof(IClient2ServerMessage), new NetNamedPipeBinding(), "net.pipe://localhost/UMPServer");
            App.GServiceHost.Open();
        }

        private void IBackgroundWorkerInitServerHost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LoadUMPSubFeature();
            //throw new NotImplementedException();
        }
        #endregion

        #region ======加载功能模块,测试开发用，后面将删除
        private void LoadUMPSubFeature()
        {
            WebBrowserFeature.Navigate("http://localhost:8088/UMPS1101.xbap");
        }
        #endregion

        #region 处理各模块发送过来的消息
        public void ProcessingClientMessage(ShareClassesInterface AInterfaceArgs)
        {
            if (AInterfaceArgs.StrObjectTag == "LOADED")
            {
                IStrClientGuid = AInterfaceArgs.ObjObjectSource0.ToString();

                //以下代码为测试代码，将被删除

                //this.Dispatcher.Invoke(new Action(() =>
                //{
                //    WebBrowserFeature.Visibility = System.Windows.Visibility.Visible;
                //    ImageBackGround.Visibility = System.Windows.Visibility.Collapsed;
                //    ImageLoadingFeature.Visibility = System.Windows.Visibility.Collapsed;
                //    SprocketControlLoading.Visibility = System.Windows.Visibility.Collapsed;
                //}));

                ShareClassesInterface LShareClasses = new ShareClassesInterface();
                LShareClasses.StrObjectTag = "SetValue";
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrLoginSerialID);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrLoginAccount);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrLoginUserName);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrFeatureID);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrFeatureName);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrFeatureImage);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrImageSize);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrUseProtol);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrServerHost);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrServerPort);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrUseStyle);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrOpenXbap);
                LShareClasses.ListObjectSource.Add(App.GIntermediateParam.StrXbapArgs);

                SendMessage2SubFeature(LShareClasses);

                
            }
            else if (AInterfaceArgs.StrObjectTag == "INITED")
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    WebBrowserFeature.Visibility = System.Windows.Visibility.Visible;
                    ImageBackGround.Visibility = System.Windows.Visibility.Collapsed;
                    ImageLoadingFeature.Visibility = System.Windows.Visibility.Collapsed;
                    SprocketControlLoading.Visibility = System.Windows.Visibility.Collapsed;
                }));
            }
        }
        #endregion

        #region 属性值变化触发事件
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String StrPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(StrPropertyName));
            }
        }
        #endregion

        #region 将消息发送给子模块
        private string IStrClientGuid = string.Empty;
        public string SendMessage2SubFeature(ShareClassesInterface AEventArgs)
        {
            string LStrReturn = string.Empty;

            if (string.IsNullOrEmpty(IStrClientGuid))
            {
                LStrReturn = "Error001";
                return LStrReturn;
            }

            using (ChannelFactory<IServer2ClientMessage> LFactory = new ChannelFactory<IServer2ClientMessage>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/Client_" + IStrClientGuid.ToString())))
            {
                IServer2ClientMessage LServerToClientChannel = LFactory.CreateChannel();
                try
                {
                    LServerToClientChannel.CommandInClient(AEventArgs); 
                }
                catch (Exception ex)
                {
                    LStrReturn = "Error002" + ex.ToString();
                }
                finally
                {
                    LStrReturn = CloseCommunicationChannel((ICommunicationObject)LServerToClientChannel);
                }
            }
            
            return LStrReturn;
        }

        private string CloseCommunicationChannel(ICommunicationObject ACommunicationChannel)
        {
            string LStrReturn = string.Empty;
            try
            {
                ACommunicationChannel.Close();
            }
            catch (Exception ex)
            {
                LStrReturn = "CloseCommunicationChannel\n" + ex.ToString();
            }
            finally
            {
                ACommunicationChannel.Abort();
            }

            return LStrReturn;
        }
        #endregion
    }
}
