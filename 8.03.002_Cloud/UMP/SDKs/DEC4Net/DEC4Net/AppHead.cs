//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    abcd1464-aacf-440b-96fe-abb169cecb74
//        CLR Version:              4.0.30319.18063
//        Name:                     AppHead
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.DEC
//        File Name:                AppHead
//
//        created by Charley at 2015/6/19 12:11:58
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.SDKs.DEC
{
    /// <summary>
    /// 应用层消息的头部
    /// </summary>
    public class AppHead
    {
        /// <summary>
        /// 通道号
        /// </summary>
        public int Channel { get; set; }
        /// <summary>
        /// 加密方式，0 表示无加密
        /// </summary>
        public int Encrypt { get; set; }
        /// <summary>
        /// 压缩方式，0 表示未压缩
        /// </summary>
        public int Compress { get; set; }
        /// <summary>
        /// 数据格式
        /// </summary>
        public int Format { get; set; }
        /// <summary>
        /// 语言
        /// </summary>
        public int CodePage { get; set; }
        /// <summary>
        /// 标识（保留，目前保持为 0 ）
        /// </summary>
        public int Identify { get; set; }
        /// <summary>
        /// 数据区大小
        /// </summary>
        public int DataSize { get; set; }
        /// <summary>
        /// 有效数据的大小
        /// </summary>
        public int ValidSize { get; set; }
        /// <summary>
        /// 保留
        /// </summary>
        public string Reserved { get; set; }

        internal static AppHead FromAppVer1(NETPACK_BASEHEAD_APPLICATION_VER1 appVer1)
        {
            AppHead head=new AppHead();
            head.Channel = appVer1._channel;
            head.Encrypt = appVer1._encrypt;
            head.Compress = appVer1._compress;
            head.Format = appVer1._format;
            head.CodePage = appVer1._codepage;
            head.Identify = appVer1._identify;
            head.DataSize = appVer1._datasize;
            head.ValidSize = appVer1._validsize;
            head.Reserved = Helpers.ConvertByteArrayToString(appVer1._reserved);
            return head;
        } 
    }
}
