//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fdfddf01-3f94-4b23-a5a2-464c810fde59
//        CLR Version:              4.0.30319.18444
//        Name:                     IMessageHandler
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                IMessageHandler
//
//        created by Charley at 2014/8/25 12:00:16
//        http://www.voicecyber.com 
//
//======================================================================

using System.ServiceModel;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 在NetPipe通讯中，使用此接口调用远程操作
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IMessageHandler
    {
        /// <summary>
        /// 处理远程消息
        /// </summary>
        /// <param name="webRequest">请求消息</param>
        /// <returns>响应消息</returns>
        [OperationContract]
        WebReturn DealMessage(WebRequest webRequest);
    }
}
