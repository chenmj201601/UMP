using System;
using System.Collections.Generic;
using System.Data;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using VoiceCyber.UMP.Communications;
using Wcf11011.Wcf11012;

namespace Wcf11011
{
    public partial class Service11011
    {
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
                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_005_{0} B WHERE A.C001 = B.C006 AND B.C001 = {1}"
                                , rentToken
                                , userID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C004 = {1} AND B.C003 = {2}",
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
                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_005_{0} B WHERE A.C001 = B.C006 AND B.C001 = {1}"
                                , rentToken
                                , userID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C004 = {1} AND B.C003 = {2}",
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

                    BasicOrgInfo orgInfo = new BasicOrgInfo();
                    orgInfo.OrgID = Convert.ToInt64(dr["C001"]);
                    orgInfo.OrgName = DecryptFromDB(dr["C002"].ToString());
                    orgInfo.OrgType = Convert.ToInt32(dr["C003"]);
                    orgInfo.ParentID = Convert.ToInt64(dr["C004"]);
                    orgInfo.IsActived = dr["C005"].ToString();
                    orgInfo.IsDeleted = dr["C006"].ToString();
                    orgInfo.State = dr["C007"].ToString();
                    orgInfo.StrStartTime = DecryptFromDB(dr["C008"].ToString());
                    orgInfo.StrEndTime = DecryptFromDB(dr["C009"].ToString());
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
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006 = {1} AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})",
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
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006 = {1} AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})",
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
                    //C007为H为系统内置隐藏用户
                    if (!dr["C007"].ToString().ToUpper().Equals("H"))
                    {
                        BasicUserInfo userInfo = new BasicUserInfo();
                        userInfo.UserID = Convert.ToInt64(dr["C001"]);
                        userInfo.Account = DecryptFromDB(dr["C002"].ToString());
                        userInfo.FullName = DecryptFromDB(dr["C003"].ToString());
                        userInfo.OrgID = Convert.ToInt64(dr["C006"]);
                        userInfo.SourceFlag = dr["C007"].ToString();
                        userInfo.IsLocked = dr["C008"].ToString();
                        userInfo.LockMethod = dr["C009"].ToString();
                        userInfo.StrStartTime = DecryptFromDB(dr["C017"].ToString());
                        userInfo.StrEndTime = DecryptFromDB(dr["C018"].ToString());
                        userInfo.IsActived = dr["C010"].ToString();
                        userInfo.IsDeleted = dr["C011"].ToString();
                        userInfo.State = dr["C012"].ToString();
                        userInfo.Creator = Convert.ToInt64(dr["C019"]);
                        userInfo.StrCreateTime = DecryptFromDB(dr["C020"].ToString());
                        userInfo.IsOrgManagement = dr["C027"].ToString();
                        optReturn = XMLHelper.SeriallizeObject(userInfo);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        listUsers.Add(optReturn.Data.ToString());
                    } 
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

        private OperationReturn GetUserExtInfo(SessionInfo session, string userID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (string.IsNullOrEmpty(userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                DataSet objDataSet = null;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} AND C002 = 1", rentToken, userID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} AND C002 = 1", rentToken, userID);
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
                if (objDataSet.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = strSql;
                }
                else
                {
                    DataRow dr = objDataSet.Tables[0].Rows[0];
                    ExtendUserInfo extUserInfo = new ExtendUserInfo();
                    extUserInfo.MailAddress = dr["C011"].ToString();
                    extUserInfo.PhoneNumber = dr["C012"].ToString();
                    extUserInfo.Birthday = dr["C013"].ToString();
                    extUserInfo.HeadIcon = dr["C014"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(extUserInfo);
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

        private OperationReturn GetUserRoleList(SessionInfo session, List<string> listParams)
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
                string parentID, strUserID;
                //0     parentID (-1 表示从权限表中获取用户所管理的角色）
                //1     userID
                parentID = listParams[0];
                strUserID = listParams[1];
                if (string.IsNullOrEmpty(parentID) || string.IsNullOrEmpty(strUserID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        if (parentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1}"
                             , rentToken
                             , strUserID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.*  FROM T_11_004_{0} A, T_11_201_{0} B WHERE A.C001 = B.C003 AND A.C002 = {1} AND B.C004 = {2} AND A.C006 = '0'"
                             , rentToken
                             , parentID
                             , strUserID);
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
                        if (parentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C004 = {1}"
                              , rentToken
                              , strUserID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.*  FROM T_11_004_{0} A, T_11_201_{0} B WHERE A.C001 = B.C003 AND A.C002 = {1} AND B.C004 = {2} AND A.C006 = '0'"
                            , rentToken
                            , parentID
                            , strUserID);
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
                        optReturn.Message = string.Format("Database type not support");
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listRoles = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strRoleInfo;
                    if (parentID == "-1")
                    {
                        strRoleInfo = dr["C003"].ToString();
                    }
                    else
                    {
                        string strRoleId = dr["C001"].ToString();
                        string strRoleName = dr["C004"].ToString();
                        strRoleName = DecryptFromDB(strRoleName);
                        strRoleInfo = string.Format("{0}{1}{2}", strRoleId, ConstValue.SPLITER_CHAR, strRoleName);
                    }
                    listRoles.Add(strRoleInfo);
                }
                optReturn.Data = listRoles;
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

        private OperationReturn GetControlObjectList(SessionInfo session, List<string> listParams)
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
                string parentID, strUserID;
                //0     parentID (-1 表示从权限表中获取用户所管理的对象）
                //1     userID
                parentID = listParams[0];
                strUserID = listParams[1];
                if (string.IsNullOrEmpty(parentID) || string.IsNullOrEmpty(strUserID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        if (parentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1} AND (C004 LIKE '101%' OR C004 LIKE '102%')"
                             , rentToken
                             , strUserID);
                        }
                        else
                        {
                            strSql = string.Empty;
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
                        if (parentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1} AND (C004 LIKE '101%' OR C004 LIKE '102%')"
                             , rentToken
                             , strUserID);
                        }
                        else
                        {
                            strSql = string.Empty;
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
                        optReturn.Message = string.Format("Database type not support");
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listObjs = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strObjInfo;
                    if (parentID == "-1")
                    {
                        strObjInfo = dr["C004"].ToString();
                    }
                    else
                    {
                        strObjInfo = string.Empty;
                    }
                    listObjs.Add(strObjInfo);
                }
                optReturn.Data = listObjs;
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

        private OperationReturn GetResourceProperty(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                //ListParams
                //0         资源类型
                //1         属性名称
                //2         数量
                //3..       资源编码  
                string strResourceType = listParams[0];
                string strPropertyName = listParams[1];
                string strCount = listParams[2];
                int intCount;
                if (!int.TryParse(strCount, out intCount) || intCount <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource count invalid");
                    return optReturn;
                }
                if (listParams.Count < 3 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource count invalid");
                    return optReturn;
                }

                string strResourceID = string.Empty;
                string strTempID = string.Empty;
                string rentToken = session.RentInfo.Token;
                if (intCount == 1)
                {
                    strResourceID = listParams[3];
                }
                else
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.WSInsertTempData;
                    webRequest.ListData.Add(string.Empty);
                    webRequest.ListData.Add(intCount.ToString());
                    for (int i = 0; i < intCount; i++)
                    {
                        webRequest.ListData.Add(listParams[i + 3]);
                    }
                    Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(session),
                        WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service11012"));
                    WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.DoOperation(webRequest);
                    if (!webReturn.Result)
                    {
                        optReturn.Result = false;
                        optReturn.Code = webReturn.Code;
                        optReturn.Message = webReturn.Message;
                        return optReturn;
                    }
                    strTempID = webReturn.Data;
                }
                string strSql;
                DataSet objDataSet = null;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        switch (strResourceType)
                        {
                            case "101":
                                if (intCount == 1)
                                {
                                    strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 = {1}", rentToken, strResourceID);
                                }
                                else
                                {
                                    strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 in (select c011 from t_00_901 where c001 = {1})", rentToken, strTempID);
                                }
                                break;
                            case "102":
                                if (intCount == 1)
                                {
                                    strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C001 = {1}", rentToken, strResourceID);
                                }
                                else
                                {
                                    strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C001 in (select c011 from t_00_901 where c001 = {1})", rentToken, strTempID);
                                }
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("ResourceType invalid");
                                return optReturn;
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
                        switch (strResourceType)
                        {
                            case "101":
                                if (intCount == 1)
                                {
                                    strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 = {1}", rentToken, strResourceID);
                                }
                                else
                                {
                                    strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 in (select c011 from t_00_901 where c001 = {1})", rentToken, strTempID);
                                }
                                break;
                            case "102":
                                if (intCount == 1)
                                {
                                    strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C001 = {1}", rentToken, strResourceID);
                                }
                                else
                                {
                                    strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C001 in (select c011 from t_00_901 where c001 = {1})", rentToken, strTempID);
                                }
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("ResourceType invalid");
                                return optReturn;
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
                if (objDataSet.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("Object not exist");
                    return optReturn;
                }
                List<string> listReturnInfo = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string objID;
                    string propertyValue;
                    switch (strResourceType + strPropertyName)
                    {
                        case "102Account":
                            objID = dr["C001"].ToString();
                            propertyValue = dr["C002"].ToString();
                            propertyValue = DecryptFromDB(propertyValue);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("PropertyName invalid");
                            return optReturn;
                    }
                    listReturnInfo.Add(string.Format("{0}{1}{2}", objID, ConstValue.SPLITER_CHAR, propertyValue));
                }
                optReturn.Data = listReturnInfo;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GetObjectBasicInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                //ListParam
                //0     资源类型
                //1     资源编码
                //2     要取得的资源的类型
                //3     其他参数，如上级资源的编码
                string strObjType = listParams[0];
                string strObjID = listParams[1];
                string strGetType = listParams[2];
                string strOtherParam = listParams[3];
                int intObjType;
                int intGetType;
                if (string.IsNullOrEmpty(strObjType)
                    || string.IsNullOrEmpty(strObjID)
                    || !int.TryParse(strObjType, out intObjType)
                    || !int.TryParse(strGetType, out intGetType))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param invalid");
                    return optReturn;
                }
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    #region MSSQL

                    //MSSQL
                    case 2:
                        switch (intObjType)
                        {
                            #region User

                            case ConstValue.RESOURCE_USER:
                                switch (intGetType)
                                {
                                    case ConstValue.RESOURCE_ORG:
                                        //用户所属机构
                                        if (strOtherParam == "-1")
                                        {
                                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_005_{0} B WHERE A.C001 = B.C006 AND B.C001 = {1}"
                                               , rentToken
                                               , strObjID);
                                        }
                                        else
                                        {
                                            //其他参数为上级机构编码
                                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C004 = {1} AND B.C003 = {2}",
                                                rentToken,
                                                strOtherParam,
                                                strObjID);
                                        }
                                        break;
                                    case ConstValue.RESOURCE_USER:
                                        //当前用户信息
                                        if (strOtherParam == "-1")
                                        {
                                            strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C001 = {1}"
                                               , rentToken
                                               , strObjID);
                                        }
                                        else
                                        {
                                            //其他参数为所属机构编码
                                            strSql = string.Format("SELECT A.* FROM T_11_005_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C006 = {1} AND B.C003 = {2}",
                                               rentToken,
                                               strOtherParam,
                                               strObjID);
                                        }
                                        break;
                                    default:
                                        optReturn.Result = false;
                                        optReturn.Code = Defines.RET_PARAM_INVALID;
                                        optReturn.Message = string.Format("GetObjType invalid");
                                        return optReturn;
                                }
                                optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                objDataSet = optReturn.Data as DataSet;
                                break;

                            #endregion

                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("ObjType invalid");
                                return optReturn;
                        }
                        break;

                    #endregion


                    #region Oracle

                    //ORCL
                    case 3:
                        switch (intObjType)
                        {

                            #region User

                            case ConstValue.RESOURCE_USER:
                                switch (intGetType)
                                {
                                    case ConstValue.RESOURCE_ORG:
                                        //用户所属机构
                                        if (strOtherParam == "-1")
                                        {
                                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_005_{0} B WHERE A.C001 = B.C006 AND B.C001 = {1}"
                                               , rentToken
                                               , strObjID);
                                        }
                                        else
                                        {
                                            //其他参数为上级机构编码
                                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C004 = {1} AND B.C003 = {2}",
                                                rentToken,
                                                strOtherParam,
                                                strObjID);
                                        }

                                        break;
                                    case ConstValue.RESOURCE_USER:
                                        //当前用户信息
                                        if (strOtherParam == "-1")
                                        {
                                            strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C001 = {1}"
                                               , rentToken
                                               , strObjID);
                                        }
                                        else
                                        {
                                            //其他参数为所属机构编码
                                            strSql = string.Format("SELECT A.* FROM T_11_005_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C006 = {1} AND B.C003 = {2}",
                                               rentToken,
                                               strOtherParam,
                                               strObjID);
                                        }
                                        break;
                                    default:
                                        optReturn.Result = false;
                                        optReturn.Code = Defines.RET_PARAM_INVALID;
                                        optReturn.Message = string.Format("GetObjType invalid");
                                        return optReturn;
                                }
                                optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                objDataSet = optReturn.Data as DataSet;
                                break;

                            #endregion

                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("ObjType invalid");
                                return optReturn;
                        }
                        break;

                    #endregion


                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
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

                    string strType, strID, strName;
                    strType = strGetType;
                    strID = dr["C001"].ToString();
                    strName = dr["C002"].ToString();
                    strName = DecryptFromDB(strName);
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strType, ConstValue.SPLITER_CHAR, strID, strName);
                    listReturn.Add(strInfo);
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

        private OperationReturn GetOrgTypeList(SessionInfo session, List<string> listParams)
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
                string strUserID;
                //0     用户编码（暂时不用，如果机构类型受用户管理，此为管理者的编码）
                strUserID = listParams[0];
                if (string.IsNullOrEmpty(strUserID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C001 >= 9050000000000000000 AND C001 < 9060000000000000000 AND C004 = '1' ORDER BY C002", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C001 >= 9050000000000000000 AND C001 < 9060000000000000000 AND C004 = '1' ORDER BY C002", rentToken);
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
                        optReturn.Message = string.Format("Database type not support");
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
                    OrgTypeInfo item=new OrgTypeInfo();
                    item.ID = Convert.ToInt64(dr["C001"]);
                    item.Name = dr["C006"].ToString();
                    item.Name = DecryptFromDB(item.Name);
                    item.SortID = Convert.ToInt32(dr["C002"]);
                    item.Description = dr["C009"].ToString();
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
            }
            return optReturn;
        }

        //px
        private OperationReturn GetParameters(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string rentID = session.RentID.ToString();
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT C003,C006 FROM T_11_001_{0} WHERE C003={1}", rentToken, "12010401");
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT C003,C006 FROM T_11_001_{0} WHERE C003={1}", rentToken, "12010401");
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
                        optReturn.Message = string.Format("Database type not support");
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                string parameter = objDataSet.Tables[0].Rows[0]["C006"].ToString();
                parameter = DecryptFromDB(parameter);
                optReturn.StringValue = parameter;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
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
                        if (strParentID != "0")
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C002=1 AND C001>1030000000000000000 AND C001<1040000000000000000 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2} )"
                                    , rentToken
                                    , strParentID
                                    , strUserID);
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C002=1 AND C001>1030000000000000000 AND C001<1040000000000000000 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1} )"
                                    , rentToken
                                    , strUserID); 
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strParentID != "0")
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C002=1 AND C001>1030000000000000000 AND C001<1040000000000000000 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})"
                                    , rentToken
                                    , strParentID
                                    , strUserID);
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C002=1 AND C001>1030000000000000000 AND C001<1040000000000000000 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1} )"
                                    , rentToken
                                    , strUserID);
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

                    string Time = dr["C019"].ToString();

                    BasicUserInfo userInfo = new BasicUserInfo();
                    if (Time != string.Empty)
                    {
                        string[] TimeS = Time.Split(ConstValue.SPLITER_CHAR);
                        userInfo.StrStartTime = TimeS[0];
                        userInfo.StrEndTime = TimeS[1];
                    }
                    else
                    {
                        userInfo.StrStartTime = "";
                        userInfo.StrEndTime = "";
                    }

                    userInfo.UserID = Convert.ToInt64(dr["C001"]);
                    userInfo.Account = DecryptFromDB(dr["C017"].ToString());
                    userInfo.FullName = DecryptFromDB(dr["C018"].ToString());
                    userInfo.OrgID = Convert.ToInt64(dr["C011"]);
                    userInfo.SourceFlag = dr["C016"].ToString();
                    userInfo.IsLocked = dr["C014"].ToString();
                    userInfo.LockMethod = dr["C015"].ToString();

                    userInfo.IsActived = dr["C012"].ToString();
                    if (dr["C012"].ToString() == "0")
                        userInfo.IsDeleted = "1";
                    else
                        userInfo.IsDeleted = "0";
                    //userInfo.State = dr["C012"].ToString();
                    //userInfo.Creator = Convert.ToInt64(dr["C019"]);
                    //userInfo.StrCreateTime = DecryptFromDB(dr["C020"].ToString());

                    optReturn = XMLHelper.SeriallizeObject(userInfo);
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
                        if (strParentID != "0")
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C011='{1}' AND  C001>1040000000000000000 AND C001<1050000000000000000 AND C002=1 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2} )", rentToken, strParentID, strUserID);
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001>1040000000000000000 AND C001<1050000000000000000 AND C002=1 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1} )", rentToken, strUserID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strParentID != "0")
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C011='{1}' AND  C001>1040000000000000000 AND C001<1050000000000000000 AND C002=1 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2} )", rentToken, strParentID, strUserID);
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001>1040000000000000000 AND C001<1050000000000000000 AND C002=1 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1} )", rentToken, strUserID);
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

                    string Time = dr["C019"].ToString();

                    BasicUserInfo userInfo = new BasicUserInfo();
                    if (Time != string.Empty)
                    {
                        string[] TimeS = Time.Split(ConstValue.SPLITER_CHAR);
                        if (TimeS.Length == 2)
                        {
                            userInfo.StrStartTime = TimeS[0];
                            userInfo.StrEndTime = TimeS[1];
                        }
                    }
                    else
                    {
                        userInfo.StrStartTime = "";
                        userInfo.StrEndTime = "";
                    }
                    string name = DecryptFromDB(dr["C017"].ToString());
                    string Name = string.Empty;
                    if (name != string.Empty)
                    {
                        //string[] TimeS = name.Split(ConstValue.SPLITER_CHAR);
                        //userInfo.Account = TimeS[0];
                        //userInfo.FullName = string.Format("[{0}]", TimeS[TimeS.Length - 1]);

                        string strIP1 = string.Empty;
                        string strIP2 = string.Empty;
                        string strRole = string.Empty;
                        //string[] arrInfos = name.Split(ConstValue.SPLITER_CHAR);
                        string[] arrInfos = name.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.None);
                        if (arrInfos.Length > 0)
                        {
                            name = arrInfos[0];
                        }
                        if (arrInfos.Length > 1)
                        {
                            strIP1 = arrInfos[1];
                            Name = strIP1;
                        }
                        if (arrInfos.Length > 2)
                        {
                            strRole = arrInfos[2];
                        }
                        if (arrInfos.Length > 3)
                        {
                            strIP2 = arrInfos[3];
                        }
                        int intRole; 
                        if (int.TryParse(strRole, out intRole))
                        {
                            if (intRole == 1)
                            {
                                Name = "[R]" + strIP1;
                            }
                            if (intRole == 2)
                            {
                                Name = "[S]" + strIP1;
                            }
                            if (intRole == 3)
                            {
                                Name = string.Format("{0};{1}", strIP1, strIP2);
                            }
                        }
                    }
                    userInfo.Account = name;
                    userInfo.FullName = Name;
                    userInfo.UserID = Convert.ToInt64(dr["C001"]);
                    //userInfo.Account = DecryptFromDB(dr["C017"].ToString());
                    userInfo.FullName += DecryptFromDB(dr["C018"].ToString());
                    userInfo.OrgID = Convert.ToInt64(dr["C011"]);
                    userInfo.SourceFlag = dr["C016"].ToString();
                    userInfo.IsLocked = dr["C014"].ToString();
                    userInfo.LockMethod = dr["C015"].ToString();

                    userInfo.IsActived = dr["C012"].ToString();
                    if (dr["C012"].ToString() == "0")
                        userInfo.IsDeleted = "1";
                    else
                        userInfo.IsDeleted = "0";
                    //userInfo.State = dr["C012"].ToString();
                    //userInfo.Creator = Convert.ToInt64(dr["C019"]);
                    //userInfo.StrCreateTime = DecryptFromDB(dr["C020"].ToString());

                    optReturn = XMLHelper.SeriallizeObject(userInfo);
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1050000000000000000 AND C001 < 1060000000000000000"
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= 1050000000000000000 AND C001 < 1060000000000000000"
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

                    string Time = dr["C019"].ToString();

                    BasicUserInfo userInfo = new BasicUserInfo();
                    if (Time != string.Empty)
                    {
                        string[] TimeS = Time.Split(ConstValue.SPLITER_CHAR);
                        if (TimeS.Length == 2)
                        {
                            userInfo.StrStartTime = TimeS[0];
                            userInfo.StrEndTime = TimeS[1];
                        }
                    }
                    else
                    {
                        userInfo.StrStartTime = "";
                        userInfo.StrEndTime = "";
                    }
                    string name = DecryptFromDB(dr["C017"].ToString());
                    if (name != string.Empty)
                    {
                        string[] TimeS = name.Split(ConstValue.SPLITER_CHAR);

                        userInfo.Account = TimeS[0];
                        userInfo.FullName = string.Format("[{0}]", TimeS[TimeS.Length - 1]);

                    }
                    userInfo.UserID = Convert.ToInt64(dr["C001"]);
                    //userInfo.Account = DecryptFromDB(dr["C017"].ToString());
                    userInfo.FullName += DecryptFromDB(dr["C018"].ToString());
                    userInfo.OrgID = Convert.ToInt64(dr["C011"]);
                    userInfo.SourceFlag = dr["C016"].ToString();
                    userInfo.IsLocked = dr["C014"].ToString();
                    userInfo.LockMethod = dr["C015"].ToString();

                    userInfo.IsActived = dr["C012"].ToString();
                    if (dr["C012"].ToString() == "0")
                        userInfo.IsDeleted = "1";
                    else
                        userInfo.IsDeleted = "0";
                    //userInfo.State = dr["C012"].ToString();
                    //userInfo.Creator = Convert.ToInt64(dr["C019"]);
                    //userInfo.StrCreateTime = DecryptFromDB(dr["C020"].ToString());

                    optReturn = XMLHelper.SeriallizeObject(userInfo);
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

        private OperationReturn GetResourceObjList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     用户编号
                //1     资源编号

                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strResourceID = listParams[1];
                string rentID;
                string Rentid = session.RentInfo.ID.ToString();
                if (Rentid.Length == 1)
                {
                    rentID = string.Format("100000000000000000{0}", Rentid);
                }
                else
                {
                    rentID = Rentid;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1} AND C004 LIKE '{2}%'", rentToken, rentID, strResourceID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1} AND C004 LIKE '{2}%'", rentToken, rentID, strResourceID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Object type invalid");
                        return optReturn;
                }
                
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
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strID;
                    strID = dr["C004"].ToString();
                    listReturn.Add(strID);
                }
                optReturn.Message = strSql;
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

        private OperationReturn GetAgentOrExt(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     用户编号
                //1     资源编号

                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strResourceID = listParams[1];
                
                string Rentid = session.RentInfo.ID.ToString();
                
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1} AND C004 LIKE '{2}%'", rentToken, strUserID, strResourceID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1} AND C004 LIKE '{2}%'", rentToken, strUserID, strResourceID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Object type invalid");
                        return optReturn;
                }

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
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    string strID;
                    strID = dr["C004"].ToString();
                    listReturn.Add(strID);
                }
                optReturn.Message = strSql;
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

        private OperationReturn GetDomainInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                //if (listParams == null || listParams.Count < 1)
                //{
                //    optReturn.Result = false;
                //    optReturn.Code = Defines.RET_PARAM_INVALID;
                //    optReturn.Message = string.Format("Request param is null or count invalid");
                //    return optReturn;
                //}

                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:

                        strSql = string.Format("SELECT * FROM T_00_012 WHERE C002={0}", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;

                        break;
                    case 3:

                        strSql = string.Format("SELECT * FROM T_00_012 WHERE C002={0}", rentToken);
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
                    BasicDomainInfo item = new BasicDomainInfo();

                    item.RentID = Convert.ToInt64(dr["C002"]);
                    item.DomainID = Convert.ToInt64(dr["C001"]);
                    item.DomainName = DecryptFromDB(dr["C003"].ToString().Trim()).Trim();
                    item.DomainCode = Convert.ToInt32(dr["C004"]);
                    item.DomainUserName = DecryptFromDB(dr["C005"].ToString().Trim()).Trim();
                    item.DomainUserPassWord = DecryptFromDB103(dr["C006"].ToString().Trim()).Trim();
                    item.RootDirectory = dr["C007"].ToString();
                    item.IsActive = dr["C008"].ToString() == "1";
                    item.IsActiveLogin = dr["C010"].ToString() == "1";
                    item.IsDelete = dr["C009"].ToString() == "1";
                    item.Creator = Convert.ToInt64(dr["C011"].ToString());
                    item.CreatTime = dr["C012"].ToString();
                    item.Description = dr["C999"].ToString();

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
     
    }
}