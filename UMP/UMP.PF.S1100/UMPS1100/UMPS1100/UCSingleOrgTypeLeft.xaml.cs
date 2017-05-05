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
    public partial class UCSingleOrgTypeLeft : UserControl, INotifyPropertyChanged, S1100Interface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        public UCOrganizationMaintenance IPageParent = null;

        public UCSingleOrgTypeLeft(DataRow ADataRowOrgTypeInfo)
        {
            InitializeComponent();
            IDataRow11009 = ADataRowOrgTypeInfo;
            GridSingleOrgTypePanel.PreviewMouseLeftButtonDown += GridSingleOrgTypePanel_PreviewMouseLeftButtonDown;
        }

        public void ShowOrgTypeInformation()
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;
            string LStrOrgTypeID = string.Empty;

            try
            {
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                ImageOrgStatus.Style = (Style)App.Current.Resources["ImageOrgStatus" + IDataRow11009["C004"].ToString() + "Style"];
                LStrOrgTypeID = IDataRow11009["C001"].ToString();
                if (LStrOrgTypeID == "905" + App.GClassSessionInfo.RentInfo.Token + "00000000000")
                {
                    StrOrgTypeName = App.GetDisplayCharater("S1100018");
                }
                else
                {
                    LStrParameterValueDB = IDataRow11009["C006"].ToString();
                    StrOrgTypeName = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                }
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

        private void GridSingleOrgTypePanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
        private string _StrOrgTypeName;
        public string StrOrgTypeName
        {
            get { return _StrOrgTypeName; }
            set
            {
                _StrOrgTypeName = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("StrOrgTypeName");
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
