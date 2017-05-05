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
using System.Windows.Shapes;
using System.Xml;
using PFShareClassesS;
using UMP.MAMT.PublicClasses;

using System.Threading;
namespace UMP.MAMT.BasicModule
{
    public partial class ConnectToServer : Window, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        //是否在连接服务器的过程中
        private bool IBoolInConnecting = false;

        public ConnectToServer()
        {
            InitializeComponent();

            this.Loaded += ConnectToServer_Loaded;
            this.Closing += ConnectToServer_Closing;
            this.MouseLeftButtonDown += ConnectToServer_MouseLeftButtonDown;
            
            MainPanel.KeyDown += MainPanel_KeyDown;

            ButtonApplicationMenu.Click += WindowsButtonClicked;
            ButtonCloseConnect.Click += WindowsButtonClicked;
            ButtonConnectServer.Click += WindowsButtonClicked;
            ButtonCloseWindow.Click += WindowsButtonClicked;
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
                        BeginConnect2UMPAppServer();
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

        private void ConnectToServer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void ConnectToServer_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this);
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
            ImageLinkToServer.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000008.ico"), UriKind.RelativeOrAbsolute));
            DisplayElementCharacters(false);

            if (App.GBoolRunAtServer)
            {
                TextBoxServerName.Text = "127.0.0.1";
                TextBoxServerPort.Text = "8081";
                TextBoxLoginName.Text = "administrator";
            }
            TextBoxServerName.Focus();
            if (App.GStrAllowRemoteConnect == "0")
            {
                TextBoxServerName.IsReadOnly = true;
                TextBoxServerPort.IsReadOnly = true;
                PasswordBoxLoginPassword.Focus();
            }
        }

        private void GSystemMainWindow_IOperationEvent(object sender, MamtOperationEventArgs e)
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

        private void ConnectToServer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IBoolInConnecting) { e.Cancel = true; return; }
            App.GSystemMainWindow.IOperationEvent -= GSystemMainWindow_IOperationEvent;
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
            ButtonConnectServer1.Content = App.GetDisplayCharater("M01022");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M01023");
        }

        #region 连接到UMP应用服务器
        //点击Connect Button后的暂时保存的连接信息
        private List<string> IListConnectArguments = new List<string>();
        private BackgroundWorker IBackgroundWorkerConnectToServer = null;
        private bool IBoolLoginReturn = false;
        private string IStrLoginReturn = string.Empty;
        private DataSet IDataSetServerParameters = null;

        private void BeginConnect2UMPAppServer()
        {
            string LStrConfirmReturn = string.Empty;
            string LStrMessageBody = string.Empty;

            try
            {
                ButtonConnectServer.Visibility = System.Windows.Visibility.Hidden; 
                if (!ConfirmConnectParameter(ref LStrConfirmReturn))
                {
                    LStrMessageBody = App.GetDisplayCharater(LStrConfirmReturn);
                    if (!string.IsNullOrEmpty(App.GStrCatchException)) { LStrMessageBody += "\n\n" + App.GStrCatchException; }
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    ButtonConnectServer.Visibility = System.Windows.Visibility.Visible;
                    return;
                }
                IDataSetServerParameters = new DataSet();
                
                IBoolInConnecting = true;
                App.ShowCurrentStatus(1, string.Format(App.GetDisplayCharater("M01024"), IListConnectArguments[0]));
                if (IBackgroundWorkerConnectToServer == null) { IBackgroundWorkerConnectToServer = new BackgroundWorker(); }
                IBackgroundWorkerConnectToServer.RunWorkerCompleted += IBackgroundWorkerConnectToServer_RunWorkerCompleted;
                IBackgroundWorkerConnectToServer.DoWork += IBackgroundWorkerConnectToServer_DoWork;
                IBackgroundWorkerConnectToServer.RunWorkerAsync();
                
            }
            catch
            {
                IBoolInConnecting = false;
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                if (IBackgroundWorkerConnectToServer != null)
                {
                    IBackgroundWorkerConnectToServer.Dispose();
                    IBackgroundWorkerConnectToServer = null;
                }
            }
        }

        private void IBackgroundWorkerConnectToServer_DoWork(object sender, DoWorkEventArgs e)
        {
           
            if(App.GStrComputerIPAddress.Contains(IListConnectArguments[0] + App.GStrSpliterChar))
            {
                IBoolLoginReturn = VerifyLoginIDAndPassword(ref IStrLoginReturn);
                IDataSetServerParameters.Tables.Add(App.GetCertificateInstalledInfo()); //0
                IDataSetServerParameters.Tables.Add(App.GetIISBindingProtocol());       //1
                IDataSetServerParameters.Tables.Add(App.GetUMPDatabaseProfile());       //2
                IDataSetServerParameters.Tables.Add(App.GetLicenseServerInfo(false));   //3
            }
        }
        private void IBackgroundWorkerConnectToServer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {         
            try
            {

                         
                    IBoolInConnecting = false;
                    App.ShowCurrentStatus(int.MaxValue, string.Empty);
                    if (IBackgroundWorkerConnectToServer != null)
                    {
                        IBackgroundWorkerConnectToServer.Dispose();
                        IBackgroundWorkerConnectToServer = null;
                    }
                    if (IBoolLoginReturn)
                    {
                        if (IListConnectArguments != null)
                        {
                            MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                            LEventArgs.StrElementTag = "SLTS";
                            LEventArgs.ObjSource = new List<string>(IListConnectArguments.ToArray());
                            LEventArgs.AppenObjeSource1 = IDataSetServerParameters;
                            IOperationEvent(this, LEventArgs);
                            this.Close();
                        }
                    }
                    else
                    {
                        string LStrMessageBody = string.Empty;
                        LStrMessageBody = App.GetDisplayCharater(IStrLoginReturn);
                        if (!string.IsNullOrEmpty(App.GStrCatchException)) { LStrMessageBody += "\n\n" + App.GStrCatchException; }
                        MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    ButtonConnectServer.Visibility = System.Windows.Visibility.Visible; 
            }
            catch { }
        }

        //验证输入的参数是否符合格式
        private bool ConfirmConnectParameter(ref string AStrReturn)
        {
            bool LBoolReturn = false;

            string LStrServerName = string.Empty;
            string LStrServerPort = string.Empty;
            int LIntServerPort = 0;
            string LStrLoginName = string.Empty;
            string LStrLoginPassword = string.Empty;


            try
            {
                IListConnectArguments.Clear();
                App.GStrCatchException = string.Empty;

                LStrServerName = TextBoxServerName.Text.Trim();
                LStrServerPort = TextBoxServerPort.Text.Trim();
                LStrLoginName = TextBoxLoginName.Text.Trim();
                LStrLoginPassword = PasswordBoxLoginPassword.Password;

                if (string.IsNullOrEmpty(LStrServerName)) { AStrReturn = "E00001"; TextBoxServerName.Focus(); return LBoolReturn; }
                if (!int.TryParse(LStrServerPort, out LIntServerPort)) { AStrReturn = "E00002"; TextBoxServerPort.Focus(); return LBoolReturn; }
                if (LIntServerPort <= 1024 || LIntServerPort >= 65535) { AStrReturn = "E00002"; TextBoxServerPort.Focus(); return LBoolReturn; }
                if (string.IsNullOrEmpty(LStrLoginName)) { AStrReturn = "E00003"; TextBoxLoginName.Focus(); return LBoolReturn; }
                if (string.IsNullOrEmpty(LStrLoginPassword)) { AStrReturn = "E00004"; PasswordBoxLoginPassword.Focus(); return LBoolReturn; }

                IListConnectArguments.Add(LStrServerName); IListConnectArguments.Add(LStrServerPort); IListConnectArguments.Add(LStrLoginName); IListConnectArguments.Add(LStrLoginPassword);

                LBoolReturn = true;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "E00000";
                App.GStrCatchException = ex.ToString();
            }
            return LBoolReturn;
        }

        //在本机验证输入的用户名和密码
        private bool VerifyLoginIDAndPassword(ref string AStrReturn)
        {
            bool LBoolReturn = true;

            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode001 = string.Empty;
            string LStrVerificationCode101 = string.Empty;

            string LStrA01 = string.Empty, LStrA02 = string.Empty, LStrA03 = string.Empty, LStrA04 = string.Empty, LStrA05 = string.Empty, LStrA06 = string.Empty, LStrA07 = string.Empty;
            string LStrEncryptionPwd = string.Empty;

            try
            {
                App.GStrCatchException = string.Empty;
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = System.IO.Path.Combine(LStrXmlFileName, @"UMP.Server\Args02.UMP.xml");

                LStrVerificationCode001 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
                LStrVerificationCode101 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);

                XmlDocument LXmlDocArgs02 = new XmlDocument();
                LXmlDocArgs02.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListSAUsers = LXmlDocArgs02.SelectSingleNode("Parameters02").SelectSingleNode("SAUsers").ChildNodes;
                foreach (XmlNode LXmlNodeSingleUser in LXmlNodeListSAUsers)
                {
                    LStrA02 = LXmlNodeSingleUser.Attributes["A02"].Value;
                    LStrA02 = EncryptionAndDecryption.EncryptDecryptString(LStrA02, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);

                    if (LStrA02 == IListConnectArguments[2])
                    {
                        LStrA01 = LXmlNodeSingleUser.Attributes["A01"].Value;
                        LStrA03 = LXmlNodeSingleUser.Attributes["A03"].Value;
                        LStrA04 = LXmlNodeSingleUser.Attributes["A04"].Value;
                        LStrA05 = LXmlNodeSingleUser.Attributes["A05"].Value;
                        LStrA06 = LXmlNodeSingleUser.Attributes["A06"].Value;
                        LStrA07 = LXmlNodeSingleUser.Attributes["A07"].Value;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(LStrA01))
                {
                    LBoolReturn = false;
                    AStrReturn = "E00005";
                    return LBoolReturn;
                }
                LStrEncryptionPwd = EncryptionAndDecryption.EncryptStringSHA512(LStrA01 + IListConnectArguments[3], LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                if (LStrEncryptionPwd != LStrA03)
                {
                    LBoolReturn = false;
                    AStrReturn = "E00005";
                    return LBoolReturn;
                }
                App.GStrLoginPassword = IListConnectArguments[3];
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "E00000";
                App.GStrCatchException = ex.ToString();
            }

            return LBoolReturn;
        }
        #endregion
        
    }
}
