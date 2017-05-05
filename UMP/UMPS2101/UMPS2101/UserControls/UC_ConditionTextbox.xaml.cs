using System;
using System.Collections.Generic;
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
using UMPS2101.Wcf21011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common21011;
using VoiceCyber.UMP.Communications;

namespace UMPS2101.UserControls
{
    /// <summary>
    /// UC_ConditionTextbox.xaml 的交互逻辑
    /// </summary>
    public partial class UC_ConditionTextbox : UserControl
    {
        public FilterConditionMainView mParent;
        private List<StrategyInfo> mListAllStrategy;
        public UC_ConditionTextbox()
        {
            InitializeComponent();
            mListAllStrategy = new List<StrategyInfo>();
            BindLogical();
            Loaded += UC_ConditionTextbox_Loaded;
            BtnRemove.Click += BtnRemove_Click;
        }

        void BtnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (mParent != null)
            {
                mParent.RemoveCondition(this);
            }
        }

        private void UC_ConditionTextbox_Loaded(object sender, RoutedEventArgs e)
        {
            GetAllowConditions();
            BindCondition();
            cmbcondition.SelectionChanged += cmbcondition_SelectionChanged;
        }

        public void SetValues(CFilterCondition cfc)
        {
            if (cfc != null &&
                !string.IsNullOrWhiteSpace(cfc.ConditionName) &&
                !string.IsNullOrWhiteSpace(cfc.Operator) &&
                !string.IsNullOrWhiteSpace(cfc.Value))
            {
                BindCondition();
                GetAllowConditions();
                cmbcondition.SelectionChanged += cmbcondition_SelectionChanged;

                cmbcondition.Text = mParent.CurrentApp.GetLanguageInfo(("2101T0" + cfc.ConditionName).ToUpper(), cfc.ConditionName.ToUpper());
                this.cmbope.Text = cfc.Operator;
                this.txtval.Text = cfc.Value;
                string strlogi = string.IsNullOrWhiteSpace(cfc.Logical) ? "" : cfc.Logical;
                this.cmblog.Text = strlogi;
            }
        }

        private void cmbcondition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                StrategyInfo selStInfo = cmbcondition.SelectedItem as StrategyInfo;
                if (selStInfo != null)
                {
                    string selval = cmbcondition.SelectedValue.ToString();
                    List<COperator> moperator = new List<COperator>();
                    foreach (string s in selval.Split('|'))
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            moperator.Add(new COperator() { Show = s, Description = "" });
                        }
                    }
                    cmbope.ItemsSource = moperator;
                    cmbope.DisplayMemberPath = "Show";
                    mParent.SetDiscription(selStInfo);
                }
            }
            catch (Exception ex)
            { }
        }

        private void BindLogical()
        {
            IList<CLogical> mlogical = new List<CLogical>();
            mlogical.Add(new CLogical() { ID = 0, Show = "And", Description = "" });
            mlogical.Add(new CLogical() { ID = 1, Show = "Or", Description = "" });
            cmblog.ItemsSource = mlogical;
            cmblog.DisplayMemberPath = "Show";
            cmblog.SelectedValuePath = "ID";
        }

        private void BindCondition()
        {
            cmbcondition.ItemsSource = mListAllStrategy;
            cmbcondition.DisplayMemberPath = "Display";
            cmbcondition.SelectedValuePath = "AllowCondition";
        }

        private void GetAllowConditions()
        {
            mListAllStrategy.Clear();
            string datatarget = "1";//1：录音记录
            WebRequest webRequest = new WebRequest();
            webRequest.Session = mParent.CurrentApp.Session;
            webRequest.Code = (int)S2101Codes.GetAllowConditions;
            webRequest.ListData.Add(datatarget);
            //Service21011Client client = new Service21011Client();
            Service21011Client client = new Service21011Client(WebHelper.CreateBasicHttpBinding(mParent.CurrentApp.Session), WebHelper.CreateEndpointAddress(mParent.CurrentApp.Session.AppServerInfo, "Service21011"));
            WebReturn webReturn = client.DoOperation(webRequest);
            client.Close();
            if (!webReturn.Result)
            {
                mParent.CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return;
            }
            if (webReturn.ListData.Count <= 0) { return; }
            for (int i = 0; i < webReturn.ListData.Count; i++)
            {
                OperationReturn optReturn = XMLHelper.DeserializeObject<StrategyInfo>(webReturn.ListData[i]);
                if (!optReturn.Result)
                {
                    mParent.CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    continue;
                }
                StrategyInfo strategyinfo = optReturn.Data as StrategyInfo;
                if (strategyinfo == null)
                {
                    return;
                }
                string tempTableName = strategyinfo.StrategyType == 1 ? "T_21_001" : "";
                //strategyinfo.ConditionName = tempTableName + "." + strategyinfo.FieldName;
                strategyinfo.ConditionName = strategyinfo.FieldName;
                strategyinfo.Display = mParent.CurrentApp.GetLanguageInfo(("2101T0" + strategyinfo.FieldName).ToUpper(), strategyinfo.FieldName.ToUpper());
                mListAllStrategy.Add(strategyinfo);
            }
        }

    }
}
