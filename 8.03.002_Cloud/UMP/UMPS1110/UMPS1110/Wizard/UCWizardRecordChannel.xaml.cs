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
    /// UCWizardRecordChannel.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardRecordChannel : IWizard
    {
        public ConfigObject RecordConfigObj;
        public UCWizardConcurrency PrePage;
        public ResourceMainView MainPage;

        private ObservableCollection<PropertyValueEnumItem> ListStartupItems;
        private List<ConfigObject> ListRecordChannels;
        private List<ConfigObject> ListReadyExistChans;
        private ConfigObject CurrentConfigObj;
        private ObjectItem ParentObjectItem;
        private int RecordChannelNumber;

        public UCWizardRecordChannel()
        {
            InitializeComponent();

            RecordChannelNumber = 1;
            CurrentConfigObj = new ConfigObject();
            ListStartupItems = new ObservableCollection<PropertyValueEnumItem>();
            ListReadyExistChans = new List<ConfigObject>();
            this.Loaded += UCWizardRecordChannel_Loaded;
            this.ButnBack.Click += ButnBack_Click;
            this.ButnAdd.Click += ButnAdd_Click;
            this.ButnBatch.Click += ButnBatch_Click;
            this.ButnNext.Click += ButnNext_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.CombStartupMode.ItemsSource = ListStartupItems;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (this.IPTextBox.GetIP() != string.Empty && this.IPTextBox.GetIP() != "0.0.0.0")
            SaveRecordChannel();
            //可能是上一页，也可能是上一项
            RecordChannelNumber--;
            if (RecordChannelNumber == 0)
            {
                MainPage.PopupPanel.Content = PrePage;
            }
            else
            {
                InitControlObject();
            }
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            //打开界面9：录屏服务配置参数
            if (this.IPTextBox.GetIP() != string.Empty && this.IPTextBox.GetIP() != "0.0.0.0")
            SaveRecordChannel();
            if (RecordChannelNumber < ListRecordChannels.Count)
            {
                //加载下一项的内容
                RecordChannelNumber++;
                InitControlObject();
            }
            else
            {
                for (int i = 0; i < WizardHelper.ListRecords.Count; i++)
                {
                    if (RecordConfigObj.ObjectID == WizardHelper.ListRecords[i].ObjectID)
                    {
                        if (i == (WizardHelper.ListRecords.Count - 1))
                        {
                            if (WizardHelper.ListScreens != null && WizardHelper.ListScreens.Count != 0)
                            {
                                //下一页。进入界面9:录屏参数配置
                                UCWizardScreenConfig ucwizard = new UCWizardScreenConfig();
                                ucwizard.MainPage = MainPage;
                                ucwizard.ChanPrepage = this;
                                ucwizard.CurrentApp = CurrentApp;
                                //ucwizard.RecordConfigObj = this.RecordConfigObj;
                                //ucwizard.ParentItem = this.ParentObjectItem;
                                ucwizard.WizardHelper = this.WizardHelper;
                                //ucwizard.ListConfigObjects = ListConfigObjects;
                                MainPage.PopupPanel.Title = "Config Wizard";
                                MainPage.PopupPanel.Content = ucwizard;
                                MainPage.PopupPanel.IsOpen = true;
                            }
                            else
                            {
                                UCWizardCTIConfig ucwizard = new UCWizardCTIConfig();
                                ucwizard.MainPage = MainPage;
                                ucwizard.CurrentApp = CurrentApp;
                                ucwizard.PreRecordChan = this;
                                //ucwizard.ScreenConfigObj = this.ScreenSeviceConfigObj;
                                //ucwizard.ParentItem = this.ParentObjectItem;
                                ucwizard.IsAsk = true;
                                ucwizard.WizardHelper = this.WizardHelper;
                                //ucwizard.ListConfigObjects = ListConfigObjects;
                                MainPage.PopupPanel.Title = "Config Wizard";
                                MainPage.PopupPanel.Content = ucwizard;
                                MainPage.PopupPanel.IsOpen = true;
                            }
                            break;
                            
                        }
                        else
                        {
                            //循环
                            WizardHelper.UCRecordConfig.LoopLoad();
                            MainPage.PopupPanel.Content = WizardHelper.UCRecordConfig;
                        }
                    }
                }                
            }
        }

        void ButnBatch_Click(object sender, RoutedEventArgs e)
        {
            if (this.IPTextBox.GetIP() != string.Empty && this.IPTextBox.GetIP() != "0.0.0.0")
            SaveRecordChannel();
            //打开第二个通道添加界面8
            UCWizardRecordChannelAdd ucwizard = new UCWizardRecordChannelAdd();
            ucwizard.MainPage = MainPage;
            ucwizard.PrePage = this;
            ucwizard.CurrentApp = CurrentApp;
            ucwizard.RecordConfigObj = this.RecordConfigObj;
            ucwizard.ParentItem = this.ParentObjectItem;
            ucwizard.WizardHelper = this.WizardHelper;
            //ucwizard.ListConfigObjects = ListConfigObjects;
            MainPage.PopupPanel.Title = "Config Wizard";
            MainPage.PopupPanel.Content = ucwizard;
            MainPage.PopupPanel.IsOpen = true;
        }

        void ButnAdd_Click(object sender, RoutedEventArgs e)
        {
            SaveRecordChannel();
            RecordChannelNumber++;
            string StrIP = this.IPTextBox.GetIP();
            string[] IPList = StrIP.Split('.');
            StrIP = string.Empty;
            int IPFour = Convert.ToInt32(IPList[IPList.Count() - 1]);
            IPFour++;
            if (IPFour < 255)
            {
                IPList[IPList.Count() - 1] = IPFour.ToString();
            }
            else
            {
                IPList[IPList.Count() - 1] = "254";
            }
            for (int i = 0; i < IPList.Count(); i++)
            {
                StrIP += IPList[i];
            }
            this.IPTextBox.SetIP(StrIP);
            this.TexChanName.Text = string.Empty;
            this.CombStartupMode.SelectedIndex = -1;
            InitControlObject();
        }

        void ButnBack_Click(object sender, RoutedEventArgs e)
        {
            MainPage.PopupPanel.Content = PrePage;
        }

        void UCWizardRecordChannel_Loaded(object sender, RoutedEventArgs e)
        {
            InitControlObject();
            ChangeLanguage();
        }

        private void InitControlObject()
        {
            ListReadyExistChans = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 225 && p.ParentID == RecordConfigObj.ObjectID).ToList();
            ListRecordChannels = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 225 && p.ParentID == RecordConfigObj.ObjectID).ToList();
            //if (RecordAgreementNumber == 1)
            //{
            //    RecordAgreementNumber = ListAgreements.Count();
            //}
            int ListCount = ListRecordChannels.Count;
            List<BasicInfoData> TempBasicInfo = WizardHelper.ListAllBasicInfos.Where(p => p.InfoID == 111000100).ToList();
            ListStartupItems.Clear();
            foreach (BasicInfoData info in TempBasicInfo)
            {
                PropertyValueEnumItem propertyValueItem = new PropertyValueEnumItem();
                propertyValueItem.IsCheckedChanged += EnumItem_IsCheckedChanged;
                propertyValueItem.Value = info.Value;
                propertyValueItem.Display =
                    CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                        info.Icon);
                propertyValueItem.Info = info;
                ListStartupItems.Add(propertyValueItem);
            }
            if (ListCount >= RecordChannelNumber)
            {
                //加载显示已有项
                CurrentConfigObj = ListRecordChannels[RecordChannelNumber - 1];
                ResourceProperty NetworkName = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                if (NetworkName != null)
                {
                    this.TexChanName.Text = NetworkName.Value;
                }
                ResourceProperty ChannelIP = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 13);
                if (ChannelIP != null)
                {
                    this.TexChanName.Text = ChannelIP.Value;
                }
                ResourceProperty mPropertyValue = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 21);
                if (mPropertyValue != null)
                {
                    int value = 0;
                    if (int.TryParse(mPropertyValue.Value, out value))
                    {
                        for (int i = 0; i < ListStartupItems.Count; i++)
                        {
                            if (ListStartupItems[i].Value == value.ToString())
                            {
                                this.CombStartupMode.SelectedIndex = i;
                            }
                        }
                    }
                }
            }
            else
            {
                List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 225 && p.ParentID == RecordConfigObj.ObjectID).ToList().Count();
                for (int i = 0; i < tempObjItem.Count; i++)
                {
                    ConfigGroup tempCG = tempObjItem[i].Data as ConfigGroup;
                    if (tempCG.GroupID == 25 && tempCG.ChildType == 225 && tempCG.ConfigObject == RecordConfigObj)
                    {
                        int MinValue = tempCG.GroupInfo.IntValue01; int MaxValue = tempCG.GroupInfo.IntValue02;
                        if ((ReadyNumber + 1) > MaxValue || (ReadyNumber + 1) < MinValue)
                        { return; }
                        CurrentConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 225);
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
                for (int i = 0; i < ListStartupItems.Count; i++)
                {
                    if (ListStartupItems[i].Value == Value)
                    {
                        IsFound = true;
                        this.CombStartupMode.SelectedIndex = i;
                    }
                }
                if (!IsFound)
                {
                    this.CombStartupMode.Text = Value;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveRecordChannel()
        {
            if (this.IPTextBox.GetIP() != string.Empty && this.IPTextBox.GetIP() != "0.0.0.0")
            {
                ResourceProperty TempResourceProper = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 13);
                if (TempResourceProper != null)
                {
                    CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 13).Value = this.IPTextBox.GetIP();
                }
            }
            
            if (this.TexChanName.Text != string.Empty)
            {
                ResourceProperty TempResourceProper = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                if (TempResourceProper != null)
                {
                    CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 12).Value = this.TexChanName.Text;
                }
            }
            
            if (this.CombStartupMode.SelectedIndex != -1)
            {
                ResourceProperty TempResourceProper = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 21);
                if (TempResourceProper != null)
                {
                    PropertyValueEnumItem propertyItem = this.CombStartupMode.SelectedItem as PropertyValueEnumItem;
                    CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 21).Value = propertyItem.Value;
                }
            }
           
            CurrentConfigObj.GetBasicPropertyValues();
            bool IsFind = false;
            //List<ConfigObject> TempList = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 225 && p.ParentID == RecordConfigObj.ObjectID).ToList();
            for (int i = 0; i < ListReadyExistChans.Count; i++)
            {
                if (ListReadyExistChans[i].ObjectID == CurrentConfigObj.ObjectID)
                {
                    IsFind = true;
                }
            }
            if (!IsFind)
            {
                WizardHelper.ListAllConfigObjects.Add(CurrentConfigObj);
                //WizardHelper.ListRecords.Add(CurrentConfigObj);
                ListRecordChannels.Add(CurrentConfigObj);
            }
            if (RecordChannelNumber > ListRecordChannels.Count)
            {
                ListRecordChannels.Add(CurrentConfigObj);
            }
            if (ParentObjectItem != null)
            {
                MainPage.CreateNewChannelConfigObjectItem(ParentObjectItem, CurrentConfigObj);
                //MainPage.RefreshConfigObjectItem(ParentObjectItem);
            }
        }

        public WizardHelper WizardHelper { get; set; }

        public override void ChangeLanguage()
        {
            this.LabChanIP.Content = CurrentApp.GetLanguageInfo("1110WIZ07002", "通道IP:");
            this.LabChanName.Content = CurrentApp.GetLanguageInfo("1110WIZ07003", "通道名称：");
            this.LabStartupMode.Content = CurrentApp.GetLanguageInfo("1110WIZ07004", "启动方式：");

            this.ButnBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            this.ButnAdd.Content = CurrentApp.GetLanguageInfo("1110WIZB0004", "Add");
            this.ButnBatch.Content = CurrentApp.GetLanguageInfo("1110WIZB0005", "Batch");
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ07001", "录音通道");

            for (int i = 0; i < ListStartupItems.Count; i++)
            {
                PropertyValueEnumItem item = ListStartupItems[i];
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
