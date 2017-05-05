using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using UMPS1106.WCFService00000;
using VoiceCyber.UMP.Communications;

namespace UMPS1106
{
    public partial class Page00000A : Page, INotifyPropertyChanged, S1106Interface,S1106ChangeLanguageInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;
        public event EventHandler<OperationEventArgs> IChangeLanguageEvent;

        private bool IBoolIsBusy = false;

        public string IStrSpliterChar = string.Empty;
        private string IStrCurrentParameterID = string.Empty;
        private string IStrCurrentMethod = string.Empty;
        DataRow IDataRowCurrentFocused = null;

        public Page00000A()
        {
            InitializeComponent();
            this.Loaded += Page00000A_Loaded;
            IStrCurrentMethod = "V";
            ButtonEditApply.Click += ButtonEditApplyCancelClick;
            ButtonCancelEdit.Click += ButtonEditApplyCancelClick;
            IStrSpliterChar = App.AscCodeToChr(27);
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
                    if (IStrCurrentMethod == "V") { ShowParameterEditStyle(); return; }
                    if (IStrCurrentMethod == "E") { SaveParameterEdited(); return; }
                }
                if (LStrSenderName == "ButtonCancelEdit")
                {
                    IOperationEvent = null;
                    ShowSingleParameterDetailInfo();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowParameterEditStyle()
        {
            string LStr11001003 = string.Empty;     //参数编码
            int LInt00003009 = 0;                   //显示格式
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;
            string LStrParameterValueSub = string.Empty;
            string LStrCanEditeCheckReturn = string.Empty;

            LStr11001003 = IDataRowCurrentFocused["C003"].ToString();
            LStrCanEditeCheckReturn = ParameterCanEditCheck(LStr11001003);
            if (LStrCanEditeCheckReturn == "0")
            {
                MessageBox.Show(App.GetDisplayCharater("Page00000A", "CRV0" + LStr11001003), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IStrCurrentMethod = "E";

            ButtonEditApply.Style = (Style)App.Current.Resources["ButtonApplyStyle"];
            StrEditApply = App.GetDisplayCharater("Page00000A", "ButtonApply");
            ButtonCancelEdit.Visibility = System.Windows.Visibility.Visible;
            GridViewOrEditValue.Children.Clear();

            LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            

            LStrParameterValueDB = IDataRowCurrentFocused["C006"].ToString();
            LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrParameterValueSub = LStrParameterValueDB.Substring(8);
            LInt00003009 = int.Parse(IDataRowCurrentFocused["C009"].ToString());
            switch (LInt00003009)
            {
                case 0:
                    UCSetType000 LUCSetType000 = new UCSetType000();
                    LUCSetType000.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType000);
                    LUCSetType000.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 101:
                    UCSetType101 LUCSetType101 = new UCSetType101();
                    LUCSetType101.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType101);
                    LUCSetType101.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 102:
                    UCSetType0AX LUCSetType0AX = new UCSetType0AX();
                    LUCSetType0AX.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType0AX);
                    LUCSetType0AX.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 103:
                    UCSetType103 LUCSetType103 = new UCSetType103();
                    LUCSetType103.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType103);
                    LUCSetType103.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 104:
                    UCSetType104 LUCSetType104 = new UCSetType104();
                    LUCSetType104.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType104);
                    LUCSetType104.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 105:
                    UCSetType105 LUCSetType105 = new UCSetType105();
                    LUCSetType105.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType105);
                    LUCSetType105.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 106:
                    UCSetType106 LUCSetType106 = new UCSetType106();
                    LUCSetType106.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType106);
                    LUCSetType106.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 107:
                    UCSetType107 LUCSetType107 = new UCSetType107();
                    LUCSetType107.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType107);
                    LUCSetType107.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                default:
                    break;
            }
            
        }

        private void SaveParameterEdited()
        {
            if (IOperationEvent != null)
            {
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrObjectTag = "Save";
                IOperationEvent(this, LEventArgs);
            }
        }

        private string ParameterCanEditCheck(string AStrParameterID)
        {
            string LStrReturn = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrCheckValueInDatabase = string.Empty;
            string LStrCheckValueTarget = string.Empty;

            LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

            if (AStrParameterID == "11030102" || AStrParameterID == "11030103")
            {
                LStrCheckValueTarget = EncryptionAndDecryption.EncryptDecryptString("110301010", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                DataRow[] LDataRowSource = App.GDataTable11001.Select("C003 = 11030101");
                LStrCheckValueInDatabase = LDataRowSource[0]["C006"].ToString();
                if (LStrCheckValueInDatabase == LStrCheckValueTarget) { LStrReturn = "0"; } else { LStrReturn = "1"; }
            }

            if (AStrParameterID == "11010401") { LStrReturn = "0"; }

            if (AStrParameterID == "11020102")
            {
                LStrCheckValueTarget = EncryptionAndDecryption.EncryptDecryptString("110201010", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                DataRow[] LDataRowSource = App.GDataTable11001.Select("C003 = 11020101");
                LStrCheckValueInDatabase = LDataRowSource[0]["C006"].ToString();
                if (LStrCheckValueInDatabase == LStrCheckValueTarget) { LStrReturn = "0"; }
            }

            return LStrReturn;
        }

        #endregion

        private void Page_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }

        private void Page00000A_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.IPageMainOpend = this;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = (int)RequestCode.CSModuleLoading;
                LWebRequestClientLoading.Session = App.GClassSessionInfo;
                LWebRequestClientLoading.Session.SessionID = App.GClassSessionInfo.SessionID;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                if (LWebReturn.Result)
                {
                    App.GClassSessionInfo.AppName = LWebReturn.Session.AppName;
                    App.GClassSessionInfo.AppServerInfo = LWebReturn.Session.AppServerInfo;
                    App.GClassSessionInfo.DatabaseInfo = LWebReturn.Session.DatabaseInfo;
                    App.GClassSessionInfo.DBConnectionString = LWebReturn.Session.DBConnectionString;
                    App.GClassSessionInfo.DBType = LWebReturn.Session.DBType;
                    App.GClassSessionInfo.LangTypeInfo = LWebReturn.Session.LangTypeInfo;
                    App.GClassSessionInfo.LocalMachineInfo = LWebReturn.Session.LocalMachineInfo;
                    App.GClassSessionInfo.RentInfo = LWebReturn.Session.RentInfo;
                    App.GClassSessionInfo.RoleInfo = LWebReturn.Session.RoleInfo;
                    App.GClassSessionInfo.ThemeInfo = LWebReturn.Session.ThemeInfo;
                    App.GClassSessionInfo.UserInfo = LWebReturn.Session.UserInfo;
                    if (!string.IsNullOrEmpty(LWebReturn.Data)) { App.GStrCurrentOperation = LWebReturn.Data; }
                    App.LoadStyleDictionary();
                    App.LoadApplicationLanguages();
                    App.LoadSecurityPlicy();
                }
                DoingMainSendMessage(App.GStrCurrentOperation);

                ShowElementLanguages();

                WebRequest LWebRequestClientLoaded = new WebRequest();
                LWebRequestClientLoaded.Code = (int)RequestCode.CSModuleLoaded;
                LWebRequestClientLoaded.Session = App.GClassSessionInfo;
                LWebRequestClientLoaded.Session.SessionID = App.GClassSessionInfo.SessionID;
                App.SendNetPipeMessage(LWebRequestClientLoaded);

                GridMainPanel.KeyDown += GridMainPanel_KeyDown;
                GridMainPanel.MouseMove += GridMainPanel_MouseMove;
                ITimerSendMessage2Main.Elapsed += ITimerSendMessage2Main_Elapsed;
                ITimerSendMessage2Main.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void DoingMainSendMessage(string AStrData)
        {
            string LStrBegin = string.Empty, LStrEnd = string.Empty;
            string LStrOldGroupID = string.Empty;
            string LStrGetGroupID = string.Empty;

            try
            {
                BorderParameterDetail.Visibility = System.Windows.Visibility.Collapsed;

                StackPanelContainsGroupParameter.Children.Clear();
                LStrBegin = AStrData + "0000";
                LStrEnd = AStrData + "9999";
                DataRow[] LDataRowThisModule = App.GDataTable11001.Select("C003 >= " + LStrBegin + " AND C003 <= " + LStrEnd, "C003 ASC");
                foreach (DataRow LDataRowParameter in LDataRowThisModule)
                {
                    LStrGetGroupID = LDataRowParameter["C004"].ToString();
                    if (LStrGetGroupID == "110201") { continue; }
                    if (LStrGetGroupID != LStrOldGroupID)
                    {
                        UCGroupParameter LUCGroupParameter = new UCGroupParameter();
                        LUCGroupParameter.IPageParent = this;
                        LUCGroupParameter.Margin = new Thickness(0, 2, 2, 2);
                        LUCGroupParameter.ShowGroupParameters(App.GDataTable11001, LStrGetGroupID);
                        LUCGroupParameter.IOperationEvent += LUCGroupParameter_IOperationEvent;
                        StackPanelContainsGroupParameter.Children.Add(LUCGroupParameter);
                        LStrOldGroupID = LStrGetGroupID;
                    }
                }
            }
            catch { }
        }

        public void ShowElementLanguages()
        {
            if (IStrCurrentMethod == "V")
            {
                StrEditApply = App.GetDisplayCharater("Page00000A", "ButtonEdit");
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonEditStyle"];
            }
            else
            {
                StrEditApply = App.GetDisplayCharater("Page00000A", "ButtonApply");
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonApplyStyle"];
            }

            StrCancel = App.GetDisplayCharater("Page00000A", "ButtonCancel");
            StrHeader = App.GetDisplayCharater("Page00000A", "GroupBoxParameterValueHeader");
            StrDescriber = App.GetDisplayCharater("Page00000A", "GroupBoxParameterDescriber");

        }

        public void ApplicationLanguageChanged()
        {
            string LStr11001003 = string.Empty;     //参数编码

            ShowElementLanguages();

            #region 左侧参数列表切换语言
            foreach (object LObjectSingleGroup in StackPanelContainsGroupParameter.Children)
            {
                UCGroupParameter LUCGroupParameter = LObjectSingleGroup as UCGroupParameter;
                LUCGroupParameter.StrGroupName = App.GetDisplayCharater("UCGroupParameter", "G" + LUCGroupParameter.IStrGroupID);
                foreach (object LObjectSingleArg in LUCGroupParameter.StackPanelContainsParameters.Children)
                {
                    UCSingleParameter LUCSingleParameter = LObjectSingleArg as UCSingleParameter;
                    LUCSingleParameter.ShowParameterSettedInfo();
                }
            }
            #endregion

            if (IDataRowCurrentFocused == null) { return; }

            if (IStrCurrentMethod == "V")
            {
                ShowSingleParameterDetailInfo();
            }
            else
            {
                LStr11001003 = IDataRowCurrentFocused["C003"].ToString();
                StrParaName = App.GetDisplayCharater("UCSingleParameter", "P" + LStr11001003);
                StrParameterDesc = App.GetDisplayCharater("Page00000A", "Des" + LStr11001003);

                

                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrObjectTag = App.GClassSessionInfo.LangTypeInfo.LangID.ToString();
                if (IChangeLanguageEvent != null) { IChangeLanguageEvent(this, LEventArgs); }
            }
        }

        #region 点击单个参数，处理
        private void LUCGroupParameter_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (IBoolIsBusy) { return; }
            IOperationEvent = null;
            BorderParameterDetail.Visibility = System.Windows.Visibility.Visible;
            DataRow LDataRowParamInfo = e.ObjectSource0 as DataRow;
            IDataRowCurrentFocused = LDataRowParamInfo;
            ShowSingleParameterDetailInfo();
        }

        private void ShowSingleParameterDetailInfo()
        {
            string LStr11001003 = string.Empty;     //参数编码
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            try
            {
                StrEditApply = App.GetDisplayCharater("Page00000A", "ButtonEdit");
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonEditStyle"];
                ButtonCancelEdit.Visibility = System.Windows.Visibility.Collapsed;
                IStrCurrentMethod = "V";

                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStr11001003 = IDataRowCurrentFocused["C003"].ToString();
                StrParaName = App.GetDisplayCharater("UCSingleParameter", "P" + LStr11001003);
                LStrParameterValueDB = IDataRowCurrentFocused["C006"].ToString();
                UCLabelParametersCurrentValue LParamCurrentValue = new UCLabelParametersCurrentValue();
                GridViewOrEditValue.Children.Clear();
                GridViewOrEditValue.Children.Add(LParamCurrentValue);
                LParamCurrentValue.StrCurrentValue = ActValue2Display(IDataRowCurrentFocused);
                StrParameterDesc = App.GetDisplayCharater("Page00000A", "Des" + LStr11001003);
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

        private string _StrParaName;
        public string StrParaName
        {
            get { return _StrParaName; }
            set
            {
                _StrParaName = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrParaName"); }
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

        private string _StrParameterDesc;
        public string StrParameterDesc
        {
            get { return _StrParameterDesc; }
            set
            {
                _StrParameterDesc = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrParameterDesc"); }
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

        public string ActValue2Display(DataRow ADataRowParamInfo)
        {
            string LStrReturn = string.Empty;
            string LStr11001003 = string.Empty;     //参数编码
            int LInt00003009 = 0;                   //显示格式
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;
            string LStrParameterValueSub = string.Empty;

            try
            {
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStr11001003 = ADataRowParamInfo["C003"].ToString();

                LStrParameterValueDB = ADataRowParamInfo["C006"].ToString();
                LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrParameterValueSub = LStrParameterValueDB.Substring(8);
                LInt00003009 = int.Parse(ADataRowParamInfo["C009"].ToString());
                switch (LInt00003009)
                {
                    case 101:       //开关性质的配置选项
                        LStrReturn = App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrParameterValueSub);
                        break;
                    case 102:       //0:表示关闭、禁用等信息，大于 0 表示实际配置值
                        if (LStrParameterValueSub == "0")
                        {
                            LStrReturn = App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrParameterValueSub);
                        }
                        else if (LStrParameterValueSub == "-99")
                        {
                            LStrReturn = App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrParameterValueSub);
                        }
                        else
                        {
                            LStrReturn = string.Format(App.GetDisplayCharater("Page00000A", "Act" + LStr11001003), LStrParameterValueSub);
                        }
                        break;
                    case 103:       //数字对应礼拜几
                        LStrReturn = App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrParameterValueSub);
                        break;
                    case 104:       //1:表示自然月； 2 ~ 28 表示开始日期
                        if (LStrParameterValueSub == "1")
                        {
                            LStrReturn = App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrParameterValueSub);
                        }
                        else
                        {
                            LStrReturn = string.Format(App.GetDisplayCharater("Page00000A", "Act" + LStr11001003), LStrParameterValueSub);
                        }
                        break;
                    case 105:       //字符对应分组方式
                        string[] LStrGroupType = LStrParameterValueSub.Split(IStrSpliterChar.ToCharArray());
                        foreach (string LStrType in LStrGroupType)
                        {
                            LStrReturn += App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrType) + " / ";
                        }
                        LStrReturn = LStrReturn.Substring(0, LStrReturn.Length - 3);
                        break;
                    default:
                        if (LStrParameterValueSub == "-99")
                        {
                            LStrReturn = App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V-99");
                        }
                        else
                        {
                            LStrReturn = string.Format(App.GetDisplayCharater("Page00000A", "Act" + LStr11001003), LStrParameterValueSub);
                        }
                        break;
                }
                
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        #region 将设置的参数保存到数据库
        private BackgroundWorker IBWSaveData = null;
        private bool IBoolCallReturn = true;
        private string IStrCallReturn = string.Empty;
        public void SaveEditedParameterValue(string AStrNewValue)
        {
            if (!VarifuValueIsRight(AStrNewValue))
            {
                IOperationEvent = null;
                ShowSingleParameterDetailInfo();
                return;
            }
            SaveData2DatabaseBegin(AStrNewValue);
        }

        /// <summary>
        /// 验证设置的参数有效性
        /// </summary>
        /// <param name="AStrNewValue"></param>
        /// <returns></returns>
        private bool VarifuValueIsRight(string AStrNewValue)
        {
            bool LBoolReturn = true;
            string LStr11001003 = string.Empty;
            string LStr11001006 = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;

            string LStrCallReturn = string.Empty;

            try
            {
                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStr11001003 = IDataRowCurrentFocused["C003"].ToString();
                LStr11001006 = IDataRowCurrentFocused["C006"].ToString();

                #region 判断新值与旧值是否相同
                if (LStr11001006 == EncryptionAndDecryption.EncryptDecryptString(LStr11001003 + AStrNewValue, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004)) { return false; }
                #endregion

                #region 判断密码复杂度
                if (LStr11001003 == "11010501")
                {
                    int LIntMinLength = 0;
                    LStr11001006 = (App.GDataTable11001.Select("C003 = 11010102"))[0]["C006"].ToString();
                    LStr11001006 = EncryptionAndDecryption.EncryptDecryptString(LStr11001006, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStr11001006 = LStr11001006.Substring(8);
                    LIntMinLength = int.Parse(LStr11001006);

                    LStr11001006 = (App.GDataTable11001.Select("C003 = 11010101"))[0]["C006"].ToString();
                    LStr11001006 = EncryptionAndDecryption.EncryptDecryptString(LStr11001006, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStr11001006 = LStr11001006.Substring(8);
                    if (LStr11001006 == "0")
                    {
                        if (AStrNewValue.Length < LIntMinLength)
                        {
                            MessageBox.Show(string.Format(App.GetDisplayCharater("S1106134"), LIntMinLength.ToString()), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                        if (AStrNewValue.Length > 64)
                        {
                            MessageBox.Show(App.GetDisplayCharater("S1106135"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }
                    else
                    {
                        LBoolReturn = PasswordVerifyOptions.MeetComplexityRequirements(AStrNewValue, LIntMinLength, 64, "", ref LStrCallReturn);
                        if (!LBoolReturn)
                        {
                            MessageBox.Show(App.GetDisplayCharater("Page00000A", "CPWDR" + LStrCallReturn), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }
                }
                #endregion

                #region 判断超级管理员帐户的长度
                if (LStr11001003 == "11020401")
                {
                    string LStrNewAccount = AStrNewValue.Trim();
                    if (LStrNewAccount.Length <= 0 )
                    {
                        MessageBox.Show(App.GetDisplayCharater("S1106145"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (LStrNewAccount.Length > 128)
                    {
                        MessageBox.Show(App.GetDisplayCharater("S1106146"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                }
                #endregion
            }
            catch { LBoolReturn = false; }

            return LBoolReturn;
        }

        /// <summary>
        /// 将设置的数据保存到数据库实际动作
        /// </summary>
        /// <param name="AStrNewValue"></param>
        /// <returns></returns>
        private void SaveData2DatabaseBegin(string AStrNewValue)
        {
            try
            {
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12111;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                IBoolIsBusy = true;
                IBWSaveData = new BackgroundWorker();
                IBWSaveData.RunWorkerCompleted += IBWSaveData_RunWorkerCompleted;
                IBWSaveData.DoWork += IBWSaveData_DoWork;
                IBWSaveData.RunWorkerAsync(AStrNewValue);
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
            string LStrVerificationCode004 = string.Empty;
            string LStrSettedValue = string.Empty;
            string LStrCurrent003 = string.Empty;

            try
            {
                string LStrNewValue = e.Argument as string;

                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSettedValue = EncryptionAndDecryption.EncryptDecryptString(LStrNewValue, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);
                LStrCurrent003 = IDataRowCurrentFocused["C003"].ToString();
                LListStrWcfArgs.Add(LStrCurrent003);
                LListStrWcfArgs.Add(LStrSettedValue);
                LListStrWcfArgs.Add(App.GClassSessionInfo.UserInfo.UserID.ToString());

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(21, LListStrWcfArgs);
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                if (IBoolCallReturn)
                {
                    IStrCallReturn = EncryptionAndDecryption.EncryptDecryptString(IDataRowCurrentFocused["C003"].ToString() + LStrNewValue, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    ChangeOtherParameters(LStrCurrent003, LStrNewValue);
                }
                else
                {
                    if (LWCFOperationReturn.ListStringReturn.Count > 0)
                    {
                        IStrCallReturn = LWCFOperationReturn.ListStringReturn[0];
                    }
                }
            }
            catch (Exception ex)
            {
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
            }
            finally
            {
                if (LService00000Client != null)
                {
                    LService00000Client.Close(); LService00000Client = null;
                }
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
                    MessageBox.Show(App.GetDisplayCharater("S1106095") + "\n" + IStrCallReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    IDataRowCurrentFocused["C006"] = IStrCallReturn;
                    foreach (DataRow LDataRowSingle in App.GDataTable11001.Rows)
                    {
                        if (LDataRowSingle["C003"].ToString() == IDataRowCurrentFocused["C003"].ToString()) { LDataRowSingle["C006"] = IStrCallReturn; break; }
                    }
                    ReShowLeftStackPanel();
                    IOperationEvent = null;
                    ShowSingleParameterDetailInfo();
                    ReShowEffectParameters(IDataRowCurrentFocused["C003"].ToString());
                }
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

        private void ChangeOtherParameters(string AStrCurrent003, string AStrNewValue)
        {
            if (AStrCurrent003 == "11010101") { Data11010101Changed(AStrNewValue); return; }

            if (AStrCurrent003 == "11010202") { Data11010202Changed(AStrNewValue); return; }

            if (AStrCurrent003 == "11020101") { Data11020101Changed(AStrNewValue); return; }

            if (AStrCurrent003 == "11030101") { Data11030101Changed(AStrNewValue); return; }
           
        }

        //密码必须符合复杂性要求设置为开启后，修改最短密码长度
        private void Data11010101Changed(string AStrNewValue)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrSettedValue = string.Empty;
            string LStrSettedValueInDB = string.Empty;

            try
            {
                if (AStrNewValue != "1") { return; }

                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);

                foreach (DataRow LDataRowSingle in App.GDataTable11001.Rows)
                {
                    if (LDataRowSingle["C003"].ToString() != "11010102") { continue; }

                    LStrSettedValueInDB = LDataRowSingle["C006"].ToString();
                    LStrSettedValueInDB = EncryptionAndDecryption.EncryptDecryptString(LStrSettedValueInDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrSettedValueInDB = LStrSettedValueInDB.Substring(8);

                    if (int.Parse(LStrSettedValueInDB) >= 6) { break; }

                    LDataRowSingle["C006"] = EncryptionAndDecryption.EncryptDecryptString("110101026", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    LStrSettedValue = EncryptionAndDecryption.EncryptDecryptString("6", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LListStrWcfArgs.Clear();
                    LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                    LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                    LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);
                    LListStrWcfArgs.Add("11010102");
                    LListStrWcfArgs.Add(LStrSettedValue);
                    LListStrWcfArgs.Add(App.GClassSessionInfo.UserInfo.UserID.ToString());
                    LWCFOperationReturn = LService00000Client.OperationMethodA(21, LListStrWcfArgs);

                    break;
                }
            }
            catch { }
            finally
            {
                if (LService00000Client != null)
                {
                    LService00000Client.Close(); LService00000Client = null;
                }
            }
        }

        //密码最长使用期限改变后，检查 “提示用户在密码过期之前进行更改”
        private void Data11010202Changed(string AStrNewValue)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrSettedValue = string.Empty;
            string LStrSettedValueInDB = string.Empty;

            try
            {
                if (AStrNewValue == "0") { return; }

                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);

                foreach (DataRow LDataRowSingle in App.GDataTable11001.Rows)
                {
                    if (LDataRowSingle["C003"].ToString() != "11010203") { continue; }

                    LStrSettedValueInDB = LDataRowSingle["C006"].ToString();
                    LStrSettedValueInDB = EncryptionAndDecryption.EncryptDecryptString(LStrSettedValueInDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrSettedValueInDB = LStrSettedValueInDB.Substring(8);
                    if (int.Parse(LStrSettedValueInDB) <= int.Parse(AStrNewValue)) { return; }

                    LDataRowSingle["C006"] = EncryptionAndDecryption.EncryptDecryptString("11010203" + AStrNewValue, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    LStrSettedValue = EncryptionAndDecryption.EncryptDecryptString(AStrNewValue, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LListStrWcfArgs.Clear();
                    LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                    LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                    LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);
                    LListStrWcfArgs.Add("11010203");
                    LListStrWcfArgs.Add(LStrSettedValue);
                    LListStrWcfArgs.Add(App.GClassSessionInfo.UserInfo.UserID.ToString());
                    LWCFOperationReturn = LService00000Client.OperationMethodA(21, LListStrWcfArgs);

                    break;
                }
            }
            catch { }
            finally
            {
                if (LService00000Client != null)
                {
                    LService00000Client.Close(); LService00000Client = null;
                }
            }
        }

        //显示最后登录用户 修改后，修改 “允许记住用户帐号和密码”选项
        private void Data11020101Changed(string AStrNewValue)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrSettedValue = string.Empty;
            string LStrSettedValueInDB = string.Empty;

            try
            {
                if (AStrNewValue == "1") { return; }

                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);

                foreach (DataRow LDataRowSingle in App.GDataTable11001.Rows)
                {
                    if (LDataRowSingle["C003"].ToString() != "11020102") { continue; }

                    LStrSettedValueInDB = LDataRowSingle["C006"].ToString();
                    LStrSettedValueInDB = EncryptionAndDecryption.EncryptDecryptString(LStrSettedValueInDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrSettedValueInDB = LStrSettedValueInDB.Substring(8);

                    if (LStrSettedValueInDB == "0") { break; }

                    LDataRowSingle["C006"] = EncryptionAndDecryption.EncryptDecryptString("110201020", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    LStrSettedValue = EncryptionAndDecryption.EncryptDecryptString("0", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LListStrWcfArgs.Clear();
                    LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                    LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                    LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);
                    LListStrWcfArgs.Add("11020102");
                    LListStrWcfArgs.Add(LStrSettedValue);
                    LListStrWcfArgs.Add(App.GClassSessionInfo.UserInfo.UserID.ToString());
                    LWCFOperationReturn = LService00000Client.OperationMethodA(21, LListStrWcfArgs);

                    break;
                }
            }
            catch { }
            finally
            {
                if (LService00000Client != null)
                {
                    LService00000Client.Close(); LService00000Client = null;
                }
            }
        }

        //账户锁定阈值改变时需要修改 账户锁定时间 和 复位账户锁定计数器
        private void Data11030101Changed(string AStrNewValue)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();
            string LStrVerificationCode004 = string.Empty;
            string LStrSettedValue = string.Empty;

            string LStrSettedValueInDB = string.Empty;

            List<string> LListStrChangeTarget = new List<string>();

            try
            {
                LStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);

                LListStrChangeTarget.Add("11030102"); LListStrChangeTarget.Add("11030103");

                if (AStrNewValue == "0")
                {
                    #region 设置为永不锁定账户
                    LStrSettedValue = EncryptionAndDecryption.EncryptDecryptString("-99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    foreach (string LStrSingleTarget in LListStrChangeTarget)
                    {
                        LListStrWcfArgs.Clear();
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                        LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                        LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);
                        LListStrWcfArgs.Add(LStrSingleTarget);
                        LListStrWcfArgs.Add(LStrSettedValue);
                        LListStrWcfArgs.Add(App.GClassSessionInfo.UserInfo.UserID.ToString());
                        LWCFOperationReturn = LService00000Client.OperationMethodA(21, LListStrWcfArgs);
                        foreach (DataRow LDataRowSingle in App.GDataTable11001.Rows)
                        {
                            if (LDataRowSingle["C003"].ToString() == LStrSingleTarget)
                            {
                                LDataRowSingle["C006"] = EncryptionAndDecryption.EncryptDecryptString(LStrSingleTarget + "-99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                break;
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    foreach (string LStrSingleTarget in LListStrChangeTarget)
                    {
                        LListStrWcfArgs.Clear();
                        foreach (DataRow LDataRowSingle in App.GDataTable11001.Rows)
                        {
                            if (LDataRowSingle["C003"].ToString() == LStrSingleTarget)
                            {
                                LStrSettedValueInDB = LDataRowSingle["C006"].ToString();
                                if (LStrSettedValueInDB != EncryptionAndDecryption.EncryptDecryptString(LStrSingleTarget + "-99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004)) { continue; }

                                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());
                                LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);
                                LListStrWcfArgs.Add(LStrSingleTarget);
                                if (LStrSingleTarget == "11030102")
                                {
                                    LStrSettedValue = EncryptionAndDecryption.EncryptDecryptString("30", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                    LDataRowSingle["C006"] = EncryptionAndDecryption.EncryptDecryptString(LStrSingleTarget + "30", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                }
                                else
                                {
                                    LStrSettedValue = EncryptionAndDecryption.EncryptDecryptString("25", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                    LDataRowSingle["C006"] = EncryptionAndDecryption.EncryptDecryptString(LStrSingleTarget + "25", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                                }
                                LListStrWcfArgs.Add(LStrSettedValue);
                                LListStrWcfArgs.Add(App.GClassSessionInfo.UserInfo.UserID.ToString());
                                LWCFOperationReturn = LService00000Client.OperationMethodA(21, LListStrWcfArgs);
                            }
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (LService00000Client != null)
                {
                    LService00000Client.Close(); LService00000Client = null;
                }
            }
        }

        private void ReShowLeftStackPanel()
        {
            foreach (object LObjectSingleGroup in StackPanelContainsGroupParameter.Children)
            {
                UCGroupParameter LUCGroupParameter = LObjectSingleGroup as UCGroupParameter;
                foreach (object LObjectSingleArg in LUCGroupParameter.StackPanelContainsParameters.Children)
                {
                    UCSingleParameter LUCSingleParameter = LObjectSingleArg as UCSingleParameter;
                    if (LUCSingleParameter.IDataRow11001["C003"].ToString() == IDataRowCurrentFocused["C003"].ToString())
                    {
                        LUCSingleParameter.IDataRow11001["C006"] = IStrCallReturn;
                        LUCSingleParameter.ShowParameterSettedInfo();
                        return;
                    }
                }
            }
        }

        private void ReShowEffectParameters(string AStrCurrent003)
        {
            List<string> LListStrChangeTarget = new List<string>();

            if (AStrCurrent003 == "11010101")
            {
                LListStrChangeTarget.Add("11010102");
            }

            if (AStrCurrent003 == "11010202")
            {
                LListStrChangeTarget.Add("11010203");
            }

            if (AStrCurrent003 == "11020101")
            {
                LListStrChangeTarget.Add("11020102");
            }

            if (AStrCurrent003 == "11030101")
            {
                LListStrChangeTarget.Add("11030102"); LListStrChangeTarget.Add("11030103");
            }


            foreach (string LStrSingleTarget in LListStrChangeTarget)
            {
                foreach (object LObjectSingleGroup in StackPanelContainsGroupParameter.Children)
                {
                    UCGroupParameter LUCGroupParameter = LObjectSingleGroup as UCGroupParameter;
                    foreach (object LObjectSingleArg in LUCGroupParameter.StackPanelContainsParameters.Children)
                    {
                        UCSingleParameter LUCSingleParameter = LObjectSingleArg as UCSingleParameter;
                        if (LUCSingleParameter.IDataRow11001["C003"].ToString() == LStrSingleTarget)
                        {
                            LUCSingleParameter.ShowParameterSettedInfo();
                            break;
                        }
                    }
                }
            }
        }
        #endregion

        #region 判断是否处于"忙"状态
        int IIntIdleCount = 0;
        private bool IBoolTimerBusy = false;
        private Timer ITimerSendMessage2Main = new Timer(1000);
        private delegate void IDelegateSendMessage2Main();

        private void GridMainPanel_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                IIntIdleCount = 0;
            }
            catch { }
        }

        private void GridMainPanel_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                IIntIdleCount = 0;
            }
            catch { }
        }

        private void ITimerSendMessage2Main_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Normal, new IDelegateSendMessage2Main(SendMessage2MainFram));
        }

        private void SendMessage2MainFram()
        {
            try
            {
                IIntIdleCount += 1;
                if (IBoolTimerBusy) { return; }
                IBoolTimerBusy = true;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 91001;
                LWebRequestClientLoading.Data = IIntIdleCount.ToString();
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
            }
            catch { }
            finally
            {
                IBoolTimerBusy = false;
            }
        }

        public void StartStopTimer(bool ABoolStart)
        {
            if (ABoolStart) { IIntIdleCount = 0; ITimerSendMessage2Main.Start(); } else { ITimerSendMessage2Main.Stop(); IIntIdleCount = 0; }
        }

        #endregion
    }
}
