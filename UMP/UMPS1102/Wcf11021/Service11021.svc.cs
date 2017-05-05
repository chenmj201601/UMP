using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.ServiceModel.Activation;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Common;
using VoiceCyber.Common;
using PFShareClassesS;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.IO;
using VoiceCyber.UMP.Common11021;

namespace Wcf11021
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service11021 : IService11021
    {

        #region Encryption and Decryption
        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType keyIVID)
        {
            string strReturn;
            int LIntRand;
            Random random = new Random();
            string strTemp;

            try
            {
                strReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = random.Next(0, 14);
                strTemp = LIntRand.ToString("00");
                strReturn = strReturn.Insert(LIntRand, "VCT");
                LIntRand = random.Next(0, 17);
                strTemp += LIntRand.ToString("00");
                strReturn = strReturn.Insert(LIntRand, "UMP");
                LIntRand = random.Next(0, 20);
                strTemp += LIntRand.ToString("00");
                strReturn = strReturn.Insert(LIntRand, ((int)keyIVID).ToString("000"));

                strReturn = EncryptionAndDecryption.EncryptStringY(strTemp + strReturn);
            }
            catch { strReturn = string.Empty; }

            return strReturn;
        }

        private string EncryptDecryptToDB(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
               CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
               EncryptionAndDecryption.UMPKeyAndIVType.M104);
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strTemp,
                CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
                EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strReturn;
        }

        private string EncryptDecryptFromDB(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
                CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
                EncryptionAndDecryption.UMPKeyAndIVType.M102);
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strTemp,
                CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
                EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strReturn;
        }

        private string DecryptNamesFromDB(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
                CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
                EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strTemp;
        }

        private string DecryptFromClient(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
               CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
               EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
        }

        #endregion

        public WebReturn URPOperation(WebRequest webRequest)
        {
            WebReturn webReturn = new WebReturn();
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
                DatabaseInfo dbInfo = session.DatabaseInfo;
                if (dbInfo != null)
                {
                    dbInfo.RealPassword = DecryptFromClient(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }
                switch (webRequest.Code)
                {
                    case (int)S1102Codes.GetOperationList:
                        optReturn = GetOperationList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listOpts = optReturn.Data as List<string>;
                        webReturn.ListData = listOpts;
                        break;    
                    case (int)S1102Codes.GetRoleList:
                        optReturn = GetRoleList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listRoles = optReturn.Data as List<string>;
                        webReturn.ListData = listRoles;
                        break;
                    case (int)S1102Codes.AddNewRole:
                        optReturn = AddNewRole(session,webRequest.Data);
                        if(!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S1102Codes.ModifyRole:
                        optReturn = ModifyRole(session,webRequest.ListData);
                        if(!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        //webReturn.Data = optReturn.Data.ToString();
                        List<string> listRole = optReturn.Data as List<string>;
                        webReturn.ListData = listRole;
                        break;
                    case (int)S1102Codes.GetOrganizationList:
                        optReturn = GetControledOrgList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listOrgs = optReturn.Data as List<string>;
                        webReturn.ListData = listOrgs;
                        break;
                    case (int)S1102Codes.GetUserList:
                        optReturn = GetControledUserList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listUsers = optReturn.Data as List<string>;
                        webReturn.ListData = listUsers;
                        break;
                    case (int)S1102Codes.GetRolePermission:
                        optReturn = GetRolePermission(session,webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listRolePermissions = optReturn.Data as List<string>;
                        webReturn.ListData = listRolePermissions;
                        break;
                    case (int)S1102Codes.SubmitRolePermission:
                        optReturn = SubmitRolePermission(session, webRequest.ListData,webRequest.Data);
                        if(!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int) S1102Codes.SubmitRoleUser:
                        optReturn = SubmitRoleUsers(session, webRequest.ListData, webRequest.Data);
                        if(!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1102Codes.GetRoleUsers:
                        optReturn = GetRoleUsers(session,webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listRoleUsers = optReturn.Data as List<string>;
                        webReturn.ListData = listRoleUsers;
                        break;
                    case (int)S1102Codes.DeleteRolePermission:
                        optReturn=DeleteRolePermissions(session, webRequest.Data);
                        if(!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S1102Codes.DeleteRoleUser:
                        optReturn = DeleteRoleUsers(session, webRequest.Data);
                        if(!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S1102Codes.UpdateUserPerimission:
                        optReturn = UpdateUserPermission(session, webRequest.ListData);
                        if(!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S1102Codes.GetCurrentOperationList:
                        optReturn = GetCurrentRoleOperationList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listCurrentOpts = optReturn.Data as List<string>;
                        webReturn.ListData = listCurrentOpts;
                        break;
                    case (int)S1102Codes.GetControlAgentInfoList:
                        optReturn = GetControlAgentInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    default:
                        webReturn.Result = false;
                        webReturn.Code = Defines.RET_PARAM_INVALID;
                        webReturn.Message = string.Format("WebCodes invalid.\t{0}", webRequest.Code);
                        return webReturn;
                }
                webReturn.Result = true;
                webReturn.Code = 0;
                webReturn.Message = optReturn.Message;
                return webReturn;
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
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
                    strName = DecryptNamesFromDB(strName);
                    strFullName = DecryptNamesFromDB(strFullName);
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

        private OperationReturn GetCurrentRoleOperationList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     用户编号
                //1     模块代码
                //2     上级模块或操作编号
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string moduleID = listParams[0];
                string parentID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string roleId = session.RoleInfo.ID.ToString();
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND (C002 LIKE '{2}%' ) AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {3} AND  C003='1' ) ORDER BY  C004 ",
                                rentToken,
                                moduleID,
                                parentID,
                                roleId);
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
                                "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND (C002 LIKE  '{2}%' ) AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {3} AND  C003='1' ) ORDER BY  C004",
                                rentToken,
                                moduleID,
                                parentID,
                                roleId
                                );
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
                List<string> listOpts = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    OperationInfo opt = new OperationInfo();
                    opt.ID = Convert.ToInt64(dr["C002"]);
                    opt.ParentID = Convert.ToInt64(dr["C003"]);
                    opt.Display = string.Format("Opt({0})", opt.ID);
                    opt.Description = opt.Description;
                    opt.Icon = dr["C013"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(opt);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listOpts.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listOpts;
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

        //批量更新用户的权限
        private OperationReturn UpdateUserPermission(SessionInfo session,List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int errNum = 0;
            string errMsg = string.Empty;
            try
            {

                string strOperatorID = session.UserInfo.UserID.ToString();
                string RentID = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        {
                            foreach (string strData in listParams)
                            {
                                optReturn = XMLHelper.DeserializeObject<RoleUsersInfo>(strData);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                RoleUsersInfo newRoleUser = optReturn.Data as RoleUsersInfo;
                                if (newRoleUser == null)
                                {
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_OBJECT_NULL;
                                    optReturn.Message = string.Format("Role User  Is Null");
                                    return optReturn;
                                }
                                DbParameter[] mssqlParameters =
                                {
                                    MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.Varchar,5 ),
                                    MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.Varchar,20),
                                    MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.Varchar,20),
                                    MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                                    MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.Varchar,200)
                                };
                                mssqlParameters[0].Value = RentID;
                                mssqlParameters[1].Value = newRoleUser.UserID;
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
                                    optReturn.Message = mssqlParameters[4].Value.ToString();
                                }
                                else
                                {
                                    optReturn.Data = mssqlParameters[3].Value.ToString();
                                }
                            }
                           
                        }
                        break;
                    //ORCL
                    case 3:
                        {
                            foreach (string strData in listParams)
                            {
                                optReturn = XMLHelper.DeserializeObject<RoleUsersInfo>(strData);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                RoleUsersInfo newRoleUser = optReturn.Data as RoleUsersInfo;
                                if (newRoleUser == null)
                                {
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_OBJECT_NULL;
                                    optReturn.Message = string.Format("Role User  Is Null");
                                    return optReturn;
                                }
                                DbParameter[] orclParameters =
                                {
                                    OracleOperation.GetDbParameter("ainparam01",OracleDataType.Varchar2,5 ),
                                    OracleOperation.GetDbParameter("ainparam02",OracleDataType.Varchar2,20),
                                    OracleOperation.GetDbParameter("ainparam03",OracleDataType.Varchar2,20),
                                    OracleOperation.GetDbParameter("errornumber",OracleDataType.Int32,0),
                                    OracleOperation.GetDbParameter("errorstring",OracleDataType.Varchar2,200)
                                };
                                orclParameters[0].Value = session.RentInfo.Token;
                                orclParameters[1].Value = newRoleUser.UserID;
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
                                    optReturn.Message = orclParameters[4].Value.ToString();
                                }
                                else
                                {
                                    optReturn.Data = orclParameters[3].Value.ToString();
                                }
                            }
                           
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

        private OperationReturn DeleteRoleUsers(SessionInfo session,  string roleid) 
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("DELETE  FROM T_11_201_{0} WHERE  C003={1}  AND  C004 >= 1020000000000000000 AND C004 < 1040000000000000000 ", rentToken, roleid);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("DELETE  FROM T_11_201_{0} WHERE   C003={1}  AND  C004 >= 1020000000000000000 AND C004 < 1040000000000000000  ", rentToken, roleid);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);

                        //得

                        break;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = string.Format(ex.Message);
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn DeleteRolePermissions(SessionInfo session, string roleid) 
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("DELETE  FROM T_11_202_{0} WHERE C001 = {1} ",  rentToken,roleid);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("DELETE  FROM T_11_202_{0} WHERE C001 = {1} ", rentToken, roleid);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = string.Format(ex.Message);
                return optReturn;
            }
            return optReturn;
        }


        private OperationReturn SubmitRoleUsers(SessionInfo session, List<string> listParams,string roleid) 
        {                       
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {  
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C001={1}  AND C003={2} AND  C004 >= 1020000000000000000 AND C004 < 1040000000000000000 ", rentToken, 0, roleid);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT  *  FROM  T_11_201_{0} WHERE  C001={1}  AND  C003={2} AND   C004 >= 1020000000000000000 AND C004 < 1040000000000000000  ", rentToken, 0, roleid);
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
                    foreach (string strData in listParams)
                    {
                        optReturn = XMLHelper.DeserializeObject<RoleUsersInfo>(strData);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        RoleUsersInfo newRoleUser = optReturn.Data as RoleUsersInfo;
                        if (newRoleUser == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("Role User  Is Null");
                            return optReturn;
                        }
                        DataRow drCurrent = objDataSet.Tables[0].Select(string.Format("C001=0 AND C003={0} AND C004={1}", newRoleUser.RoleID, newRoleUser.UserID)).Count() > 0 ? objDataSet.Tables[0].Select(string.Format("C001=0 AND C003={0} AND C004={1}", newRoleUser.RoleID, newRoleUser.UserID)).First() : null;
                            //objDataSet.Tables[0].Rows.Find(new object[3] { 0, newRoleUser.RoleID, newRoleUser.UserID });
                        if (drCurrent != null)
                        {
                            //如果打的是删除标记是true,删除,否则为更新
                            if (newRoleUser.IsDelete)
                            {
                                //dataSet.Tables[0].Rows.Remove(drCurrent);
                                int i = 0;
                                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                                {

                                    if (dr[0].ToString().Equals("0") && dr[2].ToString().Equals(newRoleUser.RoleID.ToString()) && dr[3].ToString().Equals(newRoleUser.UserID.ToString()))
                                    {
                                        objDataSet.Tables[0].Rows[i].Delete();
                                        listMsg.Add(string.Format("{0}{1}{2}", "D", ConstValue.SPLITER_CHAR, newRoleUser.UserID));
                                        break;
                                    }
                                    i = i + 1;
                                }
                            }
                            else
                            {
                                drCurrent.BeginEdit();
                                drCurrent["C001"] = 0;
                                drCurrent["C002"] = 0;
                                drCurrent["C005"] = newRoleUser.StrStartTime;
                                drCurrent["C006"] = newRoleUser.StrEndTime;
                                drCurrent.EndEdit();
                            }
                        }
                        else
                        {
                            if (!newRoleUser.IsDelete)
                            {
                                DataRow drNewRow = objDataSet.Tables[0].NewRow();
                                drNewRow["C001"] = 0;
                                drNewRow["C002"] = 0;
                                drNewRow["C003"] = newRoleUser.RoleID;
                                drNewRow["C004"] = newRoleUser.UserID;
                                drNewRow["C005"] = newRoleUser.StrStartTime;
                                drNewRow["C006"] = newRoleUser.StrEndTime;
                                objDataSet.Tables[0].Rows.Add(drNewRow);

                                listMsg.Add(string.Format("{0}{1}{2}", "A", ConstValue.SPLITER_CHAR, newRoleUser.UserID));
                            }
                        }
                    }
                    //初始化用户权限
                    OperationReturn tempReturn = UpdateUserPermission(session, listParams);
                    if (!tempReturn.Result)
                    {
                        return tempReturn;
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
                optReturn.Message = string.Format(ex.Message);
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn  GetRoleUsers(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strSql;
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT  * FROM T_11_201_{1} WHERE C003 = {0} AND C004 >= 1020000000000000000 AND C004 < 1040000000000000000  ", listParams[0], rentToken);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT  *  FROM  T_11_201_{1} WHERE  C003 = {0}  AND C004 >= 1020000000000000000 AND C004 < 1040000000000000000 ", listParams[0], rentToken);
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
                try
                {
                    objAdapter.Fill(objDataSet);

                    List<string> listRoleUsers = new List<string>();
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        RoleUsersInfo roleUsersInfo = new RoleUsersInfo();
                        roleUsersInfo.NumberID = Convert.ToInt64(dr["C001"]);
                        roleUsersInfo.ParentID = Convert.ToInt64(dr["C002"]);
                        roleUsersInfo.RoleID = Convert.ToInt64(dr["C003"]);
                        roleUsersInfo.UserID = Convert.ToInt64(dr["C004"]);
                        roleUsersInfo.StartTime = DateTime.Parse(dr["C005"].ToString()).ToLocalTime();
                        roleUsersInfo.EndTime = DateTime.Parse(dr["C006"].ToString()).ToLocalTime(); 
                        optReturn = XMLHelper.SeriallizeObject(roleUsersInfo);
                        if (!optReturn.Result)
                        {
                            break;
                        }
                        listRoleUsers.Add(optReturn.Data.ToString());
                    }
                    optReturn.Data = listRoleUsers;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    objConn.Close();
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

        private OperationReturn SubmitRolePermission(SessionInfo session, List<string> listParams,string roleid)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT  *  FROM  T_11_202_{0} WHERE  C001={1}", rentToken, roleid);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT  * FROM  T_11_202_{0} WHERE  C001={1}", rentToken, roleid);
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
                       DataSet dataSet = new DataSet();
                       objAdapter.Fill(dataSet);
                       List<string> listMsg = new List<string>();
                        foreach (string strData in listParams)
                        {
                            optReturn = XMLHelper.DeserializeObject<RolePermissionInfo>(strData);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            RolePermissionInfo newRolePermission = optReturn.Data as RolePermissionInfo;
                            if (newRolePermission == null)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_OBJECT_NULL;
                                optReturn.Message = string.Format("Role Permission  Is Null");
                                return optReturn;
                            }
                            DataRow drCurrent = dataSet.Tables[0].Select(string.Format("C001={0} AND C002={1}", newRolePermission.RoleID, newRolePermission.PermissionID)).Count() > 0 ? dataSet.Tables[0].Select(string.Format("C001={0} AND C002={1}", newRolePermission.RoleID, newRolePermission.PermissionID)).First() : null;
                                //dataSet.Tables[0].Rows.Find(new object[2] { newRolePermission.RoleID, newRolePermission.PermissionID.ToString() });
                            if (drCurrent != null)
                            {
                                //如果打的是删除标记是true,否则为更新
                                if (newRolePermission.IsDelete)
                                {
                                    //dataSet.Tables[0].Rows.Remove(drCurrent);
                                    int position = 0;
                                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                    {
                                        if (dataSet.Tables[0].Rows[i] == drCurrent)
                                        { position = i; }
                                    }
                                    dataSet.Tables[0].Rows[position].Delete();
                                    //
                                    listMsg.Add(string.Format("{0}{1}{2}", "D", ConstValue.SPLITER_CHAR, newRolePermission.PermissionID));
                                }
                                else
                                {
                                    drCurrent.BeginEdit();
                                    drCurrent["C003"] = newRolePermission.IsCanUse;
                                    drCurrent["C004"] = newRolePermission.IsCanDownAssign;
                                    drCurrent["C005"] = newRolePermission.IsCanCascadeRecycle;
                                    drCurrent["C006"] = newRolePermission.ModifyID;
                                    drCurrent["C007"] = newRolePermission.StrModifyTime;
                                    drCurrent["C008"] = newRolePermission.StrEnableTime;
                                    drCurrent["C009"] = newRolePermission.StrEndTime;
                                    drCurrent.EndEdit();
                                }
                            }
                            else
                            {
                                if (!newRolePermission.IsDelete)
                                {
                                    DataRow drNewRow = dataSet.Tables[0].NewRow();
                                    drNewRow["C001"] = newRolePermission.RoleID;
                                    drNewRow["C002"] = newRolePermission.PermissionID;
                                    drNewRow["C003"] = newRolePermission.IsCanUse;
                                    drNewRow["C004"] = newRolePermission.IsCanDownAssign;
                                    drNewRow["C005"] = newRolePermission.IsCanCascadeRecycle;
                                    drNewRow["C006"] = newRolePermission.ModifyID;
                                    drNewRow["C007"] = newRolePermission.StrModifyTime;
                                    drNewRow["C008"] = newRolePermission.StrEnableTime;
                                    drNewRow["C009"] = newRolePermission.StrEndTime;
                                    dataSet.Tables[0].Rows.Add(drNewRow);
                                    listMsg.Add(string.Format("{0}{1}{2}", "A", ConstValue.SPLITER_CHAR, newRolePermission.PermissionID));
                                }
                            }
                        }


                        objAdapter.Update(dataSet);
                        dataSet.AcceptChanges();
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
                optReturn.Message = string.Format(ex.Message);
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetRolePermission(SessionInfo session, List<string> listParams) 
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strSql;
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT  * FROM  T_11_202_{1} WHERE C001 = {0}", listParams[0],rentToken);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT  * FROM  T_11_202_{1}  WHERE  C001 = {0}", listParams[0],rentToken);
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
                try
                {
                    objAdapter.Fill(objDataSet);

                    List<string> listRolePermissions= new List<string>();
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        RolePermissionInfo rolePermissionInfo = new RolePermissionInfo();
                        rolePermissionInfo.RoleID = Convert.ToInt64(dr["C001"]);
                        rolePermissionInfo.PermissionID = Convert.ToInt64(dr["C002"]);
                        rolePermissionInfo.IsCanUse =dr["C003"].ToString();
                        rolePermissionInfo.IsCanDownAssign = dr["C004"].ToString();
                        rolePermissionInfo.IsCanCascadeRecycle = dr["C005"].ToString();
                        rolePermissionInfo.ModifyID = Convert.ToInt64(dr["C006"].ToString());
                        rolePermissionInfo.ModifyTime =DateTime.Parse( dr["C007"].ToString()).ToLocalTime();
                        rolePermissionInfo.EnableTime = DateTime.Parse( dr["C008"].ToString()).ToLocalTime();;
                        rolePermissionInfo.EndTime = DateTime.Parse(dr["C009"].ToString()).ToLocalTime();
                        optReturn = XMLHelper.SeriallizeObject(rolePermissionInfo);
                        if (!optReturn.Result)
                        {
                            break;
                        }
                        listRolePermissions.Add(optReturn.Data.ToString());
                    }
                    optReturn.Data = listRolePermissions;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    objConn.Close();
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

        private OperationReturn GetControledOrgList(SessionInfo session, List<string> listParams)
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
                string rentToken = session.RentInfo.Token;
                //ListParam
                //0     userID
                //1     parentID( -1:用户所属机构信息）
                string userID = listParams[0];
                string parentID = listParams[1];
                string strSql;
                DataSet objDataSet = null;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        //所属机构
                        if (parentID == "-1")
                        {
                            strSql = string.Format("SELECT  A.*  FROM  T_11_006_{0} A , T_11_005_{0} B WHERE  A.C001 = B.C006 AND B.C001 = {1}"
                                , rentToken
                                , userID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.*  FROM  T_11_006_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C004 = {1} AND B.C003 = {2}",
                                rentToken,
                                parentID,
                                userID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    //ORCL
                    case 3:
                        //所属机构
                        if (parentID == "-1")
                        {
                            strSql = string.Format("SELECT A.* FROM  T_11_006_{0} A, T_11_005_{0} B WHERE  A.C001 = B.C006 AND B.C001 = {1}"
                                , rentToken
                                , userID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT  A.*  FROM  T_11_006_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C004 = {1} AND B.C003 = {2}",
                                rentToken,
                                parentID,
                                userID);
                        }
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
                List<string> listOrgs = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    OrganizationInfo orgInfo = new OrganizationInfo();
                    orgInfo.OrgID = Convert.ToInt64(dr["C001"]);
                    orgInfo.OrgName = DecryptNamesFromDB(dr["C002"].ToString());
                        //DecryptFromDB(dr["C002"].ToString());
                    orgInfo.OrgType = Convert.ToInt32(dr["C003"]);
                    orgInfo.ParentID = Convert.ToInt64(dr["C004"]);
                    orgInfo.IsActived = dr["C005"].ToString();
                    orgInfo.IsDeleted = dr["C006"].ToString();
                    orgInfo.State = dr["C007"].ToString();
                    orgInfo.StrStartTime = DecryptNamesFromDB(dr["C008"].ToString());
                    orgInfo.StrEndTime = DecryptNamesFromDB(dr["C009"].ToString());
                        //DecryptFromDB(dr["C009"].ToString());
                    orgInfo.Creator = Convert.ToInt64(dr["C010"]);
                    orgInfo.CreateTime = Convert.ToDateTime(dr["C011"]);
                    orgInfo.Description = dr["C012"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(orgInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listOrgs.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listOrgs;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GetControledUserList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count != 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string userID = listParams[0];
                string parentID = listParams[1];
                string strSql;
                DataSet objDataSet = null;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT  * FROM T_11_005_{0} WHERE C006 = {1} AND C001 IN (SELECT  C004 FROM T_11_201_{0} WHERE C003 = {2}   )",
                            rentToken,
                            parentID,
                            userID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT  * FROM T_11_005_{0} WHERE C006 = {1} AND  C001 IN  (SELECT  C004 FROM  T_11_201_{0} WHERE C003 = {2}   )",
                            rentToken,
                            parentID,
                            userID);
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
                List<string> listUsers = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    BasicUserInfo userInfo = new BasicUserInfo();
                    userInfo.UserID = Convert.ToInt64(dr["C001"]);
                    userInfo.Account = DecryptNamesFromDB(dr["C002"].ToString());
                    userInfo.FullName = DecryptNamesFromDB(dr["C003"].ToString());
                    userInfo.OrgID = Convert.ToInt64(dr["C006"]);
                    userInfo.SourceFlag = dr["C007"].ToString();
                    userInfo.IsLocked = dr["C008"].ToString();
                    userInfo.LockMethod = dr["C009"].ToString();
                    userInfo.StrStartTime = DecryptNamesFromDB(dr["C017"].ToString());
                    userInfo.StrEndTime = DecryptNamesFromDB(dr["C018"].ToString());
                    userInfo.IsActived = dr["C010"].ToString();
                    userInfo.IsDeleted = dr["C011"].ToString();
                    userInfo.State = dr["C012"].ToString();
                    userInfo.Creator = Convert.ToInt64(dr["C019"]);
                    userInfo.StrCreateTime = DecryptNamesFromDB(dr["C020"].ToString());
                    optReturn = XMLHelper.SeriallizeObject(userInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listUsers.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listUsers;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn ModifyRole(SessionInfo session, List<string> strData) 
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string errMsg = string.Empty;
            try
            {
                //px屏蔽
                //optReturn = XMLHelper.DeserializeObject<RoleModel>(strData);
                //if (!optReturn.Result)
                //{
                //    return optReturn;
                //}
                //RoleModel newRole = optReturn.Data as RoleModel;
                //if (newRole == null)
                //{
                //    optReturn.Result = false;
                //    optReturn.Code = Defines.RET_OBJECT_NULL;
                //    optReturn.Message = string.Format("NewRoleInfo Is Null");
                //    return optReturn;
                //}
                //newRole.RoleName = EncryptDecryptToDB(newRole.RoleName);
                //newRole.StrEnableTime = EncryptDecryptToDB(newRole.StrEnableTime);
                //string rentToken = session.RentInfo.Token;
                //newRole.StrEndTime = EncryptDecryptToDB(newRole.StrEndTime);
                //px-end

                //px+
                if (strData.Count != 11)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("NewRoleInfo Is Null");
                    return optReturn;
                }
                string RoleID = strData[0];
                string RoleName = EncryptDecryptToDB(strData[1]);
                string ParentRoleID = strData[2];
                string ModeID = strData[3];
                string CreatorID = strData[4];
                string CreatTime = strData[5];
                string IsActive = strData[6];
                string IsDelete = strData[7];
                string StrEnableTime = EncryptDecryptToDB(strData[8]);
                string StrEndTime = EncryptDecryptToDB(strData[9]);
                string OtherStatus = strData[10];
                string rentToken = session.RentInfo.Token;
                List<string> listRole = new List<string>();
                foreach (string Str in strData)
                {
                    OperationReturn StrTemp = XMLHelper.SeriallizeObject(listRole);
                    if (!StrTemp.Result)
                    {
                        optReturn.Data = strData;
                        return optReturn;
                    }
                    listRole.Add(StrTemp.Data.ToString());
                }
                optReturn.Data = listRole;
                //end

                //++++++ dm++++++++++++++++++++++
                ///添加是否存在该角色名
                string strSql;
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DataSet objDataSet = new DataSet();
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT  * FROM T_11_004_{0} WHERE C004='{1}'  AND C006='0' AND C001<> {2} ", rentToken,RoleName,RoleID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT  * FROM  T_11_004_{0}  WHERE  C004 = '{1}' AND C006='0' AND C001<>{2} ", rentToken, RoleName,RoleID);
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
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_EXIST;
                    optReturn.Message = string.Empty;

                    return optReturn;
                }
                //++++++ dm++++++++++++++++++++++


                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        using (SqlConnection connection = new SqlConnection(session.DBConnectionString)) 
                        {
                            DataSet dataSet = new DataSet();
                            connection.Open();
                            SqlDataAdapter sqlDA = new SqlDataAdapter(string.Format("SELECT  * FROM T_11_004_{1} WHERE C001={0}", RoleID, rentToken), connection);
                            sqlDA.Fill(dataSet);
                            //设置主键
                            dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0] };
                            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
                            sqlDA.InsertCommand = sqlCB.GetInsertCommand();
                            DataRow drCurrent = dataSet.Tables[0].Select(string.Format("C001={0}", RoleID)).Count() > 0 ? dataSet.Tables[0].Select(string.Format("C001={0}",RoleID)).First() : null;
                                //dataSet.Tables[0].Rows.Find(newRole.RoleID.ToString());
                            if (drCurrent != null)
                            {
                                drCurrent.BeginEdit();
                                drCurrent["C002"] = ParentRoleID;
                                drCurrent["C003"] = ModeID;
                                drCurrent["C004"] = RoleName;
                                drCurrent["C005"] = IsActive;
                                drCurrent["C006"] = IsDelete;
                                drCurrent["C007"] = OtherStatus;
                                drCurrent["C008"] = StrEnableTime;
                                drCurrent["C009"] = StrEndTime;
                                drCurrent["C010"] = CreatorID;
                                drCurrent["C011"] = CreatTime;
                                drCurrent.EndEdit();
                            }
                            sqlDA.Update(dataSet);
                            dataSet.AcceptChanges();
                            sqlDA.Dispose();
                            connection.Close();
                        }
                        break;
                    //ORCL
                    case 3:
                        using (OracleConnection connection = new OracleConnection(session.DBConnectionString))
                        {
                            DataSet dataSet = new DataSet();
                            connection.Open();
                            OracleDataAdapter oracleDA = new OracleDataAdapter(string.Format("SELECT * FROM T_11_004_{1} WHERE  C001={0}" , RoleID,rentToken), connection);
                            oracleDA.Fill(dataSet);
                            //设置主键
                            dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0] };
                            OracleCommandBuilder oracleCB = new OracleCommandBuilder(oracleDA);
                            oracleDA.InsertCommand = oracleCB.GetInsertCommand();
                            DataRow drCurrent = dataSet.Tables[0].Select(string.Format("C001={0}", RoleID)).Count() > 0 ? dataSet.Tables[0].Select(string.Format("C001={0}", RoleID)).First() : null;
                                //dataSet.Tables[0].Rows.Find(newRole.RoleID.ToString());
                            if (drCurrent != null)
                            {
                                drCurrent.BeginEdit();
                                drCurrent["C002"] = ParentRoleID;
                                drCurrent["C003"] = ModeID;
                                drCurrent["C004"] = RoleName;
                                drCurrent["C005"] = IsActive;
                                drCurrent["C006"] = IsDelete;
                                drCurrent["C007"] = OtherStatus;
                                drCurrent["C008"] = StrEnableTime;
                                drCurrent["C009"] = StrEndTime;
                                drCurrent["C010"] = CreatorID;
                                drCurrent["C011"] = CreatTime;
                                drCurrent.EndEdit();
                            }       
                            oracleDA.Update(dataSet);
                            dataSet.AcceptChanges();
                            oracleDA.Dispose();
                            connection.Close();        
                        }

                        break;
                    default:
                        break;
                }
            }
            catch (Exception  ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = string.Format(ex.Message);
                return optReturn;               
            }

            return optReturn;
        }
        private OperationReturn AddNewRole(SessionInfo session, string strData) 
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int errNum = 0;
            long serialID = 0;
            string errMsg = string.Empty;
            try
            {
               
                optReturn = XMLHelper.DeserializeObject<RoleModel>(strData);
                if(!optReturn.Result)
                {
                    return optReturn;
                }
                RoleModel newRole = optReturn.Data as RoleModel;
                if(newRole==null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("NewRoleInfo Is Null");
                    return optReturn;
                }
                newRole.RoleName = EncryptDecryptToDB(newRole.RoleName);
                newRole.StrEnableTime = EncryptDecryptToDB(newRole.StrEnableTime);
                newRole.StrEndTime = EncryptDecryptToDB(newRole.StrEndTime);
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DataSet objDataSet = new DataSet();
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT  * FROM T_11_004_{0} WHERE C004='{1}'  AND C006='0' ", rentToken,newRole.RoleName);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT  * FROM  T_11_004_{0}  WHERE  C004 = '{1}' AND C006='0' ", rentToken, newRole.RoleName);
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
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_EXIST;
                    optReturn.Message = string.Empty;

                    return optReturn;
                }


                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.Varchar,5 ),
                            MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@ainparam04",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam05",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam06",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam07",MssqlDataType.Varchar,32),
                            MssqlOperation.GetDbParameter("@ainparam08",MssqlDataType.Varchar,512),
                            MssqlOperation.GetDbParameter("@ainparam09",MssqlDataType.Varchar,512),
                            MssqlOperation.GetDbParameter("@ainparam10",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam11",MssqlDataType.Varchar,0),
                            MssqlOperation.GetDbParameter("@aoutparam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.NVarchar,4000)
                        };
                        mssqlParameters[0].Value = session.RentInfo.Token;
                        mssqlParameters[1].Value = newRole.ParentRoleID;
                        mssqlParameters[2].Value = newRole.ModeID;
                        mssqlParameters[3].Value = newRole.RoleName;
                        mssqlParameters[4].Value = newRole.IsActive;
                        mssqlParameters[5].Value = newRole.IsDelete;
                        mssqlParameters[6].Value = newRole.OtherStatus;
                        mssqlParameters[7].Value = newRole.StrEnableTime;
                        mssqlParameters[8].Value = newRole.StrEndTime;
                        mssqlParameters[9].Value = newRole.CreatorID;
                        mssqlParameters[10].Value = newRole.CreatTime.ToString("yyyy/MM/dd HH:mm:ss");
                        mssqlParameters[11].Value = serialID;
                        mssqlParameters[12].Value = errNum;
                        mssqlParameters[13].Value = errMsg;
                        mssqlParameters[11].Direction = ParameterDirection.Output;
                        mssqlParameters[12].Direction = ParameterDirection.Output;
                        mssqlParameters[13].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_014",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[12].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[13].Value, mssqlParameters[14].Value);
                        }
                        else
                        {
                            optReturn.Data = mssqlParameters[11].Value.ToString();
                        }
                        break;
                    //ORCL
                    case 3:
                        DbParameter[] dbParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01",OracleDataType.Varchar2,5 ),
                            OracleOperation.GetDbParameter("ainparam02",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("ainparam03",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("ainparam04",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam05",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam06",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam07",OracleDataType.Varchar2,32),
                            OracleOperation.GetDbParameter("ainparam08",OracleDataType.Varchar2,512),
                            OracleOperation.GetDbParameter("ainparam09",OracleDataType.Varchar2,512),
                            OracleOperation.GetDbParameter("ainparam10",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("ainparam11",OracleDataType.Varchar2,0),
                            OracleOperation.GetDbParameter("aoutparam01",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("errornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("errorstring",OracleDataType.Nvarchar2,200)
                        };
                        dbParameters[0].Value = session.RentInfo.Token;
                        dbParameters[1].Value=newRole.ParentRoleID;
                        dbParameters[2].Value=newRole.ModeID;
                        dbParameters[3].Value=newRole.RoleName;
                        dbParameters[4].Value=newRole.IsActive; 
                        dbParameters[5].Value=newRole.IsDelete;
                        dbParameters[6].Value=newRole.OtherStatus;
                        dbParameters[7].Value=newRole.StrEnableTime;
                        dbParameters[8].Value=newRole.StrEndTime;
                        dbParameters[9].Value=newRole.CreatorID;
                        dbParameters[10].Value = newRole.CreatTime.ToString("yyyy/MM/dd HH:mm:ss");
                        dbParameters[11].Value = serialID;
                        dbParameters[12].Value = errNum;
                        dbParameters[13].Value = errMsg;
                        dbParameters[11].Direction = ParameterDirection.Output;
                        dbParameters[12].Direction = ParameterDirection.Output;
                        dbParameters[13].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_014",
                            dbParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (dbParameters[12].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = dbParameters[13].Value.ToString();
                        }
                        else
                        {
                            optReturn.Data = dbParameters[11].Value.ToString();
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = string.Format(ex.Message);
                return optReturn;
            }
            return optReturn;
        }
        private OperationReturn GetRoleList(SessionInfo session ,List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strSql;
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT  T_11_004_{0}.* ,T_11_005_{0}.C002  UserName  FROM T_11_004_{0} LEFT JOIN T_11_005_{0} ON  T_11_004_{0}.C010= T_11_005_{0}.C001 WHERE  T_11_004_{0}.C001 IN (SELECT C003  FROM T_11_201_{0} WHERE  C004={1} AND  C003 LIKE '106%' )  AND  T_11_004_{0}.C006='0'  ", rentToken, listParams[0]);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT T_11_004_{0}.* ,T_11_005_{0}.C002  UserName  FROM T_11_004_{0} LEFT JOIN T_11_005_{0} ON  T_11_004_{0}.C010= T_11_005_{0}.C001 WHERE  T_11_004_{0}.C001 IN (SELECT C003  FROM T_11_201_{0} WHERE  C004={1} AND C003 LIKE '106%' )  AND  T_11_004_{0}.C006='0'  ", rentToken, listParams[0]);
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
                try
                {
                    objAdapter.Fill(objDataSet);

                    List<string> listRoles = new List<string>();
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        RoleModel RoleInfo = new RoleModel();
                        RoleInfo.RoleID = Convert.ToInt64(dr["C001"]);
                        RoleInfo.ParentRoleID = Convert.ToInt64(dr["C002"]);
                        RoleInfo.ModeID = Convert.ToInt64(dr["C003"]);
                        RoleInfo.RoleName = EncryptDecryptFromDB(dr["C004"].ToString());
                        RoleInfo.IsActive = dr["C005"].ToString();
                        RoleInfo.IsDelete = dr["C006"].ToString();
                        RoleInfo.OtherStatus = dr["C007"].ToString();
                        RoleInfo.StrEnableTime = EncryptDecryptFromDB(dr["C008"].ToString());
                        RoleInfo.StrEndTime = EncryptDecryptFromDB(dr["C009"].ToString());
                        RoleInfo.CreatorID = Convert.ToInt64(dr["C010"]);
                        RoleInfo.CreatorName = EncryptDecryptFromDB(dr["UserName"].ToString());
                        RoleInfo.CreatTime = Convert.ToDateTime(dr["C011"].ToString()).ToLocalTime();

                        optReturn = XMLHelper.SeriallizeObject(RoleInfo);
                        if (!optReturn.Result)
                        {
                            break;
                        }
                        listRoles.Add(optReturn.Data.ToString());
                    }
                    optReturn.Data = listRoles;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    objConn.Close();
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
        private OperationReturn GetOperationList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strSql = "";
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                string strLog = string.Empty;

                #region 获取当前的狗号

                optReturn = GetDoggleNumber();
                if (!optReturn.Result)
                {
                    strLog += string.Format("Get doggle number fail.");
                    optReturn.Message += strLog;
                    return optReturn;
                }
                string doggleNumber = optReturn.Data.ToString();
                strLog += string.Format("{0};", doggleNumber);

                #endregion

                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT A.* ,B.C003 AS IsCanUse ,B.C004 AS  IsCanDownAssign, B.C005 AS IsCanCascadeRecycle  FROM  T_11_003_{0} A  ,(  SELECT  C002,C003,C004,C005 FROM T_11_202_{0} WHERE   C001 IN  (  SELECT  C003  FROM T_11_201_{0} WHERE   C004={1} AND  C003 LIKE '106%')) B WHERE  A.C002=B.C002  ORDER  BY  A.C004 ",
                            rentToken, listParams[0]);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        optReturn.Message += strSql + "******";
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT  A.* ,B.C003 AS IsCanUse ,B.C004 AS  IsCanDownAssign, B.C005 AS IsCanCascadeRecycle  FROM  T_11_003_{0} A  ,(  SELECT  C002,C003,C004,C005 FROM  T_11_202_{0} WHERE    C001 IN  (  SELECT  C003  FROM  T_11_201_{0} WHERE   C004={1} AND  C003 LIKE  '106%')) B WHERE  A.C002=B.C002  ORDER BY  A.C004 ", 
                            rentToken, listParams[0]);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        optReturn.Message += strSql+"******";
                        break;
                }
                optReturn.Message += strSql;
                if (objConn == null
                    || objAdapter == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DBConnection or DBDataAdapter is null");
                    return optReturn;
                }
                try
                {
                    objAdapter.Fill(objDataSet);

                    List<string> listOpts = new List<string>();
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];

                        #region License 控制

                        //string strC006 = dr["C006"].ToString();
                        //string strC007 = dr["C007"].ToString();
                        //string strC008 = dr["C008"].ToString();
                        //if (string.IsNullOrEmpty(strC006))
                        //{
                        //    //C006为空的跳过
                        //    strLog += string.Format("C006 is empty;");
                        //    continue;
                        //}
                        //strC006 = DecryptFromDB(strC006);
                        //string[] listC006 = strC006.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                        //if (listC006.Length < 3)
                        //{
                        //    //C006无效
                        //    strLog += string.Format("C006 is invalid;");
                        //    continue;
                        //}
                        //long optID = Convert.ToInt64(dr["C002"]);
                        //long licID = optID + 1000000000;
                        //if (licID.ToString() != listC006[0])
                        //{
                        //    //LicenseID与操作ID不对应
                        //    strLog += string.Format("LicID not equal;");
                        //    continue;
                        //}
                        //if (listC006[2] == "Y")
                        //{
                        //    string strC008Hash = GetMD5HasString(strC008);
                        //    string strC008Hash8 = strC008Hash.Substring(0, 8);
                        //    string strLicDoggle = string.Format("{0}{1}", licID, doggleNumber);
                        //    strC008 = DecryptFromDB(strC008);
                        //    if (strLicDoggle != strC008)
                        //    {
                        //        //与C008不匹配
                        //        strLog += string.Format("C008 not equal;");
                        //        continue;
                        //    }
                        //    string strDecryptC007 = EncryptionAndDecryption.DecryptStringYKeyIV(strC007, strC008Hash8,
                        //        strC008Hash8);
                        //    string[] listC007 = strDecryptC007.Split(new[] { ConstValue.SPLITER_CHAR },
                        //        StringSplitOptions.None);
                        //    if (listC007.Length < 2)
                        //    {
                        //        //C007无效
                        //        strLog += string.Format("C007 is invalid;");
                        //        continue;
                        //    }
                        //    if (listC007[1] != "Y")
                        //    {
                        //        //没有许可
                        //        strLog += string.Format("No license;");
                        //        continue;
                        //    }
                        //}

                        #endregion

                        ROperationInfo opt = new ROperationInfo();
                        opt.ID = Convert.ToInt64(dr["C002"]);
                        opt.ParentID = Convert.ToInt64(dr["C003"]);
                        opt.Display = string.Format("Opt({0})", opt.ID);
                        opt.Description = opt.Description;
                        opt.Icon = dr["C013"].ToString();
                        opt.IsCanUse = dr["IsCanUse"].ToString();
                        opt.IsCanDownAssign = dr["IsCanDownAssign"].ToString();
                        opt.IsCanCascadeRecycle = dr["IsCanCascadeRecycle"].ToString();
                        opt.IsHide = dr["C011"].ToString();
                        optReturn = XMLHelper.SeriallizeObject(opt);
                        if (!optReturn.Result)
                        {
                            break;
                        }
                        listOpts.Add(optReturn.Data.ToString());
                    }
                    optReturn.Data = listOpts;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    objConn.Close();
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

        #region Others

        private const string PATH_DOGGLECONFIG = "GlobalSettings\\UMP.Young.01";

        private OperationReturn GetDoggleNumber()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, PATH_DOGGLECONFIG);
                if (!File.Exists(path))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("DoggleConfig file not exist.\t{0}", path);
                    return optReturn;
                }
                string[] listContents = File.ReadAllLines(path);
                if (listContents.Length < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_CONFIG_NOT_EXIST;
                    optReturn.Message = string.Format("DoggleConfig not exist.");
                    return optReturn;
                }
                string doggleNumber = listContents[0];
                doggleNumber = DecryptM001(doggleNumber);
                optReturn.Data = doggleNumber;
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

        private string GetMD5HasString(string strSource)
        {
            byte[] byteSource = Encoding.Unicode.GetBytes(strSource);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] byteReturn = md5.ComputeHash(byteSource);
            string strReturn = Converter.Byte2Hex(byteReturn);
            return strReturn;
        }

        private string DecryptM001(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
            CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101),
            EncryptionAndDecryption.UMPKeyAndIVType.M101);
            return strReturn;
        }

        private string DecryptFromDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
              EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strReturn;
        }
        #endregion
    }
}
