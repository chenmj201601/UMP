using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UMPS1100
{
    public partial class UCSingleSkillGroupLeft : UserControl, INotifyPropertyChanged, S1100Interface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        public UCSkillGroupMaintenance IPageParent = null;

        public UCSingleSkillGroupLeft(DataRow ADataRowSkillGroupInfo)
        {
            InitializeComponent();
            IDataRow11009 = ADataRowSkillGroupInfo;
            GridSingleSkillGroupPanel.PreviewMouseLeftButtonDown += GridSingleSkillGroupPanel_PreviewMouseLeftButtonDown;
        }

        public void ShowSkillGroupInformation()
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            try
            {
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                ImageSkillGroup.Style = (Style)App.Current.Resources["ImageSkillStatus" + IDataRow11009["C004"].ToString() + "Style"];
                LStrParameterValueDB = IDataRow11009["C006"].ToString();
                StrSkillCode = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrParameterValueDB = IDataRow11009["C008"].ToString();
                StrSkillName = LStrParameterValueDB;
            }
            catch { }
        }

        public void SendClickedMessage()
        {
            try
            {
                if (IOperationEvent != null)
                {
                    OperationEventArgs LEventArgs = new OperationEventArgs();
                    LEventArgs.StrObjectTag = "M001";
                    LEventArgs.ObjectSource0 = IDataRow11009;
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GridSingleSkillGroupPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (IOperationEvent != null)
                {
                    OperationEventArgs LEventArgs = new OperationEventArgs();
                    LEventArgs.StrObjectTag = "M001";
                    LEventArgs.ObjectSource0 = IDataRow11009;
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region 属性定义
        private string _StrSkillCode;
        public string StrSkillCode
        {
            get { return _StrSkillCode; }
            set
            {
                _StrSkillCode = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrSkillCode");
                }
            }
        }

        private string _StrSkillName;
        public string StrSkillName
        {
            get { return _StrSkillName; }
            set
            {
                _StrSkillName = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrSkillName");
                }
            }
        }

        /// <summary>
        /// 当前功能的信息，对应表T_11_009中的一行数据
        /// </summary>
        public DataRow IDataRow11009 { get; set; }
        #endregion

        #region 属性值变化触发事件
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String StrPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(StrPropertyName));
            }
        }
        #endregion

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }
    }
}
