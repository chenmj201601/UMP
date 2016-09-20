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

namespace UMP.MAMT.CertificateSetting
{
    public partial class UCCertificateListView : UserControl, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        private List<object> IListObjSource = new List<object>();

        public UCCertificateListView()
        {
            InitializeComponent();
            this.Loaded += UCCertificateListView_Loaded;
        }

        private void UCCertificateListView_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayElementCharacters(false);
        }

        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelStoreName.Text = App.GetDisplayCharater("M01038");
            TextBoxInstalled.Text = App.GetDisplayCharater("M01039");
            LabelStoreMy.Text = App.GetDisplayCharater("M01040");
            LabelStoreRoot.Text = App.GetDisplayCharater("M01041");
            LabelTrustedPublisher.Text = App.GetDisplayCharater("M01042");
        }

        public void ShowObjectContainChildObject(object AObjectSource, bool ABoollanguageChanage)
        {
            string LStrStoreName = string.Empty;
            string LStrInstalled = string.Empty;
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

            DataTable LDataTableCertificate = LTreeViewItemCurrent.DataContext as DataTable;
            foreach (DataRow LDataRowSingleStoreName in LDataTableCertificate.Rows)
            {
                LStrStoreName = LDataRowSingleStoreName["StoreName"].ToString();
                LStrInstalled = LDataRowSingleStoreName["IsInstalled"].ToString();
                LStrInstalled = App.GetConvertedData("CerInstalled" + LStrInstalled);
                if (LStrStoreName == "My") { TextBoxStoreMy.Text = LStrInstalled; }
                if (LStrStoreName == "Root") { TextBoxStoreRoot.Text = LStrInstalled; }
                if (LStrStoreName == "TrustedPublisher") { TextBoxTrustedPublisher.Text = LStrInstalled; }

            }
        }
    }
}
