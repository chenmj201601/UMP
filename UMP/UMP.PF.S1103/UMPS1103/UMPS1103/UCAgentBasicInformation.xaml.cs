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
    public partial class UCAgentBasicInformation : UserControl
    {
        public UCAgentMaintenance IPageParent = null;

        public string IStrAgentID = string.Empty;

        private List<TreeViewItem> IListrTVItemAllOrg = new List<TreeViewItem>();

        public UCAgentBasicInformation()
        {
            InitializeComponent();
            this.Loaded += UCAgentBasicInformation_Loaded;
            ImageUpDownArrow.PreviewMouseLeftButtonDown += GridAgentBasicTitle_PreviewMouseLeftButtonDown;
            IListrTVItemAllOrg.Clear();
        }

        public void ShowMyControlOrg()
        {
            string LStrMyOrg = string.Empty;
            string LStrOrgID = string.Empty;
            string LStrOrgName = string.Empty;
            string LStrOrgParent = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrImagesPath = string.Empty;

            try
            {
                IListrTVItemAllOrg.Clear();
                TreeViewMyCtrlOrg.Items.Clear();
                LStrImagesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S1103");
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                DataRow[] LDataRowMyInfo = App.IDataTable11005.Select("C001 = " + App.GClassSessionInfo.UserInfo.UserID.ToString());
                LStrMyOrg = LDataRowMyInfo[0]["C006"].ToString();
                LStrOrgID = LStrMyOrg;
                DataRow[] LDataRowOrg = App.IDataTable11006.Select("C001 = " + LStrOrgID);
                LStrOrgName = LDataRowOrg[0]["C002"].ToString();
                LStrOrgName = EncryptionAndDecryption.EncryptDecryptString(LStrOrgName, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrOrgParent = LDataRowOrg[0]["C004"].ToString();
                TreeViewItem LTreeViewItemRoot = new TreeViewItem();
                LTreeViewItemRoot.Header = LStrOrgName;
                LTreeViewItemRoot.DataContext = LStrOrgID;
                if (LStrOrgParent != "0")
                {
                    TreeViewItemProps.SetItemImageName(LTreeViewItemRoot, LStrImagesPath + @"\S1103004.ico");
                }
                else
                {
                    TreeViewItemProps.SetItemImageName(LTreeViewItemRoot, LStrImagesPath + @"\S1103000.ico");
                }
                TreeViewMyCtrlOrg.Items.Add(LTreeViewItemRoot);
                ShowMyControlOrg(LTreeViewItemRoot, LStrOrgID);
                LTreeViewItemRoot.IsExpanded = true;
                IListrTVItemAllOrg.Add(LTreeViewItemRoot);
            }
            catch { }
        }

        private void ShowMyControlOrg(TreeViewItem ATreeViewItem, string AStrParentOrgID)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrOrgID = string.Empty;
            string LStrOrgName = string.Empty;
            string LStrOrgParent = string.Empty;
            string LStrImagesPath = string.Empty;

            try
            {
                LStrImagesPath = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S1103");
                LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                DataRow[] LDataRowOrg = App.IDataTable11006.Select("C004 = " + AStrParentOrgID);
                foreach (DataRow LDataRowSingleOrg in LDataRowOrg)
                {
                    LStrOrgID = LDataRowSingleOrg["C001"].ToString();
                    if (App.IDataTable11201UO.Select("C004 = " + LStrOrgID).Length <= 0) { continue; }
                    LStrOrgName = LDataRowSingleOrg["C002"].ToString();
                    LStrOrgName = EncryptionAndDecryption.EncryptDecryptString(LStrOrgName, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    TreeViewItem LTreeViewItemSubOrg = new TreeViewItem();
                    LTreeViewItemSubOrg.Header = LStrOrgName;
                    LTreeViewItemSubOrg.DataContext = LStrOrgID;
                    TreeViewItemProps.SetItemImageName(LTreeViewItemSubOrg, LStrImagesPath + @"\S1103004.ico");
                    ATreeViewItem.Items.Add(LTreeViewItemSubOrg);
                    ShowMyControlOrg(LTreeViewItemSubOrg, LStrOrgID);
                    IListrTVItemAllOrg.Add(LTreeViewItemSubOrg);
                    
                }
            }
            catch { }
        }

        public void EnabledOrDisabledElement(bool ABoolEnabled)
        {
            string LStrOrgID = string.Empty;
            string LStrOrgFullName = string.Empty;
            TreeViewItem LTreeViewItemParent = null;

            ButtonOrg.IsEnabled = ABoolEnabled;
            TextBoxAgentID.IsReadOnly = true;
            TextBoxAgentName.IsReadOnly = !ABoolEnabled;
            ComboBoxStatus.IsEnabled = ABoolEnabled;

            GridBelongOrgView.Visibility = System.Windows.Visibility.Visible;
            GridBelongOrgSelect.Visibility = System.Windows.Visibility.Collapsed;

            if (!ABoolEnabled)
            {
                TextBoxAgentID.Background = Brushes.Transparent;
                TextBoxAgentName.Background = Brushes.Transparent;
            }
            else
            {
                TextBoxAgentName.Background = Brushes.White;
                TextBoxAgentID.IsReadOnly = true;
                if (IPageParent.IStrCurrentMethod == "A")
                {
                    TextBoxAgentID.IsReadOnly = false;
                    TextBoxAgentID.Background = Brushes.White;

                    TreeViewItem LTreeViewItemCurrent = TreeViewMyCtrlOrg.SelectedItem as TreeViewItem;
                    if (LTreeViewItemCurrent == null)
                    {
                        TextBoxOrg.Text = (IPageParent.TreeViewOrgAgent.Items[0] as TreeViewItem).Header.ToString();
                        TextBoxOrg.Tag = (IPageParent.TreeViewOrgAgent.Items[0] as TreeViewItem).DataContext.ToString();
                        TreeViewItem LTreeViewItemRoot = TreeViewMyCtrlOrg.Items[0] as TreeViewItem;
                        LTreeViewItemRoot.Focus();
                        LTreeViewItemRoot.BringIntoView();
                        return;
                    }
                    else
                    {
                        LStrOrgID = LTreeViewItemCurrent.DataContext.ToString();
                        LStrOrgFullName += @" \ " + LTreeViewItemCurrent.Header.ToString();
                        LTreeViewItemParent = LTreeViewItemCurrent.Parent as TreeViewItem;
                        while (LTreeViewItemParent != null)
                        {
                            LStrOrgFullName = @" \ " + LTreeViewItemParent.Header.ToString() + LStrOrgFullName;
                            LTreeViewItemParent = LTreeViewItemParent.Parent as TreeViewItem;
                        }

                        LStrOrgFullName = LStrOrgFullName.Substring(3);

                        TextBoxOrg.Tag = LStrOrgID;
                        TextBoxOrg.Text = LStrOrgFullName;
                    }
                    TextBoxAgentID.Text = string.Empty;
                    TextBoxAgentName.Text = string.Empty;
                }
            }
            
        }

        private void UCAgentBasicInformation_Loaded(object sender, RoutedEventArgs e)
        {
            GridObjectView.Visibility = System.Windows.Visibility.Visible;
            ImageUpDownArrow.Style = (Style)App.Current.Resources["ImageUpDownArrowOpenStyle"];
            ButtonOrg.Click += ButtonOperationClick;
            ButtonSelectOrg.Click += ButtonOperationClick;
            ButtonOrgCancel.Click += ButtonOperationClick;
        }

        private void ButtonOperationClick(object sender, RoutedEventArgs e)
        {
            string LStrSenderName = string.Empty;

            Button LButtonClicked = sender as Button;
            LStrSenderName = LButtonClicked.Name;

            if (LStrSenderName == "ButtonOrg")
            {
                GridBelongOrgView.Visibility = System.Windows.Visibility.Collapsed;
                GridBelongOrgSelect.Visibility = System.Windows.Visibility.Visible;
                TreeViewItem LTreeViewItemSelected = TreeViewMyCtrlOrg.SelectedItem as TreeViewItem;
                if (LTreeViewItemSelected != null) { LTreeViewItemSelected.BringIntoView(); }
                return;
            }
            if (LStrSenderName == "ButtonOrgCancel")
            {
                GridBelongOrgView.Visibility = System.Windows.Visibility.Visible;
                GridBelongOrgSelect.Visibility = System.Windows.Visibility.Collapsed;
                return;
            }
            if (LStrSenderName == "ButtonSelectOrg")
            {
                AgentBelongOrgChanged();
                return;
            }
        }

        private void AgentBelongOrgChanged()
        {
            string LStrOrgID = string.Empty;
            string LStrOrgFullName = string.Empty;
            List<string> LListStrParentOrg = new List<string>();

            TreeViewItem LTreeViewItemParent = null;

            try
            {
                TreeViewItem LTreeViewItemCurrent = TreeViewMyCtrlOrg.SelectedItem as TreeViewItem;
                if (LTreeViewItemCurrent == null) { return; }
                LStrOrgID = LTreeViewItemCurrent.DataContext.ToString();
                LStrOrgFullName += @" \ " + LTreeViewItemCurrent.Header.ToString();
                LListStrParentOrg.Add(LStrOrgID + App.AscCodeToChr(27) + LTreeViewItemCurrent.Header.ToString());
                LTreeViewItemParent = LTreeViewItemCurrent.Parent as TreeViewItem;
                while (LTreeViewItemParent != null)
                {
                    LListStrParentOrg.Add(LTreeViewItemParent.DataContext.ToString() + App.AscCodeToChr(27) + LTreeViewItemParent.Header.ToString());
                    LStrOrgFullName = @" \ " + LTreeViewItemParent.Header.ToString() + LStrOrgFullName;
                    LTreeViewItemParent = LTreeViewItemParent.Parent as TreeViewItem;
                }

                LStrOrgFullName = LStrOrgFullName.Substring(3);

                TextBoxOrg.Tag = LStrOrgID;
                TextBoxOrg.Text = LStrOrgFullName;
                if (IPageParent.IStrCurrentMethod != "A")
                {
                    IPageParent.IUCManger.ShowInformation(LListStrParentOrg, IStrAgentID);
                }
                GridBelongOrgView.Visibility = System.Windows.Visibility.Visible;
                GridBelongOrgSelect.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch { }
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

        public void ShowInformation(string AStrAgentID)
        {
            #region 当前部门
            string LStrOrgFullPath = string.Empty;
            string LStrCurrentOrgID = string.Empty;

            IStrAgentID = AStrAgentID;
            LStrCurrentOrgID = IPageParent.GetAgentParentOrg(AStrAgentID, ref LStrOrgFullPath);
            if (!string.IsNullOrEmpty(LStrOrgFullPath))
            {
                TextBoxOrg.Tag = LStrCurrentOrgID;
                TextBoxOrg.Text = LStrOrgFullPath;
            }
            #endregion

            foreach (TreeViewItem LTreeViewItemSingleOrg in IListrTVItemAllOrg)
            {
                if (LTreeViewItemSingleOrg.DataContext.ToString() == LStrCurrentOrgID)
                {
                    LTreeViewItemSingleOrg.IsSelected = true;
                    ExpandParentItem(LTreeViewItemSingleOrg);
                    break;
                }
            }

            ButtonSelectOrg.Visibility = System.Windows.Visibility.Collapsed;

            #region 其他基本信息
            string LStrVerificationCode104 = string.Empty;
            string LStrAgentCode = string.Empty;
            string LStrAgentName = string.Empty;
            string LStrAgentStatus = string.Empty;

            LStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            DataRow[] LDataRowAgent = App.IDataTable11101.Select("C001 = " + AStrAgentID + " AND C002 = 1", "C002 ASC");
            LStrAgentCode = LDataRowAgent[0]["C017"].ToString();
            LStrAgentCode = EncryptionAndDecryption.EncryptDecryptString(LStrAgentCode, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrAgentName = LDataRowAgent[0]["C018"].ToString();
            LStrAgentName = EncryptionAndDecryption.EncryptDecryptString(LStrAgentName, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
            LStrAgentStatus = LDataRowAgent[0]["C012"].ToString();
            TextBoxAgentID.Text = LStrAgentCode;
            TextBoxAgentName.Text = LStrAgentName;
            if (LStrAgentStatus == "1") { ComboBoxStatus.SelectedIndex = 0; } else { ComboBoxStatus.SelectedIndex = 1; }
            #endregion
        }

        private void ExpandParentItem(TreeViewItem ATreeViewItemSelected)
        {
            TreeViewItem LTreeViewItemParent = null;

            try
            {
                LTreeViewItemParent = ATreeViewItemSelected.Parent as TreeViewItem;
                while (LTreeViewItemParent != null)
                {
                    LTreeViewItemParent.IsExpanded = true;
                    LTreeViewItemParent = LTreeViewItemParent.Parent as TreeViewItem;
                }
            }
            catch { }
        }

        public void ShowElementContent()
        {
            TextBlockObjectHeader.Text = App.GetDisplayCharater("S1103005");
            LabelOrg.Content = App.GetDisplayCharater("S1103006");
            LabelAgentID.Content = App.GetDisplayCharater("S1103007");
            LabelAgentName.Content = App.GetDisplayCharater("S1103008");
            LabelAgentStatus.Content = App.GetDisplayCharater("S1103011");
            ComboBoxItemLockStatus1.Content = App.GetDisplayCharater("S1103009");
            ComboBoxItemLockStatus0.Content = App.GetDisplayCharater("S1103010");

            ButtonSelectOrg.Content = App.GetDisplayCharater("S1103016");
            ButtonOrgCancel.Content = App.GetDisplayCharater("S1103017");
        }

        public List<string> GetElementSettedData()
        {
            List<string> LListStrReturn = new List<string>();
            string LStrAgentID = string.Empty;
            string LStrAgentName = string.Empty;

            try
            {
                LStrAgentID = TextBoxAgentID.Text.Trim();
                LStrAgentName = TextBoxAgentName.Text.Trim();
                if (string.IsNullOrEmpty(LStrAgentID))
                {
                    LListStrReturn.Clear();
                    MessageBox.Show(App.GetDisplayCharater("S1103022"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return LListStrReturn;
                }
                if (LStrAgentID.Length > 128)
                {
                    LListStrReturn.Clear();
                    MessageBox.Show(App.GetDisplayCharater("S1103025"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return LListStrReturn;
                }
                if (LStrAgentName.Length > 128)
                {
                    LListStrReturn.Clear();
                    MessageBox.Show(App.GetDisplayCharater("S1103026"), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return LListStrReturn;
                }
                LListStrReturn.Add("B01" + App.AscCodeToChr(27) + TextBoxOrg.Tag.ToString());
                LListStrReturn.Add("B07" + App.AscCodeToChr(27) + LStrAgentID);
                if (string.IsNullOrEmpty(LStrAgentName)) { LListStrReturn.Add("B08" + App.AscCodeToChr(27) + LStrAgentID); }
                else { LListStrReturn.Add("B08" + App.AscCodeToChr(27) + LStrAgentName); }

                if (ComboBoxStatus.SelectedIndex == 0) { LListStrReturn.Add("B02" + App.AscCodeToChr(27) + "1"); }
                else { LListStrReturn.Add("B02" + App.AscCodeToChr(27) + "0"); }
            }
            catch { LListStrReturn.Clear(); }

            return LListStrReturn;
        }

        private void TreeViewMyCtrlOrg_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string LStrOrgID = string.Empty;

            try
            {
                TreeViewItem LTreeViewItemCurrent = TreeViewMyCtrlOrg.SelectedItem as TreeViewItem;
                if (LTreeViewItemCurrent == null) { return; }
                LStrOrgID = LTreeViewItemCurrent.DataContext.ToString();
                if (LStrOrgID == TextBoxOrg.Tag.ToString())
                {
                    ButtonSelectOrg.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    ButtonSelectOrg.Visibility = System.Windows.Visibility.Visible;
                }
            }
            catch { }
        }
    }
}
