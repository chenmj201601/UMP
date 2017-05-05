using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.ScoreSheets;
using Wcf31021.Wcf11012;

namespace Wcf31021
{
    public partial class Service31021
    {
        private OperationReturn SaveConditions(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     查询条件信息，如过ID为0，则新增一个查询条件
                //1     条件详情总数
                //2...  条件详情
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strQueryCondition = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (string.IsNullOrEmpty(strQueryCondition) || !int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param invalid");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<QueryCondition>(strQueryCondition);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                QueryCondition queryCondition = optReturn.Data as QueryCondition;
                if (queryCondition == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("QueryCondition is null");
                    return optReturn;
                }
                if (queryCondition.ID <= 0)
                {
                    //获取新增查询条件ID
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("31");
                    webRequest.ListData.Add("302");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
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
                    queryCondition.ID = Convert.ToInt64(webReturn.Data);
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                bool bIsAdded = false;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_028_{0} WHERE C001 = {1}"
                           , rentToken
                           , queryCondition.ID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_028_{0} WHERE C001 = {1}"
                            , rentToken
                            , queryCondition.ID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                    DataRow dr;
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = queryCondition.ID;
                        bIsAdded = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C002"] = queryCondition.UserID;
                    dr["C003"] = queryCondition.Creator;
                    dr["C004"] = queryCondition.CreateTime;
                    dr["C005"] = queryCondition.CreateType;
                    dr["C006"] = queryCondition.SortID;
                    dr["C007"] = queryCondition.LastQueryTime;
                    dr["C008"] = queryCondition.Name;
                    dr["C009"] = queryCondition.Description;
                    dr["C010"] = queryCondition.IsEnable ? "Y" : "N";
                    if (bIsAdded)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                    bIsAdded = true;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                    bIsAdded = false;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                if (!bIsAdded)
                {
                    return optReturn;
                }
                optReturn.Data = queryCondition.ID;

                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("QueryConditionDetail count invalid");
                    return optReturn;
                }
                List<string> listDetails = new List<string>();
                for (int i = 0; i < intCount; i++)
                {
                    listDetails.Add(listParams[i + 2]);
                }
                OperationReturn saveDetailReturn = SaveConditionDetails(session, queryCondition, listDetails);
                if (!saveDetailReturn.Result)
                {
                    return saveDetailReturn;
                }
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

        private OperationReturn SaveQueryResult(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     查询条件ID
                //1     记录数
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strQueryID = listParams[0];
                string strRecordCount = listParams[1];
                long queryID;
                int recordCount;
                if (!long.TryParse(strQueryID, out queryID)
                    || !int.TryParse(strRecordCount, out recordCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("QueryID or RecordCount invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                int dbType = session.DBType;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_028_{0} WHERE C001 = {1}"
                          , rentToken
                          , queryID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_028_{0} WHERE C001 = {1}"
                           , rentToken
                           , queryID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                int intUseCount = 0;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_DBACCESS_NOT_EXIST;
                        optReturn.Message = string.Format("QueryCondition record not exist");
                    }
                    else
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[0];
                        dr["C007"] = DateTime.Now.ToUniversalTime();
                        string strValue = dr["C103"].ToString();
                        int intValue;
                        if (int.TryParse(strValue, out intValue))
                        {
                            intValue++;
                            dr["C103"] = intValue;
                            intUseCount = intValue;
                        }
                        else
                        {
                            dr["C103"] = "1";
                            intUseCount = 1;
                        }
                        dr["C104"] = recordCount;
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

                optReturn.Data = intUseCount;
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

        private OperationReturn SaveConditionDetails(SessionInfo session, QueryCondition queryCondition, List<string> listStrDetails)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                List<QueryConditionDetail> listDetails = new List<QueryConditionDetail>();
                for (int i = 0; i < listStrDetails.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<QueryConditionDetail>(listStrDetails[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    QueryConditionDetail detail = optReturn.Data as QueryConditionDetail;
                    if (detail == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("QueryConditionDetail is null");
                        return optReturn;
                    }
                    listDetails.Add(detail);
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
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C001 = {1}"
                            , rentToken
                            , queryCondition.ID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C001 = {1}"
                            , rentToken
                            , queryCondition.ID);
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
                    //List<QueryConditionSubItem> listSubItems = new List<QueryConditionSubItem>();
                    for (int i = 0; i < listDetails.Count; i++)
                    {
                        QueryConditionDetail detail = listDetails[i];
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C002 = {0}", detail.ConditionItemID));
                        DataRow dr;
                        bool isAdd = false;
                        if (drs.Length <= 0)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = queryCondition.ID;
                            dr["C002"] = detail.ConditionItemID;
                            isAdd = true;
                        }
                        else
                        {
                            dr = drs[0];
                        }
                        dr["C003"] = detail.IsEnable ? "Y" : "N";
                        dr["C004"] = detail.Value01;
                        dr["C005"] = detail.Value02;
                        dr["C006"] = detail.Value03;
                        dr["C007"] = detail.Value04;
                        dr["C008"] = detail.Value05;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
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
            }
            return optReturn;
        }

        private OperationReturn SaveConditionSubItems(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     总数
                //1...  子项信息
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strCount = listParams[0];
                int intCount;
                if (!int.TryParse(strCount, out intCount) || intCount <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("SubItems count invalid");
                    return optReturn;
                }
                if (listParams.Count < 1 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("SubItems count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        for (int i = 0; i < intCount; i++)
                        {
                            string strSubItemInfo = listParams[i + 1];
                            string[] arrSubItemInfo = strSubItemInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.RemoveEmptyEntries);
                            if (arrSubItemInfo.Length < 3) { continue; }
                            string strQueryConditionID = arrSubItemInfo[0];
                            string strConditionItemID = arrSubItemInfo[1];
                            string strTempID = arrSubItemInfo[2];
                            strSql = string.Format("DELETE FROM T_31_045_{0} WHERE C001 = {1} AND C002 = {2}"
                                , rentToken
                                , strQueryConditionID
                                , strConditionItemID);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strSql =
                                string.Format("INSERT INTO T_31_045_{0} SELECT {1}, {2}, C002, C011, C012, C013, C014, C015 FROM T_00_901 WHERE C001 = {3}"
                                    , rentToken
                                    , strQueryConditionID
                                    , strConditionItemID
                                    , strTempID);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                        }
                        break;
                    case 3:
                        for (int i = 0; i < intCount; i++)
                        {
                            string strSubItemInfo = listParams[i + 1];
                            string[] arrSubItemInfo = strSubItemInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.RemoveEmptyEntries);
                            if (arrSubItemInfo.Length < 3) { continue; }
                            string strQueryConditionID = arrSubItemInfo[0];
                            string strConditionItemID = arrSubItemInfo[1];
                            string strTempID = arrSubItemInfo[2];
                            strSql = string.Format("DELETE FROM T_31_045_{0} WHERE C001 = {1} AND C002 = {2}"
                                , rentToken
                                , strQueryConditionID
                                , strConditionItemID);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strSql =
                                string.Format("INSERT INTO T_31_045_{0} SELECT {1}, {2}, C002, C011, C012, C013, C014, C015 FROM T_00_901 WHERE C001 = {3}"
                                    , rentToken
                                    , strQueryConditionID
                                    , strConditionItemID
                                    , strTempID);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
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

        private OperationReturn SaveRecordMemoInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     记录ID
                //1     用户ID
                //2     记录备注信息
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRecordID = listParams[0];
                string strUserID = listParams[1];
                string strMemoInfo = listParams[2];
                if (string.IsNullOrEmpty(strRecordID)
                    || string.IsNullOrEmpty(strUserID)
                    || string.IsNullOrEmpty(strMemoInfo))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param invalid");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<RecordMemoInfo>(strMemoInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                RecordMemoInfo memoInfo = optReturn.Data as RecordMemoInfo;
                if (memoInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("RecordMemoInfo is null");
                    return optReturn;
                }
                if (memoInfo.ID <= 0)
                {
                    //获取新增备注ID
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("31");
                    webRequest.ListData.Add("306");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
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
                    memoInfo.ID = Convert.ToInt64(webReturn.Data);
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                bool bIsAdded = false;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_046_{0} WHERE C001 = {1}"
                           , rentToken
                           , memoInfo.ID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_046_{0} WHERE C001 = {1}"
                           , rentToken
                           , memoInfo.ID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                    DataRow dr;
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = memoInfo.ID;
                        bIsAdded = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C002"] = memoInfo.RecordSerialID;
                    dr["C003"] = memoInfo.UserID;
                    dr["C004"] = Convert.ToInt64(memoInfo.MemoTime.ToString("yyyyMMddHHmmss"));
                    dr["C005"] = memoInfo.Content;
                    dr["C006"] = memoInfo.State;
                    dr["C007"] = memoInfo.LastModifyUserID;
                    dr["C008"] = Convert.ToInt64(memoInfo.LastModifyTime.ToString("yyyyMMddHHmmss"));
                    dr["C009"] = memoInfo.DeleteUserID;
                    dr["C010"] = Convert.ToInt64(memoInfo.DeleteTime.ToString("yyyyMMddHHmmss"));
                    dr["C011"] = memoInfo.Source;
                    if (bIsAdded)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                    bIsAdded = true;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                    bIsAdded = false;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                if (!bIsAdded)
                {
                    return optReturn;
                }
                optReturn.Data = memoInfo.ID;
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

        private OperationReturn SaveScoreSheetResult(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     评分表成绩信息
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreSheetResult = listParams[0];
                optReturn = XMLHelper.DeserializeObject<BasicScoreSheetInfo>(strScoreSheetResult);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                BasicScoreSheetInfo scoreSheetResult = optReturn.Data as BasicScoreSheetInfo;
                if (scoreSheetResult == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheetResult is null");
                    return optReturn;
                }
                string strResultID = scoreSheetResult.ScoreResultID.ToString();
                //是否修改成绩
                bool isModify = !string.IsNullOrEmpty(strResultID) && strResultID != "0";

                #region 获取新的成绩ID

                //生成一个新的成绩ID
                WebRequest webRequest = new WebRequest();
                webRequest.Session = session;
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("307");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
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
                string strNewResultID = webReturn.Data;
                if (string.IsNullOrEmpty(strNewResultID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("New ScoreResultID is empty");
                    return optReturn;
                }

                #endregion

                #region DBQuery

                string rentToken = session.RentInfo.Token;
                //是否增加记录
                bool isAdd = false;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_008_{0} WHERE C001 = {1} OR C001 = {2}",
                            rentToken,
                            strResultID,
                            strNewResultID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_008_{0} WHERE C001 = {1} OR C001 = {2}",
                            rentToken,
                            strResultID,
                            strNewResultID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }

                #endregion

                #region DBOperation

                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    DataRow drModify, drAdd;
                    //修改成绩时，向成绩表里新增一行成绩信息，同时将原成绩记录的C009字段值为N，并且新增记录的C010字段值为2
                    drModify = objDataSet.Tables[0].Select(string.Format("C001 = {0}", strResultID)).FirstOrDefault();
                    //drModify = objDataSet.Tables[0].Select(string.Format("C002 = {0} AND C009='Y'", scoreSheetResult.RecordSerialID.ToString())).FirstOrDefault();
                    if (drModify != null)
                    {
                        drModify["C009"] = "N";
                    }

                    drAdd = objDataSet.Tables[0].Select(string.Format("C001 = {0}", strNewResultID)).FirstOrDefault();
                    if (drAdd == null)
                    {
                        drAdd = objDataSet.Tables[0].NewRow();
                        drAdd["C001"] = Convert.ToInt64(strNewResultID);
                        isAdd = true;
                    }
                    drAdd["C002"] = scoreSheetResult.RecordSerialID;
                    drAdd["C003"] = scoreSheetResult.ScoreSheetID;
                    drAdd["C004"] = scoreSheetResult.Score;
                    drAdd["C005"] = session.UserID;
                    drAdd["C006"] = DateTime.Now.ToUniversalTime();
                    drAdd["C007"] = "N";
                    drAdd["C008"] = 0;
                    drAdd["C009"] = "Y";
                    if (isModify)
                    {
                        drAdd["C010"] = "2";
                    }
                    else
                    {
                        drAdd["C010"] = "1";
                    }
                    drAdd["C011"] = "N";
                    drAdd["C012"] = scoreSheetResult.OrgID;
                    drAdd["C013"] = scoreSheetResult.AgentID;
                    drAdd["C014"] = "0";
                    drAdd["C015"] = 0;
                    drAdd["C016"] = scoreSheetResult.WasteTime;//评分所花费时间
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(drAdd);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

                #endregion

                optReturn.Data = strNewResultID;
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

        private OperationReturn SaveScoreItemResultInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     评分表编码
                //1     成绩编码
                //2     打分项总数
                //3..   打分项得分信息
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreSheetID = listParams[0];
                string strResultID = listParams[1];
                string strCount = listParams[2];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ScoreResultCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 3 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ScoreResult count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_009_{0} WHERE C001 = {1} AND C003 = {2}",
                            rentToken,
                            strScoreSheetID,
                            strResultID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_009_{0} WHERE C001 = {1} AND C003 = {2}",
                            rentToken,
                            strScoreSheetID,
                            strResultID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                bool isAdd = false;
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    for (int i = 0; i < intCount; i++)
                    {
                        string strScoreItemResult = listParams[3 + i];
                        optReturn = XMLHelper.DeserializeObject<BasicScoreItemInfo>(strScoreItemResult);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        BasicScoreItemInfo scoreItemResult = optReturn.Data as BasicScoreItemInfo;
                        if (scoreItemResult == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("ScoreItemResult is null");
                            return optReturn;
                        }
                        DataRow[] drs =
                            objDataSet.Tables[0].Select(string.Format("C002 = {0}", scoreItemResult.ScoreItemID));
                        DataRow dr;
                        if (drs.Length <= 0)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strScoreSheetID;
                            dr["C002"] = scoreItemResult.ScoreItemID;
                            dr["C003"] = strResultID;
                            isAdd = true;
                        }
                        else
                        {
                            dr = drs[0];
                        }
                        dr["C004"] = scoreItemResult.Score;
                        dr["C005"] = scoreItemResult.IsNA;
                        dr["C006"] = "N";
                        dr["C007"] = "N";
                        dr["C008"] = scoreItemResult.RealScore;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);

                            listReturn.Add(string.Format("Add{0}{1}", ConstValue.SPLITER_CHAR,
                                scoreItemResult.ScoreItemID));
                        }
                        else
                        {
                            listReturn.Add(string.Format("Modify{0}{1}", ConstValue.SPLITER_CHAR,
                              scoreItemResult.ScoreItemID));
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

        private OperationReturn SaveScoreCommentResultInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     成绩编码
                //1     评分备注项总数
                //2...     评分备注项信息
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strResultID = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ScoreCommentResultCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ScoreCommentResult count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_011_{0} WHERE C001 = {1}",
                            rentToken,
                            strResultID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_011_{0} WHERE C001 = {1}",
                            rentToken,
                            strResultID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                bool isAdd = false;
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    for (int i = 0; i < intCount; i++)
                    {
                        string strScoreCommentResult = listParams[2 + i];
                        optReturn = XMLHelper.DeserializeObject<BasicScoreCommentInfo>(strScoreCommentResult);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        BasicScoreCommentInfo scoreCommentResult = optReturn.Data as BasicScoreCommentInfo;
                        if (scoreCommentResult == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("ScoreItemResult is null");
                            return optReturn;
                        }
                        DataRow[] drs =
                            objDataSet.Tables[0].Select(string.Format("C002 = {0}", scoreCommentResult.ScoreCommentID));
                        DataRow dr;
                        if (drs.Length <= 0)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strResultID;
                            dr["C002"] = scoreCommentResult.ScoreCommentID;
                            isAdd = true;
                        }
                        else
                        {
                            dr = drs[0];
                        }
                        dr["C003"] = scoreCommentResult.CommentText;
                        dr["C004"] = scoreCommentResult.CommentItemOrderID;
                        dr["C005"] = scoreCommentResult.CommentItemID;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);

                            listReturn.Add(string.Format("Add{0}{1}", ConstValue.SPLITER_CHAR,
                                scoreCommentResult.ScoreCommentID));
                        }
                        else
                        {
                            listReturn.Add(string.Format("Modify{0}{1}", ConstValue.SPLITER_CHAR,
                              scoreCommentResult.ScoreCommentID));
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

        private OperationReturn SaveScoreDataResult(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     评分表成绩信息
                //1     评分表信息
                //2     录音记录信息
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreSheetResult = listParams[0];
                string strScoreSheet = listParams[1];
                string strRecordInfo = listParams[2];
                optReturn = XMLHelper.DeserializeObject<BasicScoreSheetInfo>(strScoreSheetResult);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                BasicScoreSheetInfo scoreSheetResult = optReturn.Data as BasicScoreSheetInfo;
                if (scoreSheetResult == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheetResult is null");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<ScoreSheet>(strScoreSheet);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                ScoreSheet scoreSheet = optReturn.Data as ScoreSheet;
                if (scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet is null");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<RecordInfo>(strRecordInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                RecordInfo recordInfo = optReturn.Data as RecordInfo;
                if (recordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                string strResultID = scoreSheetResult.ScoreResultID.ToString();
                string strOldResultID = scoreSheetResult.OldScoreResultID.ToString();
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_041_{0} WHERE C001 = {1} OR C001 = {2}",
                            rentToken,
                            strResultID,
                            strOldResultID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_041_{0} WHERE C001 = {1} OR C001 = {2}",
                            rentToken,
                            strResultID,
                            strOldResultID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }

                //++++++++++功能:没使用一次评分表都在T_31_001里面增加次数  by  tangche  +++++++++++++++++++++++++++++++
                string strSql_IncreaseTimes;
                OperationReturn optReturn_ = new OperationReturn();
                optReturn_.Result = true;
                optReturn_.Code = 0;
                //IDbConnection objConn_;
                //IDbDataAdapter objAdapter_;
                //DbCommandBuilder objCmdBuilder_;
                switch (session.DBType)
                {
                    case 2:
                        strSql_IncreaseTimes = string.Format("UPDATE T_31_001_{0} SET C017=C017+1 WHERE C001={1}", rentToken, scoreSheetResult.ScoreSheetID.ToString());
                        optReturn_ = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql_IncreaseTimes);
                        //if (!optReturn.Result)
                        //{
                        //    return optReturn;
                        //}
                        //objConn_ = MssqlOperation.GetConnection(session.DBConnectionString);
                        //objAdapter_ = MssqlOperation.GetDataAdapter(objConn_, strSql_IncreaseTimes);
                        //objCmdBuilder_ = MssqlOperation.GetCommandBuilder(objAdapter_);
                        break;
                    case 3:
                        strSql_IncreaseTimes = string.Format("UPDATE T_31_001_{0} SET C017=C017+1 WHERE C001={1}", rentToken, scoreSheetResult.ScoreSheetID.ToString());
                        optReturn_ = OracleOperation.ExecuteSql(session.DBConnectionString, strSql_IncreaseTimes);
                        //if (!optReturn.Result)
                        //{
                        //    return optReturn;
                        //}
                        //objConn_ = OracleOperation.GetConnection(session.DBConnectionString);
                        //objAdapter_ = OracleOperation.GetDataAdapter(objConn_, strSql_IncreaseTimes);
                        //objCmdBuilder_ = OracleOperation.GetCommandBuilder(objAdapter_);
                        break;
                    default:
                        optReturn_.Result = false;
                        optReturn_.Code = Defines.RET_PARAM_INVALID;
                        optReturn_.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn_;
                }
                //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                //+++++++++++++++++++++++++++++++++++++++++++++++修改t_31_041里的其他的记录[录音流水号 评分表ID 一样的]的C005【是否为有效评分】置为0【无效评分】
                string strSql_UpdateC005;
                OperationReturn optReturn__ = new OperationReturn();
                optReturn__.Result = true;
                optReturn__.Code = 0;
                switch (session.DBType)
                {
                    case 2:
                        strSql_UpdateC005 = string.Format("UPDATE T_31_041_{0} SET C005='0' WHERE C000={1} AND C002 = {2}", rentToken, scoreSheetResult.ScoreSheetID.ToString(), recordInfo.SerialID);
                        optReturn__ = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql_UpdateC005);

                        break;
                    case 3:
                        strSql_UpdateC005 = string.Format("UPDATE T_31_041_{0} SET C005='0' WHERE C000={1} AND C002 = {2}", rentToken, scoreSheetResult.ScoreSheetID.ToString(), recordInfo.SerialID);
                        optReturn__ = OracleOperation.ExecuteSql(session.DBConnectionString, strSql_UpdateC005);
                        break;
                    default:
                        optReturn__.Result = false;
                        optReturn__.Code = Defines.RET_PARAM_INVALID;
                        optReturn__.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn__;
                }
                //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                bool isAdd = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    DataRow drAdd, drModify;
                    drModify = objDataSet.Tables[0].Select(string.Format("C001 = {0}", strOldResultID)).FirstOrDefault();
                    if (drModify != null)
                    {
                        drModify["C005"] = "0";
                    }
                    drAdd = objDataSet.Tables[0].Select(string.Format("C001= {0}", strResultID)).FirstOrDefault();
                    if (drAdd == null)
                    {
                        drAdd = objDataSet.Tables[0].NewRow();
                        drAdd["C001"] = Convert.ToInt64(strResultID);
                        isAdd = true;
                    }
                    drAdd["C000"] = scoreSheetResult.ScoreSheetID;
                    drAdd["C002"] = scoreSheetResult.RecordSerialID;
                    drAdd["C003"] = Convert.ToInt64(DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
                    drAdd["C004"] = session.UserID;
                    drAdd["C005"] = "1";
                    drAdd["C006"] = scoreSheetResult.Flag;//分数来源
                    drAdd["C007"] = "0";
                    drAdd["C008"] = 0;
                    drAdd["C009"] = "0";
                    drAdd["C010"] = 0;
                    drAdd["C011"] = 0;
                    drAdd["C012"] = string.Empty;
                    drAdd["C013"] = scoreSheetResult.WasteTime;
                    drAdd["C014"] = Converter.Second2Time(scoreSheetResult.WasteTime);
                    drAdd["C015"] = scoreSheetResult.OrgID;
                    drAdd["C016"] = 0;
                    drAdd["C051"] = recordInfo.SerialID;
                    drAdd["C052"] = Convert.ToInt64(recordInfo.StartRecordTime.ToString("yyyyMMddHHmmss"));
                    drAdd["C053"] = Convert.ToInt64(recordInfo.StopRecordTime.ToString("yyyyMMddHHmmss"));
                    drAdd["C054"] = recordInfo.Duration;
                    drAdd["C055"] = Converter.Second2Time(recordInfo.Duration);
                    drAdd["C056"] = recordInfo.Agent;
                    drAdd["C057"] = recordInfo.Extension;
                    drAdd["C058"] = recordInfo.CallerID;
                    drAdd["C059"] = recordInfo.CalledID;
                    drAdd["C060"] = recordInfo.Direction;
                    drAdd["C061"] = recordInfo.Extension;
                    drAdd["C062"] = recordInfo.ChannelName.ToString();
                    drAdd["C063"] = string.Empty;
                    int scale = 10000;
                    drAdd["C099"] = scale;
                    drAdd["C100"] = (int)(scoreSheetResult.Score * scale);
                    ////==========给T_31_041表的C017字段加入坐席ID=============================================================
                    //long VOICEID_longValue;
                    //string AgentID = GetAgentID(session, recordInfo.Agent);
                    //if (long.TryParse(AgentID,out VOICEID_longValue) && AgentID!=null)
                    //{
                    //    drAdd["C017"] = long.Parse(AgentID);
                    //}
                    ////=======================================================================================================
                    drAdd["C017"] = scoreSheetResult.AgentID;
                    List<ScoreItem> listItems = new List<ScoreItem>();
                    scoreSheet.GetAllScoreItem(ref listItems);
                    listItems = listItems.OrderBy(s => s.ItemID).ToList();
                    int count = listItems.Count;
                    for (int i = 0; i < count; i++)
                    {
                        int itemID = listItems[i].ItemID;
                        drAdd[string.Format("C{0}", (itemID + 100).ToString("000"))] = (int)(listItems[i].Score * scale);
                    }
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(drAdd);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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
                optReturn.Data = strResultID;
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

        ////获得坐席ID Agent表示的是坐席工号  by 汤澈
        //private string GetAgentID(SessionInfo session, string Agent)
        //{
        //    OperationReturn optReturn = new OperationReturn();
        //    optReturn.Result = true;
        //    optReturn.Code = 0;

        //    string AgentID;

        //    string TokenID = session.RentInfo.Token;
        //    string EncryptToAgent = EncryptToDB(Agent);
        //    string strSql;
        //    DataSet objDataSet;
        //    switch(session.DBType)
        //    {
        //        case 2:
        //            strSql = string.Format("SELECT C001 FROM T_11_101_{0} WHERE C017='{1}'", TokenID, EncryptToAgent);
        //            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
        //            if (!optReturn.Result)
        //            {
        //                return null;
        //            }
        //            objDataSet = optReturn.Data as DataSet;
        //            break;
        //        case 3:
        //            strSql = string.Format("SELECT C001 FROM T_11_101_{0} WHERE C017='{1}'", TokenID, EncryptToAgent);
        //            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
        //            if (!optReturn.Result)
        //            {
        //                return null;
        //            }
        //            objDataSet = optReturn.Data as DataSet;
        //            break;
        //        default:
        //            optReturn.Result = false;
        //            optReturn.Code = Defines.RET_PARAM_INVALID;
        //            optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
        //            return null;
        //    }
        //    if (objDataSet == null)
        //    {
        //        optReturn.Result = false;
        //        optReturn.Code = Defines.RET_OBJECT_NULL;
        //        optReturn.Message = string.Format("DataSet is null");
        //        return null;
        //    }
        //    string strAgentID;
        //    DataRow dr = objDataSet.Tables[0].Rows[0];
        //    strAgentID = dr["C001"].ToString();
        //    AgentID = strAgentID;
        //    return AgentID;
        //}

        private OperationReturn SaveUserSettingInfos(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编号
                //1     个数
                //2...  设置信息
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
                    optReturn.Message = string.Format("SettingInfoCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("SettingInfo count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_011_{0} WHERE C001 = {1}"
                           , rentToken
                           , strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_011_{0} WHERE C001 = {1}"
                           , rentToken
                           , strUserID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    for (int i = 0; i < intCount; i++)
                    {
                        string strSettingInfo = listParams[i + 2];
                        optReturn = XMLHelper.DeserializeObject<SettingInfo>(strSettingInfo);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        SettingInfo settingInfo = optReturn.Data as SettingInfo;
                        if (settingInfo == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("SettingInfo is null");
                            return optReturn;
                        }
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C002 = {0}", settingInfo.ParamID));
                        DataRow dr;
                        bool isAdd = false;
                        if (drs.Length <= 0)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = settingInfo.UserID;
                            dr["C002"] = settingInfo.ParamID;
                            isAdd = true;
                        }
                        else
                        {
                            dr = drs[0];
                        }
                        dr["C003"] = settingInfo.GroupID;
                        dr["C004"] = settingInfo.SortID;
                        dr["C005"] = settingInfo.StringValue;
                        dr["C006"] = settingInfo.DataType;
                        dr["C008"] = "0";
                        dr["C009"] = "0";
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            listReturn.Add(string.Format("Add{0}{1}", ConstValue.SPLITER_CHAR, settingInfo.ParamID));
                        }
                        else
                        {
                            listReturn.Add(string.Format("Update{0}{1}", ConstValue.SPLITER_CHAR, settingInfo.ParamID));
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

        private OperationReturn SaveViewColumnInfos(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编号
                //1     视图编号
                //2     个数
                //3...  列配置信息
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strViewID = listParams[1];
                string strCount = listParams[2];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ViewColumnInfoCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 3 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ViewColumnInfo count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_203_{0} WHERE C001 = {1} and C002 = {2}"
                           , rentToken
                           , strUserID
                           , strViewID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_203_{0} WHERE C001 = {1} and C002 = {2}"
                          , rentToken
                          , strUserID
                          , strViewID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    for (int i = 0; i < intCount; i++)
                    {
                        string strSettingInfo = listParams[i + 3];
                        optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(strSettingInfo);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                        if (columnInfo == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("SettingInfo is null");
                            return optReturn;
                        }
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C003 = '{0}'", columnInfo.ColumnName));
                        DataRow dr;
                        bool isAdd = false;
                        if (drs.Length <= 0)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strUserID;
                            dr["C002"] = strViewID;
                            dr["C003"] = columnInfo.ColumnName;
                            isAdd = true;
                        }
                        else
                        {
                            dr = drs[0];
                        }
                        dr["C004"] = columnInfo.SortID;
                        dr["C005"] = "1";
                        dr["C006"] = "1";
                        dr["C007"] = "1";
                        dr["C008"] = "1";
                        dr["C009"] = "1";
                        dr["C010"] = "1";
                        dr["C011"] = columnInfo.Visibility;
                        dr["C012"] = "0";
                        dr["C013"] = "0";
                        dr["C014"] = "0";
                        dr["C015"] = "0";
                        dr["C016"] = columnInfo.Width;
                        dr["C017"] = 26;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            listReturn.Add(string.Format("Add{0}{1}", ConstValue.SPLITER_CHAR, columnInfo.ColumnName));
                        }
                        else
                        {
                            listReturn.Add(string.Format("Update{0}{1}", ConstValue.SPLITER_CHAR, columnInfo.ColumnName));
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

        private OperationReturn SaveUserConditionItemInfos(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编号
                //1     个数
                //2...  查询条件项
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
                    optReturn.Message = string.Format("ViewColumnInfoCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ViewColumnInfo count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_043_{0} WHERE C002 = {1}"
                           , rentToken
                           , strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_043_{0} WHERE C002 = {1}"
                          , rentToken
                          , strUserID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        objDataSet.Tables[0].Rows[i].Delete();
                    }
                    for (int i = 0; i < intCount; i++)
                    {
                        string strConditionItem = listParams[i + 2];
                        optReturn = XMLHelper.DeserializeObject<CustomConditionItem>(strConditionItem);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        CustomConditionItem conditionItem = optReturn.Data as CustomConditionItem;
                        if (conditionItem == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("ConditionItem is null");
                            return optReturn;
                        }
                        DataRow dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = conditionItem.ID;
                        dr["C002"] = strUserID;
                        dr["C003"] = conditionItem.TabIndex;
                        dr["C004"] = conditionItem.TabName;
                        dr["C005"] = conditionItem.SortID;
                        dr["C006"] = conditionItem.ViewMode;
                        objDataSet.Tables[0].Rows.Add(dr);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

        private OperationReturn SavePlayInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     Count
                //1...  播放信息

                //==============================================================================================
                //这里我将下面的listParams.Count<2  改为listParams.Count < 1 或者直接将后面的listParams搞没就行了
                //这样改就能在播放列表里面没有播放记录的时候也能保存播放记录
                //==============================================================================================
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strCount = listParams[0];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("SettingInfoCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 1 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("SettingInfo count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_038_{0}"
                           , rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_038_{0}"
                            , rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    for (int i = 0; i < intCount; i++)
                    {
                        string strInfo = listParams[i + 1];
                        optReturn = XMLHelper.DeserializeObject<RecordPlayInfo>(strInfo);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        RecordPlayInfo info = optReturn.Data as RecordPlayInfo;
                        if (info == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("SettingInfo is null");
                            return optReturn;
                        }
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C001 = {0}", info.SerialID));
                        DataRow dr;
                        bool isAdd = false;
                        if (drs.Length <= 0)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = info.SerialID;
                            isAdd = true;
                        }
                        else
                        {
                            dr = drs[0];
                        }
                        dr["C002"] = info.RecordID;
                        dr["C003"] = info.Player;
                        dr["C004"] = info.PlayTime;
                        dr["C005"] = info.Duration;
                        dr["C006"] = info.PlayTerminal;
                        dr["C007"] = info.PlayTimes;
                        dr["C008"] = info.StartPosition;
                        dr["C009"] = info.StopPosition;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            listReturn.Add(string.Format("Add{0}{1}", ConstValue.SPLITER_CHAR, info.SerialID));
                        }
                        else
                        {
                            listReturn.Add(string.Format("Update{0}{1}", ConstValue.SPLITER_CHAR, info.SerialID));
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

        private OperationReturn InsertConditionSubItems(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     QueryConditionDetail
                //1     Count
                //2...  SubItemInfo
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strDetail = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("SubItems count param invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("SubItems count invalid");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<QueryConditionDetail>(strDetail);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                QueryConditionDetail detail = optReturn.Data as QueryConditionDetail;
                if (detail == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("QueryConditionDetail is null");
                    return optReturn;
                }
                string strLog = string.Empty;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = session;
                webRequest.Code = (int)RequestCode.WSInsertTempData;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(intCount.ToString());
                for (int i = 0; i < intCount; i++)
                {
                    string strSubItem = listParams[i + 2];
                    optReturn = XMLHelper.DeserializeObject<QueryConditionSubItem>(strSubItem);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    QueryConditionSubItem subItem = optReturn.Data as QueryConditionSubItem;
                    if (subItem == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("QueryConditionDetail is null");
                        return optReturn;
                    }
                    //坐席，需要将坐席号加密保存到C014
                    if (detail.ConditionItemID == S3102Consts.CON_AGENT_MULTITEXT)
                    {
                        subItem.Value04 = EncryptString02(subItem.Value01);
                    }
                    string strInfo = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}"
                       , subItem.Value01
                       , ConstValue.SPLITER_CHAR
                       , subItem.Value02
                       , subItem.Value03
                       , subItem.Value04
                       , subItem.Value05);
                    webRequest.ListData.Add(strInfo);
                    strLog += string.Format("{0} ", subItem.Value01);
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
                detail.Value01 = webReturn.Data;
                detail.Value02 = strLog;
                optReturn = XMLHelper.SeriallizeObject(detail);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
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

        private OperationReturn InsertManageObjectQueryInfos(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                //1     个数
                //2...  管理对象信息（Type+ID+Name）
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
                    optReturn.Message = string.Format("ManageObject count param invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ManageObject count invalid");
                    return optReturn;
                }
                List<List<string>> listInsertInfo = new List<List<string>>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 2];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("ManageObject info count invalid");
                        return optReturn;
                    }
                    string strType = arrInfo[0];
                    string strID = arrInfo[1];
                    //ListInfo
                    //0     ObjType
                    //1     ObjID
                    //2     Name
                    //3     Other
                    //4     Other
                    List<string> listInfos = new List<string>();
                    if (strType == ConstValue.RESOURCE_AGENT.ToString())
                    {
                        listInfos.Add(strType);
                        listInfos.Add(strID);
                        listInfos.Add(arrInfo.Length > 2 ? arrInfo[2] : string.Empty);
                        listInfos.Add(string.Empty);
                        listInfos.Add(string.Empty);
                        listInsertInfo.Add(listInfos);
                    }
                    if (strType == ConstValue.RESOURCE_EXTENSION.ToString())
                    {
                        listInfos.Add(strType);
                        listInfos.Add(strID);
                        listInfos.Add(arrInfo.Length > 2 ? arrInfo[2] : string.Empty);
                        listInfos.Add(string.Empty);
                        listInfos.Add(string.Empty);
                        listInsertInfo.Add(listInfos);
                    }
                    if (strType == ConstValue.RESOURCE_REALEXT.ToString())
                    {
                        listInfos.Add(strType);
                        listInfos.Add(strID);
                        listInfos.Add(arrInfo.Length > 2 ? arrInfo[2] : string.Empty);
                        listInfos.Add(string.Empty);
                        listInfos.Add(string.Empty);
                        listInsertInfo.Add(listInfos);
                    }
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;


                #region Agent

                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 >= 1030000000000000000 AND C001 < 1040000000000000000 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1})",
                                rentToken,
                                strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 >= 1030000000000000000 AND C001 < 1040000000000000000 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1})",
                                rentToken,
                                strUserID);
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
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strID = dr["C001"].ToString();
                    string strEncryptName = dr["C017"].ToString();
                    List<string> temp =
                        listInsertInfo.FirstOrDefault(o => o[0] == ConstValue.RESOURCE_AGENT.ToString() && o[1] == strID);
                    if (temp != null)
                    {
                        temp[3] = strEncryptName;
                    }
                }

                #endregion

                #region Extension
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 >= 1040000000000000000 AND C001 < 1050000000000000000 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1})",
                                rentToken,
                                strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 >= 1040000000000000000 AND C001 < 1050000000000000000 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1})",
                                rentToken,
                                strUserID);
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
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strID = dr["C001"].ToString();

                    //分机号 +  ip
                    string strEncryptName = dr["C017"].ToString();
                    List<string> temp =
                        listInsertInfo.FirstOrDefault(o => o[0] == ConstValue.RESOURCE_EXTENSION.ToString() && o[1] == strID);
                    if (temp != null)
                    {
                        temp[3] = strEncryptName;
                    }
                }
                #endregion

                if (listInsertInfo.Count == 0)
                {
                    optReturn.Data = string.Empty;
                    return optReturn;
                }

                WebRequest webRequest = new WebRequest();
                webRequest.Session = session;
                webRequest.Code = (int)RequestCode.WSInsertTempData;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(listInsertInfo.Count.ToString());
                for (int i = 0; i < listInsertInfo.Count; i++)
                {
                    List<string> listInfo = listInsertInfo[i];
                    string strInfo = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}"
                       , listInfo[0]
                       , ConstValue.SPLITER_CHAR
                       , listInfo[1]
                       , listInfo[2]
                       , listInfo[3]
                       , listInfo[4]);
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
                optReturn.Data = webReturn.Data;
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

        private OperationReturn SaveRecordBookmarkInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     记录编号
                //1     用户编号
                //2     Count
                //3...  Bookmark信息
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRecordID = listParams[0];
                string strUserID = listParams[1];
                string strCount = listParams[2];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("BookmarkCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 3 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Bookmark count invalid");
                    return optReturn;
                }
                List<RecordBookmarkInfo> listBookmarks = new List<RecordBookmarkInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<RecordBookmarkInfo>(listParams[3 + i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    RecordBookmarkInfo info = optReturn.Data as RecordBookmarkInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("BookmarkInfo is null");
                        return optReturn;
                    }
                    listBookmarks.Add(info);
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_042_{0} WHERE C003 = {1} AND C010 = {2} AND C009 = '1'"
                           , rentToken,
                           strRecordID,
                           strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_042_{0} WHERE C003 = {1} AND C010 = {2} AND C009 = '1'"
                          , rentToken,
                          strRecordID,
                          strUserID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    //删除Bookmark
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        var temp = listBookmarks.FirstOrDefault(t => t.SerialID.ToString() == strID);
                        if (temp == null)
                        {
                            objDataSet.Tables[0].Rows[i].Delete();
                            listReturn.Add(string.Format("D{0}{1}", ConstValue.SPLITER_CHAR, strID));
                        }
                    }

                    for (int i = 0; i < listBookmarks.Count; i++)
                    {
                        RecordBookmarkInfo info = listBookmarks[i];
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C001 = {0}", info.SerialID));
                        DataRow dr;
                        bool isAdd = false;
                        if (drs.Length <= 0)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = info.SerialID;
                            isAdd = true;
                        }
                        else
                        {
                            dr = drs[0];
                        }
                        dr["C002"] = info.RecordRowID;
                        dr["C003"] = info.RecordID;
                        dr["C004"] = info.Offset;
                        dr["C005"] = info.Duration;
                        dr["C006"] = info.Title;
                        dr["C007"] = info.Content;
                        dr["C008"] = "A";
                        dr["C009"] = "1";
                        dr["C010"] = info.MarkerID;
                        dr["C011"] = info.MarkTime.ToString("yyyyMMddHHmmss");
                        dr["C012"] = strUserID;
                        dr["C013"] = DateTime.Now.ToString("yyyyMMddHHmmss");
                        dr["C016"] = info.RankID;
                        dr["C017"] = info.Teminal;
                        dr["C101"] = info.IsHaveBookMarkRecord;
                        if (string.IsNullOrEmpty(info.IsHaveBookMarkRecord))
                        {
                            dr["C101"] = "0";
                        }
                        dr["C102"] = info.BookmarkTimesLength;
                        if (string.IsNullOrEmpty(info.BookmarkTimesLength))
                        {
                            dr["C102"] = "-1";
                        }
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            listReturn.Add(string.Format("A{0}{1}", ConstValue.SPLITER_CHAR, info.SerialID));
                        }
                        else
                        {
                            listReturn.Add(string.Format("U{0}{1}", ConstValue.SPLITER_CHAR, info.SerialID));
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

        //删除该用户的关于该条录音的所有的标签
        private OperationReturn DeleteUsersRecordBookMark(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0    录音流水号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string seriesID = listParams[0];
                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("DELETE FROM T_31_042_{0} WHERE  C003={1}",
                            rentToken, seriesID);
                        optReturn.Data = strSql;
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("DELETE FROM T_31_042_{0} WHERE  C003={1}",
                            rentToken, seriesID);
                        optReturn.Data = strSql;
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }

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

        private OperationReturn SaveBookmarkRankInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                //1     Count
                //2...  BookmarkRank信息
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
                    optReturn.Message = string.Format("BookmarkRankCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("BookmarkRank count invalid");
                    return optReturn;
                }
                List<BookmarkRankInfo> listBookmarkRanks = new List<BookmarkRankInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<BookmarkRankInfo>(listParams[2 + i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    BookmarkRankInfo info = optReturn.Data as BookmarkRankInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("BookmarkRankInfo is null");
                        return optReturn;
                    }
                    listBookmarkRanks.Add(info);
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C001 >= 3090000000000000000 and C001 < 3100000000000000000"
                           , rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C001 >= 3090000000000000000 and C001 < 3100000000000000000"
                           , rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    //删除BookmarkRank
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        var temp = listBookmarkRanks.FirstOrDefault(t => t.ID.ToString() == strID);
                        if (temp == null)
                        {
                            objDataSet.Tables[0].Rows[i].Delete();
                            listReturn.Add(string.Format("D{0}{1}", ConstValue.SPLITER_CHAR, strID));
                        }
                    }

                    for (int i = 0; i < listBookmarkRanks.Count; i++)
                    {
                        BookmarkRankInfo info = listBookmarkRanks[i];
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C001 = {0}", info.ID));
                        DataRow dr;
                        bool isAdd = false;
                        if (drs.Length <= 0)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = info.ID;
                            isAdd = true;
                        }
                        else
                        {
                            dr = drs[0];
                        }
                        dr["C000"] = 3;
                        dr["C002"] = info.OrderID;
                        dr["C003"] = 0;
                        dr["C004"] = "1";
                        dr["C005"] = 0;
                        dr["C006"] = EncryptString02(info.Color);
                        if (isAdd)
                        {
                            dr["C007"] = session.UserID;
                        }
                        dr["C008"] = "BM";
                        dr["C009"] = info.Name;
                        dr["C010"] = "0";
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            listReturn.Add(string.Format("A{0}{1}", ConstValue.SPLITER_CHAR, info.ID));
                        }
                        else
                        {
                            listReturn.Add(string.Format("U{0}{1}", ConstValue.SPLITER_CHAR, info.ID));
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

        private OperationReturn SaveMemoInfoToT_21_001(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     录音流水号ID
                //1     备注内容
                //2     表名
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string SerialID = listParams[0];
                string MemoInfo = listParams[1];
                string TableName = listParams[2];

                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM {1} WHERE C002 = {0}", SerialID, TableName);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM {1} WHERE C002 = {0}", SerialID, TableName);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                //bool isAdd = false;
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    DataRow drModify;
                    drModify = objDataSet.Tables[0].Select(string.Format("C002 = {0}", SerialID)).FirstOrDefault();
                    if (drModify != null)
                    {
                        drModify["C072"] = MemoInfo;

                        listReturn.Add(string.Format("M{0}{1}", ConstValue.SPLITER_CHAR, SerialID));
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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
                //这里的Data其实是返回到WPF里的,但是这里是保存到数据库,所以这个Data不会返回到WPF,可以写也可以不写
                optReturn.Message = strSql;
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

        private OperationReturn SaveBookMarkTitleToT_21_001(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     录音流水号ID
                //1     标注标题
                //2     表名
                //3     标注次数
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string SerialID = listParams[0];
                string AllBookMarkTitle = listParams[1];
                string TableName = listParams[2];
                string MarkedTimes = listParams[3];

                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM {1} WHERE C002 = {0}", SerialID, TableName);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM {1} WHERE C002 = {0}", SerialID, TableName);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
                List<string> listReturn = new List<string>();
                //bool isAdd = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    DataRow drModify;
                    drModify = objDataSet.Tables[0].Select(string.Format("C002 = {0}", SerialID)).FirstOrDefault();
                    if (drModify != null)
                    {
                        drModify["C071"] = MarkedTimes;
                        drModify["C073"] = AllBookMarkTitle;

                        listReturn.Add(string.Format("M{0}{1}", ConstValue.SPLITER_CHAR, SerialID));

                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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
                //这里的Data其实是返回到WPF里的,但是这里是保存到数据库,所以这个Data不会返回到WPF,可以写也可以不写
                optReturn.Message = strSql;
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

        //将录音教材存入教材库，也就是存入T_31_060
        private OperationReturn InsertLearningToLibrary(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0    教材库文件夹主键
                //1    教材名字
                //2    录音流水号存放在教材库的路径
                //3    描述
                //4    该录音是否加密(0/1 未加密/加密)
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;

                string dirID = listParams[0];
                string learningName = listParams[1];
                string recordPath = listParams[2];
                string tempDiscription = listParams[3];
                string isEncrytp = listParams[4];

                #region 获取新的教材ID
                //生成一个新的教材ID
                WebRequest webRequest = new WebRequest();
                webRequest.Session = session;
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("362");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
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
                string strNewLearningID = webReturn.Data;
                if (string.IsNullOrEmpty(strNewLearningID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("New ScoreResultID is empty");
                    return optReturn;
                }
                #endregion

                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format(@"INSERT INTO T_31_060_{0} (C001,C002,C003,C004,C005,C006,C007,C008) VALUES('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                            rentToken, strNewLearningID, dirID, learningName, recordPath, tempDiscription, "1", isEncrytp, "1");
                        optReturn.Data = strSql;
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format(@"INSERT INTO T_31_060_{0} (C001,C002,C003,C004,C005,C006,C007,C008) VALUES('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')",
                            rentToken, strNewLearningID, dirID, learningName, recordPath, tempDiscription, "1", isEncrytp, "1");
                        optReturn.Data = strSql;
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }

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