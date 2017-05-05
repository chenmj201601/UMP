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

namespace YoungControlLibrary
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public partial class FeatureHeader : UserControl, INotifyPropertyChanged, IServer2ClientMessage, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        #region 属性定义，界面XAML中使用

        private string _StrBackArrowSource;
        /// <summary>
        /// 返回的图标
        /// </summary>
        public string StrBackArrowSource
        {
            get { return _StrBackArrowSource; }
            set
            {
                _StrBackArrowSource = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrBackArrowSource");
                }
            }
        }

        private string _StrFeatureImageSource;
        /// <summary>
        /// 功能的图标
        /// </summary>
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

        private string _StrFeatureContent;
        /// <summary>
        /// 功能显示的文字
        /// </summary>
        public string StrFeatureContent
        {
            get { return _StrFeatureContent; }
            set
            {
                _StrFeatureContent = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrFeatureContent");
                }
            }
        }

        private string _StrLoginAccountSource;
        /// <summary>
        /// 登录账户图片
        /// </summary>
        public string StrLoginAccountSource
        {
            get { return _StrLoginAccountSource; }
            set
            {
                _StrLoginAccountSource = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrLoginAccountSource");
                }
            }
        }

        private string _StrLoginAccountText;
        /// <summary>
        /// 登录账户和账户名
        /// </summary>
        public string StrLoginAccountText
        {
            get { return _StrLoginAccountText; }
            set
            {
                _StrLoginAccountText = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrLoginAccountText");
                }
            }
        }

        private string _StrNotesTipSource;
        /// <summary>
        /// 消息提醒的图片
        /// </summary>
        public string StrNotesTipSource
        {
            get { return _StrNotesTipSource; }
            set
            {
                _StrNotesTipSource = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrNotesTipSource");
                }
            }
        }

        private int _IntNotesCount;
        /// <summary>
        /// 提醒消息的总数量
        /// </summary>
        public int IIntNotesCount
        {
            get { return _IntNotesCount; }
            set
            {
                _IntNotesCount = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("IIntNotesCount");
                }
            }
        }
        #endregion


        /// <summary>
        /// net.pipe 通讯的 GUID
        /// </summary>
        private static string IStrClient = string.Empty;

        public IntermediateSingleFeature IIntermediateParam = new IntermediateSingleFeature();

        public FeatureHeader()
        {
            InitializeComponent();
        }
        
        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }

        public void CommandInClient(ShareClassesInterface AInterfaceArgs)
        {
            try
            {
                if (AInterfaceArgs.StrObjectTag == "SetValue")
                {
                    
                    IIntermediateParam.StrLoginSerialID = AInterfaceArgs.ListObjectSource[0].ToString();
                    IIntermediateParam.StrLoginAccount = AInterfaceArgs.ListObjectSource[1].ToString();
                    IIntermediateParam.StrLoginUserName = AInterfaceArgs.ListObjectSource[2].ToString();
                    IIntermediateParam.StrFeatureID = AInterfaceArgs.ListObjectSource[3].ToString();
                    IIntermediateParam.StrFeatureName = AInterfaceArgs.ListObjectSource[4].ToString();
                    IIntermediateParam.StrFeatureImage = AInterfaceArgs.ListObjectSource[5].ToString();
                    IIntermediateParam.StrImageSize = AInterfaceArgs.ListObjectSource[6].ToString();
                    IIntermediateParam.StrUseProtol = AInterfaceArgs.ListObjectSource[7].ToString();
                    IIntermediateParam.StrServerHost = AInterfaceArgs.ListObjectSource[8].ToString();
                    IIntermediateParam.StrServerPort = AInterfaceArgs.ListObjectSource[9].ToString();
                    IIntermediateParam.StrUseStyle = AInterfaceArgs.ListObjectSource[10].ToString();
                    IIntermediateParam.StrOpenXbap = AInterfaceArgs.ListObjectSource[11].ToString();
                    IIntermediateParam.StrXbapArgs = AInterfaceArgs.ListObjectSource[12].ToString();

                    StrBackArrowSource = IIntermediateParam.StrUseProtol + "://" + IIntermediateParam.StrServerHost + ":" + IIntermediateParam.StrServerPort + "/Themes/" + IIntermediateParam.StrUseStyle + "/BackArrow.png";
                    //StrFeatureImageSource = IIntermediateParam.StrUseProtol + "://" + IIntermediateParam.StrServerHost + ":" + IIntermediateParam.StrServerPort + "/Themes/" + IIntermediateParam.StrUseStyle + "/VF_" + IIntermediateParam.StrFeatureImage;
                    StrFeatureContent = IIntermediateParam.StrFeatureName;
                    StrLoginAccountSource = IIntermediateParam.StrUseProtol + "://" + IIntermediateParam.StrServerHost + ":" + IIntermediateParam.StrServerPort + "/Themes/" + IIntermediateParam.StrUseStyle + "/LoginAccount.png";
                    StrLoginAccountText = IIntermediateParam.StrLoginAccount + " (" + IIntermediateParam.StrLoginUserName + ")";
                    StrNotesTipSource = IIntermediateParam.StrUseProtol + "://" + IIntermediateParam.StrServerHost + ":" + IIntermediateParam.StrServerPort + "/Themes/" + IIntermediateParam.StrUseStyle + "/NoteTip.png";
                    IIntNotesCount = 0;
                    
                    OperationEventArgs LEventArgs = new OperationEventArgs();
                    LEventArgs.StrObjectTag = "INIT";
                    LEventArgs.ObjectSource0 = IIntermediateParam;
                    SendMessgeToRefrence(LEventArgs);
                    
                }
                else
                {
                    OperationEventArgs LEventArgs = new OperationEventArgs();
                    LEventArgs.StrObjectTag = AInterfaceArgs.StrObjectTag;
                    LEventArgs.ObjectSource0 = AInterfaceArgs.ObjObjectSource0;
                    LEventArgs.ObjectSource1 = AInterfaceArgs.ObjObjectSource1;
                    LEventArgs.ObjectSource2 = AInterfaceArgs.ObjObjectSource2;
                    LEventArgs.ObjectSource3 = AInterfaceArgs.ObjObjectSource3;
                    LEventArgs.ObjectSource4 = AInterfaceArgs.ObjObjectSource4;
                    LEventArgs.ListObjectSource = AInterfaceArgs.ListObjectSource;
                    SendMessgeToRefrence(LEventArgs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "CommandInClient");
            }
        }

        private void SendMessgeToRefrence(OperationEventArgs AEventArgs)
        {
            if (IOperationEvent != null)
            {
                IOperationEvent(this, AEventArgs);
            }
        }

        #region 初始化ServiceHost，用来与主应用模块通讯

        BackgroundWorker IBackgroundWorkerInitClientHost = null;
        private static ServiceHost IServiceHost = null;
        public bool IBoolServiceIsExists = false;

        /// <summary>
        /// 初始化通讯用的Service，一般指模块主页面加载完毕后，调用该方法，通知主应用程序显示该模块的WebBrower
        /// </summary>
        /// <param name="AStrClientID">模块编号。如 1101，3101 等</param>
        public void InitClientHost(string AStrClientID)
        {
            if (!string.IsNullOrEmpty(AStrClientID))
            {
                IStrClient = AStrClientID;
            }
            else
            {
                Guid LGuid = Guid.NewGuid();
                IStrClient = LGuid.ToString();
            }
            if (IBackgroundWorkerInitClientHost == null)
            {
                IBackgroundWorkerInitClientHost = new BackgroundWorker();
            }
            IBackgroundWorkerInitClientHost.RunWorkerCompleted += IBackgroundWorkerInitClientHost_RunWorkerCompleted;
            IBackgroundWorkerInitClientHost.DoWork += IBackgroundWorkerInitClientHost_DoWork;
            IBackgroundWorkerInitClientHost.RunWorkerAsync();
        }

        private void IBackgroundWorkerInitClientHost_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                IServiceHost = new ServiceHost(this);
                IServiceHost.AddServiceEndpoint(typeof(IServer2ClientMessage), new NetNamedPipeBinding(), "net.pipe://localhost/Client_" + IStrClient);
                IServiceHost.Open();
                IBoolServiceIsExists = true;
            }
            catch { IBoolServiceIsExists = false; }
        }

        private void IBackgroundWorkerInitClientHost_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                ShareClassesInterface LEventArgs = new ShareClassesInterface();
                LEventArgs.StrObjectTag = "LOADED";
                LEventArgs.ObjObjectSource0 = IStrClient;
                SendMessageToParent(LEventArgs);
            }
            catch { }
        }

        #endregion

        #region 与主应用能程序通讯
        /// <summary>
        /// 与主应用能程序通讯
        /// </summary>
        /// <param name="AEventArgs"></param>
        /// <returns>00000000：发送成功；
        /// Error001：通讯Service未创建成功
        /// </returns>
        public string SendMessageToParent(ShareClassesInterface AEventArgs)
        {
            string LStrReturn = "00000000";

            try
            {
                if (IBoolServiceIsExists == false)
                {
                    LStrReturn = "Error001";
                    return LStrReturn;
                }
                using (ChannelFactory<IClient2ServerMessage> LFactory = new ChannelFactory<IClient2ServerMessage>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/UMPServer")))
                {
                    IClient2ServerMessage LClientToServerChannel = LFactory.CreateChannel();
                    try
                    {
                        LClientToServerChannel.ProcessingClientMessage(AEventArgs);
                    }
                    catch (Exception ex)
                    {
                        LStrReturn = "Error002" + ex.ToString();
                    }
                    finally
                    {
                        LStrReturn = CloseCommunicationChannel((ICommunicationObject)LClientToServerChannel);
                    }
                }
            }
            catch { }

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

        #region 显示或关闭返回图标
        /// <summary>
        /// 显示或关闭返回图标
        /// </summary>
        /// <param name="IBoolShow">
        /// true：显示
        /// false：关闭
        /// </param>
        private void ShowBackImage(bool IBoolShow)
        {
            if (IBoolShow == true)
            {
                ImageBackArrow.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                ImageBackArrow.Visibility = System.Windows.Visibility.Collapsed;
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
    }
}
