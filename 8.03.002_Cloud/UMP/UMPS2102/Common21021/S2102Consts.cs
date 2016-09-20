//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    abd13fb4-09df-448a-9e80-b66fc0338189
//        CLR Version:              4.0.30319.18063
//        Name:                     S2102Consts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common21021
//        File Name:                S2102Consts
//
//        created by Charley at 2015/6/19 15:57:53
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common21021
{
    public class S2102Consts
    {
        //操作编码
        public const long OPT_ADDMONOBJ = 2102001;
        public const long OPT_REMOVEMONOBJ = 2102002;
        public const long OPT_CLEARMONOBJ = 2102003;
        public const long OPT_SAVEMONLIST = 2102004;
        public const long OPT_OPTION = 2102005;
        public const long OPT_STATEMON = 2102100;
        public const long OPT_NETMON = 2102101;
        public const long OPT_SCRMON = 2102102;
        public const long OPT_CHANSTATEMON = 2102100001;
        public const long OPT_EXTSTATEMON = 2102100002;
        public const long OPT_AGTSTATEMON = 2102100003;
        public const long OPT_CHANNETMON = 2102101001;
        public const long OPT_EXTNETMON = 2102101002;
        public const long OPT_AGTNETMON = 2102101003;

        //其他操作，一般不受权限控制
        public const long OPT_ADDTOMONLIST = 2102200;
        public const long OPT_REFRESHMONLIST = 2102201;
        public const long OPT_STOPNETMON = 2102202;
        public const long OPT_CLOSENETMON = 2102203;
        public const long OPT_STOPSCRMON = 2102204;
        public const long OPT_CLOSESCRMON = 2102205;

        //基本信息编码
        public const int BID_VIEWTYPE = 210200001;
        public const int BID_LOGSTATE = 210200002;
        public const int BID_CALLSTATE = 210200003;
        public const int BID_RECORDSTATE = 210200004;
        public const int BID_MONTYPE = 210200005;

        //用户参数编码
        public const int UP_COLOR_VOCLOGINSTATE = 21020101;
        public const int UP_COLOR_VOCRECORDSTATE = 21020102;
        public const int UP_COLOR_CALLINSTATE = 21020103;
        public const int UP_COLOR_CALLOUTSTATE = 21020104;
        public const int UP_COLOR_SCRLOGINSTATE = 21020105;
        public const int UP_COLOR_SCRRECORDSTATE = 21020106;
        public const int UP_COLOR_VOCSCRLOGINSTATE = 21020107;
        public const int UP_COLOR_VOCSCRRECORDSTATE = 21020108;

        public const int UP_PLAYSCREEN_TOPMOST = 21020201;
        public const int UP_PLAYSCREEN_SCALE = 21020202;

        //用户参数组编号
        public const int UP_GROUP_COLOR = 210201;
        public const int UP_GROUP_SCREEN = 210202;
    }
}
