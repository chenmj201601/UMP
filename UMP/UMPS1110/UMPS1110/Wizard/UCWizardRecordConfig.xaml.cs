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
    /// UCWizardRecordConfig.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardRecordConfig : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardMachineConfig PrePage;
        //public UCWizardRecordChannel PrePageChan;
        //public UCWizardRecordChannelAdd PrePageAdd;


        public List<ConfigObject> ListConfigObjects;

        public List<ConfigObject> ListRecordConfig;
        public List<ConfigObject> ListScreenConfig;

        private int RecordNumber;
        private ObservableCollection<ConfigObject> mListDBBridgeItems;
        private ObservableCollection<ConfigObject> mListSaveDeviceItems;
        private ObjectItem ParentObjectItem;

        public UCWizardRecordConfig()
        {
            InitializeComponent();

            RecordNumber = 1;
            mListDBBridgeItems = new ObservableCollection<ConfigObject>();
            mListSaveDeviceItems = new ObservableCollection<ConfigObject>();
            this.CombDBBridge.ItemsSource = mListDBBridgeItems;
            this.CombDeviceIndex.ItemsSource = mListSaveDeviceItems;
            this.Loaded += UCWizardRecordConfig_Loaded;
            this.ButnBack.Click += ButnBack_Click;
            this.ButnNext.Click += ButnNext_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            //前一页
            RecordNumber--;
            if (RecordNumber == 0)//=返回了
            {
                PrePage.ReturnPage();
                MainPage.PopupPanel.Content = PrePage;
            }
            else//打开上一项的内容
            {
                InitControlObject();
            }
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            //下一页,保存
            SaveRecordConfig();
            //if (RecordNumber == ListRecordConfig.Count)
            {
                //打开下一个界面4
                WizardHelper.UCRecordConfig = this;
                this.WizardHelper.ListRecords = ListRecordConfig;
                this.WizardHelper.ListScreens = ListScreenConfig;
                UCWizardRecordNetWork ucwizard = new UCWizardRecordNetWork();
                ucwizard.MainPage = MainPage;
                ucwizard.PrePage = this;
                ucwizard.CurrentApp = CurrentApp;
                ucwizard.RecordConfigObj = this.ListRecordConfig[RecordNumber - 1];
                ucwizard.WizardHelper = this.WizardHelper;
                //ucwizard.ListConfigObjects = ListConfigObjects;
                MainPage.PopupPanel.Title = "Config Wizard";
                MainPage.PopupPanel.Content = ucwizard;
                MainPage.PopupPanel.IsOpen = true;
            }
            //else
            //{
            //    //打开自己页
            //    this.CombDBBridge.SelectedIndex = 0;
            //    this.CombDeviceIndex.SelectedIndex = -1;
            //    RecordNumber++;
            //}
        }

        private void SaveRecordConfig()
        {
            string RecordID = ListRecordConfig[RecordNumber - 1].ObjectID.ToString();
            ConfigObject RecordConfigObj = WizardHelper.ListAllConfigObjects.FirstOrDefault(p => p.ObjectID.ToString() == RecordID);
            if (RecordConfigObj != null)
            {
                if (this.CombDBBridge.SelectedIndex != -1)
                {
                    ConfigObject SelectItem = this.CombDBBridge.SelectedItem as ConfigObject;
                    RecordConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 16).Value = SelectItem.Description;
                }
                if (this.CombDeviceIndex.SelectedIndex != -1)
                {
                    ConfigObject SelectItem = this.CombDeviceIndex.SelectedItem as ConfigObject;
                    RecordConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 192).Value = SelectItem.Description;
                }
            }
        }

        void ButnBack_Click(object sender, RoutedEventArgs e)
        {
            //打开界面2
            PrePage.ReturnPage();
            MainPage.PopupPanel.Content = PrePage;
        }

        void UCWizardRecordConfig_Loaded(object sender, RoutedEventArgs e)
        {
            InitControlObject();
            ChangeLanguage();
        }

        private void InitControlObject()
        {
            string RecordID = string.Empty;
            if (ListRecordConfig != null && ListRecordConfig.Count != 0)
            {
                var IPName = ListRecordConfig[RecordNumber - 1].ListProperties.FirstOrDefault(p => p.PropertyID == 7);
                string IP = "";
                if (IPName != null)
                {
                    IP = IPName.Value;
                }
                if (IP != null && IP != string.Empty)
                {
                    this.IPTextBox.SetIP(IP);
                }
                else
                {
                    this.IPTextBox.SetIP("127.0.0.1");
                }
                RecordID = ListRecordConfig[RecordNumber - 1].ObjectID.ToString();
            }
            this.IPTextBox.IsEnabled = false;
            //DBBridge和设备索引的Comb加载项
            List<ConfigObject> TempListItems = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 215).ToList();
            foreach (ConfigObject co in TempListItems)
            {
                mListDBBridgeItems.Add(co);
            }
            TempListItems = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 214).ToList();
            foreach (ConfigObject co in TempListItems)
            {
                mListSaveDeviceItems.Add(co);
            }
            //加载
            ConfigObject RecordConfigObj = WizardHelper.ListAllConfigObjects.FirstOrDefault(p => p.ObjectID.ToString() == RecordID);
            string DBValue = string.Empty; string DevieValue = string.Empty;
            if (RecordConfigObj != null)
            {
                DBValue = RecordConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 16).Value;
                DevieValue = RecordConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 192).Value;
            }
            for (int i = 0; i < mListDBBridgeItems.Count; i++)
            {
                string ConfigObjValue = mListDBBridgeItems[i].Description;
                if (ConfigObjValue == DBValue)
                {
                    this.CombDBBridge.SelectedIndex = i; break;
                }
            }
            for (int i = 0; i < mListSaveDeviceItems.Count; i++)
            {
                string ConfigObjValue = mListSaveDeviceItems[i].Description;
                if (ConfigObjValue == DBValue)
                {
                    this.CombDeviceIndex.SelectedIndex = i; break;
                }
            }
        }

        public override void ChangeLanguage()
        {
            this.LabDBBridge.Content = CurrentApp.GetLanguageInfo("1110WIZ03004", "DBBridge:");
            this.LabDeviceIndex.Content = CurrentApp.GetLanguageInfo("1110WIZ03005", "设备索引:");
            this.LabIP.Content = CurrentApp.GetLanguageInfo("1110WIZ03002", "机器IP:");
            this.LabPort.Content = CurrentApp.GetLanguageInfo("1110WIZ03003", "端口：");

            this.ButnBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ03001", "配置向导");
        }

        public WizardHelper WizardHelper { get; set; }

        public void LoopLoad()
        {
            RecordNumber++;
            if (WizardHelper.ListRecords != null)
                ListRecordConfig = WizardHelper.ListRecords;
            if (WizardHelper.ListScreens != null)
                ListScreenConfig = WizardHelper.ListScreens;
            InitControlObject();
            ChangeLanguage();
        }
    }
}
