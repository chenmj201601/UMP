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
    public static class S1101Consts
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
        public const long OPT_SETUSERRESOURCEMANAGEMENT = 1101009;
        public const long OPT_IMPORTUSERDATA = 1101010;
        public const long OPT_IMPORTAGENTDATA = 1101011;
        public const long OPT_LDAP = 1101012;

        //资源类型编码
        public const int OBJTYPE_ORG = 101;
        public const int OBJTYPE_USER = 102;
        public const int OBJTYPE_ORGTYPE = 906;
        public const int OBJTYPE_AGENT = 103;
        public const int OBJTYPE_EXTENSION = 104;
        public const int OBJTYPE_REALEXTENSION = 105;

        //特定机构编号
        public const long ORG_ROOT = 1010000000000000001;
        //特定用户编号
        public const long USER_ADMIN = 1020000000000000001;
        //特定角色编号
        public const long ROLE_SYSTEMADMIN = 1060000000000000001;

        //特定全局参数编号
        public const int PARAM_DEFAULT_PASSWORD = 11010501;

        //特定租户编号
        public const long RENT_DEFAULT = 1000000000000000001;
        public const string RENT_DEFAULT_TOKEN = "00000";

        //默认使用结束期限
        public const string Default_StrEndTime = "2199/12/31 23:59:59";

        //资源编码
        public const long RESOURCE_VCLog = 221;
        public const long RESOURCE_AGENT = 103;
        public const long RESOURCE_EXTENSION = 104;
        //资源类型
        public const long VCLogServer = 110100001;
        public const long Agent = 110100002;
        public const long Exten = 110100003;
        //ABCD小项类型
        public const string CallPeak = "3110000000000000011";
        public const string CallDurationCompare = "3110000000000000012";
        public const string AfterDealDurationCompare = "3110000000000000013";
        public const string ACSpeExceptProportion = "3110000000000000014";
        public const string RecordDurationExcept = "3110000000000000015";
        public const string AfterDealDurationExcept = "3110000000000000016";
        public const string ExceptionSorce = "3110000000000000017";
        public const string CollisionDuration = "3110000000000000001";
        public const string CollisionPercent = "3110000000000000002";
        public const string HoldDuration = "3110000000000000003";
        public const string HoldPercent = "3110000000000000004";
        public const string HoldTimes = "3110000000000000005";
        public const string TransferTimes = "3110000000000000006";
        public const string IsAgentHanged = "3110000000000000007";
        public const string AfterDelDurationSec = "3110000000000000008";
        public const string AfDeDurMoreAvaDeDurSec = "3110000000000000009";
        public const string RepeatedCallinTimes = "3110000000000000010";
        public const string CallDurationCompare1 = "3110000000000000018";
        public const string AfterDealDurationCompare1 = "3110000000000000019";
    }
}
