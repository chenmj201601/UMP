using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace UMP.PF.MAMT.Classes
{
    public class LanguageInfo : INotifyPropertyChanged
    {
        private string _LanguageCode;

        public string LanguageCode
        {
            get { return _LanguageCode; }
            set { _LanguageCode = value; }
        }
        private string _MessageID;

        public string MessageID
        {
            get { return _MessageID; }
            set { _MessageID = value; }
        }
        private string _DisplayMessage;

        public string DisplayMessage
        {
            get { return _DisplayMessage; }
            set
            {
                _DisplayMessage = value;
                OnPropertyChanged(DisplayMessage);
            }
        }
        private string _TipMessage;

        public string TipMessage
        {
            get { return _TipMessage; }
            set { _TipMessage = value; }
        }

        private string _ModuleID;

        public string ModuleID
        {
            get { return _ModuleID; }
            set { _ModuleID = value; }
        }
        private string _ChildModuleID;

        public string ChildModuleID
        {
            get { return _ChildModuleID; }
            set { _ChildModuleID = value; }
        }
        private string _InFrame;

        public string InFrame
        {
            get { return _InFrame; }
            set { _InFrame = value; }
        }
        private string _InObject;

        public string InObject
        {
            get { return _InObject; }
            set { _InObject = value; }
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
