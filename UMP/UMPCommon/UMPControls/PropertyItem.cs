//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0b09f364-4429-478f-a7af-06ce3b1c7118
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                PropertyItem
//
//        created by Charley at 2014/9/16 11:36:03
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 属性项
    /// </summary>
    public class PropertyItem
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 鼠标悬停的提示文本
        /// </summary>
        public string ToolTip { get; set; }
        /// <summary>
        /// 显示文本
        /// </summary>
        public string Display { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public string Value { get; set; }
    }
}
