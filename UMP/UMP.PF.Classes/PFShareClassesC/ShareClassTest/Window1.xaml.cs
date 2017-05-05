using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace ShareClassTest
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public partial class Window1 : Window, IMessageServer2Client
    {
        private ServiceHost IServiceHost = null;
        private static string IStrClient = string.Empty;

        public Window1()
        {
            InitializeComponent();
            this.Loaded += Window1_Loaded;
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            //Guid LGuid = Guid.NewGuid();
            //IStrClient = LGuid.ToString();

            //IServiceHost = new ServiceHost(this);
            //IServiceHost.AddServiceEndpoint(typeof(IMessageServer2Client), new NetNamedPipeBinding(), "net.pipe://localhost/Client_" + IStrClient);
            //IServiceHost.Open();

            //ShareClassForInterface LEventArgs = new ShareClassForInterface();
            //LEventArgs.StrObjectTag = "LOADED";
            //LEventArgs.ObjObjectSource0 = IStrClient;
            //SendMessageToParent(LEventArgs);
        }

        #region 与主应用能程序通讯
        /// <summary>
        /// 与主应用能程序通讯
        /// </summary>
        /// <param name="AEventArgs"></param>
        /// <returns>00000000：发送成功；
        /// Error001：通讯Service未创建成功
        /// </returns>
        public string SendMessageToParent(ShareClassForInterface AEventArgs)
        {
            string LStrReturn = "00000000";

            try
            {

                using (ChannelFactory<IMessageClient2Server> LFactory = new ChannelFactory<IMessageClient2Server>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/UMPServer")))
                {
                    IMessageClient2Server LClientToServerChannel = LFactory.CreateChannel();
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
                ACommunicationChannel.Abort();
                //ACommunicationChannel.Close();
            }
            catch (Exception ex)
            {
                LStrReturn = "CloseCommunicationChannel\n" + ex.ToString();
            }
            finally
            {
                //ACommunicationChannel.Abort();
            }

            return LStrReturn;
        }
        #endregion

        public void CommandInClient(ShareClassForInterface AInterfaceArgs)
        {
            //MessageBox.Show(AInterfaceArgs.StrObjectTag, "CommandInClient");
            //TextBoxShow.Text = AInterfaceArgs.StrObjectTag + "\n";
            //string LStrClass = AInterfaceArgs.ObjObjectSource0 as string;
            //IntermediateArgs LTest = Deserialize(typeof(IntermediateArgs), LStrClass) as IntermediateArgs;
            //LTest.LongUserID = 1111111;
            //LTest.StrLoginAccount = "sa";
            //LTest.StrLoginUserName = "系统管理员";
            //TextBoxShow.Text += LTest.LongUserID.ToString() + "\n";
            //TextBoxShow.Text += LTest.StrLoginAccount + "\n";
            //TextBoxShow.Text += LTest.StrLoginUserName + "\n";
        }

        public object Deserialize(Type type, string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {

                return null;
            }
        }

    }
}
