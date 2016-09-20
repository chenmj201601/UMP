//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a6c709b8-12d6-4b8e-8781-2d9afdbb5f02
//        CLR Version:              4.0.30319.18063
//        Name:                     LicDefines
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.Licenses
//        File Name:                LicDefines
//
//        created by Charley at 2015/7/27 10:23:22
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.SDKs.Licenses
{
    /// <summary>
    /// License 相关定义
    /// </summary>
    public class LicDefines
    {
        /// <summary>
        /// 认证码的长度
        /// </summary>
        public const int LENGTH_VERIFICATION = 32;


        public const int NET_PACKET_HEADER_SIZE = 16;       //消息头长度固定为16个字节

        //返回码
        public const int RET_CODE_SUCCESS = 0;		        // 成功
        public const int RET_CODE_OTHERERROR = -1;	        // 其他错误
        public const int RET_CODE_DISCONNECT = -2;	        // 网络未连接或已断开
        public const int RET_CODE_NOMESSAGE = -3;	        // 没有消息
        public const int RET_CODE_NOT_SUPPORT = -4;	        // 不支持此操作
        public const int RET_CODE_NOT_AUTHENTICATE = -5;	// 未认证

        //消息类别
        public const int LICENSE_MSG_CLASS_UNKNOWN = 0;	    // 未知
        public const int LICENSE_MSG_CLASS_EXCEPTION = 1;	// 发生异常
        public const int LICENSE_MSG_CLASS_CONNECTION = 2;	// 链接与握手消息
        public const int LICENSE_MSG_CLASS_AUTHENTICATE = 3;	// 认证
        public const int LICENSE_MSG_CLASS_NOTIFY = 4;	    // 通知，仅服务端向客户端发送
        public const int LICENSE_MSG_CLASS_REQRES = 5;	    // 请求应答

        public const int LICENSE_MSG_UNKNOWN = 0;	        // 未知消息

        // 异常信息										 	
        public const int LICENSE_MSG_EXCEPTION_UNKNOWN = LICENSE_MSG_UNKNOWN;	// 未知
        public const int LICENSE_MSG_EXCEPTION_UNKNOWN_CLASS = 1;	            // 未知消息类型


        // 链接握手消息								   	
        public const int LICENSE_MSG_CONNECTION_UNKNOWN = LICENSE_MSG_UNKNOWN;	// 未知
        public const int LICENSE_MSG_CONNECTION_HEARTBEAT = 1;	                // 心跳
        public const int LICENSE_MSG_CONNECTION_FAILED = 2;	                    // 链接失败，客户端本地消息
        public const int LICENSE_MSG_CONNECTION_LOST = 3;	                    // 意外中断，客户端本地消息
        public const int LICENSE_MSG_CONNECTION_CLOSE = 4;	                    // 客户端关闭连接
        public const int LICENSE_MSG_CONNECTION_TIMEOUT = 5;	                // 网络超时
        public const int LICENSE_MSG_CONNECTION_SHUTDOWN = 6;	                // 服务关闭
        public const int LICENSE_MSG_CONNECTION_MASERT_RECOVER = 7;	            // 1ST恢复
        public const int LICENSE_MSG_CONNECTION_DATA_FORMAT = 8;	            // 数据格式错误

        // 认证消息
        public const int LICENSE_MSG_AUTH_UNKNOWN = LICENSE_MSG_UNKNOWN;	    // 未知
        public const int LICENSE_MSG_AUTH_WELCOME = 1;	                        // 欢迎消息，仅服务端向客户端发送
        public const int LICENSE_MSG_AUTH_LOGON = 2;	                        // 登录,仅客户端向服务端发送
        public const int LICENSE_MSG_AUTH_SUCCESS = 3;	                        // 通过，仅服务端向客户端发送
        public const int LICENSE_MSG_AUTH_FAILED = 4;	                        // 认证失败，仅服务端向客户端发送
        public const int LICENSE_MSG_AUTH_DENY_CLIENT_TYPE = 5;	                // 拒绝的客户端类型，仅服务端向客户端发送
        public const int LICENSE_MSG_AUTH_PROTOCOL_CLIENT = 6;	                // 客户端不支持该版本协议
        public const int LICENSE_MSG_AUTH_PROTOCOL_SERVER = 7;	                // 服务端端不支持该版本协议
        public const int LICENSE_MSG_AUTH_WRONGFUL_SERVER = 8;	                // 对服务端的认证未通过，可能为假冒的服务端
        public const int LICENSE_MSG_AUTH_TIME_NOT_SYNC = 9;	                // 客户端与服务端时间不同步
        // 通知消息										 	
        public const int LICENSE_MSG_NOTIFY_UNKNOWN = LICENSE_MSG_UNKNOWN;	    // 未知
        public const int LICENSE_MSG_NOTIFY_NEW_CLIENT = 1; 	                // 有新的Clinet连接，只通知到Manager端
        public const int LICENSE_MSG_NOTIFY_DEL_CLIENT = 2;	                    // 有Client连接断开，只通知到Manager端
        public const int LICENSE_MSG_NOTIFY_CHANGED_LICENSE = 3;	            // 向APPLICATION端发送License数目变化消息，包含该客户端最新License的总数信息
        public const int LICENSE_MSG_NOTIFY_LICESNE_POOL = 4;	                // 通知License pool中License数据信息，只通知到Manager端
        public const int LICENSE_MSG_NOTIFY_APPLICATION_LICENSE = 5;	        // 某APPLICATION当前License信息，只通知到Manager端,消息中包括剩余License数目信息
        public const int LICENSE_MSG_NOTIFY_SOFTDOGS_INFO = 6;	                // 软件狗信息，多只狗在一个消息中，通知到Manager\Synchronism\Alarm端
        public const int LICENSE_MSG_NOTIFY_LICENSE_SERVERS = 7;	            // License服务器连接信息，通知到Application\Manager端
        public const int LICENSE_MSG_NOTIFY_LOCAL_SERVER = 8;	                // 本License服务器连接信息，通知到Application\Manager端
        //const int LICENSE_MSG_NOTIFY_PRAMETER_DATABSE		= 9;	            // 参数数据库信息，通知到Application\Manager端,8.02.001之后不支持
        public const int LICENSE_MSG_NOTIFY_HOLD_SOFTDOG = 10;	                // 挂起或解除挂起软件狗消息,通知到Manager端
        // 请求应答消息								   	
        public const int LICENSE_MSG_REQRES_UNKNOWN = LICENSE_MSG_UNKNOWN;	    // 未知
        public const int LICENSE_MSG_REQUEST_QUERY_TOTAL_LICENSE = 2;	        // 查询合计License数据
        public const int LICENSE_MSG_RESPONSE_QUERY_TOTAL_LICENSE = 3;	        // 响应合计License数据
        public const int LICENSE_MSG_REQUEST_QUERY_FREE_LICENSE = 4;	        // 查询可用License数据
        public const int LICENSE_MSG_RESPONSE_QUERY_FREE_LICENSE = 5;	        // 响应可用License数据
        public const int LICENSE_MSG_REQUEST_GET_LICENSE = 6;	                // 获取License，包括check，消息中包含本次请求的License数目
        public const int LICENSE_MSG_RESPONSE_GET_LICENSE = 7;	                // 响应获取License，包括check，消息中包含本次应答的License数目，同时包含该客户端请求License的总数信息
        public const int LICENSE_MSG_REQUEST_RELEASE_LICENSE = 8;	            // 释放License，包括check，消息中包含本次要施放的License数目
        public const int LICENSE_MSG_RESPONSE_RELEASE_LICENSE = 9;	            // 响应释放License，包括check，消息中包含本次真实释放的License数目
        public const int LICENSE_MSG_REQUEST_RELEASE_ALL_LICENSE = 10;	        // 释放所有License，可以包括check
        public const int LICENSE_MSG_RESPONSE_RELEASE_ALL_LICENSE = 11;	        // 响应释放所有License，可以包括check
        public const int LICENSE_MSG_REQUEST_QUERY_SELF_LICENSE = 12;	        // 查询当前请求的所有License信息
        public const int LICENSE_MSG_RESPONSE_QUERY_SELF_LICENSE = 13;	        // 响应查询当前请求的所有License信息
        public const int LICENSE_MSG_REQUEST_LIST_SOFTDOG = 14;	                // 列出所有软件狗
        public const int LICENSE_MSG_RESPONSE_LIST_SOFTDOG = 15;	            // 响应列出所有软件狗
        public const int LICENSE_MSG_REQUEST_SELECT_SOFTDOG = 16;	            // 选择当前软件狗
        public const int LICENSE_MSG_RESPONSE_SELECT_SOFTDOG = 17;	            // 响应选择当前软件狗
        public const int LICENSE_MSG_REQUEST_QUERY_SPECIFIC_LICENSE = 18;	    // 查找指定的License信息
        public const int LICENSE_MSG_RESPONSE_QUERY_SPECIFIC_LICENSE = 19;	    // 响应查找指定的License信息
        public const int LICENSE_MSG_REQUEST_HOLD_SOFTDOG = 20;	                // 挂起软件狗，此操作后再一定时间内不会读取软件狗，此时可以安全更换软件狗，只有Manager端可以做此操作
        public const int LICENSE_MSG_RESPONSE_HOLD_SOFTDOG = 21;	            // 响应挂起软件狗，此操作后再一定时间内不会读取软件狗，此时可以安全更换软件狗
        public const int LICENSE_MSG_REQUEST_UNHOLD_SOFTDOG = 22;	            // 解除挂起软件狗，只有Manager端可以做此操作
        public const int LICENSE_MSG_RESPONSE_UNHOLD_SOFTDOG = 23;	            // 响应解除挂起软件狗

        // 应答结果
        public const int LICENSE_RESPONSE_RESULT_SUCCESS = 0;	                // 成功
        public const int LICENSE_RESPONSE_RESULT_UNKNOWN = 1;	                // 未知请求
        public const int LICENSE_RESPONSE_RESULT_DENY = 2;	                    // 拒绝
        public const int LICENSE_RESPONSE_RESULT_INVALID_PARAMS = 3;	        // 参数错误
        public const int LICENSE_RESPONSE_RESULT_FAILED = 4;	                // 本操作失败
        public const int LICENSE_RESPONSE_RESULT_NOT_FOUND_SOFTDOG = 5;	        // 未找到软件狗
        public const int LICENSE_RESPONSE_RESULT_NOT_FOUND_SPECIFY_SOFTDOG = 6;	// 未找到指定的软件狗
        public const int LICENSE_RESPONSE_RESULT_SAVE_PARAMETERS = 7;	        // 保存参数失败
        public const int LICENSE_RESPONSE_RESULT_OPERATOR_FREQUENTLY = 8;	    // 操作过于频繁
        public const int LICENSE_RESPONSE_RESULT_ALLREADY_EXECUTED = 9;	        // 操作已执行
        public const int LICENSE_RESPONSE_RESULT_HOLD_SOFTDOG = 10;	            // 软件狗被挂起，不能执行此操作


        /////////////////////////////////////////////////
        // 关键字

        // common 
        public const string KEYWORD_MSG_COMMON_CLASSID = "ClassID";		// 消息类别
        public const string KEYWORD_MSG_COMMON_CLASSDESC = "ClassDesc";	// 消息类别
        public const string KEYWORD_MSG_COMMON_MESSAGEID = "MessageID";	// 消息
        public const string KEYWORD_MSG_COMMON_MESSAGEDESC = "MessageDesc";	// 消息
        public const string KEYWORD_MSG_COMMON_CURRENTTIME = "CurrentTime";	// 当前时间
        public const string KEYWORD_MSG_COMMON_DATA = "Data";		// 数据

        // AUTHENTICATE
        public const string KEYWORD_MSG_AUTH_PROTOCOL = "Protocol";	// 协议版本
        public const string KEYWORD_MSG_AUTH_VERIFICATION = "Verification";	// 校验码

        // connection
        public const string KEYWORD_MSG_CONNTION_HEARTBEAT = "Heartbeat";	// heartbeat

        // client info
        public const string KEYWORD_MSG_APPINFO = "ApplicationInfo";// 客户端信息
        public const string KEYWORD_MSG_APPINFO_PRODUCT = "Product";	// 产品
        public const string KEYWORD_MSG_APPINFO_MODULETYPEID = "ModuleTypeID";	// ModuleTypeID
        public const string KEYWORD_MSG_APPINFO_MODULENAME = "ModuleName";	// 模块名
        public const string KEYWORD_MSG_APPINFO_VERSION = "Version";	// 版本
        public const string KEYWORD_MSG_APPINFO_SESSION = "Session";	// Session
        public const string KEYWORD_MSG_APPINFO_CLIENTLICPOOL = "ClientLicensePool";// license pool
        public const string KEYWORD_MSG_APPINFO_LICENSES = "Licenses";    // license 数组
        public const string KEYWORD_MSG_APPINFO_MODULENUMBER = "ModuleNumber";	// 模块编号
        public const string KEYWORD_MSG_APPINFO_HOST = "Host";	// 主机地址
        public const string KEYWORD_MSG_APPINFO_PORT = "Port";	// port
        public const string KEYWORD_MSG_APPINFO_CONNECTTIME = "ConnectTime";	// connect time

        // 软件狗相关信息
        public const string KEYWORD_MSG_SOFTDOG = "Softdog";	// 软件狗信息
        public const string KEYWORD_MSG_SOFTDOG_VERSION = "Version";	// 软件狗版本
        public const string KEYWORD_MSG_SOFTDOG_GLOBALINFO = "GlobalInfo";	// 软件狗信息
        public const string KEYWORD_MSG_SOFTDOG_SERIALNUMBER = "SerialNumber";	// 软件狗序列号
        public const string KEYWORD_MSG_SOFTDOG_TYPE = "SoftdogType";	// 软件狗类型
        public const string KEYWORD_MSG_SOFTDOG_TYPE_DESC = "SoftdogTypeDescription";// 软件狗类型
        public const string KEYWORD_MSG_SOFTDOG_DURATION = "Duration";	// 可用持续时间
        public const string KEYWORD_MSG_SOFTDOG_MASTERSOFTDOG = "MasterSoftdog";	// 主狗
        public const string KEYWORD_MSG_SOFTDOG_OPERATIONDATE = "OperationDate";	// 软件狗操作时间
        public const string KEYWORD_MSG_SOFTDOG_SERVICEDATE = "ServiceDate";     // 服务期限
        public const string KEYWORD_MSG_SOFTDOG_PRODUCTORVER = "ProductorVersion";  // 产品版本
        public const string KEYWORD_MSG_SOFTDOG_DEADLINE = "Deadline";	// 最后期限
        public const string KEYWORD_MSG_SOFTDOG_EXPIRATION = "Expiration";	// 动态过期时间
        public const string KEYWORD_MSG_SOFTDOG_LOSTEFFICACY = "LostEfficacy";	// 失效时间，最后期限与动态过期时间计算所得
        public const string KEYWORD_MSG_SOFTDOG_VALIDITY = "Validity";		// 有效性
        public const string KEYWORD_MSG_SOFTDOG_LICENSES = "Licenses";		// licenses
        public const string KEYWORD_MSG_SOFTDOG_MAJORITEM = "Item";	    // 项目,对应大项目
        public const string KEYWORD_MSG_SOFTDOG_MINORITEM = "Item";	    // 项目,对应小项目
        public const string KEYWORD_MSG_SOFTDOG_MODULEMARJORTYPEID = "ModuleMajorTypeID";	// 模块主类型编号

        // License
        public const string KEYWORD_MSG_LICENSE_LICENSEID = "LicenseID";      // License id
        public const string KEYWORD_MSG_LICENSE_OWNERTYPE = "OwnerType";     // license owner type, 独占、共享
        public const string KEYWORD_MSG_LICENSE_EXPIRATION = "Expiration";      // 过期时间
        public const string KEYWORD_MSG_LICENSE_VALUETYPE = "ValueType";    // license 值类型
        public const string KEYWORD_MSG_LICENSE_VALUE = "Value";     // license 值
        public const string KEYWORD_MSG_LICENSE_DISPLAY = "Display";	// license display
        public const string KEYWORD_MSG_LICENSE_MODULEMARJORTYPEID = "ModuleMajorTypeID";// 模块主类型编号
        public const string KEYWORD_MSG_LICENSE_MODULEMINJORTYPEID = "ModuleMinortypeID";	// 模块子类型编号

        // License Pool
        public const string KEYWORD_MSG_SERVER_LICPOOL = "ServerLicensePool";	// 服务器LicensePool
        public const string KEYWORD_MSG_SERVER_LICPOOL_TOTAL = "Total";           // 总的license信息
        public const string KEYWORD_MSG_SERVER_LICPOOL_FREE = "Free";       // 剩余license信息

        // request response license
        public const string KEYWORD_MSG_REQRES_REQUESTID = "RequestID";	// 请求id
        public const string KEYWORD_MSG_REQRES_RESPONSEID = "ResponseID";	// 应答id,对应requestid
        public const string KEYWORD_MSG_REQRES_LICENSES = "Licenses";   // license 数组
        public const string KEYWORD_MSG_REQRES_THISTIME = "ThisTime";	// 本次
        public const string KEYWORD_MSG_REQRES_RESRESULT = "ResponseResult";// 应答结果

        // notify
        public const string KEYWORD_MSG_SOFTDOGS = "Softdogs";	// 软件狗
        public const string KEYWORD_MSG_ERROR_SOFTDOGS = "ErrorSoftdogs";	// 读取错误的软件狗
        public const string KEYWORD_MSG_CURRENT_SOFTDOG = "CurrentSoftdog";	// 当前使用的软件狗
        public const string KEYWORD_MSG_LICENSESERVERS = "LicenseServer";// License服务器

        // error softdog
        public const string KEYWORD_MSG_ERROR_SOFTDOG_CODE = "ErrorCodes";	// 错误码
        public const string KEYWORD_MSG_ERROR_SOFTDOG_DESC = "Description";   // 描述信息

        // hold softdog
        public const string KEYWORD_MSG_HOLD_SOFTDOG = "HoldSoftdog";	// 挂起软件狗
        public const string KEYWORD_MSG_HOLD_SOFTDOG_STATE = "HoldState";	// 挂起状态
        public const string KEYWORD_MSG_HOLD_SOFTDOG_BEGIN_TIME = "BeginTime";	// 开始时间
        public const string KEYWORD_MSG_HOLD_SOFTDOG_TOTAL_TIMELENGTH = "TotalTimeLength";	// 时长
        public const string KEYWORD_MSG_HOLD_SOFTDOG_ELAPSE_TIMELENGTH = "ElapseTimeLength";	// 已流逝时间
        public const string KEYWORD_MSG_HOLD_SOFTDOG_BEGIN_TICKCOUNT = "BeginTickcount";// 开始时的计数
        public const string KEYWORD_MSG_HOLD_SOFTDOG_REMAIN_TIMELENGTH = "RemaintimeLength";// 剩余时间

        //其他
        public const string KEYWORD_LICENSE_EXPIRATION_UNLIMITED = "Unlimited";
        public const string KEYWORD_LICENSE_EXPIRATION_INVALID = "Invalid";
        public const string KEYWORD_LICENSE_EXPIRATION_EXPIRED = "Expired";

        // 数据格式
        public const int DH_FORMAT_BYTE = 0;		// BYTE
        public const int DH_FORMAT_JSON = 1;	    // Json
        public const int DH_FORMAT_XML = 2;         // XML

    }
}
