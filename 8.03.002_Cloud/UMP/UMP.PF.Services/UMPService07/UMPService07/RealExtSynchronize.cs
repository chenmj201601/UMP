//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7ebb7c52-8423-415c-8501-6ea481b5fe3a
//        CLR Version:              4.0.30319.18408
//        Name:                     RealExtSynchronize
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService07
//        File Name:                RealExtSynchronize
//
//        created by Charley at 2016/7/12 09:59:33
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService07;


namespace UMPService07
{
    public partial class SyncServer
    {
        private void CreateSyncRealExtThread()
        {
            try
            {
                if (mThreadSyncRealExtData != null)
                {
                    try
                    {
                        mThreadSyncRealExtData.Abort();
                    }
                    catch { }
                    mThreadSyncRealExtData = null;
                }
                mThreadSyncRealExtData = new Thread(SyncRealExtWorker);
                mThreadSyncRealExtData.Start();
                OnDebug(LogMode.Info,
                    string.Format("SyncRealExtData started.\t{0}", mThreadSyncRealExtData.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateSyncRealExtThread fail.\t{0}", ex.Message));
            }
        }

        private void StopSyncRealExtThread()
        {
            try
            {
                if (mThreadSyncRealExtData != null)
                {
                    try
                    {
                        mThreadSyncRealExtData.Abort();
                    }
                    catch { }
                    mThreadSyncRealExtData = null;
                    OnDebug(LogMode.Info, string.Format("SyncRealExtThread stopped"));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopSyncRealExtThread fail.\t{0}", ex.Message));
            }
        }

        private void SyncRealExtWorker()
        {
            try
            {
                while (true)
                {
                    DoSyncRealExtData();

                    Thread.Sleep(mSyncRealExtDataInterval * 1000);
                }
            }
            catch (Exception ex)
            {
                var abort = ex as ThreadAbortException;
                if (abort == null)
                {
                    OnDebug(LogMode.Error, string.Format("SyncRealExtWorker fail.\t{0}", ex.Message));
                }
            }
        }

        private void DoSyncRealExtData()
        {
            try
            {
                if (mSession == null
                   || mSession.DatabaseInfo == null)
                {
                    OnDebug(LogMode.Error, string.Format("SessionInfo or DatabaseInfo is null"));
                    return;
                }
                DatabaseInfo dbInfo = mSession.DatabaseInfo;
                string strConn = dbInfo.GetConnectionString();
                int dbType = dbInfo.TypeID;
                string rentToken = mSession.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                OperationReturn optReturn;
                int pbxDeviceCount = 0;
                int extCount = 0;
                long longValue;
                int intValue;
                string strValue;

                List<ResourceConfigInfo> listResourceInfos = new List<ResourceConfigInfo>();
                List<RealExtInfo> listExtInfos = new List<RealExtInfo>();


                #region 获取PBXDevice资源

                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                Service07Consts.RESOURCE_PBXDEVICE * 10000000000000000,
                                (Service07Consts.RESOURCE_PBXDEVICE + 1) * 10000000000000000);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadResourceInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                             string.Format(
                                 "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                 rentToken,
                                 Service07Consts.RESOURCE_PBXDEVICE * 10000000000000000,
                                 (Service07Consts.RESOURCE_PBXDEVICE + 1) * 10000000000000000);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadResourceInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                        return;
                }
                if (objDataSet == null)
                {
                    OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                    return;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    PBXDeviceConfigInfo pbxDevice = new PBXDeviceConfigInfo();
                    pbxDevice.ObjType = Service07Consts.RESOURCE_PBXDEVICE;
                    long objID = Convert.ToInt64(dr["C001"]);
                    pbxDevice.ObjID = objID;

                    var temp = listResourceInfos.FirstOrDefault(r => r.ObjID == pbxDevice.ObjID);
                    if (temp != null)
                    {
                        listResourceInfos.Remove(temp);
                    }
                    listResourceInfos.Add(pbxDevice);
                    pbxDeviceCount++;
                }

                #endregion


                #region 获取PBXDevice的配置信息

                for (int i = 0; i < listResourceInfos.Count; i++)
                {
                    var resource = listResourceInfos[i];
                    if (resource.ObjType != Service07Consts.RESOURCE_PBXDEVICE) { continue; }
                    var pbxDevice = resource as PBXDeviceConfigInfo;
                    if (pbxDevice == null) { continue; }
                    switch (dbType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2 OR C002 = 93) ORDER BY C001, C002",
                                    rentToken,
                                    pbxDevice.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadResourceInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2 OR C002 = 93) ORDER BY C001, C002",
                                    rentToken,
                                    pbxDevice.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadResourceInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                            return;
                    }
                    if (objDataSet == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                        return;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        int row = Convert.ToInt32(dr["C002"]);
                        if (row == 1)
                        {
                            int key = Convert.ToInt32(dr["C011"]);
                            pbxDevice.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            pbxDevice.ID = id;
                        }
                        if (row == 2)
                        {
                            strValue = dr["C012"].ToString();
                            if (int.TryParse(strValue, out intValue))
                            {
                                pbxDevice.CTIType = intValue;
                            }
                            else
                            {
                                pbxDevice.CTIType = 0;
                            }
                            strValue = dr["C013"].ToString();
                            if (int.TryParse(strValue, out intValue))
                            {
                                pbxDevice.DeviceType = intValue;
                            }
                            else
                            {
                                pbxDevice.DeviceType = 0;
                            }
                            strValue = dr["C014"].ToString();
                            if (int.TryParse(strValue, out intValue))
                            {
                                pbxDevice.MonitorType = intValue;
                            }
                            else
                            {
                                pbxDevice.MonitorType = 0;
                            }
                            pbxDevice.DeviceName = dr["C015"].ToString();
                        }
                        if (row == 93)
                        {
                            pbxDevice.XmlKey = dr["C011"].ToString();
                        }
                    }
                }

                #endregion


                #region 获取真实分机资源

                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                Service07Consts.RESOURCE_REALEXT * 10000000000000000,
                                (Service07Consts.RESOURCE_REALEXT + 1) * 10000000000000000);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadResourceInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                             string.Format(
                                 "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                 rentToken,
                                 Service07Consts.RESOURCE_REALEXT * 10000000000000000,
                                 (Service07Consts.RESOURCE_REALEXT + 1) * 10000000000000000);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadResourceInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                        return;
                }
                if (objDataSet == null)
                {
                    OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                    return;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    RealExtInfo realExt = new RealExtInfo();
                    realExt.ObjType = Service07Consts.RESOURCE_REALEXT;
                    long objID = Convert.ToInt64(dr["C001"]);
                    realExt.ObjID = objID;

                    var temp = listExtInfos.FirstOrDefault(r => r.ObjID == realExt.ObjID);
                    if (temp != null)
                    {
                        listExtInfos.Remove(temp);
                    }
                    listExtInfos.Add(realExt);
                    extCount++;
                }

                #endregion


                #region 获取真实分机配置信息

                for (int i = 0; i < listExtInfos.Count; i++)
                {
                    var resource = listExtInfos[i];
                    if (resource.ObjType != Service07Consts.RESOURCE_REALEXT) { continue; }
                    var ext = resource;
                    switch (dbType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    ext.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadResourceInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    ext.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadResourceInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                            return;
                    }
                    if (objDataSet == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                        return;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        int row = Convert.ToInt32(dr["C002"]);
                        if (row == 1)
                        {
                            strValue = dr["C011"].ToString();
                            if (long.TryParse(strValue, out longValue))
                            {
                                ext.OrgID = longValue;
                            }
                            else
                            {
                                ext.OrgID = 0;
                            }
                            ext.Status = dr["C012"].ToString();
                            ext.IsNew = dr["C013"].ToString() == "1";
                            ext.IsLock = dr["C014"].ToString() == "1";
                            ext.LockMethod = dr["C015"].ToString();
                            ext.SourceType = dr["C016"].ToString();
                            strValue = dr["C017"].ToString();
                            strValue = DecryptFromDB(strValue);
                            ext.Name = strValue;
                            string[] arrValue = strValue.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.RemoveEmptyEntries);
                            if (arrValue.Length > 0)
                            {
                                ext.Extension = arrValue[0];
                            }
                            strValue = dr["C018"].ToString();
                            strValue = DecryptFromDB(strValue);
                            ext.ChanName = strValue;
                        }
                        if (row == 2)
                        {

                        }
                    }
                }

                #endregion


                OnDebug(LogMode.Debug,
                   string.Format(
                       "LoadResourceInfos end.\tPBXDeviceCount:{0};\tRealExtCount:{1};",
                       pbxDeviceCount,
                       extCount));

                List<RealExtInfo> listAddExtensions = new List<RealExtInfo>();
                List<RealExtInfo> listModifyExtensions = new List<RealExtInfo>();
                List<RealExtInfo> listDeleteExtensions = new List<RealExtInfo>();


                #region 同步分机信息

                for (int i = 0; i < listResourceInfos.Count; i++)
                {
                    var resource = listResourceInfos[i];
                    if (resource.ObjType == Service07Consts.RESOURCE_PBXDEVICE)
                    {
                        var pbxDevice = resource as PBXDeviceConfigInfo;
                        if (pbxDevice == null) { continue; }
                        if (pbxDevice.DeviceType != 1) { continue; }    //只需处理设备类型为1（坐席分机）设备
                        string strExt = pbxDevice.DeviceName;
                        var extension = listExtInfos.FirstOrDefault(e => e.Extension == strExt);
                        if (extension == null)
                        {
                            //不存在，则增加
                            optReturn = GetSerialID(Service07Consts.MODULE_BASEMODULE, Service07Consts.RESOURCE_REALEXT, true);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("GetSerialID fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                continue;
                            }
                            extension = new RealExtInfo();
                            extension.ObjID = Convert.ToInt64(optReturn.Data);
                            extension.ObjType = Service07Consts.RESOURCE_REALEXT;

                            extension.OrgID = ConstValue.ORG_ROOT;
                            extension.Status = "1";
                            extension.IsNew = true;
                            extension.IsLock = false;
                            extension.LockMethod = "N";
                            extension.SourceType = "09";
                            extension.Name = string.Format("{0}", strExt);
                            extension.Extension = strExt;
                            extension.ChanName = strExt;

                            //不能添加重复的分机
                            var temp = listAddExtensions.FirstOrDefault(e => e.Extension == extension.Extension);
                            if (temp == null)
                            {
                                listAddExtensions.Add(extension);
                            }

                            //listAddExtensions.Add(extension);
                        }
                        else
                        {
                            //更新
                            strValue = extension.Status;
                            extension.Status = strValue == "0" ? "1" : strValue;  //如果原来是被删除的分机，重新恢复成正常的分机
                            extension.ChanName = strExt;

                            listModifyExtensions.Add(extension);
                        }
                    }
                }

                #endregion


                #region 删除的分机打上删除标记

                for (int i = 0; i < listExtInfos.Count; i++)
                {
                    //即不在listAddExtensions列表中，又不在listModifyExtensions中的分机是要被删除的分机，需要打上删除标记
                    var extension = listExtInfos[i];
                    if (!listAddExtensions.Contains(extension)
                        && !listModifyExtensions.Contains(extension))
                    {
                        extension.Status = "0";
                        listDeleteExtensions.Add(extension);
                    }
                }
                for (int i = 0; i < listDeleteExtensions.Count; i++)
                {
                    //删除的分机也添加到listModifyExtensions列表中，以便后面写入到数据中
                    var extension = listDeleteExtensions[i];
                    listModifyExtensions.Add(extension);
                }

                #endregion


                #region 新增的分机写入数据库

                if (listAddExtensions.Count > 0)
                {
                    int count = listAddExtensions.Count;
                    OnDebug(LogMode.Debug, string.Format("Begin add extension to database.\t{0}", count));

                    var gp = mListGlobalParamInfos.FirstOrDefault(p => p.ParamID == Service07Consts.GP_DEFULT_PASSWORD);
                    if (gp == null)
                    {
                        OnDebug(LogMode.Error, string.Format("DefaultPassword param is null"));
                        return;
                    }
                    string strDefaultPassword = gp.ParamValue;
                    if (string.IsNullOrEmpty(strDefaultPassword))
                    {
                        OnDebug(LogMode.Error, string.Format("DefaultPassword param invalid"));
                        return;
                    }

                    IDbConnection objConn;
                    IDbDataAdapter objAdapter;
                    DbCommandBuilder objCmdBuilder;
                    switch (dbType)
                    {
                        case 2:
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE 1 = 2", rentToken);
                            objConn = MssqlOperation.GetConnection(strConn);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE 1 = 2", rentToken);
                            objConn = OracleOperation.GetConnection(strConn);
                            objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type invalid.\t{0}", dbType));
                            return;
                    }
                    if (objConn == null || objAdapter == null || objCmdBuilder == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataAdapter is null"));
                        return;
                    }
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    try
                    {
                        objDataSet = new DataSet();
                        objAdapter.Fill(objDataSet);

                        for (int i = 0; i < listAddExtensions.Count; i++)
                        {
                            var ext = listAddExtensions[i];

                            DataRow dr1 = objDataSet.Tables[0].NewRow();
                            DataRow dr2 = objDataSet.Tables[0].NewRow();

                            dr1["C001"] = ext.ObjID;
                            dr1["C002"] = 1;
                            dr1["C011"] = ext.OrgID;
                            dr1["C012"] = ext.Status;
                            dr1["C013"] = ext.IsNew ? "1" : "0";
                            dr1["C014"] = ext.IsLock ? "1" : "0";
                            dr1["C015"] = ext.LockMethod;
                            dr1["C016"] = ext.SourceType;
                            strValue = ext.Name;
                            strValue = EncryptToDB(strValue);
                            dr1["C017"] = strValue;
                            strValue = ext.ChanName;
                            strValue = EncryptToDB(strValue);
                            dr1["C018"] = strValue;
                            strValue = ext.ObjID.ToString();
                            strValue = string.Format("{0}{1}", strValue, strDefaultPassword);
                            strValue = EncryptSHA512(strValue);
                            dr1["C020"] = strValue;

                            dr2["C001"] = ext.ObjID;
                            dr2["C002"] = 2;
                            dr2["C011"] = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
                            dr2["C012"] = 0;
                            dr2["C013"] = 0;

                            objDataSet.Tables[0].Rows.Add(dr1);
                            objDataSet.Tables[0].Rows.Add(dr2);


                            #region 增加住户管理员的管理权限

                            optReturn = null;
                            switch (dbType)
                            {
                                case 2:
                                    strSql = string.Format("INSERT INTO T_11_201_{0} VALUES (0, 0, {1}, {2}, '{3}', '{4}')",
                                        rentToken,
                                        string.Format("102{0}00000000001", rentToken),
                                        ext.ObjID,
                                        DateTime.Parse("2014/1/1").ToString("yyyy-MM-dd HH:mm:ss"),
                                        DateTime.Parse("2199/12/31 23:59:59").ToString("yyyy-MM-dd HH:mm:ss"));
                                    optReturn = MssqlOperation.ExecuteSql(strConn, strSql);
                                    break;
                                case 3:
                                    strSql = string.Format("INSERT INTO T_11_201_{0} VALUES (0, 0, {1}, {2}, TO_DATE('{3}','YYYY-MM-DD HH24:MI:SS'), TO_DATE('{4}','YYYY-MM-DD HH24:MI:SS'))",
                                        rentToken,
                                        string.Format("102{0}00000000001", rentToken),
                                        ext.ObjID,
                                        DateTime.Parse("2014/1/1").ToString("yyyy-MM-dd HH:mm:ss"),
                                        DateTime.Parse("2199/12/31 23:59:59").ToString("yyyy-MM-dd HH:mm:ss"));
                                    optReturn = OracleOperation.ExecuteSql(strConn, strSql);
                                    break;
                                default:
                                    OnDebug(LogMode.Error, string.Format("Database type invalid.\t{0}", dbType));
                                    break;
                            }
                            if (optReturn != null
                                && !optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("Insert11201 fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            }

                            #endregion

                        }

                        objAdapter.Update(objDataSet);
                        objDataSet.AcceptChanges();
                    }
                    catch (Exception ex)
                    {
                        OnDebug(LogMode.Error, string.Format("Add extension fail.\t{0}", ex.Message));
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


                #region 修改的分机(包括打上删除标记的分机）写入数据库

                if (listModifyExtensions.Count > 0)
                {
                    int count = listModifyExtensions.Count;
                    OnDebug(LogMode.Debug, string.Format("Begin modify extension to database.\t{0}", count));

                    for (int i = 0; i < listModifyExtensions.Count; i++)
                    {
                        var ext = listModifyExtensions[i];

                        IDbConnection objConn;
                        IDbDataAdapter objAdapter;
                        DbCommandBuilder objCmdBuilder;
                        switch (dbType)
                        {
                            case 2:
                                strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001,C002", rentToken,
                                    ext.ObjID);
                                objConn = MssqlOperation.GetConnection(strConn);
                                objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                                objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                                break;
                            case 3:
                                strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001,C002", rentToken,
                                    ext.ObjID);
                                objConn = OracleOperation.GetConnection(strConn);
                                objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                                objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                                break;
                            default:
                                OnDebug(LogMode.Error, string.Format("Database type invalid.\t{0}", dbType));
                                return;
                        }
                        if (objConn == null || objAdapter == null || objCmdBuilder == null)
                        {
                            OnDebug(LogMode.Error, string.Format("ObjDataAdapter is null"));
                            return;
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

                                int rowID = Convert.ToInt32(dr["C002"]);
                                if (rowID == 1)
                                {
                                    dr["C012"] = ext.Status;
                                    strValue = ext.ChanName;
                                    strValue = EncryptToDB(strValue);
                                    dr["C018"] = strValue;
                                }
                                if (rowID == 2)
                                {

                                }
                            }

                            objAdapter.Update(objDataSet);
                            objDataSet.AcceptChanges();
                        }
                        catch (Exception ex)
                        {
                            OnDebug(LogMode.Error, string.Format("Modify extension fail.\t{0}", ex.Message));
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

                #endregion

            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DoSyncRealExtData fail.\t{0}", ex.Message));
            }
        }
    }
}
