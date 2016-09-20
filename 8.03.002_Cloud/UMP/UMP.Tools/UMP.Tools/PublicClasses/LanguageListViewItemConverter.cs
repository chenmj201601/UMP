using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace UMP.Tools.PublicClasses
{
    public class LanguageListViewItemSearchConverter : IValueConverter
    {
        public object Convert(object AObjValue, Type ATypeTarget, object AObjParameter, System.Globalization.CultureInfo ACultureInfo)
        {
            if (AObjValue == null)
            {
                return Brushes.Transparent;
            }
            var LVarStatus = (ListViewItemSearchStatus)Enum.Parse(typeof(ListViewItemSearchStatus), AObjValue.ToString());
            switch (LVarStatus)
            {
                case ListViewItemSearchStatus.IsDefault:
                    return Brushes.Transparent;
                case ListViewItemSearchStatus.IsSearched:
                    //满足查找结果的所有Item
                    return Brushes.LightGoldenrodYellow;
                case ListViewItemSearchStatus.IsCurrent:
                    return Brushes.WhiteSmoke;
                default:
                    return Brushes.Transparent;
            }
        }

        public object ConvertBack(object AObjValue, Type ATypeTarget, object AObjParameter, System.Globalization.CultureInfo ACultureInfo)
        {
            return null;
        }
    }

    public class LanguageListViewItemDataChangeConverter : IValueConverter
    {
        public object Convert(object AObjValue, Type ATypeTarget, object AObjParameter, System.Globalization.CultureInfo ACultureInfo)
        {
            if (AObjValue == null)
            {
                return Brushes.Transparent;
            }
            var LVarStatus = (ListViewItemDataChangeStatus)Enum.Parse(typeof(ListViewItemDataChangeStatus), AObjValue.ToString());
            switch (LVarStatus)
            {
                case ListViewItemDataChangeStatus.IsDefault:
                    return Brushes.Black;
                case ListViewItemDataChangeStatus.IsChanged:
                    return Brushes.Red;
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object AObjValue, Type ATypeTarget, object AObjParameter, System.Globalization.CultureInfo ACultureInfo)
        {
            return null;
        }
    }

    public class LanguageListViewItemTipChangeConverter : IValueConverter
    {
        public object Convert(object AObjValue, Type ATypeTarget, object AObjParameter, System.Globalization.CultureInfo ACultureInfo)
        {
            if (AObjValue == null)
            {
                return Brushes.Transparent;
            }
            var LVarStatus = (ListViewItemDataChangeStatus)Enum.Parse(typeof(ListViewItemDataChangeStatus), AObjValue.ToString());
            switch (LVarStatus)
            {
                case ListViewItemDataChangeStatus.IsDefault:
                    return Brushes.Black;
                case ListViewItemDataChangeStatus.IsChanged:
                    return Brushes.Red;
                default:
                    return Brushes.Black;
            }
        }

        public object ConvertBack(object AObjValue, Type ATypeTarget, object AObjParameter, System.Globalization.CultureInfo ACultureInfo)
        {
            return null;
        }
    }

    /// <summary>
    /// 单个ListViewItem定义类
    /// </summary>
    public class ListViewItemSingle : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int IntItemIndex { get; set; }

        private ListViewItemSearchStatus _SearchStatus;
        private ListViewItemDataChangeStatus _DataChangeStatus;
        private ListViewItemTipChangeStatus _TipChangeStatus;

        public int LanguageCode { get; set; }
        public string MessageID { get; set; }
        public int MessageServerity { get; set; }
        public int MessageLevel { get; set; }
        public string MessageContentText01 { get; set; }
        public string MessageContentText02 { get; set; }
        public string MessageTipDisplay01 { get; set; }
        public string MessageTipDisplay02 { get; set; }
        public int BelongModuleID { get; set; }
        public int BelongSubModuleID { get; set; }
        public string InFrameOrPage { get; set; }
        public string ObjectName { get; set; }

        public string MessageContentText
        {
            get { return MessageContentText01 + MessageContentText02; }
            set
            {
                MessageContentText01 = YoungParseString(value.ToString(), "01");
                MessageContentText02 = YoungParseString(value.ToString(), "02");
            }
        }

        public string MessageTipDisplay
        {
            get { return MessageTipDisplay01 + MessageTipDisplay02; }
            set
            {
                MessageTipDisplay01 = YoungParseString(value.ToString(), "01");
                MessageTipDisplay02 = YoungParseString(value.ToString(), "02");
            }
        }

        public string MessageContentTextOld { get; set; }

        public string MessageTipDisplayOld { get; set; }

        private string YoungParseString(string AStrSource, string AStrSection)
        {
            string LStrReturn = string.Empty;
            int LIntSourceLen = 0;

            try
            {
                LIntSourceLen = AStrSource.Length;
                if (AStrSection == "01")
                {
                    if (LIntSourceLen <= 1024) { LStrReturn = AStrSource; } else { LStrReturn = AStrSource.Substring(0, 1024); }
                }
                if (AStrSection == "02")
                {
                    if (LIntSourceLen > 1024) { LStrReturn = AStrSource.Substring(1024); } else { LStrReturn = string.Empty; }
                }

                if (LStrReturn.Length > 1024) { LStrReturn = LStrReturn.Substring(0, 1024); }
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public ListViewItemSearchStatus SearchStatus
        {
            get { return _SearchStatus; }
            set
            {
                _SearchStatus = value;
                OnPropertyChanged("SearchStatus");
            }
        }

        public ListViewItemDataChangeStatus DataChangeStatus
        {
            get { return _DataChangeStatus; }
            set
            {
                _DataChangeStatus = value;
                OnPropertyChanged("DataChangeStatus");
            }
        }

        public ListViewItemTipChangeStatus TipChangeStatus
        {
            get { return _TipChangeStatus; }
            set
            {
                _TipChangeStatus = value;
                OnPropertyChanged("TipChangeStatus");
            }
        }

        private void OnPropertyChanged(string AStrPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(AStrPropertyName));
            }
        }
    }

    /// <summary>
    /// 当前ListViewItem查找的状态
    /// </summary>
    [Flags]
    public enum ListViewItemSearchStatus
    {
        IsDefault = 0,
        IsSearched = 1,
        IsCurrent = 2,
    }

    /// <summary>
    /// 当前ListViewItem显示数据改变状态
    /// </summary>
    [Flags]
    public enum ListViewItemDataChangeStatus
    {
        IsDefault = 0,
        IsChanged = 1
    }

    /// <summary>
    /// 当前ListViewItem提示数据改变状态
    /// </summary>
    [Flags]
    public enum ListViewItemTipChangeStatus
    {
        IsDefault = 0,
        IsChanged = 1
    }
}
