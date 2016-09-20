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
    /// UCWizardConcurrency.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardConcurrency:IWizard
    {
        public ResourceMainView MainPage;
        //public List<ConfigObject> ListConfigObjects;
        public UCWizardAgreement PrePage;
        public ConfigObject RecordConfigObj;

        private int RecordConcurrencyNumber;
        private ObjectItem ParentObjectItem;
        private ObservableCollection<PropertyValueEnumItem> ListConcurrencyItems;
        private int RecordNetWorkNumber;
        private List<ConfigObject> ListConcurrencies = new List<ConfigObject>();
        private ConfigObject CurrentConfigObj;
        public WizardHelper WizardHelper { get; set; }

        public UCWizardConcurrency()
        {
            InitializeComponent();

            RecordConcurrencyNumber = 1;
            ListConcurrencyItems = new ObservableCollection<PropertyValueEnumItem>();
            CurrentConfigObj = new ConfigObject();

            this.Loaded += UCWizardConcurrency_Loaded;
            this.ButnBack.Click += ButnBack_Click;
            this.ButnNext.Click += ButnNext_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.CombNumber.ItemsSource = ListConcurrencyItems;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            //上一页，等同于返回
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            SaveRecordConcurrency();
            //打开界面7
            UCWizardRecordChannel ucwizard = new UCWizardRecordChannel();
            ucwizard.MainPage = MainPage;
            ucwizard.PrePage = this;
            ucwizard.CurrentApp = CurrentApp;
            ucwizard.WizardHelper = this.WizardHelper;
            ucwizard.RecordConfigObj = this.RecordConfigObj;
            //ucwizard.ListConfigObjects = ListConfigObjects;
            MainPage.PopupPanel.Title = "Config Wizard";
            MainPage.PopupPanel.Content = ucwizard;
            MainPage.PopupPanel.IsOpen = true;
        }

        private void SaveRecordConcurrency()
        {
            if (this.TexLicenseNumber.Text != string.Empty)
            {
                ResourceProperty TempResourceProper = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                if (TempResourceProper != null)
                {
                    TempResourceProper.Value = this.TexLicenseNumber.Text;
                }
            }
            if (this.CombNumber.SelectedIndex != -1)
            {
                ResourceProperty TempResourceProper = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 11);
                if (TempResourceProper != null)
                {
                    PropertyValueEnumItem propertyItem = this.CombNumber.SelectedItem as PropertyValueEnumItem;
                    TempResourceProper.Value = propertyItem.Value;
                }
            }
            CurrentConfigObj.GetBasicPropertyValues();
            bool IsFind = false;
            List<ConfigObject> TempList = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 224 && p.ParentID == RecordConfigObj.ObjectID).ToList();
            for (int i = 0; i < TempList.Count; i++)
            {
                if (TempList[i].ObjectID == CurrentConfigObj.ObjectID)
                {
                    IsFind = true;
                }
            }
            if (!IsFind)
            {
                WizardHelper.ListAllConfigObjects.Add(CurrentConfigObj);
            }
            if (RecordConcurrencyNumber > ListConcurrencies.Count)
            {
                ListConcurrencies.Add(CurrentConfigObj);
            }
            if (ParentObjectItem != null)
            {
                MainPage.RefreshConfigObjectItem(ParentObjectItem);
            }
        }

        void ButnBack_Click(object sender, RoutedEventArgs e)
        {
            //返回
            MainPage.PopupPanel.Content = PrePage;
        }

        void UCWizardConcurrency_Loaded(object sender, RoutedEventArgs e)
        {
            InitControlObject();
            ChangeLanguage();
        }

        private void InitControlObject()
        {
            ListConcurrencies = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 224 && p.ParentID == RecordConfigObj.ObjectID).ToList();
            //if (RecordAgreementNumber == 1)
            //{
            //    RecordAgreementNumber = ListAgreements.Count();
            //}
            int ListCount = ListConcurrencies.Count;
            List<BasicInfoData> TempBasicInfo = WizardHelper.ListAllBasicInfos.Where(p => p.InfoID == 111000150).ToList();
            foreach (BasicInfoData info in TempBasicInfo)
            {
                PropertyValueEnumItem propertyValueItem = new PropertyValueEnumItem();
                propertyValueItem.IsCheckedChanged += EnumItem_IsCheckedChanged;
                propertyValueItem.Value = info.Value;
                propertyValueItem.Display =
                    CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                        info.Icon);
                propertyValueItem.Info = info;
                ListConcurrencyItems.Add(propertyValueItem);
            }
            this.TexLicenseNumber.Text = "0";
            if (ListCount >= RecordConcurrencyNumber)
            {
                //加载显示已有项
                CurrentConfigObj = ListConcurrencies[RecordConcurrencyNumber - 1];
                ResourceProperty NetworkName = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                if (NetworkName != null)
                {
                    this.TexLicenseNumber.Text = NetworkName.Value;
                }
                ResourceProperty mPropertyValue = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 11);
                if (mPropertyValue != null)
                {
                    int value = 0;
                    if (int.TryParse(mPropertyValue.Value, out value))
                    {
                        for (int i = 0; i < ListConcurrencyItems.Count; i++)
                        {
                            if (ListConcurrencyItems[i].Value == value.ToString())
                            {
                                this.CombNumber.SelectedIndex = i;
                            }
                        }
                    }
                }
            }
            else
            {
                List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 224 && p.ParentID == RecordConfigObj.ObjectID).ToList().Count();
                for (int i = 0; i < tempObjItem.Count; i++)
                {
                    ConfigGroup tempCG = tempObjItem[i].Data as ConfigGroup;
                    if (tempCG.GroupID == 28 && tempCG.ChildType == 224 && tempCG.ConfigObject == RecordConfigObj)
                    {
                        int MinValue = tempCG.GroupInfo.IntValue01; int MaxValue = tempCG.GroupInfo.IntValue02;
                        if ((ReadyNumber + 1) > MaxValue || (ReadyNumber + 1) < MinValue)
                        { return; }
                        //WizardHelper.ListAllConfigObjects.Add(WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 222));
                        CurrentConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 224);
                        CurrentConfigObj.CurrentApp = CurrentApp;
                        CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 3).Value = RecordConfigObj.ObjectID.ToString();
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
                bool IsFound = false;
                string Value = string.Empty;
                for (int i = 0; i < ListConcurrencyItems.Count; i++)
                {
                    if (ListConcurrencyItems[i].Value == Value)
                    {
                        IsFound = true;
                        this.CombNumber.SelectedIndex = i;
                    }
                }
                if (!IsFound)
                {
                    this.CombNumber.Text = Value;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            this.LabLicenseNumber.Content = CurrentApp.GetLanguageInfo("1110WIZ06003", "License数量：");
            this.LabNumber.Content = CurrentApp.GetLanguageInfo("1110WIZ06002", "Number:");

            this.ButnBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ06001", "配置向导");

            for (int i = 0; i < ListConcurrencyItems.Count; i++)
            {
                PropertyValueEnumItem item = ListConcurrencyItems[i];
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
