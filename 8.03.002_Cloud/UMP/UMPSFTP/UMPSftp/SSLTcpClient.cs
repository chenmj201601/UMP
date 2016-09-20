using System;
using System.Security;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Text;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.IO;
using System.Collections;

namespace UMPSftp
{
    class SSLTcpClient
    {
        private static Hashtable certificateErrors = new Hashtable();

        TcpClient _client = null;
        SslStream _sslStream = null;
        // The following method is invoked by the RemoteCertificateValidationDelegate.
        public static bool ValidateServerCertificate(
              object sender,
              X509Certificate certificate,
              X509Chain chain,
              SslPolicyErrors sslPolicyErrors)
        {
            //if (sslPolicyErrors == SslPolicyErrors.None)
            return true;    
        }

        public void RunClient(string machineName, int machinePort)
        {
            // Create a TCP/IP client socket.
            // machineName is the host running the server application.
            if (_client == null)
            {
                _client = new TcpClient(machineName, machinePort);
            }
            Console.WriteLine("Client connected.");
            // Create an SSL stream that will close the client's stream.
            _sslStream = new SslStream(_client.GetStream(),
                false,
                new RemoteCertificateValidationCallback(ValidateServerCertificate),
                null);
            _sslStream.ReadTimeout = 5000;
            _sslStream.WriteTimeout = 5000;

            // The server name must match the name on the server certificate.

            //X509Store store = new X509Store(StoreName.My);
            //store.Open(OpenFlags.ReadWrite);

            //// 检索证书 
            //X509Certificate2Collection certs = store.Certificates.Find(X509FindType.FindBySubjectName, "MyServer", false); // vaildOnly = true时搜索无结果。

            //X509CertificateCollection certs = new X509CertificateCollection();
            //X509Certificate cert = X509Certificate.CreateFromCertFile(@"D:\cashcer.cer");
            //certs.Add(cert);
            try
            {
                _sslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Tls, false);
            }
            catch (AuthenticationException e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                _client.Close();
                return;
            }
            // Encode a test message into a byte array.
            // Signal the end of the message using the "<EOF>".
            // Read message from the server.
        }
        public void Close()
        {
            if (_client != null)
            {
                _client.Close();
                _client = null;
                _sslStream = null;
            }
            Console.WriteLine("Client closed.");
        }

        public string SendMessage(string string_message)
        {
            try
            {
                if (_client == null || !_client.Connected)
                {
                    return "";
                }
                byte[] messsage = Encoding.UTF8.GetBytes(string_message + "\r\n");
                // Send hello message to the server. 
                _sslStream.Write(messsage);

                _sslStream.Flush();
                string serverMessage = ReadMessage();
                return serverMessage;

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }

        public string ReadMessage()
        {       
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            try
            {
                do
                {
                    bytes = _sslStream.Read(buffer, 0, buffer.Length);

                    // Use Decoder class to convert from bytes to UTF8
                    // in case a character spans two buffers.
                    Decoder decoder = Encoding.UTF8.GetDecoder();
                    char[] chars = new char[decoder.GetCharCount(buffer, 0, bytes)];
                    decoder.GetChars(buffer, 0, bytes, chars, 0);
                    messageData.Append(chars);
                    if (messageData.ToString().IndexOf("\r\n") != -1)
                    {
                        break;
                    }
                } while (bytes != 0);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
                messageData.Clear();
            }

            return messageData.ToString();
        }
    }
}
