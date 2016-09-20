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
using UMPS1110.Models;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Wizard
{
    /// <summary>
    /// UCWizardRecordNetWork.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardRecordNetWork : IWizard
    {
        public ResourceMainView MainPage;
        private List<ConfigObject> ListConfigObjects;
        public UCWizardRecordConfig PrePage;
        //public int RecordNumber;
        public ConfigObject RecordConfigObj;

        private int RecordNetWorkNumber;
        private ObservableCollection<PropertyValueEnumItem> ListCaptureItems;
        private ConfigObject NetworkConfigObj;
        private List<ConfigObject> ListRecordNetwork = new List<ConfigObject>();
        private ObjectItem ParentObjectItem;
        public WizardHelper WizardHelper { get; set; }

        public UCWizardRecordNetWork()
        {
            InitializeComponent();
            RecordNetWorkNumber = 1;

            ListCaptureItems = new ObservableCollection<PropertyValueEnumItem>();

            this.Loaded += UCWizardRecordNetWork_Loaded;
            this.ButnAdd.Click += ButnAdd_Click;
            this.ButnBack.Click += ButnBack_Click;
            this.ButnNext.Click += ButnNext_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;

            this.CombCapture.ItemsSource = ListCaptureItems;
        }

        void UCWizardRecordNetWork_Loaded(object sender, RoutedEventArgs e)
        {
            InitControlObject();
            ChangeLanguage();
        }

        private void InitControlObject()
        {
            ListRecordNetwork = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 222 && p.ParentID == RecordConfigObj.ObjectID).ToList();
            int ListCount = ListRecordNetwork.Count;
            List<BasicInfoData> TempBasicInfo = WizardHelper.ListAllBasicInfos.Where(p => p.InfoID == 111000200).ToList();
            foreach (BasicInfoData info in TempBasicInfo)
            {
                PropertyValueEnumItem propertyValueItem = new PropertyValueEnumItem();
                propertyValueItem.IsCheckedChanged += EnumItem_IsCheckedChanged;
                propertyValueItem.Value = info.Value;
                propertyValueItem.Display =
                    CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                        info.Icon);
                propertyValueItem.Info = info;
                ListCaptureItems.Add(propertyValueItem);
            }
            if (ListCount >= RecordNetWorkNumber)
            {
                //加载显示已有项
                NetworkConfigObj = ListRecordNetwork[RecordNetWorkNumber - 1];
                ResourceProperty NetworkName = NetworkConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                if (NetworkName != null)
                {
                    this.CombNetworkName.Text = NetworkName.Value;
                }
                ResourceProperty mPropertyValue = NetworkConfigObj.ListProperties.Find(p => p.PropertyID == 11);
                if (mPropertyValue != null)
                {
                    int value = 0;
                    if (int.TryParse(mPropertyValue.Value, out value))
                    {
                        if ((value & 1) == 1)
                        {
                            PropertyValueEnumItem propertyItem = ListCaptureItems.FirstOrDefault(p => p.Value == "1");
                            if (propertyItem != null)
                            {
                                propertyItem.IsChecked = true;
                            }
                            else
                            {
                                propertyItem.IsChecked = false;
                            }
                        }
                        if ((value & 2) == 2)
                        {
                            PropertyValueEnumItem propertyItem = ListCaptureItems.FirstOrDefault(p => p.Value == "2");
                            if (propertyItem != null)
                            {
                                propertyItem.IsChecked = true;
                            }
                            else
                            {
                                propertyItem.IsChecked = false;
                            }
                        }
                    }
                }
            }
            else
            {
                List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 222 && p.ParentID == RecordConfigObj.ObjectID).ToList().Count();
                for (int i = 0; i < tempObjItem.Count; i++)
                {
                    ConfigGroup tempCG = tempObjItem[i].Data as ConfigGroup;
                    if (tempCG.GroupID == 26 && tempCG.ChildType == 222 && tempCG.ConfigObject == RecordConfigObj)
                    {
                        int MinValue = tempCG.GroupInfo.IntValue01; int MaxValue = tempCG.GroupInfo.IntValue02;
                        if ((ReadyNumber + 1) > MaxValue || (ReadyNumber + 1) < MinValue)
                        { return; }
                        //WizardHelper.ListAllConfigObjects.Add(WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 222));
                        NetworkConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 222);
                        NetworkConfigObj.ListProperties.Find(p => p.PropertyID == 3).Value = RecordConfigObj.ObjectID.ToString();
                        ParentObjectItem = tempObjItem[i];
                        break;
                    }
                }
            }
        }

        void EnumItem_IsCheckedChanged()
        {
            try
            {
                int intValue = 0;
                int intTemp;
                string strText = string.Empty;
                for (int i = 0; i < ListCaptureItems.Count; i++)
                {
                    if (ListCaptureItems[i].IsChecked)
                    {
                        strText += string.Format("{0},", ListCaptureItems[i].Display);

                        if (!int.TryParse(ListCaptureItems[i].Value, out intTemp))
                        {
                            continue;
                        }
                        intValue = (intValue | intTemp);
                    }
                }
                strText = strText.TrimEnd(new[] { ',' });
                ResourceProperty mPropertyValue = NetworkConfigObj.ListProperties.Find(p => p.PropertyID == 11);
                if (mPropertyValue != null)
                {
                    mPropertyValue.Value = intValue.ToString();
                }
                this.CombCapture.Text = strText;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            SaveRecordNetWork();
            //前一页。看当前配了几个啦
            if (RecordNetWorkNumber == 1)
            {
                MainPage.PopupPanel.Content = PrePage;
            }
            else
            {
                //打开前一项
                RecordNetWorkNumber--;
                InitControlObject();
            }
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            SaveRecordNetWork();
            if (RecordNetWorkNumber < ListRecordNetwork.Count)
            {
                //加载下一项的内容
                RecordNetWorkNumber++;
                InitControlObject();
            }
            else
            {
                //下一页。进入界面5，协议配置
                UCWizardAgreement ucwizard = new UCWizardAgreement();
                ucwizard.CurrentApp = CurrentApp;
                ucwizard.MainPage = MainPage;
                ucwizard.PrePage = this;
                //ucwizard.ListConfigObjects = ListConfigObjects;
                ucwizard.RecordConfigObj = this.RecordConfigObj;
                ucwizard.WizardHelper = this.WizardHelper;
                MainPage.PopupPanel.Title = "Config Wizard";
                MainPage.PopupPanel.Content = ucwizard;
                MainPage.PopupPanel.IsOpen = true;
            }
        }

        private void SaveRecordNetWork()
        {
            if (this.CombNetworkName.Text != string.Empty)
            {
                ResourceProperty TempResourceProper = NetworkConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                if (TempResourceProper != null)
                {
                    TempResourceProper.Value = this.CombNetworkName.Text;
                }
            }
            NetworkConfigObj.GetBasicPropertyValues();
            bool IsFind = false;
            List<ConfigObject> TempList = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 222 && p.ParentID == RecordConfigObj.ObjectID).ToList();
            for (int i = 0; i < TempList.Count; i++)
            {
                if (TempList[i].ObjectID == NetworkConfigObj.ObjectID)
                {
                    IsFind = true;
                }
            }
            if (!IsFind)
            {
                WizardHelper.ListAllConfigObjects.Add(NetworkConfigObj);
            }
            if (RecordNetWorkNumber > ListRecordNetwork.Count)
            {
                ListRecordNetwork.Add(NetworkConfigObj);
            }
            if (ParentObjectItem != null)
            {
                MainPage.RefreshConfigObjectItem(ParentObjectItem);
            }
        }

        void ButnBack_Click(object sender, RoutedEventArgs e)
        {
            //回到上一个界面PrePage
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButnAdd_Click(object sender, RoutedEventArgs e)
        {
            SaveRecordNetWork();
            //添加一个网卡，弹出自己界面.是否需要检查最大添加数。
            RecordNetWorkNumber++;
            ListCaptureItems.Clear();
            this.CombNetworkName.Text = string.Empty;
            InitControlObject();
        }

        public override void ChangeLanguage()
        {
            this.ButnAdd.Content = CurrentApp.GetLanguageInfo("1110WIZB0004", "添加");
            this.ButnBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ04001", "网卡：IP");

            this.LabCapture.Content = CurrentApp.GetLanguageInfo("1110WIZ04002", "抓包模式:");
            this.LabNetworkName.Content = CurrentApp.GetLanguageInfo("1110WIZ04003", "网卡名称：");

            for (int i = 0; i < ListCaptureItems.Count; i++)
            {
                PropertyValueEnumItem item = ListCaptureItems[i];
                BasicInfoData basicInfoData = item.Info as BasicInfoData;
                if (basicInfoData != null)
                {
                    item.Display =
                        CurrentApp.GetLanguageInfo(
                            string.Format("BID{0}{1}", basicInfoData.InfoID, basicInfoData.SortID.ToString("000")),
                            basicInfoData.Icon);
                }
            }
        }
    }
}
