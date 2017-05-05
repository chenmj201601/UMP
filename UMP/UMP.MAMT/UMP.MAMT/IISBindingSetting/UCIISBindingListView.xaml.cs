using System;
using System.Collections.Generic;
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

namespace UMP.MAMT.IISBindingSetting
{
    public partial class UCIISBindingListView : UserControl,MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        private List<object> IListObjSource = new List<object>();

        public UCIISBindingListView()
        {
            InitializeComponent();
            this.Loaded += UCIISBindingListView_Loaded;
        }

        private void UCIISBindingListView_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayElementCharacters(false);
        }

        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            DisplayElementObjectCharacters.DisplayUIObjectCharacters(ListViewIISBindingProtol, ListViewIISBindingProtol);
        }

        public void ShowObjectContainChildObject(object AObjectSource, bool ABoollanguageChanage)
        {
            if (!ABoollanguageChanage)
            {
                TreeViewItem LTreeViewItemCurrent = AObjectSource as TreeViewItem;
                DataTable LDataTableIISBinding = LTreeViewItemCurrent.DataContext as DataTable;
                string LStrProtol = string.Empty;

                IListObjSource.Clear();
                IListObjSource.Add(AObjectSource);
                ListViewIISBindingProtol.Items.Clear();
                foreach (DataRow LDataRowSingleBinding in LDataTableIISBinding.Rows)
                {
                    LStrProtol = LDataRowSingleBinding["Protocol"].ToString();
                    if (LStrProtol == "net.tcp") { continue; }
                    ListViewItem LListViewItemBindings = new ListViewItem();
                    LListViewItemBindings.Height = 26;
                    LListViewItemBindings.Content = new IISBindingViewColumnDefine(LDataRowSingleBinding["Protocol"].ToString(), LDataRowSingleBinding["IPAddress"].ToString(), LDataRowSingleBinding["BindInfo"].ToString(), LDataRowSingleBinding["Used"].ToString());
                    ListViewIISBindingProtol.Items.Add(LListViewItemBindings);
                }
            }
            else
            {
                TreeViewItem LTreeViewItemCurrent = IListObjSource[0] as TreeViewItem;
                DataTable LDataTableIISBinding = LTreeViewItemCurrent.DataContext as DataTable;

                string LStrProtol = string.Empty;
                ListViewIISBindingProtol.Items.Clear();
                foreach (DataRow LDataRowSingleBinding in LDataTableIISBinding.Rows)
                {
                    LStrProtol = LDataRowSingleBinding["Protocol"].ToString();
                    if (LStrProtol == "net.tcp") { continue; }
                    ListViewItem LListViewItemBindings = new ListViewItem();
                    LListViewItemBindings.Height = 26;
                    LListViewItemBindings.Content = new IISBindingViewColumnDefine(LDataRowSingleBinding["Protocol"].ToString(), LDataRowSingleBinding["IPAddress"].ToString(), LDataRowSingleBinding["BindInfo"].ToString(), LDataRowSingleBinding["Used"].ToString());
                    ListViewIISBindingProtol.Items.Add(LListViewItemBindings);
                }
            }
        }
    }
}
