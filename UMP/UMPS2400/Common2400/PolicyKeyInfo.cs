using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common2400
{
    /// <summary>
    /// 密钥策略使用的密钥 
    /// </summary>
    public class PolicyKeyInfo
    {
        private string _PolicyID;

        public string PolicyID
        {
            get { return _PolicyID; }
            set { _PolicyID = value; }
        }
        private string _Key;

        public string Key
        {
            get { return _Key; }
            set { _Key = value; }
        }
        private string _DurationTime;

        public string DurationTime
        {
            get { return _DurationTime; }
            set { _DurationTime = value; }
        }
    }
}
