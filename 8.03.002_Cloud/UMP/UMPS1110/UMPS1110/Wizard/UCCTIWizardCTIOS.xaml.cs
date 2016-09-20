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
    /// UCCTIWizardCTIOS.xaml 的交互逻辑
    /// </summary>
    public partial class UCCTIWizardCTIOS : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardCTIConfig PrePage;
        public List<ConfigObject> mListCtiConfigObjs;
        public ObjectItem CtiObjItem;

        private ConfigObject CurrentConfigObj;

        public UCCTIWizardCTIOS()
        {
            InitializeComponent();
            this.Loaded += UCCTIWizardCTIOS_Loaded;
            this.ButBack.Click += ButBack_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.ButNext.Click += ButNext_Click;
        }

        void ButNext_Click(object sender, RoutedEventArgs e)
        {
            SaveCTIOSConfig();
            PrePage.LoopLoad();
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            SaveCTIOSConfig();
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButBack_Click(object sender, RoutedEventArgs e)
        {
            MainPage.PopupPanel.Content = PrePage;
        }

        void UCCTIWizardCTIOS_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
            InitControlObject();
        }

        private void InitControlObject()
        {
            ConfigObject TempConfig = mListCtiConfigObjs.FirstOrDefault(p => p.ObjectID.ToString().Length>3 && p.ObjectID.ToString().Substring(0, 3) == "242");
            if (TempConfig != null)
            {
                CurrentConfigObj = WizardHelper.CreateNewConfigObject(TempConfig, 241);
                this.IPTextBoxA.SetIP(CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 31).Value);
                this.IPTexBoxB.SetIP(CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 33).Value);
                this.TexPortA.Text = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 32).Value;
                this.TexPortB.Text = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 34).Value;
            }
        }

        private void SaveCTIOSConfig()
        {
            if (this.IPTextBoxA.GetIP() != string.Empty && this.IPTextBoxA.GetIP() != "...")
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 31).Value = this.IPTextBoxA.GetIP();
            }
            if (this.IPTexBoxB.GetIP() != string.Empty && this.IPTexBoxB.GetIP() != "...")
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 33).Value = this.IPTexBoxB.GetIP();
            }
            if (this.TexPortA.Text != string.Empty)
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 32).Value = this.TexPortA.Text;
            }
            if (this.TexPortB.Text != string.Empty)
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 34).Value = this.TexPortB.Text;
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

            this.LabCTIB.Content = CurrentApp.GetLanguageInfo("1110WIZ14016", "CTI主机地址B:");
            this.LabCTIIPA.Content = CurrentApp.GetLanguageInfo("1110WIZ14014", "CTI主机地址A:");
            this.LabPortA.Content = CurrentApp.GetLanguageInfo("1110WIZ14015", "CTI端口A：");
            this.LabPortB.Content = CurrentApp.GetLanguageInfo("1110WIZ14017", "CTI端口B：");
            this.ButBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "Back");
            this.ButNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "Next");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "Previous");

            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ12001", "CTI");
        }
    }
}
