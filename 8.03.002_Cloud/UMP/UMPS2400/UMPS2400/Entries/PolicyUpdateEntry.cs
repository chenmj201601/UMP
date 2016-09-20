using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS2400.Entries
{
    /// <summary>
    /// 密钥策略的修改实体类 用于写操作日志
    /// </summary>
   public  class PolicyUpdateEntry
    {
        private string policyName;

        public string PolicyName
        {
            get { return policyName; }
            set { policyName = value; }
        }

        private bool _IsUpdatePwd;

       /// <summary>
       /// 是否更新了密钥
       /// </summary>
        public bool IsUpdatePwd
        {
            get { return _IsUpdatePwd; }
            set { _IsUpdatePwd = value; }
        }

        private string _EffectTime;

       /// <summary>
       /// 新密钥生效时间
       /// </summary>
        public string EffectTime
        {
            get { return _EffectTime; }
            set { _EffectTime = value; }
        }
        private bool _IsResetCycle;

       /// <summary>
       /// 是否重置了周期
       /// </summary>
        public bool IsResetCycle
        {
            get { return _IsResetCycle; }
            set { _IsResetCycle = value; }
        }
        private bool _IsUpdateEndTime;

       /// <summary>
       /// 是否更新了密钥策略结束时间
       /// </summary>
        public bool IsUpdateEndTime
        {
            get { return _IsUpdateEndTime; }
            set { _IsUpdateEndTime = value; }
        }
        private string _EndTime;

       /// <summary>
       /// 密钥结束时间 
       /// </summary>
        public string EndTime
        {
            get { return _EndTime; }
            set { _EndTime = value; }
        }

        private long _PolicyEndTime;

       /// <summary>
       /// 修改前的密钥策略结束时间
       /// </summary>
        public long PolicyEndTime
        {
            get { return _PolicyEndTime; }
            set { _PolicyEndTime = value; }
        }

    }
}
