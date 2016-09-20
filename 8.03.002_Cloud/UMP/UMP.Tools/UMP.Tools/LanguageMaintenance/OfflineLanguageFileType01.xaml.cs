using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using UMP.Tools.BasicModule;
using UMP.Tools.PublicClasses;
using UMP.Tools.UMPWcfService00003;

namespace UMP.Tools.LanguageMaintenance
{
    public partial class OfflineLanguageFileType01 : UserControl, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        public SystemMainWindow IWindowsParent = null;

        private OperationEventArgs IOperationArgs = null;

        //存放所有语言包中所有的数据
        private ObservableCollection<ListViewItemSingle> IListObservableCollectionLanguagePackage;

        //当前被修改的对象
        private TextBox ITextBoxCurrentClicked = null;

        private DataTable IDataTableLanguagePackage = null;

        //将语言包数据写入到UI中
        private BackgroundWorker IBackgroundWorkerA = null;

        //将语言包数据写入到XML文件中
        private BackgroundWorker IBackgroundWorkerB = null;

        private OperationDataArgs I00003OperationReturn = new OperationDataArgs();

        //保存选项
        private List<string> IListStrSaveOptions = new List<string>();            //0:文件存放路径；1:语言ID；2:加密密码

        public OfflineLanguageFileType01()
        {
            InitializeComponent();
            this.Loaded += OfflineLanguageFileType01_Loaded;
            this.Unloaded += OfflineLanguageFileType01_Unloaded;
            ButtonSearchBegin.Click += ButtonDockPanelClicked;
            ButtonSearchPre.Click += ButtonDockPanelClicked;
            ButtonSearchClear.Click += ButtonDockPanelClicked;
            ButtonSearchNext.Click += ButtonDockPanelClicked;
        }

        private void OfflineLanguageFileType01_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.GSystemMainWindow.IOperationEvent -= GSystemMainWindow_IOperationEvent;
            }
            catch { }
        }

        #region 显示界面语言/语言改变
        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            DisplayElementObjectCharacters.DisplayUIObjectCharacters(ListViewLanguagePackageDetail, ListViewLanguagePackageDetail);
        }
        #endregion

        #region 根据输入的内容查找语言
        //存放查找用的临时变量
        private int IIntSearchedCursor = 0;
        private void ButtonDockPanelClicked(object sender, RoutedEventArgs e)
        {
            string LStrClickedObject = string.Empty;
            string LStrSearchBody = string.Empty;
            int LIntCurrentIndex = -1;
            int LIntLastMatchIndex = -1;
            bool LBoolIsFirstMatch = true;

            try
            {
                LStrSearchBody = TexBoxSearchBody.Text;
                LStrClickedObject = ((Button)sender).Tag.ToString();

                if (LStrClickedObject == "SearchC")
                {
                    TexBoxSearchBody.IsReadOnly = false;
                    ButtonSearchBegin.IsEnabled = true;
                }

                foreach (ListViewItemSingle LSingleItem in IListObservableCollectionLanguagePackage)
                {
                    LIntCurrentIndex += 1;

                    #region 清除查找
                    if (LStrClickedObject == "SearchC")
                    {
                        IIntSearchedCursor = 0;
                        LSingleItem.SearchStatus = ListViewItemSearchStatus.IsDefault; continue;
                    }
                    #endregion

                    #region 首次查找匹配内容
                    if (LStrClickedObject == "SearchB")
                    {
                        if (string.IsNullOrEmpty(LStrSearchBody.Trim())) { return; }
                        if (LSingleItem.MessageContentText.ToLower().Contains(LStrSearchBody.ToLower()) || LSingleItem.MessageTipDisplay.ToLower().Contains(LStrSearchBody.ToLower()))
                        {
                            LSingleItem.SearchStatus = ListViewItemSearchStatus.IsSearched;
                            if (LBoolIsFirstMatch)
                            {
                                LBoolIsFirstMatch = false;
                                LSingleItem.SearchStatus = ListViewItemSearchStatus.IsCurrent;
                                IIntSearchedCursor = LIntCurrentIndex;
                                ListViewLanguagePackageDetail.ScrollIntoView(LSingleItem);
                                TexBoxSearchBody.IsReadOnly = true;
                                ButtonSearchBegin.IsEnabled = false;
                            }
                        }
                        else
                        {
                            LSingleItem.SearchStatus = ListViewItemSearchStatus.IsDefault;
                        }
                    }
                    #endregion

                    #region 查找下一个匹配项
                    if (LStrClickedObject == "SearchN")
                    {
                        if (string.IsNullOrEmpty(LStrSearchBody.Trim())) { return; }
                        if (LIntCurrentIndex < IIntSearchedCursor) { continue; }

                        if (LIntCurrentIndex == IIntSearchedCursor)
                        {
                            //LSingleItem.SearchStatus = ListViewItemSearchStatus.IsSearched;
                            continue;
                        }
                        if (LSingleItem.SearchStatus == ListViewItemSearchStatus.IsSearched)
                        {
                            LSingleItem.SearchStatus = ListViewItemSearchStatus.IsCurrent;
                            IListObservableCollectionLanguagePackage[IIntSearchedCursor].SearchStatus = ListViewItemSearchStatus.IsSearched;
                            IIntSearchedCursor = LIntCurrentIndex;
                            ListViewLanguagePackageDetail.ScrollIntoView(IListObservableCollectionLanguagePackage[IIntSearchedCursor]);
                            return;
                        }
                    }
                    #endregion

                    #region 查找上一个匹配项
                    if (LStrClickedObject == "SearchP")
                    {
                        if (string.IsNullOrEmpty(LStrSearchBody.Trim())) { return; }
                        if (LIntCurrentIndex < IIntSearchedCursor && LSingleItem.SearchStatus == ListViewItemSearchStatus.IsSearched)
                        {
                            LIntLastMatchIndex = LIntCurrentIndex;
                            continue;
                        }
                        if (LIntCurrentIndex == IIntSearchedCursor)
                        {
                            if (LIntLastMatchIndex >= 0)
                            {
                                IListObservableCollectionLanguagePackage[LIntLastMatchIndex].SearchStatus = ListViewItemSearchStatus.IsCurrent;
                                LSingleItem.SearchStatus = ListViewItemSearchStatus.IsSearched;
                                IIntSearchedCursor = LIntLastMatchIndex;
                                ListViewLanguagePackageDetail.ScrollIntoView(LSingleItem);
                            }
                            return;
                        }
                    }
                    #endregion
                }
            }
            catch { }
        }
        #endregion

        private void OfflineLanguageFileType01_Loaded(object sender, RoutedEventArgs e)
        {
            ImageSearchBegin.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000031.png"), UriKind.RelativeOrAbsolute));
            ImageSearchNext.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000033.png"), UriKind.RelativeOrAbsolute));
            ImageSearchPre.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000034.png"), UriKind.RelativeOrAbsolute));
            ImageSearchClear.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000032.png"), UriKind.RelativeOrAbsolute));
            DisplayElementCharacters(false);
            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
            if (IListObservableCollectionLanguagePackage == null)
            {
                IListObservableCollectionLanguagePackage = new ObservableCollection<ListViewItemSingle>();
            }
            else { IListObservableCollectionLanguagePackage.Clear(); }
        }

        #region 从主窗口发送过来的消息
        private void GSystemMainWindow_IOperationEvent(object sender, OperationEventArgs e)
        {
            if (e.StrElementTag == "CLID") { DisplayElementCharacters(true); return; }
            //if (e.StrElementTag == "SOLANG") { WriteDataTable2ListView(e); return; }
            if (e.StrElementTag == "SAVEOL") { WriteLanguagePackage2OfflineFile(); return; }
        }
        #endregion

        #region 将语言包数据写入到UI中
        public void WriteDataTable2ListView(OperationEventArgs e)
        {
            string LStrLanguagePackageBody = string.Empty;

            try
            {
                IOperationArgs = e;

                if (IListObservableCollectionLanguagePackage == null) { IListObservableCollectionLanguagePackage = new ObservableCollection<ListViewItemSingle>(); }
                IListObservableCollectionLanguagePackage.Clear();
                ListViewLanguagePackageDetail.ItemsSource = IListObservableCollectionLanguagePackage;

                IDataTableLanguagePackage = e.ObjSource as DataTable;

                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01040"), true);
                if (IBackgroundWorkerA == null) { IBackgroundWorkerA = new BackgroundWorker(); }
                IBackgroundWorkerA.RunWorkerCompleted += IBackgroundWorkerA_RunWorkerCompleted;
                IBackgroundWorkerA.DoWork += IBackgroundWorkerA_DoWork;
                IBackgroundWorkerA.RunWorkerAsync();
            }
            catch { }
        }

        private void IBackgroundWorkerA_DoWork(object sender, DoWorkEventArgs e)
        {
            int LIntCurrent = 0, LIntAll = 0;
            string LStrShowTip = string.Empty;

            try
            {
                DataTable LDataTableLanguagePackage = IDataTableLanguagePackage;
                LIntAll = LDataTableLanguagePackage.Rows.Count;

                foreach (DataRow LDataRowSingleLanguage in LDataTableLanguagePackage.Rows)
                {
                    LIntCurrent += 1;

                    LStrShowTip = (((decimal)LIntCurrent / (decimal)LIntAll) * 100).ToString("F2") + "% " + App.GetDisplayCharater("M01040");
                    Dispatcher.Invoke(new Action(() => App.ShowCurrentStatus(1, LStrShowTip)));

                    ListViewItemSingle LListViewItemSingleLanguage = new ListViewItemSingle();

                    LListViewItemSingleLanguage.SearchStatus = ListViewItemSearchStatus.IsDefault;
                    LListViewItemSingleLanguage.DataChangeStatus = ListViewItemDataChangeStatus.IsDefault;
                    LListViewItemSingleLanguage.TipChangeStatus = ListViewItemTipChangeStatus.IsDefault;

                    LListViewItemSingleLanguage.IntItemIndex = LIntCurrent;
                    LListViewItemSingleLanguage.LanguageCode = int.Parse(LDataRowSingleLanguage["C001"].ToString());
                    LListViewItemSingleLanguage.MessageID = LDataRowSingleLanguage["C002"].ToString();
                    LListViewItemSingleLanguage.MessageServerity = int.Parse(LDataRowSingleLanguage["C003"].ToString());
                    LListViewItemSingleLanguage.MessageLevel = int.Parse(LDataRowSingleLanguage["C004"].ToString());
                    LListViewItemSingleLanguage.MessageContentText = LDataRowSingleLanguage["C005"].ToString() + LDataRowSingleLanguage["C006"].ToString();
                    LListViewItemSingleLanguage.MessageTipDisplay = LDataRowSingleLanguage["C007"].ToString() + LDataRowSingleLanguage["C008"].ToString();
                    LListViewItemSingleLanguage.MessageContentTextOld = LDataRowSingleLanguage["C005"].ToString() + LDataRowSingleLanguage["C006"].ToString();
                    LListViewItemSingleLanguage.MessageTipDisplayOld = LDataRowSingleLanguage["C007"].ToString() + LDataRowSingleLanguage["C008"].ToString();
                    LListViewItemSingleLanguage.BelongModuleID = int.Parse(LDataRowSingleLanguage["C009"].ToString());
                    LListViewItemSingleLanguage.BelongSubModuleID = int.Parse(LDataRowSingleLanguage["C010"].ToString());
                    LListViewItemSingleLanguage.InFrameOrPage = LDataRowSingleLanguage["C011"].ToString();
                    LListViewItemSingleLanguage.ObjectName = LDataRowSingleLanguage["C012"].ToString();

                    LListViewItemSingleLanguage.DataChangeStatus = ListViewItemDataChangeStatus.IsDefault;
                    LListViewItemSingleLanguage.TipChangeStatus = ListViewItemTipChangeStatus.IsDefault;

                    ListViewItemSingle LListViewItemSingleArg = LListViewItemSingleLanguage;
                    Dispatcher.Invoke(new Action(() => IListObservableCollectionLanguagePackage.Add(LListViewItemSingleArg)));
                }
            }
            catch { }
        }

        private void IBackgroundWorkerA_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
            }
            catch { }
            finally
            {
                if (IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
            }
        }
        #endregion

        #region 将语言包数据写入到离线文件
        private void WriteLanguagePackage2OfflineFile()
        {
            try
            {
                IListStrSaveOptions.Add(IOperationArgs.AppenObjeSource1.ToString());
                IListStrSaveOptions.Add(IOperationArgs.AppenObjeSource4.ToString());
                IListStrSaveOptions.Add(IOperationArgs.AppenObjeSource2.ToString());

                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01048"), true);
                if (IBackgroundWorkerB == null) { IBackgroundWorkerB = new BackgroundWorker(); }
                IBackgroundWorkerB.RunWorkerCompleted += IBackgroundWorkerB_RunWorkerCompleted;
                IBackgroundWorkerB.WorkerReportsProgress = true;
                IBackgroundWorkerB.DoWork += IBackgroundWorkerB_DoWork;
                IBackgroundWorkerB.ProgressChanged += IBackgroundWorkerB_ProgressChanged;
                IBackgroundWorkerB.RunWorkerAsync();
            }
            catch
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                if (IBackgroundWorkerB != null)
                {
                    IBackgroundWorkerB.Dispose(); IBackgroundWorkerB = null;
                }
            }
        }

        private void IBackgroundWorkerB_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string LStrShowTip = string.Empty;

            try
            {
                int LIntCurrentStep = e.ProgressPercentage;

                LStrShowTip = App.GetConvertedData("SOFL" + LIntCurrentStep.ToString("00"));

                App.ShowCurrentStatus(1, LStrShowTip);
            }
            catch { }

        }

        private void IBackgroundWorkerB_DoWork(object sender, DoWorkEventArgs e)
        {
            string LStrCallReturn = string.Empty;

            try
            {
                BackgroundWorker LBackgroundWorker = sender as BackgroundWorker;
                I00003OperationReturn.BoolReturn = true;

                LBackgroundWorker.ReportProgress(1);
                I00003OperationReturn.BoolReturn = Save2XmlStep01(ref LStrCallReturn);
                if (!I00003OperationReturn.BoolReturn) { I00003OperationReturn.StringReturn = LStrCallReturn; return; }

                LBackgroundWorker.ReportProgress(2);
                I00003OperationReturn.BoolReturn = Save2XmlStep02(ref LStrCallReturn);
                if (!I00003OperationReturn.BoolReturn) { I00003OperationReturn.StringReturn = LStrCallReturn; return; }

                LBackgroundWorker.ReportProgress(3);
                I00003OperationReturn.BoolReturn = Save2XmlStep03(ref LStrCallReturn);
                if (!I00003OperationReturn.BoolReturn) { I00003OperationReturn.StringReturn = LStrCallReturn; return; }

                LBackgroundWorker.ReportProgress(4);
                I00003OperationReturn.BoolReturn = Save2XmlStep04(ref LStrCallReturn);
                if (!I00003OperationReturn.BoolReturn) { I00003OperationReturn.StringReturn = LStrCallReturn; return; }
            }
            catch { }
        }

        private void IBackgroundWorkerB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrMessageBody = string.Empty;

            try
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                if (IBackgroundWorkerB != null)
                {
                    IBackgroundWorkerB.Dispose(); IBackgroundWorkerB = null;
                }
                if (I00003OperationReturn.BoolReturn)
                {
                    OperationEventArgs LOperationEventArgs = new OperationEventArgs();
                    LOperationEventArgs.StrElementTag = "SSOXL";
                    LOperationEventArgs.ObjSource = IDataTableLanguagePackage;
                    IOperationEvent(this, LOperationEventArgs);
                    LStrMessageBody = App.GetDisplayCharater("M01047");
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    string[] LStrOperationReturn = I00003OperationReturn.StringReturn.Split(App.GStrSpliterChar.ToCharArray());
                    LStrMessageBody = App.GetDisplayCharater(LStrOperationReturn[0]);
                    LStrMessageBody += "\n" + LStrOperationReturn[1];
                    MessageBox.Show(LStrMessageBody, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch { }
        }

        /// <summary>
        /// 将修改后的数据写入到 IDataTableLanguagePackage
        /// </summary>
        /// <param name="AStrReturn"></param>
        /// <returns></returns>
        private bool Save2XmlStep01(ref string AStrReturn)
        {
            bool LBooReturn = true;

            string LStrRowSelect = string.Empty;

            try
            {
                AStrReturn = string.Empty;
                foreach (ListViewItemSingle LListViewItemSingleLanguage in IListObservableCollectionLanguagePackage)
                {
                    if (LListViewItemSingleLanguage.DataChangeStatus == ListViewItemDataChangeStatus.IsDefault && LListViewItemSingleLanguage.TipChangeStatus == ListViewItemTipChangeStatus.IsDefault) { continue; }
                    LStrRowSelect = string.Format("C001 = {0} AND C002 = '{1}'", LListViewItemSingleLanguage.LanguageCode, LListViewItemSingleLanguage.MessageID);
                    DataRow[] LDataRowSelected = IDataTableLanguagePackage.Select(LStrRowSelect);
                    LDataRowSelected[0]["C005"] = LListViewItemSingleLanguage.MessageContentText01;
                    LDataRowSelected[0]["C006"] = LListViewItemSingleLanguage.MessageContentText01;
                    LDataRowSelected[0]["C007"] = LListViewItemSingleLanguage.MessageTipDisplay01;
                    LDataRowSelected[0]["C008"] = LListViewItemSingleLanguage.MessageTipDisplay02;
                }
            }
            catch (Exception ex)
            {
                LBooReturn = false;
                AStrReturn = "UMP003E001" + App.GStrSpliterChar + ex.Message;
            }

            return LBooReturn;
        }

        /// <summary>
        /// 备份历史文件
        /// </summary>
        /// <param name="AStrReturn"></param>
        /// <returns></returns>
        private bool Save2XmlStep02(ref string AStrReturn)
        {
            bool LBoolReturn = true;

            string LStrBackupTargetFile = string.Empty;

            try
            {
                AStrReturn = string.Empty;
                LStrBackupTargetFile = IListStrSaveOptions[0] + ".bak";
                if (System.IO.File.Exists(LStrBackupTargetFile)) { System.IO.File.Delete(LStrBackupTargetFile); System.Threading.Thread.Sleep(500); }
                System.IO.File.Move(IListStrSaveOptions[0], LStrBackupTargetFile);
                System.Threading.Thread.Sleep(500); 
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "UMP003E002" + App.GStrSpliterChar + ex.Message;
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 将修改后的数据写入到XML文件
        /// </summary>
        /// <param name="AStrReturn"></param>
        /// <returns></returns>
        private bool Save2XmlStep03(ref string AStrReturn)
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

                LStrSourceFile = System.IO.Path.Combine(App.GStrUserMyDocumentsDirectory, IListStrSaveOptions[1] + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xml");
                XmlTextWriter LXmlWriter = new XmlTextWriter(LStrSourceFile, Encoding.UTF8);
                LXmlWriter.Formatting = Formatting.Indented;
                LXmlWriter.WriteStartDocument(true);

                LXmlWriter.WriteStartElement("LanguageDataRowsList");
                LXmlWriter.WriteAttributeString("TableName", "T_00_005");
                LXmlWriter.WriteAttributeString("LanguageCode", IListStrSaveOptions[1]);
                LXmlWriter.WriteAttributeString("Version", "8.02.001");
                LXmlWriter.WriteAttributeString("Options", "Export");
                LXmlWriter.WriteAttributeString("Author", "Young Yang");

                LIntAllRows = IDataTableLanguagePackage.Rows.Count;
                LIntAllColumns = IDataTableLanguagePackage.Columns.Count;
                for (LIntLoopRows = 0; LIntLoopRows < LIntAllRows; LIntLoopRows++)
                {
                    LXmlWriter.WriteStartElement("T_00_005");
                    for (LIntLoopColumns = 0; LIntLoopColumns < LIntAllColumns; LIntLoopColumns++)
                    {
                        LStrColumnName = IDataTableLanguagePackage.Columns[LIntLoopColumns].ColumnName;
                        if (IDataTableLanguagePackage.Columns[LIntLoopColumns].DataType == typeof(DateTime))
                        {
                            string LStrDateTimeData = IDataTableLanguagePackage.Rows[LIntLoopRows][LIntLoopColumns].ToString();
                            if (!string.IsNullOrEmpty(LStrDateTimeData))
                            {
                                LXmlWriter.WriteElementString(LStrColumnName, ((DateTime)IDataTableLanguagePackage.Rows[LIntLoopRows][LIntLoopColumns]).ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                            else
                            {
                                LXmlWriter.WriteElementString(LStrColumnName, DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }
                        else
                        {
                            LXmlWriter.WriteElementString(LStrColumnName, IDataTableLanguagePackage.Rows[LIntLoopRows][LIntLoopColumns].ToString());
                        }
                    }
                    LXmlWriter.WriteEndElement();
                }
                LXmlWriter.WriteEndElement();
                LXmlWriter.WriteEndDocument();
                LXmlWriter.Flush();
                LXmlWriter.Close();

                if (string.IsNullOrEmpty(IListStrSaveOptions[2]))
                {
                    if (System.IO.File.Exists(IListStrSaveOptions[0])) { System.IO.File.Delete(IListStrSaveOptions[0]); }
                    System.IO.File.Move(LStrSourceFile, IListStrSaveOptions[0]);
                }
                else
                {
                    LBoolReturn = VoiceCyberPrivateEncryptionDecryption.EncryptFileToFile(LStrSourceFile, IListStrSaveOptions[0], IListStrSaveOptions[2], true, true, ref LStrCallReturn);
                    if (!LBoolReturn)
                    {
                        if (LStrCallReturn == "000")
                        {
                            AStrReturn = "UMP000E005" + App.GStrSpliterChar + "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "UMP003E003" + App.GStrSpliterChar + ex.Message;
            }
            return LBoolReturn;
        }

        /// <summary>
        /// 更新修改标志
        /// </summary>
        /// <param name="AStrReturn"></param>
        /// <returns></returns>
        private bool Save2XmlStep04(ref string AStrReturn)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReturn = string.Empty;
                foreach (ListViewItemSingle LListViewItemSingleLanguage in IListObservableCollectionLanguagePackage)
                {
                    if (LListViewItemSingleLanguage.DataChangeStatus == ListViewItemDataChangeStatus.IsDefault && LListViewItemSingleLanguage.TipChangeStatus == ListViewItemTipChangeStatus.IsDefault) { continue; }
                    LListViewItemSingleLanguage.DataChangeStatus = ListViewItemDataChangeStatus.IsDefault;
                    LListViewItemSingleLanguage.TipChangeStatus = ListViewItemTipChangeStatus.IsDefault;
                    LListViewItemSingleLanguage.MessageContentTextOld = LListViewItemSingleLanguage.MessageContentText01 + LListViewItemSingleLanguage.MessageContentText02;
                    LListViewItemSingleLanguage.MessageTipDisplayOld = LListViewItemSingleLanguage.MessageTipDisplay01 + LListViewItemSingleLanguage.MessageTipDisplay02;
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "UMP003E004" + App.GStrSpliterChar + ex.Message;
            }

            return LBoolReturn;
        }
        #endregion

        private void TextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            string LStrSender = string.Empty;

            try
            {
                TextBox LTextBoxSender = sender as TextBox;
                LStrSender = LTextBoxSender.Name;
                ListViewItemSingle LListViewItemSingle = LTextBoxSender.Tag as ListViewItemSingle;
                if (LStrSender == "TextBoxContentText")
                {
                    LListViewItemSingle.MessageContentText = LTextBoxSender.Text;
                    if (LListViewItemSingle.MessageContentText != LListViewItemSingle.MessageContentTextOld)
                    {
                        LListViewItemSingle.DataChangeStatus = ListViewItemDataChangeStatus.IsChanged;
                    }
                    else
                    {
                        LListViewItemSingle.DataChangeStatus = ListViewItemDataChangeStatus.IsDefault;
                    }
                }
                else
                {
                    LListViewItemSingle.MessageTipDisplay = LTextBoxSender.Text;
                    if (LListViewItemSingle.MessageTipDisplay != LListViewItemSingle.MessageTipDisplayOld)
                    {
                        LListViewItemSingle.TipChangeStatus = ListViewItemTipChangeStatus.IsChanged;
                    }
                    else
                    {
                        LListViewItemSingle.TipChangeStatus = ListViewItemTipChangeStatus.IsDefault;
                    }
                }
            }
            catch { }
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                ListViewItemSingle LListViewItemSingle = ((TextBox)sender).Tag as ListViewItemSingle;

                ListViewLanguagePackageDetail.SelectedIndex = LListViewItemSingle.IntItemIndex - 1;
            }
            catch { }
        }

        private void TextBoxMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string LStrSenderName = string.Empty;
            string LStrLanguageTarget = string.Empty;

            try
            {
                TextBox LTextBoxSender = sender as TextBox;
                LStrSenderName = LTextBoxSender.Name;
                ListViewItemSingle LListViewItemSingle = LTextBoxSender.Tag as ListViewItemSingle;
                if (LStrSenderName == "TextBoxContentText")
                {
                    LStrLanguageTarget = "C";
                }
                else
                {
                    LStrLanguageTarget = "T";
                }
                ITextBoxCurrentClicked = LTextBoxSender;
                OfflineLanguageEditType01 LOfflineLanguageEditType01 = new OfflineLanguageEditType01();
                LOfflineLanguageEditType01.Owner = App.GSystemMainWindow;
                LOfflineLanguageEditType01.IOfflineLanguageFileType01 = this;
                LOfflineLanguageEditType01.ShowSingleLanguageDetail(LListViewItemSingle, LStrLanguageTarget);
                LOfflineLanguageEditType01.ShowDialog();
            }
            catch { }
        }

        #region 更改修改后的语言
        public bool ChangeModifiedLanguage(string AStrAfterModified)
        {
            bool LBoolReturn = true;

            try
            {
                ITextBoxCurrentClicked.Text = AStrAfterModified;

            }
            catch { LBoolReturn = false; }

            return LBoolReturn;
        }
        #endregion
    }
}
