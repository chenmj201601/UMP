//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2b44d00f-2ef1-48ae-a3a9-c408cf68663e
//        CLR Version:              4.0.30319.42000
//        Name:                     WidgetInfo
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12001
//        File Name:                WidgetInfo
//
//        created by Charley at 2016/3/2 16:32:15
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common12001
{
    /// <summary>
    /// 小部件信息
    /// </summary>
    public class WidgetInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public long WidgetID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        public bool IsCenter { get; set; }

        public int SortID { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}]{1}", WidgetID, Name);
        }
    }
}
