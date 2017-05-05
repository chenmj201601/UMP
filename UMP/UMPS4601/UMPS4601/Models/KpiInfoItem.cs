using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common4601;

namespace UMPS4601.Models
{
    public class KpiInfoItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ApplyCycle { get; set; }
        public string UseType { get; set; }

        private string _mActive;
        public string Active 
        { 
            get { return _mActive; }
            set
            {
                _mActive = value;
                OnProeprtyChanged("Active");
            }
        }

        private string mDefaultSymbol;
        public string DefaultSymbol 
        {
            get { return mDefaultSymbol; }
            set
            {
                mDefaultSymbol = value;
                OnProeprtyChanged("DefaultSymbol");
            } 
        }
        private string mGoalValue1;
        public string GoalValue1 
        {
            get { return mGoalValue1; }
            set
            {
                mGoalValue1 = value;
                OnProeprtyChanged("GoalValue1");
            }  
        }
        private string mGoalValue2;
        public string GoalValue2 
        {
            get { return mGoalValue2; }
            set
            {
                mGoalValue2 = value;
                OnProeprtyChanged("GoalValue2");
            }  
        }

        public KpiInfo KpiInfo { get; set; }

        public KpiInfoItem(KpiInfo kpiInfo)
        {
            Name = kpiInfo.Name;
            Description = kpiInfo.Description;
            ApplyCycle = kpiInfo.ApplyCycle;
            UseType = kpiInfo.UseType;
            Active = kpiInfo.Active;
            DefaultSymbol = kpiInfo.DefaultSymbol;
            GoalValue1 = kpiInfo.GoalValue1;
            GoalValue2 = kpiInfo.GoalValue2;

            KpiInfo = kpiInfo;
        }

        private string mActive;
        public string StrActive
        {
            get { return mActive; }
            set
            {
                mActive = value;
                OnProeprtyChanged("StrActive");
            } 
        }

        public string StrDescription { get; set; }
        public string StrUseType { get; set; }
        public string StrApplyCycle { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnProeprtyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
