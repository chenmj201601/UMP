using System;
using System.ComponentModel;

namespace VoiceCyber.UMP.Common11021
{
    public class RoleModel :INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(PropertyChangedEventArgs e) 
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }

        public long RoleID{set;get;}
        public long ParentRoleID { set; get; }
        public long ModeID { set; get; }

        private string roleName;
        public string RoleName
        {
            get { return roleName; }
            set
            {
                roleName = value;
               // OnPropertyChanged(new PropertyChangedEventArgs("RoleName"));
            }
        }

        private string isActive;
        public string IsActive 
        {
            get { return isActive; }
            set 
            {
                isActive = value;
                OnPropertyChanged(new PropertyChangedEventArgs("IsActive"));
            }
        }

        private string strIsActive;

        public string StrIsActive 
        {
            get 
            {
                return strIsActive;
            }
            set 
            {
                strIsActive = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StrIsActive"));
            }
        }


        #region Tip

        private string mTipModifyRole;

        public string TipModifyRole
        {
            get { return mTipModifyRole; }
            set
            {
                mTipModifyRole = value;
                OnPropertyChanged(new PropertyChangedEventArgs("TipModifyRole"));
            }
        }

        private string mTipRemoveRole;

        public string TipRemoveRole
        {
            get { return mTipRemoveRole; }
            set { mTipRemoveRole = value;
            OnPropertyChanged(new PropertyChangedEventArgs("TipRemoveRole"));
            }
        }

        #endregion

        public string IsDelete { set; get; }
        public string OtherStatus { set; get; }
        public DateTime enableTime;
        public DateTime EnableTime 
        {
            get { return enableTime; }
            set
            {
                enableTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("enableTime"));
            }
        }

        private string strEnableTime;
        public String StrEnableTime 
        {
            get { return strEnableTime; }
            set
            {
                strEnableTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StrEnableTime"));
            }
        }
        public DateTime endTime;
        public DateTime EndTime
        {
            get { return endTime; }
            set
            {
                endTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("endTime"));
            }
        }

        private string strEndTime;
        public String StrEndTime 
        {
            get { return strEndTime; }
            set
            {
                strEndTime = value;
                OnPropertyChanged(new PropertyChangedEventArgs("StrEndTime"));
            }
        }

        public long CreatorID { set; get; }
        public string CreatorName { set; get; }
        public DateTime CreatTime { set; get; }
    }
}
