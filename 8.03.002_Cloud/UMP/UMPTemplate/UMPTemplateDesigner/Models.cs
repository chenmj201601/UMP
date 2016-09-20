//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7cb13703-a854-4741-a6b5-64248f2c4f10
//        CLR Version:              4.0.30319.18444
//        Name:                     Models
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTemplateDesigner
//        File Name:                Models
//
//        created by Charley at 2014/6/9 16:59:36
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace UMPTemplateDesigner
{
    public class NewButtonItem
    {
        public string Name { get; set; }
        public string Header { get; set; }
        public string Icon { get; set; }
        public string ToolTip { get; set; }
    }

    public class LanguageTypeItem
    {
        public int LangID { get; set; }
        public string Display { get; set; }
    }

    public class EnumItem:INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Display { get; set; }
        public Type Type { get; set; }

        private bool mIsSelected;
        public bool IsSelected
        {
            get { return mIsSelected; }
            set { mIsSelected = value; SubPropertyChanged("IsSelected"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SubPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }

    public class ObjectItem : INotifyPropertyChanged
    {
        /// <summary>
        /// 0       ScoreSheet
        /// 1       ScoreGroup
        /// 2       ScoreItem
        /// 3       Comment
        /// 4       CommentItem
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
    }

    [XmlRoot]
    public class TestClass
    {
        [XmlAttribute]
        public int ID { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
    }
}
