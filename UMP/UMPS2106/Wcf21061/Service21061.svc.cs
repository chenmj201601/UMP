using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.ServiceModel.Activation;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.SDKs.DEC;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common21061;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;
using Wcf21061.Wcf11012;

namespace Wcf21061
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service21061”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service21061.svc 或 Service21061.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service21061 : IService21061
    {

        #region Members

        private Service00Helper mService00Helper;
        private DecHelper mDecHelper;
        private LogOperator mLogOperator;

        #endregion


        public Service21061()
        {
            CreateLogOperator();
        }


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
                var dbInfo = session.DatabaseInfo;
                if (dbInfo != null)
                {
                    dbInfo.RealPassword = DecryptString004(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }
                switch (webRequest.Code)
                {
                    case (int)S2106Codes.GetVoiceList:
                        optReturn = GetVoiceList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2106Codes.GetChannelList:
                        optReturn = GetChannelList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2106Codes.GetDECList:
                        optReturn = GetDECList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2106Codes.GetRecoverServerList:
                        optReturn = GetRecoverServerList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2106Codes.GetRecoverStrategyList:
                        optReturn = GetRecoverStrategyList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2106Codes.GetRecoverChannelList:
                        optReturn = GetRecoverChannelList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2106Codes.GetDiskDriverList:
                        optReturn = GetDiskDriverList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2106Codes.GetChildDirList:
                        optReturn = GetChildDirList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2106Codes.GetStrategyFlagList:
                        optReturn = GetStrategyFlagList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2106Codes.ExecuteStrategy:
                        optReturn = ExecuteStrategy(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2106Codes.SaveRecoverStrategy:
                        optReturn = SaveRecoverStrategy(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S2106Codes.SaveRecoverChannels:
                        optReturn = SaveRecoverChannels(session, webRequest.ListData);
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

        private OperationReturn GetVoiceList(SessionInfo session, List<string> listParams)
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
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                List<string> listReturn = new List<string>();
                string rentToken = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                int dbType = session.DatabaseInfo.TypeID;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > 2210000000000000000 AND C001 < 2220000000000000000 AND C002 = 1 ORDER BY C001, C002",
                                rentToken);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_11_101_{0} WHERE C001 > 2210000000000000000 AND C001 < 2220000000000000000 AND C002 = 1 ORDER BY C001, C002",
                               rentToken);
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

                    ResourceObject obj = new ResourceObject();
                    obj.ObjID = Convert.ToInt64(dr["C001"]);
                    obj.ObjType = S2106Consts.RESOURCE_VOICESERVER;
                    obj.Name = dr["C012"].ToString();        //VoiceID
                    string strValue = dr["C017"].ToString();
                    strValue = DecodeEncryptValue(strValue);
                    obj.FullName = strValue;
                    optReturn = XMLHelper.SeriallizeObject(obj);
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

        private OperationReturn GetChannelList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     VoiceObjID
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strVoiceObjID = listParams[1];
                long voiceObjID;
                if (!long.TryParse(strVoiceObjID, out voiceObjID)
                    || voiceObjID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("VoiceObjID invalid.");
                    return optReturn;
                }
                List<ResourceObject> listObjs = new List<ResourceObject>();
                string rentToken = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                int dbType = session.DatabaseInfo.TypeID;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > 2250000000000000000 AND C001 < 2260000000000000000 AND C002 = 1 AND C013 = '{1}' ORDER BY C001, C002",
                                rentToken,
                                voiceObjID);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_11_101_{0} WHERE C001 > 2250000000000000000 AND C001 < 2260000000000000000 AND C002 = 1 AND C013 = '{1}' ORDER BY C001, C002",
                               rentToken,
                               voiceObjID);
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

                    ResourceObject obj = new ResourceObject();
                    obj.ObjID = Convert.ToInt64(dr["C001"]);
                    obj.ObjType = S2106Consts.RESOURCE_VOICECHANNEL;
                    obj.ParentObjID = Convert.ToInt64(dr["C013"]);
                    listObjs.Add(obj);
                }
                for (int i = 0; i < listObjs.Count; i++)
                {
                    var obj = listObjs[i];
                    long id = obj.ObjID;
                    switch (dbType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001, C002",
                                    rentToken,
                                    id);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001, C002",
                                    rentToken,
                                    id);
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
                    objDataSet = optReturn.Data as DataSet;
                    if (objDataSet == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("DataSet is null.");
                        return optReturn;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        int row = Convert.ToInt32(dr["C002"]);
                        if (row == 1)
                        {
                            obj.Name = dr["C012"].ToString();
                        }
                        if (row == 2)
                        {
                            obj.FullName = dr["C012"].ToString();
                        }
                    }
                }
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

        private OperationReturn GetDECList(SessionInfo session, List<string> listParams)
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
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                List<string> listReturn = new List<string>();
                string rentToken = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                int dbType = session.DatabaseInfo.TypeID;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > 2120000000000000000 AND C001 < 2130000000000000000 AND C002 = 1 ORDER BY C001, C002",
                                rentToken);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_11_101_{0} WHERE C001 > 2120000000000000000 AND C001 < 2130000000000000000 AND C002 = 1 ORDER BY C001, C002",
                               rentToken);
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

                    ResourceObject obj = new ResourceObject();
                    obj.ObjID = Convert.ToInt64(dr["C001"]);
                    obj.ObjType = S2106Consts.RESOURCE_DATAEXCHANGECENTER;
                    obj.Name = dr["C012"].ToString();        //ID
                    string strValue = dr["C017"].ToString();    //地址
                    strValue = DecodeEncryptValue(strValue);
                    obj.FullName = strValue;
                    strValue = dr["C019"].ToString();
                    strValue = DecodeEncryptValue(strValue);
                    obj.Other01 = strValue;     //端口
                    optReturn = XMLHelper.SeriallizeObject(obj);
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

        private OperationReturn GetRecoverServerList(SessionInfo session, List<string> listParams)
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
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                List<string> listReturn = new List<string>();
                string rentToken = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                int dbType = session.DatabaseInfo.TypeID;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > 2280000000000000000 AND C001 < 2290000000000000000 AND C002 = 1 ORDER BY C001, C002",
                                rentToken);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_11_101_{0} WHERE C001 > 2280000000000000000 AND C001 < 2290000000000000000 AND C002 = 1 ORDER BY C001, C002",
                               rentToken);
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

                    ResourceObject obj = new ResourceObject();
                    obj.ObjID = Convert.ToInt64(dr["C001"]);
                    obj.ObjType = S2106Consts.RESOURCE_DATAEXCHANGECENTER;
                    obj.Name = dr["C012"].ToString();        //ID
                    string strValue = dr["C017"].ToString();    //地址
                    strValue = DecodeEncryptValue(strValue);
                    obj.FullName = strValue;
                    strValue = dr["C019"].ToString();
                    strValue = DecodeEncryptValue(strValue);
                    obj.Other01 = strValue;     //端口
                    optReturn = XMLHelper.SeriallizeObject(obj);
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

        private OperationReturn GetRecoverStrategyList(SessionInfo session, List<string> listParams)
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
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                List<string> listReturn = new List<string>();
                string rentToken = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                int dbType = session.DatabaseInfo.TypeID;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_21_011_{0} ORDER BY C001 DESC",
                                rentToken);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM T_21_011_{0} ORDER BY C001 DESC",
                               rentToken);
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
                string strValue;
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    RecoverStrategyInfo item = new RecoverStrategyInfo();
                    item.SerialNo = Convert.ToInt64(dr["C001"]);
                    strValue = dr["C002"].ToString();
                    strValue = DecryptString002(strValue);
                    item.Name = strValue;
                    item.Description = dr["C003"].ToString();
                    item.State = Convert.ToInt32(dr["C004"]);
                    item.BeginTime = DecodeDatetimeValue(dr["C005"].ToString());
                    item.EndTime = DecodeDatetimeValue(dr["C006"].ToString());
                    item.PackagePath = dr["C007"].ToString();
                    item.Flag = DecodeIntValue(dr["C008"].ToString());
                    item.Progress = DecodeIntValue(dr["C009"].ToString());
                    item.Times = DecodeIntValue(dr["C010"].ToString());
                    item.Message = dr["C011"].ToString();
                    item.LastOptTime = DecodeDatetimeValue(dr["C012"].ToString());
                    item.Creator = DecodeLongValue(dr["C013"].ToString());
                    item.CreateTime = DecodeDatetimeValue(dr["C014"].ToString());
                    item.Modifier = DecodeLongValue(dr["C015"].ToString());
                    item.ModifyTime = DecodeDatetimeValue(dr["C016"].ToString());

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

        private OperationReturn GetRecoverChannelList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     StrategyID
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strStrategyID = listParams[1];
                long strategyID;
                if (!long.TryParse(strStrategyID, out strategyID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("StrategyID invalid.");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                string rentToken = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                int dbType = session.DatabaseInfo.TypeID;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_21_012_{0} WHERE C001 = {1} ORDER BY C001, C002",
                                rentToken, strategyID);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_21_012_{0} WHERE C001 = {1} ORDER BY C001, C002",
                                rentToken, strategyID);
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
                string strValue;
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    RecoverChannelInfo item = new RecoverChannelInfo();
                    item.StrategyID = Convert.ToInt64(dr["C001"]);
                    item.Number = Convert.ToInt32(dr["C002"]);
                    item.VoiceID = DecodeIntValue(dr["C003"].ToString());
                    item.ChannelID = DecodeIntValue(dr["C004"].ToString());
                    strValue = dr["C005"].ToString();
                    strValue = DecryptString002(strValue);
                    item.Extension = strValue;
                    strValue = dr["C006"].ToString();
                    strValue = DecryptString002(strValue);
                    item.VoiceIP = strValue;
                    item.VoiceObjID = DecodeLongValue(dr["C007"].ToString());
                    item.ChannelObjID = DecodeLongValue(dr["C008"].ToString());

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

        private OperationReturn GetDiskDriverList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     ServerAddress
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strAddress = listParams[1];
                List<string> listArgs = new List<string>();
                listArgs.Add("0");

                //异步模式（过指定的时间超时）
                if (mService00Helper == null)
                {
                    mService00Helper = new Service00Helper();
                    mService00Helper.Debug += mService00Helper_Debug;
                    mService00Helper.Start();
                }
                mService00Helper.HostAddress = strAddress;
                mService00Helper.HostPort = 8009;
                optReturn = mService00Helper.DoOperation(RequestCommand.GET_DISK_INFO, listArgs);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                string strMessage = optReturn.Data.ToString();
                if (!string.IsNullOrEmpty(strMessage))
                {
                    if (strMessage.StartsWith("Error"))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = string.Format("{0}", strMessage);
                        return optReturn;
                    }
                    string[] drivers = strMessage.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    if (drivers.Length > 0)
                    {
                        for (int i = 0; i < drivers.Length; i++)
                        {
                            string driver = drivers[i];
                            if (string.IsNullOrEmpty(driver)) { continue; }
                            string[] arrInfos = driver.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.None);
                            string strName = string.Empty;
                            string strVolumeName = string.Empty;
                            if (arrInfos.Length > 0)
                            {
                                strName = arrInfos[0];
                            }
                            if (arrInfos.Length > 1)
                            {
                                strVolumeName = arrInfos[1];
                            }
                            string strInfo = string.Format("{0}{1}{2}", strName, ConstValue.SPLITER_CHAR, strVolumeName);
                            listReturn.Add(strInfo);
                        }
                    }
                }
                optReturn.Data = listReturn;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GetChildDirList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     ServerAddress
                //2     父目录的完整路径
                if (listParams == null
                    || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strAddress = listParams[1];
                string strParentDir = listParams[2];
                List<string> listArgs = new List<string>();
                listArgs.Add(strParentDir);

                //异步模式（过指定的时间超时）
                if (mService00Helper == null)
                {
                    mService00Helper = new Service00Helper();
                    mService00Helper.Debug += mService00Helper_Debug;
                    mService00Helper.Start();
                }
                mService00Helper.HostAddress = strAddress;
                mService00Helper.HostPort = 8009;
                optReturn = mService00Helper.DoOperation(RequestCommand.GET_SUBDIRECTORY, listArgs);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                string strMessage = optReturn.Data.ToString();
                if (!string.IsNullOrEmpty(strMessage))
                {
                    if (strMessage.StartsWith("Error"))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = string.Format("{0}", strMessage);
                        return optReturn;
                    }
                    string[] dirs = strMessage.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    if (dirs.Length > 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            string dir = dirs[i];
                            if (string.IsNullOrEmpty(dir)) { continue; }
                            string[] arrInfos = dir.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.None);
                            string strName = string.Empty;
                            if (arrInfos.Length > 0)
                            {
                                strName = arrInfos[0];
                            }
                            string strFullName = Path.Combine(strParentDir, strName);
                            string strInfo = string.Format("{0}{1}{2}", strName, ConstValue.SPLITER_CHAR, strFullName);
                            listReturn.Add(strInfo);
                        }
                    }
                }
                optReturn.Data = listReturn;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GetStrategyFlagList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     数量
                //2...  恢复策略的编码
                if (listParams == null
                    || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Count invalid.");
                    return optReturn;
                }
                if (listParams.Count < intCount + 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("SerialNo count invalid.");
                    return optReturn;
                }
                List<string> listSerialNos = new List<string>();
                for (int i = 0; i < intCount; i++)
                {
                    listSerialNos.Add(listParams[i + 2]);
                }
                List<string> listReturn = new List<string>();
                string rentToken = session.RentInfo.Token;
                string strConn = session.DBConnectionString;
                int dbType = session.DatabaseInfo.TypeID;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT C001, C008, C009 FROM T_21_011_{0} ORDER BY C001 DESC",
                                rentToken);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT C001, C008, C009 FROM T_21_011_{0} ORDER BY C001 DESC",
                               rentToken);
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

                    long id = Convert.ToInt64(dr["C001"]);
                    if (listSerialNos.Contains(id.ToString()))
                    {
                        int flag = Convert.ToInt32(dr["C008"]);
                        double progress = Convert.ToDouble(dr["C009"]);
                        string strInfo = string.Format("{0}{1}{2}{1}{3}", id, ConstValue.SPLITER_CHAR, flag, progress);
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
            }
            return optReturn;
        }

        private OperationReturn ExecuteStrategy(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                #region 验证参数

                //ListParams
                //0     UserID
                //1     DEC Address
                //2     DEC Port
                //3     StrategyInfo
                //4     ChannelCount
                //5...  ChannelInfo
                if (listParams == null
                    || listParams.Count < 5)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strHost = listParams[1];
                string strPort = listParams[2];
                string strStrategy = listParams[3];
                string strCount = listParams[4];
                int intPort;
                if (!int.TryParse(strPort, out intPort)
                    || intPort < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Port invalid.");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<RecoverStrategyInfo>(strStrategy);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                RecoverStrategyInfo strategy = optReturn.Data as RecoverStrategyInfo;
                if (strategy == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("RecoverStrategy is null.");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ChannelCount invalid.");
                    return optReturn;
                }
                if (listParams.Count < intCount + 5)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Channel count invalid.");
                    return optReturn;
                }
                List<RecoverChannelInfo> listChannels = new List<RecoverChannelInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 5];
                    optReturn = XMLHelper.DeserializeObject<RecoverChannelInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    RecoverChannelInfo channel = optReturn.Data as RecoverChannelInfo;
                    if (channel == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("RecoverChannel is null.");
                        return optReturn;
                    }
                    listChannels.Add(channel);
                }

                #endregion


                #region 将策略的执行状态置为正在执行

                int flag = 1;   //正在执行
                string rentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConn = session.DBConnectionString;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_21_011_{0} WHERE C001 = {1}", rentToken,
                            strategy.SerialNo);
                        objConn = MssqlOperation.GetConnection(strConn);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_21_011_{0} WHERE C001 = {1}", rentToken,
                         strategy.SerialNo);
                        objConn = OracleOperation.GetConnection(strConn);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", dbType);
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

                    DataRow dr =
                        objDataSet.Tables[0].Select(string.Format("C001 = {0}", strategy.SerialNo)).FirstOrDefault();
                    if (dr != null)
                    {
                        dr["C008"] = flag;
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

                #endregion


                #region 发送DEC消息

                if (mDecHelper == null)
                {
                    mDecHelper = new DecHelper();
                    mDecHelper.Debug += (mode, cat, msg) => mDecHelper_Debug(string.Format("{0}\t{1}", cat, msg));
                }
                mDecHelper.HostAddress = strHost;
                mDecHelper.HostPort = intPort;

                List<string> listReturn = new List<string>();

                XmlDocument doc;
                DateTime dt;
                string strMessageID;
                XmlElement nodeMessage;
                XmlElement nodeFileParam;
                XmlElement nodeChannels;
                XmlElement nodeChannel;
                string strMessage;

                MessageString message = new MessageString();
                message.SourceModule = 0x0000;
                message.SourceNumber = 0x0000;
                message.TargetModule = 0x152B;
                message.TargetNumber = 0x0000;
                message.Number = 0x0003;            //开始恢复录音
                message.SmallType = 0x0001;
                message.MiddleType = 0x0001;
                message.LargeType = 0x0009;

                listReturn.Add(message.ToString());

                doc = new XmlDocument();
                doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");

                dt = DateTime.Now;
                dt = dt.AddYears(-1600);
                strMessageID = string.Format("0x{0:X4}{1:X4}{2:X4}{3:X4}",
                    message.LargeType,
                    message.MiddleType,
                    message.SmallType,
                    message.Number);
                nodeMessage = doc.CreateElement(DecHelper.NODE_MESSAGE);
                nodeMessage.SetAttribute(DecHelper.ATTR_MESSAGEID, strMessageID);
                nodeMessage.SetAttribute(DecHelper.ATTR_CURRENTTIME, dt.Ticks.ToString());
                nodeMessage.SetAttribute(DecHelper.ATTR_TASKID, strategy.SerialNo.ToString());
                nodeFileParam = doc.CreateElement(DecHelper.NODE_FILEPARAM);
                nodeFileParam.SetAttribute(DecHelper.ATTR_FILEPATH, strategy.PackagePath);
                nodeFileParam.SetAttribute(DecHelper.ATTR_TIMEFROM, strategy.BeginTime.ToString("yyyy-MM-dd HH:mm:ss"));
                nodeFileParam.SetAttribute(DecHelper.ATTR_TIMETO, strategy.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
                nodeFileParam.SetAttribute(DecHelper.ATTR_USEINDTIME, "0");
                nodeMessage.AppendChild(nodeFileParam);
                nodeChannels = doc.CreateElement(DecHelper.NODE_RECOVERCHANNELS);
                for (int i = 0; i < listChannels.Count; i++)
                {
                    var channel = listChannels[i];
                    nodeChannel = doc.CreateElement(DecHelper.NODE_RECOVERCHANNEL);
                    nodeChannel.SetAttribute(DecHelper.ATTR_CHANNELID, channel.Number.ToString());
                    nodeChannel.SetAttribute(DecHelper.ATTR_ORIGINALVOICENUMBER, channel.VoiceID.ToString());
                    nodeChannel.SetAttribute(DecHelper.ATTR_ORIGINALCHANNELID, channel.ChannelID.ToString());
                    nodeChannels.AppendChild(nodeChannel);
                }
                nodeMessage.AppendChild(nodeChannels);
                doc.AppendChild(nodeMessage);
                strMessage = doc.OuterXml;

                WriteOperationLog(string.Format("Publish:{0}", strMessage));
                listReturn.Add(strMessage);

                optReturn = mDecHelper.PublishMessage(strMessage, message);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                #endregion


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

        private OperationReturn SaveRecoverStrategy(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     Method  0:Add;1:Modify;2:Delete
                //2     StrategyIInfo or StrategyID
                if (listParams == null
                    || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strMethod = listParams[1];
                string strStrategy = listParams[2];
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
                    || intMethod < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Method invalid.");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                if (intMethod == 0)
                {

                    #region Add

                    optReturn = XMLHelper.DeserializeObject<RecoverStrategyInfo>(strStrategy);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    RecoverStrategyInfo strategy = optReturn.Data as RecoverStrategyInfo;
                    if (strategy == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Strategy is null.");
                        return optReturn;
                    }


                    #region 获取流水号

                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("11");
                    webRequest.ListData.Add(S2106Consts.RESOURCE_RECOVERSTRATEGY.ToString());
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
                    strategy.SerialNo = Convert.ToInt64(webReturn.Data);

                    #endregion


                    #region 保存信息到数据库

                    string rentToken = session.RentInfo.Token;
                    string strSql;
                    IDbConnection objConn;
                    IDbDataAdapter objAdapter;
                    DbCommandBuilder objCmdBuilder;
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_21_011_{0} WHERE 1 = 2", rentToken);
                            objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_21_011_{0} WHERE 1 = 2", rentToken);
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

                        DataRow dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = strategy.SerialNo;
                        dr["C002"] = EncryptString002(strategy.Name);
                        dr["C003"] = strategy.Description;
                        dr["C004"] = strategy.State;
                        dr["C005"] = strategy.BeginTime.ToString("yyyyMMddHHmmss");
                        dr["C006"] = strategy.EndTime.ToString("yyyyMMddHHmmss");
                        dr["C007"] = strategy.PackagePath;
                        dr["C008"] = strategy.Flag;
                        dr["C009"] = strategy.Progress;
                        dr["C010"] = strategy.Times;
                        dr["C011"] = strategy.Message;
                        dr["C012"] = strategy.LastOptTime.ToString("yyyyMMddHHmmss");
                        dr["C013"] = userID;
                        dr["C014"] = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss");
                        dr["C015"] = userID;
                        dr["C016"] = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss");
                        objDataSet.Tables[0].Rows.Add(dr);

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
                    listReturn.Add(strategy.SerialNo.ToString());

                    #endregion


                    #endregion

                }
                else if (intMethod == 1)
                {

                    #region Modify

                    optReturn = XMLHelper.DeserializeObject<RecoverStrategyInfo>(strStrategy);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    RecoverStrategyInfo strategy = optReturn.Data as RecoverStrategyInfo;
                    if (strategy == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Strategy is null.");
                        return optReturn;
                    }
                    long serialNo = strategy.SerialNo;


                    #region 保存信息到数据库

                    string rentToken = session.RentInfo.Token;
                    string strSql;
                    IDbConnection objConn;
                    IDbDataAdapter objAdapter;
                    DbCommandBuilder objCmdBuilder;
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_21_011_{0} WHERE C001 = {1}", rentToken, serialNo);
                            objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_21_011_{0} WHERE C001 = {1}", rentToken, serialNo);
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

                        if (objDataSet.Tables[0].Rows.Count <= 0)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_NOT_EXIST;
                            optReturn.Message = string.Format("Strategy not exist.");
                            return optReturn;
                        }
                        DataRow dr = objDataSet.Tables[0].Rows[0];
                        dr["C001"] = strategy.SerialNo;
                        dr["C002"] = EncryptString002(strategy.Name);
                        dr["C003"] = strategy.Description;
                        dr["C004"] = strategy.State;
                        dr["C005"] = strategy.BeginTime.ToString("yyyyMMddHHmmss");
                        dr["C006"] = strategy.EndTime.ToString("yyyyMMddHHmmss");
                        dr["C007"] = strategy.PackagePath;
                        dr["C008"] = strategy.Flag;
                        dr["C009"] = strategy.Progress;
                        dr["C010"] = strategy.Times;
                        dr["C011"] = strategy.Message;
                        dr["C012"] = strategy.LastOptTime.ToString("yyyyMMddHHmmss");
                        dr["C013"] = userID;
                        dr["C014"] = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss");
                        dr["C015"] = userID;
                        dr["C016"] = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss");

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
                    listReturn.Add(serialNo.ToString());

                    #endregion


                    #endregion

                }
                else if (intMethod == 2)
                {

                    #region Delete

                    long serialNo;
                    if (!long.TryParse(strStrategy, out serialNo))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("StrategyID invalid.");
                        return optReturn;
                    }
                    string rentToken = session.RentInfo.Token;
                    string strSql;
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("DELETE FROM T_21_011_{0} WHERE C001 = {1}", rentToken, serialNo);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strSql = string.Format("DELETE FROM T_21_012_{0} WHERE C001 = {1}", rentToken, serialNo);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            break;
                        case 3:
                            strSql = string.Format("DELETE FROM T_21_011_{0} WHERE C001 = {1}", rentToken, serialNo);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            strSql = string.Format("DELETE FROM T_21_012_{0} WHERE C001 = {1}", rentToken, serialNo);
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
                    listReturn.Add(serialNo.ToString());

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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn SaveRecoverChannels(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     UserID
                //1     StrategyID
                //2     Count
                //3...  ChannelInfo
                if (listParams == null
                    || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid.");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strStrategyID = listParams[1];
                string strCount = listParams[2];
                long userID;
                if (!long.TryParse(strUserID, out userID)
                    || userID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid.");
                    return optReturn;
                }
                long strategyID;
                if (!long.TryParse(strStrategyID, out strategyID)
                    || strategyID < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("StrategyID invalid.");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Count invalid.");
                    return optReturn;
                }
                if (listParams.Count < intCount + 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Channel count invalid.");
                    return optReturn;
                }
                List<RecoverChannelInfo> listChannels = new List<RecoverChannelInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<RecoverChannelInfo>(listParams[i + 3]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    RecoverChannelInfo channel = optReturn.Data as RecoverChannelInfo;
                    if (channel == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Channel is null.");
                        return optReturn;
                    }
                    listChannels.Add(channel);
                }
                List<string> listReturn = new List<string>();
                string rentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConn = session.DBConnectionString;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_21_012_{0} WHERE C001 = {1} ORDER BY C001, C002",
                            rentToken, strategyID);
                        objConn = MssqlOperation.GetConnection(strConn);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_21_012_{0} WHERE C001 = {1} ORDER BY C001, C002",
                            rentToken, strategyID);
                        objConn = OracleOperation.GetConnection(strConn);
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

                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];

                        int number = Convert.ToInt32(dr["C002"]);
                        var channel = listChannels.FirstOrDefault(c => c.Number == number);
                        if (channel == null)
                        {
                            dr.Delete();

                            listReturn.Add(string.Format("D{0};{1}", strategyID, number));
                        }
                    }

                    for (int i = 0; i < listChannels.Count; i++)
                    {
                        var channel = listChannels[i];
                        int number = channel.Number;
                        bool isAdd = false;

                        DataRow dr = objDataSet.Tables[0].Select(string.Format("C002 = {0}", number)).FirstOrDefault();
                        if (dr == null)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strategyID;
                            dr["C002"] = number;
                            isAdd = true;
                        }
                        dr["C003"] = channel.VoiceID;
                        dr["C004"] = channel.ChannelID;
                        dr["C005"] = EncryptString002(channel.Extension);
                        dr["C006"] = EncryptString002(channel.VoiceIP);
                        dr["C007"] = channel.VoiceObjID;
                        dr["C008"] = channel.ChannelObjID;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);

                            listReturn.Add(string.Format("A{0};{1}", strategyID, number));
                        }
                        else
                        {
                            listReturn.Add(string.Format("M{0};{1}", strategyID, number));
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


        #region Others

        private int DecodeIntValue(string strValue)
        {
            try
            {
                return int.Parse(strValue);
            }
            catch
            {
                return 0;
            }
        }

        private long DecodeLongValue(string strValue)
        {
            try
            {
                return long.Parse(strValue);
            }
            catch
            {
                return 0;
            }
        }

        private DateTime DecodeDatetimeValue(string strValue)
        {
            try
            {
                return Converter.NumberToDatetime(strValue);
            }
            catch
            {
                return DateTime.Parse("2014/1/1");
            }
        }

        #endregion


        #region Encryption

        private string DecodeEncryptValue(string strValue)
        {
            //加密的(以连续三个char27开头）
            if (strValue.StartsWith(string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR)))
            {
                strValue = strValue.Substring(3);
                string[] arrContent = strValue.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                string strVersion = string.Empty, strMode = string.Empty, strPass = strValue;
                if (arrContent.Length > 0)
                {
                    strVersion = arrContent[0];
                }
                if (arrContent.Length > 1)
                {
                    strMode = arrContent[1];
                }
                if (arrContent.Length > 2)
                {
                    strPass = arrContent[2];
                }
                if (strVersion == "2" && strMode == "hex")
                {
                    strPass = DecryptString002(strPass);
                }
                return strPass;
            }
            return strValue;
        }

        private string DecryptString004(string strSource)
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

        private string EncryptString004(string strSource)
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

        private string DecryptString002(string strSource)
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

        private string EncryptString002(string strSource)
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

        #endregion


        #region Service00Helper

        private void mService00Helper_Debug(string msg)
        {
            WriteOperationLog(string.Format("Service00Helper\t{0}", msg));
        }

        #endregion


        #region DecHelper

        private void mDecHelper_Debug(string msg)
        {
            WriteOperationLog(string.Format("DecHelper\t{0}", msg));
        }

        #endregion


        #region LogOperator


        private void CreateLogOperator()
        {
            mLogOperator = new LogOperator();
            mLogOperator.LogPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\Wcf21061\\Logs");
            mLogOperator.Start();
        }

        private void WriteOperationLog(string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, "Wcf21061", msg);
            }
        }

        #endregion


        #region Dispose

        public void Dispose()
        {
            if (mDecHelper != null)
            {
                mDecHelper.Close();
            }
            if (mService00Helper != null)
            {
                mService00Helper.Stop();
            }
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
            }
        }

        #endregion

    }
}
