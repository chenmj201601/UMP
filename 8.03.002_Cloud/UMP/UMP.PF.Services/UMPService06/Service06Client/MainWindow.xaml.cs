using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using PFShareClassesS;
using System.Threading;

namespace Service06Client
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

        #region


        /// <summary>
        /// 正常发消息时用
        /// </summary>
        /// <param name="ISslStream"></param>
        /// <param name="AStrMessage"></param>
        /// <returns></returns>
        private bool SendMessageToService(SslStream ISslStream, string AStrMessage)
        {
            string LStrSendMessage = string.Empty;
            bool ret = true;
            try
            {
                LStrSendMessage = AStrMessage;
                if (string.IsNullOrEmpty(LStrSendMessage)) { LStrSendMessage = " "; }
                LStrSendMessage += "\r\n";
                byte[] LByteMessage = Encoding.UTF8.GetBytes(LStrSendMessage);
                ISslStream.Write(LByteMessage);
                ISslStream.Flush();
            }
            catch (Exception ex)
            {

                return false;
            }
            finally
            {
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// //发消息不带\r\n  专用于发消息体太大时用
        /// </summary>
        /// <param name="ISslStream"></param>
        /// <param name="AStrMessage"></param>
        /// <returns></returns>
        private bool SendMessageToService1(SslStream ISslStream, string AStrMessage)
        {
            string LStrSendMessage = string.Empty;
            bool ret = true;
            try
            {
                LStrSendMessage = AStrMessage;
                if (string.IsNullOrEmpty(LStrSendMessage)) { LStrSendMessage = " "; }
                byte[] LByteMessage = Encoding.UTF8.GetBytes(LStrSendMessage);
                ISslStream.Write(LByteMessage);
                ISslStream.Flush();
            }
            catch (Exception ex)
            {

                return false;
            }
            finally
            {
                ret = true;
            }
            return ret;
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
                AStrReadedMessage = ex.ToString();
            }

            return LBoolReturn;
        }

        private string ReadMessageFromService(SslStream ISslStream)
        {
            string LStrReadedData = string.Empty;

            try
            {
                StringBuilder LStringBuilderData = new StringBuilder();
                int LIntReadedBytes = -1, LIntEndKeyPosition;
                byte[] LByteReadeBuffer = new byte[1024];

                do
                {
                    LIntReadedBytes = ISslStream.Read(LByteReadeBuffer, 0, LByteReadeBuffer.Length);
                    Decoder LDecoder = Encoding.UTF8.GetDecoder();
                    char[] LChars = new char[LDecoder.GetCharCount(LByteReadeBuffer, 0, LIntReadedBytes)];
                    LDecoder.GetChars(LByteReadeBuffer, 0, LIntReadedBytes, LChars, 0);
                    LStringBuilderData.Append(LChars);
                    if (LStringBuilderData.ToString().IndexOf("\r\n") > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                LStrReadedData = LStringBuilderData.ToString();
                LIntEndKeyPosition = LStrReadedData.IndexOf("\r\n");
                if (LIntEndKeyPosition > 0)
                {
                    LStrReadedData = LStrReadedData.Substring(0, LIntEndKeyPosition);
                }
            }
            catch (Exception ex)
            {
               
            }

            return LStrReadedData;
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

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            return false;
        }

        #endregion

        private void btnGetEncryRecord_Click(object sender, RoutedEventArgs e)
        {

            TcpClient LTcpClient = null;
            SslStream LSslStream = null;
            try
            {
                string LStrVerificationCode101 = string.Empty;
                string LStrVerificationCode104 = string.Empty;
                string LStrVerificationCode004 = string.Empty;
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                string LStrSendMessage = string.Empty;
                string LStrReadMessage = string.Empty;

                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("HelloService06", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LTcpClient = new TcpClient("192.168.9.76", 8075);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();

                if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                {
                    Thread.Sleep(100);
                    //查看32位密码
                    LStrReadMessage = EncryptionAndDecryption.EncryptDecryptString(LStrReadMessage, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(LStrReadMessage, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    if (SendMessageToService(LSslStream, LStrSendMessage))
                    {
                        //for (int i = 0; i < 100; i++) 
                        //{
                            
                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G001", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) + EncryptionAndDecryption.EncryptDecryptString("192.168.1.1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) + EncryptionAndDecryption.EncryptDecryptString("500", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                           // FileLog.WriteInfo("休息8 ms：" + i, "");  
                            Thread.Sleep(10);
                            LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                            LSslStream.Write(LByteMesssage); LSslStream.Flush();
                            //FileLog.WriteInfo("发送次数:" + i, "");                           
                            if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                            {
                               
                                string URL = string.Empty;
                                string Pass = string.Empty;

                                string[] ReadMessage = LStrReadMessage.Split(AscCodeToChr(27).ToArray());
                               
                                URL = ReadMessage[0];
                                URL = EncryptionAndDecryption.EncryptDecryptString(ReadMessage[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                                Pass = ReadMessage[1];
                                Pass = EncryptionAndDecryption.EncryptDecryptString(ReadMessage[1], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                                FileLog.WriteInfo("收到URL:" + EncryptionAndDecryption.EncryptDecryptString(ReadMessage[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104), "收到的密码：" + EncryptionAndDecryption.EncryptDecryptString(ReadMessage[1], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                            }
                        //}
                    }
                }
                else
                {
                    MessageBox.Show(LStrReadMessage);
                }

             }
            catch (AuthenticationException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); }
                if (LTcpClient != null) { LTcpClient.Close(); }
            }
        }

        private void btnWriteMark_Click(object sender, RoutedEventArgs e)
        {
            TcpClient LTcpClient = null;
            SslStream LSslStream = null;
            try
            {
                string LStrVerificationCode101 = string.Empty;
                string LStrVerificationCode104 = string.Empty;
                string LStrVerificationCode004 = string.Empty;
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                string LStrSendMessage = string.Empty;
                string LStrReadMessage = string.Empty;

                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("HelloService06", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LTcpClient = new TcpClient("192.168.9.76", 8075);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();

                if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                {
                    //查看32位密码
                    LStrReadMessage = EncryptionAndDecryption.EncryptDecryptString(LStrReadMessage, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(LStrReadMessage, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    if (SendMessageToService(LSslStream, LStrSendMessage))
                    {
                        string ss = "";//取小汤那边发送的消息串
                        while (ss.Length > 0)
                        {
                            string ssTemp = string.Empty;
                            if (ss.Length > 16384)
                            {
                                    ssTemp = ss.Substring(0, 16384);
                                    ss = ss.Substring(16384);
                            }
                            else 
                            {
                                ssTemp = ss;
                                ssTemp = ssTemp + "\r\n";
                                ss = string.Empty;
                            }
                            SendMessageToService1(LSslStream,ssTemp);
                        }
                      LStrReadMessage = string.Empty;
                      LStrReadMessage = ReadMessageFromService(LSslStream);
                      Console.WriteLine(LStrReadMessage);

                        //Thread.Sleep(100);
                        //for (int i = 0; i < 100; i++)
                        //{
                        //    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G002", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) +  //功能码
                        //        EncryptionAndDecryption.EncryptDecryptString("1234567891234567890", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) + //流水号
                        //        EncryptionAndDecryption.EncryptDecryptString("2", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) + //成功标志
                        //        EncryptionAndDecryption.EncryptDecryptString("444444444444444", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004)+AscCodeToChr(27)+//KeyID
                        //         EncryptionAndDecryption.EncryptDecryptString("5555555555555", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004)+AscCodeToChr(27)+//PolicyID
                        //          EncryptionAndDecryption.EncryptDecryptString("ASDFSAFSFSADDFASFD", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004)+AscCodeToChr(27)+//key1b
                        //           EncryptionAndDecryption.EncryptDecryptString("ASDFSAFSFSADDFASFD", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27)+//key1d
                        //            EncryptionAndDecryption.EncryptDecryptString("e0001", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27);//错误码

                        //    FileLog.WriteInfo("休息10Ms：" + i, "");
                        //    Thread.Sleep(10);
                        //    FileLog.WriteInfo("发送次数:" + i, "");
                        //    SendMessageToService(LSslStream, LStrSendMessage);
                        //}
                    }
                }
                else
                {
                    MessageBox.Show(LStrReadMessage);
                }

            }
            catch (AuthenticationException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); }
                if (LTcpClient != null) { LTcpClient.Close(); }
            }

        }

        private void btnNewGetOld_Click(object sender, RoutedEventArgs e)
        {

            TcpClient LTcpClient = null;
            SslStream LSslStream = null;
            try
            {
                string LStrVerificationCode101 = string.Empty;
                string LStrVerificationCode104 = string.Empty;
                string LStrVerificationCode004 = string.Empty;
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                string LStrSendMessage = string.Empty;
                string LStrReadMessage = string.Empty;

                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("HelloService06", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LTcpClient = new TcpClient("192.168.6.86", 8075);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();

                if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                {
                    //查看32位密码
                    LStrReadMessage = EncryptionAndDecryption.EncryptDecryptString(LStrReadMessage, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(LStrReadMessage, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    if (SendMessageToService(LSslStream, LStrSendMessage))
                    {
                        
                            ///@AInParam00 :    方法码
                            // @AInParam01 :	加密对象类型，目前参数值 = '1'
                            // @AInParam02 :	加密的对象， 目前参数值 = 录音服务器IP或录音服务器名
                            // @AInParam03 :	RecordReference
                            // @AInParam04 :	StartRecordTime，格式：'yyyy-MM-dd HH:mm:ss'
                            // @AInParam05 :	查询密钥截至时间，如果该值为空，则取GETDATE()
                            // @AInParam06 :	Key1b HH256
                            // @AInParam07 :	UserID，目前该参数不使用，将来以后验证是否有新密钥解老密钥的权限 暂时不用


                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G003", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) + 
                                EncryptionAndDecryption.EncryptDecryptString("1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) +
                                EncryptionAndDecryption.EncryptDecryptString("192.168.6.75", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004)+AscCodeToChr(27)+
                                EncryptionAndDecryption.EncryptDecryptString("15", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) +
                                EncryptionAndDecryption.EncryptDecryptString("2015-08-14 06:04:08", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) +
                                EncryptionAndDecryption.EncryptDecryptString(" ", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) +
                                EncryptionAndDecryption.EncryptDecryptString("445566", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27)+
                                EncryptionAndDecryption.EncryptDecryptString(" ", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) ;
                            FileLog.WriteInfo("G003发送消息：", LStrSendMessage);


                            LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                            LSslStream.Write(LByteMesssage); LSslStream.Flush();
                            ReadMessageFromServer(LSslStream, ref LStrSendMessage);
                            string[] ReadMessage = LStrSendMessage.Split(AscCodeToChr(27).ToArray());
                               
                               string KeyID=  EncryptionAndDecryption.EncryptDecryptString(ReadMessage[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                               string PolicyID = EncryptionAndDecryption.EncryptDecryptString(ReadMessage[1], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                               string Key1b = EncryptionAndDecryption.EncryptDecryptString(ReadMessage[2], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                                string Key1d=  EncryptionAndDecryption.EncryptDecryptString(ReadMessage[3], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                                FileLog.WriteInfo("btnNewGetOldKey()","收到keyid:" +KeyID+ "收到的PolicyId：" + PolicyID+"  Key1b:"+ Key1b+" Key1d:"+Key1d);
                        
                    }
                }
                else
                {
                    MessageBox.Show(LStrReadMessage);
                }

            }
            catch (AuthenticationException ex)
            {
                MessageBox.Show(ex.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); }
                if (LTcpClient != null) { LTcpClient.Close(); }
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //string IStrSpliterChar = AscCodeToChr(27);
            //string sss = "1548A4D55498677F104E1E81696303B8CD39D8E3949F3EA151070AD528AE36B70270791235E7C924F7495026A019989466309A105052AD7CFA0E543A516095326001C0FBB6432B8E64BB3AC91C6A5C20FB4410CED9428215EB417415B4A20E289498E735802C97B401D73D7603BF6E7E61D79D1FC82612490D8D21BCCFA4C963ACFFF6D75935AD892BAC7E33F607ADFA81D2A29D292DDCD753E90819FCA4C4E17CB0824DAF01E0A88A589B0EF3749D5F61F5E22BAF3720A7A56C80DC9FA429EAD50D0A21EE4F698537A4956C603BBBE00E6A14C6379558D7D15EF8F680B3CA8B53940F06F7F66BE3757DCE4844ACF314A1D406F463E7E74E76A1C7806936B13ADA7250D0BE8795CCFAE81603BE068015C79AD10CF32E080B2373BEE9777D6D7FAF6FB7A110FADBC4D0B8BD11B275452D6EB0CF91428A2EA69F501130D85076A1BDF85451D49E253038D12EB9E19729ADFFD82C519BD2DFAB265617165B5EE73FD5BCE172168A575BC2601E3C2BC0C894A93FB332A80BE6975C995E57CB33CA92AC6C8DBA72F52D2B369C3A715F5E3BA8DABBB68864F6C890E79B5D97E596592BD50357DE9EE4AF6B16FB08FECF69D8E08F52191CFEE4408D3B30A56B31719156368B0FE6C3B34D2CD4CE297381288C06D72E30BE0D10E12E110EBBABC3BC27B15E830E4A5487047BEDEF80A6FF4B368D20AD8518B57132B70A20AC5916DC69014180141E02C4CB7BCF50DAC253909AB8439776DE1B3C52E04C3685000C16B4EC8705C0757EDD7D04C1670BD78974F9B0086C0FB96E60967A0930D83283CF9416B90BA1CD2955D41CF996EE3B9C13025E76C6EE6336632097462E056CF20BED7F80F89771470A3D76C4E1989435AC118E83E25D26F67E73EE4868E4E071C1852636DC1F56ED5C485AECF0AD0DA09DC6E0A09D9EADE457D8844A315281753C74F80A9E6A2869F5C94B52070976ACFE3A2B5B20D86E5EA09B45E4DD497E24FC6C9A34AFFEF8BB26418DEF3BCF50C74339A2576417444C39FEFFE79AED7E2B0B4B884EA8198CBBED9E2B00B4BD157B6A5DBD2DB19878A09DF693FA2435C89584E1BC61BC28D0E674030C50DD2E0528B96D40FA7F8393B92037D562D39CC697DC16F0D56B0D34BD3C1F479FD5817A661B03C4D27D107CE71F2AD79B4E9DFFF88FBBF88D122B16D7E222EE3C87E457AF96A8EF66A26B927449FD9302229BA554FB7729";
            //string[] LStrUserSendData = sss.Split(IStrSpliterChar.ToArray());

            //int LIntDataLength = LStrUserSendData.Length;
            //string LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            //for (int LIntDataLoop = 0; LIntDataLoop < LIntDataLength; LIntDataLoop++)
            //{
            //    LStrUserSendData[LIntDataLoop] = EncryptionAndDecryption.EncryptDecryptString(LStrUserSendData[LIntDataLoop], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            //}

            //string[] LstrG002 = LStrUserSendData[1].Split(IStrSpliterChar.ToArray());

            string ss = "93464B864C3E105312FA624E834FEA8D4EF618CC36A14F289D85B964E89B7C6C09D6FC0A2F7FCAE81CCA67F627AD5FE8796B577213760C74AC1CA30D8A103EE6995149A4BDE98F604826F4F12E75739AD0E25EF6EEDAE6FA656030F61875A080FDA5469B25EB5D9A79FC8EFFCEAB2CE33B1ADB3D9BBAB5B7C2E9A387387842DD";
            string d = "93464B864C3E105312FA624E834FEA8D";
            string LStrVerificationCode025 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M025);
            string LStrVerificationCode125 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M125);
            ss = EncryptionAndDecryption.EncryptDecryptString(ss, LStrVerificationCode125, EncryptionAndDecryption.UMPKeyAndIVType.M125);
            if(ss.Equals("333333"))
            {
                MessageBox.Show(ss);
            }

        }

        public static DateTime StringToDateTime(string source)
        {
            if (source.Length < 14)
            {
                return DateTime.Parse("2100-1-1 00:00:00"); 
            }
            DateTime dt;
            string strTime = source.Substring(0, 4) + "-";
            strTime += source.Substring(4, 2) + "-";
            strTime += source.Substring(6, 2) + " ";
            strTime += source.Substring(8, 2) + ":";
            strTime += source.Substring(10, 2) + ":";
            strTime += source.Substring(12, 2) ;
           
            //dt = DateTime.Parse(strTime);
            dt = DateTime.Parse(DateTime.Parse(strTime).ToString("yyyy-MM-dd HH:mm:ss"));
            return dt;
        }
    }
}
