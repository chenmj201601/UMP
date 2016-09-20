//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    72aca75e-0fb1-4a7d-a90c-2aca8f4725f1
//        CLR Version:              4.0.30319.18444
//        Name:                     NetPipeHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                NetPipeHelper
//
//        created by Charley at 2014/8/25 12:04:36
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Xml;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// NetPipe通讯的帮助类，创建管道并处理管道消息
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public class NetPipeHelper : IMessageHandler
    {

        #region IsServer

        private bool mIsServer;
        /// <summary>
        /// 是否是服务端
        /// </summary>
        public bool IsServer
        {
            get { return mIsServer; }
        }

        #endregion


        #region ClientID

        private string mClientID;
        /// <summary>
        /// 客户端编号（一般是子系统编号，如UMPS1101（机构管理）），如果作为服务端时，ID为空
        /// </summary>
        public string ClientID
        {
            get { return mClientID; }
            set { mClientID = value; }
        }

        private List<string> mListClients;
        /// <summary>
        /// 客户端集合，作为服务端时，将接入的客户端都存放到此列表中
        /// </summary>
        public List<string> ListClients
        {
            get { return mListClients; }
        }

        #endregion


        #region Constructors
        /// <summary>
        /// 指定类型及ID创建一个实例
        /// </summary>
        /// <param name="isServer"></param>
        /// <param name="id"></param>
        public NetPipeHelper(bool isServer, string id)
        {
            mListClients = new List<string>();

            mIsServer = isServer;
            mClientID = id;
        }
        #endregion


        #region IMessageHandler
        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        public WebReturn DealMessage(WebRequest webRequest)
        {
            WebReturn webReturn = new WebReturn();
            if (DealMessageFunc != null)
            {
                webReturn = DealMessageFunc(webRequest);
            }
            return webReturn;
        }

        #endregion


        #region SendMessage
        /// <summary>
        /// 向指定的客户端发送消息
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="clientID"></param>
        /// <returns></returns>
        public WebReturn SendMessage(WebRequest webRequest, string clientID)
        {
            return SendMessage(webRequest, clientID, !IsServer);
        }

        public WebReturn SendMessage(WebRequest webRequest, string clientID, bool isToServer)
        {
            WebReturn webReturn = new WebReturn();
            string url;
            if (isToServer)
            {
                url = string.Format("net.pipe://localhost/UMPServer");
            }
            else
            {
                url = string.Format("net.pipe://localhost/Client_{0}", clientID);
            }
            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
            quotas.MaxArrayLength = int.MaxValue;
            quotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas = quotas;
            ChannelFactory<IMessageHandler> factory = new ChannelFactory<IMessageHandler>(binding,
                new EndpointAddress(url));
            IMessageHandler handler = factory.CreateChannel();
            try
            {
                return handler.DealMessage(webRequest);
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = 1;
                webReturn.Message = ex.Message;
                return webReturn;
            }
        }

        #endregion


        #region Function DealMessage
        /// <summary>
        /// 处理接收到的消息
        /// </summary>
        public event Func<WebRequest, WebReturn> DealMessageFunc;

        #endregion


        #region Service Host

        private ServiceHost mServiceHost;
        /// <summary>
        /// 启动服务
        /// </summary>
        /// <returns></returns>
        public void Start()
        {
            if (mServiceHost != null)
            {
                try
                {
                    mServiceHost.Close();
                }
                catch (Exception)
                { }
            }
            string url;
            if (IsServer)
            {
                url = string.Format("net.pipe://localhost/UMPServer");
            }
            else
            {
                url = string.Format("net.pipe://localhost/Client_{0}", mClientID);
            }
            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.MaxBufferPoolSize = int.MaxValue;
            binding.MaxBufferSize = int.MaxValue;
            XmlDictionaryReaderQuotas quotas = new XmlDictionaryReaderQuotas();
            quotas.MaxArrayLength = int.MaxValue;
            quotas.MaxStringContentLength = int.MaxValue;
            binding.ReaderQuotas = quotas;
            mServiceHost = new ServiceHost(this);
            mServiceHost.AddServiceEndpoint(typeof(IMessageHandler), binding, url);
            mServiceHost.Open();
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void Stop()
        {
            try
            {
                if (mServiceHost != null)
                {
                    mServiceHost.Close();
                }
            }
            catch (Exception)
            { }
        }

        #endregion

    }
}
