//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6860c113-46fe-4973-857d-af0b52950cb1
//        CLR Version:              4.0.30319.18063
//        Name:                     DECMessageHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                DECMessageHelper
//
//        created by Charley at 2015/10/25 17:15:28
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;

/*
 * ======================================================================
 * 
 * DECMessageHelper 工作逻辑
 * 
 * 1、DECMessageHelper主要包括DEC消息的定义以及DEC消息内容中节点和属性的定义
 * 2、DECMessageHelper提供了解析并获取xml消息中指定节点或属性的值的方法（参见Operations区块）
 * 3、在解析xml消息时如果出现错误将通过Debug事件将错误消息返回给MonitorServer（参见Debug区块）
 * 
 * ======================================================================
 */

namespace UMPService04
{
    public class DECMessageHelper
    {

        #region 消息类型

        public const string MSG_VOC_HEARTBEAT = "8002:0001:0001:0003";
        public const string MSG_VOC_RECORDSTARTED = "8002:0001:0002:0003";
        public const string MSG_VOC_RECORDSTOPPED = "8002:0001:0002:0004";
        public const string MSG_VOC_CALLINFO = "8002:0001:0002:0009";
        public const string MSG_VOC_AGENTLOGON = "8002:0001:0002:000A";
        public const string MSG_VOC_AGENTLOGOFF = "8002:0001:0002:000B";
        public const string MSG_VOC_CHANNELCONNECTED = "8002:0001:0002:000E";
        public const string MSG_VOC_CHANNELDISCONNECTED = "8002:0001:0002:000F";

        public const string MSG_SCR_HEARTBEAT = "8003:0001:0001:0003";
        public const string MSG_SCR_RECORDSTARTED = "8003:0001:0002:0001";
        public const string MSG_SCR_RECORDSTOPPED = "8003:0001:0002:0002";
        public const string MSG_SCR_AGENTLOGON = "8003:0001:0002:0004";
        public const string MSG_SCR_AGENTLOGOFF = "8003:0001:0002:0005";

        public const string MSG_RCV_RECOVERFINISH = "8009:0001:0001:0005";

        #endregion


        #region 节点

        public const string NODE_MESSAGE = "Message";
        public const string NODE_DEVICEINFOMATION = "DeviceInformation";
        public const string NODE_DEVICEINFOMATION2 = "DeviceInfomation";        //由于822版本的录音系统把单词Information拼写错了，写成了infomation，这里做一个兼容处理，是Service04仍然支持822的录音系统
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
        public const string ATTR_TASKID = "TaskID";
        public const string ATTR_RECORDCOUNT = "RecordCount";
        public const string ATTR_CHANNELCOUNT = "ChannelCount";

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

        public string GetTaskIDValue(string strData)
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
                if (node.Attributes != null && node.Attributes[ATTR_TASKID] != null)
                {
                    strReturn = node.Attributes[ATTR_TASKID].Value;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return strReturn;
        }

        public int GetRecordCountValue(string strData)
        {
            int intValue = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}",
                    NODE_MESSAGE));
                if (node == null)
                {
                    return intValue;
                }
                if (node.Attributes != null && node.Attributes[ATTR_RECORDCOUNT] != null)
                {
                    string str = node.Attributes[ATTR_RECORDCOUNT].Value;
                    intValue = int.Parse(str);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return intValue;
        }

        public int GetChannelCountValue(string strData)
        {
            int intValue = 0;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(strData);
                var node = doc.SelectSingleNode(string.Format("{0}",
                    NODE_MESSAGE));
                if (node == null)
                {
                    return intValue;
                }
                if (node.Attributes != null && node.Attributes[ATTR_CHANNELCOUNT] != null)
                {
                    string str = node.Attributes[ATTR_CHANNELCOUNT].Value;
                    intValue = int.Parse(str);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecodeMessageData fail.\t{0}", ex.Message));
            }
            return intValue;
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
