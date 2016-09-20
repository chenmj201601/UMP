//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    24ab5bda-33dd-4788-a5c5-01a4ab358e7b
//        CLR Version:              4.0.30319.18063
//        Name:                     ICheckableTreeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                ICheckableTreeItem
//
//        created by Charley at 2014/4/5 16:07:04
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.CustomControls
{
    /// <summary>
    /// 定义可选择的TreeItem接口
    /// </summary>
    public interface ICheckableTreeItem:ITreeItem
    {
        /// <summary>
        /// 选择状态
        /// Tree    全部（包含所有子元素）选中
        /// False   全部（包含所有子元素）未选中
        /// Null    部分选中
        /// </summary>
        bool? IsChecked { get; set; }
        /// <summary>
        /// 设置元素状态
        /// </summary>
        /// <param name="value">选中状态</param>
        /// <param name="updateChildren">是否更新子元素</param>
        /// <param name="updateParent">是否更新父元素</param>
        void SetIsChecked(bool? value, bool updateChildren, bool updateParent);
        /// <summary>
        /// 检查元素状态
        /// </summary>
        void VerifyCheckState();
    }
}
