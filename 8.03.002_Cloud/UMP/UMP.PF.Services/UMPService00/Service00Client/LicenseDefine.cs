using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace UMPService00
{
    public class LicenseDefine
    {
        public const int NET_BUFFER_SIZE = 1024000;         //接收缓冲区大小
        public const int NET_HEAD_SIZE = 16;                //消息头部大小


        // 消息类型
        public const int LICENSE_MSG_CLASS_UNKNOWN = 0;	// 未知
        public const int LICENSE_MSG_CLASS_EXCEPTION = 1;	// 发生异常
        public const int LICENSE_MSG_CLASS_CONNECTION = 2;	// 链接与握手消息
        public const int LICENSE_MSG_CLASS_AUTHENTICATE = 3;	// 认证
        public const int LICENSE_MSG_CLASS_NOTIFY = 4;	// 通知，仅服务端向客户端发送
        public const int LICENSE_MSG_CLASS_REQRES = 5;	// 请求应答

        //默认设置
        public const int NET_HEARTBEAT_INTEVAL = 30;        //心跳间隔时间，秒
        public const string NET_PROTOCOL_VERSION = "2.00";


        // 关键字
        public const string KEYWORD_MSG_SESSION = "Session";	// Session
        public const string KEYWORD_MSG_CLASSDESC = "ClassDesc";	// 消息类别
        public const string KEYWORD_MSG_MESSAGEID = "MessageID";	// 消息
        public const string KEYWORD_MSG_MESSAGEDESC = "MessageDesc";	// 消息
        public const string KEYWORD_MSG_PRODUCT = "product";	// 产品
        public const string KEYWORD_MSG_MODULE = "Module";	// 模块
        public const string KEYWORD_MSG_VERSION = "Version";	// 版本
        public const string KEYWORD_MSG_PROTOCOL = "Protocol";	// 协议版本
        public const string KEYWORD_MSG_PRIORITY = "priority";	// 优先级
        public const string KEYWORD_MSG_VERIFICATION = "Verification";	// 校验码
        public const string KEYWORD_MSG_CLASSID = "ClassID";	// 消息类别
        public const string KEYWORD_MSG_REQUESTID = "RequestID";	// 请求id
        public const string KEYWORD_MSG_RESPONSEID = "ResponseID";	// 应答id,对应requestid
        public const string KEYWORD_MSG_RESPONSE_RESULT = "ResponseResult";	// 应答结果
        public const string KEYWORD_MSG_DATA = "Data";	// 数据
        public const string KEYWORD_MSG_HEARTBEAT = "Heartbeat";	// heartbeat
        public const string KEYWORD_MSG_CURRENTTIME = "CurrentTime";	// 当前时间
        public const string KEYWORD_MSG_THISTIME = "ThisTime";	// 本次
        public const string KEYWORD_MSG_DETAIL = "Detail";	// 详情
        public const string KEYWORD_MSG_EXPIRATION = "Expiration";	// 有效期
        public const string KEYWORD_MSG_MONOPOLIZE_LICENSE = "Monopolize";	// 独占的License
        public const string KEYWORD_MSG_SHARE_LICENSE = "Share";	// 共享的License
        public const string KEYWORD_MSG_ALL = "All";	// 客户端总共获取的License
        public const string KEYWORD_MSG_FREE = "Free";	// 剩余的License
        public const string KEYWORD_MSG_TOTAL = "Total";	// 合计的license
        public const string KEYWORD_MSG_CLIENT_NAME = "ClientName";	// client name
        public const string KEYWORD_MSG_CLIENT_TYPE = "ClientType";	// client type
        public const string KEYWORD_MSG_HOST = "Host";	// 主机地址
        public const string KEYWORD_MSG_PORT = "Port";	// port
        public const string KEYWORD_MSG_TYPE = "Type";	// 类型
        public const string KEYWORD_MSG_USERNAME = "Username";	// 用户名
        public const string KEYWORD_MSG_PASSWORD = "password";	// 密码
        public const string KEYWORD_MSG_CONNECT_TIME = "ConnectTime";	// connect time
        public const string KEYWORD_MSG_LICENSE = "Licenses";	// license
        public const string KEYWORD_MSG_LICENSE_TYPE = "LicenseType";	// license type
        public const string KEYWORD_MSG_LICENSE_POOL = "LicensePool";	// license pool
        public const string KEYWORD_MSG_SOFTDOG_TYPE = "SoftdogType";	// 软件狗类型
        public const string KEYWORD_MSG_SERIALNUMBER = "SerialNumber";	// 软件狗序列号
        public const string KEYWORD_MSG_CLIENT_INFO = "ClientInfo";	// 客户端信息
        public const string KEYWORD_MSG_SOFTDOG = "Softdog";	// 软件狗信息
        public const string KEYWORD_MSG_SOFTDOGS = "Softdogs";	// 软件狗
        public const string KEYWORD_MSG_CURRENT_SOFTDOG = "CurrentSoftdog";	// 当前使用的软件狗
        public const string KEYWORD_MSG_LICENSESERVERS = "LicenseServer";	// License服务器
        public const string KEYWORD_MSG_HOLD_STATE = "HoldState";	// 挂起状态
        public const string KEYWORD_MSG_BEGIN_TIME = "BeginTime";	// 开始时间
        public const string KEYWORD_MSG_TOTAL_TIMELENGTH = "TotalTimeLength";	// 时长
        public const string KEYWORD_MSG_ELAPSE_TIMELENGTH = "ElapseTimeLength";	// 已流逝时间
        public const string KEYWORD_MSG_REMAIN_TIMELENGTH = "RemaintimeLength";// 剩余时间
        public const string KEYWORD_MSG_DURATION = "Duration";// 可用持续时间
        public const string KEYWORD_MSG_VALIDITY = "Validity";	// 有效性
        public const string KEYWORD_MSG_BEGIN_TICKCOUNT = "BeginTickcount";	// 开始时的计数
        public const string KEYWORD_MSG_HOLD_SOFTDOG = "HoldSoftdog";	// 挂起软件狗
        public const string KEYWORD_MSG_MODULENAME = "ModuleName";      //模块名
        public const string KEYWORD_MSG_MODULENUMBER = "ModuleNumber";		// 模块编号
        public const string KEYWORD_MSG_MODULETYPEID = "ModuleTypeID";  //模块类型ID
        public const string KEYWORD_MSG_LICENSE_DISPLAY = "Display";    //显示名称
        public const string KEYWORD_MSG_LICENSE_LICENSEID = "LicenseID";    //LicenseID
        public const string KEYWORD_MSG_SOFTDOG_MODULEMARJORTYPEID = "ModuleMajorTypeID";   //模块主类型编号
        public const string KEYWORD_MSG_LICENSE_MODULEMINJORTYPEID = "ModuleMinortypeID";   //模块子类型编号
        public const string KEYWORD_MSG_LICENSE_OWNERTYPE = "OwnerType";    //icense owner type, 独占、共享
        public const string KEYWORD_MSG_LICENSE_VALUE = "Value";    //license 值
        public const string KEYWORD_MSG_LICENSE_VALUETYPE = "ValueType";


    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct NetPacketHeader
    {
        /// char[2]
        /// 包同步标志(LM)
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Flag;

        /// unsigned short
        /// 状态标志，右起
        /// 0   bit         是否最后一个数据包
        /// 1   bit         是否有后续数据包
        /// 2   bit         本数据包是否被加密处理       
        public ushort State;

        /// unsigned char
        ///  数据包格式
        public byte Format;

        /// unsigned char[3]
        /// 保留1
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Reserved1;

        /// size_t->unsigned int
        /// 保留2
        public uint Reserved2;

        /// size_t->unsigned int
        /// 数据包大小
        public uint Size;

    }
}
