//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    4d3d7935-eb79-44f8-99c0-59862cd3b6f7
//        CLR Version:              4.0.30319.42000
//        Name:                     RecoverChannelInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common21061
//        File Name:                RecoverChannelInfo
//
//        Created by Charley at 2016/10/19 15:06:02
//        http://www.voicecyber.com 
//
//======================================================================


namespace VoiceCyber.UMP.Common21061
{
    /// <summary>
    /// 恢复通道信息
    /// </summary>
    public class RecoverChannelInfo
    {
        /// <summary>
        /// 所属策略编号
        /// </summary>
        public long StrategyID { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int Number { get; set; }
        /// <summary>
        /// 所在服务器ID
        /// </summary>
        public int VoiceID { get; set; }
        /// <summary>
        /// 通道号
        /// </summary>
        public int ChannelID { get; set; }
        /// <summary>
        /// 分机号
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 所在服务器地址
        /// </summary>
        public string VoiceIP { get; set; }

        public long VoiceObjID { get; set; }
        public long ChannelObjID { get; set; }
    }
}
