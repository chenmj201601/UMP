//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    33f1ea66-e327-4b34-a49d-32493af48738
//        CLR Version:              4.0.30319.18063
//        Name:                     MonitorOperations
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                MonitorOperations
//
//        created by Charley at 2015/6/26 10:48:00
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
using VoiceCyber.UMP.CommonService04;
using VoiceCyber.UMP.Communications;

/*
 * ======================================================================
 * 
 * MonitorOperation 工作逻辑
 * 
 * 说明：
 * MonitorOperation 实际上仍然是MonitorSession，在这里主要是处理各种消息
 * 1、请求消息
 * 2、通知消息
 * 
 * 这里目前有以下功能
 * 1、状态监视
 *      状态监视主要处理一下消息：
 *      请求消息
 *      （1）设置监视方式（SetMonType）
 *      （2）添加监视对象（AddMonObj）
 *      （3）查询通道信息（QueryChan）
 *      （4）查询状态信息（QueryState）
 *      另（5）移除监视对象（RemoveMonObj）
 *      注：
 *          i：以上前四个消息需要按此顺序进行，不能颠倒顺序，也就是说前一个消息处理成功了，后一个消息才能处理
 *          ii：移除监视对象后就不能再处理其他消息了
 *      通知消息
 *      （1）通道状态更新（StateChanged）
 *          该消息中包含发生变化的监视对象信息（MonitorObject）和通道当前状态的消息（ChanState）
 *      
 * 2、实时监听
 *      实时监听同样要处理状态监视的所有消息，实时监听增加了以下消息
 *      请求消息
 *      （1）开始监听（StartNMon）
 *      （2）停止监听（StopNMon）
 *      通知消息
 *      （1）音频头信息（NMonHead）
 *      （2）音频数据信息（NMonData）
 *      实时监听的逻辑过程
 *      （1）客户端发送开始监听的请求消息
 *      （2）服务端接收到开始监听的请求消息后，查询得到通道所在服务器的地址和监听端口
 *      （3）创建NMonCore，使用查询到的地址和端口连接录音服务器
 *      （4）连接成功后，服务器会返回音频头信息（SNM_Response)，然后把此音频头信息用Hex编码后发送给客户端，即NMonHead通知消息
 *      （5）当从录音服务器接收到音频数据信息时，把音频数据信息转发给客户端，即NMonData通知消息
 *      （6）客户端如果想停止监听，发送停止监听的请求消息（StopMon），服务器收到此消息，调用NMonCore的StopMon方法停止监听
 *      
 * 3、实时监屏
 *      实时监屏同样要处理状态监视的所有消息，实时监屏增加了以下消息
 *      请求消息
 *      （1）开始监屏（StartSMon）
 *      （2）停止监屏（StopSMon）
 *      实时监屏的逻辑过程
 *      （1）客户端发送开始监屏的请求消息
 *      （2）服务端接收到开始监屏的请求消息后，查询到通道所在服务器的地址和监屏端口
 *      （3）服务端将监屏服务器的信息返回给客户端，客户端直接连接监屏服务器实时监视屏幕
 *      （4）客户端发送停止监屏的消息，服务端没有做任何操作
 * 
 * ======================================================================
 */

namespace UMPService04
{
    public class MonitorOperations : NetSession
    {

        #region Members

        public string RootDir;
        public List<ChanState> ListAllChanStates;
        public List<ResourceConfigInfo> ListAllResources;
        public ConfigInfo ConfigInfo;

        private MonitorType mMonitorType;
        private List<MonitorObject> mListMonitorObjects;
        private NMonCore mNMonCore;

        #endregion


        public MonitorOperations(TcpClient tcpClient)
            : base(tcpClient)
        {
            mListMonitorObjects = new List<MonitorObject>();
            mMonitorType = MonitorType.None;
        }


        #region 处理请求消息

        private void DoSetMonTypeMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null || request.ListData.Count < 1)
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
                MonitorType oldType = mMonitorType;
                MonitorType newType = (MonitorType)intValue;
                mMonitorType = newType;
                retMessage.Command = (int)Service04Command.ResSetMonType;
                retMessage.ListData.Add(((int)newType).ToString());
                retMessage.ListData.Add(((int)oldType).ToString());
                SendMessage(retMessage);
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DoAddMonObjMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strCount = request.ListData[0];
                LogDebugMessage(request.Command, string.Format("Args:{0}", strCount));
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("Count param invalid\t{0}", strCount));
                    return;
                }
                if (request.ListData.Count < intCount + 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                       string.Format("MonObj count invalid\t{0}", strCount));
                    return;
                }
                OperationReturn optReturn;
                int validCount = 0;
                List<string> listMonIDs = new List<string>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = request.ListData[i + 1];
                    optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        SendErrorMessage(request.Command, optReturn);
                        return;
                    }
                    MonitorObject monObj = optReturn.Data as MonitorObject;
                    if (monObj == null)
                    {
                        SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                            string.Format("MonitorObject is null"));
                        return;
                    }
                    if (monObj.MonType == mMonitorType)
                    {
                        string strMonID = Guid.NewGuid().ToString();
                        monObj.MonID = strMonID;
                        mListMonitorObjects.Add(monObj);
                        validCount++;
                        listMonIDs.Add(strMonID);
                    }
                }
                retMessage.Command = (int)Service04Command.ResAddMonObj;
                retMessage.ListData.Add(validCount.ToString());
                for (int i = 0; i < validCount; i++)
                {
                    retMessage.ListData.Add(listMonIDs[i]);
                }
                SendMessage(retMessage);
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DoRemoveMonObjMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strCount = request.ListData[0];
                LogDebugMessage(request.Command, string.Format("Args:{0}", strCount));
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("Count param invalid\t{0}", strCount));
                    return;
                }
                if (request.ListData.Count < intCount + 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                       string.Format("MonObj count invalid\t{0}", strCount));
                    return;
                }
                int validCount = 0;
                List<string> listMonIDs = new List<string>();
                for (int i = 0; i < intCount; i++)
                {
                    string strMonID = request.ListData[i + 1];
                    var temp = mListMonitorObjects.FirstOrDefault(o => o.MonID == strMonID);
                    if (temp != null)
                    {
                        mListMonitorObjects.Remove(temp);
                        validCount++;
                        listMonIDs.Add(strMonID);
                    }
                }
                retMessage.Command = (int)Service04Command.ResRemoveMonObj;
                retMessage.ListData.Add(validCount.ToString());
                for (int i = 0; i < validCount; i++)
                {
                    retMessage.ListData.Add(listMonIDs[i]);
                }
                SendMessage(retMessage);
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DoQueryChanMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strCount = request.ListData[0];
                LogDebugMessage(request.Command, string.Format("Args:{0}", strCount));
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("Count param invalid\t{0}", strCount));
                    return;
                }
                if (request.ListData.Count < intCount + 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                       string.Format("MonObj count invalid\t{0}", strCount));
                    return;
                }
                if (ListAllChanStates == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                        string.Format("ListAllChanStates is null"));
                    return;
                }
                if (ListAllResources == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                       string.Format("ListAllResources is null"));
                    return;
                }
                OperationReturn optReturn;
                int validCount = 0;
                List<string> listInfos = new List<string>();
                for (int i = 0; i < intCount; i++)
                {
                    string strMonID = request.ListData[i + 1];
                    MonitorObject monObj = mListMonitorObjects.FirstOrDefault(o => o.MonID == strMonID);
                    if (monObj == null)
                    {
                        SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                           string.Format("Monitor object not in the monitor list"));
                        return;
                    }

                    #region 查询监视对象当前所在的通道信息

                    ChanState chanState = null;
                    VoiceServerInfo voiceServer;
                    ScreenServerInfo screenServer;
                    VoiceChanInfo voiceChannel;
                    ScreenChanInfo screenChannel;
                    int role = monObj.Role;
                    int objType = monObj.ObjType;
                    switch (objType)
                    {
                        case ConstValue.RESOURCE_AGENT:
                            if (role == 1)
                            {
                                chanState =
                                    ListAllChanStates.FirstOrDefault(
                                        s =>
                                            s.AgentID == monObj.ObjValue &&
                                            s.ObjType == VoiceChanInfo.RESOURCE_VOICECHANNEL);
                            }
                            if (role == 2)
                            {
                                chanState =
                                    ListAllChanStates.FirstOrDefault(
                                        s =>
                                            s.AgentID == monObj.ObjValue &&
                                            s.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL);
                            }
                            break;
                        case ConstValue.RESOURCE_EXTENSION:
                            
                            //考虑到分机可能有重复的，即不同服务器上有相同的分机号
                            if (role == 1)
                            {
                                if (string.IsNullOrEmpty(monObj.Other03))
                                {
                                    chanState =
                                        ListAllChanStates.FirstOrDefault(
                                            s =>
                                                s.Extension == monObj.ObjValue &&
                                                s.ObjType == VoiceChanInfo.RESOURCE_VOICECHANNEL);
                                }
                                else
                                {
                                    chanState =
                                        ListAllChanStates.FirstOrDefault(
                                            s =>
                                                s.Extension == monObj.ObjValue &&
                                                s.Other03 == monObj.Other03 &&
                                                s.ObjType == VoiceChanInfo.RESOURCE_VOICECHANNEL);
                                }
                            }
                            if (role == 2)
                            {
                                if (string.IsNullOrEmpty(monObj.Other03))
                                {
                                    chanState =
                                        ListAllChanStates.FirstOrDefault(
                                            s =>
                                                s.Extension == monObj.ObjValue &&
                                                s.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL);
                                }
                                else
                                {
                                    chanState =
                                        ListAllChanStates.FirstOrDefault(
                                            s =>
                                                s.Extension == monObj.ObjValue &&
                                                s.Other03 == monObj.Other03 &&
                                                s.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL);
                                }
                            }
                            break;
                        case ConstValue.RESOURCE_REALEXT:
                            if (role == 1)
                            {
                                chanState =
                                    ListAllChanStates.FirstOrDefault(
                                        s =>
                                            s.RealExt == monObj.ObjValue &&
                                            s.ObjType == VoiceChanInfo.RESOURCE_VOICECHANNEL);
                            }
                            if (role == 2)
                            {
                                chanState =
                                    ListAllChanStates.FirstOrDefault(
                                        s =>
                                            s.RealExt == monObj.ObjValue &&
                                            s.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL);
                            }
                            break;
                    }

                    if (chanState != null)
                    {
                        //获取通道资源编码
                        monObj.ChanObjID = chanState.ObjID;
                        if (role == 1)
                        {
                            voiceChannel = ListAllResources.FirstOrDefault(r => r.ObjID == monObj.ChanObjID) as VoiceChanInfo;
                            if (voiceChannel != null)
                            {
                                //获取通道ID
                                monObj.Other01 = voiceChannel.ID.ToString();
                                voiceServer =
                                    ListAllResources.FirstOrDefault(r => r.ObjID == voiceChannel.ParentObjID) as
                                        VoiceServerInfo;
                                if (voiceServer != null)
                                {
                                    //获取服务器ID
                                    monObj.Other02 = voiceServer.ID.ToString();
                                    //获取服务器地址
                                    monObj.Other03 = voiceServer.HostAddress;
                                    //获取服务器的资源编码
                                    monObj.Other04 = voiceServer.ObjID.ToString();
                                }
                            }
                        }
                        if (role == 2)
                        {
                            screenChannel = ListAllResources.FirstOrDefault(r => r.ObjID == monObj.ChanObjID) as ScreenChanInfo;
                            if (screenChannel != null)
                            {
                                //获取通道ID
                                monObj.Other01 = screenChannel.ID.ToString();
                                screenServer =
                                    ListAllResources.FirstOrDefault(r => r.ObjID == screenChannel.ParentObjID) as
                                        ScreenServerInfo;
                                if (screenServer != null)
                                {
                                    //获取服务器ID
                                    monObj.Other02 = screenServer.ID.ToString();
                                    //获取服务器地址
                                    monObj.Other03 = screenServer.HostAddress;
                                    //获取服务器的资源编码
                                    monObj.Other04 = screenServer.ObjID.ToString();
                                }
                            }
                        }
                    }

                    #endregion


                    optReturn = XMLHelper.SeriallizeObject(monObj);
                    if (!optReturn.Result)
                    {
                        SendErrorMessage(request.Command, optReturn.Code, optReturn.Message);
                        return;
                    }
                    validCount++;
                    listInfos.Add(optReturn.Data.ToString());
                }
                retMessage.Command = (int)Service04Command.ResQueryChan;
                retMessage.ListData.Add(validCount.ToString());
                for (int i = 0; i < validCount; i++)
                {
                    retMessage.ListData.Add(listInfos[i]);
                }
                SendMessage(retMessage);
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DoAddQueryChanMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strCount = request.ListData[0];
                LogDebugMessage(request.Command, string.Format("Args:{0}", strCount));
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("Count param invalid\t{0}", strCount));
                    return;
                }
                if (request.ListData.Count < intCount + 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                       string.Format("MonObj count invalid\t{0}", strCount));
                    return;
                }
                OperationReturn optReturn;
                int validCount = 0;
                List<string> listInfos = new List<string>();
                for (int i = 0; i < intCount; i++)
                {
                    string strInfo = request.ListData[i + 1];
                    optReturn = XMLHelper.DeserializeObject<MonitorObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        SendErrorMessage(request.Command, optReturn);
                        return;
                    }
                    MonitorObject monObj = optReturn.Data as MonitorObject;
                    if (monObj == null)
                    {
                        SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                            string.Format("MonitorObject is null"));
                        return;
                    }
                    if (monObj.MonType == mMonitorType)
                    {
                        string strMonID = Guid.NewGuid().ToString();
                        monObj.MonID = strMonID;


                        #region 查询监视对象当前所在的通道信息

                        ChanState chanState = null;
                        VoiceServerInfo voiceServer;
                        ScreenServerInfo screenServer;
                        VoiceChanInfo voiceChannel;
                        ScreenChanInfo screenChannel;
                        int role = monObj.Role;
                        int objType = monObj.ObjType;
                        switch (objType)
                        {
                            case ConstValue.RESOURCE_AGENT:
                                if (role == 1)
                                {
                                    chanState =
                                        ListAllChanStates.FirstOrDefault(
                                            s =>
                                                s.AgentID == monObj.ObjValue &&
                                                s.ObjType == VoiceChanInfo.RESOURCE_VOICECHANNEL);
                                }
                                if (role == 2)
                                {
                                    chanState =
                                        ListAllChanStates.FirstOrDefault(
                                            s =>
                                                s.AgentID == monObj.ObjValue &&
                                                s.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL);
                                }
                                break;
                            case ConstValue.RESOURCE_EXTENSION:

                                //考虑到分机可能有重复的，即不同服务器上有相同的分机号
                                if (role == 1)
                                {
                                    if (string.IsNullOrEmpty(monObj.Other03))
                                    {
                                        chanState =
                                            ListAllChanStates.FirstOrDefault(
                                                s =>
                                                    s.Extension == monObj.ObjValue &&
                                                    s.ObjType == VoiceChanInfo.RESOURCE_VOICECHANNEL);
                                    }
                                    else
                                    {
                                        chanState =
                                            ListAllChanStates.FirstOrDefault(
                                                s =>
                                                    s.Extension == monObj.ObjValue &&
                                                    s.Other03 == monObj.Other03 &&
                                                    s.ObjType == VoiceChanInfo.RESOURCE_VOICECHANNEL);
                                    }
                                }
                                if (role == 2)
                                {
                                    if (string.IsNullOrEmpty(monObj.Other03))
                                    {
                                        chanState =
                                            ListAllChanStates.FirstOrDefault(
                                                s =>
                                                    s.Extension == monObj.ObjValue &&
                                                    s.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL);
                                    }
                                    else
                                    {
                                        chanState =
                                            ListAllChanStates.FirstOrDefault(
                                                s =>
                                                    s.Extension == monObj.ObjValue &&
                                                    s.Other03 == monObj.Other03 &&
                                                    s.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL);
                                    }
                                }
                                break;
                            case ConstValue.RESOURCE_REALEXT:
                                if (role == 1)
                                {
                                    chanState =
                                        ListAllChanStates.FirstOrDefault(
                                            s =>
                                                s.RealExt == monObj.ObjValue &&
                                                s.ObjType == VoiceChanInfo.RESOURCE_VOICECHANNEL);
                                }
                                if (role == 2)
                                {
                                    chanState =
                                        ListAllChanStates.FirstOrDefault(
                                            s =>
                                                s.RealExt == monObj.ObjValue &&
                                                s.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL);
                                }
                                break;
                        }
                        if (chanState != null)
                        {
                            //获取通道资源编码
                            monObj.ChanObjID = chanState.ObjID;
                            if (role == 1)
                            {
                                voiceChannel = ListAllResources.FirstOrDefault(r => r.ObjID == monObj.ChanObjID) as VoiceChanInfo;
                                if (voiceChannel != null)
                                {
                                    //获取通道ID
                                    monObj.Other01 = voiceChannel.ID.ToString();
                                    voiceServer =
                                        ListAllResources.FirstOrDefault(r => r.ObjID == voiceChannel.ParentObjID) as
                                            VoiceServerInfo;
                                    if (voiceServer != null)
                                    {
                                        //获取服务器ID
                                        monObj.Other02 = voiceServer.ID.ToString();
                                        //获取服务器地址
                                        monObj.Other03 = voiceServer.HostAddress;
                                        //获取服务器的资源编码
                                        monObj.Other04 = voiceServer.ObjID.ToString();
                                    }
                                }
                            }
                            if (role == 2)
                            {
                                screenChannel = ListAllResources.FirstOrDefault(r => r.ObjID == monObj.ChanObjID) as ScreenChanInfo;
                                if (screenChannel != null)
                                {
                                    //获取通道ID
                                    monObj.Other01 = screenChannel.ID.ToString();
                                    screenServer =
                                        ListAllResources.FirstOrDefault(r => r.ObjID == screenChannel.ParentObjID) as
                                            ScreenServerInfo;
                                    if (screenServer != null)
                                    {
                                        //获取服务器ID
                                        monObj.Other02 = screenServer.ID.ToString();
                                        //获取服务器地址
                                        monObj.Other03 = screenServer.HostAddress;
                                        //获取服务器的资源编码
                                        monObj.Other04 = screenServer.ObjID.ToString();
                                    }
                                }
                            }
                        }

                        #endregion


                        optReturn = XMLHelper.SeriallizeObject(monObj);
                        if (!optReturn.Result)
                        {
                            SendErrorMessage(request.Command, optReturn.Code, optReturn.Message);
                            return;
                        }
                        listInfos.Add(optReturn.Data.ToString());
                        mListMonitorObjects.Add(monObj);
                        validCount++;
                    }
                }
                retMessage.Command = (int)Service04Command.ResAddQueryChan;
                retMessage.ListData.Add(validCount.ToString());
                for (int i = 0; i < validCount; i++)
                {
                    retMessage.ListData.Add(listInfos[i]);
                }
                SendMessage(retMessage);
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        private void DoQueryStateMessage(RequestMessage request)
        {
            ReturnMessage retMessage = new ReturnMessage();
            retMessage.Result = true;
            retMessage.SessionID = SessionID;
            retMessage.Code = 0;
            try
            {
                if (request.ListData == null || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = request.ListData[0];
                LogDebugMessage(request.Command, string.Format("Args:{0}", strMonID));
                MonitorObject monObj = mListMonitorObjects.FirstOrDefault(o => o.MonID == strMonID);
                if (monObj == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                        string.Format("Monitor object not in the monitor list"));
                    return;
                }
                //查询通道的状态
                if (ListAllChanStates == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                        string.Format("ListAllChanStates is null"));
                    return;
                }
                var chanState = ListAllChanStates.FirstOrDefault(c => c.ObjID == monObj.ChanObjID);
                if (chanState == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                      string.Format("ChannelState not exist."));
                    return;
                }
                retMessage.Command = (int)Service04Command.ResQueryState;
                retMessage.ListData.Add(strMonID);
                OperationReturn optReturn = XMLHelper.SeriallizeObject(chanState);
                if (!optReturn.Result)
                {
                    SendErrorMessage(request.Command, optReturn);
                    return;
                }
                retMessage.ListData.Add(optReturn.Data.ToString());
                SendMessage(retMessage);
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
                if (request.ListData == null || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = request.ListData[0];
                LogDebugMessage(request.Command, string.Format("Args:{0}", strMonID));
                MonitorObject monObj = mListMonitorObjects.FirstOrDefault(o => o.MonID == strMonID);
                if (monObj == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                        string.Format("Monitor object not in the monitor list"));
                    return;
                }
                if (monObj.MonType != MonitorType.NMon
                    || mMonitorType != MonitorType.NMon)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("MonType invalid"));
                    return;
                }
                if (monObj.ChanObjID <= 0)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ChanObjID invalid"));
                    return;
                }
                if (ListAllResources == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                        string.Format("ListAllResources is null"));
                    return;
                }
                var channel = ListAllResources.FirstOrDefault(r => r.ObjID == monObj.ChanObjID);
                if (channel == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                        string.Format("Channel is null"));
                    return;
                }
                var voice = ListAllResources.FirstOrDefault(r => r.ObjID == channel.ParentObjID) as VoiceServerInfo;
                if (voice == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                        string.Format("Voice is null"));
                    return;
                }
                retMessage.Command = (int)Service04Command.ResStartNMon;
                retMessage.ListData.Add(strMonID);
                retMessage.ListData.Add(voice.NMonPort.ToString());
                string strTransAudio = "1";
                if (ConfigInfo != null)
                {
                    var setting =
                        ConfigInfo.ListSettings.FirstOrDefault(s => s.Key == Service04Consts.GS_KEY_S04_TRANS_AUDIO_DATA);
                    if (setting != null)
                    {
                        if (setting.Value == "0")
                        {
                            strTransAudio = "0";
                        }
                    }
                }
                retMessage.ListData.Add(strTransAudio);
                SendMessage(retMessage);
                if (strTransAudio == "1")
                {
                    //启动NMonCore进行网络监听
                    NETMON_PARAM param = new NETMON_PARAM();
                    param.Host = voice.HostAddress;
                    param.Port = voice.NMonPort;
                    param.Channel = channel.ID;
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
                if (request.ListData == null || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = request.ListData[0];
                LogDebugMessage(request.Command, string.Format("Args:{0}", strMonID));
                MonitorObject monObj = mListMonitorObjects.FirstOrDefault(o => o.MonID == strMonID);
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
                retMessage.Command = (int)Service04Command.ResStopNMon;
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
                if (request.ListData == null || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = request.ListData[0];
                LogDebugMessage(request.Command, string.Format("Args:{0}", strMonID));
                MonitorObject monObj = mListMonitorObjects.FirstOrDefault(o => o.MonID == strMonID);
                if (monObj == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                        string.Format("Monitor object not in the monitor list"));
                    return;
                }
                if (monObj.MonType != MonitorType.MonScr
                    || mMonitorType != MonitorType.MonScr)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("MonType invalid"));
                    return;
                }
                if (monObj.ChanObjID <= 0)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ChanObjID invalid"));
                    return;
                }
                if (ListAllResources == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                        string.Format("ListAllResources is null"));
                    return;
                }
                var channel = ListAllResources.FirstOrDefault(r => r.ObjID == monObj.ChanObjID);
                if (channel == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                        string.Format("Channel is null"));
                    return;
                }
                var screen = ListAllResources.FirstOrDefault(r => r.ObjID == channel.ParentObjID) as ScreenServerInfo;
                if (screen == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_OBJECT_NULL,
                        string.Format("Screen is null"));
                    return;
                }
                int intMonPort = screen.MonPort;
                retMessage.Command = (int)Service04Command.ResStartSMon;
                retMessage.ListData.Add(strMonID);
                retMessage.ListData.Add(intMonPort.ToString());
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
                if (request.ListData == null || request.ListData.Count < 1)
                {
                    SendErrorMessage(request.Command, Defines.RET_PARAM_INVALID,
                        string.Format("ListData is null or count invalid"));
                    return;
                }
                string strMonID = request.ListData[0];
                LogDebugMessage(request.Command, string.Format("Args:{0}", strMonID));
                MonitorObject monObj = mListMonitorObjects.FirstOrDefault(o => o.MonID == strMonID);
                if (monObj == null)
                {
                    SendErrorMessage(request.Command, Defines.RET_NOT_EXIST,
                        string.Format("Monitor object not in the monitor list"));
                    return;
                }
                retMessage.Command = (int)Service04Command.ResStopSMon;
                retMessage.ListData.Add(strMonID);
                SendMessage(retMessage);
            }
            catch (Exception ex)
            {
                SendErrorMessage(request.Command, Defines.RET_FAIL, ex.Message);
            }
        }

        #endregion


        #region 处理通知消息

        public void DoNotifyMessage(ChanState chanState)
        {
            try
            {
                OperationReturn optReturn;
                MonitorObject monObj;

                long chanObjID = chanState.ObjID;
                string strExtension = chanState.Extension;
                string strAgent = chanState.AgentID;
                string strRealExt = chanState.RealExt;
                int chanType = chanState.ObjType;
                for (int i = 0; i < mListMonitorObjects.Count; i++)
                {
                    monObj = mListMonitorObjects[i];
                    int role = monObj.Role;
                    long locateChanObjID = monObj.ChanObjID;
                    NotifyMessage notifyMessage;
                    switch (monObj.ObjType)
                    {

                        #region Channel

                        case ConstValue.RESOURCE_VOICECHANNEL:

                            break;

                        #endregion


                        #region Agent

                        case ConstValue.RESOURCE_AGENT:

                            if (locateChanObjID == chanObjID)
                            {
                                //已经关联通道，将关联的通道状态发送给客户端
                                notifyMessage = new NotifyMessage();
                                notifyMessage.Command = (int)Service04Command.NotStateChanged;
                                notifyMessage.ListData.Add(monObj.MonID);   //MonID
                                optReturn = XMLHelper.SeriallizeObject(chanState);
                                if (!optReturn.Result)
                                {
                                    OnDebug(LogMode.Error,
                                        string.Format("DoNotifyMessage fail.\t{0}\t{1}", optReturn.Code,
                                            optReturn.Message));
                                    return;
                                }
                                notifyMessage.ListData.Add(optReturn.Data.ToString());  //ChanState
                                notifyMessage.ListData.Add("0"); //ChanObjID，0表示关联的通道没有改变
                                notifyMessage.ListData.Add(monObj.MonID);   //MonID
                                optReturn = XMLHelper.SeriallizeObject(chanState);
                                if (!optReturn.Result)
                                {
                                    OnDebug(LogMode.Error,
                                        string.Format("DoNotifyMessage fail.\t{0}\t{1}", optReturn.Code,
                                            optReturn.Message));
                                    return;
                                }
                                notifyMessage.ListData.Add(optReturn.Data.ToString());  //ChanState
                                notifyMessage.ListData.Add("0"); //ChanObjID，0表示关联的通道没有改变
                                OnDebug(LogMode.Debug,
                                    string.Format("{0}\t{1}", ParseCommand(notifyMessage.Command), monObj.MonID));
                                SendMessage(notifyMessage);
                            }
                            else
                            {
                                //尚未关联通道或关联的通道变了，将新关联的通道状态和通道编码发送给客户端
                                if ((role == 1 && chanType == VoiceChanInfo.RESOURCE_VOICECHANNEL)
                                    || (role == 2 && chanType == ScreenChanInfo.RESOURCE_SCREENCHANNEL))
                                {
                                    if (strAgent == monObj.ObjValue)
                                    {
                                        //坐席登录到新通道上
                                        monObj.ChanObjID = chanObjID;
                                        notifyMessage = new NotifyMessage();
                                        notifyMessage.Command = (int)Service04Command.NotStateChanged;
                                        notifyMessage.ListData.Add(monObj.MonID);   //MonID
                                        optReturn = XMLHelper.SeriallizeObject(chanState);
                                        if (!optReturn.Result)
                                        {
                                            OnDebug(LogMode.Error,
                                                string.Format("DoNotifyMessage fail.\t{0}\t{1}", optReturn.Code,
                                                    optReturn.Message));
                                            return;
                                        }
                                        notifyMessage.ListData.Add(optReturn.Data.ToString());  //ChanState
                                        notifyMessage.ListData.Add(chanObjID.ToString());       //ChanObjID
                                        OnDebug(LogMode.Debug,
                                            string.Format("{0}\t{1}", ParseCommand(notifyMessage.Command), monObj.MonID));
                                        SendMessage(notifyMessage);
                                    }
                                }
                            }

                            break;

                        #endregion


                        #region Extension

                        case ConstValue.RESOURCE_EXTENSION:

                            ////////////分机肯定都关联了固定的通道的，无需做是否关联通道的判断
                            if (locateChanObjID == chanObjID)
                            {
                                //已经关联通道，将关联的通道状态发送给客户端
                                notifyMessage = new NotifyMessage();
                                notifyMessage.Command = (int)Service04Command.NotStateChanged;
                                notifyMessage.ListData.Add(monObj.MonID);   //MonID
                                optReturn = XMLHelper.SeriallizeObject(chanState);
                                if (!optReturn.Result)
                                {
                                    OnDebug(LogMode.Error,
                                        string.Format("DoNotifyMessage fail.\t{0}\t{1}", optReturn.Code,
                                            optReturn.Message));
                                    return;
                                }
                                notifyMessage.ListData.Add(optReturn.Data.ToString());  //ChanState
                                notifyMessage.ListData.Add("0"); //ChanObjID，0表示关联的通道没有改变
                                OnDebug(LogMode.Debug,
                                    string.Format("{0}\t{1}", ParseCommand(notifyMessage.Command), monObj.MonID));
                                SendMessage(notifyMessage);
                            }
                            //else
                            //{
                            //    //尚未关联通道或关联的通道变了，将新关联的通道状态和通道编码发送给客户端
                            //    if ((role == 1 && chanType == VoiceChanInfo.RESOURCE_VOICECHANNEL)
                            //        || (role == 2 && chanType == ScreenChanInfo.RESOURCE_SCREENCHANNEL))
                            //    {
                            //        if (strExtension == monObj.ObjValue)
                            //        {
                            //            //坐席登录到新通道上
                            //            monObj.ChanObjID = chanObjID;
                            //            notifyMessage = new NotifyMessage();
                            //            notifyMessage.Command = (int)Service04Command.NotStateChanged;
                            //            notifyMessage.ListData.Add(monObj.MonID);   //MonID
                            //            optReturn = XMLHelper.SeriallizeObject(chanState);
                            //            if (!optReturn.Result)
                            //            {
                            //                OnDebug(LogMode.Error,
                            //                    string.Format("DoNotifyMessage fail.\t{0}\t{1}", optReturn.Code,
                            //                        optReturn.Message));
                            //                return;
                            //            }
                            //            notifyMessage.ListData.Add(optReturn.Data.ToString());  //ChanState
                            //            notifyMessage.ListData.Add(chanObjID.ToString());       //ChanObjID
                            //            OnDebug(LogMode.Debug,
                            //                string.Format("{0}\t{1}", ParseCommand(notifyMessage.Command), monObj.MonID));
                            //            SendMessage(notifyMessage);
                            //        }
                            //    }
                            //}
                            break;

                        #endregion


                        #region RealExt

                        case ConstValue.RESOURCE_REALEXT:

                            if (locateChanObjID == chanObjID)
                            {
                                //已经关联通道，将关联的通道状态发送给客户端
                                notifyMessage = new NotifyMessage();
                                notifyMessage.Command = (int)Service04Command.NotStateChanged;
                                notifyMessage.ListData.Add(monObj.MonID);   //MonID
                                optReturn = XMLHelper.SeriallizeObject(chanState);
                                if (!optReturn.Result)
                                {
                                    OnDebug(LogMode.Error,
                                        string.Format("DoNotifyMessage fail.\t{0}\t{1}", optReturn.Code,
                                            optReturn.Message));
                                    return;
                                }
                                notifyMessage.ListData.Add(optReturn.Data.ToString());  //ChanState
                                notifyMessage.ListData.Add("0"); //ChanObjID，0表示关联的通道没有改变
                                OnDebug(LogMode.Debug,
                                    string.Format("{0}\t{1}", ParseCommand(notifyMessage.Command), monObj.MonID));
                                SendMessage(notifyMessage);
                            }
                            else
                            {
                                //尚未关联通道或关联的通道变了，将新关联的通道状态和通道编码发送给客户端
                                if ((role == 1 && chanType == VoiceChanInfo.RESOURCE_VOICECHANNEL)
                                    || (role == 2 && chanType == ScreenChanInfo.RESOURCE_SCREENCHANNEL))
                                {
                                    if (strRealExt == monObj.ObjValue)
                                    {
                                        //真实分机登录到新通道上
                                        monObj.ChanObjID = chanObjID;
                                        notifyMessage = new NotifyMessage();
                                        notifyMessage.Command = (int)Service04Command.NotStateChanged;
                                        notifyMessage.ListData.Add(monObj.MonID);   //MonID
                                        optReturn = XMLHelper.SeriallizeObject(chanState);
                                        if (!optReturn.Result)
                                        {
                                            OnDebug(LogMode.Error,
                                                string.Format("DoNotifyMessage fail.\t{0}\t{1}", optReturn.Code,
                                                    optReturn.Message));
                                            return;
                                        }
                                        notifyMessage.ListData.Add(optReturn.Data.ToString());  //ChanState
                                        notifyMessage.ListData.Add(chanObjID.ToString());       //ChanObjID
                                        OnDebug(LogMode.Debug,
                                            string.Format("{0}\t{1}", ParseCommand(notifyMessage.Command), monObj.MonID));
                                        SendMessage(notifyMessage);
                                    }
                                }
                            }
                            break;

                        #endregion

                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DoNotifyMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Client Commands

        protected override void DoClientCommand(RequestMessage request)
        {
            base.DoClientCommand(request);

            switch (request.Command)
            {
                case (int)Service04Command.ReqSetMonType:
                    DoSetMonTypeMessage(request);
                    break;
                case (int)Service04Command.ReqAddMonObj:
                    DoAddMonObjMessage(request);
                    break;
                case (int)Service04Command.ReqRemoveMonObj:
                    DoRemoveMonObjMessage(request);
                    break;
                case (int)Service04Command.ReqQueryChan:
                    DoQueryChanMessage(request);
                    break;
                case (int)Service04Command.ReqAddQueryChan:
                    DoAddQueryChanMessage(request);
                    break;
                case (int)Service04Command.ReqQueryState:
                    DoQueryStateMessage(request);
                    break;
                case (int)Service04Command.ReqStartNMon:
                    DoStartNMonMessage(request);
                    break;
                case (int)Service04Command.ReqStopNMon:
                    DoStopNMonMessage(request);
                    break;
                case (int)Service04Command.ReqStartSMon:
                    DoStartSMonMessage(request);
                    break;
                case (int)Service04Command.ReqStopSMon:
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
                str = ((Service04Command)command).ToString();
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
                mNMonCore.HeadReceived += mNMonCore_HeadReceived;
                mNMonCore.DataReceived += mNMonCore_DataReceived;
                mNMonCore.IsPlayWave = false;
                mNMonCore.StartMon(param);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("InitNMon fail.\t{0}", ex.Message));
            }
        }

        void mNMonCore_HeadReceived(object s, SNM_RESPONSE head)
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
                notMessage.Command = (int)Service04Command.NotNMonHead;
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

        void mNMonCore_DataReceived(object s, byte[] data, int length)
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
                notMessage.Command = (int)Service04Command.NotNMonData;
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
