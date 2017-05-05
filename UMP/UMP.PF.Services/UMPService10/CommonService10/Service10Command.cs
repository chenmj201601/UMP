//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c2e1f53d-c95d-4b87-a37d-7073c0215919
//        CLR Version:              4.0.30319.18408
//        Name:                     Service10Command
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService10
//        File Name:                Service10Command
//
//        created by Charley at 2016/6/27 17:42:28
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService10
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
    public enum Service10Command
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,


        #region RequestMessage

        /// <summary>
        /// 设置监视类型
        /// ListData0：新监视类型
        /// </summary>
        ReqSetMonType = 1001,

        /// <summary>
        /// 添加监视对象
        /// ListData0：Type，0：指定分机号；1：指定分机信息（xml序列化）
        /// ListData1：Count，数量
        /// ListData2...：分机号或分机信息（xml序列化）
        /// </summary>
        ReqAddMonObj = 1002,

        /// <summary>
        /// 查询分机状态
        /// ListData0：数量
        /// ListData1...：MonID
        /// </summary>
        ReqQueryExtState = 1003,

        /// <summary>
        /// 开始网络监听
        /// ListData0：MonID
        /// </summary>
        ReqStartNMon = 1011,
        /// <summary>
        /// 停止网络监听
        /// ListData0：MonID
        /// </summary>
        ReqStopNMon = 1012,

        /// <summary>
        /// 开始监视屏幕
        /// ListData0：MonID
        /// </summary>
        ReqStartSMon = 1021,
        /// <summary>
        /// 停止监视屏幕
        /// ListData0：MonID
        /// </summary>
        ReqStopSMon = 1022,

        #endregion


        #region ResponseMessage

        /// <summary>
        /// 响应设置监视类型
        /// ListData0：新监视类型
        /// ListData1：原监视类型
        /// </summary>
        ResSetMonType = 2001,

        /// <summary>
        /// 响应添加监视对象
        /// ListData0：本次添加监视对象的有效数量
        /// ListData1：本次会话监视对象的总数量
        /// ListData2...：监视对象信息，包含生成的MonID
        /// </summary>
        ResAddMonObj = 2002,

        /// <summary>
        /// 响应查询分机状态
        /// ListData0：MonID
        /// ListData1：分机信息（包括状态信息）(xml序列化)
        /// </summary>
        ResQueryExtState = 2003,

        /// <summary>
        /// 响应开始网络监听
        /// ListData0：MonID
        /// ListData1：NMonPort，监听端口
        /// ListData2：是否由UMP服务器中转音频数据
        /// ListData3：ServerAddress
        /// ListData4：ChanID
        /// </summary>
        ResStartNMon = 2011,

        /// <summary>
        /// 响应停止网络监听
        /// ListData0：MonID
        /// </summary>
        ResStopNMon = 2012,

        /// <summary>
        /// 响应开始监视屏幕
        /// ListData0：MonID
        /// ListData1：SMonPort，监屏端口
        /// ListData2：ServerAddress
        /// ListData3：ChanID
        /// </summary>
        ResStartSMon = 2021,
        /// <summary>
        /// 响应停止监视屏幕
        /// ListData0：MonID
        /// </summary>
        ResStopSMon = 2022,

        #endregion


        #region NotifyMessage

        /// <summary>
        /// 通知分机状态更新
        /// ListData0：MonID
        /// ListData1：分机信息（包括状态信息）(xml序列化)
        /// </summary>
        NotExtStateChanged = 3001,

        /// <summary>
        /// 通知监听音频编码信息
        /// ListData0：MonID
        /// ListData1：音频编码信息（head）（十六进制表示）
        /// </summary>
        NotNMonHeadReceived = 3011,
        /// <summary>
        /// 通知监听音频数据
        /// ListData0：MonID
        /// ListData1：数据长度
        /// ListData2：音频数据（十六进制表示）
        /// </summary>
        NotNMonDataReceived = 3012,

        #endregion

    }
}
