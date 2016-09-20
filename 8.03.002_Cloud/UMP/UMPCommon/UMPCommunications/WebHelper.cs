//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    85f66d9e-f2b5-46c6-9830-1240873a09aa
//        CLR Version:              4.0.30319.18444
//        Name:                     WebHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                WebHelper
//
//        created by Charley at 2014/8/28 15:38:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Xml;
using VoiceCyber.UMP.Common;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// Web操作的帮助类型
    /// </summary>
    public class WebHelper
    {
        /// <summary>
        /// 创建BasicHttpBinding，并进行默认设置
        /// </summary>
        /// <returns></returns>
        public static BasicHttpBinding CreateBasicHttpBinding()
        {
            //默认启用https传输层加密，无需验证客户端证书
            BasicHttpSecurity security = new BasicHttpSecurity();
            security.Mode = BasicHttpSecurityMode.Transport;
            security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
            quotas.MaxArrayLength = int.MaxValue;
            quotas.MaxStringContentLength = int.MaxValue;
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Security = security;
            binding.ReaderQuotas = quotas;
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            binding.SendTimeout = new TimeSpan(0, 10, 0);
            binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
            return binding;
        }
        /// <summary>
        /// 根据Session指定的信息创建BasicHttpBinding
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static BasicHttpBinding CreateBasicHttpBinding(SessionInfo session)
        {
            BasicHttpBinding binding = new BasicHttpBinding();
            BasicHttpSecurity security = new BasicHttpSecurity();
            if (session.AppServerInfo.SupportHttps)
            {
                security.Mode = BasicHttpSecurityMode.Transport;
                security.Transport.ClientCredentialType = HttpClientCredentialType.None;
            }
            else
            {
                security.Mode = BasicHttpSecurityMode.None;
            }
            binding.Security = security;
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
        /// <summary>
        /// 根据指定的服务端信息及服务名称创建EndpointAddress
        /// </summary>
        /// <param name="serverInfo"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static EndpointAddress CreateEndpointAddress(AppServerInfo serverInfo, string serviceName)
        {
            string url;
            if (serverInfo.SupportHttps)
            {
                url = string.Format("{0}://{1}:{2}/WcfServices/{3}.svc",
                    serverInfo.Protocol,
                    serverInfo.Address,
                    serverInfo.Port,
                    serviceName);
            }
            else
            {
                url = string.Format("{0}://{1}:{2}/Wcf2Client/{3}.svc",
                    serverInfo.Protocol,
                    serverInfo.Address,
                    serverInfo.Port,
                    serviceName);
            }
            EndpointAddress address = new EndpointAddress(new Uri(url, UriKind.Absolute));
            return address;
        }
        /// <summary>
        /// 设置客户端行为（Behavior）
        /// </summary>
        /// <param name="client"></param>
        public static void SetServiceClient<TChannel>(ClientBase<TChannel> client) where TChannel : class
        {
            for (int i = 0; i < client.Endpoint.Contract.Operations.Count; i++)
            {
                var aaa = client.Endpoint.Contract.Operations[i];
                DataContractSerializerOperationBehavior behavior =
                    aaa.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (behavior != null)
                {
                    behavior.MaxItemsInObjectGraph = int.MaxValue;
                }
            }
        }
    }
}
