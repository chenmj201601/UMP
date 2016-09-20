//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    be66f76f-b700-4663-9e66-01e4fc99c544
//        CLR Version:              4.0.30319.18063
//        Name:                     IIconTreeItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                IIconTreeItem
//
//        created by Charley at 2014/4/5 17:21:45
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.CustomControls
{
    /// <summary>
    /// 定义带图标的TreeItem接口
    /// </summary>
    public interface IIconTreeItem : ITreeItem
    {
        /// <summary>
        /// 图标路径
        /// </summary>
        string Path { get; set; }
    }
}
