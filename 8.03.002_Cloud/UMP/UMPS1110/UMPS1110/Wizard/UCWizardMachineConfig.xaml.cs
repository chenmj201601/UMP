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
    /// UCWizardMachineConfig.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardMachineConfig : IWizard
    {
        public ResourceMainView MainPage;
        public UCWizardMachineNumber PrePage;
        public int TotalMachineNumber;


        public List<ConfigObject> mListSaveConfigObject;//新加的物理机器

        private List<ConfigObject> mListRecordConfigObject;
        private List<ConfigObject> mListScreenConfigObject;
        private List<ConfigObject> ListConfigObjects;

        private int MachineNumber = 0;
        private ConfigObject mConfigObject;

        public UCWizardMachineConfig()
        {
            InitializeComponent();

            MachineNumber = 1;
            mConfigObject = ConfigObject.CreateObject(210);
            mListRecordConfigObject = new List<ConfigObject>();
            mListScreenConfigObject = new List<ConfigObject>();
            this.Loaded += UCWizardMachineConfig_Loaded;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.ButnNext.Click += ButnNext_Click;
            this.ButnBack.Click += ButnBack_Click;

        }

        void UCWizardMachineConfig_Loaded(object sender, RoutedEventArgs e)
        {
            ListConfigObjects = WizardHelper.ListAllConfigObjects;
            InitControlObject();
            ChangeLanguage();
        }

        private void InitControlObject()
        {
            this.IPTextBox.SetIP("127.0.0.1");
            //加载
            OperationInfo TempOPTInfo = WizardHelper.WizListOperations.FirstOrDefault(p => p.ID == 1110221);
            if (TempOPTInfo == null)
            {
                this.CheckRecord.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckRecord.Visibility = Visibility.Visible;
            }
            TempOPTInfo = WizardHelper.WizListOperations.FirstOrDefault(p => p.ID == 1110231);
            if (TempOPTInfo == null)
            {
                this.CheckScreen.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckScreen.Visibility = Visibility.Visible;
            }
            TempOPTInfo = WizardHelper.WizListOperations.FirstOrDefault(p => p.ID == 1110217);
            if (TempOPTInfo == null)
            {
                this.CheckAlarmMonitor.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckAlarmMonitor.Visibility = Visibility.Visible;
            }
            TempOPTInfo = WizardHelper.WizListOperations.FirstOrDefault(p => p.ID == 1110218);
            if (TempOPTInfo == null)
            {
                this.CheckAlarm.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckAlarm.Visibility = Visibility.Visible;
            }
            TempOPTInfo = WizardHelper.WizListOperations.FirstOrDefault(p => p.ID == 1110219);
            if (TempOPTInfo == null)
            {
                this.CheckSFTP.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckSFTP.Visibility = Visibility.Visible;
            }
            TempOPTInfo = WizardHelper.WizListOperations.FirstOrDefault(p => p.ID == 1110211);
            if (TempOPTInfo == null)
            {
                this.CheckLicense.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckLicense.Visibility = Visibility.Visible;
            }
            TempOPTInfo = WizardHelper.WizListOperations.FirstOrDefault(p => p.ID == 1110213);
            if (TempOPTInfo == null)
            {
                this.CheckCTIHub.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckCTIHub.Visibility = Visibility.Visible;
            }
            TempOPTInfo = WizardHelper.WizListOperations.FirstOrDefault(p => p.ID == 1110212);
            if (TempOPTInfo == null)
            {
                this.CheckDEC.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckDEC.Visibility = Visibility.Visible;
            }
            TempOPTInfo = WizardHelper.WizListOperations.FirstOrDefault(p => p.ID == 1110215);
            if (TempOPTInfo == null)
            {
                this.CheckDBBridge.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckDBBridge.Visibility = Visibility.Visible;
            }
            TempOPTInfo = WizardHelper.WizListOperations.FirstOrDefault(p => p.ID == 1110251);
            if (TempOPTInfo == null)
            {
                this.CheckDocumentProcess.Visibility = Visibility.Collapsed;
            }
            else
            {
                this.CheckDocumentProcess.Visibility = Visibility.Visible;
            }
            //读许可，加载界面
            if (MachineNumber == 0) { return; }
            ConfigObject TempConfigObject = mListSaveConfigObject[MachineNumber - 1];
            ResourceProperty IPProp = TempConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == 7);
            string MachineIP = string.Empty;
            if (IPProp != null)
                MachineIP = IPProp.Value;
            if (MachineIP == string.Empty) { return; }//这里加载一个新的界面  空的
            string MachineId = TempConfigObject.ObjectID.ToString();
            this.IPTextBox.SetIP(MachineIP);
            this.TexLogPath.Text = TempConfigObject.ListProperties.FirstOrDefault(p => p.PropertyID == 11).Value;
            //get other message
            List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 214).ToList();
            for (int i = 0; i < TempListCO.Count; i++)
            {
                ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                if (machineid != null && machineid.Value == MachineId)
                {
                    this.EditorCatalog.Text = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 14).Value; break;
                }
            }
            #region Service
            TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 231).ToList();
            for (int i = 0; i < TempListCO.Count; i++)
            {
                ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                if (machineid != null && machineid.Value == MachineId)
                {
                    this.CheckScreen.IsChecked = true;
                    this.TexScreenPort.Text = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value; break;
                }
            }

            TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 221).ToList();
            for (int i = 0; i < TempListCO.Count; i++)
            {
                ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                if (machineid != null && machineid.Value == MachineId)
                {
                    this.CheckRecord.IsChecked = true;
                    this.TexRecordPort.Text = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value; break;
                }
            }

            TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 217).ToList();
            for (int i = 0; i < TempListCO.Count; i++)
            {
                ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                if (machineid != null && machineid.Value == MachineId)
                {
                    this.CheckAlarmMonitor.IsChecked = true;
                    this.TexAlarmMonitorPort.Text = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value; break;
                }
            }

            TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 218).ToList();
            for (int i = 0; i < TempListCO.Count; i++)
            {
                ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                if (machineid != null && machineid.Value == MachineId)
                {
                    this.CheckAlarm.IsChecked = true;
                    this.TexAlarmPort.Text = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value; break;
                }
            }

            TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 219).ToList();
            for (int i = 0; i < TempListCO.Count; i++)
            {
                ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                if (machineid != null && machineid.Value == MachineId)
                {
                    this.CheckSFTP.IsChecked = true;
                    this.TexSFTPPort.Text = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value; break;
                }
            }

            TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 211).ToList();
            for (int i = 0; i < TempListCO.Count; i++)
            {
                ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                if (machineid != null && machineid.Value == MachineId)
                {
                    this.CheckLicense.IsChecked = true;
                    this.TexLicensePort.Text = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value; break;
                }
            }

            TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 213).ToList();
            for (int i = 0; i < TempListCO.Count; i++)
            {
                ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                if (machineid != null && machineid.Value == MachineId)
                {
                    this.CheckCTIHub.IsChecked = true;
                    this.TexCTIHubPort.Text = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value; break;
                }
            }

            TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 212).ToList();
            for (int i = 0; i < TempListCO.Count; i++)
            {
                ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                if (machineid != null && machineid.Value == MachineId)
                {
                    this.CheckDEC.IsChecked = true;
                    this.TexDECPort.Text = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value; break;
                }
            }

            TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 215).ToList();
            for (int i = 0; i < TempListCO.Count; i++)
            {
                ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                if (machineid != null && machineid.Value == MachineId)
                {
                    this.CheckDBBridge.IsChecked = true;
                    this.TexDBBridgePort.Text = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value; break;
                }
            }

            TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 251).ToList();
            for (int i = 0; i < TempListCO.Count; i++)
            {
                ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                if (machineid != null && machineid.Value == MachineId)
                {
                    this.CheckDocumentProcess.IsChecked = true;
                    this.TexDocumentProcessPort.Text = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value; break;
                }
            }
            #endregion
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            //保存内容:
            SaveMachineConfig();
            //下一页:循环打开自己界面
            if (MachineNumber == TotalMachineNumber)
            {
                //打开下一页，界面3
                if (mListRecordConfigObject.Count != 0)
                {
                    UCWizardRecordConfig ucwizard = new UCWizardRecordConfig();
                    ucwizard.MainPage = MainPage;
                    ucwizard.PrePage = this;
                    ucwizard.CurrentApp = CurrentApp;
                    ucwizard.ListScreenConfig = mListScreenConfigObject;
                    ucwizard.ListRecordConfig = mListRecordConfigObject;
                    ucwizard.WizardHelper = this.WizardHelper;
                    MainPage.PopupPanel.Title = "Config Wizard";
                    MainPage.PopupPanel.Content = ucwizard;
                    MainPage.PopupPanel.IsOpen = true;
                }
                else if (mListScreenConfigObject.Count != 0)
                {
                    //进入录屏服务配置界面
                    //下一页。进入界面9:录屏参数配置
                    WizardHelper.ListScreens = mListScreenConfigObject;
                    UCWizardScreenConfig ucwizard = new UCWizardScreenConfig();
                    ucwizard.MainPage = MainPage;
                    ucwizard.CurrentApp = CurrentApp;
                    ucwizard.PrePage = this;
                    //ucwizard.RecordConfigObj = this.RecordConfigObj;
                    //ucwizard.ParentItem = this.ParentObjectItem;
                    ucwizard.WizardHelper = this.WizardHelper;
                    //ucwizard.ListConfigObjects = ListConfigObjects;
                    MainPage.PopupPanel.Title = "Config Wizard";
                    MainPage.PopupPanel.Content = ucwizard;
                    MainPage.PopupPanel.IsOpen = true;
                }
                else
                {
                    //接下来的界面……CTI
                    UCWizardCTIConfig ucwizard = new UCWizardCTIConfig();
                    ucwizard.MainPage = MainPage;
                    ucwizard.CurrentApp = CurrentApp;
                    ucwizard.PrePage = this;
                    //ucwizard.ScreenConfigObj = this.ScreenSeviceConfigObj;
                    //ucwizard.ParentItem = this.ParentObjectItem;
                    ucwizard.IsAsk = true;
                    ucwizard.WizardHelper = this.WizardHelper;
                    //ucwizard.ListConfigObjects = ListConfigObjects;
                    MainPage.PopupPanel.Title = "Config Wizard";
                    MainPage.PopupPanel.Content = ucwizard;
                    MainPage.PopupPanel.IsOpen = true;
                }
            }
            else
            {
                //打开自己界面，看是加载还是空白
                this.CheckAlarm.IsChecked = false;
                this.CheckAlarmMonitor.IsChecked = false;
                this.CheckCTIHub.IsChecked = false;
                this.CheckDBBridge.IsChecked = false;
                this.CheckDEC.IsChecked = false;
                this.CheckDocumentProcess.IsChecked = false;
                this.CheckLicense.IsChecked = false;
                this.CheckRecord.IsChecked = false;
                this.CheckScreen.IsChecked = false;
                this.CheckSFTP.IsChecked = false;

                this.IPTextBox.SetIP("127.0.0.1");
                this.EditorCatalog.Text = string.Empty;
                this.EditorDirectory.Text = string.Empty;
                this.TexLogPath.Text = string.Empty;

                MachineNumber++;
            }
        }

        private void SaveMachineConfig()
        {
            //没有考虑上一页之后，取消掉勾。
            if (!CheckValues()) { return; }
            string MachineName = string.Empty;
            bool IsNew = false;
            //机器的
            ConfigObject MachineConfigObj = mListSaveConfigObject[MachineNumber - 1];
            string MachineID = mListSaveConfigObject[MachineNumber - 1].ObjectID.ToString();
            if (MachineID == "0")
            {
                IsNew = true;
                MachineConfigObj = ConfigObject.CreateObject(210);
                MachineConfigObj = WizardHelper.CreateNewConfigObject(MachineConfigObj, 210);
            }
            MachineID = MachineConfigObj.ObjectID.ToString();
            //ConfigObject MachineConfigObj = ListConfigObjects.FirstOrDefault(p => p.ObjectID.ToString() == MachineID);
            if (MachineConfigObj != null)
            {
                for (int i = 0; i < MachineConfigObj.ListProperties.Count; i++)
                {
                    if (MachineConfigObj.ListProperties[i].PropertyID == 7)
                    {
                        MachineConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                    }
                    if (MachineConfigObj.ListProperties[i].PropertyID == 11)
                    {
                        MachineConfigObj.ListProperties[i].Value = this.TexLogPath.Text;
                    }
                    if (MachineConfigObj.ListProperties[i].PropertyID == 8)
                    {
                        MachineName = MachineConfigObj.ListProperties[i].Value;
                    }
                }
                MachineConfigObj.GetBasicPropertyValues();
                List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                int j = 0;
                for (; j < tempObjItem.Count; j++)
                {
                    ConfigGroup tempCG = tempObjItem[j].Data as ConfigGroup;
                    if (tempCG.GroupID == 1 && tempCG.ChildType == 210)
                    {
                        if (IsNew)
                            MainPage.CreateNewObjectItem(tempObjItem[j], MachineConfigObj, false);
                        break;
                    }
                }
                mListSaveConfigObject[MachineNumber - 1] = MachineConfigObj;
                MainPage.RefreshConfigObjectItem(tempObjItem[j]);
                MainPage.RefreshCenterView();
            }
            //存储设备保存
            if (this.EditorCatalog.Text.Trim() != null && this.EditorCatalog.Text.Trim() != string.Empty)
            {
                bool IsExist = false; ObjectItem SaveObjItem = new ObjectItem();
                List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 214).ToList();
                for (int i = 0; i < TempListCO.Count; i++)
                {
                    ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 12);
                    if (machineid != null && machineid.Value == MachineID)
                    {
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 14).Value = this.EditorCatalog.Text;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10).Value = MachineID;
                        TempListCO[i].GetBasicPropertyValues();
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                    int j = 0; bool IsFind = false; ConfigGroup tempCG = new ConfigGroup();
                    for (; j < tempObjItem.Count; j++)
                    {
                        SaveObjItem = tempObjItem[j];
                        tempCG = SaveObjItem.Data as ConfigGroup;
                        if (tempCG.GroupID == 24 && tempCG.ChildType == 214)
                        {
                            int maxValue = tempCG.GroupInfo.IntValue02;
                            int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 214).ToList().Count;
                            if ((ReadyNumber + 1) > maxValue) { return; }
                            IsFind = true;
                            break;
                        }
                    }
                    if (IsFind)
                    {
                        ConfigObject SaveConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 214);
                        SaveConfigObj.CurrentApp = CurrentApp;
                        for (int i = 0; i < SaveConfigObj.ListProperties.Count; i++)
                        {
                            if (SaveConfigObj.ListProperties[i].PropertyID == 13)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 12)
                            {
                                SaveConfigObj.ListProperties[i].Value = MachineID;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 14)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.EditorCatalog.Text;
                            }
                        }
                        SaveConfigObj.GetBasicPropertyValues();
                        MainPage.CreateNewObjectItem(SaveObjItem, SaveConfigObj, false);
                    }
                }
                MainPage.RefreshConfigObjectItem(SaveObjItem);
            }
            #region 服务

            if (this.CheckAlarm.IsChecked == true)
            {
                bool IsExist = false; ObjectItem SaveObjItem = new ObjectItem();
                List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                int j = 0; bool IsFind = false; ConfigGroup tempCG = new ConfigGroup();
                for (; j < tempObjItem.Count; j++)
                {
                    SaveObjItem = tempObjItem[j];
                    tempCG = SaveObjItem.Data as ConfigGroup;
                    if (tempCG.GroupID == 27 && tempCG.ChildType == 218)
                    {
                        int maxValue = tempCG.GroupInfo.IntValue02;
                        int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 218).ToList().Count;
                        if ((ReadyNumber + 1) > maxValue) { return; }
                        IsFind = true;
                        break;
                    }
                }
                List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 218).ToList();
                for (int i = 0; i < TempListCO.Count; i++)
                {
                    ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                    if (machineid != null && machineid.Value == MachineID)
                    {
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value = this.TexAlarmPort.Text;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10).Value = MachineID;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 7).Value = this.IPTextBox.GetIP();
                        TempListCO[i].GetBasicPropertyValues();
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    if (IsFind)
                    {
                        ConfigObject SaveConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 218);
                        SaveConfigObj.CurrentApp = CurrentApp;
                        for (int i = 0; i < SaveConfigObj.ListProperties.Count; i++)
                        {
                            if (SaveConfigObj.ListProperties[i].PropertyID == 7)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 9)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.TexAlarmPort.Text;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 10)
                            {
                                SaveConfigObj.ListProperties[i].Value = MachineID;
                            }
                        }SaveConfigObj.GetBasicPropertyValues();
                        MainPage.CreateNewObjectItem(SaveObjItem, SaveConfigObj, false);
                    }
                }
                MainPage.RefreshConfigObjectItem(SaveObjItem);
            }
            if (this.CheckAlarmMonitor.IsChecked == true)
            {
                bool IsExist = false; ObjectItem SaveObjItem = new ObjectItem();
                List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 217).ToList();
                for (int i = 0; i < TempListCO.Count; i++)
                {
                    ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                    if (machineid != null && machineid.Value == MachineID)
                    {
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 7).Value = this.IPTextBox.GetIP();
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value = this.TexAlarmMonitorPort.Text;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10).Value = MachineID;
                        TempListCO[i].GetBasicPropertyValues();
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                    int j = 0; bool IsFind = false; ConfigGroup tempCG = new ConfigGroup();
                    for (; j < tempObjItem.Count; j++)
                    {
                        SaveObjItem = tempObjItem[j];
                        tempCG = SaveObjItem.Data as ConfigGroup;
                        if (tempCG.GroupID == 26 && tempCG.ChildType == 217)
                        {
                            int maxValue = tempCG.GroupInfo.IntValue02;
                            int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 217).ToList().Count;
                            if ((ReadyNumber + 1) > maxValue) { return; }
                            IsFind = true;
                            break;
                        }
                    }
                    if (IsFind)
                    {
                        ConfigObject SaveConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 217);
                        SaveConfigObj.CurrentApp = CurrentApp;
                        for (int i = 0; i < SaveConfigObj.ListProperties.Count; i++)
                        {
                            if (SaveConfigObj.ListProperties[i].PropertyID == 7)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 9)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.TexAlarmMonitorPort.Text;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 10)
                            {
                                SaveConfigObj.ListProperties[i].Value = MachineID;
                            }
                        }
                        SaveConfigObj.GetBasicPropertyValues();
                        MainPage.CreateNewObjectItem(SaveObjItem, SaveConfigObj, false);
                    }
                }
                MainPage.RefreshConfigObjectItem(SaveObjItem);
            }
            if (this.CheckCTIHub.IsChecked == true)
            {
                bool IsExist = false; ObjectItem SaveObjItem = new ObjectItem();
                List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 213).ToList();
                for (int i = 0; i < TempListCO.Count; i++)
                {
                    ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                    if (machineid != null && machineid.Value == MachineID)
                    {
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 7).Value = this.IPTextBox.GetIP();
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value = this.TexCTIHubPort.Text;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10).Value = MachineID;
                        TempListCO[i].GetBasicPropertyValues();
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                    int j = 0; bool IsFind = false; ConfigGroup tempCG = new ConfigGroup();
                    for (; j < tempObjItem.Count; j++)
                    {
                        SaveObjItem = tempObjItem[j];
                        tempCG = SaveObjItem.Data as ConfigGroup;
                        if (tempCG.GroupID == 23 && tempCG.ChildType == 213)
                        {
                            int maxValue = tempCG.GroupInfo.IntValue02;
                            int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 213).ToList().Count;
                            if ((ReadyNumber + 1) > maxValue) { return; }
                            IsFind = true;
                            break;
                        }
                    }
                    if (IsFind)
                    {
                        ConfigObject SaveConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 213);
                        SaveConfigObj.CurrentApp = CurrentApp;
                        for (int i = 0; i < SaveConfigObj.ListProperties.Count; i++)
                        {
                            if (SaveConfigObj.ListProperties[i].PropertyID == 7)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 9)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.TexCTIHubPort.Text;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 10)
                            {
                                SaveConfigObj.ListProperties[i].Value = MachineID;
                            }
                        }
                        SaveConfigObj.GetBasicPropertyValues();
                        MainPage.CreateNewObjectItem(SaveObjItem, SaveConfigObj, false);
                    }
                }
                MainPage.RefreshConfigObjectItem(SaveObjItem);
            }
            if (this.CheckDBBridge.IsChecked == true)
            {
                bool IsExist = false; ObjectItem SaveObjItem = new ObjectItem();
                List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                int j = 0; bool IsFind = false; ConfigGroup tempCG = new ConfigGroup();
                for (; j < tempObjItem.Count; j++)
                {
                    SaveObjItem = tempObjItem[j];
                    tempCG = SaveObjItem.Data as ConfigGroup;
                    if (tempCG.GroupID == 25 && tempCG.ChildType == 215)
                    {
                        int maxValue = tempCG.GroupInfo.IntValue02;
                        int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 215).ToList().Count;
                        if ((ReadyNumber + 1) > maxValue) { return; }
                        IsFind = true;
                        break;
                    }
                }
                List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 215).ToList();
                for (int i = 0; i < TempListCO.Count; i++)
                {
                    ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                    if (machineid != null && machineid.Value == MachineID)
                    {
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 7).Value = this.IPTextBox.GetIP();
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value = this.TexDBBridgePort.Text;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10).Value = MachineID;
                        TempListCO[i].GetBasicPropertyValues();
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    if (IsFind)
                    {
                        ConfigObject SaveConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 215);
                        SaveConfigObj.CurrentApp = CurrentApp;
                        for (int i = 0; i < SaveConfigObj.ListProperties.Count; i++)
                        {
                            if (SaveConfigObj.ListProperties[i].PropertyID == 7)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 9)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.TexDBBridgePort.Text;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 10)
                            {
                                SaveConfigObj.ListProperties[i].Value = MachineID;
                            }
                        }
                        SaveConfigObj.GetBasicPropertyValues();
                        MainPage.CreateNewObjectItem(SaveObjItem, SaveConfigObj, false);
                    }
                }
                MainPage.RefreshConfigObjectItem(SaveObjItem);
            }
            if (this.CheckDEC.IsChecked == true)
            {
                bool IsExist = false; ObjectItem SaveObjItem = new ObjectItem();
                List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 212).ToList();
                for (int i = 0; i < TempListCO.Count; i++)
                {
                    ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                    if (machineid != null && machineid.Value == MachineID)
                    {
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 7).Value = this.IPTextBox.GetIP();
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value = this.TexDECPort.Text;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10).Value = MachineID;
                        TempListCO[i].GetBasicPropertyValues();
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                    int j = 0; bool IsFind = false; ConfigGroup tempCG = new ConfigGroup();
                    for (; j < tempObjItem.Count; j++)
                    {
                        SaveObjItem = tempObjItem[j];
                        tempCG = SaveObjItem.Data as ConfigGroup;
                        if (tempCG.GroupID == 22 && tempCG.ChildType == 212)
                        {
                            int maxValue = tempCG.GroupInfo.IntValue02;
                            int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 212).ToList().Count;
                            if ((ReadyNumber + 1) > maxValue) { return; }
                            IsFind = true;
                            break;
                        }
                    }
                    if (IsFind)
                    {
                        ConfigObject SaveConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 212);
                        SaveConfigObj.CurrentApp = CurrentApp;
                        for (int i = 0; i < SaveConfigObj.ListProperties.Count; i++)
                        {
                            if (SaveConfigObj.ListProperties[i].PropertyID == 7)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 9)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.TexDECPort.Text;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 10)
                            {
                                SaveConfigObj.ListProperties[i].Value = MachineID;
                            }
                        }
                        SaveConfigObj.GetBasicPropertyValues();
                        MainPage.CreateNewObjectItem(SaveObjItem, SaveConfigObj, false);
                    }
                }
                MainPage.RefreshConfigObjectItem(SaveObjItem);
            }
            if (this.CheckLicense.IsChecked == true)
            {
                bool IsExist = false; ObjectItem SaveObjItem = new ObjectItem();
                List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 211).ToList();
                for (int i = 0; i < TempListCO.Count; i++)
                {
                    ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                    if (machineid != null && machineid.Value == MachineID)
                    {
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 7).Value = this.IPTextBox.GetIP();
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value = this.TexLicensePort.Text;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10).Value = MachineID;
                        TempListCO[i].GetBasicPropertyValues();
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                    int j = 0; bool IsFind = false; ConfigGroup tempCG = new ConfigGroup();
                    for (; j < tempObjItem.Count; j++)
                    {
                        SaveObjItem = tempObjItem[j];
                        tempCG = SaveObjItem.Data as ConfigGroup;
                        if (tempCG.GroupID == 21 && tempCG.ChildType == 211)
                        {
                            int maxValue = tempCG.GroupInfo.IntValue02;
                            int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 211).ToList().Count;
                            if ((ReadyNumber + 1) > maxValue) { return; }
                            IsFind = true;
                            break;
                        }
                    }
                    if (IsFind)
                    {
                        ConfigObject SaveConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 211);
                        SaveConfigObj.CurrentApp = CurrentApp;
                        for (int i = 0; i < SaveConfigObj.ListProperties.Count; i++)
                        {
                            if (SaveConfigObj.ListProperties[i].PropertyID == 7)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 9)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.TexLicensePort.Text;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 10)
                            {
                                SaveConfigObj.ListProperties[i].Value = MachineID;
                            }
                        }
                        SaveConfigObj.GetBasicPropertyValues();
                        MainPage.CreateNewObjectItem(SaveObjItem, SaveConfigObj, false);
                    }
                }
                MainPage.RefreshConfigObjectItem(SaveObjItem);
            }
            if (this.CheckDocumentProcess.IsChecked == true)
            {
                bool IsExist = false; ObjectItem SaveObjItem = new ObjectItem();
                List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 251).ToList();
                for (int i = 0; i < TempListCO.Count; i++)
                {
                    ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                    if (machineid != null && machineid.Value == MachineID)
                    {
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 7).Value = this.IPTextBox.GetIP();
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value = this.TexDocumentProcessPort.Text;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10).Value = MachineID;
                        TempListCO[i].GetBasicPropertyValues();
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                    int j = 0; bool IsFind = false; ConfigGroup tempCG = new ConfigGroup();
                    for (; j < tempObjItem.Count; j++)
                    {
                        SaveObjItem = tempObjItem[j];
                        tempCG = SaveObjItem.Data as ConfigGroup;
                        if (tempCG.GroupID == 31 && tempCG.ChildType == 251)
                        {
                            int maxValue = tempCG.GroupInfo.IntValue02;
                            int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 251).ToList().Count;
                            if ((ReadyNumber + 1) > maxValue) { return; }
                            IsFind = true;
                            break;
                        }
                    }
                    if (IsFind)
                    {
                        ConfigObject SaveConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 251);
                        SaveConfigObj.CurrentApp = CurrentApp;
                        for (int i = 0; i < SaveConfigObj.ListProperties.Count; i++)
                        {
                            if (SaveConfigObj.ListProperties[i].PropertyID == 7)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 9)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.TexDocumentProcessPort.Text;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 10)
                            {
                                SaveConfigObj.ListProperties[i].Value = MachineID;
                            }
                        }
                        SaveConfigObj.GetBasicPropertyValues();
                        MainPage.CreateNewObjectItem(SaveObjItem, SaveConfigObj, false);
                    }
                }
                MainPage.RefreshConfigObjectItem(SaveObjItem);
            }
            if (this.CheckRecord.IsChecked == true)
            {
                bool IsExist = false; ObjectItem SaveObjItem = new ObjectItem();
                List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 221).ToList();
                for (int i = 0; i < TempListCO.Count; i++)
                {
                    ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                    if (machineid != null && machineid.Value == MachineID)
                    {
                        int pos = 0;
                        for (; pos < mListRecordConfigObject.Count; pos++)
                        {
                            if (mListRecordConfigObject[pos] == TempListCO[i])
                            {
                                break;
                            }
                        }
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 7).Value = this.IPTextBox.GetIP();
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value = this.TexRecordPort.Text;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10).Value = MachineID;
                        TempListCO[i].GetBasicPropertyValues();
                        mListRecordConfigObject[pos] = TempListCO[i];
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                    int j = 0; bool IsFind = false; ConfigGroup tempCG = new ConfigGroup();
                    for (; j < tempObjItem.Count; j++)
                    {
                        SaveObjItem = tempObjItem[j];
                        tempCG = SaveObjItem.Data as ConfigGroup;
                        if (tempCG.GroupID == 41 && tempCG.ChildType == 221)
                        {
                            int maxValue = tempCG.GroupInfo.IntValue02;
                            int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 221).ToList().Count;
                            if ((ReadyNumber + 1) > maxValue) { return; }
                            IsFind = true;
                            break;
                        }
                    }
                    if (IsFind)
                    {
                        ConfigObject SaveConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 221);
                        SaveConfigObj.CurrentApp = CurrentApp;
                        for (int i = 0; i < SaveConfigObj.ListProperties.Count; i++)
                        {
                            if (SaveConfigObj.ListProperties[i].PropertyID == 9)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.TexRecordPort.Text;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 10)
                            {
                                SaveConfigObj.ListProperties[i].Value = MachineID;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 7)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                            }
                        }
                        SaveConfigObj.GetBasicPropertyValues();
                        MainPage.CreateNewObjectItem(SaveObjItem, SaveConfigObj, false);
                        mListRecordConfigObject.Add(SaveConfigObj);
                    }
                }
                MainPage.RefreshConfigObjectItem(SaveObjItem);
            }
            if (this.CheckScreen.IsChecked == true)
            {
                bool IsExist = false; ObjectItem SaveObjItem = new ObjectItem();
                List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 231).ToList();
                for (int i = 0; i < TempListCO.Count; i++)
                {
                    ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                    if (machineid != null && machineid.Value == MachineID)
                    {
                        int pos = 0;
                        for (; pos < mListScreenConfigObject.Count; pos++)
                        {
                            if (mListScreenConfigObject[pos] == TempListCO[i])
                            {
                                break;
                            }
                        }
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 7).Value = this.IPTextBox.GetIP();
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value = this.TexScreenPort.Text;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10).Value = MachineID;
                        TempListCO[i].GetBasicPropertyValues();
                        mListScreenConfigObject[pos] = TempListCO[i];
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                    int j = 0; bool IsFind = false; ConfigGroup tempCG = new ConfigGroup();
                    for (; j < tempObjItem.Count; j++)
                    {
                        SaveObjItem = tempObjItem[j];
                        tempCG = SaveObjItem.Data as ConfigGroup;
                        if (tempCG.GroupID == 42 && tempCG.ChildType == 231)
                        {
                            int maxValue = tempCG.GroupInfo.IntValue02;
                            int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 231).ToList().Count;
                            if ((ReadyNumber + 1) > maxValue) { return; }
                            IsFind = true;
                            break;
                        }
                    }
                    if (IsFind)
                    {
                        ConfigObject SaveConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 231);
                        SaveConfigObj.CurrentApp = CurrentApp;
                        for (int i = 0; i < SaveConfigObj.ListProperties.Count; i++)
                        {
                            if (SaveConfigObj.ListProperties[i].PropertyID == 9)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.TexScreenPort.Text;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 10)
                            {
                                SaveConfigObj.ListProperties[i].Value = MachineID;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 7)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                            }
                        }
                        SaveConfigObj.GetBasicPropertyValues();
                        MainPage.CreateNewObjectItem(SaveObjItem, SaveConfigObj, false);
                        mListScreenConfigObject.Add(SaveConfigObj);
                    }
                }
                MainPage.RefreshConfigObjectItem(SaveObjItem);
            }
            if (this.CheckSFTP.IsChecked == true)
            {
                bool IsExist = false; ObjectItem SaveObjItem = new ObjectItem();
                List<ConfigObject> TempListCO = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 219).ToList();
                for (int i = 0; i < TempListCO.Count; i++)
                {
                    ResourceProperty machineid = TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10);
                    if (machineid != null && machineid.Value == MachineID)
                    {
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 7).Value = this.IPTextBox.GetIP();
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 9).Value = this.TexSFTPPort.Text;
                        TempListCO[i].ListProperties.FirstOrDefault(p => p.PropertyID == 10).Value = MachineID;
                        TempListCO[i].GetBasicPropertyValues();
                        IsExist = true;
                        break;
                    }
                }
                if (!IsExist)
                {
                    List<ObjectItem> tempObjItem = WizardHelper.ListAllObjectItem.Where(p => p.Type == 1).ToList();
                    int j = 0; bool IsFind = false; ConfigGroup tempCG = new ConfigGroup();
                    for (; j < tempObjItem.Count; j++)
                    {
                        SaveObjItem = tempObjItem[j];
                        tempCG = SaveObjItem.Data as ConfigGroup;
                        if (tempCG.GroupID == 28 && tempCG.ChildType == 219)
                        {
                            int maxValue = tempCG.GroupInfo.IntValue02;
                            int ReadyNumber = WizardHelper.ListAllConfigObjects.Where(p => p.ObjectType == 219).ToList().Count;
                            if ((ReadyNumber + 1) > maxValue) { return; }
                            IsFind = true;
                            break;
                        }
                    }
                    if (IsFind)
                    {
                        ConfigObject SaveConfigObj = WizardHelper.CreateNewConfigObject(tempCG.ConfigObject, 219);
                        SaveConfigObj.CurrentApp = CurrentApp;
                        for (int i = 0; i < SaveConfigObj.ListProperties.Count; i++)
                        {
                            if (SaveConfigObj.ListProperties[i].PropertyID == 7)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.IPTextBox.GetIP();
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 9)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.TexSFTPPort.Text;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 21)
                            {
                                SaveConfigObj.ListProperties[i].Value = this.EditorDirectory.Text;
                            }
                            if (SaveConfigObj.ListProperties[i].PropertyID == 10)
                            {
                                SaveConfigObj.ListProperties[i].Value = MachineID;
                            }
                        }
                        SaveConfigObj.GetBasicPropertyValues();
                        MainPage.CreateNewObjectItem(SaveObjItem, SaveConfigObj, false);
                    }
                }
                MainPage.RefreshConfigObjectItem(SaveObjItem);
            }
            #endregion
        }

        void ButnBack_Click(object sender, RoutedEventArgs e)
        {
            ////提示该页信息清空
            //System.Windows.Forms.DialogResult dr = System.Windows.Forms.MessageBox.Show(App.GetLanguageInfo("", "返回后将无法保存该界面内容。是否返回？"),
            //    App.AppName, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
            //if (dr == System.Windows.Forms.DialogResult.No) { return; }
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            //上一页
            if (MachineNumber == 1)
            {
                //打开界面1
                MainPage.PopupPanel.Content = PrePage;
            }
            else
            {
                MachineNumber--;
                //打开上一个物理机器配置界面……ing
                InitControlObject();
            }
        }

        private bool CheckValues()
        {
            bool IsIpRight = false;
            //检查IP地址是否正确
            string ip = this.IPTextBox.GetIP();
            List<string> ipNumber = ip.Split('.').ToList();
            for (int i = 0; i < ipNumber.Count(); i++)
            {
                int tempNumber = 0;
                if (int.TryParse(ipNumber[i], out tempNumber))
                {
                    if (tempNumber >= 0 && tempNumber <= 255)
                        IsIpRight = true;
                }
                else
                {
                    CurrentApp.GetLanguageInfo("1110WIZT0003", "IP Error");
                    IsIpRight = false;
                }
            }
            //检查路径？
            if (IsIpRight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            this.CheckAlarm.Content = CurrentApp.GetLanguageInfo("1110WIZ02009", "告警服务");
            this.CheckAlarmMonitor.Content = CurrentApp.GetLanguageInfo("1110WIZ01008", "告警监视服务");
            this.LabCatalog.Content = CurrentApp.GetLanguageInfo("1110WIZ02003", "主目录");
            this.CheckCTIHub.Content = CurrentApp.GetLanguageInfo("1110WIZ02006", "CTIHub服务");
            this.CheckDBBridge.Content = CurrentApp.GetLanguageInfo("1110WIZ02007", "DBBridge服务");
            this.CheckDEC.Content = CurrentApp.GetLanguageInfo("1110WIZ02005", "DEC服务");
            this.CheckDocumentProcess.Content = CurrentApp.GetLanguageInfo("1110WIZ02010", "文件处理服务");
            this.CheckLicense.Content = CurrentApp.GetLanguageInfo("1110WIZ02004", "许可服务");
            this.LabLogPath.Content = CurrentApp.GetLanguageInfo("1110WIZ02002", "日志路径");
            this.LabMachineIP.Content = CurrentApp.GetLanguageInfo("1110WIZ02001", "主机地址");
            this.CheckRecord.Content = CurrentApp.GetLanguageInfo("1110WIZ02011", "录音服务");
            this.CheckScreen.Content = CurrentApp.GetLanguageInfo("1110WIZ02012", "录屏服务");
            this.CheckSFTP.Content = CurrentApp.GetLanguageInfo("1110WIZ02013", "SFTP服务");
            this.LabSFTPPath.Content = CurrentApp.GetLanguageInfo("1110WIZ02014", "根目录");

            string PortName = CurrentApp.GetLanguageInfo("1110WIZ02015", "端口：");
            this.LabAlarmMonitorPort.Content = PortName;
            this.LabAlarmPort.Content = PortName;
            this.LabCTIHubPort.Content = PortName;
            this.LabDBBridgePort.Content = PortName;
            this.LabDECPort.Content = PortName;
            this.LabDocumentProcessPort.Content = PortName;
            this.LabLicensePort.Content = PortName;
            this.LabSFTPPort.Content = PortName;
            this.LabRecordPort.Content = PortName;
            this.LabScreenPort.Content = PortName;

            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            this.ButnBack.Content = CurrentApp.GetLanguageInfo("1110WIZB0003", "返回");
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ01001", "配置向导");
        }

        public WizardHelper WizardHelper { get; set; }

        public void ReturnPage()
        {
            if (mListRecordConfigObject != null && mListRecordConfigObject.Count != 0)
            {
                mListRecordConfigObject.Remove(mListRecordConfigObject[mListRecordConfigObject.Count - 1]);
            }
            if (mListScreenConfigObject != null && mListScreenConfigObject.Count != 0)
            {
                mListScreenConfigObject.Remove(mListScreenConfigObject[mListScreenConfigObject.Count - 1]);
            }
        }
    }
}
