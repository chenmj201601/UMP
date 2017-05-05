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

namespace UMP.MAMT.LicenseServer
{
    public partial class UCLicenseServerView : UserControl, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        private List<object> IListObjSource = new List<object>();

        public UCLicenseServerView()
        {
            InitializeComponent();
            this.Loaded += UCLicenseServerView_Loaded;
        }

        private void UCLicenseServerView_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayElementCharacters(false);
        }

        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelMainServerTitle.Text = App.GetDisplayCharater("M01097");
            LabelMStatus.Text = App.GetDisplayCharater("M01099");
            LabelMIsEnabled.Text = App.GetDisplayCharater("M01101");
            LabelMHost.Text = App.GetDisplayCharater("M01102");
            LabelMPort.Text = App.GetDisplayCharater("M01103");

            LabelSpareServerTitle.Text = App.GetDisplayCharater("M01098");
            LabelSStatus.Text = App.GetDisplayCharater("M01099");
            LabelSIsEnabled.Text = App.GetDisplayCharater("M01101");
            LabelSHost.Text = App.GetDisplayCharater("M01102");
            LabelSPort.Text = App.GetDisplayCharater("M01103");
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

            DataTable LDataTableLicenseServer = LTreeViewItemCurrent.DataContext as DataTable;

            string LStrMainSpare = string.Empty;
            string LStrIsEnabled = string.Empty;
            string LStrServerHost = string.Empty;
            string LStrServerPort = string.Empty, LStrOtherInfo = string.Empty;

            foreach (DataRow LDataRowSingleLicenseService in LDataTableLicenseServer.Rows)
            {
                LStrMainSpare = LDataRowSingleLicenseService["MainSpare"].ToString();
                LStrIsEnabled = LDataRowSingleLicenseService["IsEnabled"].ToString();
                LStrServerHost = LDataRowSingleLicenseService["ServerHost"].ToString();
                LStrServerPort = LDataRowSingleLicenseService["ServerPort"].ToString();
                LStrOtherInfo = LDataRowSingleLicenseService["OtherInfo"].ToString();

                if (LStrMainSpare == "1")
                {
                    TextBoxMStatus.Text = App.GetConvertedData("LSStatus" + LStrOtherInfo);
                    TextBoxMIsEnabled.Text = App.GetConvertedData("LSIsEnable" + LStrIsEnabled);
                    TextBoxMHost.Text = LStrServerHost;
                    TextBoxMPort.Text = LStrServerPort;
                    if (LStrOtherInfo == "0")
                    {
                        LabelMainServerTitle.Foreground = Brushes.Red;
                    }
                    else
                    {
                        LabelMainServerTitle.Foreground = Brushes.LawnGreen;
                    }
                }
                else
                {
                    TextBoxSStatus.Text = App.GetConvertedData("LSStatus" + LStrOtherInfo);
                    TextBoxSIsEnabled.Text = App.GetConvertedData("LSIsEnable" + LStrIsEnabled);
                    TextBoxSHost.Text = LStrServerHost;
                    TextBoxSPort.Text = LStrServerPort;
                    if (LStrOtherInfo == "0")
                    {
                        LabelSpareServerTitle.Foreground = Brushes.Red;
                    }
                    else
                    {
                        LabelSpareServerTitle.Foreground = Brushes.LawnGreen;
                    }
                }
            }

            

            //LabelLogicPartitionTitle.Text = string.Format(App.GetDisplayCharater("M01075"), LTreeViewItemCurrent.Header.ToString().Trim());
            //TextBoxLPName.Text = "LP_" + LStrTablename.Substring(2) + "." + LStrColumnName;
            //TextBoxLPDepent.Text = App.GetConvertedData("LPC" + LStrAlias + "." + LStrColumnName);
            //TextBoxLPType.Text = App.GetDisplayCharater("M01082");
            //TextBoxIsEnabled.Text = App.GetConvertedData("LPStatus" + LStrS01);

            //if (LStrS00 == "1")
            //{
            //    TextBoxFirstSetTime.Text = (DateTime.Parse(LStrS02)).ToLocalTime().ToString("G");
            //    TextBoxLastSetTime.Text = (DateTime.Parse(LStrS03)).ToLocalTime().ToString("G");
            //}
        }
    }
}
