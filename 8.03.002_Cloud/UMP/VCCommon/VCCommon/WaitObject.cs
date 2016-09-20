//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    cecd82ca-91a9-4a5d-bd44-0021f35625e3
//        CLR Version:              4.0.30319.18063
//        Name:                     WaitObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                WaitObject
//
//        created by Charley at 2014/3/22 18:06:08
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.Common
{
    /// <summary>
    /// 等待对象，用于异步操作中传递相关变量
    /// </summary>
    public class WaitObject
    {
        /// <summary>
        /// 操作结果
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 是否完成，实例化的时候应该设为false，当异步操作完成设为true
        /// </summary>
        public bool IsFinished { get; set; }
        /// <summary>
        /// 等待对象的名称，作为区分不同等待对象，同一程序中等待对象的名称不能重复
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 消息信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 操作返回值
        /// </summary>
        public OperationReturn OptReturn { get; set; }
    }
}
