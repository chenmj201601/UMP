//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ea25361d-7a5e-433c-8551-41964f5cb331
//        CLR Version:              4.0.30319.18444
//        Name:                     ConstValue
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                ConstValue
//
//        created by Charley at 2014/9/12 15:31:00
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 全局常量
    /// </summary>
    public static class ConstValue
    {

        #region 常用定义

        /// <summary>
        /// 公司名称全称
        /// </summary>
        public const string VCT_COMPANY_LONGNAME = "VoiceCyber Technologies Ltd.";
        /// <summary>
        /// 公司名称简称
        /// </summary>
        public const string VCT_COMPANY_SHORTNAME = "VoiceCyber";
        /// <summary>
        /// 产品全称
        /// </summary>
        public const string UMP_PRODUCTER_LONGNAME = "Unified Management Portal";
        /// <summary>
        /// 产品简称
        /// </summary>
        public const string UMP_PRODUCTER_SHORTNAME = "UMP";

        #endregion


        #region 分隔符

        /// <summary>
        /// 分隔符
        /// </summary>
        public const char SPLITER_CHAR = (char)27;
        /// <summary>
        /// 分隔符，如果需要不止一个分割符，第二个分隔符用此分割符
        /// </summary>
        public const char SPLITER_CHAR_2 = (char)30;
        /// <summary>
        /// 如果在xml的字段的值中需要分割符，请使用分号作为分隔符，不能使用不可见字符，否则解析xml出错
        /// </summary>
        public const char SPLITER_CHAR_3 = ';';

        #endregion


        #region 资源编号

        /// <summary>
        /// 租户
        /// </summary>
        public const int RESOURCE_RENT = 100;
        /// <summary>
        /// 机构/部门
        /// </summary>
        public const int RESOURCE_ORG = 101;
        /// <summary>
        /// 用户
        /// </summary>
        public const int RESOURCE_USER = 102;
        /// <summary>
        /// 坐席
        /// </summary>
        public const int RESOURCE_AGENT = 103;
        /// <summary>
        /// 分机
        /// </summary>
        public const int RESOURCE_EXTENSION = 104;
        /// <summary>
        /// 真实分机
        /// </summary>
        public const int RESOURCE_REALEXT = 105;
        /// <summary>
        /// 角色
        /// </summary>
        public const int RESOURCE_ROLE = 106;

        /// <summary>
        /// 录音(录屏)记录
        /// </summary>
        public const int RESOURCE_LOGRECORD = 201;
        /// <summary>
        /// 告警消息
        /// </summary>
        public const int RESOURCE_ALARMMESSAGE = 202;
        /// <summary>
        /// 告警信息
        /// </summary>
        public const int RESOURCE_ALARMINFOMATION = 203;
        /// <summary>
        /// 告警记录(历史记录)
        /// </summary>
        public const int RESOURCE_ALARMHISTORY = 204;

        /// <summary>
        /// 录音服务器
        /// </summary>
        public const int RESOURCE_VOICESERVER = 221;
        /// <summary>
        /// 录音通道
        /// </summary>
        public const int RESOURCE_VOICECHANNEL = 225;

        /// <summary>
        /// 评分表
        /// </summary>
        public const int RESOURCE_SCORESHEET = 301;
        /// <summary>
        /// 查询条件
        /// </summary>
        public const int RESOURCE_QUERYCONDITION = 302;
        /// <summary>
        /// 查询条件项
        /// </summary>
        public const int RESOURCE_CONDITIONITEM = 303;
        /// <summary>
        /// 考评/书签
        /// </summary>
        public const int RESOURCE_BOOKMARK = 304;
        /// <summary>
        /// 播放历史
        /// </summary>
        public const int RESOURCE_PLAYHISTORY = 305;
        /// <summary>
        /// 录音备注
        /// </summary>
        public const int RESOURCE_RECORDMEMO = 306;
        /// <summary>
        /// 评分成绩
        /// </summary>
        public const int RESOURCE_SCORERESULT = 307;
        /// <summary>
        /// 初检任务
        /// </summary>
        public const int RESOURCE_FIRSTTASK = 308;
        /// <summary>
        /// 书签等级
        /// </summary>
        public const int RESOURCE_BOOKMARKRANK = 309;
        /// <summary>
        /// 复检任务
        /// </summary>
        public const int RESOURCE_RECHECKTASK = 310;

        /// <summary>
        /// 操作日志(UMP系统)
        /// </summary>
        public const int RESOURCE_OPERATIONLOG_UMP = 901;
        /// <summary>
        /// 操作日志(其他模块)
        /// </summary>
        public const int RESOURCE_OPERATIONLOG_OTHERMODULE = 902;
        /// <summary>
        /// 登录流水号(UMP系统)
        /// </summary>
        public const int RESOURCE_OPERATIONLOG_UMP_LOGINID = 903;
        /// <summary>
        /// 登录流水号(其他模块)
        /// </summary>
        public const int RESOURCE_OPERATIONLOG_OTHERMODULE_LOGINID = 904;

        /// <summary>
        /// 机构类型
        /// </summary>
        public const int RESOURCE_ORGTYPE = 905;
        /// <summary>
        /// 技能组
        /// </summary>
        public const int RESOURCE_TECHGROUP = 906;

        /// <summary>
        /// 临时对象
        /// </summary>
        public const int RESOURCE_TEMPOBJECT = 911;
        /// <summary>
        /// 枚举记录的流水号
        /// </summary>
        public const int RESOURCE_ENUMOBJECT = 922;

        #endregion


        #region 特定资源编号

        /// <summary>
        /// 根机构编号
        /// </summary>
        public const long ORG_ROOT = 1010000000000000001;
        /// <summary>
        /// 系统超级管理员编号
        /// </summary>
        public const long USER_ADMIN = 1020000000000000001;
        /// <summary>
        /// 系统超级管理员角色编号
        /// </summary>
        public const long ROLE_SYSTEMADMIN = 1060000000000000001;
        /// <summary>
        /// 默认租户编号
        /// </summary>
        public const long RENT_DEFAULT = 1000000000000000000;
        /// <summary>
        /// 默认租户标识符
        /// </summary>
        public const string RENT_DEFAULT_TOKEN = "00000";

        #endregion


        #region 操作结果

        /// <summary>
        /// 失败
        /// </summary>
        public const string OPT_RESULT_FAIL = "R0";
        /// <summary>
        /// 成功
        /// </summary>
        public const string OPT_RESULT_SUCCESS = "R1";
        /// <summary>
        /// 异常
        /// </summary>
        public const string OPT_RESULT_EXCEPTION = "R2";
        /// <summary>
        /// 取消（关闭）
        /// </summary>
        public const string OPT_RESULT_CANCEL = "R3";
        /// <summary>
        /// 其他
        /// </summary>
        public const string OPT_RESULT_OTHER = "R4";

        #endregion


        #region 特定表名

        /// <summary>
        /// 录音录屏记录表
        /// </summary>
        public const string TABLE_NAME_RECORD = "T_21_001";
        /// <summary>
        /// 操作日志表
        /// </summary>
        public const string TABLE_NAME_OPTLOG = "T_11_901";
        /// <summary>
        /// 统计结果表
        /// </summary>
        public const string TABLE_NAME_STATISTICS = "T_31_054";

        #endregion


        #region 特定表中字段名

        /// <summary>
        /// 录音录屏记录开始时间
        /// </summary>
        public const string TABLE_FIELD_NAME_RECORD_STARTRECORDTIME = "C005";
        /// <summary>
        /// 操作日志的操作时间
        /// </summary>
        public const string TABLE_FIELD_NAME_OPTLOG_OPERATETIME = "C008";

        #endregion


        #region 网络相关

        /// <summary>
        /// 网络缓冲区的最大大小
        /// </summary>
        public const int NET_BUFFER_MAX_SIZE = 102400;
        /// <summary>
        /// 网络数据包包头大小（26个字节）
        /// </summary>
        public const int NET_MESSAGE_HEAD_SIZE = 26;
        /// <summary>
        /// 认证码的大小（32个字节）
        /// </summary>
        public const int NET_AUTH_CODE_SIZE = 32;

        #endregion


        #region 临时目录

        public const string TEMP_PATH_UMP = "UMP";

        public const string TEMP_PATH_MEDIAUTILS = "UMP\\MediaUtils";

        public const string TEMP_PATH_MEDIADATA = "UMP\\MediaData";

        public const string TEMP_PATH_UPLOADFILES = "UMP\\UploadFiles";

        public const string TEMP_PATH_FILELIST = "UMP\\MediaUtils\\FileList.txt";

        public const string TEMP_DIR_UMP = "UMP";

        public const string TEMP_DIR_MEDIAUTILS = "MediaUtils";

        public const string TEMP_DIR_MEDIADATA = "MediaData";

        public const string TEMP_DIR_UPLOADFILES = "UploadFiles";

        public const string TEMP_FILE_FILELIST = "FileList.txt";

        public const string TEMP_FILE_FILECONVERT = "FileConvert.exe";

        public const string TEMP_FILE_SCREENMP = "ScreenMP.dll";

        public const string TEMP_FILE_CONFIGINFO = "ConfigInfo.xml";

        public const string TEMP_FILE_UMPSESSION = "umpsession.xml";

        #endregion


        #region Local Monitor 相关

        //Monitor 指令
        public const int MONITOR_COMMAND_GETSESSIONINFO = 1;

        //Monitor 对象类型
        public const int MONITOR_TYPE_SESSIONINFO = 1;
        public const int MONITOR_TYPE_WEBREQUEST = 10;
        public const int MONITOR_TYPE_WEBRETURN = 11;

        #endregion


        #region 全局设定参数相关


        #region 参数类

        /// <summary>
        /// 登录密码参数
        /// 值：密码（暂未加密）
        /// </summary>
        public const string GS_KEY_PARAM_PASSWORD = "ParamPassword";
        /// <summary>
        /// 登录角色参数
        /// 值：角色信息
        /// 0：角色编码
        /// 1：角色名称
        /// </summary>
        public const string GS_KEY_PARAM_ROLE = "ParamRole";
        /// <summary>
        /// 登录用户参数
        /// 值：用户信息
        /// 0：用户编码
        /// 1：用户帐号
        /// 2：用户全名
        /// </summary>
        public const string GS_KEY_PARAM_USER = "ParamUser";

        #endregion


        #region C021Mode

        /// <summary>
        /// 录音表中C021字段存放数据的模式
        /// 根据系统初始化的不同，录音表中C021字段存放的数据可能不同
        /// 默认情况下存放坐席工号（Mode为0或1）
        /// </summary>
        public const string GS_KEY_C021_MODE = "C021Mode";
        /// <summary>
        /// C021存放坐席工号
        /// </summary>
        public const int GS_C021_MODE_AGENT = 1;
        /// <summary>
        /// C021存放分机号码（分机号码+VoiceIP）
        /// </summary>
        public const int GS_C021_MODE_EXTENTION = 2;
        /// <summary>
        /// C021存放主叫号码
        /// </summary>
        public const int GS_C021_MODE_CALLERID = 10;
        /// <summary>
        /// C021存放被叫号码
        /// </summary>
        public const int GS_C021_MODE_CALLEDID = 11;

        #endregion


        #region 管理对象分组模式

        /// <summary>
        /// 管理对象分组模式，默认为（EA）
        /// </summary>
        public const string GS_KEY_OBJ_CONTROLMODE = "ObjControlMode";
        /// <summary>
        /// 仅按坐席管理
        /// </summary>
        public const string GS_OBJ_CONTROLMODE_AGT = "A";
        /// <summary>
        /// 仅按分机管理
        /// </summary>
        public const string GS_OBJ_CONTROLMODE_EXT = "E";
        /// <summary>
        /// 仅管理真实分机
        /// </summary>
        public const string GS_OBJ_CONTROLMODE_REALEXT = "R";
        /// <summary>
        /// 按坐席分机管理（默认）
        /// </summary>
        public const string GS_OBJ_CONTROLMODE_AGTEXT = "EA";

        #endregion


        #region Windows 服务相关

        /// <summary>
        /// 日志级别
        /// 数值，参考VoiceCyber.Common.LogMode枚举的定义
        /// </summary>
        public const string GS_KEY_LOG_MODE = "LogMode";
        /// <summary>
        /// 客户端连接超时时间，单位：秒
        /// 范围 5 ~ 60 * 60 * 24 ，即5秒到1天，默认600秒（10分钟）
        /// </summary>
        public const string GS_KEY_TIMEOUT_SESSION = "SessionTimeout";
        /// <summary>
        /// 服务重读参数时间间隔，单位：分钟
        /// 范围 1 ~ 60 * 24，即1分钟到1天，默认30分钟
        /// </summary>
        public const string GS_KEY_INTERVAL_REREADPARAM = "ReReadParamInterval";
        /// <summary>
        /// 当数据库连接失败，重新连接的时间间隔，单位：秒
        /// 范围 1 ~ 60 * 60，即1秒到1小时，默认10秒
        /// </summary>
        public const string GS_KEY_INTERVAL_RECONNECTDB = "ReConnectDBInterval";
        /// <summary>
        /// 服务端检查客户端连接状况的时间间隔，单位：秒
        /// 范围 5 ~ 60 * 60 ，即5秒到1小时，默认30秒
        /// </summary>
        public const string GS_KEY_INTERVAL_CHECKSESSION = "CheckSessionInterval";

        #endregion


        #region 客户端临时文件

        /// <summary>
        /// 回删MediaData目录中的临时文件的时间间隔，单位：分钟
        /// 范围：10 ~ 60 * 24，即10分钟到1天，默认30分钟
        /// </summary>
        public const string GS_KEY_INTERVAL_MEDIADATARECYCLE = "MediaDataRecycleInterval";
        /// <summary>
        /// MediaData目录中的临时文件保留时间，单位：分钟
        /// 范围：10 ~ 10 * 60 * 24，即10分钟到10天，默认1天
        /// </summary>
        public const string GS_KEY_TIME_MEDIADATASAVE = "MediaDataSaveTime";

        #endregion


        #region 其他

        /// <summary>
        /// 每周开始的一天
        /// 0：周日
        /// 1：周一
        /// ...
        /// 6：周六
        /// </summary>
        public const string GS_KEY_WEEK_FIRSTDAY = "WeekFirstDay";
        /// <summary>
        /// 每月开始的一天
        /// 1：1号（自然月）
        /// 2：2号
        /// ...
        /// 28：28号
        /// 注意：没有29号，30号，31号，因为这些有的月中没有
        /// </summary>
        public const string GS_KEY_MONTH_FIRSTDAY = "MonthFirstDay";
        /// <summary>
        /// 单次查询最大记录数，有以下可用值
        /// 100：
        /// 200：
        /// 500：
        /// 1000：
        /// </summary>
        public const string GS_KEY_MAXNUM_ONCE = "MaxNumOnce";
        /// <summary>
        /// 查询总的最大记录数，有以下可用值（单位：万）
        /// 1：
        /// 5：
        /// 10：
        /// 50：
        /// 100：
        /// </summary>
        public const string GS_KEY_MAXNUM_TOTAL = "MaxNumTotal";
        /// <summary>
        /// 操作日志保留时间，有以下可用值（单位：月）
        /// 1：
        /// 2：
        /// 3：
        /// 6：
        /// 12：
        /// 24：
        /// 36：
        /// </summary>
        public const string GS_KEY_OPTLOG_SAVEMONTH = "OptLogSaveMonth";

        #endregion


        #endregion


        #region 全局参数相关

        /// <summary>
        /// 管理对象模式组
        /// </summary>
        public const int GP_GROUP_OBJ_CONTROL = 120104;

        /// <summary>
        /// 管理对象模式
        /// </summary>
        public const int GP_OBJ_CONTROLMODE = 12010401;

        /// <summary>
        /// 用户注销参数组
        /// </summary>
        public const int GP_GROUP_USER_LOGOUT = 110202;
        /// <summary>
        /// 超时强制注销时间
        /// </summary>
        public const int GP_USER_LOGOUT_TIMEOUT = 11020201;
        /// <summary>
        /// 无操作强制注销时间
        /// </summary>
        public const int GP_USER_LOGOUT_IDLE = 11020202;

        #endregion


        #region 用户参数相关

        /// <summary>
        /// 用户语言组
        /// </summary>
        public const int UP_GROUP_LANG = 11001;

        /// <summary>
        /// 用户登录系统的默认语言
        /// </summary>
        public const int UP_DEFAULT_LANG = 1100101;

        /// <summary>
        /// 默认页组
        /// </summary>
        public const int UP_GROUP_PAGE = 11002;

        /// <summary>
        /// 用户登录系统的默认页
        /// </summary>
        public const int UP_DEFAULT_PAGE = 1100201;

        #endregion

    }
}
