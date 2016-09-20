using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService02
{
    public class CStaticDataClass
    {
        /// <summary>
        /// 是否添加：I/U  插入/更新 
        /// </summary>
        public string ISADD { get; set; }
        /// <summary>
        /// 需要操作的列:C0000
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 操作的值
        /// </summary>
        public long ColumnValue { get; set; }
        /// <summary>
        /// 统计对象类型（1:通道；2:分机；3:座席；4:技能组；5:主叫号码；6:被叫号码）
        /// </summary>
        public int C001 { get; set; }
        /// <summary>
        /// 统计日期（格式:yyyyMMdd）
        /// </summary>
        public long C002 { get; set; }
        /// <summary>
        /// 时间类型（U:UTC 时间；L:本地时间）
        /// </summary>
        public string C003 { get; set; }
        /// <summary>
        /// 统计对象的具体值（如：通道-就是服务器IP + char(27) + 通道号；分机-就是服务器IP + char(27) + 分机号；座席：座席ID；技能组：技能组编码(19位)）
        /// </summary>
        public string C004 { get; set; }
        /// <summary>
        /// 数据扩大倍数 1
        /// </summary>
        public int C005 { get; set; }
        /// <summary>
        /// 租户
        /// </summary>
        public string C006 { get; set; }
        /// <summary>
        /// 扩展字段，记录类型（1：录音，2：录屏）
        /// </summary>
        public string C007 { get; set; }
    }
}
