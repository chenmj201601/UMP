using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common25011;
using VoiceCyber.UMP.Communications;
using Wcf25011.Wcf11012;

namespace Wcf25011
{
    public partial class Service25011
    {
        private OperationReturn SaveAlarmInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                //1     告警信息总数
                //2...     告警信息
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AlarmInfo count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AlarmInfo count invalid");
                    return optReturn;
                }
                List<AlarmInfomationInfo> listAlarmInfos = new List<AlarmInfomationInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<AlarmInfomationInfo>(listParams[i + 2]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    AlarmInfomationInfo info = optReturn.Data as AlarmInfomationInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("PropertyValue is null");
                        return optReturn;
                    }
                    listAlarmInfos.Add(info);
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT * FROM T_25_007");
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_25_007");
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not support");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    List<string> listMsg = new List<string>();
                    for (int i = 0; i < listAlarmInfos.Count; i++)
                    {
                        bool isAdd = false;
                        AlarmInfomationInfo info = listAlarmInfos[i];
                        long serialID = info.SerialID;
                        DataRow dr = objDataSet.Tables[0].Select(string.Format("C001 = {0}", serialID)).FirstOrDefault();
                        //如果不存在此行列，追加上
                        if (dr == null)
                        {
                            isAdd = true;
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = serialID;
                        }
                        dr["C002"] = info.MessageID;
                        dr["C003"] = info.Level;
                        dr["C004"] = info.IsEnabled ? "1" : "0";
                        dr["C005"] = info.Name;
                        dr["C006"] = info.Description;
                        dr["C007"] = info.CreateTime;
                        dr["C008"] = info.Creator;
                        dr["C009"] = info.LastModifyTime;
                        dr["C010"] = info.LastModifyUser;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            string strMsg = string.Format("A{0}{1}", ConstValue.SPLITER_CHAR, serialID);
                            listMsg.Add(strMsg);
                        }
                        else
                        {
                            string strMsg = string.Format("M{0}{1}", ConstValue.SPLITER_CHAR, serialID);
                            listMsg.Add(strMsg);
                        }
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                    optReturn.Data = listMsg;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn RemoveAlarmInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                //1     待删除的告警信息总数
                //2...     待删除告警信息的SerialID
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AlarmInfo count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AlarmInfo count invalid");
                    return optReturn;
                }
                List<long> listAlarmInfoIDs = new List<long>();
                for (int i = 0; i < intCount; i++)
                {
                    string strID = listParams[i + 2];
                    long id;
                    if (!long.TryParse(strID, out id))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("AlarmInfo serialID invalid");
                        return optReturn;
                    }
                    listAlarmInfoIDs.Add(id);
                }
                //将SerialID插入到临时表中
                WebRequest webRequest = new WebRequest();
                webRequest.Session = session;
                webRequest.Code = (int)RequestCode.WSInsertTempData;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(intCount.ToString());
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listAlarmInfoIDs[i].ToString();
                    webRequest.ListData.Add(strInfo);
                }
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(session),
                   WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                string strTempID = webReturn.Data;
                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql =
                            string.Format(
                                "DELETE FROM T_25_007 WHERE C001 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})",
                                strTempID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        strSql =
                            string.Format(
                                "DELETE FROM T_25_008 WHERE C002 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})",
                                strTempID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    //ORCL
                    case 3:
                        strSql =
                            string.Format(
                                "DELETE FROM T_25_007 WHERE C001 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})",
                                strTempID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        strSql =
                            string.Format(
                                "DELETE FROM T_25_008 WHERE C002 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0})",
                                strTempID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not support");
                        return optReturn;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn SaveAlarmReceiverList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                //1     告警信息编码
                //2     告警接收人总数
                //3...     告警接收人信息
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strAlarmInfoID = listParams[1];
                string strCount = listParams[2];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AlarmReceiver count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AlarmReceiver count invalid");
                    return optReturn;
                }
                List<AlarmReceiverInfo> listInfos = new List<AlarmReceiverInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<AlarmReceiverInfo>(listParams[i + 3]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    AlarmReceiverInfo info = optReturn.Data as AlarmReceiverInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("AlarmReceiverInfo is null");
                        return optReturn;
                    }
                    listInfos.Add(info);
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT * FROM T_25_008 WHERE C002 = {0}", strAlarmInfoID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_25_008 WHERE C002 = {0}", strAlarmInfoID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not support");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    List<string> listMsg = new List<string>();
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        //删除不在列表中的用户
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        long userID = Convert.ToInt64(dr["C001"]);
                        var info = listInfos.FirstOrDefault(a => a.UserID == userID);
                        if (info == null)
                        {
                            dr.Delete();
                            string strMsg = string.Format("D{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, userID,
                                strAlarmInfoID);
                            listMsg.Add(strMsg);
                        }
                    }
                    for (int i = 0; i < listInfos.Count; i++)
                    {
                        bool isAdd = false;
                        AlarmReceiverInfo info = listInfos[i];
                        long userID = info.UserID;
                        DataRow dr = objDataSet.Tables[0].Select(string.Format("C001 = {0}", userID)).FirstOrDefault();
                        //如果不存在此行列，追加上
                        if (dr == null)
                        {
                            isAdd = true;
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = userID;
                        }
                        dr["C002"] = info.AlarmInfoID;
                        dr["C003"] = info.TenantID;
                        dr["C004"] = info.TenantToken;
                        dr["C005"] = info.Method;
                        dr["C006"] = info.ReplyMode;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            string strMsg = string.Format("A{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, userID,
                                strAlarmInfoID);
                            listMsg.Add(strMsg);
                        }
                        else
                        {
                            string strMsg = string.Format("M{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, userID,
                                 strAlarmInfoID);
                            listMsg.Add(strMsg);
                        }
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                    optReturn.Data = listMsg;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }
    }
}