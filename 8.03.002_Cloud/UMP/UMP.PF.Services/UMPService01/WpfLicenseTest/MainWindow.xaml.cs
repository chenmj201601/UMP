using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PFShareClassesS;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using WpfTest.Wcf1102;

namespace WpfTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string LStr11003C008 = string.Empty;
        string LStrVerificationCode002 = string.Empty;
        string LStr11003C007 = string.Empty;


        public MainWindow()
        {
            InitializeComponent();

            ComboBox_DogNum.Text = "11-10528150";
            ComboBox_LongValue.Text = "1100101";
            ComboBox_OptNum.Text = "Y";
        }

        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            string LStrTemp = string.Empty;


            try
            {
                Random LRandom = new Random();
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

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        private string CreateMD5HashString(string AStrSource)
        {
            string LStrHashPassword = string.Empty;
            try
            {
                MD5CryptoServiceProvider LMD5Crypto = new MD5CryptoServiceProvider();
                byte[] LByteArray = Encoding.Unicode.GetBytes(AStrSource);
                LByteArray = LMD5Crypto.ComputeHash(LByteArray);
                StringBuilder LStrBuilder = new StringBuilder();
                foreach (byte LByte in LByteArray) { LStrBuilder.Append(LByte.ToString("X2").ToUpper()); }
                LStrHashPassword = LStrBuilder.ToString();
            }
            catch { LStrHashPassword = "YoungPassword"; }
            return LStrHashPassword;
        }

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string LStr11003C008Hash8 = string.Empty;
            long longValue;
            
            long.TryParse(ComboBox_LongValue.Text, out longValue);
            longValue = longValue + 1000000000;
            LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
            LStr11003C008 = EncryptionAndDecryption.EncryptDecryptString(longValue + ComboBox_DogNum.Text, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
            LStr11003C008Hash8 = CreateMD5HashString(LStr11003C008).Substring(0, 8);
            LStr11003C007 = longValue + AscCodeToChr(27) + ComboBox_OptNum.Text;
            LStr11003C007 = EncryptionAndDecryption.EncryptStringYKeyIV(LStr11003C007, LStr11003C008Hash8, LStr11003C008Hash8);
            ComboBox_C007.Text = LStr11003C007;
            InitOperations();
        }

        private void InitOperations()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(ComboBox_moduleID.Text);
                string parentID = ComboBox_parentID.Text;
                webRequest.ListData.Add(parentID);
                //                 Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                //                     WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                var client = new Service11012Client();
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("InitOperations Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("InitOperations Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                    }
                }

                App.WriteLog("PageInit", string.Format("Init UserOperation"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

    }
}
