//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d6e90b51-af71-449d-8376-8a537da08510
//        CLR Version:              4.0.30319.18063
//        Name:                     Service04Consts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService04
//        File Name:                Service04Consts
//
//        created by Charley at 2015/7/12 11:12:30
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService04
{
    public class Service04Consts
    {
        //Monitor 指令
        public const int MONITOR_COMMAND_GETRESOURCEOBJECT = 101;
        public const int MONITOR_COMMAND_GETCHANSTATE = 102;

        //配置参数

        /// <summary>
        /// 是否控制艺赛旗录屏服务，默认不控制，启用后可以根据录音的启停消息控制艺赛旗录屏的启停
        /// 1：控制
        /// 0：不控制
        /// 默认：0
        /// </summary>
        public const string GS_KEY_S04_CTLISA = "CtlIsa"; 
        /// <summary>
        /// 更新通道状态时间间隔，单位：分钟
        /// 范围 1 ~ 60 * 24 ，即1分钟到1天，默认30分钟
        /// </summary>
        public const string GS_KEY_S04_INTERVAL_UPDATECHANSTATE = "UpdateChanStateInterval";
        /// <summary>
        /// 查询每个通道状态时之间的时间间隔，单位：毫秒
        /// 范围：1 ~ 60 * 1000 ，即1毫秒到1分钟，默认10毫秒
        /// </summary>
        public const string GS_KEY_S04_QUERYCHANSTATE_WAITNUM = "QueryChanWaitNum";
        /// <summary>
        /// 
        /// 是否中转网络监听的音频数据
        /// 默认情况下，网络监听是Service04从录音服务器获取音频数据，然后转发给客户端
        /// 这种情况客户端与录音服务器的网络可以不相通
        /// 
        /// 0   不中转，客户端直接连接录音服务器的网络监听端口进行监听
        /// 1   中转，（此为缺省值），通过Service04中转网络监听数据
        /// 
        /// </summary>
        public const string GS_KEY_S04_TRANS_AUDIO_DATA = "TransAudioData";

    }
}
