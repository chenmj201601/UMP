using Common3106;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace Wcf31061
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service31061”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service31061.svc 或 Service31061.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service31061 : IService31061
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

        private string DecryptM001(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
            CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101),
            EncryptionAndDecryption.UMPKeyAndIVType.M101);
            return strReturn;
        }

        private string DecryptFromClient(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
              EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
        }

        #endregion

        public WebReturn UMPTreeOperation(WebRequest webRequest)
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
                    case (int)S3106Codes.GetUserOperationList:
                        optReturn = GetUserOperationList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3106Codes.GetControlAgentInfoList:
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
                    case (int)S3106Codes.GetControlOrgInfoList:
                        optReturn = GetControlOrgInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3106Codes.GetQA:
                        optReturn = GetQA(session, webRequest.ListData);
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
                    case (int)S3106Codes.GetFolder:
                        optReturn = GetFolder(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3106Codes.GetFiles:
                        optReturn = GetFiles(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3106Codes.FolderDBO:
                        optReturn = FolderDBO(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3106Codes.UploadFile:
                        optReturn = UploadFile(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3106Codes.UploadToDisk:
                        optReturn = UploadToDisk(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3106Codes.DeleteFolder:
                        optReturn = DeleteFolder(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3106Codes.AllotAgent:
                        optReturn = AllotAgent(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)RequestCode.WSGetSerialID:
                        optReturn = GetSerialID(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3106Codes.GetRecordInfo:
                        optReturn = GetRecordInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3106Codes.GetUMPSetupPath:
                        optReturn = GetUMPSetupPath(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        break;
                    case (int)S3106Codes.WriteBrowseHistory:
                        optReturn = WriteBrowseHistory(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3106Codes.GetBrowseHistory:
                        optReturn = GetBrowseHistory(session, webRequest.ListData);
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
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1} AND C007<>'H' )"
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
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1} AND C007<>'H')"
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
                    strName = DecryptNamesFromDB(strName);
                    string ParentID = dr["C004"].ToString();
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strID, ConstValue.SPLITER_CHAR, strName, ParentID);
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and C001 >= 1030000000000000000 and c001 < 1040000000000000000"
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and c001 >= 1030000000000000000 and c001 < 1040000000000000000"
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

        private OperationReturn GetQA(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string OrgID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE  C006={1}  AND C001 IN" +
                            "  (SELECT  C004  FROM T_11_201_{0} WHERE C003 IN(SELECT C001 FROM T_11_202_{0} WHERE C001 LIKE '106%' ) AND C004 LIKE '102%') AND C007 <> 'H' ",
                            rentToken, OrgID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE  C006={1}  AND C001 IN" +
                            "  (SELECT  C004  FROM T_11_201_{0} WHERE C003 IN(SELECT C001 FROM T_11_202_{0} WHERE C001 LIKE '106%' ) AND C004 LIKE '102%') AND C007 <> 'H' ",
                            rentToken, OrgID);
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
                    CtrolQA ctrolqa = new CtrolQA();
                    ctrolqa.UserID = dr["C001"].ToString();
                    ctrolqa.UserName = DecryptNamesFromDB(dr["C002"].ToString());
                    ctrolqa.UserFullName = DecryptNamesFromDB(dr["C003"].ToString());
                    ctrolqa.OrgID = OrgID;
                    optReturn = XMLHelper.SeriallizeObject(ctrolqa);
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

        /// <summary>
        /// 生成主键
        /// 1 模块编码
        /// 2 资源编码
        /// </summary>
        private OperationReturn GetSerialID(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     模块编码
                //1     资源编码
                //2     时间变量
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string moduleID = listParams[0];
                string resourceID = listParams[1];
                string dateFormat = listParams[2];
                string rentToken = session.RentInfo.Token;
                string strSerialID = string.Empty;
                long errNumber = 0;
                string strErrMsg = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@AInParam01",MssqlDataType.Varchar,2),
                            MssqlOperation.GetDbParameter("@AInParam02",MssqlDataType.Varchar,3),
                            MssqlOperation.GetDbParameter("@AInParam03",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@Ainparam04",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AOutParam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AOutErrorNumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@AOutErrorString",MssqlDataType.NVarchar,4000)
                        };
                        mssqlParameters[0].Value = moduleID;
                        mssqlParameters[1].Value = resourceID;
                        mssqlParameters[2].Value = rentToken;
                        mssqlParameters[3].Value = dateFormat;
                        mssqlParameters[4].Value = strSerialID;
                        mssqlParameters[5].Value = errNumber;
                        mssqlParameters[6].Value = strErrMsg;
                        mssqlParameters[4].Direction = ParameterDirection.Output;
                        mssqlParameters[5].Direction = ParameterDirection.Output;
                        mssqlParameters[6].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_00_001",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[5].Value, mssqlParameters[6].Value);
                        }
                        else
                        {
                            strSerialID = mssqlParameters[4].Value.ToString();
                            optReturn.Data = strSerialID;
                        }
                        break;
                    case 3:
                        DbParameter[] orclParameters =
                        {
                            OracleOperation.GetDbParameter("AInParam01",OracleDataType.Varchar2,2),
                            OracleOperation.GetDbParameter("AInParam02",OracleDataType.Varchar2,3),
                            OracleOperation.GetDbParameter("AInParam03",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("Ainparam04",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("AOutParam01",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("AOutErrorNumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("AOutErrorString",OracleDataType.Nvarchar2,4000)
                        };
                        orclParameters[0].Value = moduleID;
                        orclParameters[1].Value = resourceID;
                        orclParameters[2].Value = rentToken;
                        orclParameters[3].Value = dateFormat;
                        orclParameters[4].Value = strSerialID;
                        orclParameters[5].Value = errNumber;
                        orclParameters[6].Value = strErrMsg;
                        orclParameters[4].Direction = ParameterDirection.Output;
                        orclParameters[5].Direction = ParameterDirection.Output;
                        orclParameters[6].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_00_001",
                           orclParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (orclParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", orclParameters[5].Value, orclParameters[6].Value);
                        }
                        else
                        {
                            strSerialID = orclParameters[4].Value.ToString();
                            optReturn.Data = strSerialID;
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

        /// <summary>
        /// 获取DB中所有文件夹
        /// </summary>
        private OperationReturn GetFolder(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count <0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM T_31_058_{0} ", session.RentInfo.Token);
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
                    FolderInfo Info = new FolderInfo();
                    Info.FolderID = Convert.ToInt64(dr["C001"]);
                    Info.TreeParentID = Convert.ToInt64(dr["C002"]);
                    Info.FolderName = dr["C003"].ToString();
                    Info.TreeParentName = dr["C004"].ToString();
                    Info.CreatorId = Convert.ToInt64(dr["C005"]);
                    Info.CreatorName = dr["C006"].ToString();
                    Info.CreatorTime = Convert.ToDateTime(dr["C007"]).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    Info.UserID1 = dr["C008"].ToString();
                    Info.UserID2 = dr["C009"].ToString();
                    Info.UserID3 = dr["C010"].ToString();

                    optReturn = XMLHelper.SeriallizeObject(Info);
                    if (!optReturn.Result)
                    {
                        optReturn.Code = i;
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
        
        /// <summary>
        /// 获取该目录下所有文件
        /// </summary>
        private OperationReturn GetFiles(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM T_31_060_{0} WHERE C002={1}", session.RentInfo.Token,listParams[0]);
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
                    FilesItemInfo Info = new FilesItemInfo();
                    Info.FileID = Convert.ToInt64(dr["C001"]);
                    Info.FolderID = Convert.ToInt64(dr["C002"]);
                    Info.FileName = dr["C003"].ToString();
                    Info.FilePath = dr["C004"].ToString();
                    Info.FileDescription = dr["C005"].ToString();
                    Info.FromType = dr["C006"].ToString();
                    Info.IsEncrytp = dr["C007"].ToString();
                    Info.FileType = dr["C008"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(Info);
                    if (!optReturn.Result)
                    {
                        optReturn.Code = i;
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

        /// <summary>
        ///  文件夹操作（0、新建；1、重命名）
        /// </summary>
        private OperationReturn FolderDBO(SessionInfo session, List<string> listParams)
        {
            //0、操作ID：0、新建；1、重命名
            //1、文件夹树信息
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count <4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strSql = listParams[1];
                optReturn = XMLHelper.DeserializeObject<FolderInfo>(strSql);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                if (listParams[0] == "0")//0新建，1修改
                {
                    if (!string.IsNullOrWhiteSpace(listParams[3]) && (!Directory.Exists(listParams[3])))//已存在文件夹在DB中被删除，物理路径未删除而遗留下来，然后又命名一个同样名字的文件夹的处理
                    {
                        Directory.CreateDirectory(listParams[3]);
                    }
                }
                if (listParams[0] == "1")//0新建，1修改
                {
                    DirectoryInfo dir = new DirectoryInfo(listParams[2]);//旧的地址
                    dir.MoveTo(listParams[3]);//新的地址  
                }
                
                FolderInfo folderInfo = optReturn.Data as FolderInfo;
                if (folderInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("folderInfo is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                strSql = string.Format("SELECT * FROM T_31_058_{0}", rentToken);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
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
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                DataRow dr;
                if (listParams[0] == "0")//0新建，1修改
                {
                    dr = objDataSet.Tables[0].NewRow();
                    dr["C001"] = folderInfo.FolderID;
                    dr["C002"] = folderInfo.TreeParentID;
                    dr["C003"] = folderInfo.FolderName;
                    dr["C004"] = folderInfo.TreeParentName;
                    dr["C005"] = folderInfo.CreatorId;
                    dr["C006"] = folderInfo.CreatorName;
                    dr["C007"] = DateTime.UtcNow;
                    dr["C008"] =folderInfo.CreatorId;
                    dr["C009"] = string.Empty;
                    dr["C010"] = string.Empty;
                    objDataSet.Tables[0].Rows.Add(dr);
                }
                if (listParams[0] == "1")//0新建，1修改
                {
                    dr = objDataSet.Tables[0].Select(string.Format("C001 = {0}", folderInfo.FolderID)).FirstOrDefault();
                    dr.BeginEdit();
                    dr["C003"] = folderInfo.FolderName;
                    dr.EndEdit();
                }
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();

                if(listParams[0]=="1")//重命名的文件夹需要更新数据库中子节点的父节点文件夹名
                {
                    strSql = string.Format("UPDATE T_31_058_{0} SET C004='{1}' WHERE C002='{2}'", rentToken, folderInfo.FolderName, folderInfo.FolderID);
                    switch (session.DBType)
                    {
                        case 2:
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                        case 3:
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                            return optReturn;
                    }
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

        /// <summary>
        /// 删除文件夹、文件
        /// </summary>
        private OperationReturn DeleteFolder(SessionInfo session, List<string> listParams)
        {
            // 0、操作ID，0,删除文件夹及其目录下的文件; 1, 删除文件
            // 1、主键ID

            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count <3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string StrCondition = string.Empty;

                Directory.Delete(listParams[2], true);

                if (listParams[0] == "0")//删除文件夹及其目录下的文件
                {
                    StrCondition = string.Format("DELETE FROM T_31_058_{0} WHERE C001={1}; DELETE FROM T_31_060_{0} WHERE C002={1};", rentToken, listParams[1]);
                }
                else if (listParams[0] == "1")//删除文件
                {
                    StrCondition = string.Format("DELETE FROM T_31_060_{0} WHERE C001 IN {1};",rentToken,listParams[1]);
                }
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, StrCondition);
                        break;
                    //ORCL
                    case 3:
                        if (listParams[0]=="0")
                        {
                            StrCondition = string.Format("BEGIN {0} END;", StrCondition);
                        }
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, StrCondition);
                        break;
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

        /// <summary>
        /// 修改权限控制
        /// </summary>
        private OperationReturn AllotAgent(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<FolderInfo>(listParams[0]);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                FolderInfo folderInfo = optReturn.Data as FolderInfo;
                if (folderInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("folderInfo is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_31_058_{0} WHERE C001={1}", rentToken, folderInfo.FolderID);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
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
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                DataRow dr = objDataSet.Tables[0].Select(string.Format("C001 = {0}", folderInfo.FolderID)).FirstOrDefault();
                dr.BeginEdit();
                dr["C008"] = folderInfo.UserID1;
                dr["C009"] = folderInfo.UserID2;
                dr["C010"] = folderInfo.UserID3;
                dr.EndEdit();
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
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

        /// <summary>
        ///  上传文件
        /// </summary>
        private OperationReturn UploadFile(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strSql = listParams[0];
                optReturn = XMLHelper.DeserializeObject<FilesItemInfo>(strSql);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                FilesItemInfo filesInfo = optReturn.Data as FilesItemInfo;
                if (filesInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("filesInfo is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                strSql = string.Format("SELECT * FROM T_31_060_{0}", rentToken);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
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
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                DataRow dr= objDataSet.Tables[0].NewRow();
                dr["C001"] = filesInfo.FileID;
                dr["C002"] = filesInfo.FolderID;
                dr["C003"] = filesInfo.FileName;
                dr["C004"] = filesInfo.FilePath;
                dr["C005"] = filesInfo.FileDescription;
                dr["C006"] = filesInfo.FromType;
                dr["C007"] = filesInfo.IsEncrytp;
                dr["C008"] = filesInfo.FileType;
                objDataSet.Tables[0].Rows.Add(dr);
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();                
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

        private OperationReturn UploadToDisk(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            FileStream fs1 =null;
            StreamWriter writer=null;
            try
            {
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                fs1 = new FileStream(listParams[0], FileMode.Create);
                writer = new StreamWriter(fs1);
                writer.Write(listParams[1]);
                writer.Flush();
                writer.Close();
                fs1.Close();
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            //finally { fs1.Close(); writer.Close(); }
            return optReturn;
        }

        public WebReturn UMPUpOperation(UpRequest upRequest)
        {
            WebReturn webReturn = new WebReturn();
            SessionInfo session = upRequest.Session;
            webReturn.Session = session;
            try
            {
                FileStream fileStm = new FileStream(upRequest.SvPath,FileMode.OpenOrCreate);
                fileStm.Seek(0, SeekOrigin.End);
                fileStm.Write(upRequest.ListByte, 0, upRequest.ListByte.Length);
                fileStm.Flush();
                fileStm.Close();
                fileStm.Dispose();
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
        /// 获取查询上传的录音信息
        /// </summary>
        private OperationReturn GetRecordInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM {0} WHERE C002={1}", listParams[0], listParams[1]);
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
                    VoiceCyber.UMP.Common31031.RecordInfo item = new VoiceCyber.UMP.Common31031.RecordInfo();
                    item.RowID = Convert.ToInt64(dr["C001"]);
                    item.SerialID = Convert.ToInt64(dr["C002"]);
                    item.RecordReference = dr["C077"].ToString();
                    item.StartRecordTime = Convert.ToDateTime(dr["C005"]).ToLocalTime();
                    item.StopRecordTime = Convert.ToDateTime(dr["C009"]).ToLocalTime();
                    item.VoiceIP = dr["C020"].ToString();
                    item.VoiceID = Convert.ToInt32(dr["C037"]);
                    item.ChannelID = Convert.ToInt32(dr["C038"]);
                    item.Extension = dr["C042"].ToString();
                    item.Agent = dr["C039"].ToString();
                    item.Duration = Convert.ToInt32(dr["C012"]);
                    item.Direction = dr["C045"].ToString() == "1" ? 1 : 0;
                    item.CallerID = dr["C040"].ToString();
                    item.CalledID = dr["C041"].ToString();
                    item.WaveFormat = dr["C015"].ToString();
                    item.MediaType = Convert.ToInt32(dr["C014"]);
                    item.EncryptFlag = dr["C025"].ToString();
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

        /// <summary>
        /// 获取服务端UMP安装目录
        /// </summary>
        private OperationReturn GetUMPSetupPath(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string UmpPath= System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory);//根树地址
                UmpPath=UmpPath.Substring(0,UmpPath.LastIndexOf("\\"));
                UmpPath=UmpPath.Substring(0,UmpPath.LastIndexOf("\\"));
                //UmpPath = Path.Combine(UmpPath, "BOOK");
                //if (!Directory.Exists(UmpPath))
                //{
                //    Directory.CreateDirectory(UmpPath);
                //}
                optReturn.StringValue = UmpPath;
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
        
        #region  权限 && 读狗
        /// <summary>
        /// 新的读取权限的方式，会读狗 2016年3月2日 13:50:16 
        /// </summary>
        private OperationReturn GetUserOperationList(SessionInfo session, List<string> listParams)
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
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string roleID = session.RoleInfo.ID.ToString();
                string moduleID = listParams[1];
                string parentID = listParams[2];
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


                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {2} AND C003='1') AND C002 LIKE '{3}%' ORDER BY C001, C002",
                                rentToken, moduleID, roleID, parentID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            strLog += strSql;
                            optReturn.Message += strLog;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                             string.Format(
                                 "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {2} AND C003='1') AND C002 LIKE '{3}%' ORDER BY C001, C002",
                                 rentToken, moduleID, roleID, parentID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            strLog += strSql;
                            optReturn.Message += strLog;
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
                strLog += strSql;
                List<string> listOpts = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];


                    #region License 控制

                    string strC006 = dr["C006"].ToString();
                    string strC007 = dr["C007"].ToString();
                    string strC008 = dr["C008"].ToString();
                    if (string.IsNullOrEmpty(strC006))
                    {
                        //C006为空的跳过
                        strLog += string.Format("C006 is empty;");
                        continue;
                    }
                    strC006 = DecryptNamesFromDB(strC006);
                    string[] listC006 = strC006.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                    if (listC006.Length < 3)
                    {
                        //C006无效
                        strLog += string.Format("C006 is invalid;");
                        continue;
                    }
                    long optID = Convert.ToInt64(dr["C002"]);
                    long licID = optID + 1000000000;
                    if (licID.ToString() != listC006[0])
                    {
                        //LicenseID与操作ID不对应
                        strLog += string.Format("LicID not equal;");
                        continue;
                    }
                    if (listC006[2] == "Y")
                    {
                        string strC008Hash = GetMD5HasString(strC008);
                        string strC008Hash8 = strC008Hash.Substring(0, 8);
                        string strLicDoggle = string.Format("{0}{1}", licID, doggleNumber);
                        strC008 = DecryptNamesFromDB(strC008);
                        if (strLicDoggle != strC008)
                        {
                            //与C008不匹配
                            strLog += string.Format("C008 not equal;");
                            continue;
                        }
                        string strDecryptC007 = EncryptionAndDecryption.DecryptStringYKeyIV(strC007, strC008Hash8,
                            strC008Hash8);
                        string[] listC007 = strDecryptC007.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.None);
                        if (listC007.Length < 2)
                        {
                            //C007无效
                            strLog += string.Format("C007 is invalid;");
                            continue;
                        }
                        if (listC007[1] != "Y")
                        {
                            //没有许可
                            strLog += string.Format("No license;");
                            continue;
                        }
                    }

                    #endregion


                    OperationInfo opt = new OperationInfo();
                    opt.ID = Convert.ToInt64(dr["C002"]);
                    opt.ParentID = Convert.ToInt64(dr["C003"]);
                    opt.Display = string.Format("Opt({0})", opt.ID);
                    opt.Description = opt.Display;
                    opt.Icon = dr["C013"].ToString();
                    opt.SortID = Convert.ToInt32(dr["C004"]);
                    string strType = dr["C011"].ToString();
                    int intType = 0;
                    switch (strType)
                    {
                        case "M":
                            intType = 0;
                            break;
                        case "B":
                            intType = 1;
                            break;
                        case "T":
                            intType = 2;
                            break;
                        case "H":
                            intType = 3;
                            break;
                    }
                    opt.Type = intType;
                    optReturn = XMLHelper.SeriallizeObject(opt);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listOpts.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listOpts;
                optReturn.Message = strLog;
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

        #endregion

        /// <summary>
        /// 写入浏览历史
        /// </summary>
        private OperationReturn WriteBrowseHistory(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;

            optReturn = XMLHelper.DeserializeObject<FilesItemInfo>(listParams[0]);
            if (!optReturn.Result)
            {
                return optReturn;
            }
            FilesItemInfo bookInfo = optReturn.Data as FilesItemInfo;
            if (bookInfo == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("BrowseInfo Is Null");
                return optReturn;
            }
            string rentToken = session.RentInfo.Token;
            string strSql;
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            strSql = string.Format("SELECT * FROM T_31_059_{0} WHERE C001 = {1} AND C002={2}", rentToken, bookInfo.FileID, listParams[1]);
            switch (session.DBType)
            {
                case 2:
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
                if (objDataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[0];
                    dr.BeginEdit();
                    dr["C005"] = Convert.ToInt32(dr["C005"]) + 1;
                    dr["C006"] = DateTime.UtcNow;
                    dr.EndEdit();
                }
                else
                {
                    DataRow dr = objDataSet.Tables[0].NewRow();
                    dr["C001"] = bookInfo.FileID;
                    dr["C002"] = listParams[1];
                    dr["C003"] = bookInfo.FileName;
                    dr["C004"] = listParams[2];
                    dr["C005"] = 1;
                    dr["C006"] = DateTime.UtcNow;
                    objDataSet.Tables[0].Rows.Add(dr);
                }
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
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

        /// <summary>
        /// 获取教材浏览记录
        /// </summary>
        private OperationReturn GetBrowseHistory(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, listParams[0]);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, listParams[0]);
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
                    FilesItemInfo Info = new FilesItemInfo();
                    Info.FileID = Convert.ToInt64(dr["C001"]);
                    Info.UserID = Convert.ToInt64(dr["C002"]);
                    Info.FileName = dr["C003"].ToString();
                    Info.UserName = dr["C004"].ToString();
                    Info.BrowseTimes = Convert.ToInt32(dr["C005"]);
                    Info.BrowseTime = Convert.ToDateTime(dr["C006"]).ToLocalTime().ToString("yyyy/MM/dd/HH:mm:ss");
                    optReturn = XMLHelper.SeriallizeObject(Info);
                    if (!optReturn.Result)
                    {
                        optReturn.Code = i;
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
