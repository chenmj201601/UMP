using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using Common2400;
using VoiceCyber.UMP.Communications;
using System.Data;
using System.Data.Common;
using VoiceCyber.UMP.Encryptions;

namespace Wcf24021
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service24021 : IService24021
    {
        public WebReturn DoOperation(WebRequest webRequest)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
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
                    case (int)S2400RequestCode.GetEncryptionDBCurrentTime:
                        webReturn.ListData.Add(DateTime.Now.ToString());
                        break;
                    case (int)S2400RequestCode.AddEncryptionPolicy:
                        optReturn = AddEncryptionPolicy(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S2400RequestCode.GetAllPolicies:
                        optReturn = GetAllPolicies(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.DataSetData = optReturn.Data as DataSet;
                        break;
                    case (int)S2400RequestCode.GetPolicyByID:
                        optReturn = GetPolicyByID(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S2400RequestCode.ModifyPolicy:
                        optReturn = UpdatePolicy(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S2400RequestCode.EnablePolicy:
                        webRequest.ListData.Add("1");
                        optReturn = EnableDisablePolicy(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S2400RequestCode.DisablePolicy:
                        webRequest.ListData.Add("2");
                        optReturn = EnableDisablePolicy(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S2400RequestCode.GetPolicyBinding:
                        optReturn = GetPolicyBindingByPolicyID(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2400RequestCode.GetPolicyKey:
                        optReturn = GetPolicyKeyByPolicyID(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2400RequestCode.GetPoliciesByVoiceIPSource:
                        optReturn = GetPoliciesByVoiceIPSource(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.DataSetData = optReturn.Data as DataSet;
                        break;
                }
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
            }
            return webReturn;
        }

        /// <summary>
        /// 增加加密策略
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 策略名
        /// lstParams[1] : 策略类型
        /// lstParams[2] : 是否立即开始标识
        /// lstParams[3] : 密钥（策略类型为“用户输入”时使用，周期性更新时 此值为空）
        /// lstParams[4] : 自动生成密钥周期（策略类型为“周期性自动生成”时使用，用户输入时，此值为空）
        /// lstParams[5] : 策略有效期开始时间
        /// lstParams[6] : 策略有效期结束时间
        /// lstParams[7] : 密钥更新周期
        /// lstParams[8] : 密码复杂性校验
        /// lstParams[9] : 必须包含大写字母
        /// lstParams[10] : 包含大写字母个数
        /// lstParams[11] : 必须包含小写字母
        /// lstParams[12 ] : 包含小写字母个数
        /// lstParams[13] : 必须包含数字
        /// lstParams[14] : 包含数字个数。
        /// lstParams[15 ] : 必须包含特殊字符
        /// lstParams[16] : 包含特殊字符个数
        /// lstParams[17] : 在多少个密钥内不能重复。
        /// lstParams[18] : 在多少天密钥内不能重复。
        /// lstParams[19] : 最短长度
        /// lstParams[20] : 最长长度
        /// lstParams[21] : 策略描述
        /// lstParams[22] : 是否重置周期
        /// lstParams[23] : KeyGenServer Host
        /// lstParams[24] : KeyGenServer Port
        /// <returns></returns>
        private OperationReturn AddEncryptionPolicy(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                if (lstParams == null || lstParams.Count < 25)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRentID = session.RentInfo.Token;
                long errNumber = 0;
                string strErrMsg = string.Empty;
                string strTypeUTime = string.Empty;
                string strDurationBegin = string.Empty;
                if (lstParams[2] == "1")
                {
                    if (lstParams[1] == "U")
                    {
                        strTypeUTime = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    strDurationBegin = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    if (lstParams[1] == "U")
                    {
                        strTypeUTime = DateTime.Parse(lstParams[5]).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    strDurationBegin = DateTime.Parse(lstParams[5]).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                }

                string strErrorCode = string.Empty;
                string strErrorMsg = string.Empty;
                string strPolicyID = string.Empty;

                switch (session.DBType)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@AInParam01",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInParam02",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@AInParam03",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam04",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam05",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AInparam06",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@AInparam07",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam08",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AInparam09",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AInparam10",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam11",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam12",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam13",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam14",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam15",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam16",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam17",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam18",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam19",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam20",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam21",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam22",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam23",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam24",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam25",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AInparam26",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AInparam27",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@AOutParam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.NVarchar,4000)
                        };

                        mssqlParameters[0].Value = strRentID;
                        mssqlParameters[1].Value = lstParams[0];
                        mssqlParameters[2].Value = "1";
                        mssqlParameters[3].Value = lstParams[1];
                        mssqlParameters[4].Value = strTypeUTime;
                        mssqlParameters[5].Value = lstParams[3];
                        mssqlParameters[6].Value = lstParams[4];
                        mssqlParameters[7].Value = strDurationBegin;
                        mssqlParameters[8].Value = DateTime.Parse(lstParams[6]).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        mssqlParameters[9].Value = lstParams[7];
                        mssqlParameters[10].Value = lstParams[22];
                        mssqlParameters[11].Value = lstParams[8];
                        mssqlParameters[12].Value = lstParams[9];
                        mssqlParameters[13].Value = lstParams[10];
                        mssqlParameters[14].Value = lstParams[11];
                        mssqlParameters[15].Value = lstParams[12];
                        mssqlParameters[16].Value = lstParams[13];
                        mssqlParameters[17].Value = lstParams[14];
                        mssqlParameters[18].Value = lstParams[15];
                        mssqlParameters[19].Value = lstParams[16];
                        mssqlParameters[20].Value = lstParams[17];
                        mssqlParameters[21].Value = lstParams[18];
                        mssqlParameters[22].Value = lstParams[19];
                        mssqlParameters[23].Value = lstParams[20];
                        mssqlParameters[24].Value = session.UserID.ToString();
                        mssqlParameters[25].Value = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        mssqlParameters[26].Value = lstParams[21];
                        mssqlParameters[27].Value = strPolicyID;
                        mssqlParameters[28].Value = errNumber;
                        mssqlParameters[29].Value = strErrMsg;
                        mssqlParameters[27].Direction = ParameterDirection.Output;
                        mssqlParameters[28].Direction = ParameterDirection.Output;
                        mssqlParameters[29].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_24_001",
                           mssqlParameters);
                        strErrorCode = mssqlParameters[28].Value.ToString();
                        strErrorMsg = mssqlParameters[29].Value.ToString();

                        strPolicyID = mssqlParameters[27].Value.ToString();
                        break;
                    case 3:
                        DbParameter[] oracleParameters =
                        {
                            OracleOperation.GetDbParameter("@AInParam01",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInParam02",OracleDataType.Varchar2,1024),
                            OracleOperation.GetDbParameter("@AInParam03",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam04",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam05",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("@AInparam06",OracleDataType.Varchar2,1024),
                            OracleOperation.GetDbParameter("@AInparam07",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam08",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("@AInparam09",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("@AInparam10",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam11",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam12",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam13",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam14",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam15",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam16",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam17",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam18",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam19",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam20",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam21",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam22",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam23",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam24",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam25",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("@AInparam26",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("@AInparam27",OracleDataType.Varchar2,1024),
                            OracleOperation.GetDbParameter("@AOutParam01",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("@aouterrornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("@aouterrorstring",OracleDataType.Nvarchar2,4000)
                        };

                        oracleParameters[0].Value = strRentID;
                        oracleParameters[1].Value = lstParams[0];
                        oracleParameters[2].Value = string.Empty;
                        oracleParameters[3].Value = lstParams[1];
                        oracleParameters[4].Value = strTypeUTime;
                        oracleParameters[5].Value = lstParams[3];
                        oracleParameters[6].Value = lstParams[4];
                        oracleParameters[7].Value = strDurationBegin;
                        oracleParameters[8].Value = DateTime.Parse(lstParams[6]).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        oracleParameters[9].Value = lstParams[7];
                        oracleParameters[10].Value = lstParams[22];
                        oracleParameters[11].Value = lstParams[8];
                        oracleParameters[12].Value = lstParams[9];
                        oracleParameters[13].Value = lstParams[10];
                        oracleParameters[14].Value = lstParams[11];
                        oracleParameters[15].Value = lstParams[12];
                        oracleParameters[16].Value = lstParams[13];
                        oracleParameters[17].Value = lstParams[14];
                        oracleParameters[18].Value = lstParams[15];
                        oracleParameters[19].Value = lstParams[16];
                        oracleParameters[20].Value = lstParams[17];
                        oracleParameters[21].Value = lstParams[18];
                        oracleParameters[22].Value = lstParams[19];
                        oracleParameters[23].Value = lstParams[20];
                        oracleParameters[24].Value = session.UserID;
                        oracleParameters[25].Value = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        oracleParameters[26].Value = lstParams[21];
                        oracleParameters[27].Value = strPolicyID;
                        oracleParameters[27].Value = errNumber;
                        oracleParameters[28].Value = strErrMsg;
                        oracleParameters[27].Direction = ParameterDirection.Output;
                        oracleParameters[28].Direction = ParameterDirection.Output;
                        oracleParameters[29].Direction = ParameterDirection.Output;

                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_24_001",
                          oracleParameters);

                        strErrorCode = oracleParameters[28].Value.ToString();
                        strErrorMsg = oracleParameters[29].Value.ToString();
                        strPolicyID = oracleParameters[27].Value.ToString();
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                if (strErrorCode != "0")
                {
                    optReturn.Message = "Excute error:" + strErrorCode + " : " + strErrorMsg;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Result = false;
                    return optReturn;
                }
                if (string.IsNullOrEmpty(lstParams[23]) || string.IsNullOrEmpty(lstParams[24]))
                {
                    optReturn.Code = Defines.RET_SUCCESS;
                    optReturn.Result = true;
                    optReturn.Data = strPolicyID;
                    return optReturn;
                }
                //如果是立即开始 就发消息
                if (lstParams[2] == "1")
                {
                    string LocalStrSendMessage = string.Empty;
                    if (lstParams[1] == "U")
                    {
                        LocalStrSendMessage = "invokeid=223344;command=executepolicy;policyid=" + strPolicyID + ";\r\n";
                    }
                    else
                    {
                        LocalStrSendMessage = "invokeid=445566;command=executepolicy;policyid=" + strPolicyID + ";\r\n";
                    }
                    List<string> lstServerInfo = new List<string>();
                    lstServerInfo.Add(lstParams[23]);
                    lstServerInfo.Add(lstParams[24]);
                    string strReturn = string.Empty;
                    KeyGenServerConn.SendMessageToGeneratorServer(lstServerInfo, LocalStrSendMessage, ref strReturn);
                }
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = strPolicyID;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 获得租户下所有的加密策略
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// <returns></returns>
        private OperationReturn GetAllPolicies(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_24_001 WHERE C000 = '{0}'", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_24_001 WHERE C000 = '{0}'", rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        /// <summary>
        /// 根据ID获得策略信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : PolicyID
        /// <returns></returns>
        private OperationReturn GetPolicyByID(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                if (lstParams == null || lstParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_24_001 WHERE C000 = '{0}' and C001 = {1}", rentToken, lstParams[0]);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_24_001 WHERE C000 = '{0}' and C001 = {1}", rentToken, lstParams[0]);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds == null || ds.Tables.Count < 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = 12;
                    optReturn.Message = "The record may have been deleted";
                    return optReturn;
                }
                DataRow row = ds.Tables[0].Rows[0];
                UMPEncryptionPolicy policy = new UMPEncryptionPolicy();
                policy.PolicyRent = row["C000"].ToString();
                policy.PolicyID = row["C001"].ToString();
                policy.PolicyName = row["C002"].ToString();
                policy.PolicyIsEnabled = row["C002"].ToString() == "1";
                policy.PolicyType = row["C004"].ToString();
                if (row["C005"].ToString() != "0" && row["C005"].ToString().Length == 14)
                {
                    policy.TypeUStartTime = CommonFunctions.StringToDateTime(row["C005"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    policy.TypeUStartTime = string.Empty;
                }
                policy.TypeuEncryptKey = row["C006"].ToString();
                policy.PolicyOccursFrequency = row["C007"].ToString();
                policy.DurationBegin = row["C008"].ToString();
                policy.DurationEnd = row["C009"].ToString();
                policy.BeginDayofCycle = row["C010"].ToString();
                policy.Complexityabled = row["C012"].ToString();
                policy.MustContainCapitals = row["C013"].ToString();
                policy.NumbersCapitals = int.Parse(row["C014"].ToString());
                policy.MustContainLower = row["C015"].ToString();
                policy.NumbersLower = int.Parse(row["C016"].ToString());
                policy.MustContainDigital = row["C017"].ToString();
                policy.NumbersDigital = int.Parse(row["C018"].ToString());
                policy.MustContainSpecial = row["C019"].ToString();
                policy.NumbersSpecial = int.Parse(row["C020"].ToString());
                policy.PasswordHistoryinDay = int.Parse(row["C022"].ToString());
                policy.PasswordHistoryInNumber = int.Parse(row["C021"].ToString());
                policy.ThesHortestLength = int.Parse(row["C023"].ToString());
                policy.TheLongestLength = int.Parse(row["C024"].ToString());
                policy.PolicyNotes = row["C035"].ToString();

                //判断是否被绑定了
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_24_002 where C003 = {0}", lstParams[0]);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_24_002 where C003 = {0}", lstParams[0]);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                ds = optReturn.Data as DataSet;
                if (optReturn.Data == null || ds == null || ds.Tables.Count < 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    policy.IsBinded = false;
                }
                else
                {
                    policy.IsBinded = true;
                }

                optReturn = XMLHelper.SeriallizeObject(policy);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        /// <summary>
        /// 更新加密策略
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] :Rent ID           //不用
        /// lstParams[1] : PolicyID
        /// lstParams[2] : PolicyName
        /// lstParams[3] : typeutime   仅用于用户输入--生效于
        /// lstParams[4] : typeuencryptkey   仅用于用户输入
        /// lstParams[5] : durationend 
        /// lstParams[6] : begindayofcycle   仅用于自动生成--自定义天数
        /// lstParams[7] : resetcycle   仅用于自动生成--自定义天数
        /// lstParams[8] : complexityabled 
        /// lstParams[9] : mustcontaincapitals 
        /// lstParams[10] : numberscapitals 
        /// lstParams[11] : mustcontainlower 
        /// lstParams[12] : numberslower 
        /// lstParams[13] : mustcontaindigital 
        /// lstParams[14] : numbersdigital 
        /// lstParams[15] : mustcontainspecial 
        /// lstParams[16] : numbersspecial 
        /// lstParams[17] : theshortestlength 
        /// lstParams[18] : thelongestlength 
        /// lstParams[19] : policynotes 
        /// lstParams[20] : IsImmediately 策略类型为U 时 是否立即更新密钥
        /// lstParams[21] : PolicyType
        /// lstParams[22] : KeyGenserver host
        /// lstParams[23] : KeyGenServer port
        /// <returns></returns>
        private OperationReturn UpdatePolicy(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                if (lstParams == null || lstParams.Count < 24)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strTypeUTime = string.Empty;
                if (lstParams[20] == "1")
                {
                    strTypeUTime = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    if (lstParams[21] == "U")
                    {
                        strTypeUTime = DateTime.Parse(lstParams[3]).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                }

                string strErrorCode = string.Empty;
                string strErrorMsg = string.Empty;
                long errNumber = 0;
                string strErrMsg = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@AInParam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AInParam02",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@AInparam05",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AInparam06",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@AInparam09",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AInparam10",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam11",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam12",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam13",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam14",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam15",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam16",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam17",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam18",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam19",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@AInparam20",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam23",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam24",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@AInparam30",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AInparam31",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AInparam35",MssqlDataType.NVarchar,1024),   
                            MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.NVarchar,4000)
                        };

                        mssqlParameters[0].Value = lstParams[1];
                        mssqlParameters[1].Value = lstParams[2];
                        mssqlParameters[2].Value = strTypeUTime;
                        mssqlParameters[3].Value = lstParams[4];
                        mssqlParameters[4].Value = DateTime.Parse(lstParams[5]).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        mssqlParameters[5].Value = lstParams[6];
                        mssqlParameters[6].Value = lstParams[7];
                        mssqlParameters[7].Value = lstParams[8];
                        mssqlParameters[8].Value = lstParams[9];
                        mssqlParameters[9].Value = lstParams[10];
                        mssqlParameters[10].Value = lstParams[11];
                        mssqlParameters[11].Value = lstParams[12];
                        mssqlParameters[12].Value = lstParams[13];
                        mssqlParameters[13].Value = lstParams[14];
                        mssqlParameters[14].Value = lstParams[15];
                        mssqlParameters[15].Value = lstParams[16];
                        mssqlParameters[16].Value = lstParams[17];
                        mssqlParameters[17].Value = lstParams[18];
                        mssqlParameters[18].Value = session.UserID;
                        mssqlParameters[19].Value = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        mssqlParameters[20].Value = lstParams[19];

                        mssqlParameters[21].Value = errNumber;
                        mssqlParameters[22].Value = strErrMsg;
                        mssqlParameters[21].Direction = ParameterDirection.Output;
                        mssqlParameters[22].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_24_007",
                           mssqlParameters);
                        strErrorCode = mssqlParameters[21].Value.ToString();
                        strErrorMsg = mssqlParameters[22].Value.ToString();

                        break;
                    case 3:
                        DbParameter[] oracleParameters =
                        {
                            OracleOperation.GetDbParameter("@AInParam01",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("@AInParam02",OracleDataType.Varchar2,1024),
                            OracleOperation.GetDbParameter("@AInparam05",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("@AInparam06",OracleDataType.Varchar2,1024),
                            OracleOperation.GetDbParameter("@AInparam09",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("@AInparam10",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam11",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam12",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam13",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam14",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam15",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam16",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam17",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam18",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam19",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("@AInparam20",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam23",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam24",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("@AInparam30",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("@AInparam31",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("@AInparam35",OracleDataType.Nvarchar2,1024),                            
                            OracleOperation.GetDbParameter("@aouterrornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("@aouterrorstring",OracleDataType.Nvarchar2,4000)
                        };

                        oracleParameters[0].Value = lstParams[1];
                        oracleParameters[1].Value = lstParams[2];
                        oracleParameters[2].Value = strTypeUTime;
                        oracleParameters[3].Value = lstParams[4];
                        oracleParameters[4].Value = DateTime.Parse(lstParams[5]).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        oracleParameters[5].Value = lstParams[6];
                        oracleParameters[6].Value = lstParams[7];
                        oracleParameters[7].Value = lstParams[8];
                        oracleParameters[8].Value = lstParams[9];
                        oracleParameters[9].Value = lstParams[10];
                        oracleParameters[10].Value = lstParams[11];
                        oracleParameters[11].Value = lstParams[12];
                        oracleParameters[12].Value = lstParams[13];
                        oracleParameters[13].Value = lstParams[14];
                        oracleParameters[14].Value = lstParams[15];
                        oracleParameters[15].Value = lstParams[16];
                        oracleParameters[16].Value = lstParams[17];
                        oracleParameters[17].Value = lstParams[18];
                        oracleParameters[18].Value = session.UserID;
                        oracleParameters[19].Value = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        oracleParameters[20].Value = lstParams[19];
                        oracleParameters[21].Value = errNumber;
                        oracleParameters[22].Value = strErrMsg;
                        oracleParameters[21].Direction = ParameterDirection.Output;
                        oracleParameters[22].Direction = ParameterDirection.Output;

                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_24_007",
                          oracleParameters);

                        strErrorCode = oracleParameters[21].Value.ToString();
                        strErrorMsg = oracleParameters[22].Value.ToString();
                        break;
                }

                if (!optReturn.Result)
                {
                    return optReturn;
                }
                if (strErrorCode != "0")
                {
                    optReturn.Message = "Excute error:" + strErrorCode + " : " + strErrorMsg;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Result = false;
                    return optReturn;
                }

                if (string.IsNullOrEmpty(lstParams[22]) || string.IsNullOrEmpty(lstParams[23]))
                {
                    optReturn.Code = Defines.RET_SUCCESS;
                    optReturn.Result = true;
                    return optReturn;
                }
                //如果类型为C 且是自定义天数  且选择了立即生效 则根据是否充值周期发消息
                string LocalStrSendMessage = string.Empty;
                if (lstParams[21] == "C" && lstParams[20] == "1")
                {
                    LocalStrSendMessage = "invokeid=556677;command=updatekey;policyid=" + lstParams[1] + ";updatecycle=" + lstParams[7] + "\r\n";
                }
                else if (lstParams[21] == "U" && lstParams[20] == "1")
                {
                    LocalStrSendMessage = "invokeid=667788;command=executepolicy;policyid=" + lstParams[1] + ";" + "\r\n";
                }
                List<string> lstServerInfo = new List<string>();
                lstServerInfo.Add(lstParams[22]);
                lstServerInfo.Add(lstParams[23]);
                string strReturn = string.Empty;
                KeyGenServerConn.SendMessageToGeneratorServer(lstServerInfo, LocalStrSendMessage, ref strReturn);

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : PolicyID
        /// lstParams[1] : 启用或禁用 1:启用 2:禁用
        /// <returns></returns>
        private OperationReturn EnableDisablePolicy(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strPolicyID = lstParams[0];
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("UPDATE T_24_001 SET C003 = '{0}' where C001={1}", lstParams[1], strPolicyID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("update T_24_001 set C003 = '{0}' where C001={1}", lstParams[1], strPolicyID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        /// <summary>
        /// 根据策略ID获得策略绑定关系
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : PolicyID
        /// <returns></returns>
        private OperationReturn GetPolicyBindingByPolicyID(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT T_24_002.C002,T_24_002.C003,T_24_002.C004,T_24_002.C005,T_24_001.C002  as C0021 " +
                                                        "FROM T_24_002  INNER JOIN T_24_001 ON T_24_002.C003 = T_24_001.C001 " +
                                                        "WHERE T_24_002.C001 = 1 and T_24_002.C003 = {0}", lstParams[0]);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("SELECT T_24_002.C002,T_24_002.C003,T_24_002.C004,T_24_002.C005,T_24_001.C002  as C0021 " +
                                                        "FROM T_24_002  INNER JOIN T_24_001 ON T_24_002.C003 = T_24_001.C001 " +
                                                        "WHERE T_24_002.C001 = 1 and T_24_002.C003 = {0}", lstParams[0]);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    return optReturn;
                }
                List<string> lst = new List<string>();
                PolicyBindingInfo binding = null;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    binding = new PolicyBindingInfo();
                    binding.EndTime = (CommonFunctions.StringToDateTime(row["C005"].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
                    binding.PolicyID = row["C003"].ToString();
                    binding.PolicyName = row["C0021"].ToString();
                    binding.StartTime = (CommonFunctions.StringToDateTime(row["C004"].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
                    binding.EncryptionObject = S2400EncryptOperation.DecryptFromDB(row["C002"].ToString());
                    binding.DurationTime = binding.StartTime + " -- " + binding.EndTime;
                    optReturn = XMLHelper.SeriallizeObject(binding);
                    if (!optReturn.Result)
                    {
                        continue;
                    }
                    lst.Add(optReturn.Data.ToString());
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = lst;
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

        /// <summary>
        /// 获得密钥策略使用的密钥
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : PolicyID
        /// <returns></returns>
        private OperationReturn GetPolicyKeyByPolicyID(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("select C002,C004,C007,C008 from t_24_005 where C002 = {0}", lstParams[0]);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("select C002,C004,C007,C008 from t_24_005 where C002 = {0}", lstParams[0]);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    return optReturn;
                }
                List<string> lst = new List<string>();
                PolicyKeyInfo policyKey = null;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    policyKey = new PolicyKeyInfo();
                    policyKey.DurationTime = (CommonFunctions.StringToDateTime(row["C007"].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
                    policyKey.DurationTime += " -- ";
                    policyKey.DurationTime += (CommonFunctions.StringToDateTime(row["C008"].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
                    policyKey.PolicyID = row["C002"].ToString();
                    policyKey.Key = S2400EncryptOperation.DecryptWithM025(row["C004"].ToString());
                    optReturn = XMLHelper.SeriallizeObject(policyKey);
                    if (!optReturn.Result)
                    {
                        continue;
                    }
                    lst.Add(optReturn.Data.ToString());
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = lst;
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

        /// <summary>
        /// 获得租户下所有的加密策略
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// <returns></returns>
        private OperationReturn GetPoliciesByVoiceIPSource(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                if (lstParams == null || lstParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strIPSourceID = lstParams[0];//2210000000000000002服务器资源ID
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    //SELECT T2.C003 STID,T1.* FROM T_24_002 T2 LEFT JOIN T_24_001 T1 ON T1.C001=T2.C003 AND T1.C000=00000 WHERE T2.C009='2210000000000000002'
                    case 2:
                        strSql = string.Format("SELECT T2.*,T1.C002 PONAME FROM T_24_002 T2 LEFT JOIN T_24_001 T1 ON T1.C001=T2.C003 AND T1.C000='{0}' WHERE T2.C009='{1}'", rentToken, strIPSourceID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("SELECT T2.*,T1.C002 PONAME FROM T_24_002 T2 LEFT JOIN T_24_001 T1 ON T1.C001=T2.C003 AND T1.C000='{0}' WHERE T2.C009='{1}'", rentToken, strIPSourceID);
                        //strSql = string.Format("SELECT * FROM T_24_002 WHERE C009='{0}'", strIPSourceID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        private static string DecryptFromClient(string strSource)
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
    }

}
