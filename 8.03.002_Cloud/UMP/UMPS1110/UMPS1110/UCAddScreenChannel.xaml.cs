//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1d4ee2db-ab67-4646-bc39-52fb493bd489
//        CLR Version:              4.0.30319.18444
//        Name:                     UCAddScreenChannel
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110
//        File Name:                UCAddScreenChannel
//
//        created by Charley at 2015/3/24 16:43:59
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using UMPS1110.Models;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Controls;

namespace UMPS1110
{
    /// <summary>
    /// UCAddScreenChannel.xaml 的交互逻辑
    /// </summary>
    public partial class UCAddScreenChannel
    {
        #region Members

        public ResourceMainView MainPage;
        public ObjectItem ParentItem;
        public List<ResourceTypeParam> ListResourceTypeParams;
        public List<ResourceGroupParam> ListResourceGroupParams;
        public List<ObjectPropertyInfo> ListPropertyInfos;
        public List<ResourceProperty> ListPropertyValues;
        public List<ConfigObject> ListConfigObjects;
        public List<BasicInfoData> ListAllBasicInfos; 

        private ObservableCollection<ConfigObject> mListBaseChannels;
        private ConfigObject mParentObject;

        #endregion

        public UCAddScreenChannel()
        {
            InitializeComponent();

            mListBaseChannels = new ObservableCollection<ConfigObject>();

            Loaded += UCAddScreenChannel_Loaded;
            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
        }

        void UCAddScreenChannel_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBaseExt.ItemsSource = mListBaseChannels;

            InitBaseChannels();
            Init();

            ChangeLanguage();
        }


        #region Init and Load

        private void Init()
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

        #endregion


        #region EventHandlers

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput()) { return; }
            if (!CheckCount()) { return; }
            AddChannels();
        }

        #endregion


        #region Operations

        private void AddChannels()
        {
            try
            {
                if (TxtCount.Value == null) { return; }
                int intCount = (int)TxtCount.Value;
                string strExtension = TxtExt.Text;
                int intExtenstion;
                int.TryParse(strExtension, out intExtenstion);
                var baseExtension = ComboBaseExt.SelectedItem as ConfigObject;
                int objType = S1110Consts.RESOURCE_SCREENCHANNEL;
                if (ListResourceTypeParams == null) { return; }
                ResourceTypeParam typeParam =
                    ListResourceTypeParams.FirstOrDefault(t => t.TypeID == objType);
                if (typeParam == null) { return; }
                if (ListPropertyInfos == null) { return; }
                List<ObjectPropertyInfo> listPropertyInfos =
                    ListPropertyInfos.Where(p => p.ObjType == objType).OrderBy(p => p.PropertyID).ToList();
                if (ListConfigObjects == null) { return; }
                if (mParentObject == null) { return; }
                long parentObjID = mParentObject.ObjectID;
                //逐一添加
                for (int i = 0; i < intCount; i++)
                {
                    //获取当前所有已经存在的通道
                    List<ConfigObject> channels =
                        ListConfigObjects.Where(o => o.ObjectType == objType)
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
                    configObject.CurrentApp = CurrentApp;
                    configObject.ObjectID = objID;
                    configObject.Icon = typeParam.Icon;
                    configObject.TypeParam = typeParam;
                    configObject.ListAllObjects = ListConfigObjects;
                    configObject.ListAllTypeParams = ListResourceTypeParams;
                    configObject.ListAllBasicInfos = ListAllBasicInfos;
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
                        if (ListPropertyValues != null)
                        {
                            ListPropertyValues.Add(propertyValue);
                        }
                    }
                    configObject.Name = string.Format("[{0}] {1}", id.ToString("000"), strExt);
                    configObject.Description = string.Format("[{0}] {1}", id.ToString("000"), strExt);
                    configObject.GetBasicPropertyValues();
                    mParentObject.ListChildObjects.Add(configObject);
                    ListConfigObjects.Add(configObject);
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
            if (TxtCount.Value == null)
            {
                ShowException(CurrentApp.GetMessageLanguageInfo("016", "Count invalid"));
                return false;
            }
            intValue = (int)TxtCount.Value;
            if (intValue <= 0 || intValue > 1024)
            {
                ShowException(CurrentApp.GetMessageLanguageInfo("016", "Count invalid"));
                return false;
            }
            if (string.IsNullOrEmpty(TxtExt.Text))
            {
                ShowException(CurrentApp.GetMessageLanguageInfo("017", "Extension invalid"));
                return false;
            }
            return true;
        }

        private bool CheckCount()
        {
            if (TxtCount.Value == null)
            {
                return false;
            }
            int intCount = (int)TxtCount.Value;
            if (ListResourceGroupParams == null)
            {
                return false;
            }
            ResourceGroupParam groupParam =
                ListResourceGroupParams.FirstOrDefault(
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
                parent.Title = CurrentApp.GetLanguageInfo("1110201", "Add Channel");
            }

            LabelCount.Content = CurrentApp.GetLanguageInfo("1110202", "Count");
            LabelExt.Content = CurrentApp.GetLanguageInfo("1110203", "Extension");
            LabelBaseExt.Content = CurrentApp.GetLanguageInfo("1110204", "Base Extension");

            BtnConfirm.Content = CurrentApp.GetLanguageInfo("11101", "Confirm");
            BtnClose.Content = CurrentApp.GetLanguageInfo("11100", "Close");
        }

        #endregion

    }
}
