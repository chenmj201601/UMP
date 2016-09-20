using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
//using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace WpfApplication1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            object lo = null;
            lo = ExecuteMethod<IService00000>("http://localhost:8081/Wcf2Client/Service00000.svc", "OperationMethodA", 2, new List<string>());
            OperationDataArgs Lr = lo as OperationDataArgs;
            MessageBox.Show("");
        }

        public static object ExecuteMethod<T>(string pUrl, string pMethodName, params object[] pParams)
        {
            EndpointAddress address = new EndpointAddress(pUrl);
            Binding bindinginstance = null;
            BasicHttpBinding ws = new BasicHttpBinding();
            ws.MaxReceivedMessageSize = 65535000;
            bindinginstance = ws;
            using (ChannelFactory<T> channel = new ChannelFactory<T>(bindinginstance, address))
            {
                T instance = channel.CreateChannel();
                using (instance as IDisposable)
                {
                    try
                    {
                        Type type = typeof(T);
                        MethodInfo mi = type.GetMethod(pMethodName);
                        return mi.Invoke(instance, pParams);
                    }
                    catch (TimeoutException)
                    {
                        (instance as ICommunicationObject).Abort();
                        throw;
                    }
                    catch (CommunicationException)
                    {
                        (instance as ICommunicationObject).Abort();
                        throw;
                    }
                    catch (Exception vErr)
                    {
                        (instance as ICommunicationObject).Abort();
                        MessageBox.Show(vErr.ToString());
                        return null;
                    }
                }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show( Regex.IsMatch("20150406031712", @"^\d{14}$").ToString());
            MessageBox.Show(Regex.IsMatch(DateTime.UtcNow.ToString(), @"^\d{14}$").ToString());
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show(DateTime.UtcNow.AddDays(-1).ToString("yyMMddHH"));
            MessageBox.Show(DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            TimeSpan LTimeSpan = new TimeSpan(0, 0, 3662);
            MessageBox.Show(LTimeSpan.Hours.ToString("00") + ":" + LTimeSpan.Minutes.ToString("00") + ":" + LTimeSpan.Seconds.ToString("00"));

            TimeSpan LTimeSpan1 = DateTime.Now - DateTime.UtcNow;
            MessageBox.Show(LTimeSpan1.TotalMinutes.ToString(), "123");

            string LStrTemp = @"c023ASc024CHZc0220c00300000c077ASCHZ000000020001000020150430051618784c020192.168.8.149c019WIN2008R2CMc0372c0380c0141c015MSGSMc0042015-04-30 13:16:18c0052015-04-30 05:16:18c00620150430051618c007130748445787840000c0082015-04-30 13:16:33c0092015-04-30 05:16:33c01020150430051633c011130748445937600000c0120c0428021c0218021c0250c0310c033Nc04075221460c04159003022c0450c0590c0600c0610c0640c0650c0630c0560c035D:\vox\201504\30\0002\00000\ASCHZ000000020001000020150430051618784.wavc0660";
            string LStrSpliterChar = AscCodeToChr(27);
            LStrTemp = LStrTemp.Replace(LStrSpliterChar + LStrSpliterChar, AscCodeToChr(30));
            string[] LStrArrayColumnAndData = LStrTemp.Split(AscCodeToChr(30).ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            MessageBox.Show(LStrArrayColumnAndData.Length.ToString());
        }

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string LStrTable21001Xml = string.Empty;

            LStrTable21001Xml = System.IO.Path.Combine(@"D:\UMP.PF.Site", @"UMPS3104.application");
            XmlDocument LXmlDocumentTable = new XmlDocument();
            LXmlDocumentTable.Load(LStrTable21001Xml);
            XmlNode LXMLNodeTableDefine = LXmlDocumentTable.SelectSingleNode("asmv1:assembly");//.SelectSingleNode("deployment");
            XmlNodeList LXmlNodeListTableColumns = LXMLNodeTableDefine.ChildNodes;
            foreach (XmlNode LSingleXmlNode in LXmlNodeListTableColumns)
            {

            }
        }
    }

    [ServiceContract]
    public interface IService00000
    {
        [OperationContract]
        OperationDataArgs OperationMethodA(int AIntOperationID, List<string> AListStringArgs);
    }

    [DataContract]
    public class OperationDataArgs
    {
        bool LBoolValue = true;
        string LStrValue = string.Empty;
        DataSet LDataSetValue = new DataSet();
        List<string> LListStrValue = new List<string>();
        List<DataSet> LListDataSetValue = new List<DataSet>();


        [DataMember]
        public bool BoolReturn
        {
            get { return LBoolValue; }
            set { LBoolValue = value; }
        }

        [DataMember]
        public string StringReturn
        {
            get { return LStrValue; }
            set { LStrValue = value; }
        }

        [DataMember]
        public DataSet DataSetReturn
        {
            get { return LDataSetValue; }
            set { LDataSetValue = value; }
        }

        [DataMember]
        public List<string> ListStringReturn
        {
            get { return LListStrValue; }
            set { LListStrValue = value; }
        }

        [DataMember]
        public List<DataSet> ListDataSetReturn
        {
            get { return LListDataSetValue; }
            set { LListDataSetValue = value; }
        }
    }
}
