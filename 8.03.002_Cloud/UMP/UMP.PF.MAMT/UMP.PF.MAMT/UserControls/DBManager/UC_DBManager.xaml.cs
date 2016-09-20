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
using UMP.PF.MAMT.UserControls.DBManager;

namespace UMP.PF.MAMT.UserControls
{
    /// <summary>
    /// UC_DBManager.xaml 的交互逻辑
    /// </summary>
    public partial class UC_DBManager : UserControl
    {
        public UC_DBManager()
        {
            InitializeComponent();
            this.Loaded += UC_DBManager_Loaded;
        }

        void UC_DBManager_Loaded(object sender, RoutedEventArgs e)
        {
            imgServerSource.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000010.ico"), UriKind.RelativeOrAbsolute));
            if (App.GLstConnectedServers.Count > 0)
            {
                TreeView tree;
                for (int i = 0; i < App.GLstConnectedServers.Count; i++)
                {
                    tree = new TreeView();
                    tree.SelectedItemChanged+=tree_SelectedItemChanged;
                    TreeViewItem item = new TreeViewItem();
                    item.IsExpanded = true;
                    item.Header = App.GLstConnectedServers[i].Host;
                    item.DataContext = "S-" + App.GLstConnectedServers[i].Host;
                    item.Tag = App.GLstConnectedServers[i];
                    TreeViewItemProps.SetItemImageName(item, System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000017.ico"));
                    item.SetResourceReference(TreeViewItem.StyleProperty, "TreeViewItemWithImg");
                    tree.Items.Add(item);
                    BindDBInUMPServer(item);
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
        /// tree的SelectedItemChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            TreeViewItem item = (TreeViewItem)((TreeView)sender).SelectedItem;
            string strContextValue = item.DataContext.ToString();
            string strItemType = strContextValue.Substring(0, strContextValue.IndexOf("-"));
            switch(strItemType)
            {
                 //如果选择的是UMP服务器
                case "S":
                    
                    break;
                //如果选择的是数据库
                case "DB":
                    UC_DataBaseOperationInDBM uc_DB = new UC_DataBaseOperationInDBM();
                    spOperator.Children.Add(uc_DB);
                    break;
            }
        }

        /// <summary>
        /// 绑定UMP服务器所配置的数据库
        /// </summary>
        /// <param name="selectedItem"></param>
        void BindDBInUMPServer(TreeViewItem root)
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
    }
}
