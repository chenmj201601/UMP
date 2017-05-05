//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f2923cca-2800-4ea7-b844-4b6358a365ee
//        CLR Version:              4.0.30319.18444
//        Name:                     PathListerEventEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                PathListerEventEventArgs
//
//        created by Charley at 2015/2/2 18:26:03
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS1110.Models
{
    /// <summary>
    /// PathListerEvent的事件参数
    /// </summary>
    public class PathListerEventEventArgs
    {
        /// <summary>
        /// 事件代码
        /// 0       未知
        /// 1       点击了确定按钮，数据：当前选中的节点
        /// 2       点击了取消（关闭）按钮，无数据
        /// 3       双击了一个节点，数据：当前选中的节点
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", Code, Data);
        }
    }
}
