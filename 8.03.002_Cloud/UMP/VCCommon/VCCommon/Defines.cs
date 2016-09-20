//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    411df508-c4e6-465a-a2f9-75e70d844528
//        CLR Version:              4.0.30319.18063
//        Name:                     Defines
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                Defines
//
//        created by Charley at 2014/3/21 15:32:02
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.Common
{
    /// <summary>
    /// 用于定义VoiceCyber产品名称等信息
    /// </summary>
    public class Defines
    {

        #region 常用定义

        // 公司名称定义
        public const string VCT_COMPANY_LONGNAME = "VoiceCyber Technologies Ltd.";
        public const string VCT_COMPANY_SHORTNAME = "VoiceCyber";

        // 产品名定义
        public const string VCLOG_PRODUCTER_LONGNAME = "VoiceCyber Log System";
        public const string VCLOG_PRODUCTER_SHORTNAME = "VCLog";

        //数据库类型定义
        public const int VCT_DBTYPE_UNKNOWN = 0;	// Unknown
        public const int VCT_DBTYPE_MYSQL = 1;	// MySQL
        public const int VCT_DBTYPE_MSSQL = 2;	// MS SQL Server
        public const int VCT_DBTYPE_ORACLE = 3;	// Oracle
        public const int VCT_DBTYPE_ACCESS = 4;	// Access
        public const int VCT_DBTYPE_SYBASE = 5;	// Sybase
        public const int VCT_DBTYPE_SQLITE = 6;	// SQLite
        public const string VCT_DBTYPE_STR_UNKNOWN = "Unknown";
        public const string VCT_DBTYPE_STR_MYSQL = "MySQL";
        public const string VCT_DBTYPE_STR_MSSQL = "SQL Server";
        public const string VCT_DBTYPE_STR_ORACLE = "Oracle";
        public const string VCT_DBTYPE_STR_SQLITE = "SQLite";
        public const string VCT_DBTYPE_STR_SYBASE = "Sybase";

        //数据库分类定义
        public const int VCT_DB_PARAMS = 0;	// 参数数据库
        public const int VCT_DB_VMC = 1;	// VMC数据库
        public const int VCT_DB_VCLOGDB = 2;	// 录音记录数据库
        public const int VCT_DB_ECNCRYPT = 3; // 加密系统数据库

        #endregion
       

        #region Return Code

        //基本操作结果
        public const int RET_SUCCESS = 0;
        public const int RET_FAIL = 1;

        //一般操作结果
        public const int RET_PARAM_INVALID = 101;
        public const int RET_NOT_EXIST = 102;
        public const int RET_TIMEOUT = 103;
        public const int RET_OBJECT_NULL = 104;
        public const int RET_STRING_EMPTY = 105;
        public const int RET_NOT_IMPLIMENT = 106;
        public const int RET_ALREADY_EXIST = 107;
        public const int RET_IN_USE = 108;
        public const int RET_CANCEL = 109;

        //网络相关
        public const int RET_CONNECT_SUCCESS = 201;
        public const int RET_CONNECT_FAIL = 202;
        public const int RET_NOT_CONNECTED = 203;
        public const int RET_AUTH_SUCCESS = 204;
        public const int RET_AUTH_FAIL = 205;
        public const int RET_NOT_AUTHENTICATED = 206;

        //文件相关
        public const int RET_FILE_NOT_EXIST = 301;
        public const int RET_CONFIG_NOT_EXIST = 302;
        public const int RET_CONFIG_INVALID = 303;
        public const int RET_EQUAL_PATH = 304;
        public const int RET_FILE_ALREADY_EXIST = 305;
        public const int RET_XML_NODE_NOT_EXIST = 321;
        public const int RET_XML_ELE_NOT_EXIST = 322;

        //数据库相关
        public const int RET_DBACCESS_FAIL = 401;
        public const int RET_DBACCESS_EXIST = 402;
        public const int RET_DBACCESS_NOT_EXIST = 403;
        public const int RET_DBACCESS_TABLE_NOT_EXIST = 404;
        
        //验证
        public const int RET_CHECK_USER_NOT_EXIST = 501;
        public const int RET_CHECK_PASSWORD_ERROR = 502;
        public const int RET_CHECK_EXPIRED = 503;
        public const int RET_CHECK_ALREADY_LOGINED = 504;
        public const int RET_CHECK_FAIL = 505;

        #endregion


        #region Event Codes

        //基本事件
        public const int EVT_COMMON = 0;        //通用事件
        public const int EVT_EXCEPTION = 1;     //发生异常

        //一般事件
        public const int EVT_NOTIFY = 10;     //通知
        public const int EVT_COMPLETED = 11;     //完成
        
        //网络相关
        public const int EVT_NET_CONNECTED = 101;       //连接建立
        public const int EVT_NET_DISCONNECTED = 102;    //连接断开
        public const int EVT_NET_AUTHED = 103;          //认证
        public const int EVT_NET_WELCOME = 104;         //欢迎
        public const int EVT_NET_HEARTBEAT = 106;       //心跳

        //页面或者控件相关
        public const int EVT_PAGE_LOADED = 201;     //页面加载完成
        public const int EVT_PAGE_UNLOADED = 202;   //页面卸载完成
        public const int EVT_PAGE_LOADING = 203;    //页面正在加载
        public const int EVT_PAGE_UNLOADING = 204;  //页面正在卸载
        public const int EVT_PAGE_CLOSING = 205;    //页面关闭中
        public const int EVT_PAGE_CLOSED = 206;     //页面已关闭

        #endregion

    }
}
