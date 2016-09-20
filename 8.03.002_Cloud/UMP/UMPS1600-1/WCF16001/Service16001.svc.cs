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
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class Service16001 : IService16001
    {
        IService16001CallBack currCallback = null;
        public static Dictionary<long, LoginUserInfo> lstLoginUsers = new Dictionary<long, LoginUserInfo>();
        LoginUserInfo currentUser = null;
        List<Contacter> lstContacters = new List<Contacter>();


        public Service16001()
        {
            currCallback = OperationContext.Current.GetCallbackChannel<IService16001CallBack>();
            LogOperation.WriteLog("Create new channel");
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams">
        /// </param>
        public void LoginSystem(SessionInfo session)
        {
            try
            {
                //判断传过来的参数不为空 
                if (session == null)
                {
                    currCallback.SendSysMessage(MessageType.LoginFailed, "");
                    return;
                }

                string s = string.Empty;
                foreach (KeyValuePair<long, LoginUserInfo> pair in lstLoginUsers)
                {
                    s = S1600EncryptOperation.DecryptWithM002(pair.Value.UserName) + " | ";
                }
                LogOperation.WriteLog("123");
                LogOperation.WriteLog(s);


                currentUser = new LoginUserInfo();
                currentUser.UserID = session.UserID;

                currentUser.UserName = session.UserInfo.UserName;
                currentUser.CallBack = currCallback;

                //如果不包含这个用户 则加入该对象
                //lstLoginUsers.Remove(currentUser.UserID);  
                if (!lstLoginUsers.Keys.Contains(currentUser.UserID))
                {
                    lstLoginUsers.Add(currentUser.UserID, currentUser);
                    s = "User Login : " + S1600EncryptOperation.DecryptWithM002(currentUser.UserName);
                    LogOperation.WriteLog(s);
                }
                else
                {
                    //如果包含这个用户 表明可能是已经登录 或上次登录没有正常退出 则尝试给该通道发消息 无论是否发送成功 都删掉原来的通道 再加入新的
                    IService16001CallBack callbackExists = lstLoginUsers[currentUser.UserID].CallBack;    //如果该用户已经登录 则通知该客户端账号已经在另一地点登录
                    try
                    {
                        callbackExists.SendSysMessage(MessageType.Logined, "");
                        s = "User Logined : " + S1600EncryptOperation.DecryptWithM002(currentUser.UserName);
                        LogOperation.WriteLog(s);
                    }
                    catch (Exception ex)
                    {
                        //此处无需做任何操作 因为无论消息发送是否成功 该通道都要被删掉 
                        s = "User Login send OffLine msg failed : " + S1600EncryptOperation.DecryptWithM002(currentUser.UserName) + ",\t" + ex.Message;
                        LogOperation.WriteLog(s);
                    }

                    lstLoginUsers.Remove(currentUser.UserID);       //此处不可缺 删掉原来的callback 添加进现在的callback 用来更新callback通道
                    lstLoginUsers.Add(currentUser.UserID, currentUser);
                    LogOperation.WriteLog("Delete old and add new ");
                }
                currCallback.SendSysMessage(MessageType.LoginSuceess, "");
                //更新数据库中的用户在线属性
                OperationReturn optReturn = ResourceOperations.ChangeUserStatus(session, "1");
                if (!optReturn.Result)
                {
                    currCallback.SendSysMessage(MessageType.LoginFailed, "code = " + optReturn.Code + " ; message = " + optReturn.Message);
                    return;
                }

                // currCallback.SendSysMessage(MessageType.TestMsg, "Message = " + optReturn.Message);
                //获得联系人
                optReturn = PermissionFuncs.GetContacters(session);
                if (!optReturn.Result)
                {
                    currCallback.SendSysMessage(MessageType.GetFriendFailed, "code = " + optReturn.Code + " ; message = " + optReturn.Message);
                    return;
                }
                currCallback.InitFriendList(optReturn.Data as List<string>);

                //给在线的用户发送上线通知 
                foreach (KeyValuePair<long, LoginUserInfo> user in lstLoginUsers)
                {
                    //通知每个用户 我上线啦 我本人就不用发了
                    if (user.Key != currentUser.UserID)
                    {
                        user.Value.CallBack.SendSysMessage(MessageType.FriendOnline, currentUser.UserID.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                LogOperation.WriteLog("Login error: " + ex.Message);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="session"></param>
        public void LogOff(SessionInfo session)
        {
            //更新数据库中的用户在线属性
            OperationReturn optReturn = ResourceOperations.ChangeUserStatus(session, "0");
            if (!optReturn.Result)
            {
                currCallback.SendSysMessage(MessageType.LogOffFailed, "");
                return;
            }

            if (lstLoginUsers.Keys.Contains(currentUser.UserID))
            {
                lstLoginUsers.Remove(currentUser.UserID);
                foreach (KeyValuePair<long, LoginUserInfo> user in lstLoginUsers)
                {
                    if (user.Key != currentUser.UserID)
                    {
                        user.Value.CallBack.SendSysMessage(MessageType.FriendOffline, currentUser.UserID.ToString());
                    }
                }
            }
        }

        /// <summary>
        ///  客户端给服务端发送心跳消息
        /// </summary>
        public void SendHeartMsg()
        {
            currCallback.SendSysMessage(MessageType.HeartMsg, "");
        }

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="strSender">发送人</param>
        /// <param name="strReciver">接收人</param>
        /// <param name="strMsg">消息</param>
        public void SendChatMessage(long lSenderID, long lReciverID, string strMsg)
        {
            try
            {
                if (lstLoginUsers.Keys.Contains(lReciverID))
                {
                    IService16001CallBack ReciverCallback = lstLoginUsers[lReciverID].CallBack;
                    string strChatMsg = string.Format("{0}    {1}\r\n{2}\r\n", lstLoginUsers[lSenderID].UserName, System.DateTime.Now, strMsg);
                    LogOperation.WriteLog(strChatMsg);
                    ReciverCallback.SendChatMsg(strChatMsg, lstLoginUsers[lSenderID].UserName); //发送端消息发送到接受端时 需要用发送端的名字来找到窗口
                }
            }
            catch
            {
                currCallback.SendSysMessage(MessageType.SendMsgError, strMsg);
            }
        }
    }
}
