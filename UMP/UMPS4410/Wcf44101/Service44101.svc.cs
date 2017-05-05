using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.ServiceModel.Activation;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common44101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;
using Wcf44101.Wcf11012;

namespace Wcf44101
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service44101”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service44101.svc 或 Service44101.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service44101 : IService44101
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
                    case (int)S4410Codes.GetRegionInfoList:
                        optReturn = GetRegionInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.GetUserRegionList:
                        optReturn = GetUserRegionList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.GetRegionUserList:
                        optReturn = GetRegionUserList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.GetSeatInfoList:
                        optReturn = GetSeatInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.GetRegionSeatList:
                        optReturn = GetRegionSeatList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.GetAgentStateList:
                        optReturn = GetAgentStateList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.GetAlarmMessageList:
                        optReturn = GetAlarmMessageList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.GetAlarmUserList:
                        optReturn = GetAlarmUserList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.SaveRegionInfo:
                        optReturn = SaveRegionInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.DeleteRegionInfo:
                        optReturn = DeleteRegionInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.SetRegionMmt:
                        optReturn = SetRegionMmt(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.SaveSeatInfo:
                        optReturn = SaveSeatInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.SaveRegionSeatInfo:
                        optReturn = SaveRegionSeatInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.SaveAgentStateInfo:
                        optReturn = SaveAgentStateInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.SaveAlarmMessage:
                        optReturn = SaveAlarmMessageInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4410Codes.SaveAlarmUser:
                        optReturn = SaveAlarmUserInfo(session, webRequest.ListData);
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


        #region GetDataOperations

        private OperationReturn GetRegionInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         ParentID
                //
                if (listParams == null
                   || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid");
                    return optReturn;
                }
                long parentID;
                if (!long.TryParse(strParentID, out parentID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ParentID invalid");
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
                                "SELECT * FROM T_44_101_{0} WHERE C004 = {1} ORDER BY C001,C002",
                                strRentToken,
                                parentID);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_44_101_{0} WHERE C004 = {1} ORDER BY C001,C002",
                               strRentToken,
                               parentID);
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

                    RegionInfo item = new RegionInfo();
                    item.ObjID = Convert.ToInt64(dr["C001"]);
                    item.Name = DecryptString02(dr["C002"].ToString());
                    item.Description = DecryptString02(dr["C003"].ToString());
                    item.ParentObjID = Convert.ToInt64(dr["C004"]);
                    item.Type = Convert.ToInt32(dr["C005"]);
                    item.State = Convert.ToInt32(dr["C006"]);
                    item.Width = GetIntNullValue(dr, "C007");
                    item.Height = GetIntNullValue(dr, "C008");
                    item.BgColor = DecryptString02(dr["C009"].ToString());
                    item.BgImage = DecryptString02(dr["C010"].ToString());
                    item.CreateID = GetLongNullValue(dr, "C011");
                    item.CreateTime = GetDatetimeNullValue(dr, "C012");
                    item.ModifierID = GetLongNullValue(dr, "C013");
                    item.ModifyDate = GetDatetimeNullValue(dr, "C014");
                    item.IsDefault = dr["C015"].ToString() == "1";
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetUserRegionList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
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
                    optReturn.Message = string.Format("UserID invalid");
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
                                "SELECT * FROM T_11_201_{0} WHERE C003 = {1} AND C004 > 4110000000000000000 AND C004 < 4120000000000000000 ORDER BY C004",
                                strRentToken,
                                userID);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                          string.Format(
                              "SELECT * FROM T_11_201_{0} WHERE C003 = {1} AND C004 > 4110000000000000000 AND C004 < 4120000000000000000 ORDER BY C004",
                              strRentToken,
                              userID);
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

                    long id = Convert.ToInt64(dr["C004"]);
                    listReturn.Add(id.ToString());
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

        private OperationReturn GetRegionUserList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         RegionID
                if (listParams == null
                   || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strRegionID = listParams[1];
                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid");
                    return optReturn;
                }
                long regionID;
                if (!long.TryParse(strRegionID, out regionID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RegionID invalid");
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
                                "SELECT * FROM T_11_201_{0} WHERE C004 = {1} AND C003 > 1020000000000000000 AND C003 < 1030000000000000000 ORDER BY C003",
                                strRentToken,
                                regionID);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_201_{0} WHERE C004 = {1} AND C003 > 1020000000000000000 AND C003 < 1030000000000000000 ORDER BY C003",
                                strRentToken,
                                regionID);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetSeatInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         Method（0：默认，获取Seat完整信息；1：只获取Seat基本信息，ID和名称）
                //
                if (listParams == null
                   || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid");
                    return optReturn;
                }
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid");
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
                                "SELECT * FROM T_44_102_{0} ORDER BY C001,C002",
                                strRentToken);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_44_102_{0} ORDER BY C001,C002",
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

                    SeatInfo item = new SeatInfo();
                    item.ObjID = Convert.ToInt64(dr["C001"]);
                    item.Name = DecryptString02(dr["C002"].ToString());
                    if (intMethod == 1)
                    {
                        //
                    }
                    if (intMethod == 0)
                    {
                        item.Description = dr["C003"].ToString();
                        item.State = Convert.ToInt32(dr["C004"]);
                        item.Level = Convert.ToInt32(dr["C005"]);
                        item.Creator = GetLongNullValue(dr, "C007");
                        item.CreateTime = GetDatetimeNullValue(dr, "C008");
                        item.Modifier = GetLongNullValue(dr, "C009");
                        item.ModifyTime = GetDatetimeNullValue(dr, "C010");
                        item.Extension = dr["C014"].ToString();
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetRegionSeatList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         RegionID
                //
                if (listParams == null
                   || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strRegionID = listParams[1];
                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid");
                    return optReturn;
                }
                long regionID;
                if (!long.TryParse(strRegionID, out regionID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RegionID invalid");
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
                                "SELECT * FROM T_44_104_{0} WHERE C001 = {1} ORDER BY C001,C002",
                                strRentToken, regionID);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_44_104_{0} WHERE C001 = {1} ORDER BY C001,C002",
                               strRentToken, regionID);
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

                    RegionSeatInfo item = new RegionSeatInfo();
                    item.RegionID = Convert.ToInt64(dr["C001"]);
                    item.SeatID = Convert.ToInt64(dr["C002"]);
                    item.Left = Convert.ToInt32(dr["C003"]);
                    item.Top = Convert.ToInt32(dr["C004"]);
                    item.Creator = GetLongNullValue(dr, "C005");
                    item.CreateTime = GetDatetimeNullValue(dr, "C006");
                    item.Modifier = GetLongNullValue(dr, "C007");
                    item.ModifyTime = GetDatetimeNullValue(dr, "C008");
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetAgentStateList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         Method（0：默认，获取AgentState完整信息；）
                //
                if (listParams == null
                   || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid");
                    return optReturn;
                }
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid");
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
                                "SELECT * FROM T_44_103_{0} ORDER BY C001,C002,C003",
                                strRentToken);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_44_103_{0} ORDER BY C001,C002,C003",
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

                    AgentStateInfo item = new AgentStateInfo();
                    item.ObjID = Convert.ToInt64(dr["C001"]);
                    item.Number = Convert.ToInt32(dr["C002"]);
                    item.Name = DecryptString02(dr["C003"].ToString());
                    item.Type = Convert.ToInt32(dr["C010"]);
                    if (intMethod == 0)
                    {
                        item.State = Convert.ToInt32(dr["C005"]);
                        item.Color = dr["C006"].ToString();
                        item.Icon = dr["C007"].ToString();
                        item.IsWorkTime = dr["C008"].ToString() == "1";
                        item.Value = Convert.ToInt32(dr["C009"]);
                        item.Creator = GetLongNullValue(dr, "C011");
                        item.CreateTime = GetDatetimeNullValue(dr, "C012");
                        item.Modifier = GetLongNullValue(dr, "C013");
                        item.ModifyTime = GetDatetimeNullValue(dr, "C014");
                        item.Description = dr["C004"].ToString();
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetAlarmMessageList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         Method（0：默认，获取AlarmMessage完整信息；）
                //
                if (listParams == null
                   || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid");
                    return optReturn;
                }
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid");
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
                                "SELECT * FROM T_44_105_{0} ORDER BY C001",
                                strRentToken);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                                "SELECT * FROM T_44_105_{0} ORDER BY C001",
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

                    AlarmMessageInfo item = new AlarmMessageInfo();
                    item.SerialID = Convert.ToInt64(dr["C001"]);
                    item.Name = DecryptString02(dr["C002"].ToString());
                    if (intMethod == 0)
                    {
                        item.Content = dr["C003"].ToString();
                        item.State = Convert.ToInt32(dr["C004"]);
                        item.Rank = Convert.ToInt32(dr["C005"]);
                        item.Value = dr["C006"].ToString();
                        item.Color = dr["C007"].ToString();
                        item.Icon = dr["C008"].ToString();
                        item.HoldTime = Convert.ToInt32(dr["C009"]);
                        item.Type = Convert.ToInt32(dr["C010"]);
                        item.StateID = Convert.ToInt64(dr["C011"]);
                        item.Creator = GetLongNullValue(dr, "C012");
                        item.CreateTime = GetDatetimeNullValue(dr, "C013");
                        item.Modifier = GetLongNullValue(dr, "C014");
                        item.ModifyTime = GetDatetimeNullValue(dr, "C015");
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetAlarmUserList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         AlarmID
                //
                if (listParams == null
                   || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strAlarmID = listParams[1];
                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid");
                    return optReturn;
                }
                long alarmID;
                if (!long.TryParse(strAlarmID, out alarmID)
                    || alarmID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AlarmID invalid");
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
                                "SELECT * FROM T_44_106_{0} WHERE C001 = {1} ORDER BY C001",
                                strRentToken, alarmID);
                        optReturn = MssqlOperation.GetDataSet(strConString, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_44_106_{0} WHERE C001 = {1} ORDER BY C001",
                               strRentToken, alarmID);
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

                    AlarmUserInfo item = new AlarmUserInfo();
                    item.AlarmID = Convert.ToInt64(dr["C001"]);
                    item.UserID = Convert.ToInt64(dr["C002"]);
                    item.UserType = Convert.ToInt32(dr["C003"]);
                    item.Creator = GetLongNullValue(dr, "C004");
                    item.CreateTime = GetDatetimeNullValue(dr, "C005");
                    item.Modifier = GetLongNullValue(dr, "C006");
                    item.ModifyTime = GetDatetimeNullValue(dr, "C007");
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        #endregion


        #region SaveDataOperations

        private OperationReturn SaveRegionInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         RegionID（为0，表示新增区域，否则修改区域信息）
                //2         RegionInfo
                //3         是否拷贝背景图片，由MediaData拷贝到UploadFiles/UMPS4412下，并重命名
                //
                if (listParams == null
                   || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strRegionID = listParams[1];
                string strRegionInfo = listParams[2];
                string strIsCopy = listParams[3];
                bool isCopy = strIsCopy == "1";
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID param invalid");
                    return optReturn;
                }
                long regionID;
                if (!long.TryParse(strRegionID, out regionID)
                   || regionID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RegionID param invalid");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<RegionInfo>(strRegionInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                RegionInfo regionInfo = optReturn.Data as RegionInfo;
                if (regionInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("RegionInfo is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                bool isAdd = false;
                if (regionID == 0)
                {
                    isAdd = true;


                    #region 获取流水号

                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add(S4410Consts.MODULE_MASTER_ASM.ToString());
                    webRequest.ListData.Add(S4410Consts.RESOURCE_REGION.ToString());
                    webRequest.ListData.Add(DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
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
                    regionID = Convert.ToInt64(webReturn.Data);
                    regionInfo.ObjID = regionID;

                    #endregion


                }
                listReturn.Add(regionID.ToString());


                #region 将背景图片拷贝到UploadFiles/UMPS4412下，并重命名

                if (isCopy)
                {
                    string strSourceFile = regionInfo.BgImage;
                    if (!string.IsNullOrEmpty(strSourceFile))
                    {
                        string strExt = strSourceFile.Substring(strSourceFile.LastIndexOf("."));
                        string strRootDir = AppDomain.CurrentDomain.BaseDirectory;
                        strRootDir = strRootDir.Substring(0, strRootDir.LastIndexOf("\\"));
                        strRootDir = strRootDir.Substring(0, strRootDir.LastIndexOf("\\"));
                        string strTargetDir = Path.Combine(strRootDir, ConstValue.TEMP_DIR_UPLOADFILES);
                        strTargetDir = Path.Combine(strTargetDir, string.Format("UMPS4412"));
                        if (!Directory.Exists(strTargetDir))
                        {
                            Directory.CreateDirectory(strTargetDir);
                        }
                        string strTargetFile = string.Format("RBG{0}{1}", regionID, strExt);
                        string strTarget = Path.Combine(strTargetDir, strTargetFile);
                        string strSource = Path.Combine(strRootDir, ConstValue.TEMP_DIR_MEDIADATA);
                        strSource = Path.Combine(strSource, strSourceFile);
                        try
                        {
                            File.Copy(strSource, strTarget, true);
                        }
                        catch (Exception ex)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_FAIL;
                            optReturn.Message = string.Format("Copy file fail.\t{0}", ex.Message);
                            return optReturn;
                        }
                        regionInfo.BgImage = strTargetFile;
                    }
                }

                #endregion


                #region 区域信息写入数据库

                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_44_101_{0} WHERE C001 = {1}", rentToken,
                              regionInfo.ObjID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_44_101_{0} WHERE C001 = {1}", rentToken,
                              regionInfo.ObjID);
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

                    if (isAdd
                        && objDataSet.Tables[0].Rows.Count > 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_DBACCESS_EXIST;
                        optReturn.Message = string.Format("Region already exist");
                        return optReturn;
                    }
                    if (!isAdd
                        && objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_DBACCESS_NOT_EXIST;
                        optReturn.Message = string.Format("Region not exist");
                        return optReturn;
                    }

                    DataRow dr;
                    if (isAdd)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = regionInfo.ObjID;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C002"] = EncryptString02(regionInfo.Name);
                    dr["C003"] = EncryptString02(regionInfo.Description);
                    dr["C004"] = regionInfo.ParentObjID;
                    dr["C005"] = regionInfo.Type;
                    dr["C006"] = regionInfo.State;
                    dr["C007"] = regionInfo.Width;
                    dr["C008"] = regionInfo.Height;
                    dr["C009"] = regionInfo.BgColor;
                    dr["C010"] = regionInfo.BgImage;
                    DateTime now = DateTime.Now;
                    if (isAdd)
                    {
                        dr["C011"] = userID;
                        dr["C012"] = Convert.ToInt64(now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
                        dr["C013"] = userID;
                        dr["C014"] = Convert.ToInt64(now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
                    }
                    else
                    {
                        dr["C013"] = userID;
                        dr["C014"] = Convert.ToInt64(now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
                    }
                    dr["C015"] = regionInfo.IsDefault ? "1" : "0";

                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);

                        listReturn.Add(string.Format("A{0}", regionInfo.ObjID));
                    }
                    else
                    {
                        listReturn.Add(string.Format("M{0}", regionInfo.ObjID));
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();


                    #region 设置默认管理权限

                    if (isAdd)
                    {
                        switch (session.DBType)
                        {
                            case 2:
                                strSql =
                                    string.Format(
                                        "INSERT INTO T_11_201_{0} (C001,C002,C003,C004,C005,C006) VALUES(0,0,{1},{2},'2014/1/1','2199/12/31')",
                                        rentToken, userID, regionInfo.ObjID);
                                optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                                break;
                            case 3:
                                strSql =
                                   string.Format(
                                       "INSERT INTO T_11_201_{0} (C001,C002,C003,C004,C005,C006) VALUES(0,0,{1},{2},TO_DATE('2014/1/1','YYYY-MM-DD HH:MI:SS'),TO_DATE('2199/12/31','YYYY-MM-DD HH:MI:SS'))",
                                       rentToken, userID, regionInfo.ObjID);
                                optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                                return optReturn;
                        }
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                    }

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

        private OperationReturn DeleteRegionInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         RegionID
                //
                if (listParams == null
                   || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strRegionID = listParams[1];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID param invalid");
                    return optReturn;
                }
                long regionID;
                if (!long.TryParse(strRegionID, out regionID)
                   || regionID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RegionID param invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("DELETE FROM T_44_101_{0} WHERE C001 = {1}", rentToken, regionID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        strSql = string.Format("DELETE FROM T_11_201_{0} WHERE C004= {1}", rentToken, regionID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("DELETE FROM T_44_101_{0} WHERE C001 = {1}", rentToken, regionID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        strSql = string.Format("DELETE FROM T_11_201_{0} WHERE C004= {1}", rentToken, regionID);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn SetRegionMmt(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         RegionID
                //2         count
                //3         UserID and Check state list, Splite by ';'
                //
                if (listParams == null
                   || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strRegionID = listParams[1];
                string strCount = listParams[2];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID param invalid");
                    return optReturn;
                }
                long regionID;
                if (!long.TryParse(strRegionID, out regionID)
                   || regionID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RegionID param invalid");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID count invalid");
                    return optReturn;
                }
                List<string> listUserIDStates = new List<string>();
                for (int i = 0; i < intCount; i++)
                {
                    string strIDState = listParams[i + 3];
                    listUserIDStates.Add(strIDState);
                }
                int num = listUserIDStates.Count;
                string strRentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_201_{0} WHERE C004 = {1} AND C003 > 1020000000000000000 AND C003 < 1030000000000000000",
                                strRentToken, regionID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_201_{0} WHERE C004 = {1} AND C003 > 1020000000000000000 AND C003 < 1030000000000000000",
                                strRentToken, regionID);
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DatabaseType invalid");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    DataRow dr;
                    for (int i = 0; i < num; i++)
                    {
                        string strIDState = listUserIDStates[i];
                        string[] arrIDState = strIDState.Split(new[] { ';' }, StringSplitOptions.None);
                        string strID = string.Empty;
                        string strState = string.Empty;
                        if (arrIDState.Length > 0)
                        {
                            strID = arrIDState[0];
                        }
                        if (arrIDState.Length > 1)
                        {
                            strState = arrIDState[1];
                        }
                        if (string.IsNullOrEmpty(strID)
                            || string.IsNullOrEmpty(strState)) { continue; }
                        long id;
                        bool isChecked = strState == "1";
                        if (!long.TryParse(strID, out id)) { continue; }
                        if (isChecked)
                        {
                            DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C003 = {0}", id));
                            if (drs.Length <= 0)
                            {
                                dr = objDataSet.Tables[0].NewRow();
                                dr["C001"] = 0;
                                dr["C002"] = 0;
                                dr["C003"] = id;
                                dr["C004"] = regionID;
                                dr["C005"] = DateTime.Parse("2014/1/1");
                                dr["C006"] = DateTime.Parse("2199/12/31");
                                objDataSet.Tables[0].Rows.Add(dr);

                                listReturn.Add(string.Format("A{0}", id));
                            }
                        }
                        else
                        {
                            DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C003 = {0}", id));
                            if (drs.Length > 0)
                            {
                                foreach (var row in drs)
                                {
                                    row.Delete();

                                    listReturn.Add(string.Format("D{0}", id));
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn SaveSeatInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         Method（模式：0，默认模式，即存在则修改【M】，不存在则新增【A】；1，存在则忽略【I】，不存在则新增【A】；2：存在则删除【D】，不存在则忽略【I】）
                //2         count（数量）
                //3...      SeatInfo（ObjID为0表示新增）
                //
                if (listParams == null
                   || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string strCount = listParams[2];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID param invalid");
                    return optReturn;
                }
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method param invalid");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Seat count invalid");
                    return optReturn;
                }
                List<SeatInfo> listSeatInfos = new List<SeatInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 3];
                    optReturn = XMLHelper.DeserializeObject<SeatInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    SeatInfo info = optReturn.Data as SeatInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("SeatInfo is null");
                        return optReturn;
                    }
                    listSeatInfos.Add(info);
                }
                for (int i = 0; i < intCount; i++)
                {
                    SeatInfo seatInfo = listSeatInfos[i];
                    long objID = seatInfo.ObjID;
                    if (objID <= 0)
                    {
                        //获取流水号
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = session;
                        webRequest.Code = (int)RequestCode.WSGetSerialID;
                        webRequest.ListData.Add(S4410Consts.MODULE_MASTER_ASM.ToString());
                        webRequest.ListData.Add(S4410Consts.RESOURCE_SEAT.ToString());
                        webRequest.ListData.Add(DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
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
                        seatInfo.ObjID = Convert.ToInt64(webReturn.Data);
                    }
                }
                List<string> listReturn = new List<string>();

                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_44_102_{0} ORDER BY C001,C002", rentToken);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_44_102_{0} ORDER BY C001,C002", rentToken);
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

                    for (int i = 0; i < intCount; i++)
                    {
                        SeatInfo seatInfo = listSeatInfos[i];
                        bool isAdd = false;

                        long objID = seatInfo.ObjID;
                        DataRow dr = objDataSet.Tables[0].Select(string.Format("C001 = {0}", objID)).FirstOrDefault();
                        if (dr == null)
                        {
                            if (intMethod == 0
                                || intMethod == 1)
                            {
                                //不存在，新增
                                dr = objDataSet.Tables[0].NewRow();
                                isAdd = true;
                                dr["C001"] = objID;
                            }
                            else
                            {
                                //不存在，忽略
                                listReturn.Add(string.Format("I{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, seatInfo.ObjID,
                                   seatInfo.Name));
                                continue;
                            }
                        }
                        else
                        {
                            if (intMethod == 1)
                            {
                                //存在，忽略
                                listReturn.Add(string.Format("I{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, seatInfo.ObjID,
                                  seatInfo.Name));
                                continue;
                            }
                            if (intMethod == 2)
                            {
                                //存在，删除
                                dr.Delete();
                                listReturn.Add(string.Format("D{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, seatInfo.ObjID,
                                    seatInfo.Name));
                                continue;
                            }
                        }

                        dr["C002"] = EncryptString02(seatInfo.Name);
                        dr["C003"] = seatInfo.Description;
                        dr["C004"] = seatInfo.State;
                        dr["C005"] = seatInfo.Level;
                        dr["C006"] = 0;
                        dr["C007"] = seatInfo.Creator;
                        dr["C008"] = seatInfo.CreateTime.ToString("yyyyMMddHHmmss");
                        dr["C009"] = seatInfo.Modifier;
                        dr["C010"] = seatInfo.ModifyTime.ToString("yyyyMMddHHmmss");
                        dr["C014"] = seatInfo.Extension;

                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            listReturn.Add(string.Format("A{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, seatInfo.ObjID,
                                seatInfo.Name));
                        }
                        else
                        {
                            listReturn.Add(string.Format("M{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, seatInfo.ObjID,
                               seatInfo.Name));
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn SaveRegionSeatInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         Method（0：默认方式，存在则修改，不存在则新增；1：存在则修改，不存在则新增，并且列表中不存在的删除）
                //2         RegionID
                //3         count（数量）
                //4...      RegionSeatInfo
                //
                if (listParams == null
                   || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string strRegionID = listParams[2];
                string strCount = listParams[3];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID param invalid");
                    return optReturn;
                }
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method param invalid");
                    return optReturn;
                }
                long regionID;
                if (!long.TryParse(strRegionID, out regionID)
                    || regionID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RegionID param invalid");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RegionSeat count invalid");
                    return optReturn;
                }
                List<RegionSeatInfo> listRegionSeatInfos = new List<RegionSeatInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 4];
                    optReturn = XMLHelper.DeserializeObject<RegionSeatInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    RegionSeatInfo info = optReturn.Data as RegionSeatInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("RegionSeat is null");
                        return optReturn;
                    }
                    listRegionSeatInfos.Add(info);
                }

                List<string> listReturn = new List<string>();

                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_44_104_{0} WHERE C001 = {1} ORDER BY C001,C002", rentToken, regionID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_44_104_{0} WHERE C001 = {1} ORDER BY C001,C002", rentToken, regionID);
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

                    for (int i = 0; i < intCount; i++)
                    {
                        RegionSeatInfo regionSeatInfo = listRegionSeatInfos[i];
                        bool isAdd = false;
                        long seatID = regionSeatInfo.SeatID;

                        DataRow dr =
                            objDataSet.Tables[0].Select(string.Format("C002 = {0}", seatID))
                                .FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            isAdd = true;
                            dr["C001"] = regionSeatInfo.RegionID;
                            dr["C002"] = regionSeatInfo.SeatID;
                        }
                        dr["C003"] = regionSeatInfo.Left;
                        dr["C004"] = regionSeatInfo.Top;
                        dr["C005"] = regionSeatInfo.Creator;
                        dr["C006"] = regionSeatInfo.CreateTime.ToString("yyyyMMddHHmmss");
                        dr["C007"] = regionSeatInfo.Modifier;
                        dr["C008"] = regionSeatInfo.ModifyTime.ToString("yyyyMMddHHmmss");

                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);

                            listReturn.Add(string.Format("A{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, regionID, seatID));
                        }
                        else
                        {
                            listReturn.Add(string.Format("M{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, regionID, seatID));
                        }
                    }
                    if (intMethod == 1)
                    {
                        //列表中不存在的要删除
                        for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[i];
                            long seatID = Convert.ToInt64(dr["C002"]);
                            var temp = listRegionSeatInfos.FirstOrDefault(s => s.SeatID == seatID);
                            if (temp == null)
                            {
                                dr.Delete();

                                listReturn.Add(string.Format("D{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, regionID, seatID));
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn SaveAgentStateInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         Method（模式：0，默认模式，即存在则修改【M】，不存在则新增【A】；1，存在则忽略【I】，不存在则新增【A】；2：存在则删除【D】，不存在则忽略【I】）
                //2         是否拷贝图标文件，由MediaData临时目录拷贝到 UploadFiles/UMPS4414 下
                //3         count（数量）
                //4...      SeatInfo（ObjID为0表示新增）
                //
                if (listParams == null
                   || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string strIsCopy = listParams[2];
                string strCount = listParams[3];
                bool isCopy = strIsCopy == "1";
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID param invalid");
                    return optReturn;
                }
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method param invalid");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Seat count invalid");
                    return optReturn;
                }
                List<AgentStateInfo> listAgentStateInfos = new List<AgentStateInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 4];
                    optReturn = XMLHelper.DeserializeObject<AgentStateInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    AgentStateInfo info = optReturn.Data as AgentStateInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("AgentStateInfo is null");
                        return optReturn;
                    }
                    listAgentStateInfos.Add(info);
                }
                for (int i = 0; i < intCount; i++)
                {
                    AgentStateInfo agentStateInfo = listAgentStateInfos[i];
                    long objID = agentStateInfo.ObjID;
                    if (objID <= 0)
                    {
                        //获取流水号
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = session;
                        webRequest.Code = (int)RequestCode.WSGetSerialID;
                        webRequest.ListData.Add(S4410Consts.MODULE_MASTER_ASM.ToString());
                        webRequest.ListData.Add(S4410Consts.RESOURCE_AGENTSTATE.ToString());
                        webRequest.ListData.Add(DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
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
                        agentStateInfo.ObjID = Convert.ToInt64(webReturn.Data);
                    }

                    #region 将图标文件拷贝到UploadFiles/UMPS4414下，并重命名

                    if (isCopy)
                    {
                        string strSourceFile = agentStateInfo.Icon;
                        if (!string.IsNullOrEmpty(strSourceFile))
                        {
                            string strExt = strSourceFile.Substring(strSourceFile.LastIndexOf("."));
                            string strRootDir = AppDomain.CurrentDomain.BaseDirectory;
                            strRootDir = strRootDir.Substring(0, strRootDir.LastIndexOf("\\"));
                            strRootDir = strRootDir.Substring(0, strRootDir.LastIndexOf("\\"));
                            string strTargetDir = Path.Combine(strRootDir, ConstValue.TEMP_DIR_UPLOADFILES);
                            strTargetDir = Path.Combine(strTargetDir, string.Format("UMPS4414"));
                            if (!Directory.Exists(strTargetDir))
                            {
                                Directory.CreateDirectory(strTargetDir);
                            }
                            string strTargetFile = string.Format("ASI{0}{1}", agentStateInfo.ObjID, strExt);
                            string strTarget = Path.Combine(strTargetDir, strTargetFile);
                            string strSource = Path.Combine(strRootDir, ConstValue.TEMP_DIR_MEDIADATA);
                            strSource = Path.Combine(strSource, strSourceFile);
                            try
                            {
                                File.Copy(strSource, strTarget, true);
                            }
                            catch (Exception ex)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_FAIL;
                                optReturn.Message = string.Format("Copy file fail.\t{0}", ex.Message);
                                return optReturn;
                            }
                            agentStateInfo.Icon = strTargetFile;
                        }
                    }

                    #endregion

                }
                List<string> listReturn = new List<string>();

                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_44_103_{0} ORDER BY C001,C002,C003", rentToken);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_44_103_{0} ORDER BY C001,C002,C003", rentToken);
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

                    for (int i = 0; i < intCount; i++)
                    {
                        AgentStateInfo agentSeatInfo = listAgentStateInfos[i];
                        bool isAdd = false;

                        long objID = agentSeatInfo.ObjID;
                        DataRow dr = objDataSet.Tables[0].Select(string.Format("C001 = {0}", objID)).FirstOrDefault();
                        if (dr == null)
                        {
                            if (intMethod == 0
                                || intMethod == 1)
                            {
                                //不存在，新增
                                dr = objDataSet.Tables[0].NewRow();
                                isAdd = true;
                                dr["C001"] = objID;
                            }
                            else
                            {
                                //不存在，忽略
                                listReturn.Add(string.Format("I{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, agentSeatInfo.ObjID,
                                   agentSeatInfo.Name));
                                continue;
                            }
                        }
                        else
                        {
                            if (intMethod == 1)
                            {
                                //存在，忽略
                                listReturn.Add(string.Format("I{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, agentSeatInfo.ObjID,
                                  agentSeatInfo.Name));
                                continue;
                            }
                            if (intMethod == 2)
                            {
                                //存在，删除
                                dr.Delete();
                                listReturn.Add(string.Format("D{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, agentSeatInfo.ObjID,
                                    agentSeatInfo.Name));
                                continue;
                            }
                        }

                        dr["C002"] = agentSeatInfo.Number;
                        dr["C003"] = EncryptString02(agentSeatInfo.Name);
                        dr["C004"] = agentSeatInfo.Description;
                        dr["C005"] = agentSeatInfo.State;
                        dr["C006"] = agentSeatInfo.Color;
                        dr["C007"] = agentSeatInfo.Icon;
                        dr["C008"] = agentSeatInfo.IsWorkTime ? "1" : "0";
                        dr["C009"] = agentSeatInfo.Value;
                        dr["C010"] = agentSeatInfo.Type;
                        dr["C011"] = agentSeatInfo.Creator;
                        dr["C012"] = agentSeatInfo.CreateTime.ToString("yyyyMMddHHmmss");
                        dr["C013"] = agentSeatInfo.Modifier;
                        dr["C014"] = agentSeatInfo.ModifyTime.ToString("yyyyMMddHHmmss");

                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            listReturn.Add(string.Format("A{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, agentSeatInfo.ObjID,
                                agentSeatInfo.Name));
                        }
                        else
                        {
                            listReturn.Add(string.Format("M{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, agentSeatInfo.ObjID,
                               agentSeatInfo.Name));
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn SaveAlarmMessageInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         Method（模式：0，默认模式，即存在则修改【M】，不存在则新增【A】；1，存在则忽略【I】，不存在则新增【A】；2：存在则删除【D】，不存在则忽略【I】）
                //2         是否拷贝图标文件，由MediaData临时目录拷贝到 UploadFiles/UMPS4415下
                //3         count（数量）
                //4...      AlarmMessageInfo（SerialID为0表示新增）
                //
                if (listParams == null
                   || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string strIsCopy = listParams[2];
                bool isCopy = strIsCopy == "1";
                string strCount = listParams[3];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID param invalid");
                    return optReturn;
                }
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method param invalid");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AlarmMessage count invalid");
                    return optReturn;
                }
                List<AlarmMessageInfo> listAlarmInfos = new List<AlarmMessageInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 4];
                    optReturn = XMLHelper.DeserializeObject<AlarmMessageInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    AlarmMessageInfo info = optReturn.Data as AlarmMessageInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("AlarmMessageInfo is null");
                        return optReturn;
                    }
                    listAlarmInfos.Add(info);
                }
                for (int i = 0; i < intCount; i++)
                {
                    AlarmMessageInfo alarm = listAlarmInfos[i];
                    long objID = alarm.SerialID;
                    if (objID <= 0)
                    {
                        //获取流水号
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = session;
                        webRequest.Code = (int)RequestCode.WSGetSerialID;
                        webRequest.ListData.Add(S4410Consts.MODULE_MASTER_ASM.ToString());
                        webRequest.ListData.Add(S4410Consts.RESOURCE_ALARMMSG.ToString());
                        webRequest.ListData.Add(DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
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
                        alarm.SerialID = Convert.ToInt64(webReturn.Data);
                    }

                    #region 将图标文件拷贝到UploadFiles/UMPS4415下，并重命名

                    if (isCopy)
                    {
                        string strSourceFile = alarm.Icon;
                        if (!string.IsNullOrEmpty(strSourceFile))
                        {
                            string strExt = strSourceFile.Substring(strSourceFile.LastIndexOf("."));
                            string strRootDir = AppDomain.CurrentDomain.BaseDirectory;
                            strRootDir = strRootDir.Substring(0, strRootDir.LastIndexOf("\\"));
                            strRootDir = strRootDir.Substring(0, strRootDir.LastIndexOf("\\"));
                            string strTargetDir = Path.Combine(strRootDir, ConstValue.TEMP_DIR_UPLOADFILES);
                            strTargetDir = Path.Combine(strTargetDir, string.Format("UMPS4415"));
                            if (!Directory.Exists(strTargetDir))
                            {
                                Directory.CreateDirectory(strTargetDir);
                            }
                            string strTargetFile = string.Format("AMI{0}{1}", alarm.SerialID, strExt);
                            string strTarget = Path.Combine(strTargetDir, strTargetFile);
                            string strSource = Path.Combine(strRootDir, ConstValue.TEMP_DIR_MEDIADATA);
                            strSource = Path.Combine(strSource, strSourceFile);
                            try
                            {
                                File.Copy(strSource, strTarget, true);
                            }
                            catch (Exception ex)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_FAIL;
                                optReturn.Message = string.Format("Copy file fail.\t{0}", ex.Message);
                                return optReturn;
                            }
                            alarm.Icon = strTargetFile;
                        }
                    }

                    #endregion
                }
                List<string> listReturn = new List<string>();

                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_44_105_{0} ORDER BY C001", rentToken);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_44_105_{0} ORDER BY C001", rentToken);
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

                    for (int i = 0; i < intCount; i++)
                    {
                        AlarmMessageInfo alarmInfo = listAlarmInfos[i];
                        bool isAdd = false;

                        long objID = alarmInfo.SerialID;
                        DataRow dr = objDataSet.Tables[0].Select(string.Format("C001 = {0}", objID)).FirstOrDefault();
                        if (dr == null)
                        {
                            if (intMethod == 0
                                || intMethod == 1)
                            {
                                //不存在，新增
                                dr = objDataSet.Tables[0].NewRow();
                                isAdd = true;
                                dr["C001"] = objID;
                            }
                            else
                            {
                                //不存在，忽略
                                listReturn.Add(string.Format("I{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, alarmInfo.SerialID,
                                   alarmInfo.Name));
                                continue;
                            }
                        }
                        else
                        {
                            if (intMethod == 1)
                            {
                                //存在，忽略
                                listReturn.Add(string.Format("I{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, alarmInfo.SerialID,
                                  alarmInfo.Name));
                                continue;
                            }
                            if (intMethod == 2)
                            {
                                //存在，删除
                                dr.Delete();
                                listReturn.Add(string.Format("D{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, alarmInfo.SerialID,
                                    alarmInfo.Name));
                                continue;
                            }
                        }

                        dr["C002"] = EncryptString02(alarmInfo.Name);
                        dr["C003"] = alarmInfo.Content;
                        dr["C004"] = alarmInfo.State;
                        dr["C005"] = alarmInfo.Rank;
                        dr["C006"] = alarmInfo.Value;
                        dr["C007"] = alarmInfo.Color;
                        dr["C008"] = alarmInfo.Icon;
                        dr["C009"] = alarmInfo.HoldTime;
                        dr["C010"] = alarmInfo.Type;
                        dr["C011"] = alarmInfo.StateID;
                        dr["C012"] = alarmInfo.Creator;
                        dr["C013"] = alarmInfo.CreateTime.ToString("yyyyMMddHHmmss");
                        dr["C014"] = alarmInfo.Modifier;
                        dr["C015"] = alarmInfo.ModifyTime.ToString("yyyyMMddHHmmss");

                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            listReturn.Add(string.Format("A{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, alarmInfo.SerialID,
                                alarmInfo.Name));
                        }
                        else
                        {
                            listReturn.Add(string.Format("M{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, alarmInfo.SerialID,
                               alarmInfo.Name));
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn SaveAlarmUserInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0         UserID
                //1         AlarmID
                //2         count（数量）
                //3...      AlarmUserInfo
                //
                if (listParams == null
                   || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strAlarmID = listParams[1];
                string strCount = listParams[2];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID param invalid");
                    return optReturn;
                }
                long alarmID;
                if (!long.TryParse(strAlarmID, out alarmID)
                    || alarmID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AlarmID param invalid");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AlarmUser count invalid");
                    return optReturn;
                }
                List<AlarmUserInfo> listAlarmUsers = new List<AlarmUserInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 3];
                    optReturn = XMLHelper.DeserializeObject<AlarmUserInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    AlarmUserInfo info = optReturn.Data as AlarmUserInfo;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("AlarmUserInfo is null");
                        return optReturn;
                    }
                    listAlarmUsers.Add(info);
                }

                List<string> listReturn = new List<string>();

                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_44_106_{0} WHERE C001 = {1} ORDER BY C001,C002",
                            rentToken, alarmID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_44_106_{0} WHERE C001 = {1} ORDER BY C001,C002",
                           rentToken, alarmID);
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

                    for (int i = 0; i < intCount; i++)
                    {
                        AlarmUserInfo alarmUser = listAlarmUsers[i];
                        bool isAdd = false;

                        long objID = alarmUser.UserID;
                        DataRow dr = objDataSet.Tables[0].Select(string.Format("C002 = {0}", objID)).FirstOrDefault();
                        if (dr == null)
                        {
                            //不存在，新增
                            dr = objDataSet.Tables[0].NewRow();
                            isAdd = true;
                            dr["C001"] = alarmID;
                            dr["C002"] = objID;
                        }

                        dr["C003"] = alarmUser.UserType;
                        dr["C004"] = alarmUser.Creator;
                        dr["C005"] = alarmUser.CreateTime.ToString("yyyyMMddHHmmss");
                        dr["C006"] = alarmUser.Modifier;
                        dr["C007"] = alarmUser.ModifyTime.ToString("yyyyMMddHHmmss");

                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            listReturn.Add(string.Format("A{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, alarmID,
                                alarmUser.UserID));
                        }
                        else
                        {
                            listReturn.Add(string.Format("M{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, alarmID,
                                alarmUser.UserID));
                        }
                    }
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];

                        long objID = Convert.ToInt64(dr["C002"]);
                        var alarmUser = listAlarmUsers.FirstOrDefault(a => a.UserID == objID);
                        if (alarmUser == null)
                        {
                            dr.Delete();
                            listReturn.Add(string.Format("D{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, alarmID,
                               objID));
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        #endregion


        #region Others

        private int GetIntNullValue(DataRow dr, string field)
        {
            int intReturn;
            string strValue = dr[field].ToString();
            if (int.TryParse(strValue, out intReturn))
            {

            }
            return intReturn;
        }

        private long GetLongNullValue(DataRow dr, string field)
        {
            long longReturn;
            string strValue = dr[field].ToString();
            if (long.TryParse(strValue, out longReturn))
            {

            }
            return longReturn;
        }

        private DateTime GetDatetimeNullValue(DataRow dr, string field)
        {
            DateTime dtReturn = DateTime.MinValue;
            string strValue = dr[field].ToString();
            try
            {
                dtReturn = Converter.NumberToDatetime(strValue);
            }
            catch { }
            return dtReturn;
        }

        #endregion


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
