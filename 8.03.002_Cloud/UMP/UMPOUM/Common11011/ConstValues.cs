//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    23659948-d1ab-4903-8a58-c779be507866
//        CLR Version:              4.0.30319.18444
//        Name:                     ConstValues
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common11011
//        File Name:                ConstValues
//
//        created by Charley at 2014/9/1 9:40:03
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11011
{
    /// <summary>
    /// 常量定义
    /// </summary>
    public static class ConstValues
    {
        //操作编码
        public const long OPT_ADDORG = 1101001;
        public const long OPT_DELETEORG = 1101002;
        public const long OPT_MODIFYORG = 1101003;
        public const long OPT_ADDUSER = 1101004;
        public const long OPT_DELETEUSER = 1101005;
        public const long OPT_MODIFYUSER = 1101006;
        public const long OPT_SETUSERROLE = 1101007;
        public const long OPT_SETUSERMANAGEMENT = 1101008;

        //资源类型编码
        public const int OBJTYPE_ORG = 101;
        public const int OBJTYPE_USER = 102;

        //特定机构编号
        public const long ORG_ROOT = 1010000000000000001;
        //特定用户编号
        public const long USER_ADMIN = 1020000000000000001;

        //特定全局参数编号
        public const int PARAM_DEFAULT_PASSWORD = 11010501;

        //特定租户编号
        public const long RENT_DEFAULT = 1000000000000000001;
        public const string RENT_DEFAULT_TOKEN = "00000";
    }
}
