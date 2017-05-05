using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UMPS2400.Entries
{
    public class EncryptionPolicy : INotifyPropertyChanged
    {
        public EncryptionPolicy()
        {
            PolicyID = string.Empty;
            PolicyName = string.Empty;
            PolicyIsEnabled = false;
            PolicyOccursFrequency = string.Empty;
            MustContainCapitals = string.Empty;
            MustContainDigital = string.Empty;
            MustContainLower = string.Empty;
            MustContainSpecial = string.Empty;
            NumbersCapitals = 2;
            NumbersDigital = 2;
            NumbersLower = 2;
            NumbersSpecial = 0;
            Complexityabled = "1";
            ResetCycle = "0";
            PolicyNotes = string.Empty;
            BeginDayofCycle = string.Empty;
            TypeuEncryptKey = string.Empty;
        }

        private string _PolicyID;

        /// <summary>
        /// 策略ID
        /// </summary>
        public string PolicyID
        {
            get { return _PolicyID; }
            set { _PolicyID = value; }
        }

        private string _PolicyName;

        /// <summary>
        /// 策略名称
        /// </summary>
        public string PolicyName
        {
            get { return _PolicyName; }
            set { _PolicyName = value; }
        }

        private bool _PolicyIsEnabled;

        /// <summary>
        /// 策略是否可用
        /// </summary>
        public bool PolicyIsEnabled
        {
            get { return _PolicyIsEnabled; }
            set { _PolicyIsEnabled = value;
            OnPropertyChanged("PolicyIsEnabled");
            }
        }

        private string _PolicyType;

        /// <summary>
        /// 策略类型 U或C
        /// </summary>
        public string PolicyType
        {
            get { return _PolicyType; }
            set { _PolicyType = value; }
        }


        private int _IsImmediately;

        /// <summary>
        /// 是否立即开始 因为不能直接从客户端获取当前时间 需要从wcf获取 所以需要此变量来标记 1:是 0：否 
        /// </summary>
        public int IsImmediately
        {
            get { return _IsImmediately; }
            set { _IsImmediately = value; }
        }

        private string _TypeuEncryptKey;

        /// <summary>
        /// 策略类型为用户输入时的密钥
        /// </summary>
        public string TypeuEncryptKey
        {
            get { return _TypeuEncryptKey; }
            set { _TypeuEncryptKey = value; }
        }

        private string _PolicyOccursFrequency;

        /// <summary>
        /// 执行频率
        /// </summary>
        public string PolicyOccursFrequency
        {
            get { return _PolicyOccursFrequency; }
            set { _PolicyOccursFrequency = value; }
        }

        private string _DurationBegin;

        /// <summary>
        /// 有效期开始
        /// </summary>
        public string DurationBegin
        {
            get { return _DurationBegin; }
            set { _DurationBegin = value; }
        }

        private string _DurationEnd;

        /// <summary>
        /// 有效期结束
        /// </summary>
        public string DurationEnd
        {
            get { return _DurationEnd; }
            set { _DurationEnd = value; }
        }

        private string _BeginDayofCycle;

        /// <summary>
        /// 执行周期变量
        /// </summary>
        public string BeginDayofCycle
        {
            get { return _BeginDayofCycle; }
            set { _BeginDayofCycle = value; }
        }

        private string _ResetCycle;

        /// <summary>
        /// 是否重置周期
        /// </summary>
        public string ResetCycle
        {
            get { return _ResetCycle; }
            set { _ResetCycle = value; }
        }

        private string _Complexityabled;

        /// <summary>
        /// 密码复杂性校验 
        /// </summary>
        public string Complexityabled
        {
            get { return _Complexityabled; }
            set { _Complexityabled = value; }
        }

        private string _MustContainCapitals;

        /// <summary>
        /// 必须包含大写字母
        /// </summary>
        public string MustContainCapitals
        {
            get { return _MustContainCapitals; }
            set { _MustContainCapitals = value; }
        }

        private int _NumbersCapitals;

        /// <summary>
        /// 包含大写字母个数。
        /// </summary>
        public int NumbersCapitals
        {
            get { return _NumbersCapitals; }
            set { _NumbersCapitals = value; }
        }

        private string _MustContainLower;

        /// <summary>
        /// 必须包含小写字母
        /// </summary>
        public string MustContainLower
        {
            get { return _MustContainLower; }
            set { _MustContainLower = value; }
        }

        private int _NumbersLower;

        /// <summary>
        /// 包含小写字母个数
        /// </summary>
        public int NumbersLower
        {
            get { return _NumbersLower; }
            set { _NumbersLower = value; }
        }

        private string _MustContainDigital;

        /// <summary>
        /// 必须包含数字
        /// </summary>
        public string MustContainDigital
        {
            get { return _MustContainDigital; }
            set { _MustContainDigital = value; }
        }

        private int _NumbersDigital;

        /// <summary>
        /// 包含数字个数
        /// </summary>
        public int NumbersDigital
        {
            get { return _NumbersDigital; }
            set { _NumbersDigital = value; }
        }

        private string _MustContainSpecial;

        /// <summary>
        /// 必须包含特殊字符
        /// </summary>
        public string MustContainSpecial
        {
            get { return _MustContainSpecial; }
            set { _MustContainSpecial = value; }
        }

        private int _NumbersSpecial;

        /// <summary>
        /// 包含特殊字符个数
        /// </summary>
        public int NumbersSpecial
        {
            get { return _NumbersSpecial; }
            set { _NumbersSpecial = value; }
        }

        private int _PasswordHistoryInNumber;

        /// <summary>
        /// 在多少个密钥内不能重复
        /// </summary>
        public int PasswordHistoryInNumber
        {
            get { return _PasswordHistoryInNumber; }
            set { _PasswordHistoryInNumber = value; }
        }

        private int _PasswordHistoryinDay;
        
        /// <summary>
        /// 在多少天密钥内不能重复
        /// </summary>
        public int PasswordHistoryinDay
        {
            get { return _PasswordHistoryinDay; }
            set { _PasswordHistoryinDay = value; }
        }

        private int _TheshortestLength;

        /// <summary>
        /// 最短长度
        /// </summary>
        public int ThesHortestLength
        {
            get { return _TheshortestLength; }
            set { _TheshortestLength = value; }
        }

        private int _TheLongestLength;

        /// <summary>
        /// 最长长度
        /// </summary>
        public int TheLongestLength
        {
            get { return _TheLongestLength; }
            set { _TheLongestLength = value; }
        }

        private string _NotContainUserID;

        /// <summary>
        /// 不能包含用户账号。目前不使用
        /// </summary>
        public string NotContainUserID
        {
            get { return _NotContainUserID; }
            set { _NotContainUserID = value; }
        }

        private int _PolicyCreator;

        /// <summary>
        /// 策略创建人
        /// </summary>
        public int PolicyCreator
        {
            get { return _PolicyCreator; }
            set { _PolicyCreator = value; }
        }

        private string _PolicyNotes;

        /// <summary>
        /// 策略描述 
        /// </summary>
        public string PolicyNotes
        {
            get { return _PolicyNotes; }
            set { _PolicyNotes = value; }
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
        /// <summary>
        /// 将EncryptionPolicy对象转成list<string> 用于传送给wcf
        /// </summary>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static List<string> ObjectToListString(EncryptionPolicy policy)
        {
            List<string> lstStrs = new List<string>();
            try
            {
                lstStrs.Add(policy.PolicyName);
                lstStrs.Add(policy.PolicyType);
                lstStrs.Add(policy.IsImmediately.ToString());
                lstStrs.Add(policy.TypeuEncryptKey);
                lstStrs.Add(policy.PolicyOccursFrequency);
                lstStrs.Add(policy.DurationBegin);
                lstStrs.Add(policy.DurationEnd);
                lstStrs.Add(policy.BeginDayofCycle);
                lstStrs.Add(policy.Complexityabled);
                lstStrs.Add(policy.MustContainCapitals);
                lstStrs.Add(policy.NumbersCapitals.ToString());
                lstStrs.Add(policy.MustContainLower);
                lstStrs.Add(policy.NumbersLower.ToString());
                lstStrs.Add(policy.MustContainDigital);
                lstStrs.Add(policy.NumbersDigital.ToString());
                lstStrs.Add(policy.MustContainSpecial);
                lstStrs.Add(policy.NumbersSpecial.ToString());
                lstStrs.Add("0");
                lstStrs.Add("0");
                lstStrs.Add(policy.ThesHortestLength.ToString());
                lstStrs.Add(policy.TheLongestLength.ToString());
                lstStrs.Add(policy.PolicyNotes);
                lstStrs.Add(policy.ResetCycle);
                return lstStrs;
            }
            catch (Exception ex)
            {

            }
            return lstStrs;
        } 
    }
}
