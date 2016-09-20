//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    66b2f2e9-a70a-4d7a-9093-0d54f6b7c3c8
//        CLR Version:              4.0.30319.18444
//        Name:                     MultiSelectedItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110.Models
//        File Name:                MultiSelectedItem
//
//        created by Charley at 2015/4/6 14:40:16
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;

namespace UMPS1110.Models
{
    /// <summary>
    /// 支持多选的对象
    /// 多选的对象列表必须是同一对象类型
    /// ObjType指示对象的类型
    /// ListObjectItems指示被选择的节点列表
    /// </summary>
    public class MultiSelectedItem
    {
        /// <summary>
        /// 对象类型
        /// </summary>
        public int ObjType { get; set; }

        private List<ObjectItem> mListObjectItems;
        /// <summary>
        /// 被选择的节点列表
        /// </summary>
        public List<ObjectItem> ListObjectItems
        {
            get { return mListObjectItems; }
        }
        /// <summary>
        /// 父级节点
        /// </summary>
        public ObjectItem ParentItem { get; set; }

        public MultiSelectedItem()
        {
            mListObjectItems = new List<ObjectItem>();
        }

        /// <summary>
        /// 清除选择的对象
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < mListObjectItems.Count; i++)
            {
                mListObjectItems[i].IsMultiSelected = false;
            }
            mListObjectItems.Clear();
        }
        /// <summary>
        /// 重置并使用指定的ObjectItem初始化
        /// </summary>
        public void Reset(ObjectItem objectItem)
        {
            Clear();
            var configObject = objectItem.Data as ConfigObject;
            if (configObject != null)
            {
                ObjType = configObject.ObjectType;
                ParentItem = objectItem.Parent as ObjectItem;
                objectItem.IsMultiSelected = true;
                mListObjectItems.Add(objectItem);
            }
        }
        /// <summary>
        /// 追加一个节点
        /// </summary>
        /// <param name="objectItem"></param>
        public void AddItem(ObjectItem objectItem)
        {
            if (objectItem == null) { return; }
            objectItem.IsMultiSelected = true;
            if (!mListObjectItems.Contains(objectItem))
            {
                mListObjectItems.Add(objectItem);
            }
        }
    }
}
