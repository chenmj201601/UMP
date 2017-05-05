//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0b2638c7-63d2-4387-adae-e575a40fd80e
//        CLR Version:              4.0.30319.18063
//        Name:                     IsaControlOperator
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                IsaControlOperator
//
//        created by Charley at 2015/10/25 16:42:54
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.SDKs.DEC;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService04;

/*
 * ======================================================================
 * 
 * IsaControlOperator 工作逻辑
 * 
 * ***启动***
 * 1、更新mListIsaServers列表，从ListAllResources中帅选出艺赛旗录屏服务信息
 * 
 * ***DEC消息处理***
 * 1、判断有没有启用控制艺赛旗录屏的功能
 * 2、处理录音启动停止消息
 * 3、当录音启动，取得录音流水号VoiceRefID
 * 4、向艺赛旗录屏服务器发送启动录屏的消息，使用VoiceRefID作为extno
 * 5、从响应的数据中获得录屏的流水号sessionno作为录屏记录的SessionID
 * 6、生成一条录屏记录暂时放入mListExtRefIDItems列表中
 * 7、当录音停止，根据VoiceRefID查得之前放入mListExtRefIDItems列表中录屏记录，更新某些信息后调用存储过程将记录写入数据库
 * 
 * ***参数更新***
 * 1、重新更新mListIsaServers列表
 * 
 * ======================================================================
 */

namespace UMPService04
{
    public class IsaControlOperator
    {

        #region Members

        public ConfigInfo ConfigInfo;
        public List<ResourceConfigInfo> ListAllResources;
        public DECMessageHelper DECMessageHelper;
        public SessionInfo Session;

        private List<IsaServerInfo> mListIsaServers;
        private List<RecordInfoExt> mListExtRefIDItems;

        private DateTime mCurrentTime;
        private int mCurrentNumber;

        #endregion


        public IsaControlOperator()
        {
            mListIsaServers = new List<IsaServerInfo>();
            mListExtRefIDItems = new List<RecordInfoExt>();

            mCurrentTime = DateTime.Now.ToUniversalTime();
            mCurrentNumber = 1;
        }


        #region Operations

        public void Start()
        {
            try
            {
                //更新IsaServer列表
                mListIsaServers.Clear();
                if (ListAllResources != null)
                {
                    for (int i = 0; i < ListAllResources.Count; i++)
                    {
                        var resource = ListAllResources[i];
                        if (resource.ObjType != IsaServerInfo.RESOURCE_ISASERVER) { continue; }
                        var isa = resource as IsaServerInfo;
                        if (isa != null)
                        {
                            mListIsaServers.Add(isa);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Init fail.\t{0}", ex.Message));
            }
        }

        public void Stop()
        {

        }

        public void Refresh()
        {
            Start();
        }

        public void DealDECMessage(MessageString message, string strData)
        {
            try
            {
                string strRecordReference;
                string strDeviceID;
                DateTime dtRecordTime;

                //判断有没有启用艺赛旗录屏控制功能
                if (ConfigInfo == null || ConfigInfo.ListSettings == null) { return; }
                var setting = ConfigInfo.ListSettings.FirstOrDefault(s => s.Key == Service04Consts.GS_KEY_S04_CTLISA);
                if (setting == null
                    || setting.Value != "1")
                {
                    return;
                }

                if (DECMessageHelper == null) { return; }
                //string strMessage = message.ToString();
                string strMessage = string.Format("{0:X4}:{1:X4}:{2:X4}:{3:X4}",
                   message.LargeType,
                   message.MiddleType,
                   message.SmallType,
                   message.Number);
                switch (strMessage)
                {
                    case DECMessageHelper.MSG_VOC_RECORDSTARTED:
                        //OnDebug(LogMode.Info, string.Format("Voice record started"));
                        strRecordReference = DECMessageHelper.GetRecordReferenceValue(strData);
                        strDeviceID = DECMessageHelper.GetDeviceIDValue(strData);
                        dtRecordTime = DECMessageHelper.GetRecordTimeValue(strData);
                        StartScreen(strDeviceID, strRecordReference, dtRecordTime);
                        break;
                    case DECMessageHelper.MSG_VOC_RECORDSTOPPED:
                        //OnDebug(LogMode.Info, string.Format("Voice record stopped"));
                        strRecordReference = DECMessageHelper.GetRecordReferenceValue(strData);
                        strDeviceID = DECMessageHelper.GetDeviceIDValue(strData);
                        dtRecordTime = DECMessageHelper.GetRecordTimeValue(strData);
                        StopScreen(strDeviceID, strRecordReference, dtRecordTime);
                        break;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealDECMessage fail.\t{0}", ex.Message));
            }
        }

        private void StartScreen(string extension, string voiceRefID, DateTime recordTime)
        {
            try
            {
                var isaServer = mListIsaServers.FirstOrDefault();
                if (isaServer == null) { return; }
                string strIP = isaServer.HostAddress;
                string strToken = isaServer.AccessToken;
                string tempRefID = voiceRefID;
                JsonObject jsonRequest = new JsonObject(p =>
                {
                    p["action"] = new JsonProperty(string.Format("\"{0}\"", ACTION_START));
                    p["extno"] = new JsonProperty(string.Format("\"{0}\"", tempRefID));
                    p["extnumber"] = new JsonProperty(string.Format("\"{0}\"", extension));
                    p["user_name"] = new JsonProperty(string.Format("\"{0}\"", "username"));
                    p["domain"] = new JsonProperty(string.Format("\"{0}\"", "domain"));
                    p["timeout"] = new JsonProperty(string.Format("\"{0}\"", 2 * 60 * 60));
                    p["logs"] = new JsonProperty(string.Format("\"{0}\"", "w"));
                    p["desktopno"] = new JsonProperty(string.Format("\"{0}\"", "1"));
                });
                //string strRequest = jsonRequest.ToString("F");
                string strRequest = jsonRequest.ToString();
                string url = string.Format("http://{0}/record.action?access_token={1}", strIP,
                    strToken);
                strRequest = string.Format("json={0}", strRequest);
                OnDebug(LogMode.Debug, string.Format("Send:{0}&{1}", url, strRequest));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (var streamRequest = request.GetRequestStream())
                {
                    StreamWriter writer = new StreamWriter(streamRequest);
                    writer.Write(strRequest);
                    writer.Flush();
                }
                string strResponse = string.Empty;
                using (var streamResponse = request.GetResponse().GetResponseStream())
                {
                    if (streamResponse != null)
                    {
                        StreamReader reader = new StreamReader(streamResponse);
                        strResponse = reader.ReadToEnd();
                    }
                }
                OnDebug(LogMode.Debug, string.Format("Recv:{0}", strResponse));
                try
                {
                    JsonObject jsonResponse = new JsonObject(strResponse);
                    if (jsonResponse[FIELD_ERRCODE] != null)
                    {
                        var errcode = jsonResponse[FIELD_ERRCODE].Value;
                        if (errcode != "0")
                        {
                            string strMsg = string.Empty;
                            if (jsonResponse[FIELD_ERRMSG] != null)
                            {
                                strMsg = jsonResponse[FIELD_ERRMSG].Value;
                            }
                            OnDebug(LogMode.Error,
                                string.Format("StartScreen fail.\t{0}\t{1}\t{2}", extension, errcode, strMsg));
                            return;
                        }
                        if (jsonResponse[FIELD_SESSIONNO] != null)
                        {
                            string strSid = jsonResponse[FIELD_SESSIONNO].Value;
                            string strContinent = isaServer.Continent;
                            string strCountry = isaServer.Country;
                            string strContinentWithCountry = string.Format("{0}{1}", strContinent, strCountry);
                            if (strContinentWithCountry.Length != 5)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("IsaServer's continent or country invalid.\t{0}",
                                        strContinentWithCountry));
                                strContinentWithCountry = "ASCHN";
                            }
                            int siteID = 0;
                            int mediaType = 3;
                            int channelID = 0;
                            DateTime startTime = recordTime;        //录音录屏同启同停，使用录音的开始时间作为录屏的开始时间
                            int num = mCurrentNumber;
                            if (startTime <= mCurrentTime)
                            {
                                num++;      //如果同一时间多个记录，此序号累加
                            }
                            else
                            {
                                mCurrentTime = startTime;
                                num = 1;
                            }

                            /*
                             * ***录屏流水号的生成规则***
                             * 
                             * 共38个字符，从左到右
                             * 
                             * 0 ~ 1：洲代码，2位字母
                             * 2 ~ 4：国家代码，3位字母
                             * 5 ~ 8：站点号，4位，16进制
                             * 9 ~ 12：服务器ID，4位，16进制
                             * 13 ~ 16：媒体类型，4位，16进制
                             * 17 ~ 20：通道号，4位，16进制
                             * 21 ~ 34：日期时间，yyyyMMddHHmmss
                             * 35 ~ 37：序数，3位，十进制，防止同一秒多条记录
                             * 
                             */

                            string strRefID = string.Format("{0}{1}{2}{3}{4}{5}{6}",
                                strContinentWithCountry,
                                siteID.ToString("X4"),
                                isaServer.ID.ToString("X4"),
                                mediaType.ToString("X4"),
                                channelID.ToString("X4"),
                                startTime.ToString("yyyyMMddHHmmss"),
                                num);
                            RecordInfoExt item = new RecordInfoExt();
                            item.RecordReference = strRefID;
                            item.Extension = extension;
                            item.VoiceRefID = voiceRefID;
                            item.SessionID = strSid;
                            item.StartRecordTime = startTime;
                            item.ServerIP = isaServer.HostAddress;
                            item.ServerID = isaServer.ID;
                            item.ChannelID = 0;
                            item.MediaType = 3;
                            item.EncryptFlag = "0";
                            mListExtRefIDItems.Add(item);
                            OnDebug(LogMode.Debug, string.Format("StartScreen:{0}\t{1}\t{2}", extension, voiceRefID, strSid));
                        }
                    }
                }
                catch { }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StartScreen fail.\t{0}", ex.Message));
            }
        }

        private void StopScreen(string extension, string voiceRefID, DateTime recordTime)
        {
            try
            {
                var isaServer = mListIsaServers.FirstOrDefault();
                if (isaServer == null) { return; }
                string strIP = isaServer.HostAddress;
                string strToken = isaServer.AccessToken;
                string tempRefID = voiceRefID;
                JsonObject jsonRequest = new JsonObject(p =>
                {
                    p["action"] = new JsonProperty(string.Format("\"{0}\"", ACTION_STOP));
                    p["extno"] = new JsonProperty(string.Format("\"{0}\"", tempRefID));
                    p["extnumber"] = new JsonProperty(string.Format("\"{0}\"", extension));
                    p["user_name"] = new JsonProperty(string.Format("\"{0}\"", "username"));
                    p["domain"] = new JsonProperty(string.Format("\"{0}\"", "domain"));
                    p["timeout"] = new JsonProperty(string.Format("\"{0}\"", 2 * 60 * 60));
                    p["logs"] = new JsonProperty(string.Format("\"{0}\"", "w"));
                    p["desktopno"] = new JsonProperty(string.Format("\"{0}\"", "1"));
                });
                //string strRequest = jsonRequest.ToString("F");
                string strRequest = jsonRequest.ToString();
                string url = string.Format("http://{0}/record.action?access_token={1}", strIP,
                    strToken);
                strRequest = string.Format("json={0}", strRequest);
                OnDebug(LogMode.Debug, string.Format("Send:{0}&{1}", url, strRequest));
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (var streamRequest = request.GetRequestStream())
                {
                    StreamWriter writer = new StreamWriter(streamRequest);
                    writer.Write(strRequest);
                    writer.Flush();
                }
                string strResponse = string.Empty;
                using (var streamResponse = request.GetResponse().GetResponseStream())
                {
                    if (streamResponse != null)
                    {
                        StreamReader reader = new StreamReader(streamResponse);
                        strResponse = reader.ReadToEnd();
                    }
                }
                OnDebug(LogMode.Debug, string.Format("Recv:{0}", strResponse));

                JsonObject jsonResponse = new JsonObject(strResponse);
                if (jsonResponse[FIELD_ERRCODE] != null)
                {
                    var errcode = jsonResponse[FIELD_ERRCODE].Value;
                    if (errcode != "0")
                    {
                        string strMsg = string.Empty;
                        if (jsonResponse[FIELD_ERRMSG] != null)
                        {
                            strMsg = jsonResponse[FIELD_ERRMSG].Value;
                        }
                        OnDebug(LogMode.Error,
                            string.Format("StopScreen fail.\t{0}\t{1}\t{2}", extension, errcode, strMsg));
                        return;
                    }
                    if (jsonResponse[FIELD_EXTNO] != null)
                    {
                        voiceRefID = jsonResponse[FIELD_EXTNO].Value;
                        var item = mListExtRefIDItems.FirstOrDefault(p => p.VoiceRefID == voiceRefID);
                        if (item != null)
                        {
                            string strSid = item.SessionID;
                            mListExtRefIDItems.Remove(item);
                            OnDebug(LogMode.Debug, string.Format("StopScreen:{0}\t{1}\t{2}", extension, voiceRefID, strSid));

                            //数据写入数据库
                            DateTime stopTime = recordTime;     //录音录屏同启同停，使用录音的结束时间作为录屏的结束时间
                            item.StopRecordTime = stopTime;
                            InsertScreenRecord(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopScreen fail.\t{0}", ex.Message));
            }
        }

        private void InsertScreenRecord(RecordInfoExt recordInfo)
        {
            try
            {
                if (Session == null) { return; }
                DatabaseInfo dbInfo = Session.DatabaseInfo;
                if (dbInfo == null) { return; }

                #region 创建记录信息字符串

                string strInsertString = string.Empty;
                strInsertString += string.Format("c023{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "AS");
                strInsertString += string.Format("c024{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "CHN");
                strInsertString += string.Format("c022{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, 0);
                strInsertString += string.Format("c003{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "00000");
                strInsertString += string.Format("c077{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.RecordReference);
                strInsertString += string.Format("c020{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.ServerIP);
                strInsertString += string.Format("c037{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.ServerID);
                strInsertString += string.Format("c038{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.ChannelID);
                strInsertString += string.Format("c039{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "N/A");
                strInsertString += string.Format("c014{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.MediaType);
                strInsertString += string.Format("c015{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, string.Empty);
                strInsertString += string.Format("c004{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.StartRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                strInsertString += string.Format("c005{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.StartRecordTime.ToString("yyyy-MM-dd HH:mm:ss"));
                strInsertString += string.Format("c006{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.StartRecordTime.ToString("yyyyMMddHHmmss"));
                strInsertString += string.Format("c007{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.StartRecordTime.AddYears(-1600).Ticks);
                strInsertString += string.Format("c008{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.StopRecordTime.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                strInsertString += string.Format("c009{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.StopRecordTime.ToString("yyyy-MM-dd HH:mm:ss"));
                strInsertString += string.Format("c010{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.StopRecordTime.ToString("yyyyMMddHHmmss"));
                strInsertString += string.Format("c011{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.StopRecordTime.AddYears(-1600).Ticks);
                strInsertString += string.Format("c012{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, (int)(recordInfo.StopRecordTime - recordInfo.StartRecordTime).TotalSeconds);
                strInsertString += string.Format("c042{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.Extension);
                strInsertString += string.Format("c021{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.Extension);
                strInsertString += string.Format("c025{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.EncryptFlag);
                strInsertString += string.Format("c031{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c033{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "N");
                strInsertString += string.Format("c040{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, string.Empty);
                strInsertString += string.Format("c041{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, string.Empty);
                strInsertString += string.Format("c045{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c057{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, string.Empty);
                strInsertString += string.Format("c059{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c060{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c061{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c064{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c065{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c063{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c056{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");
                strInsertString += string.Format("c035{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "N");
                strInsertString += string.Format("c109{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, recordInfo.SessionID);
                strInsertString += string.Format("c066{0}{1}{0}{0}", ConstValue.SPLITER_CHAR, "0");

                #endregion


                #region 将记录信息插入数据库

                OperationReturn optReturn;
                string strConn = dbInfo.GetConnectionString();
                int errNum = 0;
                string errMsg = string.Empty;
                switch (dbInfo.TypeID)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam02", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam03", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam04", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam05", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam06", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam07", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam08", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam09", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@ainparam10", MssqlDataType.Varchar, 1024),
                            MssqlOperation.GetDbParameter("@aouterrornumber", MssqlDataType.Bigint, 0),
                            MssqlOperation.GetDbParameter("@aouterrorstring", MssqlDataType.Varchar, 200)
                        };
                        mssqlParameters[0].Value = strInsertString;
                        mssqlParameters[1].Value = string.Empty;
                        mssqlParameters[2].Value = string.Empty;
                        mssqlParameters[3].Value = string.Empty;
                        mssqlParameters[4].Value = string.Empty;
                        mssqlParameters[5].Value = string.Empty;
                        mssqlParameters[6].Value = string.Empty;
                        mssqlParameters[7].Value = string.Empty;
                        mssqlParameters[8].Value = string.Empty;
                        mssqlParameters[9].Value = string.Empty;
                        mssqlParameters[10].Value = errNum;
                        mssqlParameters[11].Value = errMsg;
                        mssqlParameters[10].Direction = ParameterDirection.Output;
                        mssqlParameters[11].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(strConn, "P_21_001",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("InsertScreenRecord fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        if (mssqlParameters[10].Value.ToString() != "0")
                        {
                            OnDebug(LogMode.Error,
                            string.Format("InsertScreenRecord fail.\t{0}\t{1}", mssqlParameters[10].Value, mssqlParameters[11].Value));
                            return;
                        }
                        break;
                    case 3:
                        DbParameter[] orclParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam02", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam03", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam04", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam05", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam06", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam07", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam08", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam09", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("ainparam10", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("errornumber", OracleDataType.Int32, 0),
                            OracleOperation.GetDbParameter("errorstring", OracleDataType.Varchar2, 200)
                        };
                        orclParameters[0].Value = strInsertString;
                        orclParameters[1].Value = string.Empty;
                        orclParameters[2].Value = string.Empty;
                        orclParameters[3].Value = string.Empty;
                        orclParameters[4].Value = string.Empty;
                        orclParameters[5].Value = string.Empty;
                        orclParameters[6].Value = string.Empty;
                        orclParameters[7].Value = string.Empty;
                        orclParameters[8].Value = string.Empty;
                        orclParameters[9].Value = string.Empty;
                        orclParameters[10].Value = errNum;
                        orclParameters[11].Value = errMsg;
                        orclParameters[10].Direction = ParameterDirection.Output;
                        orclParameters[11].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(strConn, "P_21_001",
                            orclParameters);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                              string.Format("InsertScreenRecord fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        if (orclParameters[10].Value.ToString() != "0")
                        {
                            OnDebug(LogMode.Error,
                             string.Format("InsertScreenRecord fail.\t{0}\t{1}", orclParameters[10].Value, orclParameters[11].Value));
                            return;
                        }
                        break;
                    default:
                        OnDebug(LogMode.Error,
                              string.Format("InsertScreenRecord fail.\tDatabaseType invalid"));
                        return;
                }

                #endregion

                OnDebug(LogMode.Info, string.Format("InsertScreenRecord\t{0}", recordInfo.RecordReference));

            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("InsertScreenRecord fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Debug

        public event Action<LogMode, string, string> Debug;

        private void OnDebug(LogMode mode, string category, string msg)
        {
            if (Debug != null)
            {
                Debug(mode, category, msg);
            }
        }

        private void OnDebug(LogMode mode, string msg)
        {
            OnDebug(mode, "IsaCtlOpt", msg);
        }

        #endregion


        #region Json字段定义

        public const string FIELD_EXTNO = "ext_no";
        public const string FIELD_SESSIONNO = "session_no";
        public const string FIELD_ERRCODE = "errcode";
        public const string FIELD_ERRMSG = "errmsg";

        public const string ACTION_START = "start";
        public const string ACTION_STOP = "stop";

        #endregion

    }

    /// <summary>
    /// RecordInfo扩展信息
    /// </summary>
    public class RecordInfoExt : UMPRecordInfo
    {
        /// <summary>
        /// 录音流水号
        /// </summary>
        public string VoiceRefID { get; set; }
        /// <summary>
        /// 艺赛旗录屏流水号
        /// </summary>
        public string SessionID { get; set; }
    }
}
