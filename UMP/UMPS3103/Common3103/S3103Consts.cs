using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31031
{
    public static class S3103Consts
    {
        //任务操作编码
        public const long OPT_QMTASK = 3103;
        public const long OPT_TASKASSIGN = 3103001;
        public const long OPT_TASKTRACK = 3103002;
        public const long OPT_MODIFYTASKFINISHTIME = 3103003;
        public const long OPT_VIEWTASKDETAIL = 3103004;
        public const long OPT_TASKRECORDSCORE = 3103005;
        public const long OPT_TASKRECORDMODIFYSCORE= 3103006;
        public const long OPT_TASKASSIGNQUERY = 3103007;
        public const long OPT_SAVETOTASK = 3103008;
        public const long OPT_ADDTOTASK = 3103009;

        public const long OPT_PLAYRECORD = 3103010;
        public const long OPT_MEMORECORD = 3103011;
        public const long OPT_RESETLGYOUT = 3103012;
        public const long OPT_SAVELAYOUT = 3103013;
        public const long OPT_BOOKMARKRECORD = 3103014;
        public const long OPT_REMOVETASKRECORD = 3103015;
        public const long OPT_MOVETOOTHERTASK = 3103016;
        public const long OPT_MODIFYTASK = 3103017;
        public const long OPT_VIEWSCORE = 3103018;
        public const long OPT_TASKQUERY = 3103019;
        /// <summary>
        /// 复检任务
        /// </summary>
        public const long OPT_DoubleTask = 3103020;
        /// <summary>
        /// 复检任务分配
        /// </summary>
        public const long OPT_DoubleTaskAssign = 3103021;
        /// <summary>
        /// 复检任务评分
        /// </summary>
        public const long OPT_DoubleTaskScore = 3103022;
        /// <summary>
        /// 复检任务调整
        /// </summary>
        public const long OPT_DoubleTaskAdjust = 3103023;

        /// <summary>
        /// 质检查看初检任务的权限
        /// </summary>
        public const long OPT_CheckTask = 3103111;
        /// <summary>
        /// 质检查看复检任务的权限
        /// </summary>
        public const long OPT_RecheckTask = 3103112;


        //资源类型编码
        public const int OBJTYPE_ORG = 101;
        public const int OBJTYPE_USER = 102;
        public const int OBJTYPE_ROLE = 103;
        public const int OBJTYPE_PERMISSIONS = 104;
        public const int OBJTYPE_FIRSTTASK = 308;
        public const int OBJTYPE_RECHECKTASK = 310;

        //特定机构编号
        public const long ORG_ROOT = 1010000000000000001;
        //特定用户编号
        public const string USER_ADMIN = "102{0}00000000001";
        //特定角色编号
        public const string ROLE_SYSTEMADMIN = "106{0}00000000001";

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
        public const long CON_SCORE = 3031401010000000013;
        /// <summary>
        /// 服务态度
        /// </summary>
        public const long CON_ServiceAttitude = 3031401010000000014;
        /// <summary>
        /// 专业水平
        /// </summary>
        public const long CON_ProfessionalLevel = 3031401010000000015;
        /// <summary>
        /// 录音时长异常
        /// </summary>
        public const long CON_RecordDurationError = 3031401010000000016;
        /// <summary>
        /// 重复呼入
        /// </summary>
        public const long CON_RepeatCallIn = 3031401010000000017;
        /// <summary>
        /// 异常分数
        /// </summary>
        public const long CON_ExceptionScore = 3031401010000000018;
        /// <summary>
        /// 亊后处理时长异常
        /// </summary>
        public const long CON_AfterDealDurationExcept = 3031401010000000019;
        /// <summary>
        /// 坐席/客人讲话时长比例异常
        /// </summary>
        public const long CON_ACSpeExceptProportion = 3031401010000000020;
        /// <summary>
        /// 关键词
        /// </summary>
        public const long CON_KEYWORD = 3031401010000000021;

       //任务分配页面大小
        public const int USER_PARAM_PAGESIZE = 200;
       //任务录音查询最大录音数
        public const int USER_PARAM_MAXRECORDS = 100000;

        // 查询的表不存在
        public const string Err_TableNotExit = "ERR_TABLE_NOT_EXIT";


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


        public const string TableName_KeyWord = "T_51_009";
    }
}
