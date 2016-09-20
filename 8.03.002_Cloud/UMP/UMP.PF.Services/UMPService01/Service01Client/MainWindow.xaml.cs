using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
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

namespace Service01Client
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string LStrSendMessage = string.Empty;
            string LStrReadMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            TcpClient LTcpClient = null;
            SslStream LSslStream = null;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                //LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01A21", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004); //0
                //LStrSendMessage += AscCodeToChr(27) + "00000";      //1
                //LStrSendMessage += AscCodeToChr(27) + "1020000000000000001";      //2
                //LStrSendMessage += AscCodeToChr(27) + "voicecyber";      //3
                //LStrSendMessage += AscCodeToChr(27) + "11";      //4
                //LStrSendMessage += AscCodeToChr(27) + "C001";      //5
                //LStrSendMessage += AscCodeToChr(27) + "";      //6

                LStrSendMessage = "C5BF97E4AB9397FBC92246B5C1DB34DD"; //0
                LStrSendMessage += AscCodeToChr(27) + "836D20562EFA5939A39C717A665C1FD4";      //1
                LStrSendMessage += AscCodeToChr(27) + "A17AA7A591C8281B9C7F664C962E961BF6E7C83873E1BEB01CD9649D612F4B712DF2A3B065376555C8937B41797CFB6B";      //2
                LStrSendMessage += AscCodeToChr(27) + "9F50ACE21BD20FFDCA0EB798C985436E741956BE9079BB5C2AFED0B29F8CDFF4";      //3
                LStrSendMessage += AscCodeToChr(27) + "C3E9DF59F9742B9FF7D4E7A2470D45C7B245214ED1A1BA5630DBBFB557C9BB4584B926D07AFE67A2E0D5941DCB1F2C0995343A56115FD9945C93EFD78617CB7DB3058A073516489BCDB89CEC3BD139AF";      //4
                LStrSendMessage += AscCodeToChr(27) + "C001";      //5
                LStrSendMessage += AscCodeToChr(27) + "";      //6


                LTcpClient = new TcpClient("192.168.4.184", 8080);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Tls, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();
                ReadMessageFromServer(LSslStream, ref LStrReadMessage);
                MessageBox.Show(LStrReadMessage);
            }
            catch (AuthenticationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); }
                if (LTcpClient != null) { LTcpClient.Close(); }
            }
        }

        #region 与 Service 01 通讯用
        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            return false;
        }

        private bool ReadMessageFromServer(SslStream ASslStream, ref string AStrReadedMessage)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReadedMessage = string.Empty;

                StringBuilder LStringBuilderData = new StringBuilder();
                int LIntReadedBytes = -1, LIntEndKeyPosition;
                byte[] LByteReadeBuffer = new byte[1024];

                do
                {
                    LIntReadedBytes = ASslStream.Read(LByteReadeBuffer, 0, LByteReadeBuffer.Length);
                    Decoder LDecoder = Encoding.UTF8.GetDecoder();
                    char[] LChars = new char[LDecoder.GetCharCount(LByteReadeBuffer, 0, LIntReadedBytes)];
                    LDecoder.GetChars(LByteReadeBuffer, 0, LIntReadedBytes, LChars, 0);
                    LStringBuilderData.Append(LChars);
                    if (LStringBuilderData.ToString().IndexOf("\r\n") > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                AStrReadedMessage = LStringBuilderData.ToString();
                LIntEndKeyPosition = AStrReadedMessage.IndexOf("\r\n");
                if (LIntEndKeyPosition > 0)
                {
                    AStrReadedMessage = AStrReadedMessage.Substring(0, LIntEndKeyPosition);
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReadedMessage = ex.Message;
            }

            return LBoolReturn;
        }
        #endregion

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

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(PFShareClassesS.PasswordVerifyOptions.GeneratePassword(32, 5));
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string LStrSendMessage = string.Empty;
            string LStrReadMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            TcpClient LTcpClient = null;
            SslStream LSslStream = null;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);


                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01E02", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004); //0
                LStrSendMessage += AscCodeToChr(27) + "11223344";      //1
                LStrSendMessage += AscCodeToChr(27) + "0";      //2
                LStrSendMessage += AscCodeToChr(27) + "1";      //3
                


                LTcpClient = new TcpClient("127.0.0.1", 8080);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Tls, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();
                ReadMessageFromServer(LSslStream, ref LStrReadMessage);
                MessageBox.Show(LStrReadMessage);
            }
            catch (AuthenticationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); }
                if (LTcpClient != null) { LTcpClient.Close(); }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            string LStrSendMessage = string.Empty;
            string LStrReadMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            TcpClient LTcpClient = null;
            SslStream LSslStream = null;


            try
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="AStrArrayConditons">
                /// 0-操作码
                /// 1-媒体类型 1，2
                /// 2-月份 YYYYMM
                /// 3-机器名
                /// 4-IP地址列表  IP+ CHAR(30) + IP .....
                /// </param>
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01E01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004); //0
                LStrSendMessage += AscCodeToChr(27) + "1";      //1
                LStrSendMessage += AscCodeToChr(27) + "201501";      //2
                LStrSendMessage += AscCodeToChr(27) + "MachineName";      //3
                LStrSendMessage += AscCodeToChr(27) + "192.168.4.14" + AscCodeToChr(30) + "192.168.4.10";     //4
                


                LTcpClient = new TcpClient("127.0.0.1", 8080);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Tls, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();
                ReadMessageFromServer(LSslStream, ref LStrReadMessage);
                MessageBox.Show(LStrReadMessage);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); }
                if (LTcpClient != null) { LTcpClient.Close(); }
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            ulong LULong = 7;

            MessageBox.Show((LULong % 5).ToString());
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            string IStrSiteRootFolder = @"D:\UMP.PF.Site";
            string[] LStrArrayWriteBatLine = new string[7];
            Stream LStreamBatch = File.Create(IStrSiteRootFolder + @"\MageConfig.bat");
            LStreamBatch.Close();

            LStrArrayWriteBatLine[0] = "cd \"" + IStrSiteRootFolder + "\"";
            LStrArrayWriteBatLine[1] = "mage -n Application -t \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.exe.manifest\" -n UMPS3104.exe -v 8.02.001.1 -fd \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\"";
            LStrArrayWriteBatLine[2] = "mage -s \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.exe.manifest\" -cf \"" + IStrSiteRootFolder + "\\Components\\Certificates\\UMP.PF.Certificate.pfx\" -pwd \"VoiceCyber,123\"";
            LStrArrayWriteBatLine[3] = "mage -n Deployment -t \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.application\" -n \"UMP Interlligent Client\" -v 8.02.001.1 -appm \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.exe.manifest\" -install true -Publisher \"VoiceCyber\"";
            LStrArrayWriteBatLine[4] = "mage -s \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.application\" -cf \"" + IStrSiteRootFolder + "\\Components\\Certificates\\UMP.PF.Certificate.pfx\" -pwd \"VoiceCyber,123\"";
            LStrArrayWriteBatLine[5] = "mage -n Deployment -t \"" + IStrSiteRootFolder + "\\UMPS3104.application\" -n \"UMP Interlligent Client\" -v 8.02.001.1 -appm \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.exe.manifest\" -install true -Publisher \"VoiceCyber\"";
            LStrArrayWriteBatLine[6] = "mage -s \"" + IStrSiteRootFolder + "\\UMPS3104.application\" -cf \"" + IStrSiteRootFolder + "\\Components\\Certificates\\UMP.PF.Certificate.pfx\" -pwd \"VoiceCyber,123\"";

            File.WriteAllLines(IStrSiteRootFolder + @"\MageConfig.bat", LStrArrayWriteBatLine);
        }
    }
}
