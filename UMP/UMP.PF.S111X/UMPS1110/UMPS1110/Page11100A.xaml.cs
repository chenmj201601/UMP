using PFShareClassesC;
using PFShareControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
using VoiceCyber.UMP.Communications;

namespace UMPS1110
{
    public partial class Page11100A : Page, S1110ChangeLanguageInterface, S1110Interface
    {
        public event EventHandler<OperationEventArgs> IOperationEvent;
        public event EventHandler<OperationEventArgs> IChangeLanguageEvent;

        public string IStrImageFolder = string.Empty;

        private string IStrVerificationCode004 = string.Empty;
        private string IStrVerificationCode104 = string.Empty;

        UCResourceType211 IUCResourceType211 = new UCResourceType211();
        UCResourceType212A IUCResourceType212A = new UCResourceType212A();
        UCResourceType212B IUCResourceType212B = new UCResourceType212B();
        UCResourceType213 IUCResourceType213 = new UCResourceType213();

        public Page11100A()
        {
            InitializeComponent();
            IStrVerificationCode004 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
            IStrVerificationCode104 = App.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            this.Loaded += Page11100A_Loaded;
        }

        private void Page11100A_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                App.IPageMainOpend = this;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = (int)RequestCode.CSModuleLoading;
                LWebRequestClientLoading.Session = App.GClassSessionInfo;
                LWebRequestClientLoading.Session.SessionID = App.GClassSessionInfo.SessionID;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                if (LWebReturn.Result)
                {
                    App.GClassSessionInfo.AppServerInfo = LWebReturn.Session.AppServerInfo;
                    App.GClassSessionInfo.DatabaseInfo = LWebReturn.Session.DatabaseInfo;
                    App.GClassSessionInfo.DBConnectionString = LWebReturn.Session.DBConnectionString;
                    App.GClassSessionInfo.DBType = LWebReturn.Session.DBType;
                    App.GClassSessionInfo.LangTypeInfo = LWebReturn.Session.LangTypeInfo;
                    App.GClassSessionInfo.LocalMachineInfo = LWebReturn.Session.LocalMachineInfo;
                    App.GClassSessionInfo.RentInfo = LWebReturn.Session.RentInfo;
                    App.GClassSessionInfo.RoleInfo = LWebReturn.Session.RoleInfo;
                    App.GClassSessionInfo.ThemeInfo = LWebReturn.Session.ThemeInfo;
                    App.GClassSessionInfo.UserInfo = LWebReturn.Session.UserInfo;
                    if (!string.IsNullOrEmpty(LWebReturn.Data)) { App.GStrCurrentOperation = LWebReturn.Data; }
                    App.LoadStyleDictionary();
                    App.LoadApplicationLanguages();
                    App.LoadResourceManagementData();
                    App.LoadThidModuleOperation();
                }
                DoingMainSendMessage(App.GStrCurrentOperation);

                WebRequest LWebRequestClientLoaded = new WebRequest();
                LWebRequestClientLoaded.Code = (int)RequestCode.CSModuleLoaded;
                LWebRequestClientLoaded.Session = App.GClassSessionInfo;
                LWebRequestClientLoaded.Session.SessionID = App.GClassSessionInfo.SessionID;
                App.SendNetPipeMessage(LWebRequestClientLoaded);

                IStrImageFolder = System.IO.Path.Combine(App.GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", App.GClassSessionInfo.ThemeInfo.Name, @"Images\S1110");

                ShowElementContent();
                ShowAllResourceType(null, 0);

                ShowType211LeftSubObjects();
                ShowType212LeftSubObjects();
                ShowType213LeftSubObjects();

                ButtonReloadOptions.Click += ButtonReloadGoHomeClick;
                ButtonHomeOptions.Click += ButtonReloadGoHomeClick;
                LabelReloadData.PreviewMouseLeftButtonDown += LabelReloadGoHomePreviewMouseLeftButtonDown;
                LabelReturnHome.PreviewMouseLeftButtonDown += LabelReloadGoHomePreviewMouseLeftButtonDown;
                TreeViewResourceList.SelectedItemChanged += TreeViewResourceList_SelectedItemChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void TreeViewResourceList_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string LStrItemData = string.Empty;

            try
            {
                TreeViewItem LTreeViewItemCurrent = TreeViewResourceList.SelectedItem as TreeViewItem;
                if (LTreeViewItemCurrent == null) { return; }
                LStrItemData = LTreeViewItemCurrent.DataContext.ToString();
                ShowTypeObjectDetails(LStrItemData);
                ShowTypeObjectOperations(LStrItemData);
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        private void LabelReloadGoHomePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string LStrClickedName = string.Empty;

            try
            {
                Label LLabelClicked = sender as Label;
                if (LLabelClicked == null) { return; }
                LStrClickedName = LLabelClicked.Name;
                if (LStrClickedName == "LabelReturnHome")
                {
                    App.SendCloseAppplicationMessage();
                    return;
                }
            }
            catch { }
        }

        private void ButtonReloadGoHomeClick(object sender, RoutedEventArgs e)
        {
            string LStrClickedName = string.Empty;

            try
            {
                Button LButtonClicked = sender as Button;
                if (LButtonClicked == null) { return; }
                LStrClickedName = LButtonClicked.Name;
                if (LStrClickedName == "ButtonHomeOptions")
                {
                    App.SendCloseAppplicationMessage();
                    return;
                }
            }
            catch { }
        }

        public void DoingMainSendMessage(string AStrData)
        {
            try
            {
                IChangeLanguageEvent = null;
                App.LoadResourceManagementData();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void ApplicationLanguageChanged()
        {
            if (IChangeLanguageEvent != null)
            {
                OperationEventArgs LEventArgs = new OperationEventArgs();
                IChangeLanguageEvent(this, LEventArgs);
            }
            ShowElementContent();
            ShowAllResourceType(null, 0);
            ShowType211LeftSubObjects();
            ShowType212LeftSubObjects();
            ShowType213LeftSubObjects();
        }

        public void ShowElementContent()
        {
            LabelReloadData.Content = App.GetDisplayCharater("1110001");
            LabelReturnHome.Content = App.GetDisplayCharater("1110002");
            LabelLoginAccountShow.Content = App.GClassSessionInfo.UserInfo.Account + " (" + App.GClassSessionInfo.UserInfo.UserName + ")";
            LabelLoginRoleShow.Content = App.GClassSessionInfo.RoleInfo.Name;
            LabelResourceList.Content = App.GetDisplayCharater("1110008");
            LabelResourceManagementCenter.Content = App.GetDisplayCharater("1110003");
            LabelResourceOperation.Content = App.GetDisplayCharater("1110009");
            LabelResourceDesc.Content = App.GetDisplayCharater("1110010");

            LabelApplyData.Content = App.GetDisplayCharater("1110000");
        }

        #region 显示资源列表
        private void ShowAllResourceType(TreeViewItem ATreeViewItemParent, int AIntParentType)
        {
            string LStrIconPngName = string.Empty;
            int LIntCurrentType = 0;

            try
            {
                if (ATreeViewItemParent == null) { TreeViewResourceList.Items.Clear();}
                DataRow[] LDataRowCurrentLevelType = App.IDataTable00010.Select("C002 = " + AIntParentType.ToString() + " AND C004 = '1'" , "C003 ASC");
                foreach (DataRow LDataRowSingleType in LDataRowCurrentLevelType)
                {
                    LIntCurrentType = int.Parse(LDataRowSingleType["C001"].ToString());
                    DataRow[] LDataRowOperation = App.IDataTableOperation.Select("C002 = 1110" + LIntCurrentType.ToString() + "00");
                    if (LDataRowOperation.Length != 1) { continue; }

                    LStrIconPngName = LDataRowSingleType["C005"].ToString();
                    TreeViewItem LTreeViewItemType = new TreeViewItem();
                    LTreeViewItemType.Header = App.GetDisplayCharater("TreeViewResourceList", "RType" + LIntCurrentType.ToString());
                    LTreeViewItemType.DataContext = LIntCurrentType.ToString();
                    TreeViewItemProps.SetItemImageName(LTreeViewItemType, IStrImageFolder + @"\" + LStrIconPngName);
                    if (ATreeViewItemParent == null)
                    {
                        TreeViewResourceList.Items.Add(LTreeViewItemType);
                    }
                    else
                    {
                        ATreeViewItemParent.Items.Add(LTreeViewItemType);
                    }
                }
                
                if (ATreeViewItemParent != null) { ATreeViewItemParent.IsExpanded = true; }
            }
            catch(Exception ex) {MessageBox.Show(ex.ToString()); }
        }

        public void ShowType211LeftSubObjects()
        {
            string LStrIconPngName = string.Empty;
            string LStrServerData011 = string.Empty;
            string LStrServerData012 = string.Empty;
            string LStrServerData013 = string.Empty;
            string LStrServerData014 = string.Empty;
            string LStrServerData017 = string.Empty;
            string LStrServerData018 = string.Empty;

            try
            {
                DataRow[] LDataRowCurrentLevelType = App.IDataTable00010.Select("C001 = 211");
                LStrIconPngName = LDataRowCurrentLevelType[0]["C005"].ToString();

                foreach (TreeViewItem LTreeViewItemSingleResurce in TreeViewResourceList.Items)
                {
                    if (LTreeViewItemSingleResurce.DataContext.ToString() != "211") { continue; }

                    CheckAndInitType211Data();
                    LTreeViewItemSingleResurce.Items.Clear();
                    
                    DataRow[] LDataRowLicenseServer = App.IListDataSetReturn[0].Tables[0].Select("C001 = 2110000000000000001 OR C001 = 2110000000000000002", "C001 ASC");

                    #region 显示Server1
                    LStrServerData011 = LDataRowLicenseServer[0]["C011"].ToString();
                    LStrServerData012 = LDataRowLicenseServer[0]["C012"].ToString();
                    LStrServerData013 = LDataRowLicenseServer[0]["C013"].ToString();
                    LStrServerData014 = LDataRowLicenseServer[0]["C014"].ToString();
                    LStrServerData014 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData014, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData017 = LDataRowLicenseServer[0]["C017"].ToString();
                    LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData018 = LDataRowLicenseServer[0]["C018"].ToString();
                    LStrServerData018 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData018, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                    TreeViewItem LTreeViewItemServer1 = new TreeViewItem();
                    if (LStrServerData011 == "0")
                    {
                        LTreeViewItemServer1.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + App.GetDisplayCharater("1110012");
                    }
                    else
                    {
                        LTreeViewItemServer1.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + LStrServerData017;// +":" + LStrServerData014;
                    }
                    LTreeViewItemServer1.DataContext = "2110000000000000001";
                    TreeViewItemProps.SetItemImageName(LTreeViewItemServer1, IStrImageFolder + @"\" + LStrIconPngName);
                    LTreeViewItemSingleResurce.Items.Add(LTreeViewItemServer1);
                    #endregion

                    #region 显示Server2
                    LStrServerData011 = LDataRowLicenseServer[1]["C011"].ToString();
                    LStrServerData012 = LDataRowLicenseServer[1]["C012"].ToString();
                    LStrServerData013 = LDataRowLicenseServer[1]["C013"].ToString();
                    LStrServerData014 = LDataRowLicenseServer[1]["C014"].ToString();
                    LStrServerData014 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData014, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData017 = LDataRowLicenseServer[1]["C017"].ToString();
                    LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData018 = LDataRowLicenseServer[1]["C018"].ToString();
                    LStrServerData018 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData018, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                    TreeViewItem LTreeViewItemServer2 = new TreeViewItem();
                    if (LStrServerData011 == "0")
                    {
                        LTreeViewItemServer2.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + App.GetDisplayCharater("1110012");
                    }
                    else
                    {
                        LTreeViewItemServer2.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + LStrServerData017;// +":" + LStrServerData014;
                    }
                    LTreeViewItemServer2.DataContext = "2110000000000000002";
                    TreeViewItemProps.SetItemImageName(LTreeViewItemServer2, IStrImageFolder + @"\" + LStrIconPngName);
                    LTreeViewItemSingleResurce.Items.Add(LTreeViewItemServer2);
                    #endregion
                    
                    LTreeViewItemSingleResurce.IsExpanded = true;
                    break;
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        public void ShowType212LeftSubObjects()
        {
            string LStrIconPngName = string.Empty;
            string LStrServerData001 = string.Empty;
            string LStrServerData011 = string.Empty;
            string LStrServerData012 = string.Empty;
            string LStrServerData013 = string.Empty;
            string LStrServerData014 = string.Empty;
            string LStrServerData015 = string.Empty;
            string LStrServerData016 = string.Empty;
            string LStrServerData017 = string.Empty;
            string LStrServerData018 = string.Empty;

            try
            {
                DataRow[] LDataRowCurrentLevelType = App.IDataTable00010.Select("C001 = 212");
                LStrIconPngName = LDataRowCurrentLevelType[0]["C005"].ToString();
                foreach (TreeViewItem LTreeViewItemSingleResurce in TreeViewResourceList.Items)
                {
                    if (LTreeViewItemSingleResurce.DataContext.ToString() != "212") { continue; }
                    CheckAndInitType212Data();
                    LTreeViewItemSingleResurce.Items.Clear();

                    DataRow[] LDataRowDECServer = App.IListDataSetReturn[1].Tables[0].Select("C001 >= 2120000000000000001 AND C001 < 2130000000000000000 AND C011 = '1'", "C012 ASC");
                    foreach (DataRow LDataRowSingleDecServer in LDataRowDECServer)
                    {
                        LStrServerData001 = LDataRowSingleDecServer["C001"].ToString();
                        LStrServerData011 = LDataRowSingleDecServer["C011"].ToString();
                        LStrServerData012 = LDataRowSingleDecServer["C012"].ToString();
                        LStrServerData013 = LDataRowSingleDecServer["C013"].ToString();
                        LStrServerData014 = LDataRowSingleDecServer["C014"].ToString();
                        LStrServerData014 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData014, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        LStrServerData015 = LDataRowSingleDecServer["C015"].ToString();
                        LStrServerData015 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData015, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        LStrServerData016 = LDataRowSingleDecServer["C016"].ToString();
                        LStrServerData016 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData016, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                        LStrServerData017 = LDataRowSingleDecServer["C017"].ToString();
                        LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        LStrServerData018 = LDataRowSingleDecServer["C018"].ToString();
                        LStrServerData018 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData018, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                        TreeViewItem LTreeViewItemDECServer = new TreeViewItem();

                        LTreeViewItemDECServer.Header = LStrServerData017;
                        LTreeViewItemDECServer.DataContext = LStrServerData001;
                        TreeViewItemProps.SetItemImageName(LTreeViewItemDECServer, IStrImageFolder + @"\" + LStrIconPngName);
                        LTreeViewItemSingleResurce.Items.Add(LTreeViewItemDECServer);
                    }
                    LTreeViewItemSingleResurce.IsExpanded = true;
                    break;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        public void ShowType213LeftSubObjects()
        {
            string LStrIconPngName = string.Empty;
            string LStrServerData011 = string.Empty;
            string LStrServerData012 = string.Empty;
            string LStrServerData013 = string.Empty;
            string LStrServerData014 = string.Empty;
            string LStrServerData017 = string.Empty;
            string LStrServerData018 = string.Empty;

            try
            {
                DataRow[] LDataRowCurrentLevelType = App.IDataTable00010.Select("C001 = 213");
                LStrIconPngName = LDataRowCurrentLevelType[0]["C005"].ToString();

                foreach (TreeViewItem LTreeViewItemSingleResurce in TreeViewResourceList.Items)
                {
                    if (LTreeViewItemSingleResurce.DataContext.ToString() != "213") { continue; }

                    CheckAndInitType213Data();
                    LTreeViewItemSingleResurce.Items.Clear();

                    DataRow[] LDataRowCtiHubServer = App.IListDataSetReturn[2].Tables[0].Select("C001 = 2130000000000000001 OR C001 = 2130000000000000002", "C001 ASC");

                    #region 显示Server1
                    LStrServerData011 = LDataRowCtiHubServer[0]["C011"].ToString();
                    LStrServerData012 = LDataRowCtiHubServer[0]["C012"].ToString();
                    LStrServerData013 = LDataRowCtiHubServer[0]["C013"].ToString();
                    LStrServerData014 = LDataRowCtiHubServer[0]["C014"].ToString();
                    LStrServerData014 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData014, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData017 = LDataRowCtiHubServer[0]["C017"].ToString();
                    LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData018 = LDataRowCtiHubServer[0]["C018"].ToString();
                    LStrServerData018 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData018, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                    TreeViewItem LTreeViewItemServer1 = new TreeViewItem();
                    if (LStrServerData011 == "0")
                    {
                        LTreeViewItemServer1.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + App.GetDisplayCharater("1110012");
                    }
                    else
                    {
                        LTreeViewItemServer1.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + LStrServerData017;// +":" + LStrServerData014;
                    }
                    LTreeViewItemServer1.DataContext = "2130000000000000001";
                    TreeViewItemProps.SetItemImageName(LTreeViewItemServer1, IStrImageFolder + @"\" + LStrIconPngName);
                    LTreeViewItemSingleResurce.Items.Add(LTreeViewItemServer1);
                    #endregion

                    #region 显示Server2
                    LStrServerData011 = LDataRowCtiHubServer[1]["C011"].ToString();
                    LStrServerData012 = LDataRowCtiHubServer[1]["C012"].ToString();
                    LStrServerData013 = LDataRowCtiHubServer[1]["C013"].ToString();
                    LStrServerData014 = LDataRowCtiHubServer[1]["C014"].ToString();
                    LStrServerData014 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData014, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData017 = LDataRowCtiHubServer[1]["C017"].ToString();
                    LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData018 = LDataRowCtiHubServer[1]["C018"].ToString();
                    LStrServerData018 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData018, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                    TreeViewItem LTreeViewItemServer2 = new TreeViewItem();
                    if (LStrServerData011 == "0")
                    {
                        LTreeViewItemServer2.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + App.GetDisplayCharater("1110012");
                    }
                    else
                    {
                        LTreeViewItemServer2.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + LStrServerData017;// +":" + LStrServerData014;
                    }
                    LTreeViewItemServer2.DataContext = "2130000000000000002";
                    TreeViewItemProps.SetItemImageName(LTreeViewItemServer2, IStrImageFolder + @"\" + LStrIconPngName);
                    LTreeViewItemSingleResurce.Items.Add(LTreeViewItemServer2);
                    #endregion

                    LTreeViewItemSingleResurce.IsExpanded = true;
                    break;
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }
        #endregion

        #region 显示资源详细信息
        private void ShowTypeObjectDetails(string AStrItemData)
        {
            string LStrTypeID = string.Empty;
            string LStrTypeIDLeft3 = string.Empty;

            LStrTypeID = AStrItemData;

            if (IOperationEvent != null)
            {
                OperationEventArgs LEventArgs = new OperationEventArgs();
                LEventArgs.StrObjectTag = "M-Save";
                IOperationEvent(this, LEventArgs);
            }

            GridResourceDataDetail.Children.Clear();
            LabelResourceName.Content = string.Empty;

            IOperationEvent = null;

            LStrTypeIDLeft3 = AStrItemData.Substring(0, 3);
            if (LStrTypeIDLeft3 == "211") { ShowType211ObjectDetails(AStrItemData); return; }
            if (LStrTypeIDLeft3 == "212") { ShowType212ObjectDetails(AStrItemData); return; }
            if (LStrTypeIDLeft3 == "213") { ShowType213ObjectDetails(AStrItemData); return; }
        }

        private void ShowType211ObjectDetails(string AStrItemData)
        {
            LabelResourceName.Content = App.GetDisplayCharater("1110004");
            IUCResourceType211.IPageTopParent = this;
            IUCResourceType211.ShowElementContent();
            IUCResourceType211.ShowSettedData(App.IListDataSetReturn[0]);
            GridResourceDataDetail.Children.Add(IUCResourceType211);
        }

        private void ShowType212ObjectDetails(string AStrItemData)
        {
            LabelResourceName.Content = App.GetDisplayCharater("1110005");

            if (AStrItemData.Length == 3)
            {
                IUCResourceType212A.IPageTopParent = this;
                IUCResourceType212A.ShowElementContent();
                IUCResourceType212A.IOperationEvent += IUCResourceType212A_IOperationEvent;
                IUCResourceType212A.ShowSettedData(App.IListDataSetReturn[1]);
                GridResourceDataDetail.Children.Add(IUCResourceType212A);
            }
            else
            {
                IUCResourceType212B.IPageTopParent = this;
                IUCResourceType212B.ShowElementContent();
                IUCResourceType212B.ShowSettedData(App.IListDataSetReturn[1], AStrItemData);
                GridResourceDataDetail.Children.Add(IUCResourceType212B);
            }
        }

        private void ShowType213ObjectDetails(string AStrItemData)
        {
            LabelResourceName.Content = App.GetDisplayCharater("1110006");
            IUCResourceType213.IPageTopParent = this;
            IUCResourceType213.ShowElementContent();
            IUCResourceType213.ShowSettedData(App.IListDataSetReturn[2]);
            GridResourceDataDetail.Children.Add(IUCResourceType213);
        }
        #endregion

        #region 显示资源对应的操作
        private void ShowTypeObjectOperations(string AStrItemData)
        {
            string LStrTypeID = string.Empty;
            string LStrTypeIDLeft3 = string.Empty;

            LStrTypeID = AStrItemData;
            LStrTypeIDLeft3 = AStrItemData.Substring(0, 3);
            StackPanelObjectOperations.Children.Clear();

            if (LStrTypeIDLeft3 == "211") { ShowType211ObjectOperations(AStrItemData); return; }
            if (LStrTypeIDLeft3 == "212") { ShowType212ObjectOperations(AStrItemData); return; }
            if (LStrTypeIDLeft3 == "213") { ShowType213ObjectOperations(AStrItemData); return; }
        }

        private void ShowType211ObjectOperations(string AStrItemData)
        {
            if (App.IDataTableOperation.Select("C002 = 111021102").Length > 0)
            { IUCResourceType211.IBoolCanEdit = true; }
            else { IUCResourceType211.IBoolCanEdit = false; }

            if (App.IDataTableOperation.Select("C002 = 111021199").Length > 0)
            {
                UCValidationData LUCValidationDataOperation = new UCValidationData("1110999");
                LUCValidationDataOperation.ShowOperationDetails(null);
                LUCValidationDataOperation.Margin = new Thickness(0, 1, 0, 1);
                LUCValidationDataOperation.IOperationEvent += LUCObjectOperationsEvent;
                StackPanelObjectOperations.Children.Add(LUCValidationDataOperation);
            }
        }

        private void ShowType212ObjectOperations(string AStrItemData)
        {
            if (App.IDataTableOperation.Select("C002 = 111021202").Length > 0)
            { IUCResourceType212B.IBoolCanEdit = true; }
            else { IUCResourceType212B.IBoolCanEdit = false; }

            if (App.IDataTableOperation.Select("C002 = 111021299").Length > 0)
            {
                UCValidationData LUCValidationDataOperation = new UCValidationData("1110999");
                LUCValidationDataOperation.ShowOperationDetails(null);
                LUCValidationDataOperation.Margin = new Thickness(0, 1, 0, 1);
                LUCValidationDataOperation.IOperationEvent += LUCObjectOperationsEvent;
                StackPanelObjectOperations.Children.Add(LUCValidationDataOperation);
            }

            TreeViewItem LTreeViewItemCurrent = TreeViewResourceList.SelectedItem as TreeViewItem;
            OperationParameters LOperationParameters = new OperationParameters();
            List<string> LListStrOperationID = new List<string>();

            //选择DEC根级目录
            if (AStrItemData.Length == 3)
            {
                LListStrOperationID.Add("111021202");
                LListStrOperationID.Add("");
                LListStrOperationID.Add("111021206");
                LListStrOperationID.Add("111021207");
                LOperationParameters.StrObjectTag = "212";
                LOperationParameters.ObjectSource1 = App.IDataTableOperation;
                LOperationParameters.ObjectSource2 = LListStrOperationID;
                LOperationParameters.ObjectSource3 = LTreeViewItemCurrent;

                UCObjectOperationGroup LUCObjectOperationGroup = new UCObjectOperationGroup();
                LUCObjectOperationGroup.ShowObjectAllOperations(LOperationParameters);
                LUCObjectOperationGroup.IOperationEvent += LUCObjectOperationsEvent;
                LUCObjectOperationGroup.Margin = new Thickness(0, 1, 0, 1);
                StackPanelObjectOperations.Children.Add(LUCObjectOperationGroup);
                return;
            }
            else
            {
                TreeViewItem LTreeViewItemParent = LTreeViewItemCurrent.Parent as TreeViewItem;

                LListStrOperationID.Add("111021206");
                LOperationParameters.StrObjectTag = "212";
                LOperationParameters.ObjectSource1 = App.IDataTableOperation;
                LOperationParameters.ObjectSource2 = LListStrOperationID;
                LOperationParameters.ObjectSource3 = LTreeViewItemParent;

                UCObjectOperationGroup LUCObjectOperationGroup1 = new UCObjectOperationGroup();
                LUCObjectOperationGroup1.ShowObjectAllOperations(LOperationParameters);
                LUCObjectOperationGroup1.IOperationEvent += LUCObjectOperationsEvent;
                LUCObjectOperationGroup1.Margin = new Thickness(0, 1, 0, 1);
                StackPanelObjectOperations.Children.Add(LUCObjectOperationGroup1);

                LListStrOperationID.Clear();
                LListStrOperationID.Add("111021207");
                LOperationParameters.StrObjectTag = "212";
                LOperationParameters.ObjectSource1 = App.IDataTableOperation;
                LOperationParameters.ObjectSource2 = LListStrOperationID;
                LOperationParameters.ObjectSource3 = LTreeViewItemCurrent;

                UCObjectOperationGroup LUCObjectOperationGroup2 = new UCObjectOperationGroup();
                LUCObjectOperationGroup2.ShowObjectAllOperations(LOperationParameters);
                LUCObjectOperationGroup2.IOperationEvent += LUCObjectOperationsEvent;
                LUCObjectOperationGroup2.Margin = new Thickness(0, 1, 0, 1);
                StackPanelObjectOperations.Children.Add(LUCObjectOperationGroup2);
            }


        }

        private void ShowType213ObjectOperations(string AStrItemData)
        {
            if (App.IDataTableOperation.Select("C002 = 111021302").Length > 0)
            { IUCResourceType211.IBoolCanEdit = true; }
            else { IUCResourceType211.IBoolCanEdit = false; }

            if (App.IDataTableOperation.Select("C002 = 111021399").Length > 0)
            {
                UCValidationData LUCValidationDataOperation = new UCValidationData("1110999");
                LUCValidationDataOperation.ShowOperationDetails(null);
                LUCValidationDataOperation.Margin = new Thickness(0, 1, 0, 1);
                LUCValidationDataOperation.IOperationEvent += LUCObjectOperationsEvent;
                StackPanelObjectOperations.Children.Add(LUCValidationDataOperation);
            }
        }
        #endregion

        #region 刷新TreeView数据
        public void RefreshTreeViewViewData(OperationEventArgs AEventArgs)
        {
            if (AEventArgs.StrObjectTag == "S211") { RefreshType211ViewData(AEventArgs); return; }
            if (AEventArgs.StrObjectTag == "S212") { RefreshType212ViewData(AEventArgs); return; }
            if (AEventArgs.StrObjectTag == "S213") { RefreshType213ViewData(AEventArgs); return; }
        }

        private void RefreshType211ViewData(OperationEventArgs AEventArgs)
        {
            string LStrDataContext = string.Empty;

            string LStrServerData011 = string.Empty;
            string LStrServerData012 = string.Empty;
            string LStrServerData013 = string.Empty;
            string LStrServerData014 = string.Empty;
            string LStrServerData017 = string.Empty;
            string LStrServerData018 = string.Empty;

            foreach (TreeViewItem LTreeViewItemSingleResurce in TreeViewResourceList.Items)
            {
                if (LTreeViewItemSingleResurce.DataContext.ToString() != "211") { continue; }
                foreach (TreeViewItem LTreeViewItemSingleLicenseServer in LTreeViewItemSingleResurce.Items)
                {
                    LStrDataContext = LTreeViewItemSingleLicenseServer.DataContext.ToString();
                    DataRow[] LDataRowLicenseServer = App.IListDataSetReturn[0].Tables[0].Select("C001 = " + LStrDataContext);

                    LStrServerData011 = LDataRowLicenseServer[0]["C011"].ToString();
                    LStrServerData012 = LDataRowLicenseServer[0]["C012"].ToString();
                    LStrServerData013 = LDataRowLicenseServer[0]["C013"].ToString();
                    LStrServerData014 = LDataRowLicenseServer[0]["C014"].ToString();
                    LStrServerData014 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData014, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData017 = LDataRowLicenseServer[0]["C017"].ToString();
                    LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData018 = LDataRowLicenseServer[0]["C018"].ToString();
                    LStrServerData018 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData018, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (LStrServerData011 == "0")
                    {
                        LTreeViewItemSingleLicenseServer.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + App.GetDisplayCharater("1110012");
                    }
                    else
                    {
                        LTreeViewItemSingleLicenseServer.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + LStrServerData017;
                    }
                }

                break;
            }
        }

        private void RefreshType212ViewData(OperationEventArgs AEventArgs)
        {
            string LStrDataContext = string.Empty;

            string LStrServerData001 = string.Empty;
            string LStrServerData011 = string.Empty;
            string LStrServerData012 = string.Empty;
            string LStrServerData013 = string.Empty;
            string LStrServerData014 = string.Empty;
            string LStrServerData015 = string.Empty;
            string LStrServerData016 = string.Empty;
            string LStrServerData017 = string.Empty;
            string LStrServerData018 = string.Empty;

            foreach (TreeViewItem LTreeViewItemSingleResurce in TreeViewResourceList.Items)
            {
                if (LTreeViewItemSingleResurce.DataContext.ToString() != "212") { continue; }
                foreach (TreeViewItem LTreeViewItemSingleDecServer in LTreeViewItemSingleResurce.Items)
                {
                    LStrDataContext = LTreeViewItemSingleDecServer.DataContext.ToString();
                    if (LStrDataContext != AEventArgs.ObjectSource0.ToString()) { continue; }

                    DataRow[] LDataRowDecServer = App.IListDataSetReturn[1].Tables[0].Select("C001 = " + LStrDataContext);

                    LStrServerData017 = LDataRowDecServer[0]["C017"].ToString();
                    LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                    LTreeViewItemSingleDecServer.Header = LStrServerData017;
                    break;
                }
                break;
            }
        }

        private void RefreshType213ViewData(OperationEventArgs AEventArgs)
        {
            string LStrDataContext = string.Empty;

            string LStrServerData011 = string.Empty;
            string LStrServerData012 = string.Empty;
            string LStrServerData013 = string.Empty;
            string LStrServerData014 = string.Empty;
            string LStrServerData017 = string.Empty;
            string LStrServerData018 = string.Empty;

            foreach (TreeViewItem LTreeViewItemSingleResurce in TreeViewResourceList.Items)
            {
                if (LTreeViewItemSingleResurce.DataContext.ToString() != "213") { continue; }
                foreach (TreeViewItem LTreeViewItemSingleCtiHubServer in LTreeViewItemSingleResurce.Items)
                {
                    LStrDataContext = LTreeViewItemSingleCtiHubServer.DataContext.ToString();
                    DataRow[] LDataRowCtiHubServer = App.IListDataSetReturn[2].Tables[0].Select("C001 = " + LStrDataContext);

                    LStrServerData011 = LDataRowCtiHubServer[0]["C011"].ToString();
                    LStrServerData012 = LDataRowCtiHubServer[0]["C012"].ToString();
                    LStrServerData013 = LDataRowCtiHubServer[0]["C013"].ToString();
                    LStrServerData014 = LDataRowCtiHubServer[0]["C014"].ToString();
                    LStrServerData014 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData014, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData017 = LDataRowCtiHubServer[0]["C017"].ToString();
                    LStrServerData017 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData017, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrServerData018 = LDataRowCtiHubServer[0]["C018"].ToString();
                    LStrServerData018 = EncryptionAndDecryption.EncryptDecryptString(LStrServerData018, IStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (LStrServerData011 == "0")
                    {
                        LTreeViewItemSingleCtiHubServer.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + App.GetDisplayCharater("1110012");
                    }
                    else
                    {
                        LTreeViewItemSingleCtiHubServer.Header = "(" + App.GetDisplayCharater("UCResourceType211", "C012V" + LStrServerData012) + ") " + LStrServerData017;
                    }
                }

                break;
            }
        }
        #endregion

        #region 处理DecServer
        private void IUCResourceType212A_IOperationEvent(object sender, OperationEventArgs e)
        {
            string LStrDecID = string.Empty;

            if (e.StrObjectTag == "M-DEC01")
            {
                LStrDecID = e.ObjectSource0.ToString();
                foreach (TreeViewItem LTreeViewItemSingleResurce in TreeViewResourceList.Items)
                {
                    if (LTreeViewItemSingleResurce.DataContext.ToString() != "212") { continue; }
                    foreach (TreeViewItem LTreeViewItemSingleDecServer in LTreeViewItemSingleResurce.Items)
                    {
                        if (LTreeViewItemSingleDecServer.DataContext.ToString() != LStrDecID) { continue; }
                        LTreeViewItemSingleDecServer.IsSelected = true;
                        LTreeViewItemSingleDecServer.BringIntoView();
                    }
                    break;
                }
                return;
            }
        }
        #endregion

        #region 检测数据是否完整，初始化数据
        private void CheckAndInitType211Data()
        {
            DataRow[] LDataRowLicenseServerCheck = App.IListDataSetReturn[0].Tables[0].Select("C001 = 2110000000000000001 OR C001 = 2110000000000000002", "C001 ASC");
            if (LDataRowLicenseServerCheck.Length <= 0)
            {
                DataRow LDataRowNew1 = App.IListDataSetReturn[0].Tables[0].NewRow();
                LDataRowNew1.BeginEdit();
                LDataRowNew1["C001"] = 2110000000000000001;
                LDataRowNew1["C002"] = 1;
                LDataRowNew1["C011"] = "0";
                LDataRowNew1["C012"] = "1";
                LDataRowNew1["C013"] = "I";
                LDataRowNew1["C014"] = EncryptionAndDecryption.EncryptDecryptString("3070", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LDataRowNew1["C017"] = EncryptionAndDecryption.EncryptDecryptString("0.0.0.0", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004); ;
                LDataRowNew1["C018"] = "";
                LDataRowNew1.EndEdit();
                App.IListDataSetReturn[0].Tables[0].Rows.Add(LDataRowNew1);

                DataRow LDataRowNew2 = App.IListDataSetReturn[0].Tables[0].NewRow();
                LDataRowNew2.BeginEdit();
                LDataRowNew2["C001"] = 2110000000000000002;
                LDataRowNew2["C002"] = 1;
                LDataRowNew2["C011"] = "0";
                LDataRowNew2["C012"] = "2";
                LDataRowNew2["C013"] = "I";
                LDataRowNew2["C014"] = EncryptionAndDecryption.EncryptDecryptString("3070", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LDataRowNew2["C017"] = EncryptionAndDecryption.EncryptDecryptString("0.0.0.0", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004); ;
                LDataRowNew2["C018"] = "";
                LDataRowNew2.EndEdit();
                App.IListDataSetReturn[0].Tables[0].Rows.Add(LDataRowNew2);
            }
        }

        private void CheckAndInitType212Data()
        {
            DataRow[] LDataRowDECServerCheck = App.IListDataSetReturn[1].Tables[0].Select("C001 >= 2120000000000000001 AND C001 < 2130000000000000000");
            if (LDataRowDECServerCheck.Length > 0) { return; }
            for (int LIntLoopServer = 1; LIntLoopServer <= 15; LIntLoopServer++)
            {
                DataRow LDataRowNew = App.IListDataSetReturn[1].Tables[0].NewRow();
                LDataRowNew.BeginEdit();
                LDataRowNew["C001"] = 2120000000000000000 + LIntLoopServer;
                LDataRowNew["C002"] = 1;
                LDataRowNew["C011"] = "0";
                LDataRowNew["C012"] = "1";
                LDataRowNew["C013"] = "I";
                LDataRowNew["C014"] = EncryptionAndDecryption.EncryptDecryptString("3072", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LDataRowNew["C015"] = EncryptionAndDecryption.EncryptDecryptString("256", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LDataRowNew["C016"] = EncryptionAndDecryption.EncryptDecryptString("1", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LDataRowNew["C017"] = EncryptionAndDecryption.EncryptDecryptString("0.0.0.0", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004); ;
                LDataRowNew["C018"] = "";
                LDataRowNew.EndEdit();
                App.IListDataSetReturn[1].Tables[0].Rows.Add(LDataRowNew);
            }
        }

        private void CheckAndInitType213Data()
        {
            DataRow[] LDataRowLicenseServerCheck = App.IListDataSetReturn[2].Tables[0].Select("C001 = 2130000000000000001 OR C001 = 2130000000000000002", "C001 ASC");
            if (LDataRowLicenseServerCheck.Length <= 0)
            {
                DataRow LDataRowNew1 = App.IListDataSetReturn[2].Tables[0].NewRow();
                LDataRowNew1.BeginEdit();
                LDataRowNew1["C001"] = 2130000000000000001;
                LDataRowNew1["C002"] = 1;
                LDataRowNew1["C011"] = "0";
                LDataRowNew1["C012"] = "1";
                LDataRowNew1["C013"] = "I";
                LDataRowNew1["C014"] = EncryptionAndDecryption.EncryptDecryptString("3420", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LDataRowNew1["C017"] = EncryptionAndDecryption.EncryptDecryptString("0.0.0.0", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004); ;
                LDataRowNew1["C018"] = "";
                LDataRowNew1.EndEdit();
                App.IListDataSetReturn[2].Tables[0].Rows.Add(LDataRowNew1);

                DataRow LDataRowNew2 = App.IListDataSetReturn[2].Tables[0].NewRow();
                LDataRowNew2.BeginEdit();
                LDataRowNew2["C001"] = 2130000000000000002;
                LDataRowNew2["C002"] = 1;
                LDataRowNew2["C011"] = "0";
                LDataRowNew2["C012"] = "2";
                LDataRowNew2["C013"] = "I";
                LDataRowNew2["C014"] = EncryptionAndDecryption.EncryptDecryptString("3420", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LDataRowNew2["C017"] = EncryptionAndDecryption.EncryptDecryptString("0.0.0.0", IStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004); ;
                LDataRowNew2["C018"] = "";
                LDataRowNew2.EndEdit();
                App.IListDataSetReturn[2].Tables[0].Rows.Add(LDataRowNew2);
            }
        }
        #endregion

        #region 处理右侧操作发送过来的消息
        private void LUCObjectOperationsEvent(object sender, OperationEventArgs e)
        {
            MessageBox.Show(e.StrObjectTag);
        }
        #endregion
        
    }
}
