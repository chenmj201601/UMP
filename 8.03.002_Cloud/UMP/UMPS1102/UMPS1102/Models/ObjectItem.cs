
using VoiceCyber.Wpf.CustomControls;
using System.ComponentModel;

namespace UMPS1102.Models
{
    public class ObjectItem : CheckableItemBase,INotifyPropertyChanged
    {
        public ObjectItem()
        {
            mIsHidden = true;
        }
        private string mName;
        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        public int ObjType { get; set; }
        public long ObjID { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }
        private bool mIsSelected;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        private bool mIsSingleSelected;
        public bool IsSingleSelected
        {
            get { return mIsSingleSelected; }
            set { mIsSingleSelected = value; OnPropertyChanged("IsSingleSelected"); }
        }

        //专给用户和机构用
        public int State { get; set; }
        public string IsActive { get; set; }
        public string IsDelete { get; set; }
        public string StrEnableTime { set; get; }
        public string StrEndTime { set; get; }
        

        //专给权限树用
        //public string IsCanUse { set; get; }
        //public string IsCanDownAssign { set; get; }
        //public string IsCanCascadeRecycle { set; get; }
        //public string IsHide { get; set; }
        private string isCanUse;
        public string IsCanUse
        {
            get { return isCanUse; }
            set
            {
                isCanUse = value; OnPropertyChanged("IsCanUse");
            }
        }
        private string isCanDownAssign;
        public string IsCanDownAssign
        {
            get { return isCanDownAssign; }
            set
            {
                isCanDownAssign = value; OnPropertyChanged("IsCanDownAssign");
            }
        }
        private string isCanCascadeRecycle;
        public string IsCanCascadeRecycle
        {
            get { return isCanCascadeRecycle; }
            set
            {
                isCanCascadeRecycle = value;
                OnPropertyChanged("IsCanCascadeRecycle");
            }
        }


        private bool mIsHidden;

        public bool IsHidden
        {
            get { return mIsHidden; }
            set { mIsHidden = value; OnPropertyChanged("IsHidden"); }
        }
    }
}
