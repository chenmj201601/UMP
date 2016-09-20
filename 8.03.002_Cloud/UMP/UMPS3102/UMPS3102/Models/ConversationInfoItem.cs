using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common31021;

namespace UMPS3102.Models
{
    public class ConversationInfoItem : INotifyPropertyChanged
    {
        public string SerialID { get; set; }
        public string RecordReference { get; set; }
        public DateTime StartRecordTime { get; set; }
        public long Offset { get; set; }
        public DateTime EndRecordTime { get; set; }
        public string Extension { get; set; }
        public string Direction { get; set; }
        public string RowNumber { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 结束位置的偏移量
        /// </summary>
        public long EndOffset { get; set; }
        /// <summary>
        /// 根据Direction选择图标
        /// </summary>
        public string Icon { get; set; }

        public string StrOffset { get; set; }
        //public string ToStart

        private string mColor;

        public string Color
        {
            get { return mColor; }
            set 
            { 
                mColor = value; 
                OnPropertyChanged("Color"); 
            }
        }

        private bool mIsVisible;

        public bool IsVisible
        {
            get { return mIsVisible; }
            set { mIsVisible = value; OnPropertyChanged("IsVisible"); }
        }

        public ConversationInfoItem(ConversationInfo conversationInfo)
        {
            SerialID = conversationInfo.SerialID;
            RecordReference = conversationInfo.RecordReference;
            StartRecordTime = conversationInfo.StartRecordTime;
            Offset = conversationInfo.Offset;
            Extension = conversationInfo.Extension;
            Direction = conversationInfo.Direction;
            RowNumber = conversationInfo.RowNumber;
            Content = conversationInfo.Content;
            EndRecordTime = conversationInfo.EndRecordTime;
            //结束位置的偏移量 = 开始位置偏移量+该条句子的录音长度
            EndOffset = Offset + Convert.ToInt64((EndRecordTime - StartRecordTime).TotalMilliseconds);
            StrOffset = Converter.MilliSecond2Time(conversationInfo.Offset);
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
