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
using UMP.PF.MAMT.UserControls;
using UMP.PF.MAMT.WCF_LanPackOperation;
using System.ServiceModel;
using System.Data;

namespace UMP.PF.MAMT.UserControls
{
    /// <summary>
    /// UC_LanManager.xaml 的交互逻辑
    /// </summary>
    public partial class UC_LanManager : UserControl
    {
        //DataTable dtAllLans = new DataTable();
        List<LanguageInfo> lstAllLans = null;

        public UC_LanManager()
        {
            InitializeComponent();
            this.Loaded += UC_LanManager_Loaded;
        }

        void UC_LanManager_Loaded(object sender, RoutedEventArgs e)
        {
            InitControl();
        }

        /// <summary>
        /// 初始化界面上的空间
        /// </summary>
        public void InitControl()
        {
            imgServerSource.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000010.ico"), UriKind.RelativeOrAbsolute));
            if (App.GLstConnectedServers.Count > 0)
            {
                TreeView tree;
                for (int i = 0; i < App.GLstConnectedServers.Count; i++)
                {
                    tree = new TreeView();
                    tree.SelectedItemChanged += tree_SelectedItemChanged;
                    TreeViewItem item = new TreeViewItem();
                    item.IsExpanded = true;
                    item.Header = App.GLstConnectedServers[i].Host;
                    item.DataContext = "S-" + App.GLstConnectedServers[i].Host;
                    item.Tag = App.GLstConnectedServers[i];
                    TreeViewItemProps.SetItemImageName(item, System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000017.ico"));
                    item.SetResourceReference(TreeViewItem.StyleProperty, "TreeViewItemWithImg");
                    tree.Items.Add(item);
                    BindTree(item);
                    //如果已经存在树 则删掉再加 用于刷新
                    if (spContent.Children.Count >= 3)
                    {
                        spContent.Children.RemoveAt(2);
                    }
                    spContent.Children.Add(tree);
                    tree.BorderThickness = new Thickness(0);
                    tree.Background = Brushes.Transparent;

                }
            }

        }

        /// <summary>
        /// 根据传入的根节点绑定子节点
        /// </summary>
        /// <param name="root"></param>
        void BindTree(TreeViewItem root)
        {
            ServerInfomation server = root.Tag as ServerInfomation;
            List<DBInfo> lstDBs = ServerConfigOperationInServer.GetAllDBs(server.Host, server.Port);
            TreeViewItem dbItem;
            string strDBType = string.Empty;
            for (int i = 0; i < lstDBs.Count; i++)
            {
                dbItem = new TreeViewItem();
                switch (lstDBs[i].DbType)
                {
                    case (int)Enums.DBType.MSSQL:
                        strDBType = "MSSQL";
                        break;
                    case (int)Enums.DBType.MySql:
                        strDBType = "MySql";
                        break;
                    case (int)Enums.DBType.Oracle:
                        strDBType = "Oracle";
                        break;
                }
                dbItem.Header = lstDBs[i].Host + " (" + strDBType + ")";
                dbItem.DataContext = "DB-" + lstDBs[i].Host;
                dbItem.Tag = lstDBs[i];
                TreeViewItemProps.SetItemImageName(dbItem, System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000015.ico"));
                dbItem.SetResourceReference(TreeViewItem.StyleProperty, "TreeViewItemWithImg");
                root.Items.Add(dbItem);
            }
        }

        /// <summary>
        /// tree的SelectedItemChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = (TreeViewItem)((TreeView)sender).SelectedItem;
            string strContextValue = item.DataContext.ToString();
            string strItemType = strContextValue.Substring(0, strContextValue.IndexOf("-"));
            switch (strItemType)
            {
                case "S":
                    Server_Selected();
                    break;
                case "DB":
                    dbItem_Selected(item);
                    break;
                case "Lan":
                    Language_Selected(item);
                    break;
            }
        }

        /// <summary>
        /// 选择数据库事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dbItem_Selected(TreeViewItem selectedItem)
        {
            try
            {
                UC_DBOperation uc_db = new UC_DBOperation(this);
                spOperator.Children.Clear();
                spOperator.Children.Add(uc_db);
                UC_DBOpeartionDefault uc_DBOpeartionDefault = new UC_DBOpeartionDefault(this);
                spOperator.Children.Add(uc_DBOpeartionDefault);
                App.GCurrentDBServer = selectedItem.Tag as DBInfo;

                ////没有打开过 才加载语言种类
                //if (selectedItem.Items.Count <= 0)
                //{
                selectedItem.Items.Clear();
                InitTreeLanguageItem(selectedItem);
                //}
                TreeViewItemProps.SetItemImageName(selectedItem, System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000015.ico"));

                DataTable dt = GetAllLanguages(selectedItem);
                UC_LanguageData uc_LanData = new UC_LanguageData(dt, this);
                dpData.Children.Clear();
                dpData.Children.Add(uc_LanData);
                //lstAllLans = uc_LanData.lstViewrSource;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                   this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 在选择中数据库时 加载语言类型列表
        /// </summary>
        /// <param name="dbItem"></param>
        private void InitTreeLanguageItem(TreeViewItem dbItem)
        {
            try
            {
                TreeViewItem parentItem = dbItem.Parent as TreeViewItem;
                ServerInfomation serverInfo = parentItem.Tag as ServerInfomation;
                App.GCurrentUmpServer = serverInfo;
                DBInfo dbInfo = dbItem.Tag as DBInfo;
                ReturnResult result = AboutLanguagesInServer.WCFOperationMthodA("Http", serverInfo.Host, serverInfo.Port
                    , 1, dbInfo);
                if (result.BoolReturn)
                {
                    if (result.DataSetReturn.Tables.Count > 0)
                    {
                        DataTable dt = result.DataSetReturn.Tables[0];
                        TreeViewItem item;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            item = new TreeViewItem();
                            item.Header = dt.Rows[i]["C005"].ToString();
                            item.Tag = dt.Rows[i];
                            item.DataContext = "Lan-" + dt.Rows[i]["C005"].ToString();
                            TreeViewItemProps.SetItemImageName(item, System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000027.ico"));
                            item.SetResourceReference(TreeViewItem.StyleProperty, "TreeViewItemWithImg");
                            dbItem.Items.Add(item);
                        }
                    }
                    dbItem.IsExpanded = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                       this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 通过wcf获得所有的语言信息
        /// </summary>
        /// <param name="dbItem"></param>
        /// <returns></returns>
        private DataTable GetAllLanguages(TreeViewItem dbItem)
        {
            DataTable dt = new DataTable();
            try
            {
                TreeViewItem parentItem = dbItem.Parent as TreeViewItem;
                ServerInfomation serverInfo = parentItem.Tag as ServerInfomation;
                DBInfo dbInfo = dbItem.Tag as DBInfo;
                ReturnResult result = AboutLanguagesInServer.WCFOperationMthodA("HTTP", serverInfo.Host, serverInfo.Port, 2, dbInfo);
                if (result.BoolReturn)
                {
                    if (result.DataSetReturn.Tables.Count > 0)
                    {
                        dt = result.DataSetReturn.Tables[0];
                    }
                }
                else
                {
                    MessageBox.Show(result.StringReturn,
                       this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                       this.TryFindResource("ErrorMsgTitle").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return dt;
        }

        /// <summary>
        /// 选择服务器事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Server_Selected()
        {
            UC_ServerOperation uc_Server = new UC_ServerOperation(this);
            spOperator.Children.Clear();
            spOperator.Children.Add(uc_Server);
        }

        /// <summary>
        /// 选择语言事件
        /// </summary>
        void Language_Selected(TreeViewItem selectedItem)
        {
            UC_LanguageData uc_LanData = dpData.Children[0] as UC_LanguageData;
            DataRow row = selectedItem.Tag as DataRow;
            string strLanCode = row["C002"].ToString();
            lstAllLans = uc_LanData.lstViewrSource;
            List<LanguageInfo> lstSingleLan = lstAllLans.Where(p => p.LanguageCode == strLanCode).ToList();

            if (lstSingleLan.Count > 0)
            {
                uc_LanData.lvLanguage.ItemsSource = lstSingleLan;
            }

        }

        void exp_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Expander exp = sender as Expander;
            exp.Background = Brushes.DarkBlue;
            exp.Foreground = Brushes.WhiteSmoke;
        }
    }
}
