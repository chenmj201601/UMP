//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    845a25ba-a8ca-4351-91a5-d50ff602f4d3
//        CLR Version:              4.0.30319.18444
//        Name:                     ChildObjectListerEventEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                ChildObjectListerEventEventArgs
//
//        created by Charley at 2015/1/30 16:33:50
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS1110.Models
{
    public class ChildObjectListerEventEventArgs
    {
        /// <summary>
        /// Code        
        /// 0       Unkown
        /// 1       ChildObjectItemChanged,Data:ChildObjectItem 
        /// 2       ModifyChildObjectParam,Data:ChildObjectItem
        /// 3       DeleteChildObjectParam,Data:List(ChildObjectItem)
        /// </summary>
        public int Code { get; set; }
        public object Data { get; set; }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", Code, Data);
        }
    }
}
