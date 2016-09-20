using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common1600
{
    public enum MessageType
    {
        LoginFailed = 1,        //登录失败
        Logined = 2,                //已经登录 提示被迫下线
        FriendOnline = 3,       //好友上线
        FriendOffline = 4,     //好友下线
        GetFriendFailed = 5,      //获得好友列表失败
        ConnServerFailed = 6,  //连接服务器失败
        LoginSuceess=7,              //登录成功
        HeartMsg=8,                 //心跳消息
        LogOffFailed=9,         //更新数据库状态为离线时失败
        TestMsg=10,             //测试消息
        SendMsgError=11,        //消息发送失败
    }

    public enum S1600RequestCode
    {
        /// <summary>
        /// 修改用户在线状态
        /// </summary>
        ChangeUserStatus=1600001,
  
    }

    public enum S1600WcfError
    {
        //Session不可用
        SessionNotAvailable=1600901,
        SetUserStatusError=1600902,
        /// <summary>
        /// 获得联系人时异常
        /// </summary>
        GetContacterException = 1600903,
        /// <summary>
        /// 获得我的上级时异常
        /// </summary>
        GetSuperiorException=1600904,
        /// <summary>
        /// 获得我的下属时异常
        /// </summary>
        GetSubordinateException=1600905,
        /// <summary>
        /// 获得我的上级时出错
        /// </summary>
        GetSuperiorError=1900906,
        /// <summary>
        /// 获得我的下属时出错
        /// </summary>
        GetSubordinateError=1600907,
        /// <summary>
        /// 检查用户属性出现异常
        /// </summary>
        CheckUserPropertyException=1600908,
        /// <summary>
        /// 检查用户属性出现错误
        /// </summary>
        CheckUserPropertyError=1600909,
        /// <summary>
        /// 用户属性为空
        /// </summary>
        PropertyNone=1600910,
    }

    public enum UserStatusChangeType
    {
        Login=1,
        LogOff=2,
        //新消息
        NewMsg=3
    }

    /// <summary>
    /// 属性加密模式
    /// </summary>
    public enum ObjectPropertyEncryptMode
    {
        /// <summary>
        /// 未知(不加密)
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 加密版本2，模式hex，AES加密
        /// </summary>
        E2Hex = 21,
        /// <summary>
        /// SHA256加密
        /// </summary>
        SHA256 = 31
    }

}
