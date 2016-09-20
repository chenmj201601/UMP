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
                string strID, account, fullName, strLock, strReset, IsActive, validTime, invalidTime, IsOrgManagement, Org;
                //0     UserID
                //1     Account
                //2     FullName
                //3     Lock ( C 解除锁定, U   界面锁定,D 老杨禁用）
                //4     ResetPassword
                //5    IsAcitve
                //6     ValidTime
                //7     InValidTime
                //8     IsOrgManagement
                //9     Org
                strID = listParams[0];
                account = listParams[1];
                fullName = listParams[2];
                strLock = listParams[3];
                strReset = listParams[4];
                IsActive = listParams[5];
                validTime = listParams[6];
                invalidTime = listParams[7];
                IsOrgManagement = listParams[8];
                Org = listParams[9];

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
                                IsOrgManagement);
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
                        if (IsOrgManagement == "1")
                        {
                            DataSet DS_OrgManagement = new DataSet(); List<string> ListControlObj = new List<string>();
                            ListControlObj.Add(Org);
                            //设置为部门管理员:找出该机构下的所有可管理对象，将其插入11-201表被该用户管理；添加一条该用户管理该机构的数据到11-201里
                            string sql_orgmanagement = string.Format("SELECT C001 FROM T_11_101_{0} WHERE C002=1 AND C012>0 AND C001>1030000000000000000 AND C001<1060000000000000000 AND C011={1} "
                                , rentToken
                                , Org);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, sql_orgmanagement);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            DS_OrgManagement = optReturn.Data as DataSet;
                            if (DS_OrgManagement.Tables != null && DS_OrgManagement.Tables.Count != 0)
                            {
                                for (int row = 0; row < DS_OrgManagement.Tables[0].Rows.Count; row++)
                                {
                                    ListControlObj.Add(DS_OrgManagement.Tables[0].Rows[row]["C001"].ToString());
                                }
                            }
                            sql_orgmanagement = string.Format("SELECT C001 FROM T_11_005_{0} WHERE C011=0 AND C006={1} "
                                , rentToken
                                , Org);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, sql_orgmanagement);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            DS_OrgManagement = optReturn.Data as DataSet;
                            if (DS_OrgManagement.Tables != null && DS_OrgManagement.Tables.Count != 0)
                            {
                                for (int row = 0; row < DS_OrgManagement.Tables[0].Rows.Count; row++)
                                {
                                    ListControlObj.Add(DS_OrgManagement.Tables[0].Rows[row]["C001"].ToString());
                                }
                            }
                            for (int listID = 0; listID < ListControlObj.Count; listID++)
                            {
                                sql_orgmanagement = string.Format("SELECT COUNT(1) FROM T_11_201_{0} WHERE C004={1} AND C003={2} "
                                    , rentToken
                                    , ListControlObj[listID]
                                    , strID);
                                optReturn = MssqlOperation.GetRecordCount(session.DBConnectionString, sql_orgmanagement);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                count = Convert.ToInt32(optReturn.Data);
                                if (count == 0)
                                {
                                    sql_orgmanagement = string.Format("INSERT INTO T_11_201_{0} VALUES ('0','0','{1}','{2}','{3}','2199/12/31')"
                                        , rentToken
                                        , strID
                                        , ListControlObj[listID]
                                        , DateTime.Now.ToString("yyyy/MM/dd"));
                                    optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, sql_orgmanagement);
                                    if (!optReturn.Result)
                                    {
                                        optReturn.Message += " insert fail:" + sql_orgmanagement;
                                        return optReturn;
                                    }
                                }
                            }optReturn = ManagementRelationChildObject(session, Org, strID);
                            if (!optReturn.Result) { return optReturn; }
                        }
                        else
                        {
                            //该用户不是机构管理者，暂不做任何处理。若之前是部门管理者，则保留用户管理对象内容
                            //将添加对象绑给管理员管理
                            OperationReturn opt_Temp = new OperationReturn();
                            opt_Temp.Result = true;
                            opt_Temp.Code = 0;
                            opt_Temp = ManagementRelationParentObject(session, Org, strID);
                            if (!opt_Temp.Result) { return opt_Temp; }
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
                                IsOrgManagement);
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
                        if (IsOrgManagement == "1")
                        {
                            DataSet DS_OrgManagement = new DataSet(); List<string> ListControlObj = new List<string>();
                            ListControlObj.Add(Org);
                            //设置为部门管理员:找出该机构下的所有可管理对象，将其插入11-201表被该用户管理；添加一条该用户管理该机构的数据到11-201里
                            string sql_orgmanagement = string.Format("SELECT C001 FROM T_11_005_{0} WHERE C011=0 AND C006={1} "
                                , rentToken
                                , Org);
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, sql_orgmanagement);
                            if (!optReturn.Result)
                            {
                                optReturn.Message += "  11-005:" + sql_orgmanagement;
                                return optReturn;
                            }
                            DS_OrgManagement = optReturn.Data as DataSet;
                            if (DS_OrgManagement.Tables != null && DS_OrgManagement.Tables.Count != 0)
                            {
                                for (int row = 0; row < DS_OrgManagement.Tables[0].Rows.Count; row++)
                                {
                                    ListControlObj.Add(DS_OrgManagement.Tables[0].Rows[row]["C001"].ToString());
                                }
                            }
                            sql_orgmanagement = string.Format("SELECT C001 FROM T_11_101_{0} WHERE C002=1 AND C012>0 AND C001>1030000000000000000 AND C001<1060000000000000000 AND C011='{1}' "
                                 , rentToken
                                 , Org);
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, sql_orgmanagement);
                            if (!optReturn.Result)
                            {
                                optReturn.Message += "\t11-101:" + sql_orgmanagement;
                                return optReturn;
                            }
                            DS_OrgManagement = optReturn.Data as DataSet;
                            if (DS_OrgManagement.Tables != null && DS_OrgManagement.Tables.Count != 0)
                            {
                                for (int row = 0; row < DS_OrgManagement.Tables[0].Rows.Count; row++)
                                {
                                    ListControlObj.Add(DS_OrgManagement.Tables[0].Rows[row]["C001"].ToString());
                                }
                            }
                            for (int listID = 0; listID < ListControlObj.Count; listID++)
                            {
                                sql_orgmanagement = string.Format("SELECT COUNT(1) FROM T_11_201_{0} WHERE C004={1} AND C003={2} "
                                    , rentToken
                                    , ListControlObj[listID]
                                    , strID);
                                optReturn = OracleOperation.GetRecordCount(session.DBConnectionString, sql_orgmanagement);
                                if (!optReturn.Result)
                                {
                                    optReturn.Message += "\t11-201:" + sql_orgmanagement;
                                    return optReturn;
                                }
                                count = Convert.ToInt32(optReturn.Data);
                                if (count == 0)
                                {
                                    sql_orgmanagement = string.Format("INSERT INTO T_11_201_{0} VALUES ('0','0','{1}','{2}',TO_DATE('{3}','yyyy/MM/dd'),TO_DATE('2199/12/31','yyyy/MM/dd')) "
                                        , rentToken
                                        , strID
                                        , ListControlObj[listID]
                                        , DateTime.Now.ToString("yyyy/MM/dd"));
                                    optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, sql_orgmanagement);
                                    if (!optReturn.Result)
                                    {
                                        optReturn.Message += " insert fail:" + sql_orgmanagement;
                                        return optReturn;
                                    }
                                }
                            }
                            optReturn = ManagementRelationChildObject(session, Org, strID);
                            if (!optReturn.Result) { return optReturn; }
                        }
                        else
                        {  //该用户不是机构管理者，暂不做任何处理。若之前是部门管理者，则保留用户管理对象内容
                            //将添加对象绑给管理员管理
                            OperationReturn opt_T = new OperationReturn();
                            opt_T.Result = true; opt_T.Code = 0;
                           opt_T= ManagementRelationParentObject(session, Org, strID);
                           if (!opt_T.Result) { return opt_T; }
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

        #region 机构管理员相关操作
        private OperationReturn ManagementRelationChildObject(SessionInfo session, string OrgID, string ObjID)
        {
            OperationReturn OptReturn = new OperationReturn();
            OptReturn.Result = true;
            OptReturn.Code = 0;

            string sql_Manage = string.Empty; DataSet DS_Manage = new DataSet();
            switch (session.DBType)
            {
                case 2:
                    sql_Manage = string.Format("SELECT C001 FROM T_11_006_{0} WHERE C006=0 AND C004={1} "
                        , session.RentInfo.Token
                        , OrgID);
                    OptReturn = MssqlOperation.GetDataSet(session.DBConnectionString, sql_Manage);
                    break;
                case 3:
                    sql_Manage = string.Format("SELECT C001 FROM T_11_006_{0} WHERE C006=0 AND C004={1} "
                        , session.RentInfo.Token
                        , OrgID);
                    OptReturn = OracleOperation.GetDataSet(session.DBConnectionString, OrgID);
                    break;
            }
            if (!OptReturn.Result)
            {
                return OptReturn;
            }
            DS_Manage = OptReturn.Data as DataSet; bool IsData = false;
            if (DS_Manage.Tables != null && DS_Manage.Tables.Count > 0)
            {
                if (DS_Manage.Tables[0].Rows != null && DS_Manage.Tables[0].Rows.Count != 0)
                {
                    IsData = true;
                }
            }
            if (IsData)
            {
                //插入数据
                for (int i = 0; i < DS_Manage.Tables[0].Rows.Count; i++)
                {
                    string orgID = DS_Manage.Tables[0].Rows[i]["C001"].ToString();
                    switch (session.DBType)
                    {
                        case 2:
                            sql_Manage = string.Format("INSERT INTO T_11_201_{0} VALUES ('0','0','{1}','{2}','{3}','2199/12/31') "
                                , session.RentInfo.Token
                                , ObjID
                                , orgID
                                , DateTime.Now.ToString("yyyy/MM/dd"));
                            OptReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, sql_Manage);
                            break;
                        case 3:
                            sql_Manage = string.Format("INSERT INTO T_11_201_{0} VALUES ('0','0','{1}','{2}',TO_DATE('{3}','yyyy/MM/dd'),TO_DATE('2199/12/31','yyyy/MM/dd') "
                                , session.RentInfo.Token
                                , ObjID
                                , orgID
                                , DateTime.Now.ToString("yyyy/MM/dd"));
                            OptReturn = OracleOperation.ExecuteSql(session.DBConnectionString, OrgID);
                            break;
                    }
                    //if (!OptReturn.Result)
                    //{
                    //    return OptReturn;
                    //}
                    //插入该机构下的所有资源给该对象管理
                    OptReturn = ManagementRelationResource(session, orgID, ObjID);
                    if (!OptReturn.Result)
                    {
                        return OptReturn;
                    }
                    OptReturn = ManagementRelationChildObject(session, orgID, ObjID);
                    if (!OptReturn.Result)
                    {
                        return OptReturn;
                    }
                }
            }

            return OptReturn;
        }

        private OperationReturn ManagementRelationResource(SessionInfo session, string OrgID, string ObjID)
        {
            OperationReturn MoptReturn = new OperationReturn();
            MoptReturn.Code = 0;
            MoptReturn.Result = true;

            string sql_M = string.Empty;
            DataSet DS_M = new DataSet();
            List<string> ListObjID = new List<string>();
            //获取该机构下的座席、分机、真实分机给Obj管理
            switch (session.DBType)
            {
                case 2:
                    sql_M = string.Format("SELECT C001 FROM T_11_101_{0} WHERE C002=1 AND C012>0 AND C001>1030000000000000000 AND C001<1060000000000000000 AND C011={1} "
                        , session.RentInfo.Token
                        , OrgID);
                    MoptReturn = MssqlOperation.GetDataSet(session.DBConnectionString, sql_M);
                    break;
                case 3:
                    sql_M = string.Format("SELECT C001 FROM T_11_101_{0} WHERE C002=1 AND C012>0 AND C001>1030000000000000000 AND C001<1060000000000000000 AND C011={1} "
                        , session.RentInfo.Token
                        , OrgID);
                    MoptReturn = OracleOperation.GetDataSet(session.DBConnectionString, sql_M);
                    break;
            }
            if (!MoptReturn.Result)
            {
                return MoptReturn;
            }
            DS_M = MoptReturn.Data as DataSet; bool IsData = false;
            if (DS_M.Tables != null && DS_M.Tables.Count > 0)
            {
                if (DS_M.Tables[0].Rows != null && DS_M.Tables[0].Rows.Count != 0)
                {
                    IsData = true;
                }
            }
            if (IsData)
            {
                for (int i = 0; i < DS_M.Tables[0].Rows.Count; i++)
                {
                    ListObjID.Add(DS_M.Tables[0].Rows[i]["C001"].ToString());
                }
            }
            //获取该机构下的用户给Obj管理
            switch (session.DBType)
            {
                case 2:
                    sql_M = string.Format("SELECT C001 FROM T_11_005_{0} WHERE C011=0 AND C006={1} "
                        , session.RentInfo.Token
                        , OrgID);
                    MoptReturn = MssqlOperation.GetDataSet(session.DBConnectionString, sql_M);
                    break;
                case 3:
                    sql_M = string.Format("SELECT C001 FROM T_11_005_{0} WHERE C011=0 AND C006={1} "
                        , session.RentInfo.Token
                        , OrgID);
                    MoptReturn = OracleOperation.GetDataSet(session.DBConnectionString, sql_M);
                    break;
            }
            if (!MoptReturn.Result)
            {
                return MoptReturn;
            }
            DS_M = MoptReturn.Data as DataSet; bool IsData1 = false;
            if (DS_M.Tables != null && DS_M.Tables.Count > 0)
            {
                if (DS_M.Tables[0].Rows != null && DS_M.Tables[0].Rows.Count != 0)
                {
                    IsData1 = true;
                }
            }
            if (IsData1)
            {
                for (int i = 0; i < DS_M.Tables[0].Rows.Count; i++)
                {
                    ListObjID.Add(DS_M.Tables[0].Rows[i]["C001"].ToString());
                }
            }
            //数据放入T_11_201_
            for (int j = 0; j < ListObjID.Count; j++)
            {
                switch (session.DBType)
                {
                    case 2:
                        sql_M = string.Format("INSERT INTO T_11_201_{0} VALUES ('0','0','{1}','{2}','{3}','2199/12/31') "
                            , session.RentInfo.Token
                            , ObjID
                            , ListObjID[j]
                            , DateTime.Now.ToString("yyyy/MM/dd"));
                        MoptReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, sql_M);
                        break;
                    case 3:
                        sql_M = string.Format("INSERT INTO T_11_201_{0} VALUES ('0','0','{1}','{2}',TO_DATE('{3}','yyyy/MM/dd'),TO_DATE('2199/12/31','yyyy/MM/dd')) "
                            , session.RentInfo.Token
                            , ObjID
                            , ListObjID[j]
                            , DateTime.Now.ToString("yyyy/MM/dd"));
                        MoptReturn = OracleOperation.ExecuteSql(session.DBConnectionString, sql_M);
                        break;
                }
                //if (!MoptReturn.Result)
                //{
                //    return MoptReturn;
                //}
            }

            return MoptReturn;
        }

        private OperationReturn ManagementRelationParentObject(SessionInfo session, string OrgID, string ObjID)
        {
            OperationReturn MoptR = new OperationReturn();
            MoptR.Code = 0;
            MoptR.Result = true;
            if (OrgID == ConstValue.ORG_ROOT.ToString()) { return MoptR; }
            string sql_M = string.Empty;
            DataSet DS_M = new DataSet();
            List<string> ListObjID = new List<string>();
            //获取该机构下是否有管理员
            switch (session.DBType)
            {
                case 2:
                    sql_M = string.Format("SELECT C001 FROM T_11_005_{0} WHERE C027=1 AND C011=0 AND C006={1} "
                        , session.RentInfo.Token
                        , OrgID);
                    MoptR = MssqlOperation.GetDataSet(session.DBConnectionString, sql_M);
                    break;
                case 3:
                    sql_M = string.Format("SELECT C001 FROM T_11_005_{0} WHERE C027=1 AND C011=0 AND C006={1} "
                        , session.RentInfo.Token
                        , OrgID);
                    MoptR = OracleOperation.GetDataSet(session.DBConnectionString, sql_M);
                    break;
            }
            if (!MoptR.Result)
            {
                return MoptR;
            }
            DS_M = MoptR.Data as DataSet; bool IsData = false; string ManageID = string.Empty;
            if (DS_M.Tables != null && DS_M.Tables.Count > 0)
            {
                if (DS_M.Tables[0].Rows != null && DS_M.Tables[0].Rows.Count != 0)
                {
                    IsData = true; ManageID = DS_M.Tables[0].Rows[0]["C001"].ToString();
                }
            }
            if (IsData)
            {
                //找到管理员  绑定关系
                switch (session.DBType)
                {
                    case 2:
                        sql_M = string.Format("INSERT INTO T_11_201_{0} VALUES ('0','0','{1}','{2}','{3}','2199/12/31') "
                            , session.RentInfo.Token
                            , ManageID
                            , ObjID
                            , DateTime.Now.ToString("yyyy/MM/dd"));
                        MoptR = MssqlOperation.ExecuteSql(session.DBConnectionString, sql_M);
                        break;
                    case 3:
                        sql_M = string.Format("INSERT INTO T_11_201_{0} VALUES ('0','0','{1}','{2}',TO_DATE('{3}','yyyy/MM/dd'),TO_DATE('2199/12/31','yyyy/MM/dd')) "
                            , session.RentInfo.Token
                            , ManageID
                            , ObjID
                            , DateTime.Now.ToString("yyyy/MM/dd"));
                        MoptR = OracleOperation.ExecuteSql(session.DBConnectionString, sql_M);
                        break;
                }
                //if (!MoptR.Result)
                //{
                //    MoptR.Message += "[ManagementRelationParentObject]:" + OrgID+"{"+ObjID+"}"+ManageID;
                //    return MoptR;
                //}
            }
            else
            {
                //没找到继续找  直到机构是顶级机构为止
                switch (session.DBType)
                {
                    case 2:
                        sql_M = string.Format("SELECT C004 FROM T_11_006_{0} WHERE C006=0 AND C001={1} "
                            , session.RentInfo.Token
                            , OrgID);
                        MoptR = MssqlOperation.GetDataSet(session.DBConnectionString, sql_M);
                        break;
                    case 3:
                        sql_M = string.Format("SELECT C004 FROM T_11_006_{0} WHERE C006=0 AND C001={1} "
                            , session.RentInfo.Token
                            , OrgID);
                        MoptR = OracleOperation.GetDataSet(session.DBConnectionString, sql_M);
                        break;
                }
                if (!MoptR.Result)
                {
                    return MoptR;
                }
                DS_M = MoptR.Data as DataSet; bool IsData1 = false; string ParentOrgID = string.Empty;
                if (DS_M.Tables != null && DS_M.Tables.Count > 0)
                {
                    if (DS_M.Tables[0].Rows != null && DS_M.Tables[0].Rows.Count != 0)
                    {
                        IsData1 = true; ParentOrgID = DS_M.Tables[0].Rows[0]["C004"].ToString();
                    }
                }
                if (!IsData1) { return MoptR; }
                if (ParentOrgID != ConstValue.ORG_ROOT.ToString())
                {
                    OperationReturn opt_Temp = new OperationReturn();
                    opt_Temp.Code = 0;
                    opt_Temp.Result = true;
                    opt_Temp= ManagementRelationParentObject(session, ParentOrgID, ObjID);
                    if (!opt_Temp.Result)
                    {
                        return opt_Temp;
                    }
                }
                else
                {
                    return MoptR;
                }
            }
            return MoptR;
        }
        #endregion
    }
}