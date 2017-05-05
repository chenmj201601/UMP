using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common3105
{
    public class S3105Consts
    {
        //审批复核处理
        public const long OPT_ProcessAppeal = 3105;
        /// <summary>
        /// 審批
        /// </summary>
        public const long OPT_Approval = 3105001;
        /// <summary>
        /// 複核
        /// </summary>
        public const long OPT_Review = 3105002;
        public const long OPT_Play = 3105003;
        public const long OPT_Memo = 3105004;
        public const long OPT_Query = 3105005;
        /// <summary>
        /// 已複核
        /// </summary>
        public const long OPT_Reviewed = 3105006;
        /// <summary>
        /// 已審批
        /// </summary>
        public const long OPT_Approved = 3105007;
        public const long OPT_ViewScore = 3105008;
        //申诉完成 不显示申诉详情
        public const string AppealOvered = "Appeal_Overed";

        ////资源类型编码
        //public const int OBJTYPE_ORG = 101;
        //public const int OBJTYPE_USER = 102;
        //public const int OBJTYPE_ROLE = 103;
        //public const int OBJTYPE_PERMISSIONS = 104;

        //特定机构编号
        public const long ORG_ROOT = 1010000000000000001;
        public const string ORG_ROOTName = "上海宇高通讯设备有限公司";

        //超级管理员
        public const string USER_ADMIN = "102{0}00000000001";

        //特定角色编号(座席和超级管理员)
        public const string ROLE_SYSTEMAGENT = "106{0}00000000004";
        public const string ROLE_SYSTEMADMIN = "106{0}00000000001";

        //特定全局参数编号
        public const int PARAM_DEFAULT_PASSWORD = 11010501;

        //特定租户编号
        public const long RENT_DEFAULT = 1000000000000000001;
        public const string RENT_DEFAULT_TOKEN = "00000";

        //录音页面大小
        public const int USER_PARAM_PAGESIZE = 200;
        //录音查询最大录音数
        public const int USER_PARAM_MAXRECORDS = 100000;

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

        public const long CON_ALL = 3031401010000000010;
        public const long CON_TO_REVIEW=3031401010000000011;
        public const long CON_TO_APPROVAL = 3031401010000000012;
        public const long CON_APPROVALED = 3031401010000000013;



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
