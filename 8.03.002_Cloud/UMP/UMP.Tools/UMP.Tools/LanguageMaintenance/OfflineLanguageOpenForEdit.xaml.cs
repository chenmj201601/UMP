using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using UMP.Tools.PublicClasses;
using UMP.Tools.UMPWcfService00003;

namespace UMP.Tools.LanguageMaintenance
{
    public partial class OfflineLanguageOpenForEdit : Window, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        private bool IBoolInOpening = false;
        private string IStrLanguageID = string.Empty;
        private string IStrOfflineFileType = string.Empty;
        //保存解密后语言包文件的xml数据
        private string IStrLanguagePackageBody = string.Empty;

        public OfflineLanguageOpenForEdit()
        {
            InitializeComponent();
            this.Loaded += OfflineLanguageOpenForEdit_Loaded;
            this.Closing += OfflineLanguageOpenForEdit_Closing;
            MainPanel.KeyDown += MainPanel_KeyDown;
            this.MouseLeftButtonDown += OfflineLanguageOpenForEdit_MouseLeftButtonDown;

            ButtonCloseThis.Click += ButtonCloseWindowClicked;
            ButtonCloseWindow.Click += ButtonCloseWindowClicked;

            ButtonOLanguagePackagePath.Click += ButtonOLanguagePackagePath_Click;
            ButtonODecryptionFile.Click += ButtonODecryptionFile_Click;
            ButtonOpenFile.Click += ButtonOpenFile_Click;
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            string LStrLanguagePackageFile = string.Empty;
            string LStrDecryptionPassword = string.Empty;

            bool LBoolCallReturn = true;
            string LStrCallReturn = string.Empty;

            try
            {
                #region 导入前的逻辑判断
                LStrLanguagePackageFile = TextBoxOLanguagePackagePath.Text;
                if (!File.Exists(LStrLanguagePackageFile))
                {
                    MessageBox.Show(App.GetDisplayCharater("E004004"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (PasswordBoxODecryptPassword.IsEnabled == false) { LStrDecryptionPassword = "vctyoung"; }
                else { LStrDecryptionPassword = PasswordBoxODecryptPassword.Password.Trim(); }
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
                #endregion

                OperationEventArgs LOperationEventArgs = new OperationEventArgs();
                LOperationEventArgs.StrElementTag = "SOLANG";
                LOperationEventArgs.ObjSource = IStrLanguagePackageBody;
                LOperationEventArgs.AppenObjeSource1 = LStrLanguagePackageFile;
                LOperationEventArgs.AppenObjeSource2 = LStrDecryptionPassword;
                LOperationEventArgs.AppenObjeSource3 = IStrOfflineFileType;
                LOperationEventArgs.AppenObjeSource4 = IStrLanguageID;
                IOperationEvent(this, LOperationEventArgs);
                this.Close();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
            

        }

        private void ButtonODecryptionFile_Click(object sender, RoutedEventArgs e)
        {
            string LStrLanguagePackageFile = string.Empty;
            string LStrDecryptionPassword = string.Empty;
            bool LBoolCallReturn = true;
            string LStrCallReturn = string.Empty;

            try
            {
                if (IBoolInOpening) { return; }
                LStrLanguagePackageFile = TextBoxOLanguagePackagePath.Text;
                LStrDecryptionPassword = PasswordBoxODecryptPassword.Password.Trim();
                if (string.IsNullOrEmpty(LStrDecryptionPassword))
                {
                    MessageBox.Show(App.GetDisplayCharater("E004003"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                ButtonOpenFile.IsEnabled = false;
                LBoolCallReturn = TryDecryptionLanguagePackage(LStrLanguagePackageFile, LStrDecryptionPassword, ref LStrCallReturn);
                if (!LBoolCallReturn)
                {
                    MessageBox.Show(App.GetDisplayCharater("E004002"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                ButtonOpenFile.IsEnabled = true;
                MessageBox.Show(App.GetDisplayCharater("M04009"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                ButtonOpenFile.IsEnabled = false;
            }
        }

        private void ButtonOLanguagePackagePath_Click(object sender, RoutedEventArgs e)
        {
            bool LBoolCallReturn = true;
            string LStrCallReturn = string.Empty;

            try
            {
                if (IBoolInOpening) { return; }
                System.Windows.Forms.OpenFileDialog LOpenFileDialog = new System.Windows.Forms.OpenFileDialog();
                LOpenFileDialog.Filter = App.GetDisplayCharater("M05003") + "|*.YLP";
                LOpenFileDialog.Multiselect = false;
                LOpenFileDialog.CheckFileExists = true;
                if (LOpenFileDialog.ShowDialog() != System.Windows.Forms.DialogResult.OK) { return; }
                ButtonOpenFile.IsEnabled = false;
                TextBoxOLanguagePackagePath.Text = LOpenFileDialog.FileName;
                IStrLanguageID = string.Empty;
                LBoolCallReturn = TryDecryptionLanguagePackage(TextBoxOLanguagePackagePath.Text, "vctyoung", ref LStrCallReturn);
                if (!LBoolCallReturn)
                {
                    if (LStrCallReturn == "E004001")
                    {
                        MessageBox.Show(App.GetDisplayCharater(LStrCallReturn), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    PasswordBoxODecryptPassword.IsEnabled = true;
                    ButtonODecryptionFile.IsEnabled = true;
                    PasswordBoxODecryptPassword.Focus();
                }
                else
                {
                    if (LStrCallReturn != "E004002")
                    {
                        PasswordBoxODecryptPassword.Password = string.Empty;
                        PasswordBoxODecryptPassword.IsEnabled = false;
                        ButtonODecryptionFile.IsEnabled = false;
                        ButtonOpenFile.IsEnabled = true;

                    }
                    else
                    {
                        PasswordBoxODecryptPassword.IsEnabled = true;
                        ButtonODecryptionFile.IsEnabled = true;
                        PasswordBoxODecryptPassword.Focus();
                    }
                }
            }
            catch (Exception ex)
            {
                ButtonOpenFile.IsEnabled = false;
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void ButtonCloseWindowClicked(object sender, RoutedEventArgs e)
        {
            if (!IBoolInOpening) { this.Close(); }
        }

        private void OfflineLanguageOpenForEdit_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void OfflineLanguageOpenForEdit_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IBoolInOpening) { e.Cancel = true; return; }
        }

        private void OfflineLanguageOpenForEdit_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.DrawWindowsBackGround(this);
                ImageOpenOfflineLanguage.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000044.ico"), UriKind.RelativeOrAbsolute));
                DisplayElementCharacters(false);
            }
            catch { }
        }

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelOpenOfflineLanguageTip.Content = App.GetDisplayCharater("M05001");

            TabItemLanguageFileOpen.Header = " " + App.GetDisplayCharater("M05002") + " ";

            LabelOLanguagePackagePath.Content = App.GetDisplayCharater("M05003");
            LabelODecryptionPassword.Content = App.GetDisplayCharater("M05004");
            ButtonODecryptionFile.ToolTip = App.GetDisplayCharater("M05005");

            ButtonOpenFile.Content = App.GetDisplayCharater("M05006");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M05007");
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
                if (LStrTableName == "T_00_005")
                {
                    IStrOfflineFileType = "01";
                }
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
                if (LStrTableName == "T_00_005")
                {
                    IStrOfflineFileType = "01";
                }
            }
            catch
            {
                LBoolReturn = false;
            }
            return LBoolReturn;
        }
        #endregion
    }
}
