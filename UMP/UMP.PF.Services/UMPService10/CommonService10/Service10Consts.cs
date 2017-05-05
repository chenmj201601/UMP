//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    237bc917-3801-4dcb-8ad5-7c543816238c
//        CLR Version:              4.0.30319.18408
//        Name:                     Service10Consts
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.CommonService10
//        File Name:                Service10Consts
//
//        created by Charley at 2016/6/27 16:52:47
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.CommonService10
{
    public class Service10Consts
    {

        //Monitor 指令
        public const int MONITOR_COMMAND_GETRESOURCEOBJECT = 101;


        //状态列表字典中各类状态编号
        public const int STATE_TYPE_LOGIN = 1;
        public const int STATE_TYPE_CALL = 2;
        public const int STATE_TYPE_RECORD = 3;
        public const int STATE_TYPE_DIRECTION = 4;
        public const int STATE_TYPE_AGENT = 5;


        //配置参数

        /// <summary>
        /// 更新分机状态时间间隔，单位：分钟
        /// 范围 1 ~ 60 * 24 ，即1分钟到1天，默认30分钟
        /// </summary>
        public const string GS_KEY_S10_INTERVAL_UPDATEEXTSTATE = "UpdateExtStateInterval";
        /// <summary>
        /// 查询每个分机状态时之间的时间间隔，单位：毫秒
        /// 范围：1 ~ 60 * 1000 ，即1毫秒到1分钟，默认10毫秒
        /// </summary>
        public const string GS_KEY_S10_QUERYEXTSTATE_WAITNUM = "QueryExtWaitNum";
        
        /// <summary>
        /// 是否忽略录音服务器发来的呼叫信息，正常情况下，由录音服务器发来的呼叫信息会覆盖CTI的呼叫信息
        /// </summary>
        public const string GS_KEY_S10_IGNORVOICECALLINFO = "IgnoreVoiceCallInfo";
        /// <summary>
        /// 是否在录音停止的时候清除呼叫信息，在没有CTI的环境中，依靠录音停止的消息判断通话是否结束，如果结束，应该清除呼叫信息，默认值：1
        /// </summary>
        public const string GS_KEY_S10_CLEARCALLINFOATRECORDSTOP = "ClearCallInfoAtRecordStop";
        /// <summary>
        /// 是否使用Service10中转音频数据，（1：是；0：否；默认 1）
        /// </summary>
        public const string GS_KEY_S10_TRANSAUDIODATA = "TransAudioData";

    }
}
