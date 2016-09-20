using PFShareClasses01;
using PFShareClassesC;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace ShareClassTest
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public partial class MainWindow : Window, IMessageClient2Server, INotifyPropertyChanged
    {
        private ServiceHost IServiceHost = null;

        /// <summary>
        /// 功能编号
        /// </summary>
        private string _StrClientGuid = string.Empty;
        public string StrClientGuid
        {
            get { return _StrClientGuid; }
            set { _StrClientGuid = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            if (IServiceHost != null)
            {
                IServiceHost.Close();
            }
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IServiceHost = new ServiceHost(this);
            IServiceHost.AddServiceEndpoint(typeof(IMessageClient2Server), new NetNamedPipeBinding(), "net.pipe://localhost/UMPServer");
            IServiceHost.Open();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window1 lW = new Window1();
            lW.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IntermediateArgs LTest = new IntermediateArgs();
            LTest.LongUserID = 1111111;
            LTest.StrLoginAccount = "sa";
            LTest.StrLoginUserName = "系统管理员";

            string LStrXml = Serializer(typeof(IntermediateArgs), LTest);
            ShareClassForInterface LEventArgs = new ShareClassForInterface();
            LEventArgs.StrObjectTag = "Send Test";
            LEventArgs.ObjObjectSource0 = LStrXml;
            SendMessage2SubFeature(LEventArgs);
        }

        public string Serializer(Type type, object obj)
        {
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                //序列化对象
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();

            sr.Dispose();
            Stream.Dispose();

            return str;
        }


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

        public void ProcessingClientMessage(ShareClassForInterface AInterfaceArgs)
        {
            IStrClientGuid = AInterfaceArgs.ObjObjectSource0.ToString();
            MessageBox.Show(IStrClientGuid, "ProcessingClientMessage");
            if (AInterfaceArgs.StrObjectTag == "LOADED")
            {
                IStrClientGuid = AInterfaceArgs.ObjObjectSource0.ToString();
            }
        }

        #region 将消息发送给子模块
        private string IStrClientGuid = string.Empty;
        public string SendMessage2SubFeature(ShareClassForInterface AEventArgs)
        {
            string LStrReturn = string.Empty;

            if (string.IsNullOrEmpty(IStrClientGuid))
            {
                LStrReturn = "Error001";
                return LStrReturn;
            }

            using (ChannelFactory<IMessageServer2Client> LFactory = new ChannelFactory<IMessageServer2Client>(new NetNamedPipeBinding(), new EndpointAddress("net.pipe://localhost/Client_" + IStrClientGuid.ToString())))
            {
                IMessageServer2Client LServerToClientChannel = LFactory.CreateChannel();
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(PFShareClassesC.EncryptionAndDecryption.EncryptDecryptString("YoungYang", CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M001), PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M001));
            //MessageBox.Show(DateTime.Now.ToString("yyyyMMddHHmmss"));
            MessageBox.Show(PFShareClassesC.EncryptionAndDecryption.EncryptDecryptString("65CB86C60F6D8A68871975F9A8F72C63FFF59DAE5C15DDD8DE77885229FBD3C5", CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M101), PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M101));
            
        }

        private string CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            Random LRandom = new Random();
            string LStrTemp = string.Empty;

            try
            {
                LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = LRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                LIntRand = LRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                LIntRand = LRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                LStrReturn = PFShareClassesC.EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            string LStrSource = string.Empty;
            string LStrTarget = string.Empty;
            string LStrVerificationCode = string.Empty;

            LStrSource = TextBoxSource.Text.Trim();
            LStrVerificationCode = CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M002);
            LStrTarget = PFShareClassesS.EncryptionAndDecryption.EncryptDecryptString(LStrSource, LStrVerificationCode, PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M002);
            TextBoxTarget.Text = LStrTarget;
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            string LStrSource = string.Empty;
            string LStrTarget = string.Empty;
            string LStrVerificationCode = string.Empty;

            LStrSource = TextBoxSource.Text.Trim();
            LStrVerificationCode = CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M002);
            LStrTarget = PFShareClassesS.EncryptionAndDecryption.EncryptStringSHA512(LStrSource, LStrVerificationCode, PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M002);
            TextBoxTarget.Text = LStrTarget;
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            string LStrSource = string.Empty;
            string LStrTarget = string.Empty;
            string LStrVerificationCode = string.Empty;

            LStrSource = TextBoxTarget.Text.Trim();
            LStrVerificationCode = CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M102);
            LStrTarget = PFShareClassesS.EncryptionAndDecryption.EncryptDecryptString(LStrSource, LStrVerificationCode, PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M102);
            TextBoxSource.Text = LStrTarget;
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            DateTime dt1 = Convert.ToDateTime("2007-8-14");
            DateTime dt2 = Convert.ToDateTime("2007-8-15");
            TimeSpan span = dt2.Subtract(dt1);
            int dayDiff = span.Days + 1;

            MessageBox.Show(dayDiff.ToString());
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(DateTime.Now.ToString("yyyyMMddHHmmss"));
            //Int16 L16 = Int16.Parse("44000");
            long LL = long.Parse((DateTime.Parse("2014-12-01 03:09:39")).ToString("yyyyMMddHHmmss"));
            MessageBox.Show(LL.ToString());
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            DataOperations01 LDataOperation = new DataOperations01();
            DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
            //LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(3, "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.4.182) (PORT=1521)))(CONNECT_DATA=(SERVICE_NAME= PFOrcl)));User Id=PFDEV; Password=PF,123", 11, 901, "00000", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
            LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(2, "Data Source=192.168.4.186,1433;Initial Catalog=UMPDataDB;User Id=PFDEV;Password=PF,123", 11, 901, "00000", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            string LStrSource = string.Empty;
            string LStrTarget = string.Empty;
            string LStrVerificationCode = string.Empty;

            LStrSource = TextBoxTarget.Text.Trim();
            LStrVerificationCode = CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrTarget = PFShareClassesS.EncryptionAndDecryption.EncryptDecryptString(LStrSource, LStrVerificationCode, PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M104);
            TextBoxSource.Text = LStrTarget;
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            string LStrSource = string.Empty;
            string LStrTarget = string.Empty;
            string LStrVerificationCode = string.Empty;

            LStrSource = TextBoxSource.Text.Trim();
            LStrVerificationCode = CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M001);
            LStrTarget = PFShareClassesS.EncryptionAndDecryption.EncryptDecryptString(LStrSource, LStrVerificationCode, PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M001);
            TextBoxTarget.Text = LStrTarget;
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            string LStrSource = string.Empty;
            string LStrTarget = string.Empty;
            string LStrVerificationCode = string.Empty;

            LStrSource = TextBoxTarget.Text.Trim();
            LStrVerificationCode = CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M101);
            LStrTarget = PFShareClassesS.EncryptionAndDecryption.EncryptDecryptString(LStrSource, LStrVerificationCode, PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M101);
            TextBoxSource.Text = LStrTarget;
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            string LStrSource = string.Empty;
            string LStrTarget = string.Empty;
            string LStrVerificationCode = string.Empty;

            LStrSource = TextBoxSource.Text.Trim();
            LStrVerificationCode = CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M001);
            LStrTarget = PFShareClassesS.EncryptionAndDecryption.EncryptStringSHA512(LStrSource, LStrVerificationCode, PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M001);
            TextBoxTarget.Text = LStrTarget;
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            string[] LStrTemp = new string[] { "1", "2", "3" };
            MessageBox.Show(string.Join("-", LStrTemp));
        }

        private void Button_Click_14(object sender, RoutedEventArgs e)
        {
            string LStrSource = string.Empty;
            string LStrTarget = string.Empty;
            string LStrVerificationCode = string.Empty;

            LStrSource = TextBoxSource.Text.Trim();
            LStrVerificationCode = CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M001);
            LStrTarget = PFShareClassesS.EncryptionAndDecryption.EncryptStringSHA256(LStrSource);
            TextBoxTarget.Text = LStrTarget;
        }

        private void Button_Click_15(object sender, RoutedEventArgs e)
        {
            string LStrSource = string.Empty;
            List<bool> LListBoolIsMatch = new List<bool>();

            LStrSource = TextBoxSource.Text;

            //必须包含数字
            Regex LRegex1 = new Regex("(?=.*[0-9])", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            //必须包含小写字母
            Regex LRegex2 = new Regex("(?=.*[a-z])", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            //必须包含大写字母
            Regex LRegex3 = new Regex("(?=.*[A-Z])", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            //必须包含特殊符号
            Regex LRegex4 = new Regex("(?=([\x21-\x7e]+)[^a-zA-Z0-9])", RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);

            LListBoolIsMatch.Add(LRegex1.IsMatch(LStrSource));
            LListBoolIsMatch.Add(LRegex2.IsMatch(LStrSource));
            LListBoolIsMatch.Add(LRegex3.IsMatch(LStrSource));
            LListBoolIsMatch.Add(LRegex4.IsMatch(LStrSource));

            MessageBox.Show("");
        }

        private void Button_Click_16(object sender, RoutedEventArgs e)
        {
            string LStrSource = string.Empty;
            string LStrTarget = string.Empty;
            string LStrVerificationCode = string.Empty;

            LStrSource = TextBoxTarget.Text.Trim();
            LStrVerificationCode = CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M103);
            LStrTarget = PFShareClassesS.EncryptionAndDecryption.EncryptDecryptString(LStrSource, LStrVerificationCode, PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M103);
            TextBoxSource.Text = LStrTarget;
        }
    }
}
