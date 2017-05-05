using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.ResourceXmls;
using Wcf11101.Wcf11012;

namespace Wcf11101
{
    public partial class Service11101
    {
        private OperationReturn SaveResourcePropertyData(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     资源编码
                //1     属性总数
                //2...     属性信息
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strObjID = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("PropertyData count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("PropertyData count invalid");
                    return optReturn;
                }
                List<ResourceProperty> listPropertyValues = new List<ResourceProperty>();
                for (int i = 0; i < intCount; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ResourceProperty>(listParams[i + 2]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    ResourceProperty info = optReturn.Data as ResourceProperty;
                    if (info == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("PropertyValue is null");
                        return optReturn;
                    }
                    listPropertyValues.Add(info);
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1}"
                            , rentToken
                            , strObjID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1}"
                            , rentToken
                            , strObjID);
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
                    objAdapter.FillSchema(objDataSet, SchemaType.Mapped);
                    objAdapter.Fill(objDataSet);
                    List<string> listMsg = new List<string>();
                    for (int i = 0; i < listPropertyValues.Count; i++)
                    {
                        bool isAdd = false;
                        ResourceProperty info = listPropertyValues[i];
                        int propertyID = info.PropertyID;
                        //使用PropertyID计算行号和列号
                        int rowID = propertyID / 10;
                        int colID = propertyID - rowID * 10;
                        if (colID > 0)
                        {
                            rowID++;
                        }
                        else
                        {
                            colID = 10;
                        }
                        DataRow dr = objDataSet.Tables[0].Select(string.Format("C002 = {0}", rowID)).FirstOrDefault();
                        //如果不存在此行列，追加上
                        if (dr == null)
                        {
                            isAdd = true;
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strObjID;
                            dr["C002"] = rowID;
                        }
                        string strValue = EncodeEncryptValue(info, info.Value);
                        dr[string.Format("C{0}", (colID + 10).ToString("000"))] = strValue;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            string strMsg = string.Format("A{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, strObjID, rowID);
                            listMsg.Add(strMsg);
                        }
                        else
                        {
                            string strMsg = string.Format("M{0}{1}{0}{2}", ConstValue.SPLITER_CHAR, strObjID, rowID);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn SaveResourceChildObjectData(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     资源类型
                //1     资源编码
                //2     是否有上下级关系（0：没有，1：有）
                //3     子资源类型
                //4     子资源编码总数
                //5...     子资源编码列表
                if (listParams == null || listParams.Count < 5)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strObjType = listParams[0];
                string strObjID = listParams[1];
                string strIsParent = listParams[2];
                string strChildType = listParams[3];
                string strCount = listParams[4];
                int intChildType, intCount;
                if (!int.TryParse(strChildType, out intChildType))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ChildType or PropertyID param invalid");
                    return optReturn;
                }
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ChildObject count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 5)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ChildObject count invalid");
                    return optReturn;
                }
                List<string> listChildObjectIDs = new List<string>();
                for (int i = 0; i < intCount; i++)
                {
                    listChildObjectIDs.Add(listParams[i + 5]);
                }
                string strBeginID = string.Format("{0}0000000000000000", intChildType.ToString("000"));
                string strEndID = string.Format("{0}0000000000000000", (intChildType + 1).ToString("000"));
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql =
                            string.Format("SELECT * FROM T_11_101_{0} WHERE C001 >= {1} AND C001 < {2}"
                                , rentToken
                                , strBeginID
                                , strEndID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql =
                            string.Format("SELECT * FROM T_11_101_{0} WHERE C001 >= {1} AND C001 < {2}"
                                , rentToken
                                , strBeginID
                                , strEndID);
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
                    objAdapter.FillSchema(objDataSet, SchemaType.Mapped);
                    objAdapter.Fill(objDataSet);
                    List<string> listDeleteIDs = new List<string>();
                    List<string> listMsg = new List<string>();
                    //第一次遍历每行，首先找到需要删除的资源的ObjID
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        string strRow = dr["C002"].ToString();
                        string strParentID = dr["C013"].ToString();
                        if (strIsParent == "1")
                        {
                            if (strRow == "1" && strParentID == strObjID)
                            {
                                if (!listChildObjectIDs.Contains(strID)
                                    && !listDeleteIDs.Contains(strID))
                                {
                                    //添加到待删除集合中
                                    listDeleteIDs.Add(strID);
                                    string strLog = string.Format("D{0}{1}", ConstValue.SPLITER_CHAR, strID);
                                    listMsg.Add(strLog);
                                }
                            }
                        }
                        else
                        {
                            if (!listChildObjectIDs.Contains(strID)
                                   && !listDeleteIDs.Contains(strID))
                            {
                                //添加到待删除集合中
                                listDeleteIDs.Add(strID);
                                string strLog = string.Format("D{0}{1}", ConstValue.SPLITER_CHAR, strID);
                                listMsg.Add(strLog);
                            }
                        }

                    }
                    //第二次遍历每行，根据ObjID删除对应资源的属性信息
                    for (int i = objDataSet.Tables[0].Rows.Count - 1; i >= 0; i--)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string strID = dr["C001"].ToString();
                        if (listDeleteIDs.Contains(strID))
                        {
                            dr.Delete();
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
            }
            return optReturn;
        }

        private OperationReturn RemoveResourceObjectData(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     资源总数
                //1...     资源编码
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strCount = listParams[0];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource id count param invalid");
                    return optReturn;
                }
                if (listParams.Count < intCount + 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource id count invalid");
                    return optReturn;
                }
                if (intCount > 0)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.WSInsertTempData;
                    webRequest.ListData.Add(string.Empty);
                    webRequest.ListData.Add(intCount.ToString());
                    for (int i = 0; i < intCount; i++)
                    {
                        webRequest.ListData.Add(listParams[i + 1]);
                    }
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
                    string strID = webReturn.Data;
                    string rentToken = session.RentInfo.Token;
                    string strSql;
                    switch (session.DBType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "DELETE FROM T_11_101_{0} WHERE C001 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1})",
                                    rentToken,
                                    strID);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "DELETE FROM T_11_101_{0} WHERE C001 IN (SELECT C011 FROM T_00_901 WHERE C001 = {1})",
                                    rentToken,
                                    strID);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not support");
                            return optReturn;
                    }
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

        private OperationReturn GenerateResourceXml(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     资源编码,如果资源编码为0，生成完整的资源xml文件
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strResourceID = listParams[0];
                long resourceID;
                if (!long.TryParse(strResourceID, out resourceID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ResourceID param invalid");
                    return optReturn;
                }
                ResourceXmlHelper xmlhelper = new ResourceXmlHelper(session);
                xmlhelper.Init();
                string dir = Path.Combine(session.InstallPath, string.Format("Temp\\ResourceXml"));
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string path;
                if (resourceID == 0)
                {
                    path = Path.Combine(dir, string.Format("full.xml"));
                    optReturn = xmlhelper.GenerateAllResourceXmlFile(path);
                }
                else
                {
                    path = Path.Combine(dir, string.Format("{0}.xml", resourceID));
                    optReturn = xmlhelper.GenerateResourceXmlFile(resourceID, path);
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn.Message = path;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private string EncodeMultiValues(ResourceProperty info, string strValue)
        {
            string strReturn = strValue;
            //复合值
            if (info.MultiValueMode > 0)
            {
                strValue = string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR_2);
                strValue += string.Format("{0}", info.MultiValueMode);
                switch (info.MultiValueMode)
                {
                    case 1:
                        strValue += string.Format("{0}{1}", ConstValue.SPLITER_CHAR_2, info.Value);
                        for (int i = 0; i < info.ListOtherValues.Count; i++)
                        {
                            strValue += string.Format("{0}{1}",
                                ConstValue.SPLITER_CHAR_2,
                                info.ListOtherValues[i]);
                        }
                        break;
                }
                strReturn = strValue;
            }
            return strReturn;
        }

        private string EncodeEncryptValue(ResourceProperty info, string strValue)
        {
            string strReturn = EncodeMultiValues(info, strValue);
            //加密的
            if (info.EncryptMode > 0)
            {
                string strStart = string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR);
                switch (info.EncryptMode)
                {
                    case ObjectPropertyEncryptMode.E2Hex:
                        strReturn = string.Format("{0}2{1}hex{1}{2}", strStart, ConstValue.SPLITER_CHAR,
                            EncryptToDB(strReturn));
                        break;
                    case ObjectPropertyEncryptMode.SHA256:
                        strReturn = string.Format("{0}3{1}hex{1}{2}", strStart, ConstValue.SPLITER_CHAR,
                           EncryptToDB(strReturn));
                        break;
                }
            }
            return strReturn;
        }
    }
}