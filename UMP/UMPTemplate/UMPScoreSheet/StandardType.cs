//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d0ea659b-168f-4ba4-934d-d75d0d2d72f6
//        CLR Version:              4.0.30319.18444
//        Name:                     StandardType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                StandardType
//
//        created by Charley at 2014/6/10 14:22:05
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 评分标准类型
    /// </summary>
    public enum StandardType
    {
        /// <summary>
        /// 数值型
        /// </summary>
        Numeric = 1,
        /// <summary>
        /// 是非型
        /// </summary>
        YesNo = 2,
        /// <summary>
        /// 拖拉型
        /// </summary>
        Slider = 3,
        /// <summary>
        /// 多值型
        /// </summary>
        Item = 4
    }
}
