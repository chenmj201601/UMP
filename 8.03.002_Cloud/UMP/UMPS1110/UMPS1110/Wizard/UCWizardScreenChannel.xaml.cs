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
    /// UCWizardScreenChannel.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardScreenChannel : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardScreenConfig PrePage;
        public ConfigObject ScreenSeviceConfigObj;

        private ConfigObject CurrentConfigObj;
        private List<ConfigObject> ListScreenConfigObjs;
        private ObservableCollection<PropertyValueEnumItem> ListScreenStartupModeItems;
        private ObservableCollection<PropertyValueEnumItem> ListScreenStopModeItems;
        private int ScreenChanNumber;
        private ObjectItem ParentObjectItem;

        public UCWizardScreenChannel()
        {
            InitializeComponent();

            ScreenChanNumber = 1;
            CurrentConfigObj = new ConfigObject();
            ListScreenStartupModeItems = new ObservableCollection<PropertyValueEnumItem>();
            ListScreenStopModeItems = new ObservableCollection<PropertyValueEnumItem>();

            this.CombStartup.ItemsSource = ListScreenStartupModeItems;
            this.CombStop.ItemsSource = ListScreenStopModeItems;

            this.Loaded += UCWizardScreenChannel_Loaded;
            this.ButnAdd.Click += ButnAdd_Click;
            this.ButnBack.Click += ButnBack_Click;
            this.ButnBatch.Click += ButnBatch_Click;
            this.ButnNext.Click += ButnNext_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            //前一页
            ScreenChanNumber--;
            if (ScreenChanNumber == 0)
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
            //下一页
            SaveScreenChannel();
            for (int i = 0; i < WizardHelper.ListScreens.Count; i++)
            {
                if (ScreenSeviceConfigObj.ObjectID == WizardHelper.ListScreens[i].ObjectID)
                {
                    if (i == (WizardHelper.ListScreens.Count - 1))
                    {
                        UCWizardCTIConfig ucwizard = new UCWizardCTIConfig();
                        ucwizard.MainPage = MainPage;
                        ucwizard.PrePageChan = this;
                        ucwizard.CurrentApp = CurrentApp;
                        //ucwizard.ScreenConfigObj = this.ScreenSeviceConfigObj;
                        //ucwizard.ParentItem = this.ParentObjectItem;
                        ucwizard.IsAsk = true;
                        ucwizard.WizardHelper = this.WizardHelper;
                        //ucwizard.ListConfigObjects = ListConfigObjects;
                        MainPage.PopupPanel.Title = "Config Wizard";
                        MainPage.PopupPanel.Content = ucwizard;
                        MainPage.PopupPanel.IsOpen = true;
                    }
                    else
                    {
                        WizardHelper.UCScreenConfig.LoopLoad();
                        MainPage.PopupPanel.Content = WizardHelper.UCScreenConfig;
                    }
                }
            }
            
        }

        void ButnBatch_Click(object sender, RoutedEventArgs e)
        {
            //批量按钮
            SaveScreenChannel();
            UCWizardScreenChannelAdd ucwizard = new UCWizardScreenChannelAdd();
            ucwizard.MainPage = MainPage;
            ucwizard.PrePage = this;
            ucwizard.CurrentApp = CurrentApp;
            ucwizard.ScreenConfigObj = this.ScreenSeviceConfigObj;
            ucwizard.ParentItem = this.ParentObjectItem;
            ucwizard.WizardHelper = this.WizardHelper;
            //ucwizard.ListConfigObjects = ListConfigObjects;
            MainPage.PopupPanel.Title = "Config Wizard";
            MainPage.PopupPanel.Content = ucwizard;
            MainPage.PopupPanel.IsOpen = true;
        }

        void ButnBack_Click(object sender, RoutedEventArgs e)
        {
            //返回按钮
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButnAdd_Click(object sender, RoutedEventArgs e)
        {
            //添加按钮
            SaveScreenChannel();
            ScreenChanNumber++;
           
            this.TexExtName.Text = string.Empty;
            this.CombStartup.SelectedIndex = -1;
            this.CombStop.SelectedIndex = -1;
            InitControlObject();
        }

        void UCWizardScreenChannel_Loaded(object sender, RoutedEventArgs e)
        {
            InitControlObject();
            ChangeLanguage();
        }

        private void InitControlObject()
        {
            ListScreenConfigObjs = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 232 && p.ParentID == ScreenSeviceConfigObj.ObjectID).ToList();
            //if (RecordAgreementNumber == 1)
            //{
            //    RecordAgreementNumber = ListAgreements.Count();
            //}
            int ListCount = ListScreenConfigObjs.Count;
            List<BasicInfoData> TempBasicInfo = WizardHelper.ListAllBasicInfos.Where(p => p.InfoID == 111000165).ToList();
            ListScreenStartupModeItems.Clear();
            foreach (BasicInfoData info in TempBasicInfo)
            {
                PropertyValueEnumItem propertyValueItem = new PropertyValueEnumItem();
                propertyValueItem.IsCheckedChanged += EnumItem_IsCheckedChanged;
                propertyValueItem.Value = info.Value;
                propertyValueItem.Display =
                    CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                        info.Icon);
                propertyValueItem.Info = info;
                ListScreenStartupModeItems.Add(propertyValueItem);
            }
            List<BasicInfoData> TempBasicInfoStop = WizardHelper.ListAllBasicInfos.Where(p => p.InfoID == 111000166).ToList();
            ListScreenStopModeItems.Clear();
            foreach (BasicInfoData info in TempBasicInfoStop)
            {
                PropertyValueEnumItem propertyValueItem = new PropertyValueEnumItem();
                propertyValueItem.IsCheckedChanged += StopEnumItem_IsCheckedChanged;
                propertyValueItem.Value = info.Value;
                propertyValueItem.Display =
                    CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                        info.Icon);
                propertyValueItem.Info = info;
                ListScreenStopModeItems.Add(propertyValueItem);
            }
            if (ListCount >= ScreenChanNumber)
            {
                //加载显示已有项
                CurrentConfigObj = ListScreenConfigObjs[ScreenChanNumber - 1];
                ResourceProperty ExtName = CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                if (ExtName != null)
                {
                    this.TexExtName.Text = ExtName.Value;
                }
                ResourceProperty mPropertyValue = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 15);
                if (mPropertyValue != null)
                {
                    int value = 0;
                    if (int.TryParse(mPropertyValue.Value, out value))
                    {
                        for (int i = 0; i < ListScreenStartupModeItems.Count; i++)
                        {
                            if (ListScreenStartupModeItems[i].Value == value.ToString())
                            {
                                this.CombStartup.SelectedIndex = i;
                            }
                        }
                    }
                }
                ResourceProperty mStopPropertyValue = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 16);
                if (mStopPropertyValue != null)
                {
                    int value = 0;
                    if (int.TryParse(mStopPropertyValue.Value, out value))
                    {
                        for (int i = 0; i < ListScreenStopModeItems.Count; i++)
                        {
                            if (ListScreenStopModeItems[i].Value == value.ToString())
                            {
                                this.CombStop.SelectedIndex = i;
                            }
                        }
                    }
                }
            }
            else
            {
                List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 232 && p.ParentID == ScreenSeviceConfigObj.ObjectID).ToList().Count();
                for (int i = 0; i < tempObjItem.Count; i++)
                {
                    ConfigGroup tempCG = tempObjItem[i].Data as ConfigGroup;
                    if (tempCG.GroupID == 1 && tempCG.ChildType == 232 && tempCG.ConfigObject == ScreenSeviceConfigObj)
                    {
                        int MinValue = tempCG.GroupInfo.IntValue01; int MaxValue = tempCG.GroupInfo.IntValue02;
                        if ((ReadyNumber + 1) > MaxValue || (ReadyNumber + 1) < MinValue)
                        { return; }
                        CurrentConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 232);
                        CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 3).Value = ScreenSeviceConfigObj.ObjectID.ToString();
                        ParentObjectItem = tempObjItem[i];
                        break;
                    }
                }
            }

        }

        private void EnumItem_IsCheckedChanged()
        {
            try
            {
                int intValue = 0;
                int intTemp;
                string strText = string.Empty;
                for (int i = 0; i < ListScreenStartupModeItems.Count; i++)
                {
                    if (ListScreenStartupModeItems[i].IsChecked)
                    {
                        strText += string.Format("{0},", ListScreenStartupModeItems[i].Display);

                        if (!int.TryParse(ListScreenStartupModeItems[i].Value, out intTemp))
                        {
                            continue;
                        }
                        intValue = (intValue | intTemp);
                    }
                }
                strText = strText.TrimEnd(new[] { ',' });
                ResourceProperty mPropertyValue = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 15);
                if (mPropertyValue != null)
                {
                    mPropertyValue.Value = intValue.ToString();
                }
                this.CombStartup.Text = strText;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void StopEnumItem_IsCheckedChanged()
        {
            try
            {
                int intValue = 0;
                int intTemp;
                string strText = string.Empty;
                for (int i = 0; i < ListScreenStopModeItems.Count; i++)
                {
                    if (ListScreenStopModeItems[i].IsChecked)
                    {
                        strText += string.Format("{0},", ListScreenStopModeItems[i].Display);

                        if (!int.TryParse(ListScreenStopModeItems[i].Value, out intTemp))
                        {
                            continue;
                        }
                        intValue = (intValue | intTemp);
                    }
                }
                strText = strText.TrimEnd(new[] { ',' });
                ResourceProperty mPropertyValue = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 16);
                if (mPropertyValue != null)
                {
                    mPropertyValue.Value = intValue.ToString();
                }
                this.CombStop.Text = strText;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveScreenChannel()
        {
            if (this.TexExtName.Text != string.Empty)
            {
                if (CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 12) != null)
                {
                    CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 12).Value = this.TexExtName.Text;
                }

                CurrentConfigObj.GetBasicPropertyValues();
                MainPage.CreateNewChannelConfigObjectItem(ParentObjectItem, CurrentConfigObj);
            }
        }

        public WizardHelper WizardHelper { get; set; }

        public override void ChangeLanguage()
        {
            this.LabExtName.Content = CurrentApp.GetLanguageInfo("1110WIZ10002", "分机号:");
            this.LabStartupMode.Content = CurrentApp.GetLanguageInfo("1110WIZ10003", "录屏启动方式：");
            this.LabStopMode.Content = CurrentApp.GetLanguageInfo("1110WIZ10004", "录屏停止方式：");

            this.ButnBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            this.ButnBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0004", "Back");
            this.ButnBatch.Content = CurrentApp.GetLanguageInfo("1110WIZB0005", "Batch");
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ10001", "录屏参数配置");
        }
    }
}
