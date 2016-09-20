
namespace VoiceCyber.UMP.Common31041
{
    public static class S3104Consts
    {
        //座席端操作编码
        public const long OPT_QueryRecord = 3104001;
        public const long OPT_AgentAppeal = 3104002;
        public const long OPT_ViewScoreResult = 3104003;
        public const long OPT_PlayRecord = 3104004;
        public const long OPT_ResetLayout = 3104005;
        public const long OPT_SaveLayout = 3104006;
        public const long OPT_RecordPlayHistory = 3104007;
        /// <summary>
        /// 跳转浏览器
        /// </summary>
        public const long OPT_Goto = 3104008;
        /// <summary>
        /// 教材库
        /// </summary>
        public const long Opt_Book = 3104009;

        public const long OPT_ModifyPassWord = 3104009;

        //资源类型编码
        public const int OBJTYPE_ORG = 101;
        public const int OBJTYPE_USER = 102;
        public const int OBJTYPE_ROLE = 103;
        public const int OBJTYPE_PERMISSIONS = 104;
        public const int OBJTYPE_FIRSTTASK = 308;
        public const int OBJTYPE_RECHECKTASK = 309;

        //特定机构编号
        public const long ORG_ROOT = 1010000000000000001;
        //特定用户编号
        public const long USER_ADMIN = 1020000000000000001;
        //特定角色编号
        public const long ROLE_SYSTEMADMIN = 1060000000000000001;

        /// <summary>
        /// 申诉流程ID
        /// </summary>
        public const long CON_AppealFlowID = 3100000000000000001;
        /// <summary>
        /// 申诉流程子项ID
        /// </summary>
        public const long CON_AppealFlowItemID = 3110000000000000001;

        //预定给座席角色编号
        public const long ROLE_AGENT = 1060000000000000004;


        //特定全局参数编号
        public const int PARAM_DEFAULT_PASSWORD = 11010501;

        //特定租户编号
        public const long RENT_DEFAULT = 1000000000000000001;
        public const string RENT_DEFAULT_TOKEN = "00000";

        //查询条件项编码
        public const long CON_TIMEFROMTO = 3031401010000000001;
        public const long CON_DURATIONFROMTO = 3031401010000000002;
        public const long CON_EXTENSION_MULTITEXT = 3031401010000000003;
        public const long CON_AGENT_MULTITEXT = 3031401010000000004;
        public const long CON_CALLERID_LIKETEXT = 3031401010000000005;
        public const long CON_CALLEDID_LIKETEXT = 3031401010000000006;
        public const long CON_VOICEID_MULTITEXT = 3031401010000000007;
        public const long CON_CHANNELID_MULTITEXT = 3031401010000000008;
        public const long CON_RECORDREFERENCE_MULTITEXT = 3031401010000000009;
        public const long CON_CTIREFERENCE_MULTITEXT = 3031401010000000010;
        public const long CON_DIRECTION = 3031401010000000011;
        //public const long CON_SCREEN = 3031401010000000012;

        public const long CON_HasAppeal=3031401010000000013;
        public const long CON_HasScore = 3031401010000000014;

        public const long CON_ScoreRange = 3031401010000000015;

        public const long CON_BookMark = 3031401010000000016;



        //录音页面大小
        public const int USER_PARAM_PAGESIZE = 200;
        //录音查询最大录音数
        public const int USER_PARAM_MAXRECORDS = 100000;

        //登录用户类别
        public const int UserType_Agent = 1;
        public const int UserType_Extension = 2;
        public const int UserType_Other = 3;//UMP服务端用户也能使用
        // 查询的表不存在
        public const string Err_TableNotExit = "ERR_TABLE_NOT_EXIT";
        //申诉完成 不显示申诉详情
        public const string AppealOvered = "Appeal_Overed";
        
        //ABCD ID
        /// <summary>
        /// 服务态度
        /// </summary>
        public const long WDE_ServiceAttitude = 3110000000000000001;
        /// <summary>
        /// 专业水平
        /// </summary>
        public const long WDE_ProfessionalLevel = 3110000000000000002;
        /// <summary>
        /// 重复呼入
        /// </summary>
        public const long WDE_RepeatCallIn = 3110000000000000003;
        /// <summary>
        /// 录音时长异常
        /// </summary>
        public const long WDE_RecordDurationError = 3110000000000000006;


        /*====================================================================
         * 
         * ****用户记住的解密密码信息***
         * 1、组编号：310210
         * 2、其后3位序号表示各个服务器
         * 3、范围是310210001 ~ 310210899
         * 4、从310210900到310210999保留特别使用
         * 5、310210900：所有服务器使用相同的密码
         * 参数值的规定：
         * M002(ServerAddress+char27+Password+char27+ExpireTime)
         * 注意：
         * 1、ServerAddress：服务器地址，可空（空表示应用于所有服务器）
         * 2、Password：密码，最大长度64个字符
         * 3、ExpireTime：密码有效截止时间（UTC，格式：yyyyMMddHHmmss）
         * 
         * ===================================================================
         */
        public const int USER_PARAM_GROUP_ENCRYPTINFO = 310210;

        public const int USER_PARAM_SKIPCONDITIONPANEL = 31020103;
        public const int USER_PARAM_QUERYVOICERECORD = 31020104;
        public const int USER_PARAM_QUERYSCREENRECORD = 31020105;
        public const int USER_PARAM_AUTORELATIVEPLAY = 31020106;
        public const int USER_PARAM_SKIPPASSWORDPANEL = 31020107;

        public const int USER_PARAM_EXPORTDATA_TYPE = 31020201;
        public const int USER_PARAM_EXPORTDATA_REMEMBER = 31020202;
        public const int USER_PARAM_EXPORTDATA_NOTSHOW = 31020203;

        public const int USER_PARAM_EXPORTRECORD_REMEMBER = 31020301;
        public const int USER_PARAM_EXPORTRECORD_NOTSHOW = 31020302;
        public const int USER_PARAM_EXPORTRECORD_SAVEDIR = 31020303;
        public const int USER_PARAM_EXPORTRECORD_PATHFORMAT = 31020304;
        public const int USER_PARAM_EXPORTRECORD_CONVERTPCM = 31020305;
        public const int USER_PARAM_EXPORTRECORD_DECRYPTFILE = 31020306;
        public const int USER_PARAM_EXPORTRECORD_GENERATEDB = 31020307;
        public const int USER_PARAM_EXPORTRECORD_REPLACEFILE = 31020308;
        public const int USER_PARAM_EXPORTRECORD_EXPORTVOICE = 31020309;
        public const int USER_PARAM_EXPORTRECORD_EXPORTSCREEN = 31020310;
        public const int USER_PARAM_EXPORTRECORD_IGNOREPATHFORMAT = 31020311;

        public const int USER_PARAM_PLAYSCREEN_NOPLAY = 31020401;
        public const int USER_PARAM_PLAYSCREEN_TOPMOST = 31020402;
        public const int USER_PARAM_PLAYSCREEN_SCALE = 31020403;
    }
}
