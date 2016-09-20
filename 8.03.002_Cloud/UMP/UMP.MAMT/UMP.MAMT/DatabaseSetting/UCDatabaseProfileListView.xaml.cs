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

namespace UMP.MAMT.DatabaseSetting
{
    public partial class UCDatabaseProfileListView : UserControl, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        private List<object> IListObjSource = new List<object>();

        public UCDatabaseProfileListView()
        {
            InitializeComponent();
            this.Loaded += UCDatabaseProfileListView_Loaded;
        }

        private void UCDatabaseProfileListView_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayElementCharacters(false);
        }

        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelProfileTitle.Text = App.GetDisplayCharater("M01055");
            LabelDBType.Text = App.GetDisplayCharater("M01056");
            LabelServerName.Text = App.GetDisplayCharater("M01057");
            LabelServerPort.Text = App.GetDisplayCharater("M01058");
            LabelLoginAccount.Text = App.GetDisplayCharater("M01059");
            LabelLoginPassword.Text = App.GetDisplayCharater("M01060");
        }

        public void ShowObjectContainChildObject(object AObjectSource, bool ABoollanguageChanage)
        {
            TreeViewItem LTreeViewItemCurrent = null;

            if (!ABoollanguageChanage)
            {
                LTreeViewItemCurrent = AObjectSource as TreeViewItem;
                IListObjSource.Clear();
                IListObjSource.Add(AObjectSource);
            }
            else
            {
                LTreeViewItemCurrent = IListObjSource[0] as TreeViewItem;
            }

            DataTable LDataTableDatabaseProfile = LTreeViewItemCurrent.DataContext as DataTable;
            if (LDataTableDatabaseProfile.Rows[0]["DBType"].ToString() == "2")
            {
                LabelServiceName.Text = App.GetDisplayCharater("M01061");
                TextBoxDBType.Text = App.GetDisplayCharater("M01063");
            }
            else
            {
                LabelServiceName.Text = App.GetDisplayCharater("M01062");
                TextBoxDBType.Text = App.GetDisplayCharater("M01064");
            }

            TextBoxServerName.Text = LDataTableDatabaseProfile.Rows[0]["ServerHost"].ToString();
            TextBoxServerPort.Text = LDataTableDatabaseProfile.Rows[0]["ServerPort"].ToString();
            TextBoxLoginAccount.Text = LDataTableDatabaseProfile.Rows[0]["LoginID"].ToString();
            TextBoxServiceName.Text = LDataTableDatabaseProfile.Rows[0]["NameService"].ToString();
        }
    }
}
