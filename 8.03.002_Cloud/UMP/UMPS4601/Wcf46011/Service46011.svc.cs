using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Common4601;
using System.Data;
using VoiceCyber.DBAccesses;
using PFShareClassesS;
using System.ServiceModel.Activation;
using System.Data.Common;
using Wcf46011.Wcf11012;


namespace Wcf46011
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service46011.svc 或 Service46011.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service46011 : IService46011
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
                    case (int)S4601Codes.GetControlOrgInfoList:
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
                    case (int)S4601Codes.GetControlAgentInfoList:
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
                    case (int)S4601Codes.GetControlExtensionInfoList:
                        optReturn = GetControlExtensionInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.GetControlRealExtensionInfoList:
                        optReturn = GetControlRealExtensionInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.GetControlSkillGroupInfoList:
                        optReturn = GetControlSkillGroupInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.GetControlUserInfoList:
                        optReturn = GetControlUserInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.GetControlObjectInfoListInSkillGroup:
                        optReturn = GetControlObjectInfoListInSkillGroup(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.GetKpiMapObjectInfo:
                        optReturn = GetKpiMapObjectInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.GetKPIInfoList:
                        optReturn = GetKPIInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.IsBandingKpi:
                        optReturn = IsBandingKpi(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S4601Codes.SaveKpiMapObjectInfo:
                        optReturn = AddOrUpdateKpiMapObjectInfo(session, webRequest.ListData);
                            //SaveKpiMapObjectInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S4601Codes.GetKpiMapObjectInfoInBP:
                        optReturn = GetKpiMapObjectInfoInBP(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.LoadKpiMapObjectInfo:
                        optReturn = LoadKpiMapObjectInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.DeleteKpiMapObjectInfo:
                        optReturn = DeleteKpiMapObjectInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.GetAllKPIInfoLists:
                        optReturn = GetAllKPIInfoLists(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.AlterState:
                        optReturn = AlterState(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.ModifyDefaultValue:
                        optReturn = ModifyDefaultValue(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4601Codes.GetControlUserInfoListInSkill:
                        optReturn = GetControlUserInfoListInSkill(session, webRequest.ListData);
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
                        webReturn.Message = string.Format("Request code invalid.\t{0}", webRequest.Code);
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
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1})"
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
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1})"
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
                    strName = DecryptFromDB(strName);
                    string strInfo = string.Format("{0}{1}{2}", strID, ConstValue.SPLITER_CHAR, strName);
                    listReturn.Add(strInfo);
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C012='1' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1030000000000000000 AND C001 < 1040000000000000000"
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C012='1' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1030000000000000000 AND C001 < 1040000000000000000"
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

        private OperationReturn GetControlExtensionInfoList(SessionInfo session, List<string> listParams)
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C012='1' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1040000000000000000 AND C001 < 1050000000000000000"
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C012='1' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1040000000000000000 AND C001 < 1050000000000000000"
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
                    //由于strName是[分机号+char[27]+ip]
                    string[] arrInfo = strName.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2)
                    {
                        continue;
                    }
                    //分机号
                    strName = arrInfo[0];
                    string strIP = arrInfo[1];
                    strFullName = DecryptFromDB(strFullName);
                    string strInfo = string.Format("{0}{1}{2}{1}{3}{1}{4}", strID, ConstValue.SPLITER_CHAR, strIP, strName, strFullName);
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

        private OperationReturn GetControlRealExtensionInfoList(SessionInfo session, List<string> listParams)
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C012='1' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1050000000000000000 AND C001 < 1060000000000000000"
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C012='1' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1050000000000000000 AND C001 < 1060000000000000000"
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
                    //由于strName是[分机号+char[27]+ip]
                    //string[] arrInfo = strName.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    //if (arrInfo.Length < 2)//问下这里~~查理
                    //{
                    //    continue;
                    //}
                    //分机号
                    //strName = arrInfo[0];
                    //string strIP = arrInfo[1];
                    //strFullName = DecryptFromDB(strFullName);
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

        private OperationReturn GetControlUserInfoList(SessionInfo session, List<string> listParams)
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
                    //登陆账号
                    string strName = dr["C002"].ToString();
                    //用户全名
                    string strFullName = dr["C003"].ToString();
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

        private OperationReturn GetControlSkillGroupInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  UserID
                //
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                string UserID = listParams[0];
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C001 LIKE '906%' ", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;

                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C001 LIKE '906%' ", rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
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
                    optReturn.Message = strSql;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<SkillGroupInfo> tempList = new List<SkillGroupInfo>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    SkillGroupInfo item = new SkillGroupInfo();
                    item.SkillGroupID = dr["C001"].ToString();
                    item.SkillGroupCode = DecryptFromDB(dr["C006"].ToString());
                    item.SkillGroupName = dr["C008"].ToString();
                    tempList.Add(item);
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < tempList.Count; i++)
                {
                    var item = tempList[i];
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

        /// <summary>
        /// //得到当前人能管理技能组下的用户
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// <returns></returns>
        private OperationReturn GetControlUserInfoListInSkill(SessionInfo session ,List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  用户编号
                //1  技能组编号
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
                        strSql = string.Format(" SELECT * FROM T_11_005_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003={1} AND C004 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})) AND C011 =0 ",
                            rentToken, strParentID, strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format(" SELECT * FROM T_11_005_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003={1} AND C004 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})) AND C011 =0 ",
                            rentToken, strParentID, strUserID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
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
                    optReturn.Message = strSql;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[j];

                    string strID = dr["C001"].ToString();
                    string strName = DecryptFromDB(dr["C002"].ToString()); 
                    string strFullName =DecryptFromDB( dr["C003"].ToString());
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


        private OperationReturn GetControlObjectInfoListInSkillGroup(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  用户编号
                //1  技能组编号
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                string strGroupingWay = listParams[2];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003={1} AND C004 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})) AND C002 =1",
                            rentToken, strParentID, strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003={1} AND C004 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})) AND C002 =1",
                            rentToken, strParentID, strUserID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
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
                    optReturn.Message = strSql;
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[j];

                    string strID = dr["C001"].ToString();
                    string strName = dr["C017"].ToString();
                    string strFullName = dr["C018"].ToString();
                    string type = string.Empty;
                    if (strID.IndexOf("104") == 0)
                    {
                        type = "E";
                    }
                    if (strID.IndexOf("105") == 0)
                    {
                        type = "R";
                    }
                    if (strID.IndexOf("103") == 0)
                    {
                        type = "A";
                    }
                    if (strGroupingWay.IndexOf(type) < 0)
                    {
                        continue;
                    }
                    if (strID.IndexOf("104") == 0)
                    {
                        //分机要特殊处理
                        strName = DecryptFromDB(strName);
                        //由于strName是[分机号+char[27]+ip]
                        string[] arrInfo = strName.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrInfo.Length < 2)
                        {
                            continue;
                        }
                        //分机号
                        strName = arrInfo[0];
                        string strIP = arrInfo[1];
                        strFullName = DecryptFromDB(strFullName);
                        string strInfo = string.Format("{0}{1}{2}{1}{3}{1}{4}", strID, ConstValue.SPLITER_CHAR, strIP, strName, strFullName);
                        listReturn.Add(strInfo);
                    }
                    else
                    {
                        strName = DecryptFromDB(strName);
                        strFullName = DecryptFromDB(strFullName);
                        string strInfo = string.Format("{0}{1}{2}{1}{3}", strID, ConstValue.SPLITER_CHAR, strName, strFullName);
                        listReturn.Add(strInfo);
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

        private OperationReturn GetKpiMapObjectInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  对象ID t_46_003的C003字段
                //1  所属单位ID t_46_003的C026字段
                //2  所属单位的名称[如果是分机/坐席/真实分机/用户，那么传父级单位;如果本身是机构或者技能组,那么传进来的就是本身h9] 
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strObjectID = listParams[0];
                string strBelongUnitID = listParams[1];
                string strBelongUnitName = listParams[2];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT A.*,B.C002 AS BC002 FROM T_46_003_{0} A,T_46_001_{0} B WHERE A.C003={1} AND A.C026 = {2} AND A.C002=B.C001 AND A.C011=0 ", rentToken, strObjectID, strBelongUnitID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT A.*,B.C002 AS BC002 FROM T_46_003_{0} A,T_46_001_{0} B WHERE A.C003={1} AND A.C026 = {2} AND A.C002=B.C001 AND A.C011=0 ", rentToken, strObjectID, strBelongUnitID);
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
                    KpiMapObjectInfo item = new KpiMapObjectInfo();
                    item.KpiMappingID = dr["C001"].ToString();
                    item.KpiID = dr["C002"].ToString();
                    item.KpiName = dr["BC002"].ToString();
                    item.ObjectID = dr["C003"].ToString();
                    item.ApplyType = dr["C004"].ToString();
                    item.ApplyCycle = dr["C005"].ToString();
                    item.IsActive = dr["C006"].ToString();
                    item.StartTime = dr["C007"].ToString();
                    item.StopTime = dr["C008"].ToString();
                    item.DropDown = dr["C009"].ToString();
                    item.ApplyAll = dr["C010"].ToString();
                    item.AdderID = dr["C012"].ToString();
                    item.AddTime = dr["C013"].ToString();
                    item.IsStartGoal1 = dr["C014"].ToString();
                    item.GoldValue1 = dr["C015"].ToString();
                    item.GoalOperation1 = dr["C016"].ToString();
                    item.IsStartMultiRegion1 = dr["C017"].ToString();
                    item.IsStartGoal2 = dr["C018"].ToString();
                    item.GoldValue2 = dr["C019"].ToString();
                    item.GoalOperation2 = dr["C020"].ToString();
                    item.IsStartMultiRegion2 = dr["C021"].ToString();
                    item.BelongOrgSkg = strBelongUnitName;
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

        private OperationReturn GetKPIInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  对象类型 “103”“104”“101”“906”“102”“105”~~~
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strObjectType = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_46_001_{0} WHERE C009='1' ", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_46_001_{0} WHERE C009='1' ", rentToken);
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
                    KpiInfo item = new KpiInfo();
                    item.KpiID = dr["C001"].ToString();
                    item.Name = dr["C002"].ToString();
                    item.Description = dr["C003"].ToString();
                    item.Creator = dr["C004"].ToString();
                    item.CreateTime = dr["C005"].ToString();
                    item.UseType = dr["C006"].ToString();
                    switch (strObjectType)
                    {
                        case "103":
                            if (item.UseType.Substring(0, 1) != "1")
                            {
                                continue;
                            }
                            break;
                        case "104":
                            if (item.UseType.Substring(1, 1) != "1")
                            {
                                continue;
                            }
                            break;
                        case "102":
                            if (item.UseType.Substring(2, 1) != "1")
                            {
                                continue;
                            }
                            break;
                        case "105":
                            if (item.UseType.Substring(3, 1) != "1")
                            {
                                continue;
                            }
                            break;
                        case "101":
                            if (item.UseType.Substring(4, 1) != "1")
                            {
                                continue;
                            }
                            break;
                        case "906":
                            if (item.UseType.Substring(5, 1) != "1")
                            {
                                continue;
                            }
                            break;
                    }
                    item.KpiType = dr["C007"].ToString();
                    item.SourceType = dr["C008"].ToString();
                    item.Active = dr["C009"].ToString();
                    item.ValueFormat = dr["C010"].ToString();
                    item.ApplyCycle = dr["C011"].ToString();
                    item.IsStart1 = dr["C013"].ToString();
                    item.GoalValue1 = dr["C014"].ToString();
                    item.GoalOperator1 = dr["C015"].ToString();
                    item.IsStartMultiRegion1 = dr["C016"].ToString();
                    item.IsStart2 = dr["C017"].ToString();
                    item.GoalValue2 = dr["C018"].ToString();
                    item.GoalOperator2 = dr["C019"].ToString();
                    item.IsStartMultiRegion2 = dr["C020"].ToString();
                    item.DefaultSymbol = dr["C025"].ToString();
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

        private OperationReturn IsBandingKpi(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  KpiID
                //1  ObjectID
                //2  ParantID
                //3  应用周期
                //4  应用对象

                //逻辑是一个人同一个kpi同一个周期只允许出现在技能组和部门一次
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strKpiID = listParams[0];
                string strObjectID = listParams[1];
                string strParantID = listParams[2];
                string strApplyCycle = listParams[3];
                string strApplyType = listParams[4];
                string rentToken = session.RentInfo.Token;
                string strSql;
                string strID;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT COUNT(1) FROM T_46_003_{0} WHERE C002={1} AND C003={2}  AND C004='{4}' AND C005='{5}'   AND C026={3} AND C011 =0",
                            rentToken, strKpiID, strObjectID, strParantID, strApplyType, strApplyCycle);
                        optReturn = MssqlOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (optReturn.IntValue == 0)
                        {
                            optReturn.Data = "0";
                        }
                        else
                        {
                            optReturn.Data = "1";
                            strSql = string.Format("SELECT * FROM T_46_003_{0} WHERE C002={1} AND C003={2}  AND C004='{4}' AND C005='{5}'  AND C026={3} AND C011 =0", rentToken, strKpiID, strObjectID, strParantID, strApplyType, strApplyCycle);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            objDataSet = optReturn.Data as DataSet;
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
                                strID = dr["C001"].ToString();
                                optReturn.Data = strID;
                            }
                        }
                        if (!optReturn.Result)
                        {
                            optReturn.Message = strSql;
                            optReturn.Result = false;
                            return optReturn;
                        }
                        //objDataSet = optReturn.Data as DataSet;
                        optReturn.Message = strSql;
                        break;
                    case 3:
                        strSql = string.Format("SELECT COUNT(1) FROM T_46_003_{0} WHERE C002={1} AND C003={2}  AND C004='{4}' AND C005='{5}'  AND C026={3} AND C011 =0 ",
                          rentToken, strKpiID, strObjectID, strParantID, strApplyType, strApplyCycle);
                        optReturn = OracleOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (optReturn.IntValue == 0)
                        {
                            optReturn.Data = "0";
                        }
                        else
                        {
                            optReturn.Data = "1";
                            strSql = string.Format("SELECT * FROM T_46_003_{0} WHERE C002={1} AND C003={2}  AND C004='{4}' AND C005='{5}' AND C026={3} AND C011 =0 ",
                          rentToken, strKpiID, strObjectID, strParantID, strApplyType, strApplyCycle);
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
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
                                strID = dr["C001"].ToString();
                                optReturn.Data = strID;
                            }
                        }
                        if (!optReturn.Result)
                        {
                            optReturn.Message += strSql;
                            //optReturn.Result = false;
                            return optReturn;
                        }
                        //objDataSet = optReturn.Data as DataSet;
                        optReturn.Message = strSql;
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

        private OperationReturn SaveKpiMapObjectInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //1 KpiMapObjectInfo信息
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strXML = listParams[0];
                KpiMapObjectInfo kpiMapObjectInfo = new KpiMapObjectInfo();
                optReturn = XMLHelper.DeserializeObject<KpiMapObjectInfo>(strXML);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                kpiMapObjectInfo = optReturn.Data as KpiMapObjectInfo;
                if (kpiMapObjectInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("KpiMapObjectInfo is null");
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
                        strSql = string.Format("SELECT * FROM T_46_003_{0} WHERE C011='0' ", rentToken);
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
                        strSql = string.Format("SELECT * FROM T_46_003_{0} WHERE C011='0' ", rentToken);
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

                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr_ = objDataSet.Tables[0].Rows[i];
                        string strID = dr_["C001"].ToString();
                        if (strID == kpiMapObjectInfo.KpiMappingID)
                        {
                            objDataSet.Tables[0].Rows[i].Delete();
                            listReturn.Add(string.Format("D{0}{1}", ConstValue.SPLITER_CHAR, strID));
                        }
                    }

                    DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C001 = {0}", kpiMapObjectInfo.KpiMappingID));
                    DataRow dr;
                    bool isAdd = false;
                    if (drs.Length <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = kpiMapObjectInfo.KpiMappingID;
                        isAdd = true;
                    }
                    else
                    {
                        dr = drs[0];
                    }
                    dr["C002"] = kpiMapObjectInfo.KpiID;
                    dr["C003"] = kpiMapObjectInfo.ObjectID;
                    dr["C004"] = kpiMapObjectInfo.ApplyType;
                    dr["C005"] = kpiMapObjectInfo.ApplyCycle;
                    dr["C006"] = kpiMapObjectInfo.IsActive;
                    dr["C007"] = Convert.ToDateTime(kpiMapObjectInfo.StartTime).ToString("yyyyMMddHHmmss");
                    dr["C008"] = Convert.ToDateTime(kpiMapObjectInfo.StopTime).ToString("yyyyMMddHHmmss");
                    if (string.IsNullOrEmpty(kpiMapObjectInfo.DropDown))
                    {
                        dr["C009"] = "0";
                    }
                    else
                    {
                        dr["C009"] = kpiMapObjectInfo.DropDown;
                    }

                    if (string.IsNullOrEmpty(kpiMapObjectInfo.ApplyAll))
                    {
                        dr["C010"] = "0";
                    }
                    else
                    {
                        dr["C010"] = kpiMapObjectInfo.ApplyAll;
                    }
                    dr["C011"] = "0";
                    dr["C012"] = kpiMapObjectInfo.AdderID;
                    dr["C013"] = Convert.ToDateTime(kpiMapObjectInfo.AddTime).ToString("yyyyMMddHHmmss");
                    dr["C014"] = kpiMapObjectInfo.IsStartGoal1;
                    dr["C015"] = kpiMapObjectInfo.GoldValue1;
                    dr["C016"] = kpiMapObjectInfo.GoalOperation1;
                    dr["C017"] = kpiMapObjectInfo.IsStartMultiRegion1;
                    dr["C018"] = kpiMapObjectInfo.IsStartGoal2;
                    dr["C019"] = kpiMapObjectInfo.GoldValue2;
                    dr["C020"] = kpiMapObjectInfo.GoalOperation2;
                    dr["C021"] = kpiMapObjectInfo.IsStartMultiRegion2;
                    dr["C026"] = kpiMapObjectInfo.BelongOrgSkg;
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                        listReturn.Add(string.Format("A{0}{1}", ConstValue.SPLITER_CHAR, kpiMapObjectInfo.KpiMappingID));
                    }
                    else
                    {
                        listReturn.Add(string.Format("U{0}{1}", ConstValue.SPLITER_CHAR, kpiMapObjectInfo.KpiMappingID));
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

        private OperationReturn GetKpiMappingID(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = session;
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.ListData.Add("46");
                webRequest.ListData.Add("603");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(session),
                    WebHelper.CreateEndpointAddress(
                        session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                optReturn.Data = webReturn.Data;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }


        private OperationReturn AddOrUpdateKpiMapObjectInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //1 KpiMapObjectInfo信息
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strXML = listParams[0];
                KpiMapObjectInfo kpiMapObjectInfo = new KpiMapObjectInfo();
                optReturn = XMLHelper.DeserializeObject<KpiMapObjectInfo>(strXML);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                kpiMapObjectInfo = optReturn.Data as KpiMapObjectInfo;
                if (kpiMapObjectInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("KpiMapObjectInfo is null");
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
                        strSql = string.Format("SELECT * FROM T_46_003_{0} WHERE C002={1} AND C003={2}  AND C004='{4}' AND C005='{5}'   AND C026={3} AND C011 =0 ", rentToken, kpiMapObjectInfo.KpiID, kpiMapObjectInfo.ObjectID, kpiMapObjectInfo.BelongOrgSkg, kpiMapObjectInfo.ApplyType, kpiMapObjectInfo.ApplyCycle);
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
                        strSql = string.Format("SELECT * FROM T_46_003_{0} WHERE C002={1} AND C003={2}  AND C004='{4}' AND C005='{5}'   AND C026={3} AND C011 =0 ", rentToken, kpiMapObjectInfo.KpiID, kpiMapObjectInfo.ObjectID, kpiMapObjectInfo.BelongOrgSkg, kpiMapObjectInfo.ApplyType, kpiMapObjectInfo.ApplyCycle);
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

                    if(objDataSet !=null && objDataSet.Tables[0] !=null)
                    {
                        DataRow drCurrent = objDataSet.Tables[0].Rows.Count > 0 ? objDataSet.Tables[0].Rows[0] : null;
                        if (drCurrent != null)//更新
                        {
                            drCurrent.BeginEdit();                            
                            drCurrent["C006"] = kpiMapObjectInfo.IsActive;
                            drCurrent["C007"] = Convert.ToDateTime(kpiMapObjectInfo.StartTime).ToString("yyyyMMddHHmmss");
                            drCurrent["C008"] = Convert.ToDateTime(kpiMapObjectInfo.StopTime).ToString("yyyyMMddHHmmss");
                            if (string.IsNullOrEmpty(kpiMapObjectInfo.DropDown))
                            {
                                drCurrent["C009"] = "0";
                            }
                            else
                            {
                                drCurrent["C009"] = kpiMapObjectInfo.DropDown;
                            }

                            if (string.IsNullOrEmpty(kpiMapObjectInfo.ApplyAll))
                            {
                                drCurrent["C010"] = "0";
                            }
                            else
                            {
                                drCurrent["C010"] = kpiMapObjectInfo.ApplyAll;
                            }
                            drCurrent["C011"] = "0";
                            drCurrent["C012"] = kpiMapObjectInfo.AdderID;
                            drCurrent["C013"] = Convert.ToDateTime(kpiMapObjectInfo.AddTime).ToString("yyyyMMddHHmmss");
                            drCurrent["C014"] = kpiMapObjectInfo.IsStartGoal1;
                            drCurrent["C015"] = kpiMapObjectInfo.GoldValue1;
                            drCurrent["C016"] = kpiMapObjectInfo.GoalOperation1;
                            drCurrent["C017"] = kpiMapObjectInfo.IsStartMultiRegion1;
                            drCurrent["C018"] = kpiMapObjectInfo.IsStartGoal2;
                            drCurrent["C019"] = kpiMapObjectInfo.GoldValue2;
                            drCurrent["C020"] = kpiMapObjectInfo.GoalOperation2;
                            drCurrent["C021"] = kpiMapObjectInfo.IsStartMultiRegion2;
                            drCurrent.EndEdit();
                            listReturn.Add(string.Format("U{0}{1}", ConstValue.SPLITER_CHAR, kpiMapObjectInfo.KpiMappingID));
                        }
                        else//新增
                        {
                            DataRow drNewRow = objDataSet.Tables[0].NewRow();
                           optReturn = GetKpiMappingID(session);
                            if(optReturn.Result)
                            {
                                drNewRow["C001"] = optReturn.Data.ToString();
                                drNewRow["C002"] = kpiMapObjectInfo.KpiID;
                                drNewRow["C003"] = kpiMapObjectInfo.ObjectID;
                                drNewRow["C004"] = kpiMapObjectInfo.ApplyType;
                                drNewRow["C005"] = kpiMapObjectInfo.ApplyCycle;
                                drNewRow["C006"] = kpiMapObjectInfo.IsActive;
                                drNewRow["C007"] = Convert.ToDateTime(kpiMapObjectInfo.StartTime).ToString("yyyyMMddHHmmss");
                                drNewRow["C008"] = Convert.ToDateTime(kpiMapObjectInfo.StopTime).ToString("yyyyMMddHHmmss");
                                if (string.IsNullOrEmpty(kpiMapObjectInfo.DropDown))
                                {
                                    drNewRow["C009"] = "0";
                                }
                                else
                                {
                                    drNewRow["C009"] = kpiMapObjectInfo.DropDown;
                                }

                                if (string.IsNullOrEmpty(kpiMapObjectInfo.ApplyAll))
                                {
                                    drNewRow["C010"] = "0";
                                }
                                else
                                {
                                    drNewRow["C010"] = kpiMapObjectInfo.ApplyAll;
                                }
                                drNewRow["C011"] = "0";
                                drNewRow["C012"] = kpiMapObjectInfo.AdderID;
                                drNewRow["C013"] = Convert.ToDateTime(kpiMapObjectInfo.AddTime).ToString("yyyyMMddHHmmss");
                                drNewRow["C014"] = kpiMapObjectInfo.IsStartGoal1;
                                drNewRow["C015"] = kpiMapObjectInfo.GoldValue1;
                                drNewRow["C016"] = kpiMapObjectInfo.GoalOperation1;
                                drNewRow["C017"] = kpiMapObjectInfo.IsStartMultiRegion1;
                                drNewRow["C018"] = kpiMapObjectInfo.IsStartGoal2;
                                drNewRow["C019"] = kpiMapObjectInfo.GoldValue2;
                                drNewRow["C020"] = kpiMapObjectInfo.GoalOperation2;
                                drNewRow["C021"] = kpiMapObjectInfo.IsStartMultiRegion2;
                                drNewRow["C026"] = kpiMapObjectInfo.BelongOrgSkg;
                                objDataSet.Tables[0].Rows.Add(drNewRow);
                                listReturn.Add(string.Format("A{0}{1}", ConstValue.SPLITER_CHAR, kpiMapObjectInfo.KpiMappingID));
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

        //绑定界面的初始值
        private OperationReturn GetKpiMapObjectInfoInBP(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  KpiID
                //1  ObjectID
                //2  ParantID
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strKpiID = listParams[0];
                string strObjectID = listParams[1];
                string strParantID = listParams[2];
                string strApplyCycle = listParams[3];
                string strApplyType = listParams[4];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_46_003_{0} WHERE C002={1} AND C003={2} AND C026={3} AND C011=0 AND C004 ='{4}' AND C005 = '{5}' ",
                            rentToken, strKpiID, strObjectID, strParantID, strApplyType, strApplyCycle);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_46_003_{0} WHERE C002={1} AND C003={2} AND C026={3} AND C011=0 AND C004 ='{4}' AND C005 = '{5}' ",
                            rentToken, strKpiID, strObjectID, strParantID, strApplyType, strApplyCycle);
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
                    KpiMapObjectInfo item = new KpiMapObjectInfo();
                    item.KpiMappingID = dr["C001"].ToString();
                    item.KpiID = dr["C002"].ToString();
                    item.ObjectID = dr["C003"].ToString();
                    item.ApplyType = dr["C004"].ToString();
                    item.ApplyCycle = dr["C005"].ToString();
                    item.IsActive = dr["C006"].ToString();
                    item.StartTime = dr["C007"].ToString();
                    item.StopTime = dr["C008"].ToString();
                    item.DropDown = dr["C009"].ToString();
                    item.ApplyAll = dr["C010"].ToString();
                    item.AdderID = dr["C012"].ToString();
                    item.AddTime = dr["C013"].ToString();
                    item.IsStartGoal1 = dr["C014"].ToString();
                    item.GoldValue1 = dr["C015"].ToString();
                    item.GoalOperation1 = dr["C016"].ToString();
                    item.IsStartMultiRegion1 = dr["C017"].ToString();
                    item.IsStartGoal2 = dr["C018"].ToString();
                    item.GoldValue2 = dr["C019"].ToString();
                    item.GoalOperation2 = dr["C020"].ToString();
                    item.IsStartMultiRegion2 = dr["C021"].ToString();
                    item.BelongOrgSkg = dr["C026"].ToString();
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

        //加载Parant类型为机构的
        private OperationReturn LoadKpiMapObjectInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  ObjectID
                //1  ParantID
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strObjectID = listParams[0];
                string strParantID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strParantID.IndexOf("101") == 0)
                        {
                            strSql = string.Format(" SELECT A.*,B.C002 AS BC002,C.C003 AS CC003,D.C002 AS DC002 FROM T_46_003_00000 A,T_46_001_00000 B,T_11_005_{0} C,T_11_006_{0} D ", rentToken);
                            strSql += string.Format(" WHERE A.C003 = {0} AND A.C026={1} AND A.C011=0 AND A.C002=B.C001 AND A.C012=C.C001 AND A.C026=D.C001 ", strObjectID, strParantID);
                        }
                        else
                        {
                            strSql = string.Format(" SELECT A.*,B.C002 AS BC002,C.C003 AS CC003,D.C008 AS DC008 FROM T_46_003_00000 A,T_46_001_00000 B,T_11_005_{0} C,T_11_009_{0} D ", rentToken);
                            strSql += string.Format(" WHERE A.C003 = {0} AND A.C026={1} AND A.C011=0 AND A.C002=B.C001 AND A.C012=C.C001 AND A.C026=D.C001 ", strObjectID, strParantID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strParantID.IndexOf("101") == 0)//父亲是机构
                        {
                            strSql = string.Format(" SELECT A.*,B.C002 AS BC002,C.C003 AS CC003,D.C002 AS DC002 FROM T_46_003_00000 A,T_46_001_00000 B,T_11_005_{0} C,T_11_006_{0} D ", rentToken);
                            strSql += string.Format(" WHERE A.C003 = {0} AND A.C026={1} AND A.C011=0 AND A.C002=B.C001 AND A.C012=C.C001 AND A.C026=D.C001 ", strObjectID, strParantID);
                        }
                        else//父亲是技能组
                        {
                            strSql = string.Format(" SELECT A.*,B.C002 AS BC002,C.C003 AS CC003,D.C008 AS DC008 FROM T_46_003_00000 A,T_46_001_00000 B,T_11_005_{0} C,T_11_009_{0} D ", rentToken);
                            strSql += string.Format(" WHERE A.C003 = {0} AND A.C026={1} AND A.C011=0 AND A.C002=B.C001 AND A.C012=C.C001 AND A.C026=D.C001 ", strObjectID, strParantID);
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
                    KpiMapObjectInfo item = new KpiMapObjectInfo();
                    item.KpiMappingID = dr["C001"].ToString();
                    item.KpiID = dr["C002"].ToString();
                    item.ObjectID = dr["C003"].ToString();
                    item.ApplyType = dr["C004"].ToString();
                    item.ApplyCycle = dr["C005"].ToString();
                    item.IsActive = dr["C006"].ToString();
                    item.StartTime = dr["C007"].ToString();
                    item.StopTime = dr["C008"].ToString();
                    item.DropDown = dr["C009"].ToString();
                    item.ApplyAll = dr["C010"].ToString();
                    item.AdderID = dr["C012"].ToString();
                    item.AddTime = dr["C013"].ToString();
                    item.IsStartGoal1 = dr["C014"].ToString();
                    item.GoldValue1 = dr["C015"].ToString();
                    item.GoalOperation1 = dr["C016"].ToString();
                    item.IsStartMultiRegion1 = dr["C017"].ToString();
                    item.IsStartGoal2 = dr["C018"].ToString();
                    item.GoldValue2 = dr["C019"].ToString();
                    item.GoalOperation2 = dr["C020"].ToString();
                    item.IsStartMultiRegion2 = dr["C021"].ToString();
                    item.BelongOrgSkg = dr["C026"].ToString();
                    item.KpiName = DecryptFromDB(dr["BC002"].ToString());
                    item.AdderName = DecryptFromDB(dr["CC003"].ToString());
                    if (strParantID.IndexOf("101") == 0)
                    {
                        item.BelongOrgSkgName = DecryptFromDB(dr["DC002"].ToString());
                    }
                    else
                    {
                        item.BelongOrgSkgName = DecryptFromDB(dr["DC008"].ToString());
                    }
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



        private OperationReturn DeleteKpiMapObjectInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0 KpiMapingID
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string stKpiMapingID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("UPDATE  T_46_003_{0}  SET C011='1' WHERE C001={1}", rentToken, stKpiMapingID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("UPDATE  T_46_003_{0}  SET C011='1' WHERE C001={1}", rentToken, stKpiMapingID);
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

        private OperationReturn GetAllKPIInfoLists(SessionInfo session)
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
                        strSql = string.Format("SELECT * FROM T_46_001_{0}", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_46_001_{0}", rentToken);
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
                    KpiInfo item = new KpiInfo();
                    item.KpiID = dr["C001"].ToString();
                    item.Name = dr["C002"].ToString();
                    item.Description = dr["C003"].ToString();
                    item.Creator = dr["C004"].ToString();
                    item.CreateTime = dr["C005"].ToString();
                    item.UseType = dr["C006"].ToString();
                    item.KpiType = dr["C007"].ToString();
                    item.SourceType = dr["C008"].ToString();
                    item.Active = dr["C009"].ToString();
                    item.ValueFormat = dr["C010"].ToString();
                    item.ApplyCycle = dr["C011"].ToString();
                    item.IsStart1 = dr["C013"].ToString();
                    item.GoalValue1 = dr["C014"].ToString();
                    item.GoalOperator1 = dr["C015"].ToString();
                    item.IsStartMultiRegion1 = dr["C016"].ToString();
                    item.IsStart2 = dr["C017"].ToString();
                    item.GoalValue2 = dr["C018"].ToString();
                    item.GoalOperator2 = dr["C019"].ToString();
                    item.IsStartMultiRegion2 = dr["C020"].ToString();
                    item.DefaultSymbol = dr["C025"].ToString();
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

        private OperationReturn AlterState(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0 KpiID
                //1 当前状态[1为启用 0为不启用]
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strKpiID = listParams[0];
                string strState = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        if (strState == "1")
                        {
                            strSql = string.Format("UPDATE t_46_001_{0} SET C009 =0 WHERE C001 = {1}", rentToken, strKpiID);
                        }
                        else
                        {
                            strSql = string.Format("UPDATE t_46_001_{0} SET C009 =1 WHERE C001 = {1}", rentToken, strKpiID);
                        }
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        if (strState == "1")
                        {
                            strSql = string.Format("UPDATE T_46_001_{0} SET C009 =0 WHERE C001 = {1}", rentToken, strKpiID);
                        }
                        else
                        {
                            strSql = string.Format("UPDATE T_46_001_{0} SET C009 =1 WHERE C001 = {1}", rentToken, strKpiID);
                        }
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        //optReturn.Result = false;
                        //optReturn.Code = Defines.RET_FAIL;
                        //optReturn.Message = string.Format("aaa{0}", strSql);
                        //return optReturn;
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

        private OperationReturn ModifyDefaultValue(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0 KpiInfo 的Xml序列化文本
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strXML = listParams[0];
                KpiInfo kpiInfo = new KpiInfo();
                optReturn = XMLHelper.DeserializeObject<KpiInfo>(strXML);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                kpiInfo = optReturn.Data as KpiInfo;
                if (kpiInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("KpiMapObjectInfo is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("UPDATE T_46_001_{0} SET C025 = '{1}',C014={2},C018={3} WHERE C001={4}",
                            rentToken, kpiInfo.DefaultSymbol, kpiInfo.GoalValue1, kpiInfo.GoalValue2,kpiInfo.KpiID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("UPDATE T_46_001_{0} SET C025 = '{1}',C014={2},C018={3} WHERE C001={4}",
                            rentToken, kpiInfo.DefaultSymbol, kpiInfo.GoalValue1, kpiInfo.GoalValue2, kpiInfo.KpiID);
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

        #region Encryption and Decryption

        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType keyIVID)
        {
            string strReturn;
            Random random = new Random();
            string strTemp;

            try
            {
                strReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                int intRand = random.Next(0, 14);
                strTemp = intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, "VCT");
                intRand = random.Next(0, 17);
                strTemp += intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, "UMP");
                intRand = random.Next(0, 20);
                strTemp += intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, ((int)keyIVID).ToString("000"));

                strReturn = EncryptionAndDecryption.EncryptStringY(strTemp + strReturn);
            }
            catch { strReturn = string.Empty; }

            return strReturn;
        }

        private string EncryptToDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
              EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strReturn;
        }

        private string DecryptFromDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
              EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strReturn;
        }

        private string EncryptToClient(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strReturn;
        }

        private string DecryptFromClient(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
             EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strReturn;
        }

        private string EncryptShaToDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptStringSHA512(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
             EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strReturn;
        }

        #endregion
    }
}
