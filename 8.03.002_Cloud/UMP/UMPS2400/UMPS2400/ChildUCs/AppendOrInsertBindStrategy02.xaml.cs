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
using UMPS2400.Service24011;
using UMPS2400.Service24021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS2400.ChildUCs
{
    /// <summary>
    /// AppendOrInsertBindStrategy02.xaml 的交互逻辑
    /// </summary>
    public partial class AppendOrInsertBindStrategy02
    {
        /// <summary>
        /// 操作类型 1：追加策略;2:修改策略时间
        /// </summary>
        public string mOpType;
        public UC_EncryptionPolicyBindding mFirstPage;
        public IList<PolicyInfoInList> lstAllPolicies;
        public PolicyInfoInList selPolicyInfo;
        public static ObservableCollection<CPropertyAndValues> lstPropertyAndValues = new ObservableCollection<CPropertyAndValues>();
        public AppendOrInsertBindStrategy mParentpage;
        public AppendOrInsertBindStrategy02()
        {
            InitializeComponent();
            Loaded += AppendOrInsertBindStrategy_Loaded;
        }

        private void AppendOrInsertBindStrategy_Loaded(object sender, RoutedEventArgs e)
        {
            BindContent();
            StartDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(ButtonControl_SelectedDateChanged);
            EndDate.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(ButtonControl_SelectedDateChanged);
            LvPropertyAndValues.ItemsSource = lstPropertyAndValues;
            lstPropertyAndValues.Clear();
            if (selPolicyInfo != null)
            {
                string[] lans = "PolicyName,PolicyType,PolicyOccursFrequency,PolicyStartTime,PolicyEndTime,IsStrongPwd".Split(',');
                string[] vals = (GetStrValue(selPolicyInfo.PolicyName) + "|"
                    + GetStrValue(selPolicyInfo.PolicyType) + "|"
                    + GetStrValue(selPolicyInfo.PolicyOccursFrequency) + "|"
                    + GetStrValue(selPolicyInfo.PolicyStartTime) + "|"
                    + GetStrValue(selPolicyInfo.PolicyEndTime) + "|"
                    + GetStrValue(selPolicyInfo.IsStrongPwd)
                    ).Split('|');
                for (int i = 0; i < 6; i++)
                {
                    try
                    {
                        CPropertyAndValues cav = new CPropertyAndValues();
                        cav.AppendProperty = CurrentApp.GetLanguageInfo(lans[i], lans[i].Substring(10));
                        cav.AppendValues = vals[i];
                        lstPropertyAndValues.Add(cav);
                    }
                    catch (Exception ex)
                    { ShowException(ex.Message); }
                }
                TextStartDateTime.Text = DateTime.Parse(selPolicyInfo.PolicyStartTime).ToString("yyyy-MM-dd HH:mm:ss");
                TextEndDateTime.Text = DateTime.Parse(selPolicyInfo.PolicyEndTime).ToString("yyyy-MM-dd HH:mm:ss");
                if (selPolicyInfo.PolicyEndTimeNumber > 20990000000000)
                {
                    RadioNoEndDate.IsChecked = true;
                    TextEndDateTime.IsEnabled = false;
                }
                else
                    RadioEndDate.IsChecked = true;

            }
            if (mOpType == "1")
            {
                #region 1：追加策略
                RadioBeginDate.IsChecked = true;
                RadioBeginImmediately.IsChecked=false;

                if (mFirstPage.lstPolicyBindding.Count > 0)//之前绑定过策略
                {
                    if (mFirstPage.mcurrentPoBind.Durationend > 20990000000000)
                        TextStartDateTime.Text = "2099-12-31 23:59:59";
                    else
                        TextStartDateTime.Text = DateTime.Parse(mFirstPage.mcurrentPoBind.DurationendStr).AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    if (selPolicyInfo.PolicyEndTimeNumber == 20991231235959)
                    {
                        RadioNoEndDate.IsChecked = true;
                        TextEndDateTime.IsEnabled = false;
                        TextEndDateTime.Text = (DateTime.Parse(selPolicyInfo.PolicyStartTime).AddDays(1)).ToString("yyyy-MM-dd") + " 23:59:59";
                    }
                    else
                    {
                        RadioEndDate.IsChecked = true;
                        TextEndDateTime.Text = (DateTime.Parse(selPolicyInfo.PolicyEndTime)).ToString("yyyy-MM-dd") + " 23:59:59";
                    }
                }
                #endregion
            }
            else if (mOpType == "2")
            {
                #region 2:修改策略时间
                CVoiceServerBindStrategy selBinded = mFirstPage.mcurrentPoBind;
                RadioBeginDate.IsChecked = true;
                RadioBeginImmediately.IsEnabled = false;
                if (selBinded == null)
                {
                    TextStartDateTime.Text = (DateTime.Parse(selPolicyInfo.PolicyStartTime)).AddDays(1).ToString("yyyy-MM-dd") + " 00:00:00";
                    if (selPolicyInfo.PolicyEndTimeNumber > 20990000000000)
                    {
                        RadioNoEndDate.IsChecked = true;
                        TextEndDateTime.IsEnabled = false;
                        TextEndDateTime.Text = (DateTime.Parse(selPolicyInfo.PolicyStartTime).AddDays(1)).ToString("yyyy-MM-dd") + " 23:59:59";
                    }
                    else
                    {
                        RadioEndDate.IsChecked = true;
                        TextEndDateTime.Text = (DateTime.Parse(selPolicyInfo.PolicyEndTime)).ToString("yyyy-MM-dd") + " 23:59:59";
                    }
                }
                else
                {
                    long selStart = selBinded.Durationbegin;
                    long selEnd = selBinded.Durationend;
                    long curNow = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                    TextStartDateTime.Text = Convert.ToDateTime(selBinded.DurationbeginStr).ToString("yyyy-MM-dd HH:mm:ss");
                    if (selEnd > 20990000000000)//结束时间无限制
                    {
                        RadioNoEndDate.IsChecked = true;
                        TextEndDateTime.IsEnabled = false;
                        TextEndDateTime.Text = (DateTime.Parse(selPolicyInfo.PolicyStartTime).AddDays(1)).ToString("yyyy-MM-dd") + " 23:59:59";
                    }
                    else
                    {
                        RadioEndDate.IsChecked = true;
                        TextEndDateTime.Text = Convert.ToDateTime(selBinded.DurationendStr).ToString("yyyy-MM-dd HH:mm:ss");
                    }

                    if (selStart < curNow)//开始时间<当前时间
                    {
                        RadioBeginDate.IsEnabled = false;
                        TextStartDateTime.IsEnabled = false;
                    }
                }
                #endregion
            }
        }
        
        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (mOpType == "1" && mParentpage != null)
                mParentpage.SetPanelOpenState(false);
            else if (mOpType == "2" && mFirstPage!=null)
                mFirstPage.SetPanelOpenState(false);
        }

        /// <summary>
        /// 添加策略绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string mSelVoiceIP = mFirstPage.LLstVoiceServer.Where(p => p.IPResourceID == mFirstPage.mCurrentSelectIPSourceID).FirstOrDefault().VoiceServer;
                if (string.IsNullOrEmpty(mSelVoiceIP)) { CurrentApp.ShowInfoMessage("VoiceIP Is NULL."); return; }
                IList<CVoiceServerBindStrategy> lstPolicyBindding = mFirstPage.lstPolicyBindding;//追加策略时，该录音服务器已经绑定的策略
                CVoiceServerBindStrategy cvsbs = new CVoiceServerBindStrategy();
                if (mOpType == "1")//追加策略
                {
                    cvsbs.Objecttype = "1";
                    cvsbs.Objectvalue = mSelVoiceIP;
                    cvsbs.Bindingpolicyid = Convert.ToInt64(selPolicyInfo.PolicyID);
                    if (RadioBeginImmediately.IsChecked == true)
                        cvsbs.Durationbegin = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                    else
                        cvsbs.Durationbegin = Convert.ToInt64((Convert.ToDateTime(TextStartDateTime.Text).ToUniversalTime()).ToString("yyyyMMddHHmmss"));
                    if (RadioNoEndDate.IsChecked == true)
                        cvsbs.Durationend = Convert.ToInt64(DateTime.Parse("2099-12-31 23:59:59").ToUniversalTime().ToString("yyyyMMddHHmmss"));
                    else
                        cvsbs.Durationend = Convert.ToInt64((Convert.ToDateTime(TextEndDateTime.Text).ToUniversalTime()).ToString("yyyyMMddHHmmss"));
                    cvsbs.Setteduserid = CurrentApp.Session.UserID;
                    cvsbs.Settedtime = Convert.ToInt64(DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                    cvsbs.Grantencryption = "1";
                    cvsbs.Description = "";

                    cvsbs.CusFiled1 = mFirstPage.mCurrentSelectIPSourceID;
                    cvsbs.CusFiled2 = CurrentApp.Session.UserInfo.Account;
                    cvsbs.CusFiled3 = selPolicyInfo.PolicyName;
                    if(cvsbs.Durationbegin < selPolicyInfo.PolicyStartTimeNumber ||
                        ((cvsbs.Durationend > selPolicyInfo.PolicyEndTimeNumber)))
                    {
                        //策略期限判断
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("240300026", "Please check the policy period."));
                        return;
                    }
                    if (!IsInputDateValied(lstPolicyBindding, cvsbs) || cvsbs.Durationbegin > cvsbs.Durationend) 
                    {
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("240300020", "Time-overlap"));
                        return;
                    }

                    SaveData224002(cvsbs);
                    mParentpage.SetPanelOpenState(false);
                    mFirstPage.SetPanelOpenState(false);
                    mFirstPage.GetPoliciesByVoiceIPSource(mFirstPage.mCurrentSelectIPSourceID);
                }
                else if (mOpType == "2")
                {
                    CVoiceServerBindStrategy selBinded = mFirstPage.mcurrentPoBind;
                    lstPolicyBindding.Remove(selBinded);
                    CVoiceServerBindStrategy newStrategy = selBinded;

                    string strStart = (Convert.ToDateTime(TextStartDateTime.Text).ToUniversalTime()).ToString("yyyyMMddHHmmss");
                    string strEnd="";
                    if (RadioNoEndDate.IsChecked == true)
                        strEnd = "20991231235959";
                    else
                        strEnd = (Convert.ToDateTime(TextEndDateTime.Text).ToUniversalTime()).ToString("yyyyMMddHHmmss");
                    newStrategy.Durationbegin = Convert.ToInt64(strStart);
                    newStrategy.Durationend = Convert.ToInt64(strEnd);
                    if (!IsInputDateValied(lstPolicyBindding, newStrategy) || newStrategy.Durationbegin > newStrategy.Durationend)
                    {
                        lstPolicyBindding.Add(selBinded);
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("240300020", "Time-overlap"));
                        return;
                    }
                    ModyfyData224002(selBinded, strStart, strEnd);
                    mFirstPage.SetPanelOpenState(false);
                    mFirstPage.GetPoliciesByVoiceIPSource(mFirstPage.mCurrentSelectIPSourceID);
                }
            }
            catch (Exception ex)
            { ShowException(ex.Message); }
        }

        /// <summary>
        /// 检测添加的策略时间是否合法
        /// </summary>
        /// <param name="lstBindedStrategy">已经绑定的策略</param>
        /// <param name="cvs">需要添加的策略</param>
        private bool IsInputDateValied(IList<CVoiceServerBindStrategy> mlstBindedStrategy, CVoiceServerBindStrategy mCvs)
        {
            bool ret = true;
            bool isCvsNopre = mCvs.Durationend > 20990000000000 ? true : false;
            bool hasNopre = mlstBindedStrategy.Where(p => p.Durationend > 20990000000000).Count() > 0 ? true : false;
            foreach (CVoiceServerBindStrategy tmpcvs in mlstBindedStrategy)
            {
                if (mCvs.Durationbegin >= tmpcvs.Durationbegin && mCvs.Durationbegin <= tmpcvs.Durationend)//时间重叠1
                { ret = false; }
                else if (mCvs.Durationend >= tmpcvs.Durationbegin && mCvs.Durationend <= tmpcvs.Durationend)//时间重叠2
                { ret = false; }
                else if (isCvsNopre && hasNopre)
                { ret = false; }
            }
            return ret;
        }

        private void SaveData224002(CVoiceServerBindStrategy cvsbs)
        {
            if (cvsbs == null)
                return;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.SaveData224002;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(cvsbs);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("{0}\t{1}", "Field.", optReturn.Message));
                    return;
                }
                webRequest.Data = optReturn.Data.ToString();
                webRequest.Session = CurrentApp.Session;
                //Service24011Client client = new Service24011Client();
                Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("{0}\t{1}", "Field.", webReturn.Message));
                    return;
                }

                string strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2403003")), cvsbs.CusFiled3);
                CurrentApp.WriteOperationLog("2403003", ConstValue.OPT_RESULT_SUCCESS, strLog);
            }
            catch (Exception ex)
            {
                string strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2403003")), cvsbs.CusFiled3 +","+ ex.Message);
                CurrentApp.WriteOperationLog("2403003", ConstValue.OPT_RESULT_FAIL, strLog);
                ShowException(ex.Message); }
        }

        #region OTHER
        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            BindContent();
        }

        public void BindContent()
        {
            BtnApply.Content = CurrentApp.GetLanguageInfo("240300009", "Confirm");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("240300010", "Close");
            TabOjbect01.Header = CurrentApp.GetLanguageInfo("240300013", "Key Strategy Details");
            TabOjbect02.Header = CurrentApp.GetLanguageInfo("240300013", "Duration");

            RadioBeginDate.Content = CurrentApp.GetLanguageInfo("240300033", "Began In");
            RadioEndDate.Content = CurrentApp.GetLanguageInfo("240300034", "End In");
            RadioBeginImmediately.Content = CurrentApp.GetLanguageInfo("240300035", "Begin immediately");
            RadioNoEndDate.Content = CurrentApp.GetLanguageInfo("240300036", "No end time");

            InitPropertyAndValuesColumns();
        }

        private void ButtonControl_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            string LStrDateTime = string.Empty;
            string LStrSenderName = string.Empty;

            try
            {
                Microsoft.Windows.Controls.DatePicker LDateTimeSender = sender as Microsoft.Windows.Controls.DatePicker;
                LStrSenderName = LDateTimeSender.Name;
                if (LStrSenderName == "StartDate" && TextStartDateTime.IsEnabled == true)
                {
                    LStrDateTime = TextStartDateTime.Text.Substring(11);
                    TextStartDateTime.Text = LDateTimeSender.SelectedDate.Value.ToString("yyyy-MM-dd") + " " + LStrDateTime;
                }
                if (LStrSenderName == "StartDate" && TextStartDateTime.IsEnabled == false)
                {
                    LStrDateTime = TextStartDateTime.Text.Substring(0, 10) + " 00:00:00";
                    LDateTimeSender.SelectedDate = Convert.ToDateTime(LStrDateTime);
                }
                if (LStrSenderName == "EndDate" && TextEndDateTime.IsEnabled == true)
                {
                    LStrDateTime = TextEndDateTime.Text.Substring(11);
                    TextEndDateTime.Text = LDateTimeSender.SelectedDate.Value.ToString("yyyy-MM-dd") + " " + LStrDateTime;
                }
                if (LStrSenderName == "EndDate" && TextEndDateTime.IsEnabled == false)
                {
                    LStrDateTime = TextEndDateTime.Text.Substring(0, 10) + " 00:00:00";
                    LDateTimeSender.SelectedDate = Convert.ToDateTime(LStrDateTime);
                }
            }
            catch { }
        }

        private string GetStrValue(object obj)
        {
            if (obj != null)
                return obj.ToString();
            else
                return "";
        }
        private void InitPropertyAndValuesColumns()
        {
            try
            {
                string[] lans = "240300011,240300012".Split(',');
                string[] cols = "AppendProperty,AppendValues".Split(',');
                int[] colwidths = { 200, 300 };
                GridView columngv = new GridView();
                for (int i = 0; i < 2; i++)
                {
                    DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                    SetColumnGridView(cols[i], ref columngv, lans[i], cols[i], CellTemplate, colwidths[i]);
                }
                LvPropertyAndValues.View = columngv;
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

        private void RadioBeginDate_Checked(object sender, RoutedEventArgs e)
        {
            TextStartDateTime.IsEnabled = true;
        }

        private void RadioBeginImmediately_Checked(object sender, RoutedEventArgs e)
        {
            TextStartDateTime.IsEnabled = false;
        }

        private void RadioEndDate_Checked(object sender, RoutedEventArgs e)
        {
            TextEndDateTime.IsEnabled = true;
        }

        private void RadioNoEndDate_Checked(object sender, RoutedEventArgs e)
        {
            TextEndDateTime.IsEnabled = false;
        }
        #endregion

        #region 2:修改策略时间
        private void ModyfyData224002(CVoiceServerBindStrategy cvsbs, string strStart, string strEnd)
        {
            if (cvsbs == null)
                return;
            //修改需要传入的值
            
            string strSetuser = CurrentApp.Session.UserID.ToString();
            string strSettime = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string strSetaccount = CurrentApp.Session.UserInfo.Account;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S2400RequestCode.ModyfyData224002;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(cvsbs);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("{0}\t{1}", "Field.", optReturn.Message));
                    return;
                }
                webRequest.Data = optReturn.Data.ToString();
                webRequest.ListData.Add(strStart);
                webRequest.ListData.Add(strEnd);
                webRequest.ListData.Add(strSetuser);
                webRequest.ListData.Add(strSettime);
                webRequest.ListData.Add(strSetaccount);
                webRequest.Session = CurrentApp.Session;
                //Service24011Client client = new Service24011Client();
                Service24011Client client = new Service24011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service24011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteOperationLog("2403006", ConstValue.OPT_RESULT_FAIL, "");
                    ShowException(string.Format("{0}\t{1}", "Field.", webReturn.Message));
                }
                else
                {
                    string tempstrend = Convert.ToDateTime(strEnd.Insert(4, "-").Insert(7, "-").Insert(10, " ").Insert(13, ":").Insert(16, ":")).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
                    string strLog = string.Format("{0}{1}{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(string.Format("FO2403006")),
                        cvsbs.CusFiled3 + "," + cvsbs.DurationendStr + "->" + tempstrend);
                    CurrentApp.WriteOperationLog("2403006", ConstValue.OPT_RESULT_SUCCESS, strLog);
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteOperationLog("2403006", ConstValue.OPT_RESULT_FAIL, "");
                ShowException(ex.Message); }
        }
        #endregion
    }
}
