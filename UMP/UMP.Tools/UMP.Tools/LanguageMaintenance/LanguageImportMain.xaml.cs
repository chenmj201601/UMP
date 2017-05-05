using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;
using System.Xml;
using UMP.Tools.PublicClasses;
using UMP.Tools.UMPWcfService00003;

namespace UMP.Tools.LanguageMaintenance
{
    public partial class LanguageImportMain : Window, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        private OperationDataArgs I00003OperationReturn = new OperationDataArgs();

        //导入数据 方法 R
        private BackgroundWorker IBackgroundWorkerA = null;
        //导入数据 方法 A & U
        private BackgroundWorker IBackgroundWorkerB = null;

        private bool IBoolInImporting = false;
        private string IStrLanguageID = string.Empty;
        //保存解密后语言包文件的xml数据
        private string IStrLanguagePackageBody = string.Empty;

        public LanguageImportMain()
        {
            InitializeComponent();
            this.Loaded += LanguageImportMain_Loaded;
            this.Closing += LanguageImportMain_Closing;
            MainPanel.KeyDown += MainPanel_KeyDown;
            this.MouseLeftButtonDown += LanguageImportMain_MouseLeftButtonDown;

            ButtonCloseThis.Click += ButtonCloseWindowClicked;
            ButtonCloseWindow.Click += ButtonCloseWindowClicked;

            ButtonILanguagePackagePath.Click += ButtonILanguagePackagePath_Click;
            ButtonIDecryptionFile.Click += ButtonIDecryptionFile_Click;
            ButtonImportData.Click += ButtonImportData_Click;
        }

        #region 导入语言包数据
        private bool IBoolHaveError = false;
        private XmlNode IXMLNodeTableDataRowsList = null;
        private int IIntAllRows = 0;
        private string IStrImportOptions = string.Empty;

        private void ButtonImportData_Click(object sender, RoutedEventArgs e)
        {
            string LStrLanguagePackageFile = string.Empty;
            string LStrDecryptionPassword = string.Empty;

            bool LBoolCallReturn = true;
            string LStrCallReturn = string.Empty;


            Stream LStreamXmlBody = null;
            try
            {
                #region 导入前的逻辑判断
                LStrLanguagePackageFile = TextBoxILanguagePackagePath.Text;
                if (!File.Exists(LStrLanguagePackageFile))
                {
                    MessageBox.Show(App.GetDisplayCharater("E004004"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (PasswordBoxIDecryptPassword.IsEnabled == false) { LStrDecryptionPassword = "vctyoung"; }
                else { LStrDecryptionPassword = PasswordBoxIDecryptPassword.Password.Trim(); }
                if (string.IsNullOrEmpty(LStrDecryptionPassword))
                {
                    MessageBox.Show(App.GetDisplayCharater("E004003"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                LBoolCallReturn = TryDecryptionLanguagePackage(LStrLanguagePackageFile, LStrDecryptionPassword, ref LStrCallReturn);
                if (!LBoolCallReturn)
                {
                    MessageBox.Show(App.GetDisplayCharater("E004002"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                MessageBoxResult LMessageBoxResult = MessageBox.Show(string.Format(App.GetDisplayCharater("M04011"), IStrLanguageID), App.GStrApplicationReferredTo, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
                if (LMessageBoxResult != MessageBoxResult.Yes) { return; }
                #endregion

                #region 导入数据的读取到XmlNode
                XmlDocument LXmlDocTableDataLoaded = new XmlDocument();
                LStreamXmlBody = new MemoryStream(Encoding.UTF8.GetBytes(IStrLanguagePackageBody));
                LXmlDocTableDataLoaded.Load(LStreamXmlBody);
                IXMLNodeTableDataRowsList = LXmlDocTableDataLoaded.SelectSingleNode("LanguageDataRowsList");
                IIntAllRows = IXMLNodeTableDataRowsList.ChildNodes.Count;
                ComboBoxItem LComboBoxItem = ComboBoxIImportOption.SelectedItem as ComboBoxItem;
                IStrImportOptions = LComboBoxItem.DataContext.ToString();
                #endregion

                #region 开始导入数据
                IBoolHaveError = false;
                if (IStrImportOptions == "R") { ImportDataMethodR(); }
                else { ImportDataMethodAU(); }
                #endregion

            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
            
        }

        private void ImportDataMethodR()
        {
            try
            {
                IBoolInImporting = true;
                if (IBackgroundWorkerA == null) { IBackgroundWorkerA = new BackgroundWorker(); }
                IBackgroundWorkerA.RunWorkerCompleted += IBackgroundWorkerA_RunWorkerCompleted;
                IBackgroundWorkerA.DoWork += IBackgroundWorkerA_DoWork;
                IBackgroundWorkerA.WorkerReportsProgress = true;
                IBackgroundWorkerA.ProgressChanged += IBackgroundWorkerA_ProgressChanged;
                IBackgroundWorkerA.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                IBoolInImporting = false;
                if(IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
                App.ShowExceptionMessage(ex.Message);
            }

        }

        private void IBackgroundWorkerA_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string LStrShowTip = string.Empty;

            try
            {
                int LIntCurrentStep = e.ProgressPercentage;

                if (LIntCurrentStep == int.MaxValue)
                {
                    LStrShowTip = string.Format(App.GetDisplayCharater("M04012"), IStrLanguageID);
                }
                else
                {
                    LStrShowTip = (((decimal)LIntCurrentStep / (decimal)IIntAllRows) * 100).ToString("F2") + "% " + string.Format(App.GetDisplayCharater("M04013"), IStrLanguageID);
                }
                App.ShowCurrentStatus(1, LStrShowTip);
            }
            catch { }
        }

        private void IBackgroundWorkerA_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker LBackgroundWorker = sender as BackgroundWorker;

            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();
            int LIntCurrentRow = 0;

            string LStrColumnName = string.Empty;
            string LStrColumnValue = string.Empty;

            try
            {
                List<string> LListStrDBProfile = App.GSystemMainWindow.GetCurrentDatabaseProfile();
                List<string> LListStrAppServer = App.GSystemMainWindow.GetCurrentAppServerConnection();

                LBackgroundWorker.ReportProgress(int.MaxValue);

                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 15);
                LEndpointAddress = App.CreateEndpointAddress(LListStrAppServer[0], LListStrAppServer[1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);
                
                foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }
                LListWcfArgs.Add(IStrLanguageID);
                LListWcfArgs.Add("D");

                I00003OperationReturn = LService00003Client.OperationMethodA(7, LListWcfArgs);
                if (!I00003OperationReturn.BoolReturn) { IBoolHaveError = true; return; }

                IIntAllRows = IXMLNodeTableDataRowsList.ChildNodes.Count;
                foreach (XmlNode LXmlNodeSingleRow in IXMLNodeTableDataRowsList)
                {
                    LIntCurrentRow += 1;
                    LBackgroundWorker.ReportProgress(LIntCurrentRow);

                    LListWcfArgs.Clear();
                    foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }
                    LListWcfArgs.Add(IStrLanguageID);
                    LListWcfArgs.Add("I");
                    foreach (XmlNode LXmlNodeSingleColumn in LXmlNodeSingleRow.ChildNodes)
                    {
                        LStrColumnName = LXmlNodeSingleColumn.Name;
                        LStrColumnValue = LXmlNodeSingleColumn.InnerText;
                        if (string.IsNullOrEmpty(LStrColumnValue)) { LStrColumnValue = ""; }
                        LListWcfArgs.Add(LStrColumnName + App.GStrSpliterChar + LStrColumnValue);
                    }
                    I00003OperationReturn = LService00003Client.OperationMethodA(7, LListWcfArgs);
                    if (!I00003OperationReturn.BoolReturn) { IBoolHaveError = true; }
                }
            }
            catch (Exception ex)
            {
                IBoolHaveError = true;
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP000E006" + App.GStrSpliterChar + ex.Message;
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
            IBoolInImporting = false;
            App.ShowCurrentStatus(int.MaxValue, string.Empty);
            if (IBackgroundWorkerA != null)
            {
                IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
            }
            if (IBoolHaveError)
            {
                MessageBox.Show(App.GetDisplayCharater("E004005"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(string.Format(App.GetDisplayCharater("M04014"), IStrLanguageID), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ImportDataMethodAU()
        {
            try
            {
                IBoolInImporting = true;
                if (IBackgroundWorkerB == null) { IBackgroundWorkerB = new BackgroundWorker(); }
                IBackgroundWorkerB.RunWorkerCompleted += IBackgroundWorkerB_RunWorkerCompleted;
                IBackgroundWorkerB.DoWork += IBackgroundWorkerB_DoWork;
                IBackgroundWorkerB.WorkerReportsProgress = true;
                IBackgroundWorkerB.ProgressChanged += IBackgroundWorkerB_ProgressChanged;
                IBackgroundWorkerB.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                IBoolInImporting = false;
                if (IBackgroundWorkerB != null)
                {
                    IBackgroundWorkerB.Dispose(); IBackgroundWorkerB = null;
                }
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void IBackgroundWorkerB_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string LStrShowTip = string.Empty;

            try
            {
                int LIntCurrentStep = e.ProgressPercentage;
                LStrShowTip = (((decimal)LIntCurrentStep / (decimal)IIntAllRows) * 100).ToString("F2") + "% " + string.Format(App.GetDisplayCharater("M04013"), IStrLanguageID);
                App.ShowCurrentStatus(1, LStrShowTip);
            }
            catch { }
        }

        private void IBackgroundWorkerB_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker LBackgroundWorker = sender as BackgroundWorker;

            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();
            int LIntCurrentRow = 0;

            string LStrColumnName = string.Empty;
            string LStrColumnValue = string.Empty;


            try
            {
                List<string> LListStrDBProfile = App.GSystemMainWindow.GetCurrentDatabaseProfile();
                List<string> LListStrAppServer = App.GSystemMainWindow.GetCurrentAppServerConnection();

                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 15);
                LEndpointAddress = App.CreateEndpointAddress(LListStrAppServer[0], LListStrAppServer[1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);

                IIntAllRows = IXMLNodeTableDataRowsList.ChildNodes.Count;
                foreach (XmlNode LXmlNodeSingleRow in IXMLNodeTableDataRowsList)
                {
                    LIntCurrentRow += 1;
                    LBackgroundWorker.ReportProgress(LIntCurrentRow);

                    LListWcfArgs.Clear();
                    foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }
                    LListWcfArgs.Add(IStrLanguageID);
                    LListWcfArgs.Add(IStrImportOptions);
                    foreach (XmlNode LXmlNodeSingleColumn in LXmlNodeSingleRow.ChildNodes)
                    {
                        LStrColumnName = LXmlNodeSingleColumn.Name;
                        LStrColumnValue = LXmlNodeSingleColumn.InnerText;
                        if (string.IsNullOrEmpty(LStrColumnValue)) { LStrColumnValue = ""; }
                        LListWcfArgs.Add(LStrColumnName + App.GStrSpliterChar + LStrColumnValue);
                    }
                    I00003OperationReturn = LService00003Client.OperationMethodA(7, LListWcfArgs);
                    if (!I00003OperationReturn.BoolReturn) { IBoolHaveError = true; }
                }
            }
            catch (Exception ex)
            {
                IBoolHaveError = true;
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP000E006" + App.GStrSpliterChar + ex.Message;
            }
            finally
            {
                if (LService00003Client != null)
                {
                    if (LService00003Client.State == CommunicationState.Opened) { LService00003Client.Close(); LService00003Client = null; }
                }
            }
        }

        private void IBackgroundWorkerB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IBoolInImporting = false;
            App.ShowCurrentStatus(int.MaxValue, string.Empty);
            if (IBackgroundWorkerB != null)
            {
                IBackgroundWorkerB.Dispose(); IBackgroundWorkerB = null;
            }
            if (IBoolHaveError)
            {
                MessageBox.Show(App.GetDisplayCharater("E004005"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(string.Format(App.GetDisplayCharater("M04014"), IStrLanguageID), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        #endregion

        private void ButtonIDecryptionFile_Click(object sender, RoutedEventArgs e)
        {
            string LStrLanguagePackageFile = string.Empty;
            string LStrDecryptionPassword = string.Empty;
            bool LBoolCallReturn = true;
            string LStrCallReturn = string.Empty;

            try
            {
                if (IBoolInImporting) { return; }
                LStrLanguagePackageFile = TextBoxILanguagePackagePath.Text;
                LStrDecryptionPassword = PasswordBoxIDecryptPassword.Password.Trim();
                if (string.IsNullOrEmpty(LStrDecryptionPassword))
                {
                    MessageBox.Show(App.GetDisplayCharater("E004003"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                ButtonImportData.IsEnabled = false;
                LBoolCallReturn = TryDecryptionLanguagePackage(LStrLanguagePackageFile, LStrDecryptionPassword, ref LStrCallReturn);
                if (!LBoolCallReturn)
                {
                    MessageBox.Show(App.GetDisplayCharater("E004002"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                ButtonImportData.IsEnabled = true;
                MessageBox.Show(App.GetDisplayCharater("M04009"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                ButtonImportData.IsEnabled = false;
            }
        }

        private void ButtonILanguagePackagePath_Click(object sender, RoutedEventArgs e)
        {
            bool LBoolCallReturn = true;
            string LStrCallReturn = string.Empty;

            try
            {
                if (IBoolInImporting) { return; }
                System.Windows.Forms.OpenFileDialog LOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
                LOpenFileDialog.Filter = App.GetDisplayCharater("M04004") + "|*.YLP";
                LOpenFileDialog.Multiselect = false;
                LOpenFileDialog.CheckFileExists = true;
                if (LOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
                ButtonImportData.IsEnabled = false;
                TextBoxILanguagePackagePath.Text = LOpenFileDialog.FileName;
                IStrLanguageID = string.Empty;
                LBoolCallReturn = TryDecryptionLanguagePackage(TextBoxILanguagePackagePath.Text, "vctyoung", ref LStrCallReturn);
                if (!LBoolCallReturn)
                {
                    if (LStrCallReturn == "E004001")
                    {
                        MessageBox.Show(App.GetDisplayCharater(LStrCallReturn), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    PasswordBoxIDecryptPassword.IsEnabled = true;
                    ButtonIDecryptionFile.IsEnabled = true;
                    PasswordBoxIDecryptPassword.Focus();
                }
                else
                {
                    if (LStrCallReturn != "E004002")
                    {
                        PasswordBoxIDecryptPassword.Password = string.Empty;
                        PasswordBoxIDecryptPassword.IsEnabled = false;
                        ButtonIDecryptionFile.IsEnabled = false;
                        ButtonImportData.IsEnabled = true;
                        
                    }
                    else
                    {
                        PasswordBoxIDecryptPassword.IsEnabled = true;
                        ButtonIDecryptionFile.IsEnabled = true;
                        PasswordBoxIDecryptPassword.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                ButtonImportData.IsEnabled = false;
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void ButtonCloseWindowClicked(object sender, RoutedEventArgs e)
        {
            if (!IBoolInImporting) { this.Close(); }
        }

        private void LanguageImportMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void LanguageImportMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IBoolInImporting) { e.Cancel = true; return; }
        }

        private void LanguageImportMain_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.DrawWindowsBackGround(this);
                ImageImportLanguage.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000039.ico"), UriKind.RelativeOrAbsolute));
                DisplayElementCharacters(false);
                InitDataImportOptions();
            }
            catch { }
        }

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelImportLanguageTip.Content = App.GetDisplayCharater("M04001");

            TabItemLanguageImport.Header = " " + App.GetDisplayCharater("M04003") + " ";

            LabelILanguagePackagePath.Content = App.GetDisplayCharater("M04004");
            LabelIDecryptionPassword.Content = App.GetDisplayCharater("M04005");
            ButtonIDecryptionFile.ToolTip = App.GetDisplayCharater("M04010");
            LabelIImportOption.Content = App.GetDisplayCharater("M04006");

            ButtonImportData.Content = App.GetDisplayCharater("M04007");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M04008");
        }

        #region 尝试解密语言包
        private bool TryDecryptionLanguagePackage(string AStrSourceFile, string AStrPassword, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrSourceBody = string.Empty;

            try
            {
                IStrLanguagePackageBody = string.Empty;
                AStrReturn = string.Empty;

                //判断是否为XML格式的文件
                LBoolReturn = IsXMLTypeFile(AStrSourceFile);
                if (!LBoolReturn)
                {
                    VoiceCyberPrivateEncryptionDecryption.DecryptFileToString(AStrSourceFile, AStrPassword, ref AStrReturn);
                    LBoolReturn = StringIsLanguagePackage(AStrReturn);
                    if (!LBoolReturn) { AStrReturn = "E004002"; }
                    IStrLanguagePackageBody = AStrReturn;
                }
                else
                {
                    //判断是否为语言包文件
                    LBoolReturn = FileIsLanguagePackage(AStrSourceFile);
                    if (!LBoolReturn) { AStrReturn = "E004001"; }
                }
            }
            catch
            {
                LBoolReturn = false;
            }
            return LBoolReturn;
        }

        private bool IsXMLTypeFile(string AStrSourceFile)
        {
            bool LBoolReturn = true;

            try
            {
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(AStrSourceFile);
            }
            catch { LBoolReturn = false; }
            return LBoolReturn;
        }

        private bool FileIsLanguagePackage(string AStrSourceFile)
        {
            bool LBoolReturn = true;
            string LStrTableName = string.Empty;
            string LStrLanguageCode = string.Empty;

            try
            {
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(AStrSourceFile);
                XmlNode LXMLNodeTableDataRowsList = LXmlDocServer01.SelectSingleNode("LanguageDataRowsList");
                LStrTableName = LXMLNodeTableDataRowsList.Attributes["TableName"].Value;
                LStrLanguageCode = LXMLNodeTableDataRowsList.Attributes["LanguageCode"].Value;
                IStrLanguageID = LStrLanguageCode;
            }
            catch
            {
                LBoolReturn = false;
            }

            return LBoolReturn;
        }

        private bool StringIsLanguagePackage(string AStrSoureBody)
        {
            bool LBoolReturn = true;
            string LStrTableName = string.Empty;
            string LStrLanguageCode = string.Empty;

            try
            {
                XmlDocument LXmlDocTableDataLoaded = new XmlDocument();
                Stream LStreamXmlBody = new MemoryStream(Encoding.UTF8.GetBytes(AStrSoureBody));
                LXmlDocTableDataLoaded.Load(LStreamXmlBody);
                XmlNode LXMLNodeTableDataRowsList = LXmlDocTableDataLoaded.SelectSingleNode("LanguageDataRowsList");
                LStrTableName = LXMLNodeTableDataRowsList.Attributes["TableName"].Value;
                LStrLanguageCode = LXMLNodeTableDataRowsList.Attributes["LanguageCode"].Value;
                IStrLanguageID = LStrLanguageCode;
            }
            catch
            {
                LBoolReturn = false;
            }
            return LBoolReturn;
        }
        #endregion

        #region 初始化数据导入方式
        private void InitDataImportOptions()
        {
            try
            {
                ComboBoxIImportOption.Items.Clear();

                ComboBoxItem LComboBoxItemRefresh = new ComboBoxItem();
                LComboBoxItemRefresh.DataContext = "R";
                LComboBoxItemRefresh.Background = Brushes.Transparent;
                LComboBoxItemRefresh.Style = (Style)App.Current.Resources["ComboBoxItemNormalStyle"];
                LComboBoxItemRefresh.Height = 26;
                LComboBoxItemRefresh.Content = " " + App.GetConvertedData("ImporDataOptionR");
                ComboBoxIImportOption.Items.Add(LComboBoxItemRefresh);

                ComboBoxItem LComboBoxItemAppend = new ComboBoxItem();
                LComboBoxItemAppend.DataContext = "A";
                LComboBoxItemAppend.Background = Brushes.Transparent;
                LComboBoxItemAppend.Style = (Style)App.Current.Resources["ComboBoxItemNormalStyle"];
                LComboBoxItemAppend.Height = 26;
                LComboBoxItemAppend.Content = " " + App.GetConvertedData("ImporDataOptionA");
                ComboBoxIImportOption.Items.Add(LComboBoxItemAppend);

                ComboBoxItem LComboBoxItemUpdate = new ComboBoxItem();
                LComboBoxItemUpdate.DataContext = "U";
                LComboBoxItemUpdate.Background = Brushes.Transparent;
                LComboBoxItemUpdate.Style = (Style)App.Current.Resources["ComboBoxItemNormalStyle"];
                LComboBoxItemUpdate.Height = 26;
                LComboBoxItemUpdate.Content = " " + App.GetConvertedData("ImporDataOptionU");
                ComboBoxIImportOption.Items.Add(LComboBoxItemUpdate);

                ComboBoxIImportOption.SelectedIndex = 2;
            }
            catch { }
        }
        #endregion
    }
}
