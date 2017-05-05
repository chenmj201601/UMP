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
    /// UCWizardScreenConfig.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardScreenConfig : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardRecordChannel ChanPrepage;
        public UCWizardRecordChannelAdd ChanAddPrepage;
        public UCWizardMachineConfig PrePage;

        private int ScreenNumber;
        private ObservableCollection<PropertyValueEnumItem> ListVoiceIDItems;
        private ObservableCollection<PropertyValueEnumItem> ListDBBridgeItems;
        private ObservableCollection<PropertyValueEnumItem> ListSFTPItems;
        private ObservableCollection<PropertyValueEnumItem> ListSFTPUserItems;
        private ConfigObject CurrentConfigObje;

        public UCWizardScreenConfig()
        {
            InitializeComponent();

            this.Loaded += UCWizardScreenConfig_Loaded;
            ScreenNumber = 1;
            ListVoiceIDItems = new ObservableCollection<PropertyValueEnumItem>();
            ListDBBridgeItems = new ObservableCollection<PropertyValueEnumItem>();
            ListSFTPItems = new ObservableCollection<PropertyValueEnumItem>();
            ListSFTPUserItems = new ObservableCollection<PropertyValueEnumItem>();
            this.CombDBBridge.ItemsSource = ListDBBridgeItems;
            this.CombSFTP.ItemsSource = ListSFTPItems;
            this.CombVoiceID.ItemsSource = ListVoiceIDItems;
            this.CombSFTPName.ItemsSource = ListSFTPUserItems;
            this.ButnBack.Click += ButnBack_Click;
            this.ButnNext.Click += ButnNext_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            SaveScreenConfig();
            if (ChanPrepage != null)
                MainPage.PopupPanel.Content = ChanPrepage;
            else if (ChanAddPrepage != null)
                MainPage.PopupPanel.Content = ChanAddPrepage;
            else
            {
                PrePage.ReturnPage();
                MainPage.PopupPanel.Content = PrePage;
            }
        }

        void ButnBack_Click(object sender, RoutedEventArgs e)
        {
            if (ChanPrepage != null)
                MainPage.PopupPanel.Content = ChanPrepage;
            else if (ChanAddPrepage != null)
                MainPage.PopupPanel.Content = ChanAddPrepage;
            else
            {
                PrePage.ReturnPage();
                MainPage.PopupPanel.Content = PrePage;
            }
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            //下一页
            SaveScreenConfig();
            //打开界面10
            WizardHelper.UCScreenConfig = this;
            UCWizardScreenChannel ucwizard = new UCWizardScreenChannel();
            ucwizard.MainPage = MainPage;
            ucwizard.PrePage = this;
            ucwizard.CurrentApp = CurrentApp;
            //ucwizard.ParentItem = this.ParentObjectItem;
            ucwizard.WizardHelper = this.WizardHelper;
            ucwizard.ScreenSeviceConfigObj = CurrentConfigObje;
            //ucwizard.ListConfigObjects = ListConfigObjects;
            MainPage.PopupPanel.Title = "Config Wizard";
            MainPage.PopupPanel.Content = ucwizard;
            MainPage.PopupPanel.IsOpen = true;
        }

        void UCWizardScreenConfig_Loaded(object sender, RoutedEventArgs e)
        {
            InitControlObject();
            ChangeLanguage();
        }

        private void SaveScreenConfig()
        {
            //保存参数
            if (this.CombVoiceID.SelectedIndex != -1)
            {
                var property = CurrentConfigObje.ListProperties.FirstOrDefault(p => p.PropertyID == 11);
                var selectprop = this.CombVoiceID.SelectedItem;
                if (property != null && selectprop != null)
                {
                    property.Value = (selectprop as PropertyValueEnumItem).Value;
                }
            }
            if (this.CombDBBridge.SelectedIndex != -1)
            {
                var property = CurrentConfigObje.ListProperties.FirstOrDefault(p => p.PropertyID == 15);
                var selectprop = this.CombDBBridge.SelectedItem;
                if (property != null && selectprop != null)
                {
                    property.Value = (selectprop as PropertyValueEnumItem).Value;
                }
            }
            if (this.CombSFTP.SelectedIndex != -1)
            {
                var property = CurrentConfigObje.ListProperties.FirstOrDefault(p => p.PropertyID == 16);
                var selectprop = this.CombSFTP.SelectedItem;
                if (property != null && selectprop != null)
                {
                    property.Value = (selectprop as PropertyValueEnumItem).Value;
                }
            }
            if (this.CombSFTPName.SelectedIndex != -1)
            {
                var property = CurrentConfigObje.ListProperties.FirstOrDefault(p => p.PropertyID == 17);
                var selectprop = this.CombSFTPName.SelectedItem;
                if (property != null && selectprop != null)
                {
                    property.Value = (selectprop as PropertyValueEnumItem).Value;
                }
            }
            CurrentConfigObje.GetBasicPropertyValues();
            //MainPage.RefreshConfigObjectItem();
        }

        private void InitControlObject()
        {
            //设置当前的ConfigObject
            CurrentConfigObje = WizardHelper.ListScreens[ScreenNumber - 1];
            this.TexPort.Text = CurrentConfigObje.ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value;
            this.IPTextBox.SetIP(CurrentConfigObje.ListProperties.FirstOrDefault(p => p.PropertyID == 7).Value);
            this.IPTextBox.IsEnabled = false;
            //获取DBBridge添加项
            List<ConfigObject> TempListItems = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == S1110Consts.RESOURCE_DBBRIDGE).ToList();
            ListDBBridgeItems.Clear();
            foreach (ConfigObject co in TempListItems)
            {
                PropertyValueEnumItem item = new PropertyValueEnumItem();
                item.Value = co.ID.ToString();
                item.Display = co.Name;
                item.Info = co;
                ListDBBridgeItems.Add(item);
            }
            //获取ＳＦＴＰ添加项
            TempListItems = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == S1110Consts.RESOURCE_SFTP).ToList();
            ListSFTPItems.Clear();
            foreach (ConfigObject co in TempListItems)
            {
                PropertyValueEnumItem item = new PropertyValueEnumItem();
                item.Value = co.ID.ToString();
                item.Display = co.Name;
                item.Info = co;
                ListSFTPItems.Add(item);
            }
            //录音服务器
            TempListItems = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == S1110Consts.RESOURCE_VOICESERVER).ToList();
            ListVoiceIDItems.Clear();
            foreach (ConfigObject co in TempListItems)
            {
                PropertyValueEnumItem item = new PropertyValueEnumItem();
                item.Value = co.ID.ToString();
                item.Display = co.Name;
                item.Info = co;
                ListVoiceIDItems.Add(item);
            }

            //sftp登录名
            ListSFTPUserItems.Clear();
            foreach (BasicUserInfo BasicInfo in WizardHelper.mListSftpUsers)
            {
                PropertyValueEnumItem item = new PropertyValueEnumItem();
                item.Display = BasicInfo.Account;
                item.Description = BasicInfo.FullName;
                item.Value = string.Format("{0}|{1}", BasicInfo.UserID, CurrentApp.Session.RentInfo.Token);
                item.Info = BasicInfo;
                var temp = ListSFTPUserItems.FirstOrDefault(e => e.Value == item.Value);
                if (temp == null)
                {
                    ListSFTPUserItems.Add(item);
                }
            }
            //加载选中项
            var propertyItem = CurrentConfigObje.ListProperties.FirstOrDefault(p => p.PropertyID == 11);
            string voiceid = string.Empty;
            if (propertyItem != null)
            {
                voiceid = propertyItem.Value;
            }
            for (int index = 0; index < ListVoiceIDItems.Count; index++)
            {
                if (voiceid == ListVoiceIDItems[index].Value)
                {
                    this.CombVoiceID.SelectedIndex = index;
                }
            }

            propertyItem = CurrentConfigObje.ListProperties.FirstOrDefault(p => p.PropertyID == 15);
            string DB = string.Empty;
            if (propertyItem != null)
            {
                DB = propertyItem.Value;
            }
            for (int index = 0; index < ListDBBridgeItems.Count; index++)
            {
                if (DB == ListDBBridgeItems[index].Value)
                {
                    this.CombDBBridge.SelectedIndex = index;
                }
            }

            propertyItem = CurrentConfigObje.ListProperties.FirstOrDefault(p => p.PropertyID == 16);
            string sftp = string.Empty;
            if (propertyItem != null)
            {
                sftp = propertyItem.Value;
            }
            for (int index = 0; index < ListSFTPItems.Count; index++)
            {
                if (DB == ListSFTPItems[index].Value)
                {
                    this.CombSFTP.SelectedIndex = index;
                }
            }

            propertyItem = CurrentConfigObje.ListProperties.FirstOrDefault(p => p.PropertyID == 17);
            string sftpUser = string.Empty;
            if (propertyItem != null)
            {
                sftpUser = propertyItem.Value;
            }
            for (int index = 0; index < ListSFTPUserItems.Count; index++)
            {
                if (sftpUser == ListSFTPUserItems[index].Value)
                {
                    this.CombSFTPName.SelectedIndex = index;
                }
            }
        }

        public override void ChangeLanguage()
        {
            this.LabDBBridge.Content = CurrentApp.GetLanguageInfo("1110WIZ09005", "DBBridge:");
            this.LabIP.Content = CurrentApp.GetLanguageInfo("1110WIZ09002", "IP：");
            this.LabPort.Content = CurrentApp.GetLanguageInfo("1110WIZ09003", "端口：");
            this.LabSFTP.Content = CurrentApp.GetLanguageInfo("1110WIZ09006", "SFTP索引：");
            this.LabSFTPName.Content = CurrentApp.GetLanguageInfo("1110WIZ09007", "SFTP登录名：");
            this.LabVoiceID.Content = CurrentApp.GetLanguageInfo("1110WIZ09004", "录屏连接dVoiceID：");

            this.ButnBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ09001", "录屏参数配置");
        }

        public WizardHelper WizardHelper { get; set; }

        public void LoopLoad()
        {
            ScreenNumber++;
            
            InitControlObject();
            ChangeLanguage();
        }
    }
}
