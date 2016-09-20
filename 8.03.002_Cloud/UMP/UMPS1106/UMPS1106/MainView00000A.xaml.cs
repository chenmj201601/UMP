using System;
using System.Collections.Generic;
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
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Common;
using System.ComponentModel;
using System.Data;
using System.Timers;
using PFShareClassesC;
using VoiceCyber.UMP.Communications;
using UMPS1106.WCFService00000;
using System.ServiceModel;
using UMPS1106.MainUserControl;
using System.Windows.Threading;

namespace UMPS1106
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class MainView00000A : INotifyPropertyChanged, S1106Interface, S1106ChangeLanguageInterface
    {

        public event EventHandler<OperationEventArgs> IOperationEvent;
        public event EventHandler<OperationEventArgs> IChangeLanguageEvent;

        private bool IBoolIsBusy = false;

        public string IStrSpliterChar = string.Empty;
        private string CurrentLoadingModule = string.Empty;

        private string IStrCurrentMethod = string.Empty;
        DataRow IDataRowCurrentFocused = null;//当前选中的参数以及参数里面的内容

        #region 变量定义
        private BackgroundWorker mWorker;
        #endregion

        public MainView00000A()
        {
            InitializeComponent();

            ButtonEditApply.Click += ButtonEditApplyCancelClick;
            ButtonCancelEdit.Click += ButtonEditApplyCancelClick;
           
        }

        protected override void Init()
        {
            PageName = "Page00000A";
            StylePath = "UMPS1106/StyleDictionary1106.xaml";
            base.Init();

            IStrSpliterChar = S1106App.AscCodeToChr(27);
            Page00000A_Loaded();
            IStrCurrentMethod = "V";
          

            CurrentApp.WriteLog("Init UMPS1106 , CurrentLoadingModule = " + S1106App.CurrentLoadingModule);
            SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
            mWorker = new BackgroundWorker();
            mWorker.DoWork += (s, de) =>
            {
                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }
            };
            mWorker.RunWorkerCompleted += (s, re) =>
            {

                mWorker.Dispose();
                SetBusy(false, string.Empty);
                //ChangeLanguage();
            };
            mWorker.RunWorkerAsync();

           // App.IPageMainOpend = this;

            //ChangeTheme();
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
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
                MessageBox.Show(S1106App.GetDisplayCharater("Page00000A", "CRV0" + LStr11001003), CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            IStrCurrentMethod = "E";

            ButtonEditApply.Style = (Style)App.Current.Resources["ButtonApplyStyle"];
            StrEditApply = S1106App.GetDisplayCharater("Page00000A", "ButtonApply");
            ButtonEditApply.Content = StrEditApply;
            ButtonCancelEdit.Visibility = System.Windows.Visibility.Visible;
            GridViewOrEditValue.Children.Clear();

            LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);


            LStrParameterValueDB = IDataRowCurrentFocused["C006"].ToString();
            LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrParameterValueSub = LStrParameterValueDB.Substring(8);
            LInt00003009 = int.Parse(IDataRowCurrentFocused["C009"].ToString());
            switch (LInt00003009)
            {
                case 0:
                    UCSetType000 LUCSetType000 = new UCSetType000();
                    LUCSetType000.CurrentApp = CurrentApp;
                    LUCSetType000.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType000);
                    LUCSetType000.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 101:
                    UCSetType101 LUCSetType101 = new UCSetType101();
                    LUCSetType101.CurrentApp = CurrentApp;
                    LUCSetType101.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType101);
                    LUCSetType101.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 102:
                    UCSetType0AX LUCSetType0AX = new UCSetType0AX();
                    LUCSetType0AX.CurrentApp = CurrentApp;
                    LUCSetType0AX.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType0AX);
                    LUCSetType0AX.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 103:
                    UCSetType103 LUCSetType103 = new UCSetType103();
                    LUCSetType103.CurrentApp = CurrentApp;
                    LUCSetType103.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType103);
                    LUCSetType103.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 104:
                    UCSetType104 LUCSetType104 = new UCSetType104();
                    LUCSetType104.CurrentApp = CurrentApp;
                    LUCSetType104.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType104);
                    LUCSetType104.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 105:
                    UCSetType105 LUCSetType105 = new UCSetType105();
                    LUCSetType105.CurrentApp = CurrentApp;
                    LUCSetType105.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType105);
                    LUCSetType105.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 106:
                    UCSetType106 LUCSetType106 = new UCSetType106();
                    LUCSetType106.CurrentApp = CurrentApp;
                    LUCSetType106.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType106);
                    LUCSetType106.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                case 107:
                    UCSetType107 LUCSetType107 = new UCSetType107();
                    LUCSetType107.CurrentApp = CurrentApp;
                    LUCSetType107.IPageParent = this;
                    GridViewOrEditValue.Children.Add(LUCSetType107);
                    LUCSetType107.ShowParameterEditStyle(IDataRowCurrentFocused);
                    break;
                default:
                    break;
            }
            ApplicationLanguageChanged();
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

            LStrVerificationCode004 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

            if (AStrParameterID == "11030102" || AStrParameterID == "11030103")
            {
                LStrCheckValueTarget = EncryptionAndDecryption.EncryptDecryptString("110301010", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                DataRow[] LDataRowSource = S1106App.GDataTable11001.Select("C003 = 11030101");
                LStrCheckValueInDatabase = LDataRowSource[0]["C006"].ToString();
                if (LStrCheckValueInDatabase == LStrCheckValueTarget) { LStrReturn = "0"; } else { LStrReturn = "1"; }
            }

            if (AStrParameterID == "11010401") { LStrReturn = "0"; }

            if (AStrParameterID == "11020102")
            {
                LStrCheckValueTarget = EncryptionAndDecryption.EncryptDecryptString("110201010", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                DataRow[] LDataRowSource = S1106App.GDataTable11001.Select("C003 = 11020101");
                LStrCheckValueInDatabase = LDataRowSource[0]["C006"].ToString();
                if (LStrCheckValueInDatabase == LStrCheckValueTarget) { LStrReturn = "0"; }
            }

            return LStrReturn;
        }

        #endregion

        private void Page00000A_Loaded()
        {
            try
            {
                S1106App.IPageMainOpend = this;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = (int)RequestCode.CSModuleLoading;
                LWebRequestClientLoading.Session = CurrentApp.Session;
                LWebRequestClientLoading.Session.SessionID = CurrentApp.Session.SessionID;
                WebReturn LWebReturn = new WebReturn();
                //LWebReturn = App.LoadingMessageReturn;
                if (CurrentApp.StartArgs == null)
                {
                    CurrentApp.StartArgs = "1109";
                }
                if (!string.IsNullOrEmpty(CurrentApp.StartArgs))
                {
                    LWebReturn.Data = CurrentApp.StartArgs;
                }
                if (LWebReturn != null)
                {
                    //LWebReturn.Data = "1201";
                    LWebReturn.Session = CurrentApp.Session;
                    LWebReturn.Result = true;
                }
                switch (LWebReturn.Data)
                {
                    case "1106":
                        LWebReturn.Data = "1101";
                        break;
                    case "1107":
                        LWebReturn.Data = "1102";
                        break;
                    case "1108":
                        LWebReturn.Data = "1103";
                        break;
                    case "1109":
                        LWebReturn.Data = "1201";
                        break;
                }

                if (LWebReturn.Result)
                {
                    CurrentApp.Session.AppName = LWebReturn.Session.AppName;
                    CurrentApp.Session.AppServerInfo = LWebReturn.Session.AppServerInfo;
                    CurrentApp.Session.DatabaseInfo = LWebReturn.Session.DatabaseInfo;
                    CurrentApp.Session.DBConnectionString = LWebReturn.Session.DBConnectionString;
                    CurrentApp.Session.DBType = LWebReturn.Session.DBType;
                    CurrentApp.Session.LangTypeInfo = LWebReturn.Session.LangTypeInfo;
                    CurrentApp.Session.LocalMachineInfo = LWebReturn.Session.LocalMachineInfo;
                    CurrentApp.Session.RentInfo = LWebReturn.Session.RentInfo;
                    CurrentApp.Session.RoleInfo = LWebReturn.Session.RoleInfo;
                    CurrentApp.Session.ThemeInfo = LWebReturn.Session.ThemeInfo;
                    CurrentApp.Session.UserInfo = LWebReturn.Session.UserInfo;
                    if (!string.IsNullOrEmpty(LWebReturn.Data)) { S1106App.GStrCurrentOperation = LWebReturn.Data; }
                    ChangeTheme();
                    //App.LoadStyleDictionary();
                    //S1106App.LoadApplicationLanguages();
                    ((S1106App)CurrentApp).LoadSecurityPlicy();
                    //ChangeLanguage();
                }
                DoingMainSendMessage(LWebReturn.Data);

                ShowElementLanguages();
                ApplicationLanguageChanged();
                WebRequest LWebRequestClientLoaded = new WebRequest();
                LWebRequestClientLoaded.Code = (int)RequestCode.CSModuleLoaded;
                LWebRequestClientLoaded.Session = CurrentApp.Session;
                LWebRequestClientLoaded.Session.SessionID = CurrentApp.Session.SessionID;
                CurrentApp.SendNetPipeMessage(LWebRequestClientLoaded);

                GridMainPanel.KeyDown += GridMainPanel_KeyDown;
                GridMainPanel.MouseMove += GridMainPanel_MouseMove;
                //ITimerSendMessage2Main.Elapsed += ITimerSendMessage2Main_Elapsed;
                //ITimerSendMessage2Main.Start();
            }
            catch (Exception ex)
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
                DataRow[] LDataRowThisModule = S1106App.GDataTable11001.Select("C003 >= " + LStrBegin + " AND C003 <= " + LStrEnd, "C003 ASC");
                foreach (DataRow LDataRowParameter in LDataRowThisModule)
                {
                    LStrGetGroupID = LDataRowParameter["C004"].ToString();
                    if (LStrGetGroupID == "110201") { continue; }
                    if (LStrGetGroupID != LStrOldGroupID)
                    {
                        UCGroupParameter LUCGroupParameter = new UCGroupParameter();
                        LUCGroupParameter.CurrentApp = CurrentApp;
                        LUCGroupParameter.IPageParent = this;
                        LUCGroupParameter.Margin = new Thickness(0, 2, 2, 2);
                        LUCGroupParameter.ShowGroupParameters(S1106App.GDataTable11001, LStrGetGroupID);
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
                StrEditApply = S1106App.GetDisplayCharater("Page00000A", "ButtonEdit");
                ButtonEditApply.Content = StrEditApply;
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonEditStyle"];
            }
            else
            {
                StrEditApply = S1106App.GetDisplayCharater("Page00000A", "ButtonApply");
                ButtonEditApply.Content = StrEditApply;
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonApplyStyle"];
            }

            StrCancel = S1106App.GetDisplayCharater("Page00000A", "ButtonCancel");
            ButtonCancelEdit.Content = StrCancel;
            StrHeader = S1106App.GetDisplayCharater("Page00000A", "GroupBoxParameterValueHeader");
            StrDescriber = S1106App.GetDisplayCharater("Page00000A", "GroupBoxParameterDescriber");

        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            Init();
            //if (PageHead != null)
            //{
            //    PageHead.AppName = App.GetDisplayCharater("S1106149");
            //}


            //if (PageHead == null)
            //{
            //    PageHead = new UMPPageHead();
            //}

            //if (App.LoadingMessageReturn == null)
            //{
            //    PageHead.AppName = "UMPS1106";
            //    PageHead.AppName = App.GetDisplayCharater("S1106149");
            //    //string a = PageHead.AppName;
            //    //PageHead.AppName =  App.GetDisplayCharater("S1106149");
            //    //string b = PageHead.AppName;
            //}
            //else
            //{
                switch (CurrentApp.StartArgs)
                {
                    //case "1101":
                    //    PageHead.AppName = App.GetDisplayCharater("S1106152");
                    //    break;
                    //case "1102":
                    //    PageHead.AppName = App.GetDisplayCharater("S1106151");
                    //    break;
                    //case "1103":
                    //    PageHead.AppName = App.GetDisplayCharater("S1106150");
                    //    break;
                    //case "1201":
                    //    PageHead.AppName = App.GetDisplayCharater("S1106149");
                    //    break;
                }
            //}

        }


        public void ApplicationLanguageChanged()
        {
            string LStr11001003 = string.Empty;     //参数编码

            ShowElementLanguages();

            #region 左侧参数列表切换语言
            foreach (object LObjectSingleGroup in StackPanelContainsGroupParameter.Children)
            {
                UCGroupParameter LUCGroupParameter = LObjectSingleGroup as UCGroupParameter;
                LUCGroupParameter.CurrentApp = CurrentApp;
                LUCGroupParameter.StrGroupName = S1106App.GetDisplayCharater("UCGroupParameter", "G" + LUCGroupParameter.IStrGroupID);
                foreach (object LObjectSingleArg in LUCGroupParameter.StackPanelContainsParameters.Children)
                {
                    UCSingleParameter LUCSingleParameter = LObjectSingleArg as UCSingleParameter;
                    LUCSingleParameter.CurrentApp = CurrentApp;
                    LUCSingleParameter.ShowParameterSettedInfo();
                }
            }
            #endregion

            if (IDataRowCurrentFocused == null) { return; }

            if (IStrCurrentMethod == "V")
            {
                ShowSingleParameterDetailInfo();
                LStr11001003 = IDataRowCurrentFocused["C003"].ToString();
                StrParaName = S1106App.GetDisplayCharater("UCSingleParameter", "P" + LStr11001003);
                StrParameterDesc = S1106App.GetDisplayCharater("Page00000A", "Des" + LStr11001003, StrParaName);
                TextBoxParameterDescriber.Text = StrParameterDesc;
            }
            else
            {
                LStr11001003 = IDataRowCurrentFocused["C003"].ToString();
                StrParaName = S1106App.GetDisplayCharater("UCSingleParameter", "P" + LStr11001003);
                StrParameterDesc = S1106App.GetDisplayCharater("Page00000A", "Des" + LStr11001003, StrParaName);
                TextBoxParameterDescriber.Text = StrParameterDesc;


                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrObjectTag = CurrentApp.Session.LangTypeInfo.LangID.ToString();
                if (IChangeLanguageEvent != null) { IChangeLanguageEvent(this, LEventArgs); }
            }
        }

        #region 点击单个参数，处理
        private void LUCGroupParameter_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (IBoolIsBusy) { return; }
            IStrCurrentMethod = "V";
            //TextBoxParameterDescriber.Text = "";
            IOperationEvent = null;
            BorderParameterDetail.Visibility = System.Windows.Visibility.Visible;
            DataRow LDataRowParamInfo = e.ObjectSource0 as DataRow;
            IDataRowCurrentFocused = LDataRowParamInfo;
            ApplicationLanguageChanged();
            ShowSingleParameterDetailInfo();
        }

        private void ShowSingleParameterDetailInfo()
        {
            string LStr11001003 = string.Empty;     //参数编码
            string LStrVerificationCode104 = string.Empty;
            string LStrParameterValueDB = string.Empty;

            try
            {
                StrEditApply = S1106App.GetDisplayCharater("Page00000A", "ButtonEdit");
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonEditStyle"];
                ButtonEditApply.Content = StrEditApply;
                ButtonCancelEdit.Visibility = System.Windows.Visibility.Collapsed;
                IStrCurrentMethod = "V";

                LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStr11001003 = IDataRowCurrentFocused["C003"].ToString();
                StrParaName = S1106App.GetDisplayCharater("UCSingleParameter", "P" + LStr11001003);
                LStrParameterValueDB = IDataRowCurrentFocused["C006"].ToString();
                UCLabelParametersCurrentValue LParamCurrentValue = new UCLabelParametersCurrentValue();
                LParamCurrentValue.CurrentApp = CurrentApp;
                GridViewOrEditValue.Children.Clear();
                GridViewOrEditValue.Children.Add(LParamCurrentValue);
                LParamCurrentValue.StrCurrentValue = ActValue2Display(IDataRowCurrentFocused);
                StrParameterDesc = S1106App.GetDisplayCharater("Page00000A", "Des" + LStr11001003, StrParaName);
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
                LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStr11001003 = ADataRowParamInfo["C003"].ToString();

                LStrParameterValueDB = ADataRowParamInfo["C006"].ToString();
                LStrParameterValueDB = EncryptionAndDecryption.EncryptDecryptString(LStrParameterValueDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrParameterValueSub = LStrParameterValueDB.Substring(8);
                LInt00003009 = int.Parse(ADataRowParamInfo["C009"].ToString());
                switch (LInt00003009)
                {
                    case 101:       //开关性质的配置选项
                        LStrReturn = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrParameterValueSub);
                        break;
                    case 102:       //0:表示关闭、禁用等信息，大于 0 表示实际配置值
                        if (LStrParameterValueSub == "0")
                        {
                            LStrReturn = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrParameterValueSub);
                        }
                        else if (LStrParameterValueSub == "-99")
                        {
                            LStrReturn = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrParameterValueSub);
                        }
                        else
                        {
                            LStrReturn = string.Format(S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003), LStrParameterValueSub);
                        }
                        break;
                    case 103:       //数字对应礼拜几
                        LStrReturn = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrParameterValueSub);
                        break;
                    case 104:       //1:表示自然月； 2 ~ 28 表示开始日期
                        if (LStrParameterValueSub == "1")
                        {
                            LStrReturn = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrParameterValueSub);
                        }
                        else
                        {
                            LStrReturn = string.Format(S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003), LStrParameterValueSub);
                        }
                        break;
                    case 105:       //字符对应分组方式
                        string[] LStrGroupType = LStrParameterValueSub.Split(IStrSpliterChar.ToCharArray());
                        foreach (string LStrType in LStrGroupType)
                        {
                            LStrReturn += S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V" + LStrType) + " / ";
                        }
                        LStrReturn = LStrReturn.Substring(0, LStrReturn.Length - 3);
                        break;
                    default:
                        if (LStrParameterValueSub == "-99")
                        {
                            LStrReturn = S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003 + "V-99");
                        }
                        else
                        {
                            LStrReturn = string.Format(S1106App.GetDisplayCharater("Page00000A", "Act" + LStr11001003), LStrParameterValueSub);
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
                LStrVerificationCode004 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStr11001003 = IDataRowCurrentFocused["C003"].ToString();
                LStr11001006 = IDataRowCurrentFocused["C006"].ToString();

                #region 判断新值与旧值是否相同
                if (LStr11001006 == EncryptionAndDecryption.EncryptDecryptString(LStr11001003 + AStrNewValue, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004)) { return false; }
                #endregion

                #region 判断密码复杂度
                if (LStr11001003 == "11010501")
                {
                    int LIntMinLength = 0;
                    LStr11001006 = (S1106App.GDataTable11001.Select("C003 = 11010102"))[0]["C006"].ToString();
                    LStr11001006 = EncryptionAndDecryption.EncryptDecryptString(LStr11001006, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStr11001006 = LStr11001006.Substring(8);
                    LIntMinLength = int.Parse(LStr11001006);

                    LStr11001006 = (S1106App.GDataTable11001.Select("C003 = 11010101"))[0]["C006"].ToString();
                    LStr11001006 = EncryptionAndDecryption.EncryptDecryptString(LStr11001006, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStr11001006 = LStr11001006.Substring(8);
                    if (LStr11001006 == "0")
                    {
                        if (AStrNewValue.Length < LIntMinLength)
                        {
                            MessageBox.Show(string.Format(S1106App.GetDisplayCharater("S1106134"), LIntMinLength.ToString()), CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                        if (AStrNewValue.Length > 64)
                        {
                            MessageBox.Show(S1106App.GetDisplayCharater("S1106135"), CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }
                    else
                    {
                        LBoolReturn = PasswordVerifyOptions.MeetComplexityRequirements(AStrNewValue, LIntMinLength, 64, "", ref LStrCallReturn);
                        if (!LBoolReturn)
                        {
                            MessageBox.Show(S1106App.GetDisplayCharater("Page00000A", "CPWDR" + LStrCallReturn), CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                            return false;
                        }
                    }
                }
                #endregion

                #region 判断超级管理员帐户的长度
                if (LStr11001003 == "11020401")
                {
                    string LStrNewAccount = AStrNewValue.Trim();
                    if (LStrNewAccount.Length <= 0)
                    {
                        MessageBox.Show(S1106App.GetDisplayCharater("S1106145"), CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                        return false;
                    }
                    if (LStrNewAccount.Length > 128)
                    {
                        MessageBox.Show(S1106App.GetDisplayCharater("S1106146"), CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
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
                WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);
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
                WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);
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

                LStrVerificationCode004 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSettedValue = EncryptionAndDecryption.EncryptDecryptString(LStrNewValue, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);
                LStrCurrent003 = IDataRowCurrentFocused["C003"].ToString();
                LListStrWcfArgs.Add(LStrCurrent003);
                LListStrWcfArgs.Add(LStrSettedValue);
                LListStrWcfArgs.Add(CurrentApp.Session.UserInfo.UserID.ToString());

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding(CurrentApp.Session);
                LEndpointAddress = WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000");
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
                WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);
                if (!IBoolCallReturn)
                {
                    MessageBox.Show(S1106App.GetDisplayCharater("S1106095") + "\n" + IStrCallReturn, CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    IDataRowCurrentFocused["C006"] = IStrCallReturn;
                    foreach (DataRow LDataRowSingle in S1106App.GDataTable11001.Rows)
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

                LStrVerificationCode004 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding(CurrentApp.Session);
                LEndpointAddress = WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);

                foreach (DataRow LDataRowSingle in S1106App.GDataTable11001.Rows)
                {
                    if (LDataRowSingle["C003"].ToString() != "11010102") { continue; }

                    LStrSettedValueInDB = LDataRowSingle["C006"].ToString();
                    LStrSettedValueInDB = EncryptionAndDecryption.EncryptDecryptString(LStrSettedValueInDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrSettedValueInDB = LStrSettedValueInDB.Substring(8);

                    if (int.Parse(LStrSettedValueInDB) >= 6) { break; }

                    LDataRowSingle["C006"] = EncryptionAndDecryption.EncryptDecryptString("110101026", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    LStrSettedValue = EncryptionAndDecryption.EncryptDecryptString("6", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LListStrWcfArgs.Clear();
                    LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());
                    LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());
                    LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);
                    LListStrWcfArgs.Add("11010102");
                    LListStrWcfArgs.Add(LStrSettedValue);
                    LListStrWcfArgs.Add(CurrentApp.Session.UserInfo.UserID.ToString());
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

                LStrVerificationCode004 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding(CurrentApp.Session);
                LEndpointAddress = WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);

                foreach (DataRow LDataRowSingle in S1106App.GDataTable11001.Rows)
                {
                    if (LDataRowSingle["C003"].ToString() != "11010203") { continue; }

                    LStrSettedValueInDB = LDataRowSingle["C006"].ToString();
                    LStrSettedValueInDB = EncryptionAndDecryption.EncryptDecryptString(LStrSettedValueInDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrSettedValueInDB = LStrSettedValueInDB.Substring(8);
                    if (int.Parse(LStrSettedValueInDB) <= int.Parse(AStrNewValue)) { return; }

                    LDataRowSingle["C006"] = EncryptionAndDecryption.EncryptDecryptString("11010203" + AStrNewValue, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    LStrSettedValue = EncryptionAndDecryption.EncryptDecryptString(AStrNewValue, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LListStrWcfArgs.Clear();
                    LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());
                    LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());
                    LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);
                    LListStrWcfArgs.Add("11010203");
                    LListStrWcfArgs.Add(LStrSettedValue);
                    LListStrWcfArgs.Add(CurrentApp.Session.UserInfo.UserID.ToString());
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

                LStrVerificationCode004 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding(CurrentApp.Session);
                LEndpointAddress = WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);

                foreach (DataRow LDataRowSingle in S1106App.GDataTable11001.Rows)
                {
                    if (LDataRowSingle["C003"].ToString() != "11020102") { continue; }

                    LStrSettedValueInDB = LDataRowSingle["C006"].ToString();
                    LStrSettedValueInDB = EncryptionAndDecryption.EncryptDecryptString(LStrSettedValueInDB, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrSettedValueInDB = LStrSettedValueInDB.Substring(8);

                    if (LStrSettedValueInDB == "0") { break; }

                    LDataRowSingle["C006"] = EncryptionAndDecryption.EncryptDecryptString("110201020", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    LStrSettedValue = EncryptionAndDecryption.EncryptDecryptString("0", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LListStrWcfArgs.Clear();
                    LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());
                    LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());
                    LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);
                    LListStrWcfArgs.Add("11020102");
                    LListStrWcfArgs.Add(LStrSettedValue);
                    LListStrWcfArgs.Add(CurrentApp.Session.UserInfo.UserID.ToString());
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
                LStrVerificationCode004 = S1106App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding(CurrentApp.Session);
                LEndpointAddress = WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000");
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
                        LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());
                        LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());
                        LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);
                        LListStrWcfArgs.Add(LStrSingleTarget);
                        LListStrWcfArgs.Add(LStrSettedValue);
                        LListStrWcfArgs.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                        LWCFOperationReturn = LService00000Client.OperationMethodA(21, LListStrWcfArgs);
                        foreach (DataRow LDataRowSingle in S1106App.GDataTable11001.Rows)
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
                        foreach (DataRow LDataRowSingle in S1106App.GDataTable11001.Rows)
                        {
                            if (LDataRowSingle["C003"].ToString() == LStrSingleTarget)
                            {
                                LStrSettedValueInDB = LDataRowSingle["C006"].ToString();
                                if (LStrSettedValueInDB != EncryptionAndDecryption.EncryptDecryptString(LStrSingleTarget + "-99", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004)) { continue; }

                                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());
                                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());
                                LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);
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
                                LListStrWcfArgs.Add(CurrentApp.Session.UserInfo.UserID.ToString());
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
                LUCGroupParameter.CurrentApp = CurrentApp;
                foreach (object LObjectSingleArg in LUCGroupParameter.StackPanelContainsParameters.Children)
                {
                    UCSingleParameter LUCSingleParameter = LObjectSingleArg as UCSingleParameter;
                    LUCSingleParameter.CurrentApp = CurrentApp;
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
                    LUCGroupParameter.CurrentApp = CurrentApp;
                    foreach (object LObjectSingleArg in LUCGroupParameter.StackPanelContainsParameters.Children)
                    {
                        UCSingleParameter LUCSingleParameter = LObjectSingleArg as UCSingleParameter;
                        LUCSingleParameter.CurrentApp = CurrentApp ;
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

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS1106;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    //string uri = string.Format("/Themes/{0}/{1}",
                    //    "Default"
                    //    , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    //Resources.MergedDictionaries.Add(resource);

                    //Application.Current.Resources.MergedDictionaries.Clear();
                    App.Current.Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }


            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            //try
            //{
            //    string uri = string.Format("/Themes/Default/UMPS1106/StyleDictionary1106.xaml");
            //    ResourceDictionary resource = new ResourceDictionary();
            //    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
            //    Resources.MergedDictionaries.Add(resource);
            //}
            //catch (Exception ex)
            //{
            //    //App.ShowExceptionMessage("3" + ex.Message);
            //}

            //var pageHead = PageHead;
            //if (pageHead != null)
            //{
            //    pageHead.ChangeTheme();
            //    pageHead.InitInfo();
            //}

        }

        //这个  要重写  10.21
        //protected override void App_NetPipeEvent(WebRequest webRequest)
        //{
        //    base.App_NetPipeEvent(webRequest);

        //    Dispatcher.Invoke(new Action(() =>
        //    {
        //        try
        //        {
        //            var code = webRequest.Code;
        //            var session = webRequest.Session;
        //            var strData = webRequest.Data;
        //            switch (code)
        //            {
        //                case (int)RequestCode.SCLanguageChange:
        //                    LangTypeInfo langTypeInfo =
        //                       CurrentApp.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
        //                    if (langTypeInfo != null)
        //                    {
        //                        LangTypeInfo = langTypeInfo;
        //                        CurrentApp.Session.LangTypeInfo = langTypeInfo;
        //                        CurrentApp.Session.LangTypeID = langTypeInfo.LangID;
        //                        if (MyWaiter != null)
        //                        {
        //                            MyWaiter.Visibility = Visibility.Visible;
        //                        }
        //                        mWorker = new BackgroundWorker();
        //                        mWorker.DoWork += (s, de) => App.LoadApplicationLanguages();
        //                        mWorker.RunWorkerCompleted += (s, re) =>
        //                        {
        //                            mWorker.Dispose();
        //                            if (MyWaiter != null)
        //                            {
        //                                MyWaiter.Visibility = Visibility.Hidden;
        //                            }
        //                            //if (PopupPanel != null)
        //                            //{
        //                            //    PopupPanel.ChangeLanguage();
        //                            //}
        //                            if (PageHead != null)
        //                            {
        //                                PageHead.SessionInfo = App.Session;
        //                                PageHead.InitInfo();
        //                            }

        //                        };
        //                        mWorker.RunWorkerAsync();
        //                    }
        //                    break;
        //                case (int)RequestCode.SCThemeChange:
        //                    ThemeInfo themeInfo = App.Session.SupportThemes.FirstOrDefault(t => t.Name == strData);
        //                    if (themeInfo != null)
        //                    {
        //                        ThemeInfo = themeInfo;
        //                        App.Session.ThemeInfo = themeInfo;
        //                        App.Session.ThemeName = themeInfo.Name;
        //                        ChangeTheme();
        //                        PageHead.SessionInfo = App.Session;
        //                        PageHead.InitInfo();
        //                    }
        //                    break;
        //                case (int)RequestCode.SCOperation:
        //                    CurrentLoadingModule = strData;
        //                    //Page00000A();
        //                    Page00000A_Loaded();
        //                    break;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            App.ShowExceptionMessage(ex.Message);
        //        }
        //    }));
        //}

        //#region 判断是否处于"忙"状态
        int IIntIdleCount = 0;
        //private bool IBoolTimerBusy = false;
        //private Timer ITimerSendMessage2Main = new Timer(1000);
        //private delegate void IDelegateSendMessage2Main();

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

        //private void ITimerSendMessage2Main_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    this.Dispatcher.Invoke(DispatcherPriority.Normal, new IDelegateSendMessage2Main(SendMessage2MainFram));
        //}

        //private void SendMessage2MainFram()
        //{
        //    try
        //    {
        //        IIntIdleCount += 1;
        //        if (IBoolTimerBusy) { return; }
        //        IBoolTimerBusy = true;
        //        WebRequest LWebRequestClientLoading = new WebRequest();
        //        LWebRequestClientLoading.Code = 91001;
        //        LWebRequestClientLoading.Data = IIntIdleCount.ToString();
        //        WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
        //    }
        //    catch { }
        //    finally
        //    {
        //        IBoolTimerBusy = false;
        //    }
        //}

        //public void StartStopTimer(bool ABoolStart)
        //{
        //    if (ABoolStart) { IIntIdleCount = 0; ITimerSendMessage2Main.Start(); } else { ITimerSendMessage2Main.Stop(); IIntIdleCount = 0; }
        //}

        //#endregion

        //public override void ChangeLanguage()
        //{
        //    base.ChangeLanguage();
        //    Init();
        //}

    }
}
