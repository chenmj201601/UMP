//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8c8a69c8-7d25-42ad-aa25-6d14f81144fa
//        CLR Version:              4.0.30319.18063
//        Name:                     INetClient
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                INetClient
//
//        created by Charley at 2015/9/21 11:00:53
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using VoiceCyber.Common;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 定义Socket通讯客户端的基本功能
    /// </summary>
    public interface INetClient
    {
        /// <summary>
        /// 获取当前会话的唯一标识
        /// </summary>
        string SessionID { get; }
        /// <summary>
        /// 获取当前连接状态
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 获取或设置消息的编码方式
        /// </summary>
        int MsgEncoding { get; set; }
        /// <summary>
        /// 获取或设置消息的加密方式
        /// </summary>
        int Encryption { get; set; }
        /// <summary>
        /// 获取或设置服务主机地址
        /// </summary>
        string Host { get; set; }
        /// <summary>
        /// 获取或设置服务通讯端口
        /// </summary>
        int Port { get; set; }
        /// <summary>
        /// 是否启用SSL
        /// </summary>
        bool IsSSL { get; set; }

        /// <summary>
        /// 连接到服务器
        /// </summary>
        /// <param name="address">服务的主机地址</param>
        /// <param name="port">端口</param>
        /// <returns></returns>
        OperationReturn Connect(string address, int port);
        /// <summary>
        /// 停止并关闭连接
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
        /// 调试消息
        /// </summary>
        event Action<LogMode, string, string> Debug;
        /// <summary>
        /// 连接或认证状态
        /// </summary>
        event EventHandler<ConnectionEventArgs> ConnectionEvent;
        /// <summary>
        /// 收到返回消息
        /// </summary>
        event EventHandler<ReturnMessageReceivedEventArgs> ReturnMessageReceived;
        /// <summary>
        /// 收到通知消息
        /// </summary>
        event EventHandler<NotifyMessageReceivedEventArgs> NotifyMessageReceived;
    }
}
