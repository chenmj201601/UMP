//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b0e6b446-51c5-4b63-b639-ee5d2e01395c
//        CLR Version:              4.0.30319.18063
//        Name:                     S2501Consts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common25011
//        File Name:                S2501Consts
//
//        created by Charley at 2015/5/20 13:50:54
//        http://www.voicecyber.com
//
//======================================================================

namespace VoiceCyber.UMP.Common25011
{
    public class S2501Consts
    {

        #region 操作编码

        public const long OPT_RELOAD = 2501000;
        public const long OPT_ADDALARMINFO = 2501001;
        public const long OPT_REMOVEALARMINFO = 2501002;
        public const long OPT_MODIFYALARMINFO = 2501003;
        public const long OPT_SAVEALARMINFO = 2501004;
        public const long OPT_ALARMRECEIVERRELOAD = 2501010;
        public const long OPT_SAVEALARMRECEIVER = 2501011;
        public const long OPT_MODIFYSENDMETHOD = 2501012;
        public const long OPT_ALARMMESSAGERELOAD = 2501020;


        #endregion


        #region 节点类型

        public const int NODE_MODULE = 0;
        public const int NODE_MESSAGE = 1;
        public const int NODE_STATUS = 2;

        public const int NODE_ORG = 10;
        public const int NODE_USER = 11;

        #endregion


        #region 告警类型

        public const int ALARM_TYPE_PROMPT = 1;
        public const int ALARM_TYPE_NOTIFY = 2;
        public const int ALARM_TYPE_ALARM = 3;

        #endregion


        #region 告警等级

        public const int ALARM_LEVEL_SOURCE_LEVEL = -1;
        public const int ALARM_LEVEL_NORMAL = 0;
        public const int ALARM_LEVEL_LOW = 1;
        public const int ALARM_LEVEL_MID = 2;
        public const int ALARM_LEVEL_HIGH = 3;

        #endregion


        #region BasicInfoData ID

        public const long BID_ALARM_TYPE = 250100001;
        public const long BID_ALARM_LEVEL = 250100002;
        public const long BID_ALARM_TERMINAL = 250100003;

        #endregion

    }
}
