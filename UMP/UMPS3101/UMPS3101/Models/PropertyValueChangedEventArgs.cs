//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8d5baceb-8a42-4433-bd05-9078f5014e9a
//        CLR Version:              4.0.30319.18063
//        Name:                     PropertyValueChangedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Models
//        File Name:                PropertyValueChangedEventArgs
//
//        created by Charley at 2015/11/5 18:21:47
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.ScoreSheets;

namespace UMPS3101.Models
{
    /// <summary>
    /// 属性改变事件参数
    /// </summary>
    public class PropertyValueChangedEventArgs
    {
        /// <summary>
        /// 评分对象
        /// </summary>
        public ScoreObject ScoreObject { get; set; }
        /// <summary>
        /// 属性项
        /// </summary>
        public ScorePropertyInfoItem PropertyItem { get; set; }
        /// <summary>
        /// 评分属性
        /// </summary>
        public ScoreProperty ScoreProperty { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 属性子项
        /// </summary>
        public PropertyValueEnumItem ValueItem { get; set; }
        /// <summary>
        /// 是否初始（标识）
        /// </summary>
        public bool IsInit { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", PropertyItem, Value);
        }
    }
}
