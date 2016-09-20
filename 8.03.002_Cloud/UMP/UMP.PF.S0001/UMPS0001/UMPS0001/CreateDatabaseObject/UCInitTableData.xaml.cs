using System;
using System.Collections.Generic;
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
using UMPS0001.WCFService00001;
using VoiceCyber.UMP.Communications;

namespace UMPS0001.CreateDatabaseObject
{
    public partial class UCInitTableData : UserControl
    {
        public Page000001A IPageParent = null;

        private BackgroundWorker IBackgroundWorkerObtainInitTableData = null;
        public DataTable IDataTableInitTablesData = null;
        public int IIntCurrentObject = 0;

        public UCInitTableData()
        {
            InitializeComponent();
            this.Loaded += UCInitTableData_Loaded;
        }

        private void UCInitTableData_Loaded(object sender, RoutedEventArgs e)
        {
            ShowElementLanguage();
        }

        private void ShowElementLanguage()
        {
            TabItemTableDataInitObjects.Header = " " + App.GetDisplayCharater("M02067") + " ";
            TableNameColumnHeader.Content = " " + App.GetDisplayCharater("M02068");
            ObjectVersionColumnHeader.Content = " " + App.GetDisplayCharater("M02071");
            ImportRowsColumnHeader.Content = " " + App.GetDisplayCharater("M02069");
            StatusDescColumnHeader.Content = " " + App.GetDisplayCharater("M02070");
        }

        public void ObtainInitTablesData(string AStrDatabaseVersion)
        {
            try
            {
                IPageParent.ShowWaitProgressBar(true);
                IBackgroundWorkerObtainInitTableData = new BackgroundWorker();
                IBackgroundWorkerObtainInitTableData.RunWorkerCompleted += IBackgroundWorkerObtainInitTableData_RunWorkerCompleted;
                IBackgroundWorkerObtainInitTableData.DoWork += IBackgroundWorkerObtainInitTableData_DoWork;
                IBackgroundWorkerObtainInitTableData.RunWorkerAsync(AStrDatabaseVersion);
            }
            catch
            {
                IPageParent.ShowWaitProgressBar(true);
                if (IBackgroundWorkerObtainInitTableData != null)
                {
                    IBackgroundWorkerObtainInitTableData.Dispose();
                    IBackgroundWorkerObtainInitTableData = null;
                }
            }
        }

        private void IBackgroundWorkerObtainInitTableData_DoWork(object sender, DoWorkEventArgs e)
        {
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            Service00001Client LService00001Client = null;

            string LStrDatabaseVersion = e.Argument as string;
            List<string> LListStrWcfArguments = new List<string>();
            LListStrWcfArguments.Add(LStrDatabaseVersion);
            LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
            LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00001");
            OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
            LService00001Client = new Service00001Client(LBasicHttpBinding, LEndpointAddress);
            LWCFOperationReturn = LService00001Client.OperationMethodA(13, LListStrWcfArguments);
            if (LWCFOperationReturn.BoolReturn)
            {
                IDataTableInitTablesData = LWCFOperationReturn.DataSetReturn.Tables[0];
            }
            else { IDataTableInitTablesData = null; }
            LService00001Client.Close();
            LService00001Client = null;
        }

        private void IBackgroundWorkerObtainInitTableData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string LStrLangID = string.Empty;

            try
            {
                IPageParent.ShowWaitProgressBar(false);

                if (e.Error != null)
                {
                    MessageBox.Show(App.GetDisplayCharater("M02072") + "\n\n" + e.Error.Message, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (IDataTableInitTablesData == null)
                {
                    MessageBox.Show(App.GetDisplayCharater("M02072"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                IIntCurrentObject = 0;
                ListViewInitTableData.Items.Clear();
                foreach (DataRow LDataRowSingleObject in IDataTableInitTablesData.Rows)
                {
                    ListViewItem LListViewItemSingleInitObject = new ListViewItem();
                    LListViewItemSingleInitObject.Content = new InitObjectColumnDefine(string.Empty, string.Empty, LDataRowSingleObject, "");
                    LListViewItemSingleInitObject.DataContext = LDataRowSingleObject;
                    LListViewItemSingleInitObject.Height = 26;
                    ListViewInitTableData.Items.Add(LListViewItemSingleInitObject);
                }
            }
            catch { }
            finally
            {
                IBackgroundWorkerObtainInitTableData.Dispose();
                IBackgroundWorkerObtainInitTableData = null;
            }
        }

        public DataRow GetInitObject(ref string AStrIsInited)
        {
            DataRow LDataRowInitObject = null;

            try
            {
                AStrIsInited = "0";

                if (IIntCurrentObject >= ListViewInitTableData.Items.Count)
                {
                    LDataRowInitObject = null;
                    return LDataRowInitObject;
                }
                ListViewInitTableData.SelectedIndex = IIntCurrentObject;
                ListViewItem LListViewItemCurrent = ListViewInitTableData.SelectedItem as ListViewItem;
                LListViewItemCurrent.BringIntoView();
                LDataRowInitObject = LListViewItemCurrent.DataContext as DataRow;
                InitObjectColumnDefine LCreateObjectColumnDefine = LListViewItemCurrent.Content as InitObjectColumnDefine;
                if (LCreateObjectColumnDefine.BoolIsSuccess) { AStrIsInited = "1"; }
                IIntCurrentObject += 1;
            }
            catch { LDataRowInitObject = null; }

            return LDataRowInitObject;
        }

        public void SetObjectCurrentAction(string AStrActionType, string AStrActionResult, string AStrEffectRows)
        {
            ListViewItem LListViewItemCurrent = ListViewInitTableData.SelectedItem as ListViewItem;
            DataRow LDataRowCreateObject = LListViewItemCurrent.DataContext as DataRow;
            LListViewItemCurrent.Content = new InitObjectColumnDefine(AStrActionType, AStrActionResult, LDataRowCreateObject, AStrEffectRows);
        }
    }
}
