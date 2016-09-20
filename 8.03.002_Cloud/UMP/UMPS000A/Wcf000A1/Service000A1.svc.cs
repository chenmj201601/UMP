using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.ServiceModel.Activation;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common000A1;
using LanguageInfo = VoiceCyber.UMP.Common000A1.LanguageInfo;
using OperationInfo = VoiceCyber.UMP.Common000A1.OperationInfo;
using ResourceObject = VoiceCyber.UMP.Common000A1.ResourceObject;

namespace Wcf000A1
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service000A1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service000A1.svc 或 Service000A1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class Service000A1 : IService000A1, IDisposable
    {

        #region Members

        private LogOperator mLogOperator;
        private Service03Helper mService03Helper;

        #endregion


        public Service000A1()
        {
            CreateLogOperator();
        }

        public SDKReturn DoOperation(SDKRequest request)
        {
            SDKReturn sdkReturn = new SDKReturn();
            if (request == null)
            {
                sdkReturn.Result = false;
                sdkReturn.Code = Defines.RET_PARAM_INVALID;
                sdkReturn.Message = string.Format("SDKRequest is null");
                return sdkReturn;
            }
            try
            {
                OperationReturn optReturn;
                switch (request.Code)
                {
                    case (int)S000ACodes.GetLangList:
                        optReturn = GetLangList(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S000ACodes.GetUserOptList:
                        optReturn = GetUserOptList(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S000ACodes.GetUserCtlObjList:
                        optReturn = GetUserCtlObjList(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S000ACodes.GetSerialID:
                        optReturn = GetSerialID(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S000ACodes.GetLogRecordData:
                        optReturn = GetLogRecordData(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S000ACodes.GetLogRecordUrl:
                        optReturn = GetLogRecordUrl(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S000ACodes.UpdateLogRecordInfo:
                        optReturn = UpdateLogRecordInfo(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S000ACodes.InsertLogRecord:
                        optReturn = InsertLogRecord(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S000ACodes.GetDBInfo:
                        optReturn = GetDBInfo(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S000ACodes.GetDBData:
                        optReturn = GetDBData(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.DataSetData = optReturn.Data as DataSet;
                        break;
                    case (int)S000ACodes.ExeDBCommand:
                        optReturn = ExtDBCommand(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S000ACodes.LogonUMP:
                        optReturn = LogOnUMP(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S000ACodes.LogoutUMP:
                        optReturn = LogOutUMP(request.ListData);
                        if (!optReturn.Result)
                        {
                            sdkReturn.Result = false;
                            sdkReturn.Code = optReturn.Code;
                            sdkReturn.Message = optReturn.Message;
                            return sdkReturn;
                        }
                        sdkReturn.ListData = optReturn.Data as List<string>;
                        break;
                    default:
                        sdkReturn.Result = false;
                        sdkReturn.Code = Defines.RET_NOT_IMPLIMENT;
                        sdkReturn.Message = string.Format("Method not impliment.\t{0}", request.Code);
                        return sdkReturn;
                }
                sdkReturn.Message = optReturn.Message;
            }
            catch (Exception ex)
            {
                sdkReturn.Result = false;
                sdkReturn.Code = Defines.RET_FAIL;
                sdkReturn.Message = ex.Message;
            }
            return sdkReturn;
        }


        #region Basic Operations

        private OperationReturn GetLangList(List<string> listParams)
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
                //5     Name
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
                WriteOperationLog(
                    string.Format("GetLangList:\tLangID:{0};PreName:{1};ModuleID:{2};SubModuleID:{3};Page:{4};Name:{5}",
                        strLangID,
                        strPreName,
                        strModuleID,
                        strSubModuleID,
                        strPage,
                        strName));

                optReturn = ReadDatabaseInfo();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DatabaseInfo dbInfo = optReturn.Data as DatabaseInfo;
                if (dbInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DatabaseInfo is null");
                    return optReturn;
                }
                string strConn = dbInfo.GetConnectionString();
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
                switch (dbInfo.TypeID)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            optReturn.Data = strSql;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
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
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", dbInfo.TypeID);
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

        private OperationReturn GetUserOptList(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     用户角色编码
                //1     模块代码
                //2     上级模块或操作编号
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string roleID = listParams[0];
                string moduleID = listParams[1];
                string parentID = listParams[2];
                WriteOperationLog(string.Format("GetUserOptList:\tRoleID:{0};ModuleID:{1};ParentID:{2}",
                    roleID,
                    moduleID,
                    parentID));

                string rentToken = "00000";
                optReturn = ReadDatabaseInfo();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DatabaseInfo dbInfo = optReturn.Data as DatabaseInfo;
                if (dbInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DatabaseInfo is null");
                    return optReturn;
                }
                string strConn = dbInfo.GetConnectionString();
                string strSql;
                DataSet objDataSet;
                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {2} AND C003='1') AND C003 = {3} ORDER BY C001, C002",
                               rentToken,
                               moduleID,
                               roleID,
                               parentID);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
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
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType not support");
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
                    }
                    opt.Type = intType;
                    opt.Other01 = dr["C009"].ToString();
                    opt.Other02 = dr["C010"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(opt);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetUserCtlObjList(List<string> listParams)
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
                string strObjType = listParams[2];
                string strParentID = listParams[3];
                WriteOperationLog(string.Format("GetUserCtlObjList:\tUserID:{0};Method:{1};ObjType:{2};ParentID:{3}",
                    strUserID,
                    strMethod,
                    strObjType,
                    strParentID));

                int intObjType;
                if (!int.TryParse(strObjType, out intObjType))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ObjectType invalid");
                    return optReturn;
                }
                string strLog = string.Empty;
                strLog += string.Format("{0};{1};{2};{3}", strUserID, strMethod, strObjType, strParentID);
                string rentToken = "00000";
                optReturn = ReadDatabaseInfo();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DatabaseInfo dbInfo = optReturn.Data as DatabaseInfo;
                if (dbInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DatabaseInfo is null");
                    return optReturn;
                }
                string strConn = dbInfo.GetConnectionString();
                string strSql;
                DataSet objDataSet;
                switch (dbInfo.TypeID)
                {
                    case 2:
                        if (strMethod == "0")
                        {
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
                                string temp = arrObjType[i];
                                strType += string.Format("C004 LIKE '{0}%' OR ", temp);
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
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strMethod == "0")
                        {
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
                                    //从T_11_101表获取
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
                                string temp = arrObjType[i];
                                strType += string.Format("C004 LIKE '{0}%' OR ", temp);
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
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", dbInfo.TypeID);
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
                if (strMethod == "0")
                {
                    if (intObjType == ConstValue.RESOURCE_ORG
                        || intObjType == ConstValue.RESOURCE_USER)
                    {
                        //机构和用户等类型信息直接从基本表中的字段获取信息
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
                                    strName = DecryptFromDB(strName);
                                    resourceObj.Name = strName;
                                    listObjs.Add(resourceObj);
                                    break;
                                case ConstValue.RESOURCE_USER:
                                    resourceObj = new ResourceObject();
                                    resourceObj.ObjID = Convert.ToInt64(dr["C001"]);
                                    resourceObj.ObjType = ConstValue.RESOURCE_USER;
                                    strName = dr["C002"].ToString();
                                    strName = DecryptFromDB(strName);
                                    resourceObj.Name = strName;
                                    strFullName = dr["C003"].ToString();
                                    strFullName = DecryptFromDB(strFullName);
                                    resourceObj.FullName = strFullName;
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
                            switch (dbInfo.TypeID)
                            {
                                case 2:
                                    strSql =
                                        string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001,C002",
                                            rentToken,
                                            resourceObj.ObjID);
                                    optReturn = MssqlOperation.GetDataSet(strConn, strSql);
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
                                    optReturn = OracleOperation.GetDataSet(strConn, strSql);
                                    if (!optReturn.Result)
                                    {
                                        return optReturn;
                                    }
                                    objDataSet = optReturn.Data as DataSet;
                                    break;
                                default:
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_PARAM_INVALID;
                                    optReturn.Message = string.Format("Database type not surpport.\t{0}", dbInfo.TypeID);
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
                                        //Row1 C017 工号
                                        //Row1 C018 坐席名
                                        if (intRow == 1)
                                        {
                                            strName = dr["C017"].ToString();
                                            strName = DecryptFromDB(strName);
                                            resourceObj.Name = strName;
                                            strFullName = dr["C018"].ToString();
                                            strFullName = DecryptFromDB(strFullName);
                                            resourceObj.FullName = strFullName;
                                        }
                                        break;
                                    case ConstValue.RESOURCE_EXTENSION:
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
                                            strNameIP = DecryptFromDB(strNameIP);
                                            strName = strNameIP;
                                            string strServerIP = string.Empty;
                                            int index = strName.IndexOf(ConstValue.SPLITER_CHAR);
                                            if (index > 0)
                                            {
                                                strName = strNameIP.Substring(0, index);
                                                strServerIP = strNameIP.Substring(index + 1);
                                            }
                                            resourceObj.Name = strName;
                                            resourceObj.Other04 = strServerIP;
                                            strFullName = dr["C018"].ToString();
                                            strFullName = DecryptFromDB(strFullName);
                                            resourceObj.FullName = strFullName;
                                        }
                                        if (intRow == 2)
                                        {
                                            string strVoiceObjID = dr["C015"].ToString();
                                            string strVoiceChanID = dr["C016"].ToString();
                                            string strVoiceChanObjID = dr["C017"].ToString();
                                            string strScreenObjID = dr["C018"].ToString();
                                            string strScreenChanID = dr["C019"].ToString();
                                            string strScreenChanObjID = dr["C020"].ToString();

                                            string strRole;
                                            if (!string.IsNullOrEmpty(strVoiceChanObjID) &&
                                                !string.IsNullOrEmpty(strScreenChanObjID))
                                            {
                                                //录音录屏通道
                                                strRole = "3";
                                                resourceObj.Other01 = string.Format("{0}{1}{2}", strVoiceObjID,
                                                    ConstValue.SPLITER_CHAR, strScreenObjID);
                                                resourceObj.Other02 = string.Format("{0}{1}{2}", strVoiceChanID,
                                                    ConstValue.SPLITER_CHAR, strScreenChanID);
                                                resourceObj.Other03 = string.Format("{0}{1}{2}", strVoiceChanObjID,
                                                    ConstValue.SPLITER_CHAR, strScreenChanObjID);
                                                resourceObj.Other05 = strRole;
                                            }
                                            else if (!string.IsNullOrEmpty(strVoiceChanObjID))
                                            {
                                                //录音通道
                                                strRole = "1";
                                                resourceObj.Other01 = strVoiceObjID;
                                                resourceObj.Other02 = strVoiceChanID;
                                                resourceObj.Other03 = strVoiceChanObjID;
                                                resourceObj.Other05 = strRole;
                                            }
                                            else
                                            {
                                                //录屏通道
                                                strRole = "2";
                                                resourceObj.Other01 = strScreenObjID;
                                                resourceObj.Other02 = strScreenChanID;
                                                resourceObj.Other03 = strScreenChanObjID;
                                                resourceObj.Other05 = strRole;
                                            }
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
                    optReturn = XMLHelper.SeriallizeObject(listObjs[i]);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetSerialID(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     模块编号
                //1     模块内编号
                //2     时间变量
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strModuleID = listParams[0];
                string strSubID = listParams[1];
                string strDatetime = listParams[2];
                WriteOperationLog(string.Format("GetSerialID:\tModuleID:{0};SubID:{1};Datetime:{2}",
                    strModuleID,
                    strSubID,
                    strDatetime));


                #region 检查参数有效性

                int intModuleID;
                int intSubID;
                if (!int.TryParse(strModuleID, out intModuleID)
                    || !int.TryParse(strSubID, out intSubID)
                    || intModuleID <= 0
                    || intSubID <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ModuleID or SubID invalid");
                    return optReturn;
                }
                DateTime dtTime = DateTime.Now;
                DateTime dtTemp;
                if (DateTime.TryParse(strDatetime, out dtTemp))
                {
                    dtTime = dtTemp;
                }

                #endregion


                #region DBInfo

                string rentToken = "00000";
                string strSerialID = string.Empty;
                long errNumber = 0;
                string strErrMsg = string.Empty;
                optReturn = ReadDatabaseInfo();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DatabaseInfo dbInfo = optReturn.Data as DatabaseInfo;
                if (dbInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DatabaseInfo is null");
                    return optReturn;
                }
                string strConn = dbInfo.GetConnectionString();

                #endregion


                #region 调用存储过程生成流水号

                switch (dbInfo.TypeID)
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
                        mssqlParameters[0].Value = intModuleID;
                        mssqlParameters[1].Value = intSubID;
                        mssqlParameters[2].Value = rentToken;
                        mssqlParameters[3].Value = dtTime.ToString("yyyyMMddHHmmss");
                        mssqlParameters[4].Value = strSerialID;
                        mssqlParameters[5].Value = errNumber;
                        mssqlParameters[6].Value = strErrMsg;
                        mssqlParameters[4].Direction = ParameterDirection.Output;
                        mssqlParameters[5].Direction = ParameterDirection.Output;
                        mssqlParameters[6].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(strConn, "P_00_001",
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
                        orclParameters[0].Value = intModuleID;
                        orclParameters[1].Value = intSubID;
                        orclParameters[2].Value = rentToken;
                        orclParameters[3].Value = dtTime.ToString("yyyyMMddHHmmss");
                        orclParameters[4].Value = strSerialID;
                        orclParameters[5].Value = errNumber;
                        orclParameters[6].Value = strErrMsg;
                        orclParameters[4].Direction = ParameterDirection.Output;
                        orclParameters[5].Direction = ParameterDirection.Output;
                        orclParameters[6].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(strConn, "P_00_001",
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
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType not support");
                        return optReturn;
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

        #endregion


        #region OtherOperations

        private OperationReturn GetRecordInfoFromJsonObject(JsonObject json)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                UMPRecordInfo recordInfo = new UMPRecordInfo();
                recordInfo.RowID = Convert.ToInt64(json[UMPRecordInfo.PRO_ROWID].Number);
                recordInfo.SerialID = json[UMPRecordInfo.PRO_SERIALID].Value;
                recordInfo.RecordReference = json[UMPRecordInfo.PRO_RECORDREFERENCE].Value;
                recordInfo.StartRecordTime = Convert.ToDateTime(json[UMPRecordInfo.PRO_STARTRECORDTIME].Value);
                recordInfo.StopRecordTime = Convert.ToDateTime(json[UMPRecordInfo.PRO_STOPRECORDTIME].Value);
                recordInfo.Extension = json[UMPRecordInfo.PRO_EXTENSION].Value;
                recordInfo.Agent = json[UMPRecordInfo.PRO_AGENT].Value;
                recordInfo.MediaType = Convert.ToInt32(json[UMPRecordInfo.PRO_MEDIATYPE].Number);
                recordInfo.EncryptFlag = json[UMPRecordInfo.PRO_ENCRYPTFLAG].Value;
                recordInfo.ServerID = Convert.ToInt32(json[UMPRecordInfo.PRO_SERVERID].Number);
                recordInfo.ServerIP = json[UMPRecordInfo.PRO_SERVERIP].Value;
                recordInfo.ChannelID = Convert.ToInt32(json[UMPRecordInfo.PRO_CHANNELID].Number);
                recordInfo.StringInfo = json.ToString();

                optReturn.Data = recordInfo;
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

        private OperationReturn GetJsonObjectFromRecordInfo(UMPRecordInfo recordInfo)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                JsonObject json = new JsonObject();
                json[UMPRecordInfo.PRO_ROWID] = new JsonProperty(string.Format("{0}", recordInfo.RowID));
                json[UMPRecordInfo.PRO_SERIALID] = new JsonProperty(string.Format("\"{0}\"", recordInfo.SerialID));
                json[UMPRecordInfo.PRO_RECORDREFERENCE] = new JsonProperty(string.Format("\"{0}\"", recordInfo.RecordReference));
                json[UMPRecordInfo.PRO_STARTRECORDTIME] = new JsonProperty(string.Format("\"{0}\"", recordInfo.StartRecordTime.ToString("yyyy-MM-dd HH:mm:ss")));
                json[UMPRecordInfo.PRO_STOPRECORDTIME] = new JsonProperty(string.Format("\"{0}\"", recordInfo.StopRecordTime.ToString("yyyy-MM-dd HH:mm:ss")));
                json[UMPRecordInfo.PRO_EXTENSION] = new JsonProperty(string.Format("\"{0}\"", recordInfo.Extension));
                json[UMPRecordInfo.PRO_AGENT] = new JsonProperty(string.Format("\"{0}\"", recordInfo.Agent));
                json[UMPRecordInfo.PRO_SERVERID] = new JsonProperty(string.Format("{0}", recordInfo.ServerID));
                json[UMPRecordInfo.PRO_SERVERIP] = new JsonProperty(string.Format("\"{0}\"", recordInfo.ServerIP));
                json[UMPRecordInfo.PRO_CHANNELID] = new JsonProperty(string.Format("{0}", recordInfo.ChannelID));
                json[UMPRecordInfo.PRO_MEDIATYPE] = new JsonProperty(string.Format("{0}", recordInfo.MediaType));
                json[UMPRecordInfo.PRO_ENCRYPTFLAG] = new JsonProperty(string.Format("\"{0}\"", recordInfo.EncryptFlag));

                optReturn.Data = json;
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

        #endregion


        #region LogOperator

        private void mService03Helper_Debug(LogMode mode,string category, string msg)
        {
            WriteOperationLog(mode, category, msg);
        }

        private void CreateLogOperator()
        {
            mLogOperator = new LogOperator();
            mLogOperator.LogPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\Wcf000A1\\Logs");
            mLogOperator.Start();
        }

        private void WriteOperationLog(LogMode mode, string category, string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(mode, category, msg);
            }
        }

        private void WriteOperationLog(string category, string msg)
        {
            WriteOperationLog(LogMode.Info, category, msg);
        }

        private void WriteOperationLog(string msg)
        {
            WriteOperationLog("Wcf000A1", msg);
        }

        #endregion


        #region Dispose

        public void Dispose()
        {
            if (mService03Helper != null)
            {
                mService03Helper.Close();
            }
            mService03Helper = null;
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
                mLogOperator = null;
            }
        }

        #endregion

    }
}
