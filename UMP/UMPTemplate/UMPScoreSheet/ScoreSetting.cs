//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ccbe5975-fbf5-4af1-aa1b-27f17ace29d4
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreSetting
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                ScoreSetting
//
//        created by Charley at 2014/7/8 16:23:15
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 设定信息
    /// </summary>
    public class ScoreSetting
    {
        /// <summary>
        /// 设定的代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 设定的分类
        /// S       基本设定信息
        /// I       图标
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 设定的值
        /// </summary>
        public string Value { get; set; }
    }
}
