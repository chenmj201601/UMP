//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fb5e7b0b-37c0-4664-b621-f4cd978757dd
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyValueEnumInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                PropertyValueEnumInfo
//
//        created by Charley at 2015/1/20 11:29:36
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 参数值的下拉列表中的每项信息
    /// </summary>
    public class BasicInfoData
    {
        /// <summary>
        /// 编号，ModuleID+000000
        /// </summary>
        public int InfoID { get; set; }
        /// <summary>
        /// 排列序号
        /// </summary>
        public int SortID { get; set; }
        /// <summary>
        /// 父级编号
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 加密版本，0表示不加密
        /// </summary>
        public int EncryptVersion { get; set; }
        /// <summary>
        /// 实际值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 图标（或备注）
        /// </summary>
        public string Icon { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}]", InfoID, SortID, Value);
        }
    }
}
