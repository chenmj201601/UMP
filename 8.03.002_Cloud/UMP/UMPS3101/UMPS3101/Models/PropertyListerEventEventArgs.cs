//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    721a3563-5fd5-499b-ae1a-f7a4eb933cb5
//        CLR Version:              4.0.30319.18063
//        Name:                     PropertyListerEventEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Models
//        File Name:                PropertyListerEventEventArgs
//
//        created by Charley at 2015/11/8 15:12:24
//        http://www.voicecyber.com 
//
//======================================================================

using VoiceCyber.UMP.ScoreSheets;

namespace UMPS3101.Models
{
    /// <summary>
    /// 属性列表事件参数
    /// </summary>
    public class PropertyListerEventEventArgs
    {
        /// <summary>
        /// 事件代码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 评分对象
        /// </summary>
        public ScoreObject ScoreObject { get; set; }
        /// <summary>
        /// 事件数据
        /// </summary>
        public object Data { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]{1}]", Code, Data);
        }

        #region 事件代码

        /// <summary>
        /// PropertyItemChanged
        /// Data:PropertyItemChangedEventArgs
        /// </summary>
        public const int CODE_PRO_ITEM_CHANGED = 1;
        /// <summary>
        /// PropertyValueChanged
        /// Data:PropertyValueChangedEventArgs
        /// </summary>
        public const int CODE_PRO_VALUE_CHANGED = 2;

        #endregion
    }
}
