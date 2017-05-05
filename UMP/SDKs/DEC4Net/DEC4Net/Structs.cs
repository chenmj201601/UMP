//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6447b4e7-4a1b-475a-b697-5a2ac8e5f531
//        CLR Version:              4.0.30319.18063
//        Name:                     Structs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.DEC
//        File Name:                Structs
//
//        created by Charley at 2015/6/15 11:22:43
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.InteropServices;

namespace VoiceCyber.SDKs.DEC
{

    /// <summary>
    /// 识别头
    /// </summary>
    struct _TAG_NETPACK_DISTINGUISHHEAD
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public char[] _flag;			// 包头识别标识
        public byte _version;			// 版本
        public byte _cbsize;			// 头大小
    }

    struct _TAG_NETPACK_DISTINGUISHHEAD_VER1
    {
        // 0-3, 4 bytes
        public _TAG_NETPACK_DISTINGUISHHEAD _dist;				// 识别头
        // 4-5, 2 bytes
        public byte _sequence;			            // 序号,0-255循环，保证数据包顺序(UDP时使用,或者非点对点时使用)
        public byte _packtype;			            // 包类型，区分通讯协议还是应用层消息包
        // 6-7, 2 bytes
        public ushort _followsize;		            // 本完整数据包除去包识别头后续数据大小
        // 8-15, 8 bytes
        public ulong _source;			            // 发送源(global id)
        // 16-23, 8 bytes
        public ulong _target;			            // 接收者(global id)，如果是0xffffffffffffffff则为广播包
        // 24-31, 8 bytes
        public ulong _timestamp;			        // 时间戳，UTC时间(高精度的time_t，精度为100ns)，从1970.1.1 0:00:00
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
        public ushort _moduleid;			        // 消息源模块ID
        public ushort _number;			            // 消息源模块编号
        // 48-63, 16 bytes
        public _TAG_NETPACK_MESSAGE _message;			            // 消息id，可用于消息订阅，通常情况_packtype应为NETPACK_PACKTYPE_USER
    }

    /// <summary>
    /// App Ver1的基本包头
    /// </summary>
    struct NETPACK_BASEHEAD_APPLICATION_VER1
    {
        // 0-4, 4 bytes
        public byte _channel;			// 数据通道，可以在同一连接中传输多个通道数据，默认0
        public byte _encrypt;			// 数据加密算法，0-未加密，>0 为加密算法
        public byte _compress;			// 数据压缩算法，0-未解压缩，>0 为压缩算法
        public byte _format;			// 数据格式
        // 5-6, 2 bytes
        public ushort _codepage;        // 数据_format等数据为ANSI字符是有效
        // 7-8, 2 bytes
        public ushort _identify;        // 数据识别号，用于识别不同的数据，方便程序快速获取数据，非必选数据
        // 9-10, 2 bytes
        public ushort _datasize;			// 本次传输的数据大小，如果数据被加密，则为扩展后的数据大小
        // 11-12, 2 bytes
        public ushort _validsize;			// 有效数据长度，如果数据被加密，则保存数据的加密前长度，如果数据不加密，则同_datasize
        // 13-32, 20 bytes
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] _reserved;		// 保留,必须清空为0
    }

    /// <summary>
    /// 错误消息的基本包头(NETPACK_BASETYPE_CONNECT_ERROR)
    /// </summary>
    struct _TAG_NETPACK_BASEHEAD_ERROR
    {
        public int _error_code;					// 错误码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        public byte[] _description;				// 描述
    }

    /// <summary>
    /// 客户端发给服务端的hello
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct _TAG_NETPACK_BASEHEAD_CONNECT_HELLO
    {
        public ulong _client_globalid;				// global id，一般应用由服务端生成，通知客户端，如果是客户端是DEC服务则由客户端生成
        public ushort _client_moduleid;				// 客户端的模块ID
        public ushort _client_number;				    // 客户端模块编号
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] _client_appname;	            // 客户端应用名
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] _client_version;	            // 客户端程序版本
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ulong[] _client_protocol;                  // 客户端可使用的协议版本
        public type_datetime _client_starttime;        // 客户端程序启动时间，UTC。以客户端机器时间为准
        public type_datetime _client_connecttime;      // 客户端网络连接时间，UTC。以客户端机器时间为准
    }
    /// <summary>
    /// 欢迎消息
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct _TAG_NETPACK_BASEHEAD_CONNECT_WELCOME
    {
        /// <summary>
        ///  客户端global id，一般应用由服务端生成，通知客户端，如果是客户端是DEC服务则由客户端生成
        /// </summary>
        public ulong _client_endpointid;
        /// <summary>
        /// 服务端的模块ID
        /// </summary>
        public ulong _server_endpointid;

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
        public byte[] _server_appname;

        /// <summary>
        /// 服务端程序版本
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] _server_appver;

        /// <summary>
        /// 确定的协议版本
        /// </summary>
        public ulong _sure_protocol;

        /// <summary>
        /// 服务端程序启动时间，UTC。以客户端机器时间为准
        /// </summary>
        public type_datetime _server_starttime;

        /// <summary>
        /// 服务端程序启动时间，UTC。以客户端机器时间为准
        /// </summary>
        public type_datetime _server_connecttime;

        /// <summary>
        /// 认证码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] _authenticatecode;
    }
    /// <summary>
    /// 登录消息
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    struct _TAG_NETPACK_BASEHEAD_CONNECT_LOGON
    {
        public int _heartbeat;						// 心跳间隔时间

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] _authenticatecode;	            // 认证码
    }
    /// <summary>
    /// 验证消息
    /// </summary>
    struct _TAG_NETPACK_BASEHEAD_CONNECT_AUTHEN
    {
        public int _errorcode;						// 返回码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] _authenticatecode;	// 认证码

        public byte _encrypt;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] _reserved1;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] _reserved2;
    }

    /// <summary>
    /// 请求订阅消息（基本包头）
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    struct _TAG_NETPACK_BASEHEAD_REQ_ADD_SUBSCRIBE
    {
        public int _requestid;					// 请求id
        public int _reserved;					// 保留，必须为0(目前为了保证数据对齐)
        public _TAG_NETPACK_MESSAGE _mask;						// 订阅消息掩码
        public _TAG_NETPACK_MESSAGE _message;					// 订阅消息id
    }

    /// <summary>
    /// 订阅回复
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    struct _TAG_NETPACK_BASEHEAD_RES_ADD_SUBSCRIBE
    {
        public int _requestid;					// 请求id
        public int _errorcode;					// 返回码
        public _TAG_NETPACK_MESSAGE _mask;						// 订阅消息掩码
        public _TAG_NETPACK_MESSAGE _message;					// 订阅消息id
        public uint _currentsize;				// 当前总的订阅数量
    }
    /// <summary>
    /// 退订消息请求
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    struct _TAG_NETPACK_BASEHEAD_REQ_DEL_SUBSCRIBE
    {
        public int _requestid;                 //请求id
        public int _reserved;                   //保留，必须为0（目前为了保证数据对齐）
        public _TAG_NETPACK_MESSAGE _mask;      //订阅消息掩码
        public _TAG_NETPACK_MESSAGE _message;   //订阅消息id
    }
    /// <summary>
    /// 退订消息回复
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    struct _TAG_NETPACK_BASEHEAD_RES_DEL_SUBSCRIBE
    {
        public int _requestid;
        public int _errorcode;
        public uint _currentsize;               //当前总的订阅数量
    }
    /// <summary>
    /// 退订所有消息请求
    /// </summary>
    struct _TAG_NETPACK_BASEHEAD_REQ_CLEAR_SUBSCRIBE
    {
        public int _requestid;
    }
    /// <summary>
    /// 退订所有消息回复
    /// </summary>
    struct _TAG_NETPACK_BASEHEAD_RES_CLEAR_SUBSCRIBE    //退订所有消息回复
    {
        public int _requestid;
        public int _errorcode;
    }

    /// <summary>
    /// 通用错误包头
    /// 基本包头(NETPACK_BASETYPE_UNIVERSALLY_ERROR)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    struct _TAG_NETPACK_BASEHEAD_UNIVERSALLY_ERROR
    {
        public int _errorcode;					// 返回码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] _description;			// 描述
    }
    /// <summary>
    /// 消息（掩）码
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    struct _TAG_NETPACK_MESSAGE
    {
        [FieldOffset(0)]
        public ushort _targetNumber;
        [FieldOffset(2)]
        public ushort _targetModule;
        [FieldOffset(4)]
        public ushort _sourceNumber;
        [FieldOffset(6)]
        public ushort _sourceModule;
        [FieldOffset(8)]
        public ushort _number;
        [FieldOffset(10)]
        public ushort _samllType;
        [FieldOffset(12)]
        public ushort _middleType;
        [FieldOffset(14)]
        public ushort _largeType;
    }

    /// <summary>
    /// 加密上下文
    /// </summary>
    class _TAG_NETPACK_ENCRYPT_CONTEXT
    {
        public byte _encrypt;			// 加密算法
        public string _key;				// 加密密钥
        public short _keylength;			// 加密密钥长度
        public string _ivec;				// 加密向量
        public short _iveclength;		// 加密向量长度
    }

    /// <summary>
    /// 日期时间
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    struct type_datetime    // 暂时这么命名，便于找出使用该定义的代码
    {
        public ulong _time;						// 日期时间，自1601年1月1日起，每隔100纳秒的数量，0表示无效
        public int _bias;						// 时间偏移
    };
}
