//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0a681396-5297-40da-8d22-6b930756b815
//        CLR Version:              4.0.30319.18444
//        Name:                     S3102Consts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                S3102Consts
//
//        created by Charley at 2014/11/2 15:20:51
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.InteropServices;

namespace VoiceCyber.UMP.Common31021
{
    public class S3102Consts
    {
        //特定机构编号
        public const long ORG_ROOT = 1010000000000000001;
        //特定用户编号
        public const long USER_ADMIN = 1020000000000000001;
        //特定角色编号
        public const long ROLE_SYSTEMADMIN = 1060000000000000001;
        //特定租户编号
        public const long RENT_DEFAULT = 1000000000000000001;
        public const string RENT_DEFAULT_TOKEN = "00000";

        //面板名称
        public const string PANEL_NAME_RECORDLIST = "RecordList";
        public const string PANEL_NAME_PLAYLIST = "PlayList";
        public const string PANEL_NAME_OBJECTLIST = "ObjectList";
        public const string PANEL_NAME_PLAYBOX = "PlayBox";
        public const string PANEL_NAME_CALLINO = "CallInfo";
        public const string PANEL_NAME_MEMO = "Memo";
        public const string PANEL_NAME_BOOKMARK = "Bookmark";
        public const string PANEL_NAME_SCORE = "Score";
        public const string PANEL_NAME_SCOREDETAIL = "ScoreDetail";
        public const string PANEL_NAME_CONVERSATIONINFO = "PanelConversationInfo";

        public const string PANEL_CONTENTID_RECORDLIST = "PanelRecordList";
        public const string PANEL_CONTENTID_PLAYLIST = "PanelPlayList";
        public const string PANEL_CONTENTID_OBJECTLIST = "PanelObjectBox";
        public const string PANEL_CONTENTID_PLAYBOX = "PanelPlayBox";
        public const string PANEL_CONTENTID_CALLINO = "PanelCallInfo";
        public const string PANEL_CONTENTID_MEMO = "PanelMemo";
        public const string PANEL_CONTENTID_BOOKMARK = "PanelBookmark";
        public const string PANEL_CONTENTID_SCORE = "PanelScore";
        public const string PANEL_CONTENTID_SCOREDETAIL = "PanelScoreDetail";
        public const string PANEL_CONTENTID_CONVERSATIONINFO = "PanelConversationInfo";

        //操作编码
        public const long OPT_QUERYRECORD = 3102001;
        public const long OPT_PLAYRECORD = 3102002;
        public const long OPT_MEMORECORD = 3102003;
        public const long OPT_BOOKMARKRECORD = 3102004;
        public const long OPT_SCORERECORD = 3102005;
        public const long OPT_RECOMMANDRECORD = 3102006;
        public const long OPT_EXPORTDATA = 3102007;
        public const long OPT_EXPORTRECORD = 3102008;
        public const long OPT_SAVELAYOUT = 3102009;
        public const long OPT_RESETLGYOUT = 3102010;
        public const long OPT_CUSTOMSETTING = 3102011;
        public const long OPT_SCOREDETAIL = 3102012;
        public const long OPT_RESTORELAYOUT = 3102013;

        public const long OPT_RECORDSCOREDETAIL = 3102014;
        public const long OPT_ADDLIBRARY = 3102015;

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
        public const long CON_SERIALID_MULTITEXT = 3031401010000000011;
        public const long CON_DIRECTION_CHECKSELECT = 3031401010000000012;
        public const long CON_TIMETYPEFROMTO = 3031401010000000013;
        public const long CON_MEMOAUTO_LIKETEXT = 3031401010000000014;
        public const long CON_BOOKMARKTITLE_LIKETEXT = 3031401010000000015;
        public const long CON_INSPECTOR = 3031401010000000016;
        public const long CON_ISSCORED_CHECKSELECT = 3031401010000000017;
        public const long CON_SCORESHEET_COMBOBOX = 3031401010000000018;
        public const long CON_SKILLGROUP = 3031401010000000019;
        public const long CON_REALEXTENSION = 3031401010000000020;
        public const long CON_PARTICIPANTNUM = 3031401010000000021;
        public const long CON_SERVICEATTITUDE = 3031401010000000022;
        public const long CON_RECORDDURATIONEXCEPT = 3031401010000000023;
        public const long CON_CALLERDTMF = 3031401010000000024;
        public const long CON_CALLEDDTMF = 3031401010000000025;
        public const long CON_CTIREFERENCE = 3031401010000000026;
        public const long CON_KEYWORDS = 3031401010000000027;
        public const long CON_REPEATEDCALL = 3031401010000000028;
        public const long CON_CALLPEAK = 3031401010000000029;
        public const long CON_CHANNELNAME = 3031401010000000030;
        public const long CON_PROFESSIONAlLEVEL = 3031401010000000031; 
        public const long CON_EXCEPTIONSCORE = 3031401010000000032;
        public const long CON_AFTERDEALDURATIONEXCEPT = 3031401010000000033;
        public const long CON_ACSPEEXCEPTPROPORTION = 3031401010000000034;
        public const long CON_CUSTOMFIELD01 = 3031401010000010001;
        public const long CON_CUSTOMFIELD02 = 3031401010000010002;
        public const long CON_CUSTOMFIELD03 = 3031401010000010003;
        public const long CON_CUSTOMFIELD04 = 3031401010000010004;
        public const long CON_CUSTOMFIELD05 = 3031401010000010005;
        public const long CON_CUSTOMFIELD06 = 3031401010000010006;
        public const long CON_CUSTOMFIELD07 = 3031401010000010007;
        public const long CON_CUSTOMFIELD08 = 3031401010000010008;
        public const long CON_CUSTOMFIELD09 = 3031401010000010009;
        public const long CON_CUSTOMFIELD10 = 3031401010000010010;
        public const long CON_CUSTOMFIELD11 = 3031401010000010011;
        public const long CON_CUSTOMFIELD12 = 3031401010000010012;
        public const long CON_CUSTOMFIELD13 = 3031401010000010013;
        public const long CON_CUSTOMFIELD14 = 3031401010000010014;
        public const long CON_CUSTOMFIELD15 = 3031401010000010015;
        public const long CON_CUSTOMFIELD16 = 3031401010000010016;
        public const long CON_CUSTOMFIELD17 = 3031401010000010017;
        public const long CON_CUSTOMFIELD18 = 3031401010000010018;
        public const long CON_CUSTOMFIELD19 = 3031401010000010019;
        public const long CON_CUSTOMFIELD20 = 3031401010000010020;







        //用户自定义参数编号
        public const int USER_PARAM_GROUP_BASIC = 310201;
        public const int USER_PARAM_GROUP_EXPORTDATA = 310202;
        public const int USER_PARAM_GROUP_EXPORTRECORD = 310203;
        public const int USER_PARAM_GROUP_PLAYSCREEN = 310204;

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

        public const int USER_PARAM_PAGESIZE = 31020101;
        public const int USER_PARAM_MAXRECORDS = 31020102;
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
        public const int USER_PARAM_EXPORTRECORD_ENCRYPTRECORD = 31020312;
        public const int USER_PARAM_EXPORTRECORD_ENCRYPTPASSWORD = 31020313;

        public const int USER_PARAM_PLAYSCREEN_NOPLAY = 31020401;
        public const int USER_PARAM_PLAYSCREEN_TOPMOST = 31020402;
        public const int USER_PARAM_PLAYSCREEN_SCALE = 31020403;

    }
}
