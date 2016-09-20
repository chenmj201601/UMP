//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a64a7bc0-994b-4b67-9743-536f544f2207
//        CLR Version:              4.0.30319.18444
//        Name:                     OptReturn
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio
//        File Name:                OptReturn
//
//        created by Charley at 2014/12/8 14:40:38
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.NAudio
{
    /// <summary>
    /// 操作结果
    /// </summary>
    public class OptReturn
    {
        /// <summary>
        /// 操作结果
        /// </summary>
        public bool Result { get; set; }
        /// <summary>
        /// 返回代码，参考Defines中的定义
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 返回值，文本型
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 返回值，对象型
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 返回对象的文本表示形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}-{1}", Code, Message);
        }
    }
}
