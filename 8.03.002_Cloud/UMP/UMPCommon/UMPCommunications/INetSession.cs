//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3617bc4a-30ba-4374-a94c-8acb7c500cd0
//        CLR Version:              4.0.30319.18063
//        Name:                     INetSession
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                INetSession
//
//        created by Charley at 2015/9/21 10:01:27
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Security.Cryptography.X509Certificates;
using VoiceCyber.Common;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 定义客户端与服务端一次会话的功能
    /// </summary>
    public interface INetSession
    {
        /// <summary>
        /// 设定或返回SSL通讯所用的证书，如果SSL通讯，需要提供安全证书
        /// </summary>
        X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// 返回会话的唯一标识，每一次会话建立，服务端会生成一个GUID，用来唯一标识本次会话
        /// </summary>
        string SessionID { get; }
        /// <summary>
        /// 返回表示客户端的终结点
        /// </summary>
        string RemoteEndpoint { get; }
        /// <summary>
        /// 消息编码方式
        /// </summary>
        int MsgEncoding { get; set; }
        /// <summary>
        /// 消息加密方式
        /// </summary>
        int Encryption { get; set; }
        /// <summary>
        /// 返回最后一次接收或发送消息的时间，每次接收或发送数据都会更新此时间
        /// NetServer会定时检查该时间，如果超过指定的时间，服务端会主动关闭本次连接
        /// </summary>
        DateTime LastActiveTime { get; }
        /// <summary>
        /// 返回当前的连接状态
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 是否启用SSL
        /// </summary>
        bool IsSSL { get; set; }

        /// <summary>
        /// 启动Session
        /// </summary>
        void Start();
        /// <summary>
        /// 停止并关闭Session
        /// </summary>
        void Stop();

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="head">消息头</param>
        /// <param name="message">消息内容</param>
        /// <returns></returns>
        OperationReturn SendMessage(MessageHead head, string message);
        /// <summary>
        /// 根据当前会话信息创建消息头
        /// </summary>
        /// <returns></returns>
        MessageHead GetMessageHead();

        /// <summary>
        /// 调试信息
        /// </summary>
        event Action<LogMode, string, string> Debug;
    }
}
