using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common31081;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3108.Models
{
    /// <summary>
    /// 能组合的统计参数子项
    /// </summary>
    public class CombinedParamItemModel : IDragDropObject, INotifyPropertyChanged
    {
        /// <summary>
        /// 大项参数的ID (如果为0,那么就为可组合的参数里面的没有放入参数大项的参数小巷)
        /// </summary>
        public long ID { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 参数子项的ID
        /// </summary>
        public string StatisticalParamItemID { get; set; }

        private string mDisplay;
        public string Display
        {
            get { return mDisplay; }
            set { mDisplay = value; OnPropertyChanged("Display"); }
        }
        public int TabIndex { get; set; }
        public string TabName { get; set; }
        public int SortID { get; set; }
        public CombStatiParaItemsFormat Format { get; set; }

        
        private string mStrFormat;
        public string StrFormat
        {
            get { return mStrFormat; }
            set { mStrFormat = value; OnPropertyChanged("StrFormat"); }
        }
        public StatisticalParamItemType Type { get; set; }

        private string mStrType;

        public string StrType
        {
            get { return mStrType; }
            set { mStrType = value; OnPropertyChanged("StrType"); }
        }
        public StatisticalParamItem ParamItem { get; set; }

        public UMPApp CurrentApp { get; set; }

        public ParamItemViewItem ParamItemViewItem_ { get; set; }
        /// <summary>
        /// 是否已经添加到(服务态度或专业水平)
        /// </summary>
        public bool IsAddedItem { get; set; }

        public CombinedParamItemModel(StatisticalParamItem paramItem, UMPApp currentApp)
        {
            ID = paramItem.StatisticalParamID;
            StatisticalParamItemID = paramItem.StatisticalParamItemID.ToString();
            Name = paramItem.StatisticalParamItemName;
            Display = paramItem.StatisticalParamItemName;
            TabIndex = paramItem.TabIndex;
            TabName = paramItem.TabName;
            SortID = paramItem.SortID;
            Format = paramItem.Formart;
            Type = paramItem.Type;
            ParamItem = paramItem;
            CurrentApp = currentApp;
            IsAddedItem = false;
        }

        public void Apply()
        {
            if (ParamItem != null)
            {
                ParamItem.TabIndex = TabIndex;
                ParamItem.TabName = TabName;
                ParamItem.SortID = SortID;
                ParamItem.StatisticalParamID = ID;
            }
        }

        public void Remove()
        {
            if (ParamItem != null)
            {
                ParamItem.TabIndex = 0;
                ParamItem.TabName = string.Empty;
                ParamItem.SortID = 0;
                ParamItem.StatisticalParamID = 0;
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