using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;

namespace Wcf000A1
{
    public partial class Service000A1
    {
        private OperationReturn ReadDatabaseInfo()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
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
                            strValue = DecryptFromClient(strValue);
                            if (strValue != "1") { continue; }
                        }
                        var attr = temp.Attributes["P02"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptFromClient(strValue);
                            if (int.TryParse(strValue, out intValue))
                            {
                                dbInfo.TypeID = intValue;
                            }
                        }
                        attr = temp.Attributes["P04"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptFromClient(strValue);
                            dbInfo.Host = strValue;
                        }
                        attr = temp.Attributes["P05"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptFromClient(strValue);
                            if (int.TryParse(strValue, out intValue))
                            {
                                dbInfo.Port = intValue;
                            }
                        }
                        attr = temp.Attributes["P06"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptFromClient(strValue);
                            dbInfo.DBName = strValue;
                        }
                        attr = temp.Attributes["P07"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptFromClient(strValue);
                            dbInfo.LoginName = strValue;
                        }
                        attr = temp.Attributes["P08"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            dbInfo.Password = strValue;
                        }
                    }
                }
                dbInfo.RealPassword = DecryptFromClient(dbInfo.Password);
                optReturn.Data = dbInfo;
                optReturn.Message = dbInfo.GetConnectionString();
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

        private OperationReturn GetDBInfo(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //参考S000A1Codes中的定义
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                WriteOperationLog(
                    string.Format("GetDBInfo:\tUserID:{0};",
                        strUserID));
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
                List<string> listReturn = new List<string>();
                listReturn.Add(dbInfo.TypeID.ToString());
                listReturn.Add(dbInfo.Host);
                listReturn.Add(dbInfo.Port.ToString());
                listReturn.Add(dbInfo.DBName);
                listReturn.Add(dbInfo.LoginName);
                listReturn.Add(string.Format("***"));
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

        private OperationReturn GetDBData(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //参考S000A1Codes中的定义
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strSql = listParams[1];
                WriteOperationLog(
                    string.Format("GetDBInfo:\tUserID:{0};Sql:{1}",
                        strUserID,
                        strSql));
                strSql = DecryptFromClient(strSql);
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
                switch (dbInfo.TypeID)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid.\t{0}", dbInfo.TypeID);
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
                optReturn.Data = objDataSet;
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

        private OperationReturn ExtDBCommand(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //参考S000A1Codes中的定义
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strSql = listParams[1];
                WriteOperationLog(
                    string.Format("GetDBInfo:\tUserID:{0};Sql:{1}",
                        strUserID,
                        strSql));
                strSql = DecryptFromClient(strSql);
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
                switch (dbInfo.TypeID)
                {
                    case 2:
                        optReturn = MssqlOperation.ExecuteSql(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        optReturn = OracleOperation.ExecuteSql(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid.\t{0}", dbInfo.TypeID);
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
    }
}