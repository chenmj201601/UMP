using Common1600;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace WCF16001
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service16001 : IService16001
    {
        IService16001CallBack currCallback = null;
        public static Dictionary<long, LoginUserInfo> lstLoginUsers = new Dictionary<long, LoginUserInfo>();
        LoginUserInfo currentUser = null;
        List<Contacter> lstContacters = new List<Contacter>();

        public Service16001()
        {
            currCallback = OperationContext.Current.GetCallbackChannel<IService16001CallBack>();
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams">
        /// </param>
        public void LoginSystem(SessionInfo session)
        {
            List<string> lstSysMsg = new List<string>();
            try
            {
                //判断传过来的参数不为空 
                if (session == null)
                {
                    currCallback.SendSysMessage(S1600MessageType.LoginFailed, lstSysMsg);
                    return;
                }
                var dbInfo = session.DatabaseInfo;
                if (dbInfo != null)
                {
                    dbInfo.RealPassword = WCF16001EncryptOperation.DecryptWithM004(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }

                string s = string.Empty;
                foreach (KeyValuePair<long, LoginUserInfo> pair in lstLoginUsers)
                {
                    s = WCF16001EncryptOperation.DecryptWithM004(pair.Value.UserName) + " | ";
                }
                //LogOperation.WriteLog(s);

                currentUser = new LoginUserInfo();
                currentUser.UserID = session.UserID;
                currentUser.UserSession = session;

                currentUser.UserName = session.UserInfo.Account;
                currentUser.CallBack = currCallback;

                //如果不包含这个用户 则加入该对象
                //lstLoginUsers.Remove(currentUser.UserID);  
                if (!lstLoginUsers.Keys.Contains(currentUser.UserID))
                {
                    lstLoginUsers.Add(currentUser.UserID, currentUser);
                    s = "User Login : " + WCF16001EncryptOperation.DecryptWithM004(currentUser.UserName);
                    // LogOperation.WriteLog(s);
                }
                else
                {
                    //如果包含这个用户 表明可能是已经登录 或上次登录没有正常退出 则尝试给该通道发消息 无论是否发送成功 都删掉原来的通道 再加入新的
                    IService16001CallBack callbackExists = lstLoginUsers[currentUser.UserID].CallBack;    //如果该用户已经登录 则通知该客户端账号已经在另一地点登录
                    try
                    {
                        callbackExists.SendSysMessage(S1600MessageType.Logined, lstSysMsg);
                        s = "User Logined : " + WCF16001EncryptOperation.DecryptWithM004(currentUser.UserName);
                        // LogOperation.WriteLog(s);
                    }
                    catch (Exception ex)
                    {
                        //此处无需做任何操作 因为无论消息发送是否成功 该通道都要被删掉 
                        s = "User Login send OffLine msg failed : " + WCF16001EncryptOperation.DecryptWithM004(currentUser.UserName) + ",\t" + ex.Message;
                        //LogOperation.WriteLog(s);
                    }

                    lstLoginUsers.Remove(currentUser.UserID);       //此处不可缺 删掉原来的callback 添加进现在的callback 用来更新callback通道
                    lstLoginUsers.Add(currentUser.UserID, currentUser);
                    //LogOperation.WriteLog("Delete old and add new ");
                }
                currCallback.SendSysMessage(S1600MessageType.LoginSuceess, lstSysMsg);
                //更新数据库中的用户在线属性
                OperationReturn optReturn = ResourceOperations.ChangeUserStatus(session, "1");
                if (!optReturn.Result)
                {
                    lstSysMsg.Clear();
                    lstSysMsg.Add("code = " + optReturn.Code + " ; message = " + optReturn.Message);
                    currCallback.SendSysMessage(S1600MessageType.LoginFailed, lstSysMsg);
                    return;
                }

                // currCallback.SendSysMessage(MessageType.TestMsg, "Message = " + optReturn.Message);
                //获得联系人
                optReturn = PermissionFuncs.GetContacters(session);
                if (!optReturn.Result)
                {
                    lstSysMsg.Clear();
                    lstSysMsg.Add("code = " + optReturn.Code + " ; message = " + optReturn.Message);
                    currCallback.SendSysMessage(S1600MessageType.GetFriendFailed, lstSysMsg);
                    return;
                }
                currCallback.InitFriendList(optReturn.Data as List<string>);

                //给在线的用户发送上线通知 
                foreach (KeyValuePair<long, LoginUserInfo> user in lstLoginUsers)
                {
                    //通知每个用户 我上线啦 我本人就不用发了
                    if (user.Key != currentUser.UserID)
                    {
                        lstSysMsg.Clear();
                        lstSysMsg.Add(currentUser.UserID.ToString());
                        user.Value.CallBack.SendSysMessage(S1600MessageType.FriendOnline, lstSysMsg);
                    }
                }

                //开始接收离线消息
                List<string> lstParam = new List<string>();
                lstParam.Add(currentUser.UserID.ToString());
                optReturn = ChatMessageOperation.GetAllOffLineMsg(session, lstParam);
                if (optReturn.Result)
                {
                    //LogOperation.WriteLog("Send offline message to " + currentUser.UserName);
                    SendOfflineMsg(optReturn.Data as Dictionary<long, ChatMessage>);
                }
            }
            catch (Exception ex)
            {
                //LogOperation.WriteLog("Login error: " + ex.Message);
            }
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="session"></param>
        public void LogOff(SessionInfo session)
        {
            var dbInfo = session.DatabaseInfo;
            if (dbInfo != null)
            {
                dbInfo.RealPassword = WCF16001EncryptOperation.DecryptWithM004(dbInfo.Password);
                session.DBConnectionString = dbInfo.GetConnectionString();
            }
            List<string> lstSysMsg = new List<string>();
            //更新数据库中的用户在线属性
            OperationReturn optReturn = ResourceOperations.ChangeUserStatus(session, "0");
            if (!optReturn.Result)
            {
                currCallback.SendSysMessage(S1600MessageType.LogOffFailed, lstSysMsg);
                //LogOperation.WriteLog("Update " + lstLoginUsers[session.UserID].UserName + " status failed : " + optReturn.Message);
                return;
            }

            if (lstLoginUsers.Keys.Contains(currentUser.UserID))
            {
                lstLoginUsers.Remove(currentUser.UserID);
                foreach (KeyValuePair<long, LoginUserInfo> user in lstLoginUsers)
                {
                    if (user.Key != currentUser.UserID)
                    {
                        lstSysMsg.Clear();
                        lstSysMsg.Add(currentUser.UserID.ToString());
                        user.Value.CallBack.SendSysMessage(S1600MessageType.FriendOffline, lstSysMsg);
                    }
                }
            }
            //LogOperation.WriteLog("User " + lstLoginUsers[session.UserID].UserName + " logoff");
        }

        /// <summary>
        ///  客户端给服务端发送心跳消息
        /// </summary>
        public void SendHeartMsg()
        {
            try
            {
                currCallback.SendSysMessage(S1600MessageType.HeartMsg, new List<string>());
                //LogOperation.WriteLog("心跳消息");
            }
            catch (Exception ex)
            {
                LogOff(currentUser.UserSession);
                // LogOperation.WriteLog("Send " + currentUser.UserName + " heart failed" + ex.Message);
            }
        }

        /// <summary>
        /// 发送聊天消息
        /// </summary>
        /// <param name="msgObj"></param>
        /// <param name="bNewCookie">是否新会话</param>
        public void SendChatMessage(ChatMessage msgObj, bool bNewCookie)
        {
            //如果是新cookie 需要获得一个新cookie 写入数据库 并返回给客户端
            List<string> lstParams = new List<string>();
            lstParams.Add(bNewCookie == true ? "0" : "1");
            //判断消息接收人是否在线 在线 则写入在线消息表 离线 则写入离线消息表
            bool bReceiverOnLine = lstLoginUsers.Keys.Contains(msgObj.ReceiverID);
            lstParams.Add(bReceiverOnLine ? "2" : "3");
            OperationReturn optReturn = ChatMessageOperation.WriteChatMsg(lstLoginUsers[msgObj.SenderID].UserSession, msgObj, lstParams);
            if (!optReturn.Result)
            {
                //如果写入数据库出错 则给发送者发消息说发送失败
                List<string> lstReturn = new List<string>();
                lstReturn.Add(msgObj.MsgContent);
                lstReturn.Add(msgObj.ReceiverName);
                lstLoginUsers[msgObj.SenderID].CallBack.SendSysMessage(S1600MessageType.SendMsgError, lstReturn);
                return;
            }
            long cookieID = 0;
            long.TryParse(optReturn.Data.ToString(), out cookieID);
            //LogOperation.WriteLog("CookieID = " + cookieID);
            if (bNewCookie)
            {
                lstParams.Clear();
                lstParams.Add(msgObj.ReceiverID.ToString());
                lstParams.Add(cookieID.ToString());
                lstLoginUsers[msgObj.SenderID].CallBack.SendSysMessage(S1600MessageType.CookieID, lstParams);
                // LogOperation.WriteLog("Send cookieID to " + msgObj.SenderName);
            }
            msgObj.CookieID = cookieID;
            //如果写入数据库成功 则发给接收者 并告诉发送者cookieid
            try
            {
                if (lstLoginUsers.Keys.Contains(msgObj.ReceiverID))
                {
                    lstLoginUsers[msgObj.ReceiverID].CallBack.ReceiveChatMsg(msgObj);
                }
            }
            catch (Exception ex)
            {
                lstLoginUsers.Remove(msgObj.ReceiverID);
                LogOff(lstLoginUsers[msgObj.ReceiverID].UserSession);
                //LogOperation.WriteLog("Send chat message to " + msgObj.ReceiverName + " failed," + ex.Message + ",");
            }
            //LogOperation.WriteLog("Send message to " + msgObj.ReceiverName + ",done");
        }

        /// <summary>
        /// 发送离线消息
        /// </summary>
        /// <param name="lstMsgs"></param>
        private void SendOfflineMsg(Dictionary<long, ChatMessage> dicMsgs)
        {
            foreach (KeyValuePair<long, ChatMessage> pair in dicMsgs)
            {
                try
                {
                    currCallback.ReceiveChatMsg(pair.Value);
                    //删掉已经发送的离线消息
                    List<string> lstParams = new List<string>();
                    lstParams.Add(pair.Key.ToString());
                    ChatMessageOperation.DelteOfflineMsg(currentUser.UserSession, lstParams);
                }
                catch (Exception ex)
                {
                    LogOff(currentUser.UserSession);
                    lstLoginUsers.Remove(currentUser.UserID);
                    //LogOperation.WriteLog("Send offline msg error,account abnormal exit: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// 结束会话 并通知消息接收方 对话已经结束
        /// </summary>
        /// lstParams[0] : cookieID
        /// lstParams[1] : 消息发送人ID
        /// lstParams[2] : 消息接收人ID 用于通知
        /// <param name="lstParams"></param>
        public void EndCookieByID(List<string> lstParams)
        {
            //LogOperation.WriteLog("End cookie " + lstParams[0]);
            long lUserID = long.Parse(lstParams[1]);
            SessionInfo session = null;
            if (!lstLoginUsers.Keys.Contains(lUserID))
            {
                //LogOperation.WriteLog("EndCookieByID error ," + lUserID + " is offline");
                return;
            }
            session = currentUser.UserSession;
            var dbInfo = session.DatabaseInfo;
            if (dbInfo != null)
            {
                dbInfo.RealPassword = WCF16001EncryptOperation.DecryptWithM004(dbInfo.Password);
                session.DBConnectionString = dbInfo.GetConnectionString();
            }
            List<string> lstEndCookieParams = new List<string>();
            lstEndCookieParams.Add(lstParams[0]);
            OperationReturn optReturn = ChatMessageOperation.EndCookie(session, lstEndCookieParams);
            if (!optReturn.Result)
            {
                return;
            }
            long lReceiverID = long.Parse(lstParams[2]);
            if (!lstLoginUsers.Keys.Contains(lReceiverID))
            {
                //如果对方已经下线 不需要通知
                return;
            }
            //如果对方还在线 需要通知对方会话已经结束
            lstEndCookieParams.Clear();
            lstEndCookieParams.Add(lstLoginUsers[lUserID].UserName);
            lstEndCookieParams.Add(lUserID.ToString());
            if (lstLoginUsers.Keys.Contains(lReceiverID))
            {
                try
                {
                    lstLoginUsers[lReceiverID].CallBack.SendSysMessage(S1600MessageType.EndCookie, lstEndCookieParams);
                }
                catch (Exception ex)
                {
                    lstLoginUsers.Remove(lReceiverID);
                    //LogOperation.WriteLog("End cookie, and send message to " + lReceiverID + " failed:" + ex.Message);
                }
            }

            //LogOperation.WriteLog("End cookie done");
        }
    }
}
