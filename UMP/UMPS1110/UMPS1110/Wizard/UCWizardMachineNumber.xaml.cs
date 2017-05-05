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
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;

namespace UMPS1110.Wizard
{
    /// <summary>
    /// UCWizardMachineNumber.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardMachineNumber : IWizard
    {
        public ResourceMainView MainPage;
        public List<ConfigObject> ListConfigObjects;
        public int TotalMachineNumber;
        public bool IsBack = false;
        public List<ObjectPropertyInfo> mListResourcePropertyInfos;

        private List<ConfigObject> ListConfigObject;
        private ConfigObject DisplayConfigObj;

        public UCWizardMachineNumber()
        {
            InitializeComponent();

            TotalMachineNumber = 1;

            //mListBasicResourceInfos = new List<BasicResourceInfo>();
            //mListResourceTypeParams = new List<ResourceTypeParam>();
            ListConfigObject = new List<ConfigObject>();
            this.Loaded += UCWizardMachineNumber_Loaded;
            this.ButnNext.Click += ButnNext_Click;

            //MessageBox.Show(App.mListLicenseRev.Count.ToString());
        }

        void UCWizardMachineNumber_Loaded(object sender, RoutedEventArgs e)
        {
            InitControlObject();
            ChangeLanguage();
        }

        private void InitControlObject()
        {
            TexNumMach.Text = "1";
            if (!IsBack)
            {
                this.TexNumMach.Text = TotalMachineNumber.ToString();
            }
            else
            {
                this.TexNumMach.Text = "1";
            }
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInputNumber()) { return; }
            //新建机器
            List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
            int ReadyMachineNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 210).ToList().Count();
            int j = 0;
            for (; j < tempObjItem.Count; j++)
            {
                ConfigGroup tempCG = tempObjItem[j].Data as ConfigGroup;
                if (tempCG.GroupID == 1 && tempCG.ChildType == 210)
                {
                    int MinValue = tempCG.GroupInfo.IntValue01; int MaxValue = tempCG.GroupInfo.IntValue02;
                    if ((ReadyMachineNumber + TotalMachineNumber) > MaxValue || (ReadyMachineNumber + TotalMachineNumber) < MinValue)
                    {
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1110N008", "超过个数，不能增加"));
                        return;
                    }
                    for (int i = 0; i < TotalMachineNumber; i++)
                    {
                        //ListConfigObject.Add(WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 210));
                        ListConfigObject.Add(tempCG.ConfigObject);
                    }
                    break;
                }
            }
            //进入下一个配置界面
            UCWizardMachineConfig ucwizard = new UCWizardMachineConfig();
            ucwizard.CurrentApp = CurrentApp;
            ucwizard.MainPage = MainPage;
            ucwizard.PrePage = this;
            ucwizard.TotalMachineNumber = TotalMachineNumber;
            ucwizard.mListSaveConfigObject = ListConfigObject;
            ucwizard.WizardHelper = this.WizardHelper;
            //ucwizard.ListConfigObjects = ListConfigObjects;
            MainPage.PopupPanel.Title = "Config Wizard";
            MainPage.PopupPanel.Content = ucwizard;
            MainPage.PopupPanel.IsOpen = true;
        }

        private bool CheckInputNumber()
        {
            if (int.TryParse(this.TexNumMach.Text, out TotalMachineNumber))
            {
                if (TotalMachineNumber > 0 && TotalMachineNumber < 1024)
                {
                    return true;
                }
                else
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1110WIZT0002", "请输入一个小于1024的正整数"));
                    return false;
                }
            }
            else
            {
                CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1110WIZT0001", "请输入一个正整数"));
                return false;
            }
        }

        public override void ChangeLanguage()
        {
            this.TexbMachineNum.Content = CurrentApp.GetLanguageInfo("1110WIZ01002", "机器个数");
            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ01001", "配置向导");
        }

        public WizardHelper WizardHelper { get; set; }

    }
}
