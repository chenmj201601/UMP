//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7e95e16c-1718-4973-a238-acce6c28e315
//        CLR Version:              4.0.30319.42000
//        Name:                     S1200Consts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12001
//        File Name:                S1200Consts
//
//        created by Charley at 2016/1/22 14:13:13
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common12001
{
    public class S1200Consts
    {

        #region Core ModuleID

        /// <summary>
        /// 登录模块
        /// </summary>
        public const int MODULEID_LOGIN = 1202;
        /// <summary>
        /// PageHead
        /// </summary>
        public const int MODULEID_PAGEHEAD = 1203;
        /// <summary>
        /// 状态栏
        /// </summary>
        public const int MODULEID_STATUSBAR = 1204;
        /// <summary>
        /// 任务模块
        /// </summary>
        public const int MODULEID_TASKPAGE = 1205;
        /// <summary>
        /// Dashboard
        /// </summary>
        public const int MODULEID_DASHBOARD = 1206;

        /// <summary>
        /// 特殊模块（ASM）
        /// </summary>
        public const int MODULEID_ASM = 4401;
        /// <summary>
        /// 特殊模块（IM）
        /// </summary>
        public const int MODULEID_IM = 1601;

        #endregion


        #region Core AppNames

        /// <summary>
        /// 登录模块
        /// </summary>
        public const string APPNAME_LOGIN = "UMPS1202";
        /// <summary>
        /// PageHead
        /// </summary>
        public const string APPNAME_PAGEHEAD = "UMPS1203";
        /// <summary>
        /// 状态栏
        /// </summary>
        public const string APPNAME_STATUSBAR = "UMPS1204";
        /// <summary>
        /// 任务模块
        /// </summary>
        public const string APPNAME_TASKPAGE = "UMPS1205";
        /// <summary>
        /// Dashboard
        /// </summary>
        public const string APPNAME_DASHBOARD = "UMPS1206";

        #endregion


        #region ModuleNames

        /// <summary>
        /// 登录模块
        /// </summary>
        public const string MODULENAME_LOGIN = "S1202App";
        /// <summary>
        /// PageHead
        /// </summary>
        public const string MODULENAME_PAGEHEAD = "S1203App";
        /// <summary>
        /// 状态栏
        /// </summary>
        public const string MODULENAME_STATUSBAR = "S1204App";
        /// <summary>
        /// 任务模块
        /// </summary>
        public const string MODULENAME_TASKPAGE = "S1205App";
        /// <summary>
        /// Dashboard
        /// </summary>
        public const string MODULENAME_DASHBOARD = "S1206App";

        #endregion


        #region RegionNames

        /// <summary>
        /// 登录模块
        /// </summary>
        public const string REGIONNAME_LOGIN = "LoginRegion";
        /// <summary>
        /// PageHead
        /// </summary>
        public const string REGIONNAME_PAGEHEAD = "PageHeadRegion";
        /// <summary>
        /// 状态栏
        /// </summary>
        public const string REGIONNAME_STATUSBAR = "StatusBarRegion";
        /// <summary>
        /// 任务模块
        /// </summary>
        public const string REGIONNAME_TASKPAGE = "TaskPageRegion";
        /// <summary>
        /// Dashboard
        /// </summary>
        public const string REGIONNAME_DASHBOARD = "DashboardRegion";

        #endregion


        #region PageHeadCommand

        public const string PH_CMD_LOGOUT = "Logout";

        public const string PH_CMD_LEFTPANEL = "LeftPanel";

        public const string PH_CMD_DEFAULTPAGE = "DefaultPage";

        public const string PH_CMD_OPENIMPANEL = "OpenIMPanel";

        public const string PH_CMD_CHANGEPASSWORD = "ChangePassword";

        public const string PH_CMD_CHANGEROLE = "ChangeRole";

        #endregion


        #region ContentID

        public const string CONTENTID_TASKPAGE = "PanelTaskPage";

        #endregion
     

        #region 其他

        public const string CONFIG_FILE_NAME_07 = "UMP.Server.07.xml";

        public const string THIRDPARTY_APP_NAME_ASM = "ASM";

        #endregion

    }
}
