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

namespace UMP.MAMT.LogicPartitionSetting
{
    public partial class UCLogicPartitionView : UserControl, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        private List<object> IListObjSource = new List<object>();

        public UCLogicPartitionView()
        {
            InitializeComponent();
            this.Loaded += UCLogicPartitionView_Loaded;
        }

        private void UCLogicPartitionView_Loaded(object sender, RoutedEventArgs e)
        {
            DisplayElementCharacters(false);
        }

        public void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            //LabelLogicPartitionTitle.Text = App.GetDisplayCharater("M01075");
            LabelLPName.Text = App.GetDisplayCharater("M01076");
            LabelLPDepent.Text = App.GetDisplayCharater("M01077");
            LabelLPType.Text = App.GetDisplayCharater("M01078");
            LabelIsEnabled.Text = App.GetDisplayCharater("M01079");
            LabelFirstSetTime.Text = App.GetDisplayCharater("M01080");
            LabelLastSetTime.Text = App.GetDisplayCharater("M01081");
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

            DataRow LDataRowLogicPartitionSetted = LTreeViewItemCurrent.DataContext as DataRow;
            string LStrAlias = string.Empty;
            string LStrTablename = string.Empty;
            string LStrColumnName = string.Empty;
            string LStrS00 = string.Empty, LStrS01 = string.Empty, LStrS02 = string.Empty, LStrS03 = string.Empty;

            LStrAlias = LDataRowLogicPartitionSetted["Alias"].ToString();
            LStrTablename = LDataRowLogicPartitionSetted["TableName"].ToString();
            LStrColumnName = LDataRowLogicPartitionSetted["ColumnName"].ToString();
            LStrS00 = LDataRowLogicPartitionSetted["S00"].ToString();
            LStrS01 = LDataRowLogicPartitionSetted["S01"].ToString();
            LStrS02 = LDataRowLogicPartitionSetted["S02"].ToString();
            LStrS03 = LDataRowLogicPartitionSetted["S03"].ToString();

            LabelLogicPartitionTitle.Text = string.Format(App.GetDisplayCharater("M01075"), LTreeViewItemCurrent.Header.ToString().Trim());
            TextBoxLPName.Text = "LP_" + LStrTablename.Substring(2) + "." + LStrColumnName;
            TextBoxLPDepent.Text = App.GetConvertedData("LPC" + LStrAlias + "." + LStrColumnName);
            TextBoxLPType.Text = App.GetDisplayCharater("M01082");
            TextBoxIsEnabled.Text = App.GetConvertedData("LPStatus" + LStrS01);

            if (LStrS00 == "1")
            {
                TextBoxFirstSetTime.Text = (DateTime.Parse(LStrS02)).ToLocalTime().ToString("G");
                TextBoxLastSetTime.Text = (DateTime.Parse(LStrS03)).ToLocalTime().ToString("G");
            }
        }
    }
}
