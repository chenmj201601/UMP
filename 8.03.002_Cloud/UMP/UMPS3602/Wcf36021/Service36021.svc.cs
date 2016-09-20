using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.Common;
using System.ServiceModel.Activation;
using System.Text;
using Common3602;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;


namespace Wcf36021
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service36021 : IService36021
    {
        private readonly List<string> _mListDeleteInfo = new List<string>();

        private string _mStrUmpPath = null;

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

        private bool AnalyzingFieldIsNull(string strString)
        {
            if (strString == "")
                return false;
            return true;
        }

        private OperationReturn OptGetQuestinCategory(SessionInfo session, List<string> listParams)
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
                    PapersCategoryParam param = new PapersCategoryParam();
                    param.LongNum = Convert.ToInt64(dr["C001"]);
                    param.StrName = dr["C002"].ToString();
                    param.LongParentNodeId = Convert.ToInt64(dr["C003"]);
                    param.StrParentNodeName = dr["C004"].ToString();
                    param.LongFounderId = Convert.ToInt64(dr["C005"]);
                    param.StrFounderName = dr["C006"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C007"]).ToString("yyyy/MM/dd HH:mm:ss");
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

        private OperationReturn OptUpdatePaper(SessionInfo session, List<string> listParams)
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
                var param = optReturn.Data as CPaperParam;
                if (param == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Paper Param is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_36_023_{0}  where c001='{1}' ", rentToken, param.LongNum);
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

                objCmdBuilder.ConflictOption = ConflictOption.CompareRowVersion;
                objCmdBuilder.SetAllValues = false;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                DataRow dr = objDataSet.Tables[0].Rows[0];
                dr.BeginEdit();
                dr["c002"] = param.StrName;
                dr["c003"] = param.StrDescribe;
                dr["c004"] = param.CharType;
                dr["c008"] = param.IntScores;
                dr["c009"] = param.IntPassMark;
                dr["c010"] = param.IntTestTime;
                dr["c018"] = param.IntIntegrity;
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

        private OperationReturn OptPaperSameName(SessionInfo session, List<string> listParams)
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
                CPaperParam paperParam = optReturn.Data as CPaperParam;
                if (paperParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }

                DataSet objDataSet;
                string strSql;
                strSql = paperParam.LongSource != 0 ? string.Format("SELECT * FROM T_36_023_{0} WHERE C002='{1}' and c005='{2}' ", session.RentInfo.Token, ParsedString(paperParam.StrName), paperParam.LongSource) : string.Format("SELECT * FROM T_36_023_{0} WHERE C002='{1}'", session.RentInfo.Token,ParsedString( paperParam.StrName));
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
                if (objDataSet.Tables[0].Rows.Count > 0)
                {
                    optReturn.Message = S3602Consts.PaperExist;
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

        private OperationReturn OptAddPaper(SessionInfo session, List<string> listParams)
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
                string strCAddPaper = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CPaperParam>(strCAddPaper);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                CPaperParam cAddPaper = optReturn.Data as CPaperParam;
                if (cAddPaper == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AddPaperParam is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_36_023_{0}", rentToken);
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
                dr["c001"] = cAddPaper.LongNum;
                dr["c002"] = cAddPaper.StrName;
                if (!string.IsNullOrEmpty(cAddPaper.StrDescribe))
                {
                    dr["c003"] = cAddPaper.StrDescribe;
                }
                dr["c004"] = cAddPaper.CharType;
                dr["c007"] = cAddPaper.IntQuestionNum;
                dr["c008"] = cAddPaper.IntScores;
                dr["c009"] = cAddPaper.IntPassMark;
                dr["c010"] = cAddPaper.IntTestTime;
                dr["c011"] = cAddPaper.LongEditorId;
                dr["c012"] = cAddPaper.StrEditor;
                dr["c013"] = cAddPaper.StrDateTime;
                dr["c018"] = 0;
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

        private OperationReturn OptDeletePaper(SessionInfo session, List<string> listParams)
        {
            _mListDeleteInfo.Clear();
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
                optReturn = XMLHelper.DeserializeObject<List<CPaperParam>>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var listPaperParam = optReturn.Data as List<CPaperParam>;
                if (listPaperParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listPaperQuestions is null");
                    return optReturn;
                }

                foreach (CPaperParam param in listPaperParam)
                {
                    string strSql = string.Format("delete T_36_023_{0} Where C001='{1}'", session.RentInfo.Token, param.LongNum);
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
                }
                optReturn.Message = S3602Consts.DeleteSuccess;
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
        private OperationReturn OptGetUMPSetupPath()
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
                    optReturn.Message = "Request param is null or count invalid";
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

        private OperationReturn OptDeletePaperAllQuestions(SessionInfo session, List<string> listParams)
        {
            _mListDeleteInfo.Clear();
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
                optReturn = XMLHelper.DeserializeObject<List<CPaperParam>>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var listPaperParam = optReturn.Data as List<CPaperParam>;
                if (listPaperParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listPaperQuestions is null");
                    return optReturn;
                }

                string strSql = string.Empty;
                List<long> lstQuestionNum = new List<long>();
                foreach (CPaperParam param in listPaperParam)
                {
                    strSql = string.Format("Select * from T_36_024_{0} Where C001='{1}'", session.RentInfo.Token,
                        param.LongNum);

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
                        lstQuestionNum.Add(Convert.ToInt64(dr["C002"]));              
                    }
                }

                foreach (var param in lstQuestionNum)
                {
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("update T_36_022_{0} set c019 = c019-1 WHERE c001 = '{1}'",
                                session.RentInfo.Token, param);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            break;
                        case 3:
                            strSql = string.Format("update T_36_022_{0} set c019 = c019-1 WHERE c001 = '{1}'",
                                session.RentInfo.Token, param);
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

                foreach (CPaperParam param in listPaperParam)
                {
                    strSql = string.Format("delete T_36_024_{0} Where C001='{1}'", session.RentInfo.Token, param.LongNum);
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
                }
                optReturn.Message = S3602Consts.DeleteSuccess;
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

        private OperationReturn OptDeletePaperQuestions(SessionInfo session, List<string> listParams)
        {
            _mListDeleteInfo.Clear();
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

                string strEditPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CPaperQuestionParam>>(strEditPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var listPaperQuestions = optReturn.Data as List<CPaperQuestionParam>;
                if (listPaperQuestions == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listPaperQuestions is null");
                    return optReturn;
                }

                string strSql;
                
                foreach (CPaperQuestionParam param in listPaperQuestions)
                {
                    strSql = string.Format("delete T_36_024_{0} Where C001='{1}' and C002 = '{2}'", session.RentInfo.Token, param.LongPaperNum, param.LongQuestionNum);
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
                }

                foreach (var param in listPaperQuestions)
                {
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("update T_36_022_{0} set c019 = c019-1 WHERE c001 = '{1}'",
                                session.RentInfo.Token, param.LongQuestionNum);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            break;
                        case 3:
                            strSql = string.Format("update T_36_022_{0} set c019 = c019-1 WHERE c001 = '{1}'",
                                session.RentInfo.Token, param.LongQuestionNum);
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

                optReturn.Message = S3602Consts.DeleteSuccess;
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

        private OperationReturn OptAddPaperQuestions(SessionInfo session, List<string> listParams)
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
                string strPaperQuestionParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CPaperQuestionParam>>(strPaperQuestionParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var listeEditPapers = optReturn.Data as List<CPaperQuestionParam>;
                if (listeEditPapers == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listeEditPapers is null");
                    return optReturn;
                }

                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_36_024_{0}", rentToken);
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

                int iCount = 0;
                foreach (var editPaper in listeEditPapers)
                {
                    iCount ++;

                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    DataRow dr = objDataSet.Tables[0].NewRow();
                    dr["c001"] = editPaper.LongPaperNum;
                    dr["c002"] = editPaper.LongQuestionNum;
                    dr["c003"] = editPaper.IntQuestionType;
                    dr["c004"] = editPaper.StrQuestionsContect;
                    dr["c005"] = editPaper.EnableChange;
                    dr["c007"] = editPaper.StrAnswerOne;
                    dr["c008"] = editPaper.StrAnswerTwo;
                    dr["c009"] = editPaper.StrAnswerThree;
                    dr["c010"] = editPaper.StrAnswerFour;
                    dr["c011"] = editPaper.StrAnswerFive;
                    dr["c012"] = editPaper.StrAnswerSix;
                    dr["c013"] = editPaper.CorrectAnswerOne;
                    dr["c014"] = editPaper.CorrectAnswerTwo;
                    dr["c015"] = editPaper.CorrectAnswerThree;
                    dr["c016"] = editPaper.CorrectAnswerFour;
                    dr["c017"] = editPaper.CorrectAnswerFive;
                    dr["c018"] = editPaper.CorrectAnswerSix;
                    dr["c019"] = editPaper.IntScore;
                    dr["c021"] = editPaper.StrAccessoryType;
                    dr["c022"] = editPaper.StrAccessoryName;
                    dr["c023"] = editPaper.StrAccessoryPath;
                    dr["c024"] = editPaper.StrQuestionCategory;
                    objDataSet.Tables[0].Rows.Add(dr);
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();  
                }

                foreach (var editPaper in listeEditPapers)
                {
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("update T_36_022_{0} set c019 = c019+1 WHERE c001 = '{1}'",
                                rentToken, editPaper.LongQuestionNum);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            break;
                        case 3:
                            strSql = string.Format("update T_36_022_{0} set c019 = c019+1 WHERE c001 = '{1}'",
                                rentToken, editPaper.LongQuestionNum);
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
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn OptChangePaperQuestions(SessionInfo session, List<string> listParams)
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
                string strPaperQuestionParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CPaperQuestionParam>>(strPaperQuestionParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var listeEditPapers = optReturn.Data as List<CPaperQuestionParam>;
                if (listeEditPapers == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listeEditPapers is null");
                    return optReturn;
                }

                string rentToken = session.RentInfo.Token;
                string strSql;
                int iCount = 0;
                long longPaperNum = 0;
                foreach (var editPaper in listeEditPapers)
                {
                    iCount++;
                    longPaperNum = editPaper.LongPaperNum;
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("UPDATE T_36_024_{0} SET c019={1} WHERE c001='{2}' AND c002='{3}'",
                                rentToken, editPaper.IntScore, editPaper.LongPaperNum, editPaper.LongQuestionNum);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            break;
                        case 3:
                            strSql = string.Format("UPDATE T_36_024_{0} SET c019={1} WHERE c001='{2}' AND c002='{3}'",
                                rentToken, editPaper.IntScore, editPaper.LongPaperNum, editPaper.LongQuestionNum);
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
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn OptQueryQuestions(SessionInfo session, List<string> listParams)
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
                string strQuestionParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CQuestionsParam>(strQuestionParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var questionParam = optReturn.Data as CQuestionsParam;
                if (questionParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM T_36_022_{0} WHERE C001='{1}'", session.RentInfo.Token, questionParam.LongNum);
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
                    CQuestionsParam param = new CQuestionsParam();
                    param.LongNum = Convert.ToInt64(dr["C001"]);
                    param.LongCategoryNum = Convert.ToInt64(dr["C002"]);
                    param.StrCategoryName = dr["C003"].ToString();
                    param.IntType = Convert.ToInt32(dr["C004"]);
                    param.StrQuestionsContect = dr["C005"].ToString();
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
                    param.IntUseNumber = Convert.ToInt32(dr["C019"]);
                    param.StrAccessoryType = dr["C020"].ToString();
                    param.StrAccessoryName = dr["C021"].ToString();
                    param.StrAccessoryPath = dr["C022"].ToString();
                    param.LongFounderId = Convert.ToInt64(dr["C023"]);
                    param.StrFounderName = dr["C024"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C025"]).ToString("yyyy/MM/dd HH:mm:ss");
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

        private OperationReturn OptGetPaperQuestion(SessionInfo session, List<string> listParams)
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

        private OperationReturn OptGetQuestions(SessionInfo session, List<string> listParams)
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
                    CQuestionsParam param = new CQuestionsParam();
                    param.LongNum = Convert.ToInt64(dr["C001"]);
                    param.LongCategoryNum = Convert.ToInt64(dr["C002"]);
                    param.StrCategoryName = dr["C003"].ToString();
                    param.IntType = Convert.ToInt32(dr["C004"]);
                    param.StrQuestionsContect = dr["C005"].ToString();
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
                    param.IntUseNumber = Convert.ToInt32(dr["C019"]);
                    param.StrAccessoryType = dr["C020"].ToString();
                    param.StrAccessoryName = dr["C021"].ToString();
                    param.StrAccessoryPath = dr["C022"].ToString();
                    param.LongFounderId = Convert.ToInt64(dr["C023"]);
                    param.StrFounderName = dr["C024"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C025"]).ToString("yyyy/MM/dd HH:mm:ss");
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

        private OperationReturn OptPaperQuestionsExist(SessionInfo session, List<string> listParams)
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
                string strEditPaper = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CPaperQuestionParam>>(strEditPaper);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<CPaperQuestionParam> listEditPapers = optReturn.Data as List<CPaperQuestionParam>;
                if (listEditPapers == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }

                List<string> listReturn = new List<string>();
                foreach (var editPaper in listEditPapers)
                {
                    DataSet objDataSet;
                    string strSql = string.Format("SELECT * FROM T_36_024_{0} WHERE C001='{1}' and C002='{2}'", session.RentInfo.Token, editPaper.LongPaperNum, editPaper.LongQuestionNum);
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
                    if (objDataSet.Tables[0].Rows.Count > 0)
                    {
                        optReturn = XMLHelper.SeriallizeObject(editPaper.LongQuestionNum);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        listReturn.Add(optReturn.Data.ToString());
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

        private OperationReturn OptSetQuestionNum(SessionInfo session, List<string> listParams)
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
                var param = optReturn.Data as CPaperParam;
                if (param == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Paper Param is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("UPDATE T_36_023_{0} set c007='{1}' , c018='{2}' where c001='{3}'",
                            rentToken, param.IntQuestionNum, param.IntIntegrity,param.LongNum);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("UPDATE T_36_023_{0} set c007='{1}' , c018='{2}' where c001='{3}'",
                            rentToken, param.IntQuestionNum, param.IntIntegrity, param.LongNum);
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

        private OperationReturn OptSearchQuestion(SessionInfo session, List<string> listParams)
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
                    CQuestionsParam param = new CQuestionsParam();
                    param.LongNum = Convert.ToInt64(dr["C001"]);
                    param.LongCategoryNum = Convert.ToInt64(dr["C002"]);
                    param.StrCategoryName = dr["C003"].ToString();
                    param.IntType = Convert.ToInt32(dr["C004"]);
                    param.StrQuestionsContect = dr["C005"].ToString();
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
                    param.IntUseNumber = Convert.ToInt32(dr["C019"]);
                    param.StrAccessoryType = dr["C020"].ToString();
                    param.StrAccessoryName = dr["C021"].ToString();
                    param.StrAccessoryPath = dr["C022"].ToString();
                    param.LongFounderId = Convert.ToInt64(dr["C023"]);
                    param.StrFounderName = dr["C024"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C025"]).ToString("yyyy/MM/dd HH:mm:ss");
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

        private OperationReturn OptSearchPaper(SessionInfo session, List<string> listParams)
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

        private OperationReturn SetTestUserList(SessionInfo session, List<string> listParams)
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

                    case (int)S3602Codes.OptGetQuestionCategory:
                        optReturn = OptGetQuestinCategory(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3602Codes.OptUpdatePaper:
                        optReturn = OptUpdatePaper(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
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
                    case (int)S3602Codes.OptPaperSameName:
                        optReturn = OptPaperSameName(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3602Codes.OptGetUmpsetuppath:
                        optReturn = OptGetUMPSetupPath();
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        break;

                    case (int)S3602Codes.OptGetPapers:
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
                    case (int)S3602Codes.OptAddPaper:
                        optReturn = OptAddPaper(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3602Codes.OptDeletePaper:
                        optReturn = OptDeletePaper(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3602Codes.OptDeletePaperAllQuestions:
                        optReturn = OptDeletePaperAllQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3602Codes.OptDeletePaperQuestions:
                        optReturn = OptDeletePaperQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3602Codes.OptAddPaperQuestions:
                        optReturn = OptAddPaperQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3602Codes.OptChangePaperQuestions:
                        optReturn = OptChangePaperQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3602Codes.OptQueryQuestions:
                        optReturn = OptQueryQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3602Codes.OptGetPaperQuestions:
                        optReturn = OptGetPaperQuestion(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3602Codes.OptGetQuestions:
                        optReturn = OptGetQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3602Codes.OptPaperQuestionsExist:
                        optReturn = OptPaperQuestionsExist(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3602Codes.OptSetQuestionNum:
                        optReturn = OptSetQuestionNum(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3602Codes.OptLoadFile:
                        optReturn = OptGetUMPSetupPath();
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
                    case (int)S3602Codes.OptSearchQuestions:
                        optReturn = OptSearchQuestion(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3602Codes.OptSearchPaper:
                        optReturn = OptSearchPaper(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3602Codes.OptGetCtrolAgent:
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
                    case (int)S3602Codes.OptGetTestUserList:
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
                    case (int)S3602Codes.OptSetTestUserList:
                        optReturn = SetTestUserList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message + optReturn.Data;
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

        public WebReturn UmpUpOperation(UpRequest upRequest)
        {
            var webReturn = new WebReturn();
            SessionInfo session = upRequest.Session;
            webReturn.Session = session;
            try
            {
                var fileStm = new FileStream(upRequest.SvPath, FileMode.OpenOrCreate);
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
    }
}
