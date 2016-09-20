using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using WCF16001.Service11012;
using Common1600;
using System.Data;

namespace WCF16001
{
    /// <summary>
    /// 聊天记录表和会话表的操作
    /// </summary>
    public class ChatMessageOperation
    {

        /// <summary>
        /// 将聊天记录写进数据库
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams">
        /// lstParams[0] : 是否创建新cookie 0：创建 1：不创建
        /// lstParams[1] : 2：在线消息 3：离线消息(直接用此值拼表名)
        /// </param>
        /// <returns></returns>
        public static OperationReturn WriteChatMsg(SessionInfo session,ChatMessage chatMsg ,List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strSql = string.Empty;
                string strToken = session.RentInfo.Token;
                long lCookieID = 0;
                long now = 0;
                CommonFuncs.DateTimeToNumber(DateTime.Now, ref now);
                long nowUtc = 0;
                CommonFuncs.DateTimeToNumber(DateTime.Now.ToUniversalTime(), ref nowUtc);
                List<string> lstSerialParam = new List<string>();
                if (lstParams[0] == "0")
                {
                    //创建新会话
                    lstSerialParam.Add("16");
                    lstSerialParam.Add("161");
                    optReturn = GetSerialID(session, lstSerialParam);
                    if (!optReturn.Result)
                    {
                        //LogOperation.WriteLog("WriteChatMsg GetSerialID error: " + optReturn.Message);
                        return optReturn;
                    }
                    lCookieID = long.Parse(optReturn.Data.ToString());
                   // LogOperation.WriteLog("Create new cookie "+lCookieID.ToString());
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = "insert into t_16_001_{0}(C001,C002,C003,C006,C007,C008,C009) values({1},{2},{3},{4},'1',{5},'1')";
                            strSql = string.Format(strSql, strToken, lCookieID, now, nowUtc, chatMsg.SenderID, chatMsg.ResourceID);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                        case 3:
                            strSql = "insert into t_16_001_{0}(C001,C002,C003,C006,C007,C008,C009) values({1},{2},{3},{4},'1',{5},'1')";
                            strSql = string.Format(strSql, strToken, lCookieID, now, nowUtc, chatMsg.SenderID, chatMsg.ResourceID);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                    }
                    if (!optReturn.Result)
                    {
                       // LogOperation.WriteLog("Create cookie error:" + optReturn.Message);
                        return optReturn;
                    }
                }
                else
                {
                   // LogOperation.WriteLog("Cookie exists");
                    lCookieID = chatMsg.CookieID;
                }
                //写消息详细记录
               // LogOperation.WriteLog("Add chat message,database type = " + session.DatabaseInfo.TypeName);
                string strTableName = lstParams[1];
                if (strTableName == "3")
                {
                    //如果是离线消息 需要获得流水号 流水号用来在发送完离线消息后删除该消息
                    lstSerialParam.Clear();
                    lstSerialParam.Add("16");
                    lstSerialParam.Add("162");
                    optReturn = GetSerialID(session, lstSerialParam);
                    if (!optReturn.Result)
                    {
                       // LogOperation.WriteLog("Create offline serialID error : " + optReturn.Message);
                        return optReturn;
                    }
                   // LogOperation.WriteLog("create offline msgID : " + optReturn.Data.ToString());
                    long lMsgID = long.Parse(optReturn.Data.ToString());
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = "insert into t_16_00{0}_{1} values({10},{2},{3},{4},{5},'{6}',{7},'{8}','{9}')";
                            strSql = string.Format(strSql, lstParams[1], strToken, lCookieID, now, nowUtc,
                                chatMsg.SenderID, chatMsg.SenderName, chatMsg.ReceiverID, chatMsg.ReceiverName, chatMsg.MsgContent, lMsgID);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);

                            break;
                        case 3:
                            strSql = "insert into t_16_00{0}_{1} values({10},{2},{3},{4},{5},'{6}',{7},'{8}','{9}')";
                            strSql = string.Format(strSql, lstParams[1], strToken, lCookieID, now, nowUtc,
                                chatMsg.SenderID, chatMsg.SenderName, chatMsg.ReceiverID, chatMsg.ReceiverName, chatMsg.MsgContent, lMsgID);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                    }
                }
                else
                {
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = "insert into t_16_00{0}_{1} values({2},{3},{4},{5},'{6}',{7},'{8}','{9}')";
                            strSql = string.Format(strSql, lstParams[1], strToken, lCookieID, now, nowUtc,
                                chatMsg.SenderID, chatMsg.SenderName, chatMsg.ReceiverID, chatMsg.ReceiverName, chatMsg.MsgContent);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);

                            break;
                        case 3:
                            strSql = "insert into t_16_00{0}_{1} values({2},{3},{4},{5},'{6}',{7},'{8}','{9}')";
                            strSql = string.Format(strSql, lstParams[1], strToken, lCookieID, now, nowUtc,
                                chatMsg.SenderID, chatMsg.SenderName, chatMsg.ReceiverID, chatMsg.ReceiverName, chatMsg.MsgContent);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                    }
                }
               
                //LogOperation.WriteLog("Add chat message done");
                optReturn.Data = lCookieID;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Message = ex.Message;
                //LogOperation.WriteLog("WriteChatMsg exception ,message = " + ex.Message);
            }
            return optReturn;
        }

        /// <summary>
        /// 结束会话
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams">
        /// lstParams[0] : CookieID
        /// </param>
        /// <returns></returns>
        public static OperationReturn EndCookie(SessionInfo session,List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                //LogOperation.WriteLog("End cookie ");
                string strToken = session.RentInfo.Token;
                long lCookieID = long.Parse(lstParams[0]);
                long now = 0;
                CommonFuncs.DateTimeToNumber(DateTime.Now, ref now);
                long nowUtc = 0;
                CommonFuncs.DateTimeToNumber(DateTime.Now.ToUniversalTime(), ref nowUtc);
                string strSql = string.Empty;

                switch (session.DBType)
                {
                    case 2:
                        strSql = "update t_16_001_{0} set C004 = {1} ，C005 = {2} where C001 = {3}";
                        strSql = string.Format(strSql, strToken, now, nowUtc, lCookieID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "update t_16_001_{0} set C004 = {1} ，C005 = {2} where C001 = {3}";
                        strSql = string.Format(strSql, strToken, now, nowUtc, lCookieID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    //LogOperation.WriteLog("EndCookie failed : " + optReturn.Message);
                    return optReturn;
                }
                optReturn.Result = true;
            }
            catch (Exception ex)
            {
                //LogOperation.WriteLog("EndCookie exception : " + ex.Message);
                optReturn.Result = false;
            }
            return optReturn;
        }


        /// <summary>
        /// 获得流水号
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 模块编号
        /// lstParams[1] : 模块内编号
        /// <returns></returns>
        private static OperationReturn GetSerialID(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            Service11012Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = session;
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.ListData.Add(lstParams[0]);
                webRequest.ListData.Add(lstParams[1]);
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                client = new Service11012Client(WebHelper.CreateBasicHttpBinding(session),
                    WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                optReturn.Data = webReturn.Data;
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                //LogOperation.WriteLog("GetSerialID " + lstParams[1] + " failed: " + ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
            return optReturn;
        }

        /// <summary>
        /// 获得离线消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 用户ID
        /// <returns>true：有离线消息 falst：出错或没有离线消息</returns>
        public static OperationReturn GetAllOffLineMsg(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strToken = session.RentInfo.Token;
                long lUserID = long.Parse(lstParams[0]);
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = "select * from t_16_003_{0} where C006 ={1}";
                        strSql = string.Format(strSql, strToken, lstParams[0]);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                         strSql = "select * from t_16_003_{0} where C006 ={1}";
                        strSql = string.Format(strSql, strToken, lstParams[0]);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    //LogOperation.WriteLog("Error to get offline message : " + optReturn.Message);
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    return optReturn;
                }
                Dictionary<long, ChatMessage> dicOfflineMessage = new Dictionary<long, ChatMessage>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    long lMsgID = long.Parse(row["C000"].ToString());
                    ChatMessage msgobj = new ChatMessage();
                    msgobj.CookieID =long.Parse( row["C001"].ToString());
                    msgobj.MsgContent = row["C008"].ToString();
                    msgobj.ReceiverID = long.Parse(row["C006"].ToString());
                    msgobj.ReceiverName = row["C007"].ToString();
                    msgobj.SenderID = long.Parse(row["C004"].ToString());
                    msgobj.SenderName = row["C005"].ToString();
                    dicOfflineMessage.Add(lMsgID, msgobj);
                }
                optReturn.Data = dicOfflineMessage;
            }
            catch (Exception ex)
            {
                //LogOperation.WriteLog("Exception to get offline message: " + ex.Message);
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 删掉离线消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 
        /// <returns></returns>
        public static OperationReturn DelteOfflineMsg(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = "delete from t_16_003_{0} where C000= {1}";
                        strSql = string.Format(strSql, strToken, lstParams[0]);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                         strSql = "delete from t_16_003_{0} where C000= {1}";
                        strSql = string.Format(strSql, strToken, lstParams[0]);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    //LogOperation.WriteLog("Delete offline message error : " + optReturn.Message);
                    return optReturn;
                }

            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                //LogOperation.WriteLog("Delete offline message exception : " + ex.Message);
            }
            return optReturn;
        }
    }
}