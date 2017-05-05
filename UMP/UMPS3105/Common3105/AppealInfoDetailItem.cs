using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3105
{
    public class AppealInfoDetailItem
    {
        /// <summary>
        ///流程ID
        /// 1为申诉，2为审批，3为复核
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 处理人ID
        /// </summary>
        public long PersonID { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 处理结果
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Demo { get; set; }
    }
}
