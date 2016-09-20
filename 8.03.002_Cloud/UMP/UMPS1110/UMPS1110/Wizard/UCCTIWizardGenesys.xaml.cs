using System;
using System.Collections.Generic;
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

namespace UMPS1110.Wizard
{
    /// <summary>
    /// UCCTIWizardGenesys.xaml 的交互逻辑
    /// </summary>
    public partial class UCCTIWizardGenesys : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardCTIConfig PrePage;
        public List<ConfigObject> mListCtiConfigObjs;
        public ObjectItem CtiObjItem;

        private ConfigObject CurrentConfigObj;

        public UCCTIWizardGenesys()
        {
            InitializeComponent();

            this.Loaded += UCCTIWizardGenesys_Loaded;
            this.ButBack.Click += ButBack_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.ButNext.Click += ButNext_Click;
        }

        void ButNext_Click(object sender, RoutedEventArgs e)
        {
            SaveGenesysConfig();
            PrePage.LoopLoad();
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            SaveGenesysConfig();
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButBack_Click(object sender, RoutedEventArgs e)
        {
            MainPage.PopupPanel.Content = PrePage;
        }

        void UCCTIWizardGenesys_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
            InitControlObject();
        }

        private void InitControlObject()
        {
            ConfigObject TempConfig = mListCtiConfigObjs.FirstOrDefault(p => p.ObjectID.ToString().Length > 3 && p.ObjectID.ToString().Substring(0, 3) == "242");
            if (TempConfig != null)
            {
                CurrentConfigObj = WizardHelper.CreateNewConfigObject(TempConfig, 241);
                this.IPTexCTI.SetIP(CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 11).Value);
                this.TexPort.Text = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 12).Value;
            }
        }

        private void SaveGenesysConfig()
        {
            if (this.IPTexCTI.GetIP() != string.Empty && this.IPTexCTI.GetIP() != "...")
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 11).Value = this.IPTexCTI.GetIP();
            }

            if (this.TexPort.Text != string.Empty)
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 12).Value = this.TexPort.Text;
            }
            if (this.TexLogName.Text != string.Empty)
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 911).Value = this.TexLogName.Text;
            }
            if (this.TexLogPassw.Password != string.Empty)
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 912).Value = this.TexLogPassw.Password;
            }
            CurrentConfigObj.GetBasicPropertyValues();
            mListCtiConfigObjs.Add(CurrentConfigObj);
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
        }

        public WizardHelper WizardHelper { get; set; }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            this.ButBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
            this.ButNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ12001", "CTI");

            this.LabCTIIP.Content = CurrentApp.GetLanguageInfo("1110WIZ14001", "CTI主机地址：");
            this.LabCTIPort.Content = CurrentApp.GetLanguageInfo("1110WIZ14002", "CTI端口：");
            this.LabLogName.Content = CurrentApp.GetLanguageInfo("1110WIZ14003", "登录名：");
            this.LabLogPassw.Content = CurrentApp.GetLanguageInfo("1110WIZ14004", "登录密码：");
        }
    }
}
