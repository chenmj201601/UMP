using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using UMPServicePackCommon;
using System.Data;

namespace UMPServicePack.PublicClasses
{
    /// <summary>
    /// 数据库操作类 包括读写数据库等操作
    /// </summary>
    public class DatabaseOperator
    {
        /// <summary>
        /// 检查用户是否可以登录（检查用户名、密码、有效期、是否是管理员）
        /// </summary>
        /// <param name="strUserName"></param>
        /// <param name="strPwd"></param>
        /// <returns></returns>
        public static OperationReturn CheckUser(string strUserName, string strPwd)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                DatabaseInfo dbInfo = App.currDBInfo;
                string strDBConnString = dbInfo.GetConnectionString();
                string strSql = string.Empty;
                string strUserNameEncrypted = EncryptOperations.EncryptWithM002(strUserName);

                switch (App.currDBInfo.TypeID)
                {
                    case 2:
                        strSql = "SELECT *  FROM T_11_005_{0} where C002='{1}'";
                        strSql = string.Format(strSql, App.strRent, strUserNameEncrypted);
                        optReturn = MssqlOperation.GetDataSet(strDBConnString, strSql);
                        break;
                    case 3:
                        strSql = "SELECT *  FROM T_11_005_{0} where C002='{1}'";
                        strSql = string.Format(strSql, App.strRent, strUserNameEncrypted);
                        optReturn = OracleOperation.GetDataSet(strDBConnString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = ConstDefines.UserName_Or_Pwd_Not_Exists;
                    return optReturn;
                }
                string strUserID = ds.Tables[0].Rows[0]["C001"].ToString();
                string strPwdInDB = ds.Tables[0].Rows[0]["C004"].ToString();
                string strPwdInput = EncryptOperations.EncryptUserPwd(strUserID, strPwd);
                if (!strPwdInDB.Equals(strPwdInput))
                {
                    optReturn.Result = false;
                    optReturn.Code = ConstDefines.UserName_Or_Pwd_Not_Exists;
                    return optReturn;
                }

                DateTime dtFrom = DateTime.Parse(EncryptOperations.DecryptWithM002(ds.Tables[0].Rows[0]["C017"].ToString()));
                string strTo = EncryptOperations.DecryptWithM002(ds.Tables[0].Rows[0]["C018"].ToString());
                if (!strTo.Equals(ConstDefines.strUNLIMITED))
                {
                    //如果有效期不是UNLIMITED 需要判断是否过期
                    DateTime dtTo = DateTime.Parse(strTo);
                    if (!(DateTime.Now > dtFrom && DateTime.Now < dtTo))
                    {
                        optReturn.Result = false;
                        optReturn.Code = ConstDefines.User_Overdue;
                        return optReturn;
                    }
                }
                //检查用户是否是管理员角色
                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql = "SELECT *  FROM T_11_201_{0} where C004 ={1} and C003 =1060000000000000001";
                        strSql = string.Format(strSql, App.strRent, strUserID);
                        optReturn = MssqlOperation.GetDataSet(strDBConnString, strSql);
                        break;
                    case 3:
                        strSql = "SELECT *  FROM T_11_201_{0} where C004 ={1} and C003 =1060000000000000001";
                        strSql = string.Format(strSql, App.strRent, strUserID);
                        optReturn = OracleOperation.GetDataSet(strDBConnString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    optReturn.Code = ConstDefines.Get_User_Role_Failed;
                    return optReturn;
                }
                ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = ConstDefines.User_Not_Admin;
                    return optReturn;
                }
                optReturn.Data = strUserID;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Check_User_Exception;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private static OperationReturn GetUserIDByName(string strUserNameEncryted, string strConnString, string strRent)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                string strSql = string.Empty;
                switch (App.currDBInfo.TypeID)
                {
                    case 2:
                        strSql = "SELECT *  FROM T_11_005_{0} where C004='{1}'";
                        strSql = string.Format(strSql, strRent, strUserNameEncryted);
                        optReturn = MssqlOperation.GetDataSet(strConnString, strSql);
                        break;
                    case 3:
                        strSql = "SELECT *  FROM T_11_005_{0} where C004='{1}'";
                        strSql = string.Format(strSql, strRent, strUserNameEncryted);
                        optReturn = OracleOperation.GetDataSet(strConnString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = ConstDefines.UserName_Or_Pwd_Not_Exists;
                    return optReturn;
                }
                string strUserID = ds.Tables[0].Rows[0]["C001"].ToString();
                optReturn.Data = strUserID;
            }
            catch (Exception ex)
            {
                optReturn.Code = ConstDefines.Get_Version_Exception;
                optReturn.Result = false;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 获得最后一次安装的版本 顺便把T_00_000备份到App.dsT000
        /// </summary>
        /// <returns></returns>
        public static OperationReturn GetLastVersion()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                //获得machineID
                optReturn = Common.GetMachineID();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strMachineID = optReturn.Data as string;
                DatabaseInfo dbInfo = App.currDBInfo;
                string strConnString = dbInfo.GetConnectionString();
                string strSql = string.Empty;
                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql = "SELECT *  FROM T_00_000";
                        optReturn = MssqlOperation.GetDataSet(strConnString, strSql);
                        break;
                    case 3:
                        strSql = "SELECT *  FROM T_00_000";
                        optReturn = OracleOperation.GetDataSet(strConnString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    optReturn.Code = ConstDefines.Get_T000_Failed;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                App.dsT000 = ds;
                //从ds中查找C000=‘00000’ && C001 = ‘SP_LastVersion’的数据
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = ConstDefines.T000_Is_Null;
                    return optReturn;
                }
                string strFilter = "C000 = '{0}' and C001 = '{1}'";
                strFilter = string.Format(strFilter, App.strRent, strMachineID);
                List<DataRow> lstRows = ds.Tables[0].Select(strFilter).ToList();
                if (lstRows.Count <= 0)
                {
                    //如果没有上次升级版本的记录 就当是8.03.001版 可以升级
                    optReturn.Data = "8.03.001";
                    return optReturn;
                }
                string strLastVersion = lstRows[0]["C002"].ToString();
                optReturn.Data = strLastVersion;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Code = ConstDefines.Get_Version_Exception;
                optReturn.Result = false;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 把更新后的版本写进数据库
        /// </summary>
        /// <returns></returns>
        public static OperationReturn WriteVersionToDB()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            try
            {
                optReturn = Common.GetMachineID();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strMachineID = optReturn.Data as string;
                DatabaseInfo dbInfo = App.currDBInfo;
                string strConnString = dbInfo.GetConnectionString();
                string strSql = string.Empty;
                switch (App.currDBInfo.TypeID)
                {
                    case 2:
                        strSql = "SELECT *  FROM T_00_000 where C000 = '{0}' and C001 = '{1}'";
                        strSql = string.Format(strSql, App.strRent, strMachineID);
                        optReturn = MssqlOperation.GetDataSet(strConnString, strSql);
                        break;
                    case 3:
                        strSql = "SELECT *  FROM T_00_000 where C000 = '{0}' and C001 = '{1}'";
                        strSql = string.Format(strSql, App.strRent, strMachineID);
                        optReturn = OracleOperation.GetDataSet(strConnString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    App.WriteLog("Get version failed. message : " + optReturn.Message);
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    //记录不存在 新增
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql = "insert into T_00_000(C000,C001,C002,C003,C004,C005,C006,C009) values ('{0}','{1}','{2}','SP','1','{3}','{4}','Service Pack Version')";
                            strSql = string.Format(strSql, App.strRent, strMachineID, App.updateInfo.Version, DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                            optReturn = MssqlOperation.ExecuteSql(strConnString, strSql);
                            break;
                        case 3:
                            strSql = "insert into T_00_000(C000,C001,C002,C003,C004,C005,C006,C009) values ('{0}','{1}','{2}','SP','1',TO_DATE('{3}','YYYY-MM-DD HH24:MI:SS'),TO_DATE('{4}','YYYY-MM-DD HH24:MI:SS'),'Service Pack Version')";
                            strSql = string.Format(strSql, App.strRent, strMachineID, App.updateInfo.Version, DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"), DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                            optReturn = OracleOperation.ExecuteSql(strConnString, strSql);
                            break;
                    }
                    if (!optReturn.Result)
                    {
                        App.WriteLog("Insert sp version failed. message : " + optReturn.Message);
                        return optReturn;
                    }
                    else
                    {
                        App.WriteLog("Update version to " + App.updateInfo.Version);
                    }
                }
                else
                {
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql = "update t_00_000 set C002 = '{0}' where C000 = '{1}'";
                            strSql = string.Format(strSql, App.updateInfo.Version, App.strRent);
                            optReturn = MssqlOperation.ExecuteSql(strConnString, strSql);
                            break;
                        case 3:
                            strSql = "update t_00_000 set C002 = '{0}' where C000 = '{1}'";
                            strSql = string.Format(strSql, App.updateInfo.Version, App.strRent);
                            optReturn = OracleOperation.ExecuteSql(strConnString, strSql);
                            break;
                    }
                    if (!optReturn.Result)
                    {
                        App.WriteLog("Insert sp version failed. message : " + optReturn.Message);
                        return optReturn;
                    }
                    else
                    {
                        App.WriteLog("Update version to " + App.updateInfo.Version);
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = ConstDefines.Write_Version_To_DB_Exception;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
    }
}
