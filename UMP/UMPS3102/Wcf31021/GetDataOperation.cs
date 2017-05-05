using System.Data.Common;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.ScoreSheets;
using VoiceCyber.DBAccesses;


namespace Wcf31021
{
    public partial class Service31021
    {
        private OperationReturn GetAllCustomCondition(SessionInfo session, List<string> listParams)//将数据库里所有的查询条件都搞出来
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
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_040_{0}  WHERE C001 LIKE '303140101%' ORDER BY C005,C007 ", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_040_{0}  WHERE C001 LIKE '303140101%' ORDER BY C005,C007", rentToken);
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
                    CustomConditionItem item = new CustomConditionItem();
                    item.ID = Convert.ToInt64(dr["C001"]);
                    item.Name = dr["C002"].ToString();
                    item.Format = (CustomConditionItemFormat)Convert.ToInt32(dr["C003"]);
                    item.Type = (CustomConditionItemType)Convert.ToInt32(dr["C004"]);
                    item.TabIndex = Convert.ToInt32(dr["C005"]);
                    item.TabName = dr["C006"].ToString();
                    item.SortID = Convert.ToInt32(dr["C007"]);
                    item.ViewMode = Convert.ToInt32(dr["C008"]);
                    item.Tag = dr["C009"].ToString();
                    item.Param = dr["C010"].ToString();
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

        private OperationReturn GetUserQueryCondition(SessionInfo session, List<string> listParams)//T_31_028是保存查询条件的表，也就是界面上面快速查询的名字
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
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_028_{0} WHERE C002 = {1} AND C003 LIKE '102%' ORDER BY C006"
                            , rentToken
                            , strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_028_{0} WHERE C002 = {1} AND C003 LIKE '102%' ORDER BY C006"
                            , rentToken
                            , strUserID);
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
                    QueryCondition item = new QueryCondition();
                    item.ID = Convert.ToInt64(dr["C001"]);
                    item.Name = dr["C008"].ToString();
                    item.Description = dr["C009"].ToString();
                    item.UserID = Convert.ToInt64(dr["C002"]);
                    item.Creator = Convert.ToInt64(dr["C003"]);
                    item.CreateTime = Convert.ToDateTime(dr["C004"]);
                    item.CreateType = dr["C005"].ToString();
                    item.SortID = Convert.ToInt32(dr["C006"]);
                    item.LastQueryTime = string.IsNullOrEmpty(dr["C007"].ToString())
                        ? DateTime.MinValue
                        : Convert.ToDateTime(dr["C007"]);
                    item.IsEnable = dr["C010"].ToString() == "Y";
                    string strValue = dr["C103"].ToString();
                    int intValue;
                    if (int.TryParse(strValue, out intValue))
                    {
                        item.UseCount = intValue;
                    }
                    strValue = dr["C104"].ToString();
                    if (int.TryParse(strValue, out intValue))
                    {
                        item.RecordCount = intValue;
                    }
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

        private OperationReturn GetUserCustomConditionItem(SessionInfo session, List<string> listParams)
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
                string rentToken = session.RentInfo.Token;
                bool isDefault = false;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strUserID == "0")
                        {
                            strSql = string.Format("SELECT * FROM T_31_040_{0} WHERE C008 = '1' OR C008 = '2' AND C001 LIKE '303140101%'", rentToken);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                        }
                        else
                        {
                            strSql =
                           string.Format(
                               "select count(1) FROM T_31_040_{0} A, T_31_043_{0} B WHERE A.C001 = B.C001 AND B.C002 = {1} AND A.C001 LIKE '303140101%'"
                               , rentToken
                               , strUserID);
                            optReturn = MssqlOperation.GetRecordCount(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            if (optReturn.IntValue <= 0)
                            {
                                isDefault = true;
                                strSql = string.Format("SELECT * FROM T_31_040_{0} WHERE C008 != 0 AND C001 LIKE '303140101%'", rentToken);
                            }
                            else
                            {
                                strSql = string.Format("SELECT A.C001, A.C002, A.C003, A.C004, A.C009, A.C010, B.C003 AS BC003, B.C004 AS BC004,B.C005, B.C006 ");
                                strSql += string.Format("FROM T_31_040_{0} A, T_31_043_{0} B WHERE A.C001 = B.C001 AND B.C002 = {1} AND A.C001 LIKE '303140101%'"
                                    , rentToken
                                    , strUserID);
                            }
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                        }
                        break;
                    case 3:
                        if (strUserID == "0")
                        {
                            strSql = string.Format("SELECT * FROM T_31_040_{0} WHERE C008 = '1' OR C008 = '2' AND C001 LIKE '303140101%'", rentToken);
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                        }
                        else
                        {
                            strSql =
                          string.Format(
                              "SELECT COUNT(1) FROM T_31_040_{0} A, T_31_043_{0} B WHERE A.C001 = B.C001 AND B.C002 = {1} AND A.C001 LIKE '303140101%'"
                              , rentToken
                              , strUserID);
                            optReturn = OracleOperation.GetRecordCount(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            if (optReturn.IntValue <= 0)
                            {
                                isDefault = true;
                                strSql = string.Format("SELECT * FROM T_31_040_{0} WHERE C008 != 0 AND C001 LIKE '303140101%'", rentToken);
                            }
                            else
                            {
                                strSql = string.Format("SELECT A.C001, A.C002, A.C003, A.C004, A.C009, A.C010, B.C003 AS BC003, B.C004 AS BC004,B.C005, B.C006 ");
                                strSql += string.Format("FROM T_31_040_{0} A, T_31_043_{0} B WHERE A.C001 = B.C001 AND B.C002 = {1} AND A.C001 LIKE '303140101%'"
                                    , rentToken
                                    , strUserID);
                            }
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                        }
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
                    CustomConditionItem item = new CustomConditionItem();
                    if (strUserID == "0")
                    {
                        item.ID = Convert.ToInt64(dr["C001"]);
                        item.Name = dr["C002"].ToString();
                        item.Format = (CustomConditionItemFormat)Convert.ToInt32(dr["C003"]);
                        item.Type = (CustomConditionItemType)Convert.ToInt32(dr["C004"]);
                        item.TabIndex = Convert.ToInt32(dr["C005"]);
                        item.TabName = dr["C006"].ToString();
                        item.SortID = Convert.ToInt32(dr["C007"]);
                        item.ViewMode = Convert.ToInt32(dr["C008"]);
                        item.Tag = dr["C009"].ToString();
                        item.Param = dr["C010"].ToString();
                    }
                    else
                    {
                        if (isDefault)
                        {
                            item.ID = Convert.ToInt64(dr["C001"]);
                            item.Name = dr["C002"].ToString();
                            item.Format = (CustomConditionItemFormat)Convert.ToInt32(dr["C003"]);
                            item.Type = (CustomConditionItemType)Convert.ToInt32(dr["C004"]);
                            item.TabIndex = Convert.ToInt32(dr["C005"]);
                            item.TabName = dr["C006"].ToString();
                            item.SortID = Convert.ToInt32(dr["C007"]);
                            item.ViewMode = Convert.ToInt32(dr["C008"]);
                            item.Tag = dr["C009"].ToString();
                            item.Param = dr["C010"].ToString();
                        }
                        else
                        {
                            item.ID = Convert.ToInt64(dr["C001"]);
                            item.Name = dr["C002"].ToString();
                            item.Format = (CustomConditionItemFormat)Convert.ToInt32(dr["C003"]);
                            item.Type = (CustomConditionItemType)Convert.ToInt32(dr["C004"]);
                            item.TabIndex = Convert.ToInt32(dr["BC003"]);
                            item.TabName = dr["BC004"].ToString();
                            item.SortID = Convert.ToInt32(dr["C005"]);
                            item.ViewMode = Convert.ToInt32(dr["C006"]);
                            item.Tag = dr["C009"].ToString();
                            item.Param = dr["C010"].ToString();
                        }
                    }
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                if (isDefault)
                {
                    optReturn.Message = "0";
                }
                else
                {
                    optReturn.Message = "1";
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

        private OperationReturn GetConditionSubItem(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      查询ID（临时表中的查询编码）
                //1      查询条件ID
                //2      查询条件项ID
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strTempID = listParams[0];
                string strQueryConditionID = listParams[1];
                string strConditionItemID = listParams[2];
                if (string.IsNullOrEmpty(strTempID)
                    || string.IsNullOrEmpty(strQueryConditionID)
                    || string.IsNullOrEmpty(strConditionItemID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("TempID or QueryID or ConditionItemID is empty");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                bool isFromTemp = true;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT count(1) FROM T_00_901 WHERE C001 = {0}", strTempID);
                        optReturn = MssqlOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (optReturn.IntValue > 0)
                        {
                            strSql = string.Format("SELECT * FROM T_00_901 WHERE C001 = {0} ORDER BY C002", strTempID);
                        }
                        else
                        {
                            isFromTemp = false;
                            strSql = string.Format("SELECT * FROM T_31_045_{0}  WHERE C001 = {1} AND C002 = {2} ORDER BY C003"
                                 , rentToken
                                 , strQueryConditionID
                                 , strConditionItemID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT count(1) FROM T_00_901 WHERE C001 = {0}", strTempID);
                        optReturn = OracleOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (optReturn.IntValue > 0)
                        {
                            strSql = string.Format("SELECT * FROM T_00_901 WHERE C001 = {0} ORDER BY C002", strTempID);
                        }
                        else
                        {
                            isFromTemp = false;
                            strSql = string.Format("SELECT * FROM T_31_045_{0}  WHERE C001 = {1} AND C002 = {2} ORDER BY C003"
                                 , rentToken
                                 , strQueryConditionID
                                 , strConditionItemID);
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
                    QueryConditionSubItem item = new QueryConditionSubItem();
                    if (isFromTemp)
                    {
                        item.QueryID = Convert.ToInt64(strQueryConditionID);
                        item.ConditionItemID = Convert.ToInt64(strConditionItemID);
                        item.Number = Convert.ToInt32(dr["C002"]);
                        item.Value01 = dr["C011"].ToString();
                        item.Value02 = dr["C012"].ToString();
                        item.Value03 = dr["C013"].ToString();
                        item.Value04 = dr["C014"].ToString();
                        item.Value05 = dr["C015"].ToString();
                    }
                    else
                    {
                        item.QueryID = Convert.ToInt64(dr["C001"]);
                        item.ConditionItemID = Convert.ToInt64(dr["C002"]);
                        item.Number = Convert.ToInt32(dr["C003"]);
                        item.Value01 = dr["C004"].ToString();
                        item.Value02 = dr["C005"].ToString();
                        item.Value03 = dr["C006"].ToString();
                        item.Value04 = dr["C007"].ToString();
                        item.Value05 = dr["C008"].ToString();
                    }
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

        private OperationReturn GetQueryConditionDetail(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      查询条件编码
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strQueryConditionID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C001 = {1}"
                           , rentToken
                           , strQueryConditionID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C001 = {1}"
                            , rentToken
                            , strQueryConditionID);
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
                    QueryConditionDetail item = new QueryConditionDetail();
                    item.QueryID = Convert.ToInt64(dr["C001"]);
                    item.ConditionItemID = Convert.ToInt64(dr["C002"]);
                    item.IsEnable = dr["C003"].ToString() == "Y";
                    item.Value01 = dr["C004"].ToString();
                    item.Value02 = dr["C005"].ToString();
                    item.Value03 = dr["C006"].ToString();
                    item.Value04 = dr["C007"].ToString();
                    item.Value05 = dr["C008"].ToString();
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

        private OperationReturn GetRecordData(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     查询语句
                //1     是否关联T_31_054  如果为空,那么就没有关联   不为空 就关联了
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strQueryString = listParams[0];
                string strStatisticTable = listParams[1];
                string strSql = strQueryString;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
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
                    RecordInfo item = new RecordInfo();


                    #region 基本字段

                    item.RowID = Convert.ToInt64(dr["C001"]);
                    item.SerialID = Convert.ToInt64(dr["C002"]);
                    item.RecordReference = dr["C077"].ToString();
                    item.StartRecordTime = Convert.ToDateTime(dr["C005"]);
                    item.StopRecordTime = Convert.ToDateTime(dr["C009"]);
                    item.VoiceID = Convert.ToInt32(dr["C037"]);
                    item.VoiceIP = dr["C020"].ToString();
                    item.ChannelID = Convert.ToInt32(dr["C038"]);
                    item.Extension = dr["C042"].ToString();
                    item.Agent = dr["C039"].ToString();
                    item.Duration = Convert.ToInt32(dr["C012"]);
                    item.Direction = dr["C045"].ToString() == "1" ? 1 : 0;
                    item.CallerID = EncryptString04(dr["C040"].ToString());
                    item.CalledID = EncryptString04(dr["C041"].ToString());
                    item.WaveFormat = dr["C015"].ToString();
                    item.TaskNumber = dr["C104"].ToString();
                    item.MediaType = Convert.ToInt32(dr["C014"]);
                    item.EncryptFlag = dr["C025"].ToString();
                    item.SkillGroup = dr["C107"].ToString();
                    item.RealExtension = dr["C058"].ToString();
                    item.ParticipantNum = EncryptString04(dr["C055"].ToString());
                    item.CallerDTMF = dr["C043"].ToString();
                    item.CalledDTMF = dr["C044"].ToString();
                    item.ChannelName = dr["C046"].ToString();
                    item.CTIReference = dr["C047"].ToString();

                    #endregion


                    #region 客户化字段
                    item.CustomField01 = dr["C301"].ToString();
                    item.CustomField02 = dr["C302"].ToString();
                    item.CustomField03 = dr["C303"].ToString();
                    item.CustomField04 = dr["C304"].ToString();
                    item.CustomField05 = dr["C305"].ToString();
                    item.CustomField06 = dr["C306"].ToString();
                    item.CustomField07 = dr["C307"].ToString();
                    item.CustomField08 = dr["C308"].ToString();
                    item.CustomField09 = dr["C309"].ToString();
                    item.CustomField10 = dr["C310"].ToString();
                    item.CustomField11 = dr["C311"].ToString();
                    item.CustomField12 = dr["C312"].ToString();
                    item.CustomField13 = dr["C313"].ToString();
                    item.CustomField14 = dr["C314"].ToString();
                    item.CustomField15 = dr["C315"].ToString();
                    item.CustomField16 = dr["C316"].ToString();
                    item.CustomField17 = dr["C317"].ToString();
                    item.CustomField18 = dr["C318"].ToString();
                    item.CustomField19 = dr["C319"].ToString();
                    item.CustomField20 = dr["C320"].ToString();
                    #endregion


                    #region 统计分析相关

                    if (strStatisticTable != string.Empty)
                    {
                        //检查集合内是否用指定的列的名称
                        if (objDataSet.Tables[0].Columns.Contains("SERVICE"))
                        {
                            if (dr["SERVICE"].ToString() != null)
                            {
                                item.ServiceAttitude = dr["SERVICE"].ToString();
                            }
                        }
                        if (objDataSet.Tables[0].Columns.Contains("RECORDDUR"))
                        {
                            if (dr["RECORDDUR"].ToString() != null)
                            {
                                item.RecordDuritionExcept = dr["RECORDDUR"].ToString();
                            }
                        }
                        if (objDataSet.Tables[0].Columns.Contains("REPEATEDCALL"))
                        {
                            if (dr["REPEATEDCALL"].ToString() != null)
                            {
                                item.RepeatedCall = dr["REPEATEDCALL"].ToString();
                            }
                        }
                        if (objDataSet.Tables[0].Columns.Contains("CALLPEAK"))
                        {
                            if (dr["CALLPEAK"].ToString() != null)
                            {
                                item.CallPeak = dr["CALLPEAK"].ToString();
                            }
                        }
                        if (objDataSet.Tables[0].Columns.Contains("PROFESSIONAlLEVEL"))
                        {
                            if (dr["PROFESSIONAlLEVEL"].ToString() != null)
                            {
                                item.ProfessionalLevel = dr["PROFESSIONAlLEVEL"].ToString();
                            }
                        }
                    }

                    #endregion


                    #region 其他

                    item.IsaRefID = dr["C109"].ToString();
                    item.SavePath = dr["C035"].ToString();


                    #region 录音版本

                    string strMajorVer = dr["C091"].ToString();
                    string strMinorVer = dr["C092"].ToString();
                    string strBuildVer = dr["C093"].ToString();
                    double recordVersion = 0.0;
                    int majorVer;
                    int minorVer;
                    int buildVer;
                    if (int.TryParse(strMajorVer, out majorVer))
                    {
                        if (majorVer > 100)
                        {
                            recordVersion = majorVer * 1.0;
                        }
                        else
                        {
                            if (int.TryParse(strMinorVer, out minorVer))
                            {

                            }
                            if (int.TryParse(strBuildVer, out buildVer))
                            {

                            }
                            recordVersion = majorVer + minorVer * 0.01 + buildVer * 0.00001;
                        }
                    }
                    item.RecordVersion = recordVersion;

                    #endregion


                    #endregion


                    #region 评分判断

                    //++++++++++功能:对每条录音是否评过分做个判断，如果返回值大于0，那么就是评过分的+++++++++++++++++++++++++++++++
                    string tempSql;
                    OperationReturn optReturn_ = new OperationReturn();
                    optReturn_.Result = true;
                    optReturn_.Code = 0;
                    optReturn_.Data = 0;
                    switch (session.DBType)
                    {
                        case 2:
                            tempSql = string.Format("SELECT COUNT(1) FROM T_31_008_{0} WHERE C002={1} AND C009='Y' ", session.RentInfo.Token, item.SerialID);
                            optReturn_ = MssqlOperation.GetRecordCount(session.DBConnectionString, tempSql);
                            break;
                        case 3:
                            tempSql = string.Format("SELECT COUNT(1) FROM T_31_008_{0} WHERE C002={1} AND C009='Y' ", session.RentInfo.Token, item.SerialID);
                            optReturn_ = OracleOperation.GetRecordCount(session.DBConnectionString, tempSql);
                            break;
                        default:
                            optReturn_.Result = false;
                            optReturn_.Code = Defines.RET_PARAM_INVALID;
                            optReturn_.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                            return optReturn_;
                    }
                    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
                    if (optReturn_.IntValue == 1)
                    {
                        //已评分
                        item.IsScored = 2;
                        string tempScoreSql;
                        OperationReturn optReturn__ = new OperationReturn();
                        optReturn__.Result = true;
                        optReturn__.Code = 0;
                        optReturn__.Data = 0;
                        DataSet objDataSet__;
                        switch (session.DBType)
                        {
                            case 2:
                                tempScoreSql = string.Format("SELECT C004 FROM T_31_008_{0} WHERE C002={1} AND C009='Y' ", session.RentInfo.Token, item.SerialID);
                                optReturn__ = MssqlOperation.GetDataSet(session.DBConnectionString, tempScoreSql);
                                if (!optReturn__.Result)
                                {
                                    return optReturn__;
                                }
                                objDataSet__ = optReturn__.Data as DataSet;
                                break;
                            case 3:
                                tempScoreSql = string.Format("SELECT C004 FROM T_31_008_{0} WHERE C002={1} AND C009='Y' ", session.RentInfo.Token, item.SerialID);
                                optReturn__ = OracleOperation.GetDataSet(session.DBConnectionString, tempScoreSql);
                                if (!optReturn__.Result)
                                {
                                    return optReturn__;
                                }
                                objDataSet__ = optReturn__.Data as DataSet;
                                break;
                            default:
                                optReturn__.Result = false;
                                optReturn__.Code = Defines.RET_PARAM_INVALID;
                                optReturn__.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                                return optReturn__;
                        }
                        if (objDataSet__ == null)
                        {
                            optReturn__.Result = false;
                            optReturn__.Code = Defines.RET_OBJECT_NULL;
                            optReturn__.Message = string.Format("objDataSet__ is null");
                            return optReturn__;
                        }
                        for (int m = 0; m < objDataSet__.Tables[0].Rows.Count; m++)
                        {
                            item.Score = objDataSet__.Tables[0].Rows[m]["C004"].ToString();
                        }
                    }
                    else if (optReturn_.IntValue != 1 && optReturn_.IntValue != 0)
                    {
                        //已评分
                        item.IsScored = 1;
                    }
                    else
                    {
                        //未评分
                        item.IsScored = 0;
                    }

                    #endregion


                    #region 质检相关

                    if (string.IsNullOrEmpty(dr["C073"].ToString()))
                    {
                        item.IsHaveBookMark = "0";
                    }
                    else
                    {
                        item.IsHaveBookMark = "1";
                    }
                    if (string.IsNullOrEmpty(dr["C072"].ToString()))
                    {
                        item.IsHaveMemo = "0";
                    }
                    else
                    {
                        item.IsHaveMemo = "1";
                    }

                    #endregion


                    #region 语音标签判断

                    //====================================================是否有语音标签========================================
                    string _tempSql;
                    OperationReturn _optReturn_ = new OperationReturn();
                    _optReturn_.Result = true;
                    _optReturn_.Code = 0;
                    _optReturn_.Data = 0;
                    switch (session.DBType)
                    {
                        case 2:
                            _tempSql = string.Format("SELECT COUNT(1) FROM T_31_042_{0} WHERE C003={1} AND C002={1} AND C101='1'",
                                session.RentInfo.Token, item.SerialID);
                            _optReturn_ = MssqlOperation.GetRecordCount(session.DBConnectionString, tempSql);
                            break;
                        case 3:
                            _tempSql = string.Format("SELECT COUNT(1) FROM T_31_042_{0} WHERE C003={1} AND C002={1} AND C101='1'",
                                session.RentInfo.Token, item.SerialID);
                            _optReturn_ = OracleOperation.GetRecordCount(session.DBConnectionString, tempSql);
                            break;
                        default:
                            _optReturn_.Result = false;
                            _optReturn_.Code = Defines.RET_PARAM_INVALID;
                            _optReturn_.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                            return _optReturn_;
                    }
                    if (_optReturn_.IntValue != 0)
                    {
                        //有语音标签
                        item.IsHaveVoiceBookMark = "1";
                    }
                    else
                    {
                        //无语音标签
                        item.IsHaveVoiceBookMark = "0";
                    }
                    //==========================================================================================================

                    #endregion


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

        private OperationReturn GetControlOrgInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                //1      上级机构编号（-1表示获取当前所属机构信息）
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strParentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1})"
                           , rentToken
                           , strUserID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2}"
                           , rentToken
                           , strUserID
                           , strParentID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strParentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1})"
                           , rentToken
                           , strUserID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2}"
                           , rentToken
                           , strUserID
                           , strParentID);
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
                    string strID = dr["C001"].ToString();
                    string strName = dr["C002"].ToString();
                    strName = DecryptString02(strName);
                    string strInfo = string.Format("{0}{1}{2}", strName, ConstValue.SPLITER_CHAR, strID);
                    listReturn.Add(strInfo);
                }
                listReturn = listReturn.OrderBy(a => a).ToList();
                optReturn.Data = listReturn;
                optReturn.Message = strSql;
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

        private OperationReturn GetControlAgentInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                //1      所属机构编号
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1030000000000000000 AND C001 < 1040000000000000000"
                                , rentToken
                                , strParentID
                                , strUserID);
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1030000000000000000 AND C001 < 1040000000000000000"
                                , rentToken
                                , strParentID
                                , strUserID);
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
                    string strID = dr["C001"].ToString();
                    string strName = dr["C017"].ToString();
                    string strFullName = dr["C018"].ToString();
                    strName = DecryptString02(strName);
                    strFullName = DecryptString02(strFullName);
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strID, ConstValue.SPLITER_CHAR, strName, strFullName);
                    listReturn.Add(strInfo);
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

        private OperationReturn GetControlExtensionInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                //1      所属机构编号
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1040000000000000000 AND C001 < 1050000000000000000"
                                , rentToken
                                , strParentID
                                , strUserID);
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1040000000000000000 AND C001 < 1050000000000000000"
                                , rentToken
                                , strParentID
                                , strUserID);
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
                    string strID = dr["C001"].ToString();
                    string strName = dr["C017"].ToString();
                    string strFullName = dr["C018"].ToString();
                    strName = DecryptString02(strName);
                    //由于strName是[分机号+char[27]+ip]
                    string[] arrInfo = strName.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2)//问下这里~~查理
                    {
                        continue;
                    }
                    //分机号
                    strName = arrInfo[0];
                    string strIP = arrInfo[1];
                    strFullName = DecryptString02(strFullName);
                    string strInfo = string.Format("{0}{1}{2}{1}{3}{1}{4}", strID, ConstValue.SPLITER_CHAR, strIP, strName, strFullName);
                    listReturn.Add(strInfo);
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

        private OperationReturn GetControlRealExtensionInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                //1      所属机构编号
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1050000000000000000 AND C001 < 1060000000000000000"
                                , rentToken
                                , strParentID
                                , strUserID);
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1050000000000000000 AND C001 < 1060000000000000000"
                                , rentToken
                                , strParentID
                                , strUserID);
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
                    string strID = dr["C001"].ToString();
                    string strName = dr["C017"].ToString();
                    string strFullName = dr["C018"].ToString();
                    strName = DecryptString02(strName);
                    strFullName = DecryptString02(strFullName);
                    //由于strName是[分机号+char[27]+ip]
                    //string[] arrInfo = strName.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    //if (arrInfo.Length < 2)//问下这里~~查理
                    //{
                    //    continue;
                    //}
                    //分机号
                    //strName = arrInfo[0];
                    //string strIP = arrInfo[1];
                    //strFullName = DecryptFromDB(strFullName);
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strID, ConstValue.SPLITER_CHAR, strName, strFullName);
                    listReturn.Add(strInfo);
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

        private OperationReturn GetRecordMemoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      记录编码
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRecordID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_046_{0} WHERE C002 = {1}"
                            , rentToken
                            , strRecordID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_046_{0} WHERE C002 = {1}"
                           , rentToken
                           , strRecordID);
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
                    RecordMemoInfo item = new RecordMemoInfo();
                    item.ID = Convert.ToInt64(dr["C001"]);
                    item.RecordSerialID = Convert.ToInt64(dr["C002"]);
                    item.UserID = Convert.ToInt64(dr["C003"]);
                    item.MemoTime = Converter.NumberToDatetime(dr["C004"].ToString());
                    item.Content = dr["C005"].ToString();
                    item.State = dr["C006"].ToString();
                    item.LastModifyUserID = string.IsNullOrEmpty(dr["C007"].ToString())
                        ? 0
                        : Convert.ToInt64(dr["C007"]);
                    item.LastModifyTime = string.IsNullOrEmpty(dr["C008"].ToString())
                        ? DateTime.MinValue
                        : Converter.NumberToDatetime(dr["C008"].ToString());
                    item.DeleteUserID = string.IsNullOrEmpty(dr["C009"].ToString()) ? 0 : Convert.ToInt64(dr["C009"]);
                    item.DeleteTime = string.IsNullOrEmpty(dr["C010"].ToString())
                        ? DateTime.MinValue
                        : Converter.NumberToDatetime(dr["C010"].ToString());
                    item.Source = dr["C011"].ToString();
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

        //这个是根据选中的录音(包含了坐席信息)来获得评分表的~
        private OperationReturn GetUserScoreSheetList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      记录编码
                //1      坐席ID
                //2      用户ID
                //3      Method  0：获取用户管理的评分表列表，即可用的评分表
                //               1：获取用户已打分的评分表
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }

                string strRecordID = listParams[0];
                string AgentID = listParams[1];//这是坐席工号
                string UserID = listParams[2];
                //string strEncryptAgent = EncryptToDB(strAgent);//把坐席工号加密存入数据库
                string strDateTimeNowUtc = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                string strMethod = listParams[3];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    //=======================================================================================
                    //这里做了个修改  因为是查T_31_001表的C018字段代表的是评分表完整性，所以，我这里就做了个
                    //筛选，在sql语句里面加了个C018='Y'的条件来进行完整性检查                        by 汤澈
                    //=======================================================================================
                    case 2:
                        if (strMethod == "0")
                        {
                            strSql = string.Format("SELECT * FROM T_31_001_{0} WHERE C009<=CONVERT(DATETIME,'{2}',121) AND C010>=CONVERT(DATETIME,'{2}',121) AND C018='Y' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003={1})"
                              , rentToken
                              , AgentID
                              , strDateTimeNowUtc);
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.C001, A.C002, A.C004, B.C001 AS BC001, B.C002 AS BC002, B.C004 AS BC004, B.C005 AS BC005,B.C013 AS BC013, B.C009 AS BC009 ");
                            strSql += string.Format(" FROM T_31_001_{0} A, T_31_008_{0} B ", rentToken);
                            strSql += string.Format(" WHERE A.C001 = B.C003 AND B.C003 IN (SELECT C004 ");
                            strSql += string.Format(" FROM T_11_201_{0}  WHERE C003 ={1}) AND A.C009<=CONVERT(DATETIME,'{2}',121) AND A.C010>=CONVERT(DATETIME,'{2}',121) AND B.C002 = {3}  AND B.C009='Y' AND A.C018='Y'",
                                rentToken,
                                AgentID,
                                strDateTimeNowUtc,
                                strRecordID);
                        }
                        //FileLog.WriteInfo("1", strSql);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strMethod == "0")
                        {
                            strSql = string.Format("SELECT *  FROM T_31_001_{0} WHERE C009<=TO_DATE('{2}','yyyy-mm-dd hh24:mi:ss') AND C010>=TO_DATE('{2}','yyyy-mm-dd hh24:mi:ss') AND C018='Y' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003={1})"
                              , rentToken, AgentID, strDateTimeNowUtc);
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.C001, A.C002, A.C004, B.C001 AS BC001, B.C002 AS BC002, B.C004 AS BC004, B.C005 AS BC005,B.C013 AS BC013, B.C009 AS BC009 ");
                            strSql += string.Format(" FROM T_31_001_{0} A, T_31_008_{0} B ", rentToken);
                            strSql += string.Format(" WHERE A.C001 = B.C003 AND B.C003 IN (SELECT C004 ");
                            strSql += string.Format(" FROM T_11_201_{0} WHERE C003={1}) AND A.C009<=TO_DATE('{2}','yyyy-mm-dd hh24:mi:ss') AND A.C010>=TO_DATE('{2}','yyyy-mm-dd hh24:mi:ss') AND B.C002 = {3} AND B.C009='Y' AND A.C018='Y'",
                                rentToken,
                                AgentID,
                                strDateTimeNowUtc,
                                strRecordID);
                        }
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.Message = strSql;
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
                    BasicScoreSheetInfo item = new BasicScoreSheetInfo();
                    if (strMethod == "0")
                    {
                        item.ScoreSheetID = Convert.ToInt64(dr["C001"]);
                        item.RecordSerialID = Convert.ToInt64(strRecordID);
                        item.UserID = Convert.ToInt64(UserID);
                        item.Title = dr["C002"].ToString();
                        item.TotalScore = Convert.ToDouble(dr["C004"]);
                    }
                    else
                    {
                        item.ScoreSheetID = Convert.ToInt64(dr["C001"]);
                        item.ScoreResultID = Convert.ToInt64(dr["BC001"]);
                        item.RecordSerialID = Convert.ToInt64(dr["BC002"]);
                        item.UserID = Convert.ToInt64(dr["BC005"]);
                        item.AgentID = string.IsNullOrEmpty(dr["BC013"].ToString()) ? 0 : Convert.ToInt64(dr["BC013"]);
                        item.Score = Convert.ToDouble(dr["BC004"]);
                        item.IsFinalScore = dr["BC009"].ToString() == "Y";
                    }
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                    //listReturn.Add(strSql);
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

        //这个是直接获得所有的评分表
        private OperationReturn GetAllScoreSheetList(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_001_{0}", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_001_{0}", rentToken);
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
                    BasicScoreSheetInfo item = new BasicScoreSheetInfo();
                    item.ScoreSheetID = Convert.ToInt64(dr["C001"]);
                    item.Title = dr["C002"].ToString();
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

        private OperationReturn GetScoreSheetInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      评分表编码
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreSheetID = listParams[0];
                string strScoreID = listParams[1];
                optReturn = LoadScoreSheet(session, strScoreSheetID, strScoreID);
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
                optReturn = XMLHelper.SeriallizeObject(scoreSheet);
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

        private OperationReturn GetScoreResultList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      成绩ID
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreResultID = listParams[0];
                if (string.IsNullOrEmpty(strScoreResultID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ScoreResultID param invalid\t{0}", strScoreResultID);
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_009_{0} WHERE C003 = {1}",
                            rentToken,
                            strScoreResultID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_009_{0} WHERE C003 = {1}",
                             rentToken,
                             strScoreResultID);
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
                    BasicScoreItemInfo item = new BasicScoreItemInfo();
                    item.ScoreResultID = Convert.ToInt64(dr["C003"]);
                    item.ScoreSheetID = Convert.ToInt64(dr["C001"]);
                    item.ScoreItemID = Convert.ToInt64(dr["C002"]);
                    item.Score = Convert.ToDouble(dr["C004"]);
                    item.IsNA = dr["C005"].ToString();
                    item.RealScore = Convert.ToDouble(dr["C008"]);
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

        private OperationReturn GetScoreCommentResultList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      成绩ID
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreResultID = listParams[0];
                if (string.IsNullOrEmpty(strScoreResultID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ScoreResultID param invalid\t{0}", strScoreResultID);
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_011_{0} WHERE C001 = {1}",
                            rentToken,
                            strScoreResultID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_011_{0} WHERE C001 = {1}",
                             rentToken,
                             strScoreResultID);
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
                    BasicScoreCommentInfo item = new BasicScoreCommentInfo();
                    item.ScoreResultID = Convert.ToInt64(dr["C001"]);
                    item.ScoreCommentID = Convert.ToInt64(dr["C002"]);
                    item.CommentText = dr["C003"].ToString();
                    item.CommentItemOrderID = Convert.ToInt32(dr["C004"]);
                    item.CommentItemID = Convert.ToInt64(dr["C005"]);
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

        private OperationReturn GetUserSettingList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编码
                //1      参数组编号(可以是多个组，之间用char(27)隔开
                //2      参数编号（如果不为0，表示获取特定一个（或多个【char(27)隔开】）参数）
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strGroupIDList = listParams[1];
                string strParamIDList = listParams[2];
                if (string.IsNullOrEmpty(strUserID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("TempID or QueryID or ConditionItemID is empty");
                    return optReturn;
                }
                string[] listGroupID, listParamID;
                listGroupID = strGroupIDList.Split(new[] { ConstValue.SPLITER_CHAR },
                    StringSplitOptions.RemoveEmptyEntries);
                listParamID = strParamIDList.Split(new[] { ConstValue.SPLITER_CHAR },
                    StringSplitOptions.RemoveEmptyEntries);
                string rentToken = session.RentInfo.Token;
                string strSql;
                string strWhere;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_011_{0} WHERE C001 = {1} ", rentToken, strUserID);
                        strWhere = string.Empty;
                        if (listGroupID.Length > 0 && !string.IsNullOrEmpty(listGroupID[0]) && listGroupID[0] != "0")
                        {
                            strWhere += string.Format("AND C003 IN ({0}) ", GetInfoFromArray(listGroupID, true));
                        }
                        if (listParamID.Length > 0 && !string.IsNullOrEmpty(listParamID[0]) && listParamID[0] != "0")
                        {
                            strWhere += string.Format("AND C002 IN ({0}) ", GetInfoFromArray(listParamID, true));
                        }
                        if (strWhere.Length > 0)
                        {
                            strSql += string.Format("{0}", strWhere);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_011_{0} WHERE C001 = {1} ", rentToken, strUserID);
                        strWhere = string.Empty;
                        if (listGroupID.Length > 0 && !string.IsNullOrEmpty(listGroupID[0]) && listGroupID[0] != "0")
                        {
                            strWhere += string.Format("AND C003 IN ({0}) ", GetInfoFromArray(listGroupID, true));
                        }
                        if (listParamID.Length > 0 && !string.IsNullOrEmpty(listParamID[0]) && listParamID[0] != "0")
                        {
                            strWhere += string.Format("AND C002 IN ({0}) ", GetInfoFromArray(listParamID, true));
                        }
                        if (strWhere.Length > 0)
                        {
                            strSql += string.Format("{0}", strWhere);
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
                    SettingInfo item = new SettingInfo();
                    item.UserID = Convert.ToInt64(dr["C001"]);
                    item.ParamID = Convert.ToInt32(dr["C002"]);
                    item.GroupID = Convert.ToInt32(dr["C003"]);
                    item.SortID = Convert.ToInt32(dr["C004"]);
                    item.StringValue = dr["C005"].ToString();
                    item.DataType = Convert.ToInt32(dr["C006"]);
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

        private OperationReturn GetPlayInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编码
                //1      起始时间
                //2      终止时间
                //3      其他参数
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strBeginTime = listParams[1];
                string strEndTime = listParams[2];
                if (string.IsNullOrEmpty(strUserID) || string.IsNullOrEmpty(strBeginTime) || string.IsNullOrEmpty(strEndTime))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param is empty");
                    return optReturn;
                }
                DateTime dtBegin, dtEnd;
                if (!DateTime.TryParse(strBeginTime, out dtBegin) || !DateTime.TryParse(strEndTime, out dtEnd))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Begin time or end time invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_31_038_{0} WHERE C003 = {1} AND C004 >= '{2}' AND C004 <= '{3}' ORDER BY C004 DESC",
                                rentToken, strUserID, dtBegin, dtEnd);
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
                                 "SELECT * FROM T_31_038_{0} WHERE C003 = {1} AND C004 >= to_date('{2}','YYYY-MM-DD HH24:MI:SS') AND C004 <= to_date('{3}','YYYY-MM-DD HH24:MI:SS') ORDER BY C004 DESC",
                                 rentToken, strUserID, dtBegin, dtEnd);
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
                    RecordPlayInfo item = new RecordPlayInfo();
                    item.SerialID = Convert.ToInt64(dr["C001"]);
                    item.RecordID = Convert.ToInt64(dr["C002"]);
                    item.Player = Convert.ToInt64(dr["C003"]);
                    item.PlayTime = Convert.ToDateTime(dr["C004"]);
                    item.Duration = Convert.ToInt32(dr["C005"]);
                    item.PlayTerminal = Convert.ToInt32(dr["C006"]);
                    item.StartPosition = Convert.ToInt32(dr["C008"]);
                    item.StopPosition = Convert.ToInt32(dr["C009"]);
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

        private OperationReturn GetRecordBookmarkList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码,如果为 0 表示获取所有用户的Bookmark，如果不为 0 表示获取该用户的Bookmark
                //1     记录编号
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strRecordID = listParams[1];
                if (string.IsNullOrEmpty(strUserID) || string.IsNullOrEmpty(strRecordID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param is empty");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strUserID == "0")
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_31_042_{0} WHERE C003 = {1} AND C009 = '1' ORDER BY C001 DESC",
                                    rentToken, strRecordID);
                        }
                        else
                        {
                            strSql =
                               string.Format(
                                   "SELECT * FROM T_31_042_{0} WHERE C003 = {1} AND C010 = {2} AND C009 = '1' ORDER BY C001 DESC",
                                   rentToken, strRecordID, strUserID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strUserID == "0")
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_31_042_{0} WHERE C003 = {1} AND C009 = '1' ORDER BY C001 DESC",
                                    rentToken, strRecordID);
                        }
                        else
                        {
                            strSql =
                               string.Format(
                                   "SELECT * FROM T_31_042_{0} WHERE C003 = {1} AND C010 = {2} AND C009 = '1' ORDER BY C001 DESC",
                                   rentToken, strRecordID, strUserID);
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
                    RecordBookmarkInfo item = new RecordBookmarkInfo();
                    item.SerialID = Convert.ToInt64(dr["C001"]);
                    item.RecordRowID = Convert.ToInt64(dr["C002"]);
                    item.RecordID = Convert.ToInt64(dr["C003"]);
                    item.Offset = Convert.ToInt32(dr["C004"]);
                    item.Duration = Convert.ToInt32(dr["C005"]);
                    item.Title = dr["C006"].ToString();
                    item.Content = dr["C007"].ToString();
                    item.State = dr["C009"].ToString();
                    item.MarkerID = Convert.ToInt64(dr["C010"]);
                    item.MarkTime = Converter.NumberToDatetime(dr["C011"].ToString());
                    item.RankID = Convert.ToInt64(dr["C016"]);
                    item.Teminal = dr["C017"].ToString();
                    item.IsHaveBookMarkRecord = dr["C101"].ToString();
                    if (string.IsNullOrEmpty(item.IsHaveBookMarkRecord))
                    {
                        item.IsHaveBookMarkRecord = "0";
                    }
                    item.BookmarkTimesLength = dr["C102"].ToString();
                    if (string.IsNullOrEmpty(item.BookmarkTimesLength))
                    {
                        item.BookmarkTimesLength = "-1";
                    }
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

        private OperationReturn GetBookmarkRankList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                               string.Format(
                                   "SELECT * FROM T_11_009_{0} WHERE C001 >= 3090000000000000000 and C001 < 3100000000000000000 ORDER BY C002",
                                   rentToken);
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
                                  "SELECT * FROM T_11_009_{0} WHERE C001 >= 3090000000000000000 and C001 < 3100000000000000000 ORDER BY C002",
                                  rentToken);
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
                    BookmarkRankInfo item = new BookmarkRankInfo();
                    item.ID = Convert.ToInt64(dr["C001"]);
                    item.OrderID = Convert.ToInt32(dr["C002"]);
                    item.Name = dr["C009"].ToString();
                    item.Color = DecryptString02(dr["C006"].ToString());
                    item.RankType = dr["C008"].ToString();
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

        private OperationReturn GetSftpServerList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > 2190000000000000000 AND C001 < 2200000000000000000 ORDER BY C001,C002",
                                rentToken);
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
                                "SELECT * FROM T_11_101_{0} WHERE C001 > 2190000000000000000 AND C001 < 2200000000000000000 ORDER BY C001,C002",
                                rentToken);
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
                int intValue;
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    long id = Convert.ToInt64(dr["C001"]);
                    int row = Convert.ToInt32(dr["C002"]);
                    if (row == 1)
                    {
                        string strIP = dr["C017"].ToString();
                        strIP = DecryptResourcePropertyValue(strIP);
                        string strPort = dr["C019"].ToString();
                        strPort = DecryptResourcePropertyValue(strPort);
                        if (int.TryParse(strPort, out intValue))
                        {
                            SftpServerInfo item = new SftpServerInfo();
                            item.ObjID = id;
                            item.HostAddress = strIP;
                            item.HostPort = intValue;
                            optReturn = XMLHelper.SeriallizeObject(item);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            listReturn.Add(optReturn.Data.ToString());
                        }
                    }
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

        private OperationReturn GetDownloadParamList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > 2910000000000000000 AND C001 < 2920000000000000000 ORDER BY C001,C002",
                                rentToken);
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
                                "SELECT * FROM T_11_101_{0} WHERE C001 > 2910000000000000000 AND C001 < 2920000000000000000 ORDER BY C001,C002",
                                rentToken);
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
                int intValue;
                List<DownloadParamInfo> listItems = new List<DownloadParamInfo>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    long objID = Convert.ToInt64(dr["C001"]);
                    var temp = listItems.FirstOrDefault(d => d.ObjID == objID);
                    if (temp == null)
                    {
                        temp = new DownloadParamInfo();
                        temp.ObjID = objID;
                        listItems.Add(temp);
                    }
                    int row = Convert.ToInt32(dr["C002"]);
                    if (row == 1)
                    {
                        string strID = dr["C012"].ToString();
                        if (int.TryParse(strID, out intValue))
                        {
                            temp.ID = intValue;
                        }
                        temp.IsEnabled = dr["C016"].ToString() == "1";
                    }
                    if (row == 2)
                    {
                        string strMethod = dr["C011"].ToString();
                        if (int.TryParse(strMethod, out intValue))
                        {
                            temp.Method = intValue;
                        }
                        string strVoiceID = dr["C012"].ToString();
                        if (int.TryParse(strVoiceID, out intValue))
                        {
                            temp.VoiceID = intValue;
                        }
                        temp.Address = DecryptResourcePropertyValue(dr["C013"].ToString());
                        string strPort = DecryptResourcePropertyValue(dr["C014"].ToString());
                        if (int.TryParse(strPort, out intValue))
                        {
                            temp.Port = intValue;
                        }
                        temp.RootDir = dr["C015"].ToString();
                        temp.VoiceAddress = DecryptResourcePropertyValue(dr["C016"].ToString());
                    }
                    if (row == 3)
                    {
                        string strVocPathFormat = dr["C011"].ToString();
                        temp.VocPathFormat = strVocPathFormat;
                        string strScrPathFormat = dr["C012"].ToString();
                        temp.ScrPathFormat = strScrPathFormat;
                    }
                    if (row == 92)
                    {
                        temp.UserName = DecryptResourcePropertyValue(dr["C011"].ToString());
                        temp.Password = DecryptResourcePropertyValue(dr["C012"].ToString());
                    }
                }
                for (int i = 0; i < listItems.Count; i++)
                {
                    var item = listItems[i];
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

        private OperationReturn GetInspectorList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                //1      所属机构编号
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006={1} AND C001 IN (SELECT C005 FROM T_31_008_{0}) AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})", rentToken, strParentID, strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006={1} AND C001 IN (SELECT C005 FROM T_31_008_{0}) AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})", rentToken, strParentID, strUserID);
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
                    string strInspectorID = dr["C001"].ToString();
                    string strInspectorName = dr["C002"].ToString();
                    string strInspectorFullName = dr["C003"].ToString();
                    strInspectorName = DecryptString02(strInspectorName);
                    strInspectorFullName = DecryptString02(strInspectorFullName);
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strInspectorID, ConstValue.SPLITER_CHAR, strInspectorName, strInspectorFullName);
                    listReturn.Add(strInfo);
                }
                optReturn.Data = listReturn;
                optReturn.Message = strSql;
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

        private OperationReturn GetRecordScoreDetail(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  录音流水号
                //1  坐席ID
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                string RecordSerialID = listParams[0];
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT A.C002,C.C002 AS CC002,C.C003,A.C004,A.C009,B.C002 AS BC002                                                                             FROM T_31_008_{0} A,T_31_001_{0} B,T_11_005_{0} C                                                                                               WHERE  A.C002={1} AND C.C001=A.C005  AND A.C003=B.C001 ORDER BY C002,BC002 ", rentToken, RecordSerialID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT A.C002,C.C002 AS CC002,C.C003,A.C004,A.C009,B.C002 AS BC002                                                                             FROM T_31_008_{0} A,T_31_001_{0} B,T_11_005_{0} C                                                                                               WHERE  A.C002={1} AND C.C001=A.C005  AND A.C003=B.C001 ORDER BY C002,BC002 ", rentToken, RecordSerialID);
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
                List<RecordScoreDetailClass> listItems = new List<RecordScoreDetailClass>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    RecordScoreDetailClass item = new RecordScoreDetailClass();
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strRecordSerialID = dr["C002"].ToString();
                    //质检员账号
                    string strInspectorCount = dr["CC002"].ToString();
                    //质检员全名
                    string strInspectorFullName = dr["C003"].ToString();
                    strInspectorCount = DecryptString02(strInspectorCount);
                    strInspectorFullName = DecryptString02(strInspectorFullName);
                    string strScore = dr["C004"].ToString();
                    string strIsFinalScore = dr["C009"].ToString();
                    string strScoreSheet = dr["BC002"].ToString();

                    item.AgentID = listParams[1];
                    item.RecordSerialID = strRecordSerialID;
                    item.ScoreSheet = strScoreSheet;
                    item.Inspector = string.Format("{0}[{1}]", strInspectorFullName, strInspectorCount);
                    item.Score = strScore;
                    item.IsLastScore = strIsFinalScore;
                    listItems.Add(item);
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < listItems.Count; i++)
                {
                    var item = listItems[i];
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
                optReturn.Message = strSql;
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

        private OperationReturn IsComplainedRecord(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  录音流水号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                string RecordSerialID = listParams[0];
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT COUNT(1) FROM T_31_019_{0} WHERE C003={1} ", rentToken, RecordSerialID);
                        optReturn = MssqlOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT COUNT(1) FROM T_31_019_{0} WHERE C003={1} ", rentToken, RecordSerialID);
                        optReturn = OracleOperation.GetRecordCount(session.DBConnectionString, strSql);
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
                if (optReturn.IntValue != 0)
                {
                    //被申诉过的
                    optReturn.Data = "1";
                }
                else
                {
                    //未被申诉的
                    optReturn.Data = "0";
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

        private OperationReturn IsTaskedRecord(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  录音流水号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                string RecordSerialID = listParams[0];
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT COUNT(1) FROM T_31_022_{0} WHERE C002={1} ", rentToken, RecordSerialID);
                        optReturn = MssqlOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT COUNT(1) FROM T_31_022_{0} WHERE C002={1} ", rentToken, RecordSerialID);
                        optReturn = OracleOperation.GetRecordCount(session.DBConnectionString, strSql);
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
                if (optReturn.IntValue != 0)
                {
                    //被分配过的
                    optReturn.Data = "1";
                }
                else
                {
                    //未被分配的
                    optReturn.Data = "0";
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

        private OperationReturn GetRelativeRecordList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      录音记录信息（RecordInfo）
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRecordInfo = listParams[0];
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
                long serialID = recordInfo.SerialID;
                DateTime startRecordTime = recordInfo.StartRecordTime;
                string rentToken = session.RentInfo.Token;
                DataSet objDataSet;
                int errNum = 0;
                string errMsg = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01", MssqlDataType.Varchar, 5),
                            MssqlOperation.GetDbParameter("@ainparam02", MssqlDataType.Varchar, 20),
                            MssqlOperation.GetDbParameter("@ainparam03", MssqlDataType.Varchar, 20),
                            MssqlOperation.GetDbParameter("@ainparam04", MssqlDataType.Varchar, 2000),
                            MssqlOperation.GetDbParameter("@aouterrornumber", MssqlDataType.Bigint, 0),
                            MssqlOperation.GetDbParameter("@aouterrorstring", MssqlDataType.Varchar, 1024)
                        };
                        mssqlParameters[0].Value = rentToken;
                        mssqlParameters[1].Value = startRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                        mssqlParameters[2].Value = serialID.ToString();
                        mssqlParameters[3].Value = string.Empty;
                        mssqlParameters[4].Value = errNum;
                        mssqlParameters[5].Value = errMsg;
                        mssqlParameters[4].Direction = ParameterDirection.Output;
                        mssqlParameters[5].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.GetDataSetFromStoredProcedure(session.DBConnectionString, "P_21_002",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[4].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[4].Value, mssqlParameters[5].Value);
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        DbParameter[] orcalParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01", OracleDataType.Varchar2, 5),
                            OracleOperation.GetDbParameter("ainparam02", OracleDataType.Varchar2, 20),
                            OracleOperation.GetDbParameter("ainparam03", OracleDataType.Varchar2, 20),
                            OracleOperation.GetDbParameter("ainparam04", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("result",OracleDataType.RefCursor,0),
                            OracleOperation.GetDbParameter("aouterrornumber", OracleDataType.Int32, 0),
                            OracleOperation.GetDbParameter("aouterrorstring", OracleDataType.Varchar2, 1024)
                        };
                        orcalParameters[0].Value = rentToken;
                        orcalParameters[1].Value = startRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                        orcalParameters[2].Value = serialID.ToString();
                        orcalParameters[3].Value = string.Empty;
                        orcalParameters[4].Value = null;
                        orcalParameters[5].Value = errNum;
                        orcalParameters[6].Value = errMsg;
                        orcalParameters[4].Direction = ParameterDirection.Output;
                        orcalParameters[5].Direction = ParameterDirection.Output;
                        orcalParameters[6].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.GetDataSetFromStoredProcedure(session.DBConnectionString, "P_21_002",
                           orcalParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (orcalParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", orcalParameters[5].Value, orcalParameters[6].Value);
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
                    RecordInfo item = new RecordInfo();
                    item.RowID = Convert.ToInt64(dr["C001"]);
                    item.SerialID = Convert.ToInt64(dr["C002"]);
                    item.RecordReference = dr["C077"].ToString();
                    item.StartRecordTime = Convert.ToDateTime(dr["C005"]);
                    item.StopRecordTime = Convert.ToDateTime(dr["C009"]);
                    item.VoiceID = Convert.ToInt32(dr["C037"]);
                    item.VoiceIP = dr["C020"].ToString();
                    item.ChannelID = Convert.ToInt32(dr["C038"]);
                    item.Extension = dr["C042"].ToString();
                    item.Agent = dr["C039"].ToString();
                    item.Duration = Convert.ToInt32(dr["C012"]);
                    item.Direction = dr["C045"].ToString() == "1" ? 1 : 0;
                    item.CallerID = dr["C040"].ToString();
                    item.CalledID = dr["C041"].ToString();
                    item.WaveFormat = dr["C015"].ToString();
                    item.TaskNumber = dr["C104"].ToString();
                    item.MediaType = Convert.ToInt32(dr["C014"]);
                    item.EncryptFlag = dr["C025"].ToString();
                    item.IsaRefID = dr["C109"].ToString();
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

        private OperationReturn GetSkillGroupInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  UserID
                //
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                string UserID = listParams[0];
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C001 IN (SELECT DISTINCT C003 FROM T_11_201_{0} WHERE C003 LIKE '906%' AND C004 IN (SELECT C004 FROM T_11_201_{0}  WHERE C003 ={1} AND C004 LIKE '103%')) ", rentToken, UserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;

                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C001 IN (SELECT DISTINCT C003 FROM T_11_201_{0} WHERE C003 LIKE '906%' AND C004 IN (SELECT C004 FROM T_11_201_{0}  WHERE C003 ={1} AND C004 LIKE '103%')) ", rentToken, UserID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
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
                    optReturn.Message = strSql;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<SkillGroupInfo> tempList = new List<SkillGroupInfo>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    SkillGroupInfo item = new SkillGroupInfo();
                    item.SkillGroupID = dr["C001"].ToString();
                    item.SkillGroupCode = DecryptString02(dr["C006"].ToString());
                    item.SkillGroupName = dr["C008"].ToString();
                    tempList.Add(item);
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < tempList.Count; i++)
                {
                    var item = tempList[i];
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
                optReturn.Message = strSql;
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

        private OperationReturn GetOrgSkillGroupInf(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam 0  大项ID
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                string ParamID = listParams[0];
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT A.*,B.C002 AS BC002 FROM T_31_052_{0} A,T_11_006_{0} B WHERE A.C002={1} AND B.C001=A.C003", rentToken, ParamID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT A.*,B.C002 AS BC002 FROM T_31_052_{0} A,T_11_006_{0} B WHERE A.C002={1} AND B.C001=A.C003 ", rentToken, ParamID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
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
                    optReturn.Message = strSql;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<ABCD_OrgSkillGroup> tempList = new List<ABCD_OrgSkillGroup>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ABCD_OrgSkillGroup item = new ABCD_OrgSkillGroup();
                    item.OrgSkillGroupID = long.Parse(dr["C003"].ToString());
                    item.ParamID = long.Parse(dr["C002"].ToString());
                    item.OrgSkillGroupName = DecryptString02(dr["BC002"].ToString());
                    item.InColumn = int.Parse(dr["C008"].ToString());
                    tempList.Add(item);
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < tempList.Count; i++)
                {
                    var item = tempList[i];
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
                optReturn.Message = strSql;
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

        private OperationReturn GetOrgList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //1      机构编号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strParentID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT C001 FROM T_11_006_{0} WHERE  C004 = {1} "
                            , rentToken
                            , strParentID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT C001 FROM T_11_006_{0} WHERE  C004 = {1} "
                            , rentToken
                            , strParentID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
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
                    optReturn.Message = strSql;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<string> strlist = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    strlist.Add(dr["C001"].ToString());
                }
                optReturn.Data = strlist;
                optReturn.Message = strSql;
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

        private OperationReturn GetAgentList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0,1,2....    机构编号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                //string strUserID = listParams[0];
                string rentToken = session.RentInfo.Token;
                //坐席工号
                List<string> listReturn = new List<string>();
                for (int i = 0; i < listParams.Count; i++)
                {
                    string strParentID = listParams[i];
                    string strSql;
                    DataSet objDataSet;
                    switch (session.DBType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 >= 1030000000000000000 AND C001 < 1040000000000000000"
                                    , rentToken
                                    , strParentID);
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
                                    "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 >= 1030000000000000000 AND C001 < 1040000000000000000"
                                    , rentToken
                                    , strParentID);
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
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];
                        //string strID = dr["C001"].ToString();
                        string strName = dr["C017"].ToString();
                        string strFullName = dr["C018"].ToString();
                        strName = DecryptString02(strName);
                        strFullName = DecryptString02(strFullName);
                        //string strInfo = string.Format("{0}{1}{2}{1}{3}", strID, ConstValue.SPLITER_CHAR, strName, strFullName);
                        listReturn.Add(strName);
                    }
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

        private OperationReturn GetExtensionList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0,1,2....     所属机构编号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                //坐席工号list
                List<string> listReturn = new List<string>();
                string rentToken = session.RentInfo.Token;
                for (int i = 0; i < listParams.Count; i++)
                {
                    string strParentID = listParams[i];
                    string strSql;
                    DataSet objDataSet;
                    switch (session.DBType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}'  AND C001 >= 1040000000000000000 AND C001 < 1050000000000000000"
                                    , rentToken
                                    , strParentID);
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
                                    "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND  C001 >= 1040000000000000000 AND C001 < 1050000000000000000"
                                    , rentToken
                                    , strParentID);
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
                    //List<string> listReturn = new List<string>();
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];
                        string strID = dr["C001"].ToString();
                        string strName = dr["C017"].ToString();
                        string strFullName = dr["C018"].ToString();
                        strName = DecryptString02(strName);
                        //由于strName是[分机号+char[27]+ip]
                        string[] arrInfo = strName.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrInfo.Length < 2)//问下这里~~查理
                        {
                            continue;
                        }
                        //分机号
                        strName = arrInfo[0];
                        listReturn.Add(strName);
                    }
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

        //执行一个sql语句
        private OperationReturn ExecuteStrSql(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0 要执行的sql语句 
                string strSql = listParams[0];
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                DataSet objDataSet;
                optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                objDataSet = optReturn.Data as DataSet;
                for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[j];
                    string temp = dr["C013"].ToString();//temp 就是从数据拿到的字段 ,这里是临时表里面的C013,也就是存得管理对象
                    listReturn.Add(temp);
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

        //获取教材库文件夹
        private OperationReturn GetLibraryFolder(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      上级文件夹编号（-1表示获取当前文件夹信息）
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strParentID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strParentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_31_058_{0} WHERE C002 = '{1}'"
                           , rentToken
                           , strParentID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_31_058_{0} WHERE C002 = '{1}'"
                           , rentToken
                           , strParentID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strParentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_31_058_{0} WHERE C002 = '{1}'"
                           , rentToken
                           , strParentID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_31_058_{0} WHERE C002 = '{1}'"
                           , rentToken
                           , strParentID);
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
                    //教材库文件夹ID
                    string strFolderID = dr["C001"].ToString();
                    //教材库文件夹父级节点ID
                    string strParentFolderID = dr["C002"].ToString();
                    //教材库文件夹名称
                    string strFolderName = dr["C003"].ToString();
                    //教材库文件夹的父级文件夹名称
                    string strParentFolderName = dr["C004"].ToString();
                    if (string.IsNullOrEmpty(strParentFolderName))
                    {
                        strParentFolderName = " ";
                    }
                    string strInfo = string.Format("{0}{1}{2}{1}{3}{1}{4}", strFolderID, ConstValue.SPLITER_CHAR, strParentFolderID, strFolderName, strParentFolderName);
                    listReturn.Add(strInfo);
                }
                optReturn.Data = listReturn;
                optReturn.Message = strSql;
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

        //获取教材库文件夹下的内容
        private OperationReturn GetLibraryFolderContent(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      文件夹编号（-1表示获取当前文件夹信息）
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strFoldID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_060_{0} WHERE C002 = '{1}'"
                       , rentToken
                       , strFoldID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_060_{0} WHERE C002 = '{1}'"
                       , rentToken
                       , strFoldID);
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
                //List<LibraryContent> listLibraryContent = new List<LibraryContent>();
                //for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                //{
                //    DataRow dr = objDataSet.Tables[0].Rows[i];
                //    LibraryContent item = new LibraryContent();
                //    item.BookID = dr["C001"].ToString();
                //    item.DirID = dr["C002"].ToString();
                //    item.BookName = dr["C003"].ToString();
                //    item.Path = dr["C004"].ToString();
                //    item.Describle = dr["C005"].ToString();
                //    item.FromType = dr["C006"].ToString();
                //    item.IsEncrytp = dr["C007"].ToString();
                //    item.IsMedia = dr["C008"].ToString();
                //    listLibraryContent.Add(item);
                //}
                //for (int i = 0; i < listLibraryContent.Count; i++)
                //{
                //    var item = listLibraryContent[i];
                //    optReturn = XMLHelper.SeriallizeObject(item);
                //    if (!optReturn.Result)
                //    {
                //        return optReturn;
                //    }
                //    listReturn.Add(optReturn.Data.ToString());
                //}
                optReturn.Data = listReturn;
                optReturn.Message = strSql;
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

        private OperationReturn GetConversationInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      RowID 就是  T_21_001的C001 和  T_51_002的C002
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRowID = listParams[0];
                //string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_51_002 WHERE C003='{0}' AND C004 = 'U'", strRowID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_51_002 WHERE C003='{0}' AND C004 = 'U'", strRowID);
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
                    optReturn.Message = strSql;
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                List<ConversationInfo> listConversationInfo = new List<ConversationInfo>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ConversationInfo item = new ConversationInfo();
                    item.SerialID = dr["C002"].ToString();
                    item.RecordReference = dr["C003"].ToString();
                    //item.StartRecordTime = Converter.NumberToDatetime(dr["C005"].ToString());
                    //item.EndRecordTime = Converter.NumberToDatetime(dr["C006"].ToString());
                    item.Offset = Convert.ToInt64(dr["C007"].ToString());
                    item.Extension = dr["C011"].ToString();
                    item.Direction = dr["C014"].ToString();
                    item.RowNumber = dr["C101"].ToString();
                    item.Content = dr["C102"].ToString();
                    listConversationInfo.Add(item);
                }
                for (int i = 0; i < listConversationInfo.Count; i++)
                {
                    var item = listConversationInfo[i];
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
                optReturn.Message = strSql;
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

        private OperationReturn GetAutoScore(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                OperationReturn optReturn_ = new OperationReturn();
                optReturn_.Result = true;
                optReturn_.Code = 0;
                //listParams
                //0    录音流水号
                //1... StatisticalID(自动评分的统计ID)，对应在T_31_052的C001
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn_.Result = false;
                    optReturn_.Code = Defines.RET_PARAM_INVALID;
                    optReturn_.Message = string.Format("Request param is null or count invalid");
                    return optReturn_;
                }
                string strSerialID = listParams[0];
                //这个是存放StatisticalID的
                List<string> thisListParams = new List<string>();
                for (int i = 0; i < listParams.Count - 1; i++)
                {
                    thisListParams.Add(listParams[i + 1]);
                }
                //string strAgentID = listParams[1];
                //string enAgentID = EncryptToDB(strAgentID);
                string rentToken = session.RentInfo.Token;

                string strSql_;
                DataSet objDataSet_;
                List<string> allValues = new List<string>();
                for (int i = 0; i < thisListParams.Count; i++)
                {
                    switch (session.DBType)
                    {
                        case 2:
                            strSql_ = string.Format("SELECT C008 FROM T_31_052_{0} WHERE C001 = {1}", rentToken, thisListParams[i]);
                            optReturn_ = MssqlOperation.GetDataSet(session.DBConnectionString, strSql_);
                            if (!optReturn_.Result)
                            {
                                return optReturn_;
                            }
                            objDataSet_ = optReturn_.Data as DataSet;
                            break;
                        case 3:
                            strSql_ = string.Format("SELECT C008 FROM T_31_052_{0} WHERE C001 = {1}", rentToken, thisListParams[i]);
                            optReturn_ = OracleOperation.GetDataSet(session.DBConnectionString, strSql_);
                            if (!optReturn_.Result)
                            {
                                return optReturn_;
                            }
                            objDataSet_ = optReturn_.Data as DataSet;
                            break;
                        default:
                            optReturn_.Result = false;
                            optReturn_.Code = Defines.RET_PARAM_INVALID;
                            optReturn_.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                            return optReturn_;
                    }
                    if (objDataSet_ == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = strSql_;
                        return optReturn;
                    }
                    string Column = "";
                    for (int m = 0; m < objDataSet_.Tables[0].Rows.Count; m++)
                    {
                        DataRow dr = objDataSet_.Tables[0].Rows[m];
                        Column = dr["C008"].ToString();
                    }

                    //做下判断  如果拿不到Column  那么就不执行下面的东西  先空着

                    #region 从T_31_054拿数据
                    //先拼列号
                    string tempColum = string.Format("C{0}", int.Parse(Column.ToString()).ToString("000"));
                    //再根据录音流水号和分表参数来拿T_31_054的分表或者不分的表
                    string queryStateInfo_TableName;

                    var tableInfo =
                       session.ListPartitionTables.FirstOrDefault(
                           t => t.TableName == ConstValue.TABLE_NAME_STATISTICS && t.PartType == TablePartType.DatetimeRange);
                    if (tableInfo == null)
                    {
                        tableInfo =
                       session.ListPartitionTables.FirstOrDefault(
                            t => t.TableName == ConstValue.TABLE_NAME_STATISTICS && t.PartType == TablePartType.VoiceID);
                        if (tableInfo == null)
                        {
                            queryStateInfo_TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_STATISTICS,
                                session.RentInfo.Token);
                        }
                        else
                        {
                            //按录音服务器查询,没有实现，暂时还是按普通方式来
                            queryStateInfo_TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_STATISTICS,
                                session.RentInfo.Token);
                        }
                    }
                    else
                    {
                        //按月分表
                        string serialID_ = strSerialID;
                        string tempType = serialID_.Substring(0, 4);
                        queryStateInfo_TableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_STATISTICS,
                               session.RentInfo.Token, tempType);

                    }
                    string strSql;
                    DataSet objDataSet;
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("SELECT {0} FROM {1} WHERE C201={2}", tempColum, queryStateInfo_TableName, strSerialID);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql = string.Format("SELECT {0} FROM {1} WHERE C201={2}", tempColum, queryStateInfo_TableName, strSerialID);
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
                    string ThisValues = "";
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];
                        ThisValues = dr[tempColum].ToString() + ConstValue.SPLITER_CHAR + thisListParams[i];
                    }
                    if (string.IsNullOrWhiteSpace(ThisValues))
                    {
                        ThisValues = "N/A" + ConstValue.SPLITER_CHAR + thisListParams[i];
                    }
                    //存放的是值以及T_31_052的C001（后者的目的是为了区别是哪个自动评分项）
                    allValues.Add(ThisValues);
                    optReturn.Message = strSql;
                    #endregion
                }
                optReturn.Data = allValues;
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

        //serialID是录音流水号,col是列号
        //private void SeT_31_052(string serialID, string col, SessionInfo session)
        //{
        //    OperationReturn optReturn = new OperationReturn();
        //    optReturn.Result = true;
        //    optReturn.Code = 0;
        //    try
        //    {
        //        //先拼列号
        //        string tempColum = string.Format("C{0}", int.Parse(col.ToString()).ToString("000"));
        //        //再根据录音流水号
        //        string queryStateInfo_TableName;

        //        var tableInfo =
        //           session.ListPartitionTables.FirstOrDefault(
        //               t => t.TableName == ConstValue.TABLE_NAME_STATISTICS && t.PartType == TablePartType.DatetimeRange);
        //        if (tableInfo == null)
        //        {
        //            tableInfo =
        //           session.ListPartitionTables.FirstOrDefault(
        //                t => t.TableName == ConstValue.TABLE_NAME_STATISTICS && t.PartType == TablePartType.VoiceID);
        //            if (tableInfo == null)
        //            {
        //                queryStateInfo_TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_STATISTICS,
        //                    session.RentInfo.Token);
        //            }
        //            else
        //            {
        //                //按录音服务器查询,没有实现，暂时还是按普通方式来
        //                queryStateInfo_TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_STATISTICS,
        //                    session.RentInfo.Token);
        //            }
        //        }
        //        else
        //        {
        //            //按月分表
        //            string serialID_ = serialID;
        //            string tempType = serialID_.Substring(0, 4);
        //            queryStateInfo_TableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_STATISTICS,
        //                   session.RentInfo.Token, tempType);

        //        }

        //        string strSql;
        //        DataSet objDataSet;
        //        switch (session.DBType)
        //        {
        //            case 2:
        //                strSql = string.Format("SELECT {0} FROM {1} WHERE C201={2}", tempColum, queryStateInfo_TableName, serialID);
        //                optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
        //                if (!optReturn.Result)
        //                {
        //                    return optReturn;
        //                }
        //                objDataSet = optReturn.Data as DataSet;
        //                break;
        //            case 3:
        //                strSql = string.Format("SELECT {0} FROM {1} WHERE C201={2}", tempColum, queryStateInfo_TableName, serialID);
        //                optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
        //                if (!optReturn.Result)
        //                {
        //                    return optReturn;
        //                }
        //                objDataSet = optReturn.Data as DataSet;
        //                break;
        //            default:
        //                break;
        //            //optReturn.Result = false;
        //            //optReturn.Code = Defines.RET_PARAM_INVALID;
        //            //optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
        //            //return optReturn;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //}

        private OperationReturn GetKeyWordsInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT A.C001 AS AC001, A.C002 AS AC002, A.C003 AS AC003, B.C001 AS BC001, B.C002 AS BC002 FROM T_51_006_{0} A, T_51_007_{0} B, T_51_008_{0} C WHERE A.C001 = C.C002 AND B.C001 = C.C001 AND A.C005  = '1' AND B.C003 = '1' AND C.C003 = '1'",
                                rentToken);
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
                               "SELECT A.C001 AS AC001, A.C002 AS AC002, A.C003 AS AC003, B.C001 AS BC001, B.C002 AS BC002 FROM T_51_006_{0} A, T_51_007_{0} B, T_51_008_{0} C WHERE A.C001 = C.C002 AND B.C001 = C.C001 AND A.C005  = '1' AND B.C003 = '1' AND C.C003 = '1'",
                               rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
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
                    optReturn.Message = strSql;
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    KeywordInfo info = new KeywordInfo();
                    info.SerialNo = Convert.ToInt64(dr["AC001"]);
                    info.ContentNo = Convert.ToInt64(dr["BC001"]);
                    string strName = dr["AC002"].ToString();
                    strName = DecryptString02(strName);
                    info.Name = strName;
                    info.State = 0;     //0：正常；1：删除；2：禁用
                    info.Icon = dr["AC003"].ToString();
                    string strContent = dr["BC002"].ToString();
                    strContent = DecryptString02(strContent);
                    info.Content = strContent;
                    optReturn = XMLHelper.SeriallizeObject(info);
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

        private OperationReturn GetKeywordResultList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0         RecordSerialNo 录音流水号(C002)
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strSerialNo = listParams[0];
                string rentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConn = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_51_009_{0} WHERE C002 = {1} ORDER BY C002, C005",
                            rentToken, strSerialNo);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_51_009_{0} WHERE C002 = {1} ORDER BY C002, C005",
                           rentToken, strSerialNo);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid.");
                        return optReturn;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DataSet objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null.");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    KeywordResultInfo info = new KeywordResultInfo();
                    info.RecordNumber = Convert.ToInt64(dr["C001"]);
                    info.RecordSerialID = Convert.ToInt64(dr["C002"]);
                    info.RecordReference = dr["C003"].ToString();
                    info.Offset = Convert.ToInt32(dr["C005"]);
                    info.KeywordName = dr["C007"].ToString();
                    info.KeywordContent = dr["C008"].ToString();
                    info.KeywordNo = Convert.ToInt64(dr["C009"]);
                    info.ContentNo = Convert.ToInt64(dr["C010"]);
                    info.Agent = dr["C014"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private string DecryptResourcePropertyValue(string source)
        {
            string strReturn = source;
            if (source.StartsWith(string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR)))
            {
                string strContent = source.Substring(3);
                string[] arrContent = strContent.Split(new[] { ConstValue.SPLITER_CHAR },
                    StringSplitOptions.RemoveEmptyEntries);
                string strVersion = string.Empty, strMode = string.Empty, strPass = string.Empty;
                if (arrContent.Length > 0)
                {
                    strVersion = arrContent[0];
                }
                if (arrContent.Length > 1)
                {
                    strMode = arrContent[1];
                }
                if (arrContent.Length > 2)
                {
                    strPass = arrContent[2];
                }
                strReturn = strPass;
                if (strVersion == "2" && strMode == "hex")
                {
                    strReturn = DecryptString02(strPass);
                }
            }
            return strReturn;
        }

    }
}