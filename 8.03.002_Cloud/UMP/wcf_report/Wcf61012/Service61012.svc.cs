using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using Wcf61011;
using Common61011;
using Wcf61012.Wcf11012;
using System.Data.Common;

namespace Wcf61011
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]//这句话是查理加进去的 
    public class Service61012 : IService61012
    {
        public WebReturn UMPReportOperation(WebRequest webRequest)
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
            var dbinfo = session.DatabaseInfo;
            if (dbinfo == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = string.Format("WebRequest is null");
                return webReturn;
            }
            string RealPW = dbinfo.Password;
            dbinfo.RealPassword = DecryptString104(RealPW);
            session.DBConnectionString = dbinfo.GetConnectionString();
            if (session == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = string.Format("SessionInfo is null");
                return webReturn;
            }

            try
            {
                OperationReturn optReturn;
                List<string> listMsg;//当设置一些基本信息的时候能够用到的
                switch (webRequest.Code)
                {
                    case (int)WebCodes.GetControlOrgInfoList:
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
                    case (int)WebCodes.GetControlAgentInfoList:
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
                    case (int)WebCodes.GetControlResultsList:
                        optReturn = GetOperationResultList(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetUserList:
                        optReturn = GetUsersList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetMachineName:
                        optReturn = GetMachineName(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetMachineIP:
                        optReturn = GetMachineIP(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetUserExtInfo:
                        optReturn = GetExtension(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetVoiceIP_Name201:
                        optReturn = GetVoiceIP_Name201(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetInspectorList:
                        optReturn = GetInspectorList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetPFBList:
                        optReturn = GetPFBList(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetQueryConditions:
                        optReturn = GetQueryConditions(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetQueryItems:
                        optReturn = GetQueryItems(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetQueryConditionItems:
                        optReturn = GetQueryConditionItems(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.SaveQueryCondition:
                        optReturn = SaveQueryCondition(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.MarkToDB:
                        optReturn = MarkToDB(session, webRequest.ListData, webRequest.DataSetData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetTenant:
                        optReturn = GetTenant(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
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

        #region  一些操作
        private OperationReturn GetControlOrgInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                //1      上级机构编号（-1表示获取当前所属机构信息）,就是没有父节点的节点，就是根节点
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
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1})", rentToken, strUserID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2}", rentToken, strUserID, strParentID);
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
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1})", rentToken, strUserID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2}", rentToken, strUserID, strParentID);
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
                    string strName = dr["C001"].ToString();
                    string strFullName = dr["C002"].ToString();
                    strName = DecryptString(strName);
                    strFullName = DecryptString(strFullName);
                    string strInfo = string.Format("{0}{1}{2}", strName, ConstValue.SPLITER_CHAR, strFullName);
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 LIKE '103%' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2} )"
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 LIKE '103%' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})"
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
                    strName = DecryptString(strName);
                    strFullName = DecryptString(strFullName);
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

        private OperationReturn GetOperationResultList(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                int LangID = session.LangTypeInfo.LangID;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_00_005 WHERE C002 LIKE 'OP%' AND C001={0}", LangID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_00_005 WHERE C002 LIKE 'OP%' AND C001={0}", LangID);
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
                    string strOperationResultsCodes = dr["C002"].ToString();
                    string strOperationResultsContents = dr["C005"].ToString();
                    string strInfo = string.Format("{0}{1}{2}", strOperationResultsCodes, ConstValue.SPLITER_CHAR, strOperationResultsContents);
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

        private OperationReturn GetUsersList(SessionInfo session, List<string> listParams)
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
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006={1} AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})", rentToken, strParentID, strUserID);

                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006={1} AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})", rentToken, strParentID, strUserID);
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
                    string strFullName = dr["C003"].ToString();
                    strName = DecryptString(strName);
                    strFullName = DecryptString(strFullName);
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

        //获取机器名 
        private OperationReturn GetMachineName(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 LIKE '107%'", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 LIKE '107%'", rentToken);
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
                    string strMachineName = dr["C017"].ToString();
                    //string strVoiceIP = dr["C005"].ToString();
                    strMachineName = DecryptString(strMachineName);
                    //strVoiceIP = DecryptString(strVoiceIP);
                    string strInfo = string.Format("{0}{1}", strMachineName, ConstValue.SPLITER_CHAR);
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
        //获取机器IP
        private OperationReturn GetMachineIP(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 LIKE '108%'", rentToken);//这里要改的哦~~~~~~
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 LIKE '108%'", rentToken);
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
                    string strMachineIP = dr["C017"].ToString();
                    //string strVoiceIP = dr["C005"].ToString();
                    strMachineIP = DecryptString(strMachineIP);
                    //strVoiceIP = DecryptString(strVoiceIP);
                    string strInfo = string.Format("{0}{1}", strMachineIP, ConstValue.SPLITER_CHAR);
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

        private OperationReturn GetExtension(SessionInfo session, List<string> listParams)
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
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C011='{1}' AND  C001 LIKE '104%' AND C002=1 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2} )  AND C012!='0' ", rentToken, strParentID, strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C011='{1}' AND  C001 LIKE '104%' AND C002=1 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2} ) AND C012!='0' ", rentToken, strParentID, strUserID);
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
                    string strExtension = DecryptString(dr["C017"].ToString());
                    string strInfo = string.Format("{0}{1}", strExtension, ConstValue.SPLITER_CHAR);
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

        private OperationReturn GetVoiceIP_Name201(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * from T_11_101_{0} where C001 like '221%'  and C002=1 ", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * from T_11_101_{0} where C001 like '221%'  and C002=1 ", rentToken);
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
                    string[] arrInfo = dr["C017"].ToString().Substring(3).Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string EnVoiceIP = arrInfo[2];//从数据库里面取出来加密的那个
                    //string strVoiceName = DecryptString(dr["C018"].ToString());
                    string strVoiceIP = DecryptString(EnVoiceIP);
                    string strInfo = string.Format("{0}{1}", ConstValue.SPLITER_CHAR, strVoiceIP);
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

        private OperationReturn GetPFBList(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_001_{0} WHERE C018 = 'Y' ", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_001_{0} WHERE C018 = 'Y' ", rentToken);
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
                    string strPFBID = dr["C001"].ToString();
                    string strPFBName = dr["C002"].ToString();
                    if (string.IsNullOrEmpty(strPFBName) && string.IsNullOrWhiteSpace(strPFBName))
                    {
                        continue;
                    }
                    string strInfo = string.Format("{0}{1}{2}", strPFBID, ConstValue.SPLITER_CHAR, strPFBName);
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

        private OperationReturn GetInspectorList(SessionInfo session, List<string> listParams)
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
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006={1} AND C001 IN (SELECT C005 FROM T_31_008_{0}) AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})", rentToken, strParentID, strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006={1} AND C001 IN (SELECT C005 FROM T_31_008_{0}) AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})", rentToken, strParentID, strUserID);
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
                    string strInspectorID = dr["C001"].ToString();
                    string strInspectorName = dr["C002"].ToString();
                    string strInspectorFullName = dr["C003"].ToString();
                    strInspectorName = DecryptString(strInspectorName);
                    strInspectorFullName = DecryptString(strInspectorFullName);
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strInspectorID, ConstValue.SPLITER_CHAR, strInspectorName, strInspectorFullName);
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


        private OperationReturn GetQueryConditions(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_028_{0} WHERE C003 LIKE '6101%'", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_028_{0} WHERE C003 LIKE '6101%'", rentToken);
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
                    QueryCondition newQueryCondition = new QueryCondition();
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    newQueryCondition.QueryCode = Convert.ToInt64(dr["C001"].ToString());
                    newQueryCondition.UserID = Convert.ToInt64(dr["C002"].ToString());
                    newQueryCondition.ReportCode = Convert.ToInt64(dr["C003"].ToString());
                    newQueryCondition.SetTime = Convert.ToDateTime(dr["C004"].ToString());
                    newQueryCondition.Source = Convert.ToChar(dr["C005"].ToString());
                    newQueryCondition.Priority = Convert.ToInt32(dr["C006"].ToString());
                    newQueryCondition.LastUseTime = Convert.ToDateTime(dr["C007"].ToString());
                    newQueryCondition.mName = dr["C008"].ToString();
                    newQueryCondition.mDescription = dr["C009"].ToString();
                    newQueryCondition.IsUse = (dr["C010"].ToString() == "Y");

                    optReturn = XMLHelper.SeriallizeObject(newQueryCondition);
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

        private OperationReturn GetQueryItems(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C002 LIKE '303160222%'", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C002 LIKE '303160222%'", rentToken);
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
                    QueryConditionItem newQueryCondition = new QueryConditionItem();
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    newQueryCondition.QueryConditionCode = Convert.ToInt64(dr["C001"].ToString());
                    newQueryCondition.QueryItemCode = Convert.ToInt64(dr["C002"].ToString());
                    newQueryCondition.Value1 = dr["C004"].ToString();
                    newQueryCondition.Value2 = dr["C005"].ToString();
                    newQueryCondition.Value3 = dr["C006"].ToString();
                    newQueryCondition.Value4 = dr["C007"].ToString();
                    newQueryCondition.Value5 = dr["C008"].ToString();

                    optReturn = XMLHelper.SeriallizeObject(newQueryCondition);
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

        private OperationReturn GetQueryConditionItems(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_045_{0} WHERE C002 LIKE '303160222%'", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_045_{0} WHERE C002 LIKE '303160222%'", rentToken);
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
                    QueryConditionItem newQueryCondition = new QueryConditionItem();
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    newQueryCondition.QueryConditionCode = Convert.ToInt64(dr["C001"].ToString());
                    newQueryCondition.QueryItemCode = Convert.ToInt64(dr["C002"].ToString());
                    newQueryCondition.Sort = Convert.ToInt32(dr["C003"].ToString());
                    newQueryCondition.Value1 = dr["C004"].ToString();
                    newQueryCondition.Value2 = dr["C005"].ToString();
                    newQueryCondition.Value3 = dr["C006"].ToString();
                    newQueryCondition.Value4 = dr["C007"].ToString();
                    newQueryCondition.Value5 = dr["C008"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(newQueryCondition);
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

        private OperationReturn SaveQueryCondition(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string msg = "";
            try
            {
                //ListParam
                //0     查询条件信息，如过ID为0，则新增一个查询条件
                //1     条件详情总数
                //2...  条件详情
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strQueryCondition = listParams[0];
                int intCount;
                if (string.IsNullOrEmpty(strQueryCondition))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param invalid");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<QueryCondition>(strQueryCondition);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                QueryCondition queryCondition = optReturn.Data as QueryCondition;
                if (queryCondition == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("QueryCondition is null");
                    return optReturn;
                }
                if (queryCondition.QueryCode <= 0)
                {
                    //获取新增查询条件ID
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("61");
                    webRequest.ListData.Add("302");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
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
                    queryCondition.QueryCode = Convert.ToInt64(webReturn.Data);
                }
                msg += queryCondition.QueryCode;
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                bool bIsAdded = false;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_028_{0} WHERE C001 = {1}"
                           , rentToken
                           , queryCondition.QueryCode);
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
                        strSql = string.Format("SELECT * FROM T_31_028_{0} WHERE C001 = {1}"
                            , rentToken
                            , queryCondition.QueryCode);
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
                msg += strSql + "\t";
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
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = queryCondition.QueryCode;
                        bIsAdded = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C002"] = queryCondition.UserID;
                    dr["C003"] = queryCondition.ReportCode;
                    dr["C004"] = queryCondition.SetTime;
                    dr["C005"] = queryCondition.Source;
                    dr["C006"] = queryCondition.Priority;
                    dr["C007"] = queryCondition.LastUseTime;
                    dr["C008"] = queryCondition.mName;
                    dr["C009"] = queryCondition.mDescription;
                    dr["C010"] = queryCondition.IsUse ? "Y" : "N";
                    if (bIsAdded)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                    //bIsAdded = true;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                    return optReturn;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

                optReturn.Data = queryCondition.QueryCode;

                //44表内容添加或修改
                List<QueryConditionItem> listDetails = new List<QueryConditionItem>();
                List<QueryConditionItem> listItemDetails = new List<QueryConditionItem>();
                for (int i = 1; i < listParams.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<QueryConditionItem>(listParams[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    QueryConditionItem detail = optReturn.Data as QueryConditionItem;
                    if (detail == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("QueryConditionDetail is null");
                        return optReturn;
                    }
                    if (detail.Type == 0)
                    {
                        listDetails.Add(detail);
                    }
                    else
                    {
                        listItemDetails.Add(detail);
                    }
                }
                IDbConnection objConn44;
                IDbDataAdapter objAdapter44;
                DbCommandBuilder objCmdBuilder44;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C001 = {1}"
                            , rentToken
                            , queryCondition.QueryCode);
                        objConn44 = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter44 = MssqlOperation.GetDataAdapter(objConn44, strSql);
                        objCmdBuilder44 = MssqlOperation.GetCommandBuilder(objAdapter44);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C001 = {1}"
                            , rentToken
                            , queryCondition.QueryCode);
                        objConn44 = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter44 = OracleOperation.GetDataAdapter(objConn44, strSql);
                        objCmdBuilder44 = OracleOperation.GetCommandBuilder(objAdapter44);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not support");
                        return optReturn;
                }
                msg += strSql + "\t";
                if (objConn44 == null || objAdapter44 == null || objCmdBuilder44 == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder44.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder44.SetAllValues = false;

                DataSet DataSet44 = new DataSet();
                objAdapter44.Fill(DataSet44);
                List<string> listMsg = new List<string>();
                for (int i = 0; i < listDetails.Count; i++)
                {
                    QueryConditionItem detail = listDetails[i];
                    DataRow[] drs = DataSet44.Tables[0].Select(string.Format("C002 = {0}", detail.QueryItemCode));
                    DataRow dr;
                    bool isAdd = false;
                    if (drs.Length <= 0)
                    {
                        dr = DataSet44.Tables[0].NewRow();
                        dr["C001"] = queryCondition.QueryCode;
                        dr["C002"] = detail.QueryItemCode;
                        isAdd = true;
                    }
                    else
                    {
                        dr = drs[0];
                    }
                    dr["C003"] = "Y";
                    dr["C004"] = detail.Value1;
                    dr["C005"] = detail.Value2;
                    dr["C006"] = detail.Value3;
                    dr["C007"] = detail.Value4;
                    dr["C008"] = detail.Value5;
                    if (isAdd)
                    {
                        DataSet44.Tables[0].Rows.Add(dr);
                    }
                }
                objAdapter44.Update(DataSet44);
                DataSet44.AcceptChanges();
                optReturn.Data = listMsg;
                //保存修改45表内数据
                intCount = listItemDetails.Count;
                switch (session.DBType)
                {
                    case 2:

                        string strQueryConditionID = queryCondition.QueryCode.ToString();

                        strSql = string.Format("DELETE FROM T_31_045_{0} WHERE C001 = {1}"
                            , rentToken
                            , strQueryConditionID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        msg += strSql + "\t";
                        for (int i = 0; i < intCount; i++)
                        {
                            QueryConditionItem strSubItemInfo = listItemDetails[i];
                            string strConditionItemID = strSubItemInfo.QueryItemCode.ToString();

                            string strTempID = (i + 1).ToString();
                            strSql =
                                string.Format("INSERT INTO T_31_045_{0} VALUES( {1}, {2}, {3},'{4}','{5}', '{6}', '{7}', '{8}')"
                                    , rentToken
                                    , strQueryConditionID
                                    , strConditionItemID
                                    , strTempID
                                    , strSubItemInfo.Value1
                                     , strSubItemInfo.Value2
                                      , strSubItemInfo.Value3
                                       , strSubItemInfo.Value4
                                        , strSubItemInfo.Value5);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            msg += strSql + "\t";
                        }
                        break;
                    case 3:
                        string strQueryConditionIDOrcle = queryCondition.QueryCode.ToString();

                        strSql = string.Format("DELETE FROM T_31_045_{0} WHERE C001 = {1}"
                            , rentToken
                            , strQueryConditionIDOrcle);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        msg += strSql + "\t";
                        for (int i = 0; i < intCount; i++)
                        {
                            QueryConditionItem strSubItemInfoOrcle = listItemDetails[i];
                            string strConditionItemIDOrcle = strSubItemInfoOrcle.QueryItemCode.ToString();

                            string strTempID = (i + 1).ToString();
                            strSql =
                                  string.Format("INSERT INTO T_31_045_{0} VALUES( {1}, {2}, {3}, '{4}','{5}', '{6}', '{7}', '{8}')"
                                      , rentToken
                                      , strQueryConditionIDOrcle
                                      , strConditionItemIDOrcle
                                      , strTempID
                                      , strSubItemInfoOrcle.Value1
                                       , strSubItemInfoOrcle.Value2
                                        , strSubItemInfoOrcle.Value3
                                         , strSubItemInfoOrcle.Value4
                                          , strSubItemInfoOrcle.Value5);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            msg += strSql + "\t";
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                optReturn.Message = msg;
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

        private OperationReturn MarkToDB(SessionInfo session, List<string> listParams, DataSet DSMark)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string msg = "";
            //ListParam
            //0      表名称
            //1      列名称(实际表)
            //2      列名称（dataset）
            if (listParams == null || listParams.Count < 2 || DSMark == null || DSMark.Tables == null || DSMark.Tables.Count == 0)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_PARAM_INVALID;
                optReturn.Message = string.Format("Request param is null or count invalid");
                return optReturn;
            }
            string TableName = listParams[0];
            string ColumnName = listParams[1];
            string ColumnDS = listParams[2];
            string rentToken = session.RentInfo.Token;
            string ColumnMark; string ColumnMark2 = string.Empty; string TableName2 = string.Empty;
            if (TableName == "T_31_008")
            {
                ColumnMark = " C017 = '1' ";
                ColumnMark2 = " C018 = '1' ";
                TableName2 = "T_31_041";
            }
            else
            {
                ColumnMark = " C018 = '1' ";
                ColumnMark2 = " C017 = '1' ";
                TableName2 = "T_31_008";
            }
            string strSql;
            switch (session.DBType)
            {
                case 2:
                    for (int i = 0; i < DSMark.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = DSMark.Tables[0].Rows[i];
                        string ValueCol = dr[ColumnDS].ToString();
                        strSql = string.Format("UPDATE {0}_{1} SET {2} WHERE {3}"
                            , TableName
                            , rentToken
                            , ColumnMark
                            , ColumnName + " = '" + ValueCol + "' ");
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        { return optReturn; }

                        strSql = string.Format("UPDATE {0}_{1} SET {2} WHERE {3}"
                            , TableName2
                            , rentToken
                            , ColumnMark2
                            , ColumnName + " = '" + ValueCol + "' ");
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        { return optReturn; }
                    }
                    break;
                case 3:
                    for (int i = 0; i < DSMark.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = DSMark.Tables[0].Rows[i];
                        string ValueCol = dr[ColumnDS].ToString();
                        strSql = string.Format("UPDATE {0}_{1} SET {2} WHERE {3}"
                            , TableName
                            , rentToken
                            , ColumnMark
                            , ColumnName + " = '" + ValueCol + "' ");
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        { return optReturn; }

                        strSql = string.Format("UPDATE {0}_{1} SET {2} WHERE {3}"
                            , TableName2
                            , rentToken
                            , ColumnMark2
                            , ColumnName + " = '" + ValueCol + "' ");
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        { return optReturn; }
                    }
                    break;
            }
            return optReturn;
        }

        private OperationReturn GetTenant(SessionInfo session)
        {
            OperationReturn OptReturn = new OperationReturn();
            OptReturn.Code = 0;
            OptReturn.Result = true;
            //获取用户所在机构的租户名称
            string rentToken = session.RentInfo.Token.ToString();
            string strSql;
            string UserID = session.UserID.ToString();
            DataSet DS;
            switch (session.DBType)
            {
                case 2:
                    strSql = string.Format("SELECT C006 FROM T_11_005_{0} WHERE C001={1} "
                        , rentToken
                        , UserID);
                    OptReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                    break;
                case 3:
                    strSql = string.Format("SELECT C006 FROM T_11_005_{0} WHERE C001={1} "
                          , rentToken
                          , UserID);
                    OptReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                    break;
            }
            if (!OptReturn.Result)
            {
                return OptReturn;
            }
            DS = OptReturn.Data as DataSet;
            if (DS == null || DS.Tables == null || DS.Tables.Count == 0 || DS.Tables[0].Rows == null || DS.Tables[0].Rows.Count == 0)
            {
                OptReturn.Result = false;
                OptReturn.Code = -1;
                OptReturn.Message = "Get Org fail.";
                return OptReturn;
            }
            string OrgID = DS.Tables[0].Rows[0]["C006"].ToString();
            //获取该机构的租户名称（即父级机构是顶级机构的机构）
            if (OrgID != string.Format("101{0}00000000001", rentToken))
            {
                OrgID = FindTenantByOrgID(session, OrgID);
            }
            OptReturn.StringValue = OrgID;
            return OptReturn;
        }
        #endregion

        #region tool
        private string FindTenantByOrgID(SessionInfo session, string OrgID)
        {
            string rentToken = session.RentInfo.Token;
            string TenantID = string.Empty;
            string Strsql = string.Empty;
            DataSet ds_Tenant = new DataSet();
            OperationReturn optR = new OperationReturn();
            switch (session.DBType)
            {
                case 2:
                    Strsql = string.Format("SELECT C004 FROM T_11_006_{0} WHERE C001={1} "
                        , rentToken
                        , OrgID);
                    optR = MssqlOperation.GetDataSet(session.DBConnectionString, Strsql);
                    break;
                case 3:
                    Strsql = string.Format("SELECT C004 FROM T_11_006_{0} WHERE C001={1} "
                        , rentToken
                        , OrgID);
                    optR = OracleOperation.GetDataSet(session.DBConnectionString, Strsql);
                    break;
            }
            if (!optR.Result)
            {
                return string.Empty;
            }
            ds_Tenant = optR.Data as DataSet;
            if (ds_Tenant == null || ds_Tenant.Tables == null || ds_Tenant.Tables.Count == 0
                || ds_Tenant.Tables[0].Rows == null || ds_Tenant.Tables[0].Rows.Count == 0)
            {
                return string.Empty;
            }
            TenantID = ds_Tenant.Tables[0].Rows[0]["C004"].ToString();
            if (TenantID == string.Format("101{0}00000000001", rentToken))
            {
                return OrgID;
            }
            else
            {
                return FindTenantByOrgID(session, TenantID);
            }
        }
        #endregion
        #region 关于密的
        private static string EncryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strTemp;
        }
        private static string DecryptString104(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
             EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
        }
        private static string DecryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
              EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strTemp;
        }

        private static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
        {
            string lStrReturn;
            int LIntRand;
            Random lRandom = new Random();
            string LStrTemp;

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = lRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "VCT");
                LIntRand = lRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "UMP");
                LIntRand = lRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, ((int)aKeyIVID).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }
        #endregion
    }
}
