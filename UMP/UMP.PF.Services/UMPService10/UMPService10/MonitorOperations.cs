//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e8ffaa61-d66c-4773-a75a-b44ffe965d86
//        CLR Version:              4.0.30319.18408
//        Name:                     MonitorOperations
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10
//        File Name:                MonitorOperations
//
//        created by Charley at 2016/6/27 15:44:30
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using VoiceCyber.Common;
using VoiceCyber.SDKs.NMon;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService10;
using VoiceCyber.UMP.Communications;


namespace UMPService10
{
    public class MonitorOperations : NetSession
    {

        #region Members

        public string RootDir;
        public ConfigInfo ConfigInfo;
        public List<ExtensionInfo> ListExtensionInfos;
        public List<ResourceConfigInfo> ListResourceInfos;

        private MonType mMonType;
        private List<MonitorObject> mListMonObjects;
        private NMonCore mNMonCore;

        #endregion


        public MonitorOperations(TcpClient tcpClient)
            : base(tcpClient)
        {
            mMonType = MonType.Unkown;
            mListMonObjects = new List<MonitorObject>();
        }


        #region 请求消息

        private void DoSetMonTypeMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null
                    || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonType = request.ListData[0];
                LogDebugMessage(request.Command, string.Format("Args:{0}", strMonType));
                int intValue;
                if (!int.TryParse(strMonType, out intValue))
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("MonitorType invalid\t{0}", strMonType));
                    return;
                }
                MonType oldType = mMonType;
                MonType newType = (MonType)intValue;
                mMonType = newType;
                LogDebugMessage(request.Command, string.Format("MonType changed.\tNew:{0};Old:{1}", newType, oldType));
                retMessage.Command = (int)Service10Command.ResSetMonType;
                retMessage.ListData.Add(((int)newType).ToString());
                retMessage.ListData.Add(((int)oldType).ToString());
                SendMessage(retMessage);
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DoAddMonObjectMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                OperationReturn optReturn;
                if (request.ListData == null
                    || request.ListData.Count < 2)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMethod = request.ListData[0];
                string strCount = request.ListData[1];
                LogDebugMessage(request.Command, string.Format("Method:{0};Count:{1}", strMethod, strCount));
                int intMethod;
                if (!int.TryParse(strMethod, out intMethod)
                    || intMethod < 0)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("Method invalid.\t{0}", strMethod));
                    return;
                }
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("Count invalid.\t{0}", strCount));
                    return;
                }
                if (request.ListData.Count < 2 + intCount)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                       string.Format("Ext count invalid."));
                    return;
                }
                List<MonitorObject> listObjs = new List<MonitorObject>();
                for (int i = 0; i < intCount; i++)
                {
                    string strExt = request.ListData[i + 2];
                    if (intMethod == 0)
                    {
                        var temp = mListMonObjects.FirstOrDefault(m => m.Name == strExt);
                        if (temp == null)
                        {
                            MonitorObject obj = new MonitorObject();
                            obj.MonID = Guid.NewGuid().ToString();
                            obj.Name = strExt;
                            mListMonObjects.Add(obj);
                            listObjs.Add(obj);
                        }
                    }
                }

                int validCount = listObjs.Count;
                int totalCount = mListMonObjects.Count;
                retMessage.Command = (int)Service10Command.ResAddMonObj;
                retMessage.ListData.Add(validCount.ToString());
                retMessage.ListData.Add(totalCount.ToString());
                for (int i = 0; i < validCount; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(listObjs[i]);
                    if (!optReturn.Result)
                    {
                        SendErrorMessage(request.Command, optReturn);
                        return;
                    }
                    retMessage.ListData.Add(optReturn.Data.ToString());
                }
                SendMessage(retMessage);
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DoQueryExtStateMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null
                    || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                OperationReturn optReturn;
                string strCount = request.ListData[0];
                LogDebugMessage(request.Command, string.Format("Count:{0}", strCount));
                int intCount;
                if (!int.TryParse(strCount, out intCount)
                    || intCount < 0)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("Count invalid.\t{0}", strCount));
                    return;
                }
                if (request.ListData.Count < 1 + intCount)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                      string.Format("MonID count invalid."));
                    return;
                }
                for (int i = 0; i < intCount; i++)
                {
                    string strMonID = request.ListData[i + 1];
                    var obj = mListMonObjects.FirstOrDefault(m => m.MonID == strMonID);
                    if (obj == null)
                    {
                        SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                            string.Format("MonObject not exist.\t{0}", strMonID));
                        continue;
                    }
                    string strExt = obj.Name;
                    if (ListExtensionInfos == null)
                    {
                        SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                            string.Format("ListExtensionInfos is null."));
                        continue;
                    }
                    var extInfo = ListExtensionInfos.FirstOrDefault(e => e.Extension == strExt);
                    if (extInfo == null)
                    {
                        SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                            string.Format("ExtInfo not exist.\t{0}", strExt));
                        continue;
                    }
                    obj.ObjID = extInfo.ObjID;
                    obj.ObjType = extInfo.ObjType;
                    optReturn = XMLHelper.SeriallizeObject(extInfo);
                    if (!optReturn.Result)
                    {
                        SendErrorMessage(request.Command, optReturn);
                        continue;
                    }
                    retMessage.Command = (int)Service10Command.ResQueryExtState;
                    retMessage.ListData.Clear();
                    retMessage.ListData.Add(strMonID);
                    retMessage.ListData.Add(optReturn.Data.ToString());
                    SendMessage(retMessage);
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DoStartNMonMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null
                   || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = request.ListData[0];
                LogDebugMessage(request.Command,
                    string.Format("MonID:{0};", strMonID));
                if (mMonType != MonType.NMon)
                {
                    SendErrorMessage(request.Command, Defines.RET_CHECK_FAIL,
                        string.Format("MonType invalid.\t{0}", mMonType));
                    return;
                }
                MonitorObject monObj = mListMonObjects.FirstOrDefault(o => o.MonID == strMonID);
                if (monObj == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                        string.Format("Monitor object not in the monitor list"));
                    return;
                }
                string strExt = monObj.Name;
                if (ListExtensionInfos == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                       string.Format("ListExtensionInfos is null"));
                    return;
                }
                var extInfo = ListExtensionInfos.FirstOrDefault(e => e.Extension == strExt);
                if (extInfo == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                        string.Format("ExtInfo not exist.\t{0}", strExt));
                    return;
                }
                if (ListResourceInfos == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                     string.Format("ListResourceInfos is null"));
                    return;
                }
                VoiceChanInfo chanInfo = null;
                for (int i = 0; i < ListResourceInfos.Count; i++)
                {
                    var resource = ListResourceInfos[i];
                    chanInfo = resource as VoiceChanInfo;
                    if (chanInfo == null) { continue; }
                    if (chanInfo.Extension != strExt) { continue; }
                    break;
                }
                if (chanInfo == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                       string.Format("VoiceChanInfo not exist.\t{0}", strExt));
                    return;
                }
                int chanID = chanInfo.ID;
                long voiceObjID = chanInfo.ParentObjID;
                var voiceInfo = ListResourceInfos.FirstOrDefault(r => r.ObjID == voiceObjID) as VoiceServerInfo;
                if (voiceInfo == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                      string.Format("VoiceInfo not exist.\t{0}", voiceObjID));
                    return;
                }
                int monPort = voiceInfo.NMonPort;
                string address = voiceInfo.HostAddress;
                bool isTransAudioData = true;
                if (ConfigInfo != null)
                {
                    var setting =
                        ConfigInfo.ListSettings.FirstOrDefault(s => s.Key == Service10Consts.GS_KEY_S10_TRANSAUDIODATA);
                    if (setting != null)
                    {
                        if (setting.Value == "0")
                        {
                            isTransAudioData = false;
                        }
                    }
                }
                retMessage.Command = (int)Service10Command.ResStartNMon;
                retMessage.ListData.Add(strMonID);
                retMessage.ListData.Add(monPort.ToString());
                retMessage.ListData.Add(isTransAudioData ? "1" : "0");
                retMessage.ListData.Add(address);
                retMessage.ListData.Add(chanID.ToString());
                SendMessage(retMessage);
                if (isTransAudioData)
                {
                    //启动NMonCore进行网络监听
                    NETMON_PARAM param = new NETMON_PARAM();
                    param.Host = voiceInfo.HostAddress;
                    param.Port = voiceInfo.NMonPort;
                    param.Channel = chanID;
                    InitNMon(monObj, param);
                }
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DoStopNMonMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null
                  || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = request.ListData[0];
                LogDebugMessage(request.Command,
                    string.Format("MonID:{0};", strMonID));
                if (mMonType != MonType.NMon)
                {
                    SendErrorMessage(request.Command, Defines.RET_CHECK_FAIL,
                        string.Format("MonType invalid.\t{0}", mMonType));
                    return;
                }
                MonitorObject monObj = mListMonObjects.FirstOrDefault(o => o.MonID == strMonID);
                if (monObj == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                        string.Format("Monitor object not in the monitor list"));
                    return;
                }
                if (mNMonCore == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                      string.Format("NMonCore is null"));
                    return;
                }
                var temp = mNMonCore.User as MonitorObject;
                if (temp == null || temp.MonID != monObj.MonID)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("MonitorObject invalid"));
                    return;
                }
                retMessage.Command = (int)Service10Command.ResStopNMon;
                retMessage.ListData.Add(strMonID);
                SendMessage(retMessage);
                mNMonCore.StopMon();
                mNMonCore = null;
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DoStartSMonMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null
                  || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = request.ListData[0];
                LogDebugMessage(request.Command,
                    string.Format("MonID:{0};", strMonID));
                if (mMonType != MonType.MonScr)
                {
                    SendErrorMessage(request.Command, Defines.RET_CHECK_FAIL,
                        string.Format("MonType invalid.\t{0}", mMonType));
                    return;
                }
                MonitorObject monObj = mListMonObjects.FirstOrDefault(o => o.MonID == strMonID);
                if (monObj == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                        string.Format("Monitor object not in the monitor list"));
                    return;
                }
                string strExt = monObj.Name;
                if (ListExtensionInfos == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                       string.Format("ListExtensionInfos is null"));
                    return;
                }
                var extInfo = ListExtensionInfos.FirstOrDefault(e => e.Extension == strExt);
                if (extInfo == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                        string.Format("ExtInfo not exist.\t{0}", strExt));
                    return;
                }
                if (ListResourceInfos == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                     string.Format("ListResourceInfos is null"));
                    return;
                }
                ScreenChanInfo chanInfo = null;
                for (int i = 0; i < ListResourceInfos.Count; i++)
                {
                    var resource = ListResourceInfos[i];
                    chanInfo = resource as ScreenChanInfo;
                    if (chanInfo == null) { continue; }
                    if (chanInfo.Extension != strExt) { continue; }
                    break;
                }
                if (chanInfo == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                       string.Format("ScreenChanInfo not exist.\t{0}", strExt));
                    return;
                }
                int chanID = chanInfo.ID;
                long screenObjID = chanInfo.ParentObjID;
                var screenInfo = ListResourceInfos.FirstOrDefault(r => r.ObjID == screenObjID) as ScreenServerInfo;
                if (screenInfo == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                     string.Format("ScreenInfo not exist.\t{0}", screenObjID));
                    return;
                }
                int monPort = screenInfo.MonPort;
                string address = screenInfo.HostAddress;
                retMessage.Command = (int) Service10Command.ResStartSMon;
                retMessage.ListData.Add(strMonID);
                retMessage.ListData.Add(monPort.ToString());
                retMessage.ListData.Add(address);
                retMessage.ListData.Add(chanID.ToString());
                SendMessage(retMessage);
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DoStopSMonMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null
                 || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = request.ListData[0];
                LogDebugMessage(request.Command,
                    string.Format("MonID:{0};", strMonID));
                if (mMonType != MonType.MonScr)
                {
                    SendErrorMessage(request.Command, Defines.RET_CHECK_FAIL,
                        string.Format("MonType invalid.\t{0}", mMonType));
                    return;
                }
                MonitorObject monObj = mListMonObjects.FirstOrDefault(o => o.MonID == strMonID);
                if (monObj == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                        string.Format("Monitor object not in the monitor list"));
                    return;
                }
                retMessage.Command = (int)Service10Command.ResStopSMon;
                retMessage.ListData.Add(strMonID);
                SendMessage(retMessage);
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        #endregion


        #region 通知消息

        public void ExtStateChanged(string strExt)
        {
            try
            {
                OperationReturn optReturn;
                var monObj = mListMonObjects.FirstOrDefault(m => m.Name == strExt);
                if (monObj != null)
                {
                    if (ListExtensionInfos != null)
                    {
                        var extInfo = ListExtensionInfos.FirstOrDefault(e => e.Extension == monObj.Name);
                        if (extInfo != null)
                        {
                            NotifyMessage notMessage = new NotifyMessage();
                            notMessage.SessionID = SessionID;
                            notMessage.Command = (int)Service10Command.NotExtStateChanged;
                            optReturn = XMLHelper.SeriallizeObject(extInfo);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("ExtStateChanged fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            notMessage.ListData.Add(monObj.MonID);
                            notMessage.ListData.Add(optReturn.Data.ToString());
                            SendMessage(notMessage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("ExtStateChanged fail.\t{0}\t{1}", strExt, ex.Message));
            }
        }

        #endregion


        #region Commands

        protected override void DoClientCommand(RequestMessage request)
        {
            base.DoClientCommand(request);

            switch (request.Command)
            {
                case (int)Service10Command.ReqSetMonType:
                    DoSetMonTypeMessage(request);
                    break;
                case (int)Service10Command.ReqAddMonObj:
                    DoAddMonObjectMessage(request);
                    break;
                case (int)Service10Command.ReqQueryExtState:
                    DoQueryExtStateMessage(request);
                    break;
                case (int)Service10Command.ReqStartNMon:
                    DoStartNMonMessage(request);
                    break;
                case (int)Service10Command.ReqStopNMon:
                    DoStopNMonMessage(request);
                    break;
                case (int)Service10Command.ReqStartSMon:
                    DoStartSMonMessage(request);
                    break;
                case (int)Service10Command.ReqStopSMon:
                    DoStopSMonMessage(request);
                    break;
            }
        }

        #endregion


        #region Others

        protected override string ParseCommand(int command)
        {
            string str = base.ParseCommand(command);
            if (command >= 1000 && command < 10000)
            {
                str = ((Service10Command)command).ToString();
            }
            return str;
        }

        private void SendErrorMessage(int requestCommand, int errCode, string msg)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = false;
            retMessage.Code = errCode;
            retMessage.Command = (int)RequestCode.NCError;
            retMessage.Message = msg;
            retMessage.ListData.Add(requestCommand.ToString());
            retMessage.ListData.Add(ParseCommand(requestCommand));
            SendMessage(retMessage);

            OnDebug(LogMode.Error, string.Format("Command:{0}\t{1}\t{2}", ParseCommand(requestCommand), errCode, msg));
        }

        private void SendErrorMessage(int requestCommand, OperationReturn optReturn)
        {
            int errCode = optReturn.Code;
            string msg = optReturn.Message;
            SendErrorMessage(requestCommand, errCode, msg);
        }

        private void LogDebugMessage(int requestCommand, string msg)
        {
            OnDebug(LogMode.Debug, string.Format("[{0}]\t{1}", ParseCommand(requestCommand), msg));
        }

        public override void Stop()
        {
            if (mNMonCore != null)
            {
                mNMonCore.StopMon();
            }
            mNMonCore = null;
            base.Stop();
        }

        #endregion


        #region NMon

        private void InitNMon(MonitorObject monObj, NETMON_PARAM param)
        {
            try
            {
                if (mNMonCore != null)
                {
                    mNMonCore.StopMon();
                    mNMonCore = null;
                }
                mNMonCore = new NMonCore(monObj);
                mNMonCore.Debug += (s, msg) => OnDebug(LogMode.Debug, string.Format("{0}\t{1}", s, msg));
                mNMonCore.HeadReceived += NMonCore_HeadReceived;
                mNMonCore.DataReceived += NMonCore_DataReceived;
                mNMonCore.IsPlayWave = false;
                mNMonCore.StartMon(param);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("InitNMon fail.\t{0}", ex.Message));
            }
        }

        void NMonCore_HeadReceived(object s, SNM_RESPONSE head)
        {
            try
            {
                //收到头信息，向客户端通知头信息
                var monObj = s as MonitorObject;
                if (monObj == null)
                {
                    OnDebug(LogMode.Error, string.Format("MonitorObject is null"));
                    return;
                }
                NotifyMessage notMessage = new NotifyMessage();
                notMessage.SessionID = SessionID;
                notMessage.Command = (int)Service10Command.NotNMonHeadReceived;
                notMessage.ListData.Add(monObj.MonID);
                byte[] data = Converter.Struct2Bytes(head);
                string str = Converter.Byte2Hex(data);
                notMessage.ListData.Add(str);
                SendMessage(notMessage);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealNMonHead fail.\t{0}", ex.Message));
            }
        }

        void NMonCore_DataReceived(object s, byte[] data, int length)
        {
            try
            {
                //收到音频数据，向客户端通知音频数据
                var monObj = s as MonitorObject;
                if (monObj == null)
                {
                    OnDebug(LogMode.Error, string.Format("MonitorObject is null"));
                    return;
                }
                NotifyMessage notMessage = new NotifyMessage();
                notMessage.SessionID = SessionID;
                notMessage.Command = (int)Service10Command.NotNMonDataReceived;
                notMessage.ListData.Add(monObj.MonID);
                byte[] temp = new byte[length];
                temp.Initialize();
                Array.Copy(data, 0, temp, 0, length);
                string str = Converter.Byte2Hex(temp);
                notMessage.ListData.Add(length.ToString());
                notMessage.ListData.Add(str);
                SendMessage(notMessage);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealNMonData fail.\t{0}", ex.Message));
            }
        }

        #endregion

    }
}
