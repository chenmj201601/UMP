//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    842bc5d1-aeec-48ad-90ca-9c649d01aa73
//        CLR Version:              4.0.30319.18444
//        Name:                     StandardVisualClassic
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets
//        File Name:                StandardVisualClassic
//
//        created by Charley at 2014/6/10 14:23:21
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.ScoreSheets
{
    /// <summary>
    /// 评分标准显示风格
    /// </summary>
    public enum StandardClassic
    {
        Unkown = 0,
        /// <summary>
        /// 文本框型
        /// </summary>
        TextBox = 1,
        /// <summary>
        /// 单选按钮是非型
        /// </summary>
        YesNo = 2,
        /// <summary>
        /// 复选框是非型
        /// </summary>
        CheckBox = 3,
        /// <summary>
        /// 下拉列表多值型
        /// </summary>
        DropDownList = 4,
        /// <summary>
        /// 复选框多值型
        /// </summary>
        CheckList = 5,
        /// <summary>
        /// 拖拉型
        /// </summary>
        Slider = 6
    }
}
