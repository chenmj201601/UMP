//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    69619560-9e06-4de9-b00d-ad59bdc2a0ee
//        CLR Version:              4.0.30319.34003
//        Name:                     OptReturn
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio
//        File Name:                OptReturn
//
//        created by Charley at 2013/12/1 12:03:27
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
