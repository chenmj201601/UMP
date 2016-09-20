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
    /// UCWizardCTIConfig.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardCTIConfig : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardScreenChannel PrePageChan;
        public UCWizardScreenChannelAdd PrePageAdd;
        public UCWizardMachineConfig PrePage;
        public UCWizardRecordChannel PreRecordChan;
        public UCWizardRecordChannelAdd PreRecordAdd;
        public bool IsAsk;

        private ConfigObject CTIConfigObject;//243
        private ObservableCollection<PropertyValueEnumItem> mListCTITypeItems;
        private List<ConfigObject> mListCTIConfigObjs;
        private ObjectItem CTIObjItem;//4-1
        private string IntCTIType;
        public int CTICount;

        public UCWizardCTIConfig()
        {
            InitializeComponent();

            CTICount = 0;
            CTIObjItem = new ObjectItem();
            CTIConfigObject = new ConfigObject();
            mListCTIConfigObjs = new List<ConfigObject>();
            mListCTITypeItems = new ObservableCollection<PropertyValueEnumItem>();
            this.Loaded += UCWizardCTIConfig_Loaded;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.ButnNext.Click += ButnNext_Click;
            this.ButnBack.Click += ButnBack_Click;
            this.ComCTIType.ItemsSource = mListCTITypeItems;
        }

        void ButnBack_Click(object sender, RoutedEventArgs e)
        {
            //下一页
            if (PrePageChan != null)
            {
                MainPage.PopupPanel.Content = PrePageChan;
            }
            else if (PrePageAdd != null)
            {
                MainPage.PopupPanel.Content = PrePageAdd;
            }
            else if (PrePage != null)
            {
                MainPage.PopupPanel.Content = PrePage;
            }
            else if (PreRecordAdd != null)
            {
                MainPage.PopupPanel.Content = PreRecordAdd;
            }
            else
            {
                MainPage.PopupPanel.Content = PreRecordChan;
            }
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            //下一页
            if (IsAsk)
            {
                IsAsk = false;
                InitControlObject();
                ChangeLanguage();
            }
            else
            {
                //保存，进入下一页（相应的CTI类型的配置界面）
                SaveCTIConfig();
                switch (IntCTIType)
                {
                    case "2":
                        UCCTIWizardGenesys ucwizard = new UCCTIWizardGenesys();
                        ucwizard.CurrentApp = CurrentApp;
                        ucwizard.MainPage = MainPage;
                        ucwizard.PrePage = this;
                        ucwizard.mListCtiConfigObjs = mListCTIConfigObjs;
                        ucwizard.CtiObjItem = CTIObjItem;
                        ucwizard.WizardHelper = this.WizardHelper;
                        MainPage.PopupPanel.Title = "Config Wizard";
                        MainPage.PopupPanel.Content = ucwizard;
                        MainPage.PopupPanel.IsOpen = true;
                        break;
                    case "1":
                    case "3":
                    case "7":
                    case "9":
                        UCCTIWizardCVCTother ucwizardOthers = new UCCTIWizardCVCTother();
                        ucwizardOthers.CurrentApp = CurrentApp;
                        ucwizardOthers.MainPage = MainPage;
                        ucwizardOthers.PrePage = this;
                        ucwizardOthers.mListCtiConfigObjs = mListCTIConfigObjs;
                        ucwizardOthers.CtiObjItem = CTIObjItem;
                        ucwizardOthers.IsServiceName = false;
                        ucwizardOthers.WizardHelper = this.WizardHelper;
                        MainPage.PopupPanel.Title = "Config Wizard";
                        MainPage.PopupPanel.Content = ucwizardOthers;
                        MainPage.PopupPanel.IsOpen = true;
                        break;
                    case "4":
                        UCCTIWizardCVCTother ucwizardOther = new UCCTIWizardCVCTother();
                        ucwizardOther.CurrentApp = CurrentApp;
                        ucwizardOther.MainPage = MainPage;
                        ucwizardOther.PrePage = this;
                        ucwizardOther.mListCtiConfigObjs = mListCTIConfigObjs;
                        ucwizardOther.CtiObjItem = CTIObjItem;
                        ucwizardOther.WizardHelper = this.WizardHelper;
                        ucwizardOther.IsServiceName = true;
                        MainPage.PopupPanel.Title = "Config Wizard";
                        MainPage.PopupPanel.Content = ucwizardOther;
                        MainPage.PopupPanel.IsOpen = true;
                        break;
                    case "5":
                        UCCTIWizardAES ucwizardAES = new UCCTIWizardAES();
                        ucwizardAES.CurrentApp = CurrentApp;
                        ucwizardAES.MainPage = MainPage;
                        ucwizardAES.PrePage = this;
                        ucwizardAES.mListCtiConfigObjs = mListCTIConfigObjs;
                        ucwizardAES.CtiObjItem = CTIObjItem;
                        ucwizardAES.WizardHelper = this.WizardHelper;
                        MainPage.PopupPanel.Title = "Config Wizard";
                        MainPage.PopupPanel.Content = ucwizardAES;
                        MainPage.PopupPanel.IsOpen = true;
                        break;
                    case "6":
                        UCCTIWizardAIC ucwizardAIC = new UCCTIWizardAIC();
                        ucwizardAIC.CurrentApp = CurrentApp;
                        ucwizardAIC.MainPage = MainPage;
                        ucwizardAIC.PrePage = this;
                        ucwizardAIC.mListCtiConfigObjs = mListCTIConfigObjs;
                        ucwizardAIC.CtiObjItem = CTIObjItem;
                        ucwizardAIC.WizardHelper = this.WizardHelper;
                        MainPage.PopupPanel.Title = "Config Wizard";
                        MainPage.PopupPanel.Content = ucwizardAIC;
                        MainPage.PopupPanel.IsOpen = true;
                        break;
                    case "8":
                        UCCTIWizardCTIOS ucwizardCTIOS = new UCCTIWizardCTIOS();
                        ucwizardCTIOS.CurrentApp = CurrentApp;
                        ucwizardCTIOS.MainPage = MainPage;
                        ucwizardCTIOS.PrePage = this;
                        ucwizardCTIOS.mListCtiConfigObjs = mListCTIConfigObjs;
                        ucwizardCTIOS.CtiObjItem = CTIObjItem;
                        ucwizardCTIOS.WizardHelper = this.WizardHelper;
                        MainPage.PopupPanel.Title = "Config Wizard";
                        MainPage.PopupPanel.Content = ucwizardCTIOS;
                        MainPage.PopupPanel.IsOpen = true;
                        break;
                }
            }
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            //前一页=返回
            if (IsAsk)
            {
                //下一页 下载参数
                UCWizardDownloadConfig ucwizard = new UCWizardDownloadConfig();
                ucwizard.MainPage = MainPage;
                ucwizard.PrePage = this;
                ucwizard.CurrentApp = CurrentApp;
                ucwizard.WizardHelper = this.WizardHelper;
                MainPage.PopupPanel.Title = "Config Wizard";
                MainPage.PopupPanel.Content = ucwizard;
                MainPage.PopupPanel.IsOpen = true;
            }
            else
            {
                //回到选择CTI类型界面
                IsAsk = true;
                InitControlObject();
                ChangeLanguage();
            }
        }

        void UCWizardCTIConfig_Loaded(object sender, RoutedEventArgs e)
        {
            InitControlObject();
            ChangeLanguage();
        }

        private void InitControlObject()
        {
            if (IsAsk)
            {
                this.LabCTIType.Visibility = Visibility.Collapsed;
                this.ComCTIType.Visibility = Visibility.Collapsed;

                this.LabIsConfig.Visibility = Visibility.Visible;
                this.ButnBack.Visibility = Visibility.Visible;
            }
            else
            {
                this.LabIsConfig.Visibility = Visibility.Collapsed;

                this.LabCTIType.Visibility = Visibility.Visible;
                this.ComCTIType.Visibility = Visibility.Visible;

                this.ButnBack.Visibility = Visibility.Collapsed;
                InitCTIType();
            }
        }

        private void InitCTIType()
        {
            //加载CTI类型下拉对象
            List<BasicInfoData> TempBasicInfo = WizardHelper.ListAllBasicInfos.Where(p => p.InfoID == 111000300).ToList();
            mListCTITypeItems.Clear();
            foreach (BasicInfoData info in TempBasicInfo)
            {
                PropertyValueEnumItem propertyValueItem = new PropertyValueEnumItem();
                propertyValueItem.Value = info.Value;
                propertyValueItem.Display =
                    CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                        info.Icon);
                propertyValueItem.Info = info;
                mListCTITypeItems.Add(propertyValueItem);
            }
        }

        private void Init()//在保存前生成243
        {
            //生成243的ConfigObject对象,之前要检查是否已经建立了这个类型的CTI对象
            List<ConfigObject> TempReadyCTI = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 243).ToList();
            var PageValue = this.ComCTIType.SelectedItem; bool IsFind = false;
            if (PageValue != null)
            {
                string pValue = (PageValue as PropertyValueEnumItem).Value;
                for (int i = 0; i < TempReadyCTI.Count; i++)
                {
                    string properValueofList = TempReadyCTI[i].ListProperties.FirstOrDefault(p => p.PropertyID == 11).Value;
                    if (properValueofList == pValue)
                    {
                        IsFind = true;
                        CTIConfigObject = TempReadyCTI[i];
                        break;
                    }
                }
            }
            List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
            //int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 243).ToList().Count();
            int j = 0; ConfigGroup tempCG = new ConfigGroup();
            for (; j < tempObjItem.Count; j++)
            {
                tempCG = tempObjItem[j].Data as ConfigGroup;
                if (tempCG.GroupID == 51 && tempCG.ChildType == 243 && tempCG.TypeID == 0)
                {
                    CTIObjItem = tempObjItem[j];
                    //int MinValue = tempCG.GroupInfo.IntValue01; int MaxValue = tempCG.GroupInfo.IntValue02;
                    //if ((ReadyNumber + 1) > MaxValue || (ReadyNumber + 1) < MinValue)
                    //{ return; }
                    //CTIConfigObject = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 243);
                    break;
                }
            }
            if (!IsFind)
            {
                CTIConfigObject = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 243);
            }
        }

        private void SaveCTIConfig()
        {
            Init();
            //生成242ConfigObject，绑给CTIConfigObject的ListChildObject，写入CTIConfigObject是CTI类型属性值。将CTIConfigObject写入mList

            if (this.ComCTIType.SelectedIndex != -1)
            {
                var propertyItem = this.ComCTIType.SelectedItem as PropertyValueEnumItem;
                if (propertyItem != null)
                {
                    IntCTIType = propertyItem.Value;
                }
                var property = CTIConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == 11);
                if (property != null)
                {
                    property.Value = IntCTIType;
                }
            }
            ConfigObject CTIGroup = WizardHelper.CreateNewConfigObject(CTIConfigObject, 242);
            var property242 = CTIGroup.ListProperties.FirstOrDefault(p => p.PropertyID == 11);
            if (property242 != null)
            {
                property242.Value = IntCTIType;
            }
            mListCTIConfigObjs.Add(CTIConfigObject);
            mListCTIConfigObjs.Add(CTIGroup);
            MainPage.RefreshConfigObjectItem(CTIObjItem);
            CTICount++;
        }

        public WizardHelper WizardHelper { get; set; }

        public override void ChangeLanguage()
        {
            this.ButnBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            if (IsAsk)
            {
                this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0009", "取消");
                this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0008", "确定");
            }
            else
            {
                this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
                this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            }
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ12001", "CTI");

            this.LabIsConfig.Content = string.Format(CurrentApp.GetLanguageInfo("1110WIZ12002", "是否需要配置CTI参数？"), CTICount);
            this.LabCTIType.Content = CurrentApp.GetLanguageInfo("1110WIZ13001", "CTI类型：");
        }

        public void LoopLoad()
        {
            IsAsk = true;
            this.ComCTIType.SelectedIndex = -1;
            mListCTIConfigObjs.Clear();
            InitControlObject();
        }
    }
}
