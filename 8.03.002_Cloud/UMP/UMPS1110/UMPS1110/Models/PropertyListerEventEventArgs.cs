//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6a9b1a65-98c9-448a-afa2-0d0dcdadcbe8
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyListerEventEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                PropertyListerEventEventArgs
//
//        created by Charley at 2015/1/20 18:32:59
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;

namespace UMPS1110.Models
{
    public class PropertyListerEventEventArgs
    {
        /// <summary>
        /// Code        
        /// 0       Unkown
        /// 1       PropertyItemChanged,  Data:PropertyItemChangedEventArgs  
        /// 2       PropertyValueChanged, Data:PropertyValueChangedEventArgs     
        /// </summary>
        public int Code { get; set; }

        private List<ConfigObject> mListConfigObjects;
        /// <summary>
        /// 配置对象列表
        /// </summary>
        public List<ConfigObject> ListConfigObjects
        {
            get { return mListConfigObjects; }
        } 
        /// <summary>
        /// 事件的数据
        /// </summary>
        public object Data { get; set; }

        public PropertyListerEventEventArgs()
        {
            mListConfigObjects = new List<ConfigObject>();
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", Code, Data);
        }
    }
}
