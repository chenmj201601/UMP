using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UMPCommon
{
    public class DecDefine
    {
        #region 网络数据包头
        //flag
        //public static char[] NETPACK_FLAG = new char[2] { 'N', 'P' };
        public static char[] NETPACK_FLAG = new char[2] { 'P', 'N' };

        //version
        public static byte NETPACK_DISTHEAD_VER1 = 1;

        // 基本包头类型
        public const ushort NETPACK_BASETYPE_NOTHING = 0x0000;	// 无基本包头
        public const ushort NETPACK_BASETYPE_CONNECT_ERROR = 0x0001;	// 错误消息
        public const ushort NETPACK_BASETYPE_CONNECT_HELLO = 0x0002;	// 问候包，客户端发送给服务端
        public const ushort NETPACK_BASETYPE_CONNECT_WELCOME = 0x0003;	// 欢迎包，服务端发送给客户端
        public const ushort NETPACK_BASETYPE_CONNECT_LOGON = 0x0004;	// 登录包，客户端发送给服务器端
        public const ushort NETPACK_BASETYPE_CONNECT_AUTHEN = 0x0005;	// 认证包，服务端发送给客户端
        public const ushort NETPACK_BASETYPE_APPLICATION_VER1 = 0x0040;	// 应用层数据描述第1版
        public const ushort NETPACK_BASETYPE_EXTENSION_BASE = 0x0100;	// 扩展类型，在此之前的类型保留给公共协议，之后可用于不应用的协议

        // 请求消息
        public const ushort NETPACK_BASETYPE_REQRES_BASE = (ushort)(NETPACK_BASETYPE_EXTENSION_BASE + 100);
        public const ushort NETPACK_BASETYPE_RES_ERROR = (ushort)(NETPACK_BASETYPE_REQRES_BASE + 0);		// 请求错误
        public const ushort NETPACK_BASETYPE_REQ_QUERYCONNECTION = (ushort)(NETPACK_BASETYPE_REQRES_BASE + 1);		// 请求查询连接信息
        public const ushort NETPACK_BASETYPE_RES_QUERYCONNECTION = (ushort)(NETPACK_BASETYPE_REQRES_BASE + 2);		// 应答查询连接信息
        public const ushort NETPACK_BASETYPE_REQ_ADDSUBSCRIBE = (ushort)(NETPACK_BASETYPE_REQRES_BASE + 3);		// 请求订阅消息
        public const ushort NETPACK_BASETYPE_RES_ADDSUBSCRIBE = (ushort)(NETPACK_BASETYPE_REQRES_BASE + 4);		// 应答订阅消息
        public const ushort NETPACK_BASETYPE_REQ_DELSUBSCRIBE = (ushort)(NETPACK_BASETYPE_REQRES_BASE + 5);		// 请求删除订阅
        public const ushort NETPACK_BASETYPE_RES_DELSUBSCRIBE = (ushort)(NETPACK_BASETYPE_REQRES_BASE + 6);		// 应答删除订阅
        public const ushort NETPACK_BASETYPE_REQ_CLEARSUBSCRIBE = (ushort)(NETPACK_BASETYPE_REQRES_BASE + 7);		// 请求清理所有订阅
        public const ushort NETPACK_BASETYPE_RES_CLEARSUBSCRIBE = (ushort)(NETPACK_BASETYPE_REQRES_BASE + 8);		// 应答清理所有订阅
        public const ushort NETPACK_BASETYPE_REQ_QUERYSUBSCRIBE = (ushort)(NETPACK_BASETYPE_REQRES_BASE + 9);		// 请求清理所有订阅
        public const ushort NETPACK_BASETYPE_RES_QUERYSUBSCRIBE = (ushort)(NETPACK_BASETYPE_REQRES_BASE + 10);		// 应答清理所有订阅
        #endregion

        // 数据格式
        static public byte NETPACK_BASEHEAD_VER1_FORMAT_BINARY = 0;		// 二进制数据
        static public byte NETPACK_BASEHEAD_VER1_FORMAT_TEXT_UTF8 = 1;		// TEXT UTF8
        static public byte NETPACK_BASEHEAD_VER1_FORMAT_TEXT_UNICODE = 2;		// TEXT UNICODE
        static public byte NETPACK_BASEHEAD_VER1_FORMAT_TEXT_ANSI = 3;		// TEXT ANSI
        static public byte NETPACK_BASEHEAD_VER1_FORMAT_XML = 4;		// XML
        static public byte NETPACK_BASEHEAD_VER1_FORMAT_JSON = 5;		// Json

        //包类型
        public const byte NETPACK_PACKTYPE_UNDEFINF = 0x00;		// 未定义
        public const byte NETPACK_PACKTYPE_APPLICATION = 0x03;		// 应用层数据包
        public const byte NETPACK_PACKTYPE_CONNECT = 0x01;		// 连接层数据包(握手等)
        public const byte NETPACK_PACKTYPE_HEARTBEAT = 0x02;		// 心跳包
        public const byte NETPACK_PACKTYPE_EXTENSION_BASE = 0x80;		// 扩展类型，在此之前的类型保留给公共协议，之后可用于不应用的协议
        public const byte NETPACK_PACKTYPE_UNIVERSALLY = (byte)(NETPACK_PACKTYPE_EXTENSION_BASE + 0);	// 通用消息
        public const byte NETPACK_PACKTYPE_REQUEST = (byte)(NETPACK_PACKTYPE_EXTENSION_BASE + 1);	// 请求消息，由客户端发送给服务端
        public const byte NETPACK_PACKTYPE_RESPONSE = (byte)(NETPACK_PACKTYPE_EXTENSION_BASE + 2);	// 应答消息，由服务端发送给客户端
        public const byte NETPACK_PACKTYPE_NOTIFY = (byte)(NETPACK_PACKTYPE_EXTENSION_BASE + 3);	// 通知消息

        // 可扩展数据定义
        public static ushort NETPACK_EXTTYPE_NOTHING = 0x0000;	// 无扩展信息
        public static ushort NETPACK_EXTTYPE_BIGDATA = 0x0001;	// 大数据包
        public static ushort NETPACK_EXTTYPE_EXTENSION_BASE = 0x0100;	// 扩展类型，在此之前的类型保留给公共协议，之后可用于不应用的协议

        //加密类型
        public const byte NETPACK_ENCRYPT_NOTHING = 0;		// 无加密
        public const byte NETPACK_ENCRYPT_AES_128_CBC = 1;		// AES 128位CBC模式加密算法
        public const byte NETPACK_ENCRYPT_AES_256_CBC = 2;		// AES 256位CBC模式加密算法

        //加密中用到的变量
        public static short AES_BLOCK_SIZE = 16;

        //长度定义
        public static int LEN_APPNAME = 32;
        public static int LEN_VERSION = 32;
        public static int LEN_ATHENTICATECODE = 32;
        public static int LEN_RECEIVE = 1024;
        public static int LEN_TAG_NETPACK_DISTINGUISHHEAD_VER1 = Marshal.SizeOf(typeof(_TAG_NETPACK_DISTINGUISHHEAD_VER1));
        public static int LEN_IPADDRESS = 48;

        #region 消息大类型定义Larg type
        // 消息大类型定义Larg type
        public const ushort DECMSG_LARGTYPE_I_VOC = 0x0001;	// 录音消息，向录音程序发送的消息
        public const ushort DECMSG_LARGTYPE_O_VOC = 0x8001;	// 录音消息，由录音程序发布的消息
        public const ushort DECMSG_LARGTYPE_I_SCR = 0x0002;	// 录屏消息，向录屏程序发送的消息
        public const ushort DECMSG_LARGTYPE_O_SCR = 0x8002;	// 录屏消息，由录屏程序发布的消息
        public const ushort DECMSG_LARGTYPE_I_ALM = 0x0003;	// 告警消息，向告警系统发送的消息
        public const ushort DECMSG_LARGTYPE_O_ALM = 0x8003;	// 告警消息，由告警系统发布的消息
        public const ushort DECMSG_LARGTYPE_I_CTI = 0x0004;	// CTI消息，向CTI程序发送的消息
        public const ushort DECMSG_LARGTYPE_O_CTI = 0x8004;	// CTI消息，由CTI程序发布的消息
        public const ushort DECMSG_LARGTYPE_I_CM = 0x0005;	// CM消息，向CM程序发送的消息
        public const ushort DECMSG_LARGTYPE_O_CM = 0x8005;	// CM消息，由CM程序发布的消息
        public const ushort DECMSG_LARGTYPE_I_WSDK = 0x0006;	// WEBSDK消息，向WEBSDK程序发送的消息
        public const ushort DECMSG_LARGTYPE_O_WSDK = 0x8006;	// WEBSDK消息，由WEBSDK程序发布的消息

        public const ushort NETMSG_LARGTYPE_COMMON = 0x0001;   // 公共消息
        public const ushort NETMSG_LARGTYPE_I_VOC = 0x0002;	// 录音消息，向录音程序发送的消息
        public const ushort NETMSG_LARGTYPE_O_VOC = 0x8002;	// 录音消息，由录音程序发布的消息 q
        public const ushort NETMSG_LARGTYPE_I_SCR = 0x0003;	// 录屏消息，向录屏程序发送的消息
        public const ushort NETMSG_LARGTYPE_O_SCR = 0x8003;	// 录屏消息，由录屏程序发布的消息
        public const ushort NETMSG_LARGTYPE_I_ALM = 0x0004;	// 告警消息，向告警系统发送的消息
        public const ushort NETMSG_LARGTYPE_O_ALM = 0x8004;	// 告警消息，由告警系统发布的消息
        public const ushort NETMSG_LARGTYPE_I_CTI = 0x0005;	// CTI消息，向CTI程序发送的消息
        public const ushort NETMSG_LARGTYPE_O_CTI = 0x8005;	// CTI消息，由CTI程序发布的消息
        public const ushort NETMSG_LARGTYPE_I_CM = 0x0006;	// CM消息，向CM程序发送的消息
        public const ushort NETMSG_LARGTYPE_O_CM = 0x8006;	// CM消息，由CM程序发布的消息
        public const ushort NETMSG_LARGTYPE_I_WSDK = 0x0007;	// WEBSDK消息，向WEBSDK程序发送的消息
        public const ushort NETMSG_LARGTYPE_O_WSDK = 0x8007;	// WEBSDK消息，由WEBSDK程序发布的消息
        #endregion

        #region 录音消息定义Middle type
        // 录音消息，向录音程序发送的消息
        public const ushort DECMSG_MIDTYPE_I_VOC_CONTROL = 0x0001;   // 控制指令(启动、停止、登录、登出等)
        public const ushort DECMSG_MIDTYPE_I_VOC_GETSTATE = 0x0002;   // 获取状态指令(服务状态、通道状态等)
        // 录音消息，由录音程序发布的消息
        public const ushort DECMSG_MIDTYPE_O_VOC_STATE = 0x0001;   // 状态消息(服务状态、通道状态等)

        // 录音消息，向录音程序发送的消息
        public const ushort NETMSG_MIDTYPE_COMMON_PARAM = 0x0001;   // 参数消息

        #endregion

        #region  录音消息定义Small type
        // 录音消息，向录音程序发送的消息
        // 控制指令
        public const ushort DECMSG_MIDTYPE_I_VOC_CTRL_SERVICE = 0x0001;   // 服务控制指令
        public const ushort DECMSG_MIDTYPE_I_VOC_CTRL_CHANNEL = 0x0002;   // 通道控制指令
        // 获取状态指令
        public const ushort DECMSG_MIDTYPE_I_VOC_GETSTA_SERVICE = 0x0001;   // 获取服务状态
        public const ushort DECMSG_MIDTYPE_I_VOC_GETSTA_CHANNEL = 0x0002;   // 获取通道状态

        // 录音消息，由录音程序发布的消息
        // 状态消息
        public const ushort DECMSG_MIDTYPE_O_VOC_STATE_SERVICE = 0x0001;   // 服务状态消息
        public const ushort DECMSG_MIDTYPE_O_VOC_STATE_CHANNEL = 0x0002;   // 通过状态消息

        // 录音消息，向录音程序发送的消息
        public const ushort NETMSG_SMALLTYPE_COMMON_PARAM_NOTIFY = 0x0001;   // 参数通知
        public const ushort NETMSG_NUMBER_COMMON_PARAM_NOTIFY_CHANGE = 0x0001;
        #endregion

        #region 录音消息定义number
        // 录音消息，向录音程序发送的消息
        // 服务控制指令
        public const ushort DECMSG_NUMBER_I_VOC_CTRL_SRV_START = 0x0001;   // 启动
        public const ushort DECMSG_NUMBER_I_VOC_CTRL_SRV_STOP = 0x0002;   // 停止
        // 通道控制指令
        public const ushort DECMSG_NUMBER_I_VOC_CTRL_CH_CAPTURE = 0x0001;   // 获取控制权
        public const ushort DECMSG_NUMBER_I_VOC_CTRL_CH_RELEASE = 0x0002;   // 释放控制权
        public const ushort DECMSG_NUMBER_I_VOC_CTRL_CH_START = 0x0003;   // 启动录音
        public const ushort DECMSG_NUMBER_I_VOC_CTRL_CH_STOP = 0x0004;   // 停止录音
        public const ushort DECMSG_NUMBER_I_VOC_CTRL_CH_PAUSE = 0x0005;   // 暂停录音
        public const ushort DECMSG_NUMBER_I_VOC_CTRL_CH_RESUME = 0x0006;   // 恢复录音
        public const ushort DECMSG_NUMBER_I_VOC_CTRL_CH_LOGGEDON = 0x0007;   // 坐席登录
        public const ushort DECMSG_NUMBER_I_VOC_CTRL_CH_LOGGEDOFF = 0x0008;   // 坐席登出
        public const ushort DECMSG_NUMBER_I_VOC_CTRL_CH_UPDATE = 0x0009;   // 更新呼叫信息
        // 获取服务状态指令
        const ushort DECMSG_NUMBER_I_VOC_GETSTA_SRV = 0x0001;   // 获取服务状态与信息
        // 获取通道状态指令
        const ushort DECMSG_NUMBER_I_VOC_GETSTA_CH_ALL = 0x0002;   // 获取所有通道状态与信息
        const ushort DECMSG_NUMBER_I_VOC_GETSTA_CH_SPECIFY = 0x0002;   // 获取指定通道状态与信息

        // 录音消息，由录音程序发布的消息
        // 服务状态消息
        const ushort DECMSG_NUMBER_O_VOC_STA_SRV_STARTED = 0x0001;   // 服务已启动
        const ushort DECMSG_NUMBER_O_VOC_STA_SRV_STOPED = 0x0002;   // 服务已停止
        const ushort DECMSG_NUMBER_O_VOC_STA_SRV_HEARBEAT = 0x0003;   // 心跳消息

        // 通道状态消息
        const ushort DECMSG_NUMBER_O_VOC_STA_CH_CTRLMODE = 0x0001;   // 控制模式
        const ushort DECMSG_NUMBER_O_VOC_STA_CH_STOPED = 0x0002;   // 停止录音
        const ushort DECMSG_NUMBER_O_VOC_STA_CH_STARTED = 0x0003;   // 启动录音
        const ushort DECMSG_NUMBER_O_VOC_STA_CH_PAUSED = 0x0004;   // 暂停录音
        const ushort DECMSG_NUMBER_O_VOC_STA_CH_RESUMED = 0x0005;   // 恢复录音
        const ushort DECMSG_NUMBER_O_VOC_STA_CH_LOGGEDON = 0x0006;   // 坐席登录
        const ushort DECMSG_NUMBER_O_VOC_STA_CH_LOGGEDOFF = 0x0007;   // 坐席登出
        const ushort DECMSG_NUMBER_O_VOC_STA_CH_UPDATE = 0x0008;   // 更新呼叫信息
        const ushort DECMSG_NUMBER_O_VOC_STA_CH_MONITOR = 0x0008;   // 被监听
        const ushort DECMSG_NUMBER_O_VOC_STA_CH_UNMONITOR = 0x0009;   // 取消监听

     
        #endregion
    }
    #region 结构体
    public struct _TAG_NETPACK_DISTINGUISHHEAD_VER1
    {
        // 0-3, 4 bytes
        public _TAG_NETPACK_DISTINGUISHHEAD _dist;				// 识别头
        // 4-5, 2 bytes
        public byte _sequence;			            // 序号,0-255循环，保证数据包顺序(UDP时使用,或者非点对点时使用)
        public byte _packtype;			            // 包类型，区分通讯协议还是应用层消息包
        // 6-7, 2 bytes
        public ushort _followsize;		            // 本完整数据包除去包识别头后续数据大小
        // 8-15, 8 bytes
        public Int64 _source;			            // 发送源(global id)
        // 16-23, 8 bytes
        public Int64 _target;			            // 接收者(global id)，如果是0xffffffffffffffff则为广播包
        // 24-31, 8 bytes
        public Int64 _timestamp;			        // 时间戳，UTC时间(高精度的time_t，精度为100ns)，从1970.1.1 0:00:00
        // 32-35, 4 bytes
        public ushort _basehead;			        // 基本头类型标志, 0-无
        public ushort _basesize;			        // 基本头大小
        // 36-39, 4 bytes
        public ushort _exthead;			            // 扩展头类型标志，0-无
        public ushort _extsize;			            // 扩展头大小
        // 40-43, 4 bytes
        public ushort _datasize;                    //数据区大小
        public ushort _state;				        // 状态标志，不使用保持0
        // 44-47, 4 bytes
        public ushort _moduleid;			        // 模块ID
        public ushort _number;			            // 模块编号
        // 48-63, 16 bytes
        public _TAG_NETPACK_MESSAGE _message;			            // 消息id，可用于消息订阅，通常情况_packtype应为NETPACK_PACKTYPE_USER
    }

    /// <summary>
    /// 识别头
    /// </summary>
    public struct _TAG_NETPACK_DISTINGUISHHEAD
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] _flag;			// 包头识别标识
        public byte _version;			// 版本
        public byte _cbsize;			// 头大小
    }

    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    public struct _TAG_NETPACK_MESSAGE
    {
        [FieldOffset(0)]
        public Int64 _adressLong;
        [FieldOffset(0)]
        public Adress _adress;
        [FieldOffset(8)]
        public Int64 _identifyLong;
        [FieldOffset(8)]
        public identify identify;
    }

    public struct Adress
    {
        public MesTarget target;
        public MesSource source;
    }

    public struct identify
    {
        public ushort _number;	                    // 消息编号
        public ushort _small;	                    // 消息小类型
        public ushort _middle;	                    // 消息中类型
        public ushort _large;                        //消息大类型
    }

    public struct MesTarget
    {
        public short _number;	                    // 目标编号。不确定编号，则使用0xffff填充
        public short _module;	                    // 目标模块
    };

    public struct MesSource
    {
        public short _number;	                    // 源编号
        public short _module;	                    // 源模块
    };

    // 加密上下文
    public class _TAG_NETPACK_ENCRYPT_CONTEXT
    {
        public byte _encrypt;			// 加密算法
        public string _key;				// 加密密钥
        public short _keylength;			// 加密密钥长度
        public string _ivec;				// 加密向量
        public short _iveclength;		// 加密向量长度
    }

    /// <summary>
    /// 基本包头(NETPACK_BASETYPE_CONNECT_ERROR)
    /// </summary>
    struct _TAG_NETPACK_BASEHEAD_ERROR
    {
        int _error_code;					// 错误码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        char[] _description;				// 描述
    }

    /// <summary>
    /// 客户端发给服务端的hello
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct _TAG_NETPACK_BASEHEAD_CONNECT_HELLO
    {
        public Int64 _client_globalid;				// global id，一般应用由服务端生成，通知客户端，如果是客户端是DEC服务则由客户端生成
        public ushort _client_moduleid;				// 客户端的模块ID
        public ushort _client_number;				    // 客户端模块编号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] _client_appname;	            // 客户端应用名
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] _client_version;	            // 客户端程序版本
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public UInt64[] _client_protocol;                  // 客户端可使用的协议版本
        public type_datetime_tmp _client_starttime;        // 客户端程序启动时间，UTC。以客户端机器时间为准
        public type_datetime_tmp _client_connecttime;      // 客户端网络连接时间，UTC。以客户端机器时间为准
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct _TAG_NETPACK_BASEHEAD_CONNECT_WELCOME
    {
        /// <summary>
        ///  客户端global id，一般应用由服务端生成，通知客户端，如果是客户端是DEC服务则由客户端生成
        /// </summary>
        public Int64 _client_endpointid;
        /// <summary>
        /// 服务端的模块ID
        /// </summary>
        public Int64 _server_endpointid;

        /// <summary>
        /// 服务端的模块ID
        /// </summary>
        public ushort _server_moduletypeid;

        /// <summary>
        /// 服务端模块编号
        /// </summary>
        public short _server_modulenumber;

        /// <summary>
        /// 服务端应用名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] _server_appname;

        /// <summary>
        /// 服务端程序版本
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] _server_appver;

        /// <summary>
        /// 确定的协议版本
        /// </summary>
        public UInt64 _sure_protocol;

        /// <summary>
        /// 服务端程序启动时间，UTC。以客户端机器时间为准
        /// </summary>
        public type_datetime_tmp _server_starttime;

        /// <summary>
        /// 服务端程序启动时间，UTC。以客户端机器时间为准
        /// </summary>
        public type_datetime_tmp _server_connecttime;

        /// <summary>
        /// 认证码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] _authenticatecode;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct _TAG_NETPACK_BASEHEAD_CONNECT_LOGON
    {
        public int _heartbeat;						// 心跳间隔时间

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] _authenticatecode;	            // 认证码
    }

    // 基本包头(NETPACK_BASETYPE_CONNECT_AUTHEN)
    struct _TAG_NETPACK_BASETYPE_CONNECT_AUTHEN
    {
        public int _errorcode;						// 返回码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public char[] _authenticatecode;	// 认证码
    }

    /// <summary>
    /// 请求订阅消息（基本包头）
    /// </summary>
    public struct _TAG_NETPACK_BASEHEAD_REQ_ADDSUBSCRIBE
    {
        public int _requestid;					// 请求id
        public int _reserved;					// 保留，必须为0(目前为了保证数据对齐)
        public _TAG_NETPACK_MESSAGE _mask;						// 订阅消息掩码
        public _TAG_NETPACK_MESSAGE _message;					// 订阅消息id
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct type_datetime_tmp    // 暂时这么命名，便于找出使用该定义的代码
    {
        public UInt64 _time;						// 日期时间，自1601年1月1日起，每隔100纳秒的数量，0表示无效
        public int _bias;						// 时间偏移
    };

    //基本包头
    public struct NETPACK_BASEHEAD_APPLICATION_VER1
    {
        // 0-4, 4 bytes
        public char _channel;			// 数据通道，可以在同一连接中传输多个通道数据，默认0
        public byte _encrypt;			// 数据加密算法，0-未加密，>0 为加密算法
        public char _compress;			// 数据压缩算法，0-未解压缩，>0 为压缩算法
        public byte _format;			// 数据格式

        // 4-5, 2 bytes
        public ushort _codepage;          // 数据_format等数据为ANSI字符是有效
        // 6-7, 2 bytes
        public ushort _indentify;         // 数据识别号，用于识别不同的数据，方便程序快速获取数据，非必选数据
        // 8-11, 4 bytes
        public short _datasize;			// 本次传输的数据大小，如果数据被加密，则为扩展后的数据大小
        public short _validsize;			// 有效数据长度，如果数据被加密，则保存数据的加密前长度，如果数据不加密，则同_datasize
        // 12-31, 20 bytes
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] _reserved;        // 保留,必须清空为0
    }
    #endregion
}
