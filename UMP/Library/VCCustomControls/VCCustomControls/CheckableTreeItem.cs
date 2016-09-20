//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5dc6b86e-31ff-448b-944f-b694e02fb210
//        CLR Version:              4.0.30319.18063
//        Name:                     CheckableTreeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                CheckableTreeItem
//
//        created by Charley at 2014/4/5 16:53:49
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.ComponentModel;

namespace VoiceCyber.CustomControls
{
    /// <summary>
    /// CheckableTree绑定对象，CheckedTree的ItemsSource必须继承此类
    /// </summary>
    public class CheckableTreeItem : ICheckableTreeItem, IIconTreeItem
    {
        #region PrivateMembers

        private int mLevel;
        private string mName;
        private bool? mIsChecked;
        private string mPath;

        #endregion

        #region Properties
        /// <summary>
        /// 深度
        /// </summary>
        public int Level
        {
            get { return mLevel; }
            set { mLevel = value; SubPropertyChanged("Level"); }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; SubPropertyChanged("Name"); }
        }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool? IsChecked
        {
            get { return mIsChecked; }
            set
            {
                SetIsChecked(value, true, true);
            }
        }
        /// <summary>
        /// 图标路径
        /// </summary>
        public string Path
        {
            get { return mPath; }
            set { mPath = value; }
        }
        #endregion

        /// <summary>
        /// 构造对象
        /// </summary>
        public CheckableTreeItem()
        {
            mIsChecked = false;
        }
        /// <summary>
        /// 构造对象
        /// </summary>
        /// <param name="name"></param>
        public CheckableTreeItem(string name)
            : this()
        {
            mName = name;
            mLevel = 0;
        }

        #region Children
        private List<ITreeItem> mChlidren = new List<ITreeItem>();
        /// <summary>
        /// 子对象集合
        /// </summary>
        public List<ITreeItem> Children
        {
            get { return mChlidren; }
        }
        #endregion

        #region Parent
        private ITreeItem mParent;
        /// <summary>
        /// 父对象
        /// </summary>
        public ITreeItem Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }
        #endregion

        #region Public Functions
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
        /// 添加子元素
        /// </summary>
        /// <param name="child"></param>
        public void AddChild(ITreeItem child)
        {
            child.Parent = this;
            child.Level = mLevel + 1;
            mChlidren.Add(child);
        }
        /// <summary>
        /// 移除子元素
        /// </summary>
        /// <param name="child"></param>
        public void RemoveChild(ITreeItem child)
        {
            try
            {
                mChlidren.Remove(child);
            }
            catch { }
        }
        /// <summary>
        /// 设置元素状态
        /// </summary>
        /// <param name="value">选中状态</param>
        /// <param name="updateChildren">是否更新子元素</param>
        /// <param name="updateParent">是否更新父元素</param>
        public void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == mIsChecked)
                return;
            mIsChecked = value;
            if (updateChildren && mIsChecked.HasValue)
            {
                foreach (var child in Children)
                {
                    ICheckableTreeItem checkableItem = child as ICheckableTreeItem;
                    if (checkableItem != null) { checkableItem.SetIsChecked(mIsChecked, true, false); }
                }
            }
            if (updateParent && mParent != null)
            {
                ICheckableTreeItem parent = mParent as ICheckableTreeItem;
                if (parent != null) { parent.VerifyCheckState(); }
            }
            SubPropertyChanged("IsChecked");
        }
        /// <summary>
        /// 检查元素状态
        /// </summary>
        public void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < Children.Count; ++i)
            {
                ICheckableTreeItem checkableItem = Children[i] as ICheckableTreeItem;
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

        #region INotifyPropertyChanged 成员
        /// <summary>
        /// 引发属性更新事件
        /// </summary>
        /// <param name="property"></param>
        protected void SubPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        /// <summary>
        /// 属性更新事件
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
