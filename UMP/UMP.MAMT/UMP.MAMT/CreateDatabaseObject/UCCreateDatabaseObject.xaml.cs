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

namespace UMP.MAMT.CreateDatabaseObject
{
    public partial class UCCreateDatabaseObject : UserControl
    {
        public CreateDataBaseWindow IPageParent = null;

        public int IIntCurrentObject = 0;

        public UCCreateDatabaseObject()
        {
            InitializeComponent();
            this.Loaded += UCCreateDatabaseObject_Loaded;
        }

        private void UCCreateDatabaseObject_Loaded(object sender, RoutedEventArgs e)
        {
            ShowElementLanguage();
        }

        private void ShowElementLanguage()
        {
            TabItemCreateDabaseObjects.Header = " " + App.GetDisplayCharater("M02061") + " ";
            ObjectNameColumnHeader.Content = " " + App.GetDisplayCharater("M02062");
            ObjectTypeColumnHeader.Content = " " + App.GetDisplayCharater("M02063");
            ObjectVersionColumnHeader.Content = " " + App.GetDisplayCharater("M02064");
            StatusDescColumnHeader.Content = " " + App.GetDisplayCharater("M02065");
        }

        public void ShowWillCreateObjects(DataTable ADataTableCreateObjects)
        {
            IIntCurrentObject = 0;
            ListViewCreateObjectList.Items.Clear();
            foreach (DataRow LDataRowSingleObject in ADataTableCreateObjects.Rows)
            {
                ListViewItem LListViewItemSingleDatabaseObject = new ListViewItem();
                LListViewItemSingleDatabaseObject.Content = new CreateObjectColumnDefine(string.Empty, string.Empty, LDataRowSingleObject);
                LListViewItemSingleDatabaseObject.DataContext = LDataRowSingleObject;
                LListViewItemSingleDatabaseObject.Height = 26;
                ListViewCreateObjectList.Items.Add(LListViewItemSingleDatabaseObject);
            }
        }

        public DataRow GetCreateObject(ref string AStrIsCreated)
        {
            DataRow LDataRowCreateObject = null;

            try
            {
                AStrIsCreated = "0";

                if (IIntCurrentObject >= ListViewCreateObjectList.Items.Count)
                {
                    LDataRowCreateObject = null;
                    return LDataRowCreateObject;
                }
                ListViewCreateObjectList.SelectedIndex = IIntCurrentObject;
                ListViewItem LListViewItemCurrent = ListViewCreateObjectList.SelectedItem as ListViewItem;
                LListViewItemCurrent.BringIntoView();
                LDataRowCreateObject = LListViewItemCurrent.DataContext as DataRow;
                CreateObjectColumnDefine LCreateObjectColumnDefine = LListViewItemCurrent.Content as CreateObjectColumnDefine;
                if (LCreateObjectColumnDefine.BoolIsSuccess) { AStrIsCreated = "1"; }
                IIntCurrentObject += 1;
            }
            catch { LDataRowCreateObject = null; }

            return LDataRowCreateObject;
        }

        public void SetObjectCurrentAction(string AStrActionType, string AStrActionResult)
        {
            ListViewItem LListViewItemCurrent = ListViewCreateObjectList.SelectedItem as ListViewItem;
            DataRow LDataRowCreateObject = LListViewItemCurrent.DataContext as DataRow;
            LListViewItemCurrent.Content = new CreateObjectColumnDefine(AStrActionType, AStrActionResult, LDataRowCreateObject);
        }
    }
}
