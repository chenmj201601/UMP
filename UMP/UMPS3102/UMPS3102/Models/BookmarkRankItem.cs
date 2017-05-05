//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    904691ef-29ec-4cd9-bb9e-9bd30b8ee50a
//        CLR Version:              4.0.30319.18444
//        Name:                     BookmarkRankItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                BookmarkRankItem
//
//        created by Charley at 2014/12/11 13:50:59
//        http://www.voicecyber.com 
//
//======================================================================

using System.ComponentModel;
using VoiceCyber.UMP.Common31021;

namespace UMPS3102.Models
{
    public class BookmarkRankItem:INotifyPropertyChanged
    {
        public long ID { get; set; }
        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private int mOrderID;

        public int OrderID
        {
            get { return mOrderID; }
            set { mOrderID = value; OnPropertyChanged("OrderID"); }
        }

        private string mColor;

        public string Color
        {
            get { return mColor; }
            set { mColor = value; OnPropertyChanged("Color"); }
        }
        public string RandType { get; set; }

        public BookmarkRankInfo BookmarkRankInfo { get; set; }

        public BookmarkRankItem(BookmarkRankInfo info)
        {
            ID = info.ID;
            Name = info.Name;
            OrderID = info.OrderID;
            Color = info.Color;
            RandType = info.RankType;

            BookmarkRankInfo = info;
        }

        public void SetValues()
        {
            if (BookmarkRankInfo != null)
            {
                BookmarkRankInfo.OrderID = OrderID;
                BookmarkRankInfo.Name = Name;
                BookmarkRankInfo.Color = Color;
            }
        }

        public override string ToString()
        {
            return Name;
        }

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
