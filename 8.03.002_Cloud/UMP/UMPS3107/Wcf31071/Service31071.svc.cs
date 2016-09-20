using Common3107;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace Wcf31071
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service31071 : IService31071
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


        public WebReturn UMPTaskOperation(WebRequest webRequest)
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
                    case (int)S3107Codes.GetControlAgentInfoList:
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
                    case (int)S3107Codes.GetControlOrgInfoList:
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
                    case (int)S3107Codes.GetQA:
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
                    case (int)S3107Codes.QuerySettingDBO:
                        optReturn = QuerySettingDBO(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3107Codes.TaskSettingDBO:
                        optReturn = TaskSettingDBO(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3107Codes.GetQueryDetail:
                        optReturn = GetQueryDetail(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3107Codes.GetTaskDetail:
                        optReturn = GetTaskDetail(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3107Codes.DeleteDBO:
                        optReturn = DeleteDBO(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3107Codes.GetRateDetail:
                        optReturn = GetRateDetail(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3107Codes.GetABCD:
                        optReturn = GetABCD(session, webRequest.ListData);
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
                //2     A,代表座席,E,虚拟分机,R,代表真实分机
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                string path1 = string.Empty;
                string path2 = string.Empty;
                if (listParams[2] == "A")
                {
                    path1 = string.Format("1030000000000000000");
                    path2 = string.Format("1040000000000000000");
                }
                if (listParams[2] == "E")
                {
                    path1 = string.Format("1040000000000000000");
                    path2 = string.Format("1050000000000000000");
                }
                if (listParams[2] == "R")
                {
                    path1 = string.Format("1050000000000000000");
                    path2 = string.Format("1060000000000000000");
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =string.Format("SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and C001 >= {3} and c001 < {4}"
                                , rentToken, strParentID, strUserID, path1, path2);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =string.Format("SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and c001 >= {3} and c001 < {4}"
                                , rentToken, strParentID, strUserID, path1, path2);
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
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE  C006={1}  AND C001 IN"+
                            "  (SELECT  C004  FROM T_11_201_{0} WHERE C003 IN(SELECT C001 FROM T_11_202_{0} WHERE C001 LIKE '106%' AND C002={2}) AND C004 LIKE '102%') AND C007 <> 'H' ", 
                            rentToken, OrgID, listParams[1]);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =string.Format("SELECT * FROM T_11_005_{0} WHERE  C006={1}  AND C001 IN"+
                            "  (SELECT  C004  FROM T_11_201_{0} WHERE C003 IN(SELECT C001 FROM T_11_202_{0} WHERE C001 LIKE '106%' AND C002={2}) AND C004 LIKE '102%') AND C007 <> 'H' ",
                            rentToken, OrgID, listParams[1]);
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
        /// 查询条件参数操作
        /// </summary>
        private OperationReturn QuerySettingDBO(SessionInfo session, List<string> listParams)
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
                string strQuerySetting = listParams[0];
                optReturn = XMLHelper.DeserializeObject<QuerySettingItems>(strQuerySetting);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                QuerySettingItems QueryItems = optReturn.Data as QuerySettingItems;
                if (QueryItems == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("QuerySettingItems is null");
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
                        strSql = string.Format("SELECT * FROM T_31_024_{0} ",rentToken);
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
                        strSql = string.Format("SELECT * FROM T_31_024_{0} ",rentToken);
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
                if(listParams[1]=="F")//F新建，T修改
                {
                    dr=objDataSet.Tables[0].NewRow();
                    dr["C001"]=QueryItems.QuerySettingID;
                    dr["C002"]=QueryItems.IsUsed;
                    dr["C003"]=QueryItems.QuerySettingName;
                    dr["C004"]=QueryItems.QueryStartTime;
                    dr["C005"]=QueryItems.QueryStopTime;
                    dr["C006"]=QueryItems.IsRecentTime;
                    if(QueryItems.IsRecentTime=="Y")
                    {
                        dr["C007"] = QueryItems.RecentTimeType;
                        dr["C008"] = QueryItems.RecentTimeNum;
                    }
                    dr["C009"]=QueryItems.StartRecordTime;
                    dr["C010"]=QueryItems.StopRecordTime;
                    dr["C011"]=QueryItems.CallDirection;
                    dr["C012"]=QueryItems.HasScreen;
                    //dr["C013"]=QueryItems.IsEmtion;
                    //dr["C014"]=QueryItems.IsKeyWord;
                    //dr["C015"]=QueryItems.IsBookMark;
                    //dr["C016"]=QueryItems.IsScore;
                    dr["C017"]=QueryItems.AgentAssType;
                    if (!string.IsNullOrWhiteSpace(QueryItems.AgentAssNum))
                    {
                        dr["C018"] = QueryItems.AgentAssNum;
                    }
                    if (!string.IsNullOrWhiteSpace(QueryItems.AgentAssRate))
                    {
                        dr["C019"] = QueryItems.AgentAssRate;
                    }                    
                    dr["C020"]=QueryItems.AgentsIDOne;
                    if (!string.IsNullOrWhiteSpace(QueryItems.AgentsIDTwo))
                    {
                        dr["C021"] = QueryItems.AgentsIDTwo;
                    }
                    if (!string.IsNullOrWhiteSpace(QueryItems.AgentsIDThree))
                    {
                        dr["C022"] = QueryItems.AgentsIDThree;
                    }
                    dr["C023"] = QueryItems.DurationMin;
                    dr["C024"] = QueryItems.DurationMax;

                    if (!string.IsNullOrWhiteSpace(QueryItems.ABCDSql))
                    {
                        dr["C025"] = QueryItems.ABCDSql;
                    }
                    if (!string.IsNullOrWhiteSpace(QueryItems.ABCDSetting))
                    {
                        dr["C026"] = QueryItems.ABCDSetting;
                    }
                    objDataSet.Tables[0].Rows.Add(dr);
                }
                if (listParams[1] == "T")//F新建，T修改
                {
                    dr = objDataSet.Tables[0].Select(string.Format("C001 = {0}", QueryItems.QuerySettingID)).FirstOrDefault();
                    dr.BeginEdit();
                    dr["C002"] = QueryItems.IsUsed;
                    dr["C003"] = QueryItems.QuerySettingName;
                    dr["C004"] = QueryItems.QueryStartTime;
                    dr["C005"] = QueryItems.QueryStopTime;
                    dr["C006"] = QueryItems.IsRecentTime;
                    if (QueryItems.IsRecentTime == "Y")
                    {
                        dr["C007"] = QueryItems.RecentTimeType;
                        dr["C008"] = QueryItems.RecentTimeNum;
                    }
                    dr["C009"] = QueryItems.StartRecordTime;
                    dr["C010"] = QueryItems.StopRecordTime;
                    dr["C011"] = QueryItems.CallDirection;
                    dr["C012"] = QueryItems.HasScreen;
                    //dr["C013"]=QueryItems.IsEmtion;
                    //dr["C014"]=QueryItems.IsKeyWord;
                    //dr["C015"]=QueryItems.IsBookMark;
                    //dr["C016"]=QueryItems.IsScore;
                    dr["C017"] = QueryItems.AgentAssType;
                    if (!string.IsNullOrWhiteSpace(QueryItems.AgentAssNum))
                    {
                        dr["C018"] = QueryItems.AgentAssNum;
                    }
                    if (!string.IsNullOrWhiteSpace(QueryItems.AgentAssRate))
                    {
                        dr["C019"] = QueryItems.AgentAssRate;
                    }
                    dr["C020"] = QueryItems.AgentsIDOne;
                    if (!string.IsNullOrWhiteSpace(QueryItems.AgentsIDTwo))
                    {
                        dr["C021"] = QueryItems.AgentsIDTwo;
                    }
                    else
                    {
                        dr["C021"] = string.Empty;
                    }
                    if (!string.IsNullOrWhiteSpace(QueryItems.AgentsIDThree))
                    {
                        dr["C022"] = QueryItems.AgentsIDThree;
                    }
                    else
                    {
                        dr["C022"] = string.Empty;
                    }
                    dr["C023"] = QueryItems.DurationMin;
                    dr["C024"] = QueryItems.DurationMax;
                    if (!string.IsNullOrWhiteSpace(QueryItems.ABCDSql))
                    {
                        dr["C025"] = QueryItems.ABCDSql;
                    }
                    else
                    {
                        dr["C025"] = string.Empty;
                    }
                    if (!string.IsNullOrWhiteSpace(QueryItems.ABCDSetting))
                    {
                        dr["C026"] = QueryItems.ABCDSetting;
                    }
                    else
                    {
                        dr["C026"] = string.Empty;
                    }
                    dr.EndEdit();
                }
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
        /// 任务参数操作
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// <returns></returns>
        private OperationReturn TaskSettingDBO(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strTaskSetting = listParams[0];
                optReturn = XMLHelper.DeserializeObject<TaskSettingItems>(strTaskSetting);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                TaskSettingItems TaskItems = optReturn.Data as TaskSettingItems;
                if (TaskItems == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("TaskSettingItems is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter TaskAdapter;
                IDbDataAdapter FreqAdapter;
                IDbDataAdapter RateAdapter;
                DbCommandBuilder objCmdBuilder;
                DataRow dr;

                #region T_31_023 任务分配设置
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_023_{0} ", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        TaskAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(TaskAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_023_{0} ", rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        TaskAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(TaskAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || TaskAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                DataSet TaskDataSet = new DataSet();
                TaskAdapter.Fill(TaskDataSet);
                if (listParams[1] =="F")//F新建，T修改
                {
                    dr = TaskDataSet.Tables[0].NewRow();
                    dr["C001"] = TaskItems.TaskSettingID;
                    dr["C002"] = TaskItems.TaskType;
                    dr["C003"] = TaskItems.Status;
                    dr["C004"] = TaskItems.Creator;
                    dr["C005"] = TaskItems.CreatorName;
                    dr["C006"] = TaskItems.IsTaskShare;
                    dr["C007"] = TaskItems.IsTaskAVGAssign;
                    dr["C008"] = TaskItems.IsDisposed;
                    if (!string.IsNullOrWhiteSpace(TaskItems.QMIDOne))
                    {
                        dr["C009"] = TaskItems.QMIDOne;
                    }
                    if (!string.IsNullOrWhiteSpace(TaskItems.QMIDTwo))
                    {
                        dr["C010"] = TaskItems.QMIDTwo;
                    }
                    if (!string.IsNullOrWhiteSpace(TaskItems.QMIDThree))
                    {
                        dr["C011"] = TaskItems.QMIDThree;
                    }
                    dr["C012"] = TaskItems.OrgTenantID;
                    dr["C013"] = TaskItems.QueryID;
                    dr["C014"] = TaskItems.FrequencyID;
                    dr["C015"] = TaskItems.IsTaskPeriod;
                    dr["C016"] = TaskItems.IsDownGet;
                    dr["C017"] = TaskItems.AutoTaskName;
                    dr["C018"] = TaskItems.AutoTaskDesc;
                    dr["C020"] = TaskItems.TaskDeadline;
                    TaskDataSet.Tables[0].Rows.Add(dr);
                }
                if(listParams[1]=="T")//修改
                {
                    dr = TaskDataSet.Tables[0].Select(string.Format("C001 = {0}", TaskItems.TaskSettingID)).FirstOrDefault();
                    dr["C002"] = TaskItems.TaskType;
                    dr["C003"] = TaskItems.Status;
                    dr["C004"] = TaskItems.Creator;
                    dr["C005"] = TaskItems.CreatorName;
                    dr["C006"] = TaskItems.IsTaskShare;
                    dr["C007"] = TaskItems.IsTaskAVGAssign;
                    dr["C008"] = TaskItems.IsDisposed;
                    if (!string.IsNullOrWhiteSpace(TaskItems.QMIDOne))
                    {
                        dr["C009"] = TaskItems.QMIDOne;
                    }
                    else
                    {
                        dr["C009"] = string.Empty;
                    }
                    if (!string.IsNullOrWhiteSpace(TaskItems.QMIDTwo))
                    {
                        dr["C010"] = TaskItems.QMIDTwo;
                    }
                    else
                    {
                        dr["C010"] = string.Empty;
                    }
                    if (!string.IsNullOrWhiteSpace(TaskItems.QMIDThree))
                    {
                        dr["C011"] = TaskItems.QMIDThree;
                    }
                    else
                    {
                        dr["C011"] = string.Empty;
                    }
                    dr["C012"] = TaskItems.OrgTenantID;
                    dr["C013"] = TaskItems.QueryID;
                    dr["C014"] = TaskItems.FrequencyID;
                    dr["C015"] = TaskItems.IsTaskPeriod;
                    dr["C016"] = TaskItems.IsDownGet;
                    dr["C017"] = TaskItems.AutoTaskName;
                    dr["C018"] = TaskItems.AutoTaskDesc;
                    dr["C020"] = TaskItems.TaskDeadline;
                }
                TaskAdapter.Update(TaskDataSet);
                TaskDataSet.AcceptChanges();

                #endregion

                #region 运行周期 T_31_026
                                
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_026_{0} ", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        FreqAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(FreqAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_026_{0} ", rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        FreqAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(FreqAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || FreqAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                DataSet FreqDataSet = new DataSet();
                FreqAdapter.Fill(FreqDataSet);
                if(listParams[1]=="F")
                {
                    dr = FreqDataSet.Tables[0].NewRow();
                    dr["C001"] = TaskItems.FrequencyID;
                    dr["C002"] = TaskItems.RunFreq;
                    dr["C003"] = TaskItems.DayTime;
                    if(TaskItems.DayOfWeek>0)
                    {
                        dr["C004"] = TaskItems.DayOfWeek;
                    }
                    if(TaskItems.DayOfMonth>0)
                    {
                        dr["C005"] = TaskItems.DayOfMonth;
                    }
                    dr["C011"] = TaskItems.IsUniteSetOfSeason;
                    if (TaskItems.UniteSetSeason > 0)
                    {
                        dr["C012"] = TaskItems.DayOfMonth;
                    }
                    if (TaskItems.DayOfYear > 0)
                    {
                        dr["C017"] = TaskItems.DayOfYear;
                    }
                    FreqDataSet.Tables[0].Rows.Add(dr);
                }
                if(listParams[1]=="T")
                {
                    dr = FreqDataSet.Tables[0].Select(string.Format("C001 = {0}", TaskItems.FrequencyID)).FirstOrDefault();
                    dr["C002"] = TaskItems.RunFreq;
                    dr["C003"] = TaskItems.DayTime;
                    if (TaskItems.DayOfWeek > 0)
                    {
                        dr["C004"] = TaskItems.DayOfWeek;
                    }
                    if (TaskItems.DayOfMonth > 0)
                    {
                        dr["C005"] = TaskItems.DayOfMonth;
                    }
                    dr["C011"] = TaskItems.IsUniteSetOfSeason;
                    if (TaskItems.UniteSetSeason > 0)
                    {
                        dr["C012"] = TaskItems.DayOfMonth;
                    }
                    if (TaskItems.DayOfYear > 0)
                    {
                        dr["C017"] = TaskItems.DayOfYear;
                    }
                }
                FreqAdapter.Update(FreqDataSet);
                FreqDataSet.AcceptChanges();

                #endregion

                #region 时长比率 T_31_048  无论新建还是更新数据都删除原来的数据，再插入新数据

                //如果传入的RateItem>0 但是listParams.Count<4 就是修改数据并且删除了时长比率的数据，关闭了按钮 那么就删除该任务ID对应的时长比率
                int RateCounts = Convert.ToInt32(listParams[2]);

                if (RateCounts>=0)
               {
                   switch (session.DBType)
                   {
                       case 2:
                           strSql = string.Format("DELETE from T_31_048_{0} where c001={1};SELECT * FROM T_31_048_{0} ", rentToken, TaskItems.TaskSettingID);
                           optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                           if (!optReturn.Result)
                           {
                               return optReturn;
                           }
                           objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                           RateAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                           objCmdBuilder = MssqlOperation.GetCommandBuilder(RateAdapter);
                           break;
                       case 3:
                           strSql = string.Format("DELETE from T_31_048_{0} where c001={1}", rentToken, TaskItems.TaskSettingID);
                           optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);//begin end;在oracle中只能做删除、更新操作，查询操作需要设置一个可以返回的变量值
                           if (!optReturn.Result)
                           {
                               return optReturn;
                           }
                           strSql = string.Format("SELECT * FROM T_31_048_{0} ", rentToken, TaskItems.TaskSettingID);
                           optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                           if (!optReturn.Result)
                           {
                               return optReturn;
                           }
                           objConn = OracleOperation.GetConnection(session.DBConnectionString);
                           RateAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                           objCmdBuilder = OracleOperation.GetCommandBuilder(RateAdapter);
                           break;
                       default:
                           optReturn.Result = false;
                           optReturn.Code = Defines.RET_PARAM_INVALID;
                           optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                           return optReturn;
                   }
                    if(listParams.Count<4)
                    {
                        return optReturn;
                    }
                   if (objConn == null || RateAdapter == null || objCmdBuilder == null)
                   {
                       optReturn.Result = false;
                       optReturn.Code = Defines.RET_OBJECT_NULL;
                       optReturn.Message = string.Format("Db object is null");
                       return optReturn;
                   }
                   objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                   objCmdBuilder.SetAllValues = false;
                   DataSet RateDataSet = new DataSet();
                   RateAdapter.Fill(RateDataSet);
                   for (int i = 0; i < RateCounts;i++ )
                   {
                       string strRateItem = listParams[3 + i];
                       optReturn = XMLHelper.DeserializeObject<TaskDurationRate>(strRateItem);
                       if (!optReturn.Result)
                       {
                           return optReturn;
                       }
                       TaskDurationRate TaskRate = optReturn.Data as TaskDurationRate;
                       if (TaskRate == null)
                       {
                           optReturn.Result = false;
                           optReturn.Code = Defines.RET_OBJECT_NULL;
                           optReturn.Message = string.Format("TaskDurationRate is null");
                           return optReturn;
                       }
                       dr = RateDataSet.Tables[0].NewRow();
                       dr["C001"] = TaskRate.TaskSettingID;
                       dr["C002"] = TaskRate.DurationMin;
                       dr["C003"] = TaskRate.DurationMax;
                       dr["C004"] = TaskRate.Rate;
                       RateDataSet.Tables[0].Rows.Add(dr);
                   }
                   RateAdapter.Update(RateDataSet);
                   RateDataSet.AcceptChanges();
               }
                #endregion

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
        /// 获取查询条件参数
        /// </summary>
        private OperationReturn GetQueryDetail(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM T_31_024_{0} ", session.RentInfo.Token);
                switch(session.DBType)
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
                    QuerySettingItems Item = new QuerySettingItems();
                    Item.QuerySettingID = Convert.ToInt64(dr["C001"]);
                    Item.IsUsed = dr["C002"].ToString();
                    Item.QuerySettingName = dr["C003"].ToString();
                    Item.QueryStartTime = Convert.ToDateTime(dr["C004"]).ToString("yyyy/MM/dd HH:mm:ss");//ToLocalTime().
                    Item.QueryStopTime = Convert.ToDateTime(dr["C005"]).ToString("yyyy/MM/dd HH:mm:ss");//ToLocalTime().
                    Item.IsRecentTime = dr["C006"].ToString();
                    Item.RecentTimeType = GetStringFDB(dr["C007"].ToString());
                    Item.RecentTimeNum = GetStringFDB(dr["C008"].ToString());
                    Item.StartRecordTime = Convert.ToDateTime(dr["C009"]).ToString("yyyy/MM/dd HH:mm:ss");//ToLocalTime().
                    Item.StopRecordTime = Convert.ToDateTime(dr["C010"]).ToString("yyyy/MM/dd HH:mm:ss");//ToLocalTime().
                    Item.CallDirection = dr["C011"].ToString();
                    Item.HasScreen = GetStringFDB(dr["C012"].ToString());
                    Item.IsEmtion = GetStringFDB(dr["C013"].ToString());
                    Item.IsKeyWord = GetStringFDB(dr["C014"].ToString());
                    Item.IsBookMark = GetStringFDB(dr["C015"].ToString());
                    Item.IsScore = GetStringFDB(dr["C016"].ToString());
                    Item.AgentAssType= Convert.ToInt32(dr["C017"]);
                    Item.AgentAssNum = GetStringFDB(dr["C018"].ToString());
                    Item.AgentAssRate = GetStringFDB(dr["C019"].ToString());
                    Item.AgentsIDOne = dr["C020"].ToString();
                    Item.AgentsIDTwo = GetStringFDB(dr["C021"].ToString());
                    Item.AgentsIDThree = GetStringFDB(dr["C022"].ToString());
                    Item.DurationMin = Convert.ToInt32(dr["C023"]);
                    Item.DurationMax = Convert.ToInt32(dr["C024"]);
                    Item.ABCDSql = GetStringFDB(dr["C025"].ToString());
                    Item.ABCDSetting = GetStringFDB(dr["C026"].ToString());

                    optReturn = XMLHelper.SeriallizeObject(Item);
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
        /// 获取任务设置详情
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// <returns></returns>
        private OperationReturn GetTaskDetail(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                DataSet objDataSet;
                string strSql = string.Format("SELECT	T23.*, T26.C002 RunTime,T26.C003 DayTime,T26.C004 DayOfWeek,T26.C005 DayOfMonth,T26.C012 DayOfSeason,T26.C017 DayOfYear,T24.C003 QueryName" +
                " FROM T_31_023_{0} T23 LEFT JOIN T_31_026_{0} T26 ON T23.C014 = T26.C001 LEFT JOIN T_31_024_00000 T24 ON T23.C013=T24.C001", session.RentInfo.Token);
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
                    TaskSettingItems Item = new TaskSettingItems();
                    Item.TaskSettingID = Convert.ToInt64(dr["C001"]);
                    Item.TaskType = Convert.ToInt32(dr["C002"]);
                    Item.Status = dr["C003"].ToString();
                    Item.Creator = Convert.ToInt64(dr["C004"]);
                    Item.CreatorName = dr["C005"].ToString();
                    Item.IsTaskShare = dr["C006"].ToString();
                    Item.IsTaskAVGAssign = dr["C007"].ToString();
                    Item.IsDisposed = dr["C008"].ToString();
                    Item.QMIDOne = GetStringFDB(dr["C009"].ToString());
                    Item.QMIDTwo = GetStringFDB(dr["C010"].ToString());
                    Item.QMIDThree = GetStringFDB(dr["C011"].ToString());
                    Item.OrgTenantID = Convert.ToInt64(dr["C012"]);
                    Item.QueryID = Convert.ToInt64(dr["C013"]);
                    Item.FrequencyID = Convert.ToInt64(dr["C014"]);
                    Item.AutoTaskName = dr["C017"].ToString();
                    Item.AutoTaskDesc = GetStringFDB(dr["C018"].ToString());
                    Item.TaskDeadline = Convert.ToInt32(dr["C020"]);
                    Item.QueryName = dr["QueryName"].ToString();
                    Item.RunFreq = dr["RunTime"].ToString();
                    Item.DayTime = dr["DayTime"].ToString();
                    switch(Item.RunFreq)
                    {
                        case "W":
                            Item.DayOfWeek = Convert.ToInt32(dr["DayOfWeek"]);
                            break;
                        case "M":
                            Item.DayOfMonth = Convert.ToInt32(dr["DayOfMonth"]);
                            break;
                        case "S":
                            Item.UniteSetSeason = Convert.ToInt32(dr["DayOfSeason"]);
                            break;
                        case "Y":
                            Item.DayOfYear = Convert.ToInt32(dr["DayOfYear"]);
                            break;
                    }

                    optReturn = XMLHelper.SeriallizeObject(Item);
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

        private OperationReturn GetRateDetail(SessionInfo session, List<string> listParams)
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
                string strSql = string.Format("SELECT * FROM T_31_048_{0} where C001='{1}'", session.RentInfo.Token,listParams[0]);
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
                    TaskDurationRate Item = new TaskDurationRate();
                    Item.TaskSettingID = Convert.ToInt64(dr["C001"]);
                    Item.DurationMin = Convert.ToInt32(dr["C002"]);
                    Item.DurationMax = Convert.ToInt32(dr["C003"]);
                    Item.Rate = Convert.ToDouble(dr["C004"]);

                    optReturn = XMLHelper.SeriallizeObject(Item);
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

        private OperationReturn DeleteDBO(SessionInfo session, List<string> listParams)
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
                string rentToken = session.RentInfo.Token;
                string StrCondition;
                DataSet objDataSet=null;
                if(listParams.Count==1)
                {
                    StrCondition = string.Format("SELECT * FROM T_31_023_{0} where C013='{1}'", rentToken, listParams[0]);
                    switch (session.DBType)
                    {
                        //MSSQL
                        case 2:
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, StrCondition);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        //ORCL
                        case 3:
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, StrCondition);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                    }
                }

                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        if (listParams.Count == 1)
                        {
                            int temp = objDataSet.Tables.Count;
                            if (objDataSet.Tables[0].Rows.Count>0)//被使用
                            {
                                optReturn.Message = S3107Consts.HadUse;
                                break;
                            }
                            else
                            {
                                StrCondition = string.Format("Delete From T_31_024_{0} where C001 ={1}", rentToken, listParams[0]);
                            }
                            temp = 1;
                        }
                        else
                        {
                            StrCondition = string.Format("Delete From T_31_023_{0} where C001 ={1}; Delete From T_31_026_{0} where C001={2};Delete From T_31_048_{0} where c001 ={1}", rentToken, listParams[0],listParams[1]);
                        }
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, StrCondition);
                        break;
                    //ORCL
                    case 3:
                        if (listParams.Count == 1)
                        {
                            StrCondition = string.Format("Delete From T_31_024_{0} where C001 ={1}",rentToken,listParams[0]);
                        }
                        else 
                        {
                            StrCondition = string.Format("begin Delete From T_31_023_{0} where C001 ={1}; Delete From T_31_026_{0} where C001={2};Delete From T_31_048_{0} where c001 ={1};END;", rentToken, listParams[0], listParams[1]);
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

        private OperationReturn GetABCD(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      ABCD的统计ID
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strSql = string.Format("SELECT T352.*,T106.C002 AS OrgName FROM T_31_052_{1} T352,T_11_006_{1} T106 WHERE T352.C002={0} AND T106.C001=T352.C003", listParams[0],session.RentInfo.Token);
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
                    ABCD_OrgSkillGroup item = new ABCD_OrgSkillGroup();
                    item.OrgSkillGroupID = long.Parse(dr["C003"].ToString());
                    item.ParamID = long.Parse(dr["C002"].ToString());
                    item.InColumn = int.Parse(dr["C008"].ToString());
                    item.OrgSkillGroupName = DecryptNamesFromDB(dr["OrgName"].ToString());
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


        #region 数据库传出null值的处理
        /// <summary>
        /// 对于数据库中的值可能为null值的处理
        /// </summary>
        private string GetStringFDB(string temp)
        {
            if (string.IsNullOrWhiteSpace(temp)) temp = string.Empty;
            return temp;
        }


        #endregion
    }
}
