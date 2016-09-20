//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f6045ddc-6e1d-4d75-b774-c1063bf7b9b3
//        CLR Version:              4.0.30319.18063
//        Name:                     License
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.Licenses
//        File Name:                License
//
//        created by Charley at 2015/9/13 17:22:56
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.SDKs.Licenses
{
    /// <summary>
    /// License 信息
    /// </summary>
    public class License
    {
        /// <summary>
        /// License名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public long SerialNo { get; set; }
        /// <summary>
        /// License种类
        /// </summary>
        public LicOwnerType Type { get; set; }
        /// <summary>
        /// License数据类型
        /// </summary>
        public LicDataType DataType { get; set; }
        /// <summary>
        /// 许可过期时间
        /// Unlimited   无限期
        /// Invalid     无效
        /// Expired     过期
        /// 2014-01-01 00:00:00 指定时间 
        /// </summary>
        public string Expiration { get; set; }
        /// <summary>
        /// License值
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// 请求值
        /// </summary>
        public string RequestValue { get; set; }
        /// <summary>
        /// License所属主模块编号
        /// </summary>
        public int MajorID { get; set; }
        /// <summary>
        /// License所属子模块编号
        /// </summary>
        public int MinorID { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public License()
        {
            Name = string.Empty;
            SerialNo = 0;
            Type = LicOwnerType.Unkown;
            DataType = LicDataType.String;
            Expiration = LicDefines.KEYWORD_LICENSE_EXPIRATION_UNLIMITED;
            RequestValue = "0";
            Value = "0";
            MajorID = 0;
            MinorID = 0;
        }
        /// <summary>
        /// 重置License的值
        /// </summary>
        public void ResetValue()
        {
            if (DataType == LicDataType.Number)
            {
                Value = "0";
            }
            else
            {
                Value = string.Empty;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}][{2}][{3}]", Name, SerialNo, Expiration, Value);
        }
    }
}
