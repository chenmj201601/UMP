//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    42db71f9-1691-41c5-b28d-6ae6111af03d
//        CLR Version:              4.0.30319.18444
//        Name:                     ResourceObjectInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                ResourceObjectInfo
//
//        created by Charley at 2014/12/19 16:24:06
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 属性值
    /// </summary>
    public class ResourceProperty
    {
        /// <summary>
        /// 对象类型
        /// </summary>
        public int ObjType { get; set; }
        /// <summary>
        /// 对象ID
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 属性编号
        /// </summary>
        public int PropertyID { get; set; }
        /// <summary>
        /// 属性值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 加密模式
        /// </summary>
        public ObjectPropertyEncryptMode EncryptMode { get; set; }
        /// <summary>
        /// 复合值类型
        /// </summary>
        public int MultiValueMode { get; set; }
        /// <summary>
        /// 原值
        /// </summary>
        public string OriginalValue { get; set; }

        private List<string> mListOtherValues;
        /// <summary>
        /// 其他附加值
        /// </summary>
        public List<string> ListOtherValues
        {
            get { return mListOtherValues; }
        }

        public ResourceProperty()
        {
            mListOtherValues = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}][{3}]", ObjType, ObjID, PropertyID, Value);
        }
    }
}
