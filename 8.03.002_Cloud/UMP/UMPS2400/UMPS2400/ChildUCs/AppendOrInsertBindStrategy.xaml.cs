using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Common2400;
using UMPS2400.Entries;
using UMPS2400.MainUserControls;
using UMPS2400.Service24021;
using VoiceCyber.UMP.Communications;

namespace UMPS2400.ChildUCs
{
    /// <summary>
    /// AppendOrInsertBindStrategy.xaml 的交互逻辑
    /// </summary>
    public partial class AppendOrInsertBindStrategy
    {
        /// <summary>
        /// 操作类型:1,追加策略 ; 
        /// </summary>
        public string mOperationType;
        public UC_EncryptionPolicyBindding mParentpage;
        public static ObservableCollection<PolicyInfoInList> lstAllPolicies = new ObservableCollection<PolicyInfoInList>();
        public AppendOrInsertBindStrategy()
        {
            InitializeComponent();
            Loaded += AppendOrInsertBindStrategy_Loaded;
            BtnApply.Click +=BtnApply_Click;
            BtnCancel.Click +=BtnCancel_Click;
        }

        private void AppendOrInsertBindStrategy_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(mOperationType))
                return;
            LvAllStrategies.ItemsSource = lstAllPolicies;
            BindContent();
            GetAllPolicies();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (mParentpage != null)
                mParentpage.SetPanelOpenState(false);
        }

        public void SetPanelOpenState(bool isOpen)
        {
            PopupPanel.IsOpen = isOpen;
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            PolicyInfoInList item = LvAllStrategies.SelectedItem as PolicyInfoInList;
            if (item != null)
            {
                SelectAnd2SetTimeSpan(item);
            }
        }

        /// <summary>
        /// 选中策略，进入设置时间
        /// </summary>
        private void SelectAnd2SetTimeSpan(PolicyInfoInList item)
        {
            try
            {
                AppendOrInsertBindStrategy02 abs2 = new AppendOrInsertBindStrategy02();
                abs2.CurrentApp = CurrentApp;
                abs2.mParentpage = this;
                abs2.mFirstPage = mParentpage;
                abs2.selPolicyInfo = item;
                abs2.lstAllPolicies = lstAllPolicies;
                abs2.mOpType = "1";
                PopupPanel.Content = abs2;
                PopupPanel.Title = CurrentApp.GetLanguageInfo("240300008", "Binding Key Strategy");
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            { }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            BindContent();
        }

        public void BindContent()
        {
            BtnApply.Content = CurrentApp.GetLanguageInfo("240300009", "Confirm");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("240300010", "Close");
            TabOjbect01.Header = CurrentApp.GetLanguageInfo("240300007", "Encryption Key Strategy List");
            InitLvStrategiesColumns();
        }

        private void InitLvStrategiesColumns()
        {
            try
            {
                string[] lans = "PolicyName,PolicyType,PolicyOccursFrequency,PolicyStartTime,PolicyEndTime,IsStrongPwd".Split(',');
                string[] cols = "PolicyName,PolicyType,PolicyOccursFrequency,PolicyStartTime,PolicyEndTime,IsStrongPwd".Split(',');
                int[] colwidths = { 180, 200, 120, 145, 145, 100 };
                GridView columngv = new GridView();
                for (int i = 0; i < 6; i++)
                {
                    DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                    SetColumnGridView(cols[i], ref columngv, lans[i], cols[i], CellTemplate, colwidths[i]);
                }
                LvAllStrategies.View = columngv;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SetColumnGridView(string columnname, ref GridView ColumnGridView, string langid, string diaplay, DataTemplate datatemplate, int width)
        {
            GridViewColumn gvc = new GridViewColumn();
            gvc.Header = CurrentApp.GetLanguageInfo(langid, diaplay);
            gvc.Width = width;
            gvc.HeaderStringFormat = columnname;
            if (datatemplate != null)
            {
                gvc.CellTemplate = datatemplate;
            }
            else
                gvc.DisplayMemberBinding = new Binding(columnname);
            ColumnGridView.Columns.Add(gvc);
        }

        private void GetAllPolicies()
        {
            lstAllPolicies.Clear();
            Service24021Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.GetAllPolicies;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                client = new Service24021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                       WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, CurrentApp.GetLanguageInfo(webReturn.Code.ToString(), webReturn.Message)));
                    return;
                }
                if (webReturn.DataSetData.Tables.Count <= 0)
                {
                    return;
                }
                DataTable dt = webReturn.DataSetData.Tables[0];
                PolicyInfoInList policyItem = null;
                foreach (DataRow row in dt.Rows)
                {
                    policyItem = new PolicyInfoInList();
                    policyItem.PolicyID = row["C001"].ToString();
                    policyItem.PolicyName = row["C002"].ToString();
                    //= row["004"].ToString();
                    string strType = row["C004"].ToString();
                policyItem.PolicyType = strType == "U" ? CurrentApp.GetLanguageInfo("240300027", "Custom (user input)") : CurrentApp.GetLanguageInfo("240300028", "Periodic update key (randomly generated)");
                    if (strType == "C")
                    {
                        string strOccursFrequency = row["C007"].ToString();
                        switch (strOccursFrequency)
                        {
                            case "D":
                                policyItem.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("240300029", "Day");
                                break;
                            case "W":
                                policyItem.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("240300030", "Week");
                                break;
                            case "M":
                                policyItem.PolicyOccursFrequency = CurrentApp.GetLanguageInfo("240300031", "Month");
                                break;
                            case "U":
                                policyItem.PolicyOccursFrequency = row["C010"].ToString() + CurrentApp.GetLanguageInfo("240300029", "Day");
                                break;
                        }
                    }
                    else
                    {
                        policyItem.PolicyOccursFrequency = string.Empty;
                    }
                    policyItem.PolicyStartTime = CommonFunctions.StringToDateTime(row["C008"].ToString()).ToString();
                    long longTime = 0;
                    long.TryParse(row["C008"].ToString(), out longTime);
                    policyItem.PolicyStartTimeNumber = longTime;
                    if (row["C009"].ToString() != "20991231235959")
                    {
                        policyItem.PolicyEndTime = CommonFunctions.StringToDateTime(row["C009"].ToString()).ToString();
                    }
                    else
                    {
                        policyItem.PolicyEndTime = CurrentApp.GetLanguageInfo("240300032", "Never expires");
                    }
                    long.TryParse(row["C009"].ToString(), out longTime);
                    policyItem.PolicyEndTimeNumber = longTime;
                    policyItem.PolicyIsEnabled = row["C003"].ToString();
                    if (strType == "C")
                    {
                        policyItem.IsStrongPwd = row["C012"].ToString() == "1" ? CurrentApp.GetLanguageInfo("240300003", "Yes") : string.Empty;
                    }
                    else
                    {
                        policyItem.IsStrongPwd = string.Empty;
                    }
                    long lNow = 0;
                    CommonFunctions.DateTimeToNumber(DateTime.Now, ref lNow);
                    if (policyItem.PolicyEndTimeNumber > lNow && policyItem.PolicyIsEnabled == "1")
                    {
                        lstAllPolicies.Add(policyItem);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException("Failed." + ex.Message);
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
        }
    }
}
