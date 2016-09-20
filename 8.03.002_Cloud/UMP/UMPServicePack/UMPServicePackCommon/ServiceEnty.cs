using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPServicePackCommon
{
    public class ServiceEnty
    {
        private string _ServiceName;

        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName
        {
            get { return _ServiceName; }
            set { _ServiceName = value; }
        }
        private int _ServiceStatus;

        /// <summary>
        /// 服务状态
        /// </summary>
        public int ServiceStatus
        {
            get { return _ServiceStatus; }
            set { _ServiceStatus = value; }
        }

    }
}
