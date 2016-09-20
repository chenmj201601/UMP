using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Common12002;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;
using Wcf12001.Wcf11012;

namespace Wcf12001
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service12001”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service12001.svc 或 Service12001.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service12001 : IService12001
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
                    case (int)S1200Codes.GetAppFileList:
                        optReturn = GetAppFileList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetAppConfigList:
                        optReturn = GetAppConfigList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetAppBasicInfoList:
                        optReturn = GetAppBasicInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetOptLogInfoList:
                        optReturn = GetOptLogInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetOptContentLangList:
                        optReturn = GetOptContentLangList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetAppUsageInfoList:
                        optReturn = GetAppUsageInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetSupportLangTypeList:
                        optReturn = GetSupportLangTypeList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetLanguageInfoListXml:
                        optReturn = GetLanguageInfoListXml(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetUserRoleList:
                        optReturn = GetUserRoleList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetUserWidgetList:
                        optReturn = GetUserWidgetList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetThirdPartyAppList:
                        optReturn = GetThirdPartyAppList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetBasicInfoList:
                        optReturn = GetBasicInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetWidgetPropertyInfoList:
                        optReturn = GetWidgetPropertyInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetUserWidgetPropertyValueList:
                        optReturn = GetUserWidgetPropertyValueList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetAllWidgetList:
                        optReturn = GetAllWidgetList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetLogicalPartTableList:
                        optReturn = GetLogicalPartTableList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetDomainInfoList:
                        optReturn = GetDomainInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.GetLDAPUserList:
                        optReturn = GetLDAPUserList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.SetAppUsageInfo:
                        optReturn = SetAppUsageInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.SaveUserWidgetPropertyValues:
                        optReturn = SaveUserWidgetPropertyValues(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S1200Codes.SaveUserWidgetInfos:
                        optReturn = SaveUserWidgetInfos(session, webRequest.ListData);
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

        private OperationReturn GetAppFileList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     Application ModuleID
                if (listParams == null
                    || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strModuleID = listParams[0];
                string strVersion = session.AppVersion;
                strVersion = strVersion.Replace('.', '_');
                string strDir = AppDomain.CurrentDomain.BaseDirectory;
                strDir = strDir.Substring(0, strDir.LastIndexOf("\\"));
                strDir = strDir.Substring(0, strDir.LastIndexOf("\\"));
                strDir = Path.Combine(strDir, "Application Files");
                if (!Directory.Exists(strDir))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("Application Files directory not exist.\t{0}", strDir);
                    return optReturn;
                }
                DirectoryInfo dirInfo = new DirectoryInfo(strDir);
                DirectoryInfo[] listDirs = dirInfo.GetDirectories();
                var dir =
                    listDirs.Where(d => d.Name.StartsWith(string.Format("UMPS{0}_{1}", strModuleID, strVersion)))
                        .OrderByDescending(d => d.Name.Length)
                        .ThenByDescending(d => d.Name)
                        .ToList()
                        .FirstOrDefault();
                if (dir == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("Application directory not exist.\t{0}", strModuleID);
                    return optReturn;
                }
                FileInfo[] fileInfos = dir.GetFiles();
                List<string> listFiles = new List<string>();
                for (int i = 0; i < fileInfos.Length; i++)
                {
                    var fileInfo = fileInfos[i];
                    AppFileInfo appFile = new AppFileInfo();
                    appFile.Name = fileInfo.Name;
                    appFile.FullName = fileInfo.FullName;
                    appFile.CreateTime = fileInfo.CreationTime;
                    appFile.ModifyTime = fileInfo.LastWriteTime;
                    optReturn = XMLHelper.SeriallizeObject(appFile);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listFiles.Add(optReturn.Data.ToString());
                }
                optReturn.Message = dir.Name;
                optReturn.Data = listFiles;
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

        private OperationReturn GetAppConfigList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, "GlobalSettings");
                path = Path.Combine(path, AppConfigs.FILE_NAME);
                if (!File.Exists(path))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("UMP.Server.09.xml file not exist.\t{0}", path);
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeFile<AppConfigs>(path);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                AppConfigs configs = optReturn.Data as AppConfigs;
                if (configs == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AppConfigs is null");
                    return optReturn;
                }
                int count = configs.ListApps.Count;
                List<string> listReturns = new List<string>();
                for (int i = 0; i < count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(configs.ListApps[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturns.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturns;
                optReturn.Message = string.Format("Count:{0}", count);
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

        private OperationReturn GetAppBasicInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     RoleID
                //1     ParentID（默认 0）
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strRoleID = listParams[0];
                string strParentID = listParams[1];
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


                string strRentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_003_{0} WHERE C003 >10 AND  C003 < 100 AND C005 <> '0' AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {1}) ORDER BY C001,C002",
                                strRentToken, strRoleID);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_003_{0} WHERE C003 >10 AND  C003 < 100 AND C005 <> '0' AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {1}) ORDER BY C001,C002",
                                strRentToken, strRoleID);
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];


                    #region 标记为不可用的模块不列出

                    string strType = dr["C011"].ToString().ToUpper();
                    if (strType == "D")
                    {
                        //禁用的模块不列出
                        continue;
                    }

                    #endregion


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
                        string strDecryptC007 = DecryptStringKeyIV(strC007, strC008Hash8,
                            strC008Hash8);
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


                    int masterID = Convert.ToInt32(dr["C001"]);
                    int moduleID = Convert.ToInt32(dr["C002"]);
                    int sortID = Convert.ToInt32(dr["C004"]);
                    string strCategory = dr["C005"].ToString();
                    string strArgs = dr["C010"].ToString();
                    string strOpenMethod = dr["C011"].ToString().ToUpper();
                    string strIcon = dr["C013"].ToString();
                    string strPage = dr["C009"].ToString();
                    if (strPage.Length < 13) { continue; }
                    string strAppID = strPage.Substring(4, 4);
                    int appID;
                    if (!int.TryParse(strAppID, out appID))
                    {
                        //特殊模块
                        if (moduleID == S1200Consts.MODULEID_ASM)
                        {
                            appID = 4401;
                            strArgs = dr["C009"].ToString();
                        }
                        else
                        {
                            continue;
                        }
                    }
                    BasicAppInfo item = new BasicAppInfo();
                    item.ModuleID = moduleID;
                    item.MasterID = masterID;
                    item.AppID = appID;
                    item.Title = string.Format("UMPS{0}", moduleID);
                    item.SortID = sortID;
                    item.Category = strCategory;
                    item.Args = strArgs;
                    item.OpenMethod = strOpenMethod;
                    item.Icon = strIcon;
                    optReturn = XMLHelper.SeriallizeObject(item);
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

        private OperationReturn GetOptLogInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     操作人ID（0 表示获取所有操作人)
                //1     最大返回数量（0 表示返回所有）
                //2     需要跳过的数量（0表示不跳过）
                if (listParams == null
                    || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strOpterID = listParams[0];
                string strTopNum = listParams[1];
                string strSkipNum = listParams[2];
                long opterID;
                int topNum;
                int skipNum;
                if (!long.TryParse(strOpterID, out opterID)
                    || !int.TryParse(strTopNum, out topNum)
                    || !int.TryParse(strSkipNum, out skipNum)
                    || opterID < 0
                    || topNum < 0
                    || skipNum < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UserID or TopNum or SkipNum invalid");
                    return optReturn;
                }
                string strRentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                long userID = session.UserID;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        if (topNum > 0)
                        {
                            if (opterID > 0)
                            {
                                strSql = string.Format("SELECT TOP {0} * FROM T_11_901_{1} WHERE C005 = {2} ORDER BY C008 DESC",
                                   topNum,
                                   strRentToken,
                                   opterID);
                            }
                            else
                            {
                                strSql = string.Format("SELECT TOP {0} * FROM T_11_901_{1} WHERE C005 IN (SELECT C004 FROM T_11_201_{1} WHERE C003 = {2}) ORDER BY C008 DESC",
                                   topNum,
                                   strRentToken,
                                   userID);
                            }
                        }
                        else
                        {
                            if (opterID > 0)
                            {
                                strSql = string.Format("SELECT * FROM T_11_901_{0} WHERE C005 = {1} ORDER BY C008 DESC",
                                    strRentToken,
                                    opterID);
                            }
                            else
                            {
                                strSql = string.Format("SELECT * FROM T_11_901_{0} C005 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) ORDER BY C008 DESC", strRentToken, userID);
                            }
                        }
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        if (topNum > 0)
                        {
                            if (opterID > 0)
                            {
                                strSql = string.Format("SELECT * FROM (SELECT * FROM T_11_901_{1} WHERE C005 = {2} ORDER BY C008 DESC) WHERE ROWNUM <= {0}",
                                   topNum,
                                   strRentToken,
                                   opterID);
                            }
                            else
                            {
                                strSql = string.Format("SELECT * FROM (SELECT * FROM T_11_901_{1} WHERE C005 IN (SELECT C004 FROM T_11_201_{1} WHERE C003 = {2}) ORDER BY C008 DESC) WHERE ROWNUM < = {0}",
                                   topNum,
                                   strRentToken,
                                   userID);
                            }
                        }
                        else
                        {
                            if (opterID > 0)
                            {
                                strSql = string.Format("SELECT * FROM T_11_901_{0} WHERE C005 = {1} ORDER BY C008 DESC",
                                    strRentToken,
                                    opterID);
                            }
                            else
                            {
                                strSql = string.Format("SELECT * FROM T_11_901_{0} C005 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) ORDER BY C008 DESC", strRentToken, userID);
                            }
                        }
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    if (i < skipNum) { continue; }

                    OperationLogInfo info = new OperationLogInfo();
                    info.ID = Convert.ToInt64(dr["C001"]);
                    info.ModuleID = Convert.ToInt32(dr["C003"]);
                    info.OptID = Convert.ToInt64(dr["C004"]);
                    info.UserID = Convert.ToInt64(dr["C005"]);
                    info.MachineName = dr["C006"].ToString();
                    info.MachineIP = dr["C007"].ToString();
                    info.LogTime = Converter.NumberToDatetime(dr["C008"].ToString());
                    info.LogResult = dr["C009"].ToString();
                    info.LangID = dr["C010"].ToString();
                    string strArgs = dr["C011"].ToString();
                    strArgs += dr["C012"].ToString();
                    strArgs += dr["C013"].ToString();
                    strArgs += dr["C014"].ToString();
                    strArgs += dr["C015"].ToString();
                    info.LogArgs = EncryptString04(strArgs);
                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetOptContentLangList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     LangTypeID
                //1     Count
                //2...  OptContentID
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strLangID = listParams[0];
                string strCount = listParams[1];
                int intLangID, intCount;
                if (!int.TryParse(strLangID, out intLangID)
                    || !int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param LangID or Count invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param OptContentID count invalid");
                    return optReturn;
                }
                List<string> listLangIDs = new List<string>();
                for (int i = 0; i < intCount; i++)
                {
                    string strOptContentID = listParams[i + 2];
                    listLangIDs.Add(strOptContentID);
                }
                List<string> listReturn = new List<string>();
                if (listLangIDs.Count > 0)
                {
                    //将语言ID插入临时表，并返回临时ID
                    WebRequest request = new WebRequest();
                    request.Session = session;
                    request.Code = (int)RequestCode.WSInsertTempData;
                    request.ListData.Add("0");
                    request.ListData.Add(intCount.ToString());
                    for (int i = 0; i < intCount; i++)
                    {
                        request.ListData.Add(listLangIDs[i]);
                    }
                    Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(session),
                        WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service11012"));
                    WebReturn webReturn = client.DoOperation(request);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        optReturn.Result = false;
                        optReturn.Code = webReturn.Code;
                        optReturn.Message = webReturn.Message;
                        return optReturn;
                    }
                    string strTempID = webReturn.Data;
                    int dbType = session.DBType;
                    string strConString = session.DBConnectionString;
                    string strSql;
                    switch (dbType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_005 WHERE C002 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND C001 = {1}",
                                    strTempID, intLangID);
                            optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                            break;
                        case 3:
                            strSql =
                               string.Format(
                                   "SELECT * FROM T_00_005 WHERE C002 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND C001 = {1}",
                                   strTempID, intLangID);
                            optReturn = OracleOperation.GetDataSet(strConString, strSql);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid");
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
                        optReturn.Message = string.Format("DataSet is null");
                        return optReturn;
                    }
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
                        listReturn.Add(optReturn.Data.ToString());
                    }
                    optReturn.Message = strSql;
                }
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

        private OperationReturn GetAppUsageInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     LastDate(可选，查询此时间之后的数据，若为空查询所有数据）
                if (listParams == null
                    || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UserID invalid");
                    return optReturn;
                }
                string strLastDate = string.Empty;
                DateTime dtLastDate = DateTime.MinValue;
                if (listParams.Count > 1)
                {
                    strLastDate = listParams[1];
                    if (!DateTime.TryParse(strLastDate, out dtLastDate))
                    {
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Param LastDate invalid.\t{0}", strLastDate);
                        return optReturn;
                    }
                }
                int dbType = session.DBType;
                string strRentToken = session.RentInfo.Token;
                string strConString = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        if (string.IsNullOrEmpty(strLastDate))
                        {
                            strSql = string.Format("SELECT * FROM T_11_012_{0} WHERE C003 = {1} ORDER BY C001, C002, C003, C004", strRentToken, userID);
                        }
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_012_{0} WHERE C003 = {1} AND C004 > {2} ORDER BY C001, C002, C003, C004",
                                    strRentToken, userID, dtLastDate.ToString("yyyyMMddHHmmss"));
                        }
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        if (string.IsNullOrEmpty(strLastDate))
                        {
                            strSql = string.Format("SELECT * FROM T_11_012_{0} WHERE C003 = {1} ORDER BY C001, C002, C003, C004", strRentToken, userID);
                        }
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_012_{0} WHERE C003 = {1} AND C004 > {2} ORDER BY C001, C002, C003, C004",
                                    strRentToken, userID, dtLastDate.ToString("yyyyMMddHHmmss"));
                        }
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    ModuleUsageInfo info = new ModuleUsageInfo();
                    info.AppID = Convert.ToInt32(dr["C001"]);
                    info.ModuleID = Convert.ToInt32(dr["C002"]);
                    info.UserID = Convert.ToInt64(dr["C003"]);
                    info.BeginTime = GetDatetimeFromDB(dr["C004"].ToString());
                    info.EndTime = GetDatetimeFromDB(dr["C005"].ToString());
                    info.SessionID = dr["C006"].ToString();
                    info.StartArgs = dr["C007"].ToString();
                    info.HostName = dr["C008"].ToString();
                    info.HostAddress = dr["C009"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetSupportLangTypeList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID (可以指定0）
                if (listParams == null
                    || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_00_004 ORDER BY C001, C002");
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_00_004 ORDER BY C001, C002");
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    LangTypeInfo info = new LangTypeInfo();
                    info.LangID = Convert.ToInt32(dr["C001"]);
                    info.LangName = info.LangID.ToString();
                    info.Display = DecryptString02(dr["C003"].ToString());
                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetLanguageInfoListXml(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID (可以指定0）
                //1     LangID
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strLangID = listParams[1];
                int langID;
                if (!int.TryParse(strLangID, out langID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param LangID invalid");
                    return optReturn;
                }
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, @"MAMT\Languages");
                path = Path.Combine(path, string.Format("S{0}.xml", langID));
                if (!File.Exists(path))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("Language file not exist.\t{0}", path);
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode nodeLanguages = doc.SelectSingleNode("UMPMamt/SupportLanguages");
                if (nodeLanguages != null)
                {
                    for (int i = 0; i < nodeLanguages.ChildNodes.Count; i++)
                    {
                        XmlElement element = nodeLanguages.ChildNodes[i] as XmlElement;
                        if (element != null)
                        {
                            LanguageInfo info = new LanguageInfo();
                            info.LangID = langID;
                            info.Name = string.Format("Lang{0}", element.GetAttribute("C004"));
                            info.Display = element.GetAttribute("C002");
                            optReturn = XMLHelper.SeriallizeObject(info);
                            if (optReturn.Result)
                            {
                                listReturn.Add(optReturn.Data.ToString());
                            }
                        }
                    }
                }
                XmlNode nodeThemes = doc.SelectSingleNode("UMPMamt/SupportStyle");
                if (nodeThemes != null)
                {
                    for (int i = 0; i < nodeThemes.ChildNodes.Count; i++)
                    {
                        XmlElement element = nodeThemes.ChildNodes[i] as XmlElement;
                        if (element != null)
                        {
                            LanguageInfo info = new LanguageInfo();
                            info.LangID = langID;
                            info.Name = string.Format("Theme{0}", element.GetAttribute("C001"));
                            info.Display = element.GetAttribute("C002");
                            optReturn = XMLHelper.SeriallizeObject(info);
                            if (optReturn.Result)
                            {
                                listReturn.Add(optReturn.Data.ToString());
                            }
                        }
                    }
                }
                XmlNode nodeLangInfos = doc.SelectSingleNode("UMPMamt/Languages");
                if (nodeLangInfos != null)
                {
                    for (int i = 0; i < nodeLangInfos.ChildNodes.Count; i++)
                    {
                        XmlElement element = nodeLangInfos.ChildNodes[i] as XmlElement;
                        if (element != null)
                        {
                            LanguageInfo info = new LanguageInfo();
                            info.LangID = langID;
                            info.Name = string.Format("Info{0}", element.GetAttribute("C001"));
                            info.Display = element.GetAttribute("C002");
                            optReturn = XMLHelper.SeriallizeObject(info);
                            if (optReturn.Result)
                            {
                                listReturn.Add(optReturn.Data.ToString());
                            }
                        }
                    }
                }
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

        private OperationReturn GetUserRoleList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                if (listParams == null
                    || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UserID invalid");
                    return optReturn;
                }
                string strRentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_004_{0} WHERE C001 IN (SELECT C003 FROM T_11_201_{0} WHERE C004 = {1}) ORDER BY C001",
                                strRentToken, userID);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_004_{0} WHERE C001 IN (SELECT C003 FROM T_11_201_{0} WHERE C004 = {1}) ORDER BY C001",
                                strRentToken, userID);
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    RoleInfo info = new RoleInfo();
                    info.ID = Convert.ToInt64(dr["C001"]);
                    info.Name = DecryptString02(dr["C004"].ToString());
                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetUserWidgetList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                if (listParams == null
                    || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UserID invalid");
                    return optReturn;
                }
                string strRentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                string strSql;
                bool bIsDefault = false;
                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("SELECT COUNT(1) FROM T_11_015_{0} WHERE C001 = {1}", strRentToken, userID);
                        optReturn = MssqlOperation.GetRecordCount(strConString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (optReturn.IntValue <= 0)
                        {
                            bIsDefault = true;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_013_{0} WHERE C003 = '1' ORDER BY C001",
                                    strRentToken);
                        }
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT A.C001 AS AC001,A.C002 AS AC002,A.C006 AS AC006,B.C003 AS BC003,B.C004 AS BC004 FROM T_11_013_{0} A, T_11_015_{0} B WHERE A.C001 = B.C002 AND B.C001 = {1}",
                                    strRentToken, userID);
                        }
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT COUNT(1) FROM T_11_015_{0} WHERE C001 = {1}", strRentToken, userID);
                        optReturn = OracleOperation.GetRecordCount(strConString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (optReturn.IntValue <= 0)
                        {
                            bIsDefault = true;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_013_{0} WHERE C003 = '1' ORDER BY C001",
                                    strRentToken);
                        }
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT A.C001 AS AC001,A.C002 AS AC002,A.C006 AS AC006,B.C003 AS BC003,B.C004 AS BC004 FROM T_11_013_{0} A, T_11_015_{0} B WHERE A.C001 = B.C002 AND B.C001 = {1}",
                                    strRentToken, userID);
                        }
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    WidgetInfo info = new WidgetInfo();
                    if (bIsDefault)
                    {
                        info.WidgetID = Convert.ToInt64(dr["C001"]);
                        info.Name = DecryptString02(dr["C002"].ToString());
                        info.IsCenter = dr["C004"].ToString() == "1";
                        info.SortID = Convert.ToInt32(dr["C005"]);
                        info.Title = dr["C006"].ToString();
                    }
                    else
                    {
                        info.WidgetID = Convert.ToInt64(dr["AC001"]);
                        info.Name = DecryptString02(dr["AC002"].ToString());
                        info.IsCenter = dr["BC003"].ToString() == "1";
                        info.SortID = Convert.ToInt32(dr["BC004"]);
                        info.Title = dr["AC006"].ToString();
                    }
                    optReturn = XMLHelper.SeriallizeObject(info);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
                optReturn.Message = bIsDefault ? "1" : "0";
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

        private OperationReturn GetThirdPartyAppList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, "GlobalSettings");
                path = Path.Combine(path, S1200Consts.CONFIG_FILE_NAME_07);
                if (!File.Exists(path))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("UMP.Server.07.xml file not exist.\t{0}", path);
                    return optReturn;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                var appsNode = doc.SelectSingleNode("UMPSetted/ThirdPartyApplications");
                if (appsNode == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_CONFIG_INVALID;
                    optReturn.Message = string.Format("ThirdPartyApplications not exist");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                var listApps = appsNode.ChildNodes;
                for (int i = 0; i < listApps.Count; i++)
                {
                    var appElement = listApps[i] as XmlElement;
                    if (appElement == null) { continue; }

                    ThirdPartyAppInfo info = new ThirdPartyAppInfo();
                    info.Name = appElement.GetAttribute("Attribute00");
                    info.Protocol = appElement.GetAttribute("Attribute01");
                    info.HostAddress = appElement.GetAttribute("Attribute02");
                    string strPort = appElement.GetAttribute("Attribute03");
                    int intValue;
                    if (!int.TryParse(strPort, out intValue))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_CONFIG_INVALID;
                        optReturn.Message = string.Format("HostPort invalid");
                        return optReturn;
                    }
                    info.HostPort = intValue;
                    info.Arg01 = appElement.GetAttribute("Attribute04");
                    info.Arg02 = appElement.GetAttribute("Attribute05");
                    info.Arg03 = appElement.GetAttribute("Attribute06");
                    info.Arg04 = appElement.GetAttribute("Attribute07");
                    info.Arg05 = appElement.GetAttribute("Attribute08");
                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetBasicInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     类型      0：通过InfoID获取列表
                //                  1：通过模块号获取整个模块的列表
                //1     InfoID或ModuleID
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strType = listParams[0];
                string strInfoID = listParams[1];
                int intType;
                long longInfoID;
                if (!int.TryParse(strType, out intType)
                    || !long.TryParse(strInfoID, out longInfoID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param Type or InfoID invalid");
                    return optReturn;
                }

                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        if (intType == 1)
                        {
                            long min = longInfoID * 100000;
                            long max = (longInfoID + 1) * 100000;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 > {0} AND C001 < {1} ORDER BY C001, C002",
                                    min, max);
                        }
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 = {0} ORDER BY C001, C002",
                                    strInfoID);
                        }
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        if (intType == 1)
                        {
                            long min = longInfoID * 100000;
                            long max = (longInfoID + 1) * 100000;
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 > {0} AND C001 < {1} ORDER BY C001, C002",
                                    min, max);
                        }
                        else
                        {
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_00_003 WHERE C001 = {0} ORDER BY C001, C002",
                                    strInfoID);
                        }
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    BasicDataInfo info = new BasicDataInfo();
                    info.InfoID = Convert.ToInt32(dr["C001"]);
                    info.SortID = Convert.ToInt32(dr["C002"]);
                    info.ParentID = Convert.ToInt32(dr["C003"]);
                    info.IsEnable = dr["C004"].ToString() == "1";
                    info.EncryptVersion = Convert.ToInt32(dr["C005"]);
                    info.Value = dr["C006"].ToString();
                    info.Icon = dr["C007"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetWidgetPropertyInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     WidgetID
                if (listParams == null
                    || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strWidgetID = listParams[0];
                string strRentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_014_{0} WHERE C001 = {1} ORDER BY C001, C002",
                                strRentToken, strWidgetID);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_014_{0} WHERE C001 = {1} ORDER BY C001, C002",
                                strRentToken, strWidgetID);
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    WidgetPropertyInfo info = new WidgetPropertyInfo();
                    info.WidgetID = Convert.ToInt64(dr["C001"]);
                    info.PropertyID = Convert.ToInt32(dr["C002"]);
                    info.Name = DecryptString02(dr["C003"].ToString());
                    info.SortID = Convert.ToInt32(dr["C004"]);
                    info.DataType = (WidgetPropertyDataType)Convert.ToInt32(dr["C005"]);
                    info.ConvertFormat = (WidgetPropertyConvertFormat)Convert.ToInt32(dr["C006"]);
                    info.SourceID = Convert.ToInt64(dr["C007"]);
                    info.DefaultValue = dr["C008"].ToString();
                    info.MinValue = dr["C009"].ToString();
                    info.MaxValue = dr["C010"].ToString();

                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetUserWidgetPropertyValueList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     WidgetID
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strWidgetID = listParams[1];

                string strRentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_016_{0} WHERE C001 = {1} AND C002 = {2} ORDER BY C001, C002, C003",
                                strRentToken, strUserID, strWidgetID);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_016_{0} WHERE C001 = {1} AND C002 = {2} ORDER BY C001, C002, C003",
                                strRentToken, strUserID, strWidgetID);
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    UserWidgetPropertyValue info = new UserWidgetPropertyValue();
                    info.UserID = Convert.ToInt64(dr["C001"]);
                    info.WidgetID = Convert.ToInt64(dr["C002"]);
                    info.PropertyID = Convert.ToInt32(dr["C003"]);
                    info.Value01 = dr["C004"].ToString();
                    info.Value02 = dr["C005"].ToString();
                    info.Value03 = dr["C006"].ToString();
                    info.Value04 = dr["C007"].ToString();
                    info.Value05 = dr["C008"].ToString();

                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetAllWidgetList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                if (listParams == null
                    || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UserID invalid");
                    return optReturn;
                }
                string strRentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_013_{0} ORDER BY C001",
                                strRentToken);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_013_{0} ORDER BY C001",
                                strRentToken);
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    WidgetInfo info = new WidgetInfo();
                    info.WidgetID = Convert.ToInt64(dr["C001"]);
                    info.Name = DecryptString02(dr["C002"].ToString());
                    info.IsCenter = dr["C004"].ToString() == "1";
                    info.SortID = Convert.ToInt32(dr["C005"]);
                    info.Title = dr["C006"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetLogicalPartTableList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     RentToken
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strRentToken = listParams[1];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UserID invalid");
                    return optReturn;
                }
                if (string.IsNullOrEmpty(strRentToken))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RentToken invalid");
                    return optReturn;
                }
                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_00_000 WHERE C000 = '{0}' AND C003 = 'LP' AND C004 = '1' ORDER BY C000, C001",
                                strRentToken);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_00_000 WHERE C000 = '{0}' AND C003 = 'LP' AND C004 = '1' ORDER BY C000, C001",
                               strRentToken);
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    string strValue = dr["C001"].ToString();
                    string strColumn = dr["C008"].ToString();

                    PartitionTableInfo info = null;
                    if (strValue.IndexOf("21_001") >= 0)
                    {
                        info = new PartitionTableInfo();
                        info.TableName = ConstValue.TABLE_NAME_RECORD;
                        info.PartType = TablePartType.DatetimeRange;
                        info.Other1 = strColumn;
                    }
                    if (strValue.IndexOf("11_901") > 0)
                    {
                        info = new PartitionTableInfo();
                        info.TableName = ConstValue.TABLE_NAME_OPTLOG;
                        info.PartType = TablePartType.DatetimeRange;
                        info.Other1 = strColumn;
                    }
                    if (info != null)
                    {
                        optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetDomainInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                if (listParams == null
                    || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UserID invalid");
                    return optReturn;
                }
                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_00_012 WHERE C008 = '1' AND C009 = '0' ORDER BY C004");
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_00_012 WHERE C008 = '1' AND C009 = '0' ORDER BY C004");
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                string strValue;
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    BasicDomainInfo info = new BasicDomainInfo();
                    info.ObjID = Convert.ToInt64(dr["C001"]);
                    info.RentToken = dr["C002"].ToString();
                    info.Name = DecryptString02(dr["C003"].ToString());
                    info.SortID = Convert.ToInt32(dr["C004"]);
                    info.IsActived = dr["C008"].ToString() == "1";

                    strValue = dr["C009"].ToString();
                    if (strValue == "1") { continue; }

                    info.AllowAutoLogin = dr["C010"].ToString() == "1";
                    info.Description = dr["C999"].ToString();

                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetLDAPUserList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     Account
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strAccount = listParams[1];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UserID invalid");
                    return optReturn;
                }
                if (string.IsNullOrEmpty(strAccount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param Account invalid");
                    return optReturn;
                }
                strAccount = strAccount.ToLower();
                string strAccountDB = EncryptString02(strAccount);
                string strRentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConString = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C002 = '{1}'", strRentToken,
                            strAccountDB);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C002 = '{1}'", strRentToken,
                            strAccountDB);
                        optReturn = OracleOperation.GetDataSet(strConString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid");
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
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    UserInfo info = new UserInfo();
                    info.UserID = Convert.ToInt64(dr["C001"]);
                    info.Account = DecryptString02(dr["C002"].ToString());
                    info.UserName = DecryptString02(dr["C003"].ToString());
                    string strPass = DecryptString03(dr["C004"].ToString());
                    strPass = EncryptString04(strPass);
                    info.Password = strPass;

                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }



        private OperationReturn SetAppUsageInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     count
                //2...  UsageInfo
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strCount = listParams[1];


                #region 检查参数

                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UserID invalid");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UsageCount invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Usage count invalid");
                    return optReturn;
                }

                #endregion


                #region 获取客户端主机地址

                string strRemote = string.Empty;
                OperationContext context = OperationContext.Current;
                MessageProperties properties = context.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint =
                    properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (endpoint != null)
                {
                    strRemote = endpoint.Address;
                }

                #endregion


                List<ModuleUsageInfo> listUsageInfos = new List<ModuleUsageInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 2];
                    optReturn = XMLHelper.DeserializeObject<ModuleUsageInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    ModuleUsageInfo info = optReturn.Data as ModuleUsageInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("ModuleUsageInfo is null");
                        return optReturn;
                    }
                    info.HostAddress = strRemote;
                    listUsageInfos.Add(info);
                }
                if (listUsageInfos.Count > 0)
                {
                    string rentToken = session.RentInfo.Token;
                    string strConString = session.DBConnectionString;
                    string strSql;
                    IDbConnection objConn;
                    IDbDataAdapter objAdapter;
                    DbCommandBuilder objCmdBuilder;
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_11_012_{0} WHERE C003 = {1} ORDER BY C001, C004", rentToken, userID);
                            optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_11_012_{0} WHERE C003 = {1} ORDER BY C001, C004", rentToken, userID);
                            optReturn = OracleOperation.GetDataSet(strConString, strSql);
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
                    try
                    {
                        DataSet objDataSet = new DataSet();
                        objAdapter.Fill(objDataSet);

                        DataRow dr;
                        List<string> listReturns = new List<string>();
                        for (int i = 0; i < listUsageInfos.Count; i++)
                        {
                            var item = listUsageInfos[i];
                            bool bIsAdded = false;

                            DataRow[] drs =
                                objDataSet.Tables[0].Select(string.Format("C001 = {0} AND C002 = {1} AND C004 = {2}",
                                    item.AppID, item.ModuleID,
                                    item.BeginTime.ToString("yyyyMMddHHmmss")));
                            if (drs.Length <= 0)
                            {
                                bIsAdded = true;
                                dr = objDataSet.Tables[0].NewRow();
                                dr["C001"] = item.AppID;
                                dr["C002"] = item.ModuleID;
                                dr["C003"] = userID;
                                dr["C004"] = long.Parse(item.BeginTime.ToString("yyyyMMddHHmmss"));
                            }
                            else
                            {
                                dr = drs[0];
                            }
                            dr["C005"] = long.Parse(item.EndTime.ToString("yyyyMMddHHmmss"));
                            dr["C006"] = item.SessionID;
                            dr["C007"] = item.StartArgs;
                            dr["C008"] = item.HostName;
                            dr["C009"] = item.HostAddress;
                            if (bIsAdded)
                            {
                                objDataSet.Tables[0].Rows.Add(dr);

                                listReturns.Add(string.Format("A_{0}_{1}_{2}", item.AppID, item.ModuleID,
                                    item.BeginTime.ToString("yyyyMMddHHmmss")));
                            }
                            else
                            {
                                listReturns.Add(string.Format("M_{0}_{1}_{2}", item.AppID, item.ModuleID,
                                   item.BeginTime.ToString("yyyyMMddHHmmss")));
                            }

                            objAdapter.Update(objDataSet);
                            objDataSet.AcceptChanges();
                        }
                        optReturn.Data = listReturns;
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

        private OperationReturn SaveUserWidgetPropertyValues(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     count
                //2...  UserWidgetPropertyInfo
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strCount = listParams[1];


                #region 检查参数

                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UserID invalid");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UsageCount invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("PropertyValue count invalid");
                    return optReturn;
                }

                #endregion


                #region 处理参数

                List<UserWidgetPropertyValue> listInfos = new List<UserWidgetPropertyValue>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 2];
                    optReturn = XMLHelper.DeserializeObject<UserWidgetPropertyValue>(strInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    UserWidgetPropertyValue info = optReturn.Data as UserWidgetPropertyValue;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("UserWidgetPropertyValue is null");
                        return optReturn;
                    }
                    listInfos.Add(info);
                }

                #endregion


                if (listInfos.Count > 0)
                {
                    string rentToken = session.RentInfo.Token;
                    string strConString = session.DBConnectionString;
                    string strSql;
                    IDbConnection objConn;
                    IDbDataAdapter objAdapter;
                    DbCommandBuilder objCmdBuilder;
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_11_016_{0} WHERE C001 = {1} ORDER BY C001, C002, C003", rentToken, userID);
                            objConn = MssqlOperation.GetConnection(strConString);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_11_016_{0} WHERE C001 = {1} ORDER BY C001, C002, C003", rentToken, userID);
                            objConn = OracleOperation.GetConnection(strConString);
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
                    try
                    {
                        DataSet objDataSet = new DataSet();
                        objAdapter.Fill(objDataSet);

                        DataRow dr;
                        List<string> listReturns = new List<string>();
                        for (int i = 0; i < listInfos.Count; i++)
                        {
                            var item = listInfos[i];
                            bool bIsAdded = false;

                            DataRow[] drs =
                                objDataSet.Tables[0].Select(string.Format("C001 = {0} AND C002 = {1} AND C003 = {2}",
                                    item.UserID, item.WidgetID,
                                    item.PropertyID));
                            if (drs.Length <= 0)
                            {
                                bIsAdded = true;
                                dr = objDataSet.Tables[0].NewRow();
                                dr["C001"] = item.UserID;
                                dr["C002"] = item.WidgetID;
                                dr["C003"] = item.PropertyID;
                            }
                            else
                            {
                                dr = drs[0];
                            }
                            dr["C004"] = item.Value01;
                            dr["C005"] = item.Value02;
                            dr["C006"] = item.Value03;
                            dr["C007"] = item.Value04;
                            dr["C008"] = item.Value05;
                            if (bIsAdded)
                            {
                                objDataSet.Tables[0].Rows.Add(dr);

                                listReturns.Add(string.Format("A_{0}_{1}_{2}", item.UserID, item.WidgetID,
                                    item.PropertyID));
                            }
                            else
                            {
                                listReturns.Add(string.Format("M_{0}_{1}_{2}", item.UserID, item.WidgetID,
                                     item.PropertyID));
                            }
                        }

                        objAdapter.Update(objDataSet);
                        objDataSet.AcceptChanges();

                        optReturn.Data = listReturns;
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

        private OperationReturn SaveUserWidgetInfos(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     count
                //2...  WidgetInfo
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strCount = listParams[1];


                #region 检查参数

                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UserID invalid");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param UsageCount invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("WidgetInfo count invalid");
                    return optReturn;
                }

                #endregion


                #region 处理参数

                List<WidgetInfo> listInfos = new List<WidgetInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 2];
                    optReturn = XMLHelper.DeserializeObject<WidgetInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    WidgetInfo info = optReturn.Data as WidgetInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("WidgetInfo is null");
                        return optReturn;
                    }
                    listInfos.Add(info);
                }

                #endregion


                if (listInfos.Count > 0)
                {
                    string rentToken = session.RentInfo.Token;
                    string strConString = session.DBConnectionString;
                    string strSql;
                    IDbConnection objConn;
                    IDbDataAdapter objAdapter;
                    DbCommandBuilder objCmdBuilder;

                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_11_015_{0} WHERE C001 = {1} ORDER BY C001, C002", rentToken, userID);
                            objConn = MssqlOperation.GetConnection(strConString);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_11_015_{0} WHERE C001 = {1} ORDER BY C001, C002", rentToken, userID);
                            objConn = OracleOperation.GetConnection(strConString);
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
                    try
                    {
                        DataSet objDataSet = new DataSet();
                        objAdapter.Fill(objDataSet);

                        DataRow dr;
                        List<string> listReturns = new List<string>();
                        for (int i = 0; i < listInfos.Count; i++)
                        {
                            var item = listInfos[i];
                            bool bIsAdded = false;

                            DataRow[] drs =
                                objDataSet.Tables[0].Select(string.Format("C002 = {0}", item.WidgetID));
                            if (drs.Length <= 0)
                            {
                                bIsAdded = true;
                                dr = objDataSet.Tables[0].NewRow();
                                dr["C001"] = userID;
                                dr["C002"] = item.WidgetID;
                            }
                            else
                            {
                                dr = drs[0];
                            }
                            dr["C003"] = item.IsCenter ? "1" : "0";
                            dr["C004"] = item.SortID;
                            if (bIsAdded)
                            {
                                objDataSet.Tables[0].Rows.Add(dr);

                                listReturns.Add(string.Format("A_{0}_{1}", userID, item.WidgetID));
                            }
                            else
                            {
                                listReturns.Add(string.Format("M_{0}_{1}", userID, item.WidgetID));
                            }
                        }
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            dr = objDataSet.Tables[0].Rows[i];

                            long id = Convert.ToInt64(dr["C002"]);
                            var temp = listInfos.FirstOrDefault(w => w.WidgetID == id);
                            if (temp == null)
                            {
                                dr.Delete();

                                listReturns.Add(string.Format("D_{0}_{1}", userID, id));
                            }
                        }

                        objAdapter.Update(objDataSet);
                        objDataSet.AcceptChanges();

                        optReturn.Data = listReturns;
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


        #region Others

        private DateTime GetDatetimeFromDB(string strDatetime)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                dt = Converter.NumberToDatetime(strDatetime);
            }
            catch { }
            return dt;
        }

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

    }
}
