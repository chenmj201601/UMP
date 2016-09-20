//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9506af81-427d-48b8-a34c-ade27b8a9cc6
//        CLR Version:              4.0.30319.18408
//        Name:                     ExtensionSynchronize
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService07
//        File Name:                ExtensionSynchronize
//
//        created by Charley at 2016/7/12 09:57:50
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
        private void CreateSyncExtensionThread()
        {
            try
            {
                if (mThreadSyncExtensionData != null)
                {
                    try
                    {
                        mThreadSyncExtensionData.Abort();
                    }
                    catch { }
                    mThreadSyncExtensionData = null;
                }
                mThreadSyncExtensionData = new Thread(SyncExtensionWorker);
                mThreadSyncExtensionData.Start();
                OnDebug(LogMode.Info,
                    string.Format("SyncExtensionData started.\t{0}", mThreadSyncExtensionData.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateSyncExtensionThread fail.\t{0}", ex.Message));
            }
        }

        private void StopSyncExtensionThread()
        {
            try
            {
                if (mThreadSyncExtensionData != null)
                {
                    try
                    {
                        mThreadSyncExtensionData.Abort();
                    }
                    catch { }
                    mThreadSyncExtensionData = null;
                    OnDebug(LogMode.Info, string.Format("SyncExtensionThread stopped"));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopSyncExtensionThread fail.\t{0}", ex.Message));
            }
        }

        private void SyncExtensionWorker()
        {
            try
            {
                while (true)
                {
                    //DoSyncExtensionData();
                    SyncVoiceExtensionData();
                    SyncScreenExtensionData();

                    Thread.Sleep(mSyncExtensionDataInterval * 1000);
                }
            }
            catch (Exception ex)
            {
                var abort = ex as ThreadAbortException;
                if (abort == null)
                {
                    OnDebug(LogMode.Error, string.Format("SyncExtensionWorker fail.\t{0}", ex.Message));
                }
            }
        }

        private void SyncVoiceExtensionData()
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
                int voiceCount = 0;
                int voiceChanCount = 0;
                int extCount = 0;
                long longValue;
                int intValue;
                string strValue;

                List<ResourceConfigInfo> listResourceInfos = new List<ResourceConfigInfo>();
                List<ExtensionInfo> listExtInfos = new List<ExtensionInfo>();


                #region 获取VoiceServer资源

                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                Service07Consts.RESOURCE_VOICESERVER * 10000000000000000,
                                (Service07Consts.RESOURCE_VOICESERVER + 1) * 10000000000000000);
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
                                 Service07Consts.RESOURCE_VOICESERVER * 10000000000000000,
                                 (Service07Consts.RESOURCE_VOICESERVER + 1) * 10000000000000000);
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

                    ServiceConfigInfo voice = new ServiceConfigInfo();
                    voice.ObjType = Service07Consts.RESOURCE_VOICESERVER;
                    long objID = Convert.ToInt64(dr["C001"]);
                    voice.ObjID = objID;

                    var temp = listResourceInfos.FirstOrDefault(r => r.ObjID == voice.ObjID);
                    if (temp != null)
                    {
                        listResourceInfos.Remove(temp);
                    }
                    listResourceInfos.Add(voice);
                    voiceCount++;
                }

                #endregion


                #region 获取VoiceServer的配置信息

                for (int i = 0; i < listResourceInfos.Count; i++)
                {
                    var resource = listResourceInfos[i];
                    if (resource.ObjType != Service07Consts.RESOURCE_VOICESERVER) { continue; }
                    var voice = resource as ServiceConfigInfo;
                    if (voice == null) { continue; }
                    switch (dbType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 91) ORDER BY C001, C002",
                                    rentToken,
                                    voice.ObjID);
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
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 91) ORDER BY C001, C002",
                                    rentToken,
                                    voice.ObjID);
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
                            voice.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            voice.ID = id;
                            string strHostAddress = dr["C017"].ToString();
                            strHostAddress = DecodeEncryptValue(strHostAddress);
                            voice.HostAddress = strHostAddress;
                            string strHostName = dr["C018"].ToString();
                            strHostName = DecodeEncryptValue(strHostName);
                            voice.HostName = strHostName;
                            string strPort = dr["C019"].ToString();
                            strPort = DecodeEncryptValue(strPort);
                            if (int.TryParse(strPort, out intValue))
                            {
                                voice.HostPort = intValue;
                            }
                            else
                            {
                                voice.HostPort = 0;
                            }
                        }
                        if (row == 91)
                        {
                            voice.Continent = dr["C011"].ToString();
                            voice.Country = dr["C012"].ToString();
                        }
                    }
                }

                #endregion


                #region 获取录音通道资源

                for (int i = 0; i < listResourceInfos.Count; i++)
                {
                    var resource = listResourceInfos[i];
                    if (resource.ObjType != Service07Consts.RESOURCE_VOICESERVER) { continue; }
                    var voice = resource as ServiceConfigInfo;
                    if (voice == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 AND C013 = '{3}' ORDER BY C001, C002",
                                    rentToken,
                                    Service07Consts.RESOURCE_VOICECHANNEL * 10000000000000000,
                                    (Service07Consts.RESOURCE_VOICECHANNEL + 1) * 10000000000000000,
                                    voice.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 AND C013 = '{3}' ORDER BY C001, C002",
                                    rentToken,
                                    Service07Consts.RESOURCE_VOICECHANNEL * 10000000000000000,
                                    (Service07Consts.RESOURCE_VOICECHANNEL + 1) * 10000000000000000,
                                    voice.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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

                        ChanConfigInfo channel = new ChanConfigInfo();
                        channel.ObjType = Service07Consts.RESOURCE_VOICECHANNEL;
                        long objID = Convert.ToInt64(dr["C001"]);
                        channel.ObjID = objID;

                        var temp = listResourceInfos.FirstOrDefault(r => r.ObjID == channel.ObjID);
                        if (temp != null)
                        {
                            listResourceInfos.Remove(temp);
                        }
                        voiceChanCount++;
                        voice.ListChildObjects.Add(channel);
                        listResourceInfos.Add(channel);
                    }
                }

                #endregion


                #region 获取录音通道配置信息

                for (int i = 0; i < listResourceInfos.Count; i++)
                {
                    var resource = listResourceInfos[i];
                    if (resource.ObjType != Service07Consts.RESOURCE_VOICECHANNEL) { continue; }
                    var channel = resource as ChanConfigInfo;
                    if (channel == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 or C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    channel.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 or C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    channel.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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
                            channel.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            channel.ID = id;
                            long parentObjID = Convert.ToInt64(dr["C013"]);
                            channel.ParentObjID = parentObjID;
                        }
                        if (row == 2)
                        {
                            string strChanName = dr["C011"].ToString();
                            channel.ChanName = strChanName;
                            string strExtension = dr["C012"].ToString();
                            channel.Extension = strExtension;
                        }
                    }
                }

                #endregion


                #region 获取分机资源

                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                Service07Consts.RESOURCE_EXTENSION * 10000000000000000,
                                (Service07Consts.RESOURCE_EXTENSION + 1) * 10000000000000000);
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
                                 Service07Consts.RESOURCE_EXTENSION * 10000000000000000,
                                 (Service07Consts.RESOURCE_EXTENSION + 1) * 10000000000000000);
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

                    ExtensionInfo ext = new ExtensionInfo();
                    ext.ObjType = Service07Consts.RESOURCE_EXTENSION;
                    long objID = Convert.ToInt64(dr["C001"]);
                    ext.ObjID = objID;

                    var temp = listExtInfos.FirstOrDefault(r => r.ObjID == ext.ObjID);
                    if (temp != null)
                    {
                        listExtInfos.Remove(temp);
                    }
                    listExtInfos.Add(ext);
                    extCount++;
                }

                #endregion


                #region 获取分机配置信息

                for (int i = 0; i < listExtInfos.Count; i++)
                {
                    var resource = listExtInfos[i];
                    if (resource.ObjType != Service07Consts.RESOURCE_EXTENSION) { continue; }
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
                            string strRole = string.Empty;
                            string strIP1 = string.Empty;
                            string strIP2 = string.Empty;
                            string[] arrValue = strValue.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.None);
                            if (arrValue.Length > 0)
                            {
                                ext.Extension = arrValue[0];
                            }
                            if (arrValue.Length > 1)
                            {
                                strIP1 = arrValue[1];
                            }
                            if (arrValue.Length > 2)
                            {
                                strRole = arrValue[2];
                            }
                            if (arrValue.Length > 3)
                            {
                                strIP2 = arrValue[3];
                            }
                            int intRole;
                            if (int.TryParse(strRole, out intRole))
                            {
                                if (intRole == 1)
                                {
                                    ext.Role = intRole;
                                    ext.VoiceIP = strIP1;
                                }
                                else if (intRole == 2)
                                {
                                    ext.Role = intRole;
                                    ext.ScreenIP = strIP1;
                                }
                                else if (intRole == 3)
                                {
                                    ext.Role = intRole;
                                    ext.VoiceIP = strIP1;
                                    ext.ScreenIP = strIP2;
                                }
                            }
                            strValue = dr["C018"].ToString();
                            strValue = DecryptFromDB(strValue);
                            ext.ChanName = strValue;
                        }
                        if (row == 2)
                        {
                            strValue = dr["C015"].ToString();
                            if (long.TryParse(strValue, out longValue))
                            {
                                ext.VoiceObjID = longValue;
                            }
                            else
                            {
                                ext.VoiceObjID = 0;
                            }
                            strValue = dr["C016"].ToString();
                            if (int.TryParse(strValue, out intValue))
                            {
                                ext.VoiceChanID = intValue;
                            }
                            else
                            {
                                ext.VoiceChanID = 0;
                            }
                            strValue = dr["C017"].ToString();
                            if (long.TryParse(strValue, out longValue))
                            {
                                ext.VoiceChanObjID = longValue;
                            }
                            else
                            {
                                ext.VoiceChanObjID = 0;
                            }
                            strValue = dr["C018"].ToString();
                            if (long.TryParse(strValue, out longValue))
                            {
                                ext.ScreenObjID = longValue;
                            }
                            else
                            {
                                ext.ScreenObjID = 0;
                            }
                            strValue = dr["C019"].ToString();
                            if (int.TryParse(strValue, out intValue))
                            {
                                ext.ScreenChanID = intValue;
                            }
                            else
                            {
                                ext.ScreenChanID = 0;
                            }
                            strValue = dr["C020"].ToString();
                            if (long.TryParse(strValue, out longValue))
                            {
                                ext.ScreenChanObjID = longValue;
                            }
                            else
                            {
                                ext.ScreenChanObjID = 0;
                            }

                            #region 为了与老版本程序兼容，做一下处理得到分机所在服务器IP

                            if (ext.Role <= 0)
                            {
                                string strName = ext.Name;
                                string[] arrName = strName.Split(new[] { ConstValue.SPLITER_CHAR },
                                    StringSplitOptions.RemoveEmptyEntries);
                                string strIP = string.Empty;
                                if (arrName.Length > 1)
                                {
                                    strIP = arrName[1];
                                }
                                if (!string.IsNullOrEmpty(strIP))
                                {
                                    if (ext.VoiceObjID > 0)
                                    {
                                        ext.VoiceIP = strIP;
                                        ext.Role = ext.Role | 1;
                                    }
                                    if (ext.ScreenObjID > 0)
                                    {
                                        ext.ScreenIP = strIP;
                                        ext.Role = ext.Role | 2;
                                    }
                                }
                            }

                            #endregion

                        }
                    }
                }

                #endregion


                OnDebug(LogMode.Debug,
                    string.Format(
                        "LoadResourceInfos end.\tVoiceCount:{0};\tVoiceChanCount:{1};\tExtensionCount:{2}",
                        voiceCount,
                        voiceChanCount,
                        extCount));

                for (int i = 0; i < listExtInfos.Count; i++)
                {
                    OnDebug(LogMode.Debug, string.Format("{0}", listExtInfos[i].LogInfo()));
                }

                List<ExtensionInfo> listAddExtensions = new List<ExtensionInfo>();
                List<ExtensionInfo> listModifyExtensions = new List<ExtensionInfo>();
                List<ExtensionInfo> listDeleteExtensions = new List<ExtensionInfo>();


                #region 同步分机信息

                for (int i = 0; i < listResourceInfos.Count; i++)
                {
                    var resource = listResourceInfos[i];
                    if (resource.ObjType == Service07Consts.RESOURCE_VOICECHANNEL)
                    {
                        var channel = resource as ChanConfigInfo;
                        if (channel == null) { continue; }
                        var voice =
                            listResourceInfos.FirstOrDefault(r => r.ObjID == channel.ParentObjID) as ServiceConfigInfo;
                        if (voice == null) { continue; }
                        string strExt = channel.Extension;
                        string voiceIP = voice.HostAddress;
                        long voiceObjID = voice.ObjID;
                        int voiceChanID = channel.ID;
                        long voiceChanObjID = channel.ObjID;

                        var extension = listExtInfos.FirstOrDefault(e => e.Extension == strExt && e.VoiceIP == voiceIP);
                        if (extension == null)
                        {
                            //是否存在分机号相同的对象，如果存在则可能是录屏分机，此时不需要创建一个分机实例
                            extension = listExtInfos.FirstOrDefault(e => e.Extension == strExt && (e.Role & 2) > 0);
                            if (extension == null)
                            {
                                //不存在，则增加
                                optReturn = GetSerialID(Service07Consts.MODULE_BASEMODULE,
                                    Service07Consts.RESOURCE_EXTENSION, true);
                                if (!optReturn.Result)
                                {
                                    OnDebug(LogMode.Error,
                                        string.Format("GetSerialID fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                    continue;
                                }
                                extension = new ExtensionInfo();
                                extension.ObjID = Convert.ToInt64(optReturn.Data);
                                extension.ObjType = Service07Consts.RESOURCE_EXTENSION;

                                extension.OrgID = ConstValue.ORG_ROOT;
                                extension.Status = "1";
                                extension.IsNew = true;
                                extension.IsLock = false;
                                extension.LockMethod = "N";
                                extension.SourceType = "09";
                                extension.Role = 1;
                                extension.Name = string.Format("{0}{1}{2}{1}{3}", strExt, ConstValue.SPLITER_CHAR, voiceIP,
                                    extension.Role);
                                extension.Extension = strExt;
                                extension.ChanName = channel.ChanName;
                                extension.VoiceObjID = voiceObjID;
                                extension.VoiceChanID = voiceChanID;
                                extension.VoiceChanObjID = voiceChanObjID;
                                extension.ScreenObjID = 0;
                                extension.ScreenChanID = 0;
                                extension.ScreenChanObjID = 0;

                                listAddExtensions.Add(extension);

                                OnDebug(LogMode.Debug, string.Format("Add:{0}", extension.LogInfo()));
                            }
                            else
                            {
                                //更新
                                extension.VoiceIP = voiceIP;
                                extension.Role = extension.Role | 1;
                                extension.Name = string.Format("{0}{1}{2}{1}{3}{1}{4}", strExt, ConstValue.SPLITER_CHAR,
                                    extension.VoiceIP,
                                    extension.Role,
                                    extension.ScreenIP);
                                strValue = extension.Status;
                                extension.Status = strValue == "0" ? "1" : strValue;  //如果原来是被删除的分机，重新恢复成正常的分机
                                extension.ChanName = channel.ChanName;
                                extension.VoiceObjID = voiceObjID;
                                extension.VoiceChanID = voiceChanID;
                                extension.VoiceChanObjID = voiceChanObjID;

                                listModifyExtensions.Add(extension);

                                OnDebug(LogMode.Debug, string.Format("Modify:{0}", extension.LogInfo()));
                            }
                        }
                        else
                        {
                            //更新
                            extension.VoiceIP = voiceIP;
                            extension.Role = extension.Role | 1;
                            extension.Name = string.Format("{0}{1}{2}{1}{3}{1}{4}", strExt, ConstValue.SPLITER_CHAR,
                                extension.VoiceIP,
                                extension.Role,
                                extension.ScreenIP);
                            strValue = extension.Status;
                            extension.Status = strValue == "0" ? "1" : strValue;  //如果原来是被删除的分机，重新恢复成正常的分机
                            extension.ChanName = channel.ChanName;
                            extension.VoiceObjID = voiceObjID;
                            extension.VoiceChanID = voiceChanID;
                            extension.VoiceChanObjID = voiceChanObjID;

                            listModifyExtensions.Add(extension);

                            OnDebug(LogMode.Debug, string.Format("Modify:{0}", extension.LogInfo()));
                        }
                    }
                }

                #endregion


                #region 删除的分机打上删除标记

                for (int i = 0; i < listExtInfos.Count; i++)
                {
                    //即不在listAddExtensions列表中，又不在listModifyExtensions中，同时又是录音分机的分机是要被删除的分机，需要打上删除标记
                    var extension = listExtInfos[i];
                    if (!listAddExtensions.Contains(extension)
                        && !listModifyExtensions.Contains(extension))
                    {
                        extension.Role = extension.Role & 2;
                        if (extension.Role == 0)
                        {
                            //打上删除标记
                            extension.Status = "0";
                            listDeleteExtensions.Add(extension);

                            OnDebug(LogMode.Debug, string.Format("Delete:{0}", extension.LogInfo()));
                        }
                        else
                        {
                            //删掉录音分机信息，属于更新
                            extension.Name = string.Format("{0}{1}{2}{1}{3}", extension.Extension, ConstValue.SPLITER_CHAR,
                                extension.ScreenIP, extension.Role);
                            extension.VoiceIP = string.Empty;
                            extension.VoiceObjID = 0;
                            extension.VoiceChanID = 0;
                            extension.VoiceChanObjID = 0;

                            listModifyExtensions.Add(extension);

                            OnDebug(LogMode.Debug, string.Format("Modify:{0}", extension.LogInfo()));
                        }
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

                            dr2["C015"] = ext.VoiceObjID;
                            dr2["C016"] = ext.VoiceChanID;
                            dr2["C017"] = ext.VoiceChanObjID;
                            dr2["C018"] = ext.ScreenObjID;
                            dr2["C019"] = ext.ScreenChanID;
                            dr2["C020"] = ext.ScreenChanObjID;

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
                                    strValue = ext.Name;
                                    strValue = EncryptToDB(strValue);
                                    dr["C017"] = strValue;
                                    strValue = ext.ChanName;
                                    strValue = EncryptToDB(strValue);
                                    dr["C018"] = strValue;
                                }
                                if (rowID == 2)
                                {
                                    dr["C015"] = ext.VoiceObjID;
                                    dr["C016"] = ext.VoiceChanID;
                                    dr["C017"] = ext.VoiceChanObjID;
                                    dr["C018"] = ext.ScreenObjID;
                                    dr["C019"] = ext.ScreenChanID;
                                    dr["C020"] = ext.ScreenChanObjID;
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

                OnDebug(LogMode.Debug, string.Format("Sychronize Voice Extension end.\t"));

                #endregion

            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DoSyncExtensionData fail.\t{0}", ex.Message));
            }
        }

        private void SyncScreenExtensionData()
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
                int screenCount = 0;
                int screenChanCount = 0;
                int extCount = 0;
                long longValue;
                int intValue;
                string strValue;

                List<ResourceConfigInfo> listResourceInfos = new List<ResourceConfigInfo>();
                List<ExtensionInfo> listExtInfos = new List<ExtensionInfo>();


                #region 获取ScreenServer资源

                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                Service07Consts.RESOURCE_SCREENSERVER * 10000000000000000,
                                (Service07Consts.RESOURCE_SCREENSERVER + 1) * 10000000000000000);
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
                                  Service07Consts.RESOURCE_SCREENSERVER * 10000000000000000,
                                 (Service07Consts.RESOURCE_SCREENSERVER + 1) * 10000000000000000);
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

                    ServiceConfigInfo screen = new ServiceConfigInfo();
                    screen.ObjType = Service07Consts.RESOURCE_SCREENSERVER;
                    long objID = Convert.ToInt64(dr["C001"]);
                    screen.ObjID = objID;

                    var temp = listResourceInfos.FirstOrDefault(r => r.ObjID == screen.ObjID);
                    if (temp != null)
                    {
                        listResourceInfos.Remove(temp);
                    }
                    listResourceInfos.Add(screen);
                    screenCount++;
                }

                #endregion


                #region 获取ScreenServer 的配置信息

                for (int i = 0; i < listResourceInfos.Count; i++)
                {
                    var resource = listResourceInfos[i];
                    if (resource.ObjType != Service07Consts.RESOURCE_SCREENSERVER) { continue; }
                    var screen = resource as ServiceConfigInfo;
                    if (screen == null) { continue; }
                    switch (dbType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 91) ORDER BY C001, C002",
                                    rentToken,
                                    screen.ObjID);
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
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 91) ORDER BY C001, C002",
                                    rentToken,
                                    screen.ObjID);
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
                            screen.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            screen.ID = id;
                            string strHostAddress = dr["C017"].ToString();
                            strHostAddress = DecodeEncryptValue(strHostAddress);
                            screen.HostAddress = strHostAddress;
                            string strHostName = dr["C018"].ToString();
                            strHostName = DecodeEncryptValue(strHostName);
                            screen.HostName = strHostName;
                            string strPort = dr["C019"].ToString();
                            strPort = DecodeEncryptValue(strPort);
                            if (int.TryParse(strPort, out intValue))
                            {
                                screen.HostPort = intValue;
                            }
                            else
                            {
                                screen.HostPort = 0;
                            }
                        }
                        if (row == 91)
                        {
                            screen.Continent = dr["C011"].ToString();
                            screen.Country = dr["C012"].ToString();
                        }
                    }
                }

                #endregion


                #region 获取录屏通道资源

                for (int i = 0; i < listResourceInfos.Count; i++)
                {
                    var resource = listResourceInfos[i];
                    if (resource.ObjType != Service07Consts.RESOURCE_SCREENSERVER) { continue; }
                    var screen = resource as ServiceConfigInfo;
                    if (screen == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 AND C013 = '{3}' ORDER BY C001, C002",
                                    rentToken,
                                    Service07Consts.RESOURCE_SCREENCHANNEL * 10000000000000000,
                                    (Service07Consts.RESOURCE_SCREENCHANNEL + 1) * 10000000000000000,
                                    screen.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 AND C013 = '{3}' ORDER BY C001, C002",
                                    rentToken,
                                    Service07Consts.RESOURCE_SCREENCHANNEL * 10000000000000000,
                                    (Service07Consts.RESOURCE_SCREENCHANNEL + 1) * 10000000000000000,
                                    screen.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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

                        ChanConfigInfo channel = new ChanConfigInfo();
                        channel.ObjType = Service07Consts.RESOURCE_SCREENCHANNEL;
                        long objID = Convert.ToInt64(dr["C001"]);
                        channel.ObjID = objID;

                        var temp = listResourceInfos.FirstOrDefault(r => r.ObjID == channel.ObjID);
                        if (temp != null)
                        {
                            listResourceInfos.Remove(temp);
                        }
                        screenChanCount++;
                        screen.ListChildObjects.Add(channel);
                        listResourceInfos.Add(channel);
                    }
                }

                #endregion


                #region 获取录屏通道配置信息

                for (int i = 0; i < listResourceInfos.Count; i++)
                {
                    var resource = listResourceInfos[i];
                    if (resource.ObjType != Service07Consts.RESOURCE_SCREENCHANNEL) { continue; }
                    var channel = resource as ChanConfigInfo;
                    if (channel == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 or C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    channel.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 or C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    channel.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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
                            channel.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            channel.ID = id;
                            long parentObjID = Convert.ToInt64(dr["C013"]);
                            channel.ParentObjID = parentObjID;
                        }
                        if (row == 2)
                        {
                            string strChanName = dr["C011"].ToString();
                            channel.ChanName = strChanName;
                            string strExtension = dr["C012"].ToString();
                            channel.Extension = strExtension;
                        }
                    }
                }

                #endregion


                #region 获取分机资源

                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                Service07Consts.RESOURCE_EXTENSION * 10000000000000000,
                                (Service07Consts.RESOURCE_EXTENSION + 1) * 10000000000000000);
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
                                 Service07Consts.RESOURCE_EXTENSION * 10000000000000000,
                                 (Service07Consts.RESOURCE_EXTENSION + 1) * 10000000000000000);
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

                    ExtensionInfo ext = new ExtensionInfo();
                    ext.ObjType = Service07Consts.RESOURCE_EXTENSION;
                    long objID = Convert.ToInt64(dr["C001"]);
                    ext.ObjID = objID;

                    var temp = listExtInfos.FirstOrDefault(r => r.ObjID == ext.ObjID);
                    if (temp != null)
                    {
                        listExtInfos.Remove(temp);
                    }
                    listExtInfos.Add(ext);
                    extCount++;
                }

                #endregion


                #region 获取分机配置信息

                for (int i = 0; i < listExtInfos.Count; i++)
                {
                    var resource = listExtInfos[i];
                    if (resource.ObjType != Service07Consts.RESOURCE_EXTENSION) { continue; }
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
                            string strRole = string.Empty;
                            string strIP1 = string.Empty;
                            string strIP2 = string.Empty;
                            string[] arrValue = strValue.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.None);
                            if (arrValue.Length > 0)
                            {
                                ext.Extension = arrValue[0];
                            }
                            if (arrValue.Length > 1)
                            {
                                strIP1 = arrValue[1];
                            }
                            if (arrValue.Length > 2)
                            {
                                strRole = arrValue[2];
                            }
                            if (arrValue.Length > 3)
                            {
                                strIP2 = arrValue[3];
                            }
                            int intRole;
                            if (int.TryParse(strRole, out intRole))
                            {
                                if (intRole == 1)
                                {
                                    ext.Role = intRole;
                                    ext.VoiceIP = strIP1;
                                }
                                else if (intRole == 2)
                                {
                                    ext.Role = intRole;
                                    ext.ScreenIP = strIP1;
                                }
                                else if (intRole == 3)
                                {
                                    ext.Role = intRole;
                                    ext.VoiceIP = strIP1;
                                    ext.ScreenIP = strIP2;
                                }
                            }
                            strValue = dr["C018"].ToString();
                            strValue = DecryptFromDB(strValue);
                            ext.ChanName = strValue;
                        }
                        if (row == 2)
                        {
                            strValue = dr["C015"].ToString();
                            if (long.TryParse(strValue, out longValue))
                            {
                                ext.VoiceObjID = longValue;
                            }
                            else
                            {
                                ext.VoiceObjID = 0;
                            }
                            strValue = dr["C016"].ToString();
                            if (int.TryParse(strValue, out intValue))
                            {
                                ext.VoiceChanID = intValue;
                            }
                            else
                            {
                                ext.VoiceChanID = 0;
                            }
                            strValue = dr["C017"].ToString();
                            if (long.TryParse(strValue, out longValue))
                            {
                                ext.VoiceChanObjID = longValue;
                            }
                            else
                            {
                                ext.VoiceChanObjID = 0;
                            }
                            strValue = dr["C018"].ToString();
                            if (long.TryParse(strValue, out longValue))
                            {
                                ext.ScreenObjID = longValue;
                            }
                            else
                            {
                                ext.ScreenObjID = 0;
                            }
                            strValue = dr["C019"].ToString();
                            if (int.TryParse(strValue, out intValue))
                            {
                                ext.ScreenChanID = intValue;
                            }
                            else
                            {
                                ext.ScreenChanID = 0;
                            }
                            strValue = dr["C020"].ToString();
                            if (long.TryParse(strValue, out longValue))
                            {
                                ext.ScreenChanObjID = longValue;
                            }
                            else
                            {
                                ext.ScreenChanObjID = 0;
                            }

                            #region 为了与老版本程序兼容，做一下处理得到分机所在服务器IP

                            if (ext.Role <= 0)
                            {
                                string strName = ext.Name;
                                string[] arrName = strName.Split(new[] { ConstValue.SPLITER_CHAR },
                                    StringSplitOptions.RemoveEmptyEntries);
                                string strIP = string.Empty;
                                if (arrName.Length > 1)
                                {
                                    strIP = arrName[1];
                                }
                                if (!string.IsNullOrEmpty(strIP))
                                {
                                    if (ext.VoiceObjID > 0)
                                    {
                                        ext.VoiceIP = strIP;
                                        ext.Role = ext.Role | 1;
                                    }
                                    if (ext.ScreenObjID > 0)
                                    {
                                        ext.ScreenIP = strIP;
                                        ext.Role = ext.Role | 2;
                                    }
                                }
                            }

                            #endregion
                        }
                    }
                }

                #endregion


                OnDebug(LogMode.Debug,
                    string.Format(
                        "LoadResourceInfos end.\tScreenCount:{0};\tScreenChanCount:{1};ExtensionCount:{2}",
                        screenCount,
                        screenChanCount,
                        extCount));

                for (int i = 0; i < listExtInfos.Count; i++)
                {
                    OnDebug(LogMode.Debug, string.Format("{0}", listExtInfos[i].LogInfo()));
                }

                List<ExtensionInfo> listAddExtensions = new List<ExtensionInfo>();
                List<ExtensionInfo> listModifyExtensions = new List<ExtensionInfo>();
                List<ExtensionInfo> listDeleteExtensions = new List<ExtensionInfo>();


                #region 同步分机信息

                for (int i = 0; i < listResourceInfos.Count; i++)
                {
                    var resource = listResourceInfos[i];

                    if (resource.ObjType == Service07Consts.RESOURCE_SCREENCHANNEL)
                    {
                        var channel = resource as ChanConfigInfo;
                        if (channel == null) { continue; }
                        var screen =
                            listResourceInfos.FirstOrDefault(r => r.ObjID == channel.ParentObjID) as ServiceConfigInfo;
                        if (screen == null) { continue; }
                        string strExt = channel.Extension;
                        string screenIP = screen.HostAddress;
                        long screenObjID = screen.ObjID;
                        int screenChanID = channel.ID;
                        long screenChanObjID = channel.ObjID;

                        var extension = listExtInfos.FirstOrDefault(e => e.Extension == strExt && e.ScreenIP == screenIP);
                        if (extension == null)
                        {
                            //是否存在分机号相同的对象，如果存在则可能是录音分机，此时不需要创建一个分机实例
                            extension = listExtInfos.FirstOrDefault(e => e.Extension == strExt && (e.Role & 2) > 0);
                            if (extension == null)
                            {
                                //不存在，则增加
                                optReturn = GetSerialID(Service07Consts.MODULE_BASEMODULE,
                                    Service07Consts.RESOURCE_EXTENSION, true);
                                if (!optReturn.Result)
                                {
                                    OnDebug(LogMode.Error,
                                        string.Format("GetSerialID fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                    continue;
                                }
                                extension = new ExtensionInfo();
                                extension.ObjID = Convert.ToInt64(optReturn.Data);
                                extension.ObjType = Service07Consts.RESOURCE_EXTENSION;

                                extension.OrgID = ConstValue.ORG_ROOT;
                                extension.Status = "1";
                                extension.IsNew = true;
                                extension.IsLock = false;
                                extension.LockMethod = "N";
                                extension.SourceType = "09";
                                extension.Role = 2;
                                extension.Name = string.Format("{0}{1}{2}{1}{3}", strExt, ConstValue.SPLITER_CHAR, screenIP,
                                    extension.Role);
                                extension.Extension = strExt;
                                extension.ChanName = channel.ChanName;
                                extension.VoiceObjID = 0;
                                extension.VoiceChanID = 0;
                                extension.VoiceChanObjID = 0;
                                extension.ScreenObjID = screenObjID;
                                extension.ScreenChanID = screenChanID;
                                extension.ScreenChanObjID = screenChanObjID;

                                listAddExtensions.Add(extension);

                                OnDebug(LogMode.Debug, string.Format("Add:{0}", extension.LogInfo()));
                            }
                            else
                            {
                                //更新
                                extension.ScreenIP = screenIP;
                                extension.Role = extension.Role | 2;
                                extension.Name = string.Format("{0}{1}{2}{1}{3}{1}{4}", strExt, ConstValue.SPLITER_CHAR,
                                    extension.VoiceIP,
                                    extension.Role,
                                    extension.ScreenIP);
                                strValue = extension.Status;
                                extension.Status = strValue == "0" ? "1" : strValue;  //如果原来是被删除的分机，重新恢复成正常的分机
                                extension.ChanName = channel.ChanName;
                                extension.ScreenObjID = screenObjID;
                                extension.ScreenChanID = screenChanID;
                                extension.ScreenChanObjID = screenChanObjID;

                                listModifyExtensions.Add(extension);

                                OnDebug(LogMode.Debug, string.Format("Modify:{0}", extension.LogInfo()));
                            }
                        }
                        else
                        {
                            //更新
                            extension.ScreenIP = screenIP;
                            extension.Role = extension.Role | 2;
                            extension.Name = string.Format("{0}{1}{2}{1}{3}{1}{4}", strExt, ConstValue.SPLITER_CHAR,
                                extension.VoiceIP,
                                extension.Role,
                                extension.ScreenIP);
                            strValue = extension.Status;
                            extension.Status = strValue == "0" ? "1" : strValue;  //如果原来是被删除的分机，重新恢复成正常的分机
                            extension.ChanName = channel.ChanName;
                            extension.ScreenObjID = screenObjID;
                            extension.ScreenChanID = screenChanID;
                            extension.ScreenChanObjID = screenChanObjID;

                            listModifyExtensions.Add(extension);

                            OnDebug(LogMode.Debug, string.Format("Modify:{0}", extension.LogInfo()));
                        }
                    }
                }

                #endregion


                #region 删除的分机打上删除标记

                for (int i = 0; i < listExtInfos.Count; i++)
                {
                    //即不在listAddExtensions列表中，又不在listModifyExtensions中，同时又是录屏分机的分机是要被删除的分机，需要打上删除标记
                    var extension = listExtInfos[i];
                    if (!listAddExtensions.Contains(extension)
                        && !listModifyExtensions.Contains(extension))
                    {
                        extension.Role = extension.Role & 1;
                        if (extension.Role == 0)
                        {
                            //打上删除标记
                            extension.Status = "0";
                            listDeleteExtensions.Add(extension);

                            OnDebug(LogMode.Debug, string.Format("Delete:{0}", extension.LogInfo()));
                        }
                        else
                        {
                            //删掉录音分机信息，属于更新
                            extension.Name = string.Format("{0}{1}{2}{1}{3}", extension.Extension, ConstValue.SPLITER_CHAR,
                                extension.VoiceIP, extension.Role);
                            extension.ScreenIP = string.Empty;
                            extension.ScreenObjID = 0;
                            extension.ScreenChanID = 0;
                            extension.ScreenChanObjID = 0;

                            listModifyExtensions.Add(extension);

                            OnDebug(LogMode.Debug, string.Format("Modify:{0}", extension.LogInfo()));
                        }
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

                            dr2["C015"] = ext.VoiceObjID;
                            dr2["C016"] = ext.VoiceChanID;
                            dr2["C017"] = ext.VoiceChanObjID;
                            dr2["C018"] = ext.ScreenObjID;
                            dr2["C019"] = ext.ScreenChanID;
                            dr2["C020"] = ext.ScreenChanObjID;

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
                                    strValue = ext.Name;
                                    strValue = EncryptToDB(strValue);
                                    dr["C017"] = strValue;
                                    strValue = ext.ChanName;
                                    strValue = EncryptToDB(strValue);
                                    dr["C018"] = strValue;
                                }
                                if (rowID == 2)
                                {
                                    dr["C015"] = ext.VoiceObjID;
                                    dr["C016"] = ext.VoiceChanID;
                                    dr["C017"] = ext.VoiceChanObjID;
                                    dr["C018"] = ext.ScreenObjID;
                                    dr["C019"] = ext.ScreenChanID;
                                    dr["C020"] = ext.ScreenChanObjID;
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

                OnDebug(LogMode.Debug, string.Format("Sychronize Screen Extension end.\t"));

                #endregion

            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DoSyncExtensionData fail.\t{0}", ex.Message));
            }
        }
    }
}
