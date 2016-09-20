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
using UMPS1110.Models.ConfigObjects;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Controls;

namespace UMPS1110
{
    /// <summary>
    /// UCAddPBXDevice.xaml 的交互逻辑
    /// </summary>
    public partial class UCAddPBXDevice
    {
        #region Members

        public ResourceMainView MainPage;
        public ObjectItem ParentItem;
        public List<ResourceTypeParam> ListResourceTypeParams;
        public List<ResourceGroupParam> ListResourceGroupParams;
        public List<ObjectPropertyInfo> ListPropertyInfos;
        public List<ResourceProperty> ListPropertyValues;
        public List<ConfigObject> ListConfigObjects;
        public List<BasicInfoData> ListAllCTITypeInfos;

        private ObservableCollection<PropertyValueEnumItem> mListCTITypeItems;
        private ObservableCollection<ConfigObject> mListBasicPBXDeviceItems;
        private ConfigObject mParentObject;

        #endregion
        public UCAddPBXDevice()
        {
            InitializeComponent();

            mListCTITypeItems = new ObservableCollection<PropertyValueEnumItem>();
            mListBasicPBXDeviceItems = new ObservableCollection<ConfigObject>();
            this.Loaded += UCAddPBXDevice_Loaded;
            this.BtnClose.Click += BtnClose_Click;
            this.BtnConfirm.Click += BtnConfirm_Click;
            this.RadSyn.Click += RadSyn_Click;
            this.RadBatch.Click += RadBatch_Click;
        }

        void RadBatch_Click(object sender, RoutedEventArgs e)
        {
            if (this.RadBatch.IsChecked == true)
            {
                this.TxtCount.IsEnabled = true;
                this.TxtExt.IsEnabled = true;
                this.ComboBaseExt.IsEnabled = true;
            }
            else
            {
                this.TxtCount.IsEnabled = false;
                this.TxtExt.IsEnabled = false;
                this.ComboBaseExt.IsEnabled = false;
            }
        }

        void RadSyn_Click(object sender, RoutedEventArgs e)
        {
            if (this.RadSyn.IsChecked == true)
            {
                this.TxtCount.IsEnabled = false;
                this.TxtExt.IsEnabled = false;
                this.ComboBaseExt.IsEnabled = false;
            }
            else
            {
                this.TxtCount.IsEnabled = true;
                this.TxtExt.IsEnabled = true;
                this.ComboBaseExt.IsEnabled = true;
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckValue()) { return; }
            AddPBXDevices();
        }

        #region operation
        private void AddPBXDevices()
        {
            if (this.RadBatch.IsChecked == true)//批量:记得检查已存在的PBXDevice是否通道重复
            {
                //获取选中为样本的PBXDevice
                var TempPBXDevoceObj = this.ComboBaseExt.SelectedItem;
                //if (TempPBXDevoceObj == null) { return; }
                //获取已经存在的PBXDevices
                List<ConfigObject> listExistPBxDeviceObjs =
                    ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_PBXDEVICE).ToList();
                List<PBXDeviceObject> listExistPBXDevices = new List<PBXDeviceObject>();
                for (int i = 0; i < listExistPBxDeviceObjs.Count; i++)
                {
                    var pbx = listExistPBxDeviceObjs[i] as PBXDeviceObject;
                    if (pbx != null)
                    {
                        listExistPBXDevices.Add(pbx);
                    }
                }
                int PBXDeviceNumber = int.Parse(this.TxtCount.Text);
                string PBXDeviceName = TxtExt.Text;
                var CtiTypeItem = this.ComboCTIType.SelectedItem as PropertyValueEnumItem;
                int CTIType = 0;
                if (CtiTypeItem != null)
                {
                    CTIType = int.Parse(((PropertyValueEnumItem)CtiTypeItem).Value);
                }

                List<string> mListExtsName = new List<string>();
                CreatExtNameList(PBXDeviceNumber, PBXDeviceName, out mListExtsName);
                //遍历每个通道，根据启动方式创建PBX对象
                int count = 0;
                for (int num = 0; num < PBXDeviceNumber; num++)
                {
                    bool IsFind = false;
                    for (int i = 0; i < listExistPBXDevices.Count; i++)
                    {
                        if (listExistPBXDevices[i].DeviceName == mListExtsName[num])
                        {
                            IsFind = true; break;
                        }
                    }
                    if (!IsFind)//没有找到，新建
                    {
                        //新增PBXDevice
                        ConfigObject configObject = MainPage.GetNewConfigObj(S1110Consts.RESOURCE_PBXDEVICE);
                        configObject.CurrentApp = CurrentApp;
                        var pbxDevice = configObject as PBXDeviceObject;
                        if (pbxDevice != null)
                        {
                            pbxDevice.IsEnabled = true;
                            pbxDevice.CTIType = CTIType;
                            pbxDevice.DeviceName = mListExtsName[num];
                            if (TempPBXDevoceObj != null)
                            {
                                pbxDevice.DeviceType = ((PBXDeviceObject)TempPBXDevoceObj).DeviceType;
                                pbxDevice.MonitorMode = ((PBXDeviceObject)TempPBXDevoceObj).MonitorMode;
                            }

                            pbxDevice.SetBasicPropertyValues();
                            pbxDevice.GetBasicPropertyValues();
                        }
                        mParentObject.ListChildObjects.Add(configObject);
                        ListConfigObjects.Add(configObject);
                        //创建节点
                        MainPage.CreateNewObjectItem(ParentItem, configObject, false);
                        count++;
                    }
                }
                #region 写操作日志

                var optID = string.Format("1110{0}06", S1110Consts.RESOURCE_PBXDEVICE);
                string strOptLog = string.Format("{0}:{1}", Utils.FormatOptLogString("1110202"), count);
                CurrentApp.WriteOperationLog(optID, ConstValue.OPT_RESULT_SUCCESS, strOptLog);

                #endregion

                var lister = MainPage.PanelPropertyList.Child as UCResourceObjectLister;
                if (lister != null)
                {
                    lister.ReloadData();
                }
            }
            else
            {
                MainPage.SynPBXDevice(ParentItem);
            }
            MainPage.PopupPanel.IsOpen = false;
        }

        private bool CheckValue()
        {
            //if (this.ComboCTIType.SelectedIndex == -1)
            //{
            //    return false;
            //}
            if (this.RadBatch.IsChecked == false && this.RadSyn.IsChecked == false)
            {
                return false;
            }
            if (this.RadBatch.IsChecked == true)
            {
                if (string.IsNullOrEmpty(TxtExt.Text))
                {
                    ShowException(CurrentApp.GetMessageLanguageInfo("018", "Device Name invalid"));
                    return false;
                }
                if (this.ComboBaseExt.SelectedIndex == -1 && this.TxtCount.Text != "1")
                {
                    ShowException(CurrentApp.GetMessageLanguageInfo("014100", "Basic PBXDevice invalid"));
                    return false;
                }
                //if (mListBasicPBXDeviceItems.Count == 0)
                //{
                //    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("1110N019", "请配置一个基本的PBX Device"));
                //    ClosePanel();
                //    return;
                //}
            }
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
            //PBX有上限吗？**************************
            return true;
        }

        private void CreatExtNameList(int ExtNumber, string ExtName, out List<string> ListExt)
        {
            ListExt = new List<string>(); long TempNum = -1; string PreStr = string.Empty; int LengthStr = 0;
            for (int j = 0; j < ExtName.Length; j++)
            {
                string TempStr = ExtName.Substring(j);
                if (long.TryParse(TempStr, out TempNum))
                {
                    PreStr = ExtName.Substring(0, j);
                    LengthStr = ExtName.Length - j;
                    if (LengthStr != TempNum.ToString().Length && (LengthStr - TempNum.ToString().Length) > 0)
                    {
                        for (int q = 0; q < LengthStr - TempNum.ToString().Length; q++)
                        {
                            PreStr += "0";
                        }
                    }
                    break;
                }
            }
            for (int i = 0; i < ExtNumber; i++)
            {
                if (i == 0)
                {
                    ListExt.Add(ExtName);
                }
                else
                {
                    if (TempNum != -1)
                    {
                        string StrNum = (TempNum + i).ToString();
                        ListExt.Add(PreStr + StrNum);
                    }
                    else
                    {
                        ListExt.Add(ExtName + i.ToString());
                    }
                }
            }
        }

        private void ClosePanel()
        {
            var parent = Parent as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }
        #endregion

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            ClosePanel();
        }

        void UCAddPBXDevice_Loaded(object sender, RoutedEventArgs e)
        {
            this.ComboCTIType.ItemsSource = mListCTITypeItems;
            this.ComboBaseExt.ItemsSource = mListBasicPBXDeviceItems;
            Init();
            InitCTITypeItems();
            InitBasicPBXDevice();
            ChangeLanguage();
            this.RadBatch.IsChecked = true;
        }

        #region load & init
        private void InitCTITypeItems()
        {
            try
            {
                if (ListConfigObjects != null)
                {
                    var objs =
                        ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_CTICONNECTIONGROUPCOLLECTION)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        ConfigObject obj = objs[i];
                        ResourceProperty propertyValue = obj.ListProperties.FirstOrDefault(p => p.PropertyID == 11);
                        if (propertyValue != null)
                        {
                            string ctiType = propertyValue.Value;
                            PropertyValueEnumItem item = new PropertyValueEnumItem();
                            item.Value = ctiType;
                            item.Display = obj.Name;
                            item.Description = obj.Name;
                            item.Info = obj;
                            if (ListAllCTITypeInfos != null)
                            {
                                var basicInfo =
                                    ListAllCTITypeInfos.FirstOrDefault(
                                        b => b.InfoID == 111000300 && b.Value == ctiType);
                                if (basicInfo != null)
                                {
                                    item.Display = basicInfo.Icon;
                                }
                            }
                            mListCTITypeItems.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitBasicPBXDevice()
        {
            try
            {
                if (ListConfigObjects != null)
                {
                    var objs =
                        ListConfigObjects.Where(o => o.ObjectType == S1110Consts.RESOURCE_PBXDEVICE)
                            .ToList();
                    for (int i = 0; i < objs.Count; i++)
                    {
                        mListBasicPBXDeviceItems.Add(objs[i]);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

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

        public void AddPBXDeviceList(List<ConfigObject> ListPBXD)
        {
            for (int i = 0; i < ListPBXD.Count; i++)
            {
                ListConfigObjects.Add(ListPBXD[i]);
            }
        }
        #endregion

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            MainPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("1110301", "Add PBXDevices");
            this.LabelBaseExt.Content = CurrentApp.GetLanguageInfo("1110307", "Base PBX Device:");
            this.LabelCTIType.Content = CurrentApp.GetLanguageInfo("1110302", "CTI Type:");
            this.LabelCount.Content = CurrentApp.GetLanguageInfo("1110202", "Count:");
            this.LabelExt.Content = CurrentApp.GetLanguageInfo("1110304", "Name:");
            this.LabelWay.Content = CurrentApp.GetLanguageInfo("1110303", "Add Way:");
            this.RadBatch.Content = CurrentApp.GetLanguageInfo("1110306", "Batch");
            this.RadSyn.Content = CurrentApp.GetLanguageInfo("1110305", "同步设备");

            this.BtnClose.Content = CurrentApp.GetLanguageInfo("11100", "Close");
            this.BtnConfirm.Content = CurrentApp.GetLanguageInfo("11101", "Confirm");
        }
    }
}
