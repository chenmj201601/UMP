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
    /// UCWizardRecordChannelAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardRecordChannelAdd : IWizard
    {
        #region members
        public ResourceMainView MainPage;
        public ConfigObject RecordConfigObj;
        public UCWizardRecordChannel PrePage;

        private ObservableCollection<ConfigObject> mListBaseChannels;
        public ObjectItem ParentItem;
        private ConfigObject mParentObject;
        //private int RecordChanNumber;
        #endregion

        public UCWizardRecordChannelAdd()
        {
            InitializeComponent();

            //RecordChanNumber = 1;
            this.Loaded += UCWizardRecordChannelAdd_Loaded;
            this.ButnNext.Click += ButnNext_Click;
            this.ButnPrevious.Click += ButnPrevious_Click;
            mListBaseChannels = new ObservableCollection<ConfigObject>();
            this.CombBasedon.ItemsSource = mListBaseChannels;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            MainPage.PopupPanel.Content = PrePage;
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            //跟点击确定一样的效果：批量生成好多通道
            if (!CheckInput()) { return; }
            if (!CheckCount()) { return; }
            AddChannels();
            //opean next page
            for (int i = 0; i < WizardHelper.ListRecords.Count; i++)
            {
                if (RecordConfigObj.ObjectID == WizardHelper.ListRecords[i].ObjectID)
                {
                    if (i == (WizardHelper.ListRecords.Count - 1))
                    {
                        if (WizardHelper.ListScreens != null && WizardHelper.ListScreens.Count != 0)
                        {
                            //下一页。进入界面9:录屏参数配置
                            UCWizardScreenConfig ucwizard = new UCWizardScreenConfig();
                            ucwizard.MainPage = MainPage;
                            ucwizard.ChanAddPrepage = this;
                            ucwizard.CurrentApp = CurrentApp;
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
                            UCWizardCTIConfig ucwizard = new UCWizardCTIConfig();
                            ucwizard.MainPage = MainPage;
                            ucwizard.CurrentApp = CurrentApp;
                            ucwizard.PreRecordAdd = this;
                            //ucwizard.ScreenConfigObj = this.ScreenSeviceConfigObj;
                            //ucwizard.ParentItem = this.ParentObjectItem;
                            ucwizard.IsAsk = true;
                            ucwizard.WizardHelper = this.WizardHelper;
                            //ucwizard.ListConfigObjects = ListConfigObjects;
                            MainPage.PopupPanel.Title = "Config Wizard";
                            MainPage.PopupPanel.Content = ucwizard;
                            MainPage.PopupPanel.IsOpen = true;
                        }
                        break;
                    }
                    else
                    {
                        //循环
                        WizardHelper.UCRecordConfig.LoopLoad();
                        MainPage.PopupPanel.Content = WizardHelper.UCRecordConfig;
                    }
                }
            }                
            
        }

        void UCWizardRecordChannelAdd_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeLanguage();
            InitControlObject();
        }

        private void InitControlObject()
        {
            try
            {
                if (ParentItem == null) { return; }
                ConfigGroup parentGroup = ParentItem.Data as ConfigGroup;
                if (parentGroup == null) { return; }
                ConfigObject parentObject = parentGroup.ConfigObject;
                mParentObject = parentObject;
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
            InitBaseChannels();

        }

        private void InitBaseChannels()
        {
            try
            {
                if (ParentItem == null) { return; }
                ConfigGroup configGroup = ParentItem.Data as ConfigGroup;
                if (configGroup == null) { return; }
                ConfigObject parentObject = configGroup.ConfigObject;
                if (parentObject == null || parentObject.ObjectType != S1110Consts.RESOURCE_VOICESERVER) { return; }
                mListBaseChannels.Clear();
                for (int i = 0; i < parentObject.ListChildObjects.Count; i++)
                {
                    ConfigObject configObject = parentObject.ListChildObjects[i];
                    if (configObject.ObjectType == S1110Consts.RESOURCE_CHANNEL)
                    {
                        mListBaseChannels.Add(configObject);
                    }
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        #region Operations
        private void AddChannels()
        {
            try
            {
                if (this.UDNumber.Value == null) { return; }
                int intCount = (int)UDNumber.Value;
                string strExtension = this.TexChanName.Text;
                int intExtenstion;
                int.TryParse(strExtension, out intExtenstion);
                var baseExtension = this.CombBasedon.SelectedItem as ConfigObject;
                int objType = S1110Consts.RESOURCE_CHANNEL;
                if (WizardHelper.ListAllTypeParams == null) { return; }
                ResourceTypeParam typeParam =
                    WizardHelper.ListAllTypeParams.FirstOrDefault(t => t.TypeID == objType);
                if (typeParam == null) { return; }
                if (WizardHelper.ListAllPropertyInfos == null) { return; }
                List<ObjectPropertyInfo> listPropertyInfos =
                    WizardHelper.ListAllPropertyInfos.Where(p => p.ObjType == objType).OrderBy(p => p.PropertyID).ToList();
                if (WizardHelper.ListAllConfigObjects == null) { return; }
                //if (mParentObject == null) { return; }
                //long parentObjID = mParentObject.ObjectID;
                //逐一添加
                for (int i = 0; i < intCount; i++)
                {
                    //获取当前所有已经存在的通道
                    List<ConfigObject> channels =
                        WizardHelper.ListAllConfigObjects.Where(o => o.ObjectType == objType)
                            .OrderBy(o => o.ObjectID)
                            .ToList();
                    //确定ObjID
                    long objID = objType * (long)Math.Pow(10, 16) + 1;
                    for (int k = 0; k < channels.Count; k++)
                    {
                        if (objID == channels[k].ObjectID)
                        {
                            objID++;
                        }
                    }
                    //确定Key，Key就是ObjID的尾数
                    int key = (int)(objID - (objType * (long)Math.Pow(10, 16) + 1));
                    //确定ID
                    //获取当前VoiceServer下的所有通道
                    List<ConfigObject> channelsInVoice =
                        RecordConfigObj.ListChildObjects.Where(o => o.ObjectType == objType)
                            .OrderBy(o => o.ID)
                            .ToList();
                    int id = 0;
                    for (int k = 0; k < channelsInVoice.Count; k++)
                    {
                        if (id == channelsInVoice[k].ID)
                        {
                            id++;
                        }
                    }
                    foreach (var channel in channelsInVoice)
                    {
                        if (id == channel.ID)
                        {
                            id++;
                        }
                    }
                    //分机号
                    string strExt = strExtension;
                    if (intExtenstion > 0)
                    {
                        strExt = (intExtenstion + i).ToString();
                    }
                    else
                    {
                        if (i != 0)
                        {
                            strExt = string.Format("{0}{1}", strExt, i.ToString());
                        }
                    }
                    ConfigObject configObject = ConfigObject.CreateObject(objType);
                    configObject.CurrentApp = CurrentApp;
                    configObject.ObjectID = objID;
                    configObject.Icon = typeParam.Icon;
                    configObject.TypeParam = typeParam;
                    configObject.ListAllObjects = WizardHelper.ListAllConfigObjects;
                    configObject.ListAllTypeParams = WizardHelper.ListAllTypeParams;
                    configObject.ListAllBasicInfos = WizardHelper.ListAllBasicInfos;
                    for (int j = 0; j < listPropertyInfos.Count; j++)
                    {
                        ObjectPropertyInfo info = listPropertyInfos[j];
                        int propertyID = info.PropertyID;
                        ResourceProperty propertyValue = new ResourceProperty();
                        propertyValue.ObjID = configObject.ObjectID;
                        propertyValue.ObjType = configObject.ObjectType;
                        propertyValue.PropertyID = info.PropertyID;
                        //先使用默认属性填充
                        propertyValue.Value = info.DefaultValue;
                        propertyValue.EncryptMode = info.EncryptMode;
                        //拷贝属性
                        if (baseExtension != null)
                        {
                            ResourceProperty temp =
                                baseExtension.ListProperties.FirstOrDefault(p => p.PropertyID == propertyID);
                            if (temp != null)
                            {
                                if (temp.PropertyID == 13)
                                {
                                    string StrIP = temp.Value;
                                    propertyValue.Value = CheckIPAdd(StrIP, i + 1);
                                }
                                else
                                {
                                    propertyValue.Value = temp.Value;
                                }
                            }
                        }
                        //特别的属性重新赋值
                        if (propertyID == S1110Consts.PROPERTYID_KEY)
                        {
                            propertyValue.Value = key.ToString();
                        }
                        if (propertyID == S1110Consts.PROPERTYID_ID)
                        {
                            propertyValue.Value = id.ToString();
                        }
                        if (propertyID == S1110Consts.PROPERTYID_PARENTOBJID)
                        {
                            //propertyValue.Value = parentObjID.ToString();
                        }
                        if (propertyID == 12)
                        {
                            propertyValue.Value = strExt;
                        }
                        configObject.ListProperties.Add(propertyValue);
                        if (WizardHelper.ListAllPropertyValues != null)
                        {
                            WizardHelper.ListAllPropertyValues.Add(propertyValue);
                        }
                    }
                    configObject.Name = string.Format("[{0}] {1}", id.ToString("000"), strExt);
                    configObject.Description = string.Format("[{0}] {1}", id.ToString("000"), strExt);
                    configObject.GetBasicPropertyValues();
                    RecordConfigObj.ListChildObjects.Add(configObject);
                    WizardHelper.ListAllConfigObjects.Add(configObject);
                  
                    if (MainPage != null)
                    {
                        MainPage.CreateNewChannelConfigObjectItem(ParentItem, configObject);
                    }
                }

                #region 写操作日志

                //var optID = string.Format("1110{0}06", objType);
                //string strOptLog = string.Format("{0}:{1}", Utils.FormatOptLogString("1110202"), intCount);
                //App.WriteOperationLog(optID, ConstValue.OPT_RESULT_SUCCESS, strOptLog);

                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #endregion

        #region Others

        private bool CheckInput()
        {
            int intValue;
            if (this.UDNumber.Value == null)
            {
                ShowException(CurrentApp.GetMessageLanguageInfo("016", "Count invalid"));
                return false;
            }
            intValue = (int)UDNumber.Value;
            if (intValue <= 0 || intValue > 1024)
            {
                ShowException(CurrentApp.GetMessageLanguageInfo("016", "Count invalid"));
                return false;
            }
            if (string.IsNullOrEmpty(this.TexChanName.Text))
            {
                ShowException(CurrentApp.GetMessageLanguageInfo("017", "Extension invalid"));
                return false;
            }
            return true;
        }

        private bool CheckCount()
        {
            if (this.UDNumber.Value == null)
            {
                return false;
            }
            int intCount = (int)UDNumber.Value;
            if (WizardHelper.ListAllGroupParams == null)
            {
                return false;
            }
            ResourceGroupParam groupParam =
                WizardHelper.ListAllGroupParams.FirstOrDefault(
                    g => g.TypeID == S1110Consts.RESOURCE_VOICESERVER && g.GroupID == 25);
            if (groupParam == null)
            {
                return false;
            }
            int intMaxinum = groupParam.IntValue02;
            if (mParentObject == null)
            {
                return false;
            }
            int existNum = mParentObject.ListChildObjects.Count(o => o.ObjectType == S1110Consts.RESOURCE_CHANNEL);
            if (intCount + existNum > intMaxinum)
            {
                ShowException(string.Format("{0}\r\n\r\n{1}\t{2}",
                   CurrentApp.GetMessageLanguageInfo("008", "Over maxinum value"),
                   CurrentApp.GetLanguageInfo(string.Format("OBJ{0}", S1110Consts.RESOURCE_CHANNEL), S1110Consts.RESOURCE_CHANNEL.ToString()),
                   intMaxinum));
                return false;
            }
            return true;
        }

        private string CheckIPAdd(string IPStr, int AddNum)
        {
            string IPStrResult = IPStr;
            string[] IPStrs = IPStr.Split('.');
            if (IPStrs.Count() == 4)
            {
                int IP4 = Convert.ToInt32(IPStrs[3]) + AddNum;
                int IP3 = Convert.ToInt32(IPStrs[2]);
                int IP2 = Convert.ToInt32(IPStrs[1]);
                int IP1 = Convert.ToInt32(IPStrs[0]);
                if (IP4 >= 255)
                {
                    int Carry3 = IP4 / 255;
                    IP4 = IP4 - Carry3 * 255;
                    IP3 += Carry3;
                    if (IP3 > 255)
                    {
                        int Carry2 = IP3 / 255;
                        IP3 = IP3 - 255 * Carry2;
                        IP2 += Carry2;
                        if (IP2 > 255)
                        {
                            int Carry1 = IP2 / 255;
                            IP2 = IP2 - 255 * Carry1;
                            IP1 += Carry1;
                        }
                    }
                }
                IPStrResult = string.Format("{0}.{1}.{2}.{3}", IP1.ToString(), IP2.ToString(), IP3.ToString(), IP4.ToString());
            }
            return IPStrResult;
        }

        #endregion

        public WizardHelper WizardHelper { set; get; }

        public override void ChangeLanguage()
        {
            //this.ButnBack.Content = App.GetLanguageInfo("", "返回");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0002", "上一页");
            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0001", "下一页");
            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110WIZ08001", "录音通道：IP");

            this.LabBasedOn.Content = CurrentApp.GetLanguageInfo("1110WIZ08004", "基于已有通道:");
            this.LabChanName.Content = CurrentApp.GetLanguageInfo("1110WIZ08003", "分机号：");
            this.LabNumber.Content = CurrentApp.GetLanguageInfo("1110WIZ08002", "数量");
        }
    }
}
