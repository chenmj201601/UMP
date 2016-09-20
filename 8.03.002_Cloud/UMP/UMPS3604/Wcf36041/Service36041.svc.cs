using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Security.Cryptography;
using System.ServiceModel.Activation;
using System.Text;
using Common3604;
using PFShareClassesS;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace Wcf36041
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service36041" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service36041.svc or Service36041.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class Service36041 : IService36041
    {
        #region Encryption and Decryption
        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType keyIvid)
        {
            string strReturn;
            Random random = new Random();
            string strTemp;

            try
            {
                strReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                var lIntRand = random.Next(0, 14);
                strTemp = lIntRand.ToString("00");
                strReturn = strReturn.Insert(lIntRand, "VCT");
                lIntRand = random.Next(0, 17);
                strTemp += lIntRand.ToString("00");
                strReturn = strReturn.Insert(lIntRand, "UMP");
                lIntRand = random.Next(0, 20);
                strTemp += lIntRand.ToString("00");
                strReturn = strReturn.Insert(lIntRand, ((int)keyIvid).ToString("000"));

                strReturn = EncryptionAndDecryption.EncryptStringY(strTemp + strReturn);
            }
            catch { strReturn = string.Empty; }

            return strReturn;
        }

        private string DecryptNamesFromDb(string strSource)
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

        private const string PathDoggleConfig = "GlobalSettings\\UMP.Young.01";

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
                path = Path.Combine(path, PathDoggleConfig);
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
                    optReturn.Message = "DoggleConfig not exist.";
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
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string roleId = session.RoleInfo.ID.ToString();
                string moduleId = listParams[1];
                string parentId = listParams[2];
                string rentToken = session.RentInfo.Token;
                string strLog = string.Empty;


                #region 获取当前的狗号

                optReturn = GetDoggleNumber();
                if (!optReturn.Result)
                {
                    strLog += "Get doggle number fail.";
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
                                rentToken, moduleId, roleId, parentId);
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
                                 rentToken, moduleId, roleId, parentId);
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
                    optReturn.Message = "DataSet is null";
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
                        strLog += "C006 is empty;";
                        continue;
                    }
                    strC006 = DecryptNamesFromDb(strC006);
                    string[] listC006 = strC006.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                    if (listC006.Length < 3)
                    {
                        //C006无效
                        strLog += "C006 is invalid;";
                        continue;
                    }
                    long optId = Convert.ToInt64(dr["C002"]);
                    long licId = optId + 1000000000;
                    if (licId.ToString() != listC006[0])
                    {
                        //LicenseID与操作ID不对应
                        strLog += "LicID not equal;";
                        continue;
                    }
                    if (listC006[2] == "Y")
                    {
                        string strC008Hash = GetMD5HasString(strC008);
                        string strC008Hash8 = strC008Hash.Substring(0, 8);
                        string strLicDoggle = string.Format("{0}{1}", licId, doggleNumber);
                        strC008 = DecryptNamesFromDb(strC008);
                        if (strLicDoggle != strC008)
                        {
                            //与C008不匹配
                            strLog += "C008 not equal;";
                            continue;
                        }
                        string strDecryptC007 = EncryptionAndDecryption.DecryptStringYKeyIV(strC007, strC008Hash8,
                            strC008Hash8);
                        string[] listC007 = strDecryptC007.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.None);
                        if (listC007.Length < 2)
                        {
                            //C007无效
                            strLog += "C007 is invalid;";
                            continue;
                        }
                        if (listC007[1] != "Y")
                        {
                            //没有许可
                            strLog += "No license;";
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
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string strUserId = listParams[0];
                string strParentId = listParams[1];
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
                                , strParentId
                                , strUserId);
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
                                , strParentId
                                , strUserId);
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
                    optReturn.Message = "DataSet is null";
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strId = dr["C001"].ToString();
                    string strName = dr["C017"].ToString();
                    string strFullName = dr["C018"].ToString();
                    strName = DecryptNamesFromDb(strName);
                    strFullName = DecryptNamesFromDb(strFullName);
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strId, ConstValue.SPLITER_CHAR, strName, strFullName);
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
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string strUserId = listParams[0];
                string strParentId = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strParentId == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1} AND C007<>'H' )"
                           , rentToken
                           , strUserId);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2}"
                           , rentToken
                           , strUserId
                           , strParentId);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strParentId == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1} AND C007<>'H')"
                           , rentToken
                           , strUserId);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2}"
                           , rentToken
                           , strUserId
                           , strParentId);
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
                    optReturn.Message = "DataSet is null";
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strId = dr["C001"].ToString();
                    string strName = dr["C002"].ToString();
                    strName = DecryptNamesFromDb(strName);
                    string parentId = dr["C004"].ToString();
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strId, ConstValue.SPLITER_CHAR, strName, parentId);
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

        private OperationReturn OptGetContents(SessionInfo session, List<string> listParams)
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
                    optReturn.Message = "Contents Param is null";
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
                    optReturn.Message = "DataSet is null";
                    return optReturn;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ContentsParam param = new ContentsParam();
                    param.LongNodeId = Convert.ToInt64(dr["C001"]);
                    param.StrNodeName = dr["C002"].ToString();
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

        private OperationReturn OptCreateContents(SessionInfo session, List<string> listParams)
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
                string strContentsParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<ContentsParam>(strContentsParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                ContentsParam papersContentsParam = optReturn.Data as ContentsParam;
                if (papersContentsParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "PapersContentsParam is null";
                    return optReturn;
                }

                string rentToken = session.RentInfo.Token;

                string strSql = string.Format("SELECT * FROM T_36_001_{0}", rentToken);
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
                    optReturn.Message = "Db object is null";
                    return optReturn;
                }

                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                DataRow dr = objDataSet.Tables[0].NewRow();
                dr["C001"] = papersContentsParam.LongNodeId;
                dr["C002"] = papersContentsParam.StrNodeName;
                dr["C003"] = papersContentsParam.LongParentNodeId;
                dr["C004"] = papersContentsParam.StrParentNodeName;
                dr["C005"] = session.UserInfo.UserID;
                dr["C006"] = session.UserInfo.UserName;
                dr["C007"] = papersContentsParam.StrDateTime;
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

        private OperationReturn OptChangeContents(SessionInfo session, List<string> listParams)
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
                string strContentsParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<ContentsParam>(strContentsParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                ContentsParam papersContentsParam = optReturn.Data as ContentsParam;
                if (papersContentsParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "PapersContentsParam is null";
                    return optReturn;
                }

                string rentToken = session.RentInfo.Token;

                string strSql = string.Format("SELECT * FROM T_36_001_{0} WHERE C001 = '{1}'", rentToken, papersContentsParam.LongNodeId);
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
                    optReturn.Message = "Db object is null";
                    return optReturn;
                }

                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                if (objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = "Select is Null!";
                    return optReturn;
                }
                DataRow dr = objDataSet.Tables[0].Rows[0];
                dr["C002"] = papersContentsParam.StrNodeName;
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();

                strSql = string.Format("SELECT * FROM T_36_001_{0} WHERE C003 = '{1}'", rentToken, papersContentsParam.LongNodeId);
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
                    optReturn.Message = "Db object is null";
                    return optReturn;
                }

                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                if (objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = "Select is Null!";
                    return optReturn;
                }
                for (int i = 0; i <= objDataSet.Tables.Count; i ++ )
                {
                    dr = objDataSet.Tables[0].Rows[i];
                    dr["C004"] = papersContentsParam.StrNodeName;
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

        public WebReturn UmpTaskOperation(WebRequest webRequest)
        {
            WebReturn webReturn = new WebReturn();
            if (webRequest == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = "WebRequest is null";
                return webReturn;
            }
            SessionInfo session = webRequest.Session;
            if (session == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = "SessionInfo is null";
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
                    case (int)S3604Codes.GetUserOperationList:
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
                    case (int)S3604Codes.GetControlAgentInfoList:
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
                    case (int)S3604Codes.GetControlOrgInfoList:
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
                    case (int)S3604Codes.OptGetContents:
                        optReturn = OptGetContents(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3604Codes.OptCreateContents:
                        optReturn = OptCreateContents(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3604Codes.OptChangeContents:
                        optReturn = OptChangeContents(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
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
            WebReturn webReturn = new WebReturn();
            SessionInfo session = upRequest.Session;
            webReturn.Session = session;
            try
            {
                FileStream fileStm = new FileStream(upRequest.SvPath, FileMode.OpenOrCreate);
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
