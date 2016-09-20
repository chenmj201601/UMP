using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
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
using UMPS1100.WCFService00000;
using VoiceCyber.UMP.Communications;

namespace UMPS1100
{
    public partial class UCSkillGroupMaintenance : UserControl, INotifyPropertyChanged
    {
        public PageMainEntrance IPageParent = null;

        private bool IBoolIsBusy = false;

        private string IStrCurrentOrgTypeID = string.Empty;
        private string IStrCurrentMethod = string.Empty;
        DataRow IDataRowCurrentFocused = null;

        public UCSkillGroupMaintenance()
        {
            InitializeComponent();
            IStrCurrentMethod = "V";
            this.Loaded += UCSkillGroupMaintenance_Loaded;
            ButtonEditApply.Click += ButtonEditApplyCancelClick;
            ButtonCancelEdit.Click += ButtonEditApplyCancelClick;
            ButtonAddSkillGroup.Click += ButtonEditApplyCancelClick;
            ButtonDelSkillGroup.Click += ButtonEditApplyCancelClick;
        }

        #region 按钮事件处理部分
        private void ButtonEditApplyCancelClick(object sender, RoutedEventArgs e)
        {
            string LStrSenderName = string.Empty;

            try
            {
                LStrSenderName = (sender as Button).Name;
                if (LStrSenderName == "ButtonEditApply")
                {
                    if (IStrCurrentMethod == "V") { ShowSkillGroupEditStyle(); return; }
                    if (IStrCurrentMethod == "E" || IStrCurrentMethod == "A") { SaveSkillGroupEdited(); return; }
                }
                if (LStrSenderName == "ButtonCancelEdit")
                {
                    ShowSingleSkillGroupInformation();
                    return;
                }
                if (LStrSenderName == "ButtonDelSkillGroup")
                {
                    RemoveSkillGroup();
                    return;
                }
                if (LStrSenderName == "ButtonAddSkillGroup")
                {
                    AddSillGroupOperation();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowSkillGroupEditStyle()
        {
            IStrCurrentMethod = "E";

            ButtonEditApply.Style = (Style)App.Current.Resources["ButtonApplyStyle"];
            StrEditApply = App.GetDisplayCharater("UCOrganizationMaintenance", "ButtonApply");
            ButtonCancelEdit.Visibility = System.Windows.Visibility.Visible;
            TextBoxSkillCode.IsReadOnly = false;
            TextBoxSkillName.IsReadOnly = false;
            CheckBoxIsEnabled.IsEnabled = true;
            TextBoxSingleTypeDescriber.IsReadOnly = false;
        }

        private void AddSillGroupOperation()
        {
            IDataRowCurrentFocused = null;
            IStrCurrentMethod = "A";
            BorderSingeOrgDetail.Visibility = System.Windows.Visibility.Visible;
            ButtonEditApply.Style = (Style)App.Current.Resources["ButtonApplyStyle"];
            StrEditApply = App.GetDisplayCharater("UCOrganizationMaintenance", "ButtonApply");
            ButtonCancelEdit.Visibility = System.Windows.Visibility.Collapsed;
            TextBoxSkillCode.IsReadOnly = false;
            TextBoxSkillName.IsReadOnly = false;
            CheckBoxIsEnabled.IsEnabled = true;
            TextBoxSingleTypeDescriber.IsReadOnly = false;

            TextBoxSkillCode.Text = string.Empty;
            TextBoxSkillName.Text = string.Empty;
            CheckBoxIsEnabled.IsChecked = true;
            TextBoxSingleTypeDescriber.Text = string.Empty;
        }

        #endregion

        #region 保存数据至数据库
        private BackgroundWorker IBWSaveData = null;
        private bool IBoolCallReturn = true;
        private string IStrCallReturn = string.Empty;
        private List<string> IListStrAfterSave = new List<string>();

        private void SaveSkillGroupEdited()
        {
            List<string> LListStrWcfArgs = new List<string>();
            string LStrSkillCode = string.Empty, LStrSkillName = string.Empty;
            string LStrTypeDescriber = string.Empty;

            try
            {
                LStrSkillCode = TextBoxSkillCode.Text.Trim();
                if (string.IsNullOrEmpty(LStrSkillCode))
                {
                    MessageBox.Show(App.GetDisplayCharater("S1100020"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (LStrSkillCode.Length > 128)
                {
                    MessageBox.Show(App.GetDisplayCharater("S1100028"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                LStrSkillName = TextBoxSkillName.Text.Trim();
                if (string.IsNullOrEmpty(LStrSkillName))
                {
                    MessageBox.Show(App.GetDisplayCharater("S1100021"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (LStrSkillName.Length > 128)
                {
                    MessageBox.Show(App.GetDisplayCharater("S1100029"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                LStrTypeDescriber = TextBoxSingleTypeDescriber.Text.Trim();
                if (LStrTypeDescriber.Length > 1024)
                {
                    MessageBox.Show(App.GetDisplayCharater("S1100031"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());                                  //0
                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());                              //1
                LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);                                                  //2
                LListStrWcfArgs.Add("2");                                                                                   //3
                if (IStrCurrentMethod == "E" || IStrCurrentMethod == "D")                                                   //4
                { LListStrWcfArgs.Add(IDataRowCurrentFocused["C001"].ToString()); }
                else { LListStrWcfArgs.Add("0"); }
                if (CheckBoxIsEnabled.IsChecked == true)                                                                    //5
                { LListStrWcfArgs.Add("1"); }
                else { LListStrWcfArgs.Add("0"); }
                LListStrWcfArgs.Add(LStrSkillCode);                                                                       //6
                LListStrWcfArgs.Add(App.GClassSessionInfo.UserID.ToString());                                               //7
                LListStrWcfArgs.Add(LStrTypeDescriber);                                                //8
                LListStrWcfArgs.Add(LStrSkillName);                                                                         //9
                if (IStrCurrentMethod == "E" || IStrCurrentMethod == "D")                                                   //10
                { LListStrWcfArgs.Add(IDataRowCurrentFocused["C002"].ToString()); }
                else
                {
                    LListStrWcfArgs.Add((App.IDataTable11009.Select("C000 = 2", "C002 ASC").Length + 1).ToString());
                }
                LListStrWcfArgs.Add("");                                                                                    //11
                LListStrWcfArgs.Add(IStrCurrentMethod);                                                                     //12

                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12111;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                IBoolIsBusy = true;

                IBWSaveData = new BackgroundWorker();
                IBWSaveData.RunWorkerCompleted += IBWSaveData_RunWorkerCompleted;
                IBWSaveData.DoWork += IBWSaveData_DoWork;
                IBWSaveData.RunWorkerAsync(LListStrWcfArgs);
            }
            catch (Exception ex)
            {
                IBoolIsBusy = false;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12112;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                if (IBWSaveData != null)
                {
                    IBWSaveData.Dispose(); IBWSaveData = null;
                }
                MessageBox.Show(ex.ToString());
            }
        }

        private void RemoveSkillGroup()
        {
            List<string> LListStrWcfArgs = new List<string>();
            string LStrVerificationCode104 = string.Empty;

            try
            {
                if (IDataRowCurrentFocused == null) { return; }

                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                string LStrSkillCode = IDataRowCurrentFocused["C006"].ToString();
                LStrSkillCode = EncryptionAndDecryption.EncryptDecryptString(LStrSkillCode, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                if (MessageBox.Show(string.Format(App.GetDisplayCharater("S1100015"), LStrSkillCode), App.GClassSessionInfo.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes) { return; }
                IStrCurrentMethod = "D";

                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());                                  //0
                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());                              //1
                LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);                                                  //2
                LListStrWcfArgs.Add("2");                                                                                   //3
                LListStrWcfArgs.Add(IDataRowCurrentFocused["C001"].ToString());                                             //4
                LListStrWcfArgs.Add("");                                                                                    //5
                LListStrWcfArgs.Add(LStrSkillCode);                                                                       //6
                LListStrWcfArgs.Add(App.GClassSessionInfo.UserID.ToString());                                               //7
                LListStrWcfArgs.Add("");                                                                                    //8
                LListStrWcfArgs.Add("");                                                                                    //9
                LListStrWcfArgs.Add(IDataRowCurrentFocused["C002"].ToString());                                             //10
                LListStrWcfArgs.Add("");                                                                                    //11
                LListStrWcfArgs.Add(IStrCurrentMethod);                                                                     //12

                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12111;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                IBoolIsBusy = true;

                IBWSaveData = new BackgroundWorker();
                IBWSaveData.RunWorkerCompleted += IBWSaveData_RunWorkerCompleted;
                IBWSaveData.DoWork += IBWSaveData_DoWork;
                IBWSaveData.RunWorkerAsync(LListStrWcfArgs);
            }
            catch (Exception ex)
            {
                IBoolIsBusy = false;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12112;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                if (IBWSaveData != null)
                {
                    IBWSaveData.Dispose(); IBWSaveData = null;
                }
                MessageBox.Show(ex.ToString());
            }
        }

        private void IBWSaveData_DoWork(object sender, DoWorkEventArgs e)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                LListStrWcfArgs = e.Argument as List<string>;

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(22, LListStrWcfArgs);
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                IListStrAfterSave.Clear();
                if (IBoolCallReturn)
                {
                    foreach (string LStrSingleArgs in LListStrWcfArgs) { IListStrAfterSave.Add(LStrSingleArgs); }
                    IListStrAfterSave[4] = IStrCallReturn;
                }
            }
            catch (Exception ex)
            {
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
            }
        }

        private void IBWSaveData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                IBoolIsBusy = false;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12112;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                if (!IBoolCallReturn)
                {
                    if (IStrCallReturn == "S906E01")
                    {
                        MessageBox.Show(App.GetDisplayCharater("S1100024"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show(App.GetDisplayCharater("S1100016") + "\n" + IStrCallReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return;
                }
                ResetAllSkillGroupList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                if (IBWSaveData != null)
                {
                    IBWSaveData.Dispose(); IBWSaveData = null;
                }
            }
        }

        private void ResetAllSkillGroupList()
        {
            if (IListStrAfterSave[12] == "A")
            {
                DataRow LDataRowNew = App.IDataTable11009.NewRow();
                LDataRowNew.BeginEdit();
                LDataRowNew["C000"] = 2;
                LDataRowNew["C001"] = long.Parse(IListStrAfterSave[4]);
                LDataRowNew["C002"] = Int16.Parse(IListStrAfterSave[10]);
                LDataRowNew["C003"] = 0;
                LDataRowNew["C004"] = IListStrAfterSave[5];
                LDataRowNew["C005"] = 2;
                LDataRowNew["C006"] = IListStrAfterSave[6];
                LDataRowNew["C007"] = IListStrAfterSave[7];
                LDataRowNew["C008"] = IListStrAfterSave[9];
                LDataRowNew["C009"] = IListStrAfterSave[8];
                LDataRowNew["C010"] = "";
                LDataRowNew["C011"] = "";
                LDataRowNew.EndEdit();
                App.IDataTable11009.Rows.Add(LDataRowNew);

                UCSingleSkillGroupLeft LUCSingleSkillGroup = new UCSingleSkillGroupLeft(LDataRowNew);
                LUCSingleSkillGroup.IPageParent = this;
                LUCSingleSkillGroup.IOperationEvent += LUCSingleSkillGroup_IOperationEvent;
                LUCSingleSkillGroup.ShowSkillGroupInformation();
                LUCSingleSkillGroup.Margin = new Thickness(0, 1, 0, 1);
                StackPanelAllOrgTypeDetail.Children.Add(LUCSingleSkillGroup);
                LUCSingleSkillGroup.BringIntoView();
                LUCSingleSkillGroup.SendClickedMessage();
            }
            if (IListStrAfterSave[12] == "E")
            {
                foreach (object LObjectSingleOrgType in StackPanelAllOrgTypeDetail.Children)
                {
                    UCSingleSkillGroupLeft LUCSingleSkillGroup = LObjectSingleOrgType as UCSingleSkillGroupLeft;
                    if (LUCSingleSkillGroup.IDataRow11009["C001"].ToString() == IListStrAfterSave[4])
                    {
                        LUCSingleSkillGroup.IDataRow11009["C004"] = IListStrAfterSave[5];
                        LUCSingleSkillGroup.IDataRow11009["C006"] = IListStrAfterSave[6];
                        LUCSingleSkillGroup.IDataRow11009["C008"] = IListStrAfterSave[9];
                        LUCSingleSkillGroup.IDataRow11009["C009"] = IListStrAfterSave[8];
                        LUCSingleSkillGroup.ShowSkillGroupInformation();

                        IDataRowCurrentFocused["C004"] = IListStrAfterSave[5];
                        IDataRowCurrentFocused["C006"] = IListStrAfterSave[6];
                        IDataRowCurrentFocused["C008"] = IListStrAfterSave[9];
                        IDataRowCurrentFocused["C009"] = IListStrAfterSave[8];
                        ShowSingleSkillGroupInformation();
                        return;
                    }
                }
            }

            if (IListStrAfterSave[12] == "D")
            {
                IDataRowCurrentFocused = null;
                BorderSingeOrgDetail.Visibility = System.Windows.Visibility.Collapsed;
                foreach (object LObjectSingleOrgType in StackPanelAllOrgTypeDetail.Children)
                {
                    UCSingleSkillGroupLeft LUCSingleSkillGroup = LObjectSingleOrgType as UCSingleSkillGroupLeft;
                    if (LUCSingleSkillGroup.IDataRow11009["C001"].ToString() == IListStrAfterSave[4])
                    {
                        StackPanelAllOrgTypeDetail.Children.Remove(LUCSingleSkillGroup);
                        return;
                    }
                }
            }
        }
        #endregion

        public void ShowAllSkillGroup()
        {
            try
            {
                BorderSingeOrgDetail.Visibility = System.Windows.Visibility.Collapsed;
                StackPanelAllOrgTypeDetail.Children.Clear();
                DataRow[] LDataRowAllSkillGroup = App.IDataTable11009.Select("C000 = 2", "C002 ASC");
                foreach (DataRow LDataRowSkillGroup in LDataRowAllSkillGroup)
                {
                    UCSingleSkillGroupLeft LUCSingleSkillGroup = new UCSingleSkillGroupLeft(LDataRowSkillGroup);
                    LUCSingleSkillGroup.IPageParent = this;
                    LUCSingleSkillGroup.IOperationEvent += LUCSingleSkillGroup_IOperationEvent;
                    LUCSingleSkillGroup.ShowSkillGroupInformation();
                    LUCSingleSkillGroup.Margin = new Thickness(0, 1, 0, 1);
                    StackPanelAllOrgTypeDetail.Children.Add(LUCSingleSkillGroup);
                }
                ShowElementContent();
            }
            catch { }
        }

        public void ShowElementContent()
        {
            LabelAllOrgTypeName.Content = App.GetDisplayCharater("S1100012");

            if (IStrCurrentMethod == "V")
            {
                StrEditApply = App.GetDisplayCharater("UCOrganizationMaintenance", "ButtonEdit");
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonEditStyle"];
            }
            else
            {
                StrEditApply = App.GetDisplayCharater("UCOrganizationMaintenance", "ButtonApply");
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonApplyStyle"];
            }

            StrCancel = App.GetDisplayCharater("UCOrganizationMaintenance", "ButtonCancel");
            StrAddOrg = App.GetDisplayCharater("S1100025");
            StrDelOrg = App.GetDisplayCharater("S1100026");

            StrHeader = App.GetDisplayCharater("UCOrganizationMaintenance", "GroupBoxTypeValueHeader");
            StrDescriber = App.GetDisplayCharater("UCOrganizationMaintenance", "GroupBoxTypeDescriber");

            LabelSkillCode.Content = App.GetDisplayCharater("S1100013");
            LabelSkillName.Content = App.GetDisplayCharater("S1100014");
            LabelSkillStatus.Content = App.GetDisplayCharater("S1100008");
            CheckBoxIsEnabled.Content = App.GetDisplayCharater("S1100009");
        }

        #region 单击单个技能组，处理
        private void LUCSingleSkillGroup_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (IBoolIsBusy) { return; }

            BorderSingeOrgDetail.Visibility = System.Windows.Visibility.Visible;
            DataRow LDataRowParamInfo = e.ObjectSource0 as DataRow;
            IDataRowCurrentFocused = LDataRowParamInfo;
            ShowSingleSkillGroupInformation();
        }

        private void ShowSingleSkillGroupInformation()
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            try
            {
                TextBoxSkillCode.IsReadOnly = true;
                TextBoxSkillName.IsReadOnly = true;
                CheckBoxIsEnabled.IsEnabled = false;
                TextBoxSingleTypeDescriber.IsReadOnly = true;

                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                StrEditApply = App.GetDisplayCharater("UCOrganizationMaintenance", "ButtonEdit");
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonEditStyle"];
                ButtonCancelEdit.Visibility = System.Windows.Visibility.Collapsed;
                IStrCurrentMethod = "V";

                CheckBoxIsEnabled.IsChecked = false;
                TextBoxSingleTypeDescriber.Text = string.Empty;

                LStrParameterValueDB = IDataRowCurrentFocused["C006"].ToString();
                TextBoxSkillCode.Text = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrParameterValueDB = IDataRowCurrentFocused["C008"].ToString();

                TextBoxSkillName.Text = LStrParameterValueDB;// EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                if (IDataRowCurrentFocused["C004"].ToString() == "1") { CheckBoxIsEnabled.IsChecked = true; }

                LStrParameterValueDB = IDataRowCurrentFocused["C009"].ToString();
                TextBoxSingleTypeDescriber.Text = LStrParameterValueDB;
            }
            catch { }
        }
        #endregion

        #region 属性定义
        private string _StrEditApply;
        public string StrEditApply
        {
            get { return _StrEditApply; }
            set
            {
                _StrEditApply = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrEditApply"); }
            }
        }

        private string _StrCancel;
        public string StrCancel
        {
            get { return _StrCancel; }
            set
            {
                _StrCancel = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrCancel"); }
            }
        }

        private string _StrTypeName;
        public string StrTypeName
        {
            get { return _StrTypeName; }
            set
            {
                _StrTypeName = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrTypeName"); }
            }
        }

        private string _StrHeader;
        public string StrHeader
        {
            get { return _StrHeader; }
            set
            {
                _StrHeader = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrHeader"); }
            }
        }

        private string _StrDescriber;
        public string StrDescriber
        {
            get { return _StrDescriber; }
            set
            {
                _StrDescriber = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrDescriber"); }
            }
        }

        private string _StrTypeDesc;
        public string StrTypeDesc
        {
            get { return _StrTypeDesc; }
            set
            {
                _StrTypeDesc = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrTypeDesc"); }
            }
        }

        private string _StrAddOrg;
        public string StrAddOrg
        {
            get { return _StrAddOrg; }
            set
            {
                _StrAddOrg = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrAddOrg"); }
            }
        }

        private string _StrDelOrg;
        public string StrDelOrg
        {
            get { return _StrDelOrg; }
            set
            {
                _StrDelOrg = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrDelOrg"); }
            }
        }

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

        #region 处理父窗口发送过来的语言切换
        private void UCSkillGroupMaintenance_Loaded(object sender, RoutedEventArgs e)
        {
            IPageParent.IChangeLanguageEvent += IPageParent_IChangeLanguageEvent;
        }

        private void IPageParent_IChangeLanguageEvent(object sender, OperationEventArgs e)
        {
            ShowElementContent();
        }
        #endregion

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }
    }
}
