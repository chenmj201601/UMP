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
using UMP.PF.MAMT.Classes;
using System.Data;
using System.ComponentModel;
using System.Xml;
using VoiceCyber.EncryptionPrivate;
using UMP.PF.MAMT.WCF_LanPackOperation;

namespace UMP.PF.MAMT.UserControls
{
    /// <summary>
    /// UC_ConfirmExportLan.xaml 的交互逻辑
    /// </summary>
    public partial class UC_ConfirmExportLan : UserControl
    {
        public UC_LanManager LanManagerWindow;
        private UC_ExportLan uc_Export;
        private List<LanguageInfo> lstAllLanguages;
        BackgroundWorker InstanceBackgroundWorkerExportLanguagePackage = null;
        private List<string> IListStrExportOptions = new List<string>();        //0:文件存放路径；1:语言ID；2:加密密码
        DataTable dt_SingleLanguageType = null;
        bool bIsWriteSuccess = false;

        public UC_ConfirmExportLan(UC_LanManager _main)
        {
            InitializeComponent();
            LanManagerWindow = _main;
            this.Loaded += UC_ConfirmExportLan_Loaded;
        }

        void UC_ConfirmExportLan_Loaded(object sender, RoutedEventArgs e)
        {
            imgSave.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000024.ico"), UriKind.RelativeOrAbsolute));
            imgOperation.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000018.ico"), UriKind.RelativeOrAbsolute));
            imgCancel.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000026.ico"), UriKind.RelativeOrAbsolute));
            dpSave.MouseLeftButtonDown += dpSave_MouseLeftButtonDown;
            dpCancel.MouseLeftButtonDown += dpCancel_MouseLeftButtonDown;
            uc_Export = LanManagerWindow.dpDetail.Children[0] as UC_ExportLan;
            UC_LanguageData uc_LanData = LanManagerWindow.dpData.Children[0] as UC_LanguageData;
            //DataView dv = uc_LanData.lvLanguage.ItemsSource as DataView;
            lstAllLanguages = uc_LanData.lstViewrSource;
        }

        /// <summary>
        /// 取消导出文件事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpCancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LanManagerWindow.dpDetail.Children.Clear();
            LanManagerWindow.spOperator.Children.RemoveAt(1);
            UC_DBOpeartionDefault defaultWin2 = new UC_DBOpeartionDefault(LanManagerWindow);
            LanManagerWindow.spOperator.Children.Add(defaultWin2);
        }

        /// <summary>
        /// 确认导出文件事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpSave_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            LanManagerWindow.IsEnabled = false;
            IListStrExportOptions.Clear();
            IListStrExportOptions.Add("");
            IListStrExportOptions.Add("");
            IListStrExportOptions.Add("");

            if (string.IsNullOrEmpty(uc_Export.txtLanFile.Text))
            {
                MessageBox.Show(this.TryFindResource("Error002").ToString(),
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                LanManagerWindow.IsEnabled = true;
                return;
            }

            IListStrExportOptions[0] =  uc_Export.txtLanFile.Text;
            string strLanCode = (uc_Export.cmbLanID.SelectedItem as ComboBoxItem).DataContext.ToString();
            IListStrExportOptions[1] = strLanCode;
            string strPwd = string.Empty;

            if (uc_Export.chkIsEncryption.IsChecked == true)
            {
                if (string.IsNullOrEmpty(uc_Export.txtPwd.Password))
                {
                    MessageBox.Show(this.TryFindResource("Error003").ToString(),
                        this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    LanManagerWindow.IsEnabled = true;
                    return;
                }
                if (uc_Export.txtPwd.Password != uc_Export.txtConfirmPwd.Password)
                {
                    MessageBox.Show(this.TryFindResource("Error004").ToString(),
                        this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    LanManagerWindow.IsEnabled = true;
                    return;
                }
                strPwd = uc_Export.txtPwd.Password;
            }
            else
            {
                strPwd = "vctyoung";
            }
            IListStrExportOptions[2] = strPwd;
            StartExport();

            LanManagerWindow.spOperator.Children.RemoveAt(1);
            UC_DBOpeartionDefault defaultWin = new UC_DBOpeartionDefault(LanManagerWindow);
            LanManagerWindow.spOperator.Children.Add(defaultWin);
            LanManagerWindow.dpDetail.Children.Clear();
        }

        void StartExport()
        {
            InstanceBackgroundWorkerExportLanguagePackage = new BackgroundWorker();
            InstanceBackgroundWorkerExportLanguagePackage.DoWork += InstanceBackgroundWorkerExportLanguagePackage_DoWork;
            InstanceBackgroundWorkerExportLanguagePackage.RunWorkerCompleted += InstanceBackgroundWorkerExportLanguagePackage_RunWorkerCompleted;
            InstanceBackgroundWorkerExportLanguagePackage.RunWorkerAsync();
        }

        void InstanceBackgroundWorkerExportLanguagePackage_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LanManagerWindow.IsEnabled = true;
            if (!bIsWriteSuccess)
            {
                MessageBox.Show(this.TryFindResource("Error007").ToString(),
                        this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(this.TryFindResource("Message004").ToString(),
                        this.TryFindResource("Message003").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        void InstanceBackgroundWorkerExportLanguagePackage_DoWork(object sender, DoWorkEventArgs e)
        {
            DBInfo dbInfo = App.GCurrentDBServer;
            ReturnResult result = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 2, dbInfo);
            if (result.BoolReturn)
            {
                if (result.DataSetReturn.Tables.Count > 0)
                {
                    DataTable dtAllLan = result.DataSetReturn.Tables[0];
                    List<DataRow> lstSingleLan = dtAllLan.Select("C001=" + IListStrExportOptions[1]).ToList();
                    dt_SingleLanguageType = new DataTable();
                    dt_SingleLanguageType = dtAllLan.Clone();
                    for (int i = 0; i < lstSingleLan.Count; i++)
                    {
                        dt_SingleLanguageType.Rows.Add(lstSingleLan[i].ItemArray);
                    }
                    string LStrCallReturn = string.Empty;
                    bIsWriteSuccess = WriteDataToXMLFile(dt_SingleLanguageType, ref LStrCallReturn);
                }
            }
        }

        private bool WriteDataToXMLFile(DataTable ADataTableSelected, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            int LIntLoopColumns = 0, LIntAllColumns = 0;
            int LIntLoopRows = 0, LIntAllRows = 0;
            string LStrSourceFile = string.Empty;
            string LStrColumnName = string.Empty;

            try
            {
                LStrSourceFile = System.IO.Path.Combine(App.GStrUserMyDocumentsDirectory, IListStrExportOptions[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml");
                XmlTextWriter LXmlWriter = new XmlTextWriter(LStrSourceFile, Encoding.UTF8);
                LXmlWriter.Formatting = Formatting.Indented;
                LXmlWriter.WriteStartDocument(true);

                LXmlWriter.WriteStartElement("LanguageDataRowsList");
                LXmlWriter.WriteAttributeString("TableName", "T_00_005");
                LXmlWriter.WriteAttributeString("LanguageCode", IListStrExportOptions[1]);
                LXmlWriter.WriteAttributeString("Version", "1.01.001");
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
                    LBoolReturn = VoiceCyberPrivate.EncryptFileToFile(LStrSourceFile, IListStrExportOptions[0], IListStrExportOptions[2], true, true, ref AStrReturn);
                }
                if (LBoolReturn) { AStrReturn = LStrSourceFile; }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = ex.Message;
            }

            return LBoolReturn;
        }
    }
}
