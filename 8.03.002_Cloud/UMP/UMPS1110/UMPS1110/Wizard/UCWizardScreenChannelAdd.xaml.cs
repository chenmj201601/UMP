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
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Controls;

namespace UMPS1110.Wizard
{
    /// <summary>
    /// UCWizardScreenChannelAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UCWizardScreenChannelAdd : IWizard
    {
        public ResourceMainView MainPage;
        public ConfigObject ScreenConfigObj;
        public UCWizardScreenChannel PrePage;
        public ObjectItem ParentItem;

        private ObservableCollection<ConfigObject> mListBaseChannels;
        private List<ConfigObject> ListScreenChannels;
        //private ConfigObject CurrentConfigObj;
        private ConfigObject mParentObject;
        //private int ScreenChanNumber;

        public UCWizardScreenChannelAdd()
        {
            InitializeComponent();

            mListBaseChannels = new ObservableCollection<ConfigObject>();
            ListScreenChannels = new List<ConfigObject>();

            this.Loaded += UCWizardScreenChannelAdd_Loaded;
            this.ButnPrevious.Click += ButnPrevious_Click;
            this.ButnNext.Click += ButnNext_Click;
        }

        void ButnNext_Click(object sender, RoutedEventArgs e)
        {
            //下一页，保存后 进入CTI配置
            if (!CheckInput()) { return; }
            if (!CheckCount()) { return; }
            AddScreenChannels();
            UCWizardCTIConfig ucwizard = new UCWizardCTIConfig();
            ucwizard.MainPage = MainPage;
            ucwizard.CurrentApp = CurrentApp;
            ucwizard.PrePageAdd = this;
            ucwizard.IsAsk = true;
            ucwizard.WizardHelper = this.WizardHelper;
            //ucwizard.ListConfigObjects = ListConfigObjects;
            MainPage.PopupPanel.Title = "Config Wizard";
            MainPage.PopupPanel.Content = ucwizard;
            MainPage.PopupPanel.IsOpen = true;
        }

        void ButnPrevious_Click(object sender, RoutedEventArgs e)
        {
            //上一页，相当于返回了
            MainPage.PopupPanel.Content = PrePage;
        }

        void UCWizardScreenChannelAdd_Loaded(object sender, RoutedEventArgs e)
        {
             this.CombBasedon.ItemsSource = mListBaseChannels;
             InitBaseChannels();
             InitControlObject();
             ChangeLanguage();
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
        }

        public WizardHelper WizardHelper { get; set; }

        private void InitBaseChannels()
        {
            try
            {
                if (ParentItem == null) { return; }
                ConfigGroup configGroup = ParentItem.Data as ConfigGroup;
                if (configGroup == null) { return; }
                ConfigObject parentObject = configGroup.ConfigObject;
                if (parentObject == null || parentObject.ObjectType != S1110Consts.RESOURCE_SCREENSERVER) { return; }
                mListBaseChannels.Clear();
                for (int i = 0; i < parentObject.ListChildObjects.Count; i++)
                {
                    ConfigObject configObject = parentObject.ListChildObjects[i];
                    if (configObject.ObjectType == S1110Consts.RESOURCE_SCREENCHANNEL)
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

        private void AddScreenChannels()
        {
            try
            {
                if (UDNumber.Value == null) { return; }
                int intCount = (int)UDNumber.Value;
                string strExtension = this.TexChanName.Text;
                int intExtenstion;
                int.TryParse(strExtension, out intExtenstion);
                var baseExtension = this.CombBasedon.SelectedItem as ConfigObject;
                int objType = S1110Consts.RESOURCE_SCREENCHANNEL;
                if (WizardHelper.ListAllTypeParams == null) { return; }
                ResourceTypeParam typeParam =
                    WizardHelper.ListAllTypeParams.FirstOrDefault(t => t.TypeID == objType);
                if (typeParam == null) { return; }
                if (WizardHelper.ListAllPropertyInfos == null) { return; }
                List<ObjectPropertyInfo> listPropertyInfos =
                    WizardHelper.ListAllPropertyInfos.Where(p => p.ObjType == objType).OrderBy(p => p.PropertyID).ToList();
                if (WizardHelper.ListAllConfigObjects == null) { return; }
                if (mParentObject == null) { return; }
                long parentObjID = mParentObject.ObjectID;
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
                    //获取当前ScreenServer下的所有通道
                    List<ConfigObject> channelsInScreen =
                        mParentObject.ListChildObjects.Where(o => o.ObjectType == objType)
                            .OrderBy(o => o.ID)
                            .ToList();
                    int id = 0;
                    for (int k = 0; k < channelsInScreen.Count; k++)
                    {
                        if (id == channelsInScreen[k].ID)
                        {
                            id++;
                        }
                    }
                    foreach (var channel in channelsInScreen)
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
                    ConfigObject configObject = ConfigObject.CreateObject(objType);
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
                                propertyValue.Value = temp.Value;
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
                            propertyValue.Value = parentObjID.ToString();
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
                    mParentObject.ListChildObjects.Add(configObject);
                    WizardHelper.ListAllConfigObjects.Add(configObject);
                    if (MainPage != null)
                    {
                        MainPage.CreateNewChannelConfigObjectItem(ParentItem, configObject);
                    }
                }

                #region 写操作日志

                var optID = string.Format("1110{0}06", objType);
                string strOptLog = string.Format("{0}:{1}", Utils.FormatOptLogString("1110202"), intCount);
                CurrentApp.WriteOperationLog(optID, ConstValue.OPT_RESULT_SUCCESS, strOptLog);

                #endregion

                if (MainPage != null)
                {
                    MainPage.CreateChildObjectList();
                }
                var parent = Parent as PopupPanel;
                if (parent != null)
                {
                    parent.IsOpen = false;
                }
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
            if (UDNumber.Value == null)
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
                    g => g.TypeID == S1110Consts.RESOURCE_SCREENSERVER && g.GroupID == 1);
            if (groupParam == null)
            {
                return false;
            }
            int intMaxinum = groupParam.IntValue02;
            if (mParentObject == null)
            {
                return false;
            }
            int existNum = mParentObject.ListChildObjects.Count(o => o.ObjectType == S1110Consts.RESOURCE_SCREENCHANNEL);
            if (intCount + existNum > intMaxinum)
            {
                ShowException(string.Format("{0}\r\n\r\n{1}\t{2}",
                   CurrentApp.GetMessageLanguageInfo("008", "Over maxinum value"),
                   CurrentApp.GetLanguageInfo(string.Format("OBJ{0}", S1110Consts.RESOURCE_SCREENCHANNEL), S1110Consts.RESOURCE_SCREENCHANNEL.ToString()),
                   intMaxinum));
                return false;
            }
            return true;
        }

        #endregion


        #region Change Language

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.Title = CurrentApp.GetLanguageInfo("1110WIZ11001", "Add Channel");
            }

            this.LabNumber.Content = CurrentApp.GetLanguageInfo("1110WIZ11002", "Count");
            this.LabChanName.Content = CurrentApp.GetLanguageInfo("1110WIZ11003", "Extension");
            this.LabBasedOn.Content = CurrentApp.GetLanguageInfo("1110WIZ11004", "Base Extension");

            this.ButnNext.Content = CurrentApp.GetLanguageInfo("1110WIZB0006", "Confirm");
            this.ButnPrevious.Content = CurrentApp.GetLanguageInfo("1110WIZB0007", "Close");
        }

        #endregion
    }
}
