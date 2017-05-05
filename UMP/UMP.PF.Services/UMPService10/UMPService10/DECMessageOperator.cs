//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0e829a07-c4d0-44ea-827c-39a2b7469863
//        CLR Version:              4.0.30319.18408
//        Name:                     DECMessageOperator
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10
//        File Name:                DECMessageOperator
//
//        created by Charley at 2016/6/29 09:36:36
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Linq;
using VoiceCyber.Common;
using VoiceCyber.SDKs.DEC;
using VoiceCyber.UMP.CommonService10;


namespace UMPService10
{
    public partial class MonitorServer
    {

        private void DealDECMessage(MessageString message, string strData)
        {
            try
            {
                string strDeviceID;
                string strDirection;
                string strCallerID;
                string strCalledID;
                string strAgentID;
                string strAgentState;
                string strRecordReference;
                DateTime dtRecordTime;
                int intRecordLength;
                ExtensionInfo extInfo;
                ExtStateInfo extStateInfo;

                if (mDECMessageHelper == null) { return; }
                string strMessage = string.Format("{0:X4}:{1:X4}:{2:X4}:{3:X4}",
                    message.LargeType,
                    message.MiddleType,
                    message.SmallType,
                    message.Number);
                switch (strMessage)
                {

                    #region CTI消息处理

                    case DECMessageHelper.MSG_CTI_CALL_EVENT_SERVICEINITIATED:


                        #region 处理摘机消息，摘机即话机处于拨号状态

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_CALL);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_CALL;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = (int)CallState.Dialing;

                            OnDebug(LogMode.Info, "DealDECMessage", string.Format("[{0}]Dialing", strDeviceID));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;
                    case DECMessageHelper.MSG_CTI_CALL_EVENT_DELIVERED:


                        #region 处理振铃消息，由振铃消息可获得呼叫方向及主被叫号码

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_CALL);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_CALL;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = (int)CallState.Ringing;

                            strDirection = mDECMessageHelper.GetCTIDirectionFlagValue(strData);
                            if (strDirection == "1")
                            {
                                //呼入
                                extStateInfo =
                                    extInfo.ListStateInfos.FirstOrDefault(
                                        es =>
                                            es.ObjID == extInfo.ObjID &&
                                            es.StateType == Service10Consts.STATE_TYPE_DIRECTION);
                                if (extStateInfo == null)
                                {
                                    extStateInfo = new ExtStateInfo();
                                    extStateInfo.ObjID = extInfo.ObjID;
                                    extStateInfo.Name = extInfo.Extension;
                                    extStateInfo.StateType = Service10Consts.STATE_TYPE_DIRECTION;
                                    extInfo.ListStateInfos.Add(extStateInfo);
                                }
                                extStateInfo.State = (int)DirectionState.Callin;

                            }
                            if (strDirection == "0")
                            {
                                //呼出
                                extStateInfo =
                                   extInfo.ListStateInfos.FirstOrDefault(
                                       es =>
                                           es.ObjID == extInfo.ObjID &&
                                           es.StateType == Service10Consts.STATE_TYPE_DIRECTION);
                                if (extStateInfo == null)
                                {
                                    extStateInfo = new ExtStateInfo();
                                    extStateInfo.ObjID = extInfo.ObjID;
                                    extStateInfo.Name = extInfo.Extension;
                                    extStateInfo.StateType = Service10Consts.STATE_TYPE_DIRECTION;
                                    extInfo.ListStateInfos.Add(extStateInfo);
                                }
                                extStateInfo.State = (int)DirectionState.Callout;

                            }
                            strCallerID = mDECMessageHelper.GetCTICallerIDValue(strData);
                            strCalledID = mDECMessageHelper.GetCTICalledIDValue(strData);
                            extInfo.CallerID = strCallerID;
                            extInfo.CalledID = strCalledID;

                            OnDebug(LogMode.Info, "DealDECMessage",
                                string.Format("[{0}]Ringing;[{1}] {2}-->{3}", strDeviceID, strDirection, strCallerID, strCalledID));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;
                    case DECMessageHelper.MSG_CTI_CALL_EVENT_ESTABLISHED:


                        #region 处理通话消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                               extInfo.ListStateInfos.FirstOrDefault(
                                   es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_CALL);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_CALL;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = (int)CallState.Talking;

                            OnDebug(LogMode.Info, "DealDECMessage",
                               string.Format("[{0}]Talking", strDeviceID));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;
                    case DECMessageHelper.MSG_CTI_CALL_EVENT_CONNECTIONCLEARED:


                        #region 处理挂机消息，挂机后需清楚呼叫信息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                               extInfo.ListStateInfos.FirstOrDefault(
                                   es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_CALL);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_CALL;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = (int)CallState.Idle;

                            //挂机后，清除呼叫信息
                            extStateInfo =
                                  extInfo.ListStateInfos.FirstOrDefault(
                                      es =>
                                          es.ObjID == extInfo.ObjID &&
                                          es.StateType == Service10Consts.STATE_TYPE_DIRECTION);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_DIRECTION;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = (int)DirectionState.None;

                            extInfo.CallerID = string.Empty;
                            extInfo.CalledID = string.Empty;

                            OnDebug(LogMode.Info, "DealDECMessage",
                               string.Format("[{0}]Idle", strDeviceID));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;

                    case DECMessageHelper.MSG_CTI_CALL_EVENT_AGENTLOGON:


                        #region 处理坐席登录消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                               extInfo.ListStateInfos.FirstOrDefault(
                                   es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_LOGIN);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_LOGIN;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = extStateInfo.State | (int)LoginState.CTI;
                            strAgentID = mDECMessageHelper.GetCTIAgentIDValue(strData);
                            extInfo.AgentID = strAgentID;

                            OnDebug(LogMode.Info, "DealDECMessage",
                                string.Format("[{0}]CTILogOn;{1}", strDeviceID, strAgentID));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;

                    case DECMessageHelper.MSG_CTI_CALL_EVENT_AGENTLOGOFF:


                        #region 处理坐席登出消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                               extInfo.ListStateInfos.FirstOrDefault(
                                   es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_LOGIN);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_LOGIN;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = extStateInfo.State & 6;

                            //如果坐席没有在任何服务器登录，则清除坐席信息
                            if (extStateInfo.State == 0)
                            {
                                extInfo.AgentID = string.Empty;
                            }

                            OnDebug(LogMode.Info, "DealDECMessage",
                                string.Format("[{0}]CTILogOff", strDeviceID));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;

                    case DECMessageHelper.MSG_CTI_CALL_EVENT_AGENTSTATECHANGED:


                        #region 处理坐席状态更新消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            strAgentID = mDECMessageHelper.GetCTIAgentIDValue(strData);
                            strAgentState = mDECMessageHelper.GetCTIAgentStateValue(strData);

                            extStateInfo =
                               extInfo.ListStateInfos.FirstOrDefault(
                                   es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_LOGIN);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_LOGIN;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            if (!string.IsNullOrEmpty(strAgentID))
                            {
                                extStateInfo.State = extStateInfo.State | (int)LoginState.CTI;
                            }
                            else
                            {
                                extStateInfo.State = extStateInfo.State & 6;
                            }

                            extStateInfo =
                              extInfo.ListStateInfos.FirstOrDefault(
                                  es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_AGENT);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_AGENT;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            int intAgentState;
                            if (int.TryParse(strAgentState, out intAgentState))
                            {
                                extStateInfo.State = intAgentState;
                            }
                            string strDescription = ((AgentState)intAgentState).ToString();


                            OnDebug(LogMode.Info, "DealDECMessage",
                                string.Format("[{0}]AgentState;{1};{2}", strDeviceID, strAgentID, strDescription));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;

                    case DECMessageHelper.MSG_CTI_CALL_INFORMATION_CALLINFORMATION:


                        #region 处理查询分机状态的响应消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            strAgentID = mDECMessageHelper.GetCTIAgentIDValue(strData);
                            strAgentState = mDECMessageHelper.GetCTIAgentStateValue(strData);

                            extStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es =>
                                        es.ObjID == extInfo.ObjID &&
                                        es.StateType == Service10Consts.STATE_TYPE_LOGIN);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_LOGIN;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }

                            if (string.IsNullOrEmpty(strAgentID))
                            {
                                //坐席未登录，或没有登录信息
                                extStateInfo.State = extStateInfo.State & 6;
                            }
                            else
                            {
                                extStateInfo.State = extStateInfo.State | (int)LoginState.CTI;
                            }
                            if (string.IsNullOrEmpty(strAgentID))
                            {
                                if (extStateInfo.State == 0)
                                {
                                    extInfo.AgentID = string.Empty;
                                }
                            }
                            else
                            {
                                extInfo.AgentID = strAgentID;
                            }

                            extStateInfo =
                               extInfo.ListStateInfos.FirstOrDefault(
                                   es =>
                                       es.ObjID == extInfo.ObjID &&
                                       es.StateType == Service10Consts.STATE_TYPE_AGENT);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_AGENT;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            int intAgentState;
                            if (int.TryParse(strAgentState, out intAgentState))
                            {
                                extStateInfo.State = intAgentState;
                            }
                            string strDescription = ((AgentState)intAgentState).ToString();


                            OnDebug(LogMode.Info, "DealDECMessage",
                                string.Format("[{0}]QueryState;{1};{2}", strDeviceID, strAgentID, strDescription));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;


                    #endregion


                    #region VoiceServer消息处理

                    case DECMessageHelper.MSG_VOC_RECORDSTARTED:


                        #region 处理开始录音消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_RECORD);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_RECORD;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = extStateInfo.State | (int)RecordState.Voice;

                            strRecordReference = mDECMessageHelper.GetRecordReferenceValue(strData);
                            dtRecordTime = mDECMessageHelper.GetRecordTimeValue(strData);
                            intRecordLength = mDECMessageHelper.GetRecordLengthValue(strData);
                            extInfo.VocRecordReference = strRecordReference;
                            extInfo.VocStartRecordTime = dtRecordTime;
                            DateTime now = DateTime.Now.ToUniversalTime();
                            extInfo.VocTimeDeviation = (now - extInfo.VocStartRecordTime).TotalSeconds - intRecordLength;

                            bool isSetCallInfo = true;
                            var setting =
                                   mConfigInfo.ListSettings.FirstOrDefault(
                                       s => s.Key == Service10Consts.GS_KEY_S10_CLEARCALLINFOATRECORDSTOP);
                            if (setting != null
                                && setting.Value == "0")
                            {
                                isSetCallInfo = false;
                            }
                            if (isSetCallInfo)
                            {
                                //录音启动，设置话机状态处于通话状态
                                var callStateInfo =
                                    extInfo.ListStateInfos.FirstOrDefault(
                                        es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_CALL);
                                if (callStateInfo == null)
                                {
                                    callStateInfo = new ExtStateInfo();
                                    callStateInfo.ObjID = extInfo.ObjID;
                                    callStateInfo.Name = extInfo.Extension;
                                    callStateInfo.StateType = Service10Consts.STATE_TYPE_CALL;
                                    extInfo.ListStateInfos.Add(callStateInfo);
                                }
                                callStateInfo.State = (int)CallState.Talking;
                            }

                            OnDebug(LogMode.Info, "DealDECMessage", string.Format("[{0}]VocRecordStart;{1};{2}", strDeviceID, strRecordReference, dtRecordTime.ToString("yyyy-MM-dd HH:mm:ss")));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;

                    case DECMessageHelper.MSG_VOC_RECORDSTOPPED:


                        #region 处理停止录音消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_RECORD);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_RECORD;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = extStateInfo.State & 2;

                            strRecordReference = mDECMessageHelper.GetRecordReferenceValue(strData);
                            dtRecordTime = mDECMessageHelper.GetRecordTimeValue(strData);
                            extInfo.VocRecordReference = strRecordReference;
                            extInfo.VocStopRecordTime = dtRecordTime;

                            bool isSetCallInfo = true;
                            var setting =
                                   mConfigInfo.ListSettings.FirstOrDefault(
                                       s => s.Key == Service10Consts.GS_KEY_S10_CLEARCALLINFOATRECORDSTOP);
                            if (setting != null
                                && setting.Value == "0")
                            {
                                isSetCallInfo = false;
                            }
                            if (isSetCallInfo)
                            {
                                //录音停止,清除呼叫信息
                                var callStateInfo = extInfo.ListStateInfos.FirstOrDefault(
                                   es =>
                                       es.ObjID == extInfo.ObjID &&
                                       es.StateType == Service10Consts.STATE_TYPE_CALL);
                                if (callStateInfo == null)
                                {
                                    callStateInfo = new ExtStateInfo();
                                    callStateInfo.ObjID = extInfo.ObjID;
                                    callStateInfo.Name = extInfo.Extension;
                                    callStateInfo.StateType = Service10Consts.STATE_TYPE_CALL;
                                    extInfo.ListStateInfos.Add(callStateInfo);
                                }
                                callStateInfo.State = (int)CallState.Idle;

                                var directionStateInfo =
                                       extInfo.ListStateInfos.FirstOrDefault(
                                           es =>
                                               es.ObjID == extInfo.ObjID &&
                                               es.StateType == Service10Consts.STATE_TYPE_DIRECTION);
                                if (directionStateInfo == null)
                                {
                                    directionStateInfo = new ExtStateInfo();
                                    directionStateInfo.ObjID = extInfo.ObjID;
                                    directionStateInfo.Name = extInfo.Extension;
                                    directionStateInfo.StateType = Service10Consts.STATE_TYPE_DIRECTION;
                                    extInfo.ListStateInfos.Add(directionStateInfo);
                                }
                                directionStateInfo.State = (int)DirectionState.None;

                                extInfo.CallerID = string.Empty;
                                extInfo.CalledID = string.Empty;
                            }

                            OnDebug(LogMode.Info, "DealDECMessage", string.Format("[{0}]VocRecordStop;{1};{2}", strDeviceID, strRecordReference, dtRecordTime.ToString("yyyy-MM-dd HH:mm:ss")));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;

                    case DECMessageHelper.MSG_VOC_AGENTLOGON:


                        #region 处理坐席登录消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_LOGIN);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_LOGIN;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = extStateInfo.State | (int)LoginState.Voice;

                            strAgentID = mDECMessageHelper.GetAgentIDValue(strData);
                            extInfo.AgentID = strAgentID;

                            OnDebug(LogMode.Info, "DealDECMessage",
                                string.Format("[{0}]VocLogOn;{1}", strDeviceID, strAgentID));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;

                    case DECMessageHelper.MSG_VOC_AGENTLOGOFF:


                        #region 处理坐席登出消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_LOGIN);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_LOGIN;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = extStateInfo.State & 5;

                            //如果坐席没有在任何服务器登录，则清除坐席信息
                            if (extStateInfo.State == 0)
                            {
                                extInfo.AgentID = string.Empty;
                            }

                            OnDebug(LogMode.Info, "DealDECMessage",
                                string.Format("[{0}]VocLogOff", strDeviceID));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;

                    case DECMessageHelper.MSG_VOC_CALLINFO:


                        #region 处理呼叫信息

                        if (mConfigInfo != null)
                        {
                            var setting =
                                mConfigInfo.ListSettings.FirstOrDefault(
                                    s => s.Key == Service10Consts.GS_KEY_S10_IGNORVOICECALLINFO);
                            if (setting != null && setting.Value == "1")
                            {
                                //忽略录音服务器发来的呼叫信息
                                break;
                            }
                        }
                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {

                            strDirection = mDECMessageHelper.GetDirectionFlagValue(strData);
                            strCallerID = mDECMessageHelper.GetCallerIDValue(strData);
                            strCalledID = mDECMessageHelper.GetCalledIDValue(strData);

                            //呼叫信息
                            var callStateInfo =
                                 extInfo.ListStateInfos.FirstOrDefault(
                                     es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_CALL);
                            if (callStateInfo == null)
                            {
                                callStateInfo = new ExtStateInfo();
                                callStateInfo.ObjID = extInfo.ObjID;
                                callStateInfo.Name = extInfo.Extension;
                                callStateInfo.StateType = Service10Consts.STATE_TYPE_CALL;
                                extInfo.ListStateInfos.Add(callStateInfo);
                            }

                            //呼叫方向
                            var directionStateInfo =
                                   extInfo.ListStateInfos.FirstOrDefault(
                                       es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_DIRECTION);
                            if (directionStateInfo == null)
                            {
                                directionStateInfo = new ExtStateInfo();
                                directionStateInfo.ObjID = extInfo.ObjID;
                                directionStateInfo.Name = extInfo.Extension;
                                directionStateInfo.StateType = Service10Consts.STATE_TYPE_DIRECTION;
                                extInfo.ListStateInfos.Add(directionStateInfo);
                            }

                            //录音信息
                            var recordingStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_RECORD);
                            if (recordingStateInfo == null
                                || (recordingStateInfo.State & 1) <= 0)
                            {
                                //如果当前不在录音，此时呼叫方向置为0，表示没有状态
                                directionStateInfo.State = 0;
                                callStateInfo.State = 0;
                            }
                            else
                            {
                                if (strDirection == "1")
                                {
                                    directionStateInfo.State = (int)DirectionState.Callin;
                                }
                                else
                                {
                                    directionStateInfo.State = (int)DirectionState.Callout;
                                }
                            }

                            extInfo.CallerID = strCallerID;
                            extInfo.CalledID = strCalledID;

                            OnDebug(LogMode.Info, "DealDECMessage",
                                string.Format("[{0}]VocCallInfo;{1};{2}-->{3}", strDeviceID, strDirection, strCallerID,
                                    strCalledID));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;


                    #endregion


                    #region ScreenServer消息处理


                    case DECMessageHelper.MSG_SCR_RECORDSTARTED:


                        #region 处理开始录屏消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_RECORD);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_RECORD;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = extStateInfo.State | (int)RecordState.Screen;

                            strRecordReference = mDECMessageHelper.GetRecordReferenceValue(strData);
                            dtRecordTime = mDECMessageHelper.GetRecordTimeValue(strData);
                            intRecordLength = mDECMessageHelper.GetRecordLengthValue(strData);
                            extInfo.ScrRecordReference = strRecordReference;
                            extInfo.ScrStartRecordTime = dtRecordTime;
                            DateTime now = DateTime.Now.ToUniversalTime();
                            extInfo.ScrTimeDiviation = (now - extInfo.VocStartRecordTime).TotalSeconds - intRecordLength;

                            OnDebug(LogMode.Info, "DealDECMessage", string.Format("[{0}]ScrRecordStart;{1};{2}", strDeviceID, strRecordReference, dtRecordTime.ToString("yyyy-MM-dd HH:mm:ss")));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;

                    case DECMessageHelper.MSG_SCR_RECORDSTOPPED:


                        #region 处理停止录屏消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_RECORD);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_RECORD;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = extStateInfo.State & 1;

                            strRecordReference = mDECMessageHelper.GetRecordReferenceValue(strData);
                            dtRecordTime = mDECMessageHelper.GetRecordTimeValue(strData);
                            extInfo.ScrRecordReference = strRecordReference;
                            extInfo.ScrStopRecordTime = dtRecordTime;

                            OnDebug(LogMode.Info, "DealDECMessage", string.Format("[{0}]ScrRecordStop;{1};{2}", strDeviceID, strRecordReference, dtRecordTime.ToString("yyyy-MM-dd HH:mm:ss")));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;

                    case DECMessageHelper.MSG_SCR_AGENTLOGON:


                        #region 处理坐席登录消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_LOGIN);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_LOGIN;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = extStateInfo.State | (int)LoginState.Screen;

                            strAgentID = mDECMessageHelper.GetAgentIDValue(strData);
                            extInfo.AgentID = strAgentID;

                            OnDebug(LogMode.Info, "DealDECMessage",
                                string.Format("[{0}]ScrLogOn;{1}", strDeviceID, strAgentID));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;

                    case DECMessageHelper.MSG_SCR_AGENTLOGOFF:


                        #region 处理坐席登出消息

                        strDeviceID = mDECMessageHelper.GetDeviceIDValue(strData);
                        extInfo = mListExtensionInfos.FirstOrDefault(e => e.Extension == strDeviceID);
                        if (extInfo != null)
                        {
                            extStateInfo =
                                extInfo.ListStateInfos.FirstOrDefault(
                                    es => es.ObjID == extInfo.ObjID && es.StateType == Service10Consts.STATE_TYPE_LOGIN);
                            if (extStateInfo == null)
                            {
                                extStateInfo = new ExtStateInfo();
                                extStateInfo.ObjID = extInfo.ObjID;
                                extStateInfo.Name = extInfo.Extension;
                                extStateInfo.StateType = Service10Consts.STATE_TYPE_LOGIN;
                                extInfo.ListStateInfos.Add(extStateInfo);
                            }
                            extStateInfo.State = extStateInfo.State & 3;

                            //如果坐席没有在任何服务器登录，则清除坐席信息
                            if (extStateInfo.State == 0)
                            {
                                extInfo.AgentID = string.Empty;
                            }

                            OnDebug(LogMode.Info, "DealDECMessage",
                                string.Format("[{0}]ScrLogOff", strDeviceID));

                            ExtStateChanged(strDeviceID);
                        }

                        #endregion


                        break;


                    #endregion

                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealDECMessage fail.\t{0}", ex.Message));
            }
        }

        public void ExtStateChanged(string strExt)
        {
            try
            {
                for (int i = 0; i < mListMonitorSessions.Count; i++)
                {
                    var session = mListMonitorSessions[i] as MonitorOperations;
                    if (session != null)
                    {
                        session.ExtStateChanged(strExt);
                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("ExtStateChanged fail.\t{0}\t{1}", strExt, ex.Message));
            }
        }

    }
}
