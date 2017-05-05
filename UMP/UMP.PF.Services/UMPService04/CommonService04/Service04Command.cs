//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e553e936-d816-4a7e-aff3-2d648e8935fb
//        CLR Version:              4.0.30319.18063
//        Name:                     Service04Command
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService04
//        File Name:                Service04Command
//
//        created by Charley at 2015/6/23 17:18:59
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService04
{
    /// <summary>
    /// 消息指令
    /// 一些公共的命令在RequestCode中统一定义（范围：40000 ~ 49999），
    /// 通常这里的命令应该跳过此范围，一般取（1000 ~ 9999）
    /// 这里一般分为：
    /// 请求消息：1000 ~ 1999
    /// 回复消息：2000 ~ 2999
    /// 通知消息：3000 ~ 3999
    /// </summary>
    public enum Service04Command
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,

        /// <summary>
        /// 设置监视方式(切换监视方式)
        /// 注意，一次会话同一时间只能是一种监视方式，但是一此次会话中可以随时切换监视方式
        /// 参数：
        /// ListData1：新的监控方式
        /// </summary>
        ReqSetMonType = 1000,
        /// <summary>
        /// 添加监视对象(可以是多个)
        /// 参数：
        /// ListData1：要添加的总个数
        /// ListData2...：要添加的监视对象列表
        /// 注意：1、如果监视的对象的监视类型与当前会话的监视类型不匹配，则忽略这个对象
        /// 2、如果这个对象在当前会话的监视列表中已经存在，则也忽略
        /// 3、添加的监视对象会生成一个MonID，作为唯一标识
        /// </summary>
        ReqAddMonObj = 1001,
        /// <summary>
        /// 移除监视对象(可以是多个）
        /// 参数：
        /// ListData1：移除的总个数
        /// ListData2...：要移除的监视对象MonID列表
        /// </summary>
        ReqRemoveMonObj = 1002,
        /// <summary>
        /// 清除所有监视对象
        /// 参数：无
        /// </summary>
        ReqClearMonObj = 1003,
        /// <summary>
        /// 添加监视对象并查询对象所在的通道信息（可以是多个）
        /// 参数：
        /// ListData1：要添加的总个数
        /// ListData2：要添加的监视对象列表
        /// 注意：此命令是AddMonObj+QueryChan的组合
        /// </summary>
        ReqAddQueryChan=1004,
        /// <summary>
        /// 查询监视对象所属通道信息（可以是多个）
        /// 参数：
        /// ListData1：要查询的对象个数（必须是已经添加了的对象）
        /// ListData2...：监视对象的MonID列表
        /// </summary>
        ReqQueryChan = 1010,
        /// <summary>
        /// 查询指定监视对象的当前状态
        /// 参数：
        /// ListData1：要查询的监视对象的MonID
        /// </summary>
        ReqQueryState = 1011,
        /// <summary>
        /// 开始网络监听
        /// 参数：
        /// ListData1：要监听的对象的MonID（必须已经将此对象添加）
        /// ListData2：监听端口
        /// ListData3：是否使用Service04中转音频数据（1，中转；0，不中转）
        /// </summary>
        ReqStartNMon = 1020,
        /// <summary>
        /// 停止网络监听
        /// 参数：
        /// ListData1：要停止监听的对象的MonID
        /// </summary>
        ReqStopNMon = 1021,
        /// <summary>
        /// 开始监屏
        /// 参数：
        /// ListData1：要监屏的对象的MonID（必须已经将此对象添加）
        /// </summary>
        ReqStartSMon = 1030,
        /// <summary>
        /// 停止监屏
        /// 参数：
        /// ListData1：要停止监屏的对象的MonID
        /// </summary>
        ReqStopSMon = 1031,

        /// <summary>
        /// 设置监视方式
        /// 参数：
        /// ListData1：新的监控方式
        /// ListData2：老的监控方式
        /// </summary>
        ResSetMonType = 2000,
        /// <summary>
        /// 添加监视对象
        /// 参数：
        /// ListData1：添加的有效个数（不包括类型不匹配和已经存在的）
        /// ListData2...：监视对象信息列表，包含新生成的MonID
        /// </summary>
        ResAddMonObj = 2001,
        /// <summary>
        /// 移除监视对象
        /// ListData1：移除的有效个数
        /// ListData2...：成功移除的监视对象的MonID列表
        /// </summary>
        ResRemoveMonObj = 2002,
        /// <summary>
        /// 清除所有监视对象
        /// 参数：无
        /// </summary>
        ResClearMonObj = 2003,
        /// <summary>
        /// 添加并查询对象的通道信息
        /// 参数：
        /// ListData1：添加的有效个数
        /// ListData2...：监视对象信息列表（包含MonID，ChanObjID等）
        /// </summary>
        ResAddQueryChan=2004,
        /// <summary>
        /// 查询监视对象所属的通道
        /// 参数：
        /// ListData1：查询到的有效个数
        /// ListData2...：包含通道信息的监视对象列表
        /// </summary>
        ResQueryChan = 2010,
        /// <summary>
        /// 查询指定监视对象的当前状态
        /// 参数：
        /// ListData1：监视对象的MonID
        /// ListData2：对象状态
        /// </summary>
        ResQueryState = 2011,
        /// <summary>
        /// 开始网络监听
        /// 参数：
        /// ListData1：开始监听的对象的MonID
        /// </summary>
        ResStartNMon = 2020,
        /// <summary>
        /// 停止网络监听
        /// 参数：
        /// ListData1：停止监听的对象的MonID
        /// </summary>
        ResStopNMon = 2021,
        /// <summary>
        /// 开始监屏
        /// 参数：
        /// ListData1：开始监屏的对象的MonID
        /// ListData2：监屏的端口
        /// </summary>
        ResStartSMon = 2030,
        /// <summary>
        /// 停止监屏
        /// 参数：
        /// ListData1：停止监屏的对象的MonID
        /// </summary>
        ResStopSMon = 2031,

        /// <summary>
        /// 通知有对象的状态发生变化
        /// ListData1：发生变化的监视对象的MonID
        /// ListData2：监视对象的状态
        /// ListData3：新登录通道的ObjID（默认0，如果新登录通道，则为通道的ObjID）
        /// </summary>
        NotStateChanged = 3000,
        /// <summary>
        /// 网络监听开始后返回通道的监听头信息
        /// 参数：
        /// ListData1：正在监听的对象的MonID
        /// ListData2：监听的头信息（实际是SNM_RESPONE结构，传输中使用十六进制编码）
        /// </summary>
        NotNMonHead = 3010,
        /// <summary>
        /// 网络监听开始后返回通道的监听数据
        /// 参数：
        /// ListData1：正在监听的对象的MonID
        /// ListData2：数据长度
        /// ListData3：监听的数据（传输中使用十六进制编码）
        /// </summary>
        NotNMonData = 3011,
    }
}
