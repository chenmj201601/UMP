//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b6c5c122-27e1-4855-a7c8-1d48375c5ae5
//        CLR Version:              4.0.30319.18444
//        Name:                     CheckableItemBase
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.TreeViews.Implementation
//        File Name:                CheckableItemBase
//
//        created by Charley at 2014/8/22 14:01:45
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 可选择的项，要让TreeView实现可选中功能，则绑定的项元素可以继承此类
    /// </summary>
    public abstract class CheckableItemBase : ICheckableItem, INotifyPropertyChanged
    {
        public CheckableItemBase()
        {
            mChildren = new ObservableCollection<CheckableItemBase>();
        }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        private ObservableCollection<CheckableItemBase> mChildren;
        /// <summary>
        /// 子项
        /// </summary>
        public ObservableCollection<CheckableItemBase> Children
        {
            get { return mChildren; }
        }
        private bool mIsExpanded;
        /// <summary>
        /// 展开
        /// </summary>
        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set { mIsExpanded = value; OnPropertyChanged("IsExpanded"); }
        }
        /// <summary>
        /// 属性通知
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 引发属性通知
        /// </summary>
        /// <param name="property"></param>
        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #region ICheckableItem

        /// <summary>
        /// 初始化子元素的父元素
        /// </summary>
        public void Init()
        {
            foreach (var child in Children)
            {
                child.Parent = this;
                child.Init();
            }
        }
        /// <summary>
        /// 增加一个子项
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(CheckableItemBase child)
        {
            Children.Add(child);
            child.Parent = this;
        }
        /// <summary>
        /// 移除一个子项
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(CheckableItemBase child)
        {
            if (Children.IndexOf(child) >= 0)
            {
                Children.Remove(child);
            }
        }
        private bool? mIsChecked;
        /// <summary>
        /// 选中状态
        /// </summary>
        public bool? IsChecked
        {
            get { return mIsChecked; }
            set { SetIsChecked(value, true, true); }
        }

        private ICheckableItem mParent;
        /// <summary>
        /// 父对象
        /// </summary>
        public ICheckableItem Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        public void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == mIsChecked)
                return;
            mIsChecked = value;
            if (updateChildren && mIsChecked.HasValue)
            {
                foreach (var child in Children)
                {
                    ICheckableItem checkableItem = child;
                    if (checkableItem != null) { checkableItem.SetIsChecked(mIsChecked, true, false); }
                }
            }
            if (updateParent && mParent != null)
            {
                ICheckableItem parent = mParent;
                if (parent != null) { parent.VerifyCheckState(); }
            }
            OnPropertyChanged("IsChecked");
        }

        /// <summary>
        /// 检查元素状态
        /// </summary>
        public void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < Children.Count; ++i)
            {
                ICheckableItem checkableItem = Children[i];
                if (checkableItem == null) { continue; }
                bool? current = checkableItem.IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            SetIsChecked(state, false, true);
        }

        #endregion

    }
}
