using System;
using System.Collections.Generic;
using System.Data;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using VoiceCyber.UMP.Encryptions;

namespace Wcf11011
{
    public partial class Service11011
    {
        private OperationReturn ModifyOrgInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count != 6)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                //0     OrgID
                //1     OrgName
                //2     OrgType
                //3     OrgDesc
                //4     IsActive
                //5     ObjParentID
                string orgID = listParams[0];
                string orgName = EncryptToDB(listParams[1]);
                string orgType = listParams[2];
                string orgDesc = listParams[3];
                string IsActive = listParams[4];
                string rentToken = session.RentInfo.Token;
                string orgParentId = listParams[5];
                string strSql;
                //判断同级父机构下是否有同名的机构
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DataSet objDataSet = new DataSet();
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT  * FROM T_11_006_{0} WHERE  C002='{1}' AND C004='{2}' AND  C001<>{3} ", 
                            rentToken,
                            orgName,
                            orgParentId,
                            orgID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT  * FROM T_11_006_{0} WHERE  C002='{1}' AND C004='{2}' AND  C001 <>{3} ", 
                            rentToken,
                            orgName,
                            orgParentId,
                            orgID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        break;
                }
                if (objConn == null
                  || objAdapter == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DBConnection or DBDataAdapter is null");
                    return optReturn;
                }
                objAdapter.Fill(objDataSet);
                if (objDataSet != null && objDataSet.Tables[0].Rows.Count > 0)
                {
                    //同名机构名称
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_EXIST;
                    optReturn.Message = string.Format("EXIST");
                    return optReturn;
                }

                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql =
                           string.Format(
                               "UPDATE T_11_006_{0} SET C002 = '{1}', C003 = {2}, C012 = '{3}',C005='{5}' WHERE C001 = {4}",
                               rentToken,
                               orgName,
                               orgType,
                               orgDesc,
                               orgID,
                               IsActive);
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
                                "UPDATE T_11_006_{0} SET C002 = '{1}', C003 = {2}, C012 = '{3}' ,C005='{5}'  WHERE C001 = {4}",
                                rentToken,
                                orgName,
                                orgType,
                                orgDesc,
                                orgID,
                                IsActive
                                );
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
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

        private OperationReturn ModifyUserPassword(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strID, method, password;
                //0         UserID
                //1         Method（0：使用默认密码；1：使用指定的密码）
                //2         密码
                strID = listParams[0];
                method = listParams[1];
                password = listParams[2];
                string rentToken = session.RentInfo.Token;
                string passToDB;
                string strSql;
                DataSet objDataSet = null;

                if (method == "0")
                {
                    //从全局参数表查得默认密码
                    switch (session.DBType)
                    {
                        case 2:
                            strSql =
                               string.Format("SELECT C006 FROM T_11_001_{0} WHERE C002 = 11 AND C003 = {1}"
                                   , rentToken
                                   , S1101Consts.PARAM_DEFAULT_PASSWORD);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                               string.Format("SELECT C006 FROM T_11_001_{0} WHERE C002 = 11 AND C003 = {1}"
                                   , rentToken
                                   , S1101Consts.PARAM_DEFAULT_PASSWORD);
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                    }
                    if (objDataSet == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("DataSet is null");
                        return optReturn;
                    }
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_DBACCESS_NOT_EXIST;
                        optReturn.Message = string.Format("Globle param not exist.");
                        return optReturn;
                    }
                    string defaultPass = objDataSet.Tables[0].Rows[0]["C006"].ToString();
                    defaultPass = DecryptFromDB(defaultPass);
                    defaultPass = defaultPass.Substring(8);
                    passToDB = EncryptShaToDB(strID + defaultPass);
                }
                else
                {
                    passToDB = EncryptShaToDB(strID + password);
                }
                if (string.IsNullOrEmpty(passToDB))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("Password to database is empty");
                    return optReturn;
                }
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                         string.Format(
                             "UPDATE T_11_005_{0} SET C004 = '{1}', C023 = '{2}' WHERE C001 = {3}",
                             rentToken,
                             passToDB,
                             DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                             strID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql =
                          string.Format(
                              "UPDATE T_11_005_{0} SET C004 = '{1}', C023 = TO_DATE('{2}','YYYY-MM-DD HH24:MI:SS') WHERE C001 = {3}",
                              rentToken,
                              passToDB,
                              DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss"),
                              strID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
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

        private OperationReturn ModifyUserPasswordM003(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strID, method, password;
                //0         UserID
                //1         Method（0：使用默认密码；1：使用指定的密码；2：特殊情况，19位用户编号+默认密码）
                //2         密码
                strID = listParams[0];
                method = listParams[1];
                password = listParams[2];
                string rentToken = session.RentInfo.Token;
                string passToDB;
                string strSql;
                DataSet objDataSet = null;
                EncryptionMode mode = (EncryptionMode)Enum.Parse(typeof(EncryptionMode), "AES256V03Hex");
                if (method == "0")
                {
                    //从全局参数表查得默认密码
                    switch (session.DBType)
                    {
                        case 2:
                            strSql =
                               string.Format("SELECT C006 FROM T_11_001_{0} WHERE C002 = 11 AND C003 = {1}"
                                   , rentToken
                                   , S1101Consts.PARAM_DEFAULT_PASSWORD);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                               string.Format("SELECT C006 FROM T_11_001_{0} WHERE C002 = 11 AND C003 = {1}"
                                   , rentToken
                                   , S1101Consts.PARAM_DEFAULT_PASSWORD);
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                    }
                    if (objDataSet == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("DataSet is null");
                        return optReturn;
                    }
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_DBACCESS_NOT_EXIST;
                        optReturn.Message = string.Format("Globle param not exist.");
                        return optReturn;
                    }
                    string defaultPass = objDataSet.Tables[0].Rows[0]["C006"].ToString();
                    defaultPass = DecryptFromDB(defaultPass);
                    defaultPass = defaultPass.Substring(8);
                    passToDB = ServerAESEncryption.EncryptString(strID + defaultPass, mode);
                }
                else
                {
                    passToDB = ServerAESEncryption.EncryptString(strID + password, mode);
                }
                if (string.IsNullOrEmpty(passToDB))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("Password to database is empty");
                    return optReturn;
                }
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                         string.Format(
                             "UPDATE T_11_005_{0} SET C004 = '{1}', C023 = '{2}' WHERE C001 = {3}",
                             rentToken,
                             passToDB,
                             DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"),
                             strID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql =
                          string.Format(
                              "UPDATE T_11_005_{0} SET C004 = '{1}', C023 = TO_DATE('{2}','YYYY-MM-DD HH24:MI:SS') WHERE C001 = {3}",
                              rentToken,
                              passToDB,
                              DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss"),
                              strID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
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

        private OperationReturn ModifyUserInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 8)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strID, account, fullName, strLock, strReset, IsActive, validTime, invalidTime, IsOrgManagement;
                //0     UserID
                //1     Account
                //2     FullName
                //3     Lock ( C 解除锁定, U   界面锁定,D 老杨禁用）
                //4     ResetPassword
                //5    IsAcitve
                //6     ValidTime
                //7     InValidTime
                //8     IsOrgManagement
                ////9     Org
                strID = listParams[0];
                account = listParams[1];
                fullName = listParams[2];
                strLock = listParams[3];
                strReset = listParams[4];
                IsActive = listParams[5];
                validTime = listParams[6];
                invalidTime = listParams[7];
                //IsOrgManagement = listParams[8];
                //Org = listParams[9];

                account = EncryptToDB(account);
                fullName = EncryptToDB(fullName);
                validTime = EncryptToDB(validTime);
                invalidTime = EncryptToDB(invalidTime);

                string rentToken = session.RentInfo.Token;
                string strSql;
                int count;
                string strLockSql;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT COUNT(1) FROM T_11_005_{0} WHERE C001 <> {1} AND C002 = '{2}'"
                           , rentToken
                           , strID
                           , account);
                        optReturn = MssqlOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        count = Convert.ToInt32(optReturn.Data);
                        //账户名已经存在
                        if (count > 0)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_EXIST;
                            optReturn.Message = string.Format("User account already exist.");
                            return optReturn;
                        }
                        strLockSql = string.Empty;
                        //if (strLock == "C")
                        //{
                        //    strLockSql = string.Format(", C008 = '0', C009 = 'N'");
                        //}
                        //else if (strLock == "U")
                        //{
                        //    strLockSql = string.Format(", C008 = '1', C009 = 'U'");
                        //}
                        if (strLock == "CL") 
                        {
                            strLockSql = string.Format(", C008 = '0', C009 = 'N' ,C024 = 0 ");
                        }
                        else if (strLock == "CU") 
                        {
                            strLockSql = string.Format(", C008 = '0', C009 = 'N'");
                        }
                        else if (strLock == "CD")
                        {
                            strLockSql = string.Format(", C008 = '0', C009 = 'N', C013 = '{0}'", EncryptToDB(DateTime.Now.ToString()));
                        }
                        else if (strLock == "U")
                        {
                            strLockSql = string.Format(", C008 = '1', C009 = 'U'");
                        }

                        strSql =
                            string.Format(
                                "UPDATE T_11_005_{0} SET C002 = '{1}', C003 = '{2}'{3} ,C010='{5}' ,C017='{6}',C018='{7}',C027='{8}' WHERE C001 = {4}",
                                rentToken,
                                account,
                                fullName,
                                strLockSql,
                                strID,
                                IsActive,
                                validTime,
                                invalidTime,
                                "0");
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (strReset == "1")
                        {
                            //重置密码
                            listParams = new List<string>();
                            listParams.Add(strID);
                            listParams.Add("0");
                            listParams.Add(string.Empty);
                            optReturn = ModifyUserPassword(session, listParams);
                        }
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT COUNT(1) FROM T_11_005_{0} WHERE C001 <> {1} AND C002 = '{2}' "
                            , rentToken
                            , strID
                            , account);
                        optReturn = OracleOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        count = Convert.ToInt32(optReturn.Data);
                        //账户名已经存在
                        if (count > 0)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_EXIST;
                            optReturn.Message = string.Format("User account already exist.");
                            return optReturn;
                        }
                        strLockSql = string.Empty;
                        //if (strLock == "C")
                        //{
                        //    strLockSql = string.Format(", C008 = '0', C009 = 'N'");
                        //}
                        //else if (strLock == "U")
                        //{
                        //    strLockSql = string.Format(", C008 = '1', C009 = 'U'");
                        //}
                        if (strLock == "CL")
                        {
                            strLockSql = string.Format(", C008 = '0', C009 = 'N' ,C024 = 0 ");
                        }
                        else if (strLock == "CU")
                        {
                            strLockSql = string.Format(", C008 = '0', C009 = 'N'");
                        }
                        else if (strLock == "U")
                        {
                            strLockSql = string.Format(", C008 = '1', C009 = 'U'");
                        }

                        strSql =
                            string.Format(
                                "UPDATE T_11_005_{0} SET C002 = '{1}', C003 = '{2}'{3} ,C010='{5}' ,C017='{6}',C018='{7}',C027='{8}' WHERE C001 = {4}",
                                rentToken,
                                account,
                                fullName,
                                strLockSql,
                                strID,
                                IsActive,
                                validTime,
                                invalidTime,
                                "0");
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (strReset == "1")
                        {
                            //重置密码
                            listParams = new List<string>();
                            listParams.Add(strID);
                            listParams.Add("0");
                            listParams.Add(string.Empty);
                            optReturn = ModifyUserPassword(session, listParams);
                        }
                        break;
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

        private OperationReturn ModifyUserExtInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 5)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string userID, strEmail, strPhone, strBirthday, strHeadIcon;
                //0     UserID
                //1     Email
                //2     Phone
                //3     Birthday
                //4     HeadIcon
                userID = listParams[0];
                strEmail = listParams[1];
                strPhone = listParams[2];
                strBirthday = listParams[3];
                strHeadIcon = listParams[4];

                string rentToken = session.RentInfo.Token;
                string strSql;
                int count;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT COUNT(1) FROM T_11_101_{0} WHERE C001 = {1} AND C002 = 1"
                                                    , rentToken
                                                    , userID);
                        optReturn = MssqlOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        count = Convert.ToInt32(optReturn.Data);
                        //表中不存在，新增
                        if (count <= 0)
                        {
                            strSql =
                                string.Format(
                                    "INSERT INTO T_11_101_{0} (C001,C002,C011,C012,C013,C014) VALUES ({1},1,'{2}','{3}','{4}','{5}')"
                                    , rentToken
                                    , userID
                                    , strEmail
                                    , strPhone
                                    , strBirthday
                                    , strHeadIcon);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            return optReturn;
                        }
                        strSql =
                            string.Format(
                                "UPDATE T_11_101_{0} SET C011 = '{1}', C012 = '{2}', C013 = '{3}', C014 = '{4}' WHERE C001 = {5} AND C002 =1"
                                , rentToken
                                , strEmail
                                , strPhone
                                , strBirthday
                                , strHeadIcon
                                , userID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        return optReturn;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT COUNT(1) FROM T_11_101_{0} WHERE C001 = {1} AND C002 = 1"
                            , rentToken
                            , userID);
                        optReturn = OracleOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        count = Convert.ToInt32(optReturn.Data);
                        //表中不存在，新增
                        if (count <= 0)
                        {
                            strSql =
                                string.Format(
                                    "INSERT INTO T_11_101_{0} (C001,C002,C011,C012,C013,C014) VALUES ({1},1,'{2}','{3}','{4}','{5}')"
                                    , rentToken
                                    , userID
                                    , strEmail
                                    , strPhone
                                    , strBirthday
                                    , strHeadIcon);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            return optReturn;
                        }
                        strSql =
                           string.Format(
                                "UPDATE T_11_101_{0} SET C011 = '{1}', C012 = '{2}', C013 = '{3}', C014 = '{4}' WHERE C001 = {5} AND C002 =1"
                                , rentToken
                                , strEmail
                                , strPhone
                                , strBirthday
                                , strHeadIcon
                                , userID);
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

        private OperationReturn MotifyAgentNameByAccount(SessionInfo session, List<string> listParams)
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
                string AgentID, AgentName;
                //0     AgentID
                //1     AgentName

                AgentID = listParams[0];
                AgentName = EncryptToDB(listParams[1]);
               
                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql =
                            string.Format(
                                "UPDATE T_11_101_{0} SET C018 = '{1}' WHERE C001 = {2} AND C002 =1"
                                , rentToken
                                , AgentName
                                , AgentID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        return optReturn;
                    //ORCL
                    case 3:
                        strSql =
                           string.Format(
                                "UPDATE T_11_101_{0} SET C018 = '{1}' WHERE C001 = {2} AND C002 =1"
                                , rentToken
                                , AgentName
                                , AgentID);
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

    }
}