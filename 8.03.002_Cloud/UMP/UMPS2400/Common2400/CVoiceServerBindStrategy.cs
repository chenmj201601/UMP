using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common2400
{
    public class CVoiceServerBindStrategy
    {
        /// <summary>
        /// c001 objecttype 使用对象类型
        /// </summary>
        public string Objecttype { get; set; }
        /// <summary>
        /// c002 objectvalue 对象的值 1.录音服务器
        /// </summary>
        public string Objectvalue { get; set; }
        /// <summary>
        /// c003 bindingpolicyid 使用的 策略编码
        /// </summary>
        public long Bindingpolicyid { get; set; }
        /// <summary>
        /// c004 Durationbegin 持续时间（开始）
        /// </summary>
        public long Durationbegin { get; set; }
        /// <summary>
        /// c005 Durationend 持续时间（结束 utc）
        /// </summary>
        public long Durationend { get; set; }
        /// <summary>
        /// c006 Setteduserid 设置人
        /// </summary>
        public long Setteduserid { get; set; }
        /// <summary>
        /// c007 Settedtime 设置时间。utc
        /// </summary>
        public long Settedtime { get; set; }
        /// <summary>
        /// c008 Grantencryption
        /// </summary>
        public string Grantencryption { get; set; }
        /// <summary>
        /// c009 备用字段 服务器IP地址对应的资源编码 221....
        /// </summary>
        public string CusFiled1 { get; set; }
        /// <summary>
        /// c010 备用字段 操作人
        /// </summary>
        public string CusFiled2 { get; set; }
        /// <summary>
        /// c011 备用字段 策略名称
        /// </summary>
        public string CusFiled3 { get; set; }
        /// <summary>
        /// c012 备用字段
        /// </summary>
        public string CusFiled4 { get; set; }
        /// <summary>
        /// 说明、描述
        /// </summary>
        public string Description { get; set; }

        public string DurationbeginStr { get; set; }
        public string DurationendStr { get; set; }
        public string SettedtimeStr { get; set; }
    }
}
