using Common1600;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace WCF16001
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract(CallbackContract = typeof(IService16001CallBack))]
    public interface IService16001
    {
        /// <summary>
        /// 登录 通过传过来的session 连接UMP的wcf 验证用户名和密码 如果正确 就修改用户的在线状态 如果错误 发送错误码和错误消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        [OperationContract(IsOneWay = true)]
        void LoginSystem(SessionInfo session);

        /// <summary>
        /// 客户端发送心跳消息给服务端
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void SendHeartMsg();

        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="session"></param>
        [OperationContract(IsOneWay = true)]
        void LogOff(SessionInfo session);

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="strSender">发送人</param>
        /// <param name="strReciver">接收人</param>
        /// <param name="strMsg">消息</param>
        [OperationContract(IsOneWay = true)]
        void SendChatMessage(ChatMessage msgObj, bool bNewCookie);

        /// <summary>
        /// 结束会话 并通知消息接收方 对话已经结束
        /// </summary>
        /// <param name="lstParams">
        /// lstParams[0] : cookieID
        /// lstParams[1] : 消息发送人ID
        /// lstParams[2] : 消息接收人ID
        /// </param>
        [OperationContract(IsOneWay = true)]
        void EndCookieByID(List<string> lstParams);
    }

    public interface IService16001CallBack
    {
        /// <summary>
        /// 发送系统消息 
        /// </summary>
        /// <param name="iMesType"></param>
        /// <param name="strTag"></param>
        [OperationContract(IsOneWay = true)]
        void SendSysMessage(S1600MessageType mesType, List<string> lstArgs);

        [OperationContract(IsOneWay = true)]
        void InitFriendList(List<string> lstFriends);

        /// <summary>
        /// 把聊天消息发给接收人
        /// </summary>
        /// <param name="strMsg">消息</param>
        /// <param name="strRevicer">消息接收人</param>
        [OperationContract(IsOneWay = true)]
        void ReceiveChatMsg(ChatMessage chatMsg);
    }

}
