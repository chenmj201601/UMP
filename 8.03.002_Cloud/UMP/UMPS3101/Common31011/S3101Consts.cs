//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    367cac4a-b5e8-4c30-82ef-0789c9cd24a6
//        CLR Version:              4.0.30319.18444
//        Name:                     S3101Consts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31011
//        File Name:                S3101Consts
//
//        created by Charley at 2014/10/8 15:46:49
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31011
{
    public class S3101Consts
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

        public const string SCORESHEET_SAVEPATH = "ScoreSheets";

        //资源码
        public const int RESOURCE_STATISTICALINFO = 311;
        public const int RESOURCE_STATISTICALPARAM = 312;

        //操作编号
        public const long OPT_CREATESCORESHEET = 3101001;
        public const long OPT_MODIFYSCORESHEET = 3101002;
        public const long OPT_DELETESCORESHEET = 3101003;
        public const long OPT_SETMANAGEUSER = 3101004;
        public const long OPT_IMPORTSCORESHEET = 3101005;
        public const long OPT_EXPORTSCORESHEET = 3101006;

        public const long OPT_NEWSCORESHEET = 3101100;
        public const long OPT_NEWSCOREGROUP = 3101101;
        public const long OPT_NEWAUTOSTANDARD = 3101110;
        public const long OPT_SAVESCORESHEET = 3101120;
        public const long OPT_DELETESCOREOBJECT = 3101121;
        public const long OPT_SCORECACULATE = 3101130;
        public const long OPT_CHECKVALID = 3101131;
        public const long OPT_SAVELAYOUT = 3101200;
        public const long OPT_RESETLAYOUT = 3101201;

        public const string OPT_NAME_SAVELAYOUT = "BT3101200";
        public const string OPT_NAME_RESETLAYOUT = "BT3101201";

        //Navigate
        public const int LNK_SSM = 1;//转到评分表管理
       

        //面板序号
        public const int PANEL_ID_SCOREOBJECT = 1;
        public const int PANEL_ID_SCORETEMPLATE = 2;
        public const int PANEL_ID_AUTOSTANDARD = 3;
        public const int PANEL_ID_SCOREVIEWER = 4;
        public const int PANEL_ID_SCOREPROPERTY = 5;
        public const int PANEL_ID_CHILDLISTER = 6;
        public const int PANEL_ID_CHILDPROPERTY = 7;

        //面板名称
        public const string PANEL_NAME_SCOREOBJECT = "ScoreObject";
        public const string PANEL_NAME_SCORETEMPLATE = "ScoreTemplate";
        public const string PANEL_NAME_AUTOSTANDARD = "AutoStandard";
        public const string PANEL_NAME_SCOREVIEWER = "ScoreViewer";
        public const string PANEL_NAME_SCOREPROPERTY = "ScoreProperty";
        public const string PANEL_NAME_CHILDLISTER = "ChildLister";
        public const string PANEL_NAME_CHILDPROPERTY = "ChildProperty";

        //面板ContentID
        public const string PANEL_CONTENTID_SCOREOBJECT = "PanelScoreObject";
        public const string PANEL_CONTENTID_SCORETEMPLATE = "PanelScoreTemplate";
        public const string PANEL_CONTENTID_AUTOSTANDARD = "PanelAutoStandard";
        public const string PANEL_CONTENTID_SCOREVIEWER = "PanelScoreViewer";
        public const string PANEL_CONTENTID_SCOREPROPERTY = "PanelScoreProperty";
        public const string PANEL_CONTENTID_CHILDLISTER = "PanelChildLister";
        public const string PANEL_CONTENTID_CHILDPROPERTY = "PanelChildProperty";

        //统计分析大项语言ID
        public const string SP_LANG_SERVICEATTITUDE = "FO31080102001";
        public const string SP_LANG_PROFESSIONALLEVEL = "FO31080102002";
        public const string SP_LANG_REPEATEDCALLIN = "FO31080102003";
        public const string SP_LANG_CALLPEAK = "FO31080102004";
        public const string SP_LANG_ACSPEEXCEPTPROPORTION = "FO31080102005";
        public const string SP_LANG_RECORDDURATIONEXCEPT = "FO31080102006";
        public const string SP_LANG_AFTERDEALDURATIONEXCEPT = "FO31080102007";
        public const string SP_LANG_EXCEPTIONSCORE = "FO31080102008";
    }
}
