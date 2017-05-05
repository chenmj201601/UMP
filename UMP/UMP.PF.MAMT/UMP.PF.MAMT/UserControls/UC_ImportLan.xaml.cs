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
using System.Windows.Forms;
using VoiceCyber.EncryptionPrivate;
using System.Xml;
using System.IO;
using UMP.PF.MAMT.Classes;
using UMP.PF.MAMT.WCF_LanPackOperation;

namespace UMP.PF.MAMT.UserControls
{
    /// <summary>
    /// UC_ImportLan.xaml 的交互逻辑
    /// </summary>
    public partial class UC_ImportLan : System.Windows.Controls.UserControl
    {
        //保存解密后语言包文件的xml数据
        public string IStrLanguagePackageBody = string.Empty;
        private string IStrTableName = string.Empty;

        public UC_ImportLan()
        {
            InitializeComponent();
            this.Loaded += UC_ImportLan_Loaded;
            
        }

        void UC_ImportLan_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtLanPwd.LostFocus += txtLanPwd_LostFocus;

            ComboBoxItem item = new ComboBoxItem();
            item.DataContext = Enums.ImportOperationType.Refresh;
            item.Content = TryFindResource("ImportOperationRefersh").ToString();
            item.SetResourceReference(ComboBoxItem.StyleProperty, "ControlBaseStyle");
            cmbImoprtOptions.Items.Add(item);

            item = new ComboBoxItem();
            item.DataContext = Enums.ImportOperationType.Update;
            item.SetResourceReference(ComboBoxItem.ContentProperty, "ImportOperationUpdate");
            item.SetResourceReference(ComboBoxItem.StyleProperty, "ControlBaseStyle");
            cmbImoprtOptions.Items.Add(item);

            item = new ComboBoxItem();
            item.DataContext = Enums.ImportOperationType.Append;
            item.SetResourceReference(ComboBoxItem.ContentProperty, "ImportOperationAppend");
            item.SetResourceReference(ComboBoxItem.StyleProperty, "ControlBaseStyle");
            cmbImoprtOptions.Items.Add(item);
            btnBrowse.Click += btnBrowse_Click;
        }

        /// <summary>
        /// 浏览按钮事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "YLP文件|*.YLP|所有文件|*.*";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "zip";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                return;
            }
            string fileName = openFileDialog.FileName;
            txtLanFile.Text = fileName;

            string LStrCallReturn = string.Empty;
            bool LBoolCallReturn = TryDecryptionLanguagePackage(txtLanFile.Text, "vctyoung", ref LStrCallReturn);
            if (!LBoolCallReturn)
            {
                txtLanPwd.IsEnabled = true;
                //ButtonIDecryptionFile.IsEnabled = true;
                txtLanPwd.Focus();
            }
            else
            {
                txtLanPwd.Password = string.Empty;
                txtLanPwd.IsEnabled = false;
                //ButtonIDecryptionFile.IsEnabled = false;
                ReturnResult wcfresult = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 1, App.GCurrentDBServer);
                if (wcfresult.BoolReturn)
                {
                    if (wcfresult.DataSetReturn.Tables.Count > 0)
                    {
                        for (int i = 0; i < wcfresult.DataSetReturn.Tables[0].Rows.Count; i++)
                        {
                            if (wcfresult.DataSetReturn.Tables[0].Rows[i]["C002"].ToString() == LStrCallReturn)
                            {
                                txtLanID.Text = LStrCallReturn + " (" + wcfresult.DataSetReturn.Tables[0].Rows[i]["C005"] + ")";
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 尝试解密语言包文件(导入时用)
        /// </summary>
        /// <param name="AStrSourceFile"></param>
        /// <param name="AStrPassword"></param>
        /// <param name="AStrReturn"></param>
        /// <returns></returns>
        public bool TryDecryptionLanguagePackage(string AStrSourceFile, string AStrPassword, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrSourceBody = string.Empty;

            try
            {
                IStrLanguagePackageBody = string.Empty;
                AStrReturn = string.Empty;
                LBoolReturn = VoiceCyberPrivate.DecryptFileToString(AStrSourceFile, AStrPassword, ref AStrReturn);
                if (LBoolReturn)
                {
                    LBoolReturn = IsXMLTypeFileOrString("", AStrReturn);
                    if (LBoolReturn)
                    {
                        LStrSourceBody = AStrReturn;
                        LBoolReturn = SourceIsLanguagePackage(LStrSourceBody, ref AStrReturn);
                        if (!LBoolReturn)
                        {
                            AStrReturn = "FileContentError";
                        }
                        else
                        {
                            IStrLanguagePackageBody = LStrSourceBody;
                        }
                    }
                    else
                    {
                        AStrReturn = "FileFormatError";
                    }
                }
                else
                {
                    AStrReturn = "LanFileDecryptFailed";
                }
            }
            catch
            {
                LBoolReturn = false;
                AStrReturn = "LanFileDecryptFailed";
            }
            return LBoolReturn;
        }

        private bool IsXMLTypeFileOrString(string AStrSourceFile, string AStrSoureBody)
        {
            bool LBoolReturn = true;

            try
            {
                XmlDocument LXmlDocTableDataLoaded = new XmlDocument();

                if (!string.IsNullOrEmpty(AStrSoureBody))
                {
                    Stream LStreamXmlBody = new MemoryStream(Encoding.UTF8.GetBytes(AStrSoureBody));
                    LXmlDocTableDataLoaded.Load(LStreamXmlBody);
                }
            }
            catch { LBoolReturn = false; }
            return LBoolReturn;
        }

        private bool SourceIsLanguagePackage(string AStrSoureBody, ref string AStrReturn)
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
                IStrTableName = LStrTableName;
                AStrReturn = LStrLanguageCode;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = ex.Message;
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 解密密码的失去焦点事件  在失去焦点时 判断密码是否正确
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void txtLanPwd_LostFocus(object sender, RoutedEventArgs e)
        {
            string LStrLanguagePackageFile = string.Empty;
            string LStrDecryptionPassword = string.Empty;
            bool LBoolCallReturn = true;
            string LStrCallReturn = string.Empty;

            try
            {
               // btn.IsEnabled = true;
                LStrLanguagePackageFile = txtLanFile.Text;
                LStrDecryptionPassword = txtLanPwd.Password.Trim();
                if (string.IsNullOrEmpty(LStrDecryptionPassword))
                {
                    System.Windows.MessageBox.Show(this.TryFindResource("LanFilePwdNotNull").ToString(),
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                LBoolCallReturn = TryDecryptionLanguagePackage(LStrLanguagePackageFile, LStrDecryptionPassword, ref LStrCallReturn);
                if (!LBoolCallReturn)
                {
                    System.Windows.MessageBox.Show(this.TryFindResource("LanFileDecryptFailed").ToString(),
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                ReturnResult result = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 1, App.GCurrentDBServer);
                if (result.BoolReturn)
                {
                    if (result.DataSetReturn.Tables.Count > 0)
                    {
                        for (int i = 0; i < result.DataSetReturn.Tables[0].Rows.Count; i++)
                        {
                            if (result.DataSetReturn.Tables[0].Rows[i]["C002"].ToString() == LStrCallReturn)
                            {
                                txtLanID.Text = LStrCallReturn + " (" + result.DataSetReturn.Tables[0].Rows[i]["C005"]+ ")";
                            }
                        }
                        if (string.IsNullOrEmpty(txtLanID.Text))
                        {
                            string strError = string.Format(TryFindResource("Error001").ToString(), LStrCallReturn);
                            System.Windows.MessageBox.Show(strError,
                                this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                System.Windows.MessageBox.Show(this.TryFindResource("FilePathNotNull").ToString(),
                     this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
