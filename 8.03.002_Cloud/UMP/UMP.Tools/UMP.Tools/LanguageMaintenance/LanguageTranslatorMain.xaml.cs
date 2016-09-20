using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMP.Tools.BasicModule;
using UMP.Tools.PublicClasses;
using UMP.Tools.UMPWcfService00003;

namespace UMP.Tools.LanguageMaintenance
{
    public partial class LanguageTranslatorMain : UserControl, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;
        public SystemMainWindow IWindowsParent = null;
        private DataTable IDataTableLanguagePackage = null;

        #region 所有 BackgroundWorker
        //将语言包数据写入到UI中
        private BackgroundWorker IBackgroundWorkerA = null;
        
        //将语言包数据写入到数据库中
        private BackgroundWorker IBackgroundWorkerB = null;
        #endregion

        //存放所有语言包中所有的数据
        private ObservableCollection<ListViewItemSingle> IListObservableCollectionLanguagePackage;

        //当前被修改的对象
        private TextBox ITextBoxCurrentClicked = null;

        public LanguageTranslatorMain()
        {
            InitializeComponent();
            this.Loaded += LanguageTranslatorMain_Loaded;
            this.Unloaded += LanguageTranslatorMain_Unloaded;
            ButtonSearchBegin.Click += ButtonDockPanelClicked;
            ButtonSearchPre.Click += ButtonDockPanelClicked;
            ButtonSearchClear.Click += ButtonDockPanelClicked;
            ButtonSearchNext.Click += ButtonDockPanelClicked;
        }

        private void LanguageTranslatorMain_Unloaded(object sender, RoutedEventArgs e)
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

        private void LanguageTranslatorMain_Loaded(object sender, RoutedEventArgs e)
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
            if (e.StrElementTag == "SLANG") { WriteDataTable2ListView(e); return; }
            if (e.StrElementTag == "SAVEL") { WriteLanguagePackage2Database(); return; }
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

        #region 将语言包数据写入到UI中
        public void WriteDataTable2ListView(DataTable ADataTableLanguagePackage)
        {
            try
            {
                IDataTableLanguagePackage = ADataTableLanguagePackage;
                if (IListObservableCollectionLanguagePackage == null) { IListObservableCollectionLanguagePackage = new ObservableCollection<ListViewItemSingle>(); }
                IListObservableCollectionLanguagePackage.Clear();
                ListViewLanguagePackageDetail.ItemsSource = IListObservableCollectionLanguagePackage;

                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01040"), true);
                if (IBackgroundWorkerA == null) { IBackgroundWorkerA = new BackgroundWorker(); }
                IBackgroundWorkerA.RunWorkerCompleted += IBackgroundWorkerA_RunWorkerCompleted;
                IBackgroundWorkerA.DoWork += IBackgroundWorkerA_DoWork;
                IBackgroundWorkerA.RunWorkerAsync();
            }
            catch
            {
                if (IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
            }
        }

        private void WriteDataTable2ListView(OperationEventArgs e)
        {
            try
            {
                IDataTableLanguagePackage = e.ObjSource as DataTable;
                IListObservableCollectionLanguagePackage.Clear();
                ListViewLanguagePackageDetail.ItemsSource = IListObservableCollectionLanguagePackage;

                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01040"), true);
                if (IBackgroundWorkerA == null) { IBackgroundWorkerA = new BackgroundWorker(); }
                IBackgroundWorkerA.RunWorkerCompleted += IBackgroundWorkerA_RunWorkerCompleted;
                IBackgroundWorkerA.DoWork += IBackgroundWorkerA_DoWork;
                IBackgroundWorkerA.RunWorkerAsync();
            }
            catch
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                if (IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
            }
        }

        private void IBackgroundWorkerA_DoWork(object sender, DoWorkEventArgs e)
        {
            int LIntCurrent = 0, LIntAll = 0;
            string LStrShowTip = string.Empty;

            try
            {
                LIntAll = IDataTableLanguagePackage.Rows.Count;
                foreach (DataRow LDataRowSingleLanguage in IDataTableLanguagePackage.Rows)
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

                    if (LDataRowSingleLanguage["CIsChanged"].ToString() == "1")
                    {
                        LListViewItemSingleLanguage.DataChangeStatus = ListViewItemDataChangeStatus.IsChanged;
                    }
                    if (LDataRowSingleLanguage["TIsChanged"].ToString() == "1")
                    {
                        LListViewItemSingleLanguage.TipChangeStatus = ListViewItemTipChangeStatus.IsChanged;
                    }

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

        #region 将语言包数据写入到数据库中
        bool IBoolHasSaveError = false;
        private OperationDataArgs I00003OperationReturn = new OperationDataArgs();

        private void WriteLanguagePackage2Database()
        {
            try
            {
                IBoolHasSaveError = false;
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M02004"), true);
                if (IBackgroundWorkerB == null) { IBackgroundWorkerB = new BackgroundWorker(); }
                IBackgroundWorkerB.RunWorkerCompleted += IBackgroundWorkerB_RunWorkerCompleted;
                IBackgroundWorkerB.DoWork += IBackgroundWorkerB_DoWork;
                IBackgroundWorkerB.RunWorkerAsync();
            }
            catch(Exception ex)
            {
                App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
                if (IBackgroundWorkerB != null)
                {
                    IBackgroundWorkerB.Dispose(); IBackgroundWorkerB = null;
                }
                App.ShowExceptionMessage(App.GetDisplayCharater("E002001") + "\n" + ex.Message);
            }
        }

        private void IBackgroundWorkerB_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00003Client LService00003Client = null;

            List<string> LListWcfArgs = new List<string>();

            try
            {
                List<string> LListStrDBProfile = App.GSystemMainWindow.GetCurrentDatabaseProfile();
                List<string> LListStrAppServer = App.GSystemMainWindow.GetCurrentAppServerConnection();

                //foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }

                LBasicHttpBinding = App.CreateBasicHttpBinding(true, 15);
                LEndpointAddress = App.CreateEndpointAddress(LListStrAppServer[0], LListStrAppServer[1], true, "Service00003");
                LService00003Client = new Service00003Client(LBasicHttpBinding, LEndpointAddress);

                foreach (ListViewItemSingle LListViewItemSingleLanguage in IListObservableCollectionLanguagePackage)
                {
                    if (LListViewItemSingleLanguage.DataChangeStatus == ListViewItemDataChangeStatus.IsDefault && LListViewItemSingleLanguage.TipChangeStatus == ListViewItemTipChangeStatus.IsDefault) { continue; }
                    LListWcfArgs.Clear();
                    foreach (string LStrSingleProfile in LListStrDBProfile) { LListWcfArgs.Add(LStrSingleProfile); }
                    LListWcfArgs.Add(LListViewItemSingleLanguage.LanguageCode.ToString());
                    LListWcfArgs.Add(LListViewItemSingleLanguage.MessageID);
                    if (LListViewItemSingleLanguage.DataChangeStatus == ListViewItemDataChangeStatus.IsChanged)
                    {
                        LListWcfArgs.Add("C005" + App.GStrSpliterChar + LListViewItemSingleLanguage.MessageContentText01);
                        LListWcfArgs.Add("C006" + App.GStrSpliterChar + LListViewItemSingleLanguage.MessageContentText02);
                    }
                    if (LListViewItemSingleLanguage.TipChangeStatus == ListViewItemTipChangeStatus.IsChanged)
                    {
                        LListWcfArgs.Add("C007" + App.GStrSpliterChar + LListViewItemSingleLanguage.MessageTipDisplay01);
                        LListWcfArgs.Add("C008" + App.GStrSpliterChar + LListViewItemSingleLanguage.MessageTipDisplay02);
                    }
                    I00003OperationReturn = LService00003Client.OperationMethodA(6, LListWcfArgs);
                    if (I00003OperationReturn.BoolReturn)
                    {
                        LListViewItemSingleLanguage.DataChangeStatus = ListViewItemDataChangeStatus.IsDefault;
                        LListViewItemSingleLanguage.TipChangeStatus = ListViewItemTipChangeStatus.IsDefault;
                        LListViewItemSingleLanguage.MessageContentTextOld = LListViewItemSingleLanguage.MessageContentText01 + LListViewItemSingleLanguage.MessageContentText02;
                        LListViewItemSingleLanguage.MessageTipDisplayOld = LListViewItemSingleLanguage.MessageTipDisplay01 + LListViewItemSingleLanguage.MessageTipDisplay02;
                        OperationEventArgs LEventArgs = new OperationEventArgs();
                        LEventArgs.StrElementTag = "CSLANG";
                        LEventArgs.ObjSource = LListViewItemSingleLanguage;
                        IOperationEvent(this, LEventArgs);
                    }
                    else
                    {
                        IBoolHasSaveError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                I00003OperationReturn.BoolReturn = false;
                I00003OperationReturn.StringReturn = "UMP001E007" + App.GStrSpliterChar + ex.Message;
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
            App.ShowCurrentStatus(int.MaxValue, string.Empty, true);
            if (IBackgroundWorkerB != null)
            {
                IBackgroundWorkerB.Dispose(); IBackgroundWorkerB = null;
            }
            if (IBoolHasSaveError)
            {
                MessageBox.Show(App.GetDisplayCharater("M02006"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            MessageBox.Show(App.GetDisplayCharater("M02005"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

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
                LanguageTranslatorEdit LLanguageTranslatorEdit = new LanguageTranslatorEdit();
                LLanguageTranslatorEdit.Owner = App.GSystemMainWindow;
                LLanguageTranslatorEdit.ILanguageTranslatorMain = this;
                LLanguageTranslatorEdit.ShowSingleLanguageDetail(LListViewItemSingle, LStrLanguageTarget);
                LLanguageTranslatorEdit.ShowDialog();
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
