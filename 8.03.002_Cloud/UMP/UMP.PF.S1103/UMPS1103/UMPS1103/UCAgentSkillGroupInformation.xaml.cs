using PFShareClassesC;
using PFShareControls;
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

namespace UMPS1103
{
    public partial class UCAgentSkillGroupInformation : UserControl
    {
        public UCAgentMaintenance IPageParent = null;

        public UCAgentSkillGroupInformation()
        {
            InitializeComponent();
            this.Loaded += UCAgentSkillGroupInformation_Loaded;
            ImageUpDownArrow.PreviewMouseLeftButtonDown += GridAgentBasicTitle_PreviewMouseLeftButtonDown;
        }

        private void GridAgentBasicTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (GridObjectView.Visibility == System.Windows.Visibility.Collapsed)
            {
                GridObjectView.Visibility = System.Windows.Visibility.Visible;
                ImageUpDownArrow.Style = (Style)App.Current.Resources["ImageUpDownArrowOpenStyle"];
            }
            else
            {
                GridObjectView.Visibility = System.Windows.Visibility.Collapsed;
                ImageUpDownArrow.Style = (Style)App.Current.Resources["ImageUpDownArrowCloseStyle"];
            }
        }

        private void UCAgentSkillGroupInformation_Loaded(object sender, RoutedEventArgs e)
        {
            GridObjectView.Visibility = System.Windows.Visibility.Collapsed;
            ImageUpDownArrow.Style = (Style)App.Current.Resources["ImageUpDownArrowCloseStyle"];
        }

        public void ShowElementContent()
        {
            TextBlockObjectHeader.Text = App.GetDisplayCharater("S1103014");
            ColumnSkillGroupCode.Content = "  " + App.GetDisplayCharater("S1103012");
            ColumnSkillGroupName.Content = "  " + App.GetDisplayCharater("S1103013");
        }

        public void ShowInformation(string AStrAgentID)
        {
            string LStrSkillID = string.Empty;
            string LStrSkillCode = string.Empty;
            string LStrSkillName = string.Empty;
            string LStrVerificationCode104 = string.Empty;

            try
            {
                ShowSkillGroupTreeView();
                UCTreeViewYoung LUCTreeViewYoung = GridObjectSelect.Children[0] as UCTreeViewYoung;
                ObjectListView.Items.Clear();
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                DataRow[] LDataRowSA = App.IDataTable11201SA.Select("C004 = " + AStrAgentID, "C003 ASC");
                foreach (DataRow LDataRowSingleSkill in LDataRowSA)
                {
                    LStrSkillID = LDataRowSingleSkill["C003"].ToString();
                    DataRow[] LDataRowSkill = App.IDataTable11009.Select("C001 = " + LStrSkillID);
                    LStrSkillCode = LDataRowSkill[0]["C006"].ToString();
                    LStrSkillName = LDataRowSkill[0]["C008"].ToString();
                    LStrSkillCode = EncryptionAndDecryption.EncryptDecryptString(LStrSkillCode, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    ListViewItem LListViewItemSkill = new ListViewItem();
                    LListViewItemSkill.Content = new ObjctColumnDefine(LStrSkillID, LStrSkillCode, LStrSkillName, "");
                    LListViewItemSkill.Height = 26;
                    LListViewItemSkill.Margin = new Thickness(0, 1, 0, 1);
                    ObjectListView.Items.Add(LListViewItemSkill);
                    LUCTreeViewYoung.SetItemCheckedStatus(true, LStrSkillID);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public List<string> GetElementSettedData()
        {
            List<string> LListStrReturn = new List<string>();

            UCTreeViewYoung LUCTreeViewYoung = GridObjectSelect.Children[0] as UCTreeViewYoung;
            List<object> LListObjectSelected = LUCTreeViewYoung.GetCheckedOrUnCheckedItem("1");
            foreach(object LObjectSingle in LListObjectSelected)
            {
                LListStrReturn.Add("S00" + App.AscCodeToChr(27) + LObjectSingle.ToString());
            }

            return LListStrReturn;
        }

        public void EnabledOrDisabledElement(bool ABoolEnabled)
        {
            if (ABoolEnabled)
            {
                ObjectListView.Visibility = System.Windows.Visibility.Collapsed;
                GridObjectSelect.Visibility = System.Windows.Visibility.Visible;
                //if (IPageParent.IStrCurrentMethod == "A")
                //{
                //    ShowSkillGroupTreeView();
                //    ObjectListView.Items.Clear();
                //}
            }
            else
            {
                ObjectListView.Visibility = System.Windows.Visibility.Visible;
                GridObjectSelect.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ShowSkillGroupTreeView()
        {
            string LStrSkillID = string.Empty;
            string LStrSkillCode = string.Empty;
            string LStrSkillName = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrImagesPath = string.Empty;

            LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrImagesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S1100\S1100005.png");
            GridObjectSelect.Children.Clear();
            UCTreeViewYoung LUCTreeViewYoung = new UCTreeViewYoung();
            GridObjectSelect.Children.Add(LUCTreeViewYoung);
            foreach (DataRow LDataRowSingleSkillGroup in App.IDataTable11009.Rows)
            {
                LStrSkillID = LDataRowSingleSkillGroup["C001"].ToString();
                LStrSkillCode = LDataRowSingleSkillGroup["C006"].ToString();
                LStrSkillName = LDataRowSingleSkillGroup["C008"].ToString();
                LStrSkillCode = EncryptionAndDecryption.EncryptDecryptString(LStrSkillCode, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                TreeViewItem LTreeViewItemSkillGroup = LUCTreeViewYoung.AddTreeViewItem(null, false, LStrImagesPath, LStrSkillCode + " (" + LStrSkillName + ")", LStrSkillID);
            }
        }
    }
}
