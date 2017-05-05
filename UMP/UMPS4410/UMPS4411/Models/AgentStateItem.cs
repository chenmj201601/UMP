//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e48dbcd8-0690-4310-9906-9629a86feeb7
//        CLR Version:              4.0.30319.18408
//        Name:                     AgentStateItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411.Models
//        File Name:                AgentStateItem
//
//        created by Charley at 2016/7/4 15:41:03
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using VoiceCyber.UMP.Common44101;


namespace UMPS4411.Models
{
    public class AgentStateItem : INotifyPropertyChanged
    {

        public long ObjID { get; set; }
        private int mNumber;

        public int Number
        {
            get { return mNumber; }
            set { mNumber = value; OnPropertyChanged("Number"); }
        }

        private string mName;

        public string Name
        {
            get { return mName; }
            set { mName = value; OnPropertyChanged("Name"); }
        }

        private int mSeatCount;

        public int SeatCount
        {
            get { return mSeatCount; }
            set { mSeatCount = value; OnPropertyChanged("SeatCount"); }
        }

        private int mStateType;

        public int StateType
        {
            get { return mStateType; }
            set { mStateType = value; OnPropertyChanged("StateType"); }
        }

        private int mStateValue;

        public int StateValue
        {
            get { return mStateValue; }
            set { mStateValue = value; OnPropertyChanged("StateValue"); }
        }

        private Brush mBrushHead;

        public Brush BrushHead
        {
            get { return mBrushHead; }
            set { mBrushHead = value; OnPropertyChanged("BrushHead"); }
        }

        private ObservableCollection<StateSeatItem> mListSeatItems = new ObservableCollection<StateSeatItem>();

        public ObservableCollection<StateSeatItem> ListSeatItems
        {
            get { return mListSeatItems; }
        }


        public AgentStateInfo Info { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        public override string ToString()
        {
            return string.Format("[{0}][{1}][{2}][{3}]", ObjID, Number, Name, SeatCount);
        }
    }
}
