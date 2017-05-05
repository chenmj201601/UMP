using System;
using System.Collections.Generic;
using System.Data;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common25011;

namespace Wcf25011
{
    public partial class Service25011
    {
        private OperationReturn GetAlarmMessageList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_25_006 ORDER BY C001");
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_25_006 ORDER BY C001");
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    AlarmMessageInfo item = new AlarmMessageInfo();
                    item.SerialID = Convert.ToInt64(dr["C001"]);
                    item.AlarmType = Convert.ToInt32((dr["C002"]));
                    item.ModuleID = Convert.ToInt32(dr["C003"]);
                    item.MessageID = Convert.ToInt32(dr["C004"]);
                    item.StatusID = Convert.ToInt32(dr["C005"]);
                    item.IsEnabled = dr["C006"].ToString() == "1";
                    item.Description = dr["C007"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetAlarmInfomationList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_25_007 ORDER BY C001");
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_25_007 ORDER BY C001");
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    AlarmInfomationInfo item = new AlarmInfomationInfo();
                    item.SerialID = Convert.ToInt64(dr["C001"]);
                    item.MessageID = Convert.ToInt64(dr["C002"]);
                    item.Level = Convert.ToInt32(dr["C003"]);
                    item.IsEnabled = dr["C004"].ToString() == "1";
                    item.Name = dr["C005"].ToString();
                    item.Description = dr["C006"].ToString();
                    item.CreateTime = string.IsNullOrEmpty(dr["C007"].ToString())
                        ? DateTime.MinValue
                        : Convert.ToDateTime(dr["C007"]);
                    item.Creator = string.IsNullOrEmpty(dr["C008"].ToString())
                        ? 0
                        : Convert.ToInt64(dr["C008"]);
                    item.LastModifyTime = string.IsNullOrEmpty(dr["C009"].ToString())
                        ? DateTime.MinValue
                        : Convert.ToDateTime(dr["C009"]);
                    item.LastModifyUser = string.IsNullOrEmpty(dr["C010"].ToString())
                        ? 0
                        : Convert.ToInt64(dr["C010"]);
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetAlarmReveiverList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编号
                //1     AlarmInfo SerialID（如果为0表示获取所有接收人信息）
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strAlarmInfoID = listParams[1];
                long alarmInfoID;
                if (!long.TryParse(strAlarmInfoID, out alarmInfoID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AlarmInfoID invalid");
                    return optReturn;
                }
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (alarmInfoID == 0)
                        {
                            strSql = string.Format("SELECT * FROM T_25_008 ORDER BY C001,C002");
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_25_008 WHERE C002 = {0} ORDER BY C001,C002",
                                alarmInfoID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (alarmInfoID == 0)
                        {
                            strSql = string.Format("SELECT * FROM T_25_008 ORDER BY C001,C002");
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_25_008 WHERE C002 = {0} ORDER BY C001,C002",
                                alarmInfoID);
                        }
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    AlarmReceiverInfo item = new AlarmReceiverInfo();
                    item.UserID = Convert.ToInt64(dr["C001"]);
                    item.AlarmInfoID = Convert.ToInt64(dr["C002"]);
                    item.TenantID = Convert.ToInt64(dr["C003"]);
                    item.TenantToken = dr["C004"].ToString();
                    item.Method = string.IsNullOrEmpty(dr["C005"].ToString()) ? 0 : Convert.ToInt32(dr["C005"]);
                    item.ReplyMode = string.IsNullOrEmpty(dr["C006"].ToString()) ? 0 : Convert.ToInt32(dr["C006"]);
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }
    }
}