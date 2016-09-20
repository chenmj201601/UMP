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
using VoiceCyber.UMP.Communications;
using Common6106;
using System.Data;

namespace Wcf61061
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service61061 : IService61061
    {
        public WebReturn DoOperation(WebRequest webRequest)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;

            if (webRequest == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = string.Format("WebRequest is null");
                return webReturn;
            }
            SessionInfo session = webRequest.Session;
            if (session == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = string.Format("SessionInfo is null");
                return webReturn;
            }
            webReturn.Session = session;
            try
            {
                OperationReturn optReturn;
                switch (webRequest.Code)
                {
                    case (int)S6106RequestCode.GetRecordCount:
                        optReturn = GetRecordCount(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S6106RequestCode.GetUMPUsedCount:
                        optReturn = GetUMPUsedCount(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            webReturn.Data = optReturn.StringValue;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S6106RequestCode.GetRecordLength:
                        optReturn = GetRecordLength(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            webReturn.Data = optReturn.StringValue;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S6106RequestCode.GetRecordMode:
                        optReturn = GetRecordMode(session, webRequest.ListData);
                        webReturn.Message = optReturn.Message;
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Data = optReturn.StringValue;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        webReturn.ListData.Add(optReturn.StringValue);
                        break;
                    case (int)S6106RequestCode.GetOrgInfo:
                        optReturn = GetOrgInfo(session, webRequest.ListData);
                        webReturn.Message = optReturn.Message;
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Data = optReturn.StringValue;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S6106RequestCode.GetQutityCount:
                        optReturn = GetQutityCount(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            webReturn.Data = optReturn.StringValue;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S6106RequestCode.GetAppealCount:
                        optReturn = GetAppealCount(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            webReturn.Data = optReturn.StringValue;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S6106RequestCode.GetWarningCount:
                        optReturn = GetWarningCount(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            webReturn.Data = optReturn.StringValue;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S6106RequestCode.GetReplayCount:
                         optReturn = GetReplayCount(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            webReturn.Data = optReturn.StringValue;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S6106RequestCode.GetAvgScore:
                        optReturn = GetAvgScore(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            webReturn.Data = optReturn.StringValue;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                }
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
            return webReturn;
        }

        /// <summary>
        /// 通话量统计
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// listParams[0] : 统计时长
        /// listParams[1] : 当前时间   用于做测试时传入 正式发布时不使用该值 为空
        /// <returns></returns>
        private OperationReturn GetRecordCount(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                if (lstParams == null || lstParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                //计算开始时间
                DateTime dtCurrent;
                if (string.IsNullOrEmpty(lstParams[1]))
                {
                    //正式发布时 此值为空 当前时间是从app server获取
                    dtCurrent = DateTime.Now;
                }
                else
                {
                    dtCurrent = DateTime.Parse(lstParams[1]);
                }

                int iDay = int.Parse(lstParams[0]);
                DateTime dtStartTime = dtCurrent.AddDays(-iDay);
                long lstartTime = 0;
                long lEndTime = 0;
                bool bo = CommonFunctions.DateTimeToNumber(dtStartTime, ref lstartTime);
                bo = CommonFunctions.DateTimeToNumber(dtCurrent, ref lEndTime);
                List<string> lst = new List<string>();
                lst.Add(lstartTime.ToString());
                lst.Add(lEndTime.ToString());
                switch (session.DBType)
                {
                    case 2:
                        optReturn = RecordCountFuncs.CreateRecordCountSqlInMSSql(session, lst);
                        break;
                    case 3:
                        optReturn = RecordCountFuncs.CreateRecordCountSqlInOracle(session, lst);
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.StringValue;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:

                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                if (optReturn.Data == null)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    return optReturn;
                }
                RecordCountEntry RecEntry = null;
                List<string> lstRecords = new List<string>();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    RecEntry = new RecordCountEntry();
                    RecEntry.RecCount = row["RecCount"].ToString();
                    RecEntry.RecDate = row["RecDate"].ToString();
                    RecEntry.RecDirection = row["RecDir"].ToString();
                    optReturn = XMLHelper.SeriallizeObject<RecordCountEntry>(RecEntry);
                    if (optReturn.Result)
                    {
                        lstRecords.Add(optReturn.Data.ToString());
                    }
                }
                optReturn.Result = true;
                optReturn.Data = lstRecords;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.StringValue = strSql;
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

        /// <summary>
        /// UMP使用情况统计
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 统计时长
        /// lstParams[1] : 当前时间
        /// <returns></returns>
        private OperationReturn GetUMPUsedCount(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                if (lstParams == null || lstParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                //计算开始时间
                DateTime dtCurrent;
                if (string.IsNullOrEmpty(lstParams[1]))
                {
                    //正式发布时 此值为空 当前时间是从app server获取
                    dtCurrent = DateTime.Now;
                }
                else
                {
                    dtCurrent = DateTime.Parse(lstParams[1]);
                }

                int iDay = int.Parse(lstParams[0]);
                DateTime dtStartTime = dtCurrent.AddDays(-iDay);
                long lstartTime = 0;
                long lEndTime = 0;
                bool bo = CommonFunctions.DateTimeToNumber(dtStartTime, ref lstartTime);
                bo = CommonFunctions.DateTimeToNumber(dtCurrent, ref lEndTime);
                List<string> lst = new List<string>();
                lst.Add(lstartTime.ToString());
                lst.Add(lEndTime.ToString());
                //获得sql语句
                optReturn = UMPUsedCountFuncs.CreateUMPUsedCountSql(session, lst);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.StringValue;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                if (optReturn.Data == null)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    return optReturn;
                }
                List<string> lstRecords = new List<string>();
                object obj;
                long lTempStartTime = lstartTime;
                long lTempEndTime = 0;
                string strCondition = string.Empty;
                bo = CommonFunctions.DateTimeToNumber(dtStartTime.AddDays(1).Date, ref lTempEndTime);
                int iCount = 0;
                KeyValueEntry keyValueEntry = null;
                for (int i = 1; i <= iDay; i++)
                {
                    //如果是最后一天 那么统计结束时间为当天的此时此刻 而不是 00：00：00
                    if (i == iDay)
                    {
                        lTempEndTime = lEndTime;
                    }
                    strCondition = string.Format("C008>{0} and C008 <= {1}", lTempStartTime, lTempEndTime);
                    obj = ds.Tables[0].Compute("count(C001)", strCondition);
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out iCount);
                    }
                    if (iCount != 0)
                    {
                        DateTime dtLocalTime = CommonFunctions.StringToDateTime(lTempStartTime.ToString());
                        string strKey = dtLocalTime.Month + "/" + dtLocalTime.Day;
                        keyValueEntry = new KeyValueEntry();
                        keyValueEntry.StrKey = strKey;
                        keyValueEntry.DataValue = iCount;
                        optReturn = XMLHelper.SeriallizeObject<KeyValueEntry>(keyValueEntry);
                        if (optReturn.Result)
                        {
                            lstRecords.Add(optReturn.Data.ToString());
                        }
                    }

                    lTempStartTime = lTempEndTime;  //下次循环统计开始时间 是本次循环结束时间
                    bo = CommonFunctions.DateTimeToNumber(dtStartTime.AddDays(i + 1).Date, ref lTempEndTime); //下次循环结束时间 是本次循环结束时间加一天 
                }

                List<KeyValueEntry> lstk = new List<KeyValueEntry>();
                for (int i = 0; i < lstRecords.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KeyValueEntry>(lstRecords[i]);
                    if (optReturn.Result)
                    {
                        lstk.Add(optReturn.Data as KeyValueEntry);
                    }
                }

                optReturn.Result = true;
                optReturn.Data = lstRecords;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.StringValue = strSql;
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

        /// <summary>
        /// 统计录音时长
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 统计时长
        /// lstParams[1] : 当前时间
        /// lstParams[2] : 统计分机或坐席 A或E
        /// lstParams[3] : 部门编号
        /// <returns></returns>
        private OperationReturn GetRecordLength(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                if (lstParams == null || lstParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                //获得用户可以管理的分机或坐席
                optReturn = PermissionFuncs.GetCtrlAgentOrExtensionNames(session, lstParams[2], lstParams[3]);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strTempID = optReturn.Data as string;

                string rentToken = session.RentInfo.Token;
                //计算开始时间
                DateTime dtCurrent;
                if (string.IsNullOrEmpty(lstParams[1]))
                {
                    //正式发布时 此值为空 当前时间是从app server获取
                    dtCurrent = DateTime.Now;
                }
                else
                {
                    dtCurrent = DateTime.Parse(lstParams[1]);
                }

                int iDay = int.Parse(lstParams[0]);
                DateTime dtStartTime = dtCurrent.AddDays(-iDay);
                long lstartTime = 0;
                long lEndTime = 0;
                bool bo = CommonFunctions.DateTimeToNumber(dtStartTime, ref lstartTime);
                bo = CommonFunctions.DateTimeToNumber(dtCurrent, ref lEndTime);
                List<string> lst = new List<string>();
                lst.Add(lstartTime.ToString());
                lst.Add(lEndTime.ToString());
                lst.Add(lstParams[2]);
                lst.Add(strTempID);
                //获得sql语句
                string strSql = string.Empty;
                optReturn = RecordLengthFuncs.CreateSql(session, lst);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                strSql = optReturn.Data as string;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    optReturn.StringValue = strSql;
                    return optReturn;
                }

                if (optReturn.Data == null)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<string> lstRecords = new List<string>();
                object obj;
                long lTempStartTime = lstartTime;
                long lTempEndTime = 0;
                string strCondition = string.Empty;
                bo = CommonFunctions.DateTimeToNumber(dtStartTime.AddDays(1).Date, ref lTempEndTime);
                int iCount = 0;
                KeyValueEntry keyValueEntry = null;
                for (int i = 1; i <= iDay; i++)
                {
                    //如果是最后一天 那么统计结束时间为当天的此时此刻 而不是 00：00：00
                    if (i == iDay)
                    {
                        lTempEndTime = lEndTime;
                    }
                    strCondition = string.Format("C006>{0} and C006 <= {1}", lTempStartTime, lTempEndTime);
                    obj = ds.Tables[0].Compute("sum(C012)", strCondition);
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out iCount);
                    }
                    if (iCount != 0)
                    {
                        DateTime dtLocalTime = CommonFunctions.StringToDateTime(lTempStartTime.ToString());
                        string strKey = dtLocalTime.Month + "/" + dtLocalTime.Day;
                        keyValueEntry = new KeyValueEntry();
                        keyValueEntry.StrKey = strKey;
                        keyValueEntry.DataValue = iCount;
                        int iHour = iCount / 3600;
                        iCount = iCount % 3600;
                        int iMin = iCount / 60;
                        iCount = iCount % 60;
                        keyValueEntry.StrOthre1 = iHour + ":" + iMin + ":" + iCount;
                        optReturn = XMLHelper.SeriallizeObject<KeyValueEntry>(keyValueEntry);
                        if (optReturn.Result)
                        {
                            lstRecords.Add(optReturn.Data.ToString());
                        }
                    }

                    lTempStartTime = lTempEndTime;  //下次循环统计开始时间 是本次循环结束时间
                    bo = CommonFunctions.DateTimeToNumber(dtStartTime.AddDays(i + 1).Date, ref lTempEndTime); //下次循环结束时间 是本次循环结束时间加一天 
                }

                List<KeyValueEntry> lstk = new List<KeyValueEntry>();
                for (int i = 0; i < lstRecords.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KeyValueEntry>(lstRecords[i]);
                    if (optReturn.Result)
                    {
                        lstk.Add(optReturn.Data as KeyValueEntry);
                    }
                }

                CommonFuns.DeleteTempDataByID(session, strTempID);

                optReturn.Result = true;
                optReturn.Data = lstRecords;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.StringValue = strSql;
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

        /// <summary>
        /// 获得录音模式 分机、坐席、混合模式
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private OperationReturn GetRecordMode(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();
            string strSql = string.Empty;
            try
            {
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT C006 FROM T_11_001_{0} where C003 = 12010401", session.RentInfo.Token);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT C006 FROM T_11_001_{0} where C003 = 12010401", session.RentInfo.Token);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += " ; " + "strsql = " + strSql;

                if (!optReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S6106WcfErrorCode.GetRecordModeFailed;
                    return optReturn;
                }
                if (optReturn.Data == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S6106WcfErrorCode.RecordModeNull;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S6106WcfErrorCode.RecordModeNull;
                    return optReturn;
                }
                string str = ds.Tables[0].Rows[0][0].ToString();
                str = S6106EncryptOperation.DecryptWithM002(str);
                if (str.Contains("12010401"))
                {
                    str = str.Substring(8);
                }
                else
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S6106WcfErrorCode.RecordModeValueError;
                    return optReturn;
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Message = strSql;
                optReturn.Data = str;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Message = "strsql = " + strSql + " ; " + ex.Message;
                optReturn.Code = (int)S6106WcfErrorCode.GetRecordModeFailed;
                return optReturn;
            }
        }

        /// <summary>
        /// 获得用户所在的机构信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParam">空</param>
        /// <returns></returns>
        private OperationReturn GetOrgInfo(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Code = Defines.RET_SUCCESS;
            optReturn.Result = true;
            string strSql = string.Empty;
            try
            {
                string strUserID = session.UserID.ToString();
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("select C006 from T_11_005_{0} where C001 = {1}", rentToken, strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("select C006 from T_11_005_{0} where C001 = {1}", rentToken, strUserID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += " ; Sql = " + strSql;
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                if (optReturn.Data == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    return optReturn;
                }
                string strOrgID = ds.Tables[0].Rows[0][0].ToString();
                optReturn.Data = strOrgID;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 质检数量统计
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParam"></param>
        /// lstParams[0] : 统计时长
        /// lstParams[1] : 当前时间
        /// <returns></returns>
        private OperationReturn GetQutityCount(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                optReturn = PermissionFuncs.GetCotrlUser(session);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strTempID = optReturn.Data as string;

                int iDay = int.Parse(lstParam[0]);
                string rentToken = session.RentInfo.Token;
                //计算开始时间
                DateTime dtCurrent;
                if (string.IsNullOrEmpty(lstParam[1]))
                {
                    //正式发布时 此值为空 当前时间是从app server获取
                    dtCurrent = DateTime.Now.ToUniversalTime();
                }
                else
                {
                    dtCurrent = DateTime.Parse(lstParam[1]);
                }
                DateTime dtStartTime = dtCurrent.AddDays(-iDay).ToUniversalTime();
                string strStartTime = dtStartTime.ToString("yyyy-MM-dd HH:mm:ss");
                List<string> lst = new List<string>();
                lst.Add(strStartTime);
                lst.Add(dtCurrent.ToString("yyyy-MM-dd HH:mm:ss"));
                lst.Add(strTempID);
                optReturn = QutityCountFuncs.CreateSql(session, lst);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                if (optReturn.Data == null)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<string> lstRecords = new List<string>();
                object obj;
                DateTime dtTempStartTime = dtStartTime;
                DateTime dtTempEndTime = dtTempStartTime.AddDays(1).Date;
                string strCondition = string.Empty;

                int iCount = 0;
                KeyValueEntry keyValueEntry = null;
                for (int i = 1; i <= iDay; i++)
                {
                    //如果是最后一天 那么统计结束时间为当天的此时此刻 而不是 00：00：00
                    if (i == iDay)
                    {
                        dtTempEndTime = dtCurrent;
                    }
                    string st1 = dtTempStartTime.ToString("yyyy-MM-dd HH:mm:ss");
                    string st2 = dtTempEndTime.ToString("yyyy-MM-dd HH:mm:ss");
                    strCondition = string.Format("C006>='{0}' and C006 <= '{1}'", st1, st2);
                    obj = ds.Tables[0].Compute("count(C012)", strCondition);
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out iCount);
                    }
                    if (iCount != 0)
                    {
                        DateTime dtLocalTime = dtTempStartTime;
                        string strKey = dtLocalTime.Month + "/" + dtLocalTime.Day;
                        keyValueEntry = new KeyValueEntry();
                        keyValueEntry.StrKey = strKey;
                        keyValueEntry.DataValue = iCount;
                        optReturn = XMLHelper.SeriallizeObject<KeyValueEntry>(keyValueEntry);
                        if (optReturn.Result)
                        {
                            lstRecords.Add(optReturn.Data.ToString());
                        }
                    }

                    dtTempStartTime = dtTempEndTime;  //下次循环统计开始时间 是本次循环结束时间
                    //下次循环结束时间 是本次循环结束时间加一天 
                    dtTempEndTime = dtTempEndTime.AddDays(1);
                }

                CommonFuns.DeleteTempDataByID(session, strTempID);

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = lstRecords;
                optReturn.StringValue = strSql;
            }
            catch (Exception ex)
            {
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Result = false;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 统计最近几天的申诉数量
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParam"></param>
        /// lstParams[0] : 统计时长
        /// lstParams[1] : 当前时间
        /// <returns></returns>
        private OperationReturn GetAppealCount(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                optReturn = PermissionFuncs.GetCotrlUser(session);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strTempID = optReturn.Data as string;

                int iDay = int.Parse(lstParam[0]);
                string rentToken = session.RentInfo.Token;
                //计算开始时间
                DateTime dtCurrent;
                if (string.IsNullOrEmpty(lstParam[1]))
                {
                    //正式发布时 此值为空 当前时间是从app server获取
                    dtCurrent = DateTime.Now.ToUniversalTime();
                }
                else
                {
                    dtCurrent = DateTime.Parse(lstParam[1]);
                }
                DateTime dtStartTime = dtCurrent.AddDays(-iDay).ToUniversalTime();
                string strStartTime = dtStartTime.ToString("yyyy-MM-dd HH:mm:ss");
                List<string> lst = new List<string>();
                lst.Add(strStartTime);
                lst.Add(dtCurrent.ToString("yyyy-MM-dd HH:mm:ss"));
                lst.Add(strTempID);
                optReturn = AppealCountFuncs.CreateSql(session, lst);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                if (optReturn.Data == null)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<string> lstRecords = new List<string>();
                object obj;
                DateTime dtTempStartTime = dtStartTime;
                DateTime dtTempEndTime = dtTempStartTime.AddDays(1).Date;
                string strCondition = string.Empty;

                int iCount = 0;
                KeyValueEntry keyValueEntry = null;
                for (int i = 1; i <= iDay; i++)
                {
                    //如果是最后一天 那么统计结束时间为当天的此时此刻 而不是 00：00：00
                    if (i == iDay)
                    {
                        dtTempEndTime = dtCurrent;
                    }
                    string st1 = dtTempStartTime.ToString("yyyy-MM-dd HH:mm:ss");
                    string st2 = dtTempEndTime.ToString("yyyy-MM-dd HH:mm:ss");
                    strCondition = string.Format("C006>='{0}' and C006 <= '{1}'", st1, st2);
                    obj = ds.Tables[0].Compute("count(C012)", strCondition);
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out iCount);
                    }
                    if (iCount != 0)
                    {
                        DateTime dtLocalTime = dtTempStartTime;
                        string strKey = dtLocalTime.Month + "/" + dtLocalTime.Day;
                        keyValueEntry = new KeyValueEntry();
                        keyValueEntry.StrKey = strKey;
                        keyValueEntry.DataValue = iCount;
                        optReturn = XMLHelper.SeriallizeObject<KeyValueEntry>(keyValueEntry);
                        if (optReturn.Result)
                        {
                            lstRecords.Add(optReturn.Data.ToString());
                        }
                    }

                    dtTempStartTime = dtTempEndTime;  //下次循环统计开始时间 是本次循环结束时间
                    //下次循环结束时间 是本次循环结束时间加一天 
                    dtTempEndTime = dtTempEndTime.AddDays(1);
                }

                CommonFuns.DeleteTempDataByID(session, strTempID);

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = lstRecords;
                optReturn.StringValue = strSql;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 统计告警数量
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParam"></param>
        /// lstParams[0] : 统计时长
        /// lstParams[1] : 当前时间
        /// <returns></returns>
        private OperationReturn GetWarningCount(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                int iDay = int.Parse(lstParam[0]);
                string rentToken = session.RentInfo.Token;
                //计算开始时间
                DateTime dtCurrent;
                if (string.IsNullOrEmpty(lstParam[1]))
                {
                    //正式发布时 此值为空 当前时间是从app server获取
                    dtCurrent = DateTime.Now.ToUniversalTime();
                }
                else
                {
                    dtCurrent = DateTime.Parse(lstParam[1]);
                }
                DateTime dtStartTime = dtCurrent.AddDays(-iDay).ToUniversalTime();
                string strStartTime = dtStartTime.ToString("yyyy-MM-dd HH:mm:ss");
                List<string> lst = new List<string>();
                lst.Add(strStartTime);
                lst.Add(dtCurrent.ToString("yyyy-MM-dd HH:mm:ss"));
                optReturn = WarningCountFuncs.CreateSql(session, lst);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                if (optReturn.Data == null)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<string> lstRecords = new List<string>();
                object obj;
                DateTime dtTempStartTime = dtStartTime;
                DateTime dtTempEndTime = dtTempStartTime.AddDays(1).Date;
                string strCondition = string.Empty;

                int iCount = 0;
                KeyValueEntry keyValueEntry = null;
                for (int i = 1; i <= iDay; i++)
                {
                    //如果是最后一天 那么统计结束时间为当天的此时此刻 而不是 00：00：00
                    if (i == iDay)
                    {
                        dtTempEndTime = dtCurrent;
                    }
                    string st1 = dtTempStartTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    string st2 = dtTempEndTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    strCondition = string.Format("C018>='{0}' and C018 <= '{1}'", st1, st2);
                    obj = ds.Tables[0].Compute("count(C001)", strCondition);
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out iCount);
                    }
                    if (iCount != 0)
                    {
                        DateTime dtLocalTime = dtTempStartTime.ToLocalTime();
                        string strKey = dtLocalTime.Month + "/" + dtLocalTime.Day;
                        keyValueEntry = new KeyValueEntry();
                        keyValueEntry.StrKey = strKey;
                        keyValueEntry.DataValue = iCount;
                        optReturn = XMLHelper.SeriallizeObject<KeyValueEntry>(keyValueEntry);
                        if (optReturn.Result)
                        {
                            lstRecords.Add(optReturn.Data.ToString());
                        }
                    }

                    dtTempStartTime = dtTempEndTime;  //下次循环统计开始时间 是本次循环结束时间
                    //下次循环结束时间 是本次循环结束时间加一天 
                    dtTempEndTime = dtTempEndTime.AddDays(1);
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = lstRecords;
                optReturn.StringValue = strSql;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 回放数量统计
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 统计时长
        /// lstParams[1] : 当前时间
        /// lstParams[2] : 统计分机或坐席 A或E
        /// lstParams[3] : 部门编号
        /// <returns></returns>
        private OperationReturn GetReplayCount(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                if (lstParams == null || lstParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                //获得用户可以管理的分机或坐席 并写入临时表中
                optReturn = PermissionFuncs.GetCtrlAgentOrExtensionNames(session, lstParams[2], lstParams[3]);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strTempID = optReturn.Data as string;

                string rentToken = session.RentInfo.Token;
                //计算开始时间
                DateTime dtCurrent;
                if (string.IsNullOrEmpty(lstParams[1]))
                {
                    //正式发布时 此值为空 当前时间是从app server获取
                    dtCurrent = DateTime.Now;
                }
                else
                {
                    dtCurrent = DateTime.Parse(lstParams[1]);
                }

                int iDay = int.Parse(lstParams[0]);
                DateTime dtStartTime = dtCurrent.AddDays(-iDay);
                long lstartTime = 0;
                long lEndTime = 0;
                bool bo = CommonFunctions.DateTimeToNumber(dtStartTime, ref lstartTime);
                bo = CommonFunctions.DateTimeToNumber(dtCurrent, ref lEndTime);
                List<string> lst = new List<string>();
                lst.Add(lstartTime.ToString());
                lst.Add(lEndTime.ToString());
                lst.Add(lstParams[2]);
                lst.Add(strTempID);
                //获得sql语句
                string strSql = string.Empty;
                optReturn = RecordLengthFuncs.CreateSql(session, lst);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                strSql = optReturn.Data as string;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    optReturn.StringValue = strSql;
                    return optReturn;
                }

                if (optReturn.Data == null)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<string> lstRecords = new List<string>();
                object obj;
                DateTime dtTempStartTime = dtStartTime;
                DateTime dtTempEndTime = dtTempStartTime.AddDays(1).Date;
                string strCondition = string.Empty;
                int iCount = 0;
                KeyValueEntry keyValueEntry = null;
                for (int i = 1; i <= iDay; i++)
                {
                    //如果是最后一天 那么统计结束时间为当天的此时此刻 而不是 00：00：00
                    if (i == iDay)
                    {
                        dtTempEndTime = dtCurrent;
                    }
                    string st1 = dtTempStartTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    string st2 = dtTempEndTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    strCondition = string.Format("C004>'{0}' and C004 <= '{1}'", st1, st2);
                    obj = ds.Tables[0].Compute("sum(C068)", strCondition);
                    if (obj != null)
                    {
                        int.TryParse(obj.ToString(), out iCount);
                    }
                    if (iCount != 0)
                    {
                        DateTime dtLocalTime = dtTempStartTime.ToLocalTime();
                        string strKey = dtLocalTime.Month + "/" + dtLocalTime.Day;
                        keyValueEntry = new KeyValueEntry();
                        keyValueEntry.StrKey = strKey;
                        keyValueEntry.DataValue = iCount;
                        int iHour = iCount / 3600;
                        iCount = iCount % 3600;
                        int iMin = iCount / 60;
                        iCount = iCount % 60;
                        keyValueEntry.StrOthre1 = iHour + ":" + iMin + ":" + iCount;
                        optReturn = XMLHelper.SeriallizeObject<KeyValueEntry>(keyValueEntry);
                        if (optReturn.Result)
                        {
                            lstRecords.Add(optReturn.Data.ToString());
                        }
                    }

                    dtTempStartTime = dtTempEndTime;  //下次循环统计开始时间 是本次循环结束时间
                    dtTempEndTime = dtTempEndTime.AddDays(1);//下次循环结束时间 是本次循环结束时间加一天 
                }

                List<KeyValueEntry> lstk = new List<KeyValueEntry>();
                for (int i = 0; i < lstRecords.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KeyValueEntry>(lstRecords[i]);
                    if (optReturn.Result)
                    {
                        lstk.Add(optReturn.Data as KeyValueEntry);
                    }
                }

                CommonFuns.DeleteTempDataByID(session, strTempID);

                optReturn.Result = true;
                optReturn.Data = lstRecords;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.StringValue = strSql;
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

        private OperationReturn GetAvgScore(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                optReturn = PermissionFuncs.GetCotrlUser(session);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strTempID = optReturn.Data as string;

                int iDay = int.Parse(lstParam[0]);
                string rentToken = session.RentInfo.Token;
                //计算开始时间
                DateTime dtCurrent;
                if (string.IsNullOrEmpty(lstParam[1]))
                {
                    //正式发布时 此值为空 当前时间是从app server获取
                    dtCurrent = DateTime.Now.ToUniversalTime();
                }
                else
                {
                    dtCurrent = DateTime.Parse(lstParam[1]);
                }
                DateTime dtStartTime = dtCurrent.AddDays(-iDay).ToUniversalTime();
                string strStartTime = dtStartTime.ToString("yyyy-MM-dd HH:mm:ss");
                List<string> lst = new List<string>();
                lst.Add(strStartTime);
                lst.Add(dtCurrent.ToString("yyyy-MM-dd HH:mm:ss"));
                lst.Add(strTempID);
                optReturn = QutityCountFuncs.CreateSql(session, lst);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                if (optReturn.Data == null)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<string> lstRecords = new List<string>();
                object obj;
                DateTime dtTempStartTime = dtStartTime;
                DateTime dtTempEndTime = dtTempStartTime.AddDays(1).Date;
                string strCondition = string.Empty;

                float iScore = 0;
                KeyValueEntry keyValueEntry = null;
                for (int i = 1; i <= iDay; i++)
                {
                    //如果是最后一天 那么统计结束时间为当天的此时此刻 而不是 00：00：00
                    if (i == iDay)
                    {
                        dtTempEndTime = dtCurrent;
                    }
                    string st1 = dtTempStartTime.ToString("yyyy-MM-dd HH:mm:ss");
                    string st2 = dtTempEndTime.ToString("yyyy-MM-dd HH:mm:ss");
                    strCondition = string.Format("C006>='{0}' and C006 <= '{1}'", st1, st2);
                    obj = ds.Tables[0].Compute("avg(C004)", strCondition);
                    string strObj = obj.ToString();
                    if (!string.IsNullOrEmpty(strObj))
                    {
                        string strScore = strObj.Substring(0, strObj.IndexOf('.') + 3);
                        float.TryParse(strScore, out iScore);
                    }
                    if (iScore != 0)
                    {
                        DateTime dtLocalTime = dtTempStartTime.ToLocalTime();
                        string strKey = dtLocalTime.Month + "/" + dtLocalTime.Day;
                        keyValueEntry = new KeyValueEntry();
                        keyValueEntry.StrKey = strKey;
                        keyValueEntry.DataValue = iScore;
                        optReturn = XMLHelper.SeriallizeObject<KeyValueEntry>(keyValueEntry);
                        if (optReturn.Result)
                        {
                            lstRecords.Add(optReturn.Data.ToString());
                        }
                    }

                    dtTempStartTime = dtTempEndTime;  //下次循环统计开始时间 是本次循环结束时间
                    //下次循环结束时间 是本次循环结束时间加一天 
                    dtTempEndTime = dtTempEndTime.AddDays(1);
                }

                CommonFuns.DeleteTempDataByID(session, strTempID);

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = lstRecords;
                optReturn.StringValue = strSql;
            }
            catch (Exception ex)
            {
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Result = false;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
    }
}
