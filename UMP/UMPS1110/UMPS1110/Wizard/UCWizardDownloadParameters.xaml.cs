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
    /// UCWizardDownloadParameters.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardDownloadParameters : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardDownloadConfig PrePage;
        public int DownLoadNumber;

        private ObservableCollection<PropertyValueEnumItem> mListDownWayItems;
        private ObservableCollection<PropertyValueEnumItem> mListVoiceServiceItems;
        private ConfigObject CurrentConfigObject;
        private ObjectItem DownloadObjectItem;
        private List<ConfigObject> mListDownConfigObjs;

        public UCWizardDownloadParameters()
        {
            InitializeComponent();
            CurrentConfigObject = new ConfigObject();
            DownloadObjectItem = new ObjectItem();
            mListDownConfigObjs = new List<ConfigObject>();
            //DownLoadNumber = 1;
            mListDownWayItems = new ObservableCollection<PropertyValueEnumItem>();
            mListVoiceServiceItems = new ObservableCollection<PropertyValueEnumItem>();
            this.CombVoice.ItemsSource = mListVoiceServiceItems;
            this.CombDownWay.ItemsSource = mListDownWayItems;
            this.Loaded += UCWizardDownloadParameters_Loaded;
            this.ButnNext.Click += ButnNext_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            //上一页
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            //下一页，调回询问界面
            SaveDownloadConfig();
            PrePage.NextPage = this;
            MainPage.PopupPanel.Content = PrePage;
            //MainPage.PopupPanel.IsOpen = true;
            //PrePage.LoopLoad();
        }

        void UCWizardDownloadParameters_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
            InitControlObject();
        }

        private void InitControlObject()
        {
            //加载下载方式下拉项
            List<BasicInfoData> TempBasicInfo = WizardHelper.ListAllBasicInfos.Where(p => p.InfoID == 111000115).ToList();
            mListDownWayItems.Clear();
            foreach (BasicInfoData info in TempBasicInfo)
            {
                PropertyValueEnumItem propertyValueItem = new PropertyValueEnumItem();
                propertyValueItem.Value = info.Value;
                propertyValueItem.Display =
                    CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                        info.Icon);
                propertyValueItem.Info = info;
                mListDownWayItems.Add(propertyValueItem);
            }
            //加载录音服务器下拉项
            List<ConfigObject> TempConfigObj = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 221).ToList();
            mListVoiceServiceItems.Clear();
            foreach (ConfigObject Obj in TempConfigObj)
            {
                PropertyValueEnumItem propertyValue = new PropertyValueEnumItem();
                propertyValue.Value = Obj.ObjectID.ToString();
                propertyValue.Display = Obj.ToString();
                propertyValue.Info = Obj;
                mListVoiceServiceItems.Add(propertyValue);
            }
            
            //生成291的ConfigObject对象
            if (DownLoadNumber > mListDownConfigObjs.Count)
            {
                //加载默认值
                this.IPTextBox.SetIP("127.0.0.1");
                this.TexPort.Text = "";
                this.TexCatalog.Text = "";
                this.TexLoginName.Text = "";
                this.PasswLogin.Password = "";
           
                List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                int j = 0;
                for (; j < tempObjItem.Count; j++)
                {
                    ConfigGroup tempCG = tempObjItem[j].Data as ConfigGroup;
                    if (tempCG.GroupID == 9 && tempCG.ChildType == 291)
                    {
                        DownloadObjectItem = tempObjItem[j];
                        CurrentConfigObject = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 291);
                        mListDownConfigObjs.Add(CurrentConfigObject);
                        break;
                    }
                }
            }
            else
            {
                CurrentConfigObject = mListDownConfigObjs[DownLoadNumber - 1];
            }
            for (int i = 0; i < CurrentConfigObject.ListProperties.Count; i++)
            {
                int propertyID = CurrentConfigObject.ListProperties[i].PropertyID;
                string propertyValue = CurrentConfigObject.ListProperties[i].Value;
                if (propertyValue == null || propertyValue == string.Empty) { continue; }
                switch (propertyID)
                {
                    case 11:
                        for (int k = 0; k < mListDownWayItems.Count; k++)
                        {
                            if (mListDownWayItems[k].Value == propertyValue)
                            {
                                this.CombDownWay.SelectedItem = mListDownWayItems[k]; break;
                            }
                        }
                        break;
                    case 12:
                        for (int k = 0; k < mListVoiceServiceItems.Count; k++)
                        {
                            if (mListVoiceServiceItems[k].Value == propertyValue)
                            {
                                this.CombVoice.SelectedItem = mListVoiceServiceItems[k]; break;
                            }
                        }
                        break;
                    case 13:
                        this.IPTextBox.SetIP(propertyValue);
                        break;
                    case 14:
                        this.TexPort.Text = propertyValue;
                        break;
                    case 15:
                        this.TexCatalog.Text = propertyValue;
                        break;
                    case 911:
                        this.TexLoginName.Text = propertyValue;
                        break;
                    case 912:
                        this.PasswLogin.Password = propertyValue;
                        break;
                }
            }
        }

        private void SaveDownloadConfig()
        {
            for (int i = 0; i < CurrentConfigObject.ListProperties.Count; i++)
            {
                int propertyID = CurrentConfigObject.ListProperties[i].PropertyID;
                switch (propertyID)
                {
                    case 11:
                        if (this.CombDownWay.SelectedIndex != -1)
                        {
                            CurrentConfigObject.ListProperties[i].Value = (CombDownWay.SelectedItem as PropertyValueEnumItem).Value;
                        }
                        break;
                    case 12:
                        if (this.CombVoice.SelectedIndex != -1)
                        {
                            CurrentConfigObject.ListProperties[i].Value = (CombVoice.SelectedItem as PropertyValueEnumItem).Value;
                        }
                        break;
                    case 13:
                        CurrentConfigObject.ListProperties[i].Value = this.IPTextBox.GetIP();
                        break;
                    case 14:
                        CurrentConfigObject.ListProperties[i].Value = this.TexPort.Text;
                        break;
                    case 15:
                        CurrentConfigObject.ListProperties[i].Value = this.TexCatalog.Text;
                        break;
                    case 911:
                        CurrentConfigObject.ListProperties[i].Value = this.TexLoginName.Text;
                        break;
                    case 912:
                        CurrentConfigObject.ListProperties[i].Value = this.PasswLogin.Password;
                        break;
                }
            }
            CurrentConfigObject.GetBasicPropertyValues();
            MainPage.RefreshConfigObjectItem(MainPage.CreateNewObjectItem(DownloadObjectItem, CurrentConfigObject, false));
        }

        public WizardHelper WizardHelper { get; set; }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ15001", "下载参数配置");
            this.LabAddress.Content = CurrentApp.GetLanguageInfo("1110WIZ16003", "地址：");
            this.LabCatalog.Content = CurrentApp.GetLanguageInfo("1110WIZ16005", "主目录：");
            this.LabDownloadWay.Content = CurrentApp.GetLanguageInfo("1110WIZ16001", "下载方式：");
            this.LabLoginName.Content = CurrentApp.GetLanguageInfo("1110WIZ16006", "登录名：");
            this.LabLoginPassword.Content = CurrentApp.GetLanguageInfo("1110WIZ16007", "登录密码：");
            this.LabPort.Content = CurrentApp.GetLanguageInfo("1110WIZ16004", "端口：");
            this.LabVoiceServer.Content = CurrentApp.GetLanguageInfo("1110WIZ16002", "录音服务器：");

            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
        }

        public void LoopLoad()
        {

            InitControlObject();
            ChangeLanguage();
        }
    }
}
