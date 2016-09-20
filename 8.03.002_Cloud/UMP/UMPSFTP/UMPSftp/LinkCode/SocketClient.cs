using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace DecSDK
{
    public class MessageInfoEventArgs : EventArgs
    {
        public MessageInfoEventArgs(string msg)
        {
            Message = msg;
        }
        public string Message { get; private set; }
    }

    class SocketClient : TcpClient
    {
        bool _ssl = false;
        public bool IsSSL { get { return _ssl; } }
        public event EventHandler<MessageInfoEventArgs> OnReceiveMsg;

        public SocketClient(bool is_ssl = false) : base() { _ssl = is_ssl; }
        public SocketClient(AddressFamily family, bool is_ssl = false) : base(family) { _ssl = is_ssl; }
        public SocketClient(IPEndPoint localEP, bool is_ssl = false) : base(localEP) { _ssl = is_ssl; }
        public SocketClient(string hostname, int port, bool is_ssl = false) : base(hostname, port) { _ssl = is_ssl; }

        public void SendData(string msg)
        {
            SendData(Encoding.ASCII.GetBytes(msg));
        }
        public void SendData(byte[] by_data)
        {
            if (!base.Connected)
                return;
            // Translate the passed message into ASCII and store it as a Byte array.
            Task write_task = GetStreamEx().WriteAsync(by_data, 0, by_data.Length);
            write_task.ContinueWith((task1) =>
            {
                if (task1.IsCanceled || task1.IsFaulted)
                {
                    return;
                }
            });
        }

        Stream GetStreamEx()
        {
            Stream stream = null;
            if (_ssl)
            {
                stream = new SslStream(
                    base.GetStream(),
                    false,
                    new RemoteCertificateValidationCallback(
                        (object sender,
                        X509Certificate certificates,
                        X509Chain chain,
                        SslPolicyErrors sslPolicyErrors) =>
                        {
                            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors
                            || sslPolicyErrors == SslPolicyErrors.None) { return true; }
                            return false;
                        }));
                ((SslStream)stream).AuthenticateAsClient("VoiceCyber.PF", null, System.Security.Authentication.SslProtocols.Tls, false);
            }
            else
            {
                stream = base.GetStream();
            }
            return stream;
        }

        public void StartReceiveData()
        {
            try
            {
                if (!base.Connected)
                    return;
                byte[] byRead = new byte[2048];
                Task<int> read_task = GetStreamEx().ReadAsync(byRead, 0, byRead.Length);
                read_task.ContinueWith((task) =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        return;
                    }
                    StartReceiveData();
                    StringBuilder myCompleteMessage = new StringBuilder();
                    myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(byRead, 0, task.Result));
                    OnReceiveMsg(this, new MessageInfoEventArgs(myCompleteMessage.ToString()));
                });
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
