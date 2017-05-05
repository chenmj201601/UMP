using Common11031;
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
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;
using Wcf11031.Wcf11012;

namespace Wcf11031
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service11031”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service11031.svc 或 Service11031.svc.cs，然后开始调试。
     [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service11031 : IService11031
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
                    dbInfo.RealPassword = DecryptString04(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }
                switch (webRequest.Code)
                {
                    case (int)S1103Codes.ModifyAgent:
                        optReturn = ModifyAgent(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1103Codes.GetAgentCtledUserIDs:
                        optReturn = GetAgentCtledUserIDs(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1103Codes.SaveAgentMMT:
                        optReturn = SaveAgentMMT(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1103Codes.UPAgentPwd:
                        optReturn = UPAgentPwd(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1103Codes.ModifyAgentORGC:
                        optReturn = ModifyAgentORGC(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1103Codes.GetAllAgent:
                        optReturn = GetAllAgent(session);
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
                        webReturn.Message = string.Format("Request code invalid.");
                        return webReturn;
                }
                webReturn.Message = optReturn.Message;
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

        private OperationReturn GetAllAgent(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Code = 0;
            optReturn.Result = true;
            try
            {
                int dbType = session.DBType;
                string strRentInfo = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                string strSql;
                DataSet ds = new DataSet();
                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("Select * From T_11_101_{0} where (C001>1031401010000000000 and C001<1040001010000000000) and C002=1 ", strRentInfo);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                         if (!optReturn.Result) 
                        {
                            return optReturn;
                        }
                       
                        ds = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("Select * From T_11_101_{0} where (C001>1031401010000000000 and C001<1040001010000000000) and C002=1 ", strRentInfo);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                         if (!optReturn.Result) 
                        {
                            return optReturn;
                        }
                       
                        ds = optReturn.Data as DataSet;
                        break;                
                }
              
                if (ds == null) 
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null.");
                    return optReturn;
                }
                List<string> strList = new List<string>(); 
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    string strAgentID = DecryptString02(dr["C017"].ToString());
                    strList.Add(strAgentID);
                }

                optReturn.Data = strList;
            }
            catch (Exception ex) 
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn ModifyAgentORGC(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Code = 0;
            optReturn.Result = true;
            try
            {
                int dbType = session.DBType;
                string strAgentID = listParams[0];
                string[] arrInfo = listParams[1].Split(new[] { ConstValue.SPLITER_CHAR },
                                        StringSplitOptions.RemoveEmptyEntries);
                string strORGID = arrInfo[0];

                string strRentInfo = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "UPDATE T_11_101_{0} SET C011='{1}' ,C013=0 WHERE C001={2} and C002=1",
                                strRentInfo,
                                strORGID,
                                strAgentID);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                 "UPDATE T_11_101_{0} SET C011='{1}' ,C013=0 WHERE C001={2} and C002=1",
                                strRentInfo,
                                strORGID,
                                strAgentID);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
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

        private OperationReturn ModifyAgent(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                #region 验证参数

                //ListData
                //0     UserID
                //1     Method ( 1:AddAgent;2:DeleteAgent;3:ModifyAgent)
                //2     AgentInfo
                if (listParams == null
                    || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string strAgentInfo = listParams[2];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid.");
                    return optReturn;
                }
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid.");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<AgentInfo>(strAgentInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                AgentInfo agentInfo = optReturn.Data as AgentInfo;
                if (agentInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AgentInfo is null.");
                    return optReturn;
                }

                #endregion


                List<string> listReturn = new List<string>();
                if (intMethod == 1)
                {

                    #region 添加坐席

                    #region 获取TenantOrg的Token

                    long orgID = agentInfo.OrgID;
                    int dbType = session.DBType;
                    string rentToken = session.RentInfo.Token;
                    string strConn = session.DBConnectionString;
                    string strTenantOrgToken = string.Empty;
                    int tenantOrgToken;
                    int errNum = 0;
                    string errMsg = string.Empty;
                    switch (dbType)
                    {
                        case 2:
                            DbParameter[] mssqlParameters =
                            {
                                MssqlOperation.GetDbParameter("@AInParam01", MssqlDataType.Varchar, 5),
                                MssqlOperation.GetDbParameter("@AInParam02", MssqlDataType.Varchar, 20),
                                MssqlOperation.GetDbParameter("@AOutParam01", MssqlDataType.Varchar, 5),
                                MssqlOperation.GetDbParameter("@AOutErrorNumber", MssqlDataType.Int, 11),
                                MssqlOperation.GetDbParameter("@AOutErrorString", MssqlDataType.NVarchar, 1024)
                            };
                            mssqlParameters[0].Value = rentToken;
                            mssqlParameters[1].Value = orgID.ToString();
                            mssqlParameters[2].Value = strTenantOrgToken;
                            mssqlParameters[3].Value = errNum;
                            mssqlParameters[4].Value = errMsg;
                            mssqlParameters[2].Direction = ParameterDirection.Output;
                            mssqlParameters[3].Direction = ParameterDirection.Output;
                            mssqlParameters[4].Direction = ParameterDirection.Output;
                            optReturn = MssqlOperation.ExecuteStoredProcedure(strConn, "P_11_002", mssqlParameters);
                            if (!optReturn.Result)
                            {
                                optReturn.Message += string.Format("Step1");
                                return optReturn;
                            }
                            if (mssqlParameters[3].Value.ToString() != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[3].Value,
                                    mssqlParameters[4].Value);
                                return optReturn;
                            }
                            if (!int.TryParse(mssqlParameters[2].Value.ToString(), out tenantOrgToken))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("TennantOrgToken invalid.");
                                return optReturn;
                            }
                            break;
                        case 3:
                            DbParameter[] orclParameters =
                            {
                                OracleOperation.GetDbParameter("AInParam01", OracleDataType.Varchar2, 5),
                                OracleOperation.GetDbParameter("AInParam02", OracleDataType.Varchar2, 20),
                                OracleOperation.GetDbParameter("AOutParam01", OracleDataType.Varchar2, 5),
                                OracleOperation.GetDbParameter("AOutErrorNumber", OracleDataType.Int32, 11),
                                OracleOperation.GetDbParameter("AOutErrorString", OracleDataType.Varchar2, 1024)
                            };
                            orclParameters[0].Value = rentToken;
                            orclParameters[1].Value = orgID.ToString();
                            orclParameters[2].Value = strTenantOrgToken;
                            orclParameters[3].Value = errNum;
                            orclParameters[4].Value = errMsg;
                            orclParameters[2].Direction = ParameterDirection.Output;
                            orclParameters[3].Direction = ParameterDirection.Output;
                            orclParameters[4].Direction = ParameterDirection.Output;
                            optReturn = OracleOperation.ExecuteStoredProcedure(strConn, "P_11_002", orclParameters);
                            if (!optReturn.Result)
                            {
                                optReturn.Message += string.Format("Step1");
                                return optReturn;
                            }
                            if (orclParameters[3].Value.ToString() != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("{0}\t{1}", orclParameters[3].Value,
                                    orclParameters[4].Value);
                                return optReturn;
                            }
                            if (!int.TryParse(orclParameters[2].Value.ToString(), out tenantOrgToken))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("TennantOrgToken invalid.");
                                return optReturn;
                            }
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid.");
                            return optReturn;
                    }

                    #endregion


                    #region 判断坐席号是否存在，同租户机构下不允许重复的坐席

                    string strAgentID = agentInfo.AgentID;
                    string strEncryptAgentID = EncryptString02(strAgentID);
                    string strSql;
                    switch (dbType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT COUNT(*) FROM T_11_101_{0} WHERE C001 IN (SELECT C001 FROM T_11_101_{0} WHERE C001 > 1030000000000000000 AND C001 < 1040000000000000000 AND C002 = 3 AND C011 = '{1}') AND C002 = 1 AND C017 = '{2}'",
                                    rentToken,
                                    tenantOrgToken,
                                    strEncryptAgentID);
                            optReturn = MssqlOperation.GetRecordCount(strConn, strSql);
                            break;
                        case 3:
                            strSql =
                               string.Format(
                                   "SELECT COUNT(*) FROM T_11_101_{0} WHERE C001 IN (SELECT C001 FROM T_11_101_{0} WHERE C001 > 1030000000000000000 AND C001 < 1040000000000000000 AND C002 = 3 AND C011 = '{1}') AND C002 = 1 AND C017 = '{2}'",
                                   rentToken,
                                   tenantOrgToken,
                                   strEncryptAgentID);
                            optReturn = OracleOperation.GetRecordCount(strConn, strSql);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid.");
                            return optReturn;
                    }
                    if (!optReturn.Result)
                    {
                        optReturn.Message += string.Format("Step2");
                        return optReturn;
                    }
                    if (optReturn.IntValue > 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_ALREADY_EXIST;
                        optReturn.Message = string.Format("Agent already exist.");
                        return optReturn;
                    }

                    #endregion


                    #region 获取新坐席流水号

                    //获取流水号
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("11");
                    webRequest.ListData.Add(ConstValue.RESOURCE_AGENT.ToString());
                    webRequest.ListData.Add(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                    Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(session),
                        WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service11012"));
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        optReturn.Result = false;
                        optReturn.Code = webReturn.Code;
                        optReturn.Message = webReturn.Message;
                        return optReturn;
                    }
                    long serialID = Convert.ToInt64(webReturn.Data);
                    agentInfo.SerialID = serialID;

                    #endregion


                    #region 获取默认密码

                    string strPassword = string.Empty;
                    switch (dbType)
                    {
                        case 2:
                            strSql = string.Format("SELECT C006 FROM T_11_001_{0} WHERE C003 = 11010501", rentToken);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            break;
                        case 3:
                            strSql = string.Format("SELECT C006 FROM T_11_001_{0} WHERE C003 = 11010501", rentToken);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid.");
                            return optReturn;
                    }
                    if (optReturn.Result)
                    {
                        DataSet objDataSet = optReturn.Data as DataSet;
                        if (objDataSet == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("DataSet is null.");
                            return optReturn;
                        }
                        if (objDataSet.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[0];
                            string str = dr["C006"].ToString();
                            str = DecryptString02(str);
                            if (str.Length >= 8)
                            {
                                str = str.Substring(8);
                                str = string.Format("{0}{1}", serialID, str);
                                try
                                {
                                    byte[] temp = ServerHashEncryption.EncryptBytes(Encoding.Unicode.GetBytes(str),
                                        EncryptionMode.SHA512V00Hex);
                                    byte[] aes = ServerAESEncryption.EncryptBytes(temp, EncryptionMode.AES256V02Hex);
                                    strPassword = ServerEncryptionUtils.Byte2Hex(aes);
                                }
                                catch
                                {
                                    strPassword = str;
                                }
                            }
                        }
                    }

                    #endregion


                    #region 添加坐席信息

                    IDbConnection objConn;
                    IDbDataAdapter objAdapter;
                    DbCommandBuilder objCmdBuilder;
                    switch (dbType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE 1 = 2", rentToken);
                            objConn = MssqlOperation.GetConnection(strConn);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE 1 = 2", rentToken);
                            objConn = OracleOperation.GetConnection(strConn);
                            objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid.");
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


                        #region 坐席信息

                        DataRow dr1 = objDataSet.Tables[0].NewRow();
                        dr1["C001"] = serialID;
                        dr1["C002"] = 1;
                        dr1["C011"] = orgID;
                        dr1["C012"] = (agentInfo.State & 2) > 0 ? "2" : "1";
                        dr1["C013"] = "1";
                        dr1["C014"] = "N";
                        dr1["C015"] = "";
                        dr1["C016"] = "00";
                        dr1["C017"] = strEncryptAgentID;
                        string strEncryptAgentName = EncryptString02(agentInfo.AgentName);
                        dr1["C018"] = strEncryptAgentName;
                        dr1["C019"] = "";
                        dr1["C020"] = strPassword;
                        objDataSet.Tables[0].Rows.Add(dr1);

                        DataRow dr2 = objDataSet.Tables[0].NewRow();
                        dr2["C001"] = serialID;
                        dr2["C002"] = 2;
                        dr2["C011"] = "2014/01/01 00:00:00";
                        dr2["C012"] = "0";
                        dr2["C013"] = "0";
                        objDataSet.Tables[0].Rows.Add(dr2);

                        DataRow dr3 = objDataSet.Tables[0].NewRow();
                        dr3["C001"] = serialID;
                        dr3["C002"] = 3;
                       // dr3["C011"] = tenantOrgToken.ToString();
                        dr3["C011"] = agentInfo.Tenure;
                        objDataSet.Tables[0].Rows.Add(dr3);

                        #endregion


                        objAdapter.Update(objDataSet);
                        objDataSet.AcceptChanges();

                        listReturn.Add(serialID.ToString());

                        #region 添加默认管理权限

                        #region 1、所有管理创建人的人能管理添加的坐席

                        OperationReturn mmtReturn = null;
                        switch (dbType)
                        {
                            case 2:
                                strSql =
                                    string.Format(
                                        "INSERT INTO T_11_201_{0} SELECT 0, 0, C003, {1}, '2014/1/1', '2199/12/31' FROM T_11_201_{0} WHERE C004 = {2} AND C003 > 1020000000000000000 AND C003 < 1030000000000000000",
                                        rentToken, serialID, userID);
                                mmtReturn = MssqlOperation.ExecuteSql(strConn, strSql);
                                break;
                            case 3:
                                strSql =
                                  string.Format(
                                      "INSERT INTO T_11_201_{0} SELECT 0, 0, C003, {1}, TO_DATE('2014/1/1','YYYY-MM-DD HH:MI:SS'), TO_DATE('2199/12/31','YYYY-MM-DD HH:MI:SS') FROM T_11_201_{0} WHERE C004 = {2} AND C003 > 1020000000000000000 AND C003 < 1030000000000000000",
                                      rentToken, serialID, userID);
                                mmtReturn = OracleOperation.ExecuteSql(strConn, strSql);
                                break;
                        }
                        if (mmtReturn != null)
                        {
                            if (!mmtReturn.Result)
                            {
                                optReturn.Message += string.Format("MMT1 fail.\t{0}\t{1}", mmtReturn.Code,
                                    mmtReturn.Message);
                            }
                        }

                        #endregion


                        #region 2、租户机构的管理员能管理添加的坐席

                        //先获取租户机构的管理源的帐号
                        mmtReturn = null;
                        long tenantOrgID = long.Parse(string.Format("10114010100000{0}", tenantOrgToken.ToString("00000")));
                        switch (dbType)
                        {
                            case 2:
                                strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006 = {1} AND C027 = '1'",
                                    rentToken, tenantOrgID);
                                mmtReturn = MssqlOperation.GetDataSet(strConn, strSql);
                                break;
                            case 3:
                                strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006 = {1} AND C027 = '1'",
                                   rentToken, tenantOrgID);
                                mmtReturn = OracleOperation.GetDataSet(strConn, strSql);
                                break;
                        }
                        if (mmtReturn != null)
                        {
                            if (mmtReturn.Result)
                            {
                                objDataSet = mmtReturn.Data as DataSet;
                                if (objDataSet != null
                                    && objDataSet.Tables.Count > 0
                                    && objDataSet.Tables[0].Rows.Count > 0)
                                {
                                    DataRow dr = objDataSet.Tables[0].Rows[0];
                                    long tenantOrgAdminID = Convert.ToInt64(dr["C001"]);

                                    //添加管理权限
                                    mmtReturn = null;
                                    switch (dbType)
                                    {
                                        case 2:
                                            strSql =
                                                string.Format(
                                                    "INSERT INTO T_11_201_{0} VALUES (0, 0, {1}, {2}, '2014/1/1', '2199/12/31')",
                                                    rentToken, tenantOrgAdminID, serialID);
                                            mmtReturn = MssqlOperation.ExecuteSql(strConn, strSql);
                                            break;
                                        case 3:
                                            strSql =
                                                string.Format(
                                                    "INSERT INTO T_11_201_{0} VALUES (0, 0, {1}, {2}, TO_DATE('2014/1/1','YYYY-MM-DD HH:MI:SS'), TO_DATE('2199/12/31','YYYY-MM-DD HH:MI:SS'))",
                                                    rentToken, tenantOrgAdminID, serialID);
                                            mmtReturn = OracleOperation.ExecuteSql(strConn, strSql);
                                            break;
                                    }
                                    if (mmtReturn != null)
                                    {
                                        if (!mmtReturn.Result)
                                        {
                                            optReturn.Message += string.Format("MMT2 fail.\t{0}\t{1}", mmtReturn.Code,
                                                mmtReturn.Message);
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #endregion

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

                    #endregion

                    #endregion

                }
                else if (intMethod == 2)
                {

                    #region 删除坐席

                    long agentObjID = agentInfo.SerialID;
                    int dbType = session.DBType;
                    string rentToken = session.RentInfo.Token;
                    string strConn = session.DBConnectionString;
                    string strSql;
                    switch (dbType)
                    {
                        case 2:
                            strSql = string.Format("DELETE FROM T_11_101_{0} WHERE C001 = {1}", rentToken, agentObjID);
                            optReturn = MssqlOperation.ExecuteSql(strConn, strSql);
                            break;
                        case 3:
                            strSql = string.Format("DELETE FROM T_11_101_{0} WHERE C001 = {1}", rentToken, agentObjID);
                            optReturn = OracleOperation.ExecuteSql(strConn, strSql);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid.");
                            return optReturn;
                    }
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    switch (dbType)
                    {
                        case 2:
                            strSql = string.Format("DELETE FROM T_11_201_{0} WHERE C004 = {1}", rentToken, agentObjID);
                            optReturn = MssqlOperation.ExecuteSql(strConn, strSql);
                            break;
                        case 3:
                            strSql = string.Format("DELETE FROM T_11_201_{0} WHERE C004 = {1}", rentToken, agentObjID);
                            optReturn = OracleOperation.ExecuteSql(strConn, strSql);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid.");
                            return optReturn;
                    }
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }

                    listReturn.Add(agentObjID.ToString());

                    #endregion

                }
                else if (intMethod == 3)
                {

                    #region 修改坐席

                    long agentObjID = agentInfo.SerialID;
                    int dbType = session.DBType;
                    string rentToken = session.RentInfo.Token;
                    string strConn = session.DBConnectionString;
                    string strSql;
                    IDbConnection objConn;
                    IDbDataAdapter objAdapter;
                    DbCommandBuilder objCmdBuilder;
                    switch (dbType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001,C002", rentToken, agentObjID);
                            objConn = MssqlOperation.GetConnection(strConn);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001,C002", rentToken, agentObjID);
                            objConn = OracleOperation.GetConnection(strConn);
                            objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid.");
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

                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[i];

                            int number = Convert.ToInt32(dr["C002"]);
                            if (number == 1)
                            {
                                dr["C011"] = agentInfo.OrgID;
                                string strAgentName = agentInfo.AgentName;
                                strAgentName = EncryptString02(strAgentName);
                                dr["C012"] = agentInfo.State;
                                dr["C018"] = strAgentName;
                            }
                        }

                        objAdapter.Update(objDataSet);
                        objDataSet.AcceptChanges();

                        listReturn.Add(agentObjID.ToString());
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

                    #endregion

                }
                else
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid.");
                    return optReturn;
                }

                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GetAgentCtledUserIDs(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListData
                //0     UserID
                //1     AgentObjID
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strAgentObjID = listParams[1];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid.");
                    return optReturn;
                }
                long agentObjID;
                if (!long.TryParse(strAgentObjID, out agentObjID)
                    || agentObjID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AgentObjID invalid.");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                int dbType = session.DBType;
                string rentToken = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_201_{0} WHERE C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C004 = {1} ORDER BY C003",
                                rentToken,
                                agentObjID);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_201_{0} WHERE C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C004 = {1} ORDER BY C003",
                                rentToken,
                                agentObjID);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid.");
                        return optReturn;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DataSet objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null.");
                    return optReturn;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    long id = Convert.ToInt64(dr["C003"]);
                    listReturn.Add(id.ToString());
                }

                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn SaveAgentMMT(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListData
                //0     UserID
                //1     AgentObjID
                //2     Method（1：添加权限；2：删除权限）
                //3     数量
                //4...  用户编码列表
                if (listParams == null
                    || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strAgentObjID = listParams[1];
                string strMethod = listParams[2];
                string strCount = listParams[3];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid.");
                    return optReturn;
                }
                long agentObjID;
                if (!long.TryParse(strAgentObjID, out agentObjID)
                    || agentObjID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AgentObjID invalid.");
                    return optReturn;
                }
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid.");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param Count invalid.");
                    return optReturn;
                }
                if (listParams.Count < intCount + 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Count invalid.");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                int dbType = session.DBType;
                string rentToken = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1}", rentToken, agentObjID);
                        objConn = MssqlOperation.GetConnection(strConn);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1}", rentToken, agentObjID);
                        objConn = OracleOperation.GetConnection(strConn);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid.");
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

                    DataRow dr;
                    if (intMethod == 1)
                    {
                        for (int i = 0; i < intCount; i++)
                        {
                            string strID = listParams[i + 4];
                            long id;
                            if (long.TryParse(strID, out id))
                            {
                                dr = objDataSet.Tables[0].Select(string.Format("C003 = {0}", id)).FirstOrDefault();
                                if (dr == null)
                                {
                                    dr = objDataSet.Tables[0].NewRow();
                                    dr["C001"] = 0;
                                    dr["C002"] = 0;
                                    dr["C003"] = id;
                                    dr["C004"] = agentObjID;
                                    dr["C005"] = DateTime.Parse("2014/1/1").ToString("yyyy-MM-dd HH:mm:ss");
                                    dr["C006"] = DateTime.Parse("2199/12/31").ToString("yyyy-MM-dd HH:mm:ss");
                                    objDataSet.Tables[0].Rows.Add(dr);
                                    listReturn.Add(string.Format("A:{0}", id));
                                }
                            }
                        }
                    }

                    if (intMethod == 2)
                    {
                        for (int i = 0; i < intCount; i++)
                        {
                            string strID = listParams[i + 4];
                            long id;
                            if (long.TryParse(strID, out id))
                            {
                                dr = objDataSet.Tables[0].Select(string.Format("C003 = {0}", id)).FirstOrDefault();
                                if (dr != null)
                                {
                                    dr.Delete();
                                    listReturn.Add(string.Format("D:{0}", id));
                                }
                            }
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn UPAgentPwd(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strPassword;
                int dbType = session.DBType;
                string strAgentID = listParams[0];
                string strPwd = listParams[1];
                string strAgentPwd = string.Format("{0}{1}", strAgentID, strPwd);
                try
                {
                    byte[] temp = ServerHashEncryption.EncryptBytes(Encoding.Unicode.GetBytes(strAgentPwd),
                                    EncryptionMode.SHA512V00Hex);
                    byte[] aes = ServerAESEncryption.EncryptBytes(temp, EncryptionMode.AES256V02Hex);
                    strPassword = ServerEncryptionUtils.Byte2Hex(aes);
                }
                catch
                {
                    strPassword = strAgentPwd;
                }
                string strRentInfo = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "UPDATE T_11_101_{0} SET C020='{1}' ,C013=0 WHERE C001={2} and C002=1",
                                strRentInfo,
                                strPassword,
                                strAgentID);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "UPDATE T_11_101_{0} SET C020='{1}' ,C013=0 WHERE C001={2} and C002=1",
                                strRentInfo,
                                strPassword,
                                strAgentID);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
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

        #region Encryption and Decryption

        private string EncrytString04(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString04(string strSource)
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

        private string EncryptString02(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString02(string strSource)
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

        private string EncryptString01(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V01Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString01(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V01Hex);
            }
            catch
            {
                return strSource;
            }
        }

        #endregion
    }
}
