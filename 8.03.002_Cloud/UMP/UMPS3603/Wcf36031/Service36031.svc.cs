using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.ServiceModel.Activation;
using System.Text;
using Common3603;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;

namespace Wcf36031
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service36031”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service36031.svc 或 Service36031.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service36031 : IService36031
    {
        private string _mStrUmpPath = null;

        private string StringToHexString(string s, Encoding encode)
        {
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
            {
                result += Convert.ToString(b[i], 16);
            }
            return result;
        }

        private string ParsedString(string str)
        {
            string strTemp = string.Empty;
            if (str == null)
                return str;

            string[] strArray = str.Split(new char[] { '\'' });
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray.Length == 1)
                    return str;
                if (i == 0)
                    strTemp = strArray[i];
                else
                    strTemp += "''" + strArray[i];
            }
            return strTemp;
        }

        /// <summary>
        /// 获取服务端UMP安装目录
        /// </summary>
        private OperationReturn GetUMPSetupPath()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string umpPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory);//根树地址
                umpPath = umpPath.Substring(0, umpPath.LastIndexOf("\\"));
                umpPath = umpPath.Substring(0, umpPath.LastIndexOf("\\"));
                umpPath = Path.Combine(umpPath, "MediaData");
                if (!Directory.Exists(umpPath))
                {
                    Directory.CreateDirectory(umpPath);
                }
                optReturn.StringValue = umpPath;
                _mStrUmpPath = umpPath;
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

        private bool AnalyzingFieldIsNull(string strString)
        {
            if (strString == "")
                return false;
            return true;
        }

        private string DecryptFromDB(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptFromClient(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private OperationReturn OptGetPapers(SessionInfo session, List<string> listParams)
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
                string strSql = string.Format("SELECT * FROM T_36_023_{0} WHERE C011='{1}'", session.RentInfo.Token, session.UserInfo.UserID);
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
                List<string> listReturn = new List<string>();
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
                    CPaperParam param = new CPaperParam();
                    param.LongNum = Convert.ToInt64(dr["C001"]);
                    param.StrName = dr["C002"].ToString();
                    param.StrDescribe = dr["C003"].ToString();
                    param.CharType = Convert.ToChar(dr["C004"]);
                    if (AnalyzingFieldIsNull(dr["C005"].ToString()))
                        param.LongSource = Convert.ToInt64(dr["C005"]);
                    if (AnalyzingFieldIsNull(dr["C006"].ToString()))
                        param.IntWeight = Convert.ToInt32(dr["C006"]);
                    param.IntQuestionNum = Convert.ToInt32(dr["C007"]);
                    param.IntScores = Convert.ToInt32(dr["C008"]);
                    param.IntPassMark = Convert.ToInt32(dr["C009"]);
                    param.IntTestTime = Convert.ToInt32(dr["C010"]);
                    param.LongEditorId = Convert.ToInt64(dr["C011"]);
                    param.StrEditor = dr["C012"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C013"]).ToString("yyyy/MM/dd HH:mm:ss");
                    if (AnalyzingFieldIsNull(dr["C014"].ToString()))
                        param.IntUsed = Convert.ToInt16(dr["C014"]);
                    if (AnalyzingFieldIsNull(dr["C015"].ToString()))
                        param.IntAudit = Convert.ToInt16(dr["C015"]);
                    if (AnalyzingFieldIsNull(dr["C016"].ToString()))
                        param.LongVerifierId = Convert.ToInt64(dr["C016"]);
                    param.StrVerifier = dr["C017"].ToString();
                    param.IntIntegrity = Convert.ToInt16(dr["C018"]);
                    optReturn = XMLHelper.SeriallizeObject(param);
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

        private OperationReturn OptSearchPapers(SessionInfo session, List<string> listParams)
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
                string strParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<string>(strParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                if (strSql == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AddPaperParam is null");
                    return optReturn;
                }

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
                List<string> listReturn = new List<string>();
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
                    CPaperParam param = new CPaperParam();
                    param.LongNum = Convert.ToInt64(dr["C001"]);
                    param.StrName = dr["C002"].ToString();
                    param.StrDescribe = dr["C003"].ToString();
                    param.CharType = Convert.ToChar(dr["C004"]);
                    if (AnalyzingFieldIsNull(dr["C005"].ToString()))
                        param.LongSource = Convert.ToInt64(dr["C005"]);
                    if (AnalyzingFieldIsNull(dr["C006"].ToString()))
                        param.IntWeight = Convert.ToInt32(dr["C006"]);
                    param.IntQuestionNum = Convert.ToInt32(dr["C007"]);
                    param.IntScores = Convert.ToInt32(dr["C008"]);
                    param.IntPassMark = Convert.ToInt32(dr["C009"]);
                    param.IntTestTime = Convert.ToInt32(dr["C010"]);
                    param.LongEditorId = Convert.ToInt64(dr["C011"]);
                    param.StrEditor = dr["C012"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C013"]).ToString("yyyy/MM/dd HH:mm:ss");
                    if (AnalyzingFieldIsNull(dr["C014"].ToString()))
                        param.IntUsed = Convert.ToInt16(dr["C014"]);
                    if (AnalyzingFieldIsNull(dr["C015"].ToString()))
                        param.IntAudit = Convert.ToInt16(dr["C015"]);
                    if (AnalyzingFieldIsNull(dr["C016"].ToString()))
                        param.LongVerifierId = Convert.ToInt64(dr["C016"]);
                    param.StrVerifier = dr["C017"].ToString();
                    param.IntIntegrity = Convert.ToInt16(dr["C018"]);
                    optReturn = XMLHelper.SeriallizeObject(param);
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

        private OperationReturn OptGetSerialID(SessionInfo session, List<string> listParams)
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
                string moduleId = listParams[0];
                string resourceId = listParams[1];
                string dateFormat = listParams[2];
                string rentToken = session.RentInfo.Token;
                string strSerialId = string.Empty;
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
                        mssqlParameters[0].Value = moduleId;
                        mssqlParameters[1].Value = resourceId;
                        mssqlParameters[2].Value = rentToken;
                        mssqlParameters[3].Value = dateFormat;
                        mssqlParameters[4].Value = strSerialId;
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
                            strSerialId = mssqlParameters[4].Value.ToString();
                            optReturn.Data = strSerialId;
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
                        orclParameters[0].Value = moduleId;
                        orclParameters[1].Value = resourceId;
                        orclParameters[2].Value = rentToken;
                        orclParameters[3].Value = dateFormat;
                        orclParameters[4].Value = strSerialId;
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
                            strSerialId = orclParameters[4].Value.ToString();
                            optReturn.Data = strSerialId;
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

        private OperationReturn OptAddTestInfo(SessionInfo session, List<string> listParams)
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
                string strTestInformation = listParams[0];
                optReturn = XMLHelper.DeserializeObject<TestInfoParam>(strTestInformation);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                TestInfoParam testInformation = optReturn.Data as TestInfoParam;
                if (testInformation == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AddPaperParam is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_36_025_{0}", rentToken);
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
                DataRow dr = objDataSet.Tables[0].NewRow();
                dr["c001"] = testInformation.LongTestNum;
                dr["c002"] = testInformation.LongPaperNum;
                dr["c003"] = testInformation.StrPaperName;
                dr["c004"] = testInformation.StrExplain;
                dr["c005"] = testInformation.StrTestTime;
                dr["c006"] = testInformation.LongEditorId;
                dr["c007"] = testInformation.StrEditor;
                dr["c008"] = testInformation.StrDateTime;
                dr["c009"] = "N";
                objDataSet.Tables[0].Rows.Add(dr);
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();  

                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("update T_36_023_{0} set c014 = 1 WHERE c001 = '{1}'",
                            session.RentInfo.Token, testInformation.LongPaperNum);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("update T_36_023_{0} set c014 = 1 WHERE c001 = '{1}'",
                            session.RentInfo.Token, testInformation.LongPaperNum);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
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

        private OperationReturn OptGetTestInfo(SessionInfo session, List<string> listParams)
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
                string strParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<string>(strParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                if (strSql == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }
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
                List<string> listReturn = new List<string>();
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
                    TestInfoParam param = new TestInfoParam();
                    param.LongTestNum = Convert.ToInt64(dr["C001"]);
                    param.LongPaperNum = Convert.ToInt64(dr["C002"]);
                    param.StrPaperName = dr["C003"].ToString();
                    param.StrExplain = dr["C004"].ToString();
                    param.StrTestTime = dr["C005"].ToString();
                    param.LongEditorId = Convert.ToInt64(dr["C006"]);
                    param.StrEditor = dr["C007"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C008"]).ToString("yyyy/MM/dd HH:mm:ss");
                    param.StrTestStatue = dr["C009"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(param);
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
                    strName = DecryptFromDB(strName);
                    strFullName = DecryptFromDB(strFullName);
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

        private OperationReturn SetTestUserListT11201(SessionInfo session, List<string> listParams)
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
                string scoreSheetID, strCount;
                int intCount;
                //0     scoreSheetID
                //1     count
                //...   object check state
                scoreSheetID = listParams[0];
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
                    optReturn.Message = "Object count invalid";
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
                        //strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1}  AND C003 LIKE '102%' "
                        //    , rentToken
                        //    , scoreSheetID);
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1}  AND C003 >=1030000000000000000 AND C003<1060000000000000000"
                            , rentToken
                            , scoreSheetID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1} AND C003 >=1030000000000000000 AND C003<1060000000000000000"
                            , rentToken
                            , scoreSheetID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = "Database type not support";
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Db object is null";
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
                            optReturn.Message = "ListObjectState invalid";
                            break;
                        }
                        string objID = listObjectState[0];
                        string isChecked = listObjectState[1];
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("c003 = {0}", objID));
                        if (isChecked == "1")
                        {
                            //不存在，则插入
                            if (drs.Length <= 0)
                            {
                                DataRow newRow = objDataSet.Tables[0].NewRow();
                                newRow["C001"] = 0;
                                newRow["C002"] = 0;
                                newRow["C003"] = Convert.ToInt64(objID);
                                newRow["C004"] = Convert.ToInt64(scoreSheetID);
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

        private OperationReturn SetTestUserListT36036(SessionInfo session, List<string> listParams)
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
                string strTestUserInfo = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<TestUserParam>>(strTestUserInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<TestUserParam> lstTestUserParam = optReturn.Data as List<TestUserParam>;
                if (lstTestUserParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AddPaperParam is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                foreach (var testUserParam in lstTestUserParam)
                {
                    string strSql;
                    DataSet objDataSet;
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * from T_36_026_{0} where c001 = '{1}' and c002='{2}'",
                                rentToken, testUserParam.LongTestNum, testUserParam.LongTestUserNum);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql = string.Format("SELECT * from T_36_026_{0} where c001 = '{1}' and c002='{2}'",
                                rentToken, testUserParam.LongTestNum, testUserParam.LongTestUserNum);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
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
                    int iCount = objDataSet.Tables[0].Rows.Count;
                    if (iCount == 0)
                    {
                        if (testUserParam.IntEable == 1)
                        {
                            strSql = string.Format("SELECT * FROM T_36_026_{0}", rentToken);
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
                            objDataSet = new DataSet();
                            objAdapter.Fill(objDataSet);
                            DataRow dr = objDataSet.Tables[0].NewRow();
                            dr["c001"] = testUserParam.LongTestNum;
                            dr["c002"] = testUserParam.LongTestUserNum;
                            dr["c003"] = testUserParam.StrTestUserName;
                            dr["c004"] = testUserParam.LongPaperNum;
                            dr["c005"] = testUserParam.StrPaperName;
                            dr["c009"] = testUserParam.StrTestStatue;
                            objDataSet.Tables[0].Rows.Add(dr);
                            objAdapter.Update(objDataSet);
                            objDataSet.AcceptChanges();  
                        }
                    }
                    else
                    {
                        if (testUserParam.IntEable == 0)
                        {
                            switch (session.DBType)
                            {
                                case 2:
                                    strSql = string.Format("DELETE From T_36_026_{0} WHERE c001='{1}' and c002 = '{2}' and c004='{3}'",
                                        rentToken, testUserParam.LongTestNum, testUserParam.LongTestUserNum, testUserParam.LongPaperNum);
                                    optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                                    if (!optReturn.Result)
                                    {
                                        return optReturn;
                                    }
                                    break;
                                case 3:
                                    strSql = string.Format("DELETE From T_36_026_{0} WHERE c001='{1}' and c002 = '{2}' and c004='{3}'",
                                        rentToken, testUserParam.LongTestNum, testUserParam.LongTestUserNum, testUserParam.LongPaperNum);
                                    optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                                    if (!optReturn.Result)
                                    {
                                        return optReturn;
                                    }
                                    break;
                                default:
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_PARAM_INVALID;
                                    optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                                    return optReturn;
                            }
                        }
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

        private OperationReturn GetTestUserList(SessionInfo session, List<string> listParams)
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
                string rentToken = session.RentInfo.Token;
                //ListParam
                //0     scoreSheetID
                string paperNum = listParams[0];
                string strSql = string.Empty;
                DataSet objDataSet = null;
                switch (session.DBType)
                {
                    //MSSQL--("SELECT * FROM T_11_201_{0} WHERE C004 = {1} AND C003 LIKE '103%' ",
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1} AND C003 >=1030000000000000000 AND C003<1060000000000000000",
                            rentToken,
                            paperNum);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1} AND C003 >=1030000000000000000 AND C003<1060000000000000000",
                          rentToken,
                          paperNum);
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
                    string strID;
                    strID = dr["C003"].ToString();
                    listUsers.Add(strID);
                }
                optReturn.Data = listUsers;
                optReturn.Message = strSql;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn OptGetTestUserInfo(SessionInfo session, List<string> listParams)
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
                string strTestUserInfo = listParams[0];
                optReturn = XMLHelper.DeserializeObject<TestInfoParam>(strTestUserInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                TestInfoParam testInfoParam = optReturn.Data as TestInfoParam;
                if (testInfoParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AddPaperParam is null");
                    return optReturn;
                }
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM T_36_026_{0} WHERE C001='{1}'", session.RentInfo.Token, testInfoParam.LongTestNum);
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
                List<string> listReturn = new List<string>();
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
                    TestUserParam param = new TestUserParam();
                    param.LongTestNum = Convert.ToInt64(dr["C001"]);
                    param.LongTestUserNum = Convert.ToInt64(dr["C002"]);
                    param.StrTestUserName = dr["C003"].ToString();
                    param.LongPaperNum = Convert.ToInt64(dr["C004"]);
                    param.StrPaperName = dr["C005"].ToString();
                    if (AnalyzingFieldIsNull(dr["C006"].ToString()))
                        param.IntScore = Convert.ToInt32(dr["C006"]);
                    param.StrStartTime = dr["C007"].ToString();
                    param.StrEndTime = dr["C008"].ToString();
                    param.StrTestStatue = dr["C009"].ToString();                   
                    optReturn = XMLHelper.SeriallizeObject(param);
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

        private OperationReturn OptDeleteTestInfo(SessionInfo session, List<string> listParams)
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

                string strTestInfo = listParams[0];
                optReturn = XMLHelper.DeserializeObject<TestInfoParam>(strTestInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var testInfoParam = optReturn.Data as TestInfoParam;
                if (testInfoParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listPaperQuestions is null");
                    return optReturn;
                }

                string strSql = string.Format("delete T_36_025_{0} Where C001='{1}' and C002='{2}'", session.RentInfo.Token, testInfoParam.LongTestNum, testInfoParam.LongPaperNum);
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result) return optReturn;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result) return optReturn;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }

                strSql = string.Format("delete T_36_026_{0} Where C001='{1}' and C004='{2}'", session.RentInfo.Token, testInfoParam.LongTestNum, testInfoParam.LongPaperNum);
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result) return optReturn;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result) return optReturn;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }

                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("update T_36_023_{0} set c014 = 0 WHERE c001 = '{1}'",
                            session.RentInfo.Token, testInfoParam.LongPaperNum);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("update T_36_023_{0} set c014 = 0 WHERE c001 = '{1}'",
                            session.RentInfo.Token, testInfoParam.LongPaperNum);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }

                optReturn.Message = S3603Consts.DeleteSuccess;
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

        private OperationReturn OptLoadFiles(SessionInfo session, List<string> listParams)
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

                string struestionParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CPaperQuestionParam>(struestionParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var questionParam = optReturn.Data as CPaperQuestionParam;
                if (questionParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listPaperQuestions is null");
                    return optReturn;
                }

                string[] nameTemp = questionParam.StrAccessoryName.Split(new char[] { '.' });
                string name = null;
                for (int i = 0; i < nameTemp.Length - 1; i++)
                {
                    name += nameTemp[i].ToString();
                }
                name += StringToHexString(questionParam.StrAccessoryName, Encoding.UTF8) + "." + questionParam.StrAccessoryType;
                string targetFile = _mStrUmpPath + "/" + name;
                if (!File.Exists(targetFile))
                {
                    File.Copy(questionParam.StrAccessoryPath, targetFile, true);
                }
                optReturn.StringValue = "MediaData/" + name;
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

        private OperationReturn GetPaperQuestion(SessionInfo session, List<string> listParams)
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
                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CPaperParam>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var paperParam = optReturn.Data as CPaperParam;
                if (paperParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM T_36_024_{0} WHERE C001='{1}'", session.RentInfo.Token, paperParam.LongNum);
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
                List<string> listReturn = new List<string>();
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
                    CPaperQuestionParam param = new CPaperQuestionParam();
                    param.LongPaperNum = Convert.ToInt64(dr["C001"]);
                    param.LongQuestionNum = Convert.ToInt64(dr["C002"]);
                    param.IntQuestionType = Convert.ToInt32(dr["C003"]);
                    param.StrQuestionsContect = dr["C004"].ToString();
                    param.EnableChange = Convert.ToInt16(dr["C005"]);
                    param.StrShortAnswerAnswer = dr["C006"].ToString();
                    param.StrAnswerOne = dr["C007"].ToString();
                    param.StrAnswerTwo = dr["C008"].ToString();
                    param.StrAnswerThree = dr["C009"].ToString();
                    param.StrAnswerFour = dr["C010"].ToString();
                    param.StrAnswerFive = dr["C011"].ToString();
                    param.StrAnswerSix = dr["C012"].ToString();
                    param.CorrectAnswerOne = dr["C013"].ToString();
                    param.CorrectAnswerTwo = dr["C014"].ToString();
                    param.CorrectAnswerThree = dr["C015"].ToString();
                    param.CorrectAnswerFour = dr["C016"].ToString();
                    param.CorrectAnswerFive = dr["C017"].ToString();
                    param.CorrectAnswerSix = dr["C018"].ToString();
                    param.IntScore = Convert.ToInt32(dr["C019"]);
                    param.StrAccessoryType = dr["C021"].ToString();
                    param.StrAccessoryName = dr["C022"].ToString();
                    param.StrAccessoryPath = dr["C023"].ToString();
                    param.StrQuestionCategory = dr["C024"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(param);
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

        public WebReturn UmpTaskOperation(WebRequest webRequest)
        {
            var webReturn = new WebReturn();
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
                    case (int)S3603Codes.OptGetPapers:
                        optReturn = OptGetPapers(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSGetSerialID:
                        optReturn = OptGetSerialID(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3603Codes.OptAddTestInfo:
                        optReturn = OptAddTestInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3603Codes.OptGetTestInfo:
                        optReturn = OptGetTestInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3603Codes.OptGetCtrolAgent:
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
                    case (int)S3603Codes.OptSetTestUserT11201:
                        optReturn = SetTestUserListT11201(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message + optReturn.Data;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3603Codes.OptSetTestUserT36036:
                        optReturn = SetTestUserListT36036(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message + optReturn.Data;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3603Codes.OptGetTestUserList:
                        optReturn = GetTestUserList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message + optReturn.Data;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3603Codes.OptGetTestUserInfo:
                        optReturn = OptGetTestUserInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message + optReturn.Data;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3603Codes.OptDeleteTestInfo:
                        optReturn = OptDeleteTestInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3603Codes.OptSearchPapers:
                        optReturn = OptSearchPapers(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3603Codes.OptLoadFile:
                        optReturn = GetUMPSetupPath();
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        optReturn = OptLoadFiles(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        break;
                    case (int)S3603Codes.GetPaperQuestions:
                        optReturn = GetPaperQuestion(session, webRequest.ListData);
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
    }
}
