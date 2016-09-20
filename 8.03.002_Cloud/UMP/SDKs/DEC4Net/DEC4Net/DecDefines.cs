//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b83c65bd-3ff8-4561-8220-abef76dca0e7
//        CLR Version:              4.0.30319.18063
//        Name:                     DecDefines
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.DEC
//        File Name:                DecDefines
//
//        created by Charley at 2015/6/15 11:18:42
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.SDKs.DEC
{
    public class DecDefines
    {
        #region 网络数据包头
        /// <summary>
        /// 标识
        /// </summary>
        internal static char[] NETPACK_FLAG = { 'D', 'C' };

        //version
        internal static byte NETPACK_DISTHEAD_VER1 = 1;

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
        public const ushort NETPACK_BASETYPE_REQRES_BASE = NETPACK_BASETYPE_EXTENSION_BASE + 100;
        public const ushort NETPACK_BASETYPE_RES_ERROR = NETPACK_BASETYPE_REQRES_BASE + 0;		// 请求错误
        public const ushort NETPACK_BASETYPE_REQ_QUERYCONNECTION = NETPACK_BASETYPE_REQRES_BASE + 1;		// 请求查询连接信息
        public const ushort NETPACK_BASETYPE_RES_QUERYCONNECTION = NETPACK_BASETYPE_REQRES_BASE + 2;		// 应答查询连接信息
        public const ushort NETPACK_BASETYPE_REQ_ADDSUBSCRIBE = NETPACK_BASETYPE_REQRES_BASE + 3;		// 请求订阅消息
        public const ushort NETPACK_BASETYPE_RES_ADDSUBSCRIBE = NETPACK_BASETYPE_REQRES_BASE + 4;		// 应答订阅消息
        public const ushort NETPACK_BASETYPE_REQ_DELSUBSCRIBE = NETPACK_BASETYPE_REQRES_BASE + 5;		// 请求删除订阅
        public const ushort NETPACK_BASETYPE_RES_DELSUBSCRIBE = NETPACK_BASETYPE_REQRES_BASE + 6;		// 应答删除订阅
        public const ushort NETPACK_BASETYPE_REQ_CLEARSUBSCRIBE = NETPACK_BASETYPE_REQRES_BASE + 7;		// 请求清理所有订阅
        public const ushort NETPACK_BASETYPE_RES_CLEARSUBSCRIBE = NETPACK_BASETYPE_REQRES_BASE + 8;		// 应答清理所有订阅
        public const ushort NETPACK_BASETYPE_REQ_QUERYSUBSCRIBE = NETPACK_BASETYPE_REQRES_BASE + 9;		// 请求清理所有订阅
        public const ushort NETPACK_BASETYPE_RES_QUERYSUBSCRIBE = NETPACK_BASETYPE_REQRES_BASE + 10;		// 应答清理所有订阅
        #endregion

        // 数据格式
        public const byte NETPACK_DATA_FORMAT_UNDEFINE = 0;        //未定义
        public const byte NETPACK_BASEHEAD_VER1_FORMAT_BINARY = 1;		// 二进制数据
        public const byte NETPACK_BASEHEAD_VER1_FORMAT_TEXT_UTF8 = 2;		// TEXT UTF8
        public const byte NETPACK_BASEHEAD_VER1_FORMAT_TEXT_UNICODE = 3;		// TEXT UNICODE
        public const byte NETPACK_BASEHEAD_VER1_FORMAT_TEXT_ANSI = 4;		// TEXT ANSI
        public const byte NETPACK_BASEHEAD_VER1_FORMAT_XML = 5;		// XML
        public const byte NETPACK_BASEHEAD_VER1_FORMAT_JSON = 6;		// Json

        //包类型
        public const byte NETPACK_PACKTYPE_UNDEFINF = 0x00;		// 未定义
        public const byte NETPACK_PACKTYPE_APPLICATION = 0x03;		// 应用层数据包
        public const byte NETPACK_PACKTYPE_CONNECT = 0x01;		// 连接层数据包(握手等)
        public const byte NETPACK_PACKTYPE_HEARTBEAT = 0x02;		// 心跳包
        public const byte NETPACK_PACKTYPE_EXTENSION_BASE = 0x80;		// 扩展类型，在此之前的类型保留给公共协议，之后可用于不应用的协议
        public const byte NETPACK_PACKTYPE_UNIVERSALLY = NETPACK_PACKTYPE_EXTENSION_BASE + 0;	// 通用消息
        public const byte NETPACK_PACKTYPE_REQUEST = NETPACK_PACKTYPE_EXTENSION_BASE + 1;	// 请求消息，由客户端发送给服务端
        public const byte NETPACK_PACKTYPE_RESPONSE = NETPACK_PACKTYPE_EXTENSION_BASE + 2;	// 应答消息，由服务端发送给客户端
        public const byte NETPACK_PACKTYPE_NOTIFY = NETPACK_PACKTYPE_EXTENSION_BASE + 3;	// 通知消息

        // 可扩展数据定义
        public const ushort NETPACK_EXTTYPE_NOTHING = 0x0000;	// 无扩展信息
        public const ushort NETPACK_EXTTYPE_BIGDATA = 0x0001;	// 大数据包
        public const ushort NETPACK_EXTTYPE_EXTENSION_BASE = 0x0100;	// 扩展类型，在此之前的类型保留给公共协议，之后可用于不应用的协议

        //加密类型
        public const byte NETPACK_ENCRYPT_NOTHING = 0;		// 无加密
        public const byte NETPACK_ENCRYPT_AES_128_CBC = 1;		// AES 128位CBC模式加密算法
        public const byte NETPACK_ENCRYPT_AES_256_CBC = 2;		// AES 256位CBC模式加密算法

        //加密中用到的变量
        public const short AES_BLOCK_SIZE = 16;

        //加密中用到的盐
        public const uint NETPACK_ENCRYPT_SLAT = 2023116304;

        //长度定义
        public const int LEN_APPNAME = 32;
        public const int LEN_VERSION = 32;
        public const int LEN_ATHENTICATECODE = 32;
        public const int LEN_RECEIVE = 1024;
        public const int LEN_TAG_NETPACK_DISTINGUISHHEAD_VER1 = 64;
        public const int LEN_IPADDRESS = 48;

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
        public const ushort DECMSG_NUMBER_I_VOC_GETSTA_SRV = 0x0001;   // 获取服务状态与信息
        // 获取通道状态指令
        public const ushort DECMSG_NUMBER_I_VOC_GETSTA_CH_ALL = 0x0002;   // 获取所有通道状态与信息
        public const ushort DECMSG_NUMBER_I_VOC_GETSTA_CH_SPECIFY = 0x0002;   // 获取指定通道状态与信息

        // 录音消息，由录音程序发布的消息
        // 服务状态消息
        public const ushort DECMSG_NUMBER_O_VOC_STA_SRV_STARTED = 0x0001;   // 服务已启动
        public const ushort DECMSG_NUMBER_O_VOC_STA_SRV_STOPED = 0x0002;   // 服务已停止
        public const ushort DECMSG_NUMBER_O_VOC_STA_SRV_HEARBEAT = 0x0003;   // 心跳消息

        // 通道状态消息
        public const ushort DECMSG_NUMBER_O_VOC_STA_CH_CTRLMODE = 0x0001;   // 控制模式
        public const ushort DECMSG_NUMBER_O_VOC_STA_CH_STOPED = 0x0002;   // 停止录音
        public const ushort DECMSG_NUMBER_O_VOC_STA_CH_STARTED = 0x0003;   // 启动录音
        public const ushort DECMSG_NUMBER_O_VOC_STA_CH_PAUSED = 0x0004;   // 暂停录音
        public const ushort DECMSG_NUMBER_O_VOC_STA_CH_RESUMED = 0x0005;   // 恢复录音
        public const ushort DECMSG_NUMBER_O_VOC_STA_CH_LOGGEDON = 0x0006;   // 坐席登录
        public const ushort DECMSG_NUMBER_O_VOC_STA_CH_LOGGEDOFF = 0x0007;   // 坐席登出
        public const ushort DECMSG_NUMBER_O_VOC_STA_CH_UPDATE = 0x0008;   // 更新呼叫信息
        public const ushort DECMSG_NUMBER_O_VOC_STA_CH_MONITOR = 0x0008;   // 被监听
        public const ushort DECMSG_NUMBER_O_VOC_STA_CH_UNMONITOR = 0x0009;   // 取消监听

        #endregion
    }
}
