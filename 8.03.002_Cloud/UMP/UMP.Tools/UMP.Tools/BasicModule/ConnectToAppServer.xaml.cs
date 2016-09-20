using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UMP.Tools.PublicClasses;
using UMP.Tools.UMPWcfService00003;

namespace UMP.Tools.BasicModule
{
    public partial class ConnectToAppServer : Window,OperationsInterface
    {
        public event EventHandler<UMP.Tools.PublicClasses.OperationEventArgs> IOperationEvent;

        private int IIntServerSelectedIndex = -1, IIntLoginNameSelectedIndex = -1;

        /// <summary>
        /// 存放已经成功连接过的服务器连接信息
        /// </summary>
        private ConnectedServerInformation IConnectedServerInformation;

        //本界面上的连接参数是否被修改过
        private bool IBoolConnectArgsChanged = false;

        /// <summary>
        /// 点击Connect Button后的暂时保存的连接信息
        /// </summary>
        private List<string> IListConnectInfo = new List<string>();

        /// <summary>
        /// 是否在连接服务器的过程中
        /// </summary>
        private bool IBoolInConnecting = false;

        private OperationDataArgs I00003OperationReturn = new OperationDataArgs();

        #region 加密、解密验证码
        private string IStrVerificationCode001 = string.Empty;
        private string IStrVerificationCode101 = string.Empty;
        #endregion
        
        public ConnectToAppServer()
        {
            InitializeComponent();

            this.Loaded += ConnectToAppServer_Loaded;
            this.Closing += ConnectToAppServer_Closing;
            this.MouseLeftButtonDown += ConnectToAppServer_MouseLeftButtonDown;
            MainPanel.KeyDown += MainPanel_KeyDown;

            ButtonApplicationMenu.Click += WindowsButtonClicked;
            ButtonCloseConnect.Click += WindowsButtonClicked;

            ButtonUsedOffline.Click += WindowsButtonClicked;
            ButtonConnectServer.Click += WindowsButtonClicked;
            ButtonCloseWindow.Click += WindowsButtonClicked;
        }

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelConnectServerTip.Content = App.GetDisplayCharater("M01007");
            TabItemServer.Header = " " + App.GetDisplayCharater("M01016") + " ";
            LabelServerTip.Content = App.GetDisplayCharater("M01017");
            LabelServerName.Content = App.GetDisplayCharater("M01018");
            LabelServerPort.Content = App.GetDisplayCharater("M01019");
            LabelLoginName.Content = App.GetDisplayCharater("M01020");
            LabelLoginPassword.Content = App.GetDisplayCharater("M01021");

            ButtonConnectServer.Content = App.GetDisplayCharater("M01022");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M01023");
            ButtonUsedOffline.Content = App.GetDisplayCharater("M01024");
        }

        private void ConnectToAppServer_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this);
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();

            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
            ImageLinkToServer.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000008.ico"), UriKind.RelativeOrAbsolute));

            IStrVerificationCode001 = CreateVerificationCode(PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M001);
            IStrVerificationCode101 = CreateVerificationCode(PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M101);

            TextBoxServerPort.SetElementData("8081");

            DisplayElementCharacters(false);
            InitConnectedServerList();
            ComboBoxServerName.Focus();
        }

        private void GSystemMainWindow_IOperationEvent(object sender, UMP.Tools.PublicClasses.OperationEventArgs e)
        {
            if (e.StrElementTag == "CSID")
            {
                App.DrawWindowsBackGround(this);
            }

            if (e.StrElementTag == "CLID")
            {
                DisplayElementCharacters(true);
            }

            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
        }

        private void ConnectToAppServer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IBoolInConnecting) { e.Cancel = true; return; }
            App.GSystemMainWindow.IOperationEvent -= GSystemMainWindow_IOperationEvent;
        }

        private void ConnectToAppServer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void MainPanel_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var uie = e.OriginalSource as UIElement;
                if (e.Key == Key.Enter)
                {
                    uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
            catch { }
        }

        private void WindowsButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button LButtonClicked = sender as Button;
                string LStrClickedName = LButtonClicked.Name;

                switch (LStrClickedName)
                {
                    case "ButtonApplicationMenu":
                        //目标   
                        LButtonClicked.ContextMenu.PlacementTarget = LButtonClicked;
                        //位置   
                        LButtonClicked.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                        //显示菜单   
                        LButtonClicked.ContextMenu.IsOpen = true;
                        break;
                    case "ButtonCloseConnect":
                        CloseThisWindow();
                        break;
                    case "ButtonConnectServer":
                        BeginConnect2UMPServer();
                        break;
                    case "ButtonCloseWindow":
                        CloseThisWindow();
                        break;
                    default:
                        break;
                }

            }
            catch { }
        }

        private void CloseThisWindow()
        {
            if (!IBoolInConnecting) { this.Close(); }
        }

        private void InitConnectedServerList()
        {
            try
            {
                IConnectedServerInformation = ConnectedServerXmlOperations.ReadConnectedServerList(App.GStrProgramDataDirectory, true);
                if (!IConnectedServerInformation.BoolReturn)
                {
                    return;
                }
                ComboBoxServerName.Items.Clear();
                foreach (List<string> LListStrSingleServer in IConnectedServerInformation.ListStrConnectedServer)
                {
                    ComboBoxItem LComboBoxServerItem = new ComboBoxItem();

                    LComboBoxServerItem.Background = Brushes.Transparent;
                    LComboBoxServerItem.Style = (Style)App.Current.Resources["ComboBoxItemNormalStyle"];
                    LComboBoxServerItem.Content = LListStrSingleServer[0];
                    LComboBoxServerItem.DataContext = LListStrSingleServer[0];
                    LComboBoxServerItem.Tag = LListStrSingleServer[1];
                    ComboBoxServerName.Items.Add(LComboBoxServerItem);
                }
                ComboBoxServerName.SelectionChanged += ComboBoxObjectSelectionChanged;
                foreach (ComboBoxItem LComboBoxSingleServerItem in ComboBoxServerName.Items)
                {
                    if (LComboBoxSingleServerItem.Tag.ToString() == "1") { LComboBoxSingleServerItem.IsSelected = true; break; }
                }
            }
            catch { }
        }

        private void InitConnectedLoginArgs(int AIntServerIndex)
        {
            try
            {
                List<List<string>> LListListStrConnectArgsList = IConnectedServerInformation.ListListStrConnectedArgs[AIntServerIndex];
                ComboBoxLoginName.Items.Clear();
                foreach (List<string> LListStrSingleConnectArgs in LListListStrConnectArgsList)
                {
                    ComboBoxItem LComboBoxItemLoginName = new ComboBoxItem();
                    LComboBoxItemLoginName.Background = Brushes.Transparent;
                    LComboBoxItemLoginName.Margin = new Thickness(0, 1, 0, 1);
                    LComboBoxItemLoginName.Style = (Style)App.Current.Resources["ComboBoxItemNormalStyle"];
                    LComboBoxItemLoginName.Height = 24;
                    LComboBoxItemLoginName.Content = EncryptionAndDecryption.EncryptDecryptString(LListStrSingleConnectArgs[2], IStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);                               //登录名
                    LComboBoxItemLoginName.DataContext = LListStrSingleConnectArgs[1] + App.GStrSpliterChar + LListStrSingleConnectArgs[3]; //记住密码 + char(27) + 密码
                    LComboBoxItemLoginName.Tag = LListStrSingleConnectArgs[0];                                                                  //最后登录的用户
                    ComboBoxLoginName.Items.Add(LComboBoxItemLoginName);
                }
                CheckBoxRemberPassword.IsChecked = false;
                ComboBoxLoginName.SelectionChanged += ComboBoxObjectSelectionChanged;
                foreach (ComboBoxItem LComboBoxItemSingleConnectArgs in ComboBoxLoginName.Items)
                {
                    if (LComboBoxItemSingleConnectArgs.Tag.ToString() == "1") { LComboBoxItemSingleConnectArgs.IsSelected = true; break; }
                }
                IBoolConnectArgsChanged = false;
            }
            catch { }
        }

        private void ComboBoxObjectSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string LStrSelectionChangedComboBoxName = string.Empty;
            string LStrSelectedServerName = string.Empty;
            string LStrSelectedLoginName = string.Empty;

            try
            {
                ComboBox LComboBoxChanged = sender as ComboBox;
                LStrSelectionChangedComboBoxName = LComboBoxChanged.Name;

                if (LStrSelectionChangedComboBoxName == "ComboBoxServerName")
                {
                    IIntServerSelectedIndex = -1; IIntLoginNameSelectedIndex = -1;
                    ComboBoxItem LComboBoxItemSelectedServer = ComboBoxServerName.SelectedItem as ComboBoxItem;
                    if (LComboBoxItemSelectedServer == null)
                    {
                        if (!IBoolConnectArgsChanged)
                        {
                            PasswordBoxLoginPassword.Password = "";
                            CheckBoxRemberPassword.IsChecked = false;
                        }
                        return;
                    }
                    IIntServerSelectedIndex = ComboBoxServerName.SelectedIndex;
                    LStrSelectedServerName = LComboBoxItemSelectedServer.DataContext.ToString();
                    InitConnectedLoginArgs(IIntServerSelectedIndex);
                }

                if (LStrSelectionChangedComboBoxName == "ComboBoxLoginName")
                {
                    IIntLoginNameSelectedIndex = -1;
                    ComboBoxItem LComboBoxItemSelectedLoginName = ComboBoxLoginName.SelectedItem as ComboBoxItem;
                    if (LComboBoxItemSelectedLoginName == null || IIntServerSelectedIndex < 0)
                    {
                        IBoolConnectArgsChanged = true;
                        PasswordBoxLoginPassword.Password = "";
                        CheckBoxRemberPassword.IsChecked = false;
                        return;
                    }
                    IIntLoginNameSelectedIndex = ComboBoxLoginName.SelectedIndex;
                    List<string> LListStrSingleConnectArgs = IConnectedServerInformation.ListListStrConnectedArgs[IIntServerSelectedIndex][IIntLoginNameSelectedIndex];
                    CheckBoxRemberPassword.IsChecked = false;
                    PasswordBoxLoginPassword.Password = "";
                    if (LListStrSingleConnectArgs[1] == "1")
                    {
                        CheckBoxRemberPassword.IsChecked = true;
                        PasswordBoxLoginPassword.Password = EncryptionAndDecryption.EncryptDecryptString(LListStrSingleConnectArgs[3], IStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    }
                }
            }
            catch { }
        }

        #region 连接到 UMP 应用服务器
        private BackgroundWorker IBackgroundWorkerA = null;
        private BackgroundWorker IBackgroundWorkerB = null;
        private BackgroundWorker IBackgroundWorkerC = null;
        private BackgroundWorker IBackgroundWorkerD = null;
        private BackgroundWorker IBackgroundWorkerE = null;
        private BackgroundWorker IBackgroundWorkerF = null;
        private BackgroundWorker IBackgroundWorkerG = null;

        /// <summary>
        /// 验证界面输入的连接信息是否正确
        /// </summary>
        /// <param name="AStrReturn">返回错误码和错误信息 </param>
        /// <returns>True：验证正确； False：验证错误</returns>
        private bool ConfirmConnectParameter(ref string AStrReturn)
        {
            bool LBoolReturn = false;

            string LStrServerName = string.Empty;
            int LIntServerPort = 0;
            string LStrLoginName = string.Empty;
            string LStrLoginPassword = string.Empty;
            string LStrRememberPassword = "0";
            int LIntNetworkProtocolIndex = 1;
            string LStrNetworkProtocol = string.Empty;
            int LIntConnectionTimeOut = 60;
            int LIntExcutionTimeOut = 0;

            try
            {
                IListConnectInfo.Clear();

                LStrServerName = ComboBoxServerName.Text.Trim();
                if (string.IsNullOrEmpty(LStrServerName)) { AStrReturn = "E001001" + App.GStrSpliterChar; ComboBoxServerName.Focus(); return LBoolReturn; }

                if (!int.TryParse(TextBoxServerPort.GetElementData().Trim(), out LIntServerPort)) { AStrReturn = "E001002" + App.GStrSpliterChar; TextBoxServerPort.Focus(); return LBoolReturn; }
                if (LIntServerPort <= 10) { AStrReturn = "E001002" + App.GStrSpliterChar; TextBoxServerPort.Focus(); return LBoolReturn; }

                LStrLoginName = ComboBoxLoginName.Text.Trim();
                if (string.IsNullOrEmpty(LStrLoginName)) { AStrReturn = "E001003" + App.GStrSpliterChar; ComboBoxLoginName.Focus(); return LBoolReturn; }

                LStrLoginPassword = PasswordBoxLoginPassword.Password;
                if (string.IsNullOrEmpty(LStrLoginPassword)) { AStrReturn = "E001004" + App.GStrSpliterChar; PasswordBoxLoginPassword.Focus(); return LBoolReturn; }

                if (CheckBoxRemberPassword.IsChecked == true) { LStrRememberPassword = "1"; }

                IListConnectInfo.Add(LStrServerName);                       //0
                IListConnectInfo.Add(LIntServerPort.ToString());            //1
                IListConnectInfo.Add(LStrLoginName);                        //2
                IListConnectInfo.Add(LStrLoginPassword);                    //3
                IListConnectInfo.Add(LStrRememberPassword);                 //4
                IListConnectInfo.Add(LIntNetworkProtocolIndex.ToString());   //5
                IListConnectInfo.Add(LStrNetworkProtocol);                  //6
                IListConnectInfo.Add(LIntConnectionTimeOut.ToString());     //7
                IListConnectInfo.Add(LIntExcutionTimeOut.ToString());       //8
                IListConnectInfo.Add("F");                                  //9首次连接服务器
                IListConnectInfo.Add("");                                   //10登录流水号
                IListConnectInfo.Add("");                                   //11用户19位编码
                LBoolReturn = true;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "E001999" + App.GStrSpliterChar + ex.Message;
            }

            return LBoolReturn;
        }

        private void BeginConnect2UMPServer()
        {
            string LStrConfirmReturn = string.Empty;
            string LStrMessageBody = string.Empty;

            try
            {
                if (IBoolInConnecting) { return; }

                if (!ConfirmConnectParameter(ref LStrConfirmReturn))
                {
                    string[] LStrArrayReturn = LStrConfirmReturn.Split(App.GStrSpliterChar.ToCharArray());
                    LStrMessageBody = App.GetDisplayCharater(LStrArrayReturn[0]);
                    if (LStrArrayReturn[0] == "E001999")
                    {
                        LStrMessageBody += "\n" + LStrArrayReturn[1];
                    }
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                TryConnect2UMPServer();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("BeginConnect2UMPServer()\n" + ex.Message);
            }
        }

        #region 第一步：尝试连接到UMP应用服务器
        private void TryConnect2UMPServer()
        {
            try
            {
                IBoolInConnecting = true;
                App.ShowCurrentStatus(1, string.Format(App.GetDisplayCharater("M01025"), IListConnectInfo[0]));
                if (IBackgroundWorkerA == null) { IBackgroundWorkerA = new BackgroundWorker(); }
                IBackgroundWorkerA.RunWorkerCompleted += IBackgroundWorkerA_RunWorkerCompleted;
                IBackgroundWorkerA.DoWork += IBackgroundWorkerA_DoWork;
                IBackgroundWorkerA.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                IBoolInConnecting = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
                App.ShowExceptionMessage(App.GetDisplayCharater("E001005") + "\n" + ex.Message);
            }
        }

        private void IBackgroundWorkerA_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();

            try
            {
                LBasicHttpBinding = App.CreateBasicHttpBinding(false, 25);
                LEndpointAddress = App.CreateEndpointAddress(IListConnectInfo[0], IListConnectInfo[1], false, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);
                LListWcfArgs.Add(IListConnectInfo[0]); LListWcfArgs.Add(IListConnectInfo[1]);
                I00003OperationReturn = LService00003Client.OperationMethodA(1, LListWcfArgs);
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP001E001" + App.GStrSpliterChar + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }
        }

        private void IBackgroundWorkerA_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrMessageBody = string.Empty;

            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (!I00003OperationReturn.BoolReturn)
                {
                    string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                    LStrMessageBody = App.GetDisplayCharater(LStrOperationReturn[0]);
                    if (LStrOperationReturn[0] == "WCF003E999" || LStrOperationReturn[0] == "UMP001E001")
                    {
                        LStrMessageBody += "\n" + LStrOperationReturn[1];
                    }
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    IBoolInConnecting = false;
                    return;
                }
                CheckCertificateIsInstalled();
            }
            catch (Exception ex)
            {
                IBoolInConnecting = false;
                App.ShowExceptionMessage(App.GetDisplayCharater("E001005") + "\n" + ex.Message);
            }
            finally
            {
                if (IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
            }
        }
        #endregion

        #region 第二步：检测https使用的证书是否在客户端已经安装
        bool IBoolInRoot = false;
        bool IBoolInTrustedPublisher = false;

        private void CheckCertificateIsInstalled()
        {
            try
            {
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01026"));
                if (IBackgroundWorkerB == null) { IBackgroundWorkerB = new BackgroundWorker(); }
                IBackgroundWorkerB.RunWorkerCompleted += IBackgroundWorkerB_RunWorkerCompleted;
                IBackgroundWorkerB.DoWork += IBackgroundWorkerB_DoWork;
                IBackgroundWorkerB.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                IBoolInConnecting = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerB != null)
                {
                    IBackgroundWorkerB.Dispose(); IBackgroundWorkerB = null;
                }
                App.ShowExceptionMessage(App.GetDisplayCharater("E001006") + "\n" + ex.Message);
            }
        }

        private void IBackgroundWorkerB_DoWork(object sender, DoWorkEventArgs e)
        {
            
            string LStrServerHost = string.Empty;
            string LStrCerHashString = string.Empty;
            string LStrCallReturn = string.Empty;

            try
            {
                IBoolInRoot = false; IBoolInTrustedPublisher = false;
                string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                LStrServerHost = LStrOperationReturn[0];
                LStrCerHashString = LStrOperationReturn[1];

                IBoolInRoot = CertificateOperations.CertificateIsExist(LStrServerHost, LStrCerHashString, StoreName.Root, StoreLocation.LocalMachine, ref LStrCallReturn);
                if (!IBoolInRoot)
                {
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        I00003OperationReturn.BoolReturn = false;
                        I00003OperationReturn.StringReturn = LStrCallReturn;
                        return;
                    }
                }
                IBoolInTrustedPublisher = CertificateOperations.CertificateIsExist(LStrServerHost, LStrCerHashString, StoreName.TrustedPublisher, StoreLocation.LocalMachine, ref LStrCallReturn);
                if (!IBoolInTrustedPublisher)
                {
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        I00003OperationReturn.BoolReturn = false;
                        I00003OperationReturn.StringReturn = LStrCallReturn;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP001E002" + App.GStrSpliterChar + ex.Message;
            }
        }

        private void IBackgroundWorkerB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (!I00003OperationReturn.BoolReturn)
                {
                    IBoolInConnecting = false;
                    string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                    MessageBox.Show(App.GetDisplayCharater(LStrOperationReturn[0]) + "\n" + LStrOperationReturn[1], App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!IBoolInRoot || !IBoolInTrustedPublisher)
                {
                    InstallCertificates(); return;
                }
                VerifyLoginIDAndPassword();
            }
            catch (Exception ex)
            {
                IBoolInConnecting = false;
                App.ShowExceptionMessage(App.GetDisplayCharater("E001006") + "\n" + ex.Message);
            }
            finally
            {
                if (IBackgroundWorkerB != null)
                {
                    IBackgroundWorkerB.Dispose(); IBackgroundWorkerB = null;
                }
            }
        }
        #endregion

        #region 第三步：安装安全证书
        private void InstallCertificates()
        {
            try
            {
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01027"));
                if (IBackgroundWorkerC == null) { IBackgroundWorkerC = new BackgroundWorker(); }
                IBackgroundWorkerC.RunWorkerCompleted += IBackgroundWorkerC_RunWorkerCompleted;
                IBackgroundWorkerC.DoWork += IBackgroundWorkerC_DoWork;
                IBackgroundWorkerC.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                IBoolInConnecting = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerC != null)
                {
                    IBackgroundWorkerC.Dispose(); IBackgroundWorkerC = null;
                }
                App.ShowExceptionMessage(App.GetDisplayCharater("E001007") + "\n" + ex.Message);
            }
        }

        private void IBackgroundWorkerC_DoWork(object sender, DoWorkEventArgs e)
        {
            string LStrServerHost = string.Empty;
            bool LBoolDownloaded = false;
            bool LBoolInstalled = false;
            string LStrCertificateFile = string.Empty;
            string LStrCallReturn = string.Empty;

            try
            {
                LBoolDownloaded = CertificateOperations.DownloadCertificate(IListConnectInfo[0], IListConnectInfo[1], "UMP.S." + IListConnectInfo[0] + ".pfx", ref LStrCallReturn);
                if (!LBoolDownloaded)
                {
                    I00003OperationReturn.BoolReturn = false;
                    I00003OperationReturn.StringReturn = LStrCallReturn;
                    return;
                }
                LStrCertificateFile = LStrCallReturn;
                e.Result = LStrCertificateFile;

                if (!IBoolInRoot)
                {
                    LBoolInstalled = CertificateOperations.InstallCertificate(LStrCertificateFile, "VoiceCyber,123", StoreName.Root, StoreLocation.LocalMachine, ref LStrCallReturn);
                    if (!LBoolInstalled)
                    {
                        I00003OperationReturn.BoolReturn = false;
                        I00003OperationReturn.StringReturn = LStrCallReturn;
                        return;
                    }
                }

                if (!IBoolInTrustedPublisher)
                {
                    LBoolInstalled = CertificateOperations.InstallCertificate(LStrCertificateFile, "VoiceCyber,123", StoreName.TrustedPublisher, StoreLocation.LocalMachine, ref LStrCallReturn);
                    if (!LBoolInstalled)
                    {
                        I00003OperationReturn.BoolReturn = false;
                        I00003OperationReturn.StringReturn = LStrCallReturn;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP001E003" + App.GStrSpliterChar + ex.Message;
            }
        }

        private void IBackgroundWorkerC_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrCertificateFile = string.Empty;
            string LStrMessageBody = string.Empty;

            try
            {
                LStrCertificateFile = e.Result.ToString();
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (!I00003OperationReturn.BoolReturn)
                {
                    IBoolInConnecting = false;
                    string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                    LStrMessageBody = string.Format(App.GetDisplayCharater(LStrOperationReturn[0]), LStrCertificateFile);
                    LStrMessageBody += "\n" + LStrOperationReturn[1];
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                VerifyLoginIDAndPassword();
            }
            catch (Exception ex)
            {
                IBoolInConnecting = false;
                App.ShowExceptionMessage(App.GetDisplayCharater("E001007") + "\n" + ex.Message);
            }
            finally
            {
                if (IBackgroundWorkerC != null)
                {
                    IBackgroundWorkerC.Dispose(); IBackgroundWorkerC = null;
                }
            }
        }
        #endregion

        #region 第四步：验证用户名和密码
        private void VerifyLoginIDAndPassword()
        {
            try
            {
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01028"));
                if (IBackgroundWorkerD == null) { IBackgroundWorkerD = new BackgroundWorker(); }
                IBackgroundWorkerD.RunWorkerCompleted += IBackgroundWorkerD_RunWorkerCompleted;
                IBackgroundWorkerD.DoWork += IBackgroundWorkerD_DoWork;
                IBackgroundWorkerD.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                IBoolInConnecting = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerD != null)
                {
                    IBackgroundWorkerD.Dispose(); IBackgroundWorkerD = null;
                }
                App.ShowExceptionMessage(App.GetDisplayCharater("E001008") + "\n" + ex.Message);
            }
            
        }

        private void IBackgroundWorkerD_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();

            try
            {
                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 25);
                LEndpointAddress = App.CreateEndpointAddress(IListConnectInfo[0], IListConnectInfo[1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);
                LListWcfArgs.Add(IListConnectInfo[2]); LListWcfArgs.Add(IListConnectInfo[3]); LListWcfArgs.Add(App.GStrComputerName);
                I00003OperationReturn = LService00003Client.OperationMethodA(2, LListWcfArgs);
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP001E004" + App.GStrSpliterChar + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }
        }

        private void IBackgroundWorkerD_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrMessageBody = string.Empty;

            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (!I00003OperationReturn.BoolReturn)
                {
                    string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                    LStrMessageBody = App.GetDisplayCharater(LStrOperationReturn[0]);
                    if (LStrOperationReturn[0] == "WCF003E999" || LStrOperationReturn[0] == "UMP001E004")
                    {
                        LStrMessageBody += "\n" + LStrOperationReturn[1];
                    }
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    IBoolInConnecting = false;
                    return;
                }
                string[] LStrOperationInfo = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                IListConnectInfo[10] = LStrOperationInfo[0];
                IListConnectInfo[11] = LStrOperationInfo[1];
                SaveConnectionInformation();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(App.GetDisplayCharater("E001008") + "\n" + ex.Message);
                IBoolInConnecting = false;
            }
            finally
            {
                if (IBackgroundWorkerD != null)
                {
                    IBackgroundWorkerD.Dispose(); IBackgroundWorkerD = null;
                }
            }
        }
        #endregion

        #region 第五步：将连接信息写入到本地XML文件
        private void SaveConnectionInformation()
        {
            try
            {
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01029"));
                if (IBackgroundWorkerE == null) { IBackgroundWorkerE = new BackgroundWorker(); }
                IBackgroundWorkerE.RunWorkerCompleted += IBackgroundWorkerE_RunWorkerCompleted;
                IBackgroundWorkerE.DoWork += IBackgroundWorkerE_DoWork;
                IBackgroundWorkerE.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                IBoolInConnecting = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerE != null)
                {
                    IBackgroundWorkerE.Dispose(); IBackgroundWorkerE = null;
                }
                App.ShowExceptionMessage(App.GetDisplayCharater("E001009") + "\n" + ex.Message);
            }
        }

        private void IBackgroundWorkerE_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ConnectedServerInformation LConnectedServerInformationReturn = ConnectedServerXmlOperations.AddConnectedServerInformation(App.GStrProgramDataDirectory, IListConnectInfo, false);
                I00003OperationReturn.BoolReturn = LConnectedServerInformationReturn.BoolReturn;
                I00003OperationReturn.StringReturn = LConnectedServerInformationReturn.StrReturn;
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP001E005" + App.GStrSpliterChar + ex.Message;
            }
        }

        private void IBackgroundWorkerE_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrMessageBody = string.Empty;

            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (!I00003OperationReturn.BoolReturn)
                {
                    string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                    LStrMessageBody = App.GetDisplayCharater(LStrOperationReturn[0]);
                    LStrMessageBody += "\n" + LStrOperationReturn[1];
                    IBoolInConnecting = false;
                    return;
                }

                ReadUMPUsedDatabaseInformation();
                
            }
            catch (Exception ex)
            {
                IBoolInConnecting = false;
                App.ShowExceptionMessage(App.GetDisplayCharater("E001009") + "\n" + ex.Message);
            }
            finally
            {
                if (IBackgroundWorkerE != null)
                {
                    IBackgroundWorkerE.Dispose(); IBackgroundWorkerE = null;
                }
            }
        }
        #endregion

        #region 第六步：读取 UMP 使用的数据库信息
        private void ReadUMPUsedDatabaseInformation()
        {
            try
            {
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01030"));
                if (IBackgroundWorkerF == null) { IBackgroundWorkerF = new BackgroundWorker(); }
                IBackgroundWorkerF.RunWorkerCompleted += IBackgroundWorkerF_RunWorkerCompleted;
                IBackgroundWorkerF.DoWork += IBackgroundWorkerF_DoWork;
                IBackgroundWorkerF.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                IBoolInConnecting = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerF != null)
                {
                    IBackgroundWorkerF.Dispose(); IBackgroundWorkerF = null;
                }
                App.ShowExceptionMessage(App.GetDisplayCharater("E001010") + "\n" + ex.Message);
            }
        }

        private void IBackgroundWorkerF_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();

            try
            {
                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 25);
                LEndpointAddress = App.CreateEndpointAddress(IListConnectInfo[0], IListConnectInfo[1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);
                I00003OperationReturn = LService00003Client.OperationMethodA(3, LListWcfArgs);
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP001E006" + App.GStrSpliterChar + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }
        }

        private void IBackgroundWorkerF_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrMessageBody = string.Empty;

            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                IBoolInConnecting = false;
                if (!I00003OperationReturn.BoolReturn)
                {
                    string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                    LStrMessageBody = App.GetDisplayCharater(LStrOperationReturn[0]);
                    if (LStrOperationReturn[0] == "WCF003E999" || LStrOperationReturn[0] == "UMP001E006")
                    {
                        LStrMessageBody += "\n" + LStrOperationReturn[1];
                        MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                    
                UMP.Tools.PublicClasses.OperationEventArgs LOperationEventArgs = new UMP.Tools.PublicClasses.OperationEventArgs();
                LOperationEventArgs.StrElementTag = "SLTS";
                LOperationEventArgs.ObjSource = new List<string>(IListConnectInfo.ToArray());
                LOperationEventArgs.AppenObjeSource1 = I00003OperationReturn.DataSetReturn;
                LOperationEventArgs.AppenObjeSource2 = I00003OperationReturn.ListDataSetReturn;
                IOperationEvent(this, LOperationEventArgs);
                this.Close();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(App.GetDisplayCharater("E001008") + "\n" + ex.Message);
                IBoolInConnecting = false;
            }
            finally
            {
                if (IBackgroundWorkerD != null)
                {
                    IBackgroundWorkerD.Dispose(); IBackgroundWorkerD = null;
                }
            }
        }
        #endregion

        #endregion

        #region 创建加密解密验证字符串
        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            string LStrTemp = string.Empty;

            try
            {
                Random LRandom = new Random();
                LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = LRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                LIntRand = LRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                LIntRand = LRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }
        #endregion
    }
}
