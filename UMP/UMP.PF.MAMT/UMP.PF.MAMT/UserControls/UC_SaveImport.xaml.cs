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
using System.Xml;
using System.IO;
using System.ComponentModel;
using UMP.PF.MAMT.WCF_LanPackOperation;
using System.Data;

namespace UMP.PF.MAMT.UserControls
{
    /// <summary>
    /// UC_SaveImport.xaml 的交互逻辑
    /// </summary>
    public partial class UC_SaveImport : UserControl
    {
        public UC_LanManager LanManagerWindow;
        private bool IBoolHaveError;
        private int IIntCurrentRow;

        //获得语言类型列表所用的变量
        private ReturnResult RRLanguangeTypesResult = null;       //从数据库获得支持的语言种类 wcfclient的返回值
        private List<string> IListStrAllowSurportLanguageCode = null;       //所有支持的语言种类
        private bool bIsImportSuccess = true;

        //导入语言包用到的变量
        private BackgroundWorker InstanceBackgroundWorkerImportOnlyRefresh = null;
        private ReturnResult RRemoveLanResult = null;
        private string strImportLanguageType = string.Empty;
        private BackgroundWorker InstanceBackgroundWorkerImportAppenUpdate = null;
        int IIntAllRows = 0;
        XmlNode IXMLNodeTableDataRowsList;
        string IStrImportOptions = string.Empty;

        private List<LanguageInfo> lstAllLanguages = null;

        public UC_SaveImport(UC_LanManager _main)
        {
            InitializeComponent();
            LanManagerWindow = _main;
            this.Loaded += UC_SaveImport_Loaded;
        }

        void UC_SaveImport_Loaded(object sender, RoutedEventArgs e)
        {
            imgSave.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000024.ico"), UriKind.RelativeOrAbsolute));
            imgOperation.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000018.ico"), UriKind.RelativeOrAbsolute));
            imgCancel.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000026.ico"), UriKind.RelativeOrAbsolute));
            dpSave.MouseLeftButtonDown += dpSave_MouseLeftButtonDown;
            dpCancel.MouseLeftButtonDown += dpCancel_MouseLeftButtonDown;
            InitControlContent();
        }

        void InitControlContent()
        {
            RRLanguangeTypesResult = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 1, App.GCurrentDBServer);
            UC_LanguageData uc_LanData = LanManagerWindow.dpData.Children[0] as UC_LanguageData;
            lstAllLanguages = uc_LanData.lvLanguage.ItemsSource as List<LanguageInfo>;

            IListStrAllowSurportLanguageCode = new List<string>();
            if (RRLanguangeTypesResult.BoolReturn)
            {
                if (RRLanguangeTypesResult.DataSetReturn.Tables.Count > 0)
                {
                    for (int i = 0; i < RRLanguangeTypesResult.DataSetReturn.Tables[0].Rows.Count; i++)
                    {
                        IListStrAllowSurportLanguageCode.Add(RRLanguangeTypesResult.DataSetReturn.Tables[0].Rows[i]["C002"].ToString());
                    }
                }
            }
        }

        /// <summary>
        /// 取消导入语言包事件
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
        /// 确认导入语言包事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dpSave_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UC_ImportLan uc_Import = LanManagerWindow.dpDetail.Children[0] as UC_ImportLan;
            BeginImportLanguagePackage(uc_Import);

            //LanManagerWindow.spOperator.Children.RemoveAt(1);
            //UC_DBOpeartionDefault defaultWin = new UC_DBOpeartionDefault(LanManagerWindow);
            //LanManagerWindow.spOperator.Children.Add(defaultWin);
            //LanManagerWindow.dpDetail.Children.Clear();
        }

        private void BeginImportLanguagePackage(UC_ImportLan uc_Import)
        {
            string LStrLanguagePackageFile = string.Empty;
            string LStrDecryptionPassword = string.Empty;

            bool LBoolCallReturn = true;
            string LStrCallReturn = string.Empty;

            bool LBoolIsAllowImport = false;

            Stream LStreamXmlBody = null;

            try
            {
                #region 导入信息的逻辑判断
                LStrLanguagePackageFile = uc_Import.txtLanFile.Text;
                if (string.IsNullOrEmpty(LStrLanguagePackageFile))
                {
                    MessageBox.Show(this.TryFindResource("FilePathNotNull").ToString(),
                     this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (uc_Import.txtLanPwd.IsEnabled == false) { LStrDecryptionPassword = "vctyoung"; }
                else { LStrDecryptionPassword = uc_Import.txtLanPwd.Password.Trim(); }
                if (string.IsNullOrEmpty(LStrDecryptionPassword))
                {
                    MessageBox.Show(this.TryFindResource("LanFilePwdNotNull").ToString(),
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                LBoolCallReturn = uc_Import.TryDecryptionLanguagePackage(LStrLanguagePackageFile, LStrDecryptionPassword, ref LStrCallReturn);
                if (!LBoolCallReturn)
                {
                    MessageBox.Show(this.TryFindResource("SaveFileError").ToString(),
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                foreach (string LStrSingleLanguageCode in IListStrAllowSurportLanguageCode)
                {
                    if (LStrSingleLanguageCode == LStrCallReturn) { LBoolIsAllowImport = true; break; }
                }
                strImportLanguageType = LStrCallReturn;

                if (!LBoolIsAllowImport)
                {
                    string strError = string.Format(this.TryFindResource("Error001").ToString(), LStrCallReturn);
                    MessageBox.Show(strError,
                   this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                string strMsg = string.Format(this.TryFindResource("Confirm001").ToString(), LStrCallReturn);
                MessageBoxResult LMessageBoxResult = MessageBox.Show(strMsg,
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (LMessageBoxResult != MessageBoxResult.Yes) { return; }
                //IStrLanguageCode = LStrCallReturn;
                #endregion

                // this.MainPanel.IsEnabled = false;

                #region 导入数据的读取到XmlNode
                XmlDocument LXmlDocTableDataLoaded = new XmlDocument();
                LStreamXmlBody = new MemoryStream(Encoding.UTF8.GetBytes(uc_Import.IStrLanguagePackageBody));
                LXmlDocTableDataLoaded.Load(LStreamXmlBody);
                IXMLNodeTableDataRowsList = LXmlDocTableDataLoaded.SelectSingleNode("LanguageDataRowsList");
                IIntAllRows = IXMLNodeTableDataRowsList.ChildNodes.Count;

                ComboBoxItem LComboBoxItem = uc_Import.cmbImoprtOptions.SelectedItem as ComboBoxItem;
                IStrImportOptions = LComboBoxItem.DataContext.ToString();
                #endregion

                IBoolHaveError = false;
                IIntCurrentRow = 0;
                if (IStrImportOptions == "Refresh") { ImportOtionsIsOnlyRefresh(); }
                else
                {
                    ImportOtionsIsAppendUpdate();
                }
            }
            catch (Exception ex)
            {
                LanManagerWindow.IsEnabled = true;

                MessageBox.Show(ex.Message,
                    this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 全部替换语言包
        /// </summary>
        private void ImportOtionsIsOnlyRefresh()
        {
            try
            {
                // IBoolInConnecting = true;
                //IWindowsParent.StatusBarShowLeftStart(true, string.Format(App.GetDisplayCharacterFromLanguagePackage(this.Name, "N017"), IStrLanguageCode));
                InstanceBackgroundWorkerImportOnlyRefresh = new BackgroundWorker();
                InstanceBackgroundWorkerImportOnlyRefresh.RunWorkerCompleted += InstanceBackgroundWorkerImportOnlyRefresh_RunWorkerCompleted;
                InstanceBackgroundWorkerImportOnlyRefresh.DoWork += InstanceBackgroundWorkerImportOnlyRefresh_DoWork;
                InstanceBackgroundWorkerImportOnlyRefresh.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                //IBoolInConnecting = false;
                //IWindowsParent.StatusBarShowLeftStart(false, string.Empty);
                LanManagerWindow.IsEnabled = true;
                if (InstanceBackgroundWorkerImportOnlyRefresh != null)
                {
                    InstanceBackgroundWorkerImportOnlyRefresh.Dispose(); InstanceBackgroundWorkerImportOnlyRefresh = null;
                }
                MessageBox.Show(ex.Message,
                   this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void InstanceBackgroundWorkerImportOnlyRefresh_DoWork(object sender, DoWorkEventArgs e)
        {
            //删除一种类型的语言
            List<string> lstParams = new List<string>();
            lstParams.Add(App.GCurrentDBServer.DbType.ToString());
            lstParams.Add(App.GCurrentDBServer.Host);
            lstParams.Add(App.GCurrentDBServer.Port);
            lstParams.Add(App.GCurrentDBServer.ServiceName);
            lstParams.Add(App.GCurrentDBServer.LoginName);
            lstParams.Add(App.GCurrentDBServer.Password);
            lstParams.Add(strImportLanguageType);
            RRemoveLanResult = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 4, lstParams);
            if (RRemoveLanResult.BoolReturn)
            {
                //开始写入数据库
                ReturnResult InsertResult = null;
                foreach (XmlNode node in IXMLNodeTableDataRowsList.ChildNodes)
                {
                    if (node.ChildNodes.Count > 0)
                    {
                        InsertResult = InsertLanguage(node);
                        if (!InsertResult.BoolReturn)
                        {
                            bIsImportSuccess = false;
                            string strError = DateTime.Now + " | " + string.Format(this.TryFindResource("Error006").ToString(), node["C002"]);
                            List<string> lstErrors = new List<string>();
                            lstErrors.Add(strError);
                            strError = DateTime.Now + " | " + string.Format(this.TryFindResource("Error006").ToString(), InsertResult.StringReturn);
                            Logger.WriteLog(lstErrors);
                        }
                    }
                }
            }
            else
            {
                InstanceBackgroundWorkerImportOnlyRefresh.Dispose(); InstanceBackgroundWorkerImportOnlyRefresh = null;
                MessageBox.Show(RRemoveLanResult.StringReturn,
                   this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        void InstanceBackgroundWorkerImportOnlyRefresh_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (bIsImportSuccess)
            {
                List<string> lstParams = new List<string>();
                lstParams.Add(App.GCurrentDBServer.DbType.ToString());
                lstParams.Add(App.GCurrentDBServer.Host);
                lstParams.Add(App.GCurrentDBServer.Port);
                lstParams.Add(App.GCurrentDBServer.ServiceName);
                lstParams.Add(App.GCurrentDBServer.LoginName);
                lstParams.Add(App.GCurrentDBServer.Password);
                lstParams.Add(strImportLanguageType);

                ReturnResult result = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 7, lstParams);
                MessageBox.Show(this.TryFindResource("Message002").ToString(),
                   this.TryFindResource("Message003").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                if (LanManagerWindow.dpData.Children.Count > 0)
                {
                    UC_LanguageData uc_data = LanManagerWindow.dpData.Children[0] as UC_LanguageData;
                    Common.RefershData(uc_data.lvLanguage);
                    uc_data.lstViewrSource = uc_data.lvLanguage.ItemsSource as List<LanguageInfo>;
                }
                LanManagerWindow.InitControl();
            }
            else
            {
                MessageBox.Show(this.TryFindResource("Error005").ToString() + App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT\\Log" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Date + ".log",
                  this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }

            LanManagerWindow.spOperator.Children.RemoveAt(1);
            UC_DBOpeartionDefault defaultWin = new UC_DBOpeartionDefault(LanManagerWindow);
            LanManagerWindow.spOperator.Children.Add(defaultWin);
            LanManagerWindow.dpDetail.Children.Clear();
        }

        /// <summary>
        /// 追加/追加并更新语言包
        /// </summary>
        void ImportOtionsIsAppendUpdate()
        {
            InstanceBackgroundWorkerImportAppenUpdate = new BackgroundWorker();
            InstanceBackgroundWorkerImportAppenUpdate.RunWorkerCompleted += InstanceBackgroundWorkerImportAppenUpdate_RunWorkerCompleted;
            InstanceBackgroundWorkerImportAppenUpdate.DoWork += InstanceBackgroundWorkerImportAppenUpdate_DoWork;
            InstanceBackgroundWorkerImportAppenUpdate.RunWorkerAsync();
        }

        void InstanceBackgroundWorkerImportAppenUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            //DataTable dt_SingleLanguageType = new System.Data.DataTable();
            //dt_SingleLanguageType = dt_AllLanguages.Clone();
            //List<DataRow> lstSingleLan = dt_AllLanguages.Select("C001=" + strImportLanguageType).ToList();
            //for (int i = 0; i < lstSingleLan.Count; i++)
            //{
            //    dt_SingleLanguageType.Rows.Add(lstSingleLan[i].ItemArray);
            //}
            List<LanguageInfo> lstSingleLanguageType = lstAllLanguages.Where(p => p.LanguageCode == strImportLanguageType).ToList();
            string strLanCode = string.Empty;
            string strMsgID = string.Empty;
            List<LanguageInfo> lstSelectedRow = null;
            ReturnResult result = new ReturnResult();
            result.BoolReturn = true;
            result.StringReturn = string.Empty;
            foreach (XmlNode node in IXMLNodeTableDataRowsList.ChildNodes)
            {
                strLanCode = node["C001"].InnerText;
                strMsgID = node["C002"].InnerText;
                lstSelectedRow = lstSingleLanguageType.Where(p=>p.LanguageCode == strLanCode && p.MessageID== strMsgID).ToList();
                if (lstSelectedRow.Count <= 0)
                {
                    result = InsertLanguage(node);
                }
                else
                {
                    if (IStrImportOptions == Enums.ImportOperationType.Update.ToString())
                    {
                        result = UpdateLanguage(node);
                    }
                }
                if (!result.BoolReturn)
                {
                    bIsImportSuccess = false;
                    string strError = DateTime.Now + " | " + string.Format(this.TryFindResource("Error006").ToString(), node["C002"]);
                    List<string> lstErrors = new List<string>();
                    lstErrors.Add(strError);
                    strError = DateTime.Now + " | " + string.Format(this.TryFindResource("Error006").ToString(), result.StringReturn);
                    Logger.WriteLog(lstErrors);
                }
            }
        }

        void InstanceBackgroundWorkerImportAppenUpdate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (bIsImportSuccess)
            {
                List<string> lstParams = new List<string>();
                lstParams.Add(App.GCurrentDBServer.DbType.ToString());
                lstParams.Add(App.GCurrentDBServer.Host);
                lstParams.Add(App.GCurrentDBServer.Port);
                lstParams.Add(App.GCurrentDBServer.ServiceName);
                lstParams.Add(App.GCurrentDBServer.LoginName);
                lstParams.Add(App.GCurrentDBServer.Password);
                lstParams.Add(strImportLanguageType);

                ReturnResult result = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 7, lstParams);
                MessageBox.Show(this.TryFindResource("Message002").ToString(),
                   this.TryFindResource("Message003").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                if (LanManagerWindow.dpData.Children.Count > 0)
                {
                    UC_LanguageData uc_data = LanManagerWindow.dpData.Children[0] as UC_LanguageData;
                    Common.RefershData(uc_data.lvLanguage);
                    uc_data.lstViewrSource = uc_data.lvLanguage.ItemsSource as List<LanguageInfo>;
                }

                LanManagerWindow.InitControl();
            }
            else
            {
                MessageBox.Show(this.TryFindResource("Error005").ToString() + App.GStrLoginUserApplicationDataPath + "\\UMP.PF.MAMT\\Log" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + ".log",
                  this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            LanManagerWindow.spOperator.Children.RemoveAt(1);
            UC_DBOpeartionDefault defaultWin = new UC_DBOpeartionDefault(LanManagerWindow);
            LanManagerWindow.spOperator.Children.Add(defaultWin);
            LanManagerWindow.dpDetail.Children.Clear();
        }

        ReturnResult InsertLanguage(XmlNode lanNode)
        {
            List<string> lstParams = new List<string>();
            lstParams.Add(App.GCurrentDBServer.DbType.ToString());
            lstParams.Add(App.GCurrentDBServer.Host);
            lstParams.Add(App.GCurrentDBServer.Port);
            lstParams.Add(App.GCurrentDBServer.ServiceName);
            lstParams.Add(App.GCurrentDBServer.LoginName);
            lstParams.Add(App.GCurrentDBServer.Password);
            //lstParams.Add(strImportLanguageType); 
            //从i=1开始 因为第一个childnode值是rownum 不需要导入数据库
            for (int i = 1; i < lanNode.ChildNodes.Count; i++)
            {
                if (lanNode.ChildNodes[i].Name.ToUpper() == "DISPLAYMESSAGE" || lanNode.ChildNodes[i].Name.ToUpper() == "TIPMESSAGE")
                {
                    if (lanNode.ChildNodes[i].InnerText.Length > 2048)
                    {
                        continue;
                    }
                    if (lanNode.ChildNodes[i].InnerText.Length > 1024)
                    {
                        lstParams.Add(lanNode.ChildNodes[i].InnerText.Substring(0, 1024));
                        lstParams.Add(lanNode.ChildNodes[i].InnerText.Substring(1025));
                    }
                    else
                    {
                        lstParams.Add(lanNode.ChildNodes[i].InnerText);
                        lstParams.Add("");
                    }
                }
                else
                {
                    lstParams.Add(lanNode.ChildNodes[i].InnerText);
                }

            }
            lstParams.Add("");  //InObject字段值 目前为空
            ReturnResult Result = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, 5, lstParams);
            return Result;
        }

        ReturnResult UpdateLanguage(XmlNode lanNode)
        {
            List<string> lstParams = new List<string>();
            lstParams.Add(App.GCurrentDBServer.DbType.ToString());
            lstParams.Add(App.GCurrentDBServer.Host);
            lstParams.Add(App.GCurrentDBServer.Port);
            lstParams.Add(App.GCurrentDBServer.ServiceName);
            lstParams.Add(App.GCurrentDBServer.LoginName);
            lstParams.Add(App.GCurrentDBServer.Password);
            string strNodeName = string.Empty;
            for (int i = 1; i < lanNode.ChildNodes.Count; i++)
            {
                strNodeName = lanNode.ChildNodes[i].Name.ToUpper();
                if (strNodeName == "DISPLAYMESSAGE" || strNodeName == "TIPMESSAGE")
                {
                    if (lanNode.ChildNodes[i].InnerText.Length > 2048)
                    {
                        continue;
                    }
                    if (lanNode.ChildNodes[i].InnerText.Length > 1024)
                    {
                        lstParams.Add(lanNode.ChildNodes[i].InnerText.Substring(0, 1024));
                        lstParams.Add(lanNode.ChildNodes[i].InnerText.Substring(1025));
                    }
                    else
                    {
                        lstParams.Add(lanNode.ChildNodes[i].InnerText);
                        lstParams.Add("");
                    }
                }
                else if (strNodeName == "C001" || strNodeName == "C002")
                {
                    lstParams.Add(lanNode.ChildNodes[i].InnerText);
                }
            }
            ReturnResult result = AboutLanguagesInServer.WCFOperationMthodA("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port
                    , 3, lstParams);
            return result;
        }

    }
}
