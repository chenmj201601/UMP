//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f6351369-8046-40fe-83d0-f6c499aa102f
//        CLR Version:              4.0.30319.18063
//        Name:                     ResourceObjectListerEventEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                ResourceObjectListerEventEventArgs
//
//        created by Charley at 2015/4/19 17:46:48
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS1110.Models
{
    /// <summary>
    /// 资源列表事件参数
    /// </summary>
    public class ResourceObjectListerEventEventArgs
    {
        /// <summary>
        /// 事件代码
        /// 0       Unkown
        /// 1       ItemChanged（Data：当前选择的资源对象）
        /// 2       Modify（Data：选择的资源对象列表）
        /// 3       Delete（Data：选择的资源对象列表）
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 事件数据
        /// </summary>
        public object Data { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", Code, Data);
        }
    }
}
