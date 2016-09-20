//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f814fdd2-f2dd-41e8-8fbf-8b121aa7a493
//        CLR Version:              4.0.30319.18063
//        Name:                     ObjectItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501.Models
//        File Name:                ObjectItem
//
//        created by Charley at 2015/5/21 14:53:49
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS2501.Models
{
    public class ObjectItem : CheckableItemBase,IDragDropObject
    {
        private string mName;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private string mDescription;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; OnPropertyChanged("Description"); }
        }
        /// <summary>
        /// 0       告警模块
        /// 1       告警消息
        /// 2       告警状态
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 对象编码
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 实际数据
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 其他数据
        /// </summary>
        public object OtherData01 { get; set; }

        private bool mIsSelected;
        /// <summary>
        /// 选中
        /// </summary>
        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        private bool mIsMultiSelected;

        /// <summary>
        /// 加入多选列表
        /// </summary>
        public bool IsMultiSelected
        {
            get { return mIsMultiSelected; }
            set { mIsMultiSelected = value; OnPropertyChanged("IsMultiSelected"); }
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}]", Type, Name);
        }

        #region DragDrop

        public event EventHandler<DragDropEventArgs> DragOver;

        public event EventHandler<DragDropEventArgs> Dropped;

        public event EventHandler<DragDropEventArgs> StartDragged;

        public void OnDragOver(DragDropEventArgs e)
        {
            if (DragOver != null)
            {
                DragOver(this, e);
            }
        }

        public void OnDropped(DragDropEventArgs e)
        {
            if (Dropped != null)
            {
                Dropped(this, e);
            }
        }

        public void OnStartDragged(DragDropEventArgs e)
        {
            if (StartDragged != null)
            {
                StartDragged(this, e);
            }
        }

        #endregion

        public UCSendMethodViewer SendMothodViewer { get; set; }

    }
}
