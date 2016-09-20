//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    07d0c289-0fb2-473a-834a-2a581e185a1a
//        CLR Version:              4.0.30319.18063
//        Name:                     CheckResult
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                CheckResult
//
//        created by Charley at 2015/4/24 16:05:55
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS1110.Models
{
    /// <summary>
    /// 检查结果
    /// </summary>
    public class CheckResult
    {
        /// <summary>
        /// 对于服务资源，没有指定服务所在的机器
        /// </summary>
        public const int RES_NOMACHINE = 100;
        /// <summary>
        /// 指定的机器无效或已经被删除
        /// </summary>
        public const int RES_INVALIDMACHINE = 101;
        /// <summary>
        /// 指定的必要属性未配置
        /// </summary>
        public const int RES_NOCONFIG = 102;
        /// <summary>
        /// 通道的分机号为空
        /// </summary>
        public const int RES_CHAN_EXT_INVALID = 110;
        /// <summary>
        /// VoiceServer最大Voip通道数无效（小于实际配置Voip通道数）
        /// </summary>
        public const int RES_VOICESERVER_MAXVOIPCHANNEL_INVALID = 201;

        /// <summary>
        /// 检查结果
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 结果代码
        /// 0       检查通过，无错误（默认）
        /// 1       未知错误
        /// </summary>
        public int Code { get; set; }
        public string Message { get; set; }
        public ConfigObject ConfigObject { get; set; }
        public int PropertyID { get; set; }
    }
}
