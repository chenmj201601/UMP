using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common000A1;

namespace Wcf000A1
{
    public partial class Service000A1
    {
        private OperationReturn SyncExtension(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                #region 解析参数

                //ListParams
                //参考S000ACodes中的说明，此处从略
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strTenantOrgToken = listParams[1];
                string strCount = listParams[2];
                WriteOperationLog(string.Format("SyncExtension:\tUserID:{0};TenantOrgToken:{1};Count:{2}", strUserID,
                    strTenantOrgToken, strCount));

                #endregion


                #region 检查参数有效性

                int tenantOrgToken;
                if (!int.TryParse(strTenantOrgToken, out tenantOrgToken)
                    || tenantOrgToken <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param TenantOrgToken invalid.");
                    return optReturn;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                   || intCount < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param Count invalid.");
                    return optReturn;
                }
                if (listParams.Count < intCount + 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Extension Count invalid.");
                    return optReturn;
                }

                #endregion


                #region 得到分机列表

                List<ExtensionInfo> listExtensions = new List<ExtensionInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = listParams[i + 3];
                    JsonObject jsonExt = null;
                    try
                    {
                        jsonExt = new JsonObject(strInfo);
                    }
                    catch { }
                    if (jsonExt != null)
                    {
                        ExtensionInfo info = new ExtensionInfo();
                        if (jsonExt[S000AConsts.FIELD_NAME_EXTENSIONINFO_EXTENSION] != null)
                        {
                            info.Extension = jsonExt[S000AConsts.FIELD_NAME_EXTENSIONINFO_EXTENSION].Value;
                        }
                        if (jsonExt[S000AConsts.FIELD_NAME_EXTENSIONINFO_CHANNELNAME] != null)
                        {
                            info.ChannelName = jsonExt[S000AConsts.FIELD_NAME_EXTENSIONINFO_CHANNELNAME].Value;
                        }
                        if (jsonExt[S000AConsts.FIELD_NAME_EXTENSIONINFO_SERVERIP] != null)
                        {
                            info.ServerIP = jsonExt[S000AConsts.FIELD_NAME_EXTENSIONINFO_SERVERIP].Value;
                        }
                        if (jsonExt[S000AConsts.FIELD_NAME_EXTENSIONINFO_CHANNELID] != null)
                        {
                            info.ChannelID = (int)jsonExt[S000AConsts.FIELD_NAME_EXTENSIONINFO_CHANNELID].Number;
                        }
                        listExtensions.Add(info);
                    }
                }
                WriteOperationLog(string.Format("ListExtension:{0}", listExtensions.Count));

                #endregion


                #region 获取数据库信息

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
                WriteOperationLog(string.Format("DatabaseInfo:{0}", dbInfo));

                #endregion


                #region 根据TenantOrgToken获取数据库中分机信息列表

                string strConn = dbInfo.GetConnectionString();
                string strRentToken = "00000";
                string strSql;
                int dbType = dbInfo.TypeID;
                DataSet objDataSet;
                int addNum = 0;
                int modifyNum = 0;
                List<string> listReturn = new List<string>();


                #region 租户机构编号

                string strOrgID = string.Format("10114010100000{0}", tenantOrgToken.ToString("00000"));
                long orgID = long.Parse(strOrgID);
                long orgAdminID = 0;

                #endregion


                #region 获取租户管理员编号

                if (tenantOrgToken > 0)
                {
                    switch (dbType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006 = {1} AND C027 = '1'",
                                strRentToken, orgID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C006 = {1} AND C027 = '1'",
                              strRentToken, orgID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
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
                    objDataSet = optReturn.Data as DataSet;
                    if (objDataSet == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("DataSet is null.");
                        return optReturn;
                    }
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_NOT_EXIST;
                        optReturn.Message = string.Format("TenantOrg admin not exist.");
                        return optReturn;
                    }
                    DataRow dr = objDataSet.Tables[0].Rows[0];
                    orgAdminID = Convert.ToInt64(dr["C001"]);
                }
                if (orgAdminID <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("TenantOrg admin invalid.");
                    return optReturn;
                }

                #endregion


                #region 首先获取该TenantOrgToken下的分机编号

                List<ExtensionInfo> listExts = new List<ExtensionInfo>();
                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT C001 FROM T_11_101_{0} WHERE C001 > 1040000000000000000 AND C001 < 1050000000000000000 AND C002 = 3 AND C011 = '{1}' ORDER BY C001, C002",
                                strRentToken, tenantOrgToken);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT C001 FROM T_11_101_{0} WHERE C001 > 1040000000000000000 AND C001 < 1050000000000000000 AND C002 = 3 AND C011 = '{1}' ORDER BY C001, C002",
                               strRentToken, tenantOrgToken);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
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
                    long id = Convert.ToInt64(dr["C001"]);
                    ExtensionInfo info = new ExtensionInfo();
                    info.ObjID = id;
                    listExts.Add(info);
                }

                #endregion


                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;


                #region 遍历每个分机，获取分机的配置信息，然后更新分机信息

                bool isModify;
                for (int i = 0; i < listExts.Count; i++)
                {
                    ExtensionInfo extInfo = listExts[i];
                    long id = extInfo.ObjID;
                    switch (dbType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001, C002",
                                strRentToken, id);
                            objConn = MssqlOperation.GetConnection(strConn);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001, C002",
                                strRentToken, id);
                            objConn = OracleOperation.GetConnection(strConn);
                            objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid");
                            return optReturn;
                    }
                    if (objConn == null || objAdapter == null || objCmdBuilder == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Database object is null");
                        return optReturn;
                    }
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    try
                    {
                        objDataSet = new DataSet();
                        objAdapter.Fill(objDataSet);

                        for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[j];

                            #region 更新

                            isModify = false;
                            int rowID = Convert.ToInt32(dr["C002"]);
                            if (rowID == 1)
                            {
                                string strExt01017 = dr["C017"].ToString();     //01017代表分机名称（分机号+IP的形式）
                                strExt01017 = DecryptFromDB(strExt01017);
                                string[] arrInfos = strExt01017.Split(new[] { ConstValue.SPLITER_CHAR },
                                    StringSplitOptions.None);
                                if (arrInfos.Length > 0)
                                {
                                    string strExt = arrInfos[0];
                                    extInfo.Extension = strExt;
                                }
                                string strExt01018 = dr["C018"].ToString();
                                strExt01018 = DecryptFromDB(strExt01018);
                                extInfo.ChannelName = strExt01018;
                            }
                            var temp = listExtensions.FirstOrDefault(e => e.Extension == extInfo.Extension);
                            if (temp != null)
                            {
                                temp.ObjID = id;
                                extInfo.ChannelName = temp.ChannelName;
                                isModify = true;
                                modifyNum++;
                            }
                            if (isModify)
                            {
                                string strExt01018 = extInfo.ChannelName;
                                strExt01018 = EncryptToDB(strExt01018);
                                if (rowID == 1)
                                {
                                    dr["C012"] = "1";
                                    dr["C013"] = "0";
                                    dr["C014"] = "0";
                                    dr["C015"] = "N";
                                    dr["C018"] = strExt01018;
                                }
                            }

                            #endregion

                        }

                        objAdapter.Update(objDataSet);
                        objDataSet.AcceptChanges();
                    }
                    catch (Exception ex)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = ex.Message;
                        optReturn.Exception = ex;
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

                #endregion


                #region 遍历每个分机，如果是需要新增的，增加分机信息

                bool isAdd = false;
                for (int i = 0; i < listExtensions.Count; i++)
                {
                    ExtensionInfo extInfo = listExtensions[i];
                    long id = extInfo.ObjID;
                    if (id <= 0)
                    {
                        isAdd = isAdd || true;
                    }
                }
                if (isAdd)
                {
                    switch (dbType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE 1 = 2 ORDER BY C001, C002",
                                strRentToken);
                            objConn = MssqlOperation.GetConnection(strConn);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE 1 = 2 ORDER BY C001, C002",
                                strRentToken);
                            objConn = OracleOperation.GetConnection(strConn);
                            objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid");
                            return optReturn;
                    }
                    if (objConn == null || objAdapter == null || objCmdBuilder == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Database object is null");
                        return optReturn;
                    }
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;

                    try
                    {
                        objDataSet = new DataSet();
                        objAdapter.Fill(objDataSet);

                        for (int i = 0; i < listExtensions.Count; i++)
                        {
                            ExtensionInfo extInfo = listExtensions[i];
                            long id = extInfo.ObjID;
                            if (id <= 0)
                            {

                                #region 生成流水号

                                List<string> listSubParams = new List<string>();
                                listSubParams.Add("11");
                                listSubParams.Add("104");
                                listSubParams.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                optReturn = GetSerialID(listSubParams);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                id = Convert.ToInt64(optReturn.Data.ToString());
                                extInfo.ObjID = id;

                                #endregion


                                #region 新增

                                DataRow dr1 = objDataSet.Tables[0].NewRow();
                                dr1["C001"] = id;
                                dr1["C002"] = 1;
                                dr1["C011"] = orgID;
                                dr1["C012"] = "1";
                                dr1["C013"] = "1";
                                dr1["C014"] = "0";
                                dr1["C015"] = "N";
                                dr1["C016"] = "05";
                                string str01017 = string.Format("{0}{1}{2}", extInfo.Extension, ConstValue.SPLITER_CHAR,
                                    extInfo.ServerIP);
                                str01017 = EncryptToDB(str01017);
                                dr1["C017"] = str01017;
                                string str01018 = extInfo.ChannelName;
                                str01018 = EncryptToDB(str01018);
                                dr1["C018"] = str01018;

                                DataRow dr2 = objDataSet.Tables[0].NewRow();
                                dr2["C001"] = id;
                                dr2["C002"] = 2;

                                DataRow dr3 = objDataSet.Tables[0].NewRow();
                                dr3["C001"] = id;
                                dr3["C002"] = 3;
                                dr3["C011"] = tenantOrgToken;

                                objDataSet.Tables[0].Rows.Add(dr1);
                                objDataSet.Tables[0].Rows.Add(dr2);
                                objDataSet.Tables[0].Rows.Add(dr3);

                                addNum++;

                                #endregion


                                #region 新增的分机要添加到系统管理员权限列表中

                                if (id > 0)
                                {
                                    switch (dbType)
                                    {
                                        case 2:
                                            strSql =
                                                string.Format(
                                                    "INSERT INTO T_11_201_{0} VALUES (0, 0, {1}, {2}, '2014/1/1', '2199/12/31')",
                                                    strRentToken, ConstValue.USER_ADMIN, id);
                                            optReturn = MssqlOperation.ExecuteSql(strConn, strSql);
                                            break;
                                        case 3:
                                            strSql =
                                              string.Format(
                                                  "INSERT INTO T_11_201_{0} VALUES (0, 0, {1}, {2}, TO_DATE('2014/1/1','YYYY-MM-DD HH24:MI:SS'),TO_DATE('2199/12/31','YYYY-MM-DD HH24:MI:SS'))",
                                                  strRentToken, ConstValue.USER_ADMIN, id);
                                            optReturn = OracleOperation.ExecuteSql(strConn, strSql);
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
                                }

                                #endregion


                                #region 新增的分机要添加到租户管理员的管理权限列表中

                                if (id > 0)
                                {
                                    switch (dbType)
                                    {
                                        case 2:
                                            strSql =
                                                string.Format(
                                                    "INSERT INTO T_11_201_{0} VALUES (0, 0, {1}, {2}, '2014/1/1', '2199/12/31')",
                                                    strRentToken, orgAdminID, id);
                                            optReturn = MssqlOperation.ExecuteSql(strConn, strSql);
                                            break;
                                        case 3:
                                            strSql =
                                              string.Format(
                                                  "INSERT INTO T_11_201_{0} VALUES (0, 0, {1}, {2}, TO_DATE('2014/1/1','YYYY-MM-DD HH24:MI:SS'),TO_DATE('2199/12/31','YYYY-MM-DD HH24:MI:SS'))",
                                                  strRentToken, orgAdminID, id);
                                            optReturn = OracleOperation.ExecuteSql(strConn, strSql);
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
                                }

                                #endregion

                            }
                        }

                        objAdapter.Update(objDataSet);
                        objDataSet.AcceptChanges();

                    }
                    catch (Exception ex)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = ex.Message;
                        optReturn.Exception = ex;
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

                #endregion


                listReturn.Add(addNum.ToString());
                listReturn.Add(modifyNum.ToString());
                optReturn.Data = listReturn;

                #endregion

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

    }


    public class ExtensionInfo
    {
        public long ObjID { get; set; }
        public string Extension { get; set; }
        public string ChannelName { get; set; }
        public string ServerIP { get; set; }
        public int ChannelID { get; set; }
    }
}