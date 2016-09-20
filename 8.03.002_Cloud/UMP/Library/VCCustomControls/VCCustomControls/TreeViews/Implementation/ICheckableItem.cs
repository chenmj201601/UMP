//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    10eebe92-41f8-473b-ae1f-01da34dbfe0a
//        CLR Version:              4.0.30319.18063
//        Name:                     ICheckableItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.TreeViews.Implementation
//        File Name:                ICheckableItem
//
//        created by Charley at 2014/8/21 14:37:16
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 可选择的项，要让TreeView实现可选中功能，则绑定的项元素需实现接口
    /// </summary>
    public interface ICheckableItem
    {
        /// <summary>
        /// 选择状态
        /// True    选中（所有子项也选中）
        /// False   为选中（所有子项都未选中）
        /// Null    子项部分选中
        /// </summary>
        bool? IsChecked { get; set; }
        /// <summary>
        /// 设置元素选中状态
        /// </summary>
        /// <param name="value"></param>
        /// <param name="updateChildren"></param>
        /// <param name="updateParent"></param>
        void SetIsChecked(bool? value, bool updateChildren, bool updateParent);
        /// <summary>
        /// 检查元素状态
        /// </summary>
        void VerifyCheckState();
        /// <summary>
        /// 父元素
        /// </summary>
        ICheckableItem Parent { get; set; }
        /// <summary>
        /// 初始化，将子元素的父元素设为自身
        /// </summary>
        void Init();
    }
}
