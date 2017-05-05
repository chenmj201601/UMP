//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c3fea196-c588-4cfd-8495-6d16b83a087d
//        CLR Version:              4.0.30319.18408
//        Name:                     S4410Consts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common44101
//        File Name:                S4410Consts
//
//        created by Charley at 2016/5/11 10:35:17
//        http://www.voicecyber.com 
//
//======================================================================


namespace VoiceCyber.UMP.Common44101
{
    public class S4410Consts
    {

        public const int MODULE_MASTER_ASM = 44;

        public const int RESOURCE_REGION = 411;
        public const int RESOURCE_SEAT = 412;
        public const int RESOURCE_AGENTSTATE = 413;
        public const int RESOURCE_ALARMMSG = 414;

        public const long REGION_ROOT = 4110000000000000001;        //默认根区域的编码


        #region Operation

        public const long OPT_ADDREGION = 4412001;
        public const long OPT_DELETEREGION = 4412002;
        public const long OPT_MODIFYREGION = 4412003;
        public const long OPT_REGIONSEATSETTING = 4412004;
        public const long OPT_REGIONMANAGESETTING = 4412005;

        public const long OPT_ADDSEAT = 4413001;
        public const long OPT_DELETESEAT = 4413002;
        public const long OPT_MODIFYSEAT = 4413003;
        public const long OPT_SYNCSEAT = 4413004;
        public const long OPT_IMPORTSEAT = 4413005;

        public const long OPT_ADDSTATE = 4414001;
        public const long OPT_DELETESTATE = 4414002;
        public const long OPT_MODIFYSTATE = 4414003;

        public const long OPT_ADDALARM = 4415001;
        public const long OPT_DELETEALARM = 4415002;
        public const long OPT_MODIFYALARM = 4415003;
        public const long OPT_SETUSER = 4415004;

        #endregion


        #region StateType 坐席状态类型

        public const int STATE_TYPE_UNKOWN = 0;
        public const int STATE_TYPE_LOGIN = 1;
        public const int STATE_TYPE_CALL = 2;
        public const int STATE_TYPE_RECORD = 3;
        public const int STATE_TYPE_DIRECTION = 4;
        public const int STATE_TYPE_AGNET = 5;

        #endregion


        #region 坐席状态编码

        public const int STATE_NUMBER_BLANK = 9;
        public const int STATE_NUMBER_LOGON = 1;
        public const int STATE_NUMBER_LOGOFF = 2;
        public const int STATE_NUMBER_READY = 10;
        public const int STATE_NUMBER_NOTREADY = 11;
        public const int STATE_NUMBER_AFTERCALLWORK = 12;
        public const int STATE_NUMBER_INBOUND = 7;
        public const int STATE_NUMBER_OUTBOUND = 8;

        #endregion


        #region Color

        public const string COLOR_DEFAULT_LOGON = "#FF0000FF";
        public const string COLOR_DEFAULT_LOGOFF = "#FF708090";

        #endregion


        #region Alarm Content KeyWord

        public const string ALARM_CONTENT_KEYWORD_AGENTID = "[AGT]";
        public const string ALARM_CONTENT_KEYWORD_EXTENSION = "[EXT]";
        public const string ALARM_CONTENT_KEYWORD_TIMEOUTLENGTH = "[TOL]";

        #endregion

    }
}
