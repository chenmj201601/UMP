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
    /// UCCTIWizardCVCTother.xaml 的交互逻辑
    /// </summary>
    public partial class UCCTIWizardCVCTother : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardCTIConfig PrePage;
        public List<ConfigObject> mListCtiConfigObjs;
        public ObjectItem CtiObjItem;
        public bool IsServiceName;

        private ObservableCollection<PropertyValueEnumItem> mListServiceItems;

        public UCCTIWizardCVCTother()
        {
            InitializeComponent();
            mListServiceItems = new ObservableCollection<PropertyValueEnumItem>();
            this.Loaded += UCCTIWizardCVCTother_Loaded;
            this.ButBack.Click += ButBack_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.ButNext.Click += ButNext_Click;
        }

        void ButNext_Click(object sender, RoutedEventArgs e)
        {
            SaveCTIConfig();
            //下一页
            PrePage.LoopLoad();
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            SaveCTIConfig();
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButBack_Click(object sender, RoutedEventArgs e)
        {
            MainPage.PopupPanel.Content = PrePage;
        }

        void UCCTIWizardCVCTother_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
            InitControlObject();
        }

        private void InitControlObject()
        {
            //加载comb下拉项(以后吧，这是410格式的啦)
            //控制是否需要加载服务名称这一项
            if (IsServiceName)
            {
                this.LabServiceName.Visibility = Visibility.Visible;
                this.CombServiceName.Visibility = Visibility.Visible;
            }
            else
            {
                this.LabServiceName.Visibility = Visibility.Collapsed;
                this.CombServiceName.Visibility = Visibility.Collapsed;
            }
        }

        public void SaveCTIConfig()
        {
            //创建一个configobject241
            ConfigObject TempConfig = mListCtiConfigObjs.FirstOrDefault(p => p.ObjectID.ToString().Length > 3 && p.ObjectID.ToString().Substring(0, 3) == "242");
            if (TempConfig != null)
            {
                ConfigObject CurrenPageCTIConfigObj = WizardHelper.CreateNewConfigObject(TempConfig,241);
                if (PasswLoginPassword.Password != null && PasswLoginPassword.Password != string.Empty)
                {
                    ResourceProperty property = CurrenPageCTIConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 912);
                    if (property != null)
                    {
                        property.Value = this.PasswLoginPassword.Password;
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
                if (IsServiceName)
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
            }
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

            this.LabLoginName.Content = CurrentApp.GetLanguageInfo("1110WIZ14003", "Login Name:");
            this.LabLoginPassword.Content = CurrentApp.GetLanguageInfo("1110WIZ14004", "Login Password:");
            this.LabServiceName.Content = CurrentApp.GetLanguageInfo("1110WIZ14006", "Service Name:");

            this.ButBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "Back");
            this.ButNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "Next");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "Previous");

            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ12001", "配置向导");
        }
    }
}
