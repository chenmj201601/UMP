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
using VoiceCyber.UMP.Controls;

namespace UMPS1110.Wizard
{
    /// <summary>
    /// UCCTIWizardAES.xaml 的交互逻辑
    /// </summary>
    public partial class UCCTIWizardAES : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardCTIConfig PrePage;
        public List<ConfigObject> mListCtiConfigObjs;
        public ObjectItem CtiObjItem;

        private ObservableCollection<PropertyValueEnumItem> mListEditionItems;
        private ConfigObject CurrenPageCTIConfigObj;

        public UCCTIWizardAES()
        {
            InitializeComponent();

            mListEditionItems = new ObservableCollection<PropertyValueEnumItem>();
            CurrenPageCTIConfigObj = new ConfigObject();

            this.Loaded += UCCTIWizardAES_Loaded;
            this.ButBack.Click += ButBack_Click;
            this.ButNext.Click += ButNext_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.CombEdition.ItemsSource = mListEditionItems;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            SaveAESConfig();
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButNext_Click(object sender, RoutedEventArgs e)
        {
            SaveAESConfig();
            //下一页
            PrePage.LoopLoad();
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButBack_Click(object sender, RoutedEventArgs e)
        {
            MainPage.PopupPanel.Content = PrePage;
        }

        void UCCTIWizardAES_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
            InitControlObject();
        }

        private void InitControlObject()
        {
            InitEditionItems();
        }

        private void InitEditionItems()
        {
            List<BasicInfoData> TempBasicInfo = WizardHelper.ListAllBasicInfos.Where(p => p.InfoID == 111000303).ToList();
            mListEditionItems.Clear();
            foreach (BasicInfoData info in TempBasicInfo)
            {
                PropertyValueEnumItem propertyValueItem = new PropertyValueEnumItem();
                propertyValueItem.Value = info.Value;
                propertyValueItem.Display =
                    CurrentApp.GetLanguageInfo(string.Format("BID{0}{1}", info.InfoID, info.SortID.ToString("000")),
                        info.Icon);
                propertyValueItem.Info = info;
                mListEditionItems.Add(propertyValueItem);
            }
            //创建一个configobject241
            ConfigObject TempConfig = mListCtiConfigObjs.FirstOrDefault(p => p.ObjectID.ToString().Length > 3 && p.ObjectID.ToString().Substring(0, 3) == "242");
            if (TempConfig != null)
            {
                CurrenPageCTIConfigObj = WizardHelper.CreateNewConfigObject(TempConfig, 241);
                this.IPTextBox.SetIP(CurrenPageCTIConfigObj.ListProperties.Find(p => p.PropertyID == 11).Value);
                this.IPTexPBX.SetIP(CurrenPageCTIConfigObj.ListProperties.Find(p => p.PropertyID == 13).Value);
                this.TexPort.Text = CurrenPageCTIConfigObj.ListProperties.Find(p => p.PropertyID == 12).Value;
                this.TexAgreenment.Text = CurrenPageCTIConfigObj.ListProperties.Find(p => p.PropertyID == 19).Value;
                if (CurrenPageCTIConfigObj.ListProperties.Find(p => p.PropertyID == 16).Value == "1")
                {
                    this.RadBDMCCYes.IsChecked = true;
                }
                else
                {
                    this.RadBDMCCNo.IsChecked = true;
                }
                if (CurrenPageCTIConfigObj.ListProperties.Find(p => p.PropertyID == 17).Value == "1")
                {
                    this.RadBSSLYes.IsChecked = true;
                }
                else
                {
                    this.RadBSSLNo.IsChecked = true;
                }
            }
        }

        private void SaveAESConfig()
        {
            //创建一个configobject241

            if (PasswordLogin.Password != null && PasswordLogin.Password != string.Empty)
            {
                ResourceProperty property = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 912);
                if (property != null)
                {
                    property.Value = this.PasswordLogin.Password;
                }
            }
            if (this.TexLoginName.Text != null && this.TexLoginName.Text != string.Empty)
            {
                ResourceProperty PropertyName = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 911);
                if (PropertyName != null)
                {
                    PropertyName.Value = this.TexLoginName.Text;
                }
            }
            if (this.IPTextBox.GetIP() != string.Empty && this.IPTextBox.GetIP() != "...")
            {
                ResourceProperty PropertyIP = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 11);
                if (PropertyIP != null)
                {
                    PropertyIP.Value = this.IPTextBox.GetIP();
                }
            }
            if (this.IPTextBox.GetIP() != string.Empty && this.IPTextBox.GetIP() != "...")
            {
                ResourceProperty PropertyIP = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 13);
                if (PropertyIP != null)
                {
                    PropertyIP.Value = this.IPTextBox.GetIP();
                }
            }
            if (this.TexPort.Text != string.Empty && this.TexPort.Text != "...")
            {
                ResourceProperty PropertyPort = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                if (PropertyPort != null)
                {
                    PropertyPort.Value = this.TexPort.Text;
                }
            }
            if (this.TexConnectName.Text != string.Empty && this.TexConnectName.Text != "...")
            {
                ResourceProperty PropertyConnect = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 15);
                if (PropertyConnect != null)
                {
                    PropertyConnect.Value = this.TexConnectName.Text;
                }
            }
            if (this.TexAgreenment.Text != string.Empty && this.TexAgreenment.Text != "...")
            {
                ResourceProperty PropertyConnect = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 19);
                if (PropertyConnect != null)
                {
                    PropertyConnect.Value = this.TexAgreenment.Text;
                }
            }
            if (this.CombEdition.SelectedItem != null && this.CombEdition.SelectedIndex != -1)
            {
                ResourceProperty PropertyConnect = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 18);
                var selectitem = this.CombEdition.SelectedItem as PropertyValueEnumItem;
                if (PropertyConnect != null)
                {
                    PropertyConnect.Value = selectitem.Value;
                }
            }
            //if (this.RadBDMCCNo.IsChecked == true || this.RadBDMCCYes.IsChecked == true)
            {
                if (this.RadBDMCCYes.IsChecked == true)
                {
                    ResourceProperty PropertyConnect = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 16);
                    if (PropertyConnect != null)
                    {
                        PropertyConnect.Value = "1";
                    }
                }
                else
                {
                    ResourceProperty PropertyConnect = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 16);
                    if (PropertyConnect != null)
                    {
                        PropertyConnect.Value = "1";
                    }
                }
            }
            //if (this.RadBDMCCNo.IsChecked == true || this.RadBDMCCYes.IsChecked == true)
            {
                if (this.RadBSSLYes.IsChecked == true)
                {
                    ResourceProperty PropertyConnect = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 17);
                    if (PropertyConnect != null)
                    {
                        PropertyConnect.Value = "1";
                    }
                }
                else
                {
                    ResourceProperty PropertyConnect = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 17);
                    if (PropertyConnect != null)
                    {
                        PropertyConnect.Value = "1";
                    }
                }
            }
            //if (IsServiceName)
            {
                if (this.CombServiceName.Text != null && this.CombServiceName.Text != string.Empty)
                {
                    ResourceProperty PropertyServiceName = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 14);
                    if (PropertyServiceName != null)
                    {
                        PropertyServiceName.Value = this.TexLoginName.Text;
                    }
                }
            }
            CurrenPageCTIConfigObj.GetBasicPropertyValues();
            //TempConfig.ListChildObjects.Add(CurrenPageCTIConfigObj);
            mListCtiConfigObjs.Add(CurrenPageCTIConfigObj);
            ObjectItem ObjItem243 = new ObjectItem(); ObjectItem ObjItem242 = new ObjectItem(); ObjectItem ObjItem241 = new ObjectItem();
            if (mListCtiConfigObjs.Count == 3)
            {
                for (int i = 0; i < 3; i++)
                {
                    WizardHelper.ListAllConfigObjects.Add(mListCtiConfigObjs[i]);
                    switch (i)
                    {
                        case 0:
                            ObjItem243 = MainPage.CreateNewObjectItem(CtiObjItem, mListCtiConfigObjs[i], false);
                            MainPage.RefreshConfigObjectItem(ObjItem243);
                            break;
                        case 1:
                            ObjItem242 = MainPage.CreateNewObjectItem(ObjItem243, mListCtiConfigObjs[i], false);
                            MainPage.RefreshConfigObjectItem(ObjItem242);
                            break;
                        case 2:
                            ObjItem241 = MainPage.CreateNewObjectItem(ObjItem242, mListCtiConfigObjs[i], false);
                            MainPage.RefreshConfigObjectItem(ObjItem241);
                            break;
                    }
                }
            }
            //MainPage.RefreshConfigObjectItem(CtiObjItem);
        }

        public WizardHelper WizardHelper { get; set; }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            this.LabAgreenment.Content = CurrentApp.GetLanguageInfo("1110WIZ14011", "协议：");
            this.LabConnectName.Content = CurrentApp.GetLanguageInfo("1110WIZ14007", "连接名称：");
            this.LabCTIIP.Content = CurrentApp.GetLanguageInfo("1110WIZ14001", "CTI主机地址：");
            this.LabCTIPort.Content = CurrentApp.GetLanguageInfo("1110WIZ14002", "CTI端口：");
            this.LabDMCC.Content = CurrentApp.GetLanguageInfo("1110WIZ14008", "使用DMCC：");
            this.LabEdition.Content = CurrentApp.GetLanguageInfo("1110WIZ14010", "版本：");
            this.LabLoginName.Content = CurrentApp.GetLanguageInfo("1110WIZ14003", "登录名称：");
            this.LabPassword.Content = CurrentApp.GetLanguageInfo("1110WIZ14004", "登录密码：");
            this.LabPBXIP.Content = CurrentApp.GetLanguageInfo("1110WIZ14005", "PBX主机地址：");
            this.LabServiceName.Content = CurrentApp.GetLanguageInfo("1110WIZ14006", "服务名称：");
            this.LabSSL.Content = CurrentApp.GetLanguageInfo("1110WIZ14009", "使用SSL：");

            this.ButBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            this.ButNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");

            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ12001", "CTI");
        }
      
    }
}
