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
using System.Windows.Shapes;
using System.Xml;
using UMP.Tools.PublicClasses;
using UMP.Tools.UMPWcfService00003;

namespace UMP.Tools.LanguageMaintenance
{
    public partial class LanguageExportMain : Window, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        private bool IBoolInExporting = false;
        private string IStrLanguageID = string.Empty;

        private OperationDataArgs I00003OperationReturn = new OperationDataArgs();

        //导出语言包
        private BackgroundWorker IBackgroundWorkerA = null;

        public LanguageExportMain()
        {
            InitializeComponent();
            this.Loaded += LanguageExportMain_Loaded;
            this.Closing += LanguageExportMain_Closing;
            MainPanel.KeyDown += MainPanel_KeyDown;
            this.MouseLeftButtonDown += LanguageExportMain_MouseLeftButtonDown;

            ButtonELanguagePackagePath.Click += ButtonELanguagePackagePath_Click;
            ButtonExportData.Click += ButtonExportData_Click;
            ButtonCloseThis.Click += ButtonCloseWindowClicked;
            ButtonCloseWindow.Click += ButtonCloseWindowClicked;
        }

        public void InitArguments(string AStrLanguageID, string AStrLanguageName)
        {
            IStrLanguageID = AStrLanguageID;
            TabItemLanguageExport.Header = " " + AStrLanguageName + " ";
        }

        #region 导出语言包
        private List<string> IListStrExportOptions = new List<string>();            //0:文件存放路径；1:语言ID；2:加密密码
        private void ButtonExportData_Click(object sender, RoutedEventArgs e)
        {
            string LStrExportPath = string.Empty;
            string LStrEncryptionPwd01 = string.Empty;
            string LStrEncryptionPwd02 = string.Empty;

            try
            {
                IListStrExportOptions.Clear();
                IListStrExportOptions.Add(""); IListStrExportOptions.Add(IStrLanguageID); IListStrExportOptions.Add("");

                LStrExportPath = TextBoxELanguagePackagePath.Text;
                LStrEncryptionPwd01 = PasswordBoxEEcryptPassword.Password.Trim();
                LStrEncryptionPwd02 = PasswordBoxEConfirmEcryptPassword.Password.Trim();

                if (!string.IsNullOrEmpty(LStrEncryptionPwd01) || !string.IsNullOrEmpty(LStrEncryptionPwd02))
                {
                    if (LStrEncryptionPwd01 != LStrEncryptionPwd02)
                    {
                        MessageBox.Show(App.GetDisplayCharater("E003001"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    LStrEncryptionPwd01 = "vctyoung";
                }
                IListStrExportOptions[0] = LStrExportPath;
                IListStrExportOptions[2] = LStrEncryptionPwd01;

                MainPanel.IsEnabled = false;
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M03008"));
                if (IBackgroundWorkerA == null) { IBackgroundWorkerA = new BackgroundWorker(); }
                IBackgroundWorkerA.RunWorkerCompleted += IBackgroundWorkerA_RunWorkerCompleted;
                IBackgroundWorkerA.DoWork += IBackgroundWorkerA_DoWork;
                IBackgroundWorkerA.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
                MainPanel.IsEnabled = true;
                if (IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void IBackgroundWorkerA_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();

            bool LBoolCallReturn = true;
            string LStrCallReturn = string.Empty;

            try
            {
                List<string> LListStrDBProfile = App.GSystemMainWindow.GetCurrentDatabaseProfile();
                List<string> LListStrAppServer = App.GSystemMainWindow.GetCurrentAppServerConnection();

                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 15);
                LEndpointAddress = App.CreateEndpointAddress(LListStrAppServer[0], LListStrAppServer[1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);

                LListWcfArgs.Add(IStrLanguageID);
                foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }
                LListWcfArgs.Add("R");
                I00003OperationReturn = LService00003Client.OperationMethodA(5, LListWcfArgs);
                if (!I00003OperationReturn.BoolReturn) { return; }
                I00003OperationReturn.BoolReturn = WriteDataToXMLFile(I00003OperationReturn.DataSetReturn.Tables[0], ref LStrCallReturn);
                I00003OperationReturn.StringReturn = LStrCallReturn;

            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP000E003" + App.GStrSpliterChar + ex.Message;
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

            App.ShowCurrentStatus(int.MaxValue, string.Empty);
            MainPanel.IsEnabled = true;
            if (IBackgroundWorkerA != null)
            {
                IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
            }
            if (!I00003OperationReturn.BoolReturn)
            {
                string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                LStrMessageBody = App.GetDisplayCharater(LStrOperationReturn[0]);
                if (LStrOperationReturn[0] != "UMP000E005") { LStrMessageBody += "\n" + LStrOperationReturn[1]; }
                MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MessageBox.Show(App.GetDisplayCharater("M03009"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private bool WriteDataToXMLFile(DataTable ADataTableSelected, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            int LIntLoopColumns = 0, LIntAllColumns = 0;
            int LIntLoopRows = 0, LIntAllRows = 0;
            string LStrSourceFile = string.Empty;
            string LStrColumnName = string.Empty;
            string LStrCallReturn = string.Empty;

            try
            {
                AStrReturn = string.Empty;

                LStrSourceFile = System.IO.Path.Combine(App.GStrUserMyDocumentsDirectory, IListStrExportOptions[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml");
                XmlTextWriter LXmlWriter = new XmlTextWriter(LStrSourceFile, Encoding.UTF8);
                LXmlWriter.Formatting = Formatting.Indented;
                LXmlWriter.WriteStartDocument(true);

                LXmlWriter.WriteStartElement("LanguageDataRowsList");
                LXmlWriter.WriteAttributeString("TableName", "T_00_005");
                LXmlWriter.WriteAttributeString("LanguageCode", IListStrExportOptions[1]);
                LXmlWriter.WriteAttributeString("Version", "8.02.001");
                LXmlWriter.WriteAttributeString("Options", "Export");
                LXmlWriter.WriteAttributeString("Author", "Young Yang");

                LIntAllRows = ADataTableSelected.Rows.Count;
                LIntAllColumns = ADataTableSelected.Columns.Count;
                for (LIntLoopRows = 0; LIntLoopRows < LIntAllRows; LIntLoopRows++)
                {
                    LXmlWriter.WriteStartElement("T_00_005");
                    for (LIntLoopColumns = 0; LIntLoopColumns < LIntAllColumns; LIntLoopColumns++)
                    {
                        LStrColumnName = ADataTableSelected.Columns[LIntLoopColumns].ColumnName;
                        if (ADataTableSelected.Columns[LIntLoopColumns].DataType == typeof(DateTime))
                        {
                            string LStrDateTimeData = ADataTableSelected.Rows[LIntLoopRows][LIntLoopColumns].ToString();
                            if (!string.IsNullOrEmpty(LStrDateTimeData))
                            {
                                LXmlWriter.WriteElementString(LStrColumnName, ((DateTime)ADataTableSelected.Rows[LIntLoopRows][LIntLoopColumns]).ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else
                            {
                                LXmlWriter.WriteElementString(LStrColumnName, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }
                        else
                        {
                            LXmlWriter.WriteElementString(LStrColumnName, ADataTableSelected.Rows[LIntLoopRows][LIntLoopColumns].ToString());
                        }
                    }
                    LXmlWriter.WriteEndElement();
                }
                LXmlWriter.WriteEndElement();
                LXmlWriter.WriteEndDocument();
                LXmlWriter.Flush();
                LXmlWriter.Close();

                if (string.IsNullOrEmpty(IListStrExportOptions[2]))
                {
                    if (System.IO.File.Exists(IListStrExportOptions[0])) { System.IO.File.Delete(IListStrExportOptions[0]); }
                    System.IO.File.Move(LStrSourceFile, IListStrExportOptions[0]);
                }
                else
                {
                    LBoolReturn = VoiceCyberPrivateEncryptionDecryption.EncryptFileToFile(LStrSourceFile, IListStrExportOptions[0], IListStrExportOptions[2], true, true, ref LStrCallReturn);
                    if (!LBoolReturn)
                    {
                        if (LStrCallReturn == "000")
                        {
                            AStrReturn = "UMP000E005" + App.GStrSpliterChar;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "UMP000E004" + App.GStrSpliterChar + ex.Message;
            }
            return LBoolReturn;
        }
        #endregion

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelExportLanguageTip.Content = App.GetDisplayCharater("M03001");
            LabelELanguagePackagePath.Content = App.GetDisplayCharater("M03002");
            LabelEEncryptionOption.Content = App.GetDisplayCharater("M03003");
            LabelEEcryptPassword.Content = App.GetDisplayCharater("M03004");
            LabelEConfirmEcryptPassword.Content = App.GetDisplayCharater("M03005");

            ButtonExportData.Content = App.GetDisplayCharater("M03006");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M03007");
        }

        private void ButtonELanguagePackagePath_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog LFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
                LFolderBrowserDialog.Description = App.GetDisplayCharater("M03002");
                System.Windows.Forms.DialogResult LDialogResult = LFolderBrowserDialog.ShowDialog();
                if (LDialogResult != System.Windows.Forms.DialogResult.OK) { return; }
                TextBoxELanguagePackagePath.Text = System.IO.Path.Combine(LFolderBrowserDialog.SelectedPath, "UMP" + IStrLanguageID + ".YLP");
                TextBoxELanguagePackagePath.DataContext = System.IO.Path.Combine(LFolderBrowserDialog.SelectedPath, "UMP" + IStrLanguageID + ".YLP");
                ButtonExportData.IsEnabled = true;
            }
            catch (Exception ex)
            {
                ButtonExportData.IsEnabled = false;
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void ButtonCloseWindowClicked(object sender, RoutedEventArgs e)
        {
            if (!IBoolInExporting) { this.Close(); }
        }

        private void LanguageExportMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void LanguageExportMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IBoolInExporting) { e.Cancel = true; return; }
        }

        private void LanguageExportMain_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.DrawWindowsBackGround(this);
                ImageExportLanguage.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000028.ico"), UriKind.RelativeOrAbsolute));
                DisplayElementCharacters(false);
            }
            catch { }
        }

    }
}
