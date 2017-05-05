using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Common6106
{
    public class RecordCountEntry
    {
        private string _RecCount;

        /// <summary>
        /// 通话量
        /// </summary>
        public string RecCount
        {
            get { return _RecCount; }
            set { _RecCount = value; }
        }
        private string _RecDate;

        /// <summary>
        /// 录音开始时间
        /// </summary>
        public string RecDate
        {
            get { return _RecDate; }
            set { _RecDate = value; }
        }
        private string _RecDirection;

        /// <summary>
        /// 呼叫方向
        /// </summary>
        public string RecDirection
        {
            get { return _RecDirection; }
            set { _RecDirection = value; }
        }
    }
}
