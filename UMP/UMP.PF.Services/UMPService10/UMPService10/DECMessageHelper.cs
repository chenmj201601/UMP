//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f96dd0c3-8b68-4756-bb6d-fe6b5c7d282d
//        CLR Version:              4.0.30319.18408
//        Name:                     DECMessageHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10
//        File Name:                DECMessageHelper
//
//        created by Charley at 2016/6/27 16:14:38
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;


namespace UMPService10
{
    public class DECMessageHelper
    {

        #region 消息类型

        //CTIHubServer发布的消息
        public const string MSG_CTI_CALL_EVENT_SERVICEINITIATED = "8005:0001:0004:0004";            //呼叫方摘机
        public const string MSG_CTI_CALL_EVENT_ORIGINATED = "8005:0001:0004:0006";                  //呼叫方拨号
        public const string MSG_CTI_CALL_EVENT_DELIVERED = "8005:0001:0004:0008";                   //振铃
        public const string MSG_CTI_CALL_EVENT_ESTABLISHED = "8005:0001:0004:000F";                 //通话
        public const string MSG_CTI_CALL_EVENT_CONNECTIONCLEARED = "8005:0001:0004:0018";           //挂机
        public const string MSG_CTI_CALL_EVENT_AGENTLOGON = "8005:0001:0004:001C";                  //坐席登录
        public const string MSG_CTI_CALL_EVENT_AGENTLOGOFF = "8005:0001:0004:001D";                 //坐席登出
        public const string MSG_CTI_CALL_EVENT_AGENTSTATECHANGED = "8005:0001:0004:002E";           //坐席状态更新

        public const string MSG_CTI_DEVICE_STATE_AGENTLOGON = "8005:0001:0002:0007";                //坐席登录
        public const string MSG_CTI_DEVICE_STATE_AGENTSTATECHANGED = "8005:0001:0002:0009";         //坐席状态更新
        public const string MSG_CTI_DEVICE_STATE_AGENTLOGOFF = "8005:0001:0002:0008";               //坐席登出

        public const string MSG_CTI_CALL_INFORMATION_CALLINFORMATION = "8005:0001:0003:0001";       //查询分机信息的响应消息，这是点对点消息，查询指定设备的呼叫信息

        //VoiceServer发布的消息
        public const string MSG_VOC_HEARTBEAT = "8002:0001:0001:0003";              //心跳
        public const string MSG_VOC_RECORDSTARTED = "8002:0001:0002:0003";          //开始录音
        public const string MSG_VOC_RECORDSTOPPED = "8002:0001:0002:0004";          //停止录音
        public const string MSG_VOC_CALLINFO = "8002:0001:0002:0009";               //呼叫信息
        public const string MSG_VOC_AGENTLOGON = "8002:0001:0002:000A";             //坐席登录录音
        public const string MSG_VOC_AGENTLOGOFF = "8002:0001:0002:000B";            //坐席登出录音
        public const string MSG_VOC_CHANNELCONNECTED = "8002:0001:0002:000E";       //通道连接
        public const string MSG_VOC_CHANNELDISCONNECTED = "8002:0001:0002:000F";    //通道断开连接

        //ScreenServer发布的消息
        public const string MSG_SCR_HEARTBEAT = "8003:0001:0001:0003";              //心跳
        public const string MSG_SCR_RECORDSTARTED = "8003:0001:0002:0001";          //开始录屏
        public const string MSG_SCR_RECORDSTOPPED = "8003:0001:0002:0002";          //停止录屏
        public const string MSG_SCR_AGENTLOGON = "8003:0001:0002:0004";             //坐席登录录屏
        public const string MSG_SCR_AGENTLOGOFF = "8003:0001:0002:0005";            //坐席登出录屏

        #endregion


        #region 节点

        public const string NODE_MESSAGE = "Message";
        public const string NODE_DEVICEINFOMATION = "DeviceInformation";
        public const string NODE_DEVICEINFOMATION2 = "DeviceInfomation";        //由于822版本的录音系统把单词Information拼写错了，写成了Infomation，这里做一个兼容处理，仍然支持822的录音系统
        public const string NODE_AGENTINFORMATION = "AgentInformation";
        public const string NODE_AGENTINFORMATION2 = "AgentInfomation";         //由于822版本的录音系统把单词Information拼写错了，写成了Infomation，这里做一个兼容处理，仍然支持822的录音系统
        public const string NODE_RECORDINFO = "recordinfo";
        public const string NODE_RECORDORIGINALDATA = "recordoriginaldata";
        public const string NODE_CALLERID = "callerid";
        public const string NODE_CALLEDID = "calledid";
        public const string NODE_DIRECTIONFLAG = "directionflag";
        public const string NODE_CALLINFOAGENTID = "agentid";
        public const string NODE_REALEXTENSION = "realextension";

        #endregion


        #region 属性

        public const string ATTR_MESSAGEID = "MessageID";
        public const string ATTR_CURRENTTIME = "CurrentTime";
        public const string ATTR_VOICEID = "VoiceID";
        public const string ATTR_SCRSVRID = "ScrSvrID";
        public const string ATTR_CHANNELID = "ChannelID";
        public const string ATTR_DEVICEID = "DeviceID";
        public const string ATTR_DEVICETYPE = "DeviceType";
        public const string ATTR_AGENTID = "AgentID";
        public const string ATTR_RECORDREFERENCE = "RecordReference";
        public const string ATTR_RECORDTIME = "RecordTime";
        public const string ATTR_RECORDLENGTH = "RecordLength";
        public const string ATTR_STATE = "State";

        public const string ATTR_VALUE = "value";

        #endregion


        #region Operations

        public string GetVoiceIDValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}",
                    NODE_MESSAGE));
                if (node == null)
                {
                    return strReturn;
                }
                if (node.Attributes != null && node.Attributes[ATTR_VOICEID] != null)
                {
                    strReturn = node.Attributes[ATTR_VOICEID].Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetScreenIDValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}",
                    NODE_MESSAGE));
                if (node == null)
                {
                    return strReturn;
                }
                if (node.Attributes != null && node.Attributes[ATTR_SCRSVRID] != null)
                {
                    strReturn = node.Attributes[ATTR_SCRSVRID].Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetChannelIDValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION));
                if (node == null)
                {
                    //由于822版本的录音系统把单词Information拼写错了，写成了infomation，这里做一个兼容处理，是Service04仍然支持822的录音系统
                    node = doc.SelectSingleNode(string.Format("{0}/{1}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION2));
                    if (node == null)
                    {
                        return strReturn;
                    }
                }
                if (node.Attributes != null && node.Attributes[ATTR_CHANNELID] != null)
                {
                    strReturn = node.Attributes[ATTR_CHANNELID].Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetDeviceIDValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION));
                if (node == null)
                {
                    node = doc.SelectSingleNode(string.Format("{0}/{1}",
                     NODE_MESSAGE,
                     NODE_DEVICEINFOMATION2));
                    if (node == null)
                    {
                        return strReturn;
                    }
                }
                if (node.Attributes != null && node.Attributes[ATTR_DEVICEID] != null)
                {
                    strReturn = node.Attributes[ATTR_DEVICEID].Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetAgentIDValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION));
                if (node == null)
                {
                    node = doc.SelectSingleNode(string.Format("{0}/{1}",
                     NODE_MESSAGE,
                     NODE_DEVICEINFOMATION2));
                    if (node == null)
                    {
                        return strReturn;
                    }
                }
                if (node.Attributes != null && node.Attributes[ATTR_AGENTID] != null)
                {
                    strReturn = node.Attributes[ATTR_AGENTID].Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetDirectionFlagValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}",
                    NODE_MESSAGE,
                    NODE_RECORDINFO,
                    NODE_RECORDORIGINALDATA,
                    NODE_DIRECTIONFLAG));
                if (node == null)
                {
                    return strReturn;
                }
                if (node.Attributes != null)
                {
                    var attr = node.Attributes[ATTR_VALUE];
                    if (attr == null)
                    {
                        return strReturn;
                    }
                    strReturn = DecodeMessageValue(attr.Value);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetCallerIDValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}",
                    NODE_MESSAGE,
                    NODE_RECORDINFO,
                    NODE_RECORDORIGINALDATA,
                    NODE_CALLERID));
                if (node == null)
                {
                    return strReturn;
                }
                if (node.Attributes != null)
                {
                    var attr = node.Attributes[ATTR_VALUE];
                    if (attr == null)
                    {
                        return strReturn;
                    }
                    strReturn = DecodeMessageValue(attr.Value);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetCalledIDValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}",
                    NODE_MESSAGE,
                    NODE_RECORDINFO,
                    NODE_RECORDORIGINALDATA,
                    NODE_CALLEDID));
                if (node == null)
                {
                    return strReturn;
                }
                if (node.Attributes != null)
                {
                    var attr = node.Attributes[ATTR_VALUE];
                    if (attr == null)
                    {
                        return strReturn;
                    }
                    strReturn = DecodeMessageValue(attr.Value);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetRecordReferenceValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION));
                if (node == null)
                {
                    node = doc.SelectSingleNode(string.Format("{0}/{1}",
                     NODE_MESSAGE,
                     NODE_DEVICEINFOMATION2));
                    if (node == null)
                    {
                        return strReturn;
                    }
                }
                if (node.Attributes != null && node.Attributes[ATTR_RECORDREFERENCE] != null)
                {
                    strReturn = node.Attributes[ATTR_RECORDREFERENCE].Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public DateTime GetRecordTimeValue(string strData)
        {
            DateTime dt = DateTime.MinValue;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION));
                if (node == null)
                {
                    node = doc.SelectSingleNode(string.Format("{0}/{1}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION2));
                    if (node == null)
                    {
                        return DateTime.MinValue;
                    }
                }
                if (node.Attributes != null && node.Attributes[ATTR_RECORDTIME] != null)
                {
                    string str = node.Attributes[ATTR_RECORDTIME].Value;
                    dt = new DateTime(long.Parse(str));
                    dt = dt.AddYears(1600);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return dt;
        }

        public int GetRecordLengthValue(string strData)
        {
            int intValue = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION));
                if (node == null)
                {
                    node = doc.SelectSingleNode(string.Format("{0}/{1}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION2));
                    if (node == null)
                    {
                        return intValue;
                    }
                }
                if (node.Attributes != null && node.Attributes[ATTR_RECORDLENGTH] != null)
                {
                    string str = node.Attributes[ATTR_RECORDLENGTH].Value;
                    intValue = int.Parse(str);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return intValue;
        }

        public string GetCallInfoAgentIDValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}",
                    NODE_MESSAGE,
                    NODE_RECORDINFO,
                    NODE_RECORDORIGINALDATA,
                    NODE_CALLINFOAGENTID));
                if (node == null)
                {
                    return strReturn;
                }
                if (node.Attributes != null)
                {
                    var attr = node.Attributes[ATTR_VALUE];
                    if (attr == null)
                    {
                        return strReturn;
                    }
                    strReturn = DecodeMessageValue(attr.Value);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetRealExtensionValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}",
                    NODE_MESSAGE,
                    NODE_RECORDINFO,
                    NODE_RECORDORIGINALDATA,
                    NODE_REALEXTENSION));
                if (node == null)
                {
                    return strReturn;
                }
                if (node.Attributes != null)
                {
                    var attr = node.Attributes[ATTR_VALUE];
                    if (attr == null)
                    {
                        return strReturn;
                    }
                    strReturn = DecodeMessageValue(attr.Value);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string DecodeMessageValue(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V01B64);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageValue fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        public string GetCTIDirectionFlagValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}/{4}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION,
                    NODE_RECORDINFO,
                    NODE_RECORDORIGINALDATA,
                    NODE_DIRECTIONFLAG));
                if (node == null)
                {
                    node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}/{4}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION2,
                    NODE_RECORDINFO,
                    NODE_RECORDORIGINALDATA,
                    NODE_DIRECTIONFLAG));
                    if (node == null)
                    {
                        return strReturn;
                    }
                }
                if (node.Attributes != null)
                {
                    var attr = node.Attributes[ATTR_VALUE];
                    if (attr == null)
                    {
                        return strReturn;
                    }
                    strReturn = attr.Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetCTICallerIDValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}/{4}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION,
                    NODE_RECORDINFO,
                    NODE_RECORDORIGINALDATA,
                    NODE_CALLERID));
                if (node == null)
                {
                    node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}/{4}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION2,
                    NODE_RECORDINFO,
                    NODE_RECORDORIGINALDATA,
                    NODE_CALLERID));
                    if (node == null)
                    {
                        return strReturn;
                    }
                }
                if (node.Attributes != null)
                {
                    var attr = node.Attributes[ATTR_VALUE];
                    if (attr == null)
                    {
                        return strReturn;
                    }
                    strReturn = attr.Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetCTICalledIDValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}/{4}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION,
                    NODE_RECORDINFO,
                    NODE_RECORDORIGINALDATA,
                    NODE_CALLEDID));
                if (node == null)
                {
                    node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}/{3}/{4}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION2,
                    NODE_RECORDINFO,
                    NODE_RECORDORIGINALDATA,
                    NODE_CALLEDID));
                    if (node == null)
                    {
                        return strReturn;
                    }
                }
                if (node.Attributes != null)
                {
                    var attr = node.Attributes[ATTR_VALUE];
                    if (attr == null)
                    {
                        return strReturn;
                    }
                    strReturn = attr.Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetCTIAgentIDValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION,
                    NODE_AGENTINFORMATION));
                if (node == null)
                {
                    node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}",
                     NODE_MESSAGE,
                     NODE_DEVICEINFOMATION2,
                     NODE_AGENTINFORMATION2));
                    if (node == null)
                    {
                        return strReturn;
                    }
                }
                if (node.Attributes != null && node.Attributes[ATTR_AGENTID] != null)
                {
                    strReturn = node.Attributes[ATTR_AGENTID].Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public string GetCTIAgentStateValue(string strData)
        {
            string strReturn = string.Empty;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}",
                    NODE_MESSAGE,
                    NODE_DEVICEINFOMATION,
                    NODE_AGENTINFORMATION));
                if (node == null)
                {
                    node = doc.SelectSingleNode(string.Format("{0}/{1}/{2}",
                     NODE_MESSAGE,
                     NODE_DEVICEINFOMATION2,
                     NODE_AGENTINFORMATION2));
                    if (node == null)
                    {
                        return strReturn;
                    }
                }
                if (node.Attributes != null && node.Attributes[ATTR_AGENTID] != null)
                {
                    strReturn = node.Attributes[ATTR_STATE].Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
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
            OnDebug(mode, "DecMsgHelper", msg);
        }

        #endregion

    }
}
