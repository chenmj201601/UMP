using PFShareClassesC;
using PFShareControls;
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
using UMPS1103.WCFService00000;
using VoiceCyber.UMP.Communications;

namespace UMPS1103
{
    public partial class UCAgentMaintenance : UserControl, INotifyPropertyChanged
    {
        public PageMainEntrance IPageParent = null;

        private bool IBoolIsBusy = false;

        public string IStrCurrentMethod = string.Empty;
        public UCAgentBasicInformation IUCBasicInfo = new UCAgentBasicInformation();
        public UCAgentSkillGroupInformation IUCSkillInfo = new UCAgentSkillGroupInformation();
        public UCAgentManagerList IUCManger = new UCAgentManagerList();

        private List<TreeViewItem> IListTVIOrg = new List<TreeViewItem>();
        private List<TreeViewItem> IListTVIAgent = new List<TreeViewItem>();

        public UCAgentMaintenance()
        {
            InitializeComponent();
            IStrCurrentMethod = "V";
            this.Loaded += UCAgentMaintenance_Loaded;
            TreeViewOrgAgent.SelectedItemChanged += TreeViewOrgAgent_SelectedItemChanged;

            ButtonEditApply.Click += ButtonEditApplyCancelClick;
            ButtonCancelEdit.Click += ButtonEditApplyCancelClick;
            ButtonDelAgent.Click += ButtonEditApplyCancelClick;
            ButtonAddAgent.Click += ButtonEditApplyCancelClick;
        }

        #region 按钮事件处理部分
        private void ButtonEditApplyCancelClick(object sender, RoutedEventArgs e)
        {
            string LStrSenderName = string.Empty;

            try
            {
                if (IBoolIsBusy) { return; }
                LStrSenderName = (sender as Button).Name;
                if (LStrSenderName == "ButtonEditApply")
                {
                    if (IStrCurrentMethod == "V") { ShowAgentEditStyle(); return; }
                    if (IStrCurrentMethod == "E" || IStrCurrentMethod == "A") { SaveAgentInformation(); return; }
                }
                if (LStrSenderName == "ButtonCancelEdit")
                {
                    ShowAgentViewStyle();
                    return;
                }
                if (LStrSenderName == "ButtonAddAgent")
                {
                    AddNewAgent();
                    return;
                }
                if (LStrSenderName == "ButtonDelAgent")
                {
                    RemoveSingleAgent();
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void ShowAgentEditStyle()
        {
            IStrCurrentMethod = "E";

            ButtonEditApply.Style = (Style)App.Current.Resources["ButtonApplyStyle"];
            StrEditApply = App.GetDisplayCharater("UCAgentMaintenance", "ButtonApply");
            ButtonCancelEdit.Visibility = System.Windows.Visibility.Visible;
            IUCBasicInfo.EnabledOrDisabledElement(true);
            IUCSkillInfo.EnabledOrDisabledElement(true);
            IUCManger.EnabledOrDisabledElement(true);
        }

        private void AddNewAgent()
        {
            IStrCurrentMethod = "A";

            ButtonEditApply.Style = (Style)App.Current.Resources["ButtonApplyStyle"];
            StrEditApply = App.GetDisplayCharater("UCAgentMaintenance", "ButtonApply");
            ButtonCancelEdit.Visibility = System.Windows.Visibility.Visible;
            IUCBasicInfo.EnabledOrDisabledElement(true);

            BorderSingeAgentDetail.Visibility = System.Windows.Visibility.Visible;

            StackPanelAgentProperties.Children.Clear();

            IUCBasicInfo.BorderThickness = new Thickness(1, 1, 1, 1);
            IUCBasicInfo.BorderBrush = Brushes.LightGray;
            StackPanelAgentProperties.Children.Add(IUCBasicInfo);

        }
        #endregion

        #region 保存数据至数据库
        private BackgroundWorker IBWSaveData = null;
        private bool IBoolCallReturn = true;
        private string IStrCallReturn = string.Empty;
        private List<string> IListStrAfterSave = new List<string>();

        private void SaveAgentInformation()
        {
            /// 0：数据库类型
            /// 1：数据库连接串
            /// 2：租户编码（5位）
            /// 3：用户编码（19位）
            /// 4：方法（E：修改；D：删除；A：增加）
            /// 5：座席编码（19位。如果方法 = ‘A’，该值为 0或String.Empty）
            /// 6：座席号
            /// 7～N：BXX，属性ID("00"格式) + char(27) + 实际数据；S00 + char(27) + 技能组ID； U00 + char(27) + 用户ID
            /// 
            List<string> LListStrWcfArgs = new List<string>();
            string LStrItemData = string.Empty;
            string LStrSAUserID = string.Empty;

            try
            {
                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());                                  //0
                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());                              //1
                LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);                                                  //2
                LListStrWcfArgs.Add(App.GClassSessionInfo.UserInfo.UserID.ToString());                                      //3
                LListStrWcfArgs.Add(IStrCurrentMethod);                                                                     //4
                if (IStrCurrentMethod == "A"){ LListStrWcfArgs.Add("0"); }                                                  //5
                else
                {
                    TreeViewItem LTreeViewItemCurrent = TreeViewOrgAgent.SelectedItem as TreeViewItem;
                    if (LTreeViewItemCurrent == null) { return; }
                    LStrItemData = LTreeViewItemCurrent.DataContext.ToString();
                    if (LStrItemData.Substring(0, 3) != "103") { return; }
                    LListStrWcfArgs.Add(LStrItemData);
                }

                List<string> LListStrBasicInfo = IUCBasicInfo.GetElementSettedData();
                if (LListStrBasicInfo.Count <= 0) { return; }
                LListStrWcfArgs.Add(LListStrBasicInfo[1].Substring(4));                                                     //6

                foreach (string LStrSingleData in LListStrBasicInfo) { LListStrWcfArgs.Add(LStrSingleData); }
                if (IStrCurrentMethod == "E")
                {
                    List<string> LListStrSkillInfo = IUCSkillInfo.GetElementSettedData();
                    List<string> LListStrManagerInfo = IUCManger.GetElementSettedData();

                    foreach (string LStrSingleData in LListStrSkillInfo) { LListStrWcfArgs.Add(LStrSingleData); }
                    foreach (string LStrSingleData in LListStrManagerInfo)
                    {
                        if (LStrSingleData.Substring(0, 7) != "U00" + App.AscCodeToChr(27) + "102") { continue; }
                        LListStrWcfArgs.Add(LStrSingleData);
                    }
                }
                else
                {
                    LStrSAUserID = "102" + App.GClassSessionInfo.RentInfo.Token + "00000000001";
                    LListStrWcfArgs.Add("U00" + App.AscCodeToChr(27) + LStrSAUserID);
                    if (App.GClassSessionInfo.UserInfo.UserID.ToString() != LStrSAUserID)
                    {
                        LListStrWcfArgs.Add("U00" + App.AscCodeToChr(27) + App.GClassSessionInfo.UserInfo.UserID.ToString());
                    }
                }

                //foreach (string LStrTemp in LListStrWcfArgs) { MessageBox.Show(LStrTemp); }

                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12111;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                IBoolIsBusy = true;

                TreeViewOrgAgent.IsEnabled = false;
                IBWSaveData = new BackgroundWorker();
                IBWSaveData.RunWorkerCompleted += IBWSaveData_RunWorkerCompleted;
                IBWSaveData.DoWork += IBWSaveData_DoWork;
                IBWSaveData.RunWorkerAsync(LListStrWcfArgs);
            }
            catch (Exception ex)
            {
                IBoolIsBusy = false;
                TreeViewOrgAgent.IsEnabled = true;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12112;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                if (IBWSaveData != null)
                {
                    IBWSaveData.Dispose(); IBWSaveData = null;
                }
                MessageBox.Show(ex.ToString(),"1");
            }
        }

        private void RemoveSingleAgent()
        {
            List<string> LListStrWcfArgs = new List<string>();
            string LStrItemData = string.Empty;

            try
            {
                TreeViewItem LTreeViewItemCurrent = TreeViewOrgAgent.SelectedItem as TreeViewItem;
                if (LTreeViewItemCurrent == null) { return; }
                LStrItemData = LTreeViewItemCurrent.DataContext.ToString();
                if (LStrItemData.Substring(0, 3) != "103") { return; }

                if (MessageBox.Show(string.Format(App.GetDisplayCharater("S1103021"), LTreeViewItemCurrent.Header.ToString()), App.GClassSessionInfo.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes) { return; }

                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.TypeID.ToString());                                  //0
                LListStrWcfArgs.Add(App.GClassSessionInfo.DatabaseInfo.GetConnectionString());                              //1
                LListStrWcfArgs.Add(App.GClassSessionInfo.RentInfo.Token);                                                  //2
                LListStrWcfArgs.Add(App.GClassSessionInfo.UserInfo.UserID.ToString());                                      //3
                LListStrWcfArgs.Add("D");                                                                                   //4
                LListStrWcfArgs.Add(LStrItemData);                                                                          //5

                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12111;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                IBoolIsBusy = true;

                TreeViewOrgAgent.IsEnabled = false;
                IBWSaveData = new BackgroundWorker();
                IBWSaveData.RunWorkerCompleted += IBWSaveData_RunWorkerCompleted;
                IBWSaveData.DoWork += IBWSaveData_DoWork;
                IBWSaveData.RunWorkerAsync(LListStrWcfArgs);
            }
            catch(Exception ex)
            {
                IBoolIsBusy = false;
                TreeViewOrgAgent.IsEnabled = true;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12112;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);
                if (IBWSaveData != null)
                {
                    IBWSaveData.Dispose(); IBWSaveData = null;
                }
                MessageBox.Show(ex.ToString());
            }

        }

        private void IBWSaveData_DoWork(object sender, DoWorkEventArgs e)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                LListStrWcfArgs = e.Argument as List<string>;

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(27, LListStrWcfArgs);
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                IListStrAfterSave.Clear();
                if (IBoolCallReturn)
                {
                    foreach (string LStrSingleArgs in LListStrWcfArgs) { IListStrAfterSave.Add(LStrSingleArgs); }
                    IListStrAfterSave[5] = IStrCallReturn;
                    App.LoadAboutAgentData();
                }
            }
            catch (Exception ex)
            {
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
            }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); LService00000Client = null; }
                }
            }
        }

        private void IBWSaveData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                IBoolIsBusy = false;
                TreeViewOrgAgent.IsEnabled = true;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12112;
                WebReturn LWebReturn = App.SendNetPipeMessage(LWebRequestClientLoading);

                if (!IBoolCallReturn)
                {
                    if (IStrCallReturn.Contains("W000E"))
                    {
                        MessageBox.Show(App.GetDisplayCharater("UCAgentMaintenance", IStrCallReturn), App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    else
                    {
                        MessageBox.Show(App.GetDisplayCharater("S1103018") + "\n" + IStrCallReturn, App.GClassSessionInfo.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    return;
                }
                ResetAgentTreeViewList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"2");
            }
            finally
            {
                if (IBWSaveData != null) { IBWSaveData.Dispose(); IBWSaveData = null; }
            }
        }

        private void ResetAgentTreeViewList()
        {
            try
            {
                if (IListStrAfterSave[4] == "E" || IListStrAfterSave[4] == "A")
                {
                    ShowControlOrgAgent();
                    foreach (TreeViewItem LSingleItem in IListTVIAgent)
                    {
                        if (LSingleItem.DataContext.ToString() == IListStrAfterSave[5])
                        {
                            LSingleItem.IsSelected = true;
                            LSingleItem.BringIntoView();
                            break;
                        }
                    }

                    return;
                }
                if (IListStrAfterSave[4] == "D")
                {
                    TreeViewItem LTreeViewItemCurrent = TreeViewOrgAgent.SelectedItem as TreeViewItem;
                    if (LTreeViewItemCurrent == null) { return; }
                    TreeViewItem LTreeViewItemParent = LTreeViewItemCurrent.Parent as TreeViewItem;
                    LTreeViewItemParent.Items.Remove(LTreeViewItemCurrent);
                    foreach (TreeViewItem LSingleItem in IListTVIAgent)
                    {
                        if (LSingleItem.DataContext.ToString() == IListStrAfterSave[5]) { IListTVIAgent.Remove(LSingleItem); break; }
                    }

                    DataRow[] LDataRowAgent = App.IDataTable11101.Select("C001 = " + IListStrAfterSave[5]);
                    foreach (DataRow LSingleAgentProperty in LDataRowAgent) { App.IDataTable11101.Rows.Remove(LSingleAgentProperty); }
                    return;
                }
            }
            catch { }
        }
        #endregion

        private void ShowAgentViewStyle()
        {
            string LStrItemData = string.Empty;

            try
            {
                IStrCurrentMethod = "V";
                StrEditApply = App.GetDisplayCharater("UCAgentMaintenance", "ButtonEdit");
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonEditStyle"];
                ButtonCancelEdit.Visibility = System.Windows.Visibility.Collapsed;

                TreeViewItem LTreeViewItemCurrent = TreeViewOrgAgent.SelectedItem as TreeViewItem;
                if (LTreeViewItemCurrent == null)
                {
                    BorderSingeAgentDetail.Visibility = System.Windows.Visibility.Collapsed;
                    return;
                }
                LStrItemData = LTreeViewItemCurrent.DataContext.ToString();
                if (LStrItemData.Substring(0, 3) == "103")
                {
                    BorderSingeAgentDetail.Visibility = System.Windows.Visibility.Visible;
                    IUCBasicInfo.EnabledOrDisabledElement(false);
                    IUCSkillInfo.EnabledOrDisabledElement(false);
                    IUCManger.EnabledOrDisabledElement(false);

                    IUCBasicInfo.ShowInformation(LStrItemData);
                    IUCSkillInfo.ShowInformation(LStrItemData);
                    IUCManger.ShowInformation(LStrItemData);

                    StackPanelAgentProperties.Children.Clear();

                    IUCBasicInfo.BorderThickness = new Thickness(1, 1, 1, 1);
                    IUCBasicInfo.BorderBrush = Brushes.LightGray;
                    StackPanelAgentProperties.Children.Add(IUCBasicInfo);

                    IUCSkillInfo.Margin = new Thickness(0, 5, 0, 0);
                    IUCSkillInfo.BorderThickness = new Thickness(1, 1, 1, 1);
                    IUCSkillInfo.BorderBrush = Brushes.LightGray;
                    StackPanelAgentProperties.Children.Add(IUCSkillInfo);

                    IUCManger.Margin = new Thickness(0, 5, 0, 0);
                    IUCManger.BorderThickness = new Thickness(1, 1, 1, 1);
                    IUCManger.BorderBrush = Brushes.LightGray;
                    StackPanelAgentProperties.Children.Add(IUCManger);
                }
                else
                {
                    BorderSingeAgentDetail.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
            catch { }

            
        }

        private void TreeViewOrgAgent_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            ShowAgentViewStyle();
        }

        private void UCAgentMaintenance_Loaded(object sender, RoutedEventArgs e)
        {
            IPageParent.IChangeLanguageEvent += IPageParent_IChangeLanguageEvent;
        }

        private void IPageParent_IChangeLanguageEvent(object sender, OperationEventArgs e)
        {
            ShowElementContent();
            IUCBasicInfo.ShowElementContent();
            IUCSkillInfo.ShowElementContent();
            IUCManger.ShowElementContent();
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            this.DataContext = this;
        }

        public void ShowElementContent()
        {
            LabelAllAgentName.Content = App.GetDisplayCharater("S1103001");

            if (IStrCurrentMethod == "V")
            {
                StrEditApply = App.GetDisplayCharater("UCAgentMaintenance", "ButtonEdit");
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonEditStyle"];
            }
            else
            {
                StrEditApply = App.GetDisplayCharater("UCAgentMaintenance", "ButtonApply");
                ButtonEditApply.Style = (Style)App.Current.Resources["ButtonApplyStyle"];
            }

            StrAddAgent = App.GetDisplayCharater("S1103023");
            StrDelAgent = App.GetDisplayCharater("S1103024");

            StrCancel = App.GetDisplayCharater("UCAgentMaintenance", "ButtonCancel");
        }

        public void ShowControlOrgAgent()
        {
            string LStrMyOrg = string.Empty;
            string LStrOrgID = string.Empty;
            string LStrOrgName = string.Empty;
            string LStrOrgParent = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrImagesPath = string.Empty;

            try
            {
                TreeViewOrgAgent.Items.Clear();
                IListTVIOrg.Clear();
                IListTVIAgent.Clear();
                

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
                TreeViewOrgAgent.Items.Add(LTreeViewItemRoot);
                IListTVIOrg.Add(LTreeViewItemRoot);
                ShowOrgAgent(LTreeViewItemRoot, LStrOrgID);

                LTreeViewItemRoot.IsExpanded = true;
                LTreeViewItemRoot.Focus();
                LTreeViewItemRoot.BringIntoView();

                IUCBasicInfo.IPageParent = this;
                IUCSkillInfo.IPageParent = this;
                IUCManger.IPageParent = this;

                //StackPanelAgentProperties.Children.Clear();

                //IUCBasicInfo.BorderThickness = new Thickness(1, 1, 1, 1);
                //IUCBasicInfo.BorderBrush = Brushes.LightGray;
                //StackPanelAgentProperties.Children.Add(IUCBasicInfo);

                //IUCSkillInfo.Margin = new Thickness(0, 5, 0, 0);
                //IUCSkillInfo.BorderThickness = new Thickness(1, 1, 1, 1);
                //IUCSkillInfo.BorderBrush = Brushes.LightGray;
                //StackPanelAgentProperties.Children.Add(IUCSkillInfo);

                //IUCManger.Margin = new Thickness(0, 5, 0, 0);
                //IUCManger.BorderThickness = new Thickness(1, 1, 1, 1);
                //IUCManger.BorderBrush = Brushes.LightGray;
                //StackPanelAgentProperties.Children.Add(IUCManger);

                ShowElementContent();
                IUCBasicInfo.ShowElementContent();
                IUCBasicInfo.ShowMyControlOrg();
                IUCSkillInfo.ShowElementContent();
                IUCManger.ShowElementContent();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(),"3");
            }
        }

        private void ShowOrgAgent(TreeViewItem ATreeViewItem, string AStrParentOrgID)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrOrgID = string.Empty;
            string LStrOrgName = string.Empty;
            string LStrOrgParent = string.Empty;
            string LStrImagesPath = string.Empty;

            string LStrAgentStatus = string.Empty;
            string LStrAgentID = string.Empty;
            string LStrAgentCode = string.Empty;
            string LStrAgentName = string.Empty;

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
                    IListTVIOrg.Add(LTreeViewItemSubOrg);
                    ShowOrgAgent(LTreeViewItemSubOrg, LStrOrgID);
                    LTreeViewItemSubOrg.IsExpanded = true;
                }
                DataRow[] LDataRowAgent = App.IDataTable11101.Select("C011 = '" + AStrParentOrgID + "' AND C002 = 1", "C017 ASC");
                foreach (DataRow LDataRowSingleAgent in LDataRowAgent)
                {
                    LStrAgentID = LDataRowSingleAgent["C001"].ToString();
                    LStrAgentCode = LDataRowSingleAgent["C017"].ToString();
                    //LStrAgentCode = EncryptionAndDecryption.EncryptDecryptString(LStrAgentCode, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (App.IDataTable11201UA.Select("C003 = " + App.GClassSessionInfo.UserInfo.UserID.ToString() + " AND C004 = " + LStrAgentID).Length <= 0) { continue; }
                    LStrAgentName = LDataRowSingleAgent["C018"].ToString();
                    LStrAgentName = EncryptionAndDecryption.EncryptDecryptString(LStrAgentName, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LStrAgentStatus = LDataRowSingleAgent["C012"].ToString();
                    TreeViewItem LTreeViewItemSubAgent = new TreeViewItem();
                    LTreeViewItemSubAgent.Header = "(" + LStrAgentCode + ") " + LStrAgentName;
                    LTreeViewItemSubAgent.DataContext = LStrAgentID;
                    if (LStrAgentStatus == "1")
                    {
                        TreeViewItemProps.SetItemImageName(LTreeViewItemSubAgent, LStrImagesPath + @"\S1103001.ico");
                    }
                    else
                    {
                        TreeViewItemProps.SetItemImageName(LTreeViewItemSubAgent, LStrImagesPath + @"\S1103002.ico");
                    }
                    ATreeViewItem.Items.Add(LTreeViewItemSubAgent);
                    
                    IListTVIAgent.Add(LTreeViewItemSubAgent);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        public string GetAgentParentOrg(string AStrAgentID, ref string AStrOrgFullName)
        {
            string LStrParentOrgID = string.Empty;
            string LStrItemData = string.Empty;

            TreeViewItem LTreeViewItemParent = null;

            try
            {
                AStrOrgFullName = string.Empty;
                TreeViewItem LTreeViewItemCurrent = TreeViewOrgAgent.SelectedItem as TreeViewItem;
                if (LTreeViewItemCurrent == null) { return string.Empty; }
                LStrItemData = LTreeViewItemCurrent.DataContext.ToString();
                if (LStrItemData.Substring(0, 3) != "103") { return string.Empty; }
                LTreeViewItemParent = LTreeViewItemCurrent.Parent as TreeViewItem;
                if (LTreeViewItemParent != null) { LStrParentOrgID = LTreeViewItemParent.DataContext.ToString(); }
                while (LTreeViewItemParent != null)
                {
                    AStrOrgFullName = @" \ " + LTreeViewItemParent.Header.ToString() + AStrOrgFullName;
                    LTreeViewItemParent = LTreeViewItemParent.Parent as TreeViewItem;
                }
                AStrOrgFullName = AStrOrgFullName.Substring(3);
            }
            catch { AStrOrgFullName = string.Empty; }

            return LStrParentOrgID;
        }

        public List<string> GetAgentParentOrg(string AStrAgentID)
        {
            List<string> LListStrReturn = new List<string>();
            string LStrItemData = string.Empty;

            TreeViewItem LTreeViewItemParent = null;

            try
            {
                TreeViewItem LTreeViewItemCurrent = TreeViewOrgAgent.SelectedItem as TreeViewItem;
                if (LTreeViewItemCurrent == null) { return LListStrReturn; }
                LStrItemData = LTreeViewItemCurrent.DataContext.ToString();
                if (LStrItemData.Substring(0, 3) != "103") { return LListStrReturn; }
                LTreeViewItemParent = LTreeViewItemCurrent.Parent as TreeViewItem;
                while (LTreeViewItemParent != null)
                {
                    LListStrReturn.Add(LTreeViewItemParent.DataContext.ToString() + App.AscCodeToChr(27) + LTreeViewItemParent.Header.ToString());
                    LTreeViewItemParent = LTreeViewItemParent.Parent as TreeViewItem;
                }
            }
            catch { LListStrReturn.Clear(); }

            return LListStrReturn;
        }

        #region 属性定义
        private string _StrEditApply;
        public string StrEditApply
        {
            get { return _StrEditApply; }
            set
            {
                _StrEditApply = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrEditApply"); }
            }
        }

        private string _StrCancel;
        public string StrCancel
        {
            get { return _StrCancel; }
            set
            {
                _StrCancel = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrCancel"); }
            }
        }

        private string _StrAddAgent;
        public string StrAddAgent
        {
            get { return _StrAddAgent; }
            set
            {
                _StrAddAgent = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrAddAgent"); }
            }
        }

        private string _StrDelAgent;
        public string StrDelAgent
        {
            get { return _StrDelAgent; }
            set
            {
                _StrDelAgent = value;
                if (PropertyChanged != null) { NotifyPropertyChanged("StrDelAgent"); }
            }
        }
        #endregion

        #region 属性值变化触发事件
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String StrPropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(StrPropertyName));
            }
        }
        #endregion
    }
}
