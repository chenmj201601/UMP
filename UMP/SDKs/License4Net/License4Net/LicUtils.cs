//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dc99a3d5-d0b6-466d-a4e0-8cb7e1c5313f
//        CLR Version:              4.0.30319.18063
//        Name:                     LicUtils
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.Licenses
//        File Name:                LicUtils
//
//        created by Charley at 2015/9/13 14:19:42
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.Common;

namespace VoiceCyber.SDKs.Licenses
{
    /// <summary>
    /// 辅助方法
    /// </summary>
    public class LicUtils
    {
        /// <summary>
        /// 根据Session计算出验证码
        /// </summary>
        /// <param name="byteSession"></param>
        /// <returns></returns>
        public static byte[] GetVerificationCode(byte[] byteSession)
        {
            int offset = byteSession[0] & 0x001f;
            byte byteMask = (byte)(byteSession[offset] ^ offset);
            byte[] byteVerify = new byte[LicDefines.LENGTH_VERIFICATION];
            for (int i = 0; i < LicDefines.LENGTH_VERIFICATION; i++)
            {
                byteVerify[i] = (byte)(byteSession[i] ^ byteMask);
            }
            return byteVerify;
        }
        /// <summary>
        /// 根据Session计算出验证码
        /// </summary>
        /// <param name="strSession"></param>
        /// <returns></returns>
        public static string GetVerificationCode(string strSession)
        {
            byte[] byteSession = Converter.Hex2Byte(strSession);
            byte[] byteVerify = GetVerificationCode(byteSession);
            return Converter.Byte2Hex(byteVerify);
        }
        /// <summary>
        /// 获取类别描述信息
        /// </summary>
        /// <param name="classid"></param>
        /// <returns></returns>
        public static string GetClassDesc(int classid)
        {
            string strReturn = string.Format("[{0}]", classid);
            switch (classid)
            {
                case LicDefines.LICENSE_MSG_CLASS_UNKNOWN:
                    strReturn = "unkown";
                    break;
                case LicDefines.LICENSE_MSG_CLASS_EXCEPTION:
                    strReturn = "exception";
                    break;
                case LicDefines.LICENSE_MSG_CLASS_CONNECTION:
                    strReturn = "connection";
                    break;
                case LicDefines.LICENSE_MSG_CLASS_AUTHENTICATE:
                    strReturn = "authenticate";
                    break;
                case LicDefines.LICENSE_MSG_CLASS_NOTIFY:
                    strReturn = "notify";
                    break;
                case LicDefines.LICENSE_MSG_CLASS_REQRES:
                    strReturn = "request and response";
                    break;
            }
            return strReturn;
        }
        /// <summary>
        /// 获取消息描述信息
        /// </summary>
        /// <param name="classid"></param>
        /// <param name="messageid"></param>
        /// <returns></returns>
        public static string GetMessageDesc(int classid, int messageid)
        {
            string strReturn = string.Format("[{0}-{1}]", classid, messageid);
            switch (classid)
            {
                case LicDefines.LICENSE_MSG_CLASS_UNKNOWN:
                    switch (messageid)
                    {
                        case LicDefines.LICENSE_MSG_UNKNOWN:
                            strReturn = "unkown";
                            break;
                    }
                    break;
                case LicDefines.LICENSE_MSG_CLASS_EXCEPTION:
                    switch (messageid)
                    {
                        case LicDefines.LICENSE_MSG_EXCEPTION_UNKNOWN:
                            strReturn = "unkown";
                            break;
                        case LicDefines.LICENSE_MSG_EXCEPTION_UNKNOWN_CLASS:
                            strReturn = "unkown class";
                            break;
                    }
                    break;
                case LicDefines.LICENSE_MSG_CLASS_CONNECTION:
                    switch (messageid)
                    {
                        case LicDefines.LICENSE_MSG_CONNECTION_UNKNOWN:
                            strReturn = "unkown";
                            break;
                        case LicDefines.LICENSE_MSG_CONNECTION_HEARTBEAT:
                            strReturn = "heartbeat";
                            break;
                        case LicDefines.LICENSE_MSG_CONNECTION_FAILED:
                            strReturn = "connection failed";
                            break;
                        case LicDefines.LICENSE_MSG_CONNECTION_LOST:
                            strReturn = "connection lost";
                            break;
                        case LicDefines.LICENSE_MSG_CONNECTION_TIMEOUT:
                            strReturn = "timeout";
                            break;
                        case LicDefines.LICENSE_MSG_CONNECTION_SHUTDOWN:
                            strReturn = "server shutdown";
                            break;
                        case LicDefines.LICENSE_MSG_CONNECTION_MASERT_RECOVER:
                            strReturn = "master recover";
                            break;
                        case LicDefines.LICENSE_MSG_CONNECTION_DATA_FORMAT:
                            strReturn = "data format invalid";
                            break;
                    }
                    break;
                case LicDefines.LICENSE_MSG_CLASS_AUTHENTICATE:
                    switch (messageid)
                    {
                        case LicDefines.LICENSE_MSG_AUTH_UNKNOWN:
                            strReturn = "unkown";
                            break;
                        case LicDefines.LICENSE_MSG_AUTH_WELCOME:
                            strReturn = "welcode";
                            break;
                        case LicDefines.LICENSE_MSG_AUTH_LOGON:
                            strReturn = "logon";
                            break;
                        case LicDefines.LICENSE_MSG_AUTH_SUCCESS:
                            strReturn = "success";
                            break;
                        case LicDefines.LICENSE_MSG_AUTH_FAILED:
                            strReturn = "failed";
                            break;
                        case LicDefines.LICENSE_MSG_AUTH_DENY_CLIENT_TYPE:
                            strReturn = "deny client type";
                            break;
                        case LicDefines.LICENSE_MSG_AUTH_PROTOCOL_CLIENT:
                            strReturn = "client protocol invalid";
                            break;
                        case LicDefines.LICENSE_MSG_AUTH_PROTOCOL_SERVER:
                            strReturn = "server protocol invalid";
                            break;
                        case LicDefines.LICENSE_MSG_AUTH_WRONGFUL_SERVER:
                            strReturn = "server invalid";
                            break;
                        case LicDefines.LICENSE_MSG_AUTH_TIME_NOT_SYNC:
                            strReturn = "time not synchronized";
                            break;
                    }
                    break;
                case LicDefines.LICENSE_MSG_CLASS_NOTIFY:
                    switch (messageid)
                    {
                        case LicDefines.LICENSE_MSG_NOTIFY_UNKNOWN:
                            strReturn = "unkown";
                            break;
                        case LicDefines.LICENSE_MSG_NOTIFY_NEW_CLIENT:
                            strReturn = "new client connected";
                            break;
                        case LicDefines.LICENSE_MSG_NOTIFY_DEL_CLIENT:
                            strReturn = "delete client";
                            break;
                        case LicDefines.LICENSE_MSG_NOTIFY_CHANGED_LICENSE:
                            strReturn = "change license";
                            break;
                        case LicDefines.LICENSE_MSG_NOTIFY_LICESNE_POOL:
                            strReturn = "license pool";
                            break;
                        case LicDefines.LICENSE_MSG_NOTIFY_APPLICATION_LICENSE:
                            strReturn = "application license";
                            break;
                        case LicDefines.LICENSE_MSG_NOTIFY_SOFTDOGS_INFO:
                            strReturn = "softdogs information";
                            break;
                        case LicDefines.LICENSE_MSG_NOTIFY_LICENSE_SERVERS:
                            strReturn = "license servers";
                            break;
                        case LicDefines.LICENSE_MSG_NOTIFY_LOCAL_SERVER:
                            strReturn = "local server";
                            break;
                        case LicDefines.LICENSE_MSG_NOTIFY_HOLD_SOFTDOG:
                            strReturn = "hold softdog";
                            break;
                    }
                    break;
                case LicDefines.LICENSE_MSG_CLASS_REQRES:
                    switch (messageid)
                    {
                        case LicDefines.LICENSE_MSG_REQRES_UNKNOWN:
                            strReturn = "unkown";
                            break;
                        case LicDefines.LICENSE_MSG_REQUEST_QUERY_TOTAL_LICENSE:
                            strReturn = "query total license";
                            break;
                        case LicDefines.LICENSE_MSG_RESPONSE_QUERY_TOTAL_LICENSE:
                            strReturn = "query total license";
                            break;
                        case LicDefines.LICENSE_MSG_REQUEST_QUERY_FREE_LICENSE:
                            strReturn = "query free license";
                            break;
                        case LicDefines.LICENSE_MSG_RESPONSE_QUERY_FREE_LICENSE:
                            strReturn = "query free license";
                            break;
                        case LicDefines.LICENSE_MSG_REQUEST_GET_LICENSE:
                            strReturn = "get license";
                            break;
                        case LicDefines.LICENSE_MSG_RESPONSE_GET_LICENSE:
                            strReturn = "get license";
                            break;
                        case LicDefines.LICENSE_MSG_REQUEST_RELEASE_LICENSE:
                            strReturn = "release license";
                            break;
                        case LicDefines.LICENSE_MSG_RESPONSE_RELEASE_LICENSE:
                            strReturn = "release license";
                            break;
                        case LicDefines.LICENSE_MSG_REQUEST_RELEASE_ALL_LICENSE:
                            strReturn = "release all license";
                            break;
                        case LicDefines.LICENSE_MSG_RESPONSE_RELEASE_ALL_LICENSE:
                            strReturn = "release all license";
                            break;
                        case LicDefines.LICENSE_MSG_REQUEST_QUERY_SELF_LICENSE:
                            strReturn = "query self license";
                            break;
                        case LicDefines.LICENSE_MSG_RESPONSE_QUERY_SELF_LICENSE:
                            strReturn = "query self license";
                            break;
                        case LicDefines.LICENSE_MSG_REQUEST_LIST_SOFTDOG:
                            strReturn = "list softdog";
                            break;
                        case LicDefines.LICENSE_MSG_RESPONSE_LIST_SOFTDOG:
                            strReturn = "list softdog";
                            break;
                        case LicDefines.LICENSE_MSG_REQUEST_SELECT_SOFTDOG:
                            strReturn = "select softdog";
                            break;
                        case LicDefines.LICENSE_MSG_RESPONSE_SELECT_SOFTDOG:
                            strReturn = "select softdog";
                            break;
                        case LicDefines.LICENSE_MSG_REQUEST_HOLD_SOFTDOG:
                            strReturn = "hold softdog";
                            break;
                        case LicDefines.LICENSE_MSG_RESPONSE_HOLD_SOFTDOG:
                            strReturn = "hold softdog";
                            break;
                        case LicDefines.LICENSE_MSG_REQUEST_UNHOLD_SOFTDOG:
                            strReturn = "unhold softdog";
                            break;
                        case LicDefines.LICENSE_MSG_RESPONSE_UNHOLD_SOFTDOG:
                            strReturn = "unhold softdog";
                            break;
                        case LicDefines.LICENSE_MSG_REQUEST_QUERY_SPECIFIC_LICENSE:
                            strReturn = "query specific license";
                            break;
                        case  LicDefines.LICENSE_MSG_RESPONSE_QUERY_SPECIFIC_LICENSE:
                            strReturn = "query specific license";
                            break;
                    }
                    break;
            }
            return strReturn;
        }
    }
}
