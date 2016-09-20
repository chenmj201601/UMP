using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace UMPServicePack.PublicClasses
{
    public class UpdateProgram : INotifyPropertyChanged
    {
        public UpdateProgram()
        {
           
        }

        private string _ProgramName;

        /// <summary>
        /// 名称
        /// </summary>
        public string ProgramName
        {
            get { return _ProgramName; }
            set { _ProgramName = value; }
        }
        private string _Descript;

        /// <summary>
        /// 备注
        /// </summary>
        public string Descript
        {
            get { return _Descript; }
            set { _Descript = value; }
        }
        private bool _IsWait;
        /// <summary>
        /// 是否显示等待图标
        /// </summary>
        public bool IsWait
        {
            get { return _IsWait; }
            set { _IsWait = value;
            OnPropertyChanged("IsWait");
            }
        }
        private bool _IsDoing;

        /// <summary>
        /// 是否显示正在进行图标
        /// </summary>
        public bool IsDoing
        {
            get { return _IsDoing; }
            set { _IsDoing = value;
            OnPropertyChanged("IsDoing");
            }
        }
        private bool _IsDone;

        /// <summary>
        /// 是否已完成
        /// </summary>
        public bool IsDone
        {
            get { return _IsDone;}
            set { _IsDone = value;
            OnPropertyChanged("IsDone");
            }
        }

        private Brush mBackground;

        public Brush Background
        {
            get { return mBackground; }
            set
            {
                mBackground = value;
                OnPropertyChanged("Background");
            }
        }

        #region PropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }

        #endregion
    }
}
