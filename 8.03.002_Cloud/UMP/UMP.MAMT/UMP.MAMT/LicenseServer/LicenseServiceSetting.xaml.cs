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

namespace UMP.MAMT.LicenseServer
{
    public partial class LicenseServiceSetting : Window, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        //是否在处理的过程中
        private bool IBoolInDoing = false;

        private DataTable IDataTableLicenseService = null;

        public LicenseServiceSetting(DataTable ADataTableLicenseService)
        {
            InitializeComponent();
            IDataTableLicenseService = ADataTableLicenseService;

            this.Loaded += LicenseServiceSetting_Loaded;
            this.Closing += LicenseServiceSetting_Closing;
            this.MouseLeftButtonDown += LicenseServiceSetting_MouseLeftButtonDown;

            MainPanel.KeyDown += MainPanel_KeyDown;

            ButtonApplicationMenu.Click += WindowsButtonClicked;
            ButtonApplySetting.Click += WindowsButtonClicked;
            ButtonCloseWindow.Click += WindowsButtonClicked;
            ButtonCloseSetting.Click += WindowsButtonClicked;
        }

        private void LicenseServiceSetting_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this);
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
            ImageLicenseService.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000037.ico"), UriKind.RelativeOrAbsolute));

            DisplayElementCharacters(false);

            TextBoxMServer.Focus();
            ShowSettedLicenseService();
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
                    case "ButtonCloseSetting":
                        CloseThisWindow();
                        break;
                    case "ButtonApplySetting":
                        TryConnect2LicenseService();
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

        private void ShowSettedLicenseService()
        {
            string LStrMainSpare = string.Empty;
            string LStrIsEnabled = string.Empty;
            string LStrServerHost = string.Empty;
            string LStrServerPort = string.Empty, LStrOtherInfo = string.Empty;

            TextBoxMPort.SetMinMaxDefaultValue(1024, 65535, 3070);
            TextBoxSPort.SetMinMaxDefaultValue(1024, 65535, 3070);

            foreach (DataRow LDataRowSingleLicenseService in IDataTableLicenseService.Rows)
            {
                LStrMainSpare = LDataRowSingleLicenseService["MainSpare"].ToString();
                LStrIsEnabled = LDataRowSingleLicenseService["IsEnabled"].ToString();
                LStrServerHost = LDataRowSingleLicenseService["ServerHost"].ToString();
                LStrServerPort = LDataRowSingleLicenseService["ServerPort"].ToString();
                LStrOtherInfo = LDataRowSingleLicenseService["OtherInfo"].ToString();

                if (LStrMainSpare == "1")
                {
                    if (LStrIsEnabled == "1")
                    {
                        RadioButtonMIsEnabled1.IsChecked = true;
                    }
                    else
                    {
                        RadioButtonMIsEnabled0.IsChecked = true;
                    }
                    TextBoxMServer.SetIP(LStrServerHost);
                    TextBoxMPort.SetElementData(LStrServerPort);
                }
                else
                {
                    if (LStrIsEnabled == "1")
                    {
                        RadioButtonSIsEnabled1.IsChecked = true;
                    }
                    else
                    {
                        RadioButtonSIsEnabled0.IsChecked = true;
                    }
                    TextBoxSServer.SetIP(LStrServerHost);
                    TextBoxSPort.SetElementData(LStrServerPort);
                }
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

        private void CloseThisWindow()
        {
            if (!IBoolInDoing) { this.Close(); }
        }

        private void LicenseServiceSetting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void LicenseServiceSetting_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IBoolInDoing) { e.Cancel = true; return; }
            App.GSystemMainWindow.IOperationEvent -= GSystemMainWindow_IOperationEvent;
        }

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelLicenseService.Content = App.GetDisplayCharater("M01100");

            TabItemMainService.Header = " " + App.GetDisplayCharater("M01105") + " ";
            TabItemSpareService.Header = " " + App.GetDisplayCharater("M01106") + " ";

            LabelMGeneral.Content = App.GetDisplayCharater("M01107");
            LabelMServer.Content = App.GetDisplayCharater("M01102");
            LabelMPort.Content = App.GetDisplayCharater("M01103");
            LabelMIsEnabled.Content = App.GetDisplayCharater("M01108");
            RadioButtonMIsEnabled1.Content = App.GetConvertedData("LSIsEnable1");
            RadioButtonMIsEnabled0.Content = App.GetConvertedData("LSIsEnable0");

            LabelSGeneral.Content = App.GetDisplayCharater("M01107");
            LabelSServer.Content = App.GetDisplayCharater("M01102");
            LabelSPort.Content = App.GetDisplayCharater("M01103");
            LabelSIsEnabled.Content = App.GetDisplayCharater("M01108");
            RadioButtonSIsEnabled1.Content = App.GetConvertedData("LSIsEnable1");
            RadioButtonSIsEnabled0.Content = App.GetConvertedData("LSIsEnable0");

            ButtonApplySetting.Content = App.GetDisplayCharater("M01109");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M01110");
        }

        #region 尝试连接 License Service ，并保存数据
        private BackgroundWorker IBackgroundWorkerApplyChanged = null;
        private List<bool> IListBoolApplyReturn = new List<bool>();
        private List<string> IListStrApplyReturn = new List<string>();
        private int IIntCurrentLoop = -1;

        private bool VerfySettedParameters()
        {
            bool LBoolReturn = true;
            string LStrMServerHost = string.Empty;
            string LStrMServerPort = string.Empty;
            string LStrMIsEnabled = string.Empty;

            string LStrSServerHost = string.Empty;
            string LStrSServerPort = string.Empty;
            string LStrSIsEnabled = string.Empty;
            string LStrP01 = string.Empty;

            try
            {
                LStrMServerHost = TextBoxMServer.GetIP();
                LStrMServerPort = TextBoxMPort.GetElementData();
                if (RadioButtonMIsEnabled1.IsChecked == true) { LStrMIsEnabled = "1"; } else { LStrMIsEnabled = "0"; }

                LStrSServerHost = TextBoxSServer.GetIP();
                LStrSServerPort = TextBoxSPort.GetElementData();
                if (RadioButtonSIsEnabled1.IsChecked == true) { LStrSIsEnabled = "1"; } else { LStrSIsEnabled = "0"; }

                if (LStrMIsEnabled == "0" && LStrSIsEnabled == "0")
                {
                    MessageBox.Show(App.GetDisplayCharater("M01111"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                foreach (DataRow LDataRowSingleLicenseService in IDataTableLicenseService.Rows)
                {
                    LStrP01 = LDataRowSingleLicenseService["MainSpare"].ToString();
                    if (LStrP01 == "1")
                    {
                        LDataRowSingleLicenseService["IsEnabled"] = LStrMIsEnabled;
                        LDataRowSingleLicenseService["ServerHost"] = LStrMServerHost;
                        LDataRowSingleLicenseService["ServerPort"] = LStrMServerPort;
                    }
                    else
                    {
                        LDataRowSingleLicenseService["IsEnabled"] = LStrSIsEnabled;
                        LDataRowSingleLicenseService["ServerHost"] = LStrSServerHost;
                        LDataRowSingleLicenseService["ServerPort"] = LStrSServerPort;
                    }
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                App.ShowExceptionMessage("VerfySettedParameters()\n" + ex.ToString());
            }
            return LBoolReturn;
        }

        private void TryConnect2LicenseService()
        {
            if (!VerfySettedParameters()) { return; }
            IIntCurrentLoop = -1;
            IListBoolApplyReturn.Clear();
            IListStrApplyReturn.Clear();
            BeginTryConnect2LicenseService();
        }

        private void BeginTryConnect2LicenseService()
        {
            List<string> LListStrArguments = new List<string>();
            string LStrServerHost = string.Empty;
            string LStrServerPort = string.Empty;
            string LStrIsEnabled = string.Empty;
            string LStrMessageBody = string.Empty;

            IIntCurrentLoop += 1;
            if (IIntCurrentLoop >= IDataTableLicenseService.Rows.Count)
            {
                int LIntCurrentReturn = -1;
                foreach (bool LBoolApplyReturn in IListBoolApplyReturn)
                {
                    LIntCurrentReturn += 1;
                    if (!LBoolApplyReturn)
                    {
                        LStrMessageBody += string.Format(App.GetDisplayCharater("M01113"), IListStrApplyReturn[LIntCurrentReturn]) + "\n";
                    }
                }
                if (!string.IsNullOrEmpty(LStrMessageBody))
                {
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else
                {
                    SaveSettings2XML();
                    MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                    LEventArgs.StrElementTag = "RLSST";
                    if (IOperationEvent != null) { IOperationEvent(this, LEventArgs); }
                    CloseThisWindow();
                }
                return;
            }

            LStrIsEnabled = IDataTableLicenseService.Rows[IIntCurrentLoop]["IsEnabled"].ToString();
            if (LStrIsEnabled != "1")
            {
                IIntCurrentLoop += 1;
                BeginTryConnect2LicenseService();
            }
            else
            {
                LStrServerHost = IDataTableLicenseService.Rows[IIntCurrentLoop]["ServerHost"].ToString();
                LStrServerPort = IDataTableLicenseService.Rows[IIntCurrentLoop]["ServerPort"].ToString();
                LListStrArguments.Add(LStrServerHost);
                LListStrArguments.Add(LStrServerPort);

                App.ShowCurrentStatus(1, string.Format(App.GetDisplayCharater("M01112"), LStrServerHost));
                IBoolInDoing = true;
                IBackgroundWorkerApplyChanged = new BackgroundWorker();
                IBackgroundWorkerApplyChanged.RunWorkerCompleted += IBackgroundWorkerApplyChanged_RunWorkerCompleted;
                IBackgroundWorkerApplyChanged.DoWork += IBackgroundWorkerApplyChanged_DoWork;
                IBackgroundWorkerApplyChanged.RunWorkerAsync(LListStrArguments);
            }

        }

        private void IBackgroundWorkerApplyChanged_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> LListStrArguments = e.Argument as List<string>;

            //string LStrCallReturn = App.TryConnect2LiceseService(LListStrArguments[0], LListStrArguments[1]);
            string LStrCallReturn = App.GetLicenseInfo(LListStrArguments[0], LListStrArguments[1]);
            if (LStrCallReturn != "1")
            {
                IListBoolApplyReturn.Add(false); IListStrApplyReturn.Add(LListStrArguments[0]);
            }
            else
            {
                IListBoolApplyReturn.Add(true); IListStrApplyReturn.Add(LListStrArguments[0]);
            }

            IDataTableLicenseService.Rows[IIntCurrentLoop]["OtherInfo"] = LStrCallReturn;

        }

        private void IBackgroundWorkerApplyChanged_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IBackgroundWorkerApplyChanged.Dispose();
            IBackgroundWorkerApplyChanged = null;
            App.ShowCurrentStatus(int.MaxValue, string.Empty);
            IBoolInDoing = false;
            BeginTryConnect2LicenseService();
        }

        private void SaveSettings2XML()
        {
            string LStrMainSpare = string.Empty;
            string LStrIsEnabled = string.Empty;
            string LStrServerHost = string.Empty;
            string LStrServerPort = string.Empty;
            string LStrOtherInfo = string.Empty;

            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode001 = string.Empty;
            string LStrP01 = string.Empty;

            LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            LStrXmlFileName = System.IO.Path.Combine(LStrXmlFileName, @"UMP.Server\Args02.UMP.xml");

            XmlDocument LXmlDocArgs02 = new XmlDocument();
            LXmlDocArgs02.Load(LStrXmlFileName);
            XmlNodeList LXmlNodeListLicenseService = LXmlDocArgs02.SelectSingleNode("Parameters02").SelectSingleNode("LicenseServer").ChildNodes;

            LStrVerificationCode001 = App.CreateVerificationCode(PFShareClassesS.EncryptionAndDecryption.UMPKeyAndIVType.M001);

            foreach (DataRow LDataRowSingleLicenseService in IDataTableLicenseService.Rows)
            {
                LStrMainSpare = LDataRowSingleLicenseService["MainSpare"].ToString();
                LStrIsEnabled = LDataRowSingleLicenseService["IsEnabled"].ToString();
                LStrServerHost = LDataRowSingleLicenseService["ServerHost"].ToString();
                LStrServerPort = LDataRowSingleLicenseService["ServerPort"].ToString();
                LStrOtherInfo = LDataRowSingleLicenseService["OtherInfo"].ToString();

                foreach (XmlNode LXmlNodeSingleLicenseService in LXmlNodeListLicenseService)
                {
                    if (LXmlNodeSingleLicenseService.NodeType == XmlNodeType.Comment) { continue; }

                    LStrP01 = LXmlNodeSingleLicenseService.Attributes["P01"].Value;
                    if (LStrP01 != LStrMainSpare) { continue; }
                    LXmlNodeSingleLicenseService.Attributes["P02"].Value = LStrIsEnabled;
                    LXmlNodeSingleLicenseService.Attributes["P03"].Value = EncryptionAndDecryption.EncryptDecryptString(LStrServerHost, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                    LXmlNodeSingleLicenseService.Attributes["P04"].Value = EncryptionAndDecryption.EncryptDecryptString(LStrServerPort, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001); ;
                    LXmlNodeSingleLicenseService.Attributes["P05"].Value = LStrOtherInfo;
                }
            }
            LXmlDocArgs02.Save(LStrXmlFileName);
        }

        #endregion

    }
}
