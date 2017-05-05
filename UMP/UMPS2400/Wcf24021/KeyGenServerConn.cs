using Common2400;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace Wcf24021
{
    public class KeyGenServerConn
    {
        public static string ConvertSessionToCerification(string AStrSessionCode)
        {
            string LocalStrCerification = string.Empty;
            string LocalStrSourceString = string.Empty;

            LocalStrSourceString = CommonFunctions. ConvertHexToString(AStrSessionCode);
            char[] LocalCharSourceArray = LocalStrSourceString.ToCharArray();
            byte[] LocalByteSourceArray = new byte[LocalCharSourceArray.Length];
            for (int LIntLoop = 0; LIntLoop < LocalCharSourceArray.Length; LIntLoop++) { LocalByteSourceArray[LIntLoop] = Convert.ToByte(LocalCharSourceArray[LIntLoop]); }

            int LocalIntOffset = LocalByteSourceArray[0] & 0x001f;

            byte LocalByteMask = (byte)(LocalByteSourceArray[LocalIntOffset] ^ LocalIntOffset);

            byte[] LocalByteCerification = new byte[32];

            for (int LIntLoop = 0; LIntLoop < 32; LIntLoop++) { LocalByteCerification[LIntLoop] = (byte)(LocalByteSourceArray[LIntLoop] ^ LocalByteMask); }

            LocalStrCerification = CommonFunctions.ConvertByteToHexStr(LocalByteCerification);

            return LocalStrCerification;
        }

        public static bool SendMessageToGeneratorServer(List<string> AListStrGeneratorProfile, string AStrSendMessage, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            Socket LocalSocket = null;
            int LIntRecievedData = 0;
            string LStrRecievedData = string.Empty;
            string LStrSession = string.Empty;
            string LStrCerification = string.Empty;
            int LIntCheckKeyPosition, LIntRNKeyPosition;
            string LStrSendMessage = string.Empty;
            string LStrReturnCode = string.Empty;

            try
            {
                AStrReturn = string.Empty;

                LocalSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                LocalSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, 10000);
                LocalSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, 10000);
                LocalSocket.Connect(AListStrGeneratorProfile[0], int.Parse(AListStrGeneratorProfile[1]));
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
                LStrCerification = ConvertSessionToCerification(LStrSession);
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
                    LBoolReturn = false;
                    AStrReturn = "Error00" + LStrReturnCode;
                    return LBoolReturn;
                }

                LStrSendMessage = AStrSendMessage;

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
                    LBoolReturn = false;
                    AStrReturn = "Error00" + LStrReturnCode;
                    return LBoolReturn;
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "Windows Communication Foundation(EncryptionService01Service.SendMessageToGeneratorServer)\n" + ex.Message;
            }
            finally
            {
                if (LocalSocket != null) { if (LocalSocket.Connected) { LocalSocket.Close(); } }
            }

            return LBoolReturn;
        }
    }
}