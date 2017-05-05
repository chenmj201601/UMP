using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using UMPS4601.Models;
using UMPS4601.Wcf46011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common4601;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS4601
{
    /// <summary>
    /// 修改选中的KPI的详情
    /// </summary>
    public partial class ModifyKpiDetail
    {
        public UC_KPIInfoShowPage ParentPage = null;
        public KpiInfoItem CurrentSelectItem;
        private ComboxObjectType mCurrentSelectOperationSymbol;
        private ObservableCollection<ComboxObjectType> mListOperationSymbols;

        public ModifyKpiDetail()
        {
            InitializeComponent();

            mListOperationSymbols = new ObservableCollection<ComboxObjectType>();

            Loaded += ModifyKpiDetail_Loaded;
            GoalOperation1.SelectionChanged += GoalOperation1_SelectionChanged;
            BtnSave.Click += BtnSave_Click;
            BtnCancel.Click += BtnCancel_Click;
        }



        private void ModifyKpiDetail_Loaded(object sender, RoutedEventArgs e)
        {
            GoalOperation1.ItemsSource = mListOperationSymbols;
            InitSymbol();
            InitValue();

            ChangeLanguage();
        }

        private void InitValue()
        {
            var tempitem = mListOperationSymbols.FirstOrDefault(o => o.Name == CurrentSelectItem.DefaultSymbol) as ComboxObjectType;
            if (tempitem != null)
            {
                GoalOperation1.SelectedItem = tempitem;
            }
            KpiName.Content = CurrentSelectItem.Name.ToString();
            Goal1Value.Text = CurrentSelectItem.GoalValue1.ToString();
            Goal2Value.Text = CurrentSelectItem.GoalValue2.ToString();
        }

        private void InitSymbol()
        {
            try
            {
                mListOperationSymbols.Clear();
                for (int i = 0; i < 5; i++)
                {
                    ComboxObjectType item = new ComboxObjectType();
                    if (i == 0) { item.Name = ">"; mListOperationSymbols.Add(item); }
                    if (i == 1) { item.Name = ">="; mListOperationSymbols.Add(item); }
                    if (i == 2) { item.Name = "="; mListOperationSymbols.Add(item); }
                    if (i == 3) { item.Name = "<"; mListOperationSymbols.Add(item); }
                    if (i == 4) { item.Name = "<="; mListOperationSymbols.Add(item); }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        private bool CreateValue()
        {
            CurrentSelectItem.DefaultSymbol = mCurrentSelectOperationSymbol.Name;
            CurrentSelectItem.KpiInfo.DefaultSymbol = mCurrentSelectOperationSymbol.Name;

            double dtINT;
            if (string.IsNullOrWhiteSpace(Goal1Value.Text.ToString()) ||
                string.IsNullOrWhiteSpace(Goal2Value.Text.ToString()) ||
                !double.TryParse(Goal1Value.Text.ToString(), out dtINT) ||
                !double.TryParse(Goal2Value.Text.ToString(), out dtINT))
            {
                ShowInformation("请填写正确的目标值");
                return false;
            }
            if (double.Parse(Goal1Value.Text.ToString()) > 99999999 ||
                double.Parse(Goal1Value.Text.ToString()) < -99999999 ||
                double.Parse(Goal2Value.Text.ToString()) > 99999999 ||
                double.Parse(Goal2Value.Text.ToString()) < -99999999)
            {
                ShowInformation("填写的目标超出范围");
                return false;
            }
            CurrentSelectItem.GoalValue1 = Goal1Value.Text.ToString();
            CurrentSelectItem.KpiInfo.GoalValue1 = Goal1Value.Text.ToString();
            CurrentSelectItem.GoalValue2 = Goal2Value.Text.ToString();
            CurrentSelectItem.KpiInfo.GoalValue2 = Goal2Value.Text.ToString();
            return true;
        }

        private bool SaveToDB()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S4601Codes.ModifyDefaultValue;
                OperationReturn optReturn = new OperationReturn();
                optReturn = XMLHelper.SeriallizeObject(CurrentSelectItem.KpiInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                //Service46011Client client = new Service46011Client();
                Service46011Client client = new Service46011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service46011"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                return true;
                #region 写操作日志

                //string strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("COL3102001RecordReference"), RecordInfoItem.SerialID);
                //CurrentApp.WriteOperationLog(S3102Consts.OPT_MEMORECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
        }

        private void GoalOperation1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var aaa = GoalOperation1.SelectedItem as ComboxObjectType;
            if (aaa != null)
            {
                mCurrentSelectOperationSymbol = aaa;
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ClosePopupPannel();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CreateValue() == false)
                {
                    return;
                }
                if (SaveToDB() == false)
                {
                    return;
                }
                ClosePopupPannel();
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        private void ClosePopupPannel()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.Title = CurrentApp.GetLanguageInfo("4601MD0000", "ModifyValue Page");
            }
            BtnSave.Content = CurrentApp.GetLanguageInfo("46010", "Save");
            BtnCancel.Content = CurrentApp.GetLanguageInfo("46011", "Cancel");
            Lable_KpiName.Content = CurrentApp.GetLanguageInfo("COL4601001KPIName", "KPIName");
            DefaultValue01.Content = CurrentApp.GetLanguageInfo("COL4601001GoldValue1", "Goal1");
            DefaultValue02.Content = CurrentApp.GetLanguageInfo("COL4601001GoldValue2", "Goal2");
            DefaultSymbol.Content = CurrentApp.GetLanguageInfo("COL4603001DefaultSymbol", "DefaultSymbol");
        }

    }
}
