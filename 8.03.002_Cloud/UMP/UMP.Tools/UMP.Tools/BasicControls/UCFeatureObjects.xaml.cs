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
using System.Windows.Threading;
using UMP.Tools.BasicModule;
using UMP.Tools.PublicClasses;

namespace UMP.Tools.BasicControls
{
    public partial class UCFeatureObjects : UserControl, OperationsInterface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;

        //当前获得Focus的TreeViewItem
        public TreeViewItem ITreeViewItemCurrentSelected = null;

        //前一个获得Focus的TreeViewItem
        public TreeViewItem ITreeViewItemPreviewSelected = null;
        
        //
        private BackgroundWorker IBackgroundWorkerA = null;

        #region 当前连接的应用服务器的连接信息 和 返回的结果集
        private List<string> IListStrConnectedInfo = new List<string>();
        private List<DataSet> IListDataSetServerParameters = new List<DataSet>();
        #endregion

        public UCFeatureObjects()
        {
            InitializeComponent();

            this.Loaded += UCFeatureObjects_Loaded;
            ImageCloseFeatureObject.MouseLeftButtonDown += ImageCloseFeatureObject_MouseLeftButtonDown;

            ButtonConnectToServer.Click += ButtonOperationsClicked;
            ButtonDisconnectToServer.Click += ButtonOperationsClicked;
            ButtonServerProperties.Click += ButtonOperationsClicked;
            ButtonRefreshInformation.Click += ButtonOperationsClicked;

            TreeViewServerObjects.SelectedItemChanged += TreeViewServerObjects_SelectedItemChanged;
            TreeViewServerObjects.PreviewMouseRightButtonDown += TreeViewServerObjects_PreviewMouseRightButtonDown;
        }

        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelFeatureInformation.Content = App.GetDisplayCharater("M01005");
            ImageCloseFeatureObject.ToolTip = App.GetDisplayCharater("M01006");

            ButtonConnectToServer.ToolTip = App.GetDisplayCharater("M01007");
            ButtonDisconnectToServer.ToolTip = App.GetDisplayCharater("M01008");
            ButtonServerProperties.ToolTip = App.GetDisplayCharater("M01009");
            ButtonRefreshInformation.ToolTip = App.GetDisplayCharater("M01010");

            if (ABoolLanguageChange) { ResetTreeViewItemHeader(); }
        }

        #region 初始化事件
        private void UCFeatureObjects_Loaded(object sender, RoutedEventArgs e)
        {
            ImageFeatureObject.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000006.ico"), UriKind.RelativeOrAbsolute));
            ImageCloseFeatureObject.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000007.ico"), UriKind.RelativeOrAbsolute));

            ImageConnectToServer.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000008.ico"), UriKind.RelativeOrAbsolute));
            ImageDisconnectToServer.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000009.ico"), UriKind.RelativeOrAbsolute));
            ImageServerProperties.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000010.ico"), UriKind.RelativeOrAbsolute));
            ImageRefreshInformation.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000011.ico"), UriKind.RelativeOrAbsolute));

            ButtonDisconnectToServer.Visibility = Visibility.Collapsed;
            ButtonServerProperties.Visibility = Visibility.Collapsed;
            ButtonRefreshInformation.Visibility = Visibility.Collapsed;

            DisplayElementCharacters(false);
        }

        private void ImageCloseFeatureObject_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IOperationEvent != null)
            {
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrElementTag = ((Image)sender).Tag.ToString();
                LEventArgs.ObjSource = (Image)sender;
                IOperationEvent(this, LEventArgs);
            }
        }

        private void ButtonOperationsClicked(object sender, RoutedEventArgs e)
        {
            if (IOperationEvent != null)
            {
                OperationEventArgs LEventArgs = new OperationEventArgs();

                LEventArgs.StrElementTag = ((Button)sender).Tag.ToString();
                LEventArgs.ObjSource = ITreeViewItemCurrentSelected;
                IOperationEvent(this, LEventArgs);
            }
        }

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
                    OperationEventArgs LEventArgs = new OperationEventArgs();
                    LEventArgs.StrElementTag = "TSCH";
                    LEventArgs.ObjSource = ITreeViewItemCurrentSelected;
                    LEventArgs.AppenObjeSource3 = "TV";
                    IOperationEvent(this, LEventArgs);
                }
            }
            catch { }
        }
        #endregion
        
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
            string LStrItemNameFull = string.Empty;
            string LStrItemNameLeft6 = string.Empty;
            string LStrHeader = string.Empty;

            try
            {
                LStrItemNameFull = ATreeViewItem.Name;
                LStrItemNameLeft6 = LStrItemNameFull.Substring(0, 6);

                if (ATreeViewItem.Header != null) { LStrHeader = ATreeViewItem.Header.ToString(); }

                #region 跟节点
                if (LStrItemNameLeft6 == "TVI001")
                {
                    List<string> LListStrConnectedInfo = ATreeViewItem.Tag as List<string>;
                    LStrHeader = LListStrConnectedInfo[0] + " ( " + LListStrConnectedInfo[2] + " )";
                }
                #endregion

                #region 数据库根节点
                if (LStrItemNameLeft6 == "TVI101")
                {
                    LStrHeader = App.GetDisplayCharater("U01001");
                }
                #endregion

                #region 数据库子节点
                if (LStrItemNameLeft6 == "TVI102")
                {
                    string LStrItemTag = ATreeViewItem.Tag.ToString();
                    if (LStrItemTag == "0")
                    {
                        LStrHeader = "UMPDataDB";
                        ATreeViewItem.ToolTip = App.GetDisplayCharater("U01002");
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000020.png");
                    }
                    if (LStrItemTag == "1")
                    {
                        LStrHeader = IListDataSetServerParameters[0].Tables[0].Rows[0]["NameService"].ToString();
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000019.png");
                    }
                    if (LStrItemTag == "2")
                    {
                        LStrHeader = IListDataSetServerParameters[0].Tables[0].Rows[0]["NameService"].ToString();
                        ATreeViewItem.ToolTip = App.GetDisplayCharater("U01003");
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000021.png");
                    }
                    if (LStrItemTag == "3")
                    {
                        LStrHeader = IListDataSetServerParameters[0].Tables[0].Rows[0]["NameService"].ToString();
                        ATreeViewItem.ToolTip = App.GetDisplayCharater("U01004");
                        TreeViewItemProps.SetItemImageName(ATreeViewItem, App.GStrApplicationDirectory + @"\Images\00000020.png");
                    }
                }
                #endregion

                #region 支持的语言
                if (LStrItemNameLeft6 == "TVI201")
                {
                    LStrHeader = App.GetDisplayCharater("U01005");
                }
                #endregion

                #region 在线用户节点
                if (LStrItemNameLeft6 == "TVI501")
                {
                    LStrHeader = App.GetDisplayCharater("U01006");
                }
                #endregion

                #region 离线语言包文件
                if (LStrItemNameLeft6 == "TVI401")
                {
                    LStrHeader = App.GetDisplayCharater("U01007");
                }
                #endregion

                #region 第三方根节点
                if (LStrItemNameLeft6 == "TVI601")
                {
                    LStrHeader = App.GetDisplayCharater("U01008");
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

        #region 连接应用服务器后，在该对象中显示相关功能
        public void AddApplicationServer2TreeView(List<string> AListStrConnectedInfo, List<DataSet> AListDataSetServerParameters)
        {
            bool LBoolConnectedDB = false;
            string LStrVerificationCode104 = string.Empty;

            try
            {
                #region 将参数写入到实例变量中
                IListStrConnectedInfo.Clear();
                foreach (string LStrSingleInfo in AListStrConnectedInfo) { IListStrConnectedInfo.Add(LStrSingleInfo); }
                foreach (DataSet LDataSetSingle in AListDataSetServerParameters) { IListDataSetServerParameters.Add(LDataSetSingle); }
                #endregion

                #region 加入根节点
                TreeViewItem LTreeViewItemRoot = new TreeViewItem();
                LTreeViewItemRoot.DataContext = IListDataSetServerParameters;
                LTreeViewItemRoot.Tag = IListStrConnectedInfo;
                LTreeViewItemRoot.Name = "TVI001";
                TreeViewItemProps.SetItemImageName(LTreeViewItemRoot, App.GStrApplicationDirectory + @"\Images\00000018.ico");
                TreeViewServerObjects.Items.Add(LTreeViewItemRoot);
                #endregion

                AppendOfflineFileTranslation();

                #region 加入数据库节点
                TreeViewItem LTreeViewItemDatabase = new TreeViewItem();
                
                LTreeViewItemDatabase.DataContext = AListDataSetServerParameters[0].Tables[0];
                LTreeViewItemDatabase.Name = "TVI101";
                TreeViewItemProps.SetItemImageName(LTreeViewItemDatabase, App.GStrApplicationDirectory + @"\Images\00000022.ico");
                LTreeViewItemRoot.Items.Add(LTreeViewItemDatabase);

                TreeViewItem LTreeViewItemDbTarget = new TreeViewItem();
                string LStrDBTreeViewItemName = "TVI102";
                string LStrCanConnect = string.Empty;
                string LStrDatabaseVersion = string.Empty;
                string LStrDbNameService = string.Empty;

                if (AListDataSetServerParameters[0].Tables[0].Rows.Count == 0)
                {
                    //数据库未创建
                    LStrDBTreeViewItemName += "0";
                    LTreeViewItemDbTarget.Tag = "0";
                }
                else
                {
                    LTreeViewItemDatabase.DataContext = AListDataSetServerParameters[0].Tables[0].Rows[0];

                    LStrCanConnect = AListDataSetServerParameters[0].Tables[0].Rows[0]["CanConnect"].ToString();
                    LStrDatabaseVersion = AListDataSetServerParameters[0].Tables[0].Rows[0]["DatabaseVersion"].ToString();
                    if (LStrCanConnect == "1")
                    {
                        if (LStrDatabaseVersion != "0.00.000")
                        {
                            //数据库正常
                            LStrDBTreeViewItemName += "1";
                            LTreeViewItemDbTarget.Tag = "1";
                            LBoolConnectedDB = true;
                        }
                        else
                        {
                            //数据库不合法
                            LStrDBTreeViewItemName += "2";
                            LTreeViewItemDbTarget.Tag = "2";
                        }
                    }
                    else
                    {
                        //数据库不能成功连接
                        LStrDBTreeViewItemName += "3";
                        LTreeViewItemDbTarget.Tag = "3";
                    }
                }
                LTreeViewItemDbTarget.Name = LStrDBTreeViewItemName;
                LTreeViewItemDatabase.Items.Add(LTreeViewItemDbTarget);
                LTreeViewItemDatabase.IsExpanded = true;
                #endregion

                #region 如果数据连接成功加入语言包节点
                if (LBoolConnectedDB)
                {
                    string LStrLanguageID = string.Empty;
                    string LStrLanguageName = string.Empty;
                    string LStrIsOpened = string.Empty;

                    LStrVerificationCode104 = EncryptionAndDescryption.CreateVerificationCode(104);

                    TreeViewItem LTreeViewItemLanguages = new TreeViewItem();
                    LTreeViewItemLanguages.DataContext = IListDataSetServerParameters[1];
                    LTreeViewItemLanguages.Name = "TVI201";
                    TreeViewItemProps.SetItemImageName(LTreeViewItemLanguages, App.GStrApplicationDirectory + @"\Images\00000023.ico");
                    LTreeViewItemRoot.Items.Add(LTreeViewItemLanguages);

                    foreach (DataRow LDataRowSingleLanguage in IListDataSetServerParameters[1].Tables[0].Rows)
                    {
                        LStrLanguageID = LDataRowSingleLanguage["C001"].ToString();
                        LStrLanguageName = LDataRowSingleLanguage["C003"].ToString();
                        LStrIsOpened = LDataRowSingleLanguage["C006"].ToString();
                        LStrLanguageName = EncryptionAndDescryption.EncryptDecryptString(LStrLanguageName, LStrVerificationCode104, 104);
                        TreeViewItem LTreeViewItemSingleLanguage = new TreeViewItem();
                        LTreeViewItemSingleLanguage.Name = "TVI2020" + LStrIsOpened;       //TVI202 + '是否已经从数据库中读取数据' + '是否已经启用'
                        LTreeViewItemSingleLanguage.Header = "( " + LStrLanguageID + " ) " + LStrLanguageName;
                        LTreeViewItemSingleLanguage.Tag = LStrLanguageID;      
                        if (LStrIsOpened == "1")
                        {
                            TreeViewItemProps.SetItemImageName(LTreeViewItemSingleLanguage, App.GStrApplicationDirectory + @"\Images\00000024.ico");
                        }
                        else
                        {
                            TreeViewItemProps.SetItemImageName(LTreeViewItemSingleLanguage, App.GStrApplicationDirectory + @"\Images\00000025.ico");
                        }
                        LTreeViewItemLanguages.Items.Add(LTreeViewItemSingleLanguage);
                    }
                    LTreeViewItemLanguages.IsExpanded = true;
                }
                #endregion

                ResetTreeViewItemHeader();

                LTreeViewItemRoot.IsExpanded = true;

                TreeViewServerObjects.Focus();
                LTreeViewItemRoot.IsSelected = true;

                ButtonConnectToServer.Visibility = Visibility.Collapsed;
                ButtonDisconnectToServer.Visibility = Visibility.Visible;
                ButtonServerProperties.Visibility = Visibility.Visible;
                ButtonRefreshInformation.Visibility = Visibility.Visible;

                

                if (LBoolConnectedDB)
                {
                    IBackgroundWorkerA = new BackgroundWorker();
                    IBackgroundWorkerA.RunWorkerCompleted += IBackgroundWorkerA_RunWorkerCompleted;
                    IBackgroundWorkerA.DoWork += IBackgroundWorkerA_DoWork;
                    IBackgroundWorkerA.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("AddApplicationServer2TreeView()\n\n" + ex.Message);
                if (IBackgroundWorkerA != null)
                {
                    IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
                }
            }
        }

        private void IBackgroundWorkerA_DoWork(object sender, DoWorkEventArgs e)
        {
        }

        private void IBackgroundWorkerA_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (IBackgroundWorkerA != null)
            {
                IBackgroundWorkerA.Dispose(); IBackgroundWorkerA = null;
            }
            LoadOtherManagementUnit();
        }

        private void LoadOtherManagementUnit()
        {
            if (IOperationEvent != null)
            {
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrElementTag = "LOMU";
                IOperationEvent(this, LEventArgs);
            }
        }

        #endregion

        #region 增加离线文件翻译/矫正
        /// <summary>
        /// 增加离线文件翻译/矫正
        /// </summary>
        private void AppendOfflineFileTranslation()
        {
            try
            {
                TreeViewItem LTreeViewItemOfflineFile = new TreeViewItem();
                LTreeViewItemOfflineFile.Name = "TVI401";
                LTreeViewItemOfflineFile.DataContext = null;
                TreeViewItemProps.SetItemImageName(LTreeViewItemOfflineFile, App.GStrApplicationDirectory + @"\Images\00000043.ico");
                TreeViewItem LTreeViewItemRoot = TreeViewServerObjects.Items[0] as TreeViewItem;
                LTreeViewItemRoot.Items.Add(LTreeViewItemOfflineFile);
            }
            catch { }
        }
        #endregion

        #region 增加其他管理项
        public void AppendOtherManagementUnit(List<DataSet> AListDataSetOtherData)
        {
            string LStrRentToken = string.Empty, LStrRentName = string.Empty;

            try
            {
                #region 将参数写入到实例变量中
                foreach (DataSet LDataSetSingle in AListDataSetOtherData) { IListDataSetServerParameters.Add(LDataSetSingle); }
                #endregion

                #region 加入在线用户节点
                TreeViewItem LTreeViewItemOnlineUser = new TreeViewItem();
                LTreeViewItemOnlineUser.DataContext = AListDataSetOtherData[0].Tables[0];
                LTreeViewItemOnlineUser.Name = "TVI501";
                TreeViewItemProps.SetItemImageName(LTreeViewItemOnlineUser, App.GStrApplicationDirectory + @"\Images\00000040.ico");
                TreeViewItem LTreeViewItemRoot = TreeViewServerObjects.Items[0] as TreeViewItem;
                LTreeViewItemRoot.Items.Add(LTreeViewItemOnlineUser);
                #endregion

                #region 加入在线用户下的租户节点
                foreach (DataTable LDataTableSingleRent in AListDataSetOtherData[1].Tables)
                {
                    LStrRentToken = LDataTableSingleRent.TableName.Substring(6);
                    DataRow[] LDataRowRents = AListDataSetOtherData[0].Tables[0].Select("C021 = '" + LStrRentToken + "'");
                    LStrRentName = LDataRowRents[0]["C002"].ToString();
                    TreeViewItem LTreeViewItemSingleRent = new TreeViewItem();
                    LTreeViewItemSingleRent.DataContext = LDataTableSingleRent;
                    LTreeViewItemSingleRent.Name = "TVI502";
                    LTreeViewItemSingleRent.Header = LStrRentName + " (" + LDataTableSingleRent.Rows.Count.ToString() + ")";
                    LTreeViewItemSingleRent.Tag = LDataTableSingleRent.TableName;
                    TreeViewItemProps.SetItemImageName(LTreeViewItemSingleRent, App.GStrApplicationDirectory + @"\Images\00000041.ico");
                    LTreeViewItemOnlineUser.Items.Add(LTreeViewItemSingleRent);
                }
                #endregion

                AppendThirdPartyApplications(AListDataSetOtherData);

                LTreeViewItemOnlineUser.IsExpanded = true;
                DisplayObjectCharactersTreeViewItem(LTreeViewItemOnlineUser);
            }
            catch { }
        }

        private void AppendThirdPartyApplications(List<DataSet> AListDataSetOtherData)
        {
            try
            {
                TreeViewItem LTreeViewItemThirdPartyApps = new TreeViewItem();
                LTreeViewItemThirdPartyApps.DataContext = AListDataSetOtherData[2].Tables[0];
                LTreeViewItemThirdPartyApps.Name = "TVI601";
                TreeViewItemProps.SetItemImageName(LTreeViewItemThirdPartyApps, App.GStrApplicationDirectory + @"\Images\00000046.ico");
                TreeViewItem LTreeViewItemRoot = TreeViewServerObjects.Items[0] as TreeViewItem;
                LTreeViewItemRoot.Items.Add(LTreeViewItemThirdPartyApps);

                foreach (DataRow LDataRowSingleApp in AListDataSetOtherData[2].Tables[0].Rows)
                {
                    TreeViewItem LTreeViewItemSingleApp = new TreeViewItem();
                    LTreeViewItemSingleApp.DataContext = LDataRowSingleApp;
                    LTreeViewItemSingleApp.Name = "TVI602";
                    LTreeViewItemSingleApp.Header = LDataRowSingleApp[0].ToString();
                    //LTreeViewItemSingleApp.Tag = LDataRowSingleApp;
                    TreeViewItemProps.SetItemImageName(LTreeViewItemSingleApp, App.GStrApplicationDirectory + @"\Images\00000045.ico");
                    LTreeViewItemThirdPartyApps.Items.Add(LTreeViewItemSingleApp);
                }
                LTreeViewItemThirdPartyApps.IsExpanded = true;
                DisplayObjectCharactersTreeViewItem(LTreeViewItemThirdPartyApps);
            }
            catch { }
        }

        #endregion

        #region 刷新在线用户
        public void RefreshOnlineUser(DataTable ADataTableOnlineUser)
        {
            try
            {
                for (int LIntLoopOnlineUserTable = 0; LIntLoopOnlineUserTable < IListDataSetServerParameters[3].Tables.Count; LIntLoopOnlineUserTable++)
                {
                    if (IListDataSetServerParameters[3].Tables[LIntLoopOnlineUserTable].TableName == ADataTableOnlineUser.TableName)
                    {
                        IListDataSetServerParameters[3].Tables.RemoveAt(LIntLoopOnlineUserTable);
                        IListDataSetServerParameters[3].Tables.Add(ADataTableOnlineUser);
                        break;
                    }
                }

                string LStrRentToken = ADataTableOnlineUser.TableName.Substring(6);
                DataRow[] LDataRowRents = IListDataSetServerParameters[2].Tables[0].Select("C021 = '" + LStrRentToken + "'");
                string LStrRentName = LDataRowRents[0]["C002"].ToString();
                ITreeViewItemCurrentSelected.Header = LStrRentName + " (" + ADataTableOnlineUser.Rows.Count.ToString() + ")";
                ITreeViewItemCurrentSelected.DataContext = ADataTableOnlineUser;

                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrElementTag = "TSCH";
                LEventArgs.ObjSource = ITreeViewItemCurrentSelected;
                LEventArgs.AppenObjeSource3 = "TV";
                IOperationEvent(this, LEventArgs);
            }
            catch { }
        }
        #endregion

        #region 断开连接的应用服务器，从功能列表中移除
        public void RemoveConnectedServerFromTreeView()
        {
            try
            {
                ButtonConnectToServer.Visibility = Visibility.Visible;
                ButtonDisconnectToServer.Visibility = Visibility.Collapsed;
                ButtonServerProperties.Visibility = Visibility.Collapsed;
                ButtonRefreshInformation.Visibility = Visibility.Collapsed;

                TreeViewItem LTreeViewItemCurrentSelected = ITreeViewItemCurrentSelected;
                if (LTreeViewItemCurrentSelected == null) { return; }
                while (LTreeViewItemCurrentSelected.Parent.GetType() == typeof(TreeViewItem))
                {
                    LTreeViewItemCurrentSelected = (TreeViewItem)LTreeViewItemCurrentSelected.Parent;
                }
                if (LTreeViewItemCurrentSelected == null) { return; }

                TreeViewServerObjects.Items.Remove(LTreeViewItemCurrentSelected);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage("RemoveConnectedServerFromTreeView()\n\n" + ex.Message);
            }
        }
        #endregion

    }
}
