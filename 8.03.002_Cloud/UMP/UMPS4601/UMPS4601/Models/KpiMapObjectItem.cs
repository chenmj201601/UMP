using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common4601;

namespace UMPS4601.Models
{
    /// <summary>
    /// 在页面显示的列
    /// </summary>
    public class KpiMapObjectInfoItem
    {

        public string StrKPIName { get; set; }
        public string KPIName { get; set; }
        public string ObjectType { get; set; }
        public string IsActive { get; set; }
        public string StartTime { get; set; }
        public string StopTime { get; set; }
        public string DropDown { get; set; }
        public string ApplyAll { get; set; }
        public string AdderName { get; set; }
        public string AddTime { get; set; }
        public string GoldValue1 { get; set; }
        public string GoldValue2 { get; set; }
        public string ApplyCycle { get; set; }
        public string BelongOrgSkg { get; set; }//所属机构的名字
        public KpiMapObjectInfo KpiMapObjectInfo { get; set; }

        //新增属性 绑定对象
        public ObjectItem ObjectItem { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnProeprtyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        private string mStrObjectType;
        private string mStrIsActive;
        private string mStrDropDpwn;
        private string mStrApplyAll;
        private string mStrApplyCycle;

        public string StrObjectType 
        {
            get { return mStrObjectType; }
            set 
            { 
                mStrObjectType = value; 
                OnProeprtyChanged("StrObjectType"); 
            } 
        }
        public string StrIsActive
        {
            get { return mStrIsActive; }
            set
            {
                mStrIsActive = value;
                OnProeprtyChanged("StrIsActive");
            }
        }
        public string StrDropDown
        {
            get { return mStrDropDpwn; }
            set
            {
                mStrDropDpwn = value;
                OnProeprtyChanged("StrDropDown");
            }
        }
        public string StrApplyAll
        {
            get { return mStrApplyAll; }
            set
            {
                mStrApplyAll = value;
                OnProeprtyChanged("StrApplyAll");
            }
        }
        public string StrApplyCycle
        {
            get { return mStrApplyCycle; }
            set
            {
                mStrApplyCycle = value;
                OnProeprtyChanged("StrApplyCycle");
            }
        }


        public KpiMapObjectInfoItem(KpiMapObjectInfo kpiMapObjectInfo)
        {
            KPIName = kpiMapObjectInfo.KpiName;
            ObjectType = kpiMapObjectInfo.ApplyType;
            IsActive = kpiMapObjectInfo.IsActive;
            StartTime = Converter.NumberToDatetime(kpiMapObjectInfo.StartTime).ToString();
            StopTime = Converter.NumberToDatetime(kpiMapObjectInfo.StopTime).ToString();
            DropDown = kpiMapObjectInfo.DropDown;
            ApplyAll = kpiMapObjectInfo.ApplyAll;
            AdderName = kpiMapObjectInfo.AdderName;
            AddTime =  Converter.NumberToDatetime(kpiMapObjectInfo.AddTime).ToString();
            GoldValue1 = kpiMapObjectInfo.GoalOperation1 + kpiMapObjectInfo.GoldValue1;
            GoldValue2 = kpiMapObjectInfo.GoalOperation1 + kpiMapObjectInfo.GoldValue2;
            ApplyCycle = kpiMapObjectInfo.ApplyCycle;
            BelongOrgSkg = kpiMapObjectInfo.BelongOrgSkgName;

            KpiMapObjectInfo = kpiMapObjectInfo;
        }
    }
}
