using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonService05
{
    public class TaskAlloted
    {
        long _AutoTaskID;

        string _AllotTime;
        public TaskAlloted()
        {
            ;
        }
        /// <summary>
        /// 任务ID
        /// </summary>
        public long AutoTaskID
        {
            get
            {
                return _AutoTaskID;
            }
            set
            {
                _AutoTaskID = value;
            }
        }
        /// <summary>
        /// 运行时间 eg: 2015-06-07 13:11:10
        /// </summary>
        public string AllotTime
        {
            get
            {
                return _AllotTime;
            }
            set
            {
                _AllotTime = value;
            }
        }
    }
}
