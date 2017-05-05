using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common2400
{
    /// <summary>
    /// 策略绑定服务器对象
    /// </summary>
    public class PolicyBindingInfo
    {
        private string _EncryptionObject;

        public string EncryptionObject
        {
            get { return _EncryptionObject; }
            set { _EncryptionObject = value; }
        }

        private string _DurationTime;

        public string DurationTime
        {
            get { return _DurationTime; }
            set { _DurationTime = value; }
        }
       
        private string _PolicyID;

        public string PolicyID
        {
            get { return _PolicyID; }
            set { _PolicyID = value; }
        }
        private string _PolicyName;

        public string PolicyName
        {
            get { return _PolicyName; }
            set { _PolicyName = value; }
        }
        private string _StartTime;

        public string StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }
        private string _EndTime;

        public string EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }
    }
}
