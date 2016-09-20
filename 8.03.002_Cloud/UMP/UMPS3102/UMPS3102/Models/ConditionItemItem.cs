//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3d66fb11-0bb4-4f0b-bf05-f428ea66ecef
//        CLR Version:              4.0.30319.18444
//        Name:                     ConditionItemItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                ConditionItemItem
//
//        created by Charley at 2014/11/24 11:26:21
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.ComponentModel;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3102.Models
{
    public class ConditionItemItem : IDragDropObject, INotifyPropertyChanged
    {
        public long ID { get; set; }
        public string Name { get; set; }
        private string mDisplay;

        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }
        public int TabIndex { get; set; }
        public string TabName { get; set; }
        public int SortID { get; set; }
        public CustomConditionItemFormat Format { get; set; }
        private string mStrFormat;

        public UMPApp CurrentApp { get; set; }
        public string StrFormat
        {
            get { return mStrFormat; }
            set { mStrFormat = value; OnPropertyChanged("StrFormat"); }
        }
        public CustomConditionItemType Type { get; set; }

        private string mStrType;

        public string StrType
        {
            get { return mStrType; }
            set { mStrType = value; OnPropertyChanged("StrType"); }
        }
        public CustomConditionItem ConditionItem { get; set; }
        /// <summary>
        /// 是否已经添加到用户条件项
        /// </summary>
        public bool IsUserItem { get; set; }

        public ConditionItemItem(CustomConditionItem conditionItem, UMPApp currentApp)
        {
            ID = conditionItem.ID;
            Name = conditionItem.Name;
            Display = conditionItem.Name;
            TabIndex = conditionItem.TabIndex;
            TabName = conditionItem.TabName;
            SortID = conditionItem.SortID;
            Format = conditionItem.Format;
            Type = conditionItem.Type;
            ConditionItem = conditionItem;

            IsUserItem = false;
            CurrentApp = currentApp;
        }

        public void Apply()
        {
            if (ConditionItem != null)
            {
                ConditionItem.TabIndex = TabIndex;
                ConditionItem.TabName = TabName;
                ConditionItem.SortID = SortID;
            }
        }

        #region IDragDropObject

        public event EventHandler<DragDropEventArgs> StartDragged;

        public event EventHandler<DragDropEventArgs> DragOver;

        public event EventHandler<DragDropEventArgs> Dropped;

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


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
