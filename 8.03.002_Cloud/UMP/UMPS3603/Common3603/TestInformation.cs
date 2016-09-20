using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common3603
{
    public class TestInfoParam
    {
        #region 考试信息参数
        /// <summary>
        /// 主键:考试编号
        /// </summary>
        public long LongTestNum { get; set; }

        /// <summary>
        /// 试卷编号
        /// </summary>
        public long LongPaperNum { get; set; }

        /// <summary>
        /// 试卷名
        /// </summary>
        public string StrPaperName { get; set; }

        /// <summary>
        /// 说明描述
        /// </summary>
        public string StrExplain { get; set; }

        /// <summary>
        /// 考试时间
        /// </summary>
        public string StrTestTime { get; set; }

        /// <summary>
        /// 编辑人ID
        /// </summary>
        public long LongEditorId { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        public string StrEditor { get; set; }

        /// <summary>
        /// 编辑时间
        /// </summary>
        public string StrDateTime { get; set; }

        /// <summary>
        /// 考试状态
        /// </summary>
        public string StrTestStatue { get; set; }

        #endregion
    }
}
