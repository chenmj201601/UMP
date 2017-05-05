//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d894449d-972b-4172-a723-d38b6f9d7800
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Models
//        File Name:                ObjectItem
//
//        created by Charley at 2014/8/26 17:01:02
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS1101.Models
{
    public class ObjectItem : CheckableItemBase, IDragDropObject
    {
        public event PropertyChangedEventHandler PropertyChanged2;
        private Brush mybackground;
        public Brush MyBackground
        {
            get { return mybackground; }
            set
            {
                mybackground = value;
                if (this.PropertyChanged2 != null)
                {
                    this.PropertyChanged2.Invoke(this, new PropertyChangedEventArgs("MyBackground"));
                }
            }
        }
        public string Name { get; set; }
        public int ObjType { get; set; }
        public long ObjID { get; set; }
        public string FullName { get; set; }
        public string LockMethod { get; set; }
        public int State { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }
        public string DisplayContent { get; set; }

        private bool mIsSelected;

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; OnPropertyChanged("IsSelected"); }
        }

        private bool mIsSingleSelected;

        public bool IsSingleSelected
        {
            get { return mIsSingleSelected; }
            set { mIsSingleSelected = value; OnPropertyChanged("IsSingleSelected"); }
        }

        #region DragDrop

        public event EventHandler<DragDropEventArgs> StartDragged;

        public event EventHandler<DragDropEventArgs> DragOver;

        public event EventHandler<DragDropEventArgs> Dropped;

        public void OnStartDragged(DragDropEventArgs e)
        {
            if (StartDragged != null)
            {
                StartDragged(this, e);
            }
        }

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

        #endregion

    }
}
