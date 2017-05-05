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
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMP.MAMT.PublicClasses;

namespace UMP.MAMT.BasicControls
{
    public partial class UCServerObjects : UserControl, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        //当前获得Focus的TreeViewItem
        public TreeViewItem ITreeViewItemCurrentSelected = null;

        //前一个获得Focus的TreeViewItem
        public TreeViewItem ITreeViewItemPreviewSelected = null;

        public UCServerObjects()
        {
            InitializeComponent();
            this.Loaded += UCServerObjects_Loaded;
            ImageCloseServerObject.MouseLeftButtonDown += ImageCloseServerObject_MouseLeftButtonDown;
            ButtonConnectToServer.Click += ButtonOperationsClicked;
            ButtonDisconnectToServer.Click += ButtonOperationsClicked;
            ButtonServerProperties.Click += ButtonOperationsClicked;
            ButtonRefreshInformation.Click += ButtonOperationsClicked;

            TreeViewServerObjects.SelectedItemChanged += TreeViewServerObjects_SelectedItemChanged;
            TreeViewServerObjects.PreviewMouseRightButtonDown += TreeViewServerObjects_PreviewMouseRightButtonDown;
        }

        public void AddApplicationServer2TreeView(List<string> AListStrConnectedInfo, DataSet ADataSetServerParameters)
        {
            try
            {
                //加入根节点
                TreeViewItem LTreeViewItemRoot = new TreeViewItem();

                LTreeViewItemRoot.DataContext = ADataSetServerParameters;
                LTreeViewItemRoot.Tag = AListStrConnectedInfo;
                LTreeViewItemRoot.Name = "TVI001";
                TreeViewItemProps.SetItemImageName(LTreeViewItemRoot, App.GStrApplicationDirectory + @"\Images\00000018.ico");
                TreeViewServerObjects.Items.Add(LTreeViewItemRoot);

                //加入服务器 证书信息
                TreeViewItem LTreeViewItemCertificate = new TreeViewItem();
                LTreeViewItemCertificate.DataContext = ADataSetServerParameters.Tables[0];
                LTreeViewItemCertificate.Name = "TVI002";
                LTreeViewItemRoot.Items.Add(LTreeViewItemCertificate);

                //加入UMP.PF站点
                TreeViewItem LTreeViewItemSite = new TreeViewItem();
                LTreeViewItemSite.DataContext = ADataSetServerParameters.Tables[1];
                LTreeViewItemSite.Name = "TVI003";
                LTreeViewItemRoot.Items.Add(LTreeViewItemSite);

                //加入License Server 节点
                if (ADataSetServerParameters.Tables[3] != null)
                {
                    TreeViewItem LTreeViewItemLicenseServer = new TreeViewItem();
                    LTreeViewItemLicenseServer.DataContext = ADataSetServerParameters.Tables[3];
                    LTreeViewItemLicenseServer.Name = "TVI007";
                    LTreeViewItemRoot.Items.Add(LTreeViewItemLicenseServer);
                }
                //加入 UMPDataDB 节点
                TreeViewItem LTreeViewItemDataDB = new TreeViewItem();
                LTreeViewItemDataDB.DataContext = ADataSetServerParameters.Tables[2];
                LTreeViewItemDataDB.Name = "TVI004";
                LTreeViewItemRoot.Items.Add(LTreeViewItemDataDB);

                ResetTreeViewItemHeader();

                LTreeViewItemRoot.IsExpanded = true;

                TreeViewServerObjects.Focus();
                LTreeViewItemRoot.IsSelected = true;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("AddApplicationServer2TreeView()\n" + ex.ToString());
            }
        }

        #region 鼠标右键 TreeViewItem 的处理
        private void TreeViewServerObjects_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                TreeViewItem LTreeViewItemRightButtonDown = GetTemplatedAncestor<TreeViewItem>(e.OriginalSource as FrameworkElement);
                if (LTreeViewItemRightButtonDown != null) { LTreeViewItemRightButtonDown.IsSelected = true; }
            }
            catch { }
        }

        private T GetTemplatedAncestor<T>(FrameworkElement AFrameworkElement) where T : FrameworkElement
        {
            if (AFrameworkElement is T)
            {
                return AFrameworkElement as T;
            }

            FrameworkElement LFrameworkElement = AFrameworkElement.TemplatedParent as FrameworkElement;
            if (LFrameworkElement != null)
            {
                return GetTemplatedAncestor<T>(LFrameworkElement);
            }

            return null;
        }
        #endregion

        private void TreeViewServerObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                TreeViewItem LTreeViewItemOld = e.OldValue as TreeViewItem;
                TreeViewItem LTreeViewItemNew = e.NewValue as TreeViewItem;

                ITreeViewItemPreviewSelected = LTreeViewItemOld;
                ITreeViewItemCurrentSelected = LTreeViewItemNew;

                if (IOperationEvent != null)
                {
                    MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                    LEventArgs.StrElementTag = "TSCH";
                    LEventArgs.ObjSource = ITreeViewItemCurrentSelected;
                    LEventArgs.AppenObjeSource3 = "TV";
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch { }
        }

        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelServerInformation.Content = App.GetDisplayCharater("M01005");
            ImageCloseServerObject.ToolTip = App.GetDisplayCharater("M01006");

            ButtonConnectToServer.ToolTip = App.GetDisplayCharater("M01007");
            ButtonDisconnectToServer.ToolTip = App.GetDisplayCharater("M01008");
            ButtonServerProperties.ToolTip = App.GetDisplayCharater("M01009");
            ButtonRefreshInformation.ToolTip = App.GetDisplayCharater("M01010");

            if (ABoolLanguageChange) { ResetTreeViewItemHeader(); }
        }

        public void RefreshTreeViewItemAfterBinding(DataTable ADataTableBindingInfo)
        {
            try
            {
                if (ITreeViewItemCurrentSelected.Name != "TVI003") { return; }
                ITreeViewItemCurrentSelected.DataContext = ADataTableBindingInfo;
                ResetTreeViewItemHeader();

                if (IOperationEvent != null)
                {
                    MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                    LEventArgs.StrElementTag = "TSCH";
                    LEventArgs.ObjSource = ITreeViewItemCurrentSelected;
                    LEventArgs.AppenObjeSource3 = "TV";
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch { }
        }

        public void RefreshTreeViewItemAfterInstallCertificate(DataTable ADataTableCertificate)
        {
            try
            {
                if (ITreeViewItemCurrentSelected.Name != "TVI002") { return; }
                ITreeViewItemCurrentSelected.DataContext = ADataTableCertificate;
                ResetTreeViewItemHeader();

                if (IOperationEvent != null)
                {
                    MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                    LEventArgs.StrElementTag = "TSCH";
                    LEventArgs.ObjSource = ITreeViewItemCurrentSelected;
                    LEventArgs.AppenObjeSource3 = "TV";
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch { }
        }

        public void RefreshTreeViewItemAfterChangeDBProfile(DataTable ADataTableDatabaseProfile)
        {
            try
            {
                if (ITreeViewItemCurrentSelected.Name != "TVI004") { return; }
                ITreeViewItemCurrentSelected.DataContext = ADataTableDatabaseProfile;
                ITreeViewItemCurrentSelected.Items.Clear();

                ResetTreeViewItemHeader();

                if (IOperationEvent != null)
                {
                    MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                    LEventArgs.StrElementTag = "TSCH";
                    LEventArgs.ObjSource = ITreeViewItemCurrentSelected;
                    LEventArgs.AppenObjeSource3 = "TV";
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch { }
        }

        public void RefreshTreeViewItemAfterSettingLicenseService(DataTable ADataTableLicenseService)
        {
            try
            {
                if (ITreeViewItemCurrentSelected.Name != "TVI007") { return; }
                ITreeViewItemCurrentSelected.DataContext = ADataTableLicenseService;
                ITreeViewItemCurrentSelected.Items.Clear();

                ResetTreeViewItemHeader();

                if (IOperationEvent != null)
                {
                    MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                    LEventArgs.StrElementTag = "TSCH";
                    LEventArgs.ObjSource = ITreeViewItemCurrentSelected;
                    LEventArgs.AppenObjeSource3 = "TV";
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch { }
        }

        public void RefreshTreeViewItemAfterOpenDatabase(DataTable ADataTableRentList)
        {
            try
            {
                if (ITreeViewItemCurrentSelected.Name != "TVI004") { return; }
                ITreeViewItemCurrentSelected.Items.Clear();
                DataTable LDataTableDBProfile = ITreeViewItemCurrentSelected.DataContext as DataTable;

                TreeViewItem LTreeViewItemAllRentList = new TreeViewItem();
                LTreeViewItemAllRentList.DataContext = "";
                LTreeViewItemAllRentList.Name = "TVI008";
                LTreeViewItemAllRentList.Tag = "";
                TreeViewItemProps.SetItemImageName(LTreeViewItemAllRentList, App.GStrApplicationDirectory + @"\Images\00000038.ico");
                ITreeViewItemCurrentSelected.Items.Add(LTreeViewItemAllRentList);
                ITreeViewItemCurrentSelected.IsExpanded = true;
                ResetTreeViewItemHeader();
                GetRentLogicPartitionInfo(LDataTableDBProfile, ADataTableRentList);
            }
            catch { }
        }

        public void RefreshTreeViewItemAfterLogicPartitionSet(MamtOperationEventArgs AEventArgs)
        {
            try
            {
                if (ITreeViewItemCurrentSelected.Name != "TVI006") { return; }
                DataRow LDataRowLogicPartition = ITreeViewItemCurrentSelected.DataContext as DataRow;
                DataTable LDataTableLoginPartition = AEventArgs.ObjSource as DataTable;

                LDataRowLogicPartition["S00"] = "1";
                LDataRowLogicPartition["S01"] = LDataTableLoginPartition.Rows[0]["C004"].ToString();
                LDataRowLogicPartition["S02"] = LDataTableLoginPartition.Rows[0]["C005"].ToString();
                LDataRowLogicPartition["S03"] = LDataTableLoginPartition.Rows[0]["C006"].ToString();
                ITreeViewItemCurrentSelected.DataContext = LDataRowLogicPartition;

                ResetTreeViewItemHeader();

                if (IOperationEvent != null)
                {
                    MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                    LEventArgs.StrElementTag = "TSCH";
                    LEventArgs.ObjSource = ITreeViewItemCurrentSelected;
                    LEventArgs.AppenObjeSource3 = "TV";
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch { }
        }

        private void UCServerObjects_Loaded(object sender, RoutedEventArgs e)
        {
            ImageServerObject.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000006.ico"), UriKind.RelativeOrAbsolute));
            ImageCloseServerObject.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000007.ico"), UriKind.RelativeOrAbsolute));

            ImageConnectToServer.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000008.ico"), UriKind.RelativeOrAbsolute));
            ImageDisconnectToServer.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000009.ico"), UriKind.RelativeOrAbsolute));
            ImageServerProperties.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000010.ico"), UriKind.RelativeOrAbsolute));
            ImageRefreshInformation.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000011.ico"), UriKind.RelativeOrAbsolute));

            ButtonDisconnectToServer.Visibility = Visibility.Collapsed;
            ButtonServerProperties.Visibility = Visibility.Collapsed;
            ButtonRefreshInformation.Visibility = Visibility.Collapsed;

            DisplayElementCharacters(false);
        }

        private void ImageCloseServerObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IOperationEvent != null)
            {
                MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                LEventArgs.StrElementTag = ((Image)sender).Tag.ToString();
                LEventArgs.ObjSource = (Image)sender;
                IOperationEvent(this, LEventArgs);
            }
        }

        private void ButtonOperationsClicked(object sender, RoutedEventArgs e)
        {
            if (IOperationEvent != null)
            {
                MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();

                LEventArgs.StrElementTag = ((Button)sender).Tag.ToString();
                LEventArgs.ObjSource = ITreeViewItemCurrentSelected;
                IOperationEvent(this, LEventArgs);
            }
        }

        private void ResetTreeViewItemHeader()
        {
            try
            {
                foreach (TreeViewItem LTreeViewItem in TreeViewServerObjects.Items) { DisplayObjectCharactersTreeViewItem(LTreeViewItem); }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("ResetTreeViewItemHeader()\n" + ex.ToString());
            }
        }

        private void DisplayObjectCharactersTreeViewItem(TreeViewItem ATreeViewItem)
        {
            string LStrItemName = string.Empty;
            string LStrHeader = string.Empty;

            try
            {
                LStrItemName = ATreeViewItem.Name;

                if (ATreeViewItem.Header != null) { LStrHeader = ATreeViewItem.Header.ToString(); }

                #region 跟节点
                if (LStrItemName == "TVI001")
                {
                    List<string> LListStrConnectedInfo = ATreeViewItem.Tag as List<string>;
                    if (LListStrConnectedInfo[0] == "127.0.0.1")
                    {
                        LStrHeader = string.Format(App.GetDisplayCharater("M01025"), App.GetDisplayCharater("M01026"));
                    }
                    else
                    {
                        LStrHeader = string.Format(App.GetDisplayCharater("M01025"), LListStrConnectedInfo[0]);
                    }
                }
                #endregion

                #region 安全证书节点
                if (LStrItemName == "TVI002")
                {
                    LStrHeader = App.GetDisplayCharater("M01037");
                    DataTable LDataTableCertificate = ATreeViewItem.DataContext as DataTable;
                    if (LDataTableCertificate.Rows.Count != 3)
                    {
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000024.ico");
                    }
                    else
                    {
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000023.ico");
                    }
                }
                #endregion

                #region Site 绑定信息节点
                if (LStrItemName == "TVI003")
                {
                    bool LBoolBinded = false;
                    LStrHeader = "UMP.PF (" + App.GetDisplayCharater("M01027") + ")";
                    DataTable LDataTableIISBindingInfo = ATreeViewItem.DataContext as DataTable;
                    foreach (DataRow LDataRowBinding in LDataTableIISBindingInfo.Rows)
                    {
                        if (LDataRowBinding["Used"].ToString() == "1") { LBoolBinded = true; break; }
                    }
                    if (LBoolBinded)
                    {
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000019.ico");
                    }
                    else
                    {
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000020.ico");
                    }
                }
                #endregion

                #region License Server节点
                if (LStrItemName == "TVI007")
                {
                    bool LBoolExistEnabled = false;

                    LStrHeader = App.GetDisplayCharater("M01104");
                    DataTable LDataTableLicenseServer = ATreeViewItem.DataContext as DataTable;
                    foreach (DataRow LDataRowSingleLicenseServer in LDataTableLicenseServer.Rows)
                    {
                        if (LDataRowSingleLicenseServer["OtherInfo"].ToString() == "1") { LBoolExistEnabled = true; break; }
                    }
                    if (LBoolExistEnabled)
                    {
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000035.ico");
                    }
                    else
                    {
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000036.ico");
                    }
                }
                #endregion

                #region 数据库节点
                if (LStrItemName == "TVI004")
                {
                    DataTable LDataTableDBProfileInfo = ATreeViewItem.DataContext as DataTable;
                    if (LDataTableDBProfileInfo.Rows.Count == 0)
                    {
                        LStrHeader = App.GetDisplayCharater("M01053");
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000026.png");
                    }
                    else
                    {
                        if (LDataTableDBProfileInfo.Rows[0]["CanConnect"].ToString() == "1")
                        {
                            LStrHeader = string.Format(App.GetDisplayCharater("M01052"), LDataTableDBProfileInfo.Rows[0]["OtherArgs"].ToString());
                            TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000027.png");
                            App.IsCreatedDB = true;
                        }
                        else
                        {
                            LStrHeader = App.GetDisplayCharater("M01054");
                            TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000028.png");
                        }
                    }
                }
                #endregion

                #region 租户列表节点
                if (LStrItemName == "TVI008")
                {
                    LStrHeader = App.GetDisplayCharater("M01114");
                }
                #endregion

                #region 租户表逻辑分区节点
                string LStrAlias = string.Empty;

                if (LStrItemName == "TVI006")
                {
                    DataRow LDataRowLogicPartitionInfo = ATreeViewItem.DataContext as DataRow;
                    LStrAlias = LDataRowLogicPartitionInfo["Alias"].ToString();
                    LStrAlias = App.GetConvertedData("LPT" + LStrAlias);
                    LStrHeader = LStrAlias;
                    if (LDataRowLogicPartitionInfo["S01"].ToString() == "1")
                    {
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000032.ico");
                    }
                    else
                    {
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000033.ico");
                    }
                }
                #endregion

                ATreeViewItem.Header = LStrHeader;
                for (int LIntLoopChildren = 0; LIntLoopChildren < ATreeViewItem.Items.Count; LIntLoopChildren++)
                {
                    TreeViewItem LTreeViewItemChild = (TreeViewItem)ATreeViewItem.Items[LIntLoopChildren];
                    DisplayObjectCharactersTreeViewItem(LTreeViewItemChild);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("DisplayObjectCharactersTreeViewItem()\n" + ex.ToString());
            }
        }

        #region 获取租户的逻辑分区信息
        List<DataTable> IListDataTableRentLogicPartitionInfo = null;
        DataTable IDataTableRentList = new DataTable();
        private BackgroundWorker IBackgroundWorkerReadData = null; 
        private void GetRentLogicPartitionInfo(DataTable ADataTableDBProfile, DataTable ADataTableRentList)
        {
            List<DataTable> LListDataTableArgs = new List<DataTable>();

            try
            {
                IDataTableRentList = ADataTableRentList;
                IListDataTableRentLogicPartitionInfo = new List<DataTable>();
                LListDataTableArgs.Add(ADataTableDBProfile);
                LListDataTableArgs.Add(ADataTableRentList);
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01074"), true);
                IBackgroundWorkerReadData = new BackgroundWorker();
                IBackgroundWorkerReadData.RunWorkerCompleted += IBackgroundWorkerReadData_RunWorkerCompleted;
                IBackgroundWorkerReadData.DoWork += IBackgroundWorkerReadData_DoWork;
                IBackgroundWorkerReadData.RunWorkerAsync(LListDataTableArgs);
            }
            catch
            {
                if (IBackgroundWorkerReadData != null)
                {
                    IBackgroundWorkerReadData.Dispose();
                    IBackgroundWorkerReadData = null;
                }
                App.ShowCurrentStatus(int.MaxValue, string.Empty);
            }
        }

        private void IBackgroundWorkerReadData_DoWork(object sender, DoWorkEventArgs e)
        {
            List<DataTable> LListDataTableArgs = e.Argument as List<DataTable>;

            string LStrRentToken = string.Empty;

            foreach (DataRow LDataRowSingleRent in LListDataTableArgs[1].Rows)
            {
                LStrRentToken = LDataRowSingleRent["C021"].ToString();
                IListDataTableRentLogicPartitionInfo.Add(App.GetRentPartionInfo(LListDataTableArgs[0], LStrRentToken));
            }
        }

        private void IBackgroundWorkerReadData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            App.ShowCurrentStatus(int.MaxValue, string.Empty);
            ShowRentAndRentLogicPartitionInfo();

            IBackgroundWorkerReadData.Dispose();
            IBackgroundWorkerReadData = null;
        }

        private void ShowRentAndRentLogicPartitionInfo()
        {
            string LStrRentToken = string.Empty;

            ITreeViewItemCurrentSelected = ITreeViewItemCurrentSelected.Items[0] as TreeViewItem;
            ITreeViewItemCurrentSelected.IsSelected = true;

            foreach (DataRow LDataRowSingleRent in IDataTableRentList.Rows)
            {
                //加入租户信息
                TreeViewItem LTreeViewItemRent = new TreeViewItem();
                LStrRentToken = LDataRowSingleRent["C021"].ToString();
                LTreeViewItemRent.Header = LDataRowSingleRent["C002"].ToString();
                LTreeViewItemRent.DataContext = LDataRowSingleRent;
                LTreeViewItemRent.Name = "TVI005";
                TreeViewItemProps.SetItemImageName(LTreeViewItemRent, App.GStrApplicationDirectory + @"\Images\00000031.ico");
                ITreeViewItemCurrentSelected.Items.Add(LTreeViewItemRent);

                foreach (DataTable LDataTableSingleRentLPInfo in IListDataTableRentLogicPartitionInfo)
                {
                    if (LDataTableSingleRentLPInfo.TableName != "T_RENT_" + LStrRentToken) { continue; }
                    foreach (DataRow LDataRowSingleLogicPartion in LDataTableSingleRentLPInfo.Rows)
                    {
                        TreeViewItem LTreeViewItemRentLogicPartition = new TreeViewItem();
                        LTreeViewItemRentLogicPartition.DataContext = LDataRowSingleLogicPartion;
                        LTreeViewItemRentLogicPartition.Name = "TVI006";
                        LTreeViewItemRentLogicPartition.Tag = LStrRentToken;
                        LTreeViewItemRent.Items.Add(LTreeViewItemRentLogicPartition);
                    }
                }
            }
            ResetTreeViewItemHeader();
            ITreeViewItemCurrentSelected.IsExpanded = true;
        }
        #endregion
    }
}
