using System;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Xml;
using System.Data;
using PFShareClassesS;
using VoiceCyber.Common;
using System.Threading;
using System.Net.Sockets;
using System.Net.Security;
using System.Diagnostics;
using System.ServiceProcess;
using System.Data.SqlClient;
using System.Collections.Generic;
using Oracle.DataAccess.Client;
using VoiceCyber.SharpZips.Zip;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;


namespace UMPService06
{

    public class ClientOperations
    {
        private TcpClient ITcpClient = null;
        private bool IBoolCanSendMessage = true;
        private bool IBoolInSendingMessage = false;
        public bool IBoolCanRecive = true;
        private string IStrSpliterChar = string.Empty;
        private string IStrSpliterChar30 = string.Empty;
        public SslStream ISslStream = null;
        public  static int G002Num = 0;
        #region RemoteEndpoint
        private string mRemoteEndpoint;
        public string RemoteEndpoint
        {
            get { return mRemoteEndpoint; }
        }

        #endregion
        


        public ClientOperations(TcpClient ATcpClient)
        {
            ITcpClient = ATcpClient;
            IStrSpliterChar = AscCodeToChr(27);
            IStrSpliterChar30 = AscCodeToChr(30);
            mRemoteEndpoint = ((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Address.ToString();
            G002Num = 0;
        }
        public void StopThisClientThread()
        {
            try
            {
                FileLog.WriteInfo("StopThisClientThread() ", "Client : " + ITcpClient.Client.RemoteEndPoint.ToString());
                SendMessageToClient("Result=StopService");
                IBoolCanSendMessage = false;
                IBoolCanRecive = false;
                while (IBoolCanSendMessage) { Thread.Sleep(50); }
                if (ISslStream != null) { ISslStream.Close(); ISslStream.Dispose(); }
                if (ITcpClient != null) { ITcpClient.Close();}
            }
            catch (Exception ex)
            {
                FileLog.WriteError("StopThisClientThread()", "Catch Error" + ex.ToString());
            }
        }

        private void SendMessageToClient(string AStrMessage)
        {
            string LStrSendMessage = string.Empty;

            try
            {
                if (!IBoolCanSendMessage) {
                    FileLog.WriteInfo("", "IBoolCanSendMessage is  false");
                    return; }
                IBoolInSendingMessage = true;
                LStrSendMessage = AStrMessage;
                if (string.IsNullOrEmpty(LStrSendMessage)) { LStrSendMessage = " "; }
                LStrSendMessage += "\r\n";
                FileLog.WriteInfo("SendMessageToClient()", "Send: " + LStrSendMessage);
                byte[] LByteMessage = Encoding.UTF8.GetBytes(LStrSendMessage);
                ISslStream.Write(LByteMessage);
                ISslStream.Flush();
            }
            catch (Exception ex)
            {
                FileLog.WriteError("SendMessageToClient()", "Catch Error" + ex.ToString());
            }
            finally
            {
                IBoolInSendingMessage = false;
            }
        }

        private string ReadMessageFromClient()
        {
            string LStrReadedData = string.Empty;

            try
            {
                StringBuilder LStringBuilderData = new StringBuilder();
                int LIntReadedBytes = -1, LIntEndKeyPosition;
                byte[] LByteReadeBuffer = new byte[20480];               

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
                    FileLog.WriteInfo("ReadMessageFromClient()", "ReadMessage:" + LStrReadedData + " IP:" + ITcpClient.Client.RemoteEndPoint.ToString());
                }
                IBoolCanRecive = false;
            }
            catch (Exception ex)
            {
                IBoolCanRecive = false;
                LStrReadedData = string.Empty;
                FileLog.WriteError("ReadMessageFromClient()", "Catch Error"+ ex.ToString());
            }

            return LStrReadedData;
        }

        public void ClientMessageOperation()
        {
            string LStrReadedData = string.Empty;
            string LStrEntryBody = string.Empty;

            try
            {
                LStrEntryBody = "A Client Connected\n";
                LStrEntryBody += "IPAddress : " + ((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Address.ToString();
                LStrEntryBody += " Port : " + ((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Port.ToString() + "\n";               

                FileLog.WriteInfo("ClientMessageOperation() ", " A Client Connected :" + LStrEntryBody);

                ISslStream = new SslStream(ITcpClient.GetStream(), false);
                ISslStream.AuthenticateAsServer(UMPService06.IX509CertificateServer, false, System.Security.Authentication.SslProtocols.Default, false);
                ISslStream.ReadTimeout = 80000;
                ISslStream.WriteTimeout = 80000;
                //先验证
                IBoolCanRecive = ValidMessageFromClient();
                if(!IBoolCanRecive)
                {
                    StopThisClientThread();
                }

                FileLog.WriteInfo("ClientMessageOperation() ", " Pass ValidMessageFromClient ");
                while (IBoolCanRecive)
                {
                    try
                    {
                        LStrReadedData = ReadMessageFromClient();
                        if( string.IsNullOrEmpty(LStrReadedData) || string.IsNullOrWhiteSpace(LStrReadedData))
                        {
                            continue;
                        }
                        DealClientMessage(LStrReadedData);
                        Thread.Sleep(10);
                    }
                    catch (Exception ex)
                    {
                        IBoolCanRecive = false;
                        FileLog.WriteInfo("ClientMessageOperation() ", ex.ToString());
                    }
                }
                while (IBoolInSendingMessage) { Thread.Sleep(20); }
                IBoolCanSendMessage = false;
            }
            catch (AuthenticationException ex)
            {
                IBoolCanSendMessage = false;
                IBoolCanRecive = false;
                FileLog.WriteInfo("ClientMessageOperation()  AuthenticationException:", ex.ToString() + " | StopThisClientThread:" + mRemoteEndpoint);
                StopThisClientThread();
            }
            catch (Exception ex)
            {
                IBoolCanSendMessage = false;
                IBoolCanRecive = false;
                FileLog.WriteInfo("ClientMessageOperation() ", ex.ToString() + "| StopThisClientThread:" + mRemoteEndpoint);
                StopThisClientThread();
            }
        }

        private void DealClientMessage(string AStrMessage)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrClientMethod = string.Empty;
            string LStrSendMessage = string.Empty;
            FileLog.WriteInfo("DealClientMessage  Start()", "");

            try
            {
                string[] LStrUserSendData = AStrMessage.TrimEnd(IStrSpliterChar.ToArray()).Split(IStrSpliterChar.ToArray());
                if (LStrUserSendData.Length <= 0) { return; }
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrClientMethod = EncryptionAndDecryption.EncryptDecryptString(LStrUserSendData[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                if (LStrClientMethod.Length != 4)
                {
                    FileLog.WriteInfo("DealClientMessage()", "IP:" + ITcpClient.Client.RemoteEndPoint.ToString() + "| Error Command:" + LStrClientMethod);
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR03), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);             //错误的指令
                    return;
                }
                int LIntDataLength = LStrUserSendData.Length;

                for (int LIntDataLoop = 0; LIntDataLoop < LIntDataLength; LIntDataLoop++)
                {
                    LStrUserSendData[LIntDataLoop] = EncryptionAndDecryption.EncryptDecryptString(LStrUserSendData[LIntDataLoop], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                }

                switch (LStrClientMethod)
                {
                    case "G001":  //取待加密录音,存入XML，返回xml的URL
                        {
                            FileLog.WriteInfo("DealClientMessage()", "RUN Begin G001" + mRemoteEndpoint);
                            ClientMessageG001(LStrUserSendData);
                            FileLog.WriteInfo("DealClientMessage() RUN END G001 ", IBoolCanRecive.ToString());
                            //if (IBoolCanRecive == false)
                            //{
                            //    IBoolCanRecive = true;
                            //    FileLog.WriteInfo("DealClientMessage() RUN END G001 ", IBoolCanRecive.ToString());
                            //    Thread.Sleep(1000);
                            //}
                        }
                        break;
                    case "G002": //将录音标志写入数据库
                        {
                            FileLog.WriteInfo("DealClientMessage()", "RUN Begin G002 :"+mRemoteEndpoint);  
                            G002Num = IntParse(LStrUserSendData[1], 0);
                            FileLog.WriteInfo("DealClientMessage()", "IP:" + ITcpClient.Client.RemoteEndPoint.ToString() + "| G002Num=" + G002Num + " LStrUserSendData.Length =" + LStrUserSendData.Length);
                            LStrSendMessage =LStrUserSendData.Length.ToString();
                            SendMessageToClient(LStrSendMessage);
                            FileLog.WriteInfo("DealClientMessage()", "IP:" + ITcpClient.Client.RemoteEndPoint.ToString() + " Message:" + LStrSendMessage);
                            if (G002Num + 2 == LStrUserSendData.Length)
                            {                          
                                string LStrSingleMessage = string.Empty;
                                for (int i = 2; i < LStrUserSendData.Length; i++) 
                                {
                                    LStrSingleMessage = LStrUserSendData[i];
                                    ClientMessageG002(LStrSingleMessage);
                                }
                            }
                            FileLog.WriteInfo("DealClientMessage()", "RUN END G002 :" + mRemoteEndpoint); 
                        }
                        break;
                    case "G003": //新密钥解老密钥
                        {
                            FileLog.WriteInfo("DealClientMessage()", "RUN Begin G003 :" + mRemoteEndpoint);
                            ClientMessageG003(LStrUserSendData);
                            FileLog.WriteInfo("DealClientMessage()  RUN END G003 : ", IBoolCanRecive.ToString());
                        }
                        break;
                    default:
                        {
                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR08), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            SendMessageToClient(LStrSendMessage);             //系统不能识别的指令
                            FileLog.WriteInfo("DealClientMessage() ", "Not A  Recognize Command " + LStrClientMethod);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR04), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                SendMessageToClient(LStrSendMessage);
                FileLog.WriteError("DealClientMessage()", ex.Message.ToString() + "|" + ITcpClient.Client.RemoteEndPoint.ToString());
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
            strTime += source.Substring(12, 2);

            //dt = DateTime.Parse(strTime);
            dt = DateTime.Parse(DateTime.Parse(strTime).ToString("yyyy-MM-dd HH:mm:ss"));
            return dt;
        }

        /// <summary>
        /// 握手验证
        /// </summary>
        /// <returns></returns>
        private bool ValidMessageFromClient()
        {
            bool LValidSuccess = true;
            string LStrReadedData = string.Empty;


            ///1、 验证是不是HelloService06
            ///2、 发随机生成32位随机数用M001加密
            ///3、 接收32位随机数用M104解密
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode001 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
            LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
            LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

            string Random32Temp = string.Empty;

            while (LValidSuccess)
            {
                try
                {
                    LStrReadedData = ReadMessageFromClient();
                    if (string.IsNullOrEmpty(LStrReadedData)) { continue; }
                    LStrReadedData = EncryptionAndDecryption.EncryptDecryptString(LStrReadedData, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    //LStrReadedData = Encoding.UTF8.GetString(Encoding.Unicode.GetBytes(LStrReadedData));
                    FileLog.WriteInfo("ValidMessageFromClient() Decrypt:", LStrReadedData);

                    if (LStrReadedData.Equals("HelloService06"))
                    {
                        string Random32Number = PFShareClassesS.PasswordVerifyOptions.GeneratePassword(32, 6);
                        FileLog.WriteInfo("ValidMessageFromClient GeneratePassword:", Random32Number);

                        Random32Temp = Random32Number;
                        Random32Number = EncryptionAndDecryption.EncryptDecryptString(Random32Number, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                        FileLog.WriteInfo("ValidMessageFromClient() Random32Number M001:", Random32Number);
                        SendMessageToClient(Random32Number);
                        break;
                    }
                    else
                    {
                        return LValidSuccess = false;
                    }

                }
                catch (Exception ex)
                {
                    FileLog.WriteError("ValidMessageFromClient() 1","ISslStream.Read():"+ ex.ToString());
                    return LValidSuccess = false;
                }
            }


            while (LValidSuccess)
            {
                try
                {
                    LStrReadedData = ReadMessageFromClient();
                    if (string.IsNullOrEmpty(LStrReadedData) || string.IsNullOrWhiteSpace(LStrReadedData)) { continue; }
                    FileLog.WriteInfo("ValidMessageFromClient()", " ReceiveMessage:"+ LStrReadedData);

                    LStrReadedData = EncryptionAndDecryption.EncryptDecryptString(LStrReadedData, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    FileLog.WriteInfo("ValidMessageFromClient()", "EncryptDecryptString-->ReceiveMessage:" + LStrReadedData);
                    if (Random32Temp.Equals(LStrReadedData))
                    {
                        LValidSuccess = false;
                        return true;
                    }
                    else
                    {
                        FileLog.WriteInfo("ValidMessageFromClient() ", "Random32Temp Is Not Equal");
                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01002", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        SendMessageToClient(LStrSendMessage);             //错误的指令
                        return LValidSuccess = false;
                    }


                }
                catch (Exception ex)
                {
                    FileLog.WriteError("ValidMessageFromClient() 2", "ISslStream.Read():" + ex.ToString());
                    return LValidSuccess = false;
                }
            }
            return LValidSuccess;

        }
        /// <summary>
        /// //取待加密录音,存入XML，返回xml的URL
        /// </summary>
        /// <param name="AStrArrayInfo">客户端消息</param>
        private void ClientMessageG001(string[] AStrArrayInfo)
        {
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            int LIntNumber = 0;
            string LStrVoiceIP = string.Empty;
            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                if (AStrArrayInfo.Length < 3)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG001 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    FileLog.WriteError("ClientMessageG001()", "Error:" + GetErrorCode(Service06ErrorCode.ERROR05) + " |  FromIP:" + RemoteEndpoint);
                    StopThisClientThread();
                    return;
                }
                LStrVoiceIP = AStrArrayInfo[1];
                LIntNumber = Convert.ToInt16(AStrArrayInfo[2]);

                FileLog.WriteInfo("ClientMessageG001() ","VoiceIP: "+ LStrVoiceIP + "|  GetNumber: " + LIntNumber+" | FromIP:"+mRemoteEndpoint);

                Root LRoot = new Root();
                List<Record> lstRecord = new List<Record>();
                String LStrSQL = string.Empty;
                long ReferenceNum = 0;
                long LMaxReference = 0;
                if (LIntNumber > 0 && LStrVoiceIP.Length > 0)
                {
                    OperationReturn optReturn = new OperationReturn();
                    optReturn.Result = true;
                    optReturn.Code = 0;

                    DataSet objDataSet;
                    // 0 、先查找最大的录音流水号,防止死循环读T_21_998
                    switch (UMPService06.IIntDBType)
                    {
                        case 2:
                            {
                                LStrSQL = string.Format("SELECT MAX(C001) AS C001 FROM  T_21_998 WHERE   C001>{1}  AND C006='{0}'  AND C019='E'     ", LStrVoiceIP, ReferenceNum);
                                FileLog.WriteInfo("ClientMessageG001()", "LStrSQL=" + LStrSQL + " |  FromIP:" + RemoteEndpoint);
                                optReturn = MssqlOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LStrSQL);

                                if (!optReturn.Result)
                                {
                                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                    FileLog.WriteError("ClientMessageG001() ", "ErrorCode:" + GetErrorCode(Service06ErrorCode.ERROR06) + "| optReturn: " + optReturn.Message + " |  FromIP:" + RemoteEndpoint);
                                    SendMessageToClient(LStrSendMessage);
                                    StopThisClientThread();
                                    return;
                                }
                                objDataSet = optReturn.Data as DataSet;
                            }
                            break;
                        case 3:
                            {
                                LStrSQL = String.Format("SELECT MAX(C001) AS C001 FROM  T_21_998 WHERE C001>{1}  AND C006='{0}'  AND C019='E'  ", LStrVoiceIP, ReferenceNum);

                                FileLog.WriteInfo("ClientMessageG001()","LStrSQL="+ LStrSQL+ " |  FromIP:" + RemoteEndpoint);
                                optReturn = OracleOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LStrSQL);

                                if (!optReturn.Result)
                                {
                                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                    FileLog.WriteError("ClientMessageG001() ", "ErrorCode:" + GetErrorCode(Service06ErrorCode.ERROR06) + "| optReturn: " + optReturn.Message + " |  FromIP:" + RemoteEndpoint);
                                    SendMessageToClient(LStrSendMessage);
                                    StopThisClientThread();
                                    return;
                                }
                                objDataSet = optReturn.Data as DataSet;
                            }
                            break;
                        default:
                            {
                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR04), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                FileLog.WriteError("ClientMessageG001() ", "ErrorCode:" + GetErrorCode(Service06ErrorCode.ERROR04) + " | FromIP:" + mRemoteEndpoint);
                                SendMessageToClient(LStrSendMessage);
                                StopThisClientThread();
                            }
                            return;
                    }

                    if (objDataSet != null && objDataSet.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[i];
                            if (dr["C001"] != DBNull.Value)
                            {
                                LMaxReference = Convert.ToInt64(dr["C001"].ToString());
                            }
                            FileLog.WriteInfo("ClientMessageG001()", "Convert C001 : Value=" + LMaxReference + " | FromIP:" + mRemoteEndpoint);
                        }
                    }

                    if (LMaxReference == 0)
                    {
                        FileLog.WriteInfo("ClientMessageG001()  ", "LMaxReference= 0" + " | FromIP:" + mRemoteEndpoint);                        
                    }
                    else
                    {
                        while (lstRecord.Count < LIntNumber)
                        {
                            objDataSet = null;
                            Record recordTemp = new Record();
                            //1、先根据录音取待加密的录音
                            switch (UMPService06.IIntDBType)
                            {
                                case 2:
                                    {
                                        LStrSQL = string.Format("SELECT TOP 1  C001,C002,C003,C006,C009,C010 ,C020 FROM  T_21_998 WHERE   C001>{1} AND C001<={2}  AND C006='{0}'  AND C019='E'  OR  ( C019='F'   AND  C017<100 )   ORDER BY C001 ", LStrVoiceIP, ReferenceNum, LMaxReference);
                                        FileLog.WriteInfo("ClientMessageG001() ", "StrSQL= " + LStrSQL + " | FromIP:" + mRemoteEndpoint);
                                        optReturn = MssqlOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LStrSQL);

                                        if (!optReturn.Result)
                                        {
                                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                            FileLog.WriteError("ClientMessageG001() ", "ErrorCode:" + GetErrorCode(Service06ErrorCode.ERROR06) + "| optReturn: " + optReturn.Message + " | FromIP:" + mRemoteEndpoint);
                                            SendMessageToClient(LStrSendMessage);
                                            StopThisClientThread();
                                            return;
                                        }
                                        objDataSet = optReturn.Data as DataSet;
                                    }
                                    break;
                                case 3:
                                    {
                                        LStrSQL = String.Format("SELECT  C001,C002,C003,C006,C009,C010 ,C020  FROM T_21_998 WHERE ROWNUM = 1  AND C001>{1} AND C001<={2} AND C006='{0}'  AND C019='E'  OR ( C019='F'   AND  C017<100 )  ORDER BY C001  ", LStrVoiceIP, ReferenceNum, LMaxReference);
                                        FileLog.WriteInfo("ClientMessageG001()", "  StrSQL= " + LStrSQL + " | FromIP:" + mRemoteEndpoint);
                                        optReturn = OracleOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LStrSQL);

                                        if (!optReturn.Result)
                                        {
                                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                            FileLog.WriteError("ClientMessageG001() ", "ErrorCode:" + GetErrorCode(Service06ErrorCode.ERROR06) + "| optReturn: " + optReturn.Message + " | FromIP:" + mRemoteEndpoint);
                                            SendMessageToClient(LStrSendMessage);
                                            StopThisClientThread();
                                            return;
                                        }
                                        objDataSet = optReturn.Data as DataSet;
                                    }
                                    break;
                                default:
                                    {
                                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR04), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                        FileLog.WriteError("ClientMessageG001() ", "ErrorCode:" + GetErrorCode(Service06ErrorCode.ERROR04) + " | FromIP:" + mRemoteEndpoint);
                                        SendMessageToClient(LStrSendMessage);
                                        StopThisClientThread();
                                    }
                                    return;
                            }
                            if (objDataSet != null &&objDataSet.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                                {
                                    DataRow dr = objDataSet.Tables[0].Rows[i];
                                    recordTemp.RecordReference = dr["C001"].ToString();
                                    //记录录音流水号ID
                                    ReferenceNum = Int64.Parse(dr["C001"].ToString());
                                    recordTemp.RecordReference1 = dr["C002"].ToString();
                                    recordTemp.StartRecordTime = Convert.ToDateTime(dr["C003"].ToString());
                                    recordTemp.Path = dr["C009"].ToString();
                                    recordTemp.Path1 = dr["C010"].ToString();
                                    recordTemp.VoiceIP = EncryptionAndDecryption.EncryptDecryptString(dr["C006"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                                    recordTemp.TrueVoiceIP = dr["C020"].ToString();
                                }
                            }
                            else
                            {
                                //如果没查到录音则跳出
                                FileLog.WriteInfo("ClientMessageG001()", "No Record | FromIP:" + mRemoteEndpoint);
                                break;
                            }



                            //2、查询对象绑定的加密策略表
                            objDataSet = null;
                            switch (UMPService06.IIntDBType)
                            {
                                case 2:
                                    {
                                        LStrSQL = string.Format("SELECT   *  FROM T_24_002 WHERE C001 = '1'  AND C002 ='{0}'  AND CONVERT(decimal, C004) <= {1} AND  CONVERT(decimal, C005)>{1} ", EncryptionAndDecryption.EncryptDecryptString(recordTemp.TrueVoiceIP, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002)                                           
                                            ,Convert.ToDecimal( recordTemp.StartRecordTime.ToString("yyyyMMddHHmmss")));


                                        FileLog.WriteInfo("ClientMessageG001() ", "LStrSQL =" + LStrSQL + " | FromIP:" + mRemoteEndpoint);
                                        optReturn = MssqlOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LStrSQL);
                                        if (!optReturn.Result)
                                        {
                                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                            SendMessageToClient(LStrSendMessage);
                                            FileLog.WriteInfo("ClientMessageG001() :", GetErrorCode(Service06ErrorCode.ERROR06));
                                            StopThisClientThread();
                                            return;
                                        }
                                        objDataSet = optReturn.Data as DataSet;
                                    }
                                    break;
                                case 3:
                                    {
                                        LStrSQL = String.Format("SELECT   *  FROM T_24_002 WHERE C001 = '1'  AND  C002 ='{0}'  AND  TO_NUMBER(C004) <= {1}  AND  TO_NUMBER(C005) >= {1}  ", EncryptionAndDecryption.EncryptDecryptString(recordTemp.TrueVoiceIP, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002), Convert.ToDecimal(recordTemp.StartRecordTime.ToString("yyyyMMddHHmmss")));
                                        FileLog.WriteInfo("ClientMessageG001() ", "LStrSQL=" + LStrSQL + " | FromIP:" + mRemoteEndpoint);
                                        optReturn = OracleOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LStrSQL);
                                        if (!optReturn.Result)
                                        {
                                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                            FileLog.WriteInfo("ClientMessageG001() ", "Error:" + GetErrorCode(Service06ErrorCode.ERROR06) + " | FromIP:" + mRemoteEndpoint);
                                            SendMessageToClient(LStrSendMessage);
                                            StopThisClientThread();
                                            return;
                                        }
                                        objDataSet = optReturn.Data as DataSet;
                                    }
                                    break;
                                default:
                                    {
                                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR04), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                        SendMessageToClient(LStrSendMessage);
                                        FileLog.WriteInfo("ClientMessageG001() ", "Error:" + GetErrorCode(Service06ErrorCode.ERROR04) + " | FromIP:" + mRemoteEndpoint);
                                        StopThisClientThread();
                                    }
                                    return;
                            }


                            long LPolicyID = 0;
                            string LFirstUsedTime = string.Empty;
                            string LLastUsedTime = string.Empty;
                            DateTime LPolicyDurationBegin;
                            if (objDataSet != null && objDataSet.Tables[0].Rows.Count > 0)
                            {
                                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                                {
                                    DataRow dr = objDataSet.Tables[0].Rows[i];
                                    LPolicyID = Int64.Parse(dr["C003"].ToString());
                                   // LPolicyDurationBegin = DateTime.Parse(dr["C004"].ToString());
                                    if(dr["C004"]!= DBNull.Value)
                                    {
                                        LPolicyDurationBegin = StringToDateTime(dr["C004"].ToString());
                                    }
                                   

                                    if (dr["C012"] != DBNull.Value)
                                    {
                                        LFirstUsedTime = dr["C012"].ToString();
                                    }
                                    if (dr["C013"] != DBNull.Value)
                                    {
                                        LLastUsedTime = dr["C013"].ToString();
                                    }
                                }
                            }
                            else
                            {
                                ////如果记录为零表示没查到策略,再根据服务器ip看看该服务器有没有启用默认加密

                                //有默认加密将recordTemp写放到list中
                                objDataSet = null;
                                switch (UMPService06.IIntDBType)
                                {
                                    case 2:
                                        {
                                            LStrSQL = string.Format("SELECT   *  FROM T_24_002 WHERE C001 = '1'  AND C002 ='{0}' AND  C003=4020000000000000000 AND C008='1'  ", EncryptionAndDecryption.EncryptDecryptString(recordTemp.TrueVoiceIP, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002)
                                               );


                                            FileLog.WriteInfo("ClientMessageG001() ", "LStrSQL =" + LStrSQL + " | FromIP:" + mRemoteEndpoint);
                                            optReturn = MssqlOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LStrSQL);
                                            if (!optReturn.Result)
                                            {
                                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                                SendMessageToClient(LStrSendMessage);
                                                FileLog.WriteInfo("ClientMessageG001() :", GetErrorCode(Service06ErrorCode.ERROR06));
                                                StopThisClientThread();
                                                return;
                                            }
                                            objDataSet = optReturn.Data as DataSet;
                                        }
                                        break;
                                    case 3:
                                        {
                                            LStrSQL = String.Format("SELECT   *  FROM T_24_002 WHERE C001 = '1'  AND  C002 ='{0}' AND C003=4020000000000000000  AND C008='1'  ", EncryptionAndDecryption.EncryptDecryptString(recordTemp.TrueVoiceIP, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002));
                                            FileLog.WriteInfo("ClientMessageG001() ", "LStrSQL=" + LStrSQL + " | FromIP:" + mRemoteEndpoint);
                                            optReturn = OracleOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LStrSQL);
                                            if (!optReturn.Result)
                                            {
                                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                                FileLog.WriteInfo("ClientMessageG001() ", "Error:" + GetErrorCode(Service06ErrorCode.ERROR06) + " | FromIP:" + mRemoteEndpoint);
                                                SendMessageToClient(LStrSendMessage);
                                                StopThisClientThread();
                                                return;
                                            }
                                            objDataSet = optReturn.Data as DataSet;
                                        }
                                        break;
                                    default:
                                        {
                                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR04), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                            SendMessageToClient(LStrSendMessage);
                                            FileLog.WriteInfo("ClientMessageG001() ", "Error:" + GetErrorCode(Service06ErrorCode.ERROR04) + " | FromIP:" + mRemoteEndpoint);
                                            StopThisClientThread();
                                        }
                                        return;
                                }
                                if (objDataSet != null && objDataSet.Tables[0].Rows.Count > 0)
                                {
                                    recordTemp.KeyID = "";
                                    recordTemp.PolicyID = "";
                                    recordTemp.Key1b ="";
                                    recordTemp.Key1d = "";
                                    lstRecord.Add(recordTemp);
                                    continue;
                                }
                                else 
                                {
                                    //没默认加密continue直接更新T_21_998表的字段为0

                                    string LUpdateStr = string.Format("UPDATE T_21_998 SET  C019='0'  WHERE  C001={0} ", recordTemp.RecordReference);
                                    switch (UMPService06.IIntDBType)
                                    {
                                        case 2:
                                            {
                                                optReturn = MssqlOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                                                if (!optReturn.Result)
                                                {
                                                    FileLog.WriteError("ClientMessageG001()", "Fail  1: " + optReturn.Message + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + recordTemp.RecordReference);
                                                    return;
                                                }
                                            }
                                            break;
                                        case 3:
                                            {
                                                optReturn = OracleOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                                                if (!optReturn.Result)
                                                {
                                                    FileLog.WriteError("ClientMessageG001()", "Fail  1:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + recordTemp.RecordReference);
                                                    return;
                                                }
                                            }
                                            break;
                                        default:
                                            {
                                                FileLog.WriteError("ClientMessageG001()", "Fail  1:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + recordTemp.RecordReference);
                                            }
                                            return;
                                    }

                                    LUpdateStr = string.Format("UPDATE T_21_001 SET C025='0', C026=0, C027=0 ,C030={0} WHERE  C001={1} ", DateTime.UtcNow.ToString("yyyyMMddHHmmss"), recordTemp.RecordReference);
                                    FileLog.WriteInfo("G001()", LUpdateStr);
                                    switch (UMPService06.IIntDBType)
                                    {
                                        case 2:
                                            {
                                                optReturn = MssqlOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                                                if (!optReturn.Result)
                                                {
                                                    FileLog.WriteError("ClientMessageG001()", "Fail  2: " + optReturn.Message + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + recordTemp.RecordReference);
                                                    return;
                                                }
                                            }
                                            break;
                                        case 3:
                                            {
                                                optReturn = OracleOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                                                if (!optReturn.Result)
                                                {
                                                    FileLog.WriteError("ClientMessageG001()", "Fail  2:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + recordTemp.RecordReference);
                                                    return;
                                                }
                                            }
                                            break;
                                        default:
                                            {
                                                FileLog.WriteError("ClientMessageG001()", "Fail  2:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + recordTemp.RecordReference);
                                            }
                                            return;
                                    }
                                    continue;
                                }
                            }



                            //3--判断该时段绑定的策略是否在被修改、删除过程中 --- 先跳过
                            ////




                            //4、将待加密的录音取密钥
                            objDataSet = null;
                            switch (UMPService06.IIntDBType)
                            {
                                case 2:
                                    {
                                        LStrSQL = string.Format("SELECT C001, C002,C004, C005 FROM T_24_005  WHERE C002 = {0} AND  	C007 <= {1} AND C008 >= {1} ", LPolicyID, Convert.ToDecimal(recordTemp.StartRecordTime.ToString("yyyyMMddHHmmss")));
                                        FileLog.WriteInfo("ClientMessageG001() ", "LStrSQL=" + LStrSQL + " | FromIP:" + mRemoteEndpoint);
                                        optReturn = MssqlOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LStrSQL);
                                        if (!optReturn.Result)
                                        {
                                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                            FileLog.WriteInfo("ClientMessageG001()", "Error" + GetErrorCode(Service06ErrorCode.ERROR06) + " | FromIP:" + mRemoteEndpoint);
                                            SendMessageToClient(LStrSendMessage);
                                            StopThisClientThread();
                                            return;
                                        }
                                        objDataSet = optReturn.Data as DataSet;
                                    }
                                    break;
                                case 3:
                                    {
                                        LStrSQL = string.Format("SELECT C001, C002,C004, C005 FROM T_24_005  WHERE C002 = {0} AND  	C007 <= {1} AND C008 >= {1} ", LPolicyID, Convert.ToDecimal(recordTemp.StartRecordTime.ToString("yyyyMMddHHmmss")));
                                        FileLog.WriteInfo("ClientMessageG001 ", "LStrSQL=" + LStrSQL + " | FromIP:" + mRemoteEndpoint);
                                        optReturn = OracleOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LStrSQL);
                                        if (!optReturn.Result)
                                        {
                                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                            FileLog.WriteInfo("ClientMessageG001() ", "Error:" + GetErrorCode(Service06ErrorCode.ERROR06) + " | FromIP:" + mRemoteEndpoint);
                                            SendMessageToClient(LStrSendMessage);
                                            StopThisClientThread();
                                            return;
                                        }
                                        objDataSet = optReturn.Data as DataSet;
                                    }
                                    break;
                                default:
                                    {
                                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR04), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                        SendMessageToClient(LStrSendMessage);
                                        FileLog.WriteInfo("ClientMessageG001()", "Error:" + GetErrorCode(Service06ErrorCode.ERROR04) + " | FromIP:" + mRemoteEndpoint);
                                        StopThisClientThread();
                                    }
                                    return;
                            }
                            if (objDataSet != null && objDataSet.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dr in objDataSet.Tables[0].Rows) 
                                {
                                    recordTemp.KeyID = dr["C001"].ToString();
                                    recordTemp.PolicyID = dr["C002"].ToString();
                                    recordTemp.Key1b = dr["C004"].ToString();
                                    recordTemp.Key1d = dr["C005"].ToString();
                                }
                            }
                            else
                            {
                                ////如果记录为零表示没查到密钥则跳出本次循环
                                continue;
                            }


                            //5、更新表的C012（最早使用时间）和C013 （最晚使用时间）
                            string LUpdateSQL = string.Empty;
                            if (string.IsNullOrEmpty(LFirstUsedTime) ||string.IsNullOrWhiteSpace(LFirstUsedTime))
                            {
                                LUpdateSQL = string.Format("  C012='{0}' , ", recordTemp.StartRecordTime.ToString("yyyy/MM/dd HH:mm:ss"));
                            }
                            else if (!string.IsNullOrEmpty(LFirstUsedTime) && !string.IsNullOrWhiteSpace(LFirstUsedTime))
                            {
                                if( Convert.ToDateTime(LFirstUsedTime) > recordTemp.StartRecordTime)
                                {
                                    LUpdateSQL = string.Format("  C012='{0}' , ", recordTemp.StartRecordTime.ToString("yyyy/MM/dd HH:mm:ss"));
                                }
                            }

                            if (string.IsNullOrEmpty(LLastUsedTime) ||string.IsNullOrWhiteSpace(LLastUsedTime))
                            {
                                LUpdateSQL = string.Format(LUpdateSQL + "  C013='{0}' , ", recordTemp.StartRecordTime.ToString("yyyy/MM/dd HH:mm:ss"));
                            }
                            else if (!string.IsNullOrEmpty(LLastUsedTime) && !string.IsNullOrWhiteSpace(LLastUsedTime))
                            {
                                if( Convert.ToDateTime(LLastUsedTime) < recordTemp.StartRecordTime)
                                {
                                    LUpdateSQL = string.Format(LUpdateSQL + "  C013='{0}' , ", recordTemp.StartRecordTime.ToString("yyyy/MM/dd HH:mm:ss"));
                                }
                            }

                            if (! string.IsNullOrWhiteSpace(LUpdateSQL))
                            {
                                FileLog.WriteInfo("ClientMessageG001()", "LUpdateSQL=" + LUpdateSQL + " | FromIP:" + mRemoteEndpoint);
                                switch (UMPService06.IIntDBType)
                                {
                                    case 2:
                                        {
                                            LStrSQL = string.Format("UPDATE T_24_002  SET {0}  WHERE C001 = '1'  AND C002 ='{1}'  AND CONVERT(decimal, C004) <= {2} AND  CONVERT(decimal, C005)>={2} ", LUpdateSQL.TrimEnd(' ').TrimEnd(','), EncryptionAndDecryption.EncryptDecryptString(recordTemp.TrueVoiceIP, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002)
                                          , Convert.ToDecimal(recordTemp.StartRecordTime.ToString("yyyyMMddHHmmss")));
                                            FileLog.WriteInfo("ClientMessageG001()", "LStrSQL=" + LStrSQL + " | FromIP:" + mRemoteEndpoint);

                                            optReturn = MssqlOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LStrSQL);
                                            if (!optReturn.Result)
                                            {
                                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                                FileLog.WriteInfo("ClientMessageG001()", "Error:" + GetErrorCode(Service06ErrorCode.ERROR06) + " | FromIP:" + mRemoteEndpoint);
                                                SendMessageToClient(LStrSendMessage);
                                                StopThisClientThread();
                                                return;
                                            }
                                            objDataSet = optReturn.Data as DataSet;
                                        }
                                        break;
                                    case 3:
                                        {
                                            LStrSQL = String.Format("UPDATE T_24_002  SET {0}  WHERE C001 = '1'  AND  C002 ='{1}'  AND  TO_NUMBER(C004) <= {2}  AND  TO_NUMBER(C005) >= {2}  ", LUpdateSQL.TrimEnd(' ').TrimEnd(','), EncryptionAndDecryption.EncryptDecryptString(recordTemp.TrueVoiceIP, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002), Convert.ToDecimal(recordTemp.StartRecordTime.ToString("yyyyMMddHHmmss")));
                                            FileLog.WriteInfo("ClientMessageG001()", "LStrSQL=" + LStrSQL + " | FromIP:" + mRemoteEndpoint);
                                            optReturn = OracleOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LStrSQL);
                                            if (!optReturn.Result)
                                            {
                                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                                FileLog.WriteInfo("ClientMessageG001() ", "Error:" + GetErrorCode(Service06ErrorCode.ERROR06) + " | FromIP:" + mRemoteEndpoint);
                                                SendMessageToClient(LStrSendMessage);
                                                StopThisClientThread();
                                                return;
                                            }
                                            objDataSet = optReturn.Data as DataSet;
                                        }
                                        break;
                                    default:
                                        {
                                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR04), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                            FileLog.WriteInfo("ClientMessageG001() ", "Error:" + GetErrorCode(Service06ErrorCode.ERROR04) + " | FromIP:" + mRemoteEndpoint);
                                            SendMessageToClient(LStrSendMessage);
                                            StopThisClientThread();
                                        }
                                        return;
                                }
                            }
                            lstRecord.Add(recordTemp);


                        }//while

                    }

                    LRoot.RecList = lstRecord;

                    FileLog.WriteInfo("ClientMessageG001()", "LRoot.RecList Count="+lstRecord.Count());

                    string LIP = ((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Address.ToString();
                    string LXmlFileName = string.Format("{0}.xml", LIP);
                    string LZipFileName = string.Format("{0}.zip", LIP);
                    string LSourceDirectory = Path.Combine(UMPService06.IStrBaseDirectory, "MediaData");

                    if (!Directory.Exists(LSourceDirectory))
                    {
                        Directory.CreateDirectory(LSourceDirectory);
                    }

                    string LSourceFilePath = Path.Combine(LSourceDirectory, LXmlFileName);
                    optReturn = XMLHelper.SerializeFile(LRoot, LSourceFilePath);
                    FileLog.WriteInfo("ClientMessageG001()", "LSourceFilePath: " + LSourceFilePath + " | FromIP:" + mRemoteEndpoint);


                    string LRelativePath = Path.Combine("\\MediaData", string.Format("{0}.zip", LIP));


                    string LTargetFilePath = Path.Combine(LSourceDirectory, string.Format("{0}.zip", LIP));
                    FileLog.WriteInfo("ClientMessageG001()", "LTargetFilePath: " + LTargetFilePath + " | FromIP:" + mRemoteEndpoint);

                    string LStrZipPassword = PFShareClassesS.PasswordVerifyOptions.GeneratePassword(32, 6);
                    using (ZipOutputStream LZipOutStream = new ZipOutputStream(File.Create(LTargetFilePath)))
                    {
                        LZipOutStream.SetLevel(9);
                        LZipOutStream.UseZip64 = UseZip64.Off;
                        byte[] LByteBuffer = new byte[1024];
                        if (!string.IsNullOrEmpty(LStrZipPassword)) { LZipOutStream.Password = LStrZipPassword; }
                        ZipEntry LZipEntry = new ZipEntry(Path.GetFileName(LSourceFilePath));
                        LZipEntry.DateTime = DateTime.UtcNow;
                        LZipOutStream.PutNextEntry(LZipEntry);
                        using (FileStream LFileStream = File.OpenRead(LSourceFilePath))
                        {
                            int LIntSourceBytes;
                            do
                            {
                                LIntSourceBytes = LFileStream.Read(LByteBuffer, 0, LByteBuffer.Length);
                                LZipOutStream.Write(LByteBuffer, 0, LIntSourceBytes);
                            } while (LIntSourceBytes > 0);

                        }
                        LZipOutStream.Finish(); LZipOutStream.Close();
                    }

                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(LRelativePath, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004) + AscCodeToChr(27) + EncryptionAndDecryption.EncryptDecryptString(LStrZipPassword, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);
                }
                else //参数数量错误  
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG001 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    FileLog.WriteInfo("ClientMessageG001() ", "Error:" + GetErrorCode(Service06ErrorCode.ERROR05) + " | FromIP:" + mRemoteEndpoint);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    StopThisClientThread();
                    return;
                }
            }
            catch (Exception ex)
            {
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG001 \nReturnCode:" + GetErrorCode(Service06ErrorCode.ERROR05) + "\n" + ex.ToString(), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                FileLog.WriteInfo("ClientMessageG001()", "Error:" + GetErrorCode(Service06ErrorCode.ERROR04) + "|" + ex.Message.ToString() + " | FromIP:" + mRemoteEndpoint);
                SendMessageToClient(LStrSendMessage);          //参数错误
                StopThisClientThread();
                return;
            }
        }


        private void ClientMessageG002(string AStrArrayInfo)
        {
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrVoiceIP = string.Empty;
            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                string[] LStrUserSendData = AStrArrayInfo.Split(IStrSpliterChar30.ToArray());
                if (LStrUserSendData.Length < 2)
                {
                    FileLog.WriteError("ClientMessageG002()", " Error:" + GetErrorCode(Service06ErrorCode.ERROR05) + " | FromIP:" + mRemoteEndpoint);
                    return;
                }

                string LRecordReference = LStrUserSendData[0];
                string LFlag = LStrUserSendData[1];
                string LKeyID = string.Empty;
                string LPolicyID = string.Empty;
                string LKey1b = string.Empty;
                string LKey1d = string.Empty;
                //string LErrorCode = string.Empty;

                if (!LFlag.Equals("0"))
                {
                    if (LStrUserSendData.Length < 6)
                    {      //参数错误
                        FileLog.WriteError("ClientMessageG002()", " Error:" + GetErrorCode(Service06ErrorCode.ERROR05) + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + LRecordReference);
                        return;
                    }
                    LKeyID = LStrUserSendData[2];
                    LPolicyID = LStrUserSendData[3];
                    LKey1b = LStrUserSendData[4];
                    LKey1d = LStrUserSendData[5];
                    //LErrorCode = LStrUserSendData[6];
                }

                FileLog.WriteInfo("ClientMessageG002()", "GetMessage:" + LRecordReference + " | " + LFlag + " | " + LKeyID + " | " + LPolicyID + " | " + LKey1b + " | " + LKey1d + " |  FromIP:" + RemoteEndpoint );



                OperationReturn optReturn = new OperationReturn();
                optReturn.Result = true;
                optReturn.Code = 0;

                string LUpdateStr = string.Empty;
                //1、更新T_21_998表C017加密失败次数 C018最后加密失败时间 C019加密标志'2'表加密成功
                if (LFlag.Equals("2")) //成功
                {
                    LUpdateStr = string.Format("UPDATE T_21_998 SET  C019='2'  WHERE  C001={0} ", LRecordReference);
                }
                else if (LFlag.Equals("0"))//默认加密
                {
                    LUpdateStr = string.Format("UPDATE T_21_998 SET  C019='0'  WHERE  C001={0} ", LRecordReference);
                }
                else
                {
                    LUpdateStr = string.Format("UPDATE T_21_998 SET  C019='F' , C017=C017+1 , C018= '{0}'  WHERE  C001={1} ", DateTime.UtcNow.ToString("yyyyMMddHHmmss"), LRecordReference);
                }

                FileLog.WriteInfo("ClientMessageG002() ", "LUpdateStr=" + LUpdateStr + " | FromIP:" + mRemoteEndpoint);

                switch (UMPService06.IIntDBType)
                {
                    case 2:
                        {
                            optReturn = MssqlOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                            if (!optReturn.Result)
                            {
                                FileLog.WriteError("ClientMessageG002()", "Fail  1: " + optReturn.Message + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + LRecordReference);
                                return;
                            }
                        }
                        break;
                    case 3:
                        {
                            optReturn = OracleOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                            if (!optReturn.Result)
                            {
                                FileLog.WriteError("ClientMessageG002()", "Fail  1:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + LRecordReference);
                                return;
                            }
                        }
                        break;
                    default:
                        {
                            FileLog.WriteError("ClientMessageG002()", "Fail  1:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + LRecordReference);
                        }
                        return;
                }

                //2、更新T_21_001表C025加密版本号  C026KeyID  C027PolicyID  C028 Key1b  C029 Key1d  C030 加密时间
                LUpdateStr = string.Empty;
                if (LFlag.Equals("0"))
                {
                    LUpdateStr = string.Format("UPDATE T_21_001 SET C025='0', C026=0, C027=0 ,C030={0} WHERE  C001={1} ", DateTime.UtcNow.ToString("yyyyMMddHHmmss"), LRecordReference);
                }
                else
                {
                    LUpdateStr = string.Format("UPDATE T_21_001 SET C025='{0}',C026={1},C027={2},C028='{3}',C029='{4}' ,C030={5} WHERE  C001={6} ", LFlag, Int64.Parse(LKeyID), Int64.Parse(LPolicyID), LKey1b, LKey1d, DateTime.UtcNow.ToString("yyyyMMddHHmmss"), LRecordReference);
                }


                FileLog.WriteInfo("ClientMessageG002()", "LUpdateStr=" + LUpdateStr + " | FromIP:" + mRemoteEndpoint);
                switch (UMPService06.IIntDBType)
                {
                    case 2:
                        {
                            optReturn = MssqlOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                            if (!optReturn.Result)
                            {
                                FileLog.WriteError("ClientMessageG002()", "Fail  2:" + optReturn.Message + "LRecordReference= " + LRecordReference);
                                return;
                            }
                        }
                        break;
                    case 3:
                        {
                            optReturn = OracleOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                            if (!optReturn.Result)
                            {
                                FileLog.WriteError("ClientMessageG002()", "Fail  2:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + LRecordReference);
                                return;
                            }
                        }
                        break;
                    default:
                        {
                            FileLog.WriteError("ClientMessageG002()", "Fail  2:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint + "LRecordReference= " + LRecordReference);
                        }
                        return;
                }

            }
            catch (Exception ex)
            {
                FileLog.WriteError("ClientMessageG002() ", ex.Message+ "Error  :" + GetErrorCode(Service06ErrorCode.ERROR06) + " | FromIP:" + mRemoteEndpoint);
                return;
            }
        }

        /// <summary>
        /// //将录音标志写入数据库
        /// </summary>
        /// <param name="AStrArrayInfo"></param>
        private void ClientMessageG0021(string[] AStrArrayInfo)
        {
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrVoiceIP = string.Empty;
            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                if (AStrArrayInfo.Length < 2)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG002 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    FileLog.WriteError("ClientMessageG002()", " Error:" + GetErrorCode(Service06ErrorCode.ERROR05) + " | FromIP:" + mRemoteEndpoint);
                    StopThisClientThread();
                    return;
                }
                FileLog.WriteInfo("ClientMessageG002()", "LReadMeaasgae=" + AStrArrayInfo[1]);

                string[] LStrUserSendData = AStrArrayInfo[1].Split(IStrSpliterChar.ToArray());
                if (LStrUserSendData.Length < 2)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG002 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    FileLog.WriteError("ClientMessageG002()", " Error:" + GetErrorCode(Service06ErrorCode.ERROR05) + " | FromIP:" + mRemoteEndpoint);
                    StopThisClientThread();
                    return;
                }


                string LRecordReference = LStrUserSendData[0];
                string LFlag = LStrUserSendData[1];
                string LKeyID = string.Empty;
                string LPolicyID = string.Empty;
                string LKey1b = string.Empty;
                string LKey1d = string.Empty;
                string LErrorCode = string.Empty;

                if (!LFlag.Equals("0"))
                {
                    if (LStrUserSendData.Length < 6)
                    {
                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG002 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        SendMessageToClient(LStrSendMessage);          //参数错误
                        FileLog.WriteError("ClientMessageG002()", " Error:" + GetErrorCode(Service06ErrorCode.ERROR05) + " | FromIP:" + mRemoteEndpoint);
                        StopThisClientThread();
                        return;
                    }
                    LKeyID = LStrUserSendData[2];
                    LPolicyID = LStrUserSendData[3];
                    LKey1b = LStrUserSendData[4];
                    LKey1d = LStrUserSendData[5];
                    //LErrorCode = LStrUserSendData[6];
                }

                FileLog.WriteInfo("ClientMessageG002()", "GetMessage:" + LRecordReference + " | " + LFlag + " | " + LKeyID + " | " + LPolicyID + " | " + LKey1b + " | " + LKey1d  + " |  FromIP:" + RemoteEndpoint);
               


                OperationReturn optReturn = new OperationReturn();
                optReturn.Result = true;
                optReturn.Code = 0;

                string LUpdateStr = string.Empty;
                //1、更新T_21_998表C017加密失败次数 C018最后加密失败时间 C019加密标志'2'表加密成功
                if (LFlag.Equals("2")) //成功
                {
                    LUpdateStr = string.Format("UPDATE T_21_998 SET  C019='2'  WHERE  C001={0} ", LRecordReference);
                }
                else if(LFlag.Equals("0"))//默认加密
                {
                    LUpdateStr = string.Format("UPDATE T_21_998 SET  C019='0'  WHERE  C001={0} ", LRecordReference);
                }
                else
                {
                    LUpdateStr = string.Format("UPDATE T_21_998 SET  C019='F' , C017=C017+1 , C018= '{0}'  WHERE  C001={1} ", DateTime.UtcNow.ToString("yyyyMMddHHmmss"), LRecordReference);
                }

                FileLog.WriteInfo("ClientMessageG002() ", "LUpdateStr=" + LUpdateStr + " | FromIP:" + mRemoteEndpoint);

                switch (UMPService06.IIntDBType)
                {
                    case 2:
                        {
                            optReturn = MssqlOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                            if (!optReturn.Result)
                            {
                                FileLog.WriteError("ClientMessageG002()", "Fail  1: " + optReturn.Message + " | FromIP:" + mRemoteEndpoint);
                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG002 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                SendMessageToClient(LStrSendMessage);          //参数错误
                                StopThisClientThread();
                                return;
                            }
                        }
                        break;
                    case 3:
                        {
                            optReturn = OracleOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                            if (!optReturn.Result)
                            {
                                FileLog.WriteError("ClientMessageG002()", "Fail  1:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint);
                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG002 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                SendMessageToClient(LStrSendMessage);          //参数错误
                                StopThisClientThread();
                                return;
                            }
                        }
                        break;
                    default: 
                        {
                            FileLog.WriteError("ClientMessageG002()", "Fail  1:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint);
                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG002 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            SendMessageToClient(LStrSendMessage);          //参数错误
                            StopThisClientThread();
                        }
                        return;
                }

                //2、更新T_21_001表C025加密版本号  C026KeyID  C027PolicyID  C028 Key1b  C029 Key1d  C030 加密时间
                LUpdateStr = string.Empty;
                if (LFlag.Equals("0"))
                {
                    LUpdateStr = string.Format("UPDATE T_21_001 SET C025='0', C026=0, C027=0 ,C030={0} WHERE  C001={1} ", DateTime.UtcNow.ToString("yyyyMMddHHmmss"), LRecordReference);
                }
                else 
                {
                    LUpdateStr = string.Format("UPDATE T_21_001 SET C025='{0}',C026={1},C027={2},C028='{3}',C029='{4}' ,C030={5} WHERE  C001={6} ", LFlag, Int64.Parse(LKeyID), Int64.Parse(LPolicyID), LKey1b, LKey1d, DateTime.UtcNow.ToString("yyyyMMddHHmmss"), LRecordReference);
                }

              
                FileLog.WriteInfo("ClientMessageG002()", "LUpdateStr=" + LUpdateStr + " | FromIP:" + mRemoteEndpoint);
                switch (UMPService06.IIntDBType)
                {
                    case 2:
                        {
                            optReturn = MssqlOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                            if (!optReturn.Result)
                            {
                                FileLog.WriteError("ClientMessageG002()", "Fail  2:" + optReturn.Message);
                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG002 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                SendMessageToClient(LStrSendMessage);          //参数错误
                                StopThisClientThread();
                                return;
                            }
                        }
                        break;
                    case 3:
                        {
                            optReturn = OracleOperation.ExecuteSql(UMPService06.IStrDBConnectProfile, LUpdateStr);
                            if (!optReturn.Result)
                            {
                                FileLog.WriteError("ClientMessageG002()", "Fail  2:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint);
                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG002 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                SendMessageToClient(LStrSendMessage);          //参数错误
                                StopThisClientThread();
                                return;
                            }
                        }
                        break;
                    default: 
                        {
                            FileLog.WriteError("ClientMessageG002()", "Fail  2:" + optReturn.Message + " | FromIP:" + mRemoteEndpoint);
                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG002 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            SendMessageToClient(LStrSendMessage);          //参数错误
                            StopThisClientThread();
                        }
                        return;
                }

            }
            catch (Exception ex)
            {
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG002 \nReturnCode:" + GetErrorCode(Service06ErrorCode.ERROR06) + "\n" + ex.ToString(), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                SendMessageToClient(LStrSendMessage);          //参数错误
                FileLog.WriteError("ClientMessageG002() ", "Error  :" + GetErrorCode(Service06ErrorCode.ERROR06) + " | FromIP:" + mRemoteEndpoint);
                StopThisClientThread();
                return;
            }
        }

        /// <summary>
        /// //新密钥解老密钥
        /// </summary>
        /// <param name="AStrArrayInfo">
        ///@AInParam00 :    方法码
        // @AInParam01 :	加密对象类型，目前参数值 = '1'
        // @AInParam02 :	加密的对象， 目前参数值 = 录音服务器IP或录音服务器名
        // @AInParam03 :	RecordReference
        // @AInParam04 :	StartRecordTime，格式：'yyyy-MM-dd HH:mm:ss'
        // @AInParam05 :	查询密钥截至时间，如果该值为空，则取GETDATE()
        // @AInParam06 :	Key1b HH256
        // @AInParam07 :	UserID，目前该参数不使用，将来以后验证是否有新密钥解老密钥的权限 暂时不用
        /// </param>
        /// -- @AOutParam01:	成功后返回的KeyID。
        ///-- @AOutParam02:	成功后返回的PolicyID
        ///-- @AOutParam03:	成功后返回的EncryptKey1b
        ///-- @AOutParam04:	成功后返回的EncryptKey1d
        private void ClientMessageG003(string[] AStrArrayInfo)
        {
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode004 = string.Empty;           
            string LStrVerificationCode025 = string.Empty;
            string LStrVerificationCode125 = string.Empty;
            try
            {
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode025 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M025);
                LStrVerificationCode125 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M125);
                if (AStrArrayInfo.Length < 7)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG003 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    FileLog.WriteError("ClientMessageG003() ", "Error : "+ GetErrorCode(Service06ErrorCode.ERROR05)+"IP:"+mRemoteEndpoint);
                    StopThisClientThread();
                    return;
                }

                string LObjectType = AStrArrayInfo[1];
                string LObjectIP = AStrArrayInfo[2];
                string LRecordReference = AStrArrayInfo[3];
                DateTime LStartRecordTime = DateTime.Parse(AStrArrayInfo[4]);
                string LQueryKeyTime = AStrArrayInfo[5];
                //新密钥要转M025加密
                string LNewKey1b = AStrArrayInfo[6];
                FileLog.WriteInfo("ClientMessageG003()", "GetMessage:" + AStrArrayInfo[0] + " | " + AStrArrayInfo[1]+" | " + AStrArrayInfo[2] + " | " + AStrArrayInfo[3] + " | " + AStrArrayInfo[4] + " | " + AStrArrayInfo[5] + " | " + AStrArrayInfo[6] + " |  FromIP:" + RemoteEndpoint);

                DateTime LDateTimeQueryKeyTime;

                if (string.IsNullOrEmpty(LQueryKeyTime) || string.IsNullOrWhiteSpace(LQueryKeyTime))
                {
                    LDateTimeQueryKeyTime = DateTime.Now.ToUniversalTime();
                }
                else
                {
                    LDateTimeQueryKeyTime = Convert.ToDateTime(LQueryKeyTime);
                }


                if (LDateTimeQueryKeyTime < LStartRecordTime)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG003 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR05), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    FileLog.WriteError("ClientMessageG003()", "ERROR:" + GetErrorCode(Service06ErrorCode.ERROR05) + " | FromIP:" + mRemoteEndpoint + "Time Is Not Fit");
                    StopThisClientThread();
                    return;
                }
                if (LDateTimeQueryKeyTime > DateTime.Now.ToUniversalTime())
                {
                    LDateTimeQueryKeyTime = DateTime.Now.ToUniversalTime();
                }

                FileLog.WriteInfo("ClientMessageG003()", "LStartRecordTime= " + LStartRecordTime + " | LDateTimeQueryKeyTime=" + LDateTimeQueryKeyTime);

                OperationReturn optReturn = new OperationReturn();
                optReturn.Result = true;
                optReturn.Code = 0;
                DataSet objDataSet;

                string LSelectStr = string.Empty;
                //1、查询录音开始时间这段时间所涉及的policyid
                switch (UMPService06.IIntDBType)
                {
                    case 2:
                        {

                            LSelectStr = string.Format("  SELECT C003, C004,C005 FROM T_24_002 WHERE C001 = '{0}' AND C002 = '{1}' AND    CONVERT(decimal, C004) <= {2}  AND  CONVERT(decimal,C005) >= {2} ",
                                LObjectType,
                                EncryptionAndDecryption.EncryptDecryptString(LObjectIP, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002), 
                                Convert.ToDecimal(LStartRecordTime.ToString("yyyyMMddHHmmss")));
                            FileLog.WriteInfo("ClientMessageG003()", "LSelectStr:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                            optReturn = MssqlOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LSelectStr);
                            if (!optReturn.Result)
                            {
                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG003 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                SendMessageToClient(LStrSendMessage);          //参数错误
                                FileLog.WriteError("ClientMessageG003()", "Exe SQL Error:" + optReturn.Message + " SQL:" + LSelectStr + " | FromIP:" + mRemoteEndpoint);
                                StopThisClientThread();
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                        }
                        break;
                    case 3:
                        {
                            LSelectStr = string.Format("  SELECT C003, C004,C005 FROM T_24_002 WHERE C001 = '{0}' AND C002 = '{1}' AND    TO_NUMBER( C004) <= {2}  AND TO_NUMBER(C005) >= {2} ",
                                LObjectType, 
                                EncryptionAndDecryption.EncryptDecryptString(LObjectIP, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002),
                                Convert.ToDecimal(LStartRecordTime.ToString("yyyyMMddHHmmss")));
                            FileLog.WriteInfo("ClientMessageG003()", "LSelectStr:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                            optReturn = OracleOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LSelectStr);
                            if (!optReturn.Result)
                            {
                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG003 \nReturnCode: " + GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                SendMessageToClient(LStrSendMessage);          //参数错误
                                FileLog.WriteError("ClientMessageG003()", "Exe SQL Error:" + optReturn.Message + " SQL:" + LSelectStr + " | FromIP:" + mRemoteEndpoint);
                                StopThisClientThread();
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                        }
                        break;
                    default: 
                        {
                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            FileLog.WriteError("ClientMessageG003() ", "ErrorCode:" + GetErrorCode(Service06ErrorCode.ERROR06) + " | FromIP:" + mRemoteEndpoint);
                            SendMessageToClient(LStrSendMessage);
                            StopThisClientThread();
                        }
                        return;
                }

                List<ObjectBindingPolicy> lstObjectBindingPolicy = new List<ObjectBindingPolicy>();
                if (objDataSet.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        ObjectBindingPolicy LBindingPolicy = new ObjectBindingPolicy();
                        LBindingPolicy.PolicyID = Convert.ToInt64(dr["C003"].ToString());
                        FileLog.WriteInfo("", "PolicyID=" + LBindingPolicy.PolicyID);
                        LBindingPolicy.DurationBegin =StringToDateTime(dr["C004"].ToString());
                        FileLog.WriteInfo("", "DurationBegin=" + LBindingPolicy.DurationBegin);
                        LBindingPolicy.DurationEnd = StringToDateTime(dr["C005"].ToString());
                        FileLog.WriteInfo("", "DurationEnd=" + LBindingPolicy.DurationEnd);
                        lstObjectBindingPolicy.Add(LBindingPolicy);
                    }
                }
                else
                {
                    //如果没查到策略则跳出
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR07), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    FileLog.WriteInfo("ClientMessageG003() ","No Equal Policy 1 "+ GetErrorCode(Service06ErrorCode.ERROR07) + " | FromIP:" + mRemoteEndpoint); 
                    SendMessageToClient(LStrSendMessage);
                    return;
                }

                if (lstObjectBindingPolicy.Count != 1)
                {
                    //如果查到策略不为1,则不对返回
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR07), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    FileLog.WriteInfo("ClientMessageG003() ","No Equal Policy 2 "+ GetErrorCode(Service06ErrorCode.ERROR07) + " | FromIP:" + mRemoteEndpoint );
                    SendMessageToClient(LStrSendMessage);
                    return;
                }


                objDataSet = null;
                LSelectStr = string.Empty;
                //2、查询这条录音的key
                switch (UMPService06.IIntDBType)
                {
                    case 2:
                        {
                            LSelectStr = string.Format("SELECT C001, C002,C004, C005, C007,C008 FROM T_24_005  WHERE C002 = {0} AND C007 <= {1} AND C008 > {1} ", lstObjectBindingPolicy.First().PolicyID, Convert.ToDecimal(LStartRecordTime.ToString("yyyyMMddHHmmss")));
                            FileLog.WriteInfo("ClientMessageG003()", "LSelectStr:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                            optReturn = MssqlOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LSelectStr);
                            if (!optReturn.Result)
                            {
                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                FileLog.WriteError("ClientMessageG003()", "Exe SQL Error:" + optReturn.Message + " SQL:" + LSelectStr + " | FromIP:" + mRemoteEndpoint);
                                SendMessageToClient(LStrSendMessage);
                                StopThisClientThread();
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                        }
                        break;
                    case 3:
                        {
                            LSelectStr = string.Format("  SELECT C001, C002,C004, C005,C007,C008  FROM T_24_005  WHERE C002 = {0} AND  C007 <= {1} AND C008 > {1}", lstObjectBindingPolicy.First().PolicyID, Convert.ToDecimal(LStartRecordTime.ToString("yyyyMMddHHmmss")));
                            FileLog.WriteInfo("ClientMessageG003()", "LSelectStr:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                            optReturn = OracleOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LSelectStr);
                            if (!optReturn.Result)
                            {
                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                FileLog.WriteError("ClientMessageG003()", "Exe SQL Error:" + optReturn.Message + " SQL:" + LSelectStr + " | FromIP:" + mRemoteEndpoint);
                                SendMessageToClient(LStrSendMessage);
                                StopThisClientThread();
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                        }
                        break;
                    default: 
                        {
                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            FileLog.WriteError("ClientMessageG003() ", "ErrorCode: " + GetErrorCode(Service06ErrorCode.ERROR06) + " |  FromIP:" + RemoteEndpoint);
                            SendMessageToClient(LStrSendMessage);
                            StopThisClientThread();
                        }
                        return;
                }

                EncryptionKeyDictionary LTrueEnyptionKeyDictionary = new EncryptionKeyDictionary();
                if (objDataSet != null && objDataSet.Tables[0] !=null)
                {
                    if (objDataSet.Tables[0].Rows.Count==1)
                    {                
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                                DataRow dr = objDataSet.Tables[0].Rows[i];
                                LTrueEnyptionKeyDictionary.KeyID = dr["C001"].ToString();
                                FileLog.WriteInfo("", "KeyID = " + LTrueEnyptionKeyDictionary.KeyID);
                                LTrueEnyptionKeyDictionary.PolicyID = dr["C002"].ToString();
                                LTrueEnyptionKeyDictionary.Key1b = dr["C004"].ToString();
                                LTrueEnyptionKeyDictionary.Key1d = dr["C005"].ToString();
                                LTrueEnyptionKeyDictionary.EffectiveTime = StringToDateTime(dr["C007"].ToString());
                                LTrueEnyptionKeyDictionary.InvalidTime = StringToDateTime(dr["C008"].ToString());
                         }

                            ///如果新密钥=录音的实际密钥则返回AStrArrayInfo[6]
                            //if (LTrueEnyptionKeyDictionary.Key1b.Contains(LNewKey1b) || LTrueEnyptionKeyDictionary.Key1b.Equals(LNewKey1b))
                        if (EncryptionAndDecryption.EncryptDecryptString(LTrueEnyptionKeyDictionary.Key1b, LStrVerificationCode125, EncryptionAndDecryption.UMPKeyAndIVType.M125).Equals(LNewKey1b))
                            {
                                string LOldKey1b = EncryptionAndDecryption.EncryptDecryptString(LTrueEnyptionKeyDictionary.Key1b, LStrVerificationCode125, EncryptionAndDecryption.UMPKeyAndIVType.M125);
                                LStrSendMessage = LStrSendMessage = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}",
                        EncryptionAndDecryption.EncryptDecryptString(LTrueEnyptionKeyDictionary.KeyID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004),
                        AscCodeToChr(27),
                        EncryptionAndDecryption.EncryptDecryptString(LTrueEnyptionKeyDictionary.PolicyID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004),
                        EncryptionAndDecryption.EncryptDecryptString(LOldKey1b, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004),
                        LTrueEnyptionKeyDictionary.Key1d);
                                SendMessageToClient(LStrSendMessage);
                                FileLog.WriteInfo("ClientMessageG003()", "Send Key:" + LStrSendMessage + " |  FromIP:" + RemoteEndpoint);
                                return;
                            }
                       
                    }
                    else
                    {
                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR07), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        FileLog.WriteInfo("ClientMessageG003() ", "No Only One KeyID " + GetErrorCode(Service06ErrorCode.ERROR07) + " |  FromIP:" + RemoteEndpoint);
                        SendMessageToClient(LStrSendMessage);
                        StopThisClientThread();
                        return;
                    }
                }
                else 
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR07), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    FileLog.WriteInfo("ClientMessageG003() ", "objDataSet = null 1 " + GetErrorCode(Service06ErrorCode.ERROR07) + " |  FromIP:" + RemoteEndpoint);
                    SendMessageToClient(LStrSendMessage);
                    StopThisClientThread();
                    return;
                }



                objDataSet = null;
                LSelectStr = string.Empty;
                //3、查询这台录音服务器从上个梆定时间开始查询之后所有梆定策略
                switch (UMPService06.IIntDBType)
                {
                    case 2:
                        {

                            LSelectStr = string.Format("  SELECT C003, C004,C005 FROM T_24_002 WHERE C001 = '{0}' AND C002 = '{1}' AND    CONVERT(decimal, C004) >= {2}  AND  CONVERT(decimal,C004) <= {3} ", 
                                LObjectType,
                                EncryptionAndDecryption.EncryptDecryptString(LObjectIP, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002), 
                                Convert.ToDecimal(LStartRecordTime.ToString("yyyyMMddHHmmss")),
                                Convert.ToDecimal(LDateTimeQueryKeyTime.ToString("yyyyMMddHHmmss")));
                            FileLog.WriteInfo("ClientMessageG003()", "LSelectStr:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                            optReturn = MssqlOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LSelectStr);
                            if (!optReturn.Result)
                            {
                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                FileLog.WriteError("ClientMessageG003()", "Exe SQL Error:" + optReturn.Message + " SQL:" + LSelectStr + " | FromIP:" + mRemoteEndpoint);
                                SendMessageToClient(LStrSendMessage);
                                StopThisClientThread();
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                        }
                        break;
                    case 3:
                        {
                            LSelectStr = string.Format("  SELECT C003, C004,C005 FROM T_24_002 WHERE C001 = '{0}' AND C002 = '{1}' AND    TO_NUMBER(C004) >= {2}  AND TO_NUMBER(C004) <= {3} ", 
                                LObjectType, 
                                EncryptionAndDecryption.EncryptDecryptString(LObjectIP, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002), 
                                Convert.ToDecimal(LStartRecordTime.ToString("yyyyMMddHHmmss")), 
                                Convert.ToDecimal(LDateTimeQueryKeyTime.ToString("yyyyMMddHHmmss")));
                            FileLog.WriteInfo("ClientMessageG003()", "LSelectStr:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                            optReturn = OracleOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LSelectStr);
                            if (!optReturn.Result)
                            {
                                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                FileLog.WriteError("ClientMessageG003()", "Exe SQL Error:" + optReturn.Message + " SQL:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                                SendMessageToClient(LStrSendMessage);
                                StopThisClientThread();
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                        }
                        break;
                    default:
                        {
                            LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            FileLog.WriteInfo("ClientMessageG003() ","Error :"+ GetErrorCode(Service06ErrorCode.ERROR06) + " |  FromIP:" + RemoteEndpoint);
                            SendMessageToClient(LStrSendMessage);
                            StopThisClientThread();
                        }
                        return;
                }
                List<ObjectBindingPolicy> lstAllObjectBindingPolicy = new List<ObjectBindingPolicy>();
                if(objDataSet !=null &&objDataSet.Tables[0]!=null)
                {
                    if (objDataSet.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[i];
                            ObjectBindingPolicy LBindingPolicy = new ObjectBindingPolicy();
                            LBindingPolicy.PolicyID = Convert.ToInt64(dr["C003"].ToString());
                            LBindingPolicy.DurationBegin = StringToDateTime(dr["C004"].ToString());
                            LBindingPolicy.DurationEnd = StringToDateTime(dr["C005"].ToString());
                            lstAllObjectBindingPolicy.Add(LBindingPolicy);
                        }
                    }
                }
                else
                {
                    //如果没查到任何正确的策略
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR07), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    FileLog.WriteInfo("ClientMessageG003()", "objDataSet =null 2 " + GetErrorCode(Service06ErrorCode.ERROR07) + " |  FromIP:" + RemoteEndpoint);
                    SendMessageToClient(LStrSendMessage);
                    return;
                }


                ///如果不存在，添加到所有策略里
                if(lstAllObjectBindingPolicy.Where(p=>p.PolicyID== lstObjectBindingPolicy.First().PolicyID).Count()==0)
                {
                    lstAllObjectBindingPolicy.Add(lstObjectBindingPolicy.First());
                }

                /// 3、查询这些policyid在字典表在录音开始时间之后所有keyid
                objDataSet = null;
                LSelectStr = string.Empty;
                List<EncryptionKeyDictionary> lstNewEncry = new List<EncryptionKeyDictionary>();
                foreach (ObjectBindingPolicy o in lstAllObjectBindingPolicy)
                {
                    // policyid=实际策略id 则key生效大于录音时间  key生效小于等于policy失效  key生效小于查询时间  key失效大于policy生效
                    if (o.PolicyID == Int64.Parse(LTrueEnyptionKeyDictionary.PolicyID))
                    {
                        FileLog.WriteInfo("ClientMessageG003()", "=policyID");
                        switch (UMPService06.IIntDBType)
                        {
                            case 2:
                                {
                                    LSelectStr = string.Format("SELECT C001, C002,C004, C005, C007,C008 FROM T_24_005  WHERE C002 ={0} AND C007 >= {1} AND C007 <= {2}  AND C007<={3}  ", o.PolicyID,
                                        Convert.ToDecimal(LStartRecordTime.ToString("yyyyMMddHHmmss")),
                                        Convert.ToDecimal(LDateTimeQueryKeyTime.ToString("yyyyMMddHHmmss")),
                                        Convert.ToDecimal(o.DurationEnd.ToString("yyyyMMddHHmmss")),
                                        Convert.ToDecimal(o.DurationBegin.ToString("yyyyMMddHHmmss")));
                                    FileLog.WriteInfo("ClientMessageG003()", "LSelectStr:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                                    optReturn = MssqlOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LSelectStr);
                                    if (!optReturn.Result)
                                    {
                                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                        FileLog.WriteError("ClientMessageG003()", "Exe SQL Error:" + optReturn.Message + " SQL:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                                        SendMessageToClient(LStrSendMessage);
                                        StopThisClientThread();
                                        return;
                                    }
                                    objDataSet = optReturn.Data as DataSet;
                                }
                                break;
                            case 3:
                                {
                                    LSelectStr = string.Format("  SELECT C001, C002,C004, C005,C007,C008  FROM T_24_005  WHERE C002 ={0} AND  C007 >= {1} AND C007 <= {2}   AND C007<={3} ",  o.PolicyID,
                                        Convert.ToDecimal(LStartRecordTime.ToString("yyyyMMddHHmmss")),
                                        Convert.ToDecimal(LDateTimeQueryKeyTime.ToString("yyyyMMddHHmmss")),
                                        Convert.ToDecimal(o.DurationEnd.ToString("yyyyMMddHHmmss")),
                                        Convert.ToDecimal(o.DurationBegin.ToString("yyyyMMddHHmmss")));
                                    FileLog.WriteInfo("ClientMessageG003()", "LSelectStr:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                                    optReturn = OracleOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LSelectStr);
                                    if (!optReturn.Result)
                                    {
                                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                        FileLog.WriteError("ClientMessageG003()", "Exe SQL Error:" + optReturn.Message + " SQL:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                                        SendMessageToClient(LStrSendMessage);
                                        StopThisClientThread();
                                        return;
                                    }
                                    objDataSet = optReturn.Data as DataSet;
                                }
                                break;
                            default:
                                {
                                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                    FileLog.WriteInfo("ClientMessageG003()","Error: "+ GetErrorCode(Service06ErrorCode.ERROR06) + " |  FromIP:" + RemoteEndpoint );
                                    SendMessageToClient(LStrSendMessage);
                                    StopThisClientThread();

                                }
                                return;
                        }
                    }
                    else 
                    {
                        FileLog.WriteInfo("ClientMessageG003()", "!=policyID");
                        switch (UMPService06.IIntDBType)
                        {
                            case 2:
                                {
                                    LSelectStr = string.Format("SELECT C001, C002,C004, C005, C007,C008 FROM T_24_005  WHERE C002 ={0} AND C007 <= {1}  AND C007<={2}  ", o.PolicyID,
                                        Convert.ToDecimal(LDateTimeQueryKeyTime.ToString("yyyyMMddHHmmss")),
                                        Convert.ToDecimal(o.DurationEnd.ToString("yyyyMMddHHmmss")),
                                        Convert.ToDecimal(o.DurationBegin.ToString("yyyyMMddHHmmss")));
                                    FileLog.WriteInfo("ClientMessageG003()", "LSelectStr:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                                    optReturn = MssqlOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LSelectStr);
                                    if (!optReturn.Result)
                                    {
                                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                        FileLog.WriteError("ClientMessageG003()", "Exe SQL Error:" + optReturn.Message + " SQL:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                                        SendMessageToClient(LStrSendMessage);
                                        StopThisClientThread();
                                        return;
                                    }
                                    objDataSet = optReturn.Data as DataSet;
                                }
                                break;
                            case 3:
                                {
                                    LSelectStr = string.Format("  SELECT C001, C002,C004, C005,C007,C008  FROM T_24_005  WHERE C002 ={0} AND   C007 <= {1}   AND C007<={2}  ",  o.PolicyID,
                                        Convert.ToDecimal(LDateTimeQueryKeyTime.ToString("yyyyMMddHHmmss")),
                                        Convert.ToDecimal(o.DurationEnd.ToString("yyyyMMddHHmmss")),
                                        Convert.ToDecimal(o.DurationBegin.ToString("yyyyMMddHHmmss")));
                                    FileLog.WriteInfo("ClientMessageG003()", "LSelectStr:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                                    optReturn = OracleOperation.GetDataSet(UMPService06.IStrDBConnectProfile, LSelectStr);
                                    if (!optReturn.Result)
                                    {
                                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                        FileLog.WriteError("ClientMessageG003()", "Exe SQL Error:" + optReturn.Message + " SQL:" + LSelectStr + " |  FromIP:" + RemoteEndpoint);
                                        SendMessageToClient(LStrSendMessage);
                                        StopThisClientThread();
                                        return;
                                    }
                                    objDataSet = optReturn.Data as DataSet;
                                }
                                break;
                            default:
                                {
                                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR06), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                    FileLog.WriteInfo("ClientMessageG003()"," Error: "+ GetErrorCode(Service06ErrorCode.ERROR06) + " |  FromIP:" + RemoteEndpoint );
                                    SendMessageToClient(LStrSendMessage);
                                    StopThisClientThread();

                                }
                                return;
                        }

                    }

                    if (objDataSet != null && objDataSet.Tables[0] != null)
                    {
                        if (objDataSet.Tables[0].Rows.Count > 0)
                        {
                            for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                            {
                                EncryptionKeyDictionary encry = new EncryptionKeyDictionary();
                                DataRow dr = objDataSet.Tables[0].Rows[i];
                                encry.KeyID = dr["C001"].ToString();
                                encry.PolicyID = dr["C002"].ToString();
                                encry.Key1b = dr["C004"].ToString();
                                encry.Key1d = dr["C005"].ToString();
                                encry.EffectiveTime = StringToDateTime(dr["C007"].ToString());
                                encry.InvalidTime = StringToDateTime(dr["C008"].ToString());
                                lstNewEncry.Add(encry);
                            }
                        }
                    }
                    else 
                    {
                        //应该发消息的
                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR07), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        FileLog.WriteInfo("ClientMessageG003() ", "objDataSet =null 3 " + GetErrorCode(Service06ErrorCode.ERROR07) + " |  FromIP:" + RemoteEndpoint);
                        SendMessageToClient(LStrSendMessage);
                        StopThisClientThread();
                        
                    }
                }


                //看所有新密钥里和传过来的密钥参数有没有匹配的
                bool LHaveEqual = false;
                foreach(EncryptionKeyDictionary encry in  lstNewEncry)
                {
                    if (EncryptionAndDecryption.EncryptDecryptString(encry.Key1b, LStrVerificationCode125, EncryptionAndDecryption.UMPKeyAndIVType.M125).Equals(LNewKey1b))
                    
                    {
                        LHaveEqual = true;
                        FileLog.WriteInfo("ClientMessageG003()", "LHaveEqual: true  |  FromIP:" + RemoteEndpoint);
                        break;
                    }
                }

                if (LHaveEqual)
                {
                    string LOldKey1b = EncryptionAndDecryption.EncryptDecryptString(LTrueEnyptionKeyDictionary.Key1b, LStrVerificationCode125, EncryptionAndDecryption.UMPKeyAndIVType.M125);
                    LStrSendMessage = LStrSendMessage = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}",
            EncryptionAndDecryption.EncryptDecryptString(LTrueEnyptionKeyDictionary.KeyID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004),
            AscCodeToChr(27),
            EncryptionAndDecryption.EncryptDecryptString(LTrueEnyptionKeyDictionary.PolicyID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004),
            EncryptionAndDecryption.EncryptDecryptString(LOldKey1b, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004),
            LTrueEnyptionKeyDictionary.Key1d);
                    SendMessageToClient(LStrSendMessage);
                    FileLog.WriteInfo("ClientMessageG003()", "Send Key:" + LStrSendMessage + " |  FromIP:" + RemoteEndpoint);
                    return;
                }
                else
                {
                    //没匹配的新密钥
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(GetErrorCode(Service06ErrorCode.ERROR07), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    FileLog.WriteInfo("ClientMessageG003()", "No Fit New KeyID " + GetErrorCode(Service06ErrorCode.ERROR07) + " |  FromIP:" + RemoteEndpoint + "No Fit EncryKey");
                    SendMessageToClient(LStrSendMessage);
                    return;
                }
            }
            catch (Exception ex)
            {
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ClientMessageG003 \nReturnCode:" + GetErrorCode(Service06ErrorCode.ERROR06) + "\n" + ex.ToString(), LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                SendMessageToClient(LStrSendMessage);          //参数错误
                FileLog.WriteError("ClientMessageG003() ", "Error: " +ex.Message.ToString() +" |  FromIP:" + RemoteEndpoint);
                StopThisClientThread();
                return;
            }
        }

        private string GetErrorCode(Service06ErrorCode code)
        {
            string StrErrorCode = string.Empty;
            StrErrorCode =  code.ToString();
            return StrErrorCode;
        }

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
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

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            return false;
        }

        private int IntParse(string str, int defaultValue)
        {
            int outRet = defaultValue;
            int.TryParse(str, out outRet);

            return outRet;
        }

    }
}
