using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common6106
{
    /// <summary>
    /// Key Value键值对  用于普通的日期-数量统计 不用KeyValuePair 是因为无法序列化
    /// </summary>
    public class KeyValueEntry
    {
        private string _strKey;

        public string StrKey
        {
            get { return _strKey; }
            set { _strKey = value; }
        }
        private float _DataValue;

        public float DataValue
        {
            get { return _DataValue; }
            set { _DataValue = value; }
        }

        /// <summary>
        /// 其他值 目前用于录音时长统计中 将数字的秒数转换成字符串时间（HH:mm:ss格式 用于显示）
        /// </summary>
        private string _strOthre1;

        public string StrOthre1
        {
            get { return _strOthre1; }
            set { _strOthre1 = value; }
        }
    }
}
