//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    54bfbb0f-02f0-408a-a2f2-d3f6fce3937e
//        CLR Version:              4.0.30319.42000
//        Name:                     RecoverStrategyInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common21061
//        File Name:                RecoverStrategyInfo
//
//        Created by Charley at 2016/10/19 14:55:09
//        http://www.voicecyber.com 
//
//======================================================================

using System;


namespace VoiceCyber.UMP.Common21061
{
    /// <summary>
    /// 恢复策略信息
    /// </summary>
    public class RecoverStrategyInfo
    {
        /// <summary>
        /// 编码（205）
        /// </summary>
        public long SerialNo { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 状态
        /// 0：正常
        /// 1：禁用
        /// </summary>
        public int State { get; set; }
        /// <summary>
        /// 恢复录音的开始时间（UTC）
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 恢复录音的结束时间（UTC）
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 数据包存放目录，位于RecoverServer所在服务器上的目录位置
        /// </summary>
        public string PackagePath { get; set; }
        /// <summary>
        /// 当前执行标识
        /// 0：待执行
        /// 1：正在执行
        /// 2：执行完成（成功）
        /// 3：执行完成（失败）
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 执行进度
        /// </summary>
        public int Progress { get; set; }
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 执行次数
        /// </summary>
        public int Times { get; set; }
        /// <summary>
        /// 最后一次执行的时间
        /// </summary>
        public DateTime LastOptTime { get; set; }

        public long Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public long Modifier { get; set; }
        public DateTime ModifyTime { get; set; }

        
    }
}
