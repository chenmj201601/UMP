using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Activation;
using System.Text;
using System.Web;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;

namespace Wcf11012
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service11012”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service11012.svc 或 Service11012.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service11012 : IService11012
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
                    case (int)RequestCode.WSGetLangList:
                        optReturn = GetLangList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSGetUserOptList:
                        optReturn = GetUserOptList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSGetUserCtlObjList:
                        optReturn = GetUserCtlObjList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSGetUserObjList:
                        optReturn = GetUserObjList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSGetBasicDataInfoList:
                        optReturn = GetBasicDataInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSGetUserViewColumnList:
                        optReturn = GetUserViewColumnList(session, webRequest.ListData);
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
                    case (int)RequestCode.WSInsertTempData:
                        optReturn = InsertTempData(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)RequestCode.WSGetLayoutInfo:
                        optReturn = GetLayoutInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)RequestCode.WSSaveLayoutInfo:
                        optReturn = SaveLayoutInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)RequestCode.WSGetResourceProperty:
                        optReturn = GetResourceProperty(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSGetAgentObjList:
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
                    case (int)RequestCode.WSGetGlobalParamList:
                        optReturn = GetGlobalParamList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSGetGlobalParamList2:
                        optReturn = GetGlobalParamList2(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSGetUserParamList:
                        optReturn = GetUserParamList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSSaveUserParamList:
                        optReturn = SaveUserParamList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSWriteOperationLog:
                        optReturn = WriteOperationLog(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)RequestCode.WSGetDBInfo:
                        optReturn = GetDBInfo(session, webRequest.ListData);
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

        private OperationReturn GetLangList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     LangID
                //1     PreName（语言内容编码的前缀，比如 FO:模块、操作显示语言）
                //2     ModuleID
                //3     SubModuleID
                //4     Page
                //6     Name
                //注：以上筛选条件中每个条件均可使用char27隔成多个值
                if (listParams == null || listParams.Count < 6)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strLangID, strPreName, strModuleID, strSubModuleID, strPage, strName;
                strLangID = listParams[0];
                strPreName = listParams[1];
                strModuleID = listParams[2];
                strSubModuleID = listParams[3];
                strPage = listParams[4];
                strName = listParams[5];
                string strSql;
                string strCondition = string.Empty;
                string strSingleCondition;
                strSql = string.Format("SELECT * FROM T_00_005 WHERE");
                if (!string.IsNullOrEmpty(strLangID))
                {
                    string[] arr = strLangID.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    strSingleCondition = string.Empty;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        strSingleCondition += string.Format(" C001 = {0} Or", arr[i]);
                    }
                    if (!string.IsNullOrEmpty(strSingleCondition))
                    {
                        strSingleCondition = strSingleCondition.Substring(0, strSingleCondition.Length - 3);
                    }
                    strCondition += string.Format(" ({0}) AND", strSingleCondition);
                }
                if (!string.IsNullOrEmpty(strPreName))
                {
                    string[] arr = strPreName.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    strSingleCondition = string.Empty;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        strSingleCondition += string.Format(" C002 LIKE '{0}%' Or", arr[i]);
                    }
                    if (!string.IsNullOrEmpty(strSingleCondition))
                    {
                        strSingleCondition = strSingleCondition.Substring(0, strSingleCondition.Length - 3);
                    }
                    strCondition += string.Format(" ({0}) AND", strSingleCondition);
                }
                if (!string.IsNullOrEmpty(strModuleID))
                {
                    string[] arr = strModuleID.Split(new[] { ConstValue.SPLITER_CHAR },
                      StringSplitOptions.RemoveEmptyEntries);
                    strSingleCondition = string.Empty;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        strSingleCondition += string.Format(" C009 = {0} Or", arr[i]);
                    }
                    if (!string.IsNullOrEmpty(strSingleCondition))
                    {
                        strSingleCondition = strSingleCondition.Substring(0, strSingleCondition.Length - 3);
                    }
                    strCondition += string.Format(" ({0}) AND", strSingleCondition);
                }
                if (!string.IsNullOrEmpty(strSubModuleID))
                {
                    string[] arr = strSubModuleID.Split(new[] { ConstValue.SPLITER_CHAR },
                     StringSplitOptions.RemoveEmptyEntries);
                    strSingleCondition = string.Empty;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        strSingleCondition += string.Format(" C010 = {0} Or", arr[i]);
                    }
                    if (!string.IsNullOrEmpty(strSingleCondition))
                    {
                        strSingleCondition = strSingleCondition.Substring(0, strSingleCondition.Length - 3);
                    }
                    strCondition += string.Format(" ({0}) AND", strSingleCondition);
                }
                if (!string.IsNullOrEmpty(strPage))
                {
                    string[] arr = strPage.Split(new[] { ConstValue.SPLITER_CHAR },
                    StringSplitOptions.RemoveEmptyEntries);
                    strSingleCondition = string.Empty;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        strSingleCondition += string.Format(" C011 = '{0}' Or", arr[i]);
                    }
                    if (!string.IsNullOrEmpty(strSingleCondition))
                    {
                        strSingleCondition = strSingleCondition.Substring(0, strSingleCondition.Length - 3);
                    }
                    strCondition += string.Format(" ({0}) AND", strSingleCondition);
                }
                if (!string.IsNullOrEmpty(strName))
                {
                    string[] arr = strPage.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    strSingleCondition = string.Empty;
                    for (int i = 0; i < arr.Length; i++)
                    {
                        strSingleCondition += string.Format(" C012 = '{0}' Or", arr[i]);
                    }
                    if (!string.IsNullOrEmpty(strSingleCondition))
                    {
                        strSingleCondition = strSingleCondition.Substring(0, strSingleCondition.Length - 3);
                    }
                    strCondition += string.Format(" ({0}) AND", strSingleCondition);
                }
                if (!string.IsNullOrEmpty(strCondition))
                {
                    strCondition = strCondition.Substring(0, strCondition.Length - 4);
                }
                strSql += strCondition;
                strSql += string.Format(" ORDER BY C009, C010, C002");
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            optReturn.Data = strSql;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            optReturn.Data = strSql;
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
                List<string> listLanguageInfo = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    LanguageInfo lang = new LanguageInfo();
                    lang.LangID = Convert.ToInt32(dr["C001"]);
                    lang.Module = Convert.ToInt32(dr["C009"]);
                    lang.SubModule = Convert.ToInt32(dr["C010"]);
                    lang.Page = dr["C011"].ToString();
                    lang.ObjName = dr["C012"].ToString();
                    lang.Name = dr["C002"].ToString();
                    string display = string.Empty;
                    display += dr["C005"].ToString();
                    display += dr["C006"].ToString();
                    display += dr["C007"].ToString();
                    display += dr["C008"].ToString();
                    lang.Display = display;
                    optReturn = XMLHelper.SeriallizeObject(lang);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listLanguageInfo.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listLanguageInfo;
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

        private OperationReturn GetUserOptList(SessionInfo session, List<string> listParams)
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
                                "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {2} AND C003='1') AND C003 = {3} ORDER BY C001, C002",
                                rentToken,
                                moduleID,
                                roleID,
                                parentID);
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
                                 "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {2} AND C003='1') AND C003 = {3} ORDER BY C001, C002",
                                 rentToken,
                                 moduleID,
                                 roleID,
                                 parentID);
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
                    strC006 = DecryptString02(strC006);
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
                        strC008 = DecryptString02(strC008);
                        if (strLicDoggle != strC008)
                        {
                            //与C008不匹配
                            strLog += string.Format("C008 not equal;");
                            continue;
                        }
                        string strDecryptC007 = DecryptStringKeyIV(strC007, strC008Hash8, strC008Hash8);
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

        private OperationReturn GetUserObjList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     用户编号
                //1     方式（0：获取指定对象类型的对象信息；1：获取权限表中管理的对象）
                //2     方式 0 时 对象类型，方式 1 时 对象类型（char 27 分割）
                //3     方式 0 时 上级机构编号( -1 表示获取当前用户所属机构信息），方式 1 时 为空
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strMethod == "0")
                        {
                            string strObjType = listParams[2];
                            string strParentID = listParams[3];
                            if (strParentID == "-1")
                            {
                                strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1}) ORDER BY C001"
                               , rentToken
                               , strUserID);
                            }
                            else
                            {
                                switch (strObjType)
                                {
                                    case "101":
                                        strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2} ORDER BY C001"
                                             , rentToken
                                             , strUserID
                                             , strParentID);
                                        break;
                                    case "102":
                                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006 = {1} AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) ORDER BY C001",
                                           rentToken,
                                           strParentID,
                                           strUserID);
                                        break;
                                    case "103":
                                        strSql =
                                            string.Format(
                                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C002 = 1 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and c001 >= 1030000000000000000 and c001 < 1040000000000000000 ORDER BY C001,C002"
                                                , rentToken
                                                , strParentID
                                                , strUserID);
                                        break;
                                    default:
                                        optReturn.Result = false;
                                        optReturn.Code = Defines.RET_PARAM_INVALID;
                                        optReturn.Message = string.Format("Object type invalid");
                                        return optReturn;
                                }
                            }
                        }
                        else if (strMethod == "1")
                        {
                            string strObjTypes = listParams[2];
                            string[] arrObjType = strObjTypes.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.RemoveEmptyEntries);
                            strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1}", rentToken, strUserID);
                            string strType = string.Empty;
                            for (int i = 0; i < arrObjType.Length; i++)
                            {
                                string strObjType = arrObjType[i];
                                strType += string.Format("C004 LIKE '{0}%' OR ", strObjType);
                            }
                            if (strType.Length > 0)
                            {
                                strType = strType.Substring(0, strType.Length - 3);
                                strType = string.Format(" AND ({0})", strType);
                            }
                            strSql = strSql + strType;
                        }
                        else
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Method invalid");
                            return optReturn;
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strMethod == "0")
                        {
                            string strObjType = listParams[2];
                            string strParentID = listParams[3];
                            if (strParentID == "-1")
                            {
                                strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1}) ORDER BY C001"
                               , rentToken
                               , strUserID);
                            }
                            else
                            {
                                switch (strObjType)
                                {
                                    case "101":
                                        strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2} ORDER BY C001"
                                             , rentToken
                                             , strUserID
                                             , strParentID);
                                        break;
                                    case "102":
                                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006 = {1} AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) ORDER BY C001",
                                           rentToken,
                                           strParentID,
                                           strUserID);
                                        break;
                                    case "103":
                                        strSql =
                                            string.Format(
                                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C002 = 1 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and c001 >= 1030000000000000000 and c001 < 1040000000000000000 ORDER BY C001,C002"
                                                , rentToken
                                                , strParentID
                                                , strUserID);
                                        break;
                                    default:
                                        optReturn.Result = false;
                                        optReturn.Code = Defines.RET_PARAM_INVALID;
                                        optReturn.Message = string.Format("Object type invalid");
                                        return optReturn;
                                }
                            }
                        }
                        else if (strMethod == "1")
                        {
                            string strObjTypes = listParams[2];
                            string[] arrObjType = strObjTypes.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.RemoveEmptyEntries);
                            strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1}", rentToken, strUserID);
                            string strType = string.Empty;
                            for (int i = 0; i < arrObjType.Length; i++)
                            {
                                string strObjType = arrObjType[i];
                                strType += string.Format("C004 LIKE '{0}%' OR ", strObjType);
                            }
                            if (strType.Length > 0)
                            {
                                strType = strType.Substring(0, strType.Length - 3);
                                strType = string.Format(" AND ({0})", strType);
                            }
                            strSql = strSql + strType;
                        }
                        else
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Method invalid");
                            return optReturn;
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
                    string strID, strName, strFullName, strInfo;
                    if (strMethod == "0")
                    {
                        string strObjType = listParams[2];
                        switch (strObjType)
                        {
                            case "101":
                                strID = dr["C001"].ToString();
                                strName = dr["C002"].ToString();
                                strName = DecryptString02(strName);
                                strInfo = string.Format("{0}{1}{2}", strID, ConstValue.SPLITER_CHAR, strName);
                                listReturn.Add(strInfo);
                                break;
                            case "102":
                                strID = dr["C001"].ToString();
                                strName = dr["C002"].ToString();
                                strFullName = dr["C003"].ToString();
                                strName = DecryptString02(strName);
                                strFullName = DecryptString02(strFullName);
                                strInfo = string.Format("{0}{1}{2}{1}{3}", strID, ConstValue.SPLITER_CHAR, strName, strFullName);
                                listReturn.Add(strInfo);
                                break;
                            case "103":
                                strID = dr["C001"].ToString();
                                strName = dr["C017"].ToString();
                                strFullName = dr["C018"].ToString();
                                strName = DecryptString02(strName);
                                strFullName = DecryptString02(strFullName);
                                strInfo = string.Format("{0}{1}{2}{1}{3}", strID, ConstValue.SPLITER_CHAR, strName, strFullName);
                                listReturn.Add(strInfo);
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("Object type invalid");
                                return optReturn;
                        }
                    }
                    else if (strMethod == "1")
                    {
                        strID = dr["C004"].ToString();
                        listReturn.Add(strID);
                    }
                    else
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Object type invalid");
                        return optReturn;
                    }
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

        private OperationReturn GetUserCtlObjList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     用户编号
                //1     方式（0：获取指定对象类型的对象信息(返回的包含对象的详细信息）；1：获取权限表中管理的对象（仅返回对象的ObjID）；2：获取指定对象类型的对象信息(忽略组织架构））
                //2     方式 0 时 对象类型，方式 1 时 对象类型（如果多个，可用char 27 分割），方式 3 时 对象类型
                //3     方式 0 时 上级机构编号( -1 表示获取当前用户所属机构信息），方式 1 时 为空，方式 2 时 为空
                //4     对象状态，可选参数，默认只能获取正常状态的资源对象（0：被删除的；1：正常的；2：禁用的）
                //      一个整型的数值，按位组合，地位到高位依次为：1：被删除的；2：禁用的....，默认值为0
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string strObjType = listParams[2];
                string strParentID = listParams[3];
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid");
                    return optReturn;
                }
                int intObjType;
                int intState = 0;
                if (listParams.Count > 4)
                {
                    string strState = listParams[4];
                    if (!int.TryParse(strState, out intState))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("StateOption invalid");
                        return optReturn;
                    }
                }
                string strLog = string.Empty;
                strLog += string.Format("{0};{1};{2};{3};{4}", strUserID, strMethod, strObjType, strParentID, intState);
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strMethod == "0")
                        {
                            if (!int.TryParse(strObjType, out intObjType))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("ObjectType invalid");
                                return optReturn;
                            }
                            if (strParentID == "-1")
                            {
                                //获取用户所在的机构信息
                                strSql =
                                    string.Format(
                                        "SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1}) ORDER BY C001"
                                        , rentToken
                                        , strUserID);
                            }
                            else
                            {
                                if (intObjType == ConstValue.RESOURCE_ORG
                                    || intObjType == ConstValue.RESOURCE_USER)
                                {
                                    //从基本表获取
                                    switch (intObjType)
                                    {
                                        case ConstValue.RESOURCE_ORG:
                                            strSql =
                                                string.Format(
                                                    "SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2} ORDER BY C001"
                                                    , rentToken
                                                    , strUserID
                                                    , strParentID);
                                            break;
                                        case ConstValue.RESOURCE_USER:
                                            strSql =
                                                string.Format(
                                                    "SELECT * FROM T_11_005_{0} WHERE C006 = {1} AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) ORDER BY C001",
                                                    rentToken,
                                                    strParentID,
                                                    strUserID);
                                            break;
                                        default:
                                            optReturn.Result = false;
                                            optReturn.Code = Defines.RET_PARAM_INVALID;
                                            optReturn.Message = string.Format("Object type invalid");
                                            return optReturn;
                                    }
                                }
                                else
                                {
                                    //从T_11_101表获取资源编码
                                    strSql =
                                        string.Format(
                                            "SELECT * FROM T_11_101_{0} WHERE C002 = 1 AND C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and c001 >= {3} and c001 < {4} ORDER BY C001,C002"
                                            , rentToken
                                            , strParentID
                                            , strUserID,
                                            intObjType * 10000000000000000,
                                            (intObjType + 1) * 10000000000000000);
                                }
                            }
                        }
                        else if (strMethod == "1")
                        {
                            //从权限表获取资源编码
                            string[] arrObjType = strObjType.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.RemoveEmptyEntries);
                            strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1}", rentToken, strUserID);
                            string strType = string.Empty;
                            for (int i = 0; i < arrObjType.Length; i++)
                            {
                                if (int.TryParse(arrObjType[i], out intObjType)
                                    && intObjType > 0)
                                {
                                    long begin = intObjType * 10000000000000000;
                                    long end = (intObjType + 1) * 10000000000000000;
                                    strType += string.Format("(C004 > {0} AND C004 < {1}) OR ", begin, end);
                                }
                            }
                            if (strType.Length <= 0)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("ObjType invalid");
                                return optReturn;
                            }
                            strType = strType.Substring(0, strType.Length - 3);
                            strType = string.Format(" AND ({0})", strType);
                            strSql = strSql + strType;
                        }
                        else if (strMethod == "2")
                        {
                            if (!int.TryParse(strObjType, out intObjType))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("ObjectType invalid");
                                return optReturn;
                            }
                            if (intObjType == ConstValue.RESOURCE_ORG
                                    || intObjType == ConstValue.RESOURCE_USER)
                            {
                                //从基本表获取
                                switch (intObjType)
                                {
                                    case ConstValue.RESOURCE_ORG:
                                        strSql =
                                            string.Format(
                                                "SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) ORDER BY C001"
                                                , rentToken
                                                , strUserID);
                                        break;
                                    case ConstValue.RESOURCE_USER:
                                        strSql =
                                            string.Format(
                                                "SELECT * FROM T_11_005_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) ORDER BY C001",
                                                rentToken,
                                                strUserID);
                                        break;
                                    default:
                                        optReturn.Result = false;
                                        optReturn.Code = Defines.RET_PARAM_INVALID;
                                        optReturn.Message = string.Format("Object type invalid");
                                        return optReturn;
                                }
                            }
                            else
                            {
                                //从T_11_101表获取资源编码
                                strSql =
                                    string.Format(
                                        "SELECT * FROM T_11_101_{0} WHERE C002 = 1 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C001 >= {2} AND C001 < {3} ORDER BY C001,C002"
                                        , rentToken
                                        , strUserID,
                                        intObjType * 10000000000000000,
                                        (intObjType + 1) * 10000000000000000);
                            }
                        }
                        else
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Method invalid");
                            return optReturn;
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strMethod == "0")
                        {
                            if (!int.TryParse(strObjType, out intObjType))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("ObjectType invalid");
                                return optReturn;
                            }
                            if (strParentID == "-1")
                            {
                                //获取用户所在的机构信息
                                strSql =
                                    string.Format(
                                        "SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1}) ORDER BY C001"
                                        , rentToken
                                        , strUserID);
                            }
                            else
                            {
                                if (intObjType == ConstValue.RESOURCE_ORG
                                    || intObjType == ConstValue.RESOURCE_USER)
                                {
                                    //从基本表获取
                                    switch (intObjType)
                                    {
                                        case ConstValue.RESOURCE_ORG:
                                            strSql =
                                                string.Format(
                                                    "SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2} ORDER BY C001"
                                                    , rentToken
                                                    , strUserID
                                                    , strParentID);
                                            break;
                                        case ConstValue.RESOURCE_USER:
                                            strSql =
                                                string.Format(
                                                    "SELECT * FROM T_11_005_{0} WHERE C006 = {1} AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) ORDER BY C001",
                                                    rentToken,
                                                    strParentID,
                                                    strUserID);
                                            break;
                                        default:
                                            optReturn.Result = false;
                                            optReturn.Code = Defines.RET_PARAM_INVALID;
                                            optReturn.Message = string.Format("Object type invalid");
                                            return optReturn;
                                    }
                                }
                                else
                                {
                                    //从T_11_101表获取(坐席，分机）
                                    strSql =
                                        string.Format(
                                            "SELECT * FROM T_11_101_{0} WHERE C002 = 1 AND C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) AND C001 >= {3} AND C001 < {4} ORDER BY C001,C002"
                                            , rentToken
                                            , strParentID
                                            , strUserID,
                                            intObjType * 10000000000000000,
                                            (intObjType + 1) * 10000000000000000);
                                }
                            }
                        }
                        else if (strMethod == "1")
                        {
                            //从权限表获取资源编码
                            string[] arrObjType = strObjType.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.RemoveEmptyEntries);
                            strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1}", rentToken, strUserID);
                            string strType = string.Empty;
                            for (int i = 0; i < arrObjType.Length; i++)
                            {
                                if (int.TryParse(arrObjType[i], out intObjType)
                                    && intObjType > 0)
                                {
                                    long begin = intObjType * 10000000000000000;
                                    long end = (intObjType + 1) * 10000000000000000;
                                    strType += string.Format("(C004 > {0} AND C004 < {1}) OR ", begin, end);
                                }
                            }
                            if (strType.Length <= 0)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("ObjType invalid");
                                return optReturn;
                            }
                            strType = strType.Substring(0, strType.Length - 3);
                            strType = string.Format(" AND ({0})", strType);
                            strSql = strSql + strType;
                        }
                        else if (strMethod == "1")
                        {
                            //从权限表获取资源编码
                            string[] arrObjType = strObjType.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.RemoveEmptyEntries);
                            strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 = {1}", rentToken, strUserID);
                            string strType = string.Empty;
                            for (int i = 0; i < arrObjType.Length; i++)
                            {
                                if (int.TryParse(arrObjType[i], out intObjType)
                                    && intObjType > 0)
                                {
                                    long begin = intObjType * 10000000000000000;
                                    long end = (intObjType + 1) * 10000000000000000;
                                    strType += string.Format("(C004 > {0} AND C004 < {1}) OR ", begin, end);
                                }
                            }
                            if (strType.Length <= 0)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("ObjType invalid");
                                return optReturn;
                            }
                            strType = strType.Substring(0, strType.Length - 3);
                            strType = string.Format(" AND ({0})", strType);
                            strSql = strSql + strType;
                        }
                        else if (strMethod == "2")
                        {
                            if (!int.TryParse(strObjType, out intObjType))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("ObjectType invalid");
                                return optReturn;
                            }
                            if (intObjType == ConstValue.RESOURCE_ORG
                                    || intObjType == ConstValue.RESOURCE_USER)
                            {
                                //从基本表获取
                                switch (intObjType)
                                {
                                    case ConstValue.RESOURCE_ORG:
                                        strSql =
                                            string.Format(
                                                "SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) ORDER BY C001"
                                                , rentToken
                                                , strUserID);
                                        break;
                                    case ConstValue.RESOURCE_USER:
                                        strSql =
                                            string.Format(
                                                "SELECT * FROM T_11_005_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) ORDER BY C001",
                                                rentToken,
                                                strUserID);
                                        break;
                                    default:
                                        optReturn.Result = false;
                                        optReturn.Code = Defines.RET_PARAM_INVALID;
                                        optReturn.Message = string.Format("Object type invalid");
                                        return optReturn;
                                }
                            }
                            else
                            {
                                //从T_11_101表获取资源编码
                                strSql =
                                    string.Format(
                                        "SELECT * FROM T_11_101_{0} WHERE C002 = 1 AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C001 >= {2} AND C001 < {3} ORDER BY C001,C002"
                                        , rentToken
                                        , strUserID,
                                        intObjType * 10000000000000000,
                                        (intObjType + 1) * 10000000000000000);
                            }
                        }
                        else
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Method invalid");
                            return optReturn;
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
                strLog += strSql;
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<ResourceObject> listObjs = new List<ResourceObject>();
                ResourceObject resourceObj;
                string strName;
                string strFullName;
                string strDeleted;
                string strDisabled;
                if (strMethod == "0" || strMethod == "2")
                {
                    if (!int.TryParse(strObjType, out intObjType))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("ObjectType invalid");
                        return optReturn;
                    }
                    if (intObjType == ConstValue.RESOURCE_ORG
                        || intObjType == ConstValue.RESOURCE_USER)
                    {
                        //机构和用户信息直接从基本表中的字段获取信息
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[i];
                            switch (intObjType)
                            {
                                case ConstValue.RESOURCE_ORG:
                                    resourceObj = new ResourceObject();
                                    resourceObj.ObjID = Convert.ToInt64(dr["C001"]);
                                    resourceObj.ObjType = ConstValue.RESOURCE_ORG;
                                    strName = dr["C002"].ToString();
                                    strName = DecryptString02(strName);
                                    resourceObj.Name = strName;
                                    resourceObj.OrgObjID = Convert.ToInt64(dr["C004"]);
                                    resourceObj.ParentObjID = Convert.ToInt64(dr["C004"]);
                                    strDeleted = dr["C006"].ToString();
                                    strDisabled = dr["C005"].ToString();
                                    resourceObj.State = ((strDeleted == "1" ? 1 : 0) | (strDisabled == "0" ? 2 : 0));
                                    listObjs.Add(resourceObj);
                                    break;
                                case ConstValue.RESOURCE_USER:
                                    resourceObj = new ResourceObject();
                                    resourceObj.ObjID = Convert.ToInt64(dr["C001"]);
                                    resourceObj.ObjType = ConstValue.RESOURCE_USER;
                                    strName = dr["C002"].ToString();
                                    strName = DecryptString02(strName);
                                    resourceObj.Name = strName;
                                    strFullName = dr["C003"].ToString();
                                    strFullName = DecryptString02(strFullName);
                                    resourceObj.FullName = strFullName;
                                    resourceObj.OrgObjID = Convert.ToInt64(dr["C006"]);
                                    resourceObj.ParentObjID = Convert.ToInt64(dr["C006"]);
                                    strDeleted = dr["C008"].ToString();
                                    strDisabled = dr["C007"].ToString();
                                    resourceObj.State = ((strDeleted == "1" ? 1 : 0) | (strDisabled == "0" ? 2 : 0));
                                    listObjs.Add(resourceObj);
                                    break;
                                default:
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_PARAM_INVALID;
                                    optReturn.Message = string.Format("Object type invalid");
                                    return optReturn;
                            }
                        }
                    }
                    else
                    {
                        //其他资源（存在与T_11_101表中的资源），先获取资源的编码，然后根据资源编码获取资源的详细信息
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            //先获取资源编码
                            DataRow dr = objDataSet.Tables[0].Rows[i];
                            resourceObj = new ResourceObject();
                            resourceObj.ObjID = Convert.ToInt64(dr["C001"]);
                            resourceObj.ObjType = intObjType;
                            listObjs.Add(resourceObj);
                        }
                        for (int i = 0; i < listObjs.Count; i++)
                        {
                            //获取每个资源的详细信息
                            resourceObj = listObjs[i];
                            switch (session.DBType)
                            {
                                case 2:
                                    strSql =
                                        string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001,C002",
                                            rentToken,
                                            resourceObj.ObjID);
                                    optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                                    if (!optReturn.Result)
                                    {
                                        return optReturn;
                                    }
                                    objDataSet = optReturn.Data as DataSet;
                                    break;
                                case 3:
                                    strSql =
                                        string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001,C002",
                                            rentToken,
                                            resourceObj.ObjID);
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
                            strLog += strSql;
                            if (objDataSet == null)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_OBJECT_NULL;
                                optReturn.Message = string.Format("DataSet is null");
                                return optReturn;
                            }
                            for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                            {
                                DataRow dr = objDataSet.Tables[0].Rows[j];
                                //需要确定信息所在的行
                                int intRow = Convert.ToInt32(dr["C002"]);
                                switch (intObjType)
                                {
                                    case ConstValue.RESOURCE_AGENT:
                                        //Row1 C011 部门编号
                                        //Row1 C012 状态
                                        //Row1 C017 工号
                                        //Row1 C018 坐席名
                                        if (intRow == 1)
                                        {
                                            strName = dr["C017"].ToString();
                                            strName = DecryptString02(strName);
                                            resourceObj.Name = strName;
                                            strFullName = dr["C018"].ToString();
                                            strFullName = DecryptString02(strFullName);
                                            resourceObj.FullName = strFullName;
                                            resourceObj.OrgObjID = Convert.ToInt64(dr["C011"]);
                                            resourceObj.ParentObjID = Convert.ToInt64(dr["C011"]);
                                            string strState = dr["C012"].ToString();
                                            resourceObj.State = ((strState == "0" ? 1 : 0) | (strState == "2" ? 2 : 0));
                                        }
                                        break;
                                    case ConstValue.RESOURCE_EXTENSION:
                                        //Row1 C011 部门编号
                                        //Row1 C012 状态
                                        //Row1 C017 分机号（含服务器地址）
                                        //Row1 C018 通道名
                                        //Row2 C015 录音服务器编号
                                        //Row2 C016 录音通道号
                                        //Row2 C017 录音通道编码
                                        //Row2 C018 录屏服务器编号
                                        //Row2 C019 录屏通道号
                                        //Row2 C020 录屏通道编号
                                        if (intRow == 1)
                                        {
                                            string strNameIP = dr["C017"].ToString();
                                            strNameIP = DecryptString02(strNameIP);
                                            strName = strNameIP;
                                            string strIP1 = string.Empty;
                                            string strIP2 = string.Empty;
                                            string strRole = string.Empty;
                                            string[] arrInfos = strName.Split(new[] { ConstValue.SPLITER_CHAR },
                                                StringSplitOptions.None);
                                            if (arrInfos.Length > 0)
                                            {
                                                strName = arrInfos[0];
                                            }
                                            if (arrInfos.Length > 1)
                                            {
                                                strIP1 = arrInfos[1];
                                            }
                                            if (arrInfos.Length > 2)
                                            {
                                                strRole = arrInfos[2];
                                            }
                                            if (arrInfos.Length > 3)
                                            {
                                                strIP2 = arrInfos[3];
                                            }
                                            resourceObj.Name = strName;
                                            int intRole;
                                            if (int.TryParse(strRole, out intRole))
                                            {
                                                if (intRole == 1)
                                                {
                                                    resourceObj.Other05 = "1";
                                                    resourceObj.Other04 = strIP1;
                                                }
                                                if (intRole == 2)
                                                {
                                                    resourceObj.Other05 = "2";
                                                    resourceObj.Other04 = strIP1;
                                                }
                                                if (intRole == 3)
                                                {
                                                    resourceObj.Other05 = "3";
                                                    resourceObj.Other04 = string.Format("{0};{1}", strIP1, strIP2);
                                                }
                                            }
                                            strFullName = dr["C018"].ToString();
                                            strFullName = DecryptString02(strFullName);
                                            resourceObj.FullName = strFullName;
                                            resourceObj.OrgObjID = Convert.ToInt64(dr["C011"]);
                                            resourceObj.ParentObjID = Convert.ToInt64(dr["C011"]);
                                            string strState = dr["C012"].ToString();
                                            resourceObj.State = ((strState == "0" ? 1 : 0) | (strState == "2" ? 2 : 0));
                                        }
                                        if (intRow == 2)
                                        {
                                            string strVoiceObjID = dr["C015"].ToString();
                                            string strVoiceChanID = dr["C016"].ToString();
                                            string strVoiceChanObjID = dr["C017"].ToString();
                                            string strScreenObjID = dr["C018"].ToString();
                                            string strScreenChanID = dr["C019"].ToString();
                                            string strScreenChanObjID = dr["C020"].ToString();

                                            string strRole = resourceObj.Other05;
                                            if (strRole == "3")
                                            {
                                                //录音录屏分机
                                                resourceObj.Other01 = string.Format("{0}{1}{2}", strVoiceObjID,
                                                    ConstValue.SPLITER_CHAR_3, strScreenObjID);
                                                resourceObj.Other02 = string.Format("{0}{1}{2}", strVoiceChanID,
                                                    ConstValue.SPLITER_CHAR_3, strScreenChanID);
                                                resourceObj.Other03 = string.Format("{0}{1}{2}", strVoiceChanObjID,
                                                    ConstValue.SPLITER_CHAR_3, strScreenChanObjID);
                                            }
                                            else if (strRole == "1")
                                            {
                                                //录音分机
                                                resourceObj.Other01 = strVoiceObjID;
                                                resourceObj.Other02 = strVoiceChanID;
                                                resourceObj.Other03 = strVoiceChanObjID;
                                            }
                                            else if (strRole == "2")
                                            {
                                                //录屏分机
                                                resourceObj.Other01 = strScreenObjID;
                                                resourceObj.Other02 = strScreenChanID;
                                                resourceObj.Other03 = strScreenChanObjID;
                                            }
                                        }
                                        break;
                                    case ConstValue.RESOURCE_REALEXT:
                                        //Row1 C011 部门编号
                                        //Row1 C012 状态
                                        //Row1 C017 分机号
                                        //Row1 C018 通道名
                                        if (intRow == 1)
                                        {
                                            strName = dr["C017"].ToString();
                                            strName = DecryptString02(strName);
                                            resourceObj.Name = strName;
                                            strFullName = dr["C018"].ToString();
                                            strFullName = DecryptString02(strFullName);
                                            resourceObj.FullName = strFullName;
                                            resourceObj.OrgObjID = Convert.ToInt64(dr["C011"]);
                                            resourceObj.ParentObjID = Convert.ToInt64(dr["C011"]);
                                            string strState = dr["C012"].ToString();
                                            resourceObj.State = ((strState == "0" ? 1 : 0) | (strState == "2" ? 2 : 0));
                                        }
                                        break;
                                    default:
                                        optReturn.Result = false;
                                        optReturn.Code = Defines.RET_PARAM_INVALID;
                                        optReturn.Message = string.Format("Object type invalid");
                                        return optReturn;
                                }
                            }
                        }
                    }
                }
                else if (strMethod == "1")
                {
                    //从权限表获取资源编码
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        resourceObj = new ResourceObject();
                        resourceObj.ObjID = Convert.ToInt64(dr["C004"]);
                        listObjs.Add(resourceObj);
                    }
                }
                else
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid");
                    return optReturn;
                }
                //将资源信息序列化后返回结果
                List<string> listReturn = new List<string>();
                for (int i = 0; i < listObjs.Count; i++)
                {
                    var obj = listObjs[i];
                    //判断资源状态
                    if (obj.State > 0)
                    {
                        if ((obj.State & intState) <= 0)
                        {
                            //如果资源的状态不是正常的状态，并且没有指示要获取此状态的资源，则不获取此资源
                            continue;
                        }
                    }
                    optReturn = XMLHelper.SeriallizeObject(obj);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Message = strLog;
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

        private OperationReturn GetBasicDataInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     类型，     0：通过InfoID获取列表；
                //                 1：通过上级InfoID获取列表；
                //                 2：通过上级InfoID范围和上级InfoValue获取列表
                //                 3：通过InfoID范围(前4位，即模块号）获取整个模块下的列表
                //1     InfoID(或范围)
                //2     InfoValue
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strType = listParams[0];
                string strInfoID = listParams[1];
                string strInfoValue = listParams[2];
                int infoID;
                if (!int.TryParse(strInfoID, out infoID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("InfoID invalid");
                    return optReturn;
                }
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strType == "1")
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C003 = {0} ORDER BY C001, C002", infoID);
                        }
                        else if (strType == "2")
                        {
                            //计算范围基数
                            infoID = infoID / 1000 * 1000;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C003 IN (SELECT C001 FROM T_00_003 WHERE C001 > {0} AND C001 < {1} AND C006 = '{2}') ORDER BY C001,C002",
                                    infoID, infoID + 1000, strInfoValue);
                        }
                        else if (strType == "3")
                        {
                            //计算范围基数
                            infoID = infoID / 100000 * 100000;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 > {0} AND C001 < {1} ORDER BY C001,C002",
                                    infoID, infoID + 100000);
                        }
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 = {0} ORDER BY C001, C002", strInfoID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strType == "1")
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C003 = {0} ORDER BY C001, C002", infoID);
                        }
                        else if (strType == "2")
                        {
                            infoID = infoID / 1000 * 1000;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C003 IN (SELECT C001 FROM T_00_003 WHERE C001 > {0} AND C001 < {1} AND C006 = '{2}') ORDER BY C001,C002",
                                    infoID, infoID + 1000, strInfoValue);
                        }
                        else if (strType == "3")
                        {
                            //计算范围基数
                            infoID = infoID / 100000 * 100000;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 > {0} AND C001 < {1} ORDER BY C001,C002",
                                    infoID, infoID + 100000);
                        }
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 = {0} ORDER BY C001, C002", strInfoID);
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
                    BasicDataInfo item = new BasicDataInfo();
                    item.InfoID = Convert.ToInt32(dr["C001"]);
                    item.SortID = Convert.ToInt32(dr["C002"]);
                    item.ParentID = Convert.ToInt32(dr["C003"]);
                    item.IsEnable = dr["C004"].ToString() == "1";
                    item.EncryptVersion = Convert.ToInt32(dr["C005"]);
                    item.Value = dr["C006"].ToString();
                    item.Icon = dr["C007"].ToString();
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

        private OperationReturn GetSerialID(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     模块编码
                //1     模块内编码
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

        private OperationReturn GetUserViewColumnList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         user id
                //1         view id
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string userID = listParams[0];
                string viewID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT A.C001, A.C002, A.C003, B.C016, B.C011, B.C004 ");
                        strSql += string.Format("FROM T_00_102 A, T_11_203_{0} B ", rentToken);
                        strSql += string.Format("WHERE A.C001 = B.C002 AND A.C002 = B.C003 COLLATE DATABASE_DEFAULT ");
                        strSql += string.Format("AND B.C001 = {0} AND A.C001 = {1} ", userID, viewID);
                        strSql += string.Format("ORDER BY B.C004 ASC");
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT A.C001, A.C002, A.C003, B.C016, B.C011, B.C004 ");
                        strSql += string.Format("FROM T_00_102 A, T_11_203_{0} B ", rentToken);
                        strSql += string.Format("WHERE A.C001 = B.C002 AND A.C002 = B.C003 ");
                        strSql += string.Format("AND B.C001 = {0} AND A.C001 = {1} ", userID, viewID);
                        strSql += string.Format("ORDER BY B.C004 ASC");
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
                List<string> listColumns = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    ViewColumnInfo column = new ViewColumnInfo();
                    column.ViewID = Convert.ToInt64(dr["C001"]);
                    string strName = dr["C002"].ToString();
                    strName = DecryptString02(strName);
                    column.ColumnName = strName;
                    column.DataType = Convert.ToInt32(dr["C003"]);
                    column.Width = Convert.ToInt32(dr["C016"]);
                    column.Visibility = dr["C011"].ToString();
                    column.SortID = Convert.ToInt32(dr["C004"]);
                    optReturn = XMLHelper.SeriallizeObject(column);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listColumns.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listColumns;
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

        private OperationReturn InsertTempData(SessionInfo session, List<string> listParams)
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
                //ListParams
                //0         tempID
                //1         count
                //2..       tempData(tempData property split by char 27, less than 5)
                string strTempID = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount) || intCount <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource count invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource count invalid");
                    return optReturn;
                }
                if (string.IsNullOrEmpty(strTempID))
                {
                    List<string> listGetSerialIDParams = new List<string>();
                    listGetSerialIDParams.Add("11");
                    listGetSerialIDParams.Add("911");
                    listGetSerialIDParams.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    OperationReturn getSerialIDReturn = GetSerialID(session, listGetSerialIDParams);
                    if (!getSerialIDReturn.Result)
                    {
                        return getSerialIDReturn;
                    }
                    strTempID = getSerialIDReturn.Data.ToString();
                }
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("select * from t_00_901 where c001 = {0}", strTempID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("select * from t_00_901 where c001 = {0}", strTempID);
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

                    int number = 0;
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        number = Math.Max(number, Convert.ToInt32(objDataSet.Tables[0].Rows[i]["C002"]));
                    }
                    for (int i = 2; i < listParams.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].NewRow();
                        string strTempData = listParams[i];
                        string[] arrTempData = strTempData.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.RemoveEmptyEntries);
                        dr["C001"] = strTempID;
                        dr["C002"] = number + i - 1;
                        if (arrTempData.Length > 0)
                        {
                            dr["C011"] = arrTempData[0];
                        }
                        if (arrTempData.Length > 1)
                        {
                            dr["C012"] = arrTempData[1];
                        }
                        if (arrTempData.Length > 2)
                        {
                            dr["C013"] = arrTempData[2];
                        }
                        if (arrTempData.Length > 3)
                        {
                            dr["C014"] = arrTempData[3];
                        }
                        if (arrTempData.Length > 4)
                        {
                            dr["C015"] = arrTempData[4];
                        }
                        objDataSet.Tables[0].Rows.Add(dr);
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

                optReturn.Data = strTempID;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn WriteOperationLog(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                #region 解析参数

                //ListParams
                //0     模块编号（4位的小模块号）
                //1     操作编号
                //2     机器名（可以传入空，系统会自动解析出客户端机器名）
                //3     机器IP（可以传入空，系统会自动解析出客户端IP）
                //4     操作发生的时间（UTC时间，可以传入空，系统会自动使用当前时间作为操作时间）
                //5     操作结果（操作结果为 R0，R1...）
                //6     操作日志语言ID
                //7     操作日志参数
                if (listParams == null || listParams.Count < 8)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strModuleID = listParams[0];
                string strOptID = listParams[1];
                string strMachineName = listParams[2];
                string strMachineIP = listParams[3];
                string strLogTime = listParams[4];
                string strLogResult = listParams[5];
                string strLangID = listParams[6];
                string strLogArgs = listParams[7];

                string strUserID = session.UserID.ToString();
                string strRoleID = session.RoleID.ToString();
                string strRentToken = session.RentInfo.Token;
                string strLoginID = "0";
                DateTime dtLogTime = DateTime.Now.ToUniversalTime();
                string strLogID;

                #endregion


                #region 检查参数有效性

                if (string.IsNullOrEmpty(strModuleID)
                    || string.IsNullOrEmpty(strOptID)
                    || string.IsNullOrEmpty(strLogResult)
                    || string.IsNullOrEmpty(strLoginID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Some parameter is empty");
                    return optReturn;
                }
                if (string.IsNullOrEmpty(strMachineName)
                    || string.IsNullOrEmpty(strMachineIP))
                {
                    HttpContext context = HttpContext.Current;
                    if (context != null)
                    {
                        var httpRequest = context.Request;
                        if (string.IsNullOrEmpty(strMachineName))
                        {
                            strMachineName = httpRequest.UserHostName;
                        }
                        if (string.IsNullOrEmpty(strMachineIP))
                        {
                            strMachineIP = httpRequest.UserHostAddress;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(strLogTime))
                {
                    if (DateTime.TryParse(strLogTime, out dtLogTime))
                    {

                    }
                }


                #endregion


                #region 获取操作日志流水号

                List<string> listTemp = new List<string>();
                listTemp.Add("11");
                listTemp.Add(ConstValue.RESOURCE_OPERATIONLOG_UMP.ToString());
                listTemp.Add(DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
                optReturn = GetSerialID(session, listTemp);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                strLogID = optReturn.Data.ToString();

                #endregion


                #region 插入操作日志记录

                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_901 WHERE C001 = {0}", strLogID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_901 WHERE C001 = {0}", strLogID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType not support.\t{0}", session.DBType);
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

                    if (objDataSet.Tables[0].Rows.Count > 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_ALREADY_EXIST;
                        optReturn.Message = string.Format("OperationLog already exist.\t{0}", strLogID);
                        return optReturn;
                    }
                    DataRow dr = objDataSet.Tables[0].NewRow();
                    dr["C001"] = strLogID;
                    dr["C002"] = strLoginID;
                    dr["C003"] = strModuleID;
                    dr["C004"] = strOptID;
                    dr["C005"] = strUserID;
                    dr["C006"] = strMachineName;
                    dr["C007"] = strMachineIP;
                    dr["C008"] = Convert.ToInt64(dtLogTime.ToString("yyyyMMddHHmmss"));
                    dr["C009"] = strLogResult;
                    dr["C010"] = strLangID;

                    string strTemp;
                    for (int i = 1; i <= 5; i++)
                    {
                        if (strLogArgs.Length > 1024)
                        {
                            strTemp = strLogArgs.Substring(0, 1024);
                            strLogArgs = strLogArgs.Substring(1024 + 1);
                            dr[string.Format("C{0}", (10 + i).ToString("000"))] = strTemp;
                        }
                        else
                        {
                            strTemp = strLogArgs;
                            dr[string.Format("C{0}", (10 + i).ToString("000"))] = strTemp;
                            break;
                        }
                    }

                    dr["C021"] = strRoleID;
                    dr["C022"] = strRentToken;

                    objDataSet.Tables[0].Rows.Add(dr);

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();

                    optReturn.Data = strLogID;
                    optReturn.Message = strLogID;
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
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetLayoutInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     布局编码（4位工程编号+2位序号）
                //          序号：01       查询界面总布局
                //1     使用者编码（0表示默认布局，可能使用下划线）
                //2     Method（0：可以使用默认布局；1：不可以使用默认布局；2：优先使用默认布局
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string layoutID = listParams[0];
                string userID = listParams[1];
                string method = listParams[2];
                //string dir = session.InstallPath;
                //dir = Path.Combine(dir, "Layouts");
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                dir = dir.Substring(0, dir.LastIndexOf("\\"));
                dir = dir.Substring(0, dir.LastIndexOf("\\"));
                dir = Path.Combine(dir, "Layouts");
                if (!Directory.Exists(dir))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("Layouts directory not exist.\t{0}", dir);
                    return optReturn;
                }
                string strFile;
                if (method == "2")
                {
                    strFile = Path.Combine(dir, string.Format("{0}_0.xml", layoutID));
                }
                else
                {
                    strFile = Path.Combine(dir, string.Format("{0}_{1}.xml", layoutID, userID));
                    if (!File.Exists(strFile) && method == "1")
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                        optReturn.Message = string.Format("Layout file not exist.");
                        return optReturn;
                    }
                    if (!File.Exists(strFile))
                    {
                        strFile = Path.Combine(dir, string.Format("{0}_0.xml", layoutID));
                    }
                }
                if (!File.Exists(strFile))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("Layout file not exist.");
                    return optReturn;
                }
                string strContent = File.ReadAllText(strFile, Encoding.UTF8);
                optReturn.Data = strContent;
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

        private OperationReturn SaveLayoutInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     布局编码（4位工程编号+2位序号）
                //          序号：01       查询界面总布局
                //1     使用者编码（0表示默认布局，可能使用下划线
                //2     布局信息
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string layoutID = listParams[0];
                string userID = listParams[1];
                string layoutInfo = listParams[2];
                //string dir = session.InstallPath;
                //dir = Path.Combine(dir, "Layouts");
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                dir = dir.Substring(0, dir.LastIndexOf("\\"));
                dir = dir.Substring(0, dir.LastIndexOf("\\"));
                dir = Path.Combine(dir, "Layouts");
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string strFile;
                strFile = Path.Combine(dir, string.Format("{0}_{1}.xml", layoutID, userID));
                File.WriteAllText(strFile, layoutInfo, Encoding.UTF8);
                optReturn.Data = strFile;
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
                //0         资源类型( <100 表示从11_101表获取属性，值代表行编号）
                //1         属性名称
                //2         数量
                //3..       资源编码  
                string strResourceType = listParams[0];
                string strPropertyName = listParams[1];
                string strCount = listParams[2];
                int intResourceType;
                if (!int.TryParse(strResourceType, out intResourceType))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource type invalid");
                    return optReturn;
                }
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
                    List<string> insertParams = new List<string>();
                    insertParams.Add(string.Empty);
                    insertParams.Add(intCount.ToString());
                    for (int i = 0; i < intCount; i++)
                    {
                        insertParams.Add(listParams[i + 3]);
                    }
                    optReturn = InsertTempData(session, insertParams);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    strTempID = optReturn.Data.ToString();
                }
                string strSql;
                DataSet objDataSet = null;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        if (intResourceType < 100)
                        {
                            if (intCount == 1)
                            {
                                strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} AND C002 = {2}",
                                  rentToken,
                                  strResourceID,
                                  intResourceType);
                            }
                            else
                            {
                                strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 INT (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C002 = {2}",
                                 rentToken,
                                 strTempID,
                                 intResourceType);
                            }
                        }
                        else
                        {
                            switch (intResourceType)
                            {
                                case 101:
                                    if (intCount == 1)
                                    {
                                        strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 = {1}", rentToken, strResourceID);
                                    }
                                    else
                                    {
                                        strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 in (select c011 from t_00_901 where c001 = {1})", rentToken, strTempID);
                                    }
                                    break;
                                case 102:
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
                        if (intResourceType < 100)
                        {
                            if (intCount == 1)
                            {
                                strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} AND C002 = {2}",
                                  rentToken,
                                  strResourceID,
                                  intResourceType);
                            }
                            else
                            {
                                strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 INT (SELECT C011 FROM T_00_901 WHERE C001 = {1}) AND C002 = {2}",
                                 rentToken,
                                 strTempID,
                                 intResourceType);
                            }
                        }
                        else
                        {
                            switch (intResourceType)
                            {
                                case 101:
                                    if (intCount == 1)
                                    {
                                        strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 = {1}", rentToken, strResourceID);
                                    }
                                    else
                                    {
                                        strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 in (select c011 from t_00_901 where c001 = {1})", rentToken, strTempID);
                                    }
                                    break;
                                case 102:
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
                    if (intResourceType < 100)
                    {
                        switch (strPropertyName)
                        {
                            case "102HeadIcon":
                                objID = dr["C001"].ToString();
                                propertyValue = dr["C014"].ToString();
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("PropertyName invalid");
                                return optReturn;
                        }
                    }
                    else
                    {
                        switch (strResourceType + strPropertyName)
                        {
                            case "102Account":
                                objID = dr["C001"].ToString();
                                propertyValue = dr["C002"].ToString();
                                propertyValue = DecryptString02(propertyValue);
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("PropertyName invalid");
                                return optReturn;
                        }
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
                    strName = DecryptString02(strName);
                    strFullName = DecryptString02(strFullName);
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

        private OperationReturn GetGlobalParamList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     模块编号
                //1     参数编号
                //2     参数所在的组
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strModuleID = listParams[0];
                string strParamID = listParams[1];
                string strGroupID = listParams[2];

                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strParamID == "0")
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_001_{0} WHERE C002 = {1} AND  C004={2}"
                                    , rentToken
                                    , strModuleID,
                                    strGroupID);
                        }
                        else
                        {
                            strSql =
                                 string.Format(
                                     "SELECT * FROM T_11_001_{0} WHERE C002 = {1} AND  C003={2}"
                                       , rentToken
                                       , strModuleID,
                                       strParamID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strParamID == "0")
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_001_{0} WHERE C002 = {1} AND  C004={2}"
                                    , rentToken
                                    , strModuleID,
                                    strGroupID);
                        }
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_001_{0} WHERE C002 = {1} AND  C003={2}"
                                      , rentToken
                                      , strModuleID,
                                      strParamID);
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
                    GlobalParamInfo GlobalParam = new GlobalParamInfo();
                    GlobalParam.RentID = Convert.ToInt64(dr["C001"].ToString());
                    GlobalParam.ModuleID = Convert.ToInt32(dr["C002"].ToString());
                    GlobalParam.ParamID = Convert.ToInt32(dr["C003"].ToString());
                    GlobalParam.GroupID = Convert.ToInt32(dr["C004"].ToString());
                    GlobalParam.SortID = Convert.ToInt32(dr["C005"].ToString());
                    GlobalParam.ParamValue = DecryptString02(dr["C006"].ToString());

                    optReturn = XMLHelper.SeriallizeObject(GlobalParam);
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

        private OperationReturn GetGlobalParamList2(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                //1     方式（0：获取指定参数编码的参数，1：获取指定参数组下的所有参数，2：获取指定模块下的所有参数）
                //2     参数编码或参数组编码或模块号，其中参数编码和组编号可以用char27隔开
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty, strConditon;
                switch (strMethod)
                {
                    case "0":
                        string strArrParamID = listParams[2];
                        string[] arrParamID = strArrParamID.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.RemoveEmptyEntries);
                        strConditon = string.Empty;
                        for (int i = 0; i < arrParamID.Length; i++)
                        {
                            strConditon += string.Format("C003 = {0} OR ", arrParamID[i]);
                        }
                        if (!string.IsNullOrEmpty(strConditon))
                        {
                            strConditon = strConditon.Substring(0, strConditon.Length - 4);
                            strSql = string.Format("SELECT * FROM T_11_001_{0} WHERE ({1})", rentToken, strConditon);
                        }
                        break;
                    case "1":
                        string strArrGroupID = listParams[2];
                        string[] arrGroupID = strArrGroupID.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.RemoveEmptyEntries);
                        strConditon = string.Empty;
                        for (int i = 0; i < arrGroupID.Length; i++)
                        {
                            strConditon += string.Format("C004 = {0} OR ", arrGroupID[i]);
                        }
                        if (!string.IsNullOrEmpty(strConditon))
                        {
                            strConditon = strConditon.Substring(0, strConditon.Length - 4);
                            strSql = string.Format("SELECT * FROM T_11_001_{0} WHERE ({1})", rentToken, strConditon);
                        }
                        break;
                    case "2":
                        string strModuleID = listParams[2];
                        int intModuleID;
                        if (!int.TryParse(strModuleID, out intModuleID))
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("ModuleID invalid");
                            return optReturn;
                        }
                        strConditon = string.Format("C003 >= {0} and C003 < {1}",
                            intModuleID * 10000,
                            (intModuleID + 1) * 10000);
                        strSql = string.Format("SELECT * FROM T_11_001_{0} WHERE ({1})", rentToken, strConditon);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Method invalid");
                        return optReturn;
                }
                if (string.IsNullOrEmpty(strSql))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ParamID or GroupID invalid");
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

                    GlobalParamInfo item = new GlobalParamInfo();
                    item.ParamID = Convert.ToInt32(dr["C003"]);
                    item.GroupID = Convert.ToInt32(dr["C004"]);
                    item.SortID = Convert.ToInt32(dr["C005"]);
                    string strValue = dr["C006"].ToString();
                    strValue = DecryptString02(strValue);
                    strValue = strValue.Replace(ConstValue.SPLITER_CHAR, ConstValue.SPLITER_CHAR_3);
                    item.ParamValue = strValue;

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

        private OperationReturn GetUserParamList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                //1     方式（0：获取指定参数编码的参数，1：获取指定参数组下的所有参数，2：获取指定模块下的所有参数）
                //2     参数编码或参数组编码或模块号，其中参数编码和组编号可以用char27隔开
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty, strConditon;
                switch (strMethod)
                {
                    case "0":
                        string strArrParamID = listParams[2];
                        string[] arrParamID = strArrParamID.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.RemoveEmptyEntries);
                        strConditon = string.Empty;
                        for (int i = 0; i < arrParamID.Length; i++)
                        {
                            strConditon += string.Format("C002 = {0} OR ", arrParamID[i]);
                        }
                        if (!string.IsNullOrEmpty(strConditon))
                        {
                            strConditon = strConditon.Substring(0, strConditon.Length - 4);
                            strSql = string.Format("SELECT * FROM T_11_011_{0} WHERE C001 = {1} AND ({2})", rentToken, strUserID, strConditon);
                        }
                        break;
                    case "1":
                        string strArrGroupID = listParams[2];
                        string[] arrGroupID = strArrGroupID.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.RemoveEmptyEntries);
                        strConditon = string.Empty;
                        for (int i = 0; i < arrGroupID.Length; i++)
                        {
                            strConditon += string.Format("C003 = {0} OR ", arrGroupID[i]);
                        }
                        if (!string.IsNullOrEmpty(strConditon))
                        {
                            strConditon = strConditon.Substring(0, strConditon.Length - 4);
                            strSql = string.Format("SELECT * FROM T_11_011_{0} WHERE C001 = {1} AND ({2})", rentToken, strUserID, strConditon);
                        }
                        break;
                    case "2":
                        string strModuleID = listParams[2];
                        int intModuleID;
                        if (!int.TryParse(strModuleID, out intModuleID))
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("ModuleID invalid");
                            return optReturn;
                        }
                        strConditon = string.Format("C002 >= {0} and C002 < {1}",
                            intModuleID * 10000,
                            (intModuleID + 1) * 10000);
                        strSql = string.Format("SELECT * FROM T_11_011_{0} WHERE C001 = {1} AND ({2})", rentToken, strUserID, strConditon);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Method invalid");
                        return optReturn;
                }
                if (string.IsNullOrEmpty(strSql))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ParamID or GroupID invalid");
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

                    UserParamInfo item = new UserParamInfo();
                    item.UserID = Convert.ToInt64(dr["C001"]);
                    item.ParamID = Convert.ToInt32(dr["C002"]);
                    item.GroupID = Convert.ToInt32(dr["C003"]);
                    item.SortID = Convert.ToInt32(dr["C004"]);
                    item.ParamValue = dr["C005"].ToString();
                    item.DataType = (DBDataType)Convert.ToInt32(dr["C006"]);
                    item.ParamValue = DecryptString02(item.ParamValue);

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

        private OperationReturn SaveUserParamList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                //1     参数总数
                //2...  参数信息
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                List<UserParamInfo> listInfos = new List<UserParamInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 2];
                    optReturn = XMLHelper.DeserializeObject<UserParamInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    UserParamInfo info = optReturn.Data as UserParamInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("UserParamInfo is null");
                        return optReturn;
                    }
                    listInfos.Add(info);
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("Select * from t_11_011_{0} where C001 = {1}", rentToken, strUserID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("Select * from t_11_011_{0} where C001 = {1}", rentToken, strUserID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Method invalid");
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
                    for (int i = 0; i < listInfos.Count; i++)
                    {
                        bool isAdd = false;
                        var info = listInfos[i];
                        int paramID = info.ParamID;
                        DataRow dr =
                            objDataSet.Tables[0].Select(string.Format("C002 = {0}", paramID)).FirstOrDefault();
                        if (dr == null)
                        {
                            isAdd = true;
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = Convert.ToInt64(strUserID);
                            dr["C002"] = paramID;
                        }
                        dr["C003"] = info.GroupID;
                        dr["C004"] = info.SortID;
                        string paramValue = info.ParamValue;
                        paramValue = DecryptString02(paramValue);
                        dr["C005"] = paramValue;
                        dr["C006"] = (int)info.DataType;
                        dr["C008"] = 0;
                        dr["C009"] = 0;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            string strMsg = string.Format("A{0}{1}", ConstValue.SPLITER_CHAR, paramID);
                            listMsg.Add(strMsg);
                        }
                        else
                        {
                            string strMsg = string.Format("M{0}{1}", ConstValue.SPLITER_CHAR, paramID);
                            listMsg.Add(strMsg);
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
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetDBInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0：UserID，保留，可以指定为0
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP.Server\\Args01.UMP.xml");
                if (!File.Exists(path))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("UMP.Server\\Args01.UMP.xml file not exist.\t{0}", path);
                    return optReturn;
                }
                DatabaseInfo dbInfo = new DatabaseInfo();
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode node = doc.SelectSingleNode("DatabaseParameters");
                if (node == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_CONFIG_INVALID;
                    optReturn.Message = string.Format("DatabaseParameters node not exist");
                    return optReturn;
                }
                string strValue;
                int intValue;
                bool isSetted = false;
                XmlNodeList listNodes = node.ChildNodes;
                for (int i = 0; i < listNodes.Count; i++)
                {
                    XmlNode temp = listNodes[i];
                    if (temp.Attributes != null)
                    {
                        var isEnableAttr = temp.Attributes["P03"];
                        if (isEnableAttr != null)
                        {
                            strValue = isEnableAttr.Value;
                            strValue = DecryptString04(strValue);
                            if (strValue != "1") { continue; }
                        }
                        var attr = temp.Attributes["P02"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString04(strValue);
                            if (int.TryParse(strValue, out intValue))
                            {
                                dbInfo.TypeID = intValue;
                            }
                        }
                        attr = temp.Attributes["P04"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString04(strValue);
                            dbInfo.Host = strValue;
                        }
                        attr = temp.Attributes["P05"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString04(strValue);
                            if (int.TryParse(strValue, out intValue))
                            {
                                dbInfo.Port = intValue;
                            }
                        }
                        attr = temp.Attributes["P06"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString04(strValue);
                            dbInfo.DBName = strValue;
                        }
                        attr = temp.Attributes["P07"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString04(strValue);
                            dbInfo.LoginName = strValue;
                        }
                        attr = temp.Attributes["P08"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString04(strValue);
                            dbInfo.Password = strValue;
                        }
                        isSetted = true;
                        break;
                    }
                }
                if (!isSetted)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_CONFIG_INVALID;
                    optReturn.Message = string.Format("DB not setted");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                listReturn.Add(dbInfo.TypeID.ToString());
                listReturn.Add(dbInfo.Host);
                listReturn.Add(dbInfo.Port.ToString());
                listReturn.Add(dbInfo.DBName);
                listReturn.Add(dbInfo.LoginName);
                listReturn.Add(EncryptString04(dbInfo.Password));
                optReturn.Data = listReturn;
                optReturn.Message = dbInfo.ToString();
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

        private string EncryptString04(string strSource)
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

        private string EncryptString03(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V03Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString03(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V03Hex);
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

        private string DecryptStringKeyIV(string strSource, string key, string iv)
        {
            try
            {
                string strReturn = string.Empty;
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                int length = strSource.Length / 2;
                byte[] byteData = new byte[length];
                for (int i = 0; i < length; i++) { byteData[i] = (byte)Convert.ToInt32(strSource.Substring(i * 2, 2), 16); }
                des.Key = UnicodeEncoding.ASCII.GetBytes(key);
                des.IV = UnicodeEncoding.ASCII.GetBytes(iv);

                MemoryStream ms = new MemoryStream();
                CryptoStream stream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                stream.Write(byteData, 0, byteData.Length);
                stream.FlushFinalBlock();
                strReturn = Encoding.Unicode.GetString(ms.ToArray());
                return strReturn;
            }
            catch (Exception ex)
            {
                return strSource;
            }
        }

        #endregion


        #region Others

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
                doggleNumber = DecryptString01(doggleNumber);
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

    }
}
