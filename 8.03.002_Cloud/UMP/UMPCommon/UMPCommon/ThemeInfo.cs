//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    da5feb72-ebeb-410e-b9a6-6b0710284252
//        CLR Version:              4.0.30319.18063
//        Name:                     ThemeInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                ThemeInfo
//
//        created by Charley at 2014/8/20 15:22:11
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 主题信息
    /// </summary>
    [DataContract]
    public class ThemeInfo
    {
        /// <summary>
        /// 名称，对应主题文件夹子目录的名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 显示名，可在界面上显示的名称
        /// </summary>
        [DataMember]
        public string Display { get; set; }
        /// <summary>
        /// 颜色（同一主题下可能有多种颜色，如Green，Blue，Gray，Brown等
        /// </summary>
        [DataMember]
        public string Color { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1})", Name, Color);
        }
    }
}
