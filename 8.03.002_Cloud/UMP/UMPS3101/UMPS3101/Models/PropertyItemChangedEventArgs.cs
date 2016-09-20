//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    43c99719-c4f7-4fc0-8ba5-e7234d03e86a
//        CLR Version:              4.0.30319.18063
//        Name:                     PropertyItemChangedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Models
//        File Name:                PropertyItemChangedEventArgs
//
//        created by Charley at 2015/11/8 15:23:09
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.ScoreSheets;

namespace UMPS3101.Models
{
    /// <summary>
    /// 属性项改变事件参数
    /// </summary>
    public class PropertyItemChangedEventArgs
    {
        /// <summary>
        /// 评分对象
        /// </summary>
        public ScoreObject ScoreObject { get; set; }
        /// <summary>
        /// 当前属性项
        /// </summary>
        public ScorePropertyInfoItem PropertyItem { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", ScoreObject, PropertyItem);
        }
    }
}
