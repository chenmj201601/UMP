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
    /// UCCTIWizardAIC.xaml 的交互逻辑
    /// </summary>
    public partial class UCCTIWizardAIC : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardCTIConfig PrePage;
        public List<ConfigObject> mListCtiConfigObjs;
        public ObjectItem CtiObjItem;

        private ConfigObject CurrentConfigObj;

        public UCCTIWizardAIC()
        {
            InitializeComponent();
            this.Loaded += UCCTIWizardAIC_Loaded;
            this.ButBack.Click += ButBack_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.ButNext.Click += ButNext_Click;
        }

        void ButNext_Click(object sender, RoutedEventArgs e)
        {
            SaveAICConfig();
            PrePage.LoopLoad();
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            SaveAICConfig();
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButBack_Click(object sender, RoutedEventArgs e)
        {
            MainPage.PopupPanel.Content = PrePage;
        }

        void UCCTIWizardAIC_Loaded(object sender, RoutedEventArgs e)
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
                this.TexVDU.Text = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 21).Value;
                this.TexADU.Text = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 22).Value;
                this.TexFileName.Text = CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 24).Value;
                if (CurrentConfigObj.ListProperties.FirstOrDefault(p => p.PropertyID == 23).Value == "1")
                {
                    this.RadBADUYes.IsChecked = true;
                }
                else
                {
                    this.RadBADUNo.IsChecked = true;
                }
            }
        }

        private void SaveAICConfig()
        {
            if (this.TexVDU.Text != string.Empty)
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 21).Value = this.TexVDU.Text;
            }
            if (this.TexADU.Text != string.Empty)
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 22).Value = this.TexADU.Text;
            }
            if (this.TexFileName.Text != string.Empty)
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 24).Value = this.TexFileName.Text;
            }
            if (this.RadBADUYes.IsChecked == true)
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 23).Value = "1";
            }
            else
            {
                CurrentConfigObj.ListProperties.Find(p => p.PropertyID == 23).Value = "0";
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

            this.LabADU.Content = CurrentApp.GetLanguageInfo("1110WIZ14019", "ADU Filter:");
            this.LabFileName.Content = CurrentApp.GetLanguageInfo("1110WIZ14021", "FileName:");
            this.LabLoginVDU.Content = CurrentApp.GetLanguageInfo("1110WIZ14018", "VDU Filter:");
            this.LabVDU.Content = CurrentApp.GetLanguageInfo("1110WIZ14020", "Login VDU:");

            this.ButBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "Back");
            this.ButNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "Next");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "Previous");

            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ12001", "CTI");
        }
    }
}
