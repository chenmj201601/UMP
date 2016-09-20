using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using PFShareClassesC;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using System.Collections.Generic;
using System;

namespace UMPS3104.S3102Codes
{
    public class Service06Helper
    {
        public static OperationReturn DoOperation(Service06ServerInfo server, string strCommand, List<string> listArgs)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                TcpClient tcpClient = new TcpClient(AddressFamily.InterNetwork);
                tcpClient.Connect(server.Host, server.Port);
                if (!tcpClient.Connected)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_CONNECTED;
                    optReturn.Message = string.Format("Server not connected");
                    return optReturn;
                }
                SslStream stream = new SslStream(tcpClient.GetStream(), false, (s, cert, chain, err) => true);
                stream.AuthenticateAsClient(server.Host);

                //发送Hello消息（用M004加密）
                string strMsg = string.Format("HelloService06");
                strMsg = EncryptString(strMsg);
                optReturn = SendMessage(stream, strMsg);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                //接收认证码
                optReturn = ReadMessage(stream);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                strMsg = optReturn.Data.ToString();
                //处理认证码并返回处理后的认证码
                //先用M101解密，然后再用M004加密后返回
                strMsg = DecryptString001(strMsg);
                strMsg = EncryptString(strMsg);
                strMsg += "\r\n";
                optReturn = SendMessage(stream, strMsg);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                ////接收认证结果
                //optReturn = ReadMessage(stream);
                //if (!optReturn.Result)
                //{
                //    return optReturn;
                //}

                //发送请求指令
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("{0}{1}", EncryptString(strCommand), ConstValue.SPLITER_CHAR));
                for (int i = 0; i < listArgs.Count; i++)
                {
                    sb.Append(string.Format("{0}{1}", EncryptString(listArgs[i]), ConstValue.SPLITER_CHAR));
                }
                strMsg = sb.ToString();
                if (strMsg.Length > 0)
                {
                    strMsg = strMsg.Substring(0, strMsg.Length - 1);
                }
                strMsg += "\r\n";
                optReturn = SendMessage(stream, strMsg);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                //接收返回消息
                optReturn = ReadMessage(stream);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private static OperationReturn ReadMessage(Stream stream)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strReadedMessage;
                StringBuilder sb = new StringBuilder();
                int size;
                string str;
                byte[] buffer = new byte[1024];
                do
                {
                    size = stream.Read(buffer, 0, 1024);
                    str = Encoding.UTF8.GetString(buffer, 0, size);
                    sb.Append(str);
                    string temp = sb.ToString();
                    if (temp.IndexOf("\r\n", StringComparison.Ordinal) > 0) { break; }
                } while (size != 0);
                strReadedMessage = sb.ToString();
                int index = strReadedMessage.IndexOf("\r\n", StringComparison.Ordinal);
                if (index > 0)
                {
                    strReadedMessage = strReadedMessage.Substring(0, index);
                }
                optReturn.Data = strReadedMessage;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private static OperationReturn SendMessage(Stream stream, string msg)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                msg = msg + "\r\n";
                byte[] buffer = Encoding.UTF8.GetBytes(msg);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }


        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 128)
                {
                    strTemp = strSource.Substring(0, 128);
                    strSource = strSource.Substring(128, strSource.Length - 128);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string DecryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 512)
                {
                    strTemp = strSource.Substring(0, 512);
                    strSource = strSource.Substring(512, strSource.Length - 512);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
                    EncryptionAndDecryption.UMPKeyAndIVType.M104);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string EncryptString001(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 128)
                {
                    strTemp = strSource.Substring(0, 128);
                    strSource = strSource.Substring(128, strSource.Length - 128);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001),
                    EncryptionAndDecryption.UMPKeyAndIVType.M001);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string DecryptString001(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 512)
                {
                    strTemp = strSource.Substring(0, 512);
                    strSource = strSource.Substring(512, strSource.Length - 512);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101),
                    EncryptionAndDecryption.UMPKeyAndIVType.M101);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
        {
            string lStrReturn;
            int LIntRand;
            Random lRandom = new Random();
            string LStrTemp;

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = lRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "VCT");
                LIntRand = lRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "UMP");
                LIntRand = lRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, ((int)aKeyIVID).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }

        #endregion
    }

    public class Service06ServerInfo
    {
        public string Host { get; set; }
        public int Port { get; set; }
    }

    public class Service06Command
    {
        public const string GET_PASS = "G003";
    }
}
