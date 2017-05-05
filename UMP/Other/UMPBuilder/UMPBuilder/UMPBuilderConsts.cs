//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8669cf88-5062-4de7-af01-449f713831e4
//        CLR Version:              4.0.30319.18063
//        Name:                     UMPBuilderConsts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder
//        File Name:                UMPBuilderConsts
//
//        created by Charley at 2015/12/21 17:54:23
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPBuilder
{
    public class UMPBuilderConsts
    {

        #region 一般

        public const string FILE_NAME_PACKAGEFILE = "UMPData.zip";
        public const string FILE_NAME_UPDATEINFO = "UpdateInfo.xml";
        public const string PATH_UMPDATA = "Other\\UMPSetup\\UMPSetup\\Resources";

        #endregion


        #region Operation

        public const string OPT_NAME_RESTART = "BTRestart";
        public const string OPT_NAME_START = "BTStart";
        public const string OPT_NAME_PAUSE = "BTPause";
        public const string OPT_NAME_STOP = "BTStop";
        public const string OPT_NAME_PRE = "BTPre";
        public const string OPT_NAME_NEXT = "BTNext";
        public const string OPT_NAME_SETTING = "BTSetting";

        public const string OPT_DESC_RESTART = "Restart";
        public const string OPT_DESC_START = "Start";
        public const string OPT_DESC_PAUSE = "Pause";
        public const string OPT_DESC_STOP = "Stop";
        public const string OPT_DESC_PRE = "Pre Operation";
        public const string OPT_DESC_NEXT = "Next Operation";
        public const string OPT_DESC_SETTING = "Settings";

        #endregion


        #region 操作对象

        public const int OPTOBJ_SVNUPDATE = 1;
        public const int OPTOBJ_COMPILE = 2;
        public const int OPTOBJ_COPYFILE = 3;
        public const int OPTOBJ_PACKAGEFILE = 4;

        #endregion


        #region 状态编码

        public const int STA_DEFAULT = 0;
        public const int STA_WORKING = 1;
        public const int STA_SUCCESS = 2;
        public const int STA_FAIL = 3;
        public const int STA_WAITING = 4;

        #endregion


        #region 错误码

        /// <summary>
        /// 进程退出出错
        /// </summary>
        public const int RET_EXIT_FAIL = 1001;
        /// <summary>
        /// 用户取消
        /// </summary>
        public const int RET_EXIT_USERCANCEL = 1002;
        /// <summary>
        /// 编译器返回错误的基准
        /// </summary>
        public const int RET_DEVENV_FAIL = 1100;
        /// <summary>
        /// Svn客户端返回错误的基准
        /// </summary>
        public const int RET_SVNPROC_FAIL = 1200;
        /// <summary>
        /// 复制文件返回错误的基准
        /// </summary>
        public const int RET_XCOPY_FAIL = 1300;
        /// <summary>
        /// 编译器一般错误
        /// </summary>
        public const int RET_DEVENV_COMMON = 1101;
        /// <summary>
        /// 复制文件过程中源文件不存在
        /// </summary>
        public const int RET_XCOPY_SOURCEFILENOTEXIST = 1304;

        #endregion


        #region 子文件夹名称

        public const string DIR_WCF2CLIENT = "Wcf2Client";
        public const string DIR_WCFSERVICES = "WcfServices";
        public const string DIR_WINSERVICES = "WinServices";
        public const string DIR_WCF1600 = "WCF1600";
        public const string DIR_BIN = "bin";
        public const string DIR_MANAGEMENT = "ManagementMaintenance";

        #endregion


        #region 配置选项

        /// <summary>
        /// 是否更新主目录而不是逐个更新每个项目的目录
        /// </summary>
        public const string GS_SVNUPDATEALL = "SvnUpdateAll";
        /// <summary>
        /// 编译项目等待时长，单位：秒，默认：60
        /// </summary>
        public const string GS_DURATION_DEVENV = "DevenvDuration";
        /// <summary>
        /// Svn更新项目等待时长，单位：秒，默认：20 * 60
        /// </summary>
        public const string GS_DURATION_SVNUPDATE = "SvnUpdateDuration";
        /// <summary>
        /// Svn更新主目录等待时长，单位：秒，默认：60
        /// </summary>
        public const string GS_DURATION_SVNUPDATEROOTDIR = "SvnUpdateRootDirDuration";
        /// <summary>
        /// 复制文件等待时长，单位：秒，默认：10
        /// </summary>
        public const string GS_DURATION_XCOPYDURATION = "XCopyDuration";
        /// <summary>
        /// 是否生成更新包而不是安装包
        /// </summary>
        public const string GS_BUILDUPDATER = "BuildUpdater";

        #endregion


        #region 进度节点

        public const int PG_BASE_UPDATESVN = 0;
        public const int PG_BASE_PROJCOMPILE = 10;
        public const int PG_BASE_COPYFILE = 50;
        public const int PG_BASE_PACKAGE = 60;
        public const int PG_BASE_OTHER = 90;

        public const int PG_UPDATESVN = 10;
        public const int PG_PROJCOMPILE = 40;
        public const int PG_COPYFILE = 10;
        public const int PG_PACKAGE = 30;
        public const int PG_OTHER = 10;

        #endregion
    }
}
