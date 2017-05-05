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
    public partial class UCAgentManagerList : UserControl
    {
        public UCAgentMaintenance IPageParent = null;

        public UCAgentManagerList()
        {
            InitializeComponent();
            this.Loaded += UCAgentManagerList_Loaded;
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

        private void UCAgentManagerList_Loaded(object sender, RoutedEventArgs e)
        {
            GridObjectView.Visibility = System.Windows.Visibility.Collapsed;
            ImageUpDownArrow.Style = (Style)App.Current.Resources["ImageUpDownArrowCloseStyle"];
        }

        public void ShowElementContent()
        {
            TextBlockObjectHeader.Text = App.GetDisplayCharater("S1103015");
        }

        public void ShowInformation(string AStrAgentID)
        {
            int LIntAllParent = 0;
            int LIntCurrent = 0;
            string LStrImagesPath = string.Empty;
            TreeViewItem LTreeViewItemParent = null;

            List<string> LListStrParentOrg = IPageParent.GetAgentParentOrg(AStrAgentID);

            ShowOrgUserTreeView(LListStrParentOrg);

            UCTreeViewYoung LUCTreeViewYoung = GridObjectSelect.Children[0] as UCTreeViewYoung;

            LStrImagesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S1103");
            TreeViewManager.Items.Clear();
            LIntAllParent = LListStrParentOrg.Count - 1;
            for (LIntCurrent = LIntAllParent; LIntCurrent >= 0; LIntCurrent--)
            {
                string[] LStrArrayOrg = LListStrParentOrg[LIntCurrent].Split(App.AscCodeToChr(27).ToCharArray());

                TreeViewItem LTreeViewItemOrg = new TreeViewItem();
                LTreeViewItemOrg.Header = LStrArrayOrg[1];
                LTreeViewItemOrg.DataContext = LStrArrayOrg[0];
                if (LStrArrayOrg[0].Substring(3, 5) == App.GClassSessionInfo.RentInfo.Token && LStrArrayOrg[0].Substring(8) == "00000000001")
                {
                    TreeViewItemProps.SetItemImageName(LTreeViewItemOrg, LStrImagesPath + @"\S1103000.ico");
                }
                else
                {
                    TreeViewItemProps.SetItemImageName(LTreeViewItemOrg, LStrImagesPath + @"\S1103004.ico");
                }
                if (LTreeViewItemParent == null) { TreeViewManager.Items.Add(LTreeViewItemOrg); LTreeViewItemOrg.IsExpanded = true; }
                else { LTreeViewItemParent.Items.Add(LTreeViewItemOrg); }
                LTreeViewItemParent = LTreeViewItemOrg;
                ShowControlAgentUser(LTreeViewItemParent, LStrArrayOrg[0], LUCTreeViewYoung, AStrAgentID);
            }
        }

        public void ShowInformation(List<string> AListStrParentOrg, string AStrAgentID)
        {
            int LIntAllParent = 0;
            int LIntCurrent = 0;
            string LStrImagesPath = string.Empty;
            TreeViewItem LTreeViewItemParent = null;

            TreeViewManager.Items.Clear();

            ShowOrgUserTreeView(AListStrParentOrg);

            UCTreeViewYoung LUCTreeViewYoung = GridObjectSelect.Children[0] as UCTreeViewYoung;

            LStrImagesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S1103");
            TreeViewManager.Items.Clear();
            LIntAllParent = AListStrParentOrg.Count - 1;
            for (LIntCurrent = LIntAllParent; LIntCurrent >= 0; LIntCurrent--)
            {
                string[] LStrArrayOrg = AListStrParentOrg[LIntCurrent].Split(App.AscCodeToChr(27).ToCharArray());

                TreeViewItem LTreeViewItemOrg = new TreeViewItem();
                LTreeViewItemOrg.Header = LStrArrayOrg[1];
                LTreeViewItemOrg.DataContext = LStrArrayOrg[0];
                if (LStrArrayOrg[0].Substring(3, 5) == App.GClassSessionInfo.RentInfo.Token && LStrArrayOrg[0].Substring(8) == "00000000001")
                {
                    TreeViewItemProps.SetItemImageName(LTreeViewItemOrg, LStrImagesPath + @"\S1103000.ico");
                }
                else
                {
                    TreeViewItemProps.SetItemImageName(LTreeViewItemOrg, LStrImagesPath + @"\S1103004.ico");
                }
                if (LTreeViewItemParent == null) { TreeViewManager.Items.Add(LTreeViewItemOrg); LTreeViewItemOrg.IsExpanded = true; }
                else { LTreeViewItemParent.Items.Add(LTreeViewItemOrg); }
                LTreeViewItemParent = LTreeViewItemOrg;
                ShowControlAgentUser(LTreeViewItemParent, LStrArrayOrg[0], LUCTreeViewYoung, AStrAgentID);
            }
        }

        public List<string> GetElementSettedData()
        {
            List<string> LListStrReturn = new List<string>();

            UCTreeViewYoung LUCTreeViewYoung = GridObjectSelect.Children[0] as UCTreeViewYoung;
            LUCTreeViewYoung.SetItemCheckedStatus(true, "102" + App.GClassSessionInfo.RentInfo.Token + "00000000001");
            LUCTreeViewYoung.SetItemCheckedStatus(true, App.GClassSessionInfo.UserInfo.UserID.ToString());

            List<object> LListObjectSelected = LUCTreeViewYoung.GetCheckedOrUnCheckedItem("1");
            foreach (object LObjectSingle in LListObjectSelected)
            {
                LListStrReturn.Add("U00" + App.AscCodeToChr(27) + LObjectSingle.ToString());
            }

            return LListStrReturn;
        }

        private void ShowControlAgentUser(TreeViewItem ATreeViewItem, string AStrOrgID, UCTreeViewYoung AUCTreeViewYoung, string AStrAgentID)
        {
            string LStrUserID = string.Empty;
            string LStrUserAccount = string.Empty;
            string LStrUserName = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrImagesPath = string.Empty;

            LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrImagesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S1103");
            DataRow[] LDataRowOrgUsers = App.IDataTable11005.Select("C006 = " + AStrOrgID, "C001 ASC");
            foreach (DataRow LDataRowSingleOrgUser in LDataRowOrgUsers)
            {
                LStrUserID = LDataRowSingleOrgUser["C001"].ToString();
                if (App.IDataTable11201UA.Select("C003 = " + LStrUserID + " AND C004 = " + AStrAgentID).Length <= 0) { continue; }
                LStrUserAccount = LDataRowSingleOrgUser["C002"].ToString();
                LStrUserAccount = EncryptionAndDecryption.EncryptDecryptString(LStrUserAccount, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrUserName = LDataRowSingleOrgUser["C003"].ToString();
                LStrUserName = EncryptionAndDecryption.EncryptDecryptString(LStrUserName, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                TreeViewItem LTreeViewItemUser = new TreeViewItem();
                LTreeViewItemUser.Header = LStrUserAccount+ " (" + LStrUserName + ")";
                LTreeViewItemUser.DataContext = LStrUserID;
                TreeViewItemProps.SetItemImageName(LTreeViewItemUser, LStrImagesPath + @"\S1103005.png");
                ATreeViewItem.Items.Add(LTreeViewItemUser);
                AUCTreeViewYoung.SetItemCheckedStatus(true, LStrUserID);
            }
            //AUCTreeViewYoung.SetItemCheckedDisabled(false, "102" + App.GClassSessionInfo.RentInfo.Token + "00000000001");
            //AUCTreeViewYoung.SetItemCheckedDisabled(false, App.GClassSessionInfo.UserInfo.UserID.ToString());
        }

        public void EnabledOrDisabledElement(bool ABoolEnabled)
        {
            if (ABoolEnabled)
            {
                TreeViewManager.Visibility = System.Windows.Visibility.Collapsed;
                GridObjectSelect.Visibility = System.Windows.Visibility.Visible;
                if (IPageParent.IStrCurrentMethod == "A")
                {
                    TreeViewManager.Items.Clear();
                }
            }
            else
            {
                TreeViewManager.Visibility = System.Windows.Visibility.Visible;
                GridObjectSelect.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void ShowOrgUserTreeView(List<string> AListStrParentOrg)
        {
            int LIntAllParent = 0;
            int LIntCurrent = 0;
            string LStrImagesPath = string.Empty;
            TreeViewItem LTreeViewItemParent = null;

            GridObjectSelect.Children.Clear();
            UCTreeViewYoung LUCTreeViewYoung = new UCTreeViewYoung();
            GridObjectSelect.Children.Add(LUCTreeViewYoung);

            LStrImagesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S1103");
            LIntAllParent = AListStrParentOrg.Count - 1;
            for (LIntCurrent = LIntAllParent; LIntCurrent >= 0; LIntCurrent--)
            {
                string[] LStrArrayOrg = AListStrParentOrg[LIntCurrent].Split(App.AscCodeToChr(27).ToCharArray());

                TreeViewItem LTreeViewItemOrg = LUCTreeViewYoung.AddTreeViewItem(LTreeViewItemParent, false, LStrImagesPath + @"\S1103004.ico", LStrArrayOrg[1], LStrArrayOrg[0]);
                LTreeViewItemParent = LTreeViewItemOrg;

                ShowMyControlUser(LTreeViewItemParent, LStrArrayOrg[0], LUCTreeViewYoung);
                LTreeViewItemParent.IsExpanded = true;
            }
        }

        private void ShowMyControlUser(TreeViewItem ATreeViewItem, string AStrOrgID, UCTreeViewYoung AUCTreeViewYoung)
        {
            string LStrUserID = string.Empty;
            string LStrUserAccount = string.Empty;
            string LStrUserName = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrImagesPath = string.Empty;

            LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrImagesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S1103");
            DataRow[] LDataRowUsers = App.IDataTable11005.Select("C006 = " + AStrOrgID, "C001 ASC");
            foreach (DataRow LDataRowSingleUser in LDataRowUsers)
            {
                LStrUserID = LDataRowSingleUser["C001"].ToString();
                if (App.IDataTable11201UU.Select("C004 = " + LStrUserID).Length <= 0) { continue; }
                LStrUserAccount = LDataRowSingleUser["C002"].ToString();
                LStrUserAccount = EncryptionAndDecryption.EncryptDecryptString(LStrUserAccount, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrUserName = LDataRowSingleUser["C003"].ToString();
                LStrUserName = EncryptionAndDecryption.EncryptDecryptString(LStrUserName, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                TreeViewItem LTreeViewItemUser = AUCTreeViewYoung.AddTreeViewItem(ATreeViewItem, false, LStrImagesPath + @"\S1103005.png", LStrUserAccount + " (" + LStrUserName + ")", LStrUserID);
            }
        }
    }
}
