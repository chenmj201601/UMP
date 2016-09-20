
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using Common5100;
using System.Data;
using VoiceCyber.UMP.Encryptions;
using Wcf51001.Service11012;
using System.Xml;
using System.IO;

namespace Wcf51001
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service51001 : IService510011
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
                    case (int)S5100RequestCode.AddBookmarkLevel:
                        optReturn = AddBookmarkLevel(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S5100RequestCode.GetAllBookmarkLevels:
                        optReturn = GetAllBookmarkLevels(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S5100RequestCode.GetAllKeyWorlds:
                        optReturn = GetAllKeyWorlds(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S5100RequestCode.AddKeyWorld:
                        optReturn = AddKeyWorld(session, webRequest.ListData);
                        webReturn.Message = optReturn.Message;
                        if (!optReturn.Result)
                        {
                            if (optReturn.Code > (int)S5100WcfErrorCode.GenerateKeyWorldXmlException)
                            {
                                webReturn.Data = optReturn.Data as string;
                            }
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S5100RequestCode.ModifyKeyWorld:
                        optReturn = ModifyKeyWorld(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            if (optReturn.Code > (int)S5100WcfErrorCode.GenerateKeyWorldXmlException)
                            {
                                webReturn.Data = optReturn.Data as string;
                            }
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S5100RequestCode.DeleteKeyWorld:
                        optReturn = DeleteKeyWorld(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S5100RequestCode.ModifyBookmarkLevel:
                        optReturn = ModifyBookmarkLevel(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S5100RequestCode.SetBookmarkLevelStatus:
                        optReturn = SetLevelStatus(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S5100RequestCode.DeleteBookmarkLevel:
                          optReturn = DeleteBookmarkLevel(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                }
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

        /// <summary>
        /// 添加标签等级
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 等级名称
        /// lstParams[1] : 等级颜色
        /// <returns></returns>
        private OperationReturn AddBookmarkLevel(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                //ListParam
                //0      用户编号
                if (lstParams == null || lstParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                List<string> lst = new List<string>();
                lst.Add("51");
                lst.Add("309");
                optReturn = GetSerialID(session, lst);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string str = optReturn.Data as string;
                long levelID = 0;
                long.TryParse(str, out levelID);
                if (levelID == 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S5100WcfErrorCode.SerialIDConvertError;
                    optReturn.Message = str;
                    return optReturn;
                }

                switch (session.DBType)
                {
                    case 2:
                       strSql = "insert into T_11_009_{0}(C000,C001,C002,C003,C004,C005,C006,C007,C008,C009,C010)"
                                    + "values (3,{1},0,0,'1',0,'{2}','{3}','BM','{4}','0')";
                       strSql = string.Format(strSql, rentToken, levelID, S5100EncryptOperation.EncryptWithM002(lstParams[1]), session.UserID, lstParams[0]);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                          strSql = "insert into T_11_009_{0}(C000,C001,C002,C003,C004,C005,C006,C007,C008,C009,C010)"
                                    + "values (3,{1},0,0,'1',0,'{2}','{3}','BM','{4}',0)";
                          strSql = string.Format(strSql, rentToken, levelID, S5100EncryptOperation.EncryptWithM002(lstParams[1]), session.UserID, lstParams[0]);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.AddBookmarkLevelError;
                    return optReturn;
                }
                optReturn.Data = levelID.ToString();
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
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : bookmarkLevelID
        /// lstParams[1] : Level color
        /// <returns></returns>
        private OperationReturn ModifyBookmarkLevel(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            if (lstParams == null || lstParams.Count < 2)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_PARAM_INVALID;
                optReturn.Message = string.Format("Request param is null or count invalid");
                return optReturn;
            }

            try
            {
                string strToken = session.RentInfo.Token;
                string strLevelID = lstParams[0];
                string strColor = lstParams[1];
                string strSql = string.Empty;

                switch (session.DBType)
                {
                    case 2:
                        strSql = "update T_11_009_{0} set C006 = '{1}' where C001 = {2} and C000 = 3";
                        strSql = string.Format(strSql, strToken, strColor, strLevelID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "update T_11_009_{0} set C006 = '{1}' where C001 = {2} and C000 = 3";
                        strSql = string.Format(strSql, strToken, strColor, strLevelID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message +=" ; "+ strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.ModifyBookmarkLevelException;
                }
            }
            catch (Exception ex)
            {
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Result = false;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 启用、禁用标签等级
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : LevelID
        /// lstParams[1] : 状态--0：禁用         1：启用
        /// <returns></returns>
        private OperationReturn SetLevelStatus(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            if (lstParams.Count < 2)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = string.Format("Request param is null or count invalid");
                return optReturn;
            }
            try
            {
                string strToken = session.RentInfo.Token;
                string strLevelID = lstParams[0];
                string strStatus = lstParams[1];
                string strSql = string.Empty;

                switch (session.DBType)
                {
                    case 2:
                        strSql = "update T_11_009_{0} set C004 = '{1}' where C001 = {2} and C000 = 3";
                        strSql = string.Format(strSql, strToken, strStatus, strLevelID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "update T_11_009_{0} set C004 = '{1}' where C001 = {2} and C000 = 3";
                        strSql = string.Format(strSql, strToken, strStatus, strLevelID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += " ; " + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.ModifyBookmarkLevelStatusException;
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

        /// <summary>
        /// 删除标签等级
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : LevelID
        /// <returns></returns>
        private OperationReturn DeleteBookmarkLevel(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            if (lstParams.Count < 1)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = string.Format("Request param is null or count invalid");
                return optReturn;
            }
            try
            {
                string strToken = session.RentInfo.Token;
                string strLevelID = lstParams[0];
                string strSql = string.Empty;

                switch (session.DBType)
                {
                    case 2:
                        strSql = "update T_11_009_{0} set C010 = '1' where C001 = {1} and C000 = 3";
                        strSql = string.Format(strSql, strToken,  strLevelID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "update T_11_009_{0} set C010 = '1' where C001 = {1} and C000 = 3";
                        strSql = string.Format(strSql, strToken, strLevelID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += " ; " + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.ModifyBookmarkLevelStatusException;
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

        /// <summary>
        /// 获得流水号
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 模块编号
        /// lstParams[1] : 模块内编号
        /// <returns></returns>
        private OperationReturn GetSerialID(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = session;
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.ListData.Add(lstParams[0]);
                webRequest.ListData.Add(lstParams[1]);
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(session),
                    WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                optReturn.Data = webReturn.Data;
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = (int)S5100WcfErrorCode.GetSerialIDError;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        /// <summary>
        /// 获得所有标签等级
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// <returns></returns>
        private OperationReturn GetAllBookmarkLevels(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("select * from T_11_009_{0} where C000 = 3  and C010 = '0'", strToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("select * from T_11_009_{0} where C000 = 3 and C010 = '0'", strToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message = strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.GetAllBookmarkLevelError;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Data = new List<string>();
                    return optReturn;
                }
                List<string> lstRecords = new List<string>();
                BookmarkLevelEntity level = null;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    level = new BookmarkLevelEntity();
                    level.BookmarkLevelID = row["C001"].ToString();
                    level.BookmarkLevelName = row["C009"].ToString();
                    level.BookmarkLevelColor = S5100EncryptOperation.DecryptWithM002( row["C006"].ToString());
                    level.BookmarkLevelStatus = row["C004"].ToString();
                    optReturn = XMLHelper.SeriallizeObject<BookmarkLevelEntity>(level);
                    if (optReturn.Result)
                    {
                        lstRecords.Add(optReturn.Data as string);
                    }
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = lstRecords;
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
        /// 获得所有关键词
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// <returns></returns>
        private OperationReturn GetAllKeyWorlds(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = "select a.C001 as keyworldID,a.C006 as keyworldContent,b.C001 as levelID,b.C006 as  levelColor ,b.C009 as LevelName" +
                                    " from T_11_009_{0} as a left join T_11_009_{1} as b " +
                                     " on a.C009 = b.C001 where a.C000=4 and b.C000=3 and a.C010 <>'1'";
                        strSql = string.Format(strSql, strToken, strToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "select a.C001 as keyworldID,a.C006 as keyworldContent,b.C001 as levelID,b.C006 as  levelColor ,b.C009 as LevelName" +
                                    " from T_11_009_{0} a left join T_11_009_{1} b " +
                                    " on a.C009 = b.C001  where a.C000=4 and b.C000=3 and a.C010 <>'1'";
                        strSql = string.Format(strSql, strToken, strToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message = strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.GetAllKeyWorldsError;
                    return optReturn;
                }
                List<string> lstRecords = new List<string>();
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Data = lstRecords;
                    return optReturn;
                }
                KeyWorldsEntity keyWorld = null;
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    keyWorld = new KeyWorldsEntity();
                    keyWorld.KeyWorldID = row["keyworldID"].ToString();
                    keyWorld.KeyWorldContent =S5100EncryptOperation.DecryptWithM002( row["keyworldContent"].ToString());
                    keyWorld.BookmarkLevelID = row["levelID"].ToString();
                    keyWorld.BookmarkLevelcolor = S5100EncryptOperation.DecryptWithM002( row["levelColor"].ToString());
                    keyWorld.LevelName = row["LevelName"].ToString();
                    optReturn = XMLHelper.SeriallizeObject<KeyWorldsEntity>(keyWorld);
                    if (optReturn.Result)
                    {
                        lstRecords.Add(optReturn.Data as string);
                    }
                }
                optReturn.Data = lstRecords;
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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
        /// 添加关键词
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 关键词
        /// lstParams[1] : 等级ID
        /// <returns></returns>
        private OperationReturn AddKeyWorld(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();

            try
            {
                if (lstParams == null || lstParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strLevelID = lstParams[1];
                string strKeyWorld = S5100EncryptOperation.EncryptWithM002(lstParams[0]);
                string strSql = string.Empty;
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                List<string> lst = new List<string>();
                lst.Add("51");
                lst.Add("501");
                optReturn = GetSerialID(session, lst);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string str = optReturn.Data as string;
                long keyWorldID = 0;
                long.TryParse(str, out keyWorldID);
                if (keyWorldID == 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S5100WcfErrorCode.SerialIDConvertError;
                    optReturn.Message = str;
                    return optReturn;
                }

                switch (session.DBType)
                {
                    case 2:
                        strSql = "insert into T_11_009_{4} (C000,C001,C002,C003,C004,C005,C006,C007,C008,C009,C010)"
                                    + "values (4,{0},0,0,'1',0,'{1}','{2}','kw','{3}','0')";
                        strSql = string.Format(strSql, keyWorldID, strKeyWorld, session.UserID, strLevelID, rentToken);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "insert into T_11_009_{4} (C000,C001,C002,C003,C004,C005,C006,C007,C008,C009,C010)"
                                      + "values (4,{0},0,0,'1',0,'{1}','{2}','kw','{3}','0')";
                        strSql = string.Format(strSql, keyWorldID, strKeyWorld, session.UserID, strLevelID, rentToken);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += ";" + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.AddBookmarkLevelError;
                    return optReturn;
                }
                optReturn = GenerateKeyWorldXml(session, null);
                if (!optReturn.Result)
                {
                    optReturn.Data = keyWorldID.ToString();
                    return optReturn;
                }
                List<string> lstParamInUpload = new List<string>();
                lstParamInUpload.Add(optReturn.StringValue);
                optReturn = UploadKeyWorldXml(session, lstParamInUpload);
                if (!optReturn.Result)
                {
                    optReturn.Data = keyWorldID.ToString();
                    return optReturn;
                }
                optReturn.Data = keyWorldID.ToString();
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
        /// 修改关键词
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 关键词ID
        /// lstParams[1] : bookmark等级ID
        /// <returns></returns>
        private OperationReturn ModifyKeyWorld(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                if (lstParams == null || lstParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strLevelID = lstParams[1];
                string strKeyWorldID = lstParams[0];
                string strSql = string.Empty;
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;

                switch (session.DBType)
                {
                    case 2:
                        strSql = "  update T_11_009_{0} SET C009 = '{1}' where C001 = {2}";
                        strSql = string.Format(strSql, rentToken, strLevelID, strKeyWorldID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "  update T_11_009_{0} SET C009 = '{1}' where C001 = {2}";
                        strSql = string.Format(strSql, rentToken, strLevelID, strKeyWorldID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += ";" + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.AddBookmarkLevelError;
                    return optReturn;
                }
                optReturn = GenerateKeyWorldXml(session, null);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<string> lstParamInUpload = new List<string>();
                lstParamInUpload.Add(optReturn.StringValue);
                optReturn = UploadKeyWorldXml(session, lstParamInUpload);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn.Data = strKeyWorldID.ToString();
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

        private OperationReturn DeleteKeyWorld(SessionInfo session, List<string> lstParams)
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
                string strKeyWorldID = lstParams[0];
                string strSql = string.Empty;
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;

                switch (session.DBType)
                {
                    case 2:
                        strSql = "  update T_11_009_{0} SET C010 = '1' where C001 = {1}";
                        strSql = string.Format(strSql, rentToken, strKeyWorldID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "  update T_11_009_{0} SET C010 = '1' where C001 = {1}";
                        strSql = string.Format(strSql, rentToken, strKeyWorldID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += ";" + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.AddBookmarkLevelError;
                    return optReturn;
                }
                optReturn = GenerateKeyWorldXml(session, null);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<string> lstParamInUpload = new List<string>();
                lstParamInUpload.Add(optReturn.StringValue);
                optReturn = UploadKeyWorldXml(session, lstParamInUpload);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn.Data = strKeyWorldID.ToString();
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
        /// 生成关键词的xml
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// <returns></returns>
        private OperationReturn GenerateKeyWorldXml(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                optReturn = GetAllKeyWorlds(session, lstParams);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<string> lstKeyWorlds = optReturn.Data as List<string>;
                optReturn = Common.CreateXmlDocumentIfNotExists(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\KeyWorld", "KeyWord.xml", "KeyWords");
                string strFilePath = optReturn.StringValue;
                XmlDocument xmlDoc = optReturn.Data as XmlDocument;
                XmlNode root = xmlDoc.SelectSingleNode("KeyWords");
                XmlElement element = null;
                int keyWorldID = 0;
                for (int i = 0; i < lstKeyWorlds.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KeyWorldsEntity>(lstKeyWorlds[i]);
                    if (optReturn.Result)
                    {
                        element = xmlDoc.CreateElement("KeyWord");
                        element.SetAttribute("ID", keyWorldID.ToString());
                        keyWorldID++;
                        element.InnerText =S5100EncryptOperation.DecryptWithM002( (optReturn.Data as KeyWorldsEntity).KeyWorldContent);
                        root.AppendChild(element);
                    }
                }
                xmlDoc.Save(strFilePath);
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Message = keyWorldID.ToString();
                optReturn.StringValue = strFilePath;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = (int)S5100WcfErrorCode.GenerateKeyWorldXmlException;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 上传关键词文件
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 关键词文件源路径
        /// <returns></returns>
        private OperationReturn UploadKeyWorldXml(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strSql = string.Empty;
                string strToken = session.RentInfo.Token;
                //获得可用的设备
                switch (session.DBType)
                {
                    case 2:
                        strSql = "select C001,C016 from T_11_101_{0} where C001 > 2810000000000000000 and C001<2820000000000000000 and C002 = 1";
                        strSql = string.Format(strSql, strToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "select C001,C016 from T_11_101_{0} where C001 > 2810000000000000000 and C001<2820000000000000000 and C002 = 1";
                        strSql = string.Format(strSql, strToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += ";" + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.CanNotFindEnabledMachine;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S5100WcfErrorCode.PCMDeviceIsNull;
                    return optReturn;
                }
                string strMachineID = ds.Tables[0].Rows[0]["C001"].ToString();

                //获得PCMDeviceID
                switch (session.DBType)
                {
                    case 2:
                        strSql = "select C011 from T_11_101_{0} where C001 = {1} and C002 = 2";
                        strSql = string.Format(strSql, strToken,strMachineID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "select C011 from T_11_101_{0} where C001 ={1} and C002 = 2";
                        strSql = string.Format(strSql, strToken,strMachineID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += ";" + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.GetPCMDeviceIDError;
                    return optReturn;
                }
                ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S5100WcfErrorCode.PCMDeviceIsNull;
                    return optReturn;
                }
                int PCMDeviceID = 0;
                try
                {
                    PCMDeviceID = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S5100WcfErrorCode.PCMDeviceIDIsError;
                    return optReturn;
                }

                //根据DeviceID找到对应的存储设备 获得IP端口等相关信息
                switch (session.DBType)
                {
                    case 2:
                        strSql = "select * from T_11_101_{0} where C001 in " +
                                    "(select C001 from T_11_101_{1} where C001 > 2140000000000000000 and C001<2150000000000000001 and C002=1 and C012={2})";
                        strSql = string.Format(strSql, strToken, strToken, PCMDeviceID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "select * from T_11_101_{0} where C001 in " +
                                  "(select C001 from T_11_101_{1} where C001 > 2140000000000000000 and C001<2150000000000000001 and C002=1 and C012={2})";
                        strSql = string.Format(strSql, strToken, strToken, PCMDeviceID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message = strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S5100WcfErrorCode.GetStorageDeviceError;
                    return optReturn;
                }
                ds = optReturn.Data as DataSet;
                ds.Tables[0].PrimaryKey = new DataColumn[] { ds.Tables[0].Columns["C001"], ds.Tables[0].Columns["C002"] };
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0 || ds.Tables[0].Rows.Count < 5)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S5100WcfErrorCode.CanNotFindStorageDevice;
                    return optReturn;
                }
                string[] keys = new string[2];
                keys[0] = ds.Tables[0].Rows[0]["C001"].ToString();
                keys[1] = "2";
                DataRow row = ds.Tables[0].Rows.Find(keys);
                string strDeviceType = row["C011"].ToString();
                string strHost = S5100EncryptOperation.DecodeEncryptValue(row["C013"].ToString());
                string strPort = S5100EncryptOperation.DecodeEncryptValue(row["C015"].ToString());
                string strTargetDir = row["C014"].ToString();

                string strSourceDir = lstParams[0];
                //如果是本地 直接拷贝文件到指定目录
                if (strDeviceType == "0")
                {
                    strTargetDir = strTargetDir.Trim('\\');
                    strTargetDir += "\\KeyWord.xml";
                    optReturn = Common.CopyFile(strSourceDir, strTargetDir);
                    if (!optReturn.Result)
                    {
                        //optReturn.Result = false;
                        //optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlFailed;
                        //optReturn.Message += " ;  strTargetDir = "+strTargetDir;
                        return optReturn;
                    }
                }
                //如果是共享目录
                else if (strDeviceType == "1")
                {
                    keys[1] = "92";
                    row = ds.Tables[0].Rows.Find(keys);
                    string strUser = S5100EncryptOperation.DecodeEncryptValue(row["C011"].ToString());
                    string strPwd = S5100EncryptOperation.DecodeEncryptValue(row["C012"].ToString());
                    int iUploadCount =0;
                    do
                    {
                        optReturn = Common.UpLoadFile(strSourceDir, strTargetDir, strUser, strPwd);
                        iUploadCount++;
                    } while (optReturn.Result == false && iUploadCount < 3);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                }
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = (int)S5100WcfErrorCode.UploadKeyWorldXmlException;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private string DecryptFromClient(string strSource)
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
