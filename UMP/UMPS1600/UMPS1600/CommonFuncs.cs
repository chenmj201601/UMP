using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Xml;
using VoiceCyber.UMP.Common;

namespace UMPS1600
{
    public class CommonFuncs
    {
        public static NetTcpBinding CreateNetTcpBinding()
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);
            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
            quotas.MaxArrayLength = int.MaxValue;
            quotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas = quotas;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            binding.SendTimeout = new TimeSpan(0, 10, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
            return binding;
        }

        public static EndpointAddress CreateEndPoint(string strUMPServer,string strWcf1600Port)
        {
            string strURl = "net.tcp://{0}:{1}/WCF1600/Service16001.svc/Contract";
            strURl = string.Format(strURl, strUMPServer, strWcf1600Port);
            EndpointAddress myEndpoint = new EndpointAddress(strURl);
            return myEndpoint;
        }
    }
}
