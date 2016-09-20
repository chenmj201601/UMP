using Common2400;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Web;
using VoiceCyber.Common;

namespace Wcf24011
{
    public class KeyGenServerOperation
    {
        //启用、禁用服务
        public static OperationReturn EnableDisable(string strHost, string strPort, int iOperation, string strHostEncrypted)
        {
            OperationReturn optReturn = new OperationReturn();

            Socket LocalSocket = null;
            int LIntCheckKeyPosition, LIntRNKeyPosition;
            int LIntRecievedData = 0;
            string LStrRecievedData = string.Empty;
            string LStrSession = string.Empty;
            string LStrCerification = string.Empty;

            string LStrSendMessage = string.Empty;
            string LStrReturnCode = string.Empty;

            string LStrDynamicSQL = string.Empty;
            List<string> LocalListStrProfile = new List<string>();
            string LStrResultString = string.Empty;
            try
            {
                LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                LocalSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
                LocalSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
                LocalSocket.Connect(strHost, int.Parse(strPort));
                byte[] LByteReceiveData = new byte[1024];
                LIntCheckKeyPosition = -1; LIntRNKeyPosition = -1;
                while (true)
                {
                    LIntRecievedData = LocalSocket.Receive(LByteReceiveData);
                    byte[] LByteActRecieveData = new byte[LIntRecievedData];
                    Buffer.BlockCopy(LByteReceiveData, 0, LByteActRecieveData, 0, LIntRecievedData);
                    LStrRecievedData += Encoding.Default.GetString(LByteActRecieveData);
                    LIntCheckKeyPosition = LStrRecievedData.IndexOf("Session");
                    if (LIntCheckKeyPosition < 0) { LIntCheckKeyPosition = -1; continue; }
                    LIntRNKeyPosition = LStrRecievedData.IndexOf("\r\n", LIntCheckKeyPosition);
                    if (LIntRNKeyPosition > 0) { break; }
                    else { LIntRNKeyPosition = -1; continue; }
                }
                LStrSession = LStrRecievedData.Substring(LIntCheckKeyPosition + "Session".Length + 2);
                LIntRNKeyPosition = LStrSession.IndexOf("\r\n");
                LStrSession = LStrSession.Substring(0, LIntRNKeyPosition).Trim();
                LStrCerification = S2400EncryptOperation.ConvertSessionToCerification(LStrSession);
                LStrSendMessage = "hello " + LStrCerification + "\r\n";
                LIntCheckKeyPosition = -1; LIntRNKeyPosition = -1;
                LStrRecievedData = string.Empty;
                LocalSocket.Send(Encoding.Default.GetBytes(LStrSendMessage));
                while (true)
                {
                    LIntRecievedData = LocalSocket.Receive(LByteReceiveData);
                    byte[] LByteActRecieveData = new byte[LIntRecievedData];
                    Buffer.BlockCopy(LByteReceiveData, 0, LByteActRecieveData, 0, LIntRecievedData);
                    LStrRecievedData += Encoding.Default.GetString(LByteActRecieveData);

                    LIntCheckKeyPosition = LStrRecievedData.IndexOf("retcode");
                    if (LIntCheckKeyPosition < 0) { LIntCheckKeyPosition = -1; continue; }
                    LIntRNKeyPosition = LStrRecievedData.IndexOf("\r\n", LIntCheckKeyPosition);
                    if (LIntRNKeyPosition > 0) { break; }
                    else { LIntRNKeyPosition = -1; continue; }
                }
                LStrReturnCode = LStrRecievedData.Substring(8, 1);
                if (LStrReturnCode != "0")
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S2400WcfErrorCode.KeyGenServerConnFailed;
                    optReturn.Message = "Key generation server connection failed ";
                    return optReturn;
                }

                if (iOperation == (int)OperationType.Enable)
                {
                    LStrSendMessage = "invokeid=112233;command=enablegenerator;generatorid=" + strHostEncrypted + ";\r\n";
                }
                else if (iOperation == (int)OperationType.Disable)
                {
                    LStrSendMessage = "invokeid=112233;command=disablegenerator;\r\n";
                }
                else
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S2400WcfErrorCode.ParamError;
                    optReturn.Message = "Param error";
                    return optReturn;
                }
                LIntCheckKeyPosition = -1; LIntRNKeyPosition = -1;
                LStrRecievedData = string.Empty;
                LocalSocket.Send(Encoding.Default.GetBytes(LStrSendMessage));
                while (true)
                {
                    LIntRecievedData = LocalSocket.Receive(LByteReceiveData);
                    byte[] LByteActRecieveData = new byte[LIntRecievedData];
                    Buffer.BlockCopy(LByteReceiveData, 0, LByteActRecieveData, 0, LIntRecievedData);
                    LStrRecievedData += Encoding.Default.GetString(LByteActRecieveData);

                    LIntCheckKeyPosition = LStrRecievedData.IndexOf("retcode");
                    if (LIntCheckKeyPosition < 0) { LIntCheckKeyPosition = -1; continue; }
                    LIntRNKeyPosition = LStrRecievedData.IndexOf("\r\n", LIntCheckKeyPosition);
                    if (LIntRNKeyPosition > 0) { break; }
                    else { LIntRNKeyPosition = -1; continue; }
                }
                LStrReturnCode = LStrRecievedData.Substring(25, 1);
                if (LStrReturnCode != "0")
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S2400WcfErrorCode.EnableDisableKeyGenServerFailed;
                    optReturn.StringValue = LStrReturnCode;
                    optReturn.Message = string.Format("Enable / Disable Key Server {0} failed with error code: {1}",strHost, LStrReturnCode);
                    return optReturn;
                }
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.StringValue = LStrReturnCode;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            finally
            {
                if (LocalSocket != null) { if (LocalSocket.Connected) { LocalSocket.Close(); } }
            }

            return optReturn;
        }
    }
}