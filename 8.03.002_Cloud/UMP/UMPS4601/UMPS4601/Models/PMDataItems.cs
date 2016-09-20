using Common4602;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS4601.Models
{
    public class PMDataItems : CheckableItemBase, IDragDropObject, INotifyPropertyChanged
    {
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

        /// <summary>
        /// T_46_1?.C001,T_46_003.C001
        /// </summary>
        public long KPIMappingID { get; set; }
        /// <summary>
        /// T46_011.C015,T46_01?.C117
        /// </summary>
        public long KPIID { get; set; }

        /// <summary>
        /// KPI名字，,T_46_001.C002
        /// </summary>
        public string KPIName { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 对象ID
        /// </summary>
        public long UERAId { get; set; }

        /// <summary>
        /// 对象全名
        /// </summary>
        public string UERName { get; set; }

        public long StartUTCTime { get; set; }

        public long StartLocalTime { get; set; }

        public string PMTime { get; set; }

        public int pmYear { get; set; }

        public int pmMonth { get; set; }

        public int pmDay { get; set; }

        /// <summary>
        /// 所属对象ID
        /// </summary>
        public long BelongsId { get; set; }

        /// <summary>
        /// 实际值,T_46_1?.C101
        /// </summary>
        public double ActualValue { get; set; }
        /// <summary>
        /// 目标 ,T_46_1?.C102
        /// </summary>
        public string Goal1 { get; set; }
        /// <summary>
        /// 趋势1   趋势（根据配置连续3或4期 与上一个周期比较的值形成趋势）1、上升 2 、N/A  -1、下降  0 持平,T_46_1?.C104
        /// </summary>
        public string Trend1 { get; set; }
        /// <summary>
        ///同比, 和上一个周期相比较，是升高还是下降，如果值是正的则表示上升了多少  如果值是负的则表下降了多少,T_46_1?.C109
        /// </summary>
        public string Compare { get; set; }
        /// <summary>
        /// 实际值/目标1,T_46_1?.C105
        /// </summary>
        public string Score { get; set; }
        /// <summary>
        ///得分,T_46_1?.C111
        /// </summary>
        public string BoundaryShow1 { get; set; }
        /// <summary>
        /// 同行目标
        /// </summary>
        public string Goal2 { get; set; }

        public Brush Background { get; set; }

        private List<PMDataItems> mPMlistItems;
        public List<PMDataItems> PMlistItems
        {
            get { return mPMlistItems; }
        }

        public PMDataItems()
        {
            mPMlistItems = new List<PMDataItems>();
        }

        public PMDataItems(PMShowDataItems showItems)
            : this()
        {
            KPIMappingID = showItems.KPIMappingID;
            KPIID = showItems.KPIID;
            KPIName = showItems.KPIName; 
            Name = showItems.UERName;
            UERAId = showItems.UERAId;
            UERName = showItems.UERName;
            StartLocalTime = showItems.StartLocalTime;
            StartUTCTime = showItems.StartUTCTime;
            pmYear = showItems.pmYear;
            pmMonth = showItems.pmMonth;
            pmDay = showItems.pmDay;
            BelongsId = showItems.BelongsId;
            ActualValue = showItems.ActualValue;
            Goal1 = decimal.Round(decimal.Parse(showItems.Goal1), 2).ToString();//四舍五入
            Trend1 = showItems.Trend1;
            Compare = decimal.Round(decimal.Parse(showItems.Compare), 2).ToString();
            Score = decimal.Round(decimal.Parse(showItems.ActualGoal1), 2).ToString();
            BoundaryShow1 = showItems.BoundaryShow1;
            Goal2 = decimal.Round(decimal.Parse(showItems.Goal2), 2).ToString();
        }
    }
}
