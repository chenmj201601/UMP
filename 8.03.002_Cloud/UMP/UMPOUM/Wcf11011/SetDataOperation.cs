using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace Wcf11011
{
    public partial class Service11011
    {
        private OperationReturn MoveUser(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string orgID, strUserID;
                //0     OrgID
                //1     UserIDs
                orgID = listParams[0];
                strUserID = listParams[1];
                if (string.IsNullOrEmpty(orgID) || string.IsNullOrEmpty(strUserID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param invalid");
                    return optReturn;
                }
                string[] listUserID =
                    strUserID.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                string tempString = string.Empty;
                for (int i = 0; i < listUserID.Length; i++)
                {
                    tempString += listUserID[i] + ",";
                }
                tempString = tempString.TrimEnd(new[] { ',' });

                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("UPDATE T_11_005_{0} SET C006 = {1} WHERE C001 IN ( {2} )"
                           , rentToken
                           , orgID, tempString);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        return optReturn;
                    //ORCL
                    case 3:
                        strSql = string.Format("UPDATE T_11_005_{0} SET C006 = {1} WHERE C001 IN ( {2} )"
                            , rentToken
                            , orgID, tempString);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        return optReturn;
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

        private OperationReturn MoveAgent(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string orgID, strUserID;
                //0     OrgID
                //1     UserIDs
                orgID = listParams[0];
                strUserID = listParams[1];
                if (string.IsNullOrEmpty(orgID) || string.IsNullOrEmpty(strUserID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param invalid");
                    return optReturn;
                }
                string[] listUserID =
                    strUserID.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                string tempString = string.Empty;
                for (int i = 0; i < listUserID.Length; i++)
                {
                    tempString += listUserID[i] + ",";
                }
                tempString = tempString.TrimEnd(new[] { ',' });

                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("UPDATE T_11_101_{0} SET C011 = {1} WHERE C001 IN ( {2} ) AND C002=1"
                           , rentToken
                           , orgID, tempString);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        return optReturn;
                    //ORCL
                    case 3:
                        strSql = string.Format("UPDATE T_11_101_{0} SET C011 = {1} WHERE C001 IN ( {2} ) AND C002=1"
                            , rentToken
                            , orgID, tempString);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        return optReturn;
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

        private OperationReturn SetUserRoles(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID, strCount;
                int intCount;
                //0     userID
                //1     count
                //...   role check state
                strUserID = listParams[0];
                strCount = listParams[1];
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RoleCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Role count invalid");
                    return optReturn;
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
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1} AND C003 LIKE '106%'"
                            , rentToken
                            , strUserID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1} AND C003 LIKE '106%'"
                            , rentToken
                            , strUserID);
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
                    for (int i = 2; i < listParams.Count; i++)
                    {
                        string roleState = listParams[i];
                        string[] listRoleState = roleState.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.RemoveEmptyEntries);
                        if (listRoleState.Length < 2)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("ListRoleState invalid");
                            break;
                        }
                        string roleID = listRoleState[0];
                        string isChecked = listRoleState[1];
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("c003 = {0}", roleID));
                        if (isChecked == "1")
                        {
                            //不存在，则插入
                            if (drs.Length <= 0)
                            {
                                DataRow newRow = objDataSet.Tables[0].NewRow();
                                newRow["C001"] = 0;
                                newRow["C002"] = 0;
                                newRow["C003"] = Convert.ToInt64(roleID);
                                newRow["C004"] = Convert.ToInt64(strUserID);
                                newRow["C005"] = DateTime.Now;
                                newRow["C006"] = DateTime.MaxValue;
                                objDataSet.Tables[0].Rows.Add(newRow.ItemArray);

                                listMsg.Add(string.Format("{0}{1}{2}", "A", ConstValue.SPLITER_CHAR, roleID));
                            }
                        }
                        else
                        {
                            //存在，则移除
                            if (drs.Length > 0)
                            {
                                for (int j = drs.Length - 1; j >= 0; j--)
                                {
                                    drs[j].Delete();

                                    listMsg.Add(string.Format("{0}{1}{2}", "D", ConstValue.SPLITER_CHAR, roleID));
                                }
                            }
                        }
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                    optReturn.Data = listMsg;

                    //初始化用户权限
                    List<string> temp = new List<string>();
                    temp.Add(strUserID);
                    temp.Add(session.UserInfo.UserID.ToString());
                    OperationReturn tempReturn = UpdateUserPermission(session, temp);
                    if (!tempReturn.Result)
                    {
                        return tempReturn;
                    }
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

        private OperationReturn SetUserControlObject(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID, strCount;
                int intCount;
                //0     userID
                //1     count
                //...   object check state
                strUserID = listParams[0];
                strCount = listParams[1];
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ObjectCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Object count invalid");
                    return optReturn;
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
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C001 = 0 AND C003 = {1}"
                            , rentToken
                            , strUserID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C001 = 0 AND C003 = {1}"
                            , rentToken
                            , strUserID);
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
                    for (int i = 2; i < listParams.Count; i++)
                    {
                        string objectState = listParams[i];
                        string[] listObjectState = objectState.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.RemoveEmptyEntries);
                        if (listObjectState.Length < 2)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("ListObjectState invalid");
                            break;
                        }
                        string objID = listObjectState[0];
                        string isChecked = listObjectState[1];
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C004 = {0}", objID));
                        if (isChecked == "1")
                        {
                            //不存在，则插入
                            if (drs.Length <= 0)
                            {
                                DataRow newRow = objDataSet.Tables[0].NewRow();
                                newRow["C001"] = 0;
                                newRow["C002"] = 0;
                                newRow["C003"] = Convert.ToInt64(strUserID);
                                newRow["C004"] = Convert.ToInt64(objID);
                                newRow["C005"] = DateTime.Now;
                                newRow["C006"] = DateTime.MaxValue;
                                objDataSet.Tables[0].Rows.Add(newRow.ItemArray);

                                listMsg.Add(string.Format("{0}{1}{2}", "A", ConstValue.SPLITER_CHAR, objID));
                            }
                        }
                        else
                        {
                            //存在，则移除
                            if (drs.Length > 0)
                            {
                                for (int j = drs.Length - 1; j >= 0; j--)
                                {
                                    drs[j].Delete();

                                    listMsg.Add(string.Format("{0}{1}{2}", "D", ConstValue.SPLITER_CHAR, objID));
                                }
                            }
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

        private OperationReturn UpdateUserPermission(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int errNum = 0;
            string errMsg = string.Empty;
            try
            {
                if (listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strOperatorID = listParams[1];
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.Varchar,5 ),
                            MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.Varchar,200)
                        };
                        mssqlParameters[0].Value = session.RentInfo.Token;
                        mssqlParameters[1].Value = strUserID;
                        mssqlParameters[2].Value = strOperatorID;
                        mssqlParameters[3].Value = errNum;
                        mssqlParameters[4].Value = errMsg;
                        mssqlParameters[3].Direction = ParameterDirection.Output;
                        mssqlParameters[4].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_016",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[3].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[3].Value, mssqlParameters[4].Value);
                        }
                        break;
                    //ORCL
                    case 3:
                        DbParameter[] orclParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01",OracleDataType.Varchar2,5 ),
                            OracleOperation.GetDbParameter("ainparam02",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam03",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("errornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("errorstring",OracleDataType.Varchar2,200)
                        };
                        orclParameters[0].Value = session.RentInfo.Token;
                        orclParameters[1].Value = strUserID;
                        orclParameters[2].Value = strOperatorID;
                        orclParameters[3].Value = errNum;
                        orclParameters[4].Value = errMsg;
                        orclParameters[3].Direction = ParameterDirection.Output;
                        orclParameters[4].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_016",
                            orclParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (orclParameters[3].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", orclParameters[3].Value, orclParameters[4].Value);
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
                        return optReturn;
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

    }
}