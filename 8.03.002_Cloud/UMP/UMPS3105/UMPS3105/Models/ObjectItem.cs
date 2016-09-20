using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3105.Models
{
    public class ObjectItem : INotifyPropertyChanged, IDragDropObject
    {
        /// <summary>
        /// ScoreObjectType
        /// </summary>
        public int ObjType { get; set; }

        private string mDisplay;
        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; SubPropertyChanged("Display"); }
        }

        private string mToolTip;

        public string ToolTip
        {
            get { return mToolTip; }
            set { mToolTip = value; SubPropertyChanged("ToolTip"); }
        }
        public string Icon { get; set; }
        private bool mIsExpanded;
        public bool IsExpanded
        {
            get { return mIsExpanded; }
            set { mIsExpanded = value; SubPropertyChanged("IsExpanded"); }
        }

        private int mInvalidCode;

        public int InvalidCode
        {
            get { return mInvalidCode; }
            set { mInvalidCode = value; SubPropertyChanged("InvalidCode"); }
        }

        private string mInvalidMessage;

        public string InvalidMessage
        {
            get { return mInvalidMessage; }
            set { mInvalidMessage = value; SubPropertyChanged("InvalidMessage"); }
        }

        private bool mIsSelected;

        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; SubPropertyChanged("IsSelected"); }
        }
        public object Data { get; set; }
        private ObservableCollection<ObjectItem> mChildren;
        public ObservableCollection<ObjectItem> Children
        {
            get { return mChildren; }
        }

        private ObjectItem mParent;

        public ObjectItem Parent
        {
            get { return mParent; }
            set { mParent = value; }
        }

        public ObjectItem()
        {
            mChildren = new ObservableCollection<ObjectItem>();
        }

        public void ClearChildren()
        {
            mChildren.Clear();
        }

        public void AddChild(ObjectItem child)
        {
            child.Parent = this;
            Children.Add(child);
        }

        public void RemoveChild(ObjectItem child)
        {
            if (mChildren.Contains(child))
            {
                mChildren.Remove(child);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SubPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
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
