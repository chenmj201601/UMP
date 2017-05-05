using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace TestSSLSocketClient
{
    class Program
    {
        public static int Main(string[] args)
        {
            string serverCertificateName = null;
            string machineName = null;
            if (args == null || args.Length < 1)
            {
                DisplayUsage();
            }
            machineName = args[0];
            if (args.Length < 2)
            {
                serverCertificateName = machineName;
            }
            else
            {
                serverCertificateName = args[1];
            }
            RunClient(machineName, serverCertificateName);
            return 0;

        }

        private static Hashtable certificateErrors = new Hashtable();
        public static bool ValidateServerCertificate(
            object sender,
            X509Certificate certificate,
            X509Chain chain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None)
                return true;

            Console.WriteLine("Certificate error: {0}", sslPolicyErrors);
            return false;
        }

        public static void RunClient(string machineName, string serverName)
        {
            string LStrSendMessage = string.Empty;
            TcpClient client = new TcpClient(machineName, 8080);
            Console.WriteLine("Client connected.");
            
            SslStream sslStream = new SslStream(
                client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null
                );
            try
            {
                sslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Tls, false);
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                client.Close();
                return;
            }
            //模拟用户登录
            LStrSendMessage = "M01A01" + AscCodeToChr(27) + "administrator"+AscCodeToChr(27) + "UMP.123";
            LStrSendMessage += AscCodeToChr(27) + "N" + AscCodeToChr(27) + "21001" + AscCodeToChr(27) + "YoungPC" + AscCodeToChr(27) + "192.168.4.188";
            ////模拟用户退出
            //LStrSendMessage = "M01A02" + AscCodeToChr(27) + "00000" + AscCodeToChr(27) + "1020000000000000001";
            //LStrSendMessage += AscCodeToChr(27) + "9021409160600000001";
            ////模拟用户在线
            //LStrSendMessage = "M01A03" + AscCodeToChr(27) + "00000" + AscCodeToChr(27) + "1020000000000000001";
            //LStrSendMessage += AscCodeToChr(27) + "9021409160600000001";
         
            byte[] messsage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
            sslStream.Write(messsage);
            sslStream.Flush();
            string serverMessage = ReadMessage(sslStream);
            Console.WriteLine("Server says: {0}", serverMessage);

            client.Close();
            Console.WriteLine("Client closed.");
        }

        static string ReadMessage(SslStream sslStream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = sslStream.Read(buffer, 0, buffer.Length);
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                if (messageData.ToString().IndexOf("\r\n") != -1)
                {
                    break;
                }
            }
            while (bytes != 0);
            return messageData.ToString();
        }

        private static void DisplayUsage()
        {
            Console.WriteLine("To start the client specify:");
            Console.WriteLine("clientSync machineName [serverName]");
            Environment.Exit(1);
        }

        private static string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }
    }

    
}
