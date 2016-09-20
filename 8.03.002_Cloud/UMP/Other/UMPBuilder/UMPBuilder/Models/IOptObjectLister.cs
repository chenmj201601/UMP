//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2e46ba31-c35a-4a20-9088-33d6fcf9241b
//        CLR Version:              4.0.30319.18063
//        Name:                     IOptObjectLister
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder.Models
//        File Name:                IOptObjectLister
//
//        created by Charley at 2015/12/23 15:05:13
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPBuilder.Models
{
    public interface IOptObjectLister
    {
        /// <summary>
        /// 获取选中的项目
        /// </summary>
        /// <returns></returns>
        OptObjectItem GetSelectedItem();
        /// <summary>
        /// 设置当前项
        /// </summary>
        /// <param name="item"></param>
        void SetSelectedItem(OptObjectItem item);
    }
}
