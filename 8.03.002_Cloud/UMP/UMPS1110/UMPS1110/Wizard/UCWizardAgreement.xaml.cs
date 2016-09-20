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
    /// UCWizardAgreement.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardAgreement : IWizard
    {
        public ResourceMainView MainPage;
        //public List<ConfigObject> ListConfigObjects;
        public UCWizardRecordNetWork PrePage;
        public ConfigObject RecordConfigObj;

        private int RecordAgreementNumber;
        private ObservableCollection<PropertyValueEnumItem> ListAgreementItems;
        private ConfigObject CurrentConfigObj;
        private List<ConfigObject> ListAgreements = new List<ConfigObject>();
        private ObjectItem ParentObjectItem;
        public WizardHelper WizardHelper { get; set; }

        public UCWizardAgreement()
        {
            InitializeComponent();
            RecordAgreementNumber = 1;

            ListAgreementItems = new ObservableCollection<PropertyValueEnumItem>();

            this.Loaded += UCWizardAgreement_Loaded;
            this.ButnAdd.Click += ButnAdd_Click;
            this.ButnBack.Click += ButnBack_Click;
            this.ButnNext.Click += ButnNext_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.CombVoip.ItemsSource = ListAgreementItems;
        }

        void UCWizardAgreement_Loaded(object sender, RoutedEventArgs e)
        {
            InitControlObject();
            ChangeLanguage();
        }

        private void InitControlObject()
        {
            ListAgreements = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 223 && p.ParentID == RecordConfigObj.ObjectID).ToList();
            //if (RecordAgreementNumber == 1)
            //{
            //    RecordAgreementNumber = ListAgreements.Count();
            //}
            int ListCount = ListAgreements.Count;
            List<BasicInfoData> TempBasicInfo = WizardHelper.ListAllBasicInfos.Where(p => p.InfoID == 111000210).ToList();
            foreach (BasicInfoData info in TempBasicInfo)
            {
                PropertyValueEnumItem propertyValueItem = new PropertyValueEnumItem();
                propertyValueItem.IsCheckedChanged += EnumItem_IsCheckedChanged;
                propertyValueItem.Value = info.Value;
                propertyValueItem.Display =
                    CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                        info.Icon);
                propertyValueItem.Info = info;
                ListAgreementItems.Add(propertyValueItem);
            }
            if (ListCount >= RecordAgreementNumber)
            {
                //加载显示已有项
                CurrentConfigObj = ListAgreements[RecordAgreementNumber - 1];
                ResourceProperty NetworkName = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 22);
                if (NetworkName != null)
                {
                    this.TexPBX.Text = NetworkName.Value;
                }
                ResourceProperty mPropertyValue = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 11);
                if (mPropertyValue != null)
                {
                    int value = 0;
                    if (int.TryParse(mPropertyValue.Value, out value))
                    {
                        for (int i = 0; i < ListAgreementItems.Count; i++)
                        {
                            if (ListAgreementItems[i].Value == value.ToString())
                            {
                                this.CombVoip.SelectedIndex = i;
                            }
                        }
                    }
                }
            }
            else
            {
                List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 223 && p.ParentID == RecordConfigObj.ObjectID).ToList().Count();
                for (int i = 0; i < tempObjItem.Count; i++)
                {
                    ConfigGroup tempCG = tempObjItem[i].Data as ConfigGroup;
                    if (tempCG.GroupID == 27 && tempCG.ChildType == 223 && tempCG.ConfigObject == RecordConfigObj)
                    {
                        int MinValue = tempCG.GroupInfo.IntValue01; int MaxValue = tempCG.GroupInfo.IntValue02;
                        if ((ReadyNumber + 1) > MaxValue || (ReadyNumber + 1) < MinValue)
                        { return; }
                        //WizardHelper.ListAllConfigObjects.Add(WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 222));
                        CurrentConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 223);
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
                for (int i = 0; i < ListAgreementItems.Count; i++)
                {
                    if (ListAgreementItems[i].Value == Value)
                    {
                        IsFound = true;
                        this.CombVoip.SelectedIndex = i;
                    }
                }
                if (!IsFound)
                {
                    this.CombVoip.Text = Value;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            //前一页。看当前配了几个啦
            SaveRecordAgreement();
            RecordAgreementNumber--;
            if (RecordAgreementNumber == 0)
            {
                MainPage.PopupPanel.Content = PrePage;
            }
            else
            {
                //打开前一项
                InitControlObject();
            }
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            SaveRecordAgreement();
            if (RecordAgreementNumber < ListAgreements.Count)
            {
                //加载下一项的内容
                RecordAgreementNumber++;
                InitControlObject();
            }
            else
            {
                //下一页。进入界面6，并发配置
                UCWizardConcurrency ucwizard = new UCWizardConcurrency();
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
        }

        private void SaveRecordAgreement()
        {
            if (this.TexPBX.Text != string.Empty)
            {
                ResourceProperty TempResourceProper = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 22);
                if (TempResourceProper != null)
                {
                    TempResourceProper.Value = this.TexPBX.Text;
                }
            }
            if (this.CombVoip.SelectedIndex != -1)
            {
                ResourceProperty TempResourceProper = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 11);
                if (TempResourceProper != null)
                {
                    PropertyValueEnumItem propertyItem = this.CombVoip.SelectedItem as PropertyValueEnumItem;
                    TempResourceProper.Value = propertyItem.Value;
                }
            }
            CurrentConfigObj.GetBasicPropertyValues();
            bool IsFind = false;
            List<ConfigObject> TempList = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 223 && p.ParentID == RecordConfigObj.ObjectID).ToList();
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
            if (RecordAgreementNumber > ListAgreements.Count)
            {
                ListAgreements.Add(CurrentConfigObj);
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
            SaveRecordAgreement();
            RecordAgreementNumber++;
            //添加一个协议，弹出自己界面
            this.CombVoip.SelectedIndex = -1;
            this.TexPBX.Text = "2000";
            InitControlObject();
        }

        public override void ChangeLanguage()
        {
            this.ButnAdd.Content = CurrentApp.GetLanguageInfo("1110WIZB0004", "添加");
            this.ButnBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ05001", "协议：IP");

            this.LabPBX.Content = CurrentApp.GetLanguageInfo("1110WIZ05003", "PBX端口:");
            this.LabVoip.Content = CurrentApp.GetLanguageInfo("1110WIZ05002", "Voip协议：");

            for (int i = 0; i < ListAgreementItems.Count; i++)
            {
                PropertyValueEnumItem item = ListAgreementItems[i];
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
